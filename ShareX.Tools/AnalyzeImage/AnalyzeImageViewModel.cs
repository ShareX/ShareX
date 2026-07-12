#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace ShareX.Tools;

public sealed partial class AnalyzeImageViewModel : ViewModelBase, IDisposable
{
    private readonly AnalyzeImageOptions _options;
    private readonly AnalyzeImageHandler _analyze;
    private readonly Action<AnalyzeImageOptions>? _optionsChanged;
    private string? _imagePath;
    private byte[]? _imageData;

    public IReadOnlyList<string> PresetPrompts { get; } =
    [
        "What is in this image?",
        "Thoroughly describe this image.",
        "Transcribe the image's text. Do not write anything else.",
        "Translate this text into English. Do not write anything else."
    ];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasImage))]
    [NotifyPropertyChangedFor(nameof(CanAnalyze))]
    private Bitmap? _previewImage;

    [ObservableProperty]
    private string _prompt;

    [ObservableProperty]
    private string? _selectedPreset;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasResult))]
    private string _resultText = string.Empty;

    [ObservableProperty]
    private string _elapsedText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIdle))]
    [NotifyPropertyChangedFor(nameof(CanAnalyze))]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public Func<Task<string?>>? SelectImageRequested { get; set; }
    public Func<Task<byte[]?>>? SelectRegionRequested { get; set; }
    public Func<string, Task>? CopyTextRequested { get; set; }
    public Func<Task>? EditOptionsRequested { get; set; }
    public Action? PlayNotificationSound { get; set; }

    public bool HasImage => PreviewImage != null;
    public bool HasResult => !string.IsNullOrWhiteSpace(ResultText);
    public bool IsIdle => !IsBusy;
    public bool CanAnalyze => HasImage && _options.HasAPIKey && !IsBusy;
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public string ImageDescription => !string.IsNullOrWhiteSpace(_imagePath) ? Path.GetFileName(_imagePath) : "Captured region";

    public byte[]? GetImageData()
    {
        if (_imageData != null)
        {
            return _imageData;
        }

        return !string.IsNullOrWhiteSpace(_imagePath) && File.Exists(_imagePath) ? File.ReadAllBytes(_imagePath) : null;
    }

    public AnalyzeImageViewModel(string? imagePath, AnalyzeImageOptions options, AnalyzeImageHandler analyze,
        Action<AnalyzeImageOptions>? optionsChanged = null)
    {
        _options = options;
        _analyze = analyze;
        _optionsChanged = optionsChanged;
        _prompt = options.Input;

        if (!string.IsNullOrWhiteSpace(imagePath))
        {
            LoadImage(imagePath);
        }
    }

    public async Task InitializeAsync()
    {
        if (!HasImage && _options.AutoStartRegion)
        {
            await CaptureRegionAsync(false);
        }

        if (HasImage && _options.AutoStartAnalyze)
        {
            await AnalyzeAsync();
        }
    }

    partial void OnPromptChanged(string value)
    {
        _options.Input = value;
        _optionsChanged?.Invoke(_options);
    }

    partial void OnSelectedPresetChanged(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            Prompt = value;
        }
    }

    partial void OnErrorMessageChanged(string value) => OnPropertyChanged(nameof(HasError));

    [RelayCommand]
    private async Task SelectImageAsync()
    {
        if (IsBusy || SelectImageRequested == null)
        {
            return;
        }

        string? filePath = await SelectImageRequested();
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            LoadImage(filePath);
        }
    }

    [RelayCommand]
    private Task SelectRegionAsync() => CaptureRegionAsync(true);

    [RelayCommand]
    private async Task AnalyzeAsync()
    {
        if (!CanAnalyze)
        {
            if (HasImage && !_options.HasAPIKey)
            {
                ErrorMessage = "Configure an API key in Options before analyzing.";
            }
            return;
        }

        IsBusy = true;
        ResultText = "Thinking...";
        ElapsedText = string.Empty;
        ErrorMessage = string.Empty;
        Stopwatch timer = Stopwatch.StartNew();

        try
        {
            string result = await _analyze(_imagePath, _imageData, _options.Clone());
            timer.Stop();
            ResultText = result?.ReplaceLineEndings("\r\n") ?? string.Empty;
            ElapsedText = $"Time: {timer.ElapsedMilliseconds:N0} ms";

            if (_options.AutoCopyResult && HasResult && CopyTextRequested != null)
            {
                await CopyTextRequested(ResultText);
            }

            if (HasResult)
            {
                PlayNotificationSound?.Invoke();
            }
        }
        catch (Exception ex)
        {
            ResultText = string.Empty;
            ErrorMessage = ex.Message;
            ToolsDiagnostics.ReportWarning(nameof(AnalyzeImageViewModel), "Image analysis failed.", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CopyResultAsync()
    {
        if (HasResult && CopyTextRequested != null)
        {
            await CopyTextRequested(ResultText);
            PlayNotificationSound?.Invoke();
        }
    }

    [RelayCommand]
    private async Task EditOptionsAsync()
    {
        if (EditOptionsRequested != null)
        {
            await EditOptionsRequested();
            OnPropertyChanged(nameof(CanAnalyze));
            AnalyzeCommand.NotifyCanExecuteChanged();
        }
    }

    private async Task CaptureRegionAsync(bool analyzeAfterCapture)
    {
        if (IsBusy || SelectRegionRequested == null)
        {
            return;
        }

        byte[]? data = await SelectRegionRequested();
        if (data is { Length: > 0 } && LoadImage(data) && analyzeAfterCapture)
        {
            await AnalyzeAsync();
        }
    }

    public bool LoadImage(string filePath)
    {
        try
        {
            byte[] data = File.ReadAllBytes(filePath);
            if (LoadPreview(data))
            {
                _imagePath = filePath;
                _imageData = null;
                OnPropertyChanged(nameof(ImageDescription));
                return true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }

        return false;
    }

    private bool LoadImage(byte[] data)
    {
        if (!LoadPreview(data))
        {
            return false;
        }

        _imagePath = null;
        _imageData = data;
        OnPropertyChanged(nameof(ImageDescription));
        return true;
    }

    private bool LoadPreview(byte[] data)
    {
        try
        {
            using MemoryStream stream = new(data, writable: false);
            Bitmap preview = new(stream);
            PreviewImage?.Dispose();
            PreviewImage = preview;
            ErrorMessage = string.Empty;
            return true;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return false;
        }
    }

    public void Dispose()
    {
        PreviewImage?.Dispose();
    }
}
