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

internal static class IsoBaseMediaMetadataReader
{
    private const int MaximumBoxReadSize = 128 * 1024 * 1024;
    private static readonly DateTime QuickTimeEpoch = new(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void Read(string filePath, MetadataCollector metadata, CancellationToken cancellationToken)
    {
        using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        int trackNumber = 0;
        long position = 0;
        while (position + 8 <= stream.Length)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!TryReadBoxHeader(stream, position, stream.Length, out FileBox box)) break;
            if (box.PayloadSize <= MaximumBoxReadSize && box.PayloadSize <= int.MaxValue && box.Type is "ftyp" or "moov" or "meta")
            {
                stream.Position = box.PayloadOffset;
                byte[] payload = new byte[(int)box.PayloadSize];
                stream.ReadExactly(payload);
                if (box.Type == "ftyp") ReadFileType(payload, metadata);
                else if (box.Type == "moov") ReadMovie(payload, metadata, ref trackNumber);
                else ReadTopLevelMeta(payload, metadata);
            }
            position = box.End;
        }
    }

    private static void ReadFileType(ReadOnlySpan<byte> data, MetadataCollector metadata)
    {
        if (data.Length < 8) return;
        metadata.Add("ISO Base Media", "Major Brand", FourCc(data));
        metadata.Add("ISO Base Media", "Minor Version", BinaryPrimitives.ReadUInt32BigEndian(data[4..]));
        List<string> brands = [];
        for (int position = 8; position + 4 <= data.Length; position += 4)
        {
            string brand = FourCc(data[position..]);
            if (!string.IsNullOrWhiteSpace(brand)) brands.Add(brand);
        }
        metadata.Add("ISO Base Media", "Compatible Brands", string.Join(", ", brands.Distinct(StringComparer.Ordinal)));
    }

    private static void ReadMovie(byte[] data, MetadataCollector metadata, ref int trackNumber)
    {
        foreach (MemoryBox box in GetBoxes(data))
        {
            ReadOnlySpan<byte> payload = box.Payload(data);
            if (box.Type == "mvhd") ReadMovieHeader(payload, metadata);
            else if (box.Type == "trak") ReadTrack(payload.ToArray(), metadata, ++trackNumber);
            else if (box.Type == "udta") ReadUserData(payload.ToArray(), metadata);
            else if (box.Type == "meta") ReadMeta(payload, metadata);
        }
    }

    private static void ReadMovieHeader(ReadOnlySpan<byte> data, MetadataCollector metadata)
    {
        if (data.Length < 24) return;
        bool version1 = data[0] == 1;
        int creationOffset = 4;
        int modificationOffset = version1 ? 12 : 8;
        int timescaleOffset = version1 ? 20 : 12;
        int durationOffset = version1 ? 24 : 16;
        if (data.Length < durationOffset + (version1 ? 8 : 4)) return;
        ulong creation = version1 ? BinaryPrimitives.ReadUInt64BigEndian(data[creationOffset..]) : BinaryPrimitives.ReadUInt32BigEndian(data[creationOffset..]);
        ulong modification = version1 ? BinaryPrimitives.ReadUInt64BigEndian(data[modificationOffset..]) : BinaryPrimitives.ReadUInt32BigEndian(data[modificationOffset..]);
        uint timescale = BinaryPrimitives.ReadUInt32BigEndian(data[timescaleOffset..]);
        ulong duration = version1 ? BinaryPrimitives.ReadUInt64BigEndian(data[durationOffset..]) : BinaryPrimitives.ReadUInt32BigEndian(data[durationOffset..]);
        AddQuickTimeDate(metadata, "Movie", "Creation Time", creation);
        AddQuickTimeDate(metadata, "Movie", "Modification Time", modification);
        metadata.Add("Movie", "Time Scale", timescale);
        if (timescale > 0 && duration != ulong.MaxValue) metadata.Add("Movie", "Duration", FormatDuration((double)duration / timescale));
        int rateOffset = version1 ? 32 : 20;
        int volumeOffset = rateOffset + 4;
        if (data.Length >= volumeOffset + 2)
        {
            metadata.Add("Movie", "Preferred Rate", ReadFixed16_16(data[rateOffset..]));
            metadata.Add("Movie", "Preferred Volume", BinaryPrimitives.ReadInt16BigEndian(data[volumeOffset..]) / 256d);
        }
    }

    private static void ReadTrack(byte[] data, MetadataCollector metadata, int trackNumber)
    {
        string group = $"Track {trackNumber}";
        MemoryBox? media = null;
        foreach (MemoryBox box in GetBoxes(data))
        {
            if (box.Type == "tkhd") ReadTrackHeader(box.Payload(data), metadata, group);
            else if (box.Type == "mdia") media = box;
            else if (box.Type == "udta") ReadUserData(box.Payload(data).ToArray(), metadata, group);
        }
        if (media is MemoryBox mediaBox) ReadMedia(mediaBox.Payload(data).ToArray(), metadata, group);
    }

    private static void ReadTrackHeader(ReadOnlySpan<byte> data, MetadataCollector metadata, string group)
    {
        if (data.Length < 84) return;
        bool version1 = data[0] == 1;
        int trackIdOffset = version1 ? 20 : 12;
        int durationOffset = version1 ? 28 : 20;
        int matrixOffset = version1 ? 52 : 40;
        int widthOffset = version1 ? 88 : 76;
        if (data.Length < widthOffset + 8) return;
        metadata.Add(group, "Track ID", BinaryPrimitives.ReadUInt32BigEndian(data[trackIdOffset..]));
        ulong duration = version1 ? BinaryPrimitives.ReadUInt64BigEndian(data[durationOffset..]) : BinaryPrimitives.ReadUInt32BigEndian(data[durationOffset..]);
        if (duration != 0 && duration != ulong.MaxValue) metadata.Add(group, "Duration Units", duration);
        int width = (int)Math.Round(ReadFixed16_16(data[widthOffset..]));
        int height = (int)Math.Round(ReadFixed16_16(data[(widthOffset + 4)..]));
        if (width > 0 && height > 0) metadata.AddSize(group, width, height);
        if (data.Length >= matrixOffset + 20)
        {
            double a = ReadFixed16_16(data[matrixOffset..]);
            double b = ReadFixed16_16(data[(matrixOffset + 4)..]);
            int rotation = Math.Abs(b - 1) < 0.01 && Math.Abs(a) < 0.01 ? 90 :
                Math.Abs(a + 1) < 0.01 && Math.Abs(b) < 0.01 ? 180 :
                Math.Abs(b + 1) < 0.01 && Math.Abs(a) < 0.01 ? 270 : 0;
            if (rotation != 0) metadata.Add(group, "Rotation", $"{rotation}°");
        }
    }

    private static void ReadMedia(byte[] data, MetadataCollector metadata, string group)
    {
        string handlerType = string.Empty;
        foreach (MemoryBox box in GetBoxes(data))
        {
            ReadOnlySpan<byte> payload = box.Payload(data);
            if (box.Type == "mdhd") ReadMediaHeader(payload, metadata, group);
            else if (box.Type == "hdlr") handlerType = ReadHandler(payload, metadata, group);
            else if (box.Type == "minf") ReadMediaInformation(payload.ToArray(), metadata, group, handlerType);
        }
    }

    private static void ReadMediaHeader(ReadOnlySpan<byte> data, MetadataCollector metadata, string group)
    {
        if (data.Length < 24) return;
        bool version1 = data[0] == 1;
        int timescaleOffset = version1 ? 20 : 12;
        int durationOffset = version1 ? 24 : 16;
        int languageOffset = version1 ? 32 : 20;
        if (data.Length < languageOffset + 2) return;
        uint timescale = BinaryPrimitives.ReadUInt32BigEndian(data[timescaleOffset..]);
        ulong duration = version1 ? BinaryPrimitives.ReadUInt64BigEndian(data[durationOffset..]) : BinaryPrimitives.ReadUInt32BigEndian(data[durationOffset..]);
        metadata.Add(group, "Media Time Scale", timescale);
        if (timescale > 0 && duration != ulong.MaxValue) metadata.Add(group, "Media Duration", FormatDuration((double)duration / timescale));
        ushort language = BinaryPrimitives.ReadUInt16BigEndian(data[languageOffset..]);
        if (language != 0 && language != 0x7FFF)
        {
            char[] code = [(char)(((language >> 10) & 31) + 0x60), (char)(((language >> 5) & 31) + 0x60), (char)((language & 31) + 0x60)];
            metadata.Add(group, "Language", new string(code));
        }
    }

    private static string ReadHandler(ReadOnlySpan<byte> data, MetadataCollector metadata, string group)
    {
        if (data.Length < 12) return string.Empty;
        string type = FourCc(data[8..]);
        metadata.Add(group, "Handler Type", type switch
        {
            "vide" => "Video", "soun" => "Audio", "text" or "sbtl" or "subt" => "Subtitle",
            "meta" => "Metadata", "hint" => "Hint", _ => type
        });
        if (data.Length > 24)
        {
            ReadOnlySpan<byte> name = data[24..].TrimEnd((byte)0);
            if (!name.IsEmpty)
            {
                if (name[0] == name.Length - 1) name = name[1..];
                metadata.Add(group, "Handler Name", DecodeUtf8(name));
            }
        }
        return type;
    }

    private static void ReadMediaInformation(byte[] data, MetadataCollector metadata, string group, string handlerType)
    {
        foreach (MemoryBox box in GetBoxes(data))
        {
            if (box.Type != "stbl") continue;
            byte[] sampleTable = box.Payload(data).ToArray();
            foreach (MemoryBox child in GetBoxes(sampleTable))
            {
                if (child.Type == "stsd") ReadSampleDescriptions(child.Payload(sampleTable), metadata, group, handlerType);
            }
        }
    }

    private static void ReadSampleDescriptions(ReadOnlySpan<byte> data, MetadataCollector metadata, string group, string handlerType)
    {
        if (data.Length < 8) return;
        uint count = Math.Min(BinaryPrimitives.ReadUInt32BigEndian(data[4..]), 64u);
        int position = 8;
        for (int index = 0; index < count && position + 8 <= data.Length; index++)
        {
            uint size = BinaryPrimitives.ReadUInt32BigEndian(data[position..]);
            if (size < 8 || position + (long)size > data.Length) break;
            ReadOnlySpan<byte> entry = data.Slice(position, (int)size);
            string codec = FourCc(entry[4..]);
            metadata.Add(group, index == 0 ? "Codec" : $"Codec {index + 1}", GetCodecName(codec));
            if (handlerType == "vide" && entry.Length >= 36)
            {
                metadata.AddSize(group, BinaryPrimitives.ReadUInt16BigEndian(entry[32..]), BinaryPrimitives.ReadUInt16BigEndian(entry[34..]));
                if (entry.Length >= 84)
                {
                    metadata.Add(group, "Bit Depth", BinaryPrimitives.ReadUInt16BigEndian(entry[82..]));
                }
            }
            else if (handlerType == "soun" && entry.Length >= 36)
            {
                metadata.Add(group, "Channels", BinaryPrimitives.ReadUInt16BigEndian(entry[24..]));
                metadata.Add(group, "Bits Per Sample", BinaryPrimitives.ReadUInt16BigEndian(entry[26..]));
                metadata.Add(group, "Sample Rate", $"{ReadFixed16_16(entry[32..]):0.###} Hz");
            }
            position += (int)size;
        }
    }

    private static void ReadUserData(byte[] data, MetadataCollector metadata, string group = "QuickTime Metadata")
    {
        foreach (MemoryBox box in GetBoxes(data))
        {
            ReadOnlySpan<byte> payload = box.Payload(data);
            if (box.Type == "meta")
            {
                ReadMeta(payload, metadata);
                continue;
            }
            string? name = GetMetadataName(box.Type);
            if (name == null || payload.IsEmpty) continue;
            if (payload.Length >= 4)
            {
                ushort length = BinaryPrimitives.ReadUInt16BigEndian(payload);
                if (length > 0 && length <= payload.Length - 4) payload = payload.Slice(4, length);
            }
            metadata.Add(group, name, DecodeUtf8(payload.TrimEnd((byte)0)));
        }
    }

    private static void ReadMeta(ReadOnlySpan<byte> data, MetadataCollector metadata)
    {
        if (data.Length < 4) return;
        byte[] children = data[4..].ToArray();
        List<string> keys = [];
        MemoryBox? itemList = null;
        foreach (MemoryBox box in GetBoxes(children))
        {
            if (box.Type == "keys") keys = ReadMetadataKeys(box.Payload(children));
            else if (box.Type == "ilst") itemList = box;
        }
        if (itemList is MemoryBox ilst) ReadItemList(ilst.Payload(children).ToArray(), keys, metadata);
    }

    private static List<string> ReadMetadataKeys(ReadOnlySpan<byte> data)
    {
        List<string> keys = [];
        if (data.Length < 8) return keys;
        uint count = Math.Min(BinaryPrimitives.ReadUInt32BigEndian(data[4..]), 4096u);
        int position = 8;
        for (int index = 0; index < count && position + 8 <= data.Length; index++)
        {
            uint size = BinaryPrimitives.ReadUInt32BigEndian(data[position..]);
            if (size < 8 || position + (long)size > data.Length) break;
            keys.Add(DecodeUtf8(data.Slice(position + 8, (int)size - 8)));
            position += (int)size;
        }
        return keys;
    }

    private static void ReadItemList(byte[] data, List<string> keys, MetadataCollector metadata)
    {
        foreach (MemoryBox item in GetBoxes(data))
        {
            string name;
            if (item.TypeCode > 0 && item.TypeCode <= keys.Count) name = keys[(int)item.TypeCode - 1];
            else name = GetMetadataName(item.Type) ?? item.Type;

            byte[] itemData = item.Payload(data).ToArray();
            if (item.Type == "----")
            {
                string customName = string.Empty;
                foreach (MemoryBox child in GetBoxes(itemData))
                {
                    ReadOnlySpan<byte> payload = child.Payload(itemData);
                    if (child.Type == "name" && payload.Length > 4) customName = DecodeUtf8(payload[4..]);
                    else if (child.Type == "data") AddItemData(string.IsNullOrEmpty(customName) ? name : customName, payload, metadata);
                }
            }
            else
            {
                foreach (MemoryBox child in GetBoxes(itemData))
                {
                    if (child.Type == "data") AddItemData(name, child.Payload(itemData), metadata);
                }
            }
        }
    }

    private static void AddItemData(string name, ReadOnlySpan<byte> data, MetadataCollector metadata)
    {
        if (data.Length < 8) return;
        uint type = BinaryPrimitives.ReadUInt32BigEndian(data) & 0x00FFFFFF;
        ReadOnlySpan<byte> value = data[8..];
        string text = type switch
        {
            1 => DecodeUtf8(value),
            2 => Encoding.BigEndianUnicode.GetString(value),
            13 => $"JPEG image ({value.Length:N0} bytes)",
            14 => $"PNG image ({value.Length:N0} bytes)",
            21 => ReadSignedInteger(value).ToString(),
            22 => ReadUnsignedInteger(value).ToString(),
            _ when IsMostlyText(value) => DecodeUtf8(value.TrimEnd((byte)0)),
            _ => $"{value.Length:N0} bytes"
        };
        metadata.Add("Metadata", name, text);
    }

    private static void ReadTopLevelMeta(byte[] data, MetadataCollector metadata)
    {
        ReadMeta(data, metadata);
        FindImageSpatialExtents(data, metadata, 0);
    }

    private static void FindImageSpatialExtents(byte[] data, MetadataCollector metadata, int depth)
    {
        if (depth > 8) return;
        foreach (MemoryBox box in GetBoxes(data))
        {
            ReadOnlySpan<byte> payload = box.Payload(data);
            if (box.Type == "ispe" && payload.Length >= 12)
            {
                metadata.AddSize("Image Properties", BinaryPrimitives.ReadInt32BigEndian(payload[4..]), BinaryPrimitives.ReadInt32BigEndian(payload[8..]));
            }
            else if (box.Type is "iprp" or "ipco" or "iref" or "iinf" or "dinf")
            {
                FindImageSpatialExtents(payload.ToArray(), metadata, depth + 1);
            }
        }
    }

    private static List<MemoryBox> GetBoxes(byte[] data)
    {
        List<MemoryBox> boxes = [];
        int position = 0;
        while (position + 8 <= data.Length && boxes.Count < 100_000)
        {
            uint size32 = BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(position));
            uint typeCode = BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(position + 4));
            int headerSize = 8;
            long size = size32;
            if (size32 == 1)
            {
                if (position + 16 > data.Length) break;
                ulong extended = BinaryPrimitives.ReadUInt64BigEndian(data.AsSpan(position + 8));
                if (extended > int.MaxValue) break;
                size = (long)extended;
                headerSize = 16;
            }
            else if (size32 == 0)
            {
                size = data.Length - position;
            }
            if (size < headerSize || position + size > data.Length) break;
            boxes.Add(new MemoryBox(position, (int)size, headerSize, typeCode, FourCc(data.AsSpan(position + 4, 4))));
            position += (int)size;
        }
        return boxes;
    }

    private static bool TryReadBoxHeader(FileStream stream, long offset, long parentEnd, out FileBox box)
    {
        box = default;
        stream.Position = offset;
        Span<byte> header = stackalloc byte[16];
        if (stream.Read(header[..8]) != 8) return false;
        uint size32 = BinaryPrimitives.ReadUInt32BigEndian(header);
        string type = FourCc(header[4..8]);
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
        else if (size32 == 0)
        {
            size = parentEnd - offset;
        }
        if (size < headerSize || offset + size > parentEnd) return false;
        box = new FileBox(type, offset, size, headerSize);
        return true;
    }

    private static void AddQuickTimeDate(MetadataCollector metadata, string group, string name, ulong seconds)
    {
        if (seconds == 0 || seconds > 10_000_000_000) return;
        try { metadata.Add(group, name, QuickTimeEpoch.AddSeconds(seconds)); }
        catch (ArgumentOutOfRangeException) { }
    }

    private static string? GetMetadataName(string type) => type switch
    {
        "©nam" => "Title", "©ART" => "Artist", "©alb" => "Album", "©day" => "Date", "©too" => "Encoder",
        "©cmt" => "Comment", "©gen" => "Genre", "©wrt" => "Composer", "©cpy" => "Copyright", "©grp" => "Grouping",
        "aART" => "Album Artist", "desc" => "Description", "ldes" => "Long Description", "©xyz" => "Location",
        "keyw" => "Keywords", "purd" => "Purchase Date", "tvsh" => "TV Show", "tven" => "TV Episode ID",
        "tvnn" => "TV Network", "catg" => "Category", "purl" => "Podcast URL", "egid" => "Episode Global ID",
        "covr" => "Cover Art", "trkn" => "Track Number", "disk" => "Disc Number", "tmpo" => "Tempo", _ => null
    };

    private static string GetCodecName(string codec) => codec switch
    {
        "avc1" or "avc3" => $"H.264 / AVC ({codec})", "hvc1" or "hev1" => $"H.265 / HEVC ({codec})",
        "av01" => "AV1", "vp09" => "VP9", "mp4v" => "MPEG-4 Visual", "jpeg" => "Motion JPEG",
        "ap4h" => "Apple ProRes 4444", "apch" => "Apple ProRes 422 HQ", "apcn" => "Apple ProRes 422",
        "apcs" => "Apple ProRes 422 LT", "apco" => "Apple ProRes 422 Proxy", "mp4a" => "MPEG-4 Audio / AAC",
        "ac-3" => "Dolby Digital (AC-3)", "ec-3" => "Dolby Digital Plus (E-AC-3)", "alac" => "Apple Lossless",
        "Opus" => "Opus", "flac" => "FLAC", _ => codec
    };

    private static double ReadFixed16_16(ReadOnlySpan<byte> data) => BinaryPrimitives.ReadInt32BigEndian(data) / 65536d;
    private static long ReadSignedInteger(ReadOnlySpan<byte> data) => data.Length switch
    {
        1 => (sbyte)data[0], 2 => BinaryPrimitives.ReadInt16BigEndian(data), 4 => BinaryPrimitives.ReadInt32BigEndian(data),
        >= 8 => BinaryPrimitives.ReadInt64BigEndian(data), _ => 0
    };
    private static ulong ReadUnsignedInteger(ReadOnlySpan<byte> data) => data.Length switch
    {
        1 => data[0], 2 => BinaryPrimitives.ReadUInt16BigEndian(data), 4 => BinaryPrimitives.ReadUInt32BigEndian(data),
        >= 8 => BinaryPrimitives.ReadUInt64BigEndian(data), _ => 0
    };
    private static bool IsMostlyText(ReadOnlySpan<byte> data)
    {
        if (data.IsEmpty) return false;
        int textBytes = 0;
        foreach (byte value in data)
        {
            if (value is >= 0x20 and < 0x7F || value >= 0xC2) textBytes++;
        }
        return textBytes >= data.Length * 3 / 4;
    }
    private static string DecodeUtf8(ReadOnlySpan<byte> value)
    {
        try { return new UTF8Encoding(false, true).GetString(value); }
        catch (DecoderFallbackException) { return Encoding.Latin1.GetString(value); }
    }
    private static string FourCc(ReadOnlySpan<byte> value) => Encoding.Latin1.GetString(value[..4]).TrimEnd('\0', ' ');
    private static string FormatDuration(double seconds) => TimeSpan.FromSeconds(seconds).ToString(seconds >= 3600 ? @"h\:mm\:ss\.fff" : @"m\:ss\.fff");

    private readonly record struct FileBox(string Type, long Offset, long Size, int HeaderSize)
    {
        public long PayloadOffset => Offset + HeaderSize;
        public long PayloadSize => Size - HeaderSize;
        public long End => Offset + Size;
    }

    private readonly record struct MemoryBox(int Offset, int Size, int HeaderSize, uint TypeCode, string Type)
    {
        public ReadOnlySpan<byte> Payload(byte[] data) => data.AsSpan(Offset + HeaderSize, Size - HeaderSize);
    }
}
