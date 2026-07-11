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
using Avalonia.Markup.Xaml;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Presentation.ViewModels;
using SkiaSharp;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ShareX.ImageEditor.Presentation.Views
{
    public partial class EditorWindow : Window
    {
        private readonly ImageEditorOptions? _options;
        private readonly MainViewModel _viewModel;
        private string? _pendingFilePath;
        private bool _allowClose;

        public EditorWindow() : this(null)
        {
        }

        public EditorWindow(ImageEditorOptions? options)
        {
            InitializeComponent();

            _options = options;
            ApplySavedWindowState();

            Resized += OnWindowResized;

            _viewModel = new MainViewModel(options);
            DataContext = _viewModel;
            _viewModel.WindowTitle = GetWindowTitle(null);

            // Defer image loading until EditorView is loaded and subscribed
            this.Loaded += OnWindowLoaded;

            _viewModel.CloseRequested += (s, e) =>
            {
                _allowClose = true;
                Close();
            };
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            if (_allowClose)
            {
                base.OnClosing(e);
                return;
            }

            e.Cancel = true;
            _viewModel.RequestClose(ignoreModal: true);
        }

        protected override void OnClosed(EventArgs e)
        {
            SaveWindowState();
            base.OnClosed(e);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void ApplySavedWindowState()
        {
            if (_options?.RememberWindowState != true)
            {
                return;
            }

            if (_options.WindowWidth > 0 && _options.WindowHeight > 0)
            {
                Width = _options.WindowWidth;
                Height = _options.WindowHeight;
            }

            WindowState = _options.IsWindowMaximized ? WindowState.Maximized : WindowState.Normal;
        }

        private void OnWindowResized(object? sender, WindowResizedEventArgs e)
        {
            SaveNormalWindowSize();
        }

        private void SaveWindowState()
        {
            if (_options?.RememberWindowState != true)
            {
                return;
            }

            SaveNormalWindowSize();
            _options.IsWindowMaximized = WindowState == WindowState.Maximized;
        }

        private void SaveNormalWindowSize()
        {
            if (_options?.RememberWindowState != true || WindowState != WindowState.Normal)
            {
                return;
            }

            Size windowSize = Bounds.Size;

            if (windowSize.Width > 0 && windowSize.Height > 0)
            {
                _options.WindowWidth = windowSize.Width;
                _options.WindowHeight = windowSize.Height;
            }
        }

        private void OnWindowLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // Now that EditorView is loaded and subscribed to ViewModel, load the image
            if (!string.IsNullOrEmpty(_pendingFilePath))
            {
                LoadImageInternal(_pendingFilePath);
                _pendingFilePath = null;
            }
        }

        /// <summary>
        /// Loads an image from the specified file path.
        /// If called before window is loaded, defers loading until EditorView is ready.
        /// </summary>
        /// <param name="filePath">Absolute path to the image file.</param>
        public void LoadImage(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            if (!File.Exists(filePath))
            {
                return;
            }

            // If window not loaded yet, defer image loading
            if (!IsLoaded)
            {
                _pendingFilePath = filePath;
                return;
            }

            LoadImageInternal(filePath);
        }

        private void LoadImageInternal(string filePath)
        {
            try
            {
                using var stream = File.OpenRead(filePath);
                SKBitmap? bitmap = SKBitmap.Decode(stream);
                if (bitmap == null)
                {
                    throw new InvalidOperationException("SkiaSharp returned no bitmap.");
                }

                _viewModel.UpdatePreview(bitmap);
                _viewModel.ImageFilePath = filePath;
                _viewModel.IsDirty = false;
            }
            catch (Exception ex)
            {
                EditorServices.ReportError(nameof(EditorWindow), $"Failed to load image file '{filePath}'.", ex);
            }
        }

        /// <summary>
        /// Loads an image from a stream.
        /// </summary>
        /// <param name="stream">Stream containing image data.</param>
        public void LoadImage(Stream stream)
        {
            if (stream == null) return;
            try
            {
                // Ensure stream position is at beginning if possible
                if (stream.CanSeek && stream.Position != 0)
                    stream.Position = 0;

                SKBitmap? bitmap = SKBitmap.Decode(stream);
                if (bitmap == null)
                {
                    throw new InvalidOperationException("SkiaSharp returned no bitmap.");
                }

                _viewModel.UpdatePreview(bitmap);
                _viewModel.IsDirty = false;
            }
            catch (Exception ex)
            {
                EditorServices.ReportError(nameof(EditorWindow), "Failed to load image from stream.", ex);
            }
        }

        /// <summary>
        /// Loads an image from an existing SKBitmap instance.
        /// Ownership is transferred to the view model.
        /// </summary>
        public void LoadImage(SKBitmap bitmap)
        {
            if (bitmap == null) return;

            try
            {
                _viewModel.UpdatePreview(bitmap);
                _viewModel.IsDirty = false;
            }
            catch (Exception ex)
            {
                EditorServices.ReportError(nameof(EditorWindow), "Failed to load image from bitmap.", ex);
            }
        }

        private static string GetWindowTitle(string? dimensions)
        {
            return string.IsNullOrEmpty(dimensions)
                ? "ShareX - Image Editor"
                : $"ShareX - Image Editor - {dimensions}";
        }

        private static string GetVersionString()
        {
            var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

            var info = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (!string.IsNullOrEmpty(info?.InformationalVersion))
            {
                return info.InformationalVersion;
            }

            var version = asm.GetName().Version;
            return version?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Gets the current edited image as a SkiaSharp SKBitmap.
        /// </summary>
        public SKBitmap? GetResultBitmap()
        {
            var editorView = this.FindControl<EditorView>("EditorViewControl");
            return editorView?.GetSnapshot();
        }

        public SKBitmap? GetSourceBitmap()
        {
            var editorView = this.FindControl<EditorView>("EditorViewControl");
            return editorView?.GetSource();
        }

        /// <summary>
        /// Gets the encoded image data as a BMP byte array.
        /// Uses direct pixel copy (no compression) for maximum performance.
        /// </summary>
        public byte[]? GetResultBytes()
        {
            using var bitmap = GetResultBitmap();
            return bitmap != null ? EncodeBitmapAsBmp(bitmap) : null;
        }

        private static byte[] EncodeBitmapAsBmp(SKBitmap bitmap)
        {
            SKBitmap? converted = null;
            if (bitmap.ColorType != SKColorType.Bgra8888)
            {
                converted = new SKBitmap(bitmap.Width, bitmap.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
                bitmap.CopyTo(converted, SKColorType.Bgra8888);
                bitmap = converted;
            }

            try
            {
                int w = bitmap.Width;
                int h = bitmap.Height;
                int pixelDataSize = w * h * 4;
                byte[] buf = new byte[54 + pixelDataSize];

                // BMP file header (14 bytes)
                buf[0] = 0x42; buf[1] = 0x4D;       // "BM"
                WriteLE32(buf, 2, 54 + pixelDataSize); // file size
                WriteLE32(buf, 10, 54);                // pixel data offset

                // BITMAPINFOHEADER (40 bytes)
                WriteLE32(buf, 14, 40);          // header size
                WriteLE32(buf, 18, w);            // width
                WriteLE32(buf, 22, -h);           // negative height = top-down
                WriteLE16(buf, 26, 1);            // color planes
                WriteLE16(buf, 28, 32);           // bits per pixel (BGRA)
                WriteLE32(buf, 34, pixelDataSize); // image size

                Marshal.Copy(bitmap.GetPixels(), buf, 54, pixelDataSize);
                return buf;
            }
            finally
            {
                converted?.Dispose();
            }
        }

        private static void WriteLE32(byte[] buf, int offset, int value)
        {
            buf[offset] = (byte)value;
            buf[offset + 1] = (byte)(value >> 8);
            buf[offset + 2] = (byte)(value >> 16);
            buf[offset + 3] = (byte)(value >> 24);
        }

        private static void WriteLE16(byte[] buf, int offset, int value)
        {
            buf[offset] = (byte)value;
            buf[offset + 1] = (byte)(value >> 8);
        }

        /// <summary>
        /// Gets the encoded image data in the specified format.
        /// Useful for interoperability with other frameworks (e.g. WinForms).
        /// </summary>
        public byte[]? GetSourceBytes(SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100)
        {
            using var bitmap = GetSourceBitmap();
            if (bitmap == null) return null;

            using var data = bitmap.Encode(format, quality);
            return data.ToArray();
        }
    }
}