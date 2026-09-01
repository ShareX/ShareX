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

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class InnerShadowImageEffect : ImageEffectBase
{
    public override string Id => "inner_shadow";
    public override string Name => "Inner shadow";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.square_dashed;
    public override string Description => "Adds an inner shadow along the edges of opaque regions.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<InnerShadowImageEffect>("opacity", "Opacity", 0, 100, 70, (e, v) => e.Opacity = v),
        EffectParameters.IntSlider<InnerShadowImageEffect>("size", "Size", 0, 300, 20, (e, v) => e.Size = v),
        EffectParameters.Color<InnerShadowImageEffect>("color", "Color", SKColors.Black, (e, v) => e.Color = v),
        EffectParameters.IntNumeric<InnerShadowImageEffect>("offset_x", "Offset X", -300, 300, 0, (e, v) => e.OffsetX = v),
        EffectParameters.IntNumeric<InnerShadowImageEffect>("offset_y", "Offset Y", -300, 300, 0, (e, v) => e.OffsetY = v)
    ];

    public float Opacity { get; set; } = 70f; // 0..100
    public int Size { get; set; } = 20; // 0..300
    public SKColor Color { get; set; } = SKColors.Black;
    public int OffsetX { get; set; } = 0;
    public int OffsetY { get; set; } = 0;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float opacity = Math.Clamp(Opacity, 0f, 100f) / 100f;
        int size = Math.Clamp(Size, 0, 300);

        if (opacity <= 0f || (size <= 0 && OffsetX == 0 && OffsetY == 0) || Color.Alpha == 0)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        SKColor[] sourcePixels = source.Pixels;
        SKColor[] alphaMaskPixels = new SKColor[sourcePixels.Length];

        for (int i = 0; i < sourcePixels.Length; i++)
        {
            byte alpha = sourcePixels[i].Alpha;
            alphaMaskPixels[i] = new SKColor(alpha, alpha, alpha, alpha);
        }

        using SKBitmap alphaMask = new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = alphaMaskPixels
        };
        using SKBitmap shiftedMask = ShiftBitmap(alphaMask, -OffsetX, -OffsetY);
        using SKBitmap blurredMask = size > 0 ? ApplyTransparentBlur(shiftedMask, size / 2f) : shiftedMask.Copy();

        SKColor[] blurredPixels = blurredMask.Pixels;
        SKColor[] outputPixels = new SKColor[sourcePixels.Length];

        float colorAlpha = Color.Alpha / 255f;

        for (int i = 0; i < sourcePixels.Length; i++)
        {
            SKColor src = sourcePixels[i];
            float sourceAlpha = src.Alpha / 255f;

            if (sourceAlpha <= 0f)
            {
                outputPixels[i] = src;
                continue;
            }

            float shiftedAlpha = blurredPixels[i].Alpha / 255f;
            float innerCoverage = ProceduralEffectHelper.Clamp01(sourceAlpha - shiftedAlpha);
            float shadowMix = innerCoverage * opacity * colorAlpha;

            float r = ProceduralEffectHelper.Lerp(src.Red, Color.Red, shadowMix);
            float g = ProceduralEffectHelper.Lerp(src.Green, Color.Green, shadowMix);
            float b = ProceduralEffectHelper.Lerp(src.Blue, Color.Blue, shadowMix);

            outputPixels[i] = new SKColor(
                ProceduralEffectHelper.ClampToByte(r),
                ProceduralEffectHelper.ClampToByte(g),
                ProceduralEffectHelper.ClampToByte(b),
                src.Alpha);
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = outputPixels
        };
    }

    private static SKBitmap ShiftBitmap(SKBitmap source, int offsetX, int offsetY)
    {
        SKBitmap result = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);

        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);
        canvas.DrawBitmap(source, offsetX, offsetY);

        return result;
    }

    private static SKBitmap ApplyTransparentBlur(SKBitmap source, float radius)
    {
        if (radius <= 0.01f)
        {
            return source.Copy();
        }

        int padding = Math.Max(2, (int)MathF.Ceiling(radius * 2f));
        int expandedWidth = source.Width + (padding * 2);
        int expandedHeight = source.Height + (padding * 2);

        using SKBitmap expanded = new(expandedWidth, expandedHeight, source.ColorType, source.AlphaType);
        using (SKCanvas expandCanvas = new(expanded))
        {
            expandCanvas.Clear(SKColors.Transparent);
            expandCanvas.DrawBitmap(source, padding, padding);
        }

        float sigma = Math.Max(0.001f, radius / 3f);

        using SKBitmap blurredExpanded = new(expandedWidth, expandedHeight, source.ColorType, source.AlphaType);
        using (SKCanvas blurCanvas = new(blurredExpanded))
        using (SKPaint blurPaint = new() { ImageFilter = SKImageFilter.CreateBlur(sigma, sigma) })
        {
            blurCanvas.DrawBitmap(expanded, 0, 0, blurPaint);
        }

        SKBitmap result = new(source.Width, source.Height, source.ColorType, source.AlphaType);
        using (SKCanvas resultCanvas = new(result))
        {
            resultCanvas.Clear(SKColors.Transparent);
            resultCanvas.DrawBitmap(
                blurredExpanded,
                new SKRect(padding, padding, padding + source.Width, padding + source.Height),
                new SKRect(0, 0, source.Width, source.Height));
        }

        return result;
    }
}