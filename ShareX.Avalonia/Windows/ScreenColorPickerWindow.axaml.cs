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
using ShareX.AvaloniaUI.Input;
using ShareX.AvaloniaUI.Theming;
using ShareX.Platform;
using ShareX.Platform.Imaging;
using System.Runtime.InteropServices;

namespace ShareX.AvaloniaUI.Windows
{
    public partial class ScreenColorPickerWindow : Window
    {
        private const double PreviewOffset = 18;
        private const int MagnifierPixelCount = 15;

        private readonly ScreenColorPickerOptions _options;
        private readonly TaskCompletionSource<ScreenColorPickerResult?> _completionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private Border _pickerPreview = null!;
        private Border _colorPreview = null!;
        private Grid _magnifierPanel = null!;
        private Image _magnifierImage = null!;
        private TextBlock _infoText = null!;
        private WriteableBitmap? _magnifierBitmap;
        private WriteableBitmap? _snapshotBitmap;
        private IScreenPixelSampler? _sampler;
        private PlatformRectangle _desktopBounds;
        private readonly CancellationTokenSource _captureCancellation = new();
        private bool _started;
        private bool _hasColor;
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
                    AlphaFormat.Premul);
                _magnifierImage.Source = _magnifierBitmap;
            }

            Loaded += OnLoaded;
        }

        public async Task<ScreenColorPickerResult?> PickAsync(Window? owner = null)
        {
            if (_started) return await _completionSource.Task;
            _started = true;
            try
            {
                IScreenCaptureService capture = PlatformServices.Current.ScreenCapture;
                if (!capture.Support.IsSupported) throw new PlatformNotSupportedException(capture.Support.Reason);
                _sampler = await capture.CreatePixelSamplerAsync(_captureCancellation.Token);
                if (_completionSource.Task.IsCompleted)
                {
                    DisposeCapture();
                    return null;
                }
                ConfigureOverlayBounds();
                if (!_sampler.IsLive && _sampler.Snapshot is PixelBuffer snapshot)
                {
                    _snapshotBitmap = new WriteableBitmap(new PixelSize(snapshot.Width, snapshot.Height), new Vector(96, 96),
                        PixelFormat.Bgra8888, AlphaFormat.Premul);
                    CopyPixels(snapshot, _snapshotBitmap);
                    this.FindControl<Image>("SnapshotImage")!.Source = _snapshotBitmap;
                }
                if (owner != null) Show(owner);
                else Show();
                return await _completionSource.Task;
            }
            catch (OperationCanceledException)
            {
                DisposeCapture();
                Complete(null);
                return null;
            }
            catch
            {
                DisposeCapture();
                Complete(null);
                throw;
            }
            finally
            {
                _captureCancellation.Dispose();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (!_captureCancellation.IsCancellationRequested) _captureCancellation.Cancel();
            DisposeCapture();
            _completionSource.TrySetResult(null);
            base.OnClosed(e);
        }

        private void OnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Activate();
            Focus();

            if (PlatformServices.Current.Windows.GetCursorPosition() is PlatformPoint cursorPosition)
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

            if (pointerPoint.Properties.IsLeftButtonPressed && _hasColor)
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
            _desktopBounds = new PlatformRectangle(left, top, right - left, bottom - top);
            var topLeft = new PixelPoint(left, top);
            double scaling = Screens.ScreenFromPoint(topLeft)?.Scaling ?? screens[0].Scaling;

            Position = topLeft;
            Width = (right - left) / scaling;
            Height = (bottom - top) / scaling;
            Cursor = CursorAssetLoader.GetCrosshairCursor(scaling);
        }

        private void UpdatePicker(PixelPoint screenPoint, Point clientPoint)
        {
            if (_sampler == null) return;
            PlatformPoint sample = ScreenColorPickerPixels.MapPosition(screenPoint, _desktopBounds, _sampler.Bounds, _sampler.IsLive);
            int size = _magnifierBitmap != null ? MagnifierPixelCount : 1;
            int radius = size / 2;
            PixelBuffer? pixels = _sampler.Read(new PlatformRectangle(sample.X - radius, sample.Y - radius, size, size));
            bool colorRead = ScreenColorPickerPixels.TryGetCenterColor(pixels, out Color color);
            if (pixels != null && _magnifierBitmap != null)
            {
                CopyPixels(pixels, _magnifierBitmap);
            }
            if (!colorRead && size > 1)
            {
                colorRead = ScreenColorPickerPixels.TryGetCenterColor(_sampler.Read(new PlatformRectangle(sample.X, sample.Y, 1, 1)), out color);
            }

            if (colorRead)
            {
                _currentColor = color;
                _currentPosition = screenPoint;
                _colorPreview.Background = new SolidColorBrush(color);
                _infoText.Text = ScreenColorPickerTextFormatter.Format(_options.InfoText, color, screenPoint);
            }

            MovePreviewNextToCursor(clientPoint);
            _hasColor = colorRead;
            _pickerPreview.Opacity = colorRead ? 1 : 0;
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
            if (!_captureCancellation.IsCancellationRequested) _captureCancellation.Cancel();
            if (_completionSource.TrySetResult(result))
            {
                Close();
            }
        }

        private static void CopyPixels(PixelBuffer pixels, WriteableBitmap bitmap)
        {
            byte[] premultiplied = ScreenColorPickerPixels.Premultiply(pixels);
            using ILockedFramebuffer framebuffer = bitmap.Lock();
            for (int y = 0; y < pixels.Height; y++)
            {
                Marshal.Copy(premultiplied, y * pixels.Stride,
                    framebuffer.Address + y * framebuffer.RowBytes, pixels.Stride);
            }
        }

        private void DisposeCapture()
        {
            _sampler?.Dispose();
            _sampler = null;
            _snapshotBitmap?.Dispose();
            _snapshotBitmap = null;
            _magnifierBitmap?.Dispose();
            _magnifierBitmap = null;
        }

        private enum PointerAction
        {
            None,
            Select,
            Cancel
        }
    }
}
