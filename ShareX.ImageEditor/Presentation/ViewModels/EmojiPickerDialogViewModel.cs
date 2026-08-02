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
using ShareX.ImageEditor.Localization;
using ShareX.ImageEditor.Presentation.Emoji;
using System.Collections.ObjectModel;

namespace ShareX.ImageEditor.Presentation.ViewModels;

public partial class EmojiPickerDialogViewModel : ObservableObject
{
    private IReadOnlyList<EmojiCatalogEntry> _catalog = [];
    private readonly Action<EmojiCatalogEntry> _onSelect;
    private int _refreshVersion;
    private bool _isInitialized;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _selectedGroup = string.Empty;

    [ObservableProperty]
    private ObservableCollection<EmojiCatalogEntry> _visibleEmojis = [];

    [ObservableProperty]
    private string _resultsSummary = string.Empty;

    [ObservableProperty]
    private string _searchWatermark = Strings.EmojiPickerDialogView_LoadingEmojis;

    [ObservableProperty]
    private bool _isLoading = true;

    public ObservableCollection<string> GroupOptions { get; } = [];

    public bool HasResults => VisibleEmojis.Count > 0;

    public IRelayCommand<EmojiCatalogEntry?> SelectEmojiCommand { get; }

    public IRelayCommand CancelCommand { get; }

    public EmojiPickerDialogViewModel(Action<EmojiCatalogEntry> onSelect, Action onCancel)
    {
        _onSelect = onSelect;

        SelectEmojiCommand = new RelayCommand<EmojiCatalogEntry?>(SelectEmoji);
        CancelCommand = new RelayCommand(onCancel);
    }

    partial void OnSearchTextChanged(string value)
    {
        if (_isInitialized)
        {
            _ = RefreshResultsAsync();
        }
    }

    partial void OnSelectedGroupChanged(string value)
    {
        if (_isInitialized)
        {
            _ = RefreshResultsAsync();
        }
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        IsLoading = true;
        ResultsSummary = Strings.EmojiPickerDialogView_LoadingEmojis;
        SearchWatermark = Strings.EmojiPickerDialogView_LoadingEmojis;

        await Task.Yield();

        IReadOnlyList<EmojiCatalogEntry> catalog = await Task.Run(EmojiCatalogService.GetCatalog);
        IReadOnlyList<string> groups = await Task.Run(EmojiCatalogService.GetGroups);

        _catalog = catalog;

        GroupOptions.Clear();
        foreach (string group in groups)
        {
            GroupOptions.Add(group);
        }

        SelectedGroup = GroupOptions.FirstOrDefault() ?? string.Empty;
        _isInitialized = true;

        await RefreshResultsAsync();

        IsLoading = false;
    }

    private void SelectEmoji(EmojiCatalogEntry? emoji)
    {
        if (emoji == null)
        {
            return;
        }

        _onSelect(emoji);
    }

    private async Task RefreshResultsAsync()
    {
        int version = Interlocked.Increment(ref _refreshVersion);
        string search = SearchText.Trim();
        string selectedGroup = SelectedGroup;

        EmojiQueryResult result = await Task.Run(() => BuildQueryResult(search, selectedGroup));
        if (version != _refreshVersion)
        {
            return;
        }

        SearchWatermark = string.Format(Strings.EmojiPickerDialogView_SearchEmojisCount, result.CategoryCount);
        ResultsSummary = result.ResultsSummary;
        VisibleEmojis = [.. result.Entries];
        OnPropertyChanged(nameof(HasResults));
    }

    private EmojiQueryResult BuildQueryResult(string search, string selectedGroup)
    {
        EmojiCatalogEntry[] categoryEntries =
        [
            .. _catalog.Where(entry => string.Equals(entry.Group, selectedGroup, StringComparison.Ordinal))
        ];

        IEnumerable<EmojiCatalogEntry> query;
        string resultsSummary;

        if (string.IsNullOrWhiteSpace(search))
        {
            query = categoryEntries;
            resultsSummary = string.IsNullOrEmpty(selectedGroup)
                ? Strings.EmojiPickerDialogView_BrowseEmojis
                : string.Format(Strings.EmojiPickerDialogView_CategorySummary, selectedGroup, categoryEntries.Length);
        }
        else
        {
            EmojiCatalogEntry[] filteredEntries =
            [
                .. categoryEntries
                    .Select(entry => (Entry: entry, Score: entry.GetSearchScore(search)))
                    .Where(match => match.Score != int.MaxValue)
                    .OrderBy(match => match.Score)
                    .Select(match => match.Entry)
            ];

            query = filteredEntries;
            resultsSummary = string.Format(Strings.EmojiPickerDialogView_SearchResults, filteredEntries.Length);
        }

        return new EmojiQueryResult([.. query], resultsSummary, categoryEntries.Length);
    }

    private sealed record EmojiQueryResult(
        EmojiCatalogEntry[] Entries,
        string ResultsSummary,
        int CategoryCount);
}
