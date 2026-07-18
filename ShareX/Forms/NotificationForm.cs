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
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using DrawingBitmap = System.Drawing.Bitmap;
using DrawingColor = System.Drawing.Color;
using DrawingContentAlignment = System.Drawing.ContentAlignment;
using FormsMessageBox = System.Windows.Forms.MessageBox;
using FormsMessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
using FormsDialogResult = System.Windows.Forms.DialogResult;
using FormsCursor = System.Windows.Forms.Cursor;
using AppResources = ShareX.Properties.Resources;

namespace ShareX;

public partial class NotificationForm : Window
{
    private const double ShadowMargin = 14;
    private const double MinimumTextWidth = 240;
    private const double MaximumTextWidth = 520;
    private static NotificationForm? _instance;

    private readonly DispatcherTimer _durationTimer;
    private readonly DispatcherTimer _fadeTimer;
    private readonly DispatcherTimer _hoverTimer;
    private readonly Stopwatch _fadeStopwatch = new();
    private NotificationFormConfig? _config;
    private Bitmap? _previewBitmap;
    private bool _durationEnded;
    private bool _pointerInside;
    private bool _dragStarted;
    private Point _dragStart;
    private PointerPressedEventArgs? _dragEvent;

    public NotificationFormConfig? Config => _config;

