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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.VisualTree;
using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.Controls;

/// <summary>
/// A control that allows direct SkiaSharp rendering into a WriteableBitmap.
/// This acts as the high-performance raster layer.
/// </summary>
public class SKCanvasControl : Control, IDisposable
{
    private WriteableBitmap? _bitmap;
    private DirectSource? _direct;
    private object _lock = new object();

    /// <summary>True while a bitmap is shown by <see cref="ShowDirectly"/> instead of the backing store.</summary>
    public bool HasDirectSource => _direct != null;

    /// <summary>
    /// True when <paramref name="bitmap"/> is the very bitmap shown by <see cref="ShowDirectly"/>.
    /// Only compares references: the bitmap itself may already be disposed by its owner.
    /// </summary>
    public bool IsDirectSource(SKBitmap? bitmap) => bitmap != null && ReferenceEquals(_direct?.Bitmap, bitmap);

    /// <summary>
    /// Shows a bitmap straight from its own pixels instead of a copy in the backing store.
    /// Every pixel must be opaque (alpha 255), as in a screen capture, and must not change while it is shown.
    /// <see cref="Initialize"/> goes back to the backing store. Throws, changing nothing, if the pixels cannot be shared.
    /// </summary>
    public void ShowDirectly(SKBitmap bitmap)
    {
        ArgumentNullException.ThrowIfNull(bitmap);

        DirectSource direct = new(bitmap);
        DirectSource? old;

        lock (_lock)
        {
            _bitmap?.Dispose();
            _bitmap = null;
            old = _direct;
            _direct = direct;
        }

        RetireDirectSource(old);
        InvalidateVisual();
    }

    /// <summary>
    /// Initializes or resizes the backing store.
    /// </summary>
    public void Initialize(int width, int height)
    {
        if (width <= 0 || height <= 0) return;

        lock (_lock)
        {
            RetireDirectSource(_direct);
            _direct = null;

            if (_bitmap?.PixelSize.Width == width && _bitmap?.PixelSize.Height == height)
                return;

            _bitmap?.Dispose();
            // Create a WriteableBitmap with Bgra8888 which is standard for Skia/Avalonia interop
            _bitmap = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
        }

        InvalidateVisual();
    }

    // A scene already sent to the render thread may still draw the old source, and drawing a disposed one leaves
    // the area black for a frame. Dispose it once the next frame, which no longer uses it, has been rendered.
    private void RetireDirectSource(DirectSource? source)
    {
        if (source == null) return;

        Task? rendered = null;
        try
        {
            rendered = ElementComposition.GetElementVisual(this)?.Compositor.RequestCompositionBatchCommitAsync().Rendered;
        }
        catch (Exception)
        {
            // Not attached to a compositor: nothing on screen uses it.
        }

        if (rendered == null)
        {
            source.Dispose();
            return;
        }

        // A tray-only app may not render again soon; do not hold the pixels longer than a second.
        Task.WhenAny(rendered, Task.Delay(1000)).ContinueWith(_ => source.Dispose(), TaskScheduler.Default);
    }

    public override void Render(DrawingContext context)
    {
        if (_direct != null)
        {
            context.Custom(new DirectDrawOperation(_direct, this));
        }
        else if (_bitmap != null)
        {
            // Keep raster pixels at their native size. Stretching the backing bitmap to
            // transient stale bounds causes freshly resized images (crop/cut/undo) to be
            // painted into the previous canvas size until layout catches up.
            context.DrawImage(_bitmap, new Rect(0, 0, _bitmap.PixelSize.Width, _bitmap.PixelSize.Height));
        }
    }

    /// <summary>
    /// Update the canvas using a SkiaSharp drawing action.
    /// </summary>
    public void Draw(Action<SKCanvas> drawAction)
    {
        if (_bitmap == null) return;

        lock (_lock)
        {
            using (var buffer = _bitmap.Lock())
            {
                var info = new SKImageInfo(
                    _bitmap.PixelSize.Width,
                    _bitmap.PixelSize.Height,
                    SKColorType.Bgra8888,
                    SKAlphaType.Premul);

                using (var surface = SKSurface.Create(info, buffer.Address, buffer.RowBytes))
                {
                    if (surface != null)
                    {
                        drawAction(surface.Canvas);
                    }
                }
            }
        }

        // Try to invalidate only if on UI thread, otherwise dispatcher?
        // Render method is called by UI thread. Draw might be called from Core.
        // We need to request invalidation on UI thread.
        Avalonia.Threading.Dispatcher.UIThread.Post(InvalidateVisual, Avalonia.Threading.DispatcherPriority.Render);
    }

