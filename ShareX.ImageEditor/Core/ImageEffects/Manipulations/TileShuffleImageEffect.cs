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

public sealed class TileShuffleImageEffect : ImageEffectBase
{
    public override string Id => "tile_shuffle";
    public override string Name => "Tile Shuffle";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.shuffle;
    public override string Description => "Divides the image into a grid and randomly rearranges the tiles.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<TileShuffleImageEffect>("columns", "Columns", 2, 20, 4, (e, v) => e.Columns = v),
        EffectParameters.IntSlider<TileShuffleImageEffect>("rows", "Rows", 2, 20, 4, (e, v) => e.Rows = v),
        EffectParameters.FloatSlider<TileShuffleImageEffect>("gap", "Gap", 0, 10, 0, (e, v) => e.Gap = v),
        EffectParameters.Color<TileShuffleImageEffect>("gap_color", "Gap Color", new SKColor(0, 0, 0, 255), (e, v) => e.GapColor = v)
    ];

    public int Columns { get; set; } = 4;
    public int Rows { get; set; } = 4;
    public int Seed { get; set; } = 42;
    public float Gap { get; set; } = 0f;
    public SKColor GapColor { get; set; } = new SKColor(0, 0, 0, 255);

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int cols = Math.Clamp(Columns, 1, 100);
        int rows = Math.Clamp(Rows, 1, 100);
        int width = source.Width;
        int height = source.Height;
        float gap = Math.Clamp(Gap, 0f, 50f);

        int totalTiles = cols * rows;

        // Build a shuffled index array using Fisher-Yates with a deterministic seed.
        int[] indices = new int[totalTiles];
        for (int i = 0; i < totalTiles; i++)
            indices[i] = i;

        int rng = Seed == 0 ? 1 : Seed;
        for (int i = totalTiles - 1; i > 0; i--)
        {
            rng ^= rng << 13;
            rng ^= rng >> 17;
            rng ^= rng << 5;
            int j = (int)((uint)(rng & 0x7FFFFFFF) % (uint)(i + 1));
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        float tileW = (float)width / cols;
        float tileH = (float)height / rows;

        SKBitmap result = new(width, height, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new(result);
        canvas.Clear(GapColor);

        for (int destIdx = 0; destIdx < totalTiles; destIdx++)
        {
            int srcIdx = indices[destIdx];

            int srcCol = srcIdx % cols;
            int srcRow = srcIdx / cols;
            int dstCol = destIdx % cols;
            int dstRow = destIdx / cols;

            SKRect srcRect = new(
                srcCol * tileW,
                srcRow * tileH,
                (srcCol + 1) * tileW,
                (srcRow + 1) * tileH);

            SKRect dstRect = new(
                dstCol * tileW + gap * 0.5f,
                dstRow * tileH + gap * 0.5f,
                (dstCol + 1) * tileW - gap * 0.5f,
                (dstRow + 1) * tileH - gap * 0.5f);

            if (dstRect.Width <= 0 || dstRect.Height <= 0) continue;

            canvas.DrawBitmap(source, srcRect, dstRect);
        }

        return result;
    }
}