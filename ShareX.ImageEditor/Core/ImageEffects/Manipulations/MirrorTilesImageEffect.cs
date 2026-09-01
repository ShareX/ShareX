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

using ShareX.AvaloniaUI.Theming;
using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Manipulations;

public sealed class MirrorTilesImageEffect : ImageEffectBase
{
    public override string Id => "mirror_tiles";
    public override string Name => "Mirror tiles";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.flip_horizontal_2;
    public override string Description => "Tiles the image with alternating mirrored reflections.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<MirrorTilesImageEffect>("columns", "Columns", 1, 16, 3, (e, v) => e.Columns = v),
        EffectParameters.IntSlider<MirrorTilesImageEffect>("rows", "Rows", 1, 16, 3, (e, v) => e.Rows = v),
        EffectParameters.Bool<MirrorTilesImageEffect>("mirror_alternate_columns", "Mirror alternate columns", true, (e, v) => e.MirrorAlternateColumns = v),
        EffectParameters.Bool<MirrorTilesImageEffect>("mirror_alternate_rows", "Mirror alternate rows", true, (e, v) => e.MirrorAlternateRows = v)
    ];

    public int Columns { get; set; } = 3;
    public int Rows { get; set; } = 3;
    public bool MirrorAlternateColumns { get; set; } = true;
    public bool MirrorAlternateRows { get; set; } = true;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int columns = Math.Clamp(Columns, 1, 16);
        int rows = Math.Clamp(Rows, 1, 16);

        if (columns == 1 && rows == 1 && !MirrorAlternateColumns && !MirrorAlternateRows)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            float tiledY = (((y + 0.5f) / height) * rows);
            int tileY = Math.Min(rows - 1, Math.Max(0, (int)MathF.Floor(tiledY)));
            float localY = tiledY - tileY;
            if (tileY == rows - 1)
            {
                localY = Math.Min(1f, localY);
            }

            if (MirrorAlternateRows && (tileY & 1) == 1)
            {
                localY = 1f - localY;
            }

            float sampleY = localY * Math.Max(0, height - 1);

            for (int x = 0; x < width; x++)
            {
                float tiledX = (((x + 0.5f) / width) * columns);
                int tileX = Math.Min(columns - 1, Math.Max(0, (int)MathF.Floor(tiledX)));
                float localX = tiledX - tileX;
                if (tileX == columns - 1)
                {
                    localX = Math.Min(1f, localX);
                }

                if (MirrorAlternateColumns && (tileX & 1) == 1)
                {
                    localX = 1f - localX;
                }

                float sampleX = localX * Math.Max(0, width - 1);
                dstPixels[row + x] = DistortionEffectHelper.SampleClamped(srcPixels, width, height, sampleX, sampleY);
            }
        });

        return DistortionEffectHelper.CreateBitmap(source, width, height, dstPixels);
    }
}