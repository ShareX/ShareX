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
// Hovering a box highlights it and shows its text.
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

    // While recognition runs, a scan line sweeps over the image. ScanPosition (0 top, 1 bottom) is animated by a style.
    public static readonly StyledProperty<bool> IsScanningProperty =
        AvaloniaProperty.Register<OCRPreview, bool>(nameof(IsScanning));

    public static readonly StyledProperty<double> ScanPositionProperty =
        AvaloniaProperty.Register<OCRPreview, double>(nameof(ScanPosition));

    // Small captures such as a single button are enlarged so they are readable, but not so much they turn to blocks.
    private const double MaxZoom = 4;
    private const double ImageCornerRadius = 6;
    private static readonly BoxShadows ImageShadow = new(new BoxShadow { Blur = 24, OffsetY = 6, Color = Color.FromArgb(90, 0, 0, 0) });
    private OCRTextRegion? _hoveredRegion;

    static OCRPreview()
    {
        AffectsRender<OCRPreview>(SourceProperty, RegionsProperty, HighlightBrushProperty, ImageBorderBrushProperty, IsScanningProperty, ScanPositionProperty);
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

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        SetHoveredRegion(HitTestRegion(e.GetPosition(this)));
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        SetHoveredRegion(null);
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

        if (source == null || regions == null || !TryGetImageRect(source, out Rect imageRect, out double scale))
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
        Size imageSize = source.Size;
        Size available = Bounds.Size;
        imageRect = default;
        scale = 0;

        if (imageSize.Width <= 0 || imageSize.Height <= 0 || available.Width <= 0 || available.Height <= 0)
        {
            return false;
        }

        scale = Math.Min(Math.Min(available.Width / imageSize.Width, available.Height / imageSize.Height), MaxZoom);
        Size size = imageSize * scale;
        imageRect = new Rect((available.Width - size.Width) / 2, (available.Height - size.Height) / 2, size.Width, size.Height);
        return true;
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
