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
/// Blur annotation - applies blur to the region
/// </summary>
public partial class BlurAnnotation : BaseEffectAnnotation
{
    public BlurAnnotation()
    {
        ToolType = EditorTool.Blur;
        StrokeColor = "#00000000"; // Transparent border
        StrokeWidth = 0;
        Amount = 10; // Default blur radius
    }

    internal static SKBitmap? CreateBlurredSourceCache(SKBitmap source, float blurAmount)
    {
        if (source == null) return null;

        float blurSigma = Math.Max(0, blurAmount);
        if (blurSigma <= 0)
        {
            return source.Copy();
        }

        int padding = (int)Math.Ceiling(blurSigma * 3);
        int extendedWidth = source.Width + (padding * 2);
        int extendedHeight = source.Height + (padding * 2);

        using var extendedSurface = SKSurface.Create(new SKImageInfo(extendedWidth, extendedHeight));
        if (extendedSurface == null)
        {
            return null;
        }

        var extendedCanvas = extendedSurface.Canvas;

        using (var clampShader = source.ToShader(SKShaderTileMode.Clamp, SKShaderTileMode.Clamp))
        using (var fillPaint = new SKPaint { Shader = clampShader })
        {
            extendedCanvas.Save();
            extendedCanvas.Translate(padding, padding);
            extendedCanvas.DrawRect(new SKRect(-padding, -padding, source.Width + padding, source.Height + padding), fillPaint);
            extendedCanvas.Restore();
        }

        using var blurSurface = SKSurface.Create(new SKImageInfo(extendedWidth, extendedHeight));
        if (blurSurface == null)
        {
            return null;
        }

        using (var extendedImage = extendedSurface.Snapshot())
        using (var extendedBitmap = SKBitmap.FromImage(extendedImage))
        using (var ownedImageFilter1 = SKImageFilter.CreateBlur(blurSigma, blurSigma))
        using (var blurPaint = new SKPaint { ImageFilter = ownedImageFilter1 })
        {
            blurSurface.Canvas.DrawBitmap(extendedBitmap, 0, 0, blurPaint);
        }

        using var blurredImage = blurSurface.Snapshot();
        using var blurredBitmap = SKBitmap.FromImage(blurredImage);

        var fullSourceRect = new SKRectI(padding, padding, padding + source.Width, padding + source.Height);
        var cachedBlurredSource = new SKBitmap(source.Width, source.Height);

        if (!blurredBitmap.ExtractSubset(cachedBlurredSource, fullSourceRect))
        {
            cachedBlurredSource.Dispose();
            return null;
        }

        return cachedBlurredSource;
    }

