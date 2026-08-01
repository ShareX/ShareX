#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DrawingBitmap = System.Drawing.Bitmap;

namespace ShareX;

internal sealed class ThumbnailItemViewModel : INotifyPropertyChanged, IDisposable
{
    private int _refreshVersion;
    private Bitmap? _thumbnail;
    private bool _isSelected;
    private string _title = string.Empty;
    private string _status = string.Empty;
    private string _details = string.Empty;
    private string _placeholderIcon = LucideIcons.file;
    private double _progress;
    private bool _isProgressVisible;
    private bool _isFailed;
    private bool _isCombineTarget;

    public WorkerTask Task { get; }
    public int Width => Math.Max(96, Program.Settings.ThumbnailSize.Width);
    public int Height => Math.Max(72, Program.Settings.ThumbnailSize.Height);
    public bool ShowTitle => Program.Settings.ShowThumbnailTitle;
    public bool ShowTitleAbove => ShowTitle && Program.Settings.ThumbnailTitleLocation == ThumbnailTitleLocation.Top;
    public bool ShowTitleBelow => ShowTitle && Program.Settings.ThumbnailTitleLocation == ThumbnailTitleLocation.Bottom;

    public Bitmap? Thumbnail
    {
        get => _thumbnail;
        private set
        {
            if (ReferenceEquals(_thumbnail, value))
            {
                return;
            }

            Bitmap? old = _thumbnail;
            _thumbnail = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasThumbnail));
            OnPropertyChanged(nameof(HasNoThumbnail));
            old?.Dispose();
        }
    }

    public bool HasThumbnail => Thumbnail != null;
    public bool HasNoThumbnail => Thumbnail == null;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }

    public string Title
    {
        get => _title;
        private set => SetField(ref _title, value);
    }

    public string Status
    {
        get => _status;
        private set => SetField(ref _status, value);
    }

    public string Details
    {
        get => _details;
        private set => SetField(ref _details, value);
    }

    public string PlaceholderIcon
    {
        get => _placeholderIcon;
        private set => SetField(ref _placeholderIcon, value);
    }

    public double Progress
    {
        get => _progress;
        private set => SetField(ref _progress, value);
    }

    public bool IsProgressVisible
    {
        get => _isProgressVisible;
        private set => SetField(ref _isProgressVisible, value);
    }

    public bool IsFailed
    {
        get => _isFailed;
        private set => SetField(ref _isFailed, value);
    }

    public bool IsCombineTarget
    {
        get => _isCombineTarget;
        set => SetField(ref _isCombineTarget, value);
    }

    public ThumbnailItemViewModel(WorkerTask task)
    {
        Task = task;
        Refresh();
    }

    public void Refresh()
    {
        RefreshMetadata();

        string? filePath = Task.Info?.FilePath;

        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath) && FileHelpers.IsImageFile(filePath))
        {
            StartThumbnailRefresh(filePath, null);
        }
    }

    public void RefreshFromOwnedImage(DrawingBitmap image)
    {
        RefreshMetadata();
        StartThumbnailRefresh(null, image);
    }

    private void RefreshMetadata()
    {
        TaskInfo? info = Task.Info;

        Title = !string.IsNullOrEmpty(info?.FileName) ? info.FileName : Path.GetFileName(info?.FilePath) ?? Strings.ThumbnailItemViewModel_UntitledTask;
        Status = !string.IsNullOrEmpty(info?.Status) ? info.Status : Task.Status.ToString();
        Details = info?.ToString() ?? string.Empty;
        Progress = info?.Progress?.Percentage ?? 0;
        IsProgressVisible = Task.IsWorking && info?.Progress != null;
        IsFailed = Task.Status == TaskStatus.Failed;
        PlaceholderIcon = GetPlaceholderIcon(info?.FilePath ?? info?.FileName);
    }

    public void RefreshSettings()
    {
        OnPropertyChanged(nameof(Width));
        OnPropertyChanged(nameof(Height));
        OnPropertyChanged(nameof(ShowTitle));
        OnPropertyChanged(nameof(ShowTitleAbove));
        OnPropertyChanged(nameof(ShowTitleBelow));
        Refresh();
    }

    private void StartThumbnailRefresh(string? filePath, DrawingBitmap? image)
    {
        int version = ++_refreshVersion;
        int width = Width;
        _ = RefreshThumbnailAsync(version, width, filePath, image);
    }

    private async Task RefreshThumbnailAsync(int version, int width, string? filePath, DrawingBitmap? image)
    {
        Bitmap? bitmap = await System.Threading.Tasks.Task.Run(() => CreateThumbnail(width, filePath, image));

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (version == _refreshVersion)
            {
                if (bitmap != null)
                {
                    Thumbnail = bitmap;
                }
            }
            else
            {
                bitmap?.Dispose();
            }
        });
    }

    private static Bitmap? CreateThumbnail(int width, string? filePath, DrawingBitmap? image)
    {
        try
        {
            if (image != null)
            {
                using MemoryStream stream = new();
                image.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                return Bitmap.DecodeToWidth(stream, width, BitmapInterpolationMode.HighQuality);
            }

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath) && FileHelpers.IsImageFile(filePath))
            {
                using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return Bitmap.DecodeToWidth(stream, width, BitmapInterpolationMode.HighQuality);
            }

        }
        catch (Exception e)
        {
            DebugHelper.WriteException(e);
        }
        finally
        {
            image?.Dispose();
        }

        return null;
    }

    private static string GetPlaceholderIcon(string? filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return LucideIcons.file;
        if (FileHelpers.IsVideoFile(filePath)) return LucideIcons.file_video;
        if (FileHelpers.IsTextFile(filePath)) return LucideIcons.file_text;
        if (FileHelpers.IsImageFile(filePath)) return LucideIcons.file_image;
        return LucideIcons.file;
    }

    public void Dispose()
    {
        _refreshVersion++;
        Thumbnail?.Dispose();
        _thumbnail = null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

internal sealed class HotkeyTipViewModel
{
    public bool IsRegistered { get; }
    public string Hotkey { get; }
    public string Description { get; }

    public HotkeyTipViewModel(HotkeySettings hotkey)
    {
        IsRegistered = hotkey.HotkeyInfo.Status == HotkeyStatus.Registered;
        Hotkey = hotkey.HotkeyInfo.ToString();
        Description = hotkey.TaskSettings.ToString();
    }
}
