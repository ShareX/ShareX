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
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;
using System.ComponentModel;

namespace ShareX.Tools;

/// <summary>Crop box drawn over the GIF preview. Drag on the preview to draw a new box, drag inside it to move it,
/// and drag its edges or corners to resize it. The width and height fields provide an accessible alternative.</summary>
public sealed class AnimatedGifTrimmerCropOverlay : Control, ICustomHitTest
{
    public static readonly StyledProperty<IBrush?> AccentProperty =
        AvaloniaProperty.Register<AnimatedGifTrimmerCropOverlay, IBrush?>(nameof(Accent), Brushes.DodgerBlue);
    public IBrush? Accent { get => GetValue(AccentProperty); set => SetValue(AccentProperty, value); }

    [Flags]
    private enum Edges { None = 0, Left = 1, Top = 2, Right = 4, Bottom = 8, Move = 16, New = 32 }

    private const double HandleSize = 8;
    private const double GripDistance = 8;
    private AnimatedGifTrimmerViewModel? _model;
    private Edges _drag;
    private Point _dragStart;
    private SKRectI _dragCrop;
    private Edges _cursorEdges;

    static AnimatedGifTrimmerCropOverlay()
    {
        AffectsRender<AnimatedGifTrimmerCropOverlay>(AccentProperty);
    }

    public AnimatedGifTrimmerCropOverlay()
    {
        ClipToBounds = true;
        DataContextChanged += (_, _) => Subscribe();
        AttachedToVisualTree += (_, _) => Subscribe();
        DetachedFromVisualTree += (_, _) =>
        {
            if (_model != null) _model.PropertyChanged -= ModelChanged;
            _model = null;
        };
    }

    private void Subscribe()
    {
        if (_model != null) _model.PropertyChanged -= ModelChanged;
        _model = DataContext as AnimatedGifTrimmerViewModel;
        if (_model != null) _model.PropertyChanged += ModelChanged;
        InvalidateVisual();
    }

