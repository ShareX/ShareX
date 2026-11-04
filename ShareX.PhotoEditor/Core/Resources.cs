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

namespace ShareX.PhotoEditor.Core;

public enum GradientType
{
    Linear,
    Radial,
    Angle,
    Reflected,
    Diamond
}

public sealed record GradientStop(float Position, SKColor Color);

public sealed class GradientDefinition
{
    public string Name { get; set; } = "Custom";

    public List<GradientStop> Stops { get; set; } = [];

    public GradientType Type { get; set; } = GradientType.Linear;

    public float Angle { get; set; } = 90;

    public bool Reverse { get; set; }

    public bool Dither { get; set; } = true;

    public GradientDefinition Clone() => new()
    {
        Name = Name,
        Stops = Stops.ToList(),
        Type = Type,
        Angle = Angle,
        Reverse = Reverse,
        Dither = Dither
    };

    public static GradientDefinition CreateDefault(SKColor start, SKColor end, string name = ForegroundToBackground) => new()
    {
        Name = name,
        Stops = [new GradientStop(0, start), new GradientStop(1, end)]
    };

    public const string ForegroundToBackground = "Foreground to Background";
    public const string ForegroundToTransparent = "Foreground to Transparent";

    /// <summary>True for presets that always follow the current foreground and background colors.</summary>
    public bool IsDynamic => Name is ForegroundToBackground or ForegroundToTransparent;

    /// <summary>Returns this gradient, with the color stops of the foreground presets refreshed from the current colors.</summary>
    public GradientDefinition ResolveColors(SKColor foreground, SKColor background)
    {
        if (!IsDynamic) return this;
        GradientDefinition resolved = Clone();
        resolved.Stops = Name == ForegroundToBackground
            ? [new GradientStop(0, foreground), new GradientStop(1, background)]
            : [new GradientStop(0, foreground), new GradientStop(1, foreground.WithAlpha(0))];
        return resolved;
    }

    public static IReadOnlyList<GradientDefinition> CreatePresets(SKColor foreground, SKColor background) =>
    [
        CreateDefault(foreground, background),
        CreateDefault(foreground, foreground.WithAlpha(0), ForegroundToTransparent),
        CreateDefault(SKColors.Black, SKColors.White, "Black, White"),
        new() { Name = "Spectrum", Stops = [new(0, SKColors.Red), new(0.17f, SKColors.Yellow), new(0.33f, SKColors.Lime), new(0.5f, SKColors.Cyan), new(0.67f, SKColors.Blue), new(0.83f, SKColors.Magenta), new(1, SKColors.Red)] },
        new() { Name = "Sunset", Stops = [new(0, new SKColor(0x2B, 0x1B, 0x4F)), new(0.5f, new SKColor(0xE2, 0x4A, 0x6A)), new(1, new SKColor(0xFF, 0xC8, 0x57))] },
        new() { Name = "Ocean", Stops = [new(0, new SKColor(0x02, 0x1B, 0x3A)), new(0.6f, new SKColor(0x1E, 0x88, 0xE5)), new(1, new SKColor(0xB3, 0xE5, 0xFC))] },
        new() { Name = "Copper", Stops = [new(0, new SKColor(0x3B, 0x1E, 0x0A)), new(0.5f, new SKColor(0xC8, 0x77, 0x3A)), new(1, new SKColor(0xFF, 0xE0, 0xB2))] },
        new() { Name = "Chrome", Stops = [new(0, new SKColor(0x30, 0x30, 0x30)), new(0.45f, SKColors.White), new(0.5f, new SKColor(0x60, 0x60, 0x60)), new(1, new SKColor(0xE0, 0xE0, 0xE0))] },
        new() { Name = "Transparent Stripes", Stops = [new(0, SKColors.Black), new(0.2f, SKColors.Transparent), new(0.4f, SKColors.Black), new(0.6f, SKColors.Transparent), new(0.8f, SKColors.Black), new(1, SKColors.Transparent)] }
    ];

    /// <summary>Samples the gradient at t in 0..1.</summary>
    public SKColor Sample(float t)
    {
        if (Reverse) t = 1 - t;
        t = Math.Clamp(t, 0, 1);
        if (Stops.Count == 0) return SKColors.Transparent;
        List<GradientStop> stops = Stops.OrderBy(s => s.Position).ToList();
        if (t <= stops[0].Position) return stops[0].Color;
        for (int i = 1; i < stops.Count; i++)
        {
            if (t <= stops[i].Position)
            {
                float range = stops[i].Position - stops[i - 1].Position;
                float local = range <= 0 ? 0 : (t - stops[i - 1].Position) / range;
                return ColorMath.Lerp(stops[i - 1].Color, stops[i].Color, local);
            }
        }
        return stops[^1].Color;
    }

    /// <summary>Builds a 256 entry lookup table of premultiplied pixels.</summary>
    public uint[] BuildLut()
    {
        uint[] lut = new uint[256];
        for (int i = 0; i < 256; i++) lut[i] = ColorMath.FromColor(Sample(i / 255f));
        return lut;
    }

