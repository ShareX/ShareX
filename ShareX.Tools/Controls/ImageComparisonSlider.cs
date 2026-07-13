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
using Avalonia.Platform;
using Avalonia.VisualTree;
using ShareX.AvaloniaUI.Imaging;
using SkiaSharp;

namespace ShareX.Tools.Controls;

public class ImageComparisonSlider : Control
{
    public static readonly StyledProperty<Bitmap?> LeftImageProperty =
        AvaloniaProperty.Register<ImageComparisonSlider, Bitmap?>(nameof(LeftImage));

    public static readonly StyledProperty<Bitmap?> RightImageProperty =
        AvaloniaProperty.Register<ImageComparisonSlider, Bitmap?>(nameof(RightImage));

    public static readonly StyledProperty<double> SliderPositionProperty =
        AvaloniaProperty.Register<ImageComparisonSlider, double>(nameof(SliderPosition), 0.5);

    public static readonly StyledProperty<StretchDirection> StretchDirectionProperty =
        AvaloniaProperty.Register<ImageComparisonSlider, StretchDirection>(nameof(StretchDirection), StretchDirection.Both);

    public static readonly StyledProperty<IBrush?> CheckerboardBrushProperty =
        AvaloniaProperty.Register<ImageComparisonSlider, IBrush?>(nameof(CheckerboardBrush));

    private const double RenderCacheHeadroom = 1.25d;
    private static readonly IBrush DefaultCheckerboardBrush = CreateCheckerBrush();
    private static readonly IBrush SliderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
    private static readonly IPen SliderPen = new Pen(SliderBrush, 2);
    private static readonly IBrush HandleBrush = new SolidColorBrush(Color.FromArgb(230, 35, 35, 35));
    private static readonly IPen HandlePen = new Pen(SliderBrush, 2);
    private readonly Cursor _sliderCursor = new(StandardCursorType.SizeWestEast);
    private Bitmap? _scaledLeftImage;
    private Bitmap? _scaledRightImage;
    private PixelSize _renderCacheSize;
    private bool _isDragging;

    public ImageComparisonSlider()
    {
        ClipToBounds = true;
    }

    public Bitmap? LeftImage
    {
        get => GetValue(LeftImageProperty);
        set => SetValue(LeftImageProperty, value);
    }

    public Bitmap? RightImage
    {
        get => GetValue(RightImageProperty);
        set => SetValue(RightImageProperty, value);
    }

    public double SliderPosition
    {
        get => GetValue(SliderPositionProperty);
        set => SetValue(SliderPositionProperty, Math.Clamp(value, 0d, 1d));
    }

    public StretchDirection StretchDirection
    {
        get => GetValue(StretchDirectionProperty);
        set => SetValue(StretchDirectionProperty, value);
    }

