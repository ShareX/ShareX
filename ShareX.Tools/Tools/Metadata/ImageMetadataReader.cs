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
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ShareX.Tools;

internal static class ImageMetadataReader
{
    private const int MaximumMetadataChunkSize = 32 * 1024 * 1024;

    public static void Read(
        string filePath,
        MetadataFormat format,
        MetadataCollector metadata,
        CancellationToken cancellationToken)
    {
        switch (format)
        {
            case MetadataFormat.Jpeg:
                ReadJpeg(filePath, metadata, cancellationToken);
                break;
            case MetadataFormat.Png:
                ReadPng(filePath, metadata, cancellationToken);
                break;
            case MetadataFormat.Gif:
                ReadGif(filePath, metadata, cancellationToken);
                break;
            case MetadataFormat.Bmp:
                ReadBmp(filePath, metadata);
                break;
            case MetadataFormat.Tiff:
                TiffMetadataReader.ReadFile(filePath, metadata, cancellationToken);
                break;
            case MetadataFormat.WebP:
                RiffMetadataReader.ReadWebP(filePath, metadata, cancellationToken);
                break;
            case MetadataFormat.Icon:
                ReadIcon(filePath, metadata);
                break;
        }
    }

    private static void ReadJpeg(string filePath, MetadataCollector metadata, CancellationToken cancellationToken)
    {
        using FileStream stream = OpenRead(filePath);
        if (stream.ReadByte() != 0xFF || stream.ReadByte() != 0xD8)
        {
            return;
        }

        SortedDictionary<int, byte[]> iccChunks = [];
        int iccChunkCount = 0;
        byte[] lengthBytes = new byte[2];
        while (TryReadJpegMarker(stream, out byte marker))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (marker is 0xD9 or 0xDA)
            {
                break;
            }
            if (marker is 0x01 or >= 0xD0 and <= 0xD7)
            {
                continue;
            }

            if (!TryReadExactly(stream, lengthBytes)) break;
            int length = BinaryPrimitives.ReadUInt16BigEndian(lengthBytes) - 2;
            if (length < 0 || length > MaximumMetadataChunkSize || length > stream.Length - stream.Position)
            {
                break;
            }

            byte[] data = new byte[length];
            stream.ReadExactly(data);
            ReadOnlySpan<byte> span = data;

            if (IsStartOfFrame(marker) && span.Length >= 6)
            {
                metadata.Add("JPEG", "Encoding Process", GetJpegEncoding(marker));
                metadata.Add("JPEG", "Bits Per Sample", span[0]);
                metadata.AddSize("JPEG", BinaryPrimitives.ReadUInt16BigEndian(span[3..]), BinaryPrimitives.ReadUInt16BigEndian(span[1..]));
                metadata.Add("JPEG", "Color Components", span[5]);
            }
            else if (marker == 0xE0 && span.StartsWith("JFIF\0"u8) && span.Length >= 14)
            {
                metadata.Add("JFIF", "JFIF Version", $"{span[5]}.{span[6]:00}");
                metadata.Add("JFIF", "Resolution Unit", span[7] switch { 1 => "inches", 2 => "centimeters", _ => "none" });
                metadata.Add("JFIF", "X Resolution", BinaryPrimitives.ReadUInt16BigEndian(span[8..]));
                metadata.Add("JFIF", "Y Resolution", BinaryPrimitives.ReadUInt16BigEndian(span[10..]));
                metadata.Add("JFIF", "Thumbnail Size", $"{span[12]} x {span[13]}");
            }
            else if (marker == 0xE1 && span.StartsWith("Exif\0\0"u8))
            {
                TiffMetadataReader.Read(span[6..].ToArray(), metadata, "EXIF");
            }
            else if (marker == 0xE1 && span.StartsWith("http://ns.adobe.com/xap/1.0/\0"u8))
            {
                XmpMetadataReader.Read(span[29..], metadata);
            }
            else if (marker == 0xE2 && span.StartsWith("ICC_PROFILE\0"u8) && span.Length >= 14)
            {
                int sequence = span[12];
                iccChunkCount = Math.Max(iccChunkCount, span[13]);
                if (sequence > 0 && !iccChunks.ContainsKey(sequence))
                {
                    iccChunks.Add(sequence, span[14..].ToArray());
                }
            }
            else if (marker == 0xED && span.StartsWith("Photoshop 3.0\0"u8))
            {
                ReadPhotoshopResources(span[14..], metadata);
            }
            else if (marker == 0xEE && span.StartsWith("Adobe"u8) && span.Length >= 12)
            {
                metadata.Add("Adobe", "DCT Encode Version", BinaryPrimitives.ReadUInt16BigEndian(span[5..]));
                metadata.Add("Adobe", "Color Transform", span[11] switch { 0 => "Unknown (RGB or CMYK)", 1 => "YCbCr", 2 => "YCCK", _ => span[11] });
            }
            else if (marker == 0xFE)
            {
                metadata.Add("JPEG", "Comment", DecodeText(span));
            }
        }

