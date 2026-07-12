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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShareX.Tools;

public sealed record IndexerOutputChoice(string Name, IndexerOutput Value, string Extension);

public sealed partial class DirectoryIndexerViewModel : ViewModelBase
{
    private readonly IndexerSettings _settings;

    public IReadOnlyList<IndexerOutputChoice> OutputChoices { get; } =
    [
        new("HTML", IndexerOutput.Html, "html"),
        new("Text", IndexerOutput.Txt, "txt"),
        new("XML", IndexerOutput.Xml, "xml"),
        new("JSON", IndexerOutput.Json, "json")
    ];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanIndex))]
    private string _folderPath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsHtml))]
    [NotifyPropertyChangedFor(nameof(IsText))]
    [NotifyPropertyChangedFor(nameof(IsXml))]
    [NotifyPropertyChangedFor(nameof(IsJson))]
    [NotifyPropertyChangedFor(nameof(CanPreview))]
    [NotifyPropertyChangedFor(nameof(ShowsHtmlPreviewNotice))]
    private IndexerOutputChoice _selectedOutput;

    [ObservableProperty] private bool _skipHiddenFolders;
    [ObservableProperty] private bool _skipHiddenFiles;
    [ObservableProperty] private bool _foldersOnly;
    [ObservableProperty] private decimal _maxDepthLevel;
    [ObservableProperty] private bool _showSizeInfo;
    [ObservableProperty] private bool _addFooter;
    [ObservableProperty] private string _indentationText;
    [ObservableProperty] private bool _addEmptyLineAfterFolders;
    [ObservableProperty] private bool _useCustomCSSFile;
    [ObservableProperty] private bool _displayPath;
    [ObservableProperty] private bool _displayPathLimited;
    [ObservableProperty] private string _customCSSFilePath;
    [ObservableProperty] private bool _useAttribute;
    [ObservableProperty] private bool _createParseableJson;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIdle))]
    [NotifyPropertyChangedFor(nameof(CanIndex))]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSource))]
    [NotifyPropertyChangedFor(nameof(CanPreview))]
    [NotifyPropertyChangedFor(nameof(ShowsHtmlPreviewNotice))]
    private string _source = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string _errorMessage = string.Empty;

    public Func<Task<string?>>? SelectFolderRequested { get; set; }
    public Func<Task<string?>>? SelectCSSFileRequested { get; set; }
    public Func<string, string, string, Task<bool>>? SaveRequested { get; set; }
    public Func<string, IndexerOutput, Task>? UploadRequested { get; set; }
    public Action? CloseRequested { get; set; }

    public bool IsIdle => !IsBusy;
    public bool CanIndex => !IsBusy && Directory.Exists(FolderPath);
    public bool HasSource => !string.IsNullOrWhiteSpace(Source);
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool IsHtml => SelectedOutput.Value == IndexerOutput.Html;
    public bool IsText => SelectedOutput.Value == IndexerOutput.Txt;
    public bool IsXml => SelectedOutput.Value == IndexerOutput.Xml;
    public bool IsJson => SelectedOutput.Value == IndexerOutput.Json;
    public bool CanPreview => HasSource && !IsHtml;
    public bool ShowsHtmlPreviewNotice => HasSource && IsHtml;
    public bool CanLimitDisplayPath => IsHtml && DisplayPath;

    public DirectoryIndexerViewModel(IndexerSettings settings)
    {
        _settings = settings;
        _selectedOutput = OutputChoices.First(x => x.Value == settings.Output);
        _skipHiddenFolders = settings.SkipHiddenFolders;
        _skipHiddenFiles = settings.SkipHiddenFiles;
        _foldersOnly = settings.SkipFiles;
        _maxDepthLevel = Math.Max(settings.MaxDepthLevel, 0);
        _showSizeInfo = settings.ShowSizeInfo;
        _addFooter = settings.AddFooter;
        _indentationText = settings.IndentationText ?? string.Empty;
        _addEmptyLineAfterFolders = settings.AddEmptyLineAfterFolders;
        _useCustomCSSFile = settings.UseCustomCSSFile;
        _displayPath = settings.DisplayPath;
        _displayPathLimited = settings.DisplayPathLimited;
        _customCSSFilePath = settings.CustomCSSFilePath ?? string.Empty;
        _useAttribute = settings.UseAttribute;
        _createParseableJson = settings.CreateParseableJson;
    }

    public async Task InitializeAsync()
    {
        await BrowseFolderAsync();
    }

    [RelayCommand]
    private async Task BrowseFolderAsync()
    {
        if (IsBusy || SelectFolderRequested == null)
        {
            return;
        }

        string? folder = await SelectFolderRequested();
        if (!string.IsNullOrWhiteSpace(folder))
        {
            FolderPath = folder;
            await IndexAsync();
        }
    }

    [RelayCommand]
    private async Task BrowseCSSAsync()
    {
        if (!IsBusy && SelectCSSFileRequested != null && await SelectCSSFileRequested() is { } filePath)
        {
            CustomCSSFilePath = filePath;
        }
    }

    [RelayCommand]
    private async Task IndexAsync()
    {
        if (!CanIndex)
        {
            return;
        }

        PersistSettings();
        IsBusy = true;
        Source = string.Empty;
        ErrorMessage = string.Empty;

        try
        {
            Source = await Task.Run(() => Indexer.Index(FolderPath, _settings));
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            ToolsDiagnostics.ReportWarning(nameof(DirectoryIndexerViewModel), "Directory indexing failed.", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveAsAsync()
    {
        if (!HasSource || SaveRequested == null)
        {
            return;
        }

        string suggestedName = $"Index for {Path.GetFileName(FolderPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))}";
        if (await SaveRequested(Source, suggestedName, SelectedOutput.Extension))
        {
            CloseRequested?.Invoke();
        }
    }

    [RelayCommand]
    private async Task UploadAsync()
    {
        if (HasSource && UploadRequested != null)
        {
            await UploadRequested(Source, SelectedOutput.Value);
            CloseRequested?.Invoke();
        }
    }

    partial void OnSelectedOutputChanged(IndexerOutputChoice value)
    {
        _settings.Output = value.Value;
        OnPropertyChanged(nameof(CanLimitDisplayPath));

        if (CanIndex)
        {
            _ = IndexAsync();
        }
    }

    partial void OnSkipHiddenFoldersChanged(bool value) => _settings.SkipHiddenFolders = value;
    partial void OnSkipHiddenFilesChanged(bool value) => _settings.SkipHiddenFiles = value;
    partial void OnFoldersOnlyChanged(bool value) => _settings.SkipFiles = value;
    partial void OnMaxDepthLevelChanged(decimal value) => _settings.MaxDepthLevel = Math.Max((int)value, 0);
    partial void OnShowSizeInfoChanged(bool value) => _settings.ShowSizeInfo = value;
    partial void OnAddFooterChanged(bool value) => _settings.AddFooter = value;
    partial void OnIndentationTextChanged(string value) => _settings.IndentationText = value;
    partial void OnAddEmptyLineAfterFoldersChanged(bool value) => _settings.AddEmptyLineAfterFolders = value;
    partial void OnUseCustomCSSFileChanged(bool value) => _settings.UseCustomCSSFile = value;
    partial void OnDisplayPathChanged(bool value)
    {
        _settings.DisplayPath = value;
        OnPropertyChanged(nameof(CanLimitDisplayPath));
    }
    partial void OnDisplayPathLimitedChanged(bool value) => _settings.DisplayPathLimited = value;
    partial void OnCustomCSSFilePathChanged(string value) => _settings.CustomCSSFilePath = value;
    partial void OnUseAttributeChanged(bool value) => _settings.UseAttribute = value;
    partial void OnCreateParseableJsonChanged(bool value) => _settings.CreateParseableJson = value;

    public void SetFolder(string folderPath)
    {
        if (!IsBusy && Directory.Exists(folderPath))
        {
            FolderPath = folderPath;
        }
    }

    private void PersistSettings()
    {
        _settings.Output = SelectedOutput.Value;
        _settings.SkipHiddenFolders = SkipHiddenFolders;
        _settings.SkipHiddenFiles = SkipHiddenFiles;
        _settings.SkipFiles = FoldersOnly;
        _settings.MaxDepthLevel = Math.Max((int)MaxDepthLevel, 0);
        _settings.ShowSizeInfo = ShowSizeInfo;
        _settings.AddFooter = AddFooter;
        _settings.IndentationText = IndentationText;
        _settings.AddEmptyLineAfterFolders = AddEmptyLineAfterFolders;
        _settings.UseCustomCSSFile = UseCustomCSSFile;
        _settings.DisplayPath = DisplayPath;
        _settings.DisplayPathLimited = DisplayPathLimited;
        _settings.CustomCSSFilePath = CustomCSSFilePath;
        _settings.UseAttribute = UseAttribute;
        _settings.CreateParseableJson = CreateParseableJson;
    }
}
