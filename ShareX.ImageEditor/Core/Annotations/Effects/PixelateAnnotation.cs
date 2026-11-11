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

namespace ShareX.ImageEditor.Core.Annotations;

/// <summary>
/// Pixelate annotation - applies pixelation to the region
/// </summary>
public partial class PixelateAnnotation : BaseEffectAnnotation
{
    public PixelateAnnotation()
    {
        ToolType = EditorTool.Pixelate;
        StrokeColor = "#00000000";
        StrokeWidth = 0;
        Amount = 10; // Default pixel size
    }

    internal static SKBitmap? CreatePixelatedSourceCache(SKBitmap source, float pixelAmount)
    {
        if (source == null) return null;

        var pixelSize = (int)Math.Max(1, pixelAmount);
        int w = Math.Max(1, source.Width / pixelSize);
        int h = Math.Max(1, source.Height / pixelSize);

        var info = new SKImageInfo(w, h);
        using var small = source.Resize(info, new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.None));
        if (small == null)
        {
            return null;
        }

        info = new SKImageInfo(source.Width, source.Height);
        return small.Resize(info, new SKSamplingOptions(SKFilterMode.Nearest, SKMipmapMode.None));
    }

    internal override SKBitmap? CreateInteractionCacheBitmap(SKBitmap source)
    {
        return CreatePixelatedSourceCache(source, Amount);
    }

    internal override void UpdateEffectFromInteractionCache(SKBitmap source, SKBitmap cachedEffectBitmap)
    {
        if (RotationAngle != 0)
        {
            UpdateEffectFromProcessedSource(source, cachedEffectBitmap);
            return;
        }

        UpdateEffectFromAlignedCache(source, cachedEffectBitmap);
    }

    public override void UpdateEffect(SKBitmap source)
    {
        if (source == null) return;

        if (RotationAngle != 0)
        {
            using var pixelatedSource = CreatePixelatedSourceCache(source, Amount);
            if (pixelatedSource == null) return;

            UpdateEffectFromProcessedSource(source, pixelatedSource);
            return;
        }

        // Cover every image pixel the annotation touches
        var annotationRect = GetEffectPixelBounds();
        int fullW = annotationRect.Width;
        int fullH = annotationRect.Height;
        if (fullW <= 0 || fullH <= 0) return;

        // Find intersection with source image bounds
        var validRect = annotationRect;
        validRect.Intersect(new SKRectI(0, 0, source.Width, source.Height));

        // Create result bitmap at FULL annotation size
        var result = new SKBitmap(fullW, fullH);
        result.Erase(SKColors.Transparent);

        if (validRect.Width <= 0 || validRect.Height <= 0)
        {
            SetEffectBitmap(result, annotationRect);
            return;
        }

        using var crop = new SKBitmap(validRect.Width, validRect.Height);
        if (!source.ExtractSubset(crop, validRect))
        {
            SetEffectBitmap(result, annotationRect);
            return;
        }

        // Pixelate logic: Downscale then upscale
        var pixelSize = (int)Math.Max(1, Amount);
        int w = Math.Max(1, crop.Width / pixelSize);
        int h = Math.Max(1, crop.Height / pixelSize);

        var info = new SKImageInfo(w, h);
        using var small = crop.Resize(info, new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.None));
        if (small == null)
        {
            SetEffectBitmap(result, annotationRect);
            return;
        }

        info = new SKImageInfo(crop.Width, crop.Height);
        using var pixelated = small.Resize(info, new SKSamplingOptions(SKFilterMode.Nearest, SKMipmapMode.None)); // Nearest neighbor upscale
        if (pixelated == null)
        {
            SetEffectBitmap(result, annotationRect);
            return;
        }

        // Draw pixelated region into result at correct offset
        int drawX = validRect.Left - annotationRect.Left;
        int drawY = validRect.Top - annotationRect.Top;

        using (var resultCanvas = new SKCanvas(result))
        {
            resultCanvas.DrawBitmap(pixelated, drawX, drawY);
        }

        SetEffectBitmap(result, annotationRect);
    }
}