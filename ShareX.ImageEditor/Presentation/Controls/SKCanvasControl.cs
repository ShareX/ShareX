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
using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.Controls;

/// <summary>
/// A control that allows direct SkiaSharp rendering into a WriteableBitmap.
/// This acts as the high-performance raster layer.
/// </summary>
public class SKCanvasControl : Control, IDisposable
{
    private WriteableBitmap? _bitmap;
    private object _lock = new object();

    /// <summary>
    /// Initializes or resizes the backing store.
    /// </summary>
    public void Initialize(int width, int height)
    {
        if (width <= 0 || height <= 0) return;

        lock (_lock)
        {
            if (_bitmap?.PixelSize.Width == width && _bitmap?.PixelSize.Height == height)
                return;

            _bitmap?.Dispose();
            // Create a WriteableBitmap with Bgra8888 which is standard for Skia/Avalonia interop
            _bitmap = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
        }

        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        if (_bitmap != null)
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
    }
}