    /// <summary>Creates a linear shader spanning the area at the configured angle.</summary>
    public SKShader CreateShader(SKRect area)
    {
        float radians = Angle * MathF.PI / 180;
        SKPoint center = new(area.MidX, area.MidY);
        float half = (MathF.Abs(MathF.Cos(radians)) * area.Width + MathF.Abs(MathF.Sin(radians)) * area.Height) / 2;
        SKPoint direction = new(MathF.Cos(radians), -MathF.Sin(radians));
        SKPoint start = center - new SKPoint(direction.X * half, direction.Y * half);
        SKPoint end = center + new SKPoint(direction.X * half, direction.Y * half);
        return CreateShader(start, end);
    }

    public SKShader CreateShader(SKPoint start, SKPoint end)
    {
        List<GradientStop> stops = Stops.OrderBy(s => s.Position).ToList();
        SKColor[] colors = stops.Select(s => s.Color).ToArray();
        float[] positions = stops.Select(s => s.Position).ToArray();
        if (Reverse)
        {
            Array.Reverse(colors);
            positions = positions.Reverse().Select(p => 1 - p).ToArray();
        }
        float length = SKPoint.Distance(start, end);
        switch (Type)
        {
            case GradientType.Radial:
                return SKShader.CreateRadialGradient(start, Math.Max(0.5f, length), colors, positions, SKShaderTileMode.Clamp);
            case GradientType.Angle:
            {
                float angle = MathF.Atan2(end.Y - start.Y, end.X - start.X) * 180 / MathF.PI;
                return SKShader.CreateSweepGradient(start, colors, positions, SKShaderTileMode.Clamp, 0, 360,
                    SKMatrix.CreateRotationDegrees(angle, start.X, start.Y));
            }
            case GradientType.Reflected:
            {
                SKPoint mirrored = start - (end - start);
                List<SKColor> c = colors.Reverse().Concat(colors.Skip(1)).ToList();
                List<float> p = positions.Reverse().Select(x => (1 - x) / 2).Concat(positions.Skip(1).Select(x => 0.5f + x / 2)).ToList();
                return SKShader.CreateLinearGradient(mirrored, end, c.ToArray(), p.ToArray(), SKShaderTileMode.Clamp);
            }
            default:
                return SKShader.CreateLinearGradient(start, end, colors, positions, SKShaderTileMode.Clamp);
        }
    }
}

public sealed class PatternDefinition
{
    public PatternDefinition(string name, SKBitmap tile)
    {
        Name = name;
        Tile = tile;
    }

    public string Name { get; }

    public SKBitmap Tile { get; }

    private uint[]? _pixels;

    /// <summary>Returns the premultiplied tile pixel for a document position, wrapping around the tile.</summary>
    public uint Sample(int x, int y)
    {
        _pixels ??= ImageOps.ToPremulArray(Tile);
        int tx = ((x % Tile.Width) + Tile.Width) % Tile.Width;
        int ty = ((y % Tile.Height) + Tile.Height) % Tile.Height;
        return _pixels[ty * Tile.Width + tx];
    }

