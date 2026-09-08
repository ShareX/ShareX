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
using System.Globalization;

namespace ShareX.ImageEditor.Core.Annotations;

/// <summary>
/// Base class for effect annotations (Blur, Pixelate, Highlight)
/// </summary>
public abstract partial class BaseEffectAnnotation : Annotation, IDisposable
{
    public override AnnotationCategory Category => AnnotationCategory.Effects;

    /// <summary>
    /// Effect radius / strength
    /// </summary>
    public float Amount { get; set; } = 10;

    /// <summary>
    /// Whether the effect is applied as a region (rectangle) or freehand
    /// </summary>
    public bool IsFreehand { get; set; }

    /// <summary>
    /// Whether the effect region is rendered as an ellipse instead of a rectangle.
    /// </summary>
    public bool IsEllipse { get; set; }

    /// <summary>
    /// The generated bitmap for the effect (pre-rendered effect result)
    /// </summary>
    public SKBitmap? EffectBitmap { get; protected set; }

    public override bool HitTest(SKPoint point, float tolerance = 5)
    {
        var bounds = GetBounds();

        point = GetUnrotatedPoint(point, bounds);

        var inflatedBounds = SKRect.Inflate(bounds, tolerance, tolerance);

        if (!IsEllipse)
        {
            return inflatedBounds.Contains(point);
        }

        float radiusX = inflatedBounds.Width / 2f;
        float radiusY = inflatedBounds.Height / 2f;
        if (radiusX <= 0 || radiusY <= 0)
        {
            return false;
        }

        float normalizedX = (point.X - inflatedBounds.MidX) / radiusX;
        float normalizedY = (point.Y - inflatedBounds.MidY) / radiusY;
        return normalizedX * normalizedX + normalizedY * normalizedY <= 1f;
    }

    public override Annotation Clone()
    {
        var clone = (BaseEffectAnnotation)base.Clone();
        // Don't copy the bitmap - it will be regenerated when needed
        // This avoids shared bitmap references and potential disposal issues
        clone.EffectBitmap = null;
        return clone;
    }

    /// <summary>
    /// Updates the effect bitmap based on the source image
    /// </summary>
    public virtual void UpdateEffect(SKBitmap source) { }

    internal virtual string GetInteractionCacheKey()
    {
        return $"{GetType().FullName}:{Amount.ToString("R", CultureInfo.InvariantCulture)}";
    }

    internal virtual SKBitmap? CreateInteractionCacheBitmap(SKBitmap source)
    {
        return null;
    }

    internal virtual void UpdateEffectFromInteractionCache(SKBitmap source, SKBitmap cachedEffectBitmap)
    {
        UpdateEffectFromAlignedCache(source, cachedEffectBitmap);
    }

    protected void UpdateEffectFromProcessedSource(SKBitmap source, SKBitmap processedSource)
    {
        if (source == null || processedSource == null) return;
        if (processedSource.Width != source.Width || processedSource.Height != source.Height) return;

        var rect = GetBounds();
        int fullW = (int)rect.Width;
        int fullH = (int)rect.Height;
        if (fullW <= 0 || fullH <= 0) return;

        var result = new SKBitmap(fullW, fullH);
        result.Erase(SKColors.Transparent);

        using (var canvas = new SKCanvas(result))
        {
            if (RotationAngle != 0)
            {
                float localCenterX = fullW / 2f;
                float localCenterY = fullH / 2f;
                canvas.Translate(localCenterX, localCenterY);
                canvas.RotateDegrees(-RotationAngle);
                canvas.Translate(-localCenterX, -localCenterY);
            }

            canvas.DrawBitmap(processedSource, -rect.Left, -rect.Top);
        }

        EffectBitmap?.Dispose();
        EffectBitmap = result;
    }

    protected void UpdateEffectFromAlignedCache(SKBitmap source, SKBitmap cachedEffectBitmap)
    {
        if (source == null || cachedEffectBitmap == null) return;
        if (cachedEffectBitmap.Width != source.Width || cachedEffectBitmap.Height != source.Height) return;

        var rect = GetBounds();
        int fullW = (int)rect.Width;
        int fullH = (int)rect.Height;
        if (fullW <= 0 || fullH <= 0) return;

        var annotationRect = new SKRectI((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom);
        var validRect = annotationRect;
        validRect.Intersect(new SKRectI(0, 0, source.Width, source.Height));

        var result = new SKBitmap(fullW, fullH);
        result.Erase(SKColors.Transparent);

        if (validRect.Width <= 0 || validRect.Height <= 0)
        {
            EffectBitmap?.Dispose();
            EffectBitmap = result;
            return;
        }

        using var cachedRegion = new SKBitmap(validRect.Width, validRect.Height);
        if (!cachedEffectBitmap.ExtractSubset(cachedRegion, validRect))
        {
            EffectBitmap?.Dispose();
            EffectBitmap = result;
            return;
        }

        int drawX = validRect.Left - annotationRect.Left;
        int drawY = validRect.Top - annotationRect.Top;

        using (var resultCanvas = new SKCanvas(result))
        {
            resultCanvas.DrawBitmap(cachedRegion, drawX, drawY);
        }

        EffectBitmap?.Dispose();
        EffectBitmap = result;
    }

    /// <summary>
    /// Disposes the effect bitmap
    /// </summary>
    public void DisposeEffect()
    {
        EffectBitmap?.Dispose();
        EffectBitmap = null;
    }

    /// <summary>
    /// Dispose unmanaged resources (EffectBitmap)
    /// </summary>
    public void Dispose()
    {
        DisposeEffect();
        GC.SuppressFinalize(this);
    }
}
