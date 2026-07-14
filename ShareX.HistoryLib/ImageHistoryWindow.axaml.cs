#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.HistoryLib;

public partial class ImageHistoryWindow : Window
{
    private readonly HistoryManagerSQLite _historyManager;
    private readonly ImageHistorySettings _settings;
    private readonly HistoryWindowServices _services;
    private readonly ImageHistoryThumbnailLoader _thumbnailLoader = new();
    private readonly DispatcherTimer _filterTimer;
    private readonly ObservableCollection<ImageHistoryRow> _rows = [];
    private readonly List<ImageHistoryEntry> _loadedEntries = [];
    private readonly Dictionary<Control, ImageHistoryEntry> _realizedControls = [];
    private List<HistoryItem> _allHistoryItems = [];
    private HistoryItem[] _filteredHistoryItems = [];
    private CancellationTokenSource? _filterCancellation;
    private ScrollViewer? _scrollViewer;
    private int _filterVersion;
    private int _nextItemIndex;
    private int _columns = 1;
    private int _selectionAnchor = -1;
    private bool _isBusy;
    private bool _checkingLoadMore;
    private HistoryItem? _editingItem;
    private Func<Task>? _promptAction;
    private PointerPressedEventArgs? _dragPointerPressed;
    private Point _dragStart;
    private bool _dragStarted;
    private bool _windowPlacementApplied;

    private ShareX.HelpersLib.WindowState SavedWindowState =>
        _settings.WindowState ??= new ShareX.HelpersLib.WindowState();