    public static List<PatternDefinition> CreateDefaults()
    {
        List<PatternDefinition> list = [];

        list.Add(Create("Checkerboard", 16, (canvas, size) =>
        {
            canvas.Clear(SKColors.White);
            using SKPaint p = new() { Color = new SKColor(0xCC, 0xCC, 0xCC) };
            canvas.DrawRect(0, 0, size / 2, size / 2, p);
            canvas.DrawRect(size / 2, size / 2, size / 2, size / 2, p);
        }));
        list.Add(Create("Diagonal Lines", 12, (canvas, size) =>
        {
            canvas.Clear(SKColors.White);
            using SKPaint p = new() { Color = SKColors.Black, StrokeWidth = 2, IsAntialias = true };
            canvas.DrawLine(0, size, size, 0, p);
            canvas.DrawLine(-size / 2f, size / 2f, size / 2f, -size / 2f, p);
            canvas.DrawLine(size / 2f, size * 1.5f, size * 1.5f, size / 2f, p);
        }));
        list.Add(Create("Dots", 14, (canvas, size) =>
        {
            canvas.Clear(SKColors.White);
            using SKPaint p = new() { Color = SKColors.Black, IsAntialias = true };
            canvas.DrawCircle(size / 2f, size / 2f, size / 5f, p);
        }));
        list.Add(Create("Grid", 20, (canvas, size) =>
        {
            canvas.Clear(SKColors.White);
            using SKPaint p = new() { Color = new SKColor(0x80, 0x80, 0x80), StrokeWidth = 1 };
            canvas.DrawLine(0, 0.5f, size, 0.5f, p);
            canvas.DrawLine(0.5f, 0, 0.5f, size, p);
        }));
        list.Add(Create("Bricks", 32, (canvas, size) =>
        {
            canvas.Clear(new SKColor(0xB5, 0x4A, 0x32));
            using SKPaint p = new() { Color = new SKColor(0xD8, 0xD0, 0xC0), StrokeWidth = 2 };
            canvas.DrawLine(0, 1, size, 1, p);
            canvas.DrawLine(0, size / 2f + 1, size, size / 2f + 1, p);
            canvas.DrawLine(size / 2f, 0, size / 2f, size / 2f, p);
            canvas.DrawLine(1, size / 2f, 1, size, p);
        }));
        list.Add(Create("Noise", 64, (canvas, size) =>
        {
            Random random = new(7);
            using SKPaint p = new();
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    byte v = (byte)random.Next(96, 224);
                    p.Color = new SKColor(v, v, v);
                    canvas.DrawPoint(x, y, p);
                }
            }
        }));
        list.Add(Create("Wood Grain", 64, (canvas, size) =>
        {
            using SKPaint p = new();
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float v = 0.5f + 0.5f * MathF.Sin((x + 6 * MathF.Sin(y * 0.2f)) * 0.6f);
                    p.Color = ColorMath.Lerp(new SKColor(0x8B, 0x5A, 0x2B), new SKColor(0xC8, 0x92, 0x5A), v);
                    canvas.DrawPoint(x, y, p);
                }
            }
        }));
        list.Add(Create("Hexagons", 28, (canvas, size) =>
        {
            canvas.Clear(new SKColor(0xF0, 0xF0, 0xF0));
            using SKPaint p = new() { Color = new SKColor(0x40, 0x40, 0x40), Style = SKPaintStyle.Stroke, StrokeWidth = 1.5f, IsAntialias = true };
            using SKPath path = new();
            for (int i = 0; i < 6; i++)
            {
                float a = MathF.PI / 3 * i;
                SKPoint pt = new(size / 2f + MathF.Cos(a) * size / 2.3f, size / 2f + MathF.Sin(a) * size / 2.3f);
                if (i == 0) path.MoveTo(pt);
                else path.LineTo(pt);
            }
            path.Close();
            canvas.DrawPath(path, p);
        }));
        return list;
    }

    private static PatternDefinition Create(string name, int size, Action<SKCanvas, int> draw)
    {
        SKBitmap bitmap = new(new SKImageInfo(size, size, SKColorType.Bgra8888, SKAlphaType.Premul));
        using (SKCanvas canvas = new(bitmap)) draw(canvas, size);
        return new PatternDefinition(name, bitmap);
    }
}

public enum BrushTipShape
{
    Round,
    Square,
    Sampled
}

/// <summary>A brush preset describing tip shape and dynamics.</summary>
public sealed class BrushPreset
{
    public string Name { get; set; } = "Soft Round";

    public float Size { get; set; } = 30;

    /// <summary>0..1, where 1 is a hard edge.</summary>
    public float Hardness { get; set; } = 0;

    /// <summary>Distance between dabs as a fraction of the brush diameter.</summary>
    public float Spacing { get; set; } = 0.1f;

    /// <summary>Tip roundness from 0.05 to 1.</summary>
    public float Roundness { get; set; } = 1;

    /// <summary>Tip angle in degrees.</summary>
    public float Angle { get; set; }

    public BrushTipShape Shape { get; set; } = BrushTipShape.Round;

    /// <summary>Sampled tip alpha (white = paint) for custom brushes.</summary>
    public SKBitmap? SampledTip { get; set; }

    public bool PressureSize { get; set; }

    public bool PressureOpacity { get; set; } = true;

    /// <summary>Random size jitter 0..1.</summary>
    public float SizeJitter { get; set; }

    /// <summary>Random scatter as a fraction of diameter.</summary>
    public float Scatter { get; set; }

    public BrushPreset Clone() => (BrushPreset)MemberwiseClone();

    public static List<BrushPreset> CreateDefaults() =>
    [
        new() { Name = "Soft Round", Size = 30, Hardness = 0 },
        new() { Name = "Hard Round", Size = 30, Hardness = 1 },
        new() { Name = "Soft Round Pressure", Size = 45, Hardness = 0, PressureSize = true },
        new() { Name = "Hard Round Pressure", Size = 19, Hardness = 1, PressureSize = true },
        new() { Name = "Airbrush Soft", Size = 100, Hardness = 0, Spacing = 0.05f },
        new() { Name = "Calligraphic Flat", Size = 25, Hardness = 1, Roundness = 0.25f, Angle = 45 },
        new() { Name = "Square Hard", Size = 20, Hardness = 1, Shape = BrushTipShape.Square },
        new() { Name = "Spatter", Size = 40, Hardness = 0.6f, Spacing = 0.5f, SizeJitter = 0.8f, Scatter = 1.2f },
        new() { Name = "Chalk", Size = 36, Hardness = 0.8f, Spacing = 0.15f, SizeJitter = 0.3f, Scatter = 0.15f, Roundness = 0.7f },
        new() { Name = "Hard Pixel", Size = 1, Hardness = 1, Shape = BrushTipShape.Square, Spacing = 0.05f }
    ];
}
