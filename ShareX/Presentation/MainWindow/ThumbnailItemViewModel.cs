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

#nullable enable

using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Imaging;
using ShareX.HelpersLib;
using ShareX.Icons;
using ShareX.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DrawingBitmap = SkiaSharp.SKBitmap;

namespace ShareX;

internal sealed class ThumbnailItemViewModel : INotifyPropertyChanged, IDisposable
{
    private int _refreshVersion;
    private Bitmap? _thumbnail;
    private bool _isSelected;
    private string _title = string.Empty;
    private string _status = string.Empty;
    private string _details = string.Empty;
    private string _placeholderIcon = AppIcons.File;
    private double _progress;
    private bool _isProgressVisible;
    private bool _isFailed;
    private bool _isCombineTarget;
    private string _listStatus = string.Empty;
    private string _listStatusIcon = AppIcons.Clock;
    private string _listProgress = string.Empty;
    private string _listSpeed = string.Empty;
    private string _listElapsed = string.Empty;
    private string _listRemaining = string.Empty;
    private string _listUrl = string.Empty;

    public WorkerTask Task { get; }
    private static bool IsListView => ApplicationState.Settings.TaskViewMode == TaskViewMode.ListView;
    public int Width => Math.Max(96, ApplicationState.Settings.ThumbnailSize.Width);
    public int Height => Math.Max(72, ApplicationState.Settings.ThumbnailSize.Height);
    public bool ShowTitle => ApplicationState.Settings.ShowThumbnailTitle;
    public bool ShowTitleAbove => ShowTitle && ApplicationState.Settings.ThumbnailTitleLocation == ThumbnailTitleLocation.Top;
    public bool ShowTitleBelow => ShowTitle && ApplicationState.Settings.ThumbnailTitleLocation == ThumbnailTitleLocation.Bottom;

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
            OnPropertyChanged(nameof(ShowPlayIcon));
            old?.Dispose();
        }
    }

    public bool HasThumbnail => Thumbnail != null;
    public bool HasNoThumbnail => Thumbnail == null;
    public bool IsVideo => FileHelpers.IsVideoFile(Task.Info?.FilePath ?? Task.Info?.FileName);
    public bool ShowPlayIcon => HasThumbnail && IsVideo;
    public bool IsClickable
    {
        get
        {
            TaskInfo? info = Task.Info;
            string? filePath = info?.FilePath;

            return ApplicationState.Settings.ThumbnailClickAction switch
            {
                ThumbnailViewClickAction.Default => !string.IsNullOrEmpty(filePath) && File.Exists(filePath),
                ThumbnailViewClickAction.OpenImageViewer => !string.IsNullOrEmpty(filePath) &&
                    File.Exists(filePath) && FileHelpers.IsImageFile(filePath),
                ThumbnailViewClickAction.OpenFile => !string.IsNullOrEmpty(filePath) && File.Exists(filePath),
                ThumbnailViewClickAction.OpenFolder => !string.IsNullOrEmpty(filePath),
                ThumbnailViewClickAction.OpenURL => !string.IsNullOrWhiteSpace(info?.Result?.ToString()),
                ThumbnailViewClickAction.EditImage => !string.IsNullOrEmpty(filePath) &&
                    File.Exists(filePath) && FileHelpers.IsImageFile(filePath),
                _ => false
            };
        }
    }

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

    public string ListStatus
    {
        get => _listStatus;
        private set => SetField(ref _listStatus, value);
    }

    public string ListStatusIcon
    {
        get => _listStatusIcon;
        private set => SetField(ref _listStatusIcon, value);
    }

    public string ListProgress
    {
        get => _listProgress;
        private set => SetField(ref _listProgress, value);
    }

    public string ListSpeed
    {
        get => _listSpeed;
        private set => SetField(ref _listSpeed, value);
    }

    public string ListElapsed
    {
        get => _listElapsed;
        private set => SetField(ref _listElapsed, value);
    }

    public string ListRemaining
    {
        get => _listRemaining;
        private set => SetField(ref _listRemaining, value);
    }

    public string ListUrl
    {
        get => _listUrl;
        private set
        {
            if (_listUrl != value)
            {
                _listUrl = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ListUrlToolTip));
            }
        }
    }

    public string? ListUrlToolTip => string.IsNullOrEmpty(_listUrl) ? null : _listUrl;

    public ThumbnailItemViewModel(WorkerTask task)
    {
        Task = task;
        Refresh();
    }

    public void Refresh()
    {
        RefreshMetadata();

        // The list view never shows image content, so don't decode any.
        if (IsListView)
        {
            return;
        }

        string? filePath = Task.Info?.FilePath;

        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath) &&
            (FileHelpers.IsImageFile(filePath) || FileHelpers.IsVideoFile(filePath)))
        {
            StartThumbnailRefresh(filePath, null);
        }
    }

    public void RefreshFromOwnedImage(DrawingBitmap image)
    {
        RefreshMetadata();

        if (IsListView)
        {
            image.Dispose();
            return;
        }

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
        RefreshListColumns(info);
        OnPropertyChanged(nameof(IsVideo));
        OnPropertyChanged(nameof(ShowPlayIcon));
        OnPropertyChanged(nameof(IsClickable));
    }

    // Mirrors the ShareX 21 task list: transfer columns keep their last values once a task finishes.
    private void RefreshListColumns(TaskInfo? info)
    {
        TaskStatus status = Task.Status;
        ListStatusIcon = status switch
        {
            TaskStatus.InQueue => AppIcons.Clock,
            TaskStatus.Preparing or TaskStatus.Working or TaskStatus.Stopping => AppIcons.Upload,
            TaskStatus.Stopped => AppIcons.CircleStop,
            TaskStatus.Failed => AppIcons.CircleX,
            TaskStatus.History => AppIcons.History,
            _ => AppIcons.CircleCheck
        };

        if (status == TaskStatus.History)
        {
            ListStatus = Strings.TaskListView_History;
            ListProgress = info?.TaskEndTime.ToString() ?? string.Empty;
            ListUrl = info?.ToString() ?? string.Empty;
            return;
        }

        if (status == TaskStatus.InQueue)
        {
            ListStatus = Strings.TaskListView_InQueue;
            return;
        }

        bool binaryUnits = ApplicationState.Settings.BinaryUnits;
        var progress = info?.Progress;

        if (status == TaskStatus.Working && progress != null && progress.Length > 0)
        {
            ListStatus = string.Format("{0:0.0}%", progress.Percentage);
            ListProgress = string.Format("{0} / {1}", progress.Position.ToSizeString(binaryUnits), progress.Length.ToSizeString(binaryUnits));

            if (progress.Speed > 0)
            {
                ListSpeed = ((long)progress.Speed).ToSizeString(binaryUnits) + "/s";
            }

            ListElapsed = Helpers.ProperTimeSpan(progress.Elapsed);
            ListRemaining = Helpers.ProperTimeSpan(progress.Remaining);
        }
        else
        {
            ListStatus = !string.IsNullOrEmpty(info?.Status) ? info.Status : string.Empty;
        }

        if (status == TaskStatus.Failed)
        {
            ListUrl = string.Empty;
        }
        else if (status == TaskStatus.Completed || status == TaskStatus.Stopped)
        {
            ListUrl = info?.ToString() ?? string.Empty;
        }
    }

    public void RefreshSettings()
    {
        OnPropertyChanged(nameof(Width));
        OnPropertyChanged(nameof(Height));
        OnPropertyChanged(nameof(ShowTitle));
        OnPropertyChanged(nameof(ShowTitleAbove));
        OnPropertyChanged(nameof(ShowTitleBelow));

        if (IsListView)
        {
            // Release decoded previews when switching to the list view.
            _refreshVersion++;
            Thumbnail = null;
        }

        Refresh();
    }

    private void StartThumbnailRefresh(string? filePath, DrawingBitmap? image)
    {
        int version = ++_refreshVersion;
        int width = Width;
        int height = Height;
        _ = RefreshThumbnailAsync(version, width, height, filePath, image);
    }

    private async Task RefreshThumbnailAsync(int version, int width, int height, string? filePath, DrawingBitmap? image)
    {
        Bitmap? bitmap = await System.Threading.Tasks.Task.Run(() => CreateThumbnail(width, height, filePath, image));

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

    private static Bitmap? CreateThumbnail(int width, int height, string? filePath, DrawingBitmap? image)
    {
        try
        {
            if (image != null)
            {
                return BitmapConversionHelpers.CreatePreview(image, new Avalonia.PixelSize(width, height));
            }

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath) && FileHelpers.IsImageFile(filePath))
            {
                using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return Bitmap.DecodeToWidth(stream, width, BitmapInterpolationMode.HighQuality);
            }

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath) && FileHelpers.IsVideoFile(filePath))
            {
                using DrawingBitmap? shellThumbnail = NativeMethods.GetFileThumbnail(filePath, new System.Drawing.Size(width, height));

                if (shellThumbnail != null)
                {
                    return BitmapConversionHelpers.CreatePreview(shellThumbnail, new Avalonia.PixelSize(width, height));
                }
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
        if (string.IsNullOrEmpty(filePath)) return AppIcons.File;
        if (FileHelpers.IsVideoFile(filePath)) return AppIcons.FileVideo;
        if (FileHelpers.IsTextFile(filePath)) return AppIcons.FileText;
        if (FileHelpers.IsImageFile(filePath)) return AppIcons.FileImage;
        return AppIcons.File;
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
