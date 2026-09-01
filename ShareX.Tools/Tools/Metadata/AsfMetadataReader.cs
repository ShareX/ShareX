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

internal static class AsfMetadataReader
{
    internal static readonly Guid HeaderObject = new("75B22630-668E-11CF-A6D9-00AA0062CE6C");
    internal static readonly Guid FilePropertiesObject = new("8CABDCA1-A947-11CF-8EE4-00C00C205365");
    internal static readonly Guid StreamPropertiesObject = new("B7DC0791-A9B7-11CF-8EE6-00C00C205365");
    internal static readonly Guid ContentDescriptionObject = new("75B22633-668E-11CF-A6D9-00AA0062CE6C");
    internal static readonly Guid ExtendedContentDescriptionObject = new("D2D0A440-E307-11D2-97F0-00A0C95EA850");
    internal static readonly Guid HeaderExtensionObject = new("5FBF03B5-A92E-11CF-8EE3-00C00C205365");
    internal static readonly Guid PaddingObject = new("1806D474-CADF-4509-A4BA-9AABCB96AAE8");
    internal static readonly Guid MetadataObject = new("C5F8CBEA-5BAF-4877-8467-AA8C44FA4CCA");
    internal static readonly Guid MetadataLibraryObject = new("44231C94-9498-49D1-A141-1D134E457054");

    private static readonly Guid AudioMedia = new("F8699E40-5B4D-11CF-A8FD-00805F5C442B");
    private static readonly Guid VideoMedia = new("BC19EFC0-5B4D-11CF-A8FD-00805F5C442B");
    private const int MaximumObjectReadSize = 64 * 1024 * 1024;

    public static void Read(string filePath, MetadataCollector metadata, CancellationToken cancellationToken)
    {
        using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        Span<byte> header = stackalloc byte[30];
        if (!TryReadExactly(stream, header) || new Guid(header[..16]) != HeaderObject) return;
        ulong headerSize = BinaryPrimitives.ReadUInt64LittleEndian(header[16..]);
        uint objectCount = BinaryPrimitives.ReadUInt32LittleEndian(header[24..]);
        metadata.Add("ASF", "Header Size", $"{headerSize:N0} bytes");
        metadata.Add("ASF", "Header Object Count", objectCount);
        long headerEnd = Math.Min((long)Math.Min(headerSize, long.MaxValue), stream.Length);
        int trackIndex = 0;
        byte[] objectHeader = new byte[24];

        for (uint index = 0; index < objectCount && stream.Position + 24 <= headerEnd; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            long offset = stream.Position;
            stream.ReadExactly(objectHeader);
            Guid id = new(objectHeader.AsSpan(0, 16));
            ulong size = BinaryPrimitives.ReadUInt64LittleEndian(objectHeader.AsSpan(16));
            if (size < 24 || size > long.MaxValue || offset + (long)size > headerEnd) break;
            int payloadSize = (int)Math.Min(size - 24, MaximumObjectReadSize);
            byte[] data = new byte[payloadSize];
            if (payloadSize > 0) stream.ReadExactly(data);

            if (id == FilePropertiesObject) ReadFileProperties(data, metadata);
            else if (id == StreamPropertiesObject) ReadStreamProperties(data, metadata, ++trackIndex);
            else if (id == ContentDescriptionObject) ReadContentDescription(data, metadata);
            else if (id == ExtendedContentDescriptionObject) ReadExtendedContentDescription(data, metadata);
            else if (id == HeaderExtensionObject) ReadHeaderExtension(data, metadata);

            stream.Position = offset + (long)size;
        }
    }

