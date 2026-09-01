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
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Manipulations;

public sealed class TileRepeatImageEffect : ImageEffectBase
{
    public override string Id => "tile_repeat";
    public override string Name => "Tile repeat";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.layout_grid;
    public override string Description => "Tiles the image into a grid of smaller copies.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<TileRepeatImageEffect>("columns", "Columns", 2, 10, 3, (e, v) => e.Columns = v),
        EffectParameters.IntSlider<TileRepeatImageEffect>("rows", "Rows", 2, 10, 3, (e, v) => e.Rows = v)
    ];

    public int Columns { get; set; } = 3;
    public int Rows { get; set; } = 3;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int cols = Math.Max(1, Columns);
        int rows = Math.Max(1, Rows);
        int w = source.Width, h = source.Height;
        float tileW = w / (float)cols;
        float tileH = h / (float)rows;

        SKBitmap result = new(w, h, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                SKRect dst = new(c * tileW, r * tileH, (c + 1) * tileW, (r + 1) * tileH);
                canvas.DrawBitmap(source, new SKRect(0, 0, w, h), dst);
            }
        }

        return result;
    }
}