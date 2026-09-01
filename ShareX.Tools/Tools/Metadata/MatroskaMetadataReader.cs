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

internal static class MatroskaMetadataReader
{
    private const int MaximumElementReadSize = 128 * 1024 * 1024;
    private static readonly DateTime MatroskaEpoch = new(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void Read(string filePath, MetadataCollector metadata, CancellationToken cancellationToken)
    {
        using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        if (!TryReadElementHeader(stream, stream.Length, out StreamElement ebml) || ebml.Id != 0x1A45DFA3) return;
        if (ebml.Size <= MaximumElementReadSize)
        {
            byte[] header = ReadElement(stream, ebml);
            ReadEbmlHeader(header, metadata);
        }

        StreamElement segment = default;
        long position = ebml.End;
        while (position < stream.Length)
        {
            stream.Position = position;
            if (!TryReadElementHeader(stream, stream.Length, out StreamElement candidate)) return;
            if (candidate.Id == 0x18538067)
            {
                segment = candidate;
                break;
            }
            if (candidate.UnknownSize) return;
            position = candidate.End;
        }
        if (segment.Id != 0x18538067) return;

        long segmentEnd = segment.UnknownSize ? stream.Length : segment.End;
        stream.Position = segment.PayloadOffset;
        while (stream.Position < segmentEnd)
        {
            cancellationToken.ThrowIfCancellationRequested();
            long start = stream.Position;
            if (!TryReadElementHeader(stream, segmentEnd, out StreamElement element)) break;
            if (!element.UnknownSize && element.Size <= MaximumElementReadSize && element.Id is 0x1549A966 or 0x1654AE6B or 0x1254C367)
            {
                byte[] data = ReadElement(stream, element);
                if (element.Id == 0x1549A966) ReadInfo(data, metadata);
                else if (element.Id == 0x1654AE6B) ReadTracks(data, metadata);
                else ReadTags(data, metadata);
            }
            if (element.UnknownSize) break;
            stream.Position = element.End;
            if (stream.Position <= start) break;
        }
    }

    private static void ReadEbmlHeader(byte[] data, MetadataCollector metadata)
    {
        foreach (MemoryElement element in GetElements(data))
        {
            ReadOnlySpan<byte> value = element.Payload(data);
            switch (element.Id)
            {
                case 0x4282: metadata.Add("EBML", "Document Type", ReadString(value)); break;
                case 0x4287: metadata.Add("EBML", "Document Type Version", ReadUnsigned(value)); break;
                case 0x4285: metadata.Add("EBML", "Document Type Read Version", ReadUnsigned(value)); break;
                case 0x42F7: metadata.Add("EBML", "EBML Read Version", ReadUnsigned(value)); break;
                case 0x42F2: metadata.Add("EBML", "Maximum ID Length", ReadUnsigned(value)); break;
                case 0x42F3: metadata.Add("EBML", "Maximum Size Length", ReadUnsigned(value)); break;
            }
        }
    }

    private static void ReadInfo(byte[] data, MetadataCollector metadata)
    {
        ulong timestampScale = 1_000_000;
        double? duration = null;
        foreach (MemoryElement element in GetElements(data))
        {
            ReadOnlySpan<byte> value = element.Payload(data);
            switch (element.Id)
            {
                case 0x2AD7B1:
                    timestampScale = ReadUnsigned(value);
                    metadata.Add("Matroska Info", "Timestamp Scale", $"{timestampScale:N0} ns");
                    break;
                case 0x4489:
                    duration = ReadFloat(value);
                    break;
                case 0x4461:
                    if (value.Length == 8)
                    {
                        try { metadata.Add("Matroska Info", "Date UTC", MatroskaEpoch.AddTicks(ReadSigned(value) / 100)); }
                        catch (ArgumentOutOfRangeException) { }
                    }
                    break;
                case 0x7BA9: metadata.Add("Matroska Info", "Title", ReadString(value)); break;
                case 0x4D80: metadata.Add("Matroska Info", "Muxing Application", ReadString(value)); break;
                case 0x5741: metadata.Add("Matroska Info", "Writing Application", ReadString(value)); break;
                case 0x7384: metadata.Add("Matroska Info", "Segment Filename", ReadString(value)); break;
                case 0x73A4: metadata.Add("Matroska Info", "Segment UID", Convert.ToHexString(value)); break;
            }
        }
        if (duration is > 0) metadata.Add("Matroska Info", "Duration", FormatDuration(duration.Value * timestampScale / 1_000_000_000d));
    }

    private static void ReadTracks(byte[] data, MetadataCollector metadata)
    {
        int index = 0;
        foreach (MemoryElement element in GetElements(data).Where(x => x.Id == 0xAE))
        {
            ReadTrack(element.Payload(data).ToArray(), metadata, ++index);
        }
    }

    private static void ReadTrack(byte[] data, MetadataCollector metadata, int index)
    {
        string group = $"Track {index}";
        byte[]? video = null;
        byte[]? audio = null;
        foreach (MemoryElement element in GetElements(data))
        {
            ReadOnlySpan<byte> value = element.Payload(data);
            switch (element.Id)
            {
                case 0xD7: metadata.Add(group, "Track Number", ReadUnsigned(value)); break;
                case 0x73C5: metadata.Add(group, "Track UID", ReadUnsigned(value)); break;
                case 0x83: metadata.Add(group, "Track Type", GetTrackType(ReadUnsigned(value))); break;
                case 0xB9: metadata.Add(group, "Enabled", ReadUnsigned(value) != 0 ? "Yes" : "No"); break;
                case 0x88: metadata.Add(group, "Default", ReadUnsigned(value) != 0 ? "Yes" : "No"); break;
                case 0x55AA: metadata.Add(group, "Forced", ReadUnsigned(value) != 0 ? "Yes" : "No"); break;
                case 0x9C: metadata.Add(group, "Lacing", ReadUnsigned(value) != 0 ? "Yes" : "No"); break;
                case 0x23E383: metadata.Add(group, "Default Frame Duration", $"{ReadUnsigned(value) / 1_000_000d:0.###} ms"); break;
                case 0x536E: metadata.Add(group, "Name", ReadString(value)); break;
                case 0x22B59C: metadata.Add(group, "Language", ReadString(value)); break;
                case 0x22B59D: metadata.Add(group, "Language (BCP 47)", ReadString(value)); break;
                case 0x86: metadata.Add(group, "Codec ID", ReadString(value)); break;
                case 0x258688: metadata.Add(group, "Codec Name", ReadString(value)); break;
                case 0x56AA: metadata.Add(group, "Codec Delay", $"{ReadUnsigned(value) / 1_000_000d:0.###} ms"); break;
                case 0x56BB: metadata.Add(group, "Seek Pre-roll", $"{ReadUnsigned(value) / 1_000_000d:0.###} ms"); break;
                case 0xE0: video = value.ToArray(); break;
                case 0xE1: audio = value.ToArray(); break;
            }
        }
        if (video != null) ReadVideoTrack(video, metadata, group);
        if (audio != null) ReadAudioTrack(audio, metadata, group);
    }

    private static void ReadVideoTrack(byte[] data, MetadataCollector metadata, string group)
    {
        int width = 0;
        int height = 0;
        foreach (MemoryElement element in GetElements(data))
        {
            ReadOnlySpan<byte> value = element.Payload(data);
            switch (element.Id)
            {
                case 0xB0: width = (int)Math.Min(ReadUnsigned(value), int.MaxValue); break;
                case 0xBA: height = (int)Math.Min(ReadUnsigned(value), int.MaxValue); break;
                case 0x54B0: metadata.Add(group, "Display Width", ReadUnsigned(value)); break;
                case 0x54BA: metadata.Add(group, "Display Height", ReadUnsigned(value)); break;
                case 0x9A: metadata.Add(group, "Interlaced", ReadUnsigned(value) switch { 1 => "Yes", 2 => "No", _ => "Unknown" }); break;
                case 0x53B8: metadata.Add(group, "Stereo Mode", ReadUnsigned(value)); break;
                case 0x53C0: metadata.Add(group, "Alpha Mode", ReadUnsigned(value)); break;
            }
        }
        metadata.AddSize(group, width, height);
    }

    private static void ReadAudioTrack(byte[] data, MetadataCollector metadata, string group)
    {
        foreach (MemoryElement element in GetElements(data))
        {
            ReadOnlySpan<byte> value = element.Payload(data);
            switch (element.Id)
            {
                case 0xB5: metadata.Add(group, "Sampling Frequency", $"{ReadFloat(value):0.###} Hz"); break;
                case 0x78B5: metadata.Add(group, "Output Sampling Frequency", $"{ReadFloat(value):0.###} Hz"); break;
                case 0x9F: metadata.Add(group, "Channels", ReadUnsigned(value)); break;
                case 0x6264: metadata.Add(group, "Bit Depth", ReadUnsigned(value)); break;
            }
        }
    }

    private static void ReadTags(byte[] data, MetadataCollector metadata)
    {
        int tagIndex = 0;
        foreach (MemoryElement tag in GetElements(data).Where(x => x.Id == 0x7373))
        {
            byte[] tagData = tag.Payload(data).ToArray();
            foreach (MemoryElement child in GetElements(tagData).Where(x => x.Id == 0x67C8))
            {
                ReadSimpleTag(child.Payload(tagData).ToArray(), metadata, $"Tag {++tagIndex}");
            }
        }
    }

    private static void ReadSimpleTag(byte[] data, MetadataCollector metadata, string fallbackName)
    {
        string name = fallbackName;
        string? value = null;
        List<byte[]> nested = [];
        foreach (MemoryElement element in GetElements(data))
        {
            ReadOnlySpan<byte> payload = element.Payload(data);
            if (element.Id == 0x45A3) name = ReadString(payload);
            else if (element.Id == 0x4487) value = ReadString(payload);
            else if (element.Id == 0x4485) value = $"{payload.Length:N0} bytes";
            else if (element.Id == 0x67C8) nested.Add(payload.ToArray());
        }
        if (value != null) metadata.Add("Matroska Tags", name, value);
        foreach (byte[] child in nested) ReadSimpleTag(child, metadata, name);
    }

    internal static List<MemoryElement> GetElements(byte[] data)
    {
        List<MemoryElement> elements = [];
        int position = 0;
        while (position < data.Length && elements.Count < 100_000)
        {
            if (!TryReadElement(data, position, out MemoryElement element)) break;
            elements.Add(element);
            if (element.UnknownSize) break;
            position = element.End;
        }
        return elements;
    }

    internal static bool TryReadElementHeader(Stream stream, long parentEnd, out StreamElement element)
    {
        element = default;
        long offset = stream.Position;
        if (!TryReadVInt(stream, true, out ulong id, out int idWidth, out _)) return false;
        if (!TryReadVInt(stream, false, out ulong size, out int sizeWidth, out bool unknown)) return false;
        long payloadOffset = stream.Position;
        if (!unknown && (size > long.MaxValue || payloadOffset + (long)size > parentEnd)) return false;
        element = new StreamElement(id, offset, payloadOffset, unknown ? 0 : (long)size, idWidth, sizeWidth, unknown);
        return true;
    }

    private static bool TryReadElement(byte[] data, int offset, out MemoryElement element)
    {
        element = default;
        int position = offset;
        if (!TryReadVInt(data, ref position, true, out ulong id, out int idWidth, out _)) return false;
        if (!TryReadVInt(data, ref position, false, out ulong size, out int sizeWidth, out bool unknown)) return false;
        if (unknown || size > int.MaxValue || position + (long)size > data.Length)
        {
            if (!unknown) return false;
            size = (ulong)(data.Length - position);
        }
        element = new MemoryElement(id, offset, position, (int)size, idWidth, sizeWidth, unknown);
        return true;
    }

    private static bool TryReadVInt(Stream stream, bool keepMarker, out ulong value, out int width, out bool unknown)
    {
        value = 0;
        width = 0;
        unknown = false;
        int first = stream.ReadByte();
        if (first <= 0) return false;
        width = GetVIntWidth((byte)first);
        if (width == 0 || width > (keepMarker ? 4 : 8)) return false;
        value = keepMarker ? (byte)first : (ulong)(first & (0xFF >> width));
        bool allOnes = !keepMarker && (first & (0xFF >> width)) == (0xFF >> width);
        for (int index = 1; index < width; index++)
        {
            int next = stream.ReadByte();
            if (next < 0) return false;
            value = (value << 8) | (byte)next;
            allOnes &= next == 0xFF;
        }
        unknown = allOnes;
        return true;
    }

    private static bool TryReadVInt(byte[] data, ref int position, bool keepMarker, out ulong value, out int width, out bool unknown)
    {
        value = 0;
        width = 0;
        unknown = false;
        if (position >= data.Length || data[position] == 0) return false;
        byte first = data[position++];
        width = GetVIntWidth(first);
        if (width == 0 || width > (keepMarker ? 4 : 8) || position + width - 1 > data.Length) return false;
        value = keepMarker ? first : (ulong)(first & (0xFF >> width));
        bool allOnes = !keepMarker && (first & (0xFF >> width)) == (0xFF >> width);
        for (int index = 1; index < width; index++)
        {
            byte next = data[position++];
            value = (value << 8) | next;
            allOnes &= next == 0xFF;
        }
        unknown = allOnes;
        return true;
    }

    private static int GetVIntWidth(byte first)
    {
        int width = 1;
        byte mask = 0x80;
        while (width <= 8 && (first & mask) == 0)
        {
            width++;
            mask >>= 1;
        }
        return width <= 8 ? width : 0;
    }

    private static byte[] ReadElement(FileStream stream, StreamElement element)
    {
        stream.Position = element.PayloadOffset;
        byte[] data = new byte[(int)element.Size];
        stream.ReadExactly(data);
        return data;
    }

    private static ulong ReadUnsigned(ReadOnlySpan<byte> value)
    {
        ulong result = 0;
        foreach (byte item in value[..Math.Min(value.Length, 8)]) result = (result << 8) | item;
        return result;
    }

    private static long ReadSigned(ReadOnlySpan<byte> value)
    {
        if (value.IsEmpty || value.Length > 8) return 0;
        ulong unsigned = ReadUnsigned(value);
        int bits = value.Length * 8;
        if ((value[0] & 0x80) != 0 && bits < 64) unsigned |= ulong.MaxValue << bits;
        return unchecked((long)unsigned);
    }

    private static double ReadFloat(ReadOnlySpan<byte> value) => value.Length switch
    {
        4 => BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(value)),
        8 => BitConverter.Int64BitsToDouble(BinaryPrimitives.ReadInt64BigEndian(value)),
        _ => 0
    };

