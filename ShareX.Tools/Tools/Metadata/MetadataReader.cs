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

using System.Globalization;

namespace ShareX.Tools;

internal enum MetadataFormat
{
    Unknown,
    Jpeg,
    Png,
    Gif,
    Bmp,
    Tiff,
    WebP,
    Icon,
    IsoBaseMedia,
    Matroska,
    Avi,
    Asf
}

internal static class MetadataFormatExtensions
{
    public static bool CanStripMetadata(this MetadataFormat format) => format is
        MetadataFormat.Jpeg or MetadataFormat.Png or MetadataFormat.Gif or MetadataFormat.WebP or
        MetadataFormat.IsoBaseMedia or MetadataFormat.Matroska or MetadataFormat.Avi or MetadataFormat.Asf;
}

internal static class MetadataFormatDetector
{
    private static readonly byte[] AsfHeaderGuid =
        [0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11, 0xA6, 0xD9, 0x00, 0xAA, 0x00, 0x62, 0xCE, 0x6C];

    public static MetadataFormat Detect(string filePath)
    {
        Span<byte> header = stackalloc byte[32];
        using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        int count = stream.Read(header);
        ReadOnlySpan<byte> data = header[..count];

        if (data.StartsWith(new byte[] { 0xFF, 0xD8 })) return MetadataFormat.Jpeg;
        if (data.StartsWith(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })) return MetadataFormat.Png;
        if (data.StartsWith("GIF87a"u8) || data.StartsWith("GIF89a"u8)) return MetadataFormat.Gif;
        if (data.StartsWith("BM"u8)) return MetadataFormat.Bmp;
        if (data.StartsWith(new byte[] { 0x49, 0x49, 0x2A, 0x00 }) || data.StartsWith(new byte[] { 0x4D, 0x4D, 0x00, 0x2A })) return MetadataFormat.Tiff;
        if (data.Length >= 12 && data[..4].SequenceEqual("RIFF"u8) && data.Slice(8, 4).SequenceEqual("WEBP"u8)) return MetadataFormat.WebP;
        if (data.Length >= 12 && data[..4].SequenceEqual("RIFF"u8) && data.Slice(8, 4).SequenceEqual("AVI "u8)) return MetadataFormat.Avi;
        if (data.StartsWith(new byte[] { 0x00, 0x00, 0x01, 0x00 }) || data.StartsWith(new byte[] { 0x00, 0x00, 0x02, 0x00 })) return MetadataFormat.Icon;
        if (data.StartsWith(new byte[] { 0x1A, 0x45, 0xDF, 0xA3 })) return MetadataFormat.Matroska;
        if (data.StartsWith(AsfHeaderGuid)) return MetadataFormat.Asf;
        if (data.Length >= 12 && data.Slice(4, 4).SequenceEqual("ftyp"u8)) return MetadataFormat.IsoBaseMedia;

        string extension = Path.GetExtension(filePath);
        if (extension.Equals(".mov", StringComparison.OrdinalIgnoreCase) && data.Length >= 8)
        {
            ReadOnlySpan<byte> boxType = data.Slice(4, 4);
            if (boxType.SequenceEqual("moov"u8) || boxType.SequenceEqual("mdat"u8) ||
                boxType.SequenceEqual("wide"u8) || boxType.SequenceEqual("free"u8))
            {
                return MetadataFormat.IsoBaseMedia;
            }
        }

        return MetadataFormat.Unknown;
    }
}

internal sealed class MetadataCollector
{
    private const int MaximumEntries = 2048;
    private readonly List<MetadataValue> _items = [];
    private readonly HashSet<string> _keys = new(StringComparer.Ordinal);

    public IReadOnlyList<MetadataValue> Items => _items;

    public void Add(string group, string tag, object? value)
    {
        if (_items.Count >= MaximumEntries || value == null)
        {
            return;
        }

        string text = value switch
        {
            DateTime dateTime => dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff K", CultureInfo.InvariantCulture),
            DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("yyyy-MM-dd HH:mm:ss.fff zzz", CultureInfo.InvariantCulture),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty,
            _ => value.ToString() ?? string.Empty
        };
        text = Sanitize(text);
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        string key = string.Concat(group, "\0", tag, "\0", text);
        if (_keys.Add(key))
        {
            _items.Add(new MetadataValue(group, tag, text));
        }
    }

    public void AddSize(string group, int width, int height)
    {
        if (width > 0 && height > 0)
        {
            Add(group, "Image Width", $"{width} px");
            Add(group, "Image Height", $"{height} px");
            Add(group, "Image Size", $"{width} x {height}");
        }
    }

