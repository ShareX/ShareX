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

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class PopArtGridImageEffect : ImageEffectBase
{
    public override string Id => "pop_art_grid";
    public override string Name => "Pop art grid";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.grid_2x2;
    public override string Description => "Creates an Andy Warhol-style 2\u00d72 pop art grid with color variations.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Color<PopArtGridImageEffect>("color1", "Color 1", new SKColor(255, 50, 50), (e, v) => e.Color1 = v),
        EffectParameters.Color<PopArtGridImageEffect>("color2", "Color 2", new SKColor(50, 200, 50), (e, v) => e.Color2 = v),
        EffectParameters.Color<PopArtGridImageEffect>("color3", "Color 3", new SKColor(50, 50, 255), (e, v) => e.Color3 = v),
        EffectParameters.Color<PopArtGridImageEffect>("color4", "Color 4", new SKColor(255, 200, 50), (e, v) => e.Color4 = v),
        EffectParameters.FloatSlider<PopArtGridImageEffect>("strength", "Color strength", 0, 100, 60, (e, v) => e.Strength = v)
    ];

    public SKColor Color1 { get; set; } = new SKColor(255, 50, 50);
    public SKColor Color2 { get; set; } = new SKColor(50, 200, 50);
    public SKColor Color3 { get; set; } = new SKColor(50, 50, 255);
    public SKColor Color4 { get; set; } = new SKColor(255, 200, 50);
    public float Strength { get; set; } = 60f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int w = source.Width, h = source.Height;
        int halfW = w / 2, halfH = h / 2;
        float s = Math.Clamp(Strength / 100f, 0f, 1f);

        // Create high-contrast grayscale version
        SKBitmap gray = new(w, h, source.ColorType, source.AlphaType);
        using (SKCanvas gc = new(gray))
        {
            float[] grayMatrix =
            {
                0.33f, 0.33f, 0.33f, 0, 0,
                0.33f, 0.33f, 0.33f, 0, 0,
                0.33f, 0.33f, 0.33f, 0, 0,
                0,     0,     0,     1, 0
            };
            using SKColorFilter gf = SKColorFilter.CreateColorMatrix(grayMatrix);
            using SKPaint gp = new() { ColorFilter = gf };
            gc.DrawBitmap(source, 0, 0, gp);
        }

        SKBitmap result = new(w, h, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new(result);

        SKColor[] tints = [Color1, Color2, Color3, Color4];
        SKRect[] cells =
        [
            new SKRect(0, 0, halfW, halfH),
            new SKRect(halfW, 0, w, halfH),
            new SKRect(0, halfH, halfW, h),
            new SKRect(halfW, halfH, w, h)
        ];

        for (int i = 0; i < 4; i++)
        {
            // Draw scaled-down gray version into the cell
            canvas.Save();
            canvas.ClipRect(cells[i]);
            canvas.DrawBitmap(gray, new SKRect(0, 0, w, h), cells[i]);

            // Tint overlay
            using SKPaint tintPaint = new()
            {
                Color = tints[i].WithAlpha((byte)(255 * s)),
                BlendMode = SKBlendMode.Modulate
            };
            canvas.DrawRect(cells[i], tintPaint);
            canvas.Restore();
        }

        gray.Dispose();
        return result;
    }
}