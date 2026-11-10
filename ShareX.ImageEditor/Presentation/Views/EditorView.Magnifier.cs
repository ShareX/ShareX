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
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.VisualTree;
using ShareX.AvaloniaUI.Controls;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Presentation.ViewModels;
using SkiaSharp;
using System;

namespace ShareX.ImageEditor.Presentation.Views;

// The magnifier near the cursor, the same as the region capture magnifier and using its settings.
public partial class EditorView
{
    // Same limits as the region capture magnifier.
    private const int MagnifierSizeMinimum = 50;
    private const int MagnifierSizeMaximum = 500;
    private const int MagnifierPixelCountMinimum = 3;
    private const int MagnifierPixelCountMaximum = 35;
    private const int MagnifierPixelSizeMinimum = 3;
    private const double MagnifierPointerOffset = 18;

    private Canvas _magnifierHost = null!;
    private Grid _magnifierView = null!;
    private Image _magnifierImage = null!;
    private MagnifierPixelGrid _magnifierPixelGrid = null!;
    private WriteableBitmap? _magnifierBitmap;
    private int _magnifierSize;
    private bool _magnifierSquare;

    private void InitializeMagnifier()
    {
        _magnifierHost = this.FindControl<Canvas>("MagnifierHost")!;
        _magnifierView = this.FindControl<Grid>("MagnifierView")!;
        _magnifierImage = this.FindControl<Image>("MagnifierImage")!;
        _magnifierPixelGrid = this.FindControl<MagnifierPixelGrid>("MagnifierPixelGrid")!;

        // Drawing tools mark their pointer events as handled, so listen to handled events too.
        AddHandler(PointerMovedEvent, OnMagnifierPointerMoved, RoutingStrategies.Tunnel, handledEventsToo: true);
        PointerExited += (_, _) => HideMagnifier();
    }

