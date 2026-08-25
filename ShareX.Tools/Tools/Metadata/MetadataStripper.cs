#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

using System.Buffers.Binary;
using System.Text;

namespace ShareX.Tools;

internal static class MetadataStripper
{
    private static readonly byte[] ZeroBuffer = new byte[64 * 1024];

    public static void Strip(string filePath, CancellationToken cancellationToken)
    {
        MetadataFormat format = MetadataFormatDetector.Detect(filePath);
        switch (format)
        {
            case MetadataFormat.Jpeg:
                Rewrite(filePath, RewriteJpeg, cancellationToken);
                break;
            case MetadataFormat.Png:
                Rewrite(filePath, RewritePng, cancellationToken);
                break;
            case MetadataFormat.Gif:
                Rewrite(filePath, RewriteGif, cancellationToken);
                break;
            case MetadataFormat.WebP:
                StripWebP(filePath, cancellationToken);
                break;
            case MetadataFormat.Avi:
                StripAvi(filePath, cancellationToken);
                break;
            case MetadataFormat.IsoBaseMedia when IsIsoVideoPath(filePath):
                StripIsoBaseMedia(filePath, cancellationToken);
                break;
            case MetadataFormat.Matroska:
                StripMatroska(filePath, cancellationToken);
                break;
            case MetadataFormat.Asf:
                StripAsf(filePath, cancellationToken);
                break;
            default:
                throw new NotSupportedException($"Metadata stripping is not supported for {Path.GetExtension(filePath)} files.");
        }
    }

    internal static bool IsIsoVideoPath(string filePath)
    {
        return Path.GetExtension(filePath).ToLowerInvariant() is ".mp4" or ".m4v" or ".mov" or ".3gp" or ".3g2" or ".mqv";
    }

