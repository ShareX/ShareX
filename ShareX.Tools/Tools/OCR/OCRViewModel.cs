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
using ShareX.HelpersLib;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ShareX.Tools;

public sealed record OCRLanguageOption(string DisplayName, string LanguageTag)
{
    public override string ToString() => DisplayName;
}

public sealed partial class OCRServiceLinkOption : ObservableObject
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _url;

    public OCRServiceLinkOption() : this(string.Empty, string.Empty)
    {
    }

    public OCRServiceLinkOption(string name, string url)
    {
        _name = name;
        _url = url;
    }

    public string? GenerateLink(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(Url))
        {
            return null;
        }

        return string.Format(Url, URLHelpers.URLEncode(input));
    }

    public override string ToString() => Name;
}

public delegate Task<string> OCRRecognitionHandler(byte[] imageData, string languageTag, float scaleFactor, bool singleLine);
public delegate Task<byte[]?> OCRRegionCaptureHandler();

public sealed partial class OCRViewModel : ViewModelBase, IDisposable
{
    private readonly OCROptions _options;
    private readonly OCRRecognitionHandler _recognize;
    private readonly Action<OCROptions>? _optionsChanged;
    private readonly Action? _openHelp;
    private byte[] _sourceImageData;
    private bool _loaded;
    private int _recognitionVersion;

    public ObservableCollection<OCRLanguageOption> Languages { get; } = [];
    public ObservableCollection<OCRServiceLinkOption> ServiceLinks { get; } = [];

    [ObservableProperty]
    private Bitmap? _sourceImage;

    [ObservableProperty]
    private OCRLanguageOption? _selectedLanguage;

    [ObservableProperty]
    private decimal _scaleFactor;

    [ObservableProperty]
    private bool _singleLine;

    [ObservableProperty]
    private bool _autoCopy;

    [ObservableProperty]
    private bool _closeAfterOpeningService;

