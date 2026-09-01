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

public sealed class DrawCheckerboardEffect : ImageEffectBase
{
    private int _size = 10;

    public override string Id => "draw_checkerboard";
    public override string Name => "Checkerboard";
    public override ImageEffectCategory Category => ImageEffectCategory.Drawings;
    public override string IconKey => LucideIcons.grid_2x2_check;
    public override string Description => "Draws a checkerboard pattern behind the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<DrawCheckerboardEffect>("size", "Size", 1, 100, 10, (e, v) => e.Size = v),
        EffectParameters.Color<DrawCheckerboardEffect>("color", "Color", new SKColor(211, 211, 211), (e, v) => e.Color = v),
        EffectParameters.Color<DrawCheckerboardEffect>("color2", "Color 2", SKColors.White, (e, v) => e.Color2 = v)
    ];

    public int Size
    {
        get => _size;
        set => _size = Math.Max(1, value);
    }

    public SKColor Color { get; set; } = new SKColor(211, 211, 211);

    public SKColor Color2 { get; set; } = SKColors.White;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        using SKBitmap pattern = CreateCheckerPattern(Size, Size, Color, Color2);
        using SKShader shader = SKShader.CreateBitmap(pattern, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
        using SKPaint checkerPaint = new SKPaint { Shader = shader, IsAntialias = true };

        SKBitmap result = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new SKCanvas(result);
        canvas.DrawRect(0, 0, result.Width, result.Height, checkerPaint);
        canvas.DrawBitmap(source, 0, 0);
        return result;
    }

    private static SKBitmap CreateCheckerPattern(int width, int height, SKColor color1, SKColor color2)
    {
        SKBitmap bitmap = new SKBitmap(width * 2, height * 2);
        using SKCanvas canvas = new SKCanvas(bitmap);
        using SKPaint paint1 = new SKPaint { Color = color1, Style = SKPaintStyle.Fill, IsAntialias = false };
        using SKPaint paint2 = new SKPaint { Color = color2, Style = SKPaintStyle.Fill, IsAntialias = false };

        canvas.DrawRect(0, 0, width, height, paint1);
        canvas.DrawRect(width, height, width, height, paint1);
        canvas.DrawRect(width, 0, width, height, paint2);
        canvas.DrawRect(0, height, width, height, paint2);

        return bitmap;
    }
}