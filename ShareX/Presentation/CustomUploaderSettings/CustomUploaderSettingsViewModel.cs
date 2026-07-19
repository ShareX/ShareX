#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using ShareX.HelpersLib;
using ShareX.UploadersLib;
using ShareX.UploadersLib.FileUploaders;
using ShareX.UploadersLib.ImageUploaders;
using ShareX.UploadersLib.SharingServices;
using ShareX.UploadersLib.TextUploaders;
using ShareX.UploadersLib.URLShorteners;
using ShareX.AvaloniaUI.Theming;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShareX;

public sealed class CustomUploaderSettingsViewModel : CustomUploaderNotifyObject
{
    private readonly UploadersConfig _config;
    private CustomUploaderEditorItem? _selectedUploader;
    private bool _isTesting;
    private bool _suppressSelectionUpdates;
    private string _searchText = string.Empty;
    private int _selectedSectionIndex;
    private string _statusMessage = "Changes are applied immediately.";

    public ObservableCollection<CustomUploaderEditorItem> Uploaders { get; } = [];
    public ObservableCollection<CustomUploaderEditorItem> FilteredUploaders { get; } = [];
    public IReadOnlyList<CustomUploaderEditorSection> Sections { get; } =
    [
        new("Overview", LucideIcons.layout_dashboard),
        new("Request", LucideIcons.send),
        new("Body", LucideIcons.braces),
        new("Response", LucideIcons.reply),
        new("Test", LucideIcons.flask_conical)
    ];
    public string[] RequestMethods { get; } = Enum.GetNames<HttpMethod>();
    public string[] BodyTypes { get; } = Enum.GetValues<CustomUploaderBody>().Select(x => x.GetLocalizedDescription()).ToArray();
    public bool IsDeveloperMode => HelpersOptions.DevMode;

    public CustomUploaderEditorItem? SelectedUploader
    {
        get => _selectedUploader;
        set
        {
            if (!SetField(ref _selectedUploader, value)) return;
            OnPropertyChanged(nameof(HasSelection));
        }
    }