    private static string ReadString(ReadOnlySpan<byte> value)
    {
        try { return new UTF8Encoding(false, true).GetString(value).TrimEnd('\0'); }
        catch (DecoderFallbackException) { return Encoding.Latin1.GetString(value).TrimEnd('\0'); }
    }

    private static string GetTrackType(ulong value) => value switch
    {
        1 => "Video",
        2 => "Audio",
        3 => "Complex",
        0x10 => "Logo",
        0x11 => "Subtitle",
        0x12 => "Buttons",
        0x20 => "Control",
        0x21 => "Metadata",
        _ => value.ToString()
    };
    private static string FormatDuration(double seconds) => TimeSpan.FromSeconds(seconds).ToString(seconds >= 3600 ? @"h\:mm\:ss\.fff" : @"m\:ss\.fff");

    internal readonly record struct StreamElement(ulong Id, long Offset, long PayloadOffset, long Size, int IdWidth, int SizeWidth, bool UnknownSize)
    {
        public long End => PayloadOffset + Size;
        public long TotalSize => End - Offset;
    }

    internal readonly record struct MemoryElement(ulong Id, int Offset, int PayloadOffset, int Size, int IdWidth, int SizeWidth, bool UnknownSize)
    {
        public int End => PayloadOffset + Size;
        public ReadOnlySpan<byte> Payload(byte[] data) => data.AsSpan(PayloadOffset, Size);
    }
}
