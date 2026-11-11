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
using Avalonia.Media.Imaging;
using ShareX.OCR;

namespace ShareX.Tools.Controls;

// Shows the OCR source image fitted to the control, with the box of every recognized piece of text drawn over it.
// Hovering a box highlights it and shows its text. The mouse wheel zooms around the pointer, dragging moves a zoomed
// image and double-clicking fits it again.
public class OCRPreview : Control
{
    public static readonly StyledProperty<Bitmap?> SourceProperty =
        AvaloniaProperty.Register<OCRPreview, Bitmap?>(nameof(Source));

    public static readonly StyledProperty<IReadOnlyList<OCRTextRegion>?> RegionsProperty =
        AvaloniaProperty.Register<OCRPreview, IReadOnlyList<OCRTextRegion>?>(nameof(Regions));

    public static readonly StyledProperty<IBrush?> HighlightBrushProperty =
        AvaloniaProperty.Register<OCRPreview, IBrush?>(nameof(HighlightBrush));

    public static readonly StyledProperty<IBrush?> ImageBorderBrushProperty =
        AvaloniaProperty.Register<OCRPreview, IBrush?>(nameof(ImageBorderBrush));

    // The fitted image keeps clear of this space, such as a toolbar over the bottom edge. A zoomed image can fill it.
    public static readonly StyledProperty<Thickness> PaddingProperty =
        AvaloniaProperty.Register<OCRPreview, Thickness>(nameof(Padding));

    // While recognition runs, a scan line sweeps over the image. ScanPosition (0 top, 1 bottom) is animated by a style.
    public static readonly StyledProperty<bool> IsScanningProperty =
        AvaloniaProperty.Register<OCRPreview, bool>(nameof(IsScanning));

    public static readonly StyledProperty<double> ScanPositionProperty =
        AvaloniaProperty.Register<OCRPreview, double>(nameof(ScanPosition));

    // Small captures such as a single button are enlarged so they are readable, but not so much they turn to blocks.
    private const double MaxFitZoom = 4;
    // Zooming in stops when one image pixel is this many units wide.
    private const double MaxZoomScale = 32;
    private const double WheelZoomFactor = 1.25;
    private const double ImageCornerRadius = 6;
    private static readonly BoxShadows ImageShadow = new(new BoxShadow { Blur = 24, OffsetY = 6, Color = Color.FromArgb(90, 0, 0, 0) });
    private static readonly Cursor PanCursor = new(StandardCursorType.Hand);
    private static readonly Cursor DragCursor = new(StandardCursorType.SizeAll);

    private OCRTextRegion? _hoveredRegion;
    // Zoom relative to the fitted size, and how far the image is moved from the center of the content area.
    private double _zoom = 1;
    private Vector _pan;
    private Point? _dragStart;
    private Vector _dragStartPan;

    static OCRPreview()
    {
        AffectsRender<OCRPreview>(SourceProperty, RegionsProperty, HighlightBrushProperty, ImageBorderBrushProperty, PaddingProperty,
            IsScanningProperty, ScanPositionProperty);
    }

    public OCRPreview()
    {
        ClipToBounds = true;
    }

    public Bitmap? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public IReadOnlyList<OCRTextRegion>? Regions
    {
        get => GetValue(RegionsProperty);
        set => SetValue(RegionsProperty, value);
    }

    public IBrush? HighlightBrush
    {
        get => GetValue(HighlightBrushProperty);
        set => SetValue(HighlightBrushProperty, value);
    }

    public IBrush? ImageBorderBrush
    {
        get => GetValue(ImageBorderBrushProperty);
        set => SetValue(ImageBorderBrushProperty, value);
    }

