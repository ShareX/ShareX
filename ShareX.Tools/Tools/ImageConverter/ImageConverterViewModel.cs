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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.HelpersLib;
using System.Collections.ObjectModel;
using System.Drawing;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;
using AvaloniaColor = Avalonia.Media.Color;

namespace ShareX.Tools;

public sealed partial class ImageConverterViewModel : ViewModelBase, IDisposable
{
    private readonly HashSet<string> _selectedImages = [];
    private CancellationTokenSource? _previewCancellationTokenSource;
    private int _previewVersion;
    private bool _isDisposed;

    public ObservableCollection<string> Images { get; } = [];

    [ObservableProperty]
    private string? _selectedImage;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsJpeg))]
    private int _selectedOutputFormatIndex;

    [ObservableProperty]
    private decimal _quality = 90;

    [ObservableProperty]
    private AvaloniaColor _backgroundColor = AvaloniaColor.FromRgb(255, 255, 255);

    [ObservableProperty]
    private string _outputFolderPath = string.Empty;

    [ObservableProperty]
    private string _outputFileName = "$filename_converted";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPreview))]
    private AvaloniaBitmap? _previewImage;

    [ObservableProperty]
    private string _previewSizeText = string.Empty;

    [ObservableProperty]
    private bool _isPreviewLoading;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasMessage))]
    private string _message = string.Empty;

    public Func<Task<IReadOnlyList<string>?>>? SelectFilesRequested { get; set; }
    public Func<Task<string?>>? SelectOutputFolderRequested { get; set; }

    public IReadOnlyList<string> OutputFormatOptions { get; } = ["PNG", "JPEG"];

    public bool HasImages => Images.Count > 0;
    public bool HasPreview => PreviewImage != null;
    public bool HasMessage => !string.IsNullOrWhiteSpace(Message);
    public bool IsJpeg => SelectedOutputFormatIndex == (int)ImageConverterOutputFormat.Jpeg;
    public bool CanRemove => _selectedImages.Count > 0 || SelectedImage != null;
    public bool CanConvert => !IsBusy && HasImages && Directory.Exists(OutputFolderPath) &&
        !string.IsNullOrWhiteSpace(OutputFileName);
    public string ImageCountText => string.Format(Images.Count == 1
        ? Localization.Strings.ImageThumbnailerViewModel_One_image
        : Localization.Strings.ImageThumbnailerViewModel_Image_count, Images.Count);
    public string PreviewFilePath => SelectedImage ?? Images.FirstOrDefault() ?? string.Empty;

    [RelayCommand]
    private async Task AddAsync()
    {
        if (SelectFilesRequested != null)
        {
            AddFiles(await SelectFilesRequested());
        }
    }

    public void AddFiles(IEnumerable<string>? files)
    {
        if (files == null)
        {
            return;
        }

        foreach (string file in files.Where(File.Exists))
        {
            Images.Add(file);
            if (string.IsNullOrWhiteSpace(OutputFolderPath))
            {
                OutputFolderPath = Path.GetDirectoryName(file) ?? string.Empty;
            }
        }

        if (SelectedImage == null)
        {
            SelectedImage = Images.FirstOrDefault();
        }

        NotifyCollectionChanged();
    }

    public void SetSelectedImages(IEnumerable<string> files)
    {
        _selectedImages.Clear();
        foreach (string file in files)
        {
            _selectedImages.Add(file);
        }
        OnPropertyChanged(nameof(CanRemove));
    }

    [RelayCommand]
    private void Remove()
    {
        string[] files = _selectedImages.Count > 0
            ? _selectedImages.ToArray()
            : SelectedImage == null ? [] : [SelectedImage];

        foreach (string file in files)
        {
            Images.Remove(file);
        }

        _selectedImages.Clear();
        SelectedImage = Images.FirstOrDefault();
        NotifyCollectionChanged();
    }

    [RelayCommand]
    private async Task BrowseOutputFolderAsync()
    {
        if (SelectOutputFolderRequested == null)
        {
            return;
        }

        string? folder = await SelectOutputFolderRequested();
        if (!string.IsNullOrWhiteSpace(folder))
        {
            OutputFolderPath = folder;
        }
    }

    [RelayCommand]
    private async Task ConvertAsync()
    {
        if (!CanConvert)
        {
            return;
        }

        string[] imageFiles = Images.ToArray();
        ImageConverterOutputFormat format = GetOutputFormat();
        int quality = (int)Quality;
        Color backgroundColor = ToDrawingColor(BackgroundColor);
        string outputFolderPath = OutputFolderPath;
        string outputFileName = OutputFileName;

        IsBusy = true;
        Message = string.Empty;
        try
        {
            List<string> outputFiles = await Task.Run(() => ConvertImages(imageFiles, format, quality,
                backgroundColor, outputFolderPath, outputFileName));
            if (outputFiles.Count > 0)
            {
                FileHelpers.OpenFolderWithFile(outputFiles[0]);
            }
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(ImageConverterViewModel), "Failed to convert images.", ex);
            Message = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSelectedImageChanged(string? value)
    {
        OnPropertyChanged(nameof(CanRemove));
        OnPropertyChanged(nameof(PreviewFilePath));
        QueuePreviewRefresh();
    }
    partial void OnSelectedOutputFormatIndexChanged(int value) => NotifyOptionsChanged();
    partial void OnQualityChanged(decimal value) => NotifyOptionsChanged();
    partial void OnBackgroundColorChanged(AvaloniaColor value) => NotifyOptionsChanged();
    partial void OnOutputFolderPathChanged(string value) => NotifyStateChanged();
    partial void OnOutputFileNameChanged(string value) => NotifyStateChanged();
    partial void OnIsBusyChanged(bool value) => OnPropertyChanged(nameof(CanConvert));

    private void NotifyCollectionChanged()
    {
        OnPropertyChanged(nameof(HasImages));
        OnPropertyChanged(nameof(ImageCountText));
        OnPropertyChanged(nameof(PreviewFilePath));
        OnPropertyChanged(nameof(CanRemove));
        OnPropertyChanged(nameof(CanConvert));
        Message = string.Empty;
        QueuePreviewRefresh();
    }

    private void NotifyOptionsChanged()
    {
        OnPropertyChanged(nameof(CanConvert));
        Message = string.Empty;
        QueuePreviewRefresh();
    }

    private void NotifyStateChanged()
    {
        OnPropertyChanged(nameof(CanConvert));
        Message = string.Empty;
    }

    private void QueuePreviewRefresh()
    {
        _previewCancellationTokenSource?.Cancel();
        _previewCancellationTokenSource?.Dispose();
        _previewCancellationTokenSource = new CancellationTokenSource();
        _ = RefreshPreviewAsync(_previewCancellationTokenSource.Token);
    }

    private async Task RefreshPreviewAsync(CancellationToken cancellationToken)
    {
        int version = ++_previewVersion;
        string? filePath = SelectedImage ?? Images.FirstOrDefault();
        if (_isDisposed || string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            ReplacePreview(null);
            PreviewSizeText = string.Empty;
            IsPreviewLoading = false;
            return;
        }

        IsPreviewLoading = true;
        try
        {
            await Task.Delay(100, cancellationToken);
            ImageConverterOutputFormat format = GetOutputFormat();
            int quality = (int)Quality;
            Color backgroundColor = ToDrawingColor(BackgroundColor);
            ImageConverterPreview result = await Task.Run(() =>
                ImageConverterService.CreatePreview(filePath, format, quality, backgroundColor), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            AvaloniaBitmap? preview = null;
            if (result.Data.Length > 0)
            {
                using MemoryStream stream = new(result.Data, writable: false);
                preview = new AvaloniaBitmap(stream);
            }

            if (version == _previewVersion && !_isDisposed)
            {
                ReplacePreview(preview);
                PreviewSizeText = result.Width > 0 && result.Height > 0
                    ? $"{result.Width} × {result.Height}"
                    : string.Empty;
            }
            else
            {
                preview?.Dispose();
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            if (version == _previewVersion && !_isDisposed)
            {
                ReplacePreview(null);
                PreviewSizeText = string.Empty;
                ToolsDiagnostics.ReportWarning(nameof(ImageConverterViewModel), "Failed to create conversion preview.", ex);
            }
        }
        finally
        {
            if (version == _previewVersion && !_isDisposed)
            {
                IsPreviewLoading = false;
            }
        }
    }

    private void ReplacePreview(AvaloniaBitmap? preview)
    {
        PreviewImage?.Dispose();
        PreviewImage = preview;
    }

    private ImageConverterOutputFormat GetOutputFormat() =>
        (ImageConverterOutputFormat)Math.Clamp(SelectedOutputFormatIndex, 0, 1);

    private static List<string> ConvertImages(IEnumerable<string> imageFiles,
        ImageConverterOutputFormat format, int quality, Color backgroundColor, string outputFolderPath,
        string outputFileName)
    {
        List<string> outputFiles = [];
        string extension = format == ImageConverterOutputFormat.Jpeg ? "jpg" : "png";

        foreach (string filePath in imageFiles)
        {
            if (!File.Exists(filePath))
            {
                continue;
            }

            using Bitmap? source = ImageHelpers.LoadImage(filePath);
            if (source == null)
            {
                continue;
            }

            string sourceName = Path.GetFileNameWithoutExtension(filePath);
            string outputPath = Path.Combine(outputFolderPath,
                outputFileName.Replace("$filename", sourceName, StringComparison.Ordinal));
            outputPath = Path.ChangeExtension(outputPath, extension);
            ImageConverterService.Save(source, outputPath, format, quality, backgroundColor);
            outputFiles.Add(outputPath);
        }

        return outputFiles;
    }

    private static Color ToDrawingColor(AvaloniaColor color) => Color.FromArgb(color.R, color.G, color.B);

    public void Dispose()
    {
        _isDisposed = true;
        _previewVersion++;
        _previewCancellationTokenSource?.Cancel();
        _previewCancellationTokenSource?.Dispose();
        _previewCancellationTokenSource = null;
        ReplacePreview(null);
    }
}
