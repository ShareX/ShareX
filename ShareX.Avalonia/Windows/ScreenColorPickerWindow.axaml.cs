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
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ShareX.AvaloniaUI.Theming;
using ShareX.AvaloniaUI.Input;
using System.Runtime.InteropServices;

namespace ShareX.AvaloniaUI.Windows
{
    public partial class ScreenColorPickerWindow : Window
    {
        private const double PreviewOffset = 18;
        private const int MagnifierPixelCount = 15;
        private const uint BiRgb = 0;
        private const uint DibRgbColors = 0;
        private const uint SrcCopy = 0x00CC0020;
        private const uint ClrInvalid = 0xFFFFFFFF;

        private readonly ScreenColorPickerOptions _options;
        private readonly TaskCompletionSource<ScreenColorPickerResult?> _completionSource = new();
        private Border _pickerPreview = null!;
        private Border _colorPreview = null!;
        private Grid _magnifierPanel = null!;
        private Image _magnifierImage = null!;
        private TextBlock _infoText = null!;
        private WriteableBitmap? _magnifierBitmap;
        private nint _magnifierDc;
        private nint _magnifierDib;
        private nint _previousMagnifierObject;
        private nint _magnifierBits;
        private Color _currentColor;
        private PixelPoint _currentPosition;
        private PointerAction _pointerAction;
        private Color _pressedColor;
        private PixelPoint _pressedPosition;

        public ScreenColorPickerWindow() : this(new ScreenColorPickerOptions())
        {
        }

        public ScreenColorPickerWindow(ScreenColorPickerOptions options)
        {
            _options = options ?? new ScreenColorPickerOptions();
            RequestedThemeVariant = ThemeManager.GetCurrentTheme();
            AvaloniaXamlLoader.Load(this);

            _pickerPreview = this.FindControl<Border>("PickerPreview")!;
            _colorPreview = this.FindControl<Border>("ColorPreview")!;
            _magnifierPanel = this.FindControl<Grid>("MagnifierPanel")!;
            _magnifierImage = this.FindControl<Image>("MagnifierImage")!;
            _infoText = this.FindControl<TextBlock>("InfoText")!;

            _magnifierPanel.IsVisible = _options.ShowMagnifier;

            if (_options.ShowMagnifier)
            {
                _magnifierBitmap = new WriteableBitmap(
                    new PixelSize(MagnifierPixelCount, MagnifierPixelCount),
                    new Vector(96, 96),
                    PixelFormat.Bgra8888,
                    AlphaFormat.Opaque);
                _magnifierImage.Source = _magnifierBitmap;
                InitializeMagnifierCapture();
            }

            Loaded += OnLoaded;
        }

        public Task<ScreenColorPickerResult?> PickAsync(Window? owner = null)
        {
            ConfigureOverlayBounds();

            if (owner != null)
            {
                Show(owner);
            }
            else
            {
                Show();
            }

            return _completionSource.Task;
        }

        protected override void OnClosed(EventArgs e)
        {
            _completionSource.TrySetResult(null);
            DisposeMagnifierCapture();
            base.OnClosed(e);
        }

        private void OnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (!OperatingSystem.IsWindows())
            {
                Complete(null);
                return;
            }

            Activate();
            Focus();

            if (GetCursorPos(out NativePoint cursorPosition))
            {
                var screenPoint = new PixelPoint(cursorPosition.X, cursorPosition.Y);
                UpdatePicker(screenPoint, this.PointToClient(screenPoint));
            }
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            Point clientPoint = e.GetPosition(this);
            UpdatePicker(this.PointToScreen(clientPoint), clientPoint);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            PointerPoint pointerPoint = e.GetCurrentPoint(this);
            UpdatePicker(this.PointToScreen(pointerPoint.Position), pointerPoint.Position);

            if (pointerPoint.Properties.IsLeftButtonPressed)
            {
                e.Handled = true;
                _pressedColor = _currentColor;
                _pressedPosition = _currentPosition;
                _pointerAction = PointerAction.Select;
                e.Pointer.Capture(this);
                return;
            }

