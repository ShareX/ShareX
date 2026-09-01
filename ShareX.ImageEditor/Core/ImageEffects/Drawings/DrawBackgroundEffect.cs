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

namespace ShareX.ImageEditor.Core.ImageEffects.Drawings;

public sealed class DrawBackgroundEffect : ImageEffectBase
{
    public override string Id => "draw_background";
    public override string Name => "Background";
    public override ImageEffectCategory Category => ImageEffectCategory.Drawings;
    public override string IconKey => LucideIcons.paint_bucket;
    public override string Description => "Draws a solid color background behind the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Color<DrawBackgroundEffect>("color", "Color", SKColors.Black, (e, v) => e.Color = v)
    ];

    public SKColor Color { get; set; } = SKColors.Black;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        SKBitmap result = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new SKCanvas(result);

        using SKPaint paint = new SKPaint { IsAntialias = true, Color = Color };
        canvas.DrawRect(0, 0, source.Width, source.Height, paint);
        canvas.DrawBitmap(source, 0, 0);
        return result;
    }
}