    [ObservableProperty]
    private OCRServiceLinkOption? _selectedServiceLink;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasEditingService))]
    private OCRServiceLinkOption? _editingServiceLink;

    [ObservableProperty]
    private bool _isEditingServices;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIdle))]
    [NotifyPropertyChangedFor(nameof(CanUseResult))]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanUseResult))]
    private string _resultText = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public Func<Task<byte[]?>>? SelectRegionRequested { get; set; }
    public Func<Task<byte[]?>>? SelectImageRequested { get; set; }
    public Func<string, Task>? CopyTextRequested { get; set; }
    public Action? CloseRequested { get; set; }

    public bool IsIdle => !IsBusy;
    public bool CanUseResult => !IsBusy && !string.IsNullOrWhiteSpace(ResultText);
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool HasLanguages => Languages.Count > 0;
    public bool HasServices => ServiceLinks.Count > 0;
    public bool HasEditingService => EditingServiceLink != null;
    public string Result => ResultText.Trim();

    public OCRViewModel(
        byte[] sourceImageData,
        IEnumerable<OCRLanguageOption> languages,
        OCROptions options,
        OCRRecognitionHandler recognize,
        Action<OCROptions>? optionsChanged = null,
        Action? openHelp = null)
    {
        _sourceImageData = sourceImageData;
        _options = options;
        _recognize = recognize;
        _optionsChanged = optionsChanged;
        _openHelp = openHelp;

        foreach (OCRLanguageOption language in languages.OrderBy(x => x.DisplayName, StringComparer.CurrentCultureIgnoreCase))
        {
            Languages.Add(language);
        }

        _selectedLanguage = Languages.FirstOrDefault(x => x.LanguageTag.Equals(options.Language, StringComparison.OrdinalIgnoreCase))
            ?? Languages.FirstOrDefault();
        _scaleFactor = (decimal)Math.Clamp(options.ScaleFactor, 1f, 4f);
        _singleLine = options.SingleLine;
        _autoCopy = options.AutoCopy;
        _closeAfterOpeningService = options.CloseWindowAfterOpeningServiceLink;

        foreach (OCRServiceLinkOption service in options.ServiceLinks)
        {
            AddService(service);
        }
        _selectedServiceLink = ServiceLinks.Count > 0
            ? ServiceLinks[Math.Clamp(options.SelectedServiceLink, 0, ServiceLinks.Count - 1)]
            : null;

        ReplaceSourcePreview(sourceImageData);
    }

    public async Task StartAsync()
    {
        _loaded = true;
        SaveOptions();
        await RecognizeAsync();
    }

    partial void OnSelectedLanguageChanged(OCRLanguageOption? value) => RerunRecognition();
    partial void OnScaleFactorChanged(decimal value) => RerunRecognition();
    partial void OnSingleLineChanged(bool value) => RerunRecognition();

    partial void OnAutoCopyChanged(bool value) => SaveOptions();
    partial void OnCloseAfterOpeningServiceChanged(bool value) => SaveOptions();

    partial void OnSelectedServiceLinkChanged(OCRServiceLinkOption? value)
    {
        SaveOptions();
    }

    partial void OnResultTextChanged(string value)
    {
        ErrorMessage = string.Empty;
        OnPropertyChanged(nameof(HasError));
    }

    [RelayCommand]
    private async Task SelectRegionAsync()
    {
        if (IsBusy || SelectRegionRequested == null)
        {
            return;
        }

        await ReplaceSourceAsync(await SelectRegionRequested());
    }

    [RelayCommand]
    private async Task SelectImageAsync()
    {
        if (IsBusy || SelectImageRequested == null)
        {
            return;
        }

        await ReplaceSourceAsync(await SelectImageRequested());
    }

    [RelayCommand]
    private async Task CopyAllAsync()
    {
        if (CanUseResult && CopyTextRequested != null)
        {
            await CopyTextRequested(ResultText);
        }
    }

    [RelayCommand]
    private void OpenService()
    {
        if (!CanUseResult || SelectedServiceLink == null)
        {
            return;
        }

        string? url = SelectedServiceLink.GenerateLink(Result);
        if (!string.IsNullOrWhiteSpace(url))
        {
            URLHelpers.OpenURL(url);
            if (CloseAfterOpeningService)
            {
                CloseRequested?.Invoke();
            }
        }
    }

    [RelayCommand]
    private void OpenHelp() => _openHelp?.Invoke();

    [RelayCommand]
    private void EditServices()
    {
        EditingServiceLink = SelectedServiceLink ?? ServiceLinks.FirstOrDefault();
        IsEditingServices = true;
    }

    [RelayCommand]
    private void CloseServiceEditor()
    {
        IsEditingServices = false;
        EditingServiceLink = null;
        SaveOptions();
    }

    [RelayCommand]
    private void NewService()
    {
        OCRServiceLinkOption service = new("Name", "https://example.com/search?q={0}");
        AddService(service);
        EditingServiceLink = service;
        SelectedServiceLink = service;
        SaveOptions();
    }

    [RelayCommand]
    private void RemoveService()
    {
        if (EditingServiceLink == null)
        {
            return;
        }

        int index = ServiceLinks.IndexOf(EditingServiceLink);
        EditingServiceLink.PropertyChanged -= OnServicePropertyChanged;
        ServiceLinks.Remove(EditingServiceLink);
        EditingServiceLink = ServiceLinks.Count > 0 ? ServiceLinks[Math.Clamp(index - 1, 0, ServiceLinks.Count - 1)] : null;
        SelectedServiceLink = EditingServiceLink;
        NotifyServiceState();
        SaveOptions();
    }

    [RelayCommand]
    private void ResetServices()
    {
        foreach (OCRServiceLinkOption service in ServiceLinks)
        {
            service.PropertyChanged -= OnServicePropertyChanged;
        }
        ServiceLinks.Clear();
        foreach (OCRServiceLinkOption service in OCROptions.DefaultServiceLinks)
        {
            AddService(service);
        }
        EditingServiceLink = ServiceLinks.FirstOrDefault();
        SelectedServiceLink = EditingServiceLink;
        NotifyServiceState();
        SaveOptions();
    }

    private async Task RecognizeAsync()
    {
        if (_sourceImageData.Length == 0 || SelectedLanguage == null)
        {
            return;
        }

        int version = ++_recognitionVersion;
        IsBusy = true;
        ErrorMessage = string.Empty;
        ResultText = string.Empty;

        try
        {
            string result = await _recognize(_sourceImageData, SelectedLanguage.LanguageTag, (float)ScaleFactor, SingleLine);
            if (version == _recognitionVersion)
            {
                ResultText = result;
                if (AutoCopy && !string.IsNullOrWhiteSpace(result) && CopyTextRequested != null)
                {
                    await CopyTextRequested(result);
                }
            }
        }
        catch (Exception ex)
        {
            if (version == _recognitionVersion)
            {
                ErrorMessage = ex.Message;
                OnPropertyChanged(nameof(HasError));
                ToolsDiagnostics.ReportWarning(nameof(OCRViewModel), "OCR failed.", ex);
            }
        }
        finally
        {
            if (version == _recognitionVersion)
            {
                IsBusy = false;
            }
        }
    }

    private async Task ReplaceSourceAsync(byte[]? imageData)
    {
        if (imageData is { Length: > 0 })
        {
            _sourceImageData = imageData;
            ReplaceSourcePreview(imageData);
            await RecognizeAsync();
        }
    }

    private void RerunRecognition()
    {
        if (_loaded)
        {
            SaveOptions();
            _ = RecognizeAsync();
        }
    }

    private void SaveOptions()
    {
        if (SelectedLanguage != null)
        {
            _options.Language = SelectedLanguage.LanguageTag;
        }
        _options.ScaleFactor = (float)ScaleFactor;
        _options.SingleLine = SingleLine;
        _options.AutoCopy = AutoCopy;
        _options.CloseWindowAfterOpeningServiceLink = CloseAfterOpeningService;
        _options.SelectedServiceLink = SelectedServiceLink == null ? 0 : Math.Max(0, ServiceLinks.IndexOf(SelectedServiceLink));
        _options.ServiceLinks = ServiceLinks.ToList();
        _optionsChanged?.Invoke(_options);
    }

    private void AddService(OCRServiceLinkOption service)
    {
        service.PropertyChanged += OnServicePropertyChanged;
        ServiceLinks.Add(service);
        NotifyServiceState();
    }

    private void OnServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        SaveOptions();
    }

    private void NotifyServiceState()
    {
        OnPropertyChanged(nameof(HasServices));
    }

    private void ReplaceSourcePreview(byte[] imageData)
    {
        try
        {
            using MemoryStream stream = new(imageData, writable: false);
            Bitmap preview = new(stream);
            SourceImage?.Dispose();
            SourceImage = preview;
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(OCRViewModel), "Failed to load OCR source preview.", ex);
        }
    }

    public void Dispose()
    {
        ++_recognitionVersion;
        SourceImage?.Dispose();
        foreach (OCRServiceLinkOption service in ServiceLinks)
        {
            service.PropertyChanged -= OnServicePropertyChanged;
        }
    }
}