            if (pointerPoint.Properties.IsRightButtonPressed)
            {
                e.Handled = true;
                _pointerAction = PointerAction.Cancel;
                e.Pointer.Capture(this);
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (_pointerAction == PointerAction.None)
            {
                return;
            }

            e.Handled = true;
            e.Pointer.Capture(null);

            PointerAction action = _pointerAction;
            _pointerAction = PointerAction.None;
            Complete(action == PointerAction.Select
                ? new ScreenColorPickerResult(_pressedColor, _pressedPosition,
                    e.KeyModifiers.HasFlag(KeyModifiers.Control))
                : null);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                Complete(null);
            }
        }

        private void ConfigureOverlayBounds()
        {
            var screens = Screens.All;
            if (screens.Count == 0)
            {
                return;
            }

            int left = screens.Min(screen => screen.Bounds.X);
            int top = screens.Min(screen => screen.Bounds.Y);
            int right = screens.Max(screen => screen.Bounds.X + screen.Bounds.Width);
            int bottom = screens.Max(screen => screen.Bounds.Y + screen.Bounds.Height);
            var topLeft = new PixelPoint(left, top);
            double scaling = Screens.ScreenFromPoint(topLeft)?.Scaling ?? screens[0].Scaling;

            Position = topLeft;
            Width = (right - left) / scaling;
            Height = (bottom - top) / scaling;
            Cursor = CursorAssetLoader.GetCrosshairCursor(scaling);
        }

        private void UpdatePicker(PixelPoint screenPoint, Point clientPoint)
        {
            Color color = default;
            bool colorRead = _magnifierBitmap != null &&
                TryUpdateMagnifier(screenPoint, _magnifierBitmap, out color);

            if (!colorRead)
            {
                colorRead = TryGetScreenColor(screenPoint, out color);
            }

            if (colorRead)
            {
                _currentColor = color;
                _currentPosition = screenPoint;
                _colorPreview.Background = new SolidColorBrush(color);
                _infoText.Text = ScreenColorPickerTextFormatter.Format(_options.InfoText, color, screenPoint);
            }

            MovePreviewNextToCursor(clientPoint);
            _pickerPreview.Opacity = 1;
        }

        private void MovePreviewNextToCursor(Point cursorPosition)
        {
            double previewWidth = Math.Max(1, _pickerPreview.Bounds.Width);
            double previewHeight = Math.Max(1, _pickerPreview.Bounds.Height);
            double x = cursorPosition.X + PreviewOffset;
            double y = cursorPosition.Y + PreviewOffset;

            if (x + previewWidth > Bounds.Width)
            {
                x = cursorPosition.X - previewWidth - PreviewOffset;
            }

            if (y + previewHeight > Bounds.Height)
            {
                y = cursorPosition.Y - previewHeight - PreviewOffset;
            }

            Canvas.SetLeft(_pickerPreview, Math.Clamp(x, 0, Math.Max(0, Bounds.Width - previewWidth)));
            Canvas.SetTop(_pickerPreview, Math.Clamp(y, 0, Math.Max(0, Bounds.Height - previewHeight)));
        }

        private void Complete(ScreenColorPickerResult? result)
        {
            if (_completionSource.TrySetResult(result))
            {
                Close();
            }
        }

        private void InitializeMagnifierCapture()
        {
            nint screenDc = GetDC(nint.Zero);

            if (screenDc == nint.Zero)
            {
                return;
            }

            try
            {
                _magnifierDc = CreateCompatibleDC(screenDc);

                if (_magnifierDc == nint.Zero)
                {
                    return;
                }

                BitmapInfo bitmapInfo = new()
                {
                    Header = new BitmapInfoHeader
                    {
                        Size = (uint)Marshal.SizeOf<BitmapInfoHeader>(),
                        Width = MagnifierPixelCount,
                        Height = -MagnifierPixelCount,
                        Planes = 1,
                        BitCount = 32,
                        Compression = BiRgb
                    }
                };

                _magnifierDib = CreateDIBSection(screenDc, ref bitmapInfo, DibRgbColors,
                    out _magnifierBits, nint.Zero, 0);

                if (_magnifierDib != nint.Zero && _magnifierBits != nint.Zero)
                {
                    _previousMagnifierObject = SelectObject(_magnifierDc, _magnifierDib);
                }
            }
            finally
            {
                ReleaseDC(nint.Zero, screenDc);
            }
        }

