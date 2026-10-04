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

#nullable enable

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.Media.Imaging;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Localization;
using ShareX.Platform;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrawingBitmap = SkiaSharp.SKBitmap;
using ImageFormat = SkiaSharp.SKEncodedImageFormat;

namespace ShareX;

public partial class ClipboardUploadWindow : Window
{
    private readonly TaskSettings _taskSettings;
    private object? _clipboardContent;
    private Bitmap? _previewBitmap;
    private bool _keepClipboardContent;
    private bool _loadStarted;
    private bool _closed;

    public bool DontShowAgain => DontShowAgainCheckBox.IsChecked == true;

    public ClipboardUploadWindow() : this(TaskSettings.GetDefaultTaskSettings())
    {
    }

    public ClipboardUploadWindow(TaskSettings taskSettings, bool showDontShowAgain = false)
    {
        _taskSettings = taskSettings;

        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        Title = $"ShareX - {Strings.ClipboardUploadWindow_ClipboardUpload}";
        HeaderTitle.Text = Strings.ClipboardUploadWindow_ClipboardUpload;
        DontShowAgainCheckBox.IsVisible = showDontShowAgain;

        UploadButton.IsEnabled = false;
        ImagePreviewContainer.IsVisible = TextPreview.IsVisible = FilePreview.IsVisible = EmptyPreview.IsVisible = false;
        Activated += (_, _) =>
        {
            if (_loadStarted || _closed) return;
            _loadStarted = true;
            // Read after native focus events have been processed, using this shown top level.
            Dispatcher.UIThread.Post(async () =>
            {
                if (!_closed && Clipboard is { } clipboard)
                    await LoadClipboardContentAsync(() => clipboard.TryGetDataAsync(),
                        DataFormat.CreateBytesPlatformFormat(PlatformServices.Current.Clipboard.FormatNames.Png));
            }, DispatcherPriority.Background);
        };

        Opened += (_, _) => Activate();
        Closed += OnClosed;
    }

    private async Task LoadClipboardContentAsync(Func<Task<IAsyncDataTransfer?>> read, DataFormat<byte[]> pngFormat)
    {
        try
        {
            using var transfer = await read();
            if (_closed) return;
            if (transfer != null)
            {
                // Prefer the original PNG with the existing decoder before toolkit bitmap conversion.
                byte[]? png = await transfer.TryGetValueAsync(pngFormat);
                if (_closed) return;
                DrawingBitmap? image = png is { Length: > 0 } ? SkiaImageHelpers.ByteArrayToBitmap(png) : null;
                if (image == null)
                {
                    using Bitmap? bitmap = await transfer.TryGetBitmapAsync();
                    if (_closed) return;
                    if (bitmap != null)
                    {
                        using MemoryStream stream = new();
                        bitmap.Save(stream, PngBitmapEncoderOptions.Default);
                        image = SkiaImageHelpers.ByteArrayToBitmap(stream.ToArray());
                    }
                }
                if (image != null)
                {
                    try
                    {
                        _previewBitmap = CreatePreviewBitmap(image);
                        ImagePreview.Source = _previewBitmap;
                        _clipboardContent = image;
                    }
                    catch { image.Dispose(); throw; }
                    ImagePreviewContainer.IsVisible = true;
                    ClipboardSummary.Text = string.Format(Strings.ClipboardUploadWindow_ImageSummary, image.Width, image.Height);
                    UploadButton.IsEnabled = true;
                    return;
                }
                string? text = await transfer.TryGetTextAsync();
                if (_closed) return;
                if (!string.IsNullOrEmpty(text))
                {
                    _clipboardContent = text;
                    TextPreview.Text = text;
                    TextPreview.IsVisible = true;
                    ClipboardSummary.Text = string.Format(Strings.ClipboardUploadWindow_TextSummary, text.Length);
                    UploadButton.IsEnabled = true;
                    return;
                }
                var items = await transfer.TryGetFilesAsync();
                if (_closed) return;
                string[]? files = items?.Select(x => x.TryGetLocalPath()).Where(x => !string.IsNullOrEmpty(x)).Cast<string>().ToArray();
                if (files is { Length: > 0 })
                {
                    _clipboardContent = files;
                    FilePreview.ItemsSource = files;
                    FilePreview.IsVisible = true;
                    ClipboardSummary.Text = string.Format(Strings.ClipboardUploadWindow_FileSummary, files.Length);
                    UploadButton.IsEnabled = true;
                    return;
                }
            }
        }
        catch (Exception exception)
        {
            if (_closed) return;
            DebugHelper.WriteException(exception, "Unable to read clipboard for upload preview.");
        }
        if (_closed) return;
        ClipboardSummary.Text = Strings.ClipboardUploadWindow_Empty;
        EmptyPreview.Text = ClipboardSummary.Text;
        EmptyPreview.IsVisible = true;
    }

    private static Bitmap CreatePreviewBitmap(DrawingBitmap image)
    {
        using MemoryStream stream = new();
        image.Save(stream, ImageFormat.Png);
        stream.Position = 0;
        return new Bitmap(stream);
    }

    private void UploadClipboardContent()
    {
        switch (_clipboardContent)
        {
            case DrawingBitmap image:
                _keepClipboardContent = true;
                UploadManager.ProcessImageUpload(image, _taskSettings);
                break;
            case string text:
                UploadManager.ProcessTextUpload(text, _taskSettings);
                break;
            case string[] files:
                UploadManager.ProcessFilesUpload(files, _taskSettings);
                break;
        }
    }

    private void OnUploadClick(object? sender, RoutedEventArgs e)
    {
        if (_closed || !UploadButton.IsEnabled || _clipboardContent == null) return;
        UploadClipboardContent();
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();

    private void OnImagePreviewPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(ImagePreviewContainer).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed &&
            _clipboardContent is DrawingBitmap image)
        {
            ImageViewerWindowIntegration.ShowImage(image);
            e.Handled = true;
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _closed = true;
        _previewBitmap?.Dispose();
        _previewBitmap = null;

        if (!_keepClipboardContent && _clipboardContent is DrawingBitmap image)
        {
            image.Dispose();
        }

        _clipboardContent = null;
    }
}
