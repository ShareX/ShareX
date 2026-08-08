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

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.HistoryLib.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.HistoryLib;

public partial class HistoryWindow : Window
{
    private static readonly HttpClient PreviewHttpClient = new();

    private readonly HistoryManagerSQLite _historyManager;
    private readonly HistorySettings _settings;
    private readonly HistoryWindowServices _services;
    private readonly DispatcherTimer _filterTimer;
    private List<HistoryItem> _allHistoryItems = [];
    private HistoryItem[] _filteredHistoryItems = [];
    private CancellationTokenSource? _previewCancellation;
    private CancellationTokenSource? _filterCancellation;
    private Bitmap? _previewBitmap;
    private bool _suppressFilterChanges;
    private int _filterVersion;
    private HistoryItem? _editingItem;
    private Func<Task>? _promptAction;
    private bool _isBusy;
    private PointerPressedEventArgs? _dragPointerPressed;
    private Point _dragStart;
    private bool _dragStarted;
    private bool _windowPlacementApplied;
    private bool _virtualListLayoutRefreshPending;

    private ShareX.HelpersLib.WindowState SavedWindowState =>
        _settings.WindowState ??= new ShareX.HelpersLib.WindowState();

    public HistoryWindow()
    {
        _historyManager = null!;
        _settings = new HistorySettings();
        _services = new HistoryWindowServices();
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _filterTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(220) };
        _filterTimer.Tick += OnFilterTimerTick;
    }

    public HistoryWindow(HistoryManagerSQLite historyManager, HistorySettings settings, HistoryWindowServices services) : this()
    {
        _historyManager = historyManager;
        _settings = settings;
        _services = services;

        _suppressFilterChanges = true;
        FromDatePicker.SelectedDate = DateTimeOffset.Now.Date;
        ToDatePicker.SelectedDate = DateTimeOffset.Now.Date;
        SearchTextBox.Text = _settings.RememberSearchText ? _settings.SearchText : string.Empty;
        _suppressFilterChanges = false;
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
        _windowPlacementApplied = true;
        Activate();
        Focus();
        await RefreshHistoryAsync();
        QueueVirtualListLayoutRefresh();
    }

    private void OnClosing(object? sender, WindowClosingEventArgs e) => SaveWindowState();

    private void OnClosed(object? sender, EventArgs e)
    {
        _filterTimer.Stop();
        _previewCancellation?.Cancel();
        _previewCancellation?.Dispose();
        _filterCancellation?.Cancel();
        _filterCancellation?.Dispose();
        _previewBitmap?.Dispose();
    }

    private void OnResized(object? sender, WindowResizedEventArgs e)
    {
        if (_windowPlacementApplied) SaveNormalWindowSize();
        QueueVirtualListLayoutRefresh();
    }

    private void QueueVirtualListLayoutRefresh()
    {
        if (_virtualListLayoutRefreshPending) return;

        _virtualListLayoutRefreshPending = true;
        Dispatcher.UIThread.Post(() =>
        {
            _virtualListLayoutRefreshPending = false;
            if (!IsVisible) return;

            HistoryList.InvalidateMeasure();
            HistoryList.InvalidateArrange();

            VirtualizingStackPanel? panel = HistoryList.GetVisualDescendants()
                .OfType<VirtualizingStackPanel>()
                .FirstOrDefault();
            panel?.InvalidateMeasure();
            panel?.InvalidateArrange();
        }, DispatcherPriority.Background);
    }

    private void OnPositionChanged(object? sender, PixelPointEventArgs e)
    {
        if (_windowPlacementApplied) SaveNormalWindowSize();
    }

    private void ApplySavedWindowState()
    {
        if (!_settings.RememberWindowState)
        {
            return;
        }

        ShareX.HelpersLib.WindowState savedState = SavedWindowState;

        if (!savedState.Size.IsEmpty)
        {
            Width = savedState.Size.Width;
            Height = savedState.Size.Height;
        }

        if (!savedState.Location.IsEmpty)
        {
            PixelPoint savedPosition = new(savedState.Location.X, savedState.Location.Y);
            if (Screens.ScreenFromPoint(savedPosition) != null)
            {
                WindowStartupLocation = WindowStartupLocation.Manual;
                Position = savedPosition;
            }
        }

        MainGrid.ColumnDefinitions[0].Width = new GridLength(Math.Max(500, _settings.HistoryListWidth));
        WindowState = savedState.IsMaximized
            ? Avalonia.Controls.WindowState.Maximized
            : Avalonia.Controls.WindowState.Normal;
    }

    private void SaveWindowState()
    {
        if (!_settings.RememberWindowState)
        {
            return;
        }

        SaveNormalWindowSize();
        SavedWindowState.IsMaximized = WindowState == Avalonia.Controls.WindowState.Maximized;
        double historyListWidth = MainGrid.ColumnDefinitions[0].ActualWidth;
        if (historyListWidth > 0)
        {
            _settings.HistoryListWidth = historyListWidth;
        }
    }

    private void SaveNormalWindowSize()
    {
        if (!_settings.RememberWindowState || WindowState != Avalonia.Controls.WindowState.Normal)
        {
            return;
        }

        if (Bounds.Width > 0 && Bounds.Height > 0)
        {
            SavedWindowState.Size = new System.Drawing.Size(
                (int)Math.Round(Bounds.Width),
                (int)Math.Round(Bounds.Height));
            SavedWindowState.Location = new System.Drawing.Point(Position.X, Position.Y);
        }
    }

    private async Task RefreshHistoryAsync()
    {
        SetBusy(true);

        try
        {
            List<HistoryItem> items = await _historyManager.GetHistoryItemsAsync();
            items.Reverse();

            (string[] types, string[] hosts) = await Task.Run(() =>
            {
                string[] availableTypes = items.Select(item => item.Type)
                    .Where(type => !string.IsNullOrWhiteSpace(type))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(type => type)
                    .ToArray()!;
                string[] availableHosts = items.Select(item => item.Host)
                    .Where(host => !string.IsNullOrWhiteSpace(host))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(host => host)
                    .ToArray()!;
                return (availableTypes, availableHosts);
            });

            _allHistoryItems = items;
            _suppressFilterChanges = true;
            TypeFilterComboBox.ItemsSource = types;
            HostFilterComboBox.ItemsSource = hosts;
            _suppressFilterChanges = false;
            await ApplyFilterAsync();
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void SetBusy(bool value)
    {
        _isBusy = value;
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_settings.RememberSearchText)
        {
            _settings.SearchText = SearchTextBox.Text ?? string.Empty;
        }

        ScheduleFilter();
    }

    private void OnAdvancedFilterChanged(object? sender, RoutedEventArgs e) => ScheduleFilter();

    private void OnDateFilterChanged(object? sender, DatePickerSelectedValueChangedEventArgs e) => ScheduleFilter();

    private void ScheduleFilter()
    {
        if (_suppressFilterChanges)
        {
            return;
        }

        _filterTimer.Stop();
        _filterTimer.Start();
    }

    private async void OnFilterTimerTick(object? sender, EventArgs e)
    {
        _filterTimer.Stop();
        await ApplyFilterAsync();
    }

    private HistoryFilter CreateCurrentFilter()
    {
        if (!AdvancedPanel.IsVisible)
        {
            return new HistoryFilter
            {
                Filename = SearchTextBox.Text ?? string.Empty,
                FilterFavorites = _settings.Favorites
            };
        }

        return new HistoryFilter
        {
            Filename = FilenameFilterTextBox.Text ?? string.Empty,
            URL = UrlFilterTextBox.Text ?? string.Empty,
            FilterType = TypeFilterCheckBox.IsChecked == true,
            Type = TypeFilterComboBox.SelectedItem as string,
            FilterHost = HostFilterCheckBox.IsChecked == true,
            Host = HostFilterComboBox.Text ?? string.Empty,
            FilterDate = DateFilterCheckBox.IsChecked == true,
            FromDate = FromDatePicker.SelectedDate?.Date ?? DateTime.Today,
            ToDate = ToDatePicker.SelectedDate?.Date ?? DateTime.Today
        };
    }

    private async Task ApplyFilterAsync()
    {
        int version = Interlocked.Increment(ref _filterVersion);
        HistoryFilter filter = CreateCurrentFilter();
        List<HistoryItem> source = _allHistoryItems;
        _filterCancellation?.Cancel();
        _filterCancellation?.Dispose();
        _filterCancellation = new CancellationTokenSource();
        CancellationToken token = _filterCancellation.Token;
        SetBusy(true);

        try
        {
            HistoryItem[] items = await Task.Run(() =>
            {
                IEnumerable<HistoryItem> cancellableSource = source.Where((item, index) =>
                {
                    if ((index & 1023) == 0)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    return true;
                });
                HistoryItem[] filtered = filter.ApplyFilter(cancellableSource).ToArray();
                token.ThrowIfCancellationRequested();
                return filtered;
            }, token);

            if (version != _filterVersion || !IsVisible)
            {
                return;
            }

            _filteredHistoryItems = items;
            HistoryList.ItemsSource = items;
            ItemCountText.Text = source.Count == items.Length
                ? string.Format(Strings.HistoryWindow_ItemsFormat, items.Length)
                : string.Format(Strings.HistoryWindow_ItemsOfFormat, items.Length, source.Count);
            Title = $"{Strings.HistoryWindows_ShareX_History} ({string.Format(Strings.HistoryWindow_ItemsFormat, items.Length)})";
            EmptyState.IsVisible = items.Length == 0;
            EmptyStateText.Text = source.Count == 0
                ? Strings.HistoryWindows_No_history_items
                : Strings.HistoryWindow_NoItemsMatchCurrentFilters;

            if (items.Length > 0)
            {
                HistoryList.SelectedIndex = 0;
            }
            else
            {
                await UpdatePreviewAsync(null);
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            if (version == _filterVersion)
            {
                SetBusy(false);
            }
        }
    }

    private async void OnHistorySelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        await UpdatePreviewAsync(GetSelectedItems().FirstOrDefault());
    }

    private async Task UpdatePreviewAsync(HistoryItem? item)
    {
        _previewCancellation?.Cancel();
        _previewCancellation?.Dispose();
        _previewCancellation = new CancellationTokenSource();
        CancellationToken token = _previewCancellation.Token;

        Bitmap? oldBitmap = _previewBitmap;
        _previewBitmap = null;
        PreviewImage.Source = null;
        oldBitmap?.Dispose();

        PreviewContent.IsVisible = item != null;
        NoSelectionState.IsVisible = item == null;

        if (item == null)
        {
            return;
        }

        PreviewTitle.Text = item.FileName;
        PreviewUnavailable.IsVisible = true;

        string? imageSource = null;
        bool localFile = false;

        if (!string.IsNullOrWhiteSpace(item.FilePath) && File.Exists(item.FilePath) && FileHelpers.IsImageFile(item.FilePath))
        {
            imageSource = item.FilePath;
            localFile = true;
        }
        else if (!string.IsNullOrWhiteSpace(item.URL) && FileHelpers.IsImageFile(item.URL))
        {
            imageSource = item.URL;
        }

        if (imageSource == null)
        {
            return;
        }

        try
        {
            Bitmap bitmap;
            if (localFile)
            {
                bitmap = await Task.Run(() => new Bitmap(imageSource), token);
            }
            else
            {
                byte[] data = await PreviewHttpClient.GetByteArrayAsync(imageSource, token);
                bitmap = await Task.Run(() =>
                {
                    using MemoryStream stream = new(data, writable: false);
                    return new Bitmap(stream);
                }, token);
            }

            if (token.IsCancellationRequested)
            {
                bitmap.Dispose();
                return;
            }

            _previewBitmap = bitmap;
            PreviewImage.Source = bitmap;
            PreviewUnavailable.IsVisible = false;
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            DebugHelper.WriteException(ex);
        }
    }

    private HistoryItem[] GetSelectedItems() =>
        HistoryList.SelectedItems?.OfType<HistoryItem>().ToArray() ?? [];

    private HistoryItem? GetPrimaryItem() => GetSelectedItems().FirstOrDefault();

    private void OnHistoryPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        PointerPoint point = e.GetCurrentPoint(HistoryList);
        ListBoxItem? container = (e.Source as Visual)?.GetVisualAncestors()
            .OfType<ListBoxItem>().FirstOrDefault() ?? e.Source as ListBoxItem;

        if (point.Properties.IsRightButtonPressed && container?.DataContext is HistoryItem item &&
            HistoryList.SelectedItems?.Contains(item) != true)
        {
            HistoryList.SelectedItems?.Clear();
            HistoryList.SelectedItem = item;
        }

        if (point.Properties.IsLeftButtonPressed)
        {
            _dragPointerPressed = e;
            _dragStart = e.GetPosition(HistoryList);
            _dragStarted = false;
        }
    }

    private async void OnHistoryPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_dragPointerPressed == null || _dragStarted || !e.GetCurrentPoint(HistoryList).Properties.IsLeftButtonPressed)
        {
            return;
        }

        Point current = e.GetPosition(HistoryList);
        if (Math.Abs(current.X - _dragStart.X) < 5 && Math.Abs(current.Y - _dragStart.Y) < 5)
        {
            return;
        }

        string[] files = GetSelectedItems().Select(item => item.FilePath)
            .Where(path => !string.IsNullOrWhiteSpace(path) && File.Exists(path))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()!;

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
            if (storageFile != null)
            {
                transfer.Add(DataTransferItem.CreateFile(storageFile));
            }
        }

        if (transfer.Items.Count > 0 && _dragPointerPressed != null)
        {
            await DragDrop.DoDragDropAsync(_dragPointerPressed, transfer, DragDropEffects.Copy);
        }

        _dragPointerPressed = null;
    }

    private void OnHistoryPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_dragStarted)
        {
            _dragPointerPressed = null;
        }
    }

    private void OnHistoryDoubleTapped(object? sender, TappedEventArgs e) => TryOpen();

    private void OnHistoryContextMenuOpening(object? sender, CancelEventArgs e)
    {
        HistoryItem[] selected = GetSelectedItems();
        if (selected.Length == 0)
        {
            e.Cancel = true;
            return;
        }

        HistoryItem item = selected[0];
        bool single = selected.Length == 1;
        bool url = !string.IsNullOrWhiteSpace(item.URL);
        bool shortUrl = !string.IsNullOrWhiteSpace(item.ShortenedURL);
        bool thumbnailUrl = !string.IsNullOrWhiteSpace(item.ThumbnailURL);
        bool deletionUrl = !string.IsNullOrWhiteSpace(item.DeletionURL);
        bool validPath = !string.IsNullOrWhiteSpace(item.FilePath) && Path.HasExtension(item.FilePath);
        bool file = validPath && File.Exists(item.FilePath);
        bool imageFile = file && FileHelpers.IsImageFile(item.FilePath);
        bool textFile = file && FileHelpers.IsTextFile(item.FilePath);

        OpenMenu.IsEnabled = single;
        OpenUrlMenu.IsEnabled = single && url;
        OpenShortUrlMenu.IsEnabled = single && shortUrl;
        OpenThumbnailUrlMenu.IsEnabled = single && thumbnailUrl;
        OpenDeletionUrlMenu.IsEnabled = single && deletionUrl;
        OpenFileMenu.IsEnabled = single && file;
        OpenFolderMenu.IsEnabled = single && file;
        CopyMenu.IsEnabled = true;
        CopyUrlMenu.IsEnabled = !single || url;
        CopyShortUrlMenu.IsEnabled = !single || shortUrl;
        CopyThumbnailUrlMenu.IsEnabled = !single || thumbnailUrl;
        CopyDeletionUrlMenu.IsEnabled = !single || deletionUrl;
        CopyFileMenu.IsEnabled = !single || file;
        CopyImageMenu.IsEnabled = single && imageFile;
        CopyTextMenu.IsEnabled = single && textFile;
        CopyFilePathMenu.IsEnabled = !single || validPath;
        CopyFileNameMenu.IsEnabled = !single || validPath;
        CopyFileNameExtensionMenu.IsEnabled = !single || validPath;
        CopyFolderMenu.IsEnabled = !single || validPath;
        CopyHtmlLinkMenu.IsEnabled = !single || url;
        CopyHtmlImageMenu.IsEnabled = !single || (url && FileHelpers.IsImageFile(item.URL));
        CopyHtmlLinkedImageMenu.IsEnabled = !single || (url && FileHelpers.IsImageFile(item.URL) && thumbnailUrl);
        CopyForumLinkMenu.IsEnabled = CopyHtmlLinkMenu.IsEnabled;
        CopyForumImageMenu.IsEnabled = CopyHtmlImageMenu.IsEnabled;
        CopyForumLinkedImageMenu.IsEnabled = CopyHtmlLinkedImageMenu.IsEnabled;
        CopyMarkdownLinkMenu.IsEnabled = CopyHtmlLinkMenu.IsEnabled;
        CopyMarkdownImageMenu.IsEnabled = CopyHtmlImageMenu.IsEnabled;
        CopyMarkdownLinkedImageMenu.IsEnabled = CopyHtmlLinkedImageMenu.IsEnabled;
        FavoriteMenu.Header = item.Favorite
            ? Strings.HistoryWindow_Unfavorite
            : Strings.HistoryWindows_Favorite;
        TagMenu.IsEnabled = single;
        EditMenu.IsEnabled = single;
        RenameMenu.IsEnabled = single && validPath;
        PreviewMenu.IsEnabled = single && imageFile;
        UploadMenu.IsEnabled = single && file && _services.UploadFile != null;
        EditImageMenu.IsEnabled = single && imageFile && _services.EditImage != null;
        PinMenu.IsEnabled = single && imageFile && _services.PinToScreen != null;
        AnalyzeMenu.IsEnabled = single && imageFile && _services.AnalyzeImage != null;
    }

    private void OnAdvancedClick(object? sender, RoutedEventArgs e)
    {
        AdvancedPanel.IsVisible = !AdvancedPanel.IsVisible;
        AdvancedButton.Classes.Set("active", AdvancedPanel.IsVisible);
        _ = ApplyFilterAsync();
    }

    private void OnFavoritesClick(object? sender, RoutedEventArgs e)
    {
        _settings.Favorites = !_settings.Favorites;
        FavoritesButton.Classes.Set("active", _settings.Favorites);
        _ = ApplyFilterAsync();
    }

    private void OnResetFiltersClick(object? sender, RoutedEventArgs e)
    {
        _suppressFilterChanges = true;
        FilenameFilterTextBox.Text = string.Empty;
        UrlFilterTextBox.Text = string.Empty;
        TypeFilterCheckBox.IsChecked = false;
        TypeFilterComboBox.SelectedIndex = -1;
        HostFilterCheckBox.IsChecked = false;
        HostFilterComboBox.SelectedIndex = -1;
        HostFilterComboBox.Text = string.Empty;
        DateFilterCheckBox.IsChecked = false;
        FromDatePicker.SelectedDate = DateTimeOffset.Now.Date;
        ToDatePicker.SelectedDate = DateTimeOffset.Now.Date;
        _suppressFilterChanges = false;
        _ = ApplyFilterAsync();
    }

    private async void OnStatsClick(object? sender, RoutedEventArgs e)
    {
        ShowModal(StatsDialog);
        StatsTextBox.Text = Strings.HistoryWindow_CalculatingStatistics;
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
            Title = Strings.HistoryWindow_SelectFolderToImport,
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
            ImportStatusText.Text = Strings.HistoryWindow_SelectedFolderDoesNotExist;
            return;
        }

        SetBusy(true);
        ImportConfirmButton.IsEnabled = false;
        ImportStatusText.Text = Strings.HistoryWindow_ImportingFiles;

        try
        {
            bool imagesOnly = ImportImagesOnlyCheckBox.IsChecked == true;
            bool skipDuplicates = ImportSkipDuplicatesCheckBox.IsChecked == true;
            HashSet<string> existing = skipDuplicates
                ? _allHistoryItems.Where(item => !string.IsNullOrWhiteSpace(item.FilePath))
                    .Select(item => item.FilePath).ToHashSet(StringComparer.OrdinalIgnoreCase)!
                : [];

            int imported = await Task.Run(() => ImportFolder(folder, imagesOnly, existing));
            ImportStatusText.Text = string.Format(Strings.HistoryWindow_SuccessfullyImportedFilesFormat, imported);
            if (imported > 0)
            {
                await RefreshHistoryAsync();
            }
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
        if (onlyImages)
        {
            files = files.Where(FileHelpers.IsImageFile);
        }

        List<HistoryItem> imported = [];
        foreach (string file in files.OrderBy(File.GetLastWriteTime))
        {
            if (existing.Count > 0 && existing.Contains(file))
            {
                continue;
            }

            imported.Add(new HistoryItem
            {
                FileName = Path.GetFileName(file),
                FilePath = file,
                DateTime = File.GetLastWriteTime(file),
                Type = FileHelpers.IsImageFile(file) ? "Image" : FileHelpers.IsTextFile(file) ? "Text" : "File"
            });
        }

        if (imported.Count > 0)
        {
            _historyManager.AppendHistoryItems(imported);
        }

        return imported.Count;
    }

    private void OnSettingsClick(object? sender, RoutedEventArgs e)
    {
        RememberSearchToggle.IsChecked = _settings.RememberSearchText;
        RememberWindowToggle.IsChecked = _settings.RememberWindowState;
        ShowModal(SettingsDialog);
    }

    private void OnSaveSettingsClick(object? sender, RoutedEventArgs e)
    {
        _settings.RememberSearchText = RememberSearchToggle.IsChecked == true;
        _settings.RememberWindowState = RememberWindowToggle.IsChecked == true;
        _settings.SearchText = _settings.RememberSearchText ? SearchTextBox.Text ?? string.Empty : string.Empty;
        CloseModal();
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
        HistoryList.Focus();
    }

    private void OnCloseModalClick(object? sender, RoutedEventArgs e) => CloseModal();

    private void ShowPrompt(string title, string message, string confirmText, Func<Task> action,
        string? input = null)
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
        if (action == null)
        {
            CloseModal();
            return;
        }

        await action();
        CloseModal();
    }

    private void OnEditTagClick(object? sender, RoutedEventArgs e)
    {
        HistoryItem? item = GetPrimaryItem();
        if (item == null) return;
        ShowPrompt(Strings.HistoryWindow_EditTag, Strings.HistoryWindow_EnterTagForHistoryItem, Strings.HistoryWindows_Save, () =>
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
        Dictionary<string, string> tags = new();
        foreach (string line in (text ?? string.Empty).Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries))
        {
            int separator = line.IndexOf('=');
            string key = (separator >= 0 ? line[..separator] : line).Trim();
            if (!string.IsNullOrWhiteSpace(key))
            {
                tags[key] = separator >= 0 ? line[(separator + 1)..] : null!;
            }
        }
        return tags;
    }

    private void OnRenameFileClick(object? sender, RoutedEventArgs e)
    {
        HistoryItem? item = GetPrimaryItem();
        if (item == null || string.IsNullOrWhiteSpace(item.FilePath)) return;
        string oldName = Path.GetFileNameWithoutExtension(item.FilePath);
        ShowPrompt(Strings.HistoryWindow_RenameFile, Strings.HistoryWindow_EnterNewFileName, Strings.HistoryWindow_Rename, () =>
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

    private void OnDeleteClick(object? sender, RoutedEventArgs e) => ConfirmDelete(false);
    private void OnDeleteFileClick(object? sender, RoutedEventArgs e) => ConfirmDelete(true);

    private void ConfirmDelete(bool deleteFiles)
    {
        HistoryItem[] items = GetSelectedItems();
        if (items.Length == 0) return;
        string noun = items.Length == 1
            ? (deleteFiles ? Strings.HistoryWindow_ThisFile : Strings.HistoryWindow_ThisItem)
            : (deleteFiles
                ? string.Format(Strings.HistoryWindow_TheseFilesFormat, items.Length)
                : string.Format(Strings.HistoryWindow_TheseItemsFormat, items.Length));
        ShowPrompt(deleteFiles ? Strings.HistoryWindow_DeleteFiles : Strings.HistoryWindow_DeleteHistoryItems,
            string.Format(Strings.HistoryWindow_ConfirmDeleteFormat, noun), Strings.HistoryWindow_Delete, async () =>
            {
                if (deleteFiles)
                {
                    foreach (HistoryItem item in items)
                    {
                        if (!string.IsNullOrWhiteSpace(item.FilePath) && File.Exists(item.FilePath))
                        {
                            File.Delete(item.FilePath);
                        }
                    }
                }
                _historyManager.Delete(items);
                HashSet<long> ids = items.Select(item => item.Id).ToHashSet();
                _allHistoryItems.RemoveAll(item => ids.Contains(item.Id));
                await ApplyFilterAsync();
            });
    }

    private void RefreshVisibleItem(HistoryItem item)
    {
        int index = Array.IndexOf(_filteredHistoryItems, item);
        HistoryList.ItemsSource = null;
        HistoryList.ItemsSource = _filteredHistoryItems;
        HistoryList.SelectedItem = item;
        if (index >= 0) HistoryList.ScrollIntoView(index);
    }

    private void OnToggleFavoriteClick(object? sender, RoutedEventArgs e)
    {
        HistoryItem[] items = GetSelectedItems();
        foreach (HistoryItem item in items)
        {
            item.Favorite = !item.Favorite;
            _historyManager.Edit(item);
        }
        if (_settings.Favorites) _ = ApplyFilterAsync();
        else if (items.Length > 0) RefreshVisibleItem(items[0]);
    }

    private void TryOpen()
    {
        HistoryItem? item = GetPrimaryItem();
        if (item == null) return;
        if (!string.IsNullOrWhiteSpace(item.ShortenedURL)) URLHelpers.OpenURL(item.ShortenedURL);
        else if (!string.IsNullOrWhiteSpace(item.URL)) URLHelpers.OpenURL(item.URL);
        else if (!string.IsNullOrWhiteSpace(item.FilePath) && File.Exists(item.FilePath)) FileHelpers.OpenFile(item.FilePath);
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
    private void OnCopyHtmlLinkClick(object? sender, RoutedEventArgs e) => CopyValues(item => $"<a href=\"{item.URL}\">{item.URL}</a>", HasUrl);
    private void OnCopyHtmlImageClick(object? sender, RoutedEventArgs e) => CopyValues(item => $"<img src=\"{item.URL}\"/>", HasImageUrl);
    private void OnCopyHtmlLinkedImageClick(object? sender, RoutedEventArgs e) => CopyValues(item => $"<a href=\"{item.URL}\"><img src=\"{item.ThumbnailURL}\"/></a>", HasLinkedImage);
    private void OnCopyForumLinkClick(object? sender, RoutedEventArgs e) => CopyValues(item => $"[url]{item.URL}[/url]", HasUrl);
    private void OnCopyForumImageClick(object? sender, RoutedEventArgs e) => CopyValues(item => $"[img]{item.URL}[/img]", HasImageUrl);
    private void OnCopyForumLinkedImageClick(object? sender, RoutedEventArgs e) => CopyValues(item => $"[url={item.URL}][img]{item.ThumbnailURL}[/img][/url]", HasLinkedImage);
    private void OnCopyMarkdownLinkClick(object? sender, RoutedEventArgs e) => CopyValues(item => $"[{item.FileName}]({item.URL})", HasUrl);
    private void OnCopyMarkdownImageClick(object? sender, RoutedEventArgs e) => CopyValues(item => $"![{item.FileName}]({item.URL})", HasImageUrl);
    private void OnCopyMarkdownLinkedImageClick(object? sender, RoutedEventArgs e) => CopyValues(item => $"[![{item.FileName}]({item.ThumbnailURL})]({item.URL})", HasLinkedImage);
    private static bool HasUrl(HistoryItem item) => !string.IsNullOrWhiteSpace(item.URL);
    private static bool HasImageUrl(HistoryItem item) => HasUrl(item) && FileHelpers.IsImageFile(item.URL);
    private static bool HasLinkedImage(HistoryItem item) => HasImageUrl(item) && !string.IsNullOrWhiteSpace(item.ThumbnailURL);

    private void OnCopyFileClick(object? sender, RoutedEventArgs e)
    {
        string[] files = GetSelectedItems().Select(item => item.FilePath).Where(File.Exists).ToArray();
        if (files.Length > 0) ClipboardHelpers.CopyFile(files);
    }

    private void OnCopyImageClick(object? sender, RoutedEventArgs e)
    {
        HistoryItem? item = GetPrimaryItem();
        if (item != null && File.Exists(item.FilePath) && FileHelpers.IsImageFile(item.FilePath)) ClipboardHelpers.CopyImageFromFile(item.FilePath);
    }

    private void OnCopyTextClick(object? sender, RoutedEventArgs e)
    {
        HistoryItem? item = GetPrimaryItem();
        if (item != null && File.Exists(item.FilePath) && FileHelpers.IsTextFile(item.FilePath)) ClipboardHelpers.CopyTextFromFile(item.FilePath);
    }

    private void OnCopyFilePathClick(object? sender, RoutedEventArgs e) => CopyValues(item => item.FilePath, item => !string.IsNullOrWhiteSpace(item.FilePath) && Path.HasExtension(item.FilePath));
    private void OnCopyFileNameClick(object? sender, RoutedEventArgs e) => CopyValues(item => Path.GetFileNameWithoutExtension(item.FilePath), item => !string.IsNullOrWhiteSpace(item.FilePath));
    private void OnCopyFileNameWithExtensionClick(object? sender, RoutedEventArgs e) => CopyValues(item => Path.GetFileName(item.FilePath), item => !string.IsNullOrWhiteSpace(item.FilePath));
    private void OnCopyFolderClick(object? sender, RoutedEventArgs e) => CopyValues(item => Path.GetDirectoryName(item.FilePath), item => !string.IsNullOrWhiteSpace(item.FilePath));

    private void OnShowPreviewClick(object? sender, RoutedEventArgs e) => ShowSelectedImage();
    private void OnPreviewPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) ShowSelectedImage();
    }
    private void ShowSelectedImage()
    {
        HistoryItem? item = GetPrimaryItem();
        if (item != null && File.Exists(item.FilePath) && FileHelpers.IsImageFile(item.FilePath)) _services.ShowImage?.Invoke(item.FilePath);
    }

    private void OnUploadFileClick(object? sender, RoutedEventArgs e) => InvokeFileService(_services.UploadFile, false);
    private void OnEditImageClick(object? sender, RoutedEventArgs e) => InvokeFileService(_services.EditImage, true);
    private void OnPinToScreenClick(object? sender, RoutedEventArgs e) => InvokeFileService(_services.PinToScreen, true);
    private void OnAnalyzeImageClick(object? sender, RoutedEventArgs e) => InvokeFileService(_services.AnalyzeImage, true);
    private void InvokeFileService(Action<string>? action, bool imageOnly)
    {
        HistoryItem? item = GetPrimaryItem();
        if (action != null && item != null && File.Exists(item.FilePath) && (!imageOnly || FileHelpers.IsImageFile(item.FilePath))) action(item.FilePath);
    }

    private void OnHistoryListKeyDown(object? sender, KeyEventArgs e)
    {
        KeyModifiers modifiers = e.KeyModifiers;
        if (e.Key == Key.Enter && modifiers.HasFlag(KeyModifiers.Control)) OnOpenFileClick(sender, e);
        else if (e.Key == Key.Enter && modifiers.HasFlag(KeyModifiers.Shift)) OnOpenFolderClick(sender, e);
        else if (e.Key == Key.Enter) TryOpen();
        else if (e.Key == Key.C && modifiers == KeyModifiers.Control) OnCopyUrlClick(sender, e);
        else if (e.Key == Key.C && modifiers == KeyModifiers.Shift) OnCopyFileClick(sender, e);
        else if (e.Key == Key.C && modifiers == KeyModifiers.Alt) OnCopyImageClick(sender, e);
        else if (e.Key == Key.C && modifiers.HasFlag(KeyModifiers.Control) && modifiers.HasFlag(KeyModifiers.Shift)) OnCopyFilePathClick(sender, e);
        else if (e.Key == Key.Delete && modifiers.HasFlag(KeyModifiers.Shift)) OnDeleteFileClick(sender, e);
        else if (e.Key == Key.Delete) OnDeleteClick(sender, e);
        else if (e.Key == Key.U && modifiers.HasFlag(KeyModifiers.Control)) OnUploadFileClick(sender, e);
        else if (e.Key == Key.E && modifiers.HasFlag(KeyModifiers.Control)) OnEditImageClick(sender, e);
        else if (e.Key == Key.P && modifiers.HasFlag(KeyModifiers.Control)) OnPinToScreenClick(sender, e);
        else return;
        e.Handled = true;
    }

    private async void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.F5)
        {
            e.Handled = true;
            await RefreshHistoryAsync();
        }
        else if (e.Key == Key.Escape)
        {
            e.Handled = true;
            if (ModalOverlay.IsVisible) CloseModal();
            else Close();
        }
    }
}
