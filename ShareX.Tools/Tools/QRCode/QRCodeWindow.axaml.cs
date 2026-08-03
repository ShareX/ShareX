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

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;

namespace ShareX.Tools;

public partial class QRCodeWindow : Window
{
    private readonly QRCodeViewModel _viewModel;

    public QRCodeWindow()
        : this(new QRCodeServices
        {
            GeneratePreviewAsync = (_, _) => Task.FromResult<byte[]?>(null),
            ScanAsync = (_, _) => Task.FromResult<string[]?>(null),
            SaveAsync = (_, _, _) => Task.CompletedTask,
            CopyImage = (_, _) => { },
            UploadImage = (_, _) => { }
        }, new QRCodeWindowOptions())
    {
    }

    public QRCodeWindow(QRCodeServices services, QRCodeWindowOptions options)
    {
        _viewModel = new QRCodeViewModel(services, options);
        DataContext = _viewModel;
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _viewModel.CaptureScanRequested = ScanCaptureAsync;
        _viewModel.ImageFileScanRequested = ScanImageFileAsync;
        _viewModel.SaveRequested = SaveAsync;

        Border? previewHost = this.FindControl<Border>("PreviewHost");
        if (previewHost != null)
        {
            previewHost.SizeChanged += (_, _) => UpdateAvailableSize(previewHost);
        }

        Opened += OnOpened;
        Closed += (_, _) => _viewModel.Dispose();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        if (this.FindControl<Border>("PreviewHost") is { } previewHost)
        {
            UpdateAvailableSize(previewHost);
        }
        await _viewModel.InitializeAsync();
    }

    private void UpdateAvailableSize(Border previewHost)
    {
        _viewModel.SetAvailableSize(
            Math.Max(previewHost.Bounds.Width - 48, 64),
            Math.Max(previewHost.Bounds.Height - 48, 64));
    }

    private async Task<string[]?> ScanCaptureAsync(QRCodeScanMode mode)
    {
        try
        {
            Hide();
            await Task.Delay(250);
            return await _viewModel.ScanWithServiceAsync(mode);
        }
        finally
        {
            Show();
            Activate();
        }
    }

    private async Task<string[]?> ScanImageFileAsync()
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = Localization.Strings.QRCodeWindow_Select_image_dialog,
            AllowMultiple = false,
            FileTypeFilter = [FilePickerFileTypes.ImageAll]
        });

        return files.Count > 0
            ? await _viewModel.ScanWithServiceAsync(QRCodeScanMode.ImageFile, files[0].Path.LocalPath)
            : null;
    }

    private async Task<bool> SaveAsync(string text, int size)
    {
        string suggestedName = GetSuggestedFileName(text);
        IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = Localization.Strings.QRCodeWindow_Save_QR_code_dialog,
            SuggestedFileName = suggestedName,
            DefaultExtension = "png",
            FileTypeChoices =
            [
                new FilePickerFileType(Localization.Strings.QRCodeWindow_PNG_image) { Patterns = ["*.png"] },
                new FilePickerFileType(Localization.Strings.QRCodeWindow_JPEG_image) { Patterns = ["*.jpg", "*.jpeg"] },
                new FilePickerFileType(Localization.Strings.QRCodeWindow_Bitmap_image) { Patterns = ["*.bmp"] },
                new FilePickerFileType(Localization.Strings.QRCodeWindow_SVG_image) { Patterns = ["*.svg"] }
            ]
        });

        if (file == null)
        {
            return false;
        }

        await _viewModel.SaveWithServiceAsync(text, size, file.Path.LocalPath);
        return true;
    }

    private static string GetSuggestedFileName(string text)
    {
        string invalidChars = new(Path.GetInvalidFileNameChars());
        string fileName = new string(text.Take(80).Select(c => invalidChars.Contains(c) ? '_' : c).ToArray()).Trim();
        return string.IsNullOrWhiteSpace(fileName) ? "qr-code.png" : $"{fileName}.png";
    }

    private void OnPreviewPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_viewModel.PreviewImage == null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        using MemoryStream stream = new MemoryStream();
        _viewModel.PreviewImage.Save(stream, PngBitmapEncoderOptions.Default);
        ImageViewerWindowIntegration.ShowImage(stream.ToArray(), Localization.Strings.QRCodeWindow_Image_viewer_title, this);
        e.Handled = true;
    }
}
