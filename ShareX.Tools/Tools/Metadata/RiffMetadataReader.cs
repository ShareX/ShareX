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

internal static class RiffMetadataReader
{
    private const int MaximumChunkReadSize = 32 * 1024 * 1024;

    public static void ReadWebP(string filePath, MetadataCollector metadata, CancellationToken cancellationToken)
    {
        using FileStream stream = OpenRead(filePath);
        stream.Position = 12;
        int frameCount = 0;
        byte[] header = new byte[8];
        while (stream.Position + 8 <= stream.Length)
        {
            cancellationToken.ThrowIfCancellationRequested();
            stream.ReadExactly(header);
            string type = FourCc(header);
            uint size = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4));
            long next = stream.Position + size + (size & 1);
            if (next > stream.Length) break;
            byte[] data = size <= MaximumChunkReadSize ? new byte[size] : [];
            if (data.Length > 0) stream.ReadExactly(data);

            ReadOnlySpan<byte> span = data;
            switch (type)
            {
                case "VP8X" when span.Length >= 10:
                    metadata.AddSize("WebP", 1 + ReadUInt24LittleEndian(span[4..]), 1 + ReadUInt24LittleEndian(span[7..]));
                    List<string> features = [];
                    if ((span[0] & 0x20) != 0) features.Add("ICC profile");
                    if ((span[0] & 0x10) != 0) features.Add("Alpha");
                    if ((span[0] & 0x08) != 0) features.Add("EXIF");
                    if ((span[0] & 0x04) != 0) features.Add("XMP");
                    if ((span[0] & 0x02) != 0) features.Add("Animation");
                    metadata.Add("WebP", "Features", string.Join(", ", features));
                    break;
                case "VP8 " when span.Length >= 10 && span.Slice(3, 3).SequenceEqual(new byte[] { 0x9D, 0x01, 0x2A }):
                    metadata.AddSize("WebP", BinaryPrimitives.ReadUInt16LittleEndian(span[6..]) & 0x3FFF, BinaryPrimitives.ReadUInt16LittleEndian(span[8..]) & 0x3FFF);
                    metadata.Add("WebP", "Compression", "Lossy VP8");
                    break;
                case "VP8L" when span.Length >= 5 && span[0] == 0x2F:
                    int width = 1 + span[1] + ((span[2] & 0x3F) << 8);
                    int height = 1 + ((span[2] >> 6) | (span[3] << 2) | ((span[4] & 0x0F) << 10));
                    metadata.AddSize("WebP", width, height);
                    metadata.Add("WebP", "Compression", "Lossless VP8L");
                    break;
                case "ANIM" when span.Length >= 6:
                    ushort loops = BinaryPrimitives.ReadUInt16LittleEndian(span[4..]);
                    metadata.Add("WebP", "Animation Loop Count", loops == 0 ? "Infinite" : loops);
                    break;
                case "ANMF":
                    frameCount++;
                    break;
                case "EXIF":
                    byte[] exif = span.StartsWith("Exif\0\0"u8) ? span[6..].ToArray() : data;
                    TiffMetadataReader.Read(exif, metadata, "EXIF");
                    break;
                case "XMP ":
                    XmpMetadataReader.Read(span, metadata);
                    break;
                case "ICCP":
                    IccMetadataReader.Read(data, metadata);
                    break;
            }
            stream.Position = next;
        }
        if (frameCount > 0) metadata.Add("WebP", "Animation Frames", frameCount);
    }

    public static void ReadAvi(string filePath, MetadataCollector metadata, CancellationToken cancellationToken)
    {
        using FileStream stream = OpenRead(filePath);
        int streamIndex = 0;
        ReadAviChunks(stream, 12, stream.Length, metadata, cancellationToken, ref streamIndex, null);
    }

    private static void ReadAviChunks(
        FileStream stream,
        long start,
        long end,
        MetadataCollector metadata,
        CancellationToken cancellationToken,
        ref int streamIndex,
        AviStreamContext? streamContext)
    {
        stream.Position = start;
        byte[] header = new byte[8];
        byte[] listTypeBytes = new byte[4];
        while (stream.Position + 8 <= end)
        {
            cancellationToken.ThrowIfCancellationRequested();
            stream.ReadExactly(header);
            string type = FourCc(header);
            uint size = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4));
            long contentStart = stream.Position;
            long chunkEnd = contentStart + size;
            long next = chunkEnd + (size & 1);
            if (chunkEnd > end || chunkEnd > stream.Length) break;

            if (type == "LIST" && size >= 4)
            {
                stream.ReadExactly(listTypeBytes);
                string listType = FourCc(listTypeBytes);
                if (listType == "strl")
                {
                    AviStreamContext context = new(++streamIndex);
                    ReadAviChunks(stream, contentStart + 4, chunkEnd, metadata, cancellationToken, ref streamIndex, context);
                }
                else if (listType == "INFO")
                {
                    ReadAviInfo(stream, contentStart + 4, chunkEnd, metadata, cancellationToken);
                }
                else if (listType is "hdrl" or "odml")
                {
                    ReadAviChunks(stream, contentStart + 4, chunkEnd, metadata, cancellationToken, ref streamIndex, streamContext);
                }
            }
            else if (size <= MaximumChunkReadSize)
            {
                byte[] data = new byte[size];
                if (data.Length > 0) stream.ReadExactly(data);
                if (type == "avih") ReadAviHeader(data, metadata);
                else if (type == "strh" && streamContext != null) ReadAviStreamHeader(data, streamContext, metadata);
                else if (type == "strf" && streamContext != null) ReadAviStreamFormat(data, streamContext, metadata);
            }
            stream.Position = next;
        }
    }

    private static void ReadAviHeader(ReadOnlySpan<byte> data, MetadataCollector metadata)
    {
        if (data.Length < 40) return;
        uint microsecondsPerFrame = BinaryPrimitives.ReadUInt32LittleEndian(data);
        uint maxBytesPerSecond = BinaryPrimitives.ReadUInt32LittleEndian(data[4..]);
        uint totalFrames = BinaryPrimitives.ReadUInt32LittleEndian(data[16..]);
        metadata.Add("AVI", "Frame Rate", microsecondsPerFrame > 0 ? $"{1_000_000d / microsecondsPerFrame:0.###} fps" : null);
        metadata.Add("AVI", "Frame Count", totalFrames);
        metadata.Add("AVI", "Duration", microsecondsPerFrame > 0 ? FormatDuration(totalFrames * microsecondsPerFrame / 1_000_000d) : null);
        metadata.Add("AVI", "Maximum Data Rate", maxBytesPerSecond > 0 ? $"{maxBytesPerSecond * 8d / 1_000_000:0.###} Mbps" : null);
        metadata.Add("AVI", "Stream Count", BinaryPrimitives.ReadUInt32LittleEndian(data[24..]));
        metadata.AddSize("AVI", BinaryPrimitives.ReadInt32LittleEndian(data[32..]), BinaryPrimitives.ReadInt32LittleEndian(data[36..]));
    }

    private static void ReadAviStreamHeader(ReadOnlySpan<byte> data, AviStreamContext context, MetadataCollector metadata)
    {
        if (data.Length < 48) return;
        context.Type = FourCc(data);
        context.Group = context.Type switch
        {
            "vids" => $"Video Track {context.Index}", "auds" => $"Audio Track {context.Index}",
            "txts" => $"Text Track {context.Index}", _ => $"Track {context.Index}"
        };
        metadata.Add(context.Group, "Stream Type", context.Type switch { "vids" => "Video", "auds" => "Audio", "txts" => "Text", _ => context.Type });
        metadata.Add(context.Group, "Codec", FourCc(data[4..8]));
        uint scale = BinaryPrimitives.ReadUInt32LittleEndian(data[20..]);
        uint rate = BinaryPrimitives.ReadUInt32LittleEndian(data[24..]);
        uint length = BinaryPrimitives.ReadUInt32LittleEndian(data[32..]);
        if (scale > 0 && rate > 0)
        {
            metadata.Add(context.Group, context.Type == "vids" ? "Frame Rate" : "Rate", $"{(double)rate / scale:0.###}");
            metadata.Add(context.Group, "Duration", FormatDuration((double)length * scale / rate));
        }
        metadata.Add(context.Group, "Length", length);
        uint sampleSize = BinaryPrimitives.ReadUInt32LittleEndian(data[44..]);
        if (sampleSize > 0) metadata.Add(context.Group, "Sample Size", $"{sampleSize} bytes");
    }

    private static void ReadAviStreamFormat(ReadOnlySpan<byte> data, AviStreamContext context, MetadataCollector metadata)
    {
        if (context.Type == "vids" && data.Length >= 20)
        {
            metadata.AddSize(context.Group, BinaryPrimitives.ReadInt32LittleEndian(data[4..]), Math.Abs(BinaryPrimitives.ReadInt32LittleEndian(data[8..])));
            metadata.Add(context.Group, "Bits Per Pixel", BinaryPrimitives.ReadUInt16LittleEndian(data[14..]));
            metadata.Add(context.Group, "Compression", FourCc(data[16..20]));
        }
        else if (context.Type == "auds" && data.Length >= 16)
        {
            metadata.Add(context.Group, "Audio Format", GetWaveFormat(BinaryPrimitives.ReadUInt16LittleEndian(data)));
            metadata.Add(context.Group, "Channels", BinaryPrimitives.ReadUInt16LittleEndian(data[2..]));
            metadata.Add(context.Group, "Sample Rate", $"{BinaryPrimitives.ReadUInt32LittleEndian(data[4..])} Hz");
            metadata.Add(context.Group, "Average Bitrate", $"{BinaryPrimitives.ReadUInt32LittleEndian(data[8..]) * 8d / 1000:0.###} kbps");
            metadata.Add(context.Group, "Bits Per Sample", BinaryPrimitives.ReadUInt16LittleEndian(data[14..]));
        }
    }

    private static void ReadAviInfo(FileStream stream, long start, long end, MetadataCollector metadata, CancellationToken cancellationToken)
    {
        Dictionary<string, string> names = new(StringComparer.Ordinal)
        {
            ["INAM"] = "Title", ["IART"] = "Artist", ["ICMT"] = "Comment", ["ICOP"] = "Copyright",
            ["ICRD"] = "Creation Date", ["ISFT"] = "Software", ["IGNR"] = "Genre", ["IKEY"] = "Keywords",
            ["ISBJ"] = "Subject", ["ISRC"] = "Source", ["ITCH"] = "Technician", ["IDIT"] = "Date/Time Original"
        };
        stream.Position = start;
        byte[] header = new byte[8];
        while (stream.Position + 8 <= end)
        {
            cancellationToken.ThrowIfCancellationRequested();
            stream.ReadExactly(header);
            string type = FourCc(header);
            uint size = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4));
            long next = stream.Position + size + (size & 1);
            if (next > end) break;
            if (size <= 1024 * 1024)
            {
                byte[] value = new byte[size];
                stream.ReadExactly(value);
                metadata.Add("AVI Info", names.GetValueOrDefault(type, type), DecodeText(value));
            }
            stream.Position = next;
        }
    }

    private static int ReadUInt24LittleEndian(ReadOnlySpan<byte> data) => data[0] | (data[1] << 8) | (data[2] << 16);
    private static string FourCc(ReadOnlySpan<byte> data) => Encoding.ASCII.GetString(data[..4]);
    private static string DecodeText(ReadOnlySpan<byte> data)
    {
        data = data.TrimEnd((byte)0);
        try { return new UTF8Encoding(false, true).GetString(data); }
        catch (DecoderFallbackException) { return Encoding.Latin1.GetString(data); }
    }
    private static string FormatDuration(double seconds) => TimeSpan.FromSeconds(seconds).ToString(seconds >= 3600 ? @"h\:mm\:ss\.fff" : @"m\:ss\.fff");
    private static string GetWaveFormat(ushort value) => value switch
    {
        1 => "PCM", 2 => "Microsoft ADPCM", 3 => "IEEE float", 6 => "A-law", 7 => "mu-law",
        0x50 => "MPEG", 0x55 => "MP3", 0x00FF => "AAC", 0x0161 => "Windows Media Audio", 0x2000 => "AC-3", _ => $"0x{value:X4}"
    };
    private static FileStream OpenRead(string filePath) =>
        new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 64 * 1024, FileOptions.SequentialScan);

    private sealed class AviStreamContext(int index)
    {
        public int Index { get; } = index;
        public string Type { get; set; } = string.Empty;
        public string Group { get; set; } = $"Track {index}";
    }
}