    private static string Sanitize(string value)
    {
        Span<char> buffer = value.Length <= 1024 ? stackalloc char[value.Length] : new char[value.Length];
        int length = 0;
        bool previousWhitespace = false;
        foreach (char character in value)
        {
            bool whitespace = char.IsWhiteSpace(character) || char.IsControl(character);
            if (whitespace)
            {
                if (!previousWhitespace)
                {
                    buffer[length++] = ' ';
                    previousWhitespace = true;
                }
            }
            else
            {
                buffer[length++] = character;
                previousWhitespace = false;
            }

            if (length == 4096)
            {
                break;
            }
        }
        return new string(buffer[..length]).Trim();
    }
}

internal static class MetadataReader
{
    public static IReadOnlyList<MetadataValue> Read(string filePath, CancellationToken cancellationToken)
    {
        MetadataFormat format = MetadataFormatDetector.Detect(filePath);
        FileInfo file = new(filePath);
        MetadataCollector metadata = new();

        metadata.Add("File", "File Name", file.Name);
        metadata.Add("File", "File Size", FormatFileSize(file.Length));
        metadata.Add("File", "File Type", GetFormatName(format, file.Extension));
        metadata.Add("File", "File Type Extension", file.Extension.TrimStart('.').ToUpperInvariant());
        metadata.Add("File", "MIME Type", GetMimeType(format, file.Extension));
        metadata.Add("File System", "Created", file.CreationTime);
        metadata.Add("File System", "Modified", file.LastWriteTime);

        cancellationToken.ThrowIfCancellationRequested();
        switch (format)
        {
            case MetadataFormat.Jpeg:
            case MetadataFormat.Png:
            case MetadataFormat.Gif:
            case MetadataFormat.Bmp:
            case MetadataFormat.Tiff:
            case MetadataFormat.WebP:
            case MetadataFormat.Icon:
                ImageMetadataReader.Read(filePath, format, metadata, cancellationToken);
                break;
            case MetadataFormat.IsoBaseMedia:
                IsoBaseMediaMetadataReader.Read(filePath, metadata, cancellationToken);
                break;
            case MetadataFormat.Matroska:
                MatroskaMetadataReader.Read(filePath, metadata, cancellationToken);
                break;
            case MetadataFormat.Avi:
                RiffMetadataReader.ReadAvi(filePath, metadata, cancellationToken);
                break;
            case MetadataFormat.Asf:
                AsfMetadataReader.Read(filePath, metadata, cancellationToken);
                break;
        }

        return metadata.Items;
    }

    private static string GetFormatName(MetadataFormat format, string extension) => format switch
    {
        MetadataFormat.Jpeg => "JPEG",
        MetadataFormat.Png => "PNG",
        MetadataFormat.Gif => "GIF",
        MetadataFormat.Bmp => "BMP",
        MetadataFormat.Tiff => "TIFF",
        MetadataFormat.WebP => "WebP",
        MetadataFormat.Icon => "Windows icon",
        MetadataFormat.IsoBaseMedia => extension.ToLowerInvariant() switch
        {
            ".mov" => "QuickTime",
            ".3gp" or ".3g2" => "3GPP",
            ".heic" or ".heif" => "HEIF",
            ".avif" => "AVIF",
            _ => "ISO Base Media"
        },
        MetadataFormat.Matroska => "Matroska / WebM",
        MetadataFormat.Avi => "AVI",
        MetadataFormat.Asf => "ASF / Windows Media",
        _ => "Unknown"
    };

    private static string GetMimeType(MetadataFormat format, string extension) => format switch
    {
        MetadataFormat.Jpeg => "image/jpeg",
        MetadataFormat.Png => "image/png",
        MetadataFormat.Gif => "image/gif",
        MetadataFormat.Bmp => "image/bmp",
        MetadataFormat.Tiff => "image/tiff",
        MetadataFormat.WebP => "image/webp",
        MetadataFormat.Icon => "image/x-icon",
        MetadataFormat.Matroska => "video/x-matroska",
        MetadataFormat.Avi => "video/x-msvideo",
        MetadataFormat.Asf => "video/x-ms-asf",
        MetadataFormat.IsoBaseMedia => extension.ToLowerInvariant() switch
        {
            ".mov" => "video/quicktime",
            ".3gp" => "video/3gpp",
            ".3g2" => "video/3gpp2",
            ".heic" or ".heif" => "image/heif",
            ".avif" => "image/avif",
            _ => "video/mp4"
        },
        _ => "application/octet-stream"
    };

    private static string FormatFileSize(long bytes)
    {
        string[] units = ["B", "KiB", "MiB", "GiB", "TiB"];
        double size = bytes;
        int unit = 0;
        while (size >= 1024 && unit < units.Length - 1)
        {
            size /= 1024;
            unit++;
        }
        return unit == 0 ? $"{bytes} B" : $"{size:0.##} {units[unit]} ({bytes:N0} bytes)";
    }
}
