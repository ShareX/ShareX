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

using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Manipulations;

public enum PageCurlCorner
{
    TopLeft,
    TopRight,
    BottomRight,
    BottomLeft
}

public sealed class PageCurlImageEffect : ImageEffectBase
{
    public override string Id => "page_curl";
    public override string Name => "Page curl";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.rotate_cw_square;
    public override string Description => "Simulates a page curl at a corner of the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Enum<PageCurlImageEffect, PageCurlCorner>("corner", "Corner", PageCurlCorner.BottomRight, (e, v) => e.Corner = v,
            new (string, PageCurlCorner)[] { ("Top left", PageCurlCorner.TopLeft), ("Top right", PageCurlCorner.TopRight), ("Bottom right", PageCurlCorner.BottomRight), ("Bottom left", PageCurlCorner.BottomLeft) }),
        EffectParameters.FloatSlider<PageCurlImageEffect>("curl_size", "Curl size", 5f, 80f, 28f, (e, v) => e.CurlSize = v),
        EffectParameters.FloatSlider<PageCurlImageEffect>("curl_depth", "Curl depth", 0f, 100f, 55f, (e, v) => e.CurlDepth = v),
        EffectParameters.FloatSlider<PageCurlImageEffect>("shadow_strength", "Shadow strength", 0f, 100f, 60f, (e, v) => e.ShadowStrength = v),
        EffectParameters.Color<PageCurlImageEffect>("back_color", "Back color", new SKColor(248, 244, 236), (e, v) => e.BackColor = v)
    ];

    public PageCurlCorner Corner { get; set; } = PageCurlCorner.BottomRight;
    public float CurlSize { get; set; } = 28f;
    public float CurlDepth { get; set; } = 55f;
    public float ShadowStrength { get; set; } = 60f;
    public SKColor BackColor { get; set; } = new(248, 244, 236);

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        float curlSize = Math.Max(12f, Math.Min(width, height) * Math.Clamp(CurlSize, 5f, 80f) / 100f);
        float curlDepth01 = Math.Clamp(CurlDepth, 0f, 100f) / 100f;
        float shadow01 = Math.Clamp(ShadowStrength, 0f, 100f) / 100f;
        float foldLine = curlSize * (0.78f + (curlDepth01 * 0.22f));
        float shadowWidth = Math.Max(4f, curlSize * (0.08f + (shadow01 * 0.20f) + (curlDepth01 * 0.10f)));

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                SKColor original = srcPixels[row + x];
                ToCornerLocal(Corner, width, height, x, y, out float localX, out float localY);

                if (localX > curlSize + shadowWidth || localY > curlSize + shadowWidth)
                {
                    dstPixels[row + x] = original;
                    continue;
                }

                float diagonal = localX + localY;
                if (diagonal <= foldLine)
                {
                    float reflectedX = foldLine - localY;
                    float reflectedY = foldLine - localX;
                    FromCornerLocal(Corner, width, height, reflectedX, reflectedY, out float sampleX, out float sampleY);

                    SKColor reflected = DistortionEffectHelper.SampleClamped(srcPixels, width, height, sampleX, sampleY);
                    float curlMix = 0.28f + (curlDepth01 * 0.28f) + ((1f - (diagonal / foldLine)) * 0.18f);
                    SKColor curled = DistortionEffectHelper.Blend(reflected, BackColor, curlMix);

                    float highlight = (0.05f + (curlDepth01 * 0.09f)) *
                        ProceduralEffectHelper.SmoothStep(foldLine * 0.55f, 0f, diagonal);

                    dstPixels[row + x] = DistortionEffectHelper.Blend(
                        curled,
                        new SKColor(255, 255, 255, curled.Alpha),
                        highlight);
                }
                else if (diagonal < foldLine + shadowWidth)
                {
                    float fade = 1f - ((diagonal - foldLine) / shadowWidth);
                    float shade = 1f - (shadow01 * fade * 0.42f);
                    dstPixels[row + x] = DistortionEffectHelper.MultiplyRgb(original, shade);
                }
                else
                {
                    dstPixels[row + x] = original;
                }
            }
        });

        return DistortionEffectHelper.CreateBitmap(source, width, height, dstPixels);
    }

    private static void ToCornerLocal(PageCurlCorner corner, int width, int height, float x, float y, out float localX, out float localY)
    {
        switch (corner)
        {
            case PageCurlCorner.TopLeft:
                localX = x;
                localY = y;
                break;
            case PageCurlCorner.TopRight:
                localX = (width - 1) - x;
                localY = y;
                break;
            case PageCurlCorner.BottomRight:
                localX = (width - 1) - x;
                localY = (height - 1) - y;
                break;
            default:
                localX = x;
                localY = (height - 1) - y;
                break;
        }
    }

    private static void FromCornerLocal(PageCurlCorner corner, int width, int height, float localX, float localY, out float x, out float y)
    {
        switch (corner)
        {
            case PageCurlCorner.TopLeft:
                x = localX;
                y = localY;
                break;
            case PageCurlCorner.TopRight:
                x = (width - 1) - localX;
                y = localY;
                break;
            case PageCurlCorner.BottomRight:
                x = (width - 1) - localX;
                y = (height - 1) - localY;
                break;
            default:
                x = localX;
                y = (height - 1) - localY;
                break;
        }
    }
}