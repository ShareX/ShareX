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

using ShareX.ImageEditor.Core.Annotations;
using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.Controls;

public class SpotlightOverlayControl : SKCanvasControl
{
    private SKBitmap? _cachedBlurredSource;
    private SKBitmap? _cachedSourceReference;
    private float _cachedBlurAmount = -1;

    public void UpdateSpotlights(IReadOnlyList<SpotlightAnnotation> spotlights, SKBitmap? sourceImage, int width, int height)
    {
        width = Math.Max(1, width);
        height = Math.Max(1, height);

        Initialize(width, height);

        if (spotlights == null || spotlights.Count == 0)
        {
            IsVisible = false;
            ClearBlurCache();
            Draw(canvas => canvas.Clear(SKColors.Transparent));
            return;
        }

        IsVisible = true;

        byte darkenOpacity = spotlights.Max(spotlight => spotlight.DarkenOpacity);
        float blurAmount = spotlights.Max(spotlight => spotlight.BlurAmount);
        SKBitmap? blurredSource = GetBlurredSource(sourceImage, blurAmount);

        Draw(canvas =>
        {
            canvas.Clear(SKColors.Transparent);

            if (blurredSource != null)
            {
                var sourceRect = new SKRect(0, 0, blurredSource.Width, blurredSource.Height);
                var destinationRect = new SKRect(0, 0, width, height);
                canvas.DrawBitmap(blurredSource, sourceRect, destinationRect);
            }

            using var darkPaint = new SKPaint
            {
                Color = new SKColor(0, 0, 0, darkenOpacity),
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            using var clearPaint = new SKPaint
            {
                BlendMode = SKBlendMode.Clear,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            canvas.DrawRect(new SKRect(0, 0, width, height), darkPaint);

            foreach (SpotlightAnnotation spotlight in spotlights)
            {
                SKRect bounds = spotlight.GetBounds();
                if (spotlight.IsEllipse)
                {
                    canvas.DrawOval(bounds, clearPaint);
                }
                else
                {
                    canvas.DrawRect(bounds, clearPaint);
                }
            }
        });
    }

    private SKBitmap? GetBlurredSource(SKBitmap? sourceImage, float blurAmount)
    {
        if (sourceImage == null || blurAmount <= 0)
        {
            ClearBlurCache();
            return null;
        }

        if (_cachedBlurredSource != null &&
            ReferenceEquals(_cachedSourceReference, sourceImage) &&
            Math.Abs(_cachedBlurAmount - blurAmount) < float.Epsilon)
        {
            return _cachedBlurredSource;
        }

        ClearBlurCache();

        _cachedBlurredSource = BlurAnnotation.CreateBlurredSourceCache(sourceImage, blurAmount);
        _cachedSourceReference = sourceImage;
        _cachedBlurAmount = blurAmount;

        return _cachedBlurredSource;
    }

    public override void Dispose()
    {
        ClearBlurCache();
        base.Dispose();
    }

    private void ClearBlurCache()
    {
        _cachedBlurredSource?.Dispose();
        _cachedBlurredSource = null;
        _cachedSourceReference = null;
        _cachedBlurAmount = -1;
    }
}
