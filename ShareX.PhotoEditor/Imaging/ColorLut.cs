#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using System.Globalization;

namespace ShareX.PhotoEditor.Imaging;

/// <summary>A 3D color lookup table loaded from a .cube or .3dl file. Stored inside the settings so documents stay self-contained.</summary>
public sealed class ColorLut
{
    private const int MaxSize = 129;
    private float[]? _table;

    public string Name { get; set; } = string.Empty;

    /// <summary>Number of grid points along each axis.</summary>
    public int Size { get; set; }

    /// <summary>RGB triples as little-endian 16-bit values; red changes fastest.</summary>
    public byte[] Data { get; set; } = [];

    public float[] DomainMin { get; set; } = [0, 0, 0];

    public float[] DomainMax { get; set; } = [1, 1, 1];

    public static ColorLut Load(string path)
    {
        string[] lines = File.ReadAllLines(path);
        ColorLut lut = Path.GetExtension(path).Equals(".3dl", StringComparison.OrdinalIgnoreCase) ? Parse3dl(lines) : ParseCube(lines);
        lut.Name = Path.GetFileName(path);
        return lut;
    }

    /// <summary>Maps a color through the table with trilinear interpolation.</summary>
    public void Sample(ref float r, ref float g, ref float b)
    {
        float[] table = _table ??= Decode();
        int size = Size, last = size - 1;
        float x = GridPosition(r, 0), y = GridPosition(g, 1), z = GridPosition(b, 2);
        int x0 = Math.Min((int)x, last - 1), y0 = Math.Min((int)y, last - 1), z0 = Math.Min((int)z, last - 1);
        float fx = x - x0, fy = y - y0, fz = z - z0;
        int origin = (x0 + y0 * size + z0 * size * size) * 3;
        int dy = size * 3, dz = size * size * 3;
        float Channel(int c)
        {
            int i = origin + c;
            float v00 = table[i] + (table[i + 3] - table[i]) * fx;
            float v10 = table[i + dy] + (table[i + dy + 3] - table[i + dy]) * fx;
            float v01 = table[i + dz] + (table[i + dz + 3] - table[i + dz]) * fx;
            float v11 = table[i + dz + dy] + (table[i + dz + dy + 3] - table[i + dz + dy]) * fx;
            float v0 = v00 + (v10 - v00) * fy;
            float v1 = v01 + (v11 - v01) * fy;
            return v0 + (v1 - v0) * fz;
        }
        r = Channel(0);
        g = Channel(1);
        b = Channel(2);
    }

    private float GridPosition(float value, int channel)
    {
        float range = DomainMax[channel] - DomainMin[channel];
        return Math.Clamp((value - DomainMin[channel]) / range, 0, 1) * (Size - 1);
    }

    private float[] Decode()
    {
        float[] table = new float[Data.Length / 2];
        for (int i = 0; i < table.Length; i++) table[i] = BitConverter.ToUInt16(Data, i * 2) / 65535f;
        return table;
    }

    private static ColorLut Create(int size, float[] values, float[]? domainMin = null, float[]? domainMax = null)
    {
        byte[] data = new byte[values.Length * 2];
        for (int i = 0; i < values.Length; i++)
        {
            BitConverter.TryWriteBytes(data.AsSpan(i * 2), (ushort)MathF.Round(Math.Clamp(values[i], 0, 1) * 65535));
        }
        return new ColorLut { Size = size, Data = data, DomainMin = domainMin ?? [0, 0, 0], DomainMax = domainMax ?? [1, 1, 1] };
    }

    private static void ValidateSize(int size)
    {
        if (size < 2 || size > MaxSize) throw new InvalidDataException(string.Format(Strings.ColorLut_SizeNotSupported, size));
    }

    private static float ParseNumber(string text) => float.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);

    private static string[] SplitLine(string line) => line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

    private static ColorLut ParseCube(string[] lines)
    {
        int size = 0, count = 0;
        float[] values = [];
        float[] domainMin = [0, 0, 0], domainMax = [1, 1, 1];
        foreach (string raw in lines)
        {
            string line = raw.Trim();
            if (line.Length == 0 || line[0] == '#') continue;
            string[] parts = SplitLine(line);
            if (char.IsLetter(line[0]))
            {
                switch (parts[0].ToUpperInvariant())
                {
                    case "LUT_1D_SIZE":
                        throw new InvalidDataException(Strings.ColorLut_1DNotSupported);
                    case "LUT_3D_SIZE":
                        size = int.Parse(parts[1], CultureInfo.InvariantCulture);
                        ValidateSize(size);
                        values = new float[size * size * size * 3];
                        break;
                    case "DOMAIN_MIN":
                        domainMin = parts.Skip(1).Take(3).Select(ParseNumber).ToArray();
                        break;
                    case "DOMAIN_MAX":
                        domainMax = parts.Skip(1).Take(3).Select(ParseNumber).ToArray();
                        break;
                }
                continue;
            }
            if (size == 0) throw new InvalidDataException(Strings.ColorLut_NoSizeEntry);
            if (parts.Length < 3 || count + 3 > values.Length) throw new InvalidDataException(Strings.ColorLut_DataSizeMismatch);
            for (int i = 0; i < 3; i++) values[count++] = ParseNumber(parts[i]);
        }
        if (size == 0) throw new InvalidDataException(Strings.ColorLut_NoSizeEntry);
        if (count != values.Length) throw new InvalidDataException(Strings.ColorLut_DataSizeMismatch);
        if (domainMin.Length != 3 || domainMax.Length != 3 || Enumerable.Range(0, 3).Any(i => domainMax[i] <= domainMin[i]))
        {
            throw new InvalidDataException(Strings.ColorLut_InvalidDomain);
        }
        return Create(size, values, domainMin, domainMax);
    }

    private static ColorLut Parse3dl(string[] lines)
    {
        List<float> entries = [];
        float peak = 0;
        foreach (string raw in lines)
        {
            string line = raw.Trim();
            if (line.Length == 0 || line[0] == '#' || char.IsLetter(line[0])) continue;
            string[] parts = SplitLine(line);
            // Any other number of values is the input shaper line.
            if (parts.Length != 3) continue;
            foreach (string part in parts)
            {
                float value = ParseNumber(part);
                entries.Add(value);
                peak = Math.Max(peak, value);
            }
        }
        int size = (int)MathF.Round(MathF.Pow(entries.Count / 3f, 1 / 3f));
        ValidateSize(size);
        if (size * size * size * 3 != entries.Count) throw new InvalidDataException(Strings.ColorLut_DataSizeMismatch);
        float scale = peak <= 1 ? 1 : peak <= 1023 ? 1023 : peak <= 4095 ? 4095 : 65535;
        float[] values = new float[entries.Count];
        int index = 0;
        // The file lists blue fastest; the table stores red fastest.
        for (int r = 0; r < size; r++)
        {
            for (int g = 0; g < size; g++)
            {
                for (int b = 0; b < size; b++)
                {
                    int target = (r + g * size + b * size * size) * 3;
                    for (int c = 0; c < 3; c++) values[target + c] = entries[index++] / scale;
                }
            }
        }
        return Create(size, values);
    }
}