        private unsafe bool TryUpdateMagnifier(PixelPoint center, WriteableBitmap bitmap, out Color color)
        {
            if (_magnifierDc == nint.Zero || _magnifierDib == nint.Zero || _magnifierBits == nint.Zero)
            {
                color = default;
                return false;
            }

            nint screenDc = GetDC(nint.Zero);

            if (screenDc == nint.Zero)
            {
                color = default;
                return false;
            }

            try
            {
                int radius = MagnifierPixelCount / 2;

                if (!BitBlt(_magnifierDc, 0, 0, MagnifierPixelCount, MagnifierPixelCount,
                    screenDc, center.X - radius, center.Y - radius, SrcCopy))
                {
                    color = default;
                    return false;
                }

                using ILockedFramebuffer framebuffer = bitmap.Lock();
                int sourceStride = MagnifierPixelCount * 4;
                byte* source = (byte*)_magnifierBits;
                byte* destination = (byte*)framebuffer.Address;

                for (int y = 0; y < MagnifierPixelCount; y++)
                {
                    Buffer.MemoryCopy(source + (y * sourceStride),
                        destination + (y * framebuffer.RowBytes), framebuffer.RowBytes, sourceStride);
                }

                int centerOffset = ((radius * sourceStride) + radius * 4);
                color = Color.FromRgb(source[centerOffset + 2], source[centerOffset + 1], source[centerOffset]);
                return true;
            }
            finally
            {
                ReleaseDC(nint.Zero, screenDc);
            }
        }

        private void DisposeMagnifierCapture()
        {
            if (_magnifierDc != nint.Zero && _previousMagnifierObject != nint.Zero)
            {
                SelectObject(_magnifierDc, _previousMagnifierObject);
                _previousMagnifierObject = nint.Zero;
            }

            if (_magnifierDib != nint.Zero)
            {
                DeleteObject(_magnifierDib);
                _magnifierDib = nint.Zero;
                _magnifierBits = nint.Zero;
            }

            if (_magnifierDc != nint.Zero)
            {
                DeleteDC(_magnifierDc);
                _magnifierDc = nint.Zero;
            }
        }

        private static bool TryGetScreenColor(PixelPoint point, out Color color)
        {
            nint screenDc = GetDC(nint.Zero);

            if (screenDc == nint.Zero)
            {
                color = default;
                return false;
            }

            try
            {
                uint pixel = GetPixel(screenDc, point.X, point.Y);

                if (pixel == ClrInvalid)
                {
                    color = default;
                    return false;
                }

                color = Color.FromRgb(
                    (byte)(pixel & 0xFF),
                    (byte)((pixel >> 8) & 0xFF),
                    (byte)((pixel >> 16) & 0xFF));
                return true;
            }
            finally
            {
                ReleaseDC(nint.Zero, screenDc);
            }
        }

        private enum PointerAction
        {
            None,
            Select,
            Cancel
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativePoint
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BitmapInfoHeader
        {
            public uint Size;
            public int Width;
            public int Height;
            public ushort Planes;
            public ushort BitCount;
            public uint Compression;
            public uint SizeImage;
            public int XPelsPerMeter;
            public int YPelsPerMeter;
            public uint ColorsUsed;
            public uint ColorsImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BitmapInfo
        {
            public BitmapInfoHeader Header;
            public uint Colors;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out NativePoint point);

        [DllImport("user32.dll")]
        private static extern nint GetDC(nint windowHandle);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(nint windowHandle, nint deviceContext);

        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(nint deviceContext, int x, int y);

        [DllImport("gdi32.dll")]
        private static extern nint CreateCompatibleDC(nint deviceContext);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(nint deviceContext);

        [DllImport("gdi32.dll")]
        private static extern nint CreateDIBSection(nint deviceContext, ref BitmapInfo bitmapInfo,
            uint usage, out nint bits, nint section, uint offset);

        [DllImport("gdi32.dll")]
        private static extern nint SelectObject(nint deviceContext, nint graphicsObject);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(nint graphicsObject);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BitBlt(nint destinationDc, int x, int y, int width, int height,
            nint sourceDc, int sourceX, int sourceY, uint rasterOperation);
    }
}