    public Thickness Padding
    {
        get => GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    public bool IsScanning
    {
        get => GetValue(IsScanningProperty);
        set => SetValue(IsScanningProperty, value);
    }

    public double ScanPosition
    {
        get => GetValue(ScanPositionProperty);
        set => SetValue(ScanPositionProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SourceProperty)
        {
            ResetZoom();
        }

        if (change.Property == RegionsProperty || change.Property == SourceProperty)
        {
            SetHoveredRegion(null);
        }
    }

    public override void Render(DrawingContext context)
    {
        Bitmap? source = Source;

        if (source == null || !TryGetImageRect(source, out Rect imageRect, out double scale))
        {
            return;
        }

        // A soft shadow separates the image from a background of the same color, such as a white page on a light theme.
        RoundedRect roundedImageRect = new(imageRect, ImageCornerRadius);
        context.DrawRectangle(Brushes.Black, null, roundedImageRect, ImageShadow);

        // Enlarged pixels stay sharp so the text looks like the capture; shrinking is smoothed.
        BitmapInterpolationMode interpolation = scale > 1 ? BitmapInterpolationMode.None : BitmapInterpolationMode.HighQuality;

        using (context.PushClip(roundedImageRect))
        using (context.PushRenderOptions(new RenderOptions { BitmapInterpolationMode = interpolation }))
        {
            context.DrawImage(source, new Rect(source.Size), imageRect);
        }

        if (ImageBorderBrush != null)
        {
            context.DrawRectangle(null, new Pen(ImageBorderBrush, 1), new RoundedRect(imageRect.Inflate(0.5), ImageCornerRadius));
        }

        Color color = HighlightBrush is ISolidColorBrush solid ? solid.Color : Colors.DodgerBlue;

        if (IsScanning)
        {
            DrawScanLine(context, imageRect, color);
        }

        IReadOnlyList<OCRTextRegion>? regions = Regions;

        if (regions == null || regions.Count == 0)
        {
            return;
        }

        IBrush fill = new SolidColorBrush(color, 0.14);
        IBrush hoverFill = new SolidColorBrush(color, 0.32);
        IPen pen = new Pen(new SolidColorBrush(color), 1.5);
        double regionScale = scale / GetPixelScale(source);

        foreach (OCRTextRegion region in regions)
        {
            Geometry geometry = CreateRegionGeometry(region, imageRect, regionScale);
            context.DrawGeometry(ReferenceEquals(region, _hoveredRegion) ? hoverFill : fill, pen, geometry);
        }
    }

    private void DrawScanLine(DrawingContext context, Rect imageRect, Color color)
    {
        double y = imageRect.Top + (Math.Clamp(ScanPosition, 0, 1) * imageRect.Height);
        double glow = Math.Clamp(imageRect.Height / 4, 8, 56);

        using (context.PushClip(new RoundedRect(imageRect, ImageCornerRadius)))
        {
            context.FillRectangle(new SolidColorBrush(Colors.Black, 0.2), imageRect);

            LinearGradientBrush glowBrush = new()
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromArgb(0, color.R, color.G, color.B), 0),
                    new GradientStop(Color.FromArgb(110, color.R, color.G, color.B), 0.5),
                    new GradientStop(Color.FromArgb(0, color.R, color.G, color.B), 1)
                }
            };