    public bool HasSelection => SelectedUploader != null;
    public bool HasUploaders => Uploaders.Count > 0;
    public bool HasFilteredUploaders => FilteredUploaders.Count > 0;

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (!SetField(ref _searchText, value ?? string.Empty)) return;
            ApplyFilter();
        }
    }

    public string LibraryCountText => string.IsNullOrWhiteSpace(SearchText)
        ? $"{Uploaders.Count} uploader{(Uploaders.Count == 1 ? string.Empty : "s")}" 
        : $"{FilteredUploaders.Count} of {Uploaders.Count}";

    public string EmptyLibraryMessage => Uploaders.Count == 0
        ? "Create a custom uploader or import an .sxcu file."
        : "No uploaders match this search.";

    public int SelectedSectionIndex
    {
        get => _selectedSectionIndex;
        set
        {
            int normalized = value.Clamp(0, Sections.Count - 1);
            if (!SetField(ref _selectedSectionIndex, normalized)) return;
            OnPropertyChanged(nameof(IsOverviewSection));
            OnPropertyChanged(nameof(IsRequestSection));
            OnPropertyChanged(nameof(IsBodySection));
            OnPropertyChanged(nameof(IsResponseSection));
            OnPropertyChanged(nameof(IsTestSection));
        }
    }

    public bool IsOverviewSection => SelectedSectionIndex == 0;
    public bool IsRequestSection => SelectedSectionIndex == 1;
    public bool IsBodySection => SelectedSectionIndex == 2;
    public bool IsResponseSection => SelectedSectionIndex == 3;
    public bool IsTestSection => SelectedSectionIndex == 4;

    public bool IsTesting
    {
        get => _isTesting;
        private set => SetField(ref _isTesting, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetField(ref _statusMessage, value);
    }

    public int CustomImageUploaderSelected
    {
        get => GetSelection(_config.CustomImageUploaderSelected);
        set => SetSelection(_config.CustomImageUploaderSelected, value, x => _config.CustomImageUploaderSelected = x, nameof(CustomImageUploaderSelected));
    }

    public int CustomTextUploaderSelected
    {
        get => GetSelection(_config.CustomTextUploaderSelected);
        set => SetSelection(_config.CustomTextUploaderSelected, value, x => _config.CustomTextUploaderSelected = x, nameof(CustomTextUploaderSelected));
    }

    public int CustomFileUploaderSelected
    {
        get => GetSelection(_config.CustomFileUploaderSelected);
        set => SetSelection(_config.CustomFileUploaderSelected, value, x => _config.CustomFileUploaderSelected = x, nameof(CustomFileUploaderSelected));
    }

    public int CustomURLShortenerSelected
    {
        get => GetSelection(_config.CustomURLShortenerSelected);
        set => SetSelection(_config.CustomURLShortenerSelected, value, x => _config.CustomURLShortenerSelected = x, nameof(CustomURLShortenerSelected));
    }

    public int CustomURLSharingServiceSelected
    {
        get => GetSelection(_config.CustomURLSharingServiceSelected);
        set => SetSelection(_config.CustomURLSharingServiceSelected, value, x => _config.CustomURLSharingServiceSelected = x, nameof(CustomURLSharingServiceSelected));
    }

    public UploadResult? LastResult { get; private set; }

    public CustomUploaderSettingsViewModel(UploadersConfig config)
    {
        _config = config;
        _config.CustomUploadersList ??= [];
        Reload();
    }

    public void Reload(bool selectLast = false)
    {
        if (selectLast) SearchText = string.Empty;
        _suppressSelectionUpdates = true;
        try
        {
            Uploaders.Clear();
            foreach (CustomUploaderItem item in _config.CustomUploadersList) Uploaders.Add(CreateEditor(item));
        }
        finally
        {
            _suppressSelectionUpdates = false;
        }

        NormalizeSelections();
        ApplyFilter(false);
        SelectedUploader = Uploaders.Count == 0
            ? null
            : selectLast ? Uploaders[^1] : Uploaders[CustomImageUploaderSelected];
        if (SelectedUploader != null && !FilteredUploaders.Contains(SelectedUploader))
        {
            SelectedUploader = FilteredUploaders.FirstOrDefault();
        }
        NotifySelections();
    }

    public void NewUploader()
    {
        Add(CustomUploaderItem.Init());
        StatusMessage = "New custom uploader created.";
    }

    public void DuplicateSelected()
    {
        if (SelectedUploader == null) return;
        Add(SelectedUploader.Model.Copy());
        StatusMessage = "Custom uploader duplicated.";
    }

    public void RemoveSelected()
    {
        if (SelectedUploader == null) return;
        int index = Uploaders.IndexOf(SelectedUploader);
        if (index < 0) return;

        _suppressSelectionUpdates = true;
        try
        {
            Uploaders.RemoveAt(index);
            _config.CustomUploadersList.RemoveAt(index);
            FixSelectionsAfterRemoval(index);
        }
        finally
        {
            _suppressSelectionUpdates = false;
        }
        ApplyFilter(false);
        SelectedUploader = Uploaders.Count == 0 ? null : Uploaders[Math.Min(index, Uploaders.Count - 1)];
        if (SelectedUploader != null && !FilteredUploaders.Contains(SelectedUploader))
        {
            SelectedUploader = FilteredUploaders.FirstOrDefault();
        }
        NotifySelections();
        StatusMessage = "Custom uploader removed.";
    }

    public void Clear()
    {
        _suppressSelectionUpdates = true;
        try
        {
            Uploaders.Clear();
            _config.CustomUploadersList.Clear();
            _config.CustomImageUploaderSelected = 0;
            _config.CustomTextUploaderSelected = 0;
            _config.CustomFileUploaderSelected = 0;
            _config.CustomURLShortenerSelected = 0;
            _config.CustomURLSharingServiceSelected = 0;
        }
        finally
        {
            _suppressSelectionUpdates = false;
        }
        ApplyFilter(false);
        SelectedUploader = null;
        NotifySelections();
        StatusMessage = "All custom uploaders removed.";
    }

    public int ImportFiles(IEnumerable<string> filePaths)
    {
        int imported = 0;
        foreach (string path in filePaths.Where(x => x.EndsWith(".sxcu", StringComparison.OrdinalIgnoreCase)))
        {
            try
            {
                CustomUploaderItem? item = JsonHelpers.DeserializeFromFile<CustomUploaderItem>(path);
                if (item == null) continue;
                item.CheckBackwardCompatibility();
                Add(item);
                imported++;
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                StatusMessage = exception.Message;
            }
        }

        if (imported > 0) StatusMessage = $"Imported {imported} custom uploader(s).";
        return imported;
    }

    public string? ExportSelected(string filePath)
    {
        if (SelectedUploader == null) return "Select a custom uploader to export.";
        CustomUploaderItem item = SelectedUploader.Model;
        if (string.IsNullOrWhiteSpace(item.RequestURL)) return "Request URL must be configured before exporting.";
        if (item.DestinationType == CustomUploaderDestinationType.None) return "At least one destination type must be selected before exporting.";

        try
        {
            JsonHelpers.SerializeToFile(item, filePath, Newtonsoft.Json.DefaultValueHandling.Ignore, Newtonsoft.Json.NullValueHandling.Ignore);
            StatusMessage = $"Exported {item}.";
            return null;
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            return exception.Message;
        }
    }

    public int ExportAll(string folderPath)
    {
        int exported = 0;
        foreach (CustomUploaderEditorItem editor in Uploaders)
        {
            string filePath = Path.Combine(folderPath, editor.Model.GetFileName());
            if (ExportItem(editor.Model, filePath)) exported++;
        }

        StatusMessage = $"Exported {exported} custom uploader(s).";
        return exported;
    }

    public int UpdateFolder(string folderPath)
    {
        int updated = 0;
        foreach (string filePath in Directory.GetFiles(folderPath, "*.sxcu", SearchOption.TopDirectoryOnly))
        {
            try
            {
                CustomUploaderItem? item = JsonHelpers.DeserializeFromFile<CustomUploaderItem>(filePath);
                if (item == null) continue;
                item.CheckBackwardCompatibility();
                if (ExportItem(item, filePath)) updated++;
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
            }
        }

        StatusMessage = $"Updated {updated} custom uploader file(s).";
        return updated;
    }

    public async Task<UploadResult?> TestAsync(CustomUploaderDestinationType type, int index, string? textInput = null)
    {
        if (!_config.CustomUploadersList.IsValidIndex(index) || IsTesting) return null;
        IsTesting = true;
        SelectedUploader = Uploaders[index];
        StatusMessage = $"Testing {SelectedUploader.Title}...";

        UploadResult? result = await Task.Run(() => RunTest(type, _config.CustomUploadersList[index], textInput));
        LastResult = result;
        IsTesting = false;
        StatusMessage = result == null ? "Test cancelled." : result.IsError ? "Test failed." : "Test completed.";
        return result;
    }

    private static UploadResult? RunTest(CustomUploaderDestinationType type, CustomUploaderItem item, string? textInput)
    {
        try
        {
            UploadResult? result = null;
            switch (type)
            {
                case CustomUploaderDestinationType.ImageUploader:
                    using (Stream stream = ShareXResources.Logo.GetStream())
                    {
                        CustomImageUploader uploader = new(item);
                        result = uploader.Upload(stream, "Test.png");
                        result.Errors.Add(uploader.Errors);
                    }
                    break;
                case CustomUploaderDestinationType.TextUploader:
                    if (string.IsNullOrEmpty(textInput)) return null;
                    CustomTextUploader textUploader = new(item);
                    result = textUploader.UploadText(textInput, "Test.txt");
                    result.Errors.Add(textUploader.Errors);
                    break;
                case CustomUploaderDestinationType.FileUploader:
                    using (Stream stream = ShareXResources.Logo.GetStream())
                    {
                        CustomFileUploader uploader = new(item);
                        result = uploader.Upload(stream, "Test.png");
                        result.Errors.Add(uploader.Errors);
                    }
                    break;
                case CustomUploaderDestinationType.URLShortener:
                    CustomURLShortener shortener = new(item);
                    result = shortener.ShortenURL(Links.Website);
                    result.Errors.Add(shortener.Errors);
                    break;
                case CustomUploaderDestinationType.URLSharingService:
                    CustomURLSharer sharer = new(item);
                    result = sharer.ShareURL(Links.Website);
                    result.Errors.Add(sharer.Errors);
                    break;
            }

            return result;
        }
        catch (Exception exception)
        {
            UploadResult result = new();
            result.Errors.Add(exception.Message);
            return result;
        }
    }

    private void Add(CustomUploaderItem item)
    {
        SearchText = string.Empty;
        _config.CustomUploadersList.Add(item);
        CustomUploaderEditorItem editor = CreateEditor(item);
        Uploaders.Add(editor);
        ApplyFilter(false);
        SelectedUploader = editor;
        NormalizeSelections();
        NotifySelections();
    }

    private bool ExportItem(CustomUploaderItem item, string filePath)
    {
        try
        {
            JsonHelpers.SerializeToFile(item, filePath, Newtonsoft.Json.DefaultValueHandling.Ignore, Newtonsoft.Json.NullValueHandling.Ignore);
            return true;
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            StatusMessage = exception.Message;
            return false;
        }
    }

    private CustomUploaderEditorItem CreateEditor(CustomUploaderItem item)
    {
        CustomUploaderEditorItem editor = new(item);
        editor.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(CustomUploaderEditorItem.Title) or nameof(CustomUploaderEditorItem.HostName))
            {
                ApplyFilter();
            }
        };
        return editor;
    }

    private void ApplyFilter(bool updateSelection = true)
    {
        string query = SearchText.Trim();
        IEnumerable<CustomUploaderEditorItem> matches = Uploaders;
        if (!string.IsNullOrEmpty(query))
        {
            matches = matches.Where(x =>
                x.Title.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                x.HostName.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                x.RequestURL.Contains(query, StringComparison.CurrentCultureIgnoreCase));
        }

        FilteredUploaders.Clear();
        foreach (CustomUploaderEditorItem editor in matches) FilteredUploaders.Add(editor);

        if (updateSelection && (SelectedUploader == null || !FilteredUploaders.Contains(SelectedUploader)))
        {
            SelectedUploader = FilteredUploaders.FirstOrDefault();
        }

        OnPropertyChanged(nameof(LibraryCountText));
        OnPropertyChanged(nameof(EmptyLibraryMessage));
        OnPropertyChanged(nameof(HasUploaders));
        OnPropertyChanged(nameof(HasFilteredUploaders));
    }

    private int GetSelection(int value) => Uploaders.Count == 0 ? -1 : value.Clamp(0, Uploaders.Count - 1);

    private void SetSelection(int current, int value, Action<int> setter, string propertyName)
    {
        if (_suppressSelectionUpdates) return;
        int normalized = Uploaders.Count == 0 ? 0 : value.Clamp(0, Uploaders.Count - 1);
        if (current == normalized) return;
        setter(normalized);
        OnPropertyChanged(propertyName);
    }

    private void NormalizeSelections()
    {
        if (Uploaders.Count == 0)
        {
            _config.CustomImageUploaderSelected = 0;
            _config.CustomTextUploaderSelected = 0;
            _config.CustomFileUploaderSelected = 0;
            _config.CustomURLShortenerSelected = 0;
            _config.CustomURLSharingServiceSelected = 0;
            return;
        }

        _config.CustomImageUploaderSelected = _config.CustomImageUploaderSelected.Clamp(0, Uploaders.Count - 1);
        _config.CustomTextUploaderSelected = _config.CustomTextUploaderSelected.Clamp(0, Uploaders.Count - 1);
        _config.CustomFileUploaderSelected = _config.CustomFileUploaderSelected.Clamp(0, Uploaders.Count - 1);
        _config.CustomURLShortenerSelected = _config.CustomURLShortenerSelected.Clamp(0, Uploaders.Count - 1);
        _config.CustomURLSharingServiceSelected = _config.CustomURLSharingServiceSelected.Clamp(0, Uploaders.Count - 1);
    }

    private void FixSelectionsAfterRemoval(int removedIndex)
    {
        int resetIndex = Math.Max(0, Uploaders.Count - 1);
        _config.CustomImageUploaderSelected = FixSelection(_config.CustomImageUploaderSelected, removedIndex, resetIndex);
        _config.CustomTextUploaderSelected = FixSelection(_config.CustomTextUploaderSelected, removedIndex, resetIndex);
        _config.CustomFileUploaderSelected = FixSelection(_config.CustomFileUploaderSelected, removedIndex, resetIndex);
        _config.CustomURLShortenerSelected = FixSelection(_config.CustomURLShortenerSelected, removedIndex, resetIndex);
        _config.CustomURLSharingServiceSelected = FixSelection(_config.CustomURLSharingServiceSelected, removedIndex, resetIndex);
    }

    private static int FixSelection(int selection, int removedIndex, int resetIndex)
    {
        if (selection == removedIndex) selection = resetIndex;
        else if (selection > removedIndex) selection--;
        return selection;
    }

    private void NotifySelections()
    {
        OnPropertyChanged(nameof(CustomImageUploaderSelected));
        OnPropertyChanged(nameof(CustomTextUploaderSelected));
        OnPropertyChanged(nameof(CustomFileUploaderSelected));
        OnPropertyChanged(nameof(CustomURLShortenerSelected));
        OnPropertyChanged(nameof(CustomURLSharingServiceSelected));
    }
}
