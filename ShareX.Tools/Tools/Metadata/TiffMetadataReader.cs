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

internal static class TiffMetadataReader
{
    private const long MaximumTiffReadSize = 256L * 1024 * 1024;

    public static void ReadFile(string filePath, MetadataCollector metadata, CancellationToken cancellationToken)
    {
        FileInfo file = new(filePath);
        if (file.Length > MaximumTiffReadSize)
        {
            metadata.Add("TIFF", "Metadata Notice", "The TIFF is too large for the built-in metadata reader.");
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();
        Read(File.ReadAllBytes(filePath), metadata, "TIFF");
    }

    public static void Read(byte[] data, MetadataCollector metadata, string rootGroup)
    {
        if (data.Length < 8) return;
        bool littleEndian;
        if (data.AsSpan(0, 2).SequenceEqual("II"u8)) littleEndian = true;
        else if (data.AsSpan(0, 2).SequenceEqual("MM"u8)) littleEndian = false;
        else return;

        TiffParser parser = new(data, littleEndian, metadata, rootGroup);
        parser.Read();
    }

    private sealed class TiffParser(byte[] data, bool littleEndian, MetadataCollector metadata, string rootGroup)
    {
        private readonly HashSet<uint> _visitedOffsets = [];
        private readonly Dictionary<ushort, object> _gps = [];

        public void Read()
        {
            if (ReadUInt16(2) != 42) return;
            uint firstIfd = ReadUInt32(4);
            metadata.Add(rootGroup, "Byte Order", littleEndian ? "Little-endian (Intel)" : "Big-endian (Motorola)");
            uint next = ReadIfd(firstIfd, rootGroup, 0);
            if (next > 0) ReadIfd(next, "Thumbnail", 0);
            AddGpsPosition();
        }

        private uint ReadIfd(uint offset, string group, int depth)
        {
            if (depth > 8 || offset > data.Length - 2 || !_visitedOffsets.Add(offset)) return 0;
            int position = (int)offset;
            int count = Math.Min(ReadUInt16(position), (ushort)4096);
            position += 2;
            if (position + (long)count * 12 > data.Length) return 0;

            List<(string Group, uint Offset)> childIfds = [];
            for (int index = 0; index < count; index++, position += 12)
            {
                ushort tag = ReadUInt16(position);
                ushort type = ReadUInt16(position + 2);
                uint valueCount = ReadUInt32(position + 4);
                object? rawValue = ReadValue(position, type, valueCount);
                if (rawValue == null) continue;

                if (tag == 0x8769 && TryGetOffset(rawValue, out uint exifOffset))
                {
                    childIfds.Add(("EXIF", exifOffset));
                    continue;
                }
                if (tag == 0x8825 && TryGetOffset(rawValue, out uint gpsOffset))
                {
                    childIfds.Add(("GPS", gpsOffset));
                    continue;
                }
                if (tag == 0xA005 && TryGetOffset(rawValue, out uint interoperabilityOffset))
                {
                    childIfds.Add(("Interoperability", interoperabilityOffset));
                    continue;
                }
                if (tag == 0x014A && rawValue is ulong[] subIfds)
                {
                    int child = 1;
                    foreach (ulong subIfd in subIfds.Take(16))
                    {
                        if (subIfd <= uint.MaxValue) childIfds.Add(($"SubIFD {child++}", (uint)subIfd));
                    }
                    continue;
                }

                if (group == "GPS") _gps[tag] = rawValue;

                if (tag == 0x02BC && rawValue is byte[] xmp)
                {
                    XmpMetadataReader.Read(xmp, metadata);
                    continue;
                }
                if (tag == 0x8773 && rawValue is byte[] icc)
                {
                    IccMetadataReader.Read(icc, metadata);
                    continue;
                }

                string? name = GetTagName(group, tag);
                if (name == null) continue;
                string? value = FormatValue(group, tag, rawValue);
                if (value != null) metadata.Add(group, name, value);
            }

            foreach ((string childGroup, uint childOffset) in childIfds)
            {
                ReadIfd(childOffset, childGroup, depth + 1);
            }

            int nextPosition = (int)offset + 2 + count * 12;
            return nextPosition <= data.Length - 4 ? ReadUInt32(nextPosition) : 0;
        }

        private object? ReadValue(int entryPosition, ushort type, uint count)
        {
            int componentSize = type switch
            {
                1 or 2 or 6 or 7 => 1,
                3 or 8 => 2,
                4 or 9 or 11 or 13 => 4,
                5 or 10 or 12 => 8,
                _ => 0
            };
            if (componentSize == 0 || count == 0 || count > 1_000_000) return null;
            long byteCount = (long)componentSize * count;
            if (byteCount > 32 * 1024 * 1024) return null;
            long valueOffset = byteCount <= 4 ? entryPosition + 8 : ReadUInt32(entryPosition + 8);
            if (valueOffset < 0 || valueOffset + byteCount > data.Length) return null;
            int offset = (int)valueOffset;

            switch (type)
            {
                case 1:
                case 6:
                case 7:
                    return data.AsSpan(offset, (int)byteCount).ToArray();
                case 2:
                    return DecodeText(data.AsSpan(offset, (int)byteCount));
                case 3:
                case 13:
                {
                    ulong[] values = new ulong[count];
                    for (int index = 0; index < values.Length; index++) values[index] = ReadUInt16(offset + index * 2);
                    return values;
                }
                case 4:
                {
                    ulong[] values = new ulong[count];
                    for (int index = 0; index < values.Length; index++) values[index] = ReadUInt32(offset + index * 4);
                    return values;
                }
                case 5:
                {
                    double[] values = new double[count];
                    for (int index = 0; index < values.Length; index++)
                    {
                        uint numerator = ReadUInt32(offset + index * 8);
                        uint denominator = ReadUInt32(offset + index * 8 + 4);
                        values[index] = denominator == 0 ? double.NaN : (double)numerator / denominator;
                    }
                    return values;
                }
                case 8:
                {
                    long[] values = new long[count];
                    for (int index = 0; index < values.Length; index++) values[index] = ReadInt16(offset + index * 2);
                    return values;
                }
                case 9:
                {
                    long[] values = new long[count];
                    for (int index = 0; index < values.Length; index++) values[index] = ReadInt32(offset + index * 4);
                    return values;
                }
                case 10:
                {
                    double[] values = new double[count];
                    for (int index = 0; index < values.Length; index++)
                    {
                        int numerator = ReadInt32(offset + index * 8);
                        int denominator = ReadInt32(offset + index * 8 + 4);
                        values[index] = denominator == 0 ? double.NaN : (double)numerator / denominator;
                    }
                    return values;
                }
                case 11:
                {
                    double[] values = new double[count];
                    for (int index = 0; index < values.Length; index++) values[index] = ReadSingle(offset + index * 4);
                    return values;
                }
                case 12:
                {
                    double[] values = new double[count];
                    for (int index = 0; index < values.Length; index++) values[index] = ReadDouble(offset + index * 8);
                    return values;
                }
            }
            return null;
        }

        private string? FormatValue(string group, ushort tag, object value)
        {
            if (value is string text) return text;
            if (value is byte[] bytes)
            {
                if (tag is >= 0x9C9B and <= 0x9C9F) return Encoding.Unicode.GetString(bytes).TrimEnd('\0');
                if (tag == 0x9286) return DecodeUserComment(bytes);
                if (tag == 0x927C) return $"{bytes.Length:N0} bytes";
                if (tag is 0x9000 or 0xA000) return DecodeVersion(bytes);
                if (tag is 0x9101 or 0xA300 or 0xA301) return string.Join(' ', bytes);
                return bytes.Length <= 16 ? Convert.ToHexString(bytes) : $"{bytes.Length:N0} bytes";
            }

            double[]? numbers = value switch
            {
                double[] doubles => doubles,
                ulong[] unsigned => unsigned.Select(x => (double)x).ToArray(),
                long[] signed => signed.Select(x => (double)x).ToArray(),
                _ => null
            };
            if (numbers == null || numbers.Length == 0) return null;
            double first = numbers[0];

            if (group == "GPS")
            {
                return tag switch
                {
                    0x0000 => value is byte[] version ? string.Join('.', version) : FormatNumbers(numbers),
                    0x0002 or 0x0004 => FormatDegrees(numbers),
                    0x0006 => $"{first:0.###} m",
                    0x0007 => FormatGpsTime(numbers),
                    0x000D => $"{first:0.###} km/h",
                    0x000F or 0x0011 or 0x0018 => $"{first:0.###}°",
                    _ => FormatNumbers(numbers)
                };
            }

            return tag switch
            {
                0x0103 => GetCompression((int)first),
                0x0106 => GetPhotometricInterpretation((int)first),
                0x0112 => GetOrientation((int)first),
                0x011A or 0x011B => $"{first:0.###}",
                0x0128 => first switch { 1 => "None", 2 => "inches", 3 => "centimeters", _ => first.ToString("0") },
                0x829A => FormatExposure(first),
                0x829D => $"f/{first:0.0#}",
                0x8822 => GetExposureProgram((int)first),
                0x8827 or 0x8833 => FormatNumbers(numbers),
                0x9201 => $"{first:0.###} EV",
                0x9202 or 0x9205 => $"f/{Math.Pow(2, first / 2):0.0#}",
                0x9204 => $"{first:+0.###;-0.###;0} EV",
                0x9206 => $"{first:0.###} m",
                0x9207 => GetMeteringMode((int)first),
                0x9208 => GetLightSource((int)first),
                0x9209 => GetFlash((int)first),
                0x920A => $"{first:0.###} mm",
                0xA001 => first switch { 1 => "sRGB", 2 => "Adobe RGB", 0xFFFF => "Uncalibrated", _ => first.ToString("0") },
                0xA402 => first switch { 0 => "Auto", 1 => "Manual", 2 => "Auto bracket", _ => first.ToString("0") },
                0xA403 => first switch { 0 => "Auto", 1 => "Manual", _ => first.ToString("0") },
                0xA405 => $"{first:0} mm",
                0xA406 => first switch { 0 => "Standard", 1 => "Landscape", 2 => "Portrait", 3 => "Night", _ => first.ToString("0") },
                0xA432 when numbers.Length >= 4 => $"{numbers[0]:0.###}-{numbers[1]:0.###} mm f/{numbers[2]:0.0#}-{numbers[3]:0.0#}",
                _ => FormatNumbers(numbers)
            };
        }

        private void AddGpsPosition()
        {
            if (!TryGetString(_gps, 0x0001, out string? latitudeRef) ||
                !TryGetNumbers(_gps, 0x0002, out double[]? latitude) || latitude.Length < 3 ||
                !TryGetString(_gps, 0x0003, out string? longitudeRef) ||
                !TryGetNumbers(_gps, 0x0004, out double[]? longitude) || longitude.Length < 3)
            {
                return;
            }

            double latitudeValue = latitude[0] + latitude[1] / 60 + latitude[2] / 3600;
            double longitudeValue = longitude[0] + longitude[1] / 60 + longitude[2] / 3600;
            if (latitudeRef.Equals("S", StringComparison.OrdinalIgnoreCase)) latitudeValue = -latitudeValue;
            if (longitudeRef.Equals("W", StringComparison.OrdinalIgnoreCase)) longitudeValue = -longitudeValue;
            metadata.Add("GPS", "GPS Position", $"{latitudeValue:0.######}, {longitudeValue:0.######}");
            metadata.Add("GPS", "Map", $"https://www.openstreetmap.org/?mlat={latitudeValue:0.######}&mlon={longitudeValue:0.######}#map=16/{latitudeValue:0.######}/{longitudeValue:0.######}");
        }

        private static string? GetTagName(string group, ushort tag)
        {
            if (group == "GPS")
            {
                return tag switch
                {
                    0x0000 => "GPS Version", 0x0001 => "Latitude Reference", 0x0002 => "Latitude",
                    0x0003 => "Longitude Reference", 0x0004 => "Longitude", 0x0005 => "Altitude Reference",
                    0x0006 => "Altitude", 0x0007 => "Time Stamp", 0x0008 => "Satellites", 0x0009 => "Status",
                    0x000A => "Measure Mode", 0x000B => "DOP", 0x000C => "Speed Reference", 0x000D => "Speed",
                    0x000E => "Track Reference", 0x000F => "Track", 0x0010 => "Image Direction Reference",
                    0x0011 => "Image Direction", 0x0012 => "Map Datum", 0x001B => "Processing Method",
                    0x001D => "Date Stamp", 0x001F => "Horizontal Positioning Error", _ => null
                };
            }

            return tag switch
            {
                0x00FE => "Subfile Type", 0x0100 => "Image Width", 0x0101 => "Image Height",
                0x0102 => "Bits Per Sample", 0x0103 => "Compression", 0x0106 => "Photometric Interpretation",
                0x010E => "Image Description", 0x010F => "Make", 0x0110 => "Camera Model Name",
                0x0112 => "Orientation", 0x0115 => "Samples Per Pixel", 0x0116 => "Rows Per Strip",
                0x011A => "X Resolution", 0x011B => "Y Resolution", 0x011C => "Planar Configuration",
                0x0128 => "Resolution Unit", 0x0131 => "Software", 0x0132 => "Modify Date", 0x013B => "Artist",
                0x013C => "Host Computer", 0x0201 => "Thumbnail Offset", 0x0202 => "Thumbnail Length",
                0x8298 => "Copyright", 0x829A => "Exposure Time", 0x829D => "F Number",
                0x8822 => "Exposure Program", 0x8827 => "ISO", 0x8830 => "Sensitivity Type", 0x8832 => "Recommended Exposure Index",
                0x8833 => "ISO Speed", 0x9000 => "EXIF Version", 0x9003 => "Date/Time Original", 0x9004 => "Create Date",
                0x9010 => "Offset Time", 0x9011 => "Offset Time Original", 0x9012 => "Offset Time Digitized",
                0x9101 => "Components Configuration", 0x9201 => "Shutter Speed", 0x9202 => "Aperture",
                0x9204 => "Exposure Compensation", 0x9205 => "Max Aperture Value", 0x9206 => "Subject Distance",
                0x9207 => "Metering Mode", 0x9208 => "Light Source", 0x9209 => "Flash", 0x920A => "Focal Length",
                0x927C => "Maker Note", 0x9286 => "User Comment", 0x9290 => "Sub Sec Time", 0x9291 => "Sub Sec Time Original",
                0x9292 => "Sub Sec Time Digitized", 0x9C9B => "XP Title", 0x9C9C => "XP Comment", 0x9C9D => "XP Author",
                0x9C9E => "XP Keywords", 0x9C9F => "XP Subject", 0xA000 => "Flashpix Version", 0xA001 => "Color Space",
                0xA002 => "EXIF Image Width", 0xA003 => "EXIF Image Height", 0xA20E => "Focal Plane X Resolution",
                0xA20F => "Focal Plane Y Resolution", 0xA210 => "Focal Plane Resolution Unit", 0xA217 => "Sensing Method",
                0xA300 => "File Source", 0xA301 => "Scene Type", 0xA401 => "Custom Rendered", 0xA402 => "Exposure Mode",
                0xA403 => "White Balance", 0xA404 => "Digital Zoom Ratio", 0xA405 => "Focal Length In 35mm Format",
                0xA406 => "Scene Capture Type", 0xA407 => "Gain Control", 0xA408 => "Contrast", 0xA409 => "Saturation",
                0xA40A => "Sharpness", 0xA40C => "Subject Distance Range", 0xA420 => "Image Unique ID",
                0xA430 => "Camera Owner Name", 0xA431 => "Body Serial Number", 0xA432 => "Lens Specification",
                0xA433 => "Lens Make", 0xA434 => "Lens Model", 0xA435 => "Lens Serial Number", _ => null
            };
        }

        private static bool TryGetOffset(object value, out uint offset)
        {
            if (value is ulong[] { Length: > 0 } values && values[0] <= uint.MaxValue)
            {
                offset = (uint)values[0];
                return true;
            }
            offset = 0;
            return false;
        }

        private static bool TryGetString(Dictionary<ushort, object> values, ushort tag, out string result)
        {
            if (values.TryGetValue(tag, out object? value))
            {
                result = value switch
                {
                    string text => text,
                    byte[] bytes => DecodeText(bytes),
                    _ => string.Empty
                };
                return result.Length > 0;
            }
            result = string.Empty;
            return false;
        }

        private static bool TryGetNumbers(Dictionary<ushort, object> values, ushort tag, out double[] result)
        {
            if (values.TryGetValue(tag, out object? value))
            {
                result = value switch
                {
                    double[] doubles => doubles,
                    ulong[] unsigned => unsigned.Select(x => (double)x).ToArray(),
                    long[] signed => signed.Select(x => (double)x).ToArray(),
                    _ => []
                };
                return result.Length > 0;
            }
            result = [];
            return false;
        }

        private static string FormatNumbers(double[] values) => string.Join(", ", values.Select(x => double.IsNaN(x) ? "undefined" : x.ToString("0.#####")));
        private static string FormatDegrees(double[] values) => values.Length >= 3 ? $"{values[0]:0}° {values[1]:0}' {values[2]:0.###}\"" : FormatNumbers(values);
        private static string FormatGpsTime(double[] values) => values.Length >= 3 ? $"{values[0]:00}:{values[1]:00}:{values[2]:00.###} UTC" : FormatNumbers(values);
        private static string FormatExposure(double value) => value > 0 && value < 1 ? $"1/{Math.Round(1 / value):0} s" : $"{value:0.###} s";

        private static string DecodeVersion(byte[] bytes)
        {
            string value = Encoding.ASCII.GetString(bytes).TrimEnd('\0');
            return value.Length == 4 ? $"{value[0]}.{value[1..]}" : value;
        }

        private static string DecodeUserComment(byte[] bytes)
        {
            if (bytes.Length <= 8) return DecodeText(bytes);
            string encoding = Encoding.ASCII.GetString(bytes, 0, 8).TrimEnd('\0', ' ');
            ReadOnlySpan<byte> value = bytes.AsSpan(8);
            return encoding switch
            {
                "UNICODE" => Encoding.BigEndianUnicode.GetString(value).TrimEnd('\0'),
                "ASCII" => DecodeText(value),
                _ => DecodeText(bytes)
            };
        }

        private static string DecodeText(ReadOnlySpan<byte> bytes)
        {
            bytes = bytes.TrimEnd((byte)0);
            try { return new UTF8Encoding(false, true).GetString(bytes); }
            catch (DecoderFallbackException) { return Encoding.Latin1.GetString(bytes); }
        }

        private static string GetCompression(int value) => value switch
        {
            1 => "Uncompressed", 2 => "CCITT 1D", 3 => "Group 3 Fax", 4 => "Group 4 Fax", 5 => "LZW",
            6 => "Old JPEG", 7 => "JPEG", 8 => "Deflate", 32773 => "PackBits", 34712 => "JPEG 2000", _ => $"Unknown ({value})"
        };

        private static string GetPhotometricInterpretation(int value) => value switch
        {
            0 => "WhiteIsZero", 1 => "BlackIsZero", 2 => "RGB", 3 => "Palette", 4 => "Transparency mask",
            5 => "CMYK", 6 => "YCbCr", 8 => "CIELab", _ => $"Unknown ({value})"
        };

        private static string GetOrientation(int value) => value switch
        {
            1 => "Horizontal (normal)", 2 => "Mirror horizontal", 3 => "Rotate 180", 4 => "Mirror vertical",
            5 => "Mirror horizontal and rotate 270 CW", 6 => "Rotate 90 CW", 7 => "Mirror horizontal and rotate 90 CW",
            8 => "Rotate 270 CW", _ => $"Unknown ({value})"
        };

        private static string GetExposureProgram(int value) => value switch
        {
            0 => "Not defined", 1 => "Manual", 2 => "Program AE", 3 => "Aperture-priority AE", 4 => "Shutter-priority AE",
            5 => "Creative", 6 => "Action", 7 => "Portrait", 8 => "Landscape", _ => value.ToString()
        };

        private static string GetMeteringMode(int value) => value switch
        {
            0 => "Unknown", 1 => "Average", 2 => "Center-weighted average", 3 => "Spot", 4 => "Multi-spot",
            5 => "Multi-segment", 6 => "Partial", 255 => "Other", _ => value.ToString()
        };

        private static string GetLightSource(int value) => value switch
        {
            0 => "Unknown", 1 => "Daylight", 2 => "Fluorescent", 3 => "Tungsten", 4 => "Flash", 9 => "Fine weather",
            10 => "Cloudy", 11 => "Shade", 12 => "Daylight fluorescent", 13 => "Day white fluorescent",
            14 => "Cool white fluorescent", 15 => "White fluorescent", 17 => "Standard light A", 18 => "Standard light B",
            19 => "Standard light C", 20 => "D55", 21 => "D65", 22 => "D75", 23 => "D50", 24 => "ISO studio tungsten",
            255 => "Other", _ => value.ToString()
        };

        private static string GetFlash(int value)
        {
            string fired = (value & 1) != 0 ? "Fired" : "Did not fire";
            int returnStatus = (value >> 1) & 3;
            string returned = returnStatus switch { 2 => ", return not detected", 3 => ", return detected", _ => string.Empty };
            return fired + returned;
        }

        private ushort ReadUInt16(int offset) => littleEndian
            ? BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(offset, 2))
            : BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(offset, 2));