    public NotificationForm()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        _durationTimer = new DispatcherTimer();
        _durationTimer.Tick += OnDurationElapsed;
        _fadeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        _fadeTimer.Tick += OnFadeTick;
        _hoverTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(40) };
        _hoverTimer.Tick += OnHoverTick;

        Opened += OnOpened;
        Closed += OnClosed;
    }

    public static void Show(NotificationFormConfig config)
    {
        if (config == null || !config.IsValid)
        {
            config?.Dispose();
            return;
        }

        if (config.Image == null)
        {
            config.Image = ImageHelpers.LoadImage(config.FilePath);
        }

        if (config.Image == null && string.IsNullOrEmpty(config.Text))
        {
            config.Dispose();
            return;
        }

        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() =>
        {
            if (_instance == null)
            {
                _instance = new NotificationForm();
                _instance.LoadConfig(config);
                _instance.Show();
            }
            else
            {
                _instance.LoadConfig(config);

                if (!_instance.IsVisible)
                {
                    _instance.Show();
                }
            }
        });
    }

    public static void CloseActiveForm()
    {
        if (Application.Current == null)
        {
            return;
        }

        Dispatcher.UIThread.Post(() => _instance?.Close());
    }

    public void LoadConfig(NotificationFormConfig config)
    {
        _durationTimer.Stop();
        _fadeTimer.Stop();
        _fadeStopwatch.Reset();
        _durationEnded = false;
        _pointerInside = false;
        _dragStarted = false;
        Opacity = 1;

        _previewBitmap?.Dispose();
        _previewBitmap = null;
        _config?.Dispose();
        _config = config;

        LoadPreview(config);
        ApplyContent(config);
        BuildActionButtons(config);
        UpdateHoverState(false);

        if (IsVisible)
        {
            Dispatcher.UIThread.Post(PositionWindow, DispatcherPriority.Loaded);
        }

        if (config.Duration <= 0)
        {
            EndDuration();
        }
        else
        {
            _durationTimer.Interval = TimeSpan.FromMilliseconds(config.Duration);
            _durationTimer.Start();
        }
    }

    private void LoadPreview(NotificationFormConfig config)
    {
        DrawingBitmap? source = config.Image;

        if (source == null && !string.IsNullOrEmpty(config.FilePath))
        {
            source = ImageHelpers.LoadImage(config.FilePath);
            config.Image = source;
        }

        if (source == null)
        {
            PreviewImage.Source = null;
            PreviewImage.Clip = null;
            PreviewImage.IsVisible = false;
            return;
        }

        using MemoryStream stream = new();
        source.Save(stream, ImageFormat.Png);
        stream.Position = 0;
        _previewBitmap = new Bitmap(stream);
        PreviewImage.Source = _previewBitmap;
        PreviewImage.IsVisible = true;

        double maxWidth = Math.Max(1, config.Size.Width);
        double maxHeight = Math.Max(1, config.Size.Height);
        double scale = Math.Min(1, Math.Min(maxWidth / source.Width, maxHeight / source.Height));
        PreviewImage.Width = Math.Max(1, Math.Round(source.Width * scale));
        PreviewImage.Height = Math.Max(1, Math.Round(source.Height * scale));
        PreviewImage.Clip = new RectangleGeometry(
            new Rect(0, 0, PreviewImage.Width, PreviewImage.Height), 3, 3);
    }

    private void ApplyContent(NotificationFormConfig config)
    {
        bool hasImage = _previewBitmap != null;
        bool hasTitle = !string.IsNullOrWhiteSpace(config.Title);
        bool hasText = !string.IsNullOrWhiteSpace(config.Text);
        bool hasCaption = hasImage && (hasTitle || hasText);

        TextContent.IsVisible = !hasImage;
        ImageCaption.IsVisible = hasCaption;
        TitleText.Text = config.Title ?? string.Empty;
        BodyText.Text = config.Text ?? string.Empty;
        ImageTitleText.Text = config.Title ?? string.Empty;
        ImageBodyText.Text = config.Text ?? string.Empty;
        TitleText.IsVisible = hasTitle;
        BodyText.IsVisible = hasText;
        ImageTitleText.IsVisible = hasTitle;
        ImageBodyText.IsVisible = hasText;

        NotificationCard.Background = hasImage
            ? Brushes.Transparent
            : new SolidColorBrush(ToAvaloniaColor(config.BackgroundColor));
        TitleText.Foreground = new SolidColorBrush(ToAvaloniaColor(config.TitleColor));
        BodyText.Foreground = new SolidColorBrush(ToAvaloniaColor(config.TextColor));

        if (!hasImage)
        {
            double width = Math.Clamp(config.Size.Width, MinimumTextWidth, MaximumTextWidth);
            NotificationCard.Width = width;
            NotificationCard.Height = double.NaN;
            TextContent.MaxHeight = Math.Max(96, config.Size.Height);
        }
        else
        {
            // Overlay content must not participate in the notification's desired
            // width. Keep image notifications anchored to the preview and let the
            // caption wrap/trim inside that fixed surface.
            NotificationCard.Width = PreviewImage.Width + 2;
            NotificationCard.Height = double.NaN;
        }
    }

    private void BuildActionButtons(NotificationFormConfig config)
    {
        ActionButtons.Children.Clear();

        foreach (NotificationActionButton definition in config.ActionButtons ?? [])
        {
            if (definition == null || !CanExecute(definition.Action, config))
            {
                continue;
            }

            (string defaultLabel, string defaultIcon) = GetActionPresentation(definition.Action);
            string label = string.IsNullOrWhiteSpace(definition.Label) ? defaultLabel : definition.Label;
            string icon = string.IsNullOrWhiteSpace(definition.Icon) ? defaultIcon : definition.Icon;

            TextBlock iconText = new()
            {
                Text = icon,
                FontFamily = (FontFamily)Application.Current!.FindResource("ShareX.FontFamily.Icon")!,
                FontSize = 16,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            Button button = new()
            {
                Content = iconText,
                Tag = definition,
                Classes = { "notification-action" }
            };
            ToolTip.SetTip(button, label);
            ToolTip.SetPlacement(button, PlacementMode.Top);
            ToolTip.SetVerticalOffset(button, -4);
            ToolTip.SetShowDelay(button, 400);
            ToolTip.SetBetweenShowDelay(button, 100);
            button.PointerPressed += OnActionButtonPointerPressed;
            button.Click += OnActionButtonClick;
            ActionButtons.Children.Add(button);
        }

        bool hasActions = ActionButtons.Children.Count > 0;
        ActionsPanel.IsVisible = hasActions;
        TextContent.Margin = hasActions ? new Thickness(18, 16, 18, 54) : new Thickness(18, 16);
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        _hoverTimer.Start();
        Dispatcher.UIThread.Post(PositionWindow, DispatcherPriority.Loaded);
        Dispatcher.UIThread.Post(UpdatePointerInside, DispatcherPriority.Loaded);
    }

    private void PositionWindow()
    {
        if (_config == null || Screens.Primary == null)
        {
            return;
        }

        Screen screen = Screens.Primary;
        PixelRect area = screen.WorkingArea;
        PixelSize size = PixelSize.FromSize(Bounds.Size, screen.Scaling);
        int offset = Math.Max(0, _config.Offset) - (int)Math.Round(ShadowMargin * screen.Scaling);
        int x = GetHorizontalPosition(_config.Placement, area, size.Width, offset);
        int y = GetVerticalPosition(_config.Placement, area, size.Height, offset);
        Position = new PixelPoint(x, y);
    }

    private static int GetHorizontalPosition(DrawingContentAlignment placement, PixelRect area, int width, int offset) => placement switch
    {
        DrawingContentAlignment.TopLeft or DrawingContentAlignment.MiddleLeft or DrawingContentAlignment.BottomLeft => area.X + offset,
        DrawingContentAlignment.TopCenter or DrawingContentAlignment.MiddleCenter or DrawingContentAlignment.BottomCenter => area.X + (area.Width - width) / 2,
        _ => area.Right - width - offset
    };

    private static int GetVerticalPosition(DrawingContentAlignment placement, PixelRect area, int height, int offset) => placement switch
    {
        DrawingContentAlignment.TopLeft or DrawingContentAlignment.TopCenter or DrawingContentAlignment.TopRight => area.Y + offset,
        DrawingContentAlignment.MiddleLeft or DrawingContentAlignment.MiddleCenter or DrawingContentAlignment.MiddleRight => area.Y + (area.Height - height) / 2,
        _ => area.Bottom - height - offset
    };

    private void OnDurationElapsed(object? sender, EventArgs e) => EndDuration();

    private void EndDuration()
    {
        _durationEnded = true;
        _durationTimer.Stop();

        if (!_pointerInside)
        {
            StartFade();
        }
    }

    private void StartFade()
    {
        if (_config == null)
        {
            return;
        }

        if (_config.FadeDuration <= 0)
        {
            Close();
            return;
        }

        _fadeStopwatch.Restart();
        _fadeTimer.Start();
    }

    private void OnFadeTick(object? sender, EventArgs e)
    {
        if (_config == null)
        {
            return;
        }

        double progress = _fadeStopwatch.Elapsed.TotalMilliseconds / _config.FadeDuration;

        if (progress >= 1)
        {
            Close();
        }
        else
        {
            Opacity = 1 - progress;
        }
    }

    private void OnHoverTick(object? sender, EventArgs e) => UpdatePointerInside();

    private void UpdatePointerInside()
    {
        if (!IsVisible || NotificationCard.Bounds.Width <= 0 || NotificationCard.Bounds.Height <= 0)
        {
            return;
        }

        PixelPoint topLeft = NotificationCard.PointToScreen(default);
        PixelPoint bottomRight = NotificationCard.PointToScreen(
            new Point(NotificationCard.Bounds.Width, NotificationCard.Bounds.Height));
        System.Drawing.Point cursor = FormsCursor.Position;
        bool isInside = cursor.X >= topLeft.X && cursor.X < bottomRight.X &&
            cursor.Y >= topLeft.Y && cursor.Y < bottomRight.Y;

        if (isInside == _pointerInside)
        {
            return;
        }

        _pointerInside = isInside;
        UpdateHoverState(isInside);

        if (isInside)
        {
            _fadeTimer.Stop();
            _fadeStopwatch.Reset();
            Opacity = 1;
        }
        else
        {
            _dragStarted = false;
            _dragEvent = null;

            if (_durationEnded)
            {
                StartFade();
            }
        }
    }

    private void UpdateHoverState(bool isHovered)
    {
        ActionsPanel.Opacity = isHovered ? 1 : 0;
        ActionsPanel.IsHitTestVisible = isHovered;
        ImageCaption.Opacity = isHovered ? 1 : 0;
    }

    private void OnCardPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        PointerPoint point = e.GetCurrentPoint(NotificationCard);
        _dragStart = point.Position;
        _dragStarted = point.Properties.IsLeftButtonPressed;
        _dragEvent = _dragStarted ? e : null;
    }

    private async void OnCardPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_dragStarted || _dragEvent == null || _config == null ||
            string.IsNullOrEmpty(_config.FilePath) || !File.Exists(_config.FilePath))
        {
            return;
        }

        Point current = e.GetPosition(NotificationCard);
        if (Math.Abs(current.X - _dragStart.X) < 20 && Math.Abs(current.Y - _dragStart.Y) < 20)
        {
            return;
        }

        _dragStarted = false;
        IStorageFile? file = await StorageProvider.TryGetFileFromPathAsync(_config.FilePath);
        if (file == null)
        {
            return;
        }

        DataTransfer data = new();
        data.Add(DataTransferItem.CreateFile(file));
        await DragDrop.DoDragDropAsync(_dragEvent, data, DragDropEffects.Copy | DragDropEffects.Move);
        _dragEvent = null;
    }

    private void OnCardPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        bool wasDragging = _dragEvent == null && !_dragStarted;
        _dragStarted = false;
        _dragEvent = null;

        if (wasDragging || _config == null)
        {
            return;
        }

        PointerPoint point = e.GetCurrentPoint(NotificationCard);
        ToastClickAction action = point.Properties.PointerUpdateKind switch
        {
            PointerUpdateKind.LeftButtonReleased => _config.LeftClickAction,
            PointerUpdateKind.RightButtonReleased => _config.RightClickAction,
            PointerUpdateKind.MiddleButtonReleased => _config.MiddleClickAction,
            _ => ToastClickAction.CloseNotification
        };

        NotificationFormConfig config = _config;
        Close();
        ExecuteAction(action, config);
        e.Handled = true;
    }

    private void OnActionButtonClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: NotificationActionButton definition } || _config == null)
        {
            return;
        }

        e.Handled = true;
        NotificationFormConfig config = _config;

        if (definition.DismissNotification)
        {
            Close();
        }

        ExecuteAction(definition.Action, config);
    }

    private void OnActionButtonPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _dragStarted = false;
        _dragEvent = null;
        e.Handled = true;
    }

    private static void ExecuteAction(ToastClickAction action, NotificationFormConfig config)
    {
        if (action == ToastClickAction.CloseNotification)
        {
            return;
        }

        void Execute()
        {
            switch (action)
            {
                case ToastClickAction.AnnotateImage:
                    if (!string.IsNullOrEmpty(config.FilePath) && FileHelpers.IsImageFile(config.FilePath))
                    {
                        TaskHelpers.AnnotateImageFromFile(config.FilePath);
                    }
                    break;
                case ToastClickAction.CopyImageToClipboard:
                    if (!string.IsNullOrEmpty(config.FilePath))
                    {
                        ClipboardHelpers.CopyImageFromFile(config.FilePath);
                    }
                    break;
                case ToastClickAction.CopyFile:
                    if (!string.IsNullOrEmpty(config.FilePath))
                    {
                        ClipboardHelpers.CopyFile(config.FilePath);
                    }
                    break;
                case ToastClickAction.CopyFilePath:
                    if (!string.IsNullOrEmpty(config.FilePath))
                    {
                        ClipboardHelpers.CopyText(config.FilePath);
                    }
                    break;
                case ToastClickAction.CopyUrl:
                    ClipboardHelpers.CopyText(!string.IsNullOrEmpty(config.URL) ? config.URL : config.FilePath);
                    break;
                case ToastClickAction.OpenFile:
                    FileHelpers.OpenFile(config.FilePath);
                    break;
                case ToastClickAction.OpenFolder:
                    FileHelpers.OpenFolderWithFile(config.FilePath);
                    break;
                case ToastClickAction.OpenUrl:
                    if (!string.IsNullOrEmpty(config.URL))
                    {
                        URLHelpers.OpenURL(config.URL);
                    }
                    else
                    {
                        FileHelpers.OpenFile(config.FilePath);
                    }
                    break;
                case ToastClickAction.Upload:
                    UploadManager.UploadFile(config.FilePath);
                    break;
                case ToastClickAction.PinToScreen:
                    TaskHelpers.PinToScreen(config.FilePath);
                    break;
                case ToastClickAction.DeleteFile:
                    if (FormsMessageBox.Show(AppResources.MainForm_tsmiDeleteSelectedFile_Click_Do_you_really_want_to_delete_this_file_,
                        "ShareX - " + AppResources.MainForm_tsmiDeleteSelectedFile_Click_File_delete_confirmation,
                        FormsMessageBoxButtons.YesNo) == FormsDialogResult.Yes)
                    {
                        FileHelpers.DeleteFile(config.FilePath, true);
                    }
                    break;
            }
        }

        if (Program.MainForm != null && Program.MainForm.InvokeRequired)
        {
            Program.MainForm.BeginInvoke((Action)Execute);
        }
        else
        {
            Execute();
        }
    }

    private static bool CanExecute(ToastClickAction action, NotificationFormConfig config)
    {
        bool hasFile = !string.IsNullOrWhiteSpace(config.FilePath);
        bool hasImageFile = hasFile && FileHelpers.IsImageFile(config.FilePath);
        bool hasTarget = hasFile || !string.IsNullOrWhiteSpace(config.URL);

        return action switch
        {
            ToastClickAction.AnnotateImage or ToastClickAction.CopyImageToClipboard or ToastClickAction.PinToScreen => hasImageFile,
            ToastClickAction.CopyFile or ToastClickAction.CopyFilePath or ToastClickAction.OpenFile or
                ToastClickAction.OpenFolder or ToastClickAction.Upload or ToastClickAction.DeleteFile => hasFile,
            ToastClickAction.CopyUrl or ToastClickAction.OpenUrl => hasTarget,
            ToastClickAction.CloseNotification => true,
            _ => false
        };
    }

    private static (string Label, string Icon) GetActionPresentation(ToastClickAction action) => action switch
    {
        ToastClickAction.AnnotateImage => ("Edit", LucideIcons.pen_line),
        ToastClickAction.CopyImageToClipboard => ("Copy image", LucideIcons.copy),
        ToastClickAction.CopyFile => ("Copy file", LucideIcons.files),
        ToastClickAction.CopyFilePath => ("Copy path", LucideIcons.clipboard),
        ToastClickAction.CopyUrl => ("Copy link", LucideIcons.link),
        ToastClickAction.OpenFile => ("Open", LucideIcons.external_link),
        ToastClickAction.OpenFolder => ("Folder", LucideIcons.folder_open),
        ToastClickAction.OpenUrl => ("Open link", LucideIcons.external_link),
        ToastClickAction.Upload => ("Upload", LucideIcons.upload),
        ToastClickAction.PinToScreen => ("Pin", LucideIcons.pin),
        ToastClickAction.DeleteFile => ("Delete", LucideIcons.trash_2),
        _ => ("Close", LucideIcons.x)
    };

    private static Avalonia.Media.Color ToAvaloniaColor(DrawingColor color) =>
        Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B);

    private void OnClosed(object? sender, EventArgs e)
    {
        _durationTimer.Stop();
        _fadeTimer.Stop();
        _hoverTimer.Stop();
        _previewBitmap?.Dispose();
        _previewBitmap = null;
        _config?.Dispose();
        _config = null;

        if (ReferenceEquals(_instance, this))
        {
            _instance = null;
        }
    }
}