    public ImageHistoryWindow()
    {
        _historyManager = null!;
        _settings = new ImageHistorySettings();
        _services = new HistoryWindowServices();
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        ThumbnailRows.ItemsSource = _rows;
        _filterTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(220) };
        _filterTimer.Tick += OnFilterTimerTick;
    }

    public ImageHistoryWindow(HistoryManagerSQLite historyManager, ImageHistorySettings settings,
        HistoryWindowServices services) : this()
    {
        _historyManager = historyManager;
        _settings = settings;
        _services = services;
        SearchTextBox.Text = _settings.RememberSearchText ? _settings.SearchText : string.Empty;
        FavoritesButton.Classes.Set("active", _settings.Favorites);
        ApplySavedWindowState();

        Opened += OnOpened;
        Closing += OnClosing;
        Closed += OnClosed;
        Resized += OnResized;
        PositionChanged += OnPositionChanged;
        KeyDown += OnWindowKeyDown;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        ApplySavedWindowState();
        _windowPlacementApplied = true;
        Activate();
        SearchTextBox.Focus();
        await Dispatcher.UIThread.InvokeAsync(AttachScrollViewer, DispatcherPriority.Loaded);
        await RefreshHistoryAsync();
    }

    private void OnClosing(object? sender, WindowClosingEventArgs e) => SaveWindowState();

    private void AttachScrollViewer()
    {
        _scrollViewer = ThumbnailRows.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();
        if (_scrollViewer != null)
        {
            _scrollViewer.ScrollChanged += OnScrollChanged;
        }
        UpdateColumnCount();
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _filterTimer.Stop();
        _filterCancellation?.Cancel();
        _filterCancellation?.Dispose();
        if (_scrollViewer != null) _scrollViewer.ScrollChanged -= OnScrollChanged;
        foreach (ImageHistoryEntry entry in _loadedEntries) entry.Dispose();
        _thumbnailLoader.Dispose();
    }

    private void ApplySavedWindowState()
    {
        if (!_settings.RememberWindowState) return;

        ShareX.HelpersLib.WindowState savedState = SavedWindowState;

        if (!savedState.Size.IsEmpty)
        {
            Width = savedState.Size.Width;
            Height = savedState.Size.Height;
        }

        if (!savedState.Location.IsEmpty)
        {
            PixelPoint position = new(savedState.Location.X, savedState.Location.Y);
            if (Screens.ScreenFromPoint(position) != null)
            {
                WindowStartupLocation = WindowStartupLocation.Manual;
                Position = position;
            }
        }

        WindowState = savedState.IsMaximized
            ? Avalonia.Controls.WindowState.Maximized
            : Avalonia.Controls.WindowState.Normal;
    }

    private void OnResized(object? sender, WindowResizedEventArgs e)
    {
        if (_windowPlacementApplied) SaveNormalWindowSize();
    }

    private void OnPositionChanged(object? sender, PixelPointEventArgs e)
    {
        if (_windowPlacementApplied) SaveNormalWindowSize();
    }

    private void SaveWindowState()
    {
        if (!_settings.RememberWindowState) return;
        SaveNormalWindowSize();
        SavedWindowState.IsMaximized = WindowState == Avalonia.Controls.WindowState.Maximized;
    }

    private void SaveNormalWindowSize()
    {
        if (!_settings.RememberWindowState || WindowState != Avalonia.Controls.WindowState.Normal) return;
        if (Bounds.Width <= 0 || Bounds.Height <= 0) return;
        SavedWindowState.Size = new System.Drawing.Size(
            (int)Math.Round(Bounds.Width),
            (int)Math.Round(Bounds.Height));
        SavedWindowState.Location = new System.Drawing.Point(Position.X, Position.Y);
    }

    private async Task RefreshHistoryAsync()
    {
        SetBusy(true);
        try
        {
            List<HistoryItem> items = await _historyManager.GetHistoryItemsAsync();
            items.Reverse();
            _allHistoryItems = items;
            await ApplyFilterAsync();
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void SetBusy(bool value) => _isBusy = value;

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_settings.RememberSearchText) _settings.SearchText = SearchTextBox.Text ?? string.Empty;
        _filterTimer.Stop();
        _filterTimer.Start();
    }

    private async void OnFilterTimerTick(object? sender, EventArgs e)
    {
        _filterTimer.Stop();
        await ApplyFilterAsync();
    }

    private async Task ApplyFilterAsync()
    {
        int version = Interlocked.Increment(ref _filterVersion);
        string search = SearchTextBox.Text?.Trim() ?? string.Empty;
        bool favorites = _settings.Favorites;
        bool imageOnly = _settings.ImageOnly;
        bool filterMissing = _settings.FilterMissingFiles;
        List<HistoryItem> source = _allHistoryItems;

        _filterCancellation?.Cancel();
        _filterCancellation?.Dispose();
        _filterCancellation = new CancellationTokenSource();
        CancellationToken token = _filterCancellation.Token;
        SetBusy(true);

        try
        {
            HistoryItem[] filtered = await Task.Run(() =>
            {
                Regex? regex = null;
                if (!string.IsNullOrEmpty(search))
                {
                    string pattern = Regex.Escape(search).Replace("\\?", ".").Replace("\\*", ".*");
                    regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                }

                List<HistoryItem> result = [];
                for (int i = 0; i < source.Count; i++)
                {
                    if ((i & 1023) == 0) token.ThrowIfCancellationRequested();
                    HistoryItem item = source[i];
                    if (string.IsNullOrWhiteSpace(item.FilePath)) continue;
                    if (favorites && !item.Favorite) continue;
                    if (imageOnly && !FileHelpers.IsImageFile(item.FilePath)) continue;
                    if (filterMissing && !File.Exists(item.FilePath)) continue;
                    if (regex != null && !regex.IsMatch(item.FileName ?? string.Empty) &&
                        !(item.Tags?.Values.Any(value => !string.IsNullOrEmpty(value) && regex.IsMatch(value)) == true)) continue;
                    result.Add(item);
                }
                return result.ToArray();
            }, token);

            if (version != _filterVersion || !IsVisible) return;
            _filteredHistoryItems = filtered;
            ResetLoadedEntries();
            LoadNextBatch();
            UpdateCountAndEmptyState();
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            if (version == _filterVersion) SetBusy(false);
        }
    }

    private void ResetLoadedEntries()
    {
        foreach (ImageHistoryEntry entry in _loadedEntries) entry.Dispose();
        _realizedControls.Clear();
        _loadedEntries.Clear();
        _rows.Clear();
        _nextItemIndex = 0;
        _selectionAnchor = -1;
        if (_scrollViewer != null) _scrollViewer.Offset = new Vector(0, 0);
    }

    private void LoadNextBatch()
    {
        if (_nextItemIndex >= _filteredHistoryItems.Length) return;
        int batchSize = _settings.MaxItemCount <= 0
            ? _filteredHistoryItems.Length - _nextItemIndex
            : _settings.MaxItemCount;
        int end = Math.Min(_nextItemIndex + batchSize, _filteredHistoryItems.Length);

        for (int i = _nextItemIndex; i < end; i++)
        {
            ImageHistoryEntry entry = new(_filteredHistoryItems[i], _settings.ThumbnailSize.Width,
                _settings.ThumbnailSize.Height);
            _loadedEntries.Add(entry);
            ImageHistoryRow? row = _rows.LastOrDefault();
            if (row == null || row.Items.Count >= _columns)
            {
                row = new ImageHistoryRow();
                _rows.Add(row);
            }
            row.Items.Add(entry);
        }

        _nextItemIndex = end;
        UpdateCountAndEmptyState();
        Dispatcher.UIThread.Post(CheckLoadMoreIfViewportNotFilled, DispatcherPriority.Background);
    }

    private void UpdateCountAndEmptyState()
    {
        int loaded = _loadedEntries.Count;
        int filtered = _filteredHistoryItems.Length;
        ItemCountText.Text = loaded == filtered
            ? $"{filtered:N0} items"
            : $"{loaded:N0} shown · {filtered:N0} matched";
        EmptyState.IsVisible = filtered == 0;
        EmptyStateText.Text = _allHistoryItems.Count == 0 ? "No image history items" : "No items match the current filter";
    }

    private void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (!_settings.AutoLoadMoreItems || _scrollViewer == null) return;
        double remaining = _scrollViewer.Extent.Height - _scrollViewer.Viewport.Height - _scrollViewer.Offset.Y;
        if (remaining <= Math.Max(120, _settings.ThumbnailSize.Height)) LoadNextBatch();
    }

    private void CheckLoadMoreIfViewportNotFilled()
    {
        if (_checkingLoadMore || !_settings.AutoLoadMoreItems || _scrollViewer == null ||
            _nextItemIndex >= _filteredHistoryItems.Length) return;
        if (_scrollViewer.Extent.Height > _scrollViewer.Viewport.Height + 1) return;

        _checkingLoadMore = true;
        LoadNextBatch();
        _checkingLoadMore = false;
    }

    private void OnThumbnailRowsSizeChanged(object? sender, SizeChangedEventArgs e) => UpdateColumnCount();

    private void UpdateColumnCount()
    {
        const double thumbnailSpacing = 4;
        double cardWidth = Math.Max(36, _settings.ThumbnailSize.Width + 4);
        int columns = Math.Max(1, (int)Math.Floor(
            Math.Max(1, ThumbnailRows.Bounds.Width + thumbnailSpacing) / (cardWidth + thumbnailSpacing)));
        if (columns == _columns) return;
        _columns = columns;
        RebuildRows();
    }

    private void RebuildRows()
    {
        _rows.Clear();
        foreach (ImageHistoryEntry entry in _loadedEntries)
        {
            ImageHistoryRow? row = _rows.LastOrDefault();
            if (row == null || row.Items.Count >= _columns)
            {
                row = new ImageHistoryRow();
                _rows.Add(row);
            }
            row.Items.Add(entry);
        }
    }

    private async void OnThumbnailAttached(object? sender, VisualTreeAttachmentEventArgs e) =>
        await AttachThumbnailControlAsync(sender as Control);

    private void OnThumbnailDetached(object? sender, VisualTreeAttachmentEventArgs e) => DetachThumbnailControl(sender as Control);

    private async void OnThumbnailDataContextChanged(object? sender, EventArgs e)
    {
        if (sender is Control control && control.IsAttachedToVisualTree())
        {
            DetachThumbnailControl(control);
            await AttachThumbnailControlAsync(control);
        }
    }

    private async Task AttachThumbnailControlAsync(Control? control)
    {
        if (control?.DataContext is not ImageHistoryEntry entry) return;
        if (_realizedControls.TryGetValue(control, out ImageHistoryEntry? previous))
        {
            if (ReferenceEquals(previous, entry)) return;
            previous.Unrealize();
        }
        _realizedControls[control] = entry;
        await entry.RealizeAsync(_thumbnailLoader);
    }

    private void DetachThumbnailControl(Control? control)
    {
        if (control != null && _realizedControls.Remove(control, out ImageHistoryEntry? entry)) entry.Unrealize();
    }

    private void OnThumbnailPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control { DataContext: ImageHistoryEntry entry } control) return;
        PointerPoint point = e.GetCurrentPoint(control);

        if (point.Properties.IsRightButtonPressed)
        {
            if (!entry.IsSelected) SelectOnly(entry);
            return;
        }

        if (!point.Properties.IsLeftButtonPressed) return;
        int index = _loadedEntries.IndexOf(entry);
        if (e.KeyModifiers.HasFlag(KeyModifiers.Shift) && _selectionAnchor >= 0)
        {
            SelectRange(_selectionAnchor, index);
        }
        else if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            entry.IsSelected = !entry.IsSelected;
            _selectionAnchor = index;
        }
        else
        {
            SelectOnly(entry);
        }

        _dragPointerPressed = e;
        _dragStart = e.GetPosition(ThumbnailRows);
        _dragStarted = false;
    }

    private void SelectOnly(ImageHistoryEntry entry)
    {
        foreach (ImageHistoryEntry candidate in _loadedEntries) candidate.IsSelected = ReferenceEquals(candidate, entry);
        _selectionAnchor = _loadedEntries.IndexOf(entry);
    }

    private void SelectRange(int first, int second)
    {
        int start = Math.Min(first, second);
        int end = Math.Max(first, second);
        for (int i = 0; i < _loadedEntries.Count; i++) _loadedEntries[i].IsSelected = i >= start && i <= end;
    }

    private void SelectIndex(int index, bool extendSelection)
    {
        if (_loadedEntries.Count == 0) return;
        index = Math.Clamp(index, 0, _loadedEntries.Count - 1);
        if (extendSelection && _selectionAnchor >= 0) SelectRange(_selectionAnchor, index);
        else SelectOnly(_loadedEntries[index]);
        ThumbnailRows.ScrollIntoView(index / Math.Max(1, _columns));
    }

    private async void OnThumbnailPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_dragPointerPressed == null || _dragStarted ||
            !e.GetCurrentPoint(ThumbnailRows).Properties.IsLeftButtonPressed) return;
        Point current = e.GetPosition(ThumbnailRows);
        if (Math.Abs(current.X - _dragStart.X) < 5 && Math.Abs(current.Y - _dragStart.Y) < 5) return;

        string[] files = GetSelectedItems().Select(item => item.FilePath).Where(File.Exists)
            .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        if (files.Length == 0)
        {
            _dragPointerPressed = null;
            return;
        }

        _dragStarted = true;
        DataTransfer transfer = new();
        foreach (string file in files)
        {
            IStorageFile? storageFile = await StorageProvider.TryGetFileFromPathAsync(file);
            if (storageFile != null) transfer.Add(DataTransferItem.CreateFile(storageFile));
        }
        if (transfer.Items.Count > 0 && _dragPointerPressed != null)
        {
            await DragDrop.DoDragDropAsync(_dragPointerPressed, transfer, DragDropEffects.Copy);
        }
        _dragPointerPressed = null;
    }

    private void OnThumbnailPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_dragStarted) _dragPointerPressed = null;
    }

    private void OnThumbnailDoubleTapped(object? sender, TappedEventArgs e) => OpenSelectedItem();

    private HistoryItem[] GetSelectedItems() => _loadedEntries.Where(entry => entry.IsSelected).Select(entry => entry.Item).ToArray();
    private HistoryItem? GetPrimaryItem() => GetSelectedItems().FirstOrDefault();

    private void OnFavoritesClick(object? sender, RoutedEventArgs e)
    {
        _settings.Favorites = !_settings.Favorites;
        FavoritesButton.Classes.Set("active", _settings.Favorites);
        _ = ApplyFilterAsync();
    }

    private async void OnStatsClick(object? sender, RoutedEventArgs e)
    {
        ShowModal(StatsDialog);
        StatsTextBox.Text = "Calculating statistics...";
        StatsTextBox.Text = await Task.Run(() => HistoryHelpers.OutputStats(_allHistoryItems));
    }

    private void OnImportClick(object? sender, RoutedEventArgs e)
    {
        ImportFolderTextBox.Text = string.Empty;
        ImportStatusText.Text = string.Empty;
        ShowModal(ImportDialog);
    }

    private async void OnBrowseImportFolderClick(object? sender, RoutedEventArgs e)
    {
        IReadOnlyList<IStorageFolder> folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select folder to import",
            AllowMultiple = false
        });
        ImportFolderTextBox.Text = folders.FirstOrDefault()?.Path.LocalPath ?? ImportFolderTextBox.Text;
    }

    private void OnImportFolderTextChanged(object? sender, TextChangedEventArgs e) =>
        ImportConfirmButton.IsEnabled = !_isBusy && !string.IsNullOrWhiteSpace(ImportFolderTextBox.Text);

    private async void OnImportConfirmClick(object? sender, RoutedEventArgs e)
    {
        string folder = ImportFolderTextBox.Text?.Trim() ?? string.Empty;
        if (!Directory.Exists(folder))
        {
            ImportStatusText.Text = "The selected folder does not exist.";
            return;
        }

        SetBusy(true);
        ImportConfirmButton.IsEnabled = false;
        ImportStatusText.Text = "Importing files...";
        try
        {
            bool imagesOnly = ImportImagesOnlyCheckBox.IsChecked == true;
            HashSet<string> existing = ImportSkipDuplicatesCheckBox.IsChecked == true
                ? _allHistoryItems.Where(item => !string.IsNullOrWhiteSpace(item.FilePath))
                    .Select(item => item.FilePath).ToHashSet(StringComparer.OrdinalIgnoreCase)!
                : [];
            int imported = await Task.Run(() => ImportFolder(folder, imagesOnly, existing));
            ImportStatusText.Text = $"Successfully imported {imported:N0} files.";
            if (imported > 0) await RefreshHistoryAsync();
        }
        catch (Exception ex)
        {
            DebugHelper.WriteException(ex);
            ImportStatusText.Text = ex.Message;
        }
        finally
        {
            SetBusy(false);
            ImportConfirmButton.IsEnabled = !string.IsNullOrWhiteSpace(ImportFolderTextBox.Text);
        }
    }

    private int ImportFolder(string folderPath, bool onlyImages, HashSet<string> existing)
    {
        IEnumerable<string> files = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories);
        if (onlyImages) files = files.Where(FileHelpers.IsImageFile);
        List<HistoryItem> imported = [];
        foreach (string file in files.OrderBy(File.GetLastWriteTime))
        {
            if (existing.Contains(file)) continue;
            imported.Add(new HistoryItem
            {
                FileName = Path.GetFileName(file),
                FilePath = file,
                DateTime = File.GetLastWriteTime(file),
                Type = FileHelpers.IsImageFile(file) ? "Image" : FileHelpers.IsTextFile(file) ? "Text" : "File"
            });
        }
        if (imported.Count > 0) _historyManager.AppendHistoryItems(imported);
        return imported.Count;
    }

    private void OnSettingsClick(object? sender, RoutedEventArgs e)
    {
        ThumbnailWidthInput.Value = _settings.ThumbnailSize.Width;
        ThumbnailHeightInput.Value = _settings.ThumbnailSize.Height;
        BatchSizeInput.Value = _settings.MaxItemCount;
        AutoLoadMoreToggle.IsChecked = _settings.AutoLoadMoreItems;
        FilterMissingToggle.IsChecked = _settings.FilterMissingFiles;
        ImageOnlyToggle.IsChecked = _settings.ImageOnly;
        RememberSearchToggle.IsChecked = _settings.RememberSearchText;
        RememberWindowToggle.IsChecked = _settings.RememberWindowState;
        ShowModal(SettingsDialog);
    }

    private void OnSaveSettingsClick(object? sender, RoutedEventArgs e)
    {
        _settings.ThumbnailSize = new System.Drawing.Size((int)(ThumbnailWidthInput.Value ?? 250),
            (int)(ThumbnailHeightInput.Value ?? 150));
        _settings.MaxItemCount = (int)(BatchSizeInput.Value ?? 500);
        _settings.AutoLoadMoreItems = AutoLoadMoreToggle.IsChecked == true;
        _settings.FilterMissingFiles = FilterMissingToggle.IsChecked == true;
        _settings.ImageOnly = ImageOnlyToggle.IsChecked == true;
        _settings.RememberSearchText = RememberSearchToggle.IsChecked == true;
        _settings.RememberWindowState = RememberWindowToggle.IsChecked == true;
        _settings.SearchText = _settings.RememberSearchText ? SearchTextBox.Text ?? string.Empty : string.Empty;
        CloseModal();
        UpdateColumnCount();
        _ = ApplyFilterAsync();
    }

    private void ShowModal(Control dialog)
    {
        StatsDialog.IsVisible = false;
        ImportDialog.IsVisible = false;
        SettingsDialog.IsVisible = false;
        EditDialog.IsVisible = false;
        PromptDialog.IsVisible = false;
        dialog.IsVisible = true;
        ModalOverlay.IsVisible = true;
    }

    private void CloseModal()
    {
        ModalOverlay.IsVisible = false;
        StatsDialog.IsVisible = false;
        ImportDialog.IsVisible = false;
        SettingsDialog.IsVisible = false;
        EditDialog.IsVisible = false;
        PromptDialog.IsVisible = false;
        _promptAction = null;
        _editingItem = null;
        ThumbnailRows.Focus();
    }

    private void OnCloseModalClick(object? sender, RoutedEventArgs e) => CloseModal();

    private void ShowPrompt(string title, string message, string confirmText, Func<Task> action, string? input = null)
    {
        PromptTitle.Text = title;
        PromptMessage.Text = message;
        PromptConfirmButton.Content = confirmText;
        PromptTextBox.IsVisible = input != null;
        PromptTextBox.Text = input ?? string.Empty;
        _promptAction = action;
        ShowModal(PromptDialog);
        if (input != null)
        {
            PromptTextBox.Focus();
            PromptTextBox.SelectAll();
        }
    }

    private async void OnPromptConfirmClick(object? sender, RoutedEventArgs e)
    {
        Func<Task>? action = _promptAction;
        if (action != null) await action();
        CloseModal();
    }

    private void OnEditTagClick(object? sender, RoutedEventArgs e)
    {
        HistoryItem? item = GetPrimaryItem();
        if (item == null) return;
        ShowPrompt("Edit tag", "Enter a tag for this history item.", "Save", () =>
        {
            item.Tag = PromptTextBox.Text;
            _historyManager.Edit(item);
            RefreshVisibleItem(item);
            return Task.CompletedTask;
        }, item.Tag ?? string.Empty);
    }

    private void OnEditItemClick(object? sender, RoutedEventArgs e)
    {
        HistoryItem? item = GetPrimaryItem();
        if (item == null) return;
        _editingItem = item;
        EditFileNameTextBox.Text = item.FileName;
        EditFilePathTextBox.Text = item.FilePath;
        EditDateTextBox.Text = item.DateTime.ToString("G");
        EditTypeTextBox.Text = item.Type;
        EditHostTextBox.Text = item.Host;
        EditUrlTextBox.Text = item.URL;
        EditThumbnailUrlTextBox.Text = item.ThumbnailURL;
        EditDeletionUrlTextBox.Text = item.DeletionURL;
        EditShortUrlTextBox.Text = item.ShortenedURL;
        EditTagsTextBox.Text = item.Tags == null ? string.Empty : string.Join(Environment.NewLine,
            item.Tags.OrderBy(pair => pair.Key).Select(pair => $"{pair.Key}={pair.Value}"));
        ShowModal(EditDialog);
    }

    private void OnSaveEditClick(object? sender, RoutedEventArgs e)
    {
        HistoryItem? item = _editingItem;
        if (item == null) return;
        item.FileName = EditFileNameTextBox.Text;
        item.FilePath = EditFilePathTextBox.Text;
        item.Type = EditTypeTextBox.Text;
        item.Host = EditHostTextBox.Text;
        item.URL = EditUrlTextBox.Text;
        item.ThumbnailURL = EditThumbnailUrlTextBox.Text;
        item.DeletionURL = EditDeletionUrlTextBox.Text;
        item.ShortenedURL = EditShortUrlTextBox.Text;
        item.Tags = ParseTags(EditTagsTextBox.Text);
        _historyManager.Edit(item);
        RefreshVisibleItem(item);
        CloseModal();
    }

    private static Dictionary<string, string> ParseTags(string? text)
    {
        Dictionary<string, string> tags = [];
        foreach (string line in (text ?? string.Empty).Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries))
        {
            int separator = line.IndexOf('=');
            string key = (separator >= 0 ? line[..separator] : line).Trim();
            if (!string.IsNullOrWhiteSpace(key)) tags[key] = separator >= 0 ? line[(separator + 1)..] : null!;
        }
        return tags;
    }

    private void RefreshVisibleItem(HistoryItem item)
    {
        _loadedEntries.FirstOrDefault(entry => ReferenceEquals(entry.Item, item))?.RefreshMetadata();
    }

    private void OnRenameFileClick(object? sender, RoutedEventArgs e)
    {
        HistoryItem? item = GetPrimaryItem();
        if (item == null || string.IsNullOrWhiteSpace(item.FilePath)) return;
        string oldName = Path.GetFileNameWithoutExtension(item.FilePath);
        ShowPrompt("Rename file", "Enter a new file name.", "Rename", () =>
        {
            string newName = PromptTextBox.Text?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(newName) && !newName.Equals(oldName, StringComparison.OrdinalIgnoreCase))
            {
                if (Path.HasExtension(item.FilePath)) newName += Path.GetExtension(item.FilePath);
                item.FileName = newName;
                item.FilePath = FileHelpers.RenameFile(item.FilePath, newName);
                _historyManager.Edit(item);
                RefreshVisibleItem(item);
            }
            return Task.CompletedTask;
        }, oldName);
    }

    private void OnToggleFavoriteClick(object? sender, RoutedEventArgs e)
    {
        HistoryItem[] items = GetSelectedItems();
        foreach (HistoryItem item in items)
        {
            item.Favorite = !item.Favorite;
            _historyManager.Edit(item);
            RefreshVisibleItem(item);
        }
        if (_settings.Favorites) _ = ApplyFilterAsync();
    }

    private void OnDeleteClick(object? sender, RoutedEventArgs e) => ConfirmDelete(false);
    private void OnDeleteFileClick(object? sender, RoutedEventArgs e) => ConfirmDelete(true);

    private void ConfirmDelete(bool deleteFiles)
    {
        HistoryItem[] items = GetSelectedItems();
        if (items.Length == 0) return;
        string noun = items.Length == 1 ? (deleteFiles ? "this file" : "this item") :
            (deleteFiles ? $"these {items.Length:N0} files" : $"these {items.Length:N0} items");
        ShowPrompt(deleteFiles ? "Delete files?" : "Delete history items?",
            $"Do you really want to delete {noun}? This action cannot be undone.", "Delete", async () =>
            {
                if (deleteFiles)
                {
                    foreach (HistoryItem item in items)
                    {
                        if (File.Exists(item.FilePath)) File.Delete(item.FilePath);
                    }
                }
                _historyManager.Delete(items);
                HashSet<long> ids = items.Select(item => item.Id).ToHashSet();
                _allHistoryItems.RemoveAll(item => ids.Contains(item.Id));
                await ApplyFilterAsync();
            });
    }

    private void OnShowImageClick(object? sender, RoutedEventArgs e) => ShowSelectedImage();

    private void OpenSelectedItem()
    {
        HistoryItem? item = GetPrimaryItem();
        if (item == null || !File.Exists(item.FilePath)) return;
        if (FileHelpers.IsImageFile(item.FilePath))
        {
            ShowSelectedImage();
        }
        else if (FileHelpers.IsTextFile(item.FilePath) || FileHelpers.IsVideoFile(item.FilePath))
        {
            FileHelpers.OpenFile(item.FilePath);
        }
        else
        {
            ShowPrompt("Open file?", $"Would you like to open this file?{Environment.NewLine}{Environment.NewLine}{item.FilePath}",
                "Open", () =>
                {
                    FileHelpers.OpenFile(item.FilePath);
                    return Task.CompletedTask;
                });
        }
    }

    private void ShowSelectedImage()
    {
        HistoryItem? item = GetPrimaryItem();
        if (item == null || !File.Exists(item.FilePath) || !FileHelpers.IsImageFile(item.FilePath)) return;
        int currentIndex = Array.IndexOf(_filteredHistoryItems, item);
        int start = Math.Max(0, currentIndex - 100);
        int end = Math.Min(_filteredHistoryItems.Length, start + 201);
        List<string> files = [];
        int selectedIndex = 0;
        for (int i = start; i < end; i++)
        {
            string path = _filteredHistoryItems[i].FilePath;
            if (!File.Exists(path) || !FileHelpers.IsImageFile(path)) continue;
            if (ReferenceEquals(_filteredHistoryItems[i], item)) selectedIndex = files.Count;
            files.Add(path);
        }
        if (_services.ShowImages != null) _services.ShowImages(files, selectedIndex);
        else _services.ShowImage?.Invoke(item.FilePath);
    }

    private void OnOpenFileClick(object? sender, RoutedEventArgs e)
    {
        HistoryItem? item = GetPrimaryItem();
        if (item != null && File.Exists(item.FilePath)) FileHelpers.OpenFile(item.FilePath);
    }

    private void OnOpenFolderClick(object? sender, RoutedEventArgs e)
    {
        HistoryItem? item = GetPrimaryItem();
        if (item != null && File.Exists(item.FilePath)) FileHelpers.OpenFolderWithFile(item.FilePath);
    }

    private void OnOpenUrlClick(object? sender, RoutedEventArgs e) => OpenUrl(item => item.URL);
    private void OnOpenShortUrlClick(object? sender, RoutedEventArgs e) => OpenUrl(item => item.ShortenedURL);
    private void OnOpenThumbnailUrlClick(object? sender, RoutedEventArgs e) => OpenUrl(item => item.ThumbnailURL);
    private void OnOpenDeletionUrlClick(object? sender, RoutedEventArgs e) => OpenUrl(item => item.DeletionURL);

    private void OpenUrl(Func<HistoryItem, string?> selector)
    {
        HistoryItem? item = GetPrimaryItem();
        string? url = item == null ? null : selector(item);
        if (!string.IsNullOrWhiteSpace(url)) URLHelpers.OpenURL(url);
    }

    private void CopyValues(Func<HistoryItem, string?> selector, Func<HistoryItem, bool>? predicate = null)
    {
        string[] values = GetSelectedItems().Where(item => predicate?.Invoke(item) != false)
            .Select(selector).Where(value => !string.IsNullOrWhiteSpace(value)).ToArray()!;
        if (values.Length > 0) ClipboardHelpers.CopyText(string.Join(Environment.NewLine, values));
    }

    private void OnCopyUrlClick(object? sender, RoutedEventArgs e) => CopyValues(item => item.URL);
    private void OnCopyShortUrlClick(object? sender, RoutedEventArgs e) => CopyValues(item => item.ShortenedURL);
    private void OnCopyThumbnailUrlClick(object? sender, RoutedEventArgs e) => CopyValues(item => item.ThumbnailURL);
    private void OnCopyDeletionUrlClick(object? sender, RoutedEventArgs e) => CopyValues(item => item.DeletionURL);
    private void OnCopyFilePathClick(object? sender, RoutedEventArgs e) => CopyValues(item => item.FilePath);
    private void OnCopyFileNameClick(object? sender, RoutedEventArgs e) => CopyValues(item => Path.GetFileNameWithoutExtension(item.FilePath));
    private void OnCopyFileNameWithExtensionClick(object? sender, RoutedEventArgs e) => CopyValues(item => Path.GetFileName(item.FilePath));
    private void OnCopyFolderClick(object? sender, RoutedEventArgs e) => CopyValues(item => Path.GetDirectoryName(item.FilePath));
    private void OnCopyHtmlLinkClick(object? sender, RoutedEventArgs e) => CopyValues(item => $"<a href=\"{item.URL}\">{item.URL}</a>", HasUrl);
    private void OnCopyHtmlImageClick(object? sender, RoutedEventArgs e) => CopyValues(item => $"<img src=\"{item.URL}\"/>", HasUrl);
    private void OnCopyHtmlLinkedImageClick(object? sender, RoutedEventArgs e) =>
        CopyValues(item => $"<a href=\"{item.URL}\"><img src=\"{item.ThumbnailURL}\"/></a>", HasLinkedImage);
    private void OnCopyForumLinkClick(object? sender, RoutedEventArgs e) => CopyValues(item => $"[url]{item.URL}[/url]", HasUrl);
    private void OnCopyForumImageClick(object? sender, RoutedEventArgs e) => CopyValues(item => $"[img]{item.URL}[/img]", HasUrl);
    private void OnCopyForumLinkedImageClick(object? sender, RoutedEventArgs e) =>
        CopyValues(item => $"[url={item.URL}][img]{item.ThumbnailURL}[/img][/url]", HasLinkedImage);
    private void OnCopyMarkdownLinkClick(object? sender, RoutedEventArgs e) => CopyValues(item => $"[{item.FileName}]({item.URL})", HasUrl);
    private void OnCopyMarkdownImageClick(object? sender, RoutedEventArgs e) => CopyValues(item => $"![{item.FileName}]({item.URL})", HasUrl);
    private void OnCopyMarkdownLinkedImageClick(object? sender, RoutedEventArgs e) =>
        CopyValues(item => $"[![{item.FileName}]({item.ThumbnailURL})]({item.URL})", HasLinkedImage);
    private static bool HasUrl(HistoryItem item) => !string.IsNullOrWhiteSpace(item.URL);
    private static bool HasLinkedImage(HistoryItem item) => HasUrl(item) && !string.IsNullOrWhiteSpace(item.ThumbnailURL);

    private void OnCopyFileClick(object? sender, RoutedEventArgs e)
    {
        string[] files = GetSelectedItems().Select(item => item.FilePath).Where(File.Exists).ToArray();
        if (files.Length > 0) ClipboardHelpers.CopyFile(files);
    }

    private void OnCopyImageClick(object? sender, RoutedEventArgs e)
    {
        HistoryItem? item = GetPrimaryItem();
        if (item != null && File.Exists(item.FilePath) && FileHelpers.IsImageFile(item.FilePath))
            ClipboardHelpers.CopyImageFromFile(item.FilePath);
    }

    private void OnCopyTextClick(object? sender, RoutedEventArgs e)
    {
        HistoryItem? item = GetPrimaryItem();
        if (item != null && File.Exists(item.FilePath) && FileHelpers.IsTextFile(item.FilePath))
            ClipboardHelpers.CopyTextFromFile(item.FilePath);
    }

    private void OnUploadFileClick(object? sender, RoutedEventArgs e) => InvokeFileService(_services.UploadFile, false);
    private void OnEditImageClick(object? sender, RoutedEventArgs e) => InvokeFileService(_services.EditImage, true);
    private void OnPinToScreenClick(object? sender, RoutedEventArgs e) => InvokeFileService(_services.PinToScreen, true);
    private void OnAnalyzeImageClick(object? sender, RoutedEventArgs e) => InvokeFileService(_services.AnalyzeImage, true);

    private void InvokeFileService(Action<string>? action, bool imageOnly)
    {
        HistoryItem? item = GetPrimaryItem();
        if (action != null && item != null && File.Exists(item.FilePath) &&
            (!imageOnly || FileHelpers.IsImageFile(item.FilePath))) action(item.FilePath);
    }

    private async void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            e.Handled = true;
            if (ModalOverlay.IsVisible) CloseModal(); else Close();
        }
        else if (e.Key == Key.F5)
        {
            e.Handled = true;
            await RefreshHistoryAsync();
        }
        else if (ModalOverlay.IsVisible)
        {
            return;
        }
        else if (e.Key == Key.A && e.KeyModifiers == KeyModifiers.Control)
        {
            foreach (ImageHistoryEntry entry in _loadedEntries) entry.IsSelected = true;
        }
        else if (e.Key is Key.Left or Key.Right or Key.Up or Key.Down or Key.Home or Key.End)
        {
            int current = _loadedEntries.FindIndex(entry => entry.IsSelected);
            if (current < 0) current = 0;
            int next = e.Key switch
            {
                Key.Left => current - 1,
                Key.Right => current + 1,
                Key.Up => current - _columns,
                Key.Down => current + _columns,
                Key.Home => 0,
                Key.End => _loadedEntries.Count - 1,
                _ => current
            };
            SelectIndex(next, e.KeyModifiers.HasFlag(KeyModifiers.Shift));
        }
        else if (e.Key == Key.Enter && e.KeyModifiers.HasFlag(KeyModifiers.Control)) OnOpenFileClick(sender, e);
        else if (e.Key == Key.Enter && e.KeyModifiers.HasFlag(KeyModifiers.Shift)) OnOpenFolderClick(sender, e);
        else if (e.Key == Key.Enter) OpenSelectedItem();
        else if (e.Key == Key.C && e.KeyModifiers == KeyModifiers.Control) OnCopyUrlClick(sender, e);
        else if (e.Key == Key.C && e.KeyModifiers == KeyModifiers.Shift) OnCopyFileClick(sender, e);
        else if (e.Key == Key.C && e.KeyModifiers == KeyModifiers.Alt) OnCopyImageClick(sender, e);
        else if (e.Key == Key.Delete && e.KeyModifiers.HasFlag(KeyModifiers.Shift)) OnDeleteFileClick(sender, e);
        else if (e.Key == Key.Delete) OnDeleteClick(sender, e);
        else if (e.Key == Key.U && e.KeyModifiers.HasFlag(KeyModifiers.Control)) OnUploadFileClick(sender, e);
        else if (e.Key == Key.E && e.KeyModifiers.HasFlag(KeyModifiers.Control)) OnEditImageClick(sender, e);
        else if (e.Key == Key.P && e.KeyModifiers.HasFlag(KeyModifiers.Control)) OnPinToScreenClick(sender, e);
        else return;
        e.Handled = true;
    }
}
