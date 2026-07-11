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

public sealed class ResizeCanvasImageEffect : ImageEffectBase
{
    public override string Id => "resize_canvas";
    public override string Name => "Resize canvas";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.expand;
    public override string Description => "Resizes the canvas.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<ResizeCanvasImageEffect>("top", "Top", -10000, 10000, 0, (e, v) => e.Top = v),
        EffectParameters.IntNumeric<ResizeCanvasImageEffect>("right", "Right", -10000, 10000, 0, (e, v) => e.Right = v),
        EffectParameters.IntNumeric<ResizeCanvasImageEffect>("bottom", "Bottom", -10000, 10000, 0, (e, v) => e.Bottom = v),
        EffectParameters.IntNumeric<ResizeCanvasImageEffect>("left", "Left", -10000, 10000, 0, (e, v) => e.Left = v),
        EffectParameters.Color<ResizeCanvasImageEffect>("background_color", "Background color", SKColors.Transparent, (e, v) => e.BackgroundColor = v)
    ];

    public int Top { get; set; }
    public int Right { get; set; }
    public int Bottom { get; set; }
    public int Left { get; set; }
    public SKColor BackgroundColor { get; set; } = SKColors.Transparent;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int newWidth = source.Width + Left + Right;
        int newHeight = source.Height + Top + Bottom;

        if (newWidth <= 0 || newHeight <= 0)
        {
            return source.Copy();
        }

        SKBitmap result = new SKBitmap(newWidth, newHeight, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new SKCanvas(result);
        canvas.Clear(BackgroundColor);
        canvas.DrawBitmap(source, Left, Top);
        return result;
    }
}