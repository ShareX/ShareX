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

using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ShareX.Tools;

public sealed partial class BackgroundRemoverViewModel : ViewModelBase, IDisposable
{
    private const string BackgroundRemoverGuideUrl = "https://getsharex.com/docs/background-remover";
    private readonly BackgroundRemovalService _backgroundRemovalService = new();
    private readonly BackgroundRemoverOptions _options;
    private SKBitmap? _sourceBitmap;
    private SKBitmap? _resultBitmap;

    public BackgroundRemoverViewModel(string? modelsFolder, BackgroundRemoverOptions options)
    {
        ModelsFolder = modelsFolder;
        _options = options;
        SelectedDevice = Enum.IsDefined(options.SelectedDevice) ? options.SelectedDevice : BackgroundRemovalDevice.Auto;
        _options.SelectedDevice = SelectedDevice;
        RefreshModels();
    }

    public ObservableCollection<BackgroundRemovalModel> AvailableModels { get; } = [];

    public IReadOnlyList<BackgroundRemovalDevice> AvailableDevices { get; } = Enum.GetValues<BackgroundRemovalDevice>();

    public string? ModelsFolder { get; }

    [ObservableProperty]
    private BackgroundRemovalDevice _selectedDevice = BackgroundRemovalDevice.Auto;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedModel))]
    [NotifyPropertyChangedFor(nameof(CanRemoveBackground))]
    [NotifyCanExecuteChangedFor(nameof(RemoveBackgroundCommand))]
    private BackgroundRemovalModel? _selectedModel;

    partial void OnSelectedDeviceChanged(BackgroundRemovalDevice value)
    {
        _options.SelectedDevice = value;
    }

    partial void OnSelectedModelChanged(BackgroundRemovalModel? value)
    {
        if (value != null)
        {
            _options.SelectedModelFileName = value.FileName;
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasImage))]
    [NotifyPropertyChangedFor(nameof(CanRemoveBackground))]
    [NotifyCanExecuteChangedFor(nameof(RemoveBackgroundCommand))]
    private string? _imagePath;

    [ObservableProperty]
    private Bitmap? _sourcePreviewImage;

    [ObservableProperty]
    private Bitmap? _resultPreviewImage;

    [ObservableProperty]
    private double _comparisonSliderPosition = 0.5;

    [ObservableProperty]
    private bool _isGuideHighlighted;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [NotifyCanExecuteChangedFor(nameof(SaveAsCommand))]
    private bool _hasProcessedImage;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [NotifyCanExecuteChangedFor(nameof(SaveAsCommand))]
    private bool _isSaving;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanRemoveBackground))]
    [NotifyCanExecuteChangedFor(nameof(BrowseImageCommand))]
    [NotifyCanExecuteChangedFor(nameof(RefreshModelsCommand))]
    [NotifyCanExecuteChangedFor(nameof(OpenModelsFolderCommand))]
    [NotifyCanExecuteChangedFor(nameof(RemoveBackgroundCommand))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [NotifyCanExecuteChangedFor(nameof(SaveAsCommand))]
    private bool _isProcessing;

    public Func<string, Task<string?>>? SelectImageFileRequested { get; set; }
    public Func<SKBitmap, string?, Task<string?>>? SaveImageRequested { get; set; }
    public Func<SKBitmap, string?, Task<string?>>? SaveImageAsRequested { get; set; }

    public bool HasImage => !string.IsNullOrEmpty(ImagePath);

    public bool HasSelectedModel => SelectedModel != null;

    public bool CanRemoveBackground => HasImage && HasSelectedModel && !IsProcessing;

    [RelayCommand(CanExecute = nameof(CanRefreshModels))]
    private void RefreshModels()
    {
        string? selectedModelFileName = SelectedModel?.FileName ?? _options.SelectedModelFileName;

        AvailableModels.Clear();
        SelectedModel = null;
        IsGuideHighlighted = true;

        if (string.IsNullOrWhiteSpace(ModelsFolder))
        {
            return;
        }

        try
        {
            Directory.CreateDirectory(ModelsFolder);

            foreach (string modelPath in Directory.EnumerateFiles(ModelsFolder, "*.onnx", SearchOption.TopDirectoryOnly).OrderBy(Path.GetFileName))
            {
                FileInfo fileInfo = new(modelPath);
                AvailableModels.Add(new BackgroundRemovalModel
                {
                    FilePath = fileInfo.FullName,
                    FileName = fileInfo.Name,
                    FileSize = fileInfo.Length
                });
            }

            SelectedModel = AvailableModels.FirstOrDefault(model =>
                string.Equals(model.FileName, selectedModelFileName, StringComparison.OrdinalIgnoreCase))
                ?? AvailableModels.FirstOrDefault();
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(BackgroundRemoverViewModel), "Failed to scan background removal models.", ex);
        }
        finally
        {
            IsGuideHighlighted = AvailableModels.Count == 0;
        }
    }

    [RelayCommand(CanExecute = nameof(CanOpenModelsFolder))]
    private void OpenModelsFolder()
    {
        if (string.IsNullOrWhiteSpace(ModelsFolder))
        {
            return;
        }

        try
        {
            Directory.CreateDirectory(ModelsFolder);

            Process.Start(new ProcessStartInfo
            {
                FileName = ModelsFolder,
                UseShellExecute = true
            });

            RefreshModels();
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(BackgroundRemoverViewModel), "Failed to open models folder.", ex);
        }
    }

    [RelayCommand]
    private void OpenBackgroundRemoverGuide()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = BackgroundRemoverGuideUrl,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(BackgroundRemoverViewModel), "Failed to open the background remover guide.", ex);
        }
    }

    [RelayCommand(CanExecute = nameof(CanBrowseImage))]
    private async Task BrowseImageAsync()
    {
        if (SelectImageFileRequested == null)
        {
            return;
        }

        string? filePath = await SelectImageFileRequested(Localization.Strings.BackgroundRemoverViewModel_Select_image_dialog);
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        LoadImage(filePath);
    }

    public void LoadImage(string filePath)
    {
        if (IsProcessing || string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }

        try
        {
            SKBitmap? bitmap = SKBitmap.Decode(filePath);
            if (bitmap == null)
            {
                return;
            }

            SetSourceImage(bitmap, filePath);
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(BackgroundRemoverViewModel), $"Failed to load image '{filePath}'.", ex);
        }
    }

    [RelayCommand(CanExecute = nameof(CanRemoveBackground))]
    private async Task RemoveBackgroundAsync()
    {
        if (string.IsNullOrEmpty(ImagePath))
        {
            return;
        }

        if (SelectedModel == null)
        {
            return;
        }

        try
        {
            IsProcessing = true;
            BackgroundRemovalModel selectedModel = SelectedModel!;
            BackgroundRemovalDevice selectedDevice = SelectedDevice;
            Stopwatch stopwatch = Stopwatch.StartNew();

            SKBitmap? sourceBitmap = SKBitmap.Decode(ImagePath);
            if (sourceBitmap == null)
            {
                return;
            }

            SetSourceImage(sourceBitmap.Copy(), ImagePath);

            BackgroundRemovalResult result = await Task.Run(() =>
            {
                using (sourceBitmap)
                {
                    return _backgroundRemovalService.RemoveBackground(sourceBitmap, selectedModel, selectedDevice);
                }
            });

            SetResultImage(result.Image);
            stopwatch.Stop();
            ShowNotification(string.Format(Localization.Strings.BackgroundRemoverViewModel_Background_removed_ms, stopwatch.ElapsedMilliseconds), LucideIcons.eraser);
            string cacheStatus = result.IsSessionCached ? "cached" : "not cached";
            Debug.WriteLine(
                $"Background removal (device={selectedDevice}, execution={result.ExecutionDevice}, model={selectedModel.FileName}, {cacheStatus}): " +
                $"total={stopwatch.ElapsedMilliseconds} ms, session={result.SessionSetupMilliseconds} ms, " +
                $"preprocess={result.PreprocessingMilliseconds} ms, inference={result.InferenceMilliseconds} ms, " +
                $"postprocess={result.PostprocessingMilliseconds} ms");
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(BackgroundRemoverViewModel), "Background removal failed.", ex);
        }
        finally
        {
            IsProcessing = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanSaveImage))]
    private async Task SaveAsync()
    {
        await SaveImageAsync(SaveImageRequested, LucideIcons.save);
    }

    [RelayCommand(CanExecute = nameof(CanSaveImage))]
    private async Task SaveAsAsync()
    {
        await SaveImageAsync(SaveImageAsRequested, LucideIcons.save_all);
    }

    private async Task SaveImageAsync(Func<SKBitmap, string?, Task<string?>>? saveRequested, string notificationIcon)
    {
        if (saveRequested == null || _resultBitmap == null || !CanSaveImage())
        {
            return;
        }

        try
        {
            IsSaving = true;
            using SKBitmap image = _resultBitmap.Copy();
            string? savedPath = await saveRequested(image, ImagePath);

            if (!string.IsNullOrWhiteSpace(savedPath))
            {
                ShowNotification(string.Format(Localization.Strings.BackgroundRemoverViewModel_Image_saved_file_path, savedPath), notificationIcon);
            }
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(BackgroundRemoverViewModel), "Failed to save background removal result.", ex);
        }
        finally
        {
            IsSaving = false;
        }
    }

    private bool CanSaveImage()
    {
        return HasProcessedImage && !IsProcessing && !IsSaving;
    }

    private bool CanBrowseImage()
    {
        return !IsProcessing;
    }

    private bool CanRefreshModels()
    {
        return !IsProcessing;
    }

    private bool CanOpenModelsFolder()
    {
        return !IsProcessing && !string.IsNullOrWhiteSpace(ModelsFolder);
    }

    private void SetSourceImage(SKBitmap bitmap, string? filePath)
    {
        Bitmap preview = BitmapConversionHelpers.ToAvaloniBitmap(bitmap);

        _sourceBitmap?.Dispose();
        _resultBitmap?.Dispose();
        SourcePreviewImage?.Dispose();
        ResultPreviewImage?.Dispose();

        _sourceBitmap = bitmap;
        _resultBitmap = null;
        SourcePreviewImage = preview;
        ResultPreviewImage = null;
        ImagePath = filePath;
        HasProcessedImage = false;
    }

    private void SetResultImage(SKBitmap bitmap)
    {
        Bitmap preview = BitmapConversionHelpers.ToAvaloniBitmap(bitmap);

        _resultBitmap?.Dispose();
        ResultPreviewImage?.Dispose();

        _resultBitmap = bitmap;
        ResultPreviewImage = preview;
        ComparisonSliderPosition = 0;
        HasProcessedImage = true;
    }

    public void Dispose()
    {
        DismissNotification();
        _backgroundRemovalService.Dispose();
        _sourceBitmap?.Dispose();
        _resultBitmap?.Dispose();
        SourcePreviewImage?.Dispose();
        ResultPreviewImage?.Dispose();
    }
}