    private void ModelChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not (nameof(AnimatedGifTrimmerViewModel.Crop) or nameof(AnimatedGifTrimmerViewModel.HasGif) or
            nameof(AnimatedGifTrimmerViewModel.CanEdit))) return;
        if (Dispatcher.UIThread.CheckAccess()) InvalidateVisual();
        else Dispatcher.UIThread.Post(() =>
        {
            if (ReferenceEquals(_model, sender)) InvalidateVisual();
        });
    }

    /// <summary>Where the preview image is drawn: centered, scaled down to fit but never up, like the Image beneath.</summary>
    private Rect ImageBounds(out double scale)
    {
        scale = 1;
        if (_model is not { SourceWidth: > 0, SourceHeight: > 0 } model) return default;
        scale = Math.Min(1, Math.Min(Bounds.Width / model.SourceWidth, Bounds.Height / model.SourceHeight));
        double width = model.SourceWidth * scale, height = model.SourceHeight * scale;
        return new Rect((Bounds.Width - width) / 2, (Bounds.Height - height) / 2, width, height);
    }

    private Rect ToView(SKRectI crop)
    {
        Rect image = ImageBounds(out double scale);
        return new Rect(image.X + crop.Left * scale, image.Y + crop.Top * scale, crop.Width * scale, crop.Height * scale);
    }

    private Point ToSource(Point point)
    {
        Rect image = ImageBounds(out double scale);
        return new Point(Math.Clamp((point.X - image.X) / scale, 0, _model!.SourceWidth),
            Math.Clamp((point.Y - image.Y) / scale, 0, _model.SourceHeight));
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (_model is not { HasGif: true } model || model.SourceWidth <= 0) return;
        Rect image = ImageBounds(out _);
        Rect crop = ToView(model.Crop);

        IBrush shade = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0));
        context.DrawRectangle(shade, null, new Rect(image.X, image.Y, image.Width, crop.Y - image.Y));
        context.DrawRectangle(shade, null, new Rect(image.X, crop.Bottom, image.Width, image.Bottom - crop.Bottom));
        context.DrawRectangle(shade, null, new Rect(image.X, crop.Y, crop.X - image.X, crop.Height));
        context.DrawRectangle(shade, null, new Rect(crop.Right, crop.Y, image.Right - crop.Right, crop.Height));

        // Outline the box with the accent's halo color so it stays visible over any frame content.
        Color accentColor = AccentPalette.GetBrushColor(Accent) ?? Colors.DodgerBlue;
        IBrush halo = new SolidColorBrush(AccentPalette.GetHaloColor(accentColor));
        Rect frame = crop.Deflate(1);
        context.DrawRectangle(null, new Pen(halo, 3), frame);
        context.DrawRectangle(null, new Pen(Accent, 1.5), frame);
        if (!model.CanEdit) return;
        foreach (Point point in HandlePoints(frame))
            context.DrawRectangle(Accent, new Pen(halo, 1),
                new Rect(point.X - HandleSize / 2, point.Y - HandleSize / 2, HandleSize, HandleSize));
    }

    private static IEnumerable<Point> HandlePoints(Rect rect)
    {
        foreach (double y in new[] { rect.Top, rect.Center.Y, rect.Bottom })
            foreach (double x in new[] { rect.Left, rect.Center.X, rect.Right })
                if (x != rect.Center.X || y != rect.Center.Y) yield return new Point(x, y);
    }

    /// <summary>Takes clicks anywhere on the image, not only where the box is drawn.</summary>
    public bool HitTest(Point point) => _model is { HasGif: true } && ImageBounds(out _).Inflate(GripDistance).Contains(point);

    private void UpdateCursor(Point point)
    {
        Edges edges = EdgesAt(point);
        if (edges == _cursorEdges) return;
        _cursorEdges = edges;
        Cursor = CursorFor(edges);
    }

    private Edges EdgesAt(Point point)
    {
        if (_model is not { CanEdit: true, SourceWidth: > 0 } model) return Edges.None;
        Rect crop = ToView(model.Crop);
        if (!ImageBounds(out _).Inflate(GripDistance).Contains(point)) return Edges.None;
        bool insideY = point.Y >= crop.Top - GripDistance && point.Y <= crop.Bottom + GripDistance;
        bool insideX = point.X >= crop.Left - GripDistance && point.X <= crop.Right + GripDistance;
        Edges edges = Edges.None;
        if (insideY && Math.Abs(point.X - crop.Left) <= GripDistance) edges |= Edges.Left;
        else if (insideY && Math.Abs(point.X - crop.Right) <= GripDistance) edges |= Edges.Right;
        if (insideX && Math.Abs(point.Y - crop.Top) <= GripDistance) edges |= Edges.Top;
        else if (insideX && Math.Abs(point.Y - crop.Bottom) <= GripDistance) edges |= Edges.Bottom;
        if (edges != Edges.None) return edges;
        return crop.Contains(point) ? Edges.Move : Edges.New;
    }

    private static Cursor? CursorFor(Edges edges) => edges switch
    {
        Edges.Left | Edges.Top or Edges.Right | Edges.Bottom => new Cursor(StandardCursorType.TopLeftCorner),
        Edges.Right | Edges.Top or Edges.Left | Edges.Bottom => new Cursor(StandardCursorType.TopRightCorner),
        Edges.Left or Edges.Right => new Cursor(StandardCursorType.SizeWestEast),
        Edges.Top or Edges.Bottom => new Cursor(StandardCursorType.SizeNorthSouth),
        Edges.Move => new Cursor(StandardCursorType.SizeAll),
        Edges.New => new Cursor(StandardCursorType.Cross),
        _ => null
    };

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (_model == null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        Point point = e.GetPosition(this);
        _drag = EdgesAt(point);
        if (_drag == Edges.None) return;
        _dragStart = ToSource(point);
        _dragCrop = _model.Crop;
        e.Pointer.Capture(this);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        Point point = e.GetPosition(this);
        if (_drag == Edges.None)
        {
            UpdateCursor(point);
            return;
        }
        if (_model == null) return;
        Point current = ToSource(point);
        int x = (int)Math.Round(current.X), y = (int)Math.Round(current.Y);
        SKRectI crop = _dragCrop;
        if (_drag == Edges.New)
        {
            int startX = (int)Math.Round(_dragStart.X), startY = (int)Math.Round(_dragStart.Y);
            crop = new SKRectI(Math.Min(startX, x), Math.Min(startY, y), Math.Max(startX, x), Math.Max(startY, y));
        }
        else if (_drag == Edges.Move)
        {
            int dx = (int)Math.Round(current.X - _dragStart.X), dy = (int)Math.Round(current.Y - _dragStart.Y);
            dx = Math.Clamp(dx, -crop.Left, _model.SourceWidth - crop.Right);
            dy = Math.Clamp(dy, -crop.Top, _model.SourceHeight - crop.Bottom);
            crop.Offset(dx, dy);
        }
        else
        {
            if (_drag.HasFlag(Edges.Left)) crop.Left = Math.Min(x, crop.Right - 1);
            if (_drag.HasFlag(Edges.Right)) crop.Right = Math.Max(x, crop.Left + 1);
            if (_drag.HasFlag(Edges.Top)) crop.Top = Math.Min(y, crop.Bottom - 1);
            if (_drag.HasFlag(Edges.Bottom)) crop.Bottom = Math.Max(y, crop.Top + 1);
        }
        // A click without a drag keeps the current box instead of making a 0 pixel one.
        if (crop.Width >= 1 && crop.Height >= 1) _model.SetCrop(crop);
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_drag == Edges.None) return;
        _drag = Edges.None;
        e.Pointer.Capture(null);
        UpdateCursor(e.GetPosition(this));
        e.Handled = true;
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        _drag = Edges.None;
        base.OnPointerCaptureLost(e);
    }
}