        if (iccChunkCount > 0 && iccChunks.Count == iccChunkCount)
        {
            using MemoryStream profile = new();
            foreach (byte[] chunk in iccChunks.Values)
            {
                profile.Write(chunk);
            }
            IccMetadataReader.Read(profile.ToArray(), metadata);
        }
    }

    private static void ReadPng(string filePath, MetadataCollector metadata, CancellationToken cancellationToken)
    {
        using FileStream stream = OpenRead(filePath);
        stream.Position = 8;
        byte[] header = new byte[8];
        while (stream.Position + 12 <= stream.Length)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!TryReadExactly(stream, header)) break;
            uint chunkLength = BinaryPrimitives.ReadUInt32BigEndian(header);
            string chunkType = Encoding.ASCII.GetString(header.AsSpan(4));
            if (chunkLength > int.MaxValue || chunkLength > stream.Length - stream.Position - 4)
            {
                break;
            }

            bool shouldRead = chunkLength <= MaximumMetadataChunkSize && chunkType is
                "IHDR" or "pHYs" or "gAMA" or "sRGB" or "cHRM" or "cICP" or "mDCV" or "cLLI" or
                "tIME" or "tEXt" or "zTXt" or "iTXt" or "eXIf" or "iCCP" or "acTL";
            byte[] data = shouldRead ? new byte[(int)chunkLength] : [];
            if (shouldRead)
            {
                stream.ReadExactly(data);
            }
            else
            {
                stream.Position += chunkLength;
            }
            stream.Position += 4; // CRC

            ReadOnlySpan<byte> span = data;
            switch (chunkType)
            {
                case "IHDR" when span.Length == 13:
                    metadata.AddSize("PNG", BinaryPrimitives.ReadInt32BigEndian(span), BinaryPrimitives.ReadInt32BigEndian(span[4..]));
                    metadata.Add("PNG", "Bit Depth", span[8]);
                    metadata.Add("PNG", "Color Type", GetPngColorType(span[9]));
                    metadata.Add("PNG", "Interlace", span[12] == 1 ? "Adam7" : "None");
                    break;
                case "pHYs" when span.Length == 9:
                    uint x = BinaryPrimitives.ReadUInt32BigEndian(span);
                    uint y = BinaryPrimitives.ReadUInt32BigEndian(span[4..]);
                    metadata.Add("PNG", "Pixels Per Unit X", x);
                    metadata.Add("PNG", "Pixels Per Unit Y", y);
                    metadata.Add("PNG", "Pixel Units", span[8] == 1 ? "meters" : "unknown");
                    if (span[8] == 1)
                    {
                        metadata.Add("PNG", "Resolution", $"{x * 0.0254:0.##} x {y * 0.0254:0.##} DPI");
                    }
                    break;
                case "gAMA" when span.Length == 4:
                    metadata.Add("PNG", "Gamma", BinaryPrimitives.ReadUInt32BigEndian(span) / 100000d);
                    break;
                case "sRGB" when span.Length == 1:
                    metadata.Add("PNG", "Rendering Intent", GetRenderingIntent(span[0]));
                    break;
                case "cHRM" when span.Length == 32:
                    metadata.Add("PNG", "White Point", ReadPngPoint(span));
                    metadata.Add("PNG", "Red Primary", ReadPngPoint(span[8..]));
                    metadata.Add("PNG", "Green Primary", ReadPngPoint(span[16..]));
                    metadata.Add("PNG", "Blue Primary", ReadPngPoint(span[24..]));
                    break;
                case "cICP" when span.Length == 4:
                    metadata.Add("PNG", "Color Primaries", span[0]);
                    metadata.Add("PNG", "Transfer Characteristics", span[1]);
                    metadata.Add("PNG", "Matrix Coefficients", span[2]);
                    metadata.Add("PNG", "Video Full Range Flag", span[3]);
                    break;
                case "tIME" when span.Length == 7:
                    TryAddPngTime(span, metadata);
                    break;
                case "tEXt":
                    ReadPngText(span, metadata, compressed: false);
                    break;
                case "zTXt":
                    ReadPngText(span, metadata, compressed: true);
                    break;
                case "iTXt":
                    ReadPngInternationalText(span, metadata);
                    break;
                case "eXIf":
                    TiffMetadataReader.Read(data, metadata, "EXIF");
                    break;
                case "iCCP":
                    ReadPngIcc(span, metadata);
                    break;
                case "acTL" when span.Length == 8:
                    metadata.Add("PNG", "Animation Frames", BinaryPrimitives.ReadUInt32BigEndian(span));
                    metadata.Add("PNG", "Animation Plays", BinaryPrimitives.ReadUInt32BigEndian(span[4..]) is uint plays && plays == 0 ? "Infinite" : plays);
                    break;
            }

            if (chunkType == "IEND") break;
        }
    }

    private static void ReadGif(string filePath, MetadataCollector metadata, CancellationToken cancellationToken)
    {
        using FileStream stream = OpenRead(filePath);
        Span<byte> header = stackalloc byte[13];
        if (!TryReadExactly(stream, header)) return;
        metadata.Add("GIF", "Version", Encoding.ASCII.GetString(header[3..6]));
        metadata.AddSize("GIF", BinaryPrimitives.ReadUInt16LittleEndian(header[6..]), BinaryPrimitives.ReadUInt16LittleEndian(header[8..]));
        metadata.Add("GIF", "Color Resolution", ((header[10] >> 4) & 7) + 1);
        metadata.Add("GIF", "Background Color Index", header[11]);

        if ((header[10] & 0x80) != 0)
        {
            int tableSize = 3 << ((header[10] & 7) + 1);
            stream.Position += tableSize;
            metadata.Add("GIF", "Global Color Table", $"{tableSize / 3} colors");
        }

        int frameCount = 0;
        double durationSeconds = 0;
        byte[] descriptor = new byte[9];
        while (stream.Position < stream.Length)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int introducer = stream.ReadByte();
            if (introducer is -1 or 0x3B) break;
            if (introducer == 0x2C)
            {
                if (!TryReadExactly(stream, descriptor)) break;
                frameCount++;
                if ((descriptor[8] & 0x80) != 0)
                {
                    stream.Position += 3 << ((descriptor[8] & 7) + 1);
                }
                if (stream.ReadByte() < 0) break;
                SkipGifSubBlocks(stream);
            }
            else if (introducer == 0x21)
            {
                int label = stream.ReadByte();
                if (label == 0xF9)
                {
                    int size = stream.ReadByte();
                    byte[] block = size > 0 ? new byte[size] : [];
                    if (size > 0) stream.ReadExactly(block);
                    stream.ReadByte();
                    if (block.Length >= 3)
                    {
                        durationSeconds += BinaryPrimitives.ReadUInt16LittleEndian(block.AsSpan(1)) / 100d;
                    }
                }
                else if (label == 0xFF)
                {
                    int size = stream.ReadByte();
                    byte[] application = size > 0 ? new byte[size] : [];
                    if (size > 0) stream.ReadExactly(application);
                    byte[] content = ReadGifSubBlocks(stream);
                    string identifier = Encoding.ASCII.GetString(application);
                    if (identifier.StartsWith("NETSCAPE2.0", StringComparison.Ordinal) && content.Length >= 3 && content[0] == 1)
                    {
                        ushort loops = BinaryPrimitives.ReadUInt16LittleEndian(content.AsSpan(1));
                        metadata.Add("GIF", "Animation Loop Count", loops == 0 ? "Infinite" : loops);
                    }
                    else if (identifier.StartsWith("XMP DataXMP", StringComparison.Ordinal))
                    {
                        XmpMetadataReader.Read(content, metadata);
                    }
                    else
                    {
                        metadata.Add("GIF", "Application Extension", identifier);
                    }
                }
                else if (label == 0xFE)
                {
                    metadata.Add("GIF", "Comment", DecodeText(ReadGifSubBlocks(stream)));
                }
                else
                {
                    SkipGifSubBlocks(stream);
                }
            }
            else
            {
                break;
            }
        }

        metadata.Add("GIF", "Frame Count", frameCount);
        if (durationSeconds > 0) metadata.Add("GIF", "Duration", FormatDuration(durationSeconds));
    }

    private static void ReadBmp(string filePath, MetadataCollector metadata)
    {
        using FileStream stream = OpenRead(filePath);
        Span<byte> header = stackalloc byte[54];
        if (!TryReadExactly(stream, header)) return;
        uint dibSize = BinaryPrimitives.ReadUInt32LittleEndian(header[14..]);
        metadata.Add("BMP", "DIB Header", dibSize switch
        {
            12 => "BITMAPCOREHEADER",
            40 => "BITMAPINFOHEADER",
            108 => "BITMAPV4HEADER",
            124 => "BITMAPV5HEADER",
            _ => $"{dibSize} bytes"
        });
        if (dibSize >= 40)
        {
            int width = BinaryPrimitives.ReadInt32LittleEndian(header[18..]);
            int height = Math.Abs(BinaryPrimitives.ReadInt32LittleEndian(header[22..]));
            metadata.AddSize("BMP", width, height);
            metadata.Add("BMP", "Bits Per Pixel", BinaryPrimitives.ReadUInt16LittleEndian(header[28..]));
            metadata.Add("BMP", "Compression", GetBmpCompression(BinaryPrimitives.ReadUInt32LittleEndian(header[30..])));
            metadata.Add("BMP", "Image Data Offset", BinaryPrimitives.ReadUInt32LittleEndian(header[10..]));
        }
    }

    private static void ReadIcon(string filePath, MetadataCollector metadata)
    {
        using FileStream stream = OpenRead(filePath);
        Span<byte> header = stackalloc byte[6];
        if (!TryReadExactly(stream, header)) return;
        ushort type = BinaryPrimitives.ReadUInt16LittleEndian(header[2..]);
        ushort count = BinaryPrimitives.ReadUInt16LittleEndian(header[4..]);
        metadata.Add("ICO", "Resource Type", type == 2 ? "Cursor" : "Icon");
        metadata.Add("ICO", "Image Count", count);
        count = Math.Min(count, (ushort)256);
        byte[] entry = new byte[16];
        for (int index = 0; index < count; index++)
        {
            if (!TryReadExactly(stream, entry)) break;
            int width = entry[0] == 0 ? 256 : entry[0];
            int height = entry[1] == 0 ? 256 : entry[1];
            string value = $"{width} x {height}, {entry[2]} colors, {BinaryPrimitives.ReadUInt16LittleEndian(entry.AsSpan(6))} bpp, {BinaryPrimitives.ReadUInt32LittleEndian(entry.AsSpan(8))} bytes";
            metadata.Add("ICO", $"Image {index + 1}", value);
        }
    }

    private static void ReadPngText(ReadOnlySpan<byte> data, MetadataCollector metadata, bool compressed)
    {
        int separator = data.IndexOf((byte)0);
        if (separator <= 0) return;
        string keyword = Encoding.Latin1.GetString(data[..separator]);
        ReadOnlySpan<byte> value = data[(separator + 1)..];
        if (compressed)
        {
            if (value.Length < 2 || value[0] != 0) return;
            byte[]? decompressed = TryDecompress(value[1..]);
            if (decompressed == null) return;
            value = decompressed;
        }
        AddTextMetadata(keyword, DecodeText(value), metadata);
    }

    private static void ReadPngInternationalText(ReadOnlySpan<byte> data, MetadataCollector metadata)
    {
        int keywordEnd = data.IndexOf((byte)0);
        if (keywordEnd <= 0 || keywordEnd + 5 > data.Length) return;
        string keyword = Encoding.Latin1.GetString(data[..keywordEnd]);
        int position = keywordEnd + 1;
        bool compressed = data[position++] == 1;
        if (data[position++] != 0) return;
        int languageEnd = data[position..].IndexOf((byte)0);
        if (languageEnd < 0) return;
        string language = Encoding.ASCII.GetString(data.Slice(position, languageEnd));
        position += languageEnd + 1;
        int translatedEnd = data[position..].IndexOf((byte)0);
        if (translatedEnd < 0) return;
        position += translatedEnd + 1;
        ReadOnlySpan<byte> value = data[position..];
        if (compressed)
        {
            byte[]? decompressed = TryDecompress(value);
            if (decompressed == null) return;
            value = decompressed;
        }
        string tag = string.IsNullOrWhiteSpace(language) ? keyword : $"{keyword} ({language})";
        AddTextMetadata(tag, Encoding.UTF8.GetString(value), metadata);
    }

    private static void AddTextMetadata(string keyword, string value, MetadataCollector metadata)
    {
        if (keyword.Equals("XML:com.adobe.xmp", StringComparison.OrdinalIgnoreCase))
        {
            XmpMetadataReader.Read(Encoding.UTF8.GetBytes(value), metadata);
        }
        else
        {
            metadata.Add("PNG Text", keyword, value);
        }
    }

    private static void ReadPngIcc(ReadOnlySpan<byte> data, MetadataCollector metadata)
    {
        int separator = data.IndexOf((byte)0);
        if (separator <= 0 || separator + 2 > data.Length || data[separator + 1] != 0) return;
        metadata.Add("ICC Profile", "Profile Name", Encoding.Latin1.GetString(data[..separator]));
        byte[]? profile = TryDecompress(data[(separator + 2)..]);
        if (profile != null) IccMetadataReader.Read(profile, metadata);
    }

    private static void ReadPhotoshopResources(ReadOnlySpan<byte> data, MetadataCollector metadata)
    {
        int position = 0;
        while (position + 12 <= data.Length && data.Slice(position, 4).SequenceEqual("8BIM"u8))
        {
            ushort id = BinaryPrimitives.ReadUInt16BigEndian(data[(position + 4)..]);
            int nameLength = data[position + 6];
            int nameFieldLength = 1 + nameLength;
            if ((nameFieldLength & 1) != 0) nameFieldLength++;
            int sizePosition = position + 6 + nameFieldLength;
            if (sizePosition + 4 > data.Length) break;
            uint size = BinaryPrimitives.ReadUInt32BigEndian(data[sizePosition..]);
            int valuePosition = sizePosition + 4;
            if (size > int.MaxValue || valuePosition + (long)size > data.Length) break;
            ReadOnlySpan<byte> value = data.Slice(valuePosition, (int)size);
            if (id == 0x0404) ReadIptc(value, metadata);
            else if (id == 0x0424) XmpMetadataReader.Read(value, metadata);
            position = valuePosition + (int)size + ((int)size & 1);
        }
    }

    private static void ReadIptc(ReadOnlySpan<byte> data, MetadataCollector metadata)
    {
        Dictionary<int, string> names = new()
        {
            [5] = "Object Name", [7] = "Edit Status", [10] = "Urgency", [15] = "Category",
            [20] = "Supplemental Category", [25] = "Keywords", [40] = "Special Instructions",
            [55] = "Date Created", [60] = "Time Created", [65] = "Originating Program", [70] = "Program Version",
            [80] = "By-line", [85] = "By-line Title", [90] = "City", [92] = "Sub-location", [95] = "Province/State",
            [100] = "Country Code", [101] = "Country", [103] = "Original Transmission Reference",
            [105] = "Headline", [110] = "Credit", [115] = "Source", [116] = "Copyright Notice",
            [118] = "Contact", [120] = "Caption/Abstract", [122] = "Writer/Editor"
        };
        int position = 0;
        while (position + 5 <= data.Length)
        {
            if (data[position] != 0x1C)
            {
                position++;
                continue;
            }
            int record = data[position + 1];
            int dataset = data[position + 2];
            int length = BinaryPrimitives.ReadUInt16BigEndian(data[(position + 3)..]);
            position += 5;
            if (length < 0 || position + length > data.Length) break;
            if (record == 2 && names.TryGetValue(dataset, out string? name))
            {
                metadata.Add("IPTC", name, DecodeText(data.Slice(position, length)));
            }
            position += length;
        }
    }

    private static void TryAddPngTime(ReadOnlySpan<byte> data, MetadataCollector metadata)
    {
        try
        {
            DateTime value = new(BinaryPrimitives.ReadUInt16BigEndian(data), data[2], data[3], data[4], data[5], data[6], DateTimeKind.Utc);
            metadata.Add("PNG", "Modification Time", value);
        }
        catch (ArgumentOutOfRangeException)
        {
        }
    }

    private static byte[]? TryDecompress(ReadOnlySpan<byte> compressed)
    {
        try
        {
            using MemoryStream source = new(compressed.ToArray());
            using ZLibStream zlib = new(source, CompressionMode.Decompress);
            using MemoryStream destination = new();
            byte[] buffer = new byte[8192];
            int count;
            while ((count = zlib.Read(buffer)) > 0)
            {
                if (destination.Length + count > MaximumMetadataChunkSize) return null;
                destination.Write(buffer, 0, count);
            }
            return destination.ToArray();
        }
        catch (InvalidDataException)
        {
            return null;
        }
    }

    private static byte[] ReadGifSubBlocks(Stream stream)
    {
        using MemoryStream result = new();
        while (true)
        {
            int size = stream.ReadByte();
            if (size <= 0) break;
            if (result.Length + size > MaximumMetadataChunkSize)
            {
                stream.Position += size;
                SkipGifSubBlocks(stream);
                break;
            }
            byte[] buffer = new byte[size];
            stream.ReadExactly(buffer);
            result.Write(buffer);
        }
        return result.ToArray();
    }

    private static void SkipGifSubBlocks(Stream stream)
    {
        while (true)
        {
            int size = stream.ReadByte();
            if (size <= 0) return;
            stream.Position += size;
        }
    }

    private static bool TryReadJpegMarker(Stream stream, out byte marker)
    {
        marker = 0;
        int value;
        do
        {
            value = stream.ReadByte();
            if (value < 0) return false;
        }
        while (value != 0xFF);

        do
        {
            value = stream.ReadByte();
            if (value < 0) return false;
        }
        while (value == 0xFF);
        marker = (byte)value;
        return marker != 0;
    }

    private static bool IsStartOfFrame(byte marker) => marker is
        0xC0 or 0xC1 or 0xC2 or 0xC3 or 0xC5 or 0xC6 or 0xC7 or 0xC9 or 0xCA or 0xCB or 0xCD or 0xCE or 0xCF;

    private static string GetJpegEncoding(byte marker) => marker switch
    {
        0xC0 => "Baseline DCT, Huffman coding",
        0xC1 => "Extended sequential DCT, Huffman coding",
        0xC2 => "Progressive DCT, Huffman coding",
        0xC3 => "Lossless, Huffman coding",
        0xC5 => "Differential sequential DCT, Huffman coding",
        0xC6 => "Differential progressive DCT, Huffman coding",
        0xC7 => "Differential lossless, Huffman coding",
        0xC9 => "Extended sequential DCT, arithmetic coding",
        0xCA => "Progressive DCT, arithmetic coding",
        0xCB => "Lossless, arithmetic coding",
        _ => $"SOF marker 0x{marker:X2}"
    };

    private static string GetPngColorType(byte value) => value switch
    {
        0 => "Grayscale", 2 => "Truecolor", 3 => "Indexed color", 4 => "Grayscale with alpha",
        6 => "Truecolor with alpha", _ => $"Unknown ({value})"
    };

    private static string GetRenderingIntent(byte value) => value switch
    {
        0 => "Perceptual", 1 => "Relative colorimetric", 2 => "Saturation", 3 => "Absolute colorimetric", _ => value.ToString()
    };

    private static string GetBmpCompression(uint value) => value switch
    {
        0 => "Uncompressed", 1 => "RLE 8-bit", 2 => "RLE 4-bit", 3 => "Bit fields", 4 => "JPEG",
        5 => "PNG", 6 => "Alpha bit fields", _ => $"Unknown ({value})"
    };

    private static string ReadPngPoint(ReadOnlySpan<byte> data) =>
        $"{BinaryPrimitives.ReadUInt32BigEndian(data) / 100000d:0.#####}, {BinaryPrimitives.ReadUInt32BigEndian(data[4..]) / 100000d:0.#####}";

    private static string DecodeText(ReadOnlySpan<byte> data)
    {
        data = data.TrimEnd((byte)0);
        if (data.IsEmpty) return string.Empty;
        try
        {
            return new UTF8Encoding(false, true).GetString(data);
        }
        catch (DecoderFallbackException)
        {
            return Encoding.Latin1.GetString(data);
        }
    }

    private static string FormatDuration(double seconds) => TimeSpan.FromSeconds(seconds).ToString(seconds >= 3600 ? @"h\:mm\:ss\.fff" : @"m\:ss\.fff");

    private static FileStream OpenRead(string filePath) =>
        new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 64 * 1024, FileOptions.SequentialScan);

    private static bool TryReadExactly(Stream stream, Span<byte> buffer)
    {
        int offset = 0;
        while (offset < buffer.Length)
        {
            int read = stream.Read(buffer[offset..]);
            if (read == 0) return false;
            offset += read;
        }
        return true;
    }
}