        private short ReadInt16(int offset) => littleEndian
            ? BinaryPrimitives.ReadInt16LittleEndian(data.AsSpan(offset, 2))
            : BinaryPrimitives.ReadInt16BigEndian(data.AsSpan(offset, 2));

        private uint ReadUInt32(int offset) => littleEndian
            ? BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4))
            : BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(offset, 4));

        private int ReadInt32(int offset) => littleEndian
            ? BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(offset, 4))
            : BinaryPrimitives.ReadInt32BigEndian(data.AsSpan(offset, 4));

        private float ReadSingle(int offset)
        {
            int bits = ReadInt32(offset);
            return BitConverter.Int32BitsToSingle(bits);
        }

        private double ReadDouble(int offset)
        {
            long bits = littleEndian
                ? BinaryPrimitives.ReadInt64LittleEndian(data.AsSpan(offset, 8))
                : BinaryPrimitives.ReadInt64BigEndian(data.AsSpan(offset, 8));
            return BitConverter.Int64BitsToDouble(bits);
        }
    }
}

internal static class IccMetadataReader
{
    public static void Read(byte[] profile, MetadataCollector metadata)
    {
        if (profile.Length < 128) return;
        ReadOnlySpan<byte> data = profile;
        uint declaredSize = BinaryPrimitives.ReadUInt32BigEndian(data);
        metadata.Add("ICC Profile", "Profile Size", $"{declaredSize:N0} bytes");
        metadata.Add("ICC Profile", "CMM Type", FourCc(data[4..8]));
        metadata.Add("ICC Profile", "Profile Version", $"{data[8] >> 4}.{data[8] & 15}.{data[9] >> 4}");
        metadata.Add("ICC Profile", "Profile Class", GetProfileClass(FourCc(data[12..16])));
        metadata.Add("ICC Profile", "Color Space", FourCc(data[16..20]));
        metadata.Add("ICC Profile", "Connection Space", FourCc(data[20..24]));
        TryAddCreationDate(data, metadata);
        metadata.Add("ICC Profile", "Primary Platform", FourCc(data[40..44]));
        metadata.Add("ICC Profile", "Device Manufacturer", FourCc(data[48..52]));
        metadata.Add("ICC Profile", "Device Model", FourCc(data[52..56]));
        metadata.Add("ICC Profile", "Rendering Intent", GetRenderingIntent(BinaryPrimitives.ReadUInt32BigEndian(data[64..68])));
        metadata.Add("ICC Profile", "Profile Creator", FourCc(data[80..84]));

        if (profile.Length < 132) return;
        int count = (int)Math.Min(BinaryPrimitives.ReadUInt32BigEndian(data[128..]), 1024u);
        for (int index = 0; index < count; index++)
        {
            int position = 132 + index * 12;
            if (position + 12 > data.Length) break;
            string signature = FourCc(data.Slice(position, 4));
            uint offset = BinaryPrimitives.ReadUInt32BigEndian(data[(position + 4)..]);
            uint size = BinaryPrimitives.ReadUInt32BigEndian(data[(position + 8)..]);
            if (size > int.MaxValue || offset + (long)size > data.Length || size < 8) continue;
            ReadOnlySpan<byte> tag = data.Slice((int)offset, (int)size);
            string? value = ReadIccText(tag);
            if (value != null)
            {
                metadata.Add("ICC Profile", signature switch
                {
                    "desc" => "Profile Description", "cprt" => "Copyright", "dmnd" => "Device Manufacturer Description",
                    "dmdd" => "Device Model Description", "vued" => "Viewing Conditions Description", _ => signature
                }, value);
            }
        }
    }

