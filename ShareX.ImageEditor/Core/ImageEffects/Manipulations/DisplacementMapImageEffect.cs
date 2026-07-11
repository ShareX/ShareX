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

namespace ShareX.ImageEditor.Core.ImageEffects.Manipulations;

public sealed class DisplacementMapImageEffect : ImageEffectBase
{
    public override string Id => "displacement_map";
    public override string Name => "Displacement map";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.map;
    public override string Description => "Displaces pixels using a map image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FilePath<DisplacementMapImageEffect>("map_file_path", "Map file path", string.Empty, (e, v) => e.MapFilePath = v),
        EffectParameters.FloatSlider<DisplacementMapImageEffect>("amount_x", "Amount X", -200f, 200f, 20f, (e, v) => e.AmountX = v),
        EffectParameters.FloatSlider<DisplacementMapImageEffect>("amount_y", "Amount Y", -200f, 200f, 20f, (e, v) => e.AmountY = v)
    ];

    // Uses the selected map image when available; otherwise falls back to the source image.
    public string MapFilePath { get; set; } = string.Empty;

    public float AmountX { get; set; } = 20f;
    public float AmountY { get; set; } = 20f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        int right = width - 1;
        int bottom = height - 1;

        if (Math.Abs(AmountX) < 0.0001f && Math.Abs(AmountY) < 0.0001f)
        {
            return source.Copy();
        }

        SKColor[] srcPixels = source.Pixels;
        using SKBitmap? displacementMap = TryLoadDisplacementMap();
        SKColor[] mapPixels = displacementMap?.Pixels ?? srcPixels;
        int mapWidth = displacementMap?.Width ?? width;
        int mapHeight = displacementMap?.Height ?? height;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        for (int y = 0; y < height; y++)
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                SKColor map = mapPixels[GetMapPixelIndex(x, y, width, height, mapWidth, mapHeight)];
                float dx = ((map.Red / 255f) - 0.5f) * 2f * AmountX;
                float dy = ((map.Green / 255f) - 0.5f) * 2f * AmountY;

                int sampleX = Clamp((int)MathF.Round(x + dx), 0, right);
                int sampleY = Clamp((int)MathF.Round(y + dy), 0, bottom);

                dstPixels[row + x] = srcPixels[sampleY * width + sampleX];
            }
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType) { Pixels = dstPixels };
    }

    private SKBitmap? TryLoadDisplacementMap()
    {
        string mapPath = Environment.ExpandEnvironmentVariables(MapFilePath);
        if (string.IsNullOrWhiteSpace(mapPath) || !File.Exists(mapPath))
        {
            return null;
        }

        SKBitmap? map = SKBitmap.Decode(mapPath);
        if (map == null || map.Width <= 0 || map.Height <= 0)
        {
            map?.Dispose();
            return null;
        }

        return map;
    }

    private static int GetMapPixelIndex(int x, int y, int sourceWidth, int sourceHeight, int mapWidth, int mapHeight)
    {
        int mapX = ScaleCoordinate(x, sourceWidth, mapWidth);
        int mapY = ScaleCoordinate(y, sourceHeight, mapHeight);
        return mapY * mapWidth + mapX;
    }

    private static int ScaleCoordinate(int coordinate, int sourceLength, int targetLength)
    {
        if (targetLength <= 1 || sourceLength <= 1)
        {
            return 0;
        }

        float scaled = (float)coordinate * (targetLength - 1) / (sourceLength - 1);
        return Clamp((int)MathF.Round(scaled), 0, targetLength - 1);
    }

    private static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}