    private static void ReadFileProperties(ReadOnlySpan<byte> data, MetadataCollector metadata)
    {
        if (data.Length < 80) return;
        ulong fileSize = BinaryPrimitives.ReadUInt64LittleEndian(data[16..]);
        ulong creationFileTime = BinaryPrimitives.ReadUInt64LittleEndian(data[24..]);
        ulong packetCount = BinaryPrimitives.ReadUInt64LittleEndian(data[32..]);
        ulong playDuration = BinaryPrimitives.ReadUInt64LittleEndian(data[40..]);
        ulong preroll = BinaryPrimitives.ReadUInt64LittleEndian(data[56..]);
        metadata.Add("ASF", "Declared File Size", $"{fileSize:N0} bytes");
        metadata.Add("ASF", "Data Packet Count", packetCount);
        if (creationFileTime > 0 && creationFileTime <= long.MaxValue)
        {
            try { metadata.Add("ASF", "Creation Time", DateTime.FromFileTimeUtc((long)creationFileTime)); }
            catch (ArgumentOutOfRangeException) { }
        }
        double durationSeconds = playDuration / 10_000_000d - preroll / 1000d;
        if (durationSeconds > 0) metadata.Add("ASF", "Duration", FormatDuration(durationSeconds));
        metadata.Add("ASF", "Preroll", $"{preroll} ms");
        uint flags = BinaryPrimitives.ReadUInt32LittleEndian(data[64..]);
        metadata.Add("ASF", "Broadcast", (flags & 1) != 0 ? "Yes" : "No");
        metadata.Add("ASF", "Seekable", (flags & 2) != 0 ? "Yes" : "No");
        metadata.Add("ASF", "Minimum Packet Size", BinaryPrimitives.ReadUInt32LittleEndian(data[68..]));
        metadata.Add("ASF", "Maximum Packet Size", BinaryPrimitives.ReadUInt32LittleEndian(data[72..]));
        metadata.Add("ASF", "Maximum Bitrate", $"{BinaryPrimitives.ReadUInt32LittleEndian(data[76..]) / 1000d:0.###} kbps");
    }

    private static void ReadStreamProperties(ReadOnlySpan<byte> data, MetadataCollector metadata, int index)
    {
        if (data.Length < 54) return;
        Guid type = new(data[..16]);
        string group = type == VideoMedia ? $"Video Track {index}" : type == AudioMedia ? $"Audio Track {index}" : $"Track {index}";
        metadata.Add(group, "Stream Type", type == VideoMedia ? "Video" : type == AudioMedia ? "Audio" : type.ToString());
        uint typeDataLength = BinaryPrimitives.ReadUInt32LittleEndian(data[40..]);
        ushort flags = BinaryPrimitives.ReadUInt16LittleEndian(data[48..]);
        metadata.Add(group, "Stream Number", flags & 0x7F);
        metadata.Add(group, "Encrypted", (flags & 0x8000) != 0 ? "Yes" : "No");
        if (typeDataLength > data.Length - 54) return;
        ReadOnlySpan<byte> format = data.Slice(54, (int)typeDataLength);
        if (type == AudioMedia && format.Length >= 16)
        {
            ushort codec = BinaryPrimitives.ReadUInt16LittleEndian(format);
            metadata.Add(group, "Codec", GetAudioCodec(codec));
            metadata.Add(group, "Channels", BinaryPrimitives.ReadUInt16LittleEndian(format[2..]));
            metadata.Add(group, "Sample Rate", $"{BinaryPrimitives.ReadUInt32LittleEndian(format[4..])} Hz");
            metadata.Add(group, "Average Bitrate", $"{BinaryPrimitives.ReadUInt32LittleEndian(format[8..]) * 8d / 1000:0.###} kbps");
            metadata.Add(group, "Bits Per Sample", BinaryPrimitives.ReadUInt16LittleEndian(format[14..]));
        }
        else if (type == VideoMedia && format.Length >= 51)
        {
            int width = BinaryPrimitives.ReadInt32LittleEndian(format);
            int height = BinaryPrimitives.ReadInt32LittleEndian(format[4..]);
            metadata.AddSize(group, width, Math.Abs(height));
            metadata.Add(group, "Bits Per Pixel", BinaryPrimitives.ReadUInt16LittleEndian(format[25..]));
            metadata.Add(group, "Codec", Encoding.ASCII.GetString(format[27..31]).TrimEnd('\0', ' '));
        }
    }

    private static void ReadContentDescription(ReadOnlySpan<byte> data, MetadataCollector metadata)
    {
        if (data.Length < 10) return;
        string[] names = ["Title", "Author", "Copyright", "Description", "Rating"];
        int position = 10;
        for (int index = 0; index < names.Length; index++)
        {
            int length = BinaryPrimitives.ReadUInt16LittleEndian(data[(index * 2)..]);
            if (position + length > data.Length) break;
            metadata.Add("ASF Metadata", names[index], ReadUnicode(data.Slice(position, length)));
            position += length;
        }
    }

