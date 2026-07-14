#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ShareX.HelpersLib;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;
using DrawingBitmap = System.Drawing.Bitmap;
using DrawingRectangle = System.Drawing.Rectangle;
using DrawingSize = System.Drawing.Size;

namespace ShareX.HistoryLib;

public sealed class ImageHistoryRow
{
    public ObservableCollection<ImageHistoryEntry> Items { get; } = [];
}

public sealed class ImageHistoryEntry : INotifyPropertyChanged, IDisposable
{
    private AvaloniaBitmap? _thumbnail;
    private CancellationTokenSource? _loadCancellation;
    private int _realizationCount;
    private bool _isSelected;
    private bool _isLoading;

    public ImageHistoryEntry(HistoryItem item, int thumbnailWidth, int thumbnailHeight)
    {
        Item = item;
        ThumbnailWidth = Math.Max(32, thumbnailWidth);
        ThumbnailHeight = Math.Max(32, thumbnailHeight);
        CardWidth = ThumbnailWidth + 2;
    }

    public HistoryItem Item { get; }
    public string FileName => string.IsNullOrWhiteSpace(Item.FileName) ? Path.GetFileName(Item.FilePath) : Item.FileName;
    public int ThumbnailWidth { get; }
    public int ThumbnailHeight { get; }
    public double CardWidth { get; }
    public bool IsFavorite => Item.Favorite;
    public bool HasThumbnail => Thumbnail != null;
    public bool ShowPlaceholder => Thumbnail == null;

    public AvaloniaBitmap? Thumbnail
    {
        get => _thumbnail;
        private set
        {
            if (ReferenceEquals(_thumbnail, value)) return;
            _thumbnail?.Dispose();
            _thumbnail = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasThumbnail));
            OnPropertyChanged(nameof(ShowPlaceholder));
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (_isLoading == value) return;
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public Task RealizeAsync(ImageHistoryThumbnailLoader loader)
    {
        _realizationCount++;
        return EnsureThumbnailAsync(loader);
    }

    private async Task EnsureThumbnailAsync(ImageHistoryThumbnailLoader loader)
    {
        if (Thumbnail != null || IsLoading) return;

        CancellationTokenSource cancellation = new();
        _loadCancellation = cancellation;
        CancellationToken token = cancellation.Token;
        IsLoading = true;
        bool retryAfterCancellation = false;

        try
        {
            AvaloniaBitmap? thumbnail = await loader.LoadAsync(Item.FilePath, ThumbnailWidth, ThumbnailHeight, token);
            if (_realizationCount > 0 && !token.IsCancellationRequested)
            {
                Thumbnail = thumbnail;
            }
            else
            {
                thumbnail?.Dispose();
            }
        }
        catch (OperationCanceledException)
        {
            // Thumbnail requests are routinely cancelled when virtualized items
            // leave the viewport. Cancellation is not an error for this operation.
        }
        catch (Exception ex)
        {
            DebugHelper.WriteException(ex);
        }
        finally
        {
            retryAfterCancellation = token.IsCancellationRequested && _realizationCount > 0 && Thumbnail == null;
            if (ReferenceEquals(_loadCancellation, cancellation))
            {
                _loadCancellation = null;
            }
            IsLoading = false;
            cancellation.Dispose();
        }

        if (retryAfterCancellation)
        {
            await EnsureThumbnailAsync(loader);
        }
    }

    public void Unrealize()
    {
        if (_realizationCount > 0) _realizationCount--;
        if (_realizationCount > 0) return;

        _loadCancellation?.Cancel();
        Thumbnail = null;
    }

    public void RefreshMetadata()
    {
        OnPropertyChanged(nameof(FileName));
        OnPropertyChanged(nameof(IsFavorite));
    }

    public void Dispose()
    {
        _realizationCount = 0;
        _loadCancellation?.Cancel();
        _loadCancellation?.Dispose();
        _loadCancellation = null;
        Thumbnail = null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public sealed class ImageHistoryThumbnailLoader : IDisposable
{
    private readonly SemaphoreSlim _workers = new(Math.Clamp(Environment.ProcessorCount / 2, 2, 4));
    private bool _disposed;

    public async Task<AvaloniaBitmap?> LoadAsync(string? filePath, int width, int height, CancellationToken token)
    {
        if (_disposed || token.IsCancellationRequested || string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return null;

        try
        {
            await _workers.WaitAsync(token);
        }
        catch (OperationCanceledException)
        {
            return null;
        }

        try
        {
            return await Task.Run(() => LoadCore(filePath, width, height, token));
        }
        finally
        {
            _workers.Release();
        }
    }

    private static AvaloniaBitmap? LoadCore(string filePath, int width, int height, CancellationToken token)
    {
        if (token.IsCancellationRequested) return null;

        try
        {
            using DrawingBitmap? shellThumbnail = NativeMethods.GetFileThumbnail(filePath, new DrawingSize(width, height));
            if (shellThumbnail != null)
            {
                DrawingRectangle bounds = new(0, 0, shellThumbnail.Width, shellThumbnail.Height);
                BitmapData data = shellThumbnail.LockBits(bounds, ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                try
                {
                    if (token.IsCancellationRequested) return null;
                    return new AvaloniaBitmap(Avalonia.Platform.PixelFormat.Bgra8888, AlphaFormat.Premul, data.Scan0,
                        new PixelSize(shellThumbnail.Width, shellThumbnail.Height), new Vector(96, 96), data.Stride);
                }
                finally
                {
                    shellThumbnail.UnlockBits(data);
                }
            }
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (Exception ex)
        {
            DebugHelper.WriteException(ex);
        }

        if (token.IsCancellationRequested) return null;
        using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        AvaloniaBitmap thumbnail = AvaloniaBitmap.DecodeToWidth(stream, width);
        if (token.IsCancellationRequested)
        {
            thumbnail.Dispose();
            return null;
        }
        if (thumbnail.PixelSize.Height <= height) return thumbnail;

        thumbnail.Dispose();
        stream.Position = 0;
        AvaloniaBitmap heightScaledThumbnail = AvaloniaBitmap.DecodeToHeight(stream, height);
        if (!token.IsCancellationRequested) return heightScaledThumbnail;
        heightScaledThumbnail.Dispose();
        return null;
    }

    public void Dispose()
    {
        _disposed = true;
    }
}
