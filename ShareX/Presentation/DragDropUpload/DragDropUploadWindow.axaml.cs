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
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using System;
using System.IO;
using System.Linq;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;
using DrawingBitmap = System.Drawing.Bitmap;
using DrawingContentAlignment = System.Drawing.ContentAlignment;

namespace ShareX;

public partial class DragDropUploadWindow : Window
{
    private readonly int _dropOffset;
    private readonly DrawingContentAlignment _dropAlignment;
    private readonly double _normalOpacity;
    private readonly double _hoverOpacity;
    private bool _isHovered;

    public TaskSettings? TaskSettings { get; set; }

    public DragDropUploadWindow() : this(150, 5, DrawingContentAlignment.BottomRight, 100, 255)
    {
    }

    public DragDropUploadWindow(
        int size,
        int offset,
        DrawingContentAlignment alignment,
        int opacity,
        int hoverOpacity)
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        int dropSize = Math.Clamp(size, 10, 300);
        Width = dropSize;
        Height = dropSize;
        _dropOffset = Math.Max(0, offset);
        _dropAlignment = alignment;
        _normalOpacity = Math.Clamp(opacity, 1, 255) / 255d;
        _hoverOpacity = Math.Clamp(hoverOpacity, 1, 255) / 255d;
        Opacity = _normalOpacity;

        DropText.Text = Properties.Resources.DropForm_DrawDropImage_Drop_here;
        DropText.FontSize = Math.Clamp(dropSize / 7d, 10, 20);
        DropText.IsVisible = dropSize >= 55;
        DropIcon.FontSize = Math.Clamp(dropSize / 3d, 16, 42);
        DropIcon.IsVisible = dropSize >= 35;

        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(PositionWindow, DispatcherPriority.Loaded);
        Activate();
    }

    private void PositionWindow()
    {
        Screen? screen = Screens.Primary;
        if (screen == null)
        {
            return;
        }

        PixelRect area = screen.WorkingArea;
        PixelSize size = PixelSize.FromSize(Bounds.Size, screen.Scaling);
        int x = GetHorizontalPosition(_dropAlignment, area, size.Width, _dropOffset);
        int y = GetVerticalPosition(_dropAlignment, area, size.Height, _dropOffset);
        Position = new PixelPoint(x, y);
    }

    private static int GetHorizontalPosition(DrawingContentAlignment alignment, PixelRect area, int width, int offset) => alignment switch
    {
        DrawingContentAlignment.TopLeft or DrawingContentAlignment.MiddleLeft or DrawingContentAlignment.BottomLeft => area.X + offset,
        DrawingContentAlignment.TopCenter or DrawingContentAlignment.MiddleCenter or DrawingContentAlignment.BottomCenter => area.X + (area.Width - width) / 2,
        _ => area.Right - width - offset
    };

    private static int GetVerticalPosition(DrawingContentAlignment alignment, PixelRect area, int height, int offset) => alignment switch
    {
        DrawingContentAlignment.TopLeft or DrawingContentAlignment.TopCenter or DrawingContentAlignment.TopRight => area.Y + offset,
        DrawingContentAlignment.MiddleLeft or DrawingContentAlignment.MiddleCenter or DrawingContentAlignment.MiddleRight => area.Y + (area.Height - height) / 2,
        _ => area.Bottom - height - offset
    };

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        PointerUpdateKind kind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
        if (kind == PointerUpdateKind.LeftButtonPressed)
        {
            BeginMoveDrag(e);
            e.Handled = true;
        }
        else if (kind == PointerUpdateKind.RightButtonPressed)
        {
            Close();
            e.Handled = true;
        }
    }

    private void OnDragEnter(object? sender, DragEventArgs e) => UpdateDragState(e);

    private void OnDragOver(object? sender, DragEventArgs e) => UpdateDragState(e);

    private void UpdateDragState(DragEventArgs e)
    {
        bool supported = HasSupportedData(e.DataTransfer);
        e.DragEffects = supported ? DragDropEffects.Copy : DragDropEffects.None;
        SetHovered(supported);
        e.Handled = true;
    }

    private void OnDragLeave(object? sender, DragEventArgs e)
    {
        SetHovered(false);
        e.Handled = true;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        try
        {
            HandleDrop(e.DataTransfer);
            e.DragEffects = DragDropEffects.Copy;
        }
        finally
        {
            SetHovered(false);
            e.Handled = true;
        }
    }

    private static bool HasSupportedData(IDataTransfer dataTransfer)
    {
        return dataTransfer.TryGetFiles()?.Any(x => !string.IsNullOrEmpty(x.TryGetLocalPath())) == true ||
            dataTransfer.Contains(DataFormat.Bitmap) ||
            !string.IsNullOrEmpty(dataTransfer.TryGetText());
    }

    private void HandleDrop(IDataTransfer dataTransfer)
    {
        TaskSettings taskSettings = TaskSettings ?? global::ShareX.TaskSettings.GetDefaultTaskSettings();
        string[] files = dataTransfer.TryGetFiles()?
            .Select(x => x.TryGetLocalPath())
            .Where(x => !string.IsNullOrEmpty(x))
            .Cast<string>()
            .ToArray() ?? [];

        if (files.Length > 0)
        {
            UploadManager.UploadFile(files, taskSettings);
            return;
        }

        AvaloniaBitmap? bitmap = dataTransfer.TryGetBitmap();
        if (bitmap != null)
        {
            UploadManager.RunImageTask(ConvertBitmap(bitmap), taskSettings);
            return;
        }

        string? text = dataTransfer.TryGetText();
        if (!string.IsNullOrEmpty(text))
        {
            UploadManager.UploadText(text, taskSettings, true);
        }
    }

    private static DrawingBitmap ConvertBitmap(Bitmap bitmap)
    {
        using MemoryStream stream = new();
        bitmap.Save(stream, PngBitmapEncoderOptions.Default);
        stream.Position = 0;
        using DrawingBitmap decoded = new(stream);
        return new DrawingBitmap(decoded);
    }

    private void SetHovered(bool hovered)
    {
        if (_isHovered == hovered)
        {
            return;
        }

        _isHovered = hovered;
        Opacity = hovered ? _hoverOpacity : _normalOpacity;
        DropSurface.Classes.Set("drag-over", hovered);
    }
}
