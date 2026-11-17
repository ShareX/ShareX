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
/// Magnify annotation - zooms into the area
/// </summary>
public partial class MagnifyAnnotation : BaseEffectAnnotation
{
    public BorderStyle BorderStyle { get; set; } = BorderStyle.Solid;

    public MagnifyAnnotation()
    {
        ToolType = EditorTool.Magnify;
        StrokeColor = "#00000000"; // Transparent border
        StrokeWidth = 0;
        Amount = 2.0f; // Zoom level (2x)
    }

    internal override string GetInteractionCacheKey()
    {
        return GetType().FullName ?? nameof(MagnifyAnnotation);
    }

    internal override SKBitmap? CreateInteractionCacheBitmap(SKBitmap source)
    {
        return source?.Copy();
    }

    internal override void UpdateEffectFromInteractionCache(SKBitmap source, SKBitmap cachedEffectBitmap)
    {
        UpdateEffectCore(source, cachedEffectBitmap);
    }

    public override void UpdateEffect(SKBitmap source)
    {
        if (source == null) return;

        UpdateEffectCore(source, source);
    }

    private void UpdateEffectCore(SKBitmap source, SKBitmap drawSource)
    {
        if (source == null || drawSource == null) return;

        var rect = GetBounds();
        int fullW = (int)rect.Width;
        int fullH = (int)rect.Height;
        if (fullW <= 0 || fullH <= 0) return;

        if (RotationAngle != 0)
        {
            UpdateRotatedEffect(drawSource, rect, fullW, fullH);
            return;
        }

        // Convert annotation bounds to integer rect
        var annotationRect = new SKRectI((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom);

        // Find intersection with source image bounds
        var validRect = annotationRect;
        validRect.Intersect(new SKRectI(0, 0, source.Width, source.Height));

        // Create result bitmap at FULL annotation size
        var result = new SKBitmap(fullW, fullH);
        result.Erase(SKColors.Transparent);

        if (validRect.Width <= 0 || validRect.Height <= 0)
        {
            EffectBitmap?.Dispose();
            EffectBitmap = result;
            return;
        }

        // For magnification, capture a SMALLER area from the CENTER OF THE VALID REGION and scale it UP
        // Use the valid region's center to avoid capturing outside the image
        float zoom = Math.Max(1.0f, Amount);

        // Calculate capture size based on valid region (not full annotation)
        float captureWidth = validRect.Width / zoom;
        float captureHeight = validRect.Height / zoom;

        // Center the capture within the valid region
        float centerX = validRect.Left + validRect.Width / 2f;
        float centerY = validRect.Top + validRect.Height / 2f;

        float captureX = centerX - (captureWidth / 2);
        float captureY = centerY - (captureHeight / 2);

        var captureRect = new SKRectI(
            (int)captureX,
            (int)captureY,
            (int)(captureX + captureWidth),
            (int)(captureY + captureHeight)
        );

        // Ensure capture is within source bounds
        captureRect.Intersect(new SKRectI(0, 0, source.Width, source.Height));

        if (captureRect.Width <= 0 || captureRect.Height <= 0)
        {
            EffectBitmap?.Dispose();
            EffectBitmap = result;
            return;
        }

        // Draw scaled content at the correct offset within the full-size result
        int drawX = validRect.Left - annotationRect.Left;
        int drawY = validRect.Top - annotationRect.Top;

        using (var resultCanvas = new SKCanvas(result))
        using (var paint = new SKPaint())
        using (var drawSourceImage = SKImage.FromBitmap(drawSource))
        {
            var sourceRect = new SKRect(captureRect.Left, captureRect.Top, captureRect.Right, captureRect.Bottom);
            var destinationRect = new SKRect(drawX, drawY, drawX + validRect.Width, drawY + validRect.Height);
            resultCanvas.DrawImage(drawSourceImage, sourceRect, destinationRect, new SKSamplingOptions(SKCubicResampler.Mitchell), paint);
        }

        EffectBitmap?.Dispose();
        EffectBitmap = result;
    }

    private void UpdateRotatedEffect(SKBitmap drawSource, SKRect rect, int fullW, int fullH)
    {
        var result = new SKBitmap(fullW, fullH);
        result.Erase(SKColors.Transparent);

        float zoom = Math.Max(1.0f, Amount);
        float localCenterX = fullW / 2f;
        float localCenterY = fullH / 2f;
        float worldCenterX = rect.MidX;
        float worldCenterY = rect.MidY;

        using (var resultCanvas = new SKCanvas(result))
        using (var paint = new SKPaint())
        using (var drawSourceImage = SKImage.FromBitmap(drawSource))
        {
            // Map source image points into the annotation's local space so the
            // magnified content follows the rotated annotation axes.
            resultCanvas.Translate(localCenterX, localCenterY);
            resultCanvas.Scale(zoom, zoom);
            resultCanvas.RotateDegrees(-RotationAngle);
            resultCanvas.Translate(-worldCenterX, -worldCenterY);
            resultCanvas.DrawImage(drawSourceImage, 0, 0, new SKSamplingOptions(SKCubicResampler.Mitchell), paint);
        }

        EffectBitmap?.Dispose();
        EffectBitmap = result;
    }
}