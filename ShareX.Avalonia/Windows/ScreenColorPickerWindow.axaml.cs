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
using ShareX.AvaloniaUI.Theming;
using ShareX.AvaloniaUI.Input;
using System.Runtime.InteropServices;

namespace ShareX.AvaloniaUI.Windows
{
    public partial class ScreenColorPickerWindow : Window
    {
        private const double PreviewOffset = 18;
        private const double PreviewWidth = 128;
        private const double PreviewHeight = 52;
        private const uint ClrInvalid = 0xFFFFFFFF;

        private readonly TaskCompletionSource<Color?> _completionSource = new();
        private Border _pickerPreview = null!;
        private Border _colorPreview = null!;
        private TextBlock _hexText = null!;
        private Color _currentColor;
        private PointerAction _pointerAction;
        private Color _pressedColor;

        public ScreenColorPickerWindow()
        {
            RequestedThemeVariant = ThemeManager.GetCurrentTheme();
            AvaloniaXamlLoader.Load(this);

            _pickerPreview = this.FindControl<Border>("PickerPreview")!;
            _colorPreview = this.FindControl<Border>("ColorPreview")!;
            _hexText = this.FindControl<TextBlock>("HexText")!;

            Loaded += OnLoaded;
        }

        public Task<Color?> PickAsync(Window? owner = null)
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
            Complete(action == PointerAction.Select ? _pressedColor : null);
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
            if (TryGetScreenColor(screenPoint, out Color color))
            {
                _currentColor = color;
                _colorPreview.Background = new SolidColorBrush(color);
                _hexText.Text = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            }

            MovePreviewNextToCursor(clientPoint);
        }

        private void MovePreviewNextToCursor(Point cursorPosition)
        {
            double x = cursorPosition.X + PreviewOffset;
            double y = cursorPosition.Y + PreviewOffset;

            if (x + PreviewWidth > Bounds.Width)
            {
                x = cursorPosition.X - PreviewWidth - PreviewOffset;
            }

            if (y + PreviewHeight > Bounds.Height)
            {
                y = cursorPosition.Y - PreviewHeight - PreviewOffset;
            }

            Canvas.SetLeft(_pickerPreview, Math.Clamp(x, 0, Math.Max(0, Bounds.Width - PreviewWidth)));
            Canvas.SetTop(_pickerPreview, Math.Clamp(y, 0, Math.Max(0, Bounds.Height - PreviewHeight)));
        }

        private void Complete(Color? color)
        {
            if (_completionSource.TrySetResult(color))
            {
                Close();
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

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out NativePoint point);

        [DllImport("user32.dll")]
        private static extern nint GetDC(nint windowHandle);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(nint windowHandle, nint deviceContext);

        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(nint deviceContext, int x, int y);
    }
}
