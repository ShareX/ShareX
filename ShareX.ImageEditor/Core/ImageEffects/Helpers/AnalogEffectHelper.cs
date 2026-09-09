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

using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Helpers;

internal static class AnalogEffectHelper
{
    public static SKBitmap CreateBitmap(SKBitmap source, SKColor[] pixels)
    {
        return new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType)
        {
            Pixels = pixels
        };
    }

    public static SKBitmap CreateBlurredClamp(SKBitmap source, float radius)
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
            using SKShader shader = SKShader.CreateBitmap(
                source,
                SKShaderTileMode.Clamp,
                SKShaderTileMode.Clamp,
                SKMatrix.CreateTranslation(padding, padding));
            using SKPaint paint = new() { Shader = shader };
            expandCanvas.DrawRect(new SKRect(0, 0, expandedWidth, expandedHeight), paint);
        }

        float sigma = Math.Max(0.001f, radius / 3f);

        using SKBitmap blurredExpanded = new(expandedWidth, expandedHeight, source.ColorType, source.AlphaType);
        using (SKCanvas blurCanvas = new(blurredExpanded))
        {
            using var ownedImageFilter1 = SKImageFilter.CreateBlur(sigma, sigma);
            using SKPaint blurPaint = new()
            {
                ImageFilter = ownedImageFilter1
            };
            blurCanvas.DrawBitmap(expanded, 0, 0, blurPaint);
        }

        SKBitmap result = new(source.Width, source.Height, source.ColorType, source.AlphaType);
        using (SKCanvas resultCanvas = new(result))
        {
            resultCanvas.DrawBitmap(
                blurredExpanded,
                new SKRect(padding, padding, padding + source.Width, padding + source.Height),
                new SKRect(0, 0, source.Width, source.Height));
        }

        return result;
    }

    public static SKColor Sample(SKColor[] pixels, int width, int height, float x, float y)
    {
        return ProceduralEffectHelper.BilinearSample(pixels, width, height, x, y);
    }

    public static float Luminance(SKColor color)
    {
        return (0.2126f * color.Red) + (0.7152f * color.Green) + (0.0722f * color.Blue);
    }

    public static float Luminance01(SKColor color)
    {
        return Luminance(color) / 255f;
    }

    public static SKColor LerpColor(SKColor from, SKColor to, float t)
    {
        float alpha = ProceduralEffectHelper.Clamp01(t);
        return new SKColor(
            ProceduralEffectHelper.ClampToByte(ProceduralEffectHelper.Lerp(from.Red, to.Red, alpha)),
            ProceduralEffectHelper.ClampToByte(ProceduralEffectHelper.Lerp(from.Green, to.Green, alpha)),
            ProceduralEffectHelper.ClampToByte(ProceduralEffectHelper.Lerp(from.Blue, to.Blue, alpha)),
            ProceduralEffectHelper.ClampToByte(ProceduralEffectHelper.Lerp(from.Alpha, to.Alpha, alpha)));
    }

    public static float ApplyContrast(float value, float contrast)
    {
        return ProceduralEffectHelper.Clamp01(((value - 0.5f) * contrast) + 0.5f);
    }

    public static float Screen(float baseValue, float blendValue)
    {
        float b = ProceduralEffectHelper.Clamp01(baseValue);
        float s = ProceduralEffectHelper.Clamp01(blendValue);
        return 1f - ((1f - b) * (1f - s));
    }

    public static float Overlay(float baseValue, float blendValue)
    {
        float b = ProceduralEffectHelper.Clamp01(baseValue);
        float s = ProceduralEffectHelper.Clamp01(blendValue);
        return b < 0.5f
            ? 2f * b * s
            : 1f - (2f * (1f - b) * (1f - s));
    }

    public static SKColor FromHsl(float h, float s, float l, byte alpha)
    {
        while (h < 0f)
        {
            h += 360f;
        }

        while (h >= 360f)
        {
            h -= 360f;
        }

        return SKColor.FromHsl(
            h,
            Math.Clamp(s, 0f, 100f),
            Math.Clamp(l, 0f, 100f),
            alpha);
    }
}