    private void OnMagnifierPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!TryGetMagnifierPoint(e, out ImageEditorOptions options, out int imageX, out int imageY))
        {
            HideMagnifier();
            return;
        }

        ApplyMagnifierShape(options);
        UpdateMagnifierImage(options, imageX, imageY);
        PositionMagnifier(e.GetPosition(_magnifierHost));
        _magnifierView.IsVisible = true;
        _magnifierPixelGrid.InvalidateVisual();
    }

    private bool TryGetMagnifierPoint(PointerEventArgs e, out ImageEditorOptions options, out int imageX, out int imageY)
    {
        options = null!;
        imageX = 0;
        imageY = 0;

        // Region capture hosts this view and draws its own magnifier.
        if (_isWorkspaceHostMode || _workspaceDisposed || DataContext is not MainViewModel vm ||
            !vm.HasPreviewImage || !vm.Options.ShowMagnifier)
        {
            return false;
        }

        // Only over the canvas, not over side panels, popups or dialogs.
        if (e.Source is not Visual source ||
            !(IsInsideNamedControl(source, "EditorCanvasHost") || IsInsideNamedControl(source, "InteractionCaptureLayer")))
        {
            return false;
        }

        if (this.FindControl<Grid>("CanvasContainer") is not { } canvasContainer)
        {
            return false;
        }

        // The canvas container is one unit per image pixel; zoom is applied above it.
        Point point = e.GetPosition(canvasContainer);
        imageX = (int)Math.Floor(point.X);
        imageY = (int)Math.Floor(point.Y);
        if (imageX < 0 || imageY < 0 || imageX >= canvasContainer.Bounds.Width || imageY >= canvasContainer.Bounds.Height)
        {
            return false;
        }

        options = vm.Options;
        return true;
    }

    private bool IsInsideNamedControl(Visual visual, string name)
    {
        Control? control = this.FindControl<Control>(name);
        return control != null && (ReferenceEquals(visual, control) || control.IsVisualAncestorOf(visual));
    }

    private void HideMagnifier()
    {
        if (_magnifierView != null)
        {
            _magnifierView.IsVisible = false;
        }
    }

    private void ApplyMagnifierShape(ImageEditorOptions options)
    {
        int size = Math.Clamp(options.MagnifierSize, MagnifierSizeMinimum, MagnifierSizeMaximum);
        bool square = options.UseSquareMagnifier;
        if (size == _magnifierSize && square == _magnifierSquare)
        {
            return;
        }

        _magnifierSize = size;
        _magnifierSquare = square;
        double outerSize = size + 2;

        Control circleOuter = this.FindControl<Control>("MagnifierCircleOuter")!;
        Control circleInner = this.FindControl<Control>("MagnifierCircleInner")!;
        Control squareOuter = this.FindControl<Control>("MagnifierSquareOuter")!;
        Control squareInner = this.FindControl<Control>("MagnifierSquareInner")!;
        Grid content = this.FindControl<Grid>("MagnifierContent")!;

        circleOuter.IsVisible = !square;
        circleInner.IsVisible = !square;
        squareOuter.IsVisible = square;
        squareInner.IsVisible = square;

        _magnifierView.Width = outerSize;
        _magnifierView.Height = outerSize;
        circleOuter.Width = outerSize;
        circleOuter.Height = outerSize;
        squareOuter.Width = outerSize;
        squareOuter.Height = outerSize;
        circleInner.Width = size;
        circleInner.Height = size;
        squareInner.Width = size;
        squareInner.Height = size;
        content.Width = size;
        content.Height = size;
        _magnifierImage.Width = size;
        _magnifierImage.Height = size;
        content.Clip = square
            ? new RectangleGeometry(new Rect(0, 0, size, size))
            : new EllipseGeometry(new Rect(0, 0, size, size));
    }

    private unsafe void UpdateMagnifierImage(ImageEditorOptions options, int imageX, int imageY)
    {
        int count = GetMagnifierPixelCount(options.MagnifierPixelCount);
        if (_magnifierBitmap?.PixelSize != new PixelSize(count, count))
        {
            _magnifierBitmap?.Dispose();
            _magnifierBitmap = new WriteableBitmap(new PixelSize(count, count), new Vector(96, 96),
                PixelFormat.Bgra8888, AlphaFormat.Unpremul);
            _magnifierImage.Source = _magnifierBitmap;
            _magnifierPixelGrid.PixelCount = count;
        }

        int radius = count / 2;
        using ILockedFramebuffer framebuffer = _magnifierBitmap.Lock();
        byte* destination = (byte*)framebuffer.Address;

        for (int y = 0; y < count; y++)
        {
            byte* row = destination + y * framebuffer.RowBytes;

            for (int x = 0; x < count; x++)
            {
                // Pixels outside the image are transparent, so the white background shows there.
                SKColor color = GetWorkspacePixel(imageX + x - radius, imageY + y - radius);
                int offset = x * 4;
                row[offset] = color.Blue;
                row[offset + 1] = color.Green;
                row[offset + 2] = color.Red;
                row[offset + 3] = color.Alpha;
            }
        }

        _magnifierImage.InvalidateVisual();
    }

    // Same rule as the region capture magnifier: an odd count, with each magnified pixel at least 3 screen pixels wide.
    private int GetMagnifierPixelCount(int requestedCount)
    {
        double scale = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1;
        if (!double.IsFinite(scale) || scale <= 0)
        {
            scale = 1;
        }

        int maximum = Math.Clamp((int)Math.Floor(_magnifierSize * scale / MagnifierPixelSizeMinimum),
            MagnifierPixelCountMinimum, MagnifierPixelCountMaximum);
        if ((maximum & 1) == 0)
        {
            maximum--;
        }

        int count = Math.Clamp(requestedCount, MagnifierPixelCountMinimum, maximum);
        if ((count & 1) == 0)
        {
            count = count < maximum ? count + 1 : count - 1;
        }

        return count;
    }

    private void PositionMagnifier(Point pointer)
    {
        Rect area = new Rect(_magnifierHost.Bounds.Size);
        double size = _magnifierView.Width;
        double x = pointer.X + MagnifierPointerOffset;
        double y = pointer.Y + MagnifierPointerOffset;

        if (x + size > area.Right)
        {
            x = pointer.X - size - MagnifierPointerOffset;
        }

        if (y + size > area.Bottom)
        {
            y = pointer.Y - size - MagnifierPointerOffset;
        }

        x = Math.Clamp(x, area.Left, Math.Max(area.Left, area.Right - size));
        y = Math.Clamp(y, area.Top, Math.Max(area.Top, area.Bottom - size));

        // Keep the magnifier on whole screen pixels so the pixel grid stays sharp.
        double scale = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1;
        if (double.IsFinite(scale) && scale > 0)
        {
            x = Math.Round(x * scale) / scale;
            y = Math.Round(y * scale) / scale;
        }

        Canvas.SetLeft(_magnifierView, x);
        Canvas.SetTop(_magnifierView, y);
    }

    private void DisposeMagnifier()
    {
        if (_magnifierImage != null)
        {
            _magnifierImage.Source = null;
        }

        _magnifierBitmap?.Dispose();
        _magnifierBitmap = null;
    }
}
