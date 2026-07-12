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
using ShareX.HelpersLib;
using System.Text;
using System.Text.RegularExpressions;

namespace ShareX.Tools;

public sealed record MetadataEntry(string Group, string Tag, string Value, string? Url)
{
    public bool HasUrl => !string.IsNullOrWhiteSpace(Url);
}

public sealed record MetadataGroup(string Name, IReadOnlyList<MetadataEntry> Entries);

public sealed partial class MetadataViewModel : ViewModelBase
{
    private readonly List<MetadataEntry> _allEntries = [];
    private readonly Action? _playNotificationSound;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FileName))]
    [NotifyPropertyChangedFor(nameof(FileDescription))]
    [NotifyPropertyChangedFor(nameof(WindowTitle))]
    [NotifyPropertyChangedFor(nameof(CanStrip))]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private IReadOnlyList<MetadataGroup> _groups = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanOpen))]
    [NotifyPropertyChangedFor(nameof(CanCopy))]
    [NotifyPropertyChangedFor(nameof(CanStrip))]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isConfirmingStrip;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _emptyMessage = "Open or drop a file to inspect its metadata";

    public Func<Task<string?>>? SelectFileRequested { get; set; }
    public Func<string, Task>? CopyTextRequested { get; set; }

    public string FileName => File.Exists(FilePath) ? Path.GetFileName(FilePath) : "No file selected";
    public string FileDescription => File.Exists(FilePath) ? FilePath : "Open or drop any supported file";
    public string WindowTitle => File.Exists(FilePath) ? $"ShareX - Metadata - {FileName}" : "ShareX - Metadata";
    public bool HasGroups => Groups.Count > 0;
    public bool CanOpen => !IsBusy;
    public bool CanCopy => !IsBusy && _allEntries.Count > 0;
    public bool CanStrip => !IsBusy && File.Exists(FilePath);
    public string MetadataCountText => _allEntries.Count == 1 ? "1 tag" : $"{_allEntries.Count:N0} tags";

    public MetadataViewModel(string? filePath = null, Action? playNotificationSound = null)
    {
        _filePath = filePath ?? string.Empty;
        _playNotificationSound = playNotificationSound;
    }

    public async Task StartAsync()
    {
        if (File.Exists(FilePath))
        {
            await LoadMetadataAsync();
        }
        else
        {
            await BrowseAsync();
        }
    }

    [RelayCommand]
    private async Task BrowseAsync()
    {
        if (!CanOpen || SelectFileRequested == null)
        {
            return;
        }

        string? filePath = await SelectFileRequested();
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            await OpenFileAsync(filePath);
        }
    }

    public async Task OpenFileAsync(string filePath)
    {
        if (IsBusy || !File.Exists(filePath))
        {
            return;
        }

        FilePath = filePath;
        SearchText = string.Empty;
        await LoadMetadataAsync();
    }

    private async Task LoadMetadataAsync()
    {
        IsBusy = true;
        IsConfirmingStrip = false;
        Groups = [];
        EmptyMessage = "Reading metadata...";
        _allEntries.Clear();
        NotifyMetadataState();

        try
        {
            string output = await MetadataService.ReadMetadataAsync(FilePath);
            _allEntries.AddRange(ParseMetadata(output));
            EmptyMessage = _allEntries.Count > 0
                ? "No metadata matches the current search"
                : "No metadata was found in this file";
            ApplyFilter();
        }
        catch (Exception ex)
        {
            EmptyMessage = $"Unable to read metadata\n{ex.Message}";
            ToolsDiagnostics.ReportWarning(nameof(MetadataViewModel), "Failed to read file metadata.", ex);
        }
        finally
        {
            IsBusy = false;
            NotifyMetadataState();
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    [RelayCommand]
    private async Task CopyAllAsync()
    {
        if (CanCopy && CopyTextRequested != null)
        {
            await CopyTextRequested(BuildMetadataText(_allEntries));
        }
    }

    [RelayCommand]
    private void RequestStrip()
    {
        if (CanStrip)
        {
            IsConfirmingStrip = true;
        }
    }

    [RelayCommand]
    private void CancelStrip()
    {
        IsConfirmingStrip = false;
    }

    [RelayCommand]
    private async Task ConfirmStripAsync()
    {
        if (!CanStrip)
        {
            return;
        }

        IsConfirmingStrip = false;
        IsBusy = true;
        try
        {
            await MetadataService.StripMetadataAsync(FilePath);
            _playNotificationSound?.Invoke();
        }
        catch (Exception ex)
        {
            IsBusy = false;
            EmptyMessage = $"Unable to strip metadata\n{ex.Message}";
            ToolsDiagnostics.ReportWarning(nameof(MetadataViewModel), "Failed to strip file metadata.", ex);
            return;
        }

        IsBusy = false;
        await LoadMetadataAsync();
    }

    [RelayCommand]
    private void OpenUrl(string? url)
    {
        if (!string.IsNullOrWhiteSpace(url))
        {
            URLHelpers.OpenURL(url);
        }
    }

    private void ApplyFilter()
    {
        string filter = SearchText.Trim();
        IEnumerable<MetadataEntry> entries = _allEntries;
        if (!string.IsNullOrWhiteSpace(filter))
        {
            entries = entries.Where(x =>
                x.Group.Contains(filter, StringComparison.CurrentCultureIgnoreCase) ||
                x.Tag.Contains(filter, StringComparison.CurrentCultureIgnoreCase) ||
                x.Value.Contains(filter, StringComparison.CurrentCultureIgnoreCase));
        }

        Groups = entries
            .GroupBy(x => x.Group, StringComparer.OrdinalIgnoreCase)
            .Select(x => new MetadataGroup(x.Key, x.ToArray()))
            .ToArray();
        OnPropertyChanged(nameof(HasGroups));
    }

    private void NotifyMetadataState()
    {
        OnPropertyChanged(nameof(HasGroups));
        OnPropertyChanged(nameof(CanCopy));
        OnPropertyChanged(nameof(CanStrip));
        OnPropertyChanged(nameof(MetadataCountText));
    }

    private static IEnumerable<MetadataEntry> ParseMetadata(string metadata)
    {
        foreach (string line in metadata.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            string[] parts = line.Split('\t', 3);
            if (parts.Length < 2)
            {
                continue;
            }

            string group = parts[0].Trim();
            if (group.Equals("ExifTool", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string tag = parts[1].Trim();
            string value = parts.Length == 3 ? parts[2].Trim() : string.Empty;
            Match urlMatch = UrlRegex().Match(value);
            yield return new MetadataEntry(group, tag, value, urlMatch.Success ? urlMatch.Value : null);
        }
    }

    private static string BuildMetadataText(IEnumerable<MetadataEntry> entries)
    {
        StringBuilder output = new();
        foreach (IGrouping<string, MetadataEntry> group in entries.GroupBy(x => x.Group, StringComparer.OrdinalIgnoreCase))
        {
            if (output.Length > 0)
            {
                output.AppendLine();
            }
            output.AppendLine($"# {group.Key}");
            foreach (MetadataEntry entry in group)
            {
                output.AppendLine($"    {entry.Tag}: {entry.Value}");
            }
        }
        return output.ToString().TrimEnd();
    }

    [GeneratedRegex(@"https?://[^\s]+", RegexOptions.IgnoreCase)]
    private static partial Regex UrlRegex();
}