    private static bool RewriteJpeg(Stream input, Stream output, CancellationToken cancellationToken)
    {
        if (input.ReadByte() != 0xFF || input.ReadByte() != 0xD8) throw new InvalidDataException("Invalid JPEG header.");
        output.Write([0xFF, 0xD8]);
        bool changed = false;
        byte[] lengthBytes = new byte[2];
        while (TryReadJpegMarker(input, out byte marker))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (marker == 0xD9)
            {
                output.Write([0xFF, marker]);
                return changed;
            }
            if (marker is 0x01 or >= 0xD0 and <= 0xD7)
            {
                output.Write([0xFF, marker]);
                continue;
            }

            input.ReadExactly(lengthBytes);
            int length = BinaryPrimitives.ReadUInt16BigEndian(lengthBytes) - 2;
            if (length < 0 || length > input.Length - input.Position) throw new InvalidDataException("Invalid JPEG segment length.");
            bool remove = marker is 0xE1 or 0xED or 0xFE;
            if (remove)
            {
                input.Position += length;
                changed = true;
            }
            else
            {
                output.Write([0xFF, marker]);
                output.Write(lengthBytes);
                CopyBytes(input, output, length, cancellationToken);
            }
            if (marker == 0xDA)
            {
                input.CopyTo(output);
                return changed;
            }
        }
        throw new InvalidDataException("JPEG end marker was not found.");
    }

    private static bool RewritePng(Stream input, Stream output, CancellationToken cancellationToken)
    {
        Span<byte> signature = stackalloc byte[8];
        input.ReadExactly(signature);
        if (!signature.SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })) throw new InvalidDataException("Invalid PNG header.");
        output.Write(signature);
        bool changed = false;
        byte[] header = new byte[8];
        while (input.Position + 12 <= input.Length)
        {
            cancellationToken.ThrowIfCancellationRequested();
            input.ReadExactly(header);
            uint size = BinaryPrimitives.ReadUInt32BigEndian(header);
            if (size > input.Length - input.Position - 4) throw new InvalidDataException("Invalid PNG chunk length.");
            string type = Encoding.ASCII.GetString(header[4..]);
            bool remove = type is "eXIf" or "tEXt" or "zTXt" or "iTXt" or "tIME";
            if (remove)
            {
                input.Position += size + 4;
                changed = true;
            }
            else
            {
                output.Write(header);
                CopyBytes(input, output, size + 4, cancellationToken);
            }
            if (type == "IEND") return changed;
        }
        throw new InvalidDataException("PNG end chunk was not found.");
    }

    private static bool RewriteGif(Stream input, Stream output, CancellationToken cancellationToken)
    {
        Span<byte> header = stackalloc byte[13];
        input.ReadExactly(header);
        if (!header[..6].SequenceEqual("GIF87a"u8) && !header[..6].SequenceEqual("GIF89a"u8)) throw new InvalidDataException("Invalid GIF header.");
        output.Write(header);
        if ((header[10] & 0x80) != 0) CopyBytes(input, output, 3 << ((header[10] & 7) + 1), cancellationToken);
        bool changed = false;
        byte[] descriptor = new byte[9];
        while (input.Position < input.Length)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int introducer = input.ReadByte();
            if (introducer < 0) break;
            if (introducer == 0x3B)
            {
                output.WriteByte((byte)introducer);
                return changed;
            }
            if (introducer == 0x2C)
            {
                output.WriteByte((byte)introducer);
                input.ReadExactly(descriptor);
                output.Write(descriptor);
                if ((descriptor[8] & 0x80) != 0) CopyBytes(input, output, 3 << ((descriptor[8] & 7) + 1), cancellationToken);
                int minimumCodeSize = input.ReadByte();
                if (minimumCodeSize < 0) throw new EndOfStreamException();
                output.WriteByte((byte)minimumCodeSize);
                CopyGifSubBlocks(input, output, cancellationToken);
            }
            else if (introducer == 0x21)
            {
                int label = input.ReadByte();
                if (label < 0) throw new EndOfStreamException();
                if (label is 0xFE or 0x01)
                {
                    SkipGifExtension(input);
                    changed = true;
                }
                else if (label == 0xFF)
                {
                    int size = input.ReadByte();
                    if (size < 0) throw new EndOfStreamException();
                    byte[] identifier = new byte[size];
                    input.ReadExactly(identifier);
                    bool remove = Encoding.ASCII.GetString(identifier).StartsWith("XMP DataXMP", StringComparison.Ordinal);
                    if (remove)
                    {
                        SkipGifSubBlocks(input);
                        changed = true;
                    }
                    else
                    {
                        output.WriteByte(0x21);
                        output.WriteByte((byte)label);
                        output.WriteByte((byte)size);
                        output.Write(identifier);
                        CopyGifSubBlocks(input, output, cancellationToken);
                    }
                }
                else
                {
                    output.WriteByte(0x21);
                    output.WriteByte((byte)label);
                    CopyGifExtension(input, output, cancellationToken);
                }
            }
            else
            {
                throw new InvalidDataException("Invalid GIF block.");
            }
        }
        throw new InvalidDataException("GIF trailer was not found.");
    }

    private static void StripWebP(string filePath, CancellationToken cancellationToken)
    {
        using FileStream stream = new(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        Span<byte> riff = stackalloc byte[12];
        stream.ReadExactly(riff);
        List<OverwriteRange> ranges = [];
        long? featureFlagsOffset = null;
        long position = 12;
        byte[] header = new byte[8];
        while (position + 8 <= stream.Length)
        {
            cancellationToken.ThrowIfCancellationRequested();
            stream.Position = position;
            stream.ReadExactly(header);
            string type = Encoding.ASCII.GetString(header.AsSpan(0, 4));
            uint size = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4));
            long end = position + 8 + size + (size & 1);
            if (end > stream.Length) throw new InvalidDataException("Invalid WebP chunk length.");
            if (type is "EXIF" or "XMP ") ranges.Add(new OverwriteRange(position, 8 + size + (size & 1), true));
            else if (type == "VP8X" && size >= 1) featureFlagsOffset = position + 8;
            position = end;
        }
        foreach (OverwriteRange range in ranges) OverwriteRiffChunkAsJunk(stream, range, cancellationToken);
        if (featureFlagsOffset.HasValue && ranges.Count > 0)
        {
            stream.Position = featureFlagsOffset.Value;
            int flags = stream.ReadByte();
            stream.Position = featureFlagsOffset.Value;
            stream.WriteByte((byte)(flags & ~0x0C));
        }
        stream.Flush(true);
    }

    private static void StripAvi(string filePath, CancellationToken cancellationToken)
    {
        using FileStream stream = new(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        List<OverwriteRange> ranges = [];
        CollectAviInfoRanges(stream, 12, stream.Length, ranges, cancellationToken);
        foreach (OverwriteRange range in ranges) OverwriteRiffChunkAsJunk(stream, range, cancellationToken);
        stream.Flush(true);
    }

    private static void CollectAviInfoRanges(FileStream stream, long start, long end, List<OverwriteRange> ranges, CancellationToken cancellationToken)
    {
        long position = start;
        byte[] header = new byte[8];
        byte[] listType = new byte[4];
        while (position + 8 <= end)
        {
            cancellationToken.ThrowIfCancellationRequested();
            stream.Position = position;
            stream.ReadExactly(header);
            string type = Encoding.ASCII.GetString(header.AsSpan(0, 4));
            uint size = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4));
            long chunkEnd = position + 8 + size;
            long next = chunkEnd + (size & 1);
            if (chunkEnd > end) throw new InvalidDataException("Invalid AVI chunk length.");
            if (type == "LIST" && size >= 4)
            {
                stream.ReadExactly(listType);
                if (listType.SequenceEqual("INFO"u8)) ranges.Add(new OverwriteRange(position, 8 + size + (size & 1), true));
                else if (listType.SequenceEqual("hdrl"u8) || listType.SequenceEqual("odml"u8))
                    CollectAviInfoRanges(stream, position + 12, chunkEnd, ranges, cancellationToken);
            }
            position = next;
        }
    }

    private static void StripIsoBaseMedia(string filePath, CancellationToken cancellationToken)
    {
        using FileStream stream = new(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        List<IsoRange> ranges = [];
        long position = 0;
        while (position + 8 <= stream.Length)
        {
            if (!TryReadIsoBox(stream, position, stream.Length, out IsoRange box)) break;
            if (box.Type == "moov") CollectIsoMetadataRanges(stream, box.PayloadOffset, box.End, "moov", ranges, cancellationToken);
            position = box.End;
        }
        foreach (IsoRange range in ranges)
        {
            cancellationToken.ThrowIfCancellationRequested();
            stream.Position = range.Offset + 4;
            stream.Write("free"u8);
            stream.Position = range.PayloadOffset;
            WriteZeros(stream, range.End - range.PayloadOffset, cancellationToken);
        }
        stream.Flush(true);
    }

    private static void CollectIsoMetadataRanges(
        FileStream stream,
        long start,
        long end,
        string parentType,
        List<IsoRange> ranges,
        CancellationToken cancellationToken)
    {
        long position = start;
        while (position + 8 <= end)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!TryReadIsoBox(stream, position, end, out IsoRange box)) break;
            if (box.Type == "udta" || box.Type == "meta" && parentType is "moov" or "trak")
            {
                ranges.Add(box);
            }
            else if (box.Type is "moov" or "trak")
            {
                CollectIsoMetadataRanges(stream, box.PayloadOffset, box.End, box.Type, ranges, cancellationToken);
            }
            position = box.End;
        }
    }

    private static void StripMatroska(string filePath, CancellationToken cancellationToken)
    {
        using FileStream stream = new(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        if (!MatroskaMetadataReader.TryReadElementHeader(stream, stream.Length, out MatroskaMetadataReader.StreamElement ebml)) return;
        MatroskaMetadataReader.StreamElement segment = default;
        long segmentPosition = ebml.End;
        while (segmentPosition < stream.Length)
        {
            stream.Position = segmentPosition;
            if (!MatroskaMetadataReader.TryReadElementHeader(stream, stream.Length, out MatroskaMetadataReader.StreamElement candidate)) return;
            if (candidate.Id == 0x18538067)
            {
                segment = candidate;
                break;
            }
            if (candidate.UnknownSize) return;
            segmentPosition = candidate.End;
        }
        if (segment.Id != 0x18538067) return;

        long segmentEnd = segment.UnknownSize ? stream.Length : segment.End;
        List<MatroskaMetadataReader.StreamElement> ranges = [];
        long position = segment.PayloadOffset;
        while (position < segmentEnd)
        {
            cancellationToken.ThrowIfCancellationRequested();
            stream.Position = position;
            if (!MatroskaMetadataReader.TryReadElementHeader(stream, segmentEnd, out MatroskaMetadataReader.StreamElement element)) break;
            if (element.Id == 0x1254C367) ranges.Add(element);
            else if (!element.UnknownSize && element.Id == 0x1549A966) CollectMatroskaLeafRanges(stream, element, [0x7BA9, 0x4461, 0x4D80, 0x5741, 0x7384, 0x3C83AB, 0x3E83BB], ranges);
            else if (!element.UnknownSize && element.Id == 0x1654AE6B) CollectMatroskaTrackNames(stream, element, ranges);
            if (element.UnknownSize) break;
            position = element.End;
        }
        foreach (MatroskaMetadataReader.StreamElement range in ranges) OverwriteAsEbmlVoid(stream, range, cancellationToken);
        stream.Flush(true);
    }

    private static void CollectMatroskaLeafRanges(
        FileStream stream,
        MatroskaMetadataReader.StreamElement parent,
        ulong[] targetIds,
        List<MatroskaMetadataReader.StreamElement> ranges)
    {
        long position = parent.PayloadOffset;
        while (position < parent.End)
        {
            stream.Position = position;
            if (!MatroskaMetadataReader.TryReadElementHeader(stream, parent.End, out MatroskaMetadataReader.StreamElement child)) break;
            if (targetIds.Contains(child.Id)) ranges.Add(child);
            if (child.UnknownSize) break;
            position = child.End;
        }
    }

    private static void CollectMatroskaTrackNames(
        FileStream stream,
        MatroskaMetadataReader.StreamElement tracks,
        List<MatroskaMetadataReader.StreamElement> ranges)
    {
        long position = tracks.PayloadOffset;
        while (position < tracks.End)
        {
            stream.Position = position;
            if (!MatroskaMetadataReader.TryReadElementHeader(stream, tracks.End, out MatroskaMetadataReader.StreamElement track)) break;
            if (track.Id == 0xAE && !track.UnknownSize) CollectMatroskaLeafRanges(stream, track, [0x536E], ranges);
            if (track.UnknownSize) break;
            position = track.End;
        }
    }

    private static void StripAsf(string filePath, CancellationToken cancellationToken)
    {
        using FileStream stream = new(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        Span<byte> header = stackalloc byte[30];
        stream.ReadExactly(header);
        if (new Guid(header[..16]) != AsfMetadataReader.HeaderObject) return;
        long headerEnd = (long)Math.Min(BinaryPrimitives.ReadUInt64LittleEndian(header[16..]), (ulong)stream.Length);
        uint count = BinaryPrimitives.ReadUInt32LittleEndian(header[24..]);
        List<AsfRange> ranges = [];
        long position = 30;
        byte[] objectHeader = new byte[24];
        for (uint index = 0; index < count && position + 24 <= headerEnd; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            stream.Position = position;
            stream.ReadExactly(objectHeader);
            Guid id = new(objectHeader.AsSpan(0, 16));
            ulong size = BinaryPrimitives.ReadUInt64LittleEndian(objectHeader.AsSpan(16));
            if (size < 24 || size > long.MaxValue || position + (long)size > headerEnd) break;
            if (id == AsfMetadataReader.ContentDescriptionObject || id == AsfMetadataReader.ExtendedContentDescriptionObject)
                ranges.Add(new AsfRange(position, (long)size));
            else if (id == AsfMetadataReader.HeaderExtensionObject)
                CollectAsfExtensionMetadata(stream, position + 24, (long)size - 24, ranges);
            position += (long)size;
        }
        foreach (AsfRange range in ranges)
        {
            stream.Position = range.Offset;
            stream.Write(AsfMetadataReader.PaddingObject.ToByteArray());
            stream.Position = range.Offset + 24;
            WriteZeros(stream, range.Size - 24, cancellationToken);
        }
        stream.Flush(true);
    }

    private static void CollectAsfExtensionMetadata(FileStream stream, long offset, long payloadSize, List<AsfRange> ranges)
    {
        if (payloadSize < 22) return;
        stream.Position = offset + 18;
        Span<byte> sizeBytes = stackalloc byte[4];
        stream.ReadExactly(sizeBytes);
        uint extensionSize = BinaryPrimitives.ReadUInt32LittleEndian(sizeBytes);
        long position = offset + 22;
        long end = Math.Min(position + extensionSize, offset + payloadSize);
        byte[] header = new byte[24];
        while (position + 24 <= end)
        {
            stream.Position = position;
            stream.ReadExactly(header);
            Guid id = new(header.AsSpan(0, 16));
            ulong size = BinaryPrimitives.ReadUInt64LittleEndian(header.AsSpan(16));
            if (size < 24 || size > long.MaxValue || position + (long)size > end) break;
            if (id == AsfMetadataReader.MetadataObject || id == AsfMetadataReader.MetadataLibraryObject)
                ranges.Add(new AsfRange(position, (long)size));
            position += (long)size;
        }
    }

    private static void Rewrite(
        string filePath,
        Func<Stream, Stream, CancellationToken, bool> rewrite,
        CancellationToken cancellationToken)
    {
        string directory = Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("The file has no parent directory.");
        string temporaryPath = Path.Combine(directory, $".{Path.GetFileName(filePath)}.{Guid.NewGuid():N}.tmp");
        DateTime creationTime = File.GetCreationTimeUtc(filePath);
        FileAttributes attributes = File.GetAttributes(filePath);
        try
        {
            bool changed;
            using (FileStream input = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream output = new(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 64 * 1024, FileOptions.WriteThrough))
            {
                changed = rewrite(input, output, cancellationToken);
                output.Flush(true);
            }
            if (!changed)
            {
                File.Delete(temporaryPath);
                return;
            }
            try
            {
                File.Replace(temporaryPath, filePath, null, true);
            }
            catch (IOException)
            {
                File.Copy(temporaryPath, filePath, true);
                File.Delete(temporaryPath);
            }
            File.SetCreationTimeUtc(filePath, creationTime);
            File.SetAttributes(filePath, attributes);
        }
        catch
        {
            if (File.Exists(temporaryPath)) File.Delete(temporaryPath);
            throw;
        }
    }

    private static void OverwriteRiffChunkAsJunk(FileStream stream, OverwriteRange range, CancellationToken cancellationToken)
    {
        stream.Position = range.Offset;
        stream.Write("JUNK"u8);
        stream.Position = range.Offset + 8;
        WriteZeros(stream, range.TotalSize - 8, cancellationToken);
    }

    private static void OverwriteAsEbmlVoid(
        FileStream stream,
        MatroskaMetadataReader.StreamElement element,
        CancellationToken cancellationToken)
    {
        long totalSize = element.TotalSize;
        int sizeWidth = 1;
        long payloadSize;
        while (true)
        {
            payloadSize = totalSize - 1 - sizeWidth;
            ulong maximum = sizeWidth == 8 ? (1UL << 56) - 2 : (1UL << (sizeWidth * 7)) - 2;
            if (payloadSize >= 0 && (ulong)payloadSize <= maximum) break;
            if (++sizeWidth > 8) throw new InvalidDataException("Matroska element is too large to replace with Void.");
        }
        Span<byte> header = stackalloc byte[9];
        header[0] = 0xEC;
        ulong encodedSize = (ulong)payloadSize | (1UL << (sizeWidth * 7));
        for (int index = sizeWidth; index > 0; index--)
        {
            header[index] = (byte)encodedSize;
            encodedSize >>= 8;
        }
        stream.Position = element.Offset;
        stream.Write(header[..(sizeWidth + 1)]);
        WriteZeros(stream, payloadSize, cancellationToken);
    }

    private static bool TryReadIsoBox(FileStream stream, long offset, long parentEnd, out IsoRange box)
    {
        box = default;
        stream.Position = offset;
        Span<byte> header = stackalloc byte[16];
        if (stream.Read(header[..8]) != 8) return false;
        uint size32 = BinaryPrimitives.ReadUInt32BigEndian(header);
        string type = Encoding.Latin1.GetString(header[4..8]);
        int headerSize = 8;
        long size = size32;
        if (size32 == 1)
        {
            if (stream.Read(header[8..16]) != 8) return false;
            ulong extended = BinaryPrimitives.ReadUInt64BigEndian(header[8..]);
            if (extended > long.MaxValue) return false;
            size = (long)extended;
            headerSize = 16;
        }
        else if (size32 == 0) size = parentEnd - offset;
        if (size < headerSize || offset + size > parentEnd) return false;
        box = new IsoRange(type, offset, size, headerSize);
        return true;
    }

    private static bool TryReadJpegMarker(Stream stream, out byte marker)
    {
        marker = 0;
        int value;
        do { value = stream.ReadByte(); if (value < 0) return false; } while (value != 0xFF);
        do { value = stream.ReadByte(); if (value < 0) return false; } while (value == 0xFF);
        marker = (byte)value;
        return marker != 0;
    }

    private static void SkipGifExtension(Stream input)
    {
        int firstBlockSize = input.ReadByte();
        if (firstBlockSize < 0) throw new EndOfStreamException();
        input.Position += firstBlockSize;
        SkipGifSubBlocks(input);
    }

    private static void CopyGifExtension(Stream input, Stream output, CancellationToken cancellationToken)
    {
        int firstBlockSize = input.ReadByte();
        if (firstBlockSize < 0) throw new EndOfStreamException();
        output.WriteByte((byte)firstBlockSize);
        CopyBytes(input, output, firstBlockSize, cancellationToken);
        CopyGifSubBlocks(input, output, cancellationToken);
    }

    private static void SkipGifSubBlocks(Stream input)
    {
        while (true)
        {
            int size = input.ReadByte();
            if (size < 0) throw new EndOfStreamException();
            if (size == 0) return;
            input.Position += size;
        }
    }

    private static void CopyGifSubBlocks(Stream input, Stream output, CancellationToken cancellationToken)
    {
        while (true)
        {
            int size = input.ReadByte();
            if (size < 0) throw new EndOfStreamException();
            output.WriteByte((byte)size);
            if (size == 0) return;
            CopyBytes(input, output, size, cancellationToken);
        }
    }

    private static void CopyBytes(Stream input, Stream output, long count, CancellationToken cancellationToken)
    {
        byte[] buffer = new byte[64 * 1024];
        while (count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int request = (int)Math.Min(buffer.Length, count);
            int read = input.Read(buffer, 0, request);
            if (read == 0) throw new EndOfStreamException();
            output.Write(buffer, 0, read);
            count -= read;
        }
    }

    private static void WriteZeros(Stream stream, long count, CancellationToken cancellationToken)
    {
        while (count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int write = (int)Math.Min(ZeroBuffer.Length, count);
            stream.Write(ZeroBuffer, 0, write);
            count -= write;
        }
    }

    private readonly record struct OverwriteRange(long Offset, long TotalSize, bool Riff);
    private readonly record struct AsfRange(long Offset, long Size);
    private readonly record struct IsoRange(string Type, long Offset, long Size, int HeaderSize)
    {
        public long PayloadOffset => Offset + HeaderSize;
        public long End => Offset + Size;
    }
}
