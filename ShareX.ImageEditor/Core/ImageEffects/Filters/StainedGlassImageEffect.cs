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

using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class StainedGlassImageEffect : ImageEffectBase
{
    public override string Id => "stained_glass";
    public override string Name => "Stained glass";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.hexagon;
    public override string Description => "Creates a stained glass mosaic effect with lead borders.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<StainedGlassImageEffect>("tile_size", "Tile size", 6, 120, 22, (e, v) => e.TileSize = v),
        EffectParameters.FloatSlider<StainedGlassImageEffect>("irregularity", "Irregularity", 0f, 100f, 55f, (e, v) => e.Irregularity = v),
        EffectParameters.FloatSlider<StainedGlassImageEffect>("lead_width", "Lead width", 0f, 12f, 1.8f, (e, v) => e.LeadWidth = v),
        EffectParameters.FloatSlider<StainedGlassImageEffect>("lead_opacity", "Lead opacity", 0f, 100f, 85f, (e, v) => e.LeadOpacity = v),
        EffectParameters.FloatSlider<StainedGlassImageEffect>("color_boost", "Color boost", 0f, 100f, 20f, (e, v) => e.ColorBoost = v),
    ];

    public int TileSize { get; set; } = 22; // 6..120
    public float Irregularity { get; set; } = 55f; // 0..100
    public float LeadWidth { get; set; } = 1.8f; // 0..12
    public float LeadOpacity { get; set; } = 85f; // 0..100
    public float ColorBoost { get; set; } = 20f; // 0..100
    public int Seed { get; set; } = 1337;

    private readonly struct Cell
    {
        public Cell(float x, float y, SKColor color)
        {
            X = x;
            Y = y;
            Color = color;
        }

        public float X { get; }
        public float Y { get; }
        public SKColor Color { get; }
    }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int tileSize = Math.Clamp(TileSize, 6, 120);
        float irregularity = Math.Clamp(Irregularity, 0f, 100f) / 100f;
        float leadWidth = Math.Clamp(LeadWidth, 0f, 12f);
        float leadOpacity = Math.Clamp(LeadOpacity, 0f, 100f) / 100f;
        float colorBoost = Math.Clamp(ColorBoost, 0f, 100f) / 100f;

        int width = source.Width;
        int height = source.Height;
        int right = width - 1;
        int bottom = height - 1;

        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        int gridW = (int)Math.Ceiling(width / (float)tileSize) + 2;
        int gridH = (int)Math.Ceiling(height / (float)tileSize) + 2;
        Cell[] grid = new Cell[gridW * gridH];

        float jitterRange = tileSize * 0.5f * irregularity;
        uint baseSeed = unchecked((uint)Seed * 2246822519u);

        for (int gy = 0; gy < gridH; gy++)
        {
            for (int gx = 0; gx < gridW; gx++)
            {
                float baseX = ((gx - 1) + 0.5f) * tileSize;
                float baseY = ((gy - 1) + 0.5f) * tileSize;

                float ox = (Hash01(gx, gy, baseSeed ^ 0x9E3779B9u) * 2f - 1f) * jitterRange;
                float oy = (Hash01(gx, gy, baseSeed ^ 0x85EBCA6Bu) * 2f - 1f) * jitterRange;

                float cx = Math.Clamp(baseX + ox, 0f, right);
                float cy = Math.Clamp(baseY + oy, 0f, bottom);

                int sx = Clamp((int)MathF.Round(cx), 0, right);
                int sy = Clamp((int)MathF.Round(cy), 0, bottom);
                SKColor sampled = srcPixels[(sy * width) + sx];

                if (colorBoost > 0f)
                {
                    sampled = BoostSaturation(sampled, colorBoost);
                }

                grid[(gy * gridW) + gx] = new Cell(cx, cy, sampled);
            }
        }

        for (int y = 0; y < height; y++)
        {
            int row = y * width;
            int gy = (y / tileSize) + 1;

            for (int x = 0; x < width; x++)
            {
                int gx = (x / tileSize) + 1;

                float nearest = float.MaxValue;
                float second = float.MaxValue;
                SKColor nearestColor = srcPixels[row + x];

                for (int ny = gy - 1; ny <= gy + 1; ny++)
                {
                    int yOffset = ny * gridW;
                    for (int nx = gx - 1; nx <= gx + 1; nx++)
                    {
                        Cell cell = grid[yOffset + nx];
                        float dx = x - cell.X;
                        float dy = y - cell.Y;
                        float d2 = (dx * dx) + (dy * dy);

                        if (d2 < nearest)
                        {
                            second = nearest;
                            nearest = d2;
                            nearestColor = cell.Color;
                        }
                        else if (d2 < second)
                        {
                            second = d2;
                        }
                    }
                }

                SKColor output = nearestColor;
                if (leadWidth > 0f && second < float.MaxValue)
                {
                    float edgeDistance = (MathF.Sqrt(second) - MathF.Sqrt(nearest)) * 0.5f;
                    if (edgeDistance < leadWidth)
                    {
                        float edgeBlend = 1f - (edgeDistance / leadWidth);
                        float alpha = leadOpacity * edgeBlend;
                        output = LerpToBlack(output, alpha);
                    }
                }

                dstPixels[row + x] = output;
            }
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static SKColor BoostSaturation(SKColor color, float amount01)
    {
        float r = color.Red;
        float g = color.Green;
        float b = color.Blue;

        float gray = (r + g + b) / 3f;
        float factor = 1f + amount01;

        int nr = (int)MathF.Round(gray + ((r - gray) * factor));
        int ng = (int)MathF.Round(gray + ((g - gray) * factor));
        int nb = (int)MathF.Round(gray + ((b - gray) * factor));

        return new SKColor(ClampToByte(nr), ClampToByte(ng), ClampToByte(nb), color.Alpha);
    }

    private static SKColor LerpToBlack(SKColor color, float alpha)
    {
        alpha = Math.Clamp(alpha, 0f, 1f);
        float keep = 1f - alpha;

        int r = (int)MathF.Round(color.Red * keep);
        int g = (int)MathF.Round(color.Green * keep);
        int b = (int)MathF.Round(color.Blue * keep);

        return new SKColor(ClampToByte(r), ClampToByte(g), ClampToByte(b), color.Alpha);
    }

    private static float Hash01(int x, int y, uint seed)
    {
        unchecked
        {
            uint h = (uint)x;
            h = (h * 374761393u) + (uint)(y * 668265263);
            h ^= seed;
            h = Hash(h);
            return (h & 0xFFFFFF) / 16777215f;
        }
    }

    private static uint Hash(uint x)
    {
        x ^= x >> 16;
        x *= 0x7FEB352Du;
        x ^= x >> 15;
        x *= 0x846CA68Bu;
        x ^= x >> 16;
        return x;
    }

    private static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    private static byte ClampToByte(int value)
    {
        if (value <= 0) return 0;
        if (value >= 255) return 255;
        return (byte)value;
    }
}