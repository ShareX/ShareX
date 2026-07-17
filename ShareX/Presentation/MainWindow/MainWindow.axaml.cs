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
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AppResources = ShareX.Properties.Resources;
using DrawingBitmap = System.Drawing.Bitmap;
using DrawingIcon = System.Drawing.Icon;
using DrawingPoint = System.Drawing.Point;
using DrawingSize = System.Drawing.Size;
using FormsCursor = System.Windows.Forms.Cursor;
using FormsDataFormats = System.Windows.Forms.DataFormats;
using FormsDataObject = System.Windows.Forms.DataObject;
using FormsDialogResult = System.Windows.Forms.DialogResult;
using FormsMessageBox = System.Windows.Forms.MessageBox;
using FormsMessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
using FormsMouseButtons = System.Windows.Forms.MouseButtons;
using FormsMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using FormsOrientation = System.Windows.Forms.Orientation;

namespace ShareX;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private readonly MainForm _host;
    private readonly MainMenuBuilder _navigationMenuBuilder;
    private readonly MainMenuBuilder _trayMenuBuilder;
    private readonly UploadInfoManager _uploadInfoManager = new();
    private ContextMenu? _activeContextMenu;
    private Window? _trayMenuAnchor;
    private DrawingIcon? _ownedTrayIcon;
    private PointerPressedEventArgs? _thumbnailDragTrigger;
    private Task<IStorageFile?>? _thumbnailDragFileTask;
    private IPointer? _thumbnailDragPointer;
    private Point _thumbnailDragStart;
    private bool _thumbnailDragStarted;
    private int _thumbnailDragVersion;
    private bool _allowClose;
    private bool _disposed;
    private int _lastSelectedIndex = -1;

    internal ObservableCollection<ThumbnailItemViewModel> ThumbnailItems { get; } = new();
    internal ObservableCollection<HotkeyTipViewModel> HotkeyTips { get; } = new();
    public bool IsEmpty => ThumbnailItems.Count == 0;

    public MainWindow() : this(Program.MainForm)
    {
    }

    public MainWindow(MainForm host)
    {
        _host = host;
        _navigationMenuBuilder = new MainMenuBuilder(host);
        _trayMenuBuilder = new MainMenuBuilder(host, trayMenu: true);

        InitializeComponent();
        DataContext = this;
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        Title = Program.Title;
        Icon = CreateWindowIcon();

        BuildNavigation();
        ConfigureWindowHeightFromNavigation();
        RestoreWindowBounds();
        RefreshHotkeyTips();

        foreach (WorkerTask task in TaskManager.Tasks)
        {
            AddTask(task);
        }

        _host.niTray.ContextMenuStrip = null;
        _host.niTray.Text = Program.TitleShort;
        _host.niTray.Visible = Program.Settings.ShowTray;
        _host.niTray.MouseDown += OnHostTrayMouseDown;
        _host.niTray.MouseUp += OnHostTrayMouseUp;

        TaskManager.TaskAdded += OnTaskAdded;
        TaskManager.TaskRemoved += OnTaskRemoved;
        TaskManager.TaskChanged += OnTaskChanged;
        TaskManager.TaskImageReady += OnTaskImageReady;
        TaskManager.TaskCollectionChanged += OnTaskCollectionChanged;
        ThemeManager.ThemeChanged += OnThemeChanged;

        Closing += OnClosing;
        PositionChanged += (_, _) => SaveWindowBounds();
        Resized += (_, _) => SaveWindowBounds();
        KeyDown += OnWindowKeyDown;
    }

    public void ShowAndActivate()
    {
        if (!IsVisible)
        {
            Show();
        }

        if (WindowState == Avalonia.Controls.WindowState.Minimized)
        {
            WindowState = Avalonia.Controls.WindowState.Normal;
        }

        Activate();
        MainWindowIntegration.ReportVisibility(true);
    }

    public void HideToTray()
    {
        SaveWindowBounds();
        Hide();
        MainWindowIntegration.ReportVisibility(false);
    }

    public void SetTitle(string title)
    {
        Title = title;
        _host.niTray.Text = title.Truncate(63);
    }

    public void SetTrayVisible(bool visible) => _host.niTray.Visible = visible;

    public void SetTrayIcon(byte[] iconBytes)
    {
        using MemoryStream stream = new(iconBytes, writable: false);
        using DrawingIcon source = new(stream);
        DrawingIcon replacement = (DrawingIcon)source.Clone();
        _host.niTray.Icon = replacement;
        _ownedTrayIcon?.Dispose();
        _ownedTrayIcon = replacement;
    }

    public void ShowTrayMenu()
    {
        if (_trayMenuAnchor != null)
        {
            CloseActiveContextMenu();
            CloseTrayMenuAnchor();
            return;
        }

        CloseActiveContextMenu();

        System.Drawing.Point cursorPosition = FormsCursor.Position;
        PixelPoint anchorPosition = new(cursorPosition.X, cursorPosition.Y);
        Window anchor = new()
        {
            Width = 1,
            Height = 1,
            MinWidth = 1,
            MinHeight = 1,
            Position = anchorPosition,
            WindowStartupLocation = WindowStartupLocation.Manual,
            WindowDecorations = Avalonia.Controls.WindowDecorations.None,
            ShowInTaskbar = false,
            CanResize = false,
            Topmost = true,
            Opacity = 0,
            Background = Brushes.Transparent,
            RequestedThemeVariant = ThemeManager.GetCurrentTheme()
        };
        _trayMenuAnchor = anchor;
        anchor.Deactivated += OnTrayMenuAnchorDeactivated;
        anchor.Show();
        anchor.Position = anchorPosition;
        anchor.Activate();

        ContextMenu menu = BuildContextMenu(_trayMenuBuilder.BuildTrayMenu());
        menu.Closed += OnTrayMenuClosed;

        try
        {
            if (!TryOpenContextMenu(menu, anchor, PlacementMode.BottomEdgeAlignedLeft))
            {
                menu.Closed -= OnTrayMenuClosed;
                CloseTrayMenuAnchor();
            }
            else if (Program.Settings.TrayAutoExpandCaptureMenu && menu.Items.OfType<MenuItem>().FirstOrDefault() is MenuItem captureItem)
            {
                Dispatcher.UIThread.Post(captureItem.Open);
            }
        }
        catch
        {
            menu.Closed -= OnTrayMenuClosed;
            CloseTrayMenuAnchor();
            throw;
        }
    }

    public void RefreshMenus()
    {
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        BuildNavigation();
        RefreshHotkeyTips();

        foreach (ThumbnailItemViewModel item in ThumbnailItems)
        {
            item.RefreshSettings();
        }

        NotifyTaskCollectionChanged();
    }

    public void CloseFromHost()
    {
        if (_disposed)
        {
            return;
        }

        _allowClose = true;
        Close();
    }

    private void BuildNavigation()
    {
        NavigationPanel.Children.Clear();
        int index = 0;

        foreach (MainNavigationSection section in _navigationMenuBuilder.BuildNavigation().Where(x => x.IsVisible))
        {
            if (index is 4 or 7 or 12 or 15)
            {
                NavigationPanel.Children.Add(new Separator { Margin = new Thickness(5, 2) });
            }

            MainNavigationSection current = section;
            Button button = new()
            {
                Classes = { "nav-button" },
                Content = CreateNavigationContent(current),
                Tag = current
            };
            button.Click += OnNavigationClick;
            NavigationPanel.Children.Add(button);
            index++;
        }
    }

    private void ConfigureWindowHeightFromNavigation()
    {
        NavigationPanel.Measure(new Avalonia.Size(210, double.PositiveInfinity));
        double navigationHeight = Math.Ceiling(NavigationPanel.DesiredSize.Height);

        if (navigationHeight <= 0 || double.IsNaN(navigationHeight) || double.IsInfinity(navigationHeight))
        {
            return;
        }

        MinHeight = navigationHeight;

        DrawingSize savedSize = Program.Settings.MainFormSize;
        if (!Program.Settings.RememberMainFormSize || savedSize.IsEmpty)
        {
            Height = navigationHeight;
        }
    }

    private static Control CreateNavigationContent(MainNavigationSection section)
    {
        Grid grid = new() { ColumnDefinitions = new ColumnDefinitions("20,7,*,Auto") };
        grid.Children.Add(CreateAccentMenuIcon(section.Icon, 16));

        TextBlock label = new()
        {
            Text = section.Header,
            FontWeight = FontWeight.Normal,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };
        Grid.SetColumn(label, 2);
        grid.Children.Add(label);

        if (section.CreateChildren != null)
        {
            TextBlock chevron = CreateAccentMenuIcon(LucideIcons.chevron_right, 14);
            chevron.Opacity = 0.7;
            Grid.SetColumn(chevron, 3);
            grid.Children.Add(chevron);
        }

        return grid;
    }

    private void OnNavigationClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not MainNavigationSection section)
        {
            return;
        }

        if (section.Execute != null)
        {
            section.Execute();
            return;
        }

        if (section.CreateChildren != null)
        {
            ContextMenu menu = BuildContextMenu(section.CreateChildren());
            TryOpenContextMenu(menu, button, PlacementMode.RightEdgeAlignedTop);
        }
    }

    private ContextMenu BuildContextMenu(IEnumerable<MainMenuEntry> entries)
    {
        ContextMenu menu = new();

        foreach (Control item in BuildMenuControls(entries))
        {
            menu.Items.Add(item);
        }

        return menu;
    }

    private bool TryOpenContextMenu(ContextMenu menu, Control placementTarget, PlacementMode placement)
    {
        if (_activeContextMenu != null)
        {
            return false;
        }

        _activeContextMenu = menu;
        menu.Placement = placement;
        menu.Closed += OnActiveContextMenuClosed;

        try
        {
            menu.Open(placementTarget);
            return true;
        }
        catch
        {
            menu.Closed -= OnActiveContextMenuClosed;
            _activeContextMenu = null;
            throw;
        }
    }

    private void OnActiveContextMenuClosed(object? sender, RoutedEventArgs e)
    {
        if (sender is ContextMenu menu)
        {
            menu.Closed -= OnActiveContextMenuClosed;

            if (ReferenceEquals(_activeContextMenu, menu))
            {
                _activeContextMenu = null;
            }
        }
    }

    private void CloseActiveContextMenu()
    {
        if (_activeContextMenu == null)
        {
            return;
        }

        ContextMenu menu = _activeContextMenu;
        _activeContextMenu = null;
        menu.Closed -= OnActiveContextMenuClosed;
        menu.Close();
    }

    private IEnumerable<Control> BuildMenuControls(IEnumerable<MainMenuEntry> entries)
    {
        foreach (MainMenuEntry entry in entries.Where(x => x.IsVisible))
        {
            if (entry.IsSeparator)
            {
                Separator separator = new();
                separator.Classes.Add("compact-menu-separator");
                yield return separator;
                continue;
            }

            MenuItem item = new()
            {
                Header = entry.Header,
                Icon = CreateAccentMenuIcon(entry.Icon, 16),
                InputGesture = entry.InputGesture,
                IsEnabled = entry.IsEnabled,
                IsChecked = entry.IsChecked,
                ToggleType = entry.ToggleType switch
                {
                    MainMenuToggleType.CheckBox => MenuItemToggleType.CheckBox,
                    MainMenuToggleType.Radio => MenuItemToggleType.Radio,
                    _ => MenuItemToggleType.None
                }
            };
            item.Classes.Add("compact-menu-item");

            if (entry.CreateChildren != null)
            {
                foreach (Control child in BuildMenuControls(entry.CreateChildren()))
                {
                    item.Items.Add(child);
                }
            }

            if (entry.ExecuteAsync != null)
            {
                item.Click += async (_, _) =>
                {
                    try
                    {
                        await entry.ExecuteAsync();
                        RefreshMenus();
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.WriteException(ex);
                    }
                };
            }

            yield return item;
        }
    }

    private static TextBlock CreateLucideText(string glyph, double size)
    {
        TextBlock text = new()
        {
            Text = glyph,
            FontSize = size,
            FontWeight = FontWeight.Normal,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };
        text.Classes.Add("icon");
        return text;
    }

    private static TextBlock CreateAccentMenuIcon(string glyph, double size)
    {
        TextBlock icon = CreateLucideText(glyph, size);
        icon.Classes.Add("accent-menu-icon");
        return icon;
    }

    private void OnTaskAdded(WorkerTask task) => Dispatcher.UIThread.Post(() => AddTask(task));

    private void AddTask(WorkerTask task)
    {
        if (ThumbnailItems.Any(x => ReferenceEquals(x.Task, task)))
        {
            return;
        }

        ThumbnailItemViewModel item = new(task);
        ThumbnailItems.Insert(0, item);

        NotifyTaskCollectionChanged();
    }

    private void OnTaskRemoved(WorkerTask task) => Dispatcher.UIThread.Post(() =>
    {
        ThumbnailItemViewModel? item = ThumbnailItems.FirstOrDefault(x => ReferenceEquals(x.Task, task));
        if (item != null)
        {
            ThumbnailItems.Remove(item);
            item.Dispose();
        }

        NotifyTaskCollectionChanged();
    });

    private void OnTaskChanged(WorkerTask task) => Dispatcher.UIThread.Post(() =>
    {
        ThumbnailItems.FirstOrDefault(x => ReferenceEquals(x.Task, task))?.Refresh();
    });

    private void OnTaskImageReady(WorkerTask task, DrawingBitmap image)
    {
        if (image == null)
        {
            return;
        }

        DrawingBitmap imageCopy;

        try
        {
            // WorkerTask owns and disposes the ImageReady bitmap as soon as the event returns.
            // Give the asynchronous Avalonia decoder its own image to avoid sharing GDI+ state.
            imageCopy = (DrawingBitmap)image.Clone();
        }
        catch (Exception e)
        {
            DebugHelper.WriteException(e);
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            if (_disposed)
            {
                imageCopy.Dispose();
                return;
            }

            ThumbnailItemViewModel? item = ThumbnailItems.FirstOrDefault(x => ReferenceEquals(x.Task, task));

            if (item != null)
            {
                item.RefreshFromOwnedImage(imageCopy);
            }
            else
            {
                imageCopy.Dispose();
            }
        });
    }

    private void OnTaskCollectionChanged() => Dispatcher.UIThread.Post(NotifyTaskCollectionChanged);

    private void OnThemeChanged(object? sender, Avalonia.Styling.ThemeVariant theme) =>
        Dispatcher.UIThread.Post(() => RequestedThemeVariant = theme);

    private void NotifyTaskCollectionChanged()
    {
        OnPropertyChanged(nameof(IsEmpty));
    }

    private void RefreshHotkeyTips()
    {
        HotkeyTips.Clear();

        if (Program.HotkeysConfig?.Hotkeys == null)
        {
            return;
        }

        foreach (HotkeySettings hotkey in Program.HotkeysConfig.Hotkeys.Where(x => x.HotkeyInfo.IsValidHotkey))
        {
            HotkeyTips.Add(new HotkeyTipViewModel(hotkey));
        }
    }

    private IReadOnlyList<ThumbnailItemViewModel> GetSelectedItems() => ThumbnailItems.Where(x => x.IsSelected).ToArray();

    private void SelectOnly(ThumbnailItemViewModel selected)
    {
        foreach (ThumbnailItemViewModel item in ThumbnailItems)
        {
            item.IsSelected = ReferenceEquals(item, selected);
        }

        _lastSelectedIndex = ThumbnailItems.IndexOf(selected);
    }

    private void OnThumbnailPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control || control.DataContext is not ThumbnailItemViewModel item)
        {
            return;
        }

        PointerPoint point = e.GetCurrentPoint(control);
        int index = ThumbnailItems.IndexOf(item);

        if (point.Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed)
        {
            CloseActiveContextMenu();

            if (!item.IsSelected)
            {
                SelectOnly(item);
            }

            e.Handled = true;
            return;
        }

        if (point.Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonPressed)
        {
            return;
        }

        bool isControlPressed = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        bool isShiftPressed = e.KeyModifiers.HasFlag(KeyModifiers.Shift);

        if (isShiftPressed && _lastSelectedIndex >= 0)
        {
            int start = Math.Min(_lastSelectedIndex, index);
            int end = Math.Max(_lastSelectedIndex, index);

            for (int i = 0; i < ThumbnailItems.Count; i++)
            {
                if (!isControlPressed)
                {
                    ThumbnailItems[i].IsSelected = false;
                }

                if (i >= start && i <= end)
                {
                    ThumbnailItems[i].IsSelected = true;
                }
            }
        }
        else if (isControlPressed)
        {
            item.IsSelected = !item.IsSelected;
            _lastSelectedIndex = index;
        }
        else
        {
            SelectOnly(item);
        }

        PrepareThumbnailDrag(control, item, e);
        e.Handled = true;
    }

    private void PrepareThumbnailDrag(Control control, ThumbnailItemViewModel item, PointerPressedEventArgs e)
    {
        ResetThumbnailDrag();

        string? filePath = item.Task.Info?.FilePath;
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            return;
        }

        _thumbnailDragTrigger = e;
        _thumbnailDragFileTask = StorageProvider.TryGetFileFromPathAsync(filePath);
        _thumbnailDragPointer = e.Pointer;
        _thumbnailDragStart = e.GetPosition(control);
        e.Pointer.Capture(control);
    }

    private async void OnThumbnailPointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is not Control control ||
            _thumbnailDragTrigger == null ||
            _thumbnailDragFileTask == null ||
            _thumbnailDragStarted ||
            !e.GetCurrentPoint(control).Properties.IsLeftButtonPressed)
        {
            return;
        }

        Point position = e.GetPosition(control);
        DrawingSize dragSize = System.Windows.Forms.SystemInformation.DragSize;
        if (Math.Abs(position.X - _thumbnailDragStart.X) < dragSize.Width / 2d &&
            Math.Abs(position.Y - _thumbnailDragStart.Y) < dragSize.Height / 2d)
        {
            return;
        }

        _thumbnailDragStarted = true;
        int version = _thumbnailDragVersion;
        PointerPressedEventArgs trigger = _thumbnailDragTrigger;
        Task<IStorageFile?> fileTask = _thumbnailDragFileTask;
        _thumbnailDragPointer = null;
        e.Pointer.Capture(null);

        try
        {
            IStorageFile? file = await fileTask;
            if (file == null || version != _thumbnailDragVersion)
            {
                return;
            }

            DataTransfer data = new();
            data.Add(DataTransferItem.CreateFile(file));

            bool wasDropEnabled = DragDrop.GetAllowDrop(this);
            DragDrop.SetAllowDrop(this, false);

            try
            {
                await DragDrop.DoDragDropAsync(trigger, data, DragDropEffects.Copy);
            }
            finally
            {
                DragDrop.SetAllowDrop(this, wasDropEnabled);
            }
        }
        catch (Exception ex)
        {
            DebugHelper.WriteException(ex);
        }
    }

    private void OnThumbnailPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is not Control control || control.DataContext is not ThumbnailItemViewModel item)
        {
            return;
        }

        PointerUpdateKind pointerUpdateKind = e.GetCurrentPoint(control).Properties.PointerUpdateKind;

        if (pointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
        {
            bool wasDragging = _thumbnailDragStarted;
            ResetThumbnailDrag();

            if (!wasDragging && Program.Settings.ThumbnailClickAction != ThumbnailViewClickAction.Select)
            {
                ExecuteThumbnailClick(item, Program.Settings.ThumbnailClickAction);
            }

            e.Handled = true;
            return;
        }

        if (pointerUpdateKind != PointerUpdateKind.RightButtonReleased)
        {
            return;
        }

        ContextMenu contextMenu = BuildTaskContextMenu();
        TryOpenContextMenu(contextMenu, control, PlacementMode.Pointer);
        e.Handled = true;
    }

    private void OnThumbnailPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (!_thumbnailDragStarted)
        {
            ResetThumbnailDrag();
        }
    }

    private void ResetThumbnailDrag()
    {
        _thumbnailDragVersion++;
        _thumbnailDragTrigger = null;
        _thumbnailDragFileTask = null;
        _thumbnailDragStarted = false;

        IPointer? pointer = _thumbnailDragPointer;
        _thumbnailDragPointer = null;
        pointer?.Capture(null);
    }

    private void OnThumbnailTitlePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control control &&
            e.GetCurrentPoint(control).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
        {
            e.Handled = true;
        }
    }

    private void OnThumbnailTitlePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is not Control { DataContext: ThumbnailItemViewModel item } control ||
            e.GetCurrentPoint(control).Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonReleased)
        {
            return;
        }

        TaskInfo? info = item.Task.Info;
        string? url = info?.Result?.ToString();

        if (!string.IsNullOrEmpty(url))
        {
            URLHelpers.OpenURL(url);
        }
        else if (!string.IsNullOrEmpty(info?.FilePath))
        {
            FileHelpers.OpenFile(info.FilePath);
        }

        e.Handled = true;
    }

    private void OnThumbnailDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Control { DataContext: ThumbnailItemViewModel item } &&
            Program.Settings.ThumbnailClickAction == ThumbnailViewClickAction.Select)
        {
            ExecuteThumbnailClick(item, ThumbnailViewClickAction.OpenFile);
            e.Handled = true;
        }
    }

    private void ExecuteThumbnailClick(ThumbnailItemViewModel item, ThumbnailViewClickAction action)
    {
        TaskInfo? info = item.Task.Info;
        if (info == null)
        {
            return;
        }

        string filePath = info.FilePath;
        switch (action)
        {
            case ThumbnailViewClickAction.Default:
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    if (FileHelpers.IsImageFile(filePath))
                    {
                        string[] images = ThumbnailItems.Select(x => x.Task.Info?.FilePath)
                            .Where(x => !string.IsNullOrEmpty(x) && File.Exists(x) && FileHelpers.IsImageFile(x))
                            .Cast<string>().ToArray();
                        ImageViewer.ShowImage(images, Math.Max(0, Array.IndexOf(images, filePath)));
                    }
                    else
                    {
                        FileHelpers.OpenFile(filePath);
                    }
                }
                break;
            case ThumbnailViewClickAction.OpenImageViewer:
                if (File.Exists(filePath) && FileHelpers.IsImageFile(filePath))
                {
                    ImageViewer.ShowImage(filePath);
                }
                break;
            case ThumbnailViewClickAction.OpenFile:
                if (!string.IsNullOrEmpty(filePath)) FileHelpers.OpenFile(filePath);
                break;
            case ThumbnailViewClickAction.OpenFolder:
                if (!string.IsNullOrEmpty(filePath)) FileHelpers.OpenFolderWithFile(filePath);
                break;
            case ThumbnailViewClickAction.OpenURL:
                URLHelpers.OpenURL(info.Result?.ToString());
                break;
            case ThumbnailViewClickAction.EditImage:
                if (File.Exists(filePath) && FileHelpers.IsImageFile(filePath)) TaskHelpers.AnnotateImageFromFile(filePath);
                break;
        }
    }

    private ContextMenu BuildTaskContextMenu()
    {
        ThumbnailItemViewModel[] selectedModels = GetSelectedItems().ToArray();
        WorkerTask[] selectedTasks = selectedModels.Select(x => x.Task).ToArray();
        _uploadInfoManager.UpdateSelectedItems(selectedTasks);

        UploadInfoStatus? selected = _uploadInfoManager.SelectedItem;
        UploadInfoStatus[] statuses = _uploadInfoManager.SelectedItems ?? Array.Empty<UploadInfoStatus>();
        bool hasSelection = selected != null;
        bool isWorking = selectedTasks.Any(x => x.IsWorking);

        List<MainMenuEntry> entries = new()
        {
            Item("Show errors", LucideIcons.triangle_alert, _uploadInfoManager.ShowErrors,
                hasSelection && !isWorking && selected!.Info.Result.IsError),
            Item("Stop upload", LucideIcons.circle_stop, _uploadInfoManager.StopUpload, isWorking),
            Submenu("Open", LucideIcons.external_link, () => BuildOpenTaskMenu(selected, statuses), hasSelection),
            Submenu("Copy", LucideIcons.copy, () => BuildCopyTaskMenu(selected, statuses), hasSelection && !isWorking),
            Item("Upload selected file", LucideIcons.file_up, _uploadInfoManager.Upload,
                !SystemOptions.DisableUpload && hasSelection && !isWorking && selected!.IsFileExist,
                new KeyGesture(Key.U, KeyModifiers.Control)),
            Item("Download selected URL", LucideIcons.download, _uploadInfoManager.Download,
                hasSelection && !isWorking && selected!.IsFileURL,
                new KeyGesture(Key.D, KeyModifiers.Control)),
            Item("Edit image", LucideIcons.image, _uploadInfoManager.EditImage,
                hasSelection && !isWorking && selected!.IsImageFile,
                new KeyGesture(Key.E, KeyModifiers.Control)),
            Item("Beautify image", LucideIcons.sparkles, _uploadInfoManager.BeautifyImage,
                hasSelection && !isWorking && selected!.IsImageFile),
            Item("Add image effects", LucideIcons.wand_sparkles, _uploadInfoManager.AddImageEffects,
                hasSelection && !isWorking && selected!.IsImageFile),
            Item("Pin to screen", LucideIcons.pin, _uploadInfoManager.PinToScreen,
                hasSelection && !isWorking && selected!.IsImageFile,
                new KeyGesture(Key.P, KeyModifiers.Control)),
            Submenu("Run action", LucideIcons.play, () => BuildExternalActionsMenu(selected),
                hasSelection && !isWorking && HasExternalActions(selected!.Info.FilePath)),
            Item("Delete selected item", LucideIcons.trash_2, RemoveSelectedTasks, hasSelection && !isWorking,
                new KeyGesture(Key.Delete)),
            Item("Delete selected file", LucideIcons.file_x, DeleteSelectedFiles,
                hasSelection && !isWorking && selected!.IsFileExist,
                new KeyGesture(Key.Delete, KeyModifiers.Shift)),
            Submenu("Shorten selected URL", LucideIcons.link_2, BuildUrlShortenerMenu,
                !SystemOptions.DisableUpload && hasSelection && !isWorking && selected!.IsURLExist),
            Submenu("Share selected URL", LucideIcons.globe_2, BuildUrlSharingMenu,
                !SystemOptions.DisableUpload && hasSelection && !isWorking && selected!.IsURLExist),
            Item("Analyze image", LucideIcons.bot, _uploadInfoManager.AnalyzeImage,
                hasSelection && !isWorking && selected!.IsImageFile),
            Item("Search with Google Lens", LucideIcons.search, _uploadInfoManager.SearchImageUsingGoogleLens,
                hasSelection && !isWorking && selected!.IsURLExist),
            Item("Search with Bing Visual Search", LucideIcons.scan_search, _uploadInfoManager.SearchImageUsingBing,
                hasSelection && !isWorking && selected!.IsURLExist),
            Item("Show QR code", LucideIcons.qr_code, _uploadInfoManager.ShowQRCode,
                hasSelection && !isWorking && selected!.IsURLExist),
            Item("OCR image", LucideIcons.scan_text, async () => await _uploadInfoManager.OCRImage(),
                hasSelection && !isWorking && selected!.IsImageFile),
            Submenu("Combine images", LucideIcons.combine, BuildCombineImagesMenu,
                hasSelection && !isWorking && statuses.Count(x => x.IsImageFile) > 1),
            Item("Show response", LucideIcons.file_text, _uploadInfoManager.ShowResponse,
                hasSelection && !isWorking && !string.IsNullOrEmpty(selected!.Info.Result.Response)),
            MainMenuEntry.Separator(),
            Item("Clear thumbnail view", LucideIcons.list_x, ClearTasks, ThumbnailItems.Count > 0)
        };

        return BuildContextMenu(entries);
    }

    private IReadOnlyList<MainMenuEntry> BuildOpenTaskMenu(UploadInfoStatus? selected, UploadInfoStatus[] statuses)
    {
        return new List<MainMenuEntry>
        {
            Item("URL", LucideIcons.link, _uploadInfoManager.OpenURL, selected?.IsURLExist == true,
                new KeyGesture(Key.Enter)),
            Item("Shortened URL", LucideIcons.link_2, _uploadInfoManager.OpenShortenedURL, selected?.IsShortenedURLExist == true),
            Item("Thumbnail URL", LucideIcons.image, _uploadInfoManager.OpenThumbnailURL, selected?.IsThumbnailURLExist == true),
            Item("Deletion URL", LucideIcons.trash_2, _uploadInfoManager.OpenDeletionURL, selected?.IsDeletionURLExist == true),
            MainMenuEntry.Separator(),
            Item("File", LucideIcons.file, _uploadInfoManager.OpenFile, selected?.IsFileExist == true,
                new KeyGesture(Key.Enter, KeyModifiers.Control)),
            Item("Folder", LucideIcons.folder_open, _uploadInfoManager.OpenFolder, selected?.IsFileExist == true,
                new KeyGesture(Key.Enter, KeyModifiers.Shift)),
            Item("Thumbnail file", LucideIcons.file_image, _uploadInfoManager.OpenThumbnailFile, selected?.IsThumbnailFileExist == true)
        };
    }

    private IReadOnlyList<MainMenuEntry> BuildCopyTaskMenu(UploadInfoStatus? selected, UploadInfoStatus[] statuses)
    {
        List<MainMenuEntry> entries = new()
        {
            Item("URL", LucideIcons.link, _uploadInfoManager.CopyURL, statuses.Any(x => x.IsURLExist),
                new KeyGesture(Key.C, KeyModifiers.Control)),
            Item("Shortened URL", LucideIcons.link_2, _uploadInfoManager.CopyShortenedURL, statuses.Any(x => x.IsShortenedURLExist)),
            Item("Thumbnail URL", LucideIcons.image, _uploadInfoManager.CopyThumbnailURL, statuses.Any(x => x.IsThumbnailURLExist)),
            Item("Deletion URL", LucideIcons.trash_2, _uploadInfoManager.CopyDeletionURL, statuses.Any(x => x.IsDeletionURLExist)),
            MainMenuEntry.Separator(),
            Item("File", LucideIcons.file, _uploadInfoManager.CopyFile, selected?.IsFileExist == true,
                new KeyGesture(Key.C, KeyModifiers.Shift)),
            Item("Image", LucideIcons.image, _uploadInfoManager.CopyImage, selected?.IsImageFile == true,
                new KeyGesture(Key.C, KeyModifiers.Alt)),
            Item("Image dimensions", LucideIcons.ruler, _uploadInfoManager.CopyImageDimensions, selected?.IsImageFile == true),
            Item("Text", LucideIcons.file_text, _uploadInfoManager.CopyText, selected?.IsTextFile == true),
            Item("Thumbnail file", LucideIcons.file_image, _uploadInfoManager.CopyThumbnailFile, selected?.IsThumbnailFileExist == true),
            Item("Thumbnail image", LucideIcons.images, _uploadInfoManager.CopyThumbnailImage, selected?.IsThumbnailFileExist == true),
            MainMenuEntry.Separator(),
            Item("HTML link", LucideIcons.code, _uploadInfoManager.CopyHTMLLink, statuses.Any(x => x.IsURLExist)),
            Item("HTML image", LucideIcons.file_code, _uploadInfoManager.CopyHTMLImage, statuses.Any(x => x.IsImageURL)),
            Item("HTML linked image", LucideIcons.braces, _uploadInfoManager.CopyHTMLLinkedImage, statuses.Any(x => x.IsImageURL && x.IsThumbnailURLExist)),
            Item("Forum link", LucideIcons.message_square, _uploadInfoManager.CopyForumLink, statuses.Any(x => x.IsURLExist)),
            Item("Forum image", LucideIcons.messages_square, _uploadInfoManager.CopyForumImage, statuses.Any(x => x.IsImageURL && x.IsURLExist)),
            Item("Forum linked image", LucideIcons.message_square_share, _uploadInfoManager.CopyForumLinkedImage, statuses.Any(x => x.IsImageURL && x.IsThumbnailURLExist)),
            Item("Markdown link", LucideIcons.link, _uploadInfoManager.CopyMarkdownLink, statuses.Any(x => x.IsURLExist)),
            Item("Markdown image", LucideIcons.image, _uploadInfoManager.CopyMarkdownImage, statuses.Any(x => x.IsImageURL)),
            Item("Markdown linked image", LucideIcons.images, _uploadInfoManager.CopyMarkdownLinkedImage, statuses.Any(x => x.IsImageURL && x.IsThumbnailURLExist)),
            MainMenuEntry.Separator(),
            Item("File path", LucideIcons.route, _uploadInfoManager.CopyFilePath, statuses.Any(x => x.IsFilePathValid),
                new KeyGesture(Key.C, KeyModifiers.Control | KeyModifiers.Shift)),
            Item("File name", LucideIcons.file, _uploadInfoManager.CopyFileName, statuses.Any(x => x.IsFilePathValid)),
            Item("File name with extension", LucideIcons.files, _uploadInfoManager.CopyFileNameWithExtension, statuses.Any(x => x.IsFilePathValid)),
            Item("Folder", LucideIcons.folder, _uploadInfoManager.CopyFolder, statuses.Any(x => x.IsFilePathValid))
        };

        if (Program.Settings.ClipboardContentFormats?.Count > 0)
        {
            entries.Add(MainMenuEntry.Separator());
            foreach (ClipboardFormat format in Program.Settings.ClipboardContentFormats)
            {
                ClipboardFormat selectedFormat = format;
                entries.Add(Item(selectedFormat.Description, LucideIcons.clipboard_copy,
                    () => _uploadInfoManager.CopyCustomFormat(selectedFormat.Format)));
            }
        }

        return entries;
    }

    private static bool HasExternalActions(string filePath) =>
        !string.IsNullOrEmpty(filePath) && File.Exists(filePath) &&
        Program.DefaultTaskSettings.ExternalPrograms.Any(x => !string.IsNullOrEmpty(x.Name) && x.CheckExtension(filePath));

    private static IReadOnlyList<MainMenuEntry> BuildExternalActionsMenu(UploadInfoStatus? selected)
    {
        string filePath = selected?.Info.FilePath ?? string.Empty;
        return Program.DefaultTaskSettings.ExternalPrograms
            .Where(x => !string.IsNullOrEmpty(x.Name) && x.CheckExtension(filePath))
            .Select(action => Item(action.Name.Truncate(50, "..."), LucideIcons.play,
                async () => await action.RunAsync(filePath))).ToArray();
    }

    private IReadOnlyList<MainMenuEntry> BuildUrlShortenerMenu() =>
        Helpers.GetEnums<UrlShortenerType>().Select(value => Item(value.GetLocalizedDescription(), LucideIcons.link_2,
            () => _uploadInfoManager.ShortenURL(value))).ToArray();

    private IReadOnlyList<MainMenuEntry> BuildUrlSharingMenu() =>
        Helpers.GetEnums<URLSharingServices>().Select(value => Item(value.GetLocalizedDescription(), LucideIcons.globe_2,
            () => _uploadInfoManager.ShareURL(value))).ToArray();

    private IReadOnlyList<MainMenuEntry> BuildCombineImagesMenu() => new List<MainMenuEntry>
    {
        Item("Horizontally", LucideIcons.rows_2, () => _uploadInfoManager.CombineImages(FormsOrientation.Horizontal)),
        Item("Vertically", LucideIcons.columns_2, () => _uploadInfoManager.CombineImages(FormsOrientation.Vertical))
    };

    private void RemoveSelectedTasks()
    {
        foreach (WorkerTask task in GetSelectedItems().Select(x => x.Task).ToArray())
        {
            TaskManager.Remove(task);
        }
    }

    private void DeleteSelectedFiles()
    {
        if (FormsMessageBox.Show(AppResources.MainForm_tsmiDeleteSelectedFile_Click_Do_you_really_want_to_delete_this_file_,
            "ShareX - " + AppResources.MainForm_tsmiDeleteSelectedFile_Click_File_delete_confirmation,
            FormsMessageBoxButtons.YesNo) == FormsDialogResult.Yes)
        {
            _uploadInfoManager.DeleteFiles();
            RemoveSelectedTasks();
        }
    }

    private void ClearTasks()
    {
        foreach (WorkerTask task in TaskManager.Tasks.ToArray())
        {
            TaskManager.Remove(task);
        }

        TaskManager.RecentManager.Clear();
    }

    private void OnMainContentPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        PointerUpdateKind pointerUpdateKind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;

        if (pointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
        {
            foreach (ThumbnailItemViewModel item in ThumbnailItems)
            {
                item.IsSelected = false;
            }

            _lastSelectedIndex = -1;
            e.Handled = true;
            return;
        }

        if (pointerUpdateKind != PointerUpdateKind.RightButtonPressed)
        {
            return;
        }

        CloseActiveContextMenu();
        e.Handled = true;
    }

    private void OnMainContentPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind != PointerUpdateKind.RightButtonReleased)
        {
            return;
        }

        if (ThumbnailItems.Count == 0)
        {
            return;
        }

        if (sender is Control control)
        {
            ContextMenu menu = BuildContextMenu([
                Item("Clear thumbnail view", LucideIcons.list_x, ClearTasks)
            ]);
            TryOpenContextMenu(menu, control, PlacementMode.Pointer);
            e.Handled = true;
        }
    }

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        _uploadInfoManager.UpdateSelectedItems(GetSelectedItems().Select(x => x.Task));
        bool control = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        bool shift = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
        bool alt = e.KeyModifiers.HasFlag(KeyModifiers.Alt);

        if (e.Key == Key.Enter && control) _uploadInfoManager.OpenFile();
        else if (e.Key == Key.Enter && shift) _uploadInfoManager.OpenFolder();
        else if (e.Key == Key.Enter) _uploadInfoManager.TryOpen();
        else if (e.Key == Key.C && control && shift) _uploadInfoManager.CopyFilePath();
        else if (e.Key == Key.C && shift) _uploadInfoManager.CopyFile();
        else if (e.Key == Key.C && alt) _uploadInfoManager.CopyImage();
        else if (e.Key == Key.C && control) _uploadInfoManager.TryCopy();
        else if (e.Key == Key.V && control) UploadManager.ClipboardUploadMainWindow();
        else if (e.Key == Key.U && control) _uploadInfoManager.Upload();
        else if (e.Key == Key.D && control) _uploadInfoManager.Download();
        else if (e.Key == Key.E && control) _uploadInfoManager.EditImage();
        else if (e.Key == Key.P && control) _uploadInfoManager.PinToScreen();
        else if (e.Key == Key.Delete && shift) DeleteSelectedFiles();
        else if (e.Key == Key.Delete) RemoveSelectedTasks();
        else return;

        e.Handled = true;
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        bool supported = e.DataTransfer.TryGetFiles()?.Any() == true || !string.IsNullOrEmpty(e.DataTransfer.TryGetText());
        e.DragEffects = supported ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        FormsDataObject dataObject = new();
        string[] files = e.DataTransfer.TryGetFiles()?
            .Select(x => x.TryGetLocalPath())
            .Where(x => !string.IsNullOrEmpty(x))
            .Cast<string>().ToArray() ?? Array.Empty<string>();

        if (files.Length > 0)
        {
            dataObject.SetData(FormsDataFormats.FileDrop, files);
        }

        string? text = e.DataTransfer.TryGetText();
        if (!string.IsNullOrEmpty(text))
        {
            dataObject.SetText(text);
        }

        UploadManager.DragDropUpload(dataObject);
        e.Handled = true;
    }

    private void OnHostTrayMouseDown(object? sender, FormsMouseEventArgs e)
    {
        if (e.Button == FormsMouseButtons.Right)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (!_disposed)
                {
                    CloseActiveContextMenu();
                    CloseTrayMenuAnchor();
                }
            });
        }
    }

    private void OnHostTrayMouseUp(object? sender, FormsMouseEventArgs e)
    {
        if (e.Button == FormsMouseButtons.Right)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (!_disposed)
                {
                    ShowTrayMenu();
                }
            });
        }
    }

    private void OnTrayMenuClosed(object? sender, RoutedEventArgs e)
    {
        if (sender is ContextMenu menu)
        {
            menu.Closed -= OnTrayMenuClosed;
        }

        CloseTrayMenuAnchor();
        SettingManager.SaveAllSettingsAsync();
    }

    private void OnTrayMenuAnchorDeactivated(object? sender, EventArgs e)
    {
        if (ReferenceEquals(sender, _trayMenuAnchor))
        {
            CloseActiveContextMenu();
            CloseTrayMenuAnchor();
        }
    }

    private void CloseTrayMenuAnchor()
    {
        Window? anchor = _trayMenuAnchor;
        _trayMenuAnchor = null;

        if (anchor != null)
        {
            anchor.Deactivated -= OnTrayMenuAnchorDeactivated;

            if (anchor.IsVisible)
            {
                anchor.Close();
            }
        }
    }

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (!_allowClose && Program.Settings.ShowTray)
        {
            e.Cancel = true;
            HideToTray();
            SettingManager.SaveAllSettingsAsync();

            if (Program.Settings.FirstTimeMinimizeToTray)
            {
                TaskHelpers.ShowNotificationTip(AppResources.ShareXIsMinimizedToTheSystemTray, "ShareX", 8000);
                Program.Settings.FirstTimeMinimizeToTray = false;
            }

            return;
        }

        if (!_allowClose)
        {
            e.Cancel = true;
            Dispatcher.UIThread.Post(_host.ForceClose);
            return;
        }

        DisposeWindowResources();
    }

    private void DisposeWindowResources()
    {
        if (_disposed)
        {
            return;
        }

        SaveWindowBounds();
        _disposed = true;
        TaskManager.TaskAdded -= OnTaskAdded;
        TaskManager.TaskRemoved -= OnTaskRemoved;
        TaskManager.TaskChanged -= OnTaskChanged;
        TaskManager.TaskImageReady -= OnTaskImageReady;
        TaskManager.TaskCollectionChanged -= OnTaskCollectionChanged;
        ThemeManager.ThemeChanged -= OnThemeChanged;
        ResetThumbnailDrag();
        CloseActiveContextMenu();
        CloseTrayMenuAnchor();
        _host.niTray.MouseDown -= OnHostTrayMouseDown;
        _host.niTray.MouseUp -= OnHostTrayMouseUp;
        _host.niTray.Visible = false;
        _host.niTray.Icon = null;
        _ownedTrayIcon?.Dispose();
        _ownedTrayIcon = null;

        foreach (ThumbnailItemViewModel item in ThumbnailItems)
        {
            item.Dispose();
        }
    }

    private void RestoreWindowBounds()
    {
        DrawingSize savedSize = Program.Settings.MainFormSize;
        if (Program.Settings.RememberMainFormSize && !savedSize.IsEmpty)
        {
            Width = Math.Max(MinWidth, savedSize.Width);
            Height = Math.Max(MinHeight, savedSize.Height);
        }

        DrawingPoint savedPosition = Program.Settings.MainFormPosition;
        if (Program.Settings.RememberMainFormPosition && !savedPosition.IsEmpty)
        {
            Position = new PixelPoint(savedPosition.X, savedPosition.Y);
            WindowStartupLocation = WindowStartupLocation.Manual;
        }
    }

    private void SaveWindowBounds()
    {
        if (_disposed || WindowState != Avalonia.Controls.WindowState.Normal)
        {
            return;
        }

        Program.Settings.MainFormPosition = new DrawingPoint(Position.X, Position.Y);
        Program.Settings.MainFormSize = new DrawingSize((int)Math.Round(ClientSize.Width), (int)Math.Round(ClientSize.Height));
    }

    private static WindowIcon CreateWindowIcon()
    {
        using MemoryStream stream = new();
        ShareXResources.Icon.Save(stream);
        stream.Position = 0;
        return new WindowIcon(stream);
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private static MainMenuEntry Item(string header, string icon, Action execute, bool visible = true,
        KeyGesture? inputGesture = null) =>
        new(header, icon, execute, isVisible: visible, inputGesture: inputGesture);

    private static MainMenuEntry Item(string header, string icon, Func<Task> execute, bool visible = true,
        KeyGesture? inputGesture = null) =>
        new(header, icon, execute, isVisible: visible, inputGesture: inputGesture);

    private static MainMenuEntry Submenu(string header, string icon, Func<IReadOnlyList<MainMenuEntry>> children, bool visible = true) =>
        new(header, icon, createChildren: children, isVisible: visible);
}