internal static class XmpMetadataReader
{
    public static void Read(ReadOnlySpan<byte> data, MetadataCollector metadata)
    {
        if (data.IsEmpty) return;
        int start = data.IndexOf((byte)'<');
        int end = data.LastIndexOf((byte)'>');
        if (start < 0 || end <= start) return;

        try
        {
            using MemoryStream stream = new(data.Slice(start, end - start + 1).ToArray());
            using XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                MaxCharactersInDocument = 16 * 1024 * 1024
            });
            XDocument document = XDocument.Load(reader, LoadOptions.None);
            int count = 0;
            foreach (XAttribute attribute in document.Descendants().Attributes())
            {
                if (attribute.IsNamespaceDeclaration || IsStructuralXmpName(attribute.Name)) continue;
                metadata.Add("XMP", FriendlyXmlName(attribute.Name), attribute.Value);
                if (++count >= 256) return;
            }

            foreach (XElement element in document.Descendants().Where(x => !x.HasElements))
            {
                if (IsStructuralXmpName(element.Name)) continue;
                string value = element.Value.Trim();
                if (value.Length == 0) continue;
                XAttribute? language = element.Attribute(XNamespace.Xml + "lang");
                string name = FriendlyXmlName(element.Parent?.Name.LocalName == "Alt" ? element.Parent.Parent?.Name ?? element.Name : element.Name);
                if (language != null) name += $" ({language.Value})";
                metadata.Add("XMP", name, value);
                if (++count >= 256) return;
            }
        }
        catch (XmlException)
        {
        }
    }

    private static bool IsStructuralXmpName(XName name) => name.LocalName is
        "about" or "parseType" or "Description" or "RDF" or "Alt" or "Bag" or "Seq" or "li";

    private static string FriendlyXmlName(XName name)
    {
        string prefix = name.NamespaceName switch
        {
            "http://purl.org/dc/elements/1.1/" => "Dublin Core",
            "http://ns.adobe.com/xap/1.0/" => "XMP",
            "http://ns.adobe.com/photoshop/1.0/" => "Photoshop",
            "http://ns.adobe.com/exif/1.0/" => "EXIF",
            "http://ns.adobe.com/tiff/1.0/" => "TIFF",
            _ => string.Empty
        };
        return string.IsNullOrEmpty(prefix) ? name.LocalName : $"{prefix} {name.LocalName}";
    }
}