    public IBrush? CheckerboardBrush
    {
        get => GetValue(CheckerboardBrushProperty);
        set => SetValue(CheckerboardBrushProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == LeftImageProperty || change.Property == RightImageProperty)
        {
            ClearRenderCache();
            Cursor = LeftImage != null && RightImage != null ? _sliderCursor : null;
            InvalidateVisual();
        }
        else if (change.Property == SliderPositionProperty)
        {
            InvalidateVisual();
        }
        else if (change.Property == StretchDirectionProperty)
        {
            ClearRenderCache();
            InvalidateVisual();
        }
        else if (change.Property == CheckerboardBrushProperty)
        {
            InvalidateVisual();
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        Rect bounds = new(Bounds.Size);

        Bitmap? leftImage = LeftImage;
        Bitmap? rightImage = RightImage;

        if (leftImage == null && rightImage == null)
        {
            return;
        }

        Rect imageBounds = GetImageBounds(bounds, leftImage, rightImage, StretchDirection);
        EnsureRenderCache(imageBounds);
        DrawTransparencyBackground(context, imageBounds);

        if (leftImage == null || rightImage == null)
        {
            DrawBitmap(context, GetRenderImage(leftImage ?? rightImage!), imageBounds);
            return;
        }

        DrawBitmap(context, GetRenderImage(rightImage), imageBounds);

        double sliderX = GetSliderX(imageBounds);
        using (context.PushClip(new Rect(imageBounds.Left, imageBounds.Top, Math.Max(0, sliderX - imageBounds.Left), imageBounds.Height)))
        {
            DrawTransparencyBackground(context, imageBounds);
            DrawBitmap(context, GetRenderImage(leftImage), imageBounds);
        }

        DrawSlider(context, imageBounds);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (LeftImage != null && RightImage != null && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isDragging = true;
            e.Pointer.Capture(this);
            UpdateSliderPosition(e.GetPosition(this));
            e.Handled = true;
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (_isDragging)
        {
            UpdateSliderPosition(e.GetPosition(this));
            e.Handled = true;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (_isDragging)
        {
            _isDragging = false;
            e.Pointer.Capture(null);
            UpdateSliderPosition(e.GetPosition(this));
            e.Handled = true;
        }
    }

    private static void DrawBitmap(DrawingContext context, Bitmap bitmap, Rect destination)
    {
        PixelSize pixelSize = bitmap.PixelSize;
        Rect source = new(0, 0, pixelSize.Width, pixelSize.Height);
        context.DrawImage(bitmap, source, destination);
    }

    private void DrawTransparencyBackground(DrawingContext context, Rect bounds)
    {
        context.FillRectangle(CheckerboardBrush ?? DefaultCheckerboardBrush, bounds);
    }

    private static unsafe IBrush CreateCheckerBrush()
    {
        const int cellSize = 12;
        const int patternSize = cellSize * 2;
        WriteableBitmap bitmap = new(
            new PixelSize(patternSize, patternSize),
            new Vector(96, 96),
            PixelFormat.Bgra8888,
            AlphaFormat.Opaque);

        using (ILockedFramebuffer framebuffer = bitmap.Lock())
        {
            const uint lightColor = 0xFFEBEBEB;
            const uint darkColor = 0xFFCDCDCD;

            for (int y = 0; y < patternSize; y++)
            {
                uint* row = (uint*)((byte*)framebuffer.Address + y * framebuffer.RowBytes);

                for (int x = 0; x < patternSize; x++)
                {
                    row[x] = ((x / cellSize) + (y / cellSize)) % 2 == 0 ? lightColor : darkColor;
                }
            }
        }

        return new ImageBrush(bitmap)
        {
            SourceRect = new RelativeRect(0, 0, 1, 1, RelativeUnit.Relative),
            DestinationRect = new RelativeRect(0, 0, patternSize, patternSize, RelativeUnit.Absolute),
            Stretch = Stretch.Fill,
            TileMode = TileMode.Tile
        };
    }

    private static Rect GetImageBounds(Rect bounds, Bitmap? leftImage, Bitmap? rightImage,
        StretchDirection stretchDirection)
    {
        double imageWidth = Math.Max(leftImage?.PixelSize.Width ?? 0, rightImage?.PixelSize.Width ?? 0);
        double imageHeight = Math.Max(leftImage?.PixelSize.Height ?? 0, rightImage?.PixelSize.Height ?? 0);

        if (imageWidth <= 0 || imageHeight <= 0 || bounds.Width <= 0 || bounds.Height <= 0)
        {
            return bounds;
        }

        double scale = Math.Min(bounds.Width / imageWidth, bounds.Height / imageHeight);
        scale = stretchDirection switch
        {
            StretchDirection.DownOnly => Math.Min(1d, scale),
            StretchDirection.UpOnly => Math.Max(1d, scale),
            _ => scale
        };
        double width = imageWidth * scale;
        double height = imageHeight * scale;
        double left = Math.Round(bounds.Left + (bounds.Width - width) / 2d);
        double top = Math.Round(bounds.Top + (bounds.Height - height) / 2d);
        double right = Math.Round(bounds.Left + (bounds.Width + width) / 2d);
        double bottom = Math.Round(bounds.Top + (bounds.Height + height) / 2d);

        return new Rect(left, top, Math.Max(1d, right - left), Math.Max(1d, bottom - top));
    }

    private void DrawSlider(DrawingContext context, Rect imageBounds)
    {
        double sliderX = GetSliderX(imageBounds);
        Point top = new(sliderX, imageBounds.Top);
        Point bottom = new(sliderX, imageBounds.Bottom);
        context.DrawLine(SliderPen, top, bottom);

        const double handleWidth = 34;
        const double handleHeight = 52;
        Rect handleBounds = new(
            sliderX - handleWidth / 2d,
            imageBounds.Top + imageBounds.Height / 2d - handleHeight / 2d,
            handleWidth,
            handleHeight);

        RoundedRect roundedHandle = new(handleBounds, 16);
        context.DrawRectangle(HandleBrush, HandlePen, roundedHandle);
        context.DrawLine(SliderPen, new Point(sliderX - 5, handleBounds.Top + 17), new Point(sliderX - 5, handleBounds.Bottom - 17));
        context.DrawLine(SliderPen, new Point(sliderX + 5, handleBounds.Top + 17), new Point(sliderX + 5, handleBounds.Bottom - 17));
    }

    private void UpdateSliderPosition(Point pointerPosition)
    {
        Rect imageBounds = GetImageBounds(new Rect(Bounds.Size), LeftImage, RightImage, StretchDirection);
        double newPosition = imageBounds.Width <= 0
            ? 0.5
            : Math.Clamp((pointerPosition.X - imageBounds.Left) / imageBounds.Width, 0d, 1d);

        // Pointer events can arrive more often than the control can render. Avoid
        // property notifications when the divider would remain on the same pixel.
        double currentSliderX = Math.Round(imageBounds.Left + imageBounds.Width * SliderPosition);
        double newSliderX = Math.Round(imageBounds.Left + imageBounds.Width * newPosition);

        if (currentSliderX != newSliderX)
        {
            SliderPosition = newPosition;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ClearRenderCache();
        base.OnDetachedFromVisualTree(e);
    }

    private double GetSliderX(Rect imageBounds)
    {
        return Math.Round(imageBounds.Left + imageBounds.Width * SliderPosition);
    }

    private Bitmap GetRenderImage(Bitmap image)
    {
        if (ReferenceEquals(image, LeftImage))
        {
            return _scaledLeftImage ?? image;
        }

        if (ReferenceEquals(image, RightImage))
        {
            return _scaledRightImage ?? image;
        }

        return image;
    }

    private void EnsureRenderCache(Rect imageBounds)
    {
        if (imageBounds.Width <= 0 || imageBounds.Height <= 0)
        {
            return;
        }

        double renderScaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1d;
        int requiredWidth = Math.Max(1, (int)Math.Ceiling(imageBounds.Width * renderScaling));
        int requiredHeight = Math.Max(1, (int)Math.Ceiling(imageBounds.Height * renderScaling));

        if (_renderCacheSize.Width >= requiredWidth &&
            _renderCacheSize.Height >= requiredHeight &&
            IsRenderCacheEfficient(LeftImage, _scaledLeftImage, requiredWidth, requiredHeight) &&
            IsRenderCacheEfficient(RightImage, _scaledRightImage, requiredWidth, requiredHeight))
        {
            return;
        }

        PixelSize cacheSize = new(
            Math.Max(requiredWidth, (int)Math.Ceiling(requiredWidth * RenderCacheHeadroom)),
            Math.Max(requiredHeight, (int)Math.Ceiling(requiredHeight * RenderCacheHeadroom)));

        ClearRenderCache();
        _scaledLeftImage = CreateRenderImage(LeftImage, cacheSize);
        _scaledRightImage = CreateRenderImage(RightImage, cacheSize);
        _renderCacheSize = cacheSize;
    }

    private static Bitmap? CreateRenderImage(Bitmap? image, PixelSize cacheSize)
    {
        if (image == null ||
            image.PixelSize == cacheSize ||
            GetPixelCount(image.PixelSize) <= GetPixelCount(cacheSize))
        {
            return null;
        }

        using SKBitmap sourceBitmap = BitmapConversionHelpers.ToSKBitmap(image);
        SKImageInfo scaledInfo = new(
            cacheSize.Width,
            cacheSize.Height,
            SKColorType.Bgra8888,
            SKAlphaType.Premul);
        using SKBitmap? scaledBitmap = sourceBitmap.Resize(
            scaledInfo,
            new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));

        return scaledBitmap != null ? BitmapConversionHelpers.ToAvaloniBitmap(scaledBitmap) : null;
    }

    private static bool IsRenderCacheEfficient(Bitmap? source, Bitmap? scaledImage, int requiredWidth, int requiredHeight)
    {
        if (source == null || scaledImage != null)
        {
            return true;
        }

        PixelSize requiredSize = new(requiredWidth, requiredHeight);
        return GetPixelCount(source.PixelSize) <= GetPixelCount(requiredSize) * RenderCacheHeadroom * RenderCacheHeadroom;
    }

    private static long GetPixelCount(PixelSize size)
    {
        return (long)size.Width * size.Height;
    }

    private void ClearRenderCache()
    {
        _scaledLeftImage?.Dispose();
        _scaledRightImage?.Dispose();
        _scaledLeftImage = null;
        _scaledRightImage = null;
        _renderCacheSize = default;
    }
}