    /// <summary>
    /// Releases resources
    /// </summary>
    public virtual void Dispose()
    {
        _bitmap?.Dispose();
        _bitmap = null;
        // At once: the owner is closing, and the pixel memory is large.
        _direct?.Dispose();
        _direct = null;
    }

    /// <summary>
    /// An image over a bitmap's own pixel memory. A second bitmap over the same memory keeps it alive until Skia
    /// lets go of the image, so the render thread or a recorded picture never reads freed memory, also when the
    /// source bitmap is disposed first.
    /// </summary>
    private sealed class DirectSource : IDisposable
    {
        private SKImage? _image;

        public DirectSource(SKBitmap bitmap)
        {
            Bitmap = bitmap;
            Width = bitmap.Width;
            Height = bitmap.Height;

            SKBitmap keepAlive = new();
            if (!bitmap.ExtractSubset(keepAlive, new SKRectI(0, 0, Width, Height)))
            {
                keepAlive.Dispose();
                throw new InvalidOperationException("Unable to share the bitmap's pixels.");
            }

            // Premultiplied, like the backing store: for opaque pixels the bytes are the same, and Skia filters
            // premultiplied and unpremultiplied images differently, so only this gives the same picture.
            SKImageInfo info = keepAlive.Info;
            if (info.AlphaType == SKAlphaType.Unpremul) info = info.WithAlphaType(SKAlphaType.Premul);

            using SKPixmap pixmap = new(info, keepAlive.GetPixels(), keepAlive.RowBytes);
            _image = SKImage.FromPixels(pixmap, (address, context) => ((SKBitmap?)context)?.Dispose(), keepAlive);
            if (_image == null)
            {
                keepAlive.Dispose();
                throw new InvalidOperationException("Unable to create an image over the bitmap's pixels.");
            }
        }

        // Only compared by reference: it may already be disposed by its owner.
        public SKBitmap Bitmap { get; }

        public int Width { get; }

        public int Height { get; }

        // Read and used only while holding a lock on this object.
        public SKImage? Image => _image;

        public void Dispose()
        {
            lock (this)
            {
                _image?.Dispose();
                _image = null;
            }
        }
    }

    /// <summary>
    /// Draws a <see cref="DirectSource"/> the way Avalonia 12.1.5 draws a bitmap (DrawingContextImpl.DrawBitmap
    /// and WriteableBitmapImpl.Draw), so the result is the same as drawing the backing store.
    /// </summary>
    private sealed class DirectDrawOperation : ICustomDrawOperation
    {
        private readonly DirectSource _source;
        private readonly BitmapInterpolationMode _interpolationMode;
        private readonly BitmapBlendingMode _blendingMode;
        private readonly EdgeMode _edgeMode;

        public DirectDrawOperation(DirectSource source, Visual visual)
        {
            _source = source;

            // A pushed render option keeps an outer value over an inner one (RenderOptions.MergeWith).
            for (Visual? current = visual; current != null; current = current.GetVisualParent())
            {
                BitmapInterpolationMode interpolationMode = RenderOptions.GetBitmapInterpolationMode(current);
                if (interpolationMode != BitmapInterpolationMode.Unspecified) _interpolationMode = interpolationMode;

                BitmapBlendingMode blendingMode = RenderOptions.GetBitmapBlendingMode(current);
                if (blendingMode != BitmapBlendingMode.Unspecified) _blendingMode = blendingMode;

                EdgeMode edgeMode = RenderOptions.GetEdgeMode(current);
                if (edgeMode != EdgeMode.Unspecified) _edgeMode = edgeMode;
            }
        }

        public Rect Bounds => new(0, 0, _source.Width, _source.Height);

        // Same as a drawn bitmap: hit inside its rectangle.
        public bool HitTest(Point p) => Bounds.Contains(p);

        public bool Equals(ICustomDrawOperation? other) => false;

        public void Dispose()
        {
        }

        public void Render(ImmediateDrawingContext context)
        {
            ISkiaSharpApiLeaseFeature? feature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (feature == null) return;

            using ISkiaSharpApiLease lease = feature.Lease();

            lock (_source)
            {
                SKImage? image = _source.Image;
                if (image == null) return;

                // Source and destination are the same size, so this is never upscaling.
                SKSamplingOptions sampling = _interpolationMode == BitmapInterpolationMode.HighQuality
                    ? new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear)
                    : _interpolationMode.ToSKSamplingOptions();

                using SKPaint paint = new()
                {
                    Color = new SKColor(255, 255, 255, (byte)(255 * lease.CurrentOpacity)),
                    BlendMode = _blendingMode.ToSKBlendMode(),
                    IsAntialias = _edgeMode != EdgeMode.Aliased
                };

                SKRect rect = new(0, 0, _source.Width, _source.Height);
                lease.SkCanvas.DrawImage(image, rect, rect, sampling, paint);
            }
        }
    }
}