    private static string? ReadIccText(ReadOnlySpan<byte> tag)
    {
        string type = FourCc(tag[..4]);
        if (type == "desc" && tag.Length >= 12)
        {
            uint length = BinaryPrimitives.ReadUInt32BigEndian(tag[8..]);
            if (length > 0 && length <= tag.Length - 12) return Encoding.Latin1.GetString(tag.Slice(12, (int)length)).TrimEnd('\0');
        }
        if (type == "text" && tag.Length > 8) return Encoding.ASCII.GetString(tag[8..]).TrimEnd('\0');
        if (type == "mluc" && tag.Length >= 28)
        {
            uint count = BinaryPrimitives.ReadUInt32BigEndian(tag[8..]);
            if (count == 0) return null;
            uint length = BinaryPrimitives.ReadUInt32BigEndian(tag[20..]);
            uint offset = BinaryPrimitives.ReadUInt32BigEndian(tag[24..]);
            if (length <= int.MaxValue && offset + (long)length <= tag.Length)
            {
                return Encoding.BigEndianUnicode.GetString(tag.Slice((int)offset, (int)length));
            }
        }
        return null;
    }

    private static void TryAddCreationDate(ReadOnlySpan<byte> data, MetadataCollector metadata)
    {
        try
        {
            DateTime date = new(
                BinaryPrimitives.ReadUInt16BigEndian(data[24..]), BinaryPrimitives.ReadUInt16BigEndian(data[26..]),
                BinaryPrimitives.ReadUInt16BigEndian(data[28..]), BinaryPrimitives.ReadUInt16BigEndian(data[30..]),
                BinaryPrimitives.ReadUInt16BigEndian(data[32..]), BinaryPrimitives.ReadUInt16BigEndian(data[34..]), DateTimeKind.Utc);
            metadata.Add("ICC Profile", "Profile Date/Time", date);
        }
        catch (ArgumentOutOfRangeException)
        {
        }
    }

    private static string FourCc(ReadOnlySpan<byte> value) => Encoding.ASCII.GetString(value).TrimEnd('\0', ' ');
    private static string GetProfileClass(string value) => value switch
    {
        "scnr" => "Input device", "mntr" => "Display device", "prtr" => "Output device", "link" => "Device link",
        "spac" => "Color space conversion", "abst" => "Abstract", "nmcl" => "Named color", _ => value
    };
    private static string GetRenderingIntent(uint value) => value switch
    {
        0 => "Perceptual", 1 => "Media-relative colorimetric", 2 => "Saturation", 3 => "ICC-absolute colorimetric", _ => value.ToString()
    };
}
