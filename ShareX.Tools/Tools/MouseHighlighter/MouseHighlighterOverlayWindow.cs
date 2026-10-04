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
using ShareX.Platform;
using SkiaSharp;
using System.Drawing;

namespace ShareX.Tools;

/// <summary>Draws the highlights for one screen with Skia into the platform's click through overlay (IWindowService.CreateOverlay).</summary>
internal sealed class MouseHighlighterOverlayWindow : IDisposable
{
    private readonly Func<MouseHighlighterFrame> _getFrame;
    private readonly Rectangle _screenBounds;
    private readonly IScreenOverlay _overlay;
    private SKSurface? _surface;
    private OverlayBuffer _buffer;
    private bool _disposed;

    public MouseHighlighterOverlayWindow(MouseHighlighterService service, PixelRect bounds)
        : this(new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height),
            PlatformServices.Current.Windows.CreateOverlay(new PlatformRectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height)),
            () => new MouseHighlighterFrame(service.Options, service.CursorPosition, service.Highlights, service.Time))
    {
    }

    internal MouseHighlighterOverlayWindow(Rectangle screenBounds, IScreenOverlay overlay, Func<MouseHighlighterFrame> getFrame)
    {
        _screenBounds = screenBounds;
        _overlay = overlay;
        _getFrame = getFrame;
    }

    public void Refresh()
    {
        if (_disposed) return;
        MouseHighlighterFrame frame = _getFrame();
        Rectangle bounds = MouseHighlighterRenderer.GetEffectBounds(_screenBounds, frame);
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            _overlay.Hide();
            return;
        }

        OverlayBuffer buffer = _overlay.GetBuffer(bounds.Width, bounds.Height);
        if (_surface == null || buffer != _buffer)
        {
            // The overlay reallocated its buffer; wrap the new memory.
            _surface?.Dispose();
            _buffer = buffer;
            _surface = SKSurface.Create(new SKImageInfo(buffer.Stride / 4, buffer.Height, SKColorType.Bgra8888, SKAlphaType.Premul),
                buffer.Pixels, buffer.Stride) ?? throw new InvalidOperationException("Could not create the mouse highlighter surface.");
        }

        SKCanvas canvas = _surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        canvas.Save();
        canvas.Translate(-bounds.X, -bounds.Y);
        MouseHighlighterRenderer.Draw(canvas, frame);
        canvas.Restore();
        canvas.Flush();

        _overlay.Present(new PlatformRectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _surface?.Dispose();
        _surface = null;
        _buffer = default;
        _overlay.Dispose();
    }
}