    private static void ReadExtendedContentDescription(ReadOnlySpan<byte> data, MetadataCollector metadata)
    {
        if (data.Length < 2) return;
        int count = Math.Min(BinaryPrimitives.ReadUInt16LittleEndian(data), (ushort)4096);
        int position = 2;
        for (int index = 0; index < count && position + 2 <= data.Length; index++)
        {
            int nameLength = BinaryPrimitives.ReadUInt16LittleEndian(data[position..]);
            position += 2;
            if (position + nameLength + 4 > data.Length) break;
            string name = ReadUnicode(data.Slice(position, nameLength));
            position += nameLength;
            ushort type = BinaryPrimitives.ReadUInt16LittleEndian(data[position..]);
            int valueLength = BinaryPrimitives.ReadUInt16LittleEndian(data[(position + 2)..]);
            position += 4;
            if (position + valueLength > data.Length) break;
            ReadOnlySpan<byte> value = data.Slice(position, valueLength);
            string text = type switch
            {
                0 => ReadUnicode(value),
                1 => $"{value.Length:N0} bytes",
                2 when value.Length >= 4 => BinaryPrimitives.ReadUInt32LittleEndian(value) != 0 ? "Yes" : "No",
                3 when value.Length >= 4 => BinaryPrimitives.ReadUInt32LittleEndian(value).ToString(),
                4 when value.Length >= 8 => BinaryPrimitives.ReadUInt64LittleEndian(value).ToString(),
                5 when value.Length >= 2 => BinaryPrimitives.ReadUInt16LittleEndian(value).ToString(),
                6 when value.Length >= 16 => new Guid(value[..16]).ToString(),
                _ => $"{value.Length:N0} bytes"
            };
            metadata.Add("ASF Metadata", name, text);
            position += valueLength;
        }
    }

    private static void ReadHeaderExtension(ReadOnlySpan<byte> data, MetadataCollector metadata)
    {
        if (data.Length < 22) return;
        uint extensionSize = BinaryPrimitives.ReadUInt32LittleEndian(data[18..]);
        if (extensionSize > data.Length - 22) return;
        int position = 22;
        int end = position + (int)extensionSize;
        while (position + 24 <= end)
        {
            Guid id = new(data.Slice(position, 16));
            ulong size = BinaryPrimitives.ReadUInt64LittleEndian(data[(position + 16)..]);
            if (size < 24 || size > int.MaxValue || position + (long)size > end) break;
            ReadOnlySpan<byte> payload = data.Slice(position + 24, (int)size - 24);
            if (id == MetadataObject || id == MetadataLibraryObject)
            {
                metadata.Add("ASF Metadata", id == MetadataObject ? "Metadata Object" : "Metadata Library Object", $"{payload.Length:N0} bytes");
            }
            position += (int)size;
        }
    }

    private static string ReadUnicode(ReadOnlySpan<byte> value) => Encoding.Unicode.GetString(value).TrimEnd('\0');
    private static string FormatDuration(double seconds) => TimeSpan.FromSeconds(seconds).ToString(seconds >= 3600 ? @"h\:mm\:ss\.fff" : @"m\:ss\.fff");
    private static string GetAudioCodec(ushort value) => value switch
    {
        1 => "PCM",
        2 => "Microsoft ADPCM",
        3 => "IEEE float",
        0x50 => "MPEG",
        0x55 => "MP3",
        0x00FF => "AAC",
        0x0161 => "Windows Media Audio",
        0x0162 => "Windows Media Audio Professional",
        0x0163 => "Windows Media Audio Lossless",
        0x2000 => "AC-3",
        _ => $"0x{value:X4}"
    };

    private static bool TryReadExactly(Stream stream, Span<byte> buffer)
    {
        int offset = 0;
        while (offset < buffer.Length)
        {
            int count = stream.Read(buffer[offset..]);
            if (count == 0) return false;
            offset += count;
        }
        return true;
    }
}