            context.FillRectangle(glowBrush, new Rect(imageRect.Left, y - glow, imageRect.Width, glow * 2));
            context.DrawLine(new Pen(new SolidColorBrush(color), 2), new Point(imageRect.Left, y), new Point(imageRect.Right, y));
        }
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);

        if (Source != null && e.Delta.Y != 0)
        {
            ZoomAt(e.GetPosition(this), Math.Pow(WheelZoomFactor, e.Delta.Y));
            e.Handled = true;
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        PointerPointProperties properties = e.GetCurrentPoint(this).Properties;

        if (!properties.IsLeftButtonPressed && !properties.IsMiddleButtonPressed)
        {
            return;
        }

        if (e.ClickCount == 2 && properties.IsLeftButtonPressed)
        {
            ResetZoom();
            e.Handled = true;
        }
        else if (_zoom > 1 && Source != null && TryGetLayout(Source, _zoom, _pan, out _, out _, out Vector pan))
        {
            _dragStart = e.GetPosition(this);
            _dragStartPan = pan;
            e.Pointer.Capture(this);
            SetHoveredRegion(null);
            UpdateCursor();
            e.Handled = true;
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        Point position = e.GetPosition(this);

        if (_dragStart is Point dragStart && Source != null)
        {
            if (TryGetLayout(Source, _zoom, _dragStartPan + (position - dragStart), out _, out _, out Vector pan))
            {
                _pan = pan;
                InvalidateVisual();
            }

            return;
        }

        SetHoveredRegion(HitTestRegion(position));
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (_dragStart != null)
        {
            e.Pointer.Capture(null);
            EndDrag();
        }
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        EndDrag();
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        SetHoveredRegion(null);
    }

    private void EndDrag()
    {
        _dragStart = null;
        UpdateCursor();
    }

    // Zooms by the factor while the image point under the pointer stays where it is.
    private void ZoomAt(Point point, double factor)
    {
        Bitmap? source = Source;

        if (source == null || !TryGetLayout(source, _zoom, _pan, out Rect imageRect, out double scale, out _))
        {
            return;
        }

        double fitScale = scale / _zoom;
        double zoom = Math.Clamp(_zoom * factor, 1, Math.Max(1, MaxZoomScale / fitScale));

        if (zoom == _zoom)
        {
            return;
        }

        double newScale = fitScale * zoom;
        Point imagePoint = new((point.X - imageRect.X) / scale, (point.Y - imageRect.Y) / scale);
        Point topLeft = new(point.X - (imagePoint.X * newScale), point.Y - (imagePoint.Y * newScale));
        Vector pan = topLeft - GetCenteredTopLeft(source.Size * newScale);

        if (TryGetLayout(source, zoom, pan, out _, out _, out Vector clampedPan))
        {
            _zoom = zoom;
            _pan = clampedPan;
            UpdateCursor();
            SetHoveredRegion(HitTestRegion(point));
            InvalidateVisual();
        }
    }

    private void ResetZoom()
    {
        _zoom = 1;
        _pan = default;
        _dragStart = null;
        UpdateCursor();
        InvalidateVisual();
    }

    private void UpdateCursor()
    {
        Cursor = _dragStart != null ? DragCursor : _zoom > 1 ? PanCursor : null;
    }

    private void SetHoveredRegion(OCRTextRegion? region)
    {
        if (ReferenceEquals(region, _hoveredRegion))
        {
            return;
        }

        _hoveredRegion = region;
        ToolTip.SetTip(this, region?.Text);
        ToolTip.SetIsOpen(this, region != null);
        InvalidateVisual();
    }

    private OCRTextRegion? HitTestRegion(Point point)
    {
        Bitmap? source = Source;
        IReadOnlyList<OCRTextRegion>? regions = Regions;

        if (source == null || regions == null || !TryGetImageRect(source, out Rect imageRect, out double scale) || !imageRect.Contains(point))
        {
            return null;
        }

        // Back to image pixels, where the region boxes are.
        Point imagePoint = new((point.X - imageRect.X) / scale * GetPixelScale(source), (point.Y - imageRect.Y) / scale * GetPixelScale(source));

        foreach (OCRTextRegion region in regions)
        {
            if (region.Bounds.Contains((float)imagePoint.X, (float)imagePoint.Y))
            {
                return region;
            }
        }

        return null;
    }

    private bool TryGetImageRect(Bitmap source, out Rect imageRect, out double scale)
    {
        return TryGetLayout(source, _zoom, _pan, out imageRect, out scale, out _);
    }

    // Places the image for a zoom and pan. An image smaller than the content area is centered in it; a larger one is
    // kept covering it, so dragging stops at the image edges. clampedPan is the pan that gives the returned position.
    private bool TryGetLayout(Bitmap source, double zoom, Vector pan, out Rect imageRect, out double scale, out Vector clampedPan)
    {
        Size imageSize = source.Size;
        Rect content = GetContentRect();
        imageRect = default;
        scale = 0;
        clampedPan = default;

        if (imageSize.Width <= 0 || imageSize.Height <= 0 || content.Width <= 0 || content.Height <= 0)
        {
            return false;
        }

        double fitScale = Math.Min(Math.Min(content.Width / imageSize.Width, content.Height / imageSize.Height), MaxFitZoom);
        scale = fitScale * zoom;
        Size size = imageSize * scale;
        Point centered = GetCenteredTopLeft(size);
        double x = ClampAxis(centered.X + pan.X, size.Width, content.Left, content.Right);
        double y = ClampAxis(centered.Y + pan.Y, size.Height, content.Top, content.Bottom);
        imageRect = new Rect(x, y, size.Width, size.Height);
        clampedPan = new Vector(x - centered.X, y - centered.Y);
        return true;
    }

    private Rect GetContentRect()
    {
        return new Rect(Bounds.Size).Deflate(Padding);
    }

    private Point GetCenteredTopLeft(Size size)
    {
        Rect content = GetContentRect();
        return new Point(content.X + ((content.Width - size.Width) / 2), content.Y + ((content.Height - size.Height) / 2));
    }

    private static double ClampAxis(double position, double length, double start, double end)
    {
        return length <= end - start ? start + ((end - start - length) / 2) : Math.Clamp(position, end - length, start);
    }

    // Region boxes are in pixels, while the bitmap size is in device-independent units when the image has a DPI.
    private static double GetPixelScale(Bitmap source)
    {
        return source.Size.Width > 0 ? source.PixelSize.Width / source.Size.Width : 1;
    }

    private static Geometry CreateRegionGeometry(OCRTextRegion region, Rect imageRect, double scale)
    {
        StreamGeometry geometry = new();

        using (StreamGeometryContext stream = geometry.Open())
        {
            for (int i = 0; i < region.Points.Length; i++)
            {
                Point point = new(imageRect.X + (region.Points[i].X * scale), imageRect.Y + (region.Points[i].Y * scale));

                if (i == 0)
                {
                    stream.BeginFigure(point, true);
                }
                else
                {
                    stream.LineTo(point);
                }
            }

            stream.EndFigure(true);
        }

        return geometry;
    }
}