    internal override SKBitmap? CreateInteractionCacheBitmap(SKBitmap source)
    {
        return CreateBlurredSourceCache(source, Amount);
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

    /// <summary>
    /// Update the internal blurred bitmap based on the source image
    /// </summary>
    /// <param name="source">The full source image (SKBitmap)</param>
    public override void UpdateEffect(SKBitmap source)
    {
        if (source == null) return;

        if (RotationAngle != 0)
        {
            using var blurredSource = CreateBlurredSourceCache(source, Amount);
            if (blurredSource == null) return;

            UpdateEffectFromProcessedSource(source, blurredSource);
            return;
        }

        var rect = GetBounds();
        int fullW = (int)rect.Width;
        int fullH = (int)rect.Height;
        if (fullW <= 0 || fullH <= 0) return;

        // Convert annotation bounds to integer rect
        var annotationRect = new SKRectI((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom);

        // Find intersection with source image bounds
        var validRect = annotationRect;
        validRect.Intersect(new SKRectI(0, 0, source.Width, source.Height));

        // Create result bitmap at FULL annotation size (including out-of-bounds areas)
        var result = new SKBitmap(fullW, fullH);
        result.Erase(SKColors.Transparent);

        // If no valid intersection, return empty bitmap
        if (validRect.Width <= 0 || validRect.Height <= 0)
        {
            EffectBitmap?.Dispose();
            EffectBitmap = result;
            return;
        }

        // Calculate blur parameters
        var blurSigma = Amount;
        var padding = (int)System.Math.Ceiling(blurSigma * 3);

        // Calculate padded region for blur context (around the valid region)
        var wantedPaddedRect = new SKRectI(
            validRect.Left - padding,
            validRect.Top - padding,
            validRect.Right + padding,
            validRect.Bottom + padding
        );

        // Clamp padded region to source bounds
        var actualPaddedRect = wantedPaddedRect;
        actualPaddedRect.Intersect(new SKRectI(0, 0, source.Width, source.Height));

        if (actualPaddedRect.Width <= 0 || actualPaddedRect.Height <= 0)
        {
            EffectBitmap?.Dispose();
            EffectBitmap = result;
            return;
        }

        // Extract padded region from source
        using var paddedCrop = new SKBitmap(actualPaddedRect.Width, actualPaddedRect.Height);
        if (!source.ExtractSubset(paddedCrop, actualPaddedRect))
        {
            EffectBitmap?.Dispose();
            EffectBitmap = result;
            return;
        }

        // Step 1: Create extended surface with clamped edges
        using var extendedSurface = SKSurface.Create(new SKImageInfo(wantedPaddedRect.Width, wantedPaddedRect.Height));
        var extendedCanvas = extendedSurface.Canvas;

        // Use clamp shader to extend edge pixels where padding was clipped
        using var clampShader = paddedCrop.ToShader(SKShaderTileMode.Clamp, SKShaderTileMode.Clamp);
        using var fillPaint = new SKPaint { Shader = clampShader };

        float cropOffsetX = actualPaddedRect.Left - wantedPaddedRect.Left;
        float cropOffsetY = actualPaddedRect.Top - wantedPaddedRect.Top;

        extendedCanvas.Save();
        extendedCanvas.Translate(cropOffsetX, cropOffsetY);
        extendedCanvas.DrawRect(new SKRect(-cropOffsetX, -cropOffsetY, wantedPaddedRect.Width - cropOffsetX, wantedPaddedRect.Height - cropOffsetY), fillPaint);
        extendedCanvas.Restore();

        // Step 2: Apply blur to the extended surface
        using var extendedImage = extendedSurface.Snapshot();
        using var extendedBitmap = SKBitmap.FromImage(extendedImage);

        using var blurSurface = SKSurface.Create(new SKImageInfo(wantedPaddedRect.Width, wantedPaddedRect.Height));
        var blurCanvas = blurSurface.Canvas;

        using var blurPaint = new SKPaint();
        using var ownedImageFilter2 = SKImageFilter.CreateBlur(blurSigma, blurSigma);
        blurPaint.ImageFilter = ownedImageFilter2;
        blurCanvas.DrawBitmap(extendedBitmap, 0, 0, blurPaint);

        // Step 3: Extract the valid region from blurred result
        using var blurredImage = blurSurface.Snapshot();
        using var blurredBitmap = SKBitmap.FromImage(blurredImage);

        // The valid region within the blurred bitmap
        var validInBlurred = new SKRectI(
            padding,
            padding,
            padding + validRect.Width,
            padding + validRect.Height
        );
        validInBlurred.Intersect(new SKRectI(0, 0, blurredBitmap.Width, blurredBitmap.Height));

        if (validInBlurred.Width <= 0 || validInBlurred.Height <= 0)
        {
            EffectBitmap?.Dispose();
            EffectBitmap = result;
            return;
        }

        using var blurredRegion = new SKBitmap(validInBlurred.Width, validInBlurred.Height);
        if (!blurredBitmap.ExtractSubset(blurredRegion, validInBlurred))
        {
            EffectBitmap?.Dispose();
            EffectBitmap = result;
            return;
        }

        // Step 4: Draw blurred region into result at correct offset
        // The offset is where the valid region starts within the full annotation bounds
        int drawX = validRect.Left - annotationRect.Left;
        int drawY = validRect.Top - annotationRect.Top;

        using (var resultCanvas = new SKCanvas(result))
        {
            resultCanvas.DrawBitmap(blurredRegion, drawX, drawY);
        }

        EffectBitmap?.Dispose();
        EffectBitmap = result;
    }
}