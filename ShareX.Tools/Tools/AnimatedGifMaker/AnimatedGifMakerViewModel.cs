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

using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.HelpersLib;
using System.Collections.ObjectModel;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;

namespace ShareX.Tools;

public sealed partial class AnimatedGifMakerViewModel : ViewModelBase, IDisposable
{
    private readonly HashSet<string> _selectedImages = [];
    private readonly DispatcherTimer _previewTimer;
    private readonly List<AvaloniaBitmap> _previewFrames = [];
    private CancellationTokenSource? _previewCancellationTokenSource;
    private int _previewVersion;
    private int _previewFrameIndex;
    private int _completedPreviewRepeats;
    private bool _isDisposed;

    public ObservableCollection<string> Images { get; } = [];

    [ObservableProperty]
    private string? _selectedImage;

    [ObservableProperty]
    private decimal _delay = 500;

    [ObservableProperty]
    private bool _loop = true;

    [ObservableProperty]
    private decimal _repeatCount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPreview))]
    private AvaloniaBitmap? _previewImage;

    [ObservableProperty]
    private bool _isPreviewLoading;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasOutput))]
    private string _outputFilePath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasMessage))]
    private string _message = string.Empty;

    public AnimatedGifMakerViewModel()
    {
        _previewTimer = new DispatcherTimer();
        _previewTimer.Tick += OnPreviewTimerTick;
    }

    public Func<Task<IReadOnlyList<string>?>>? SelectFilesRequested { get; set; }
    public Func<string, Task<string?>>? SelectOutputFileRequested { get; set; }

    public bool HasImages => Images.Count > 0;
    public bool HasPreview => PreviewImage != null;
    public bool HasOutput => !string.IsNullOrWhiteSpace(OutputFilePath) && File.Exists(OutputFilePath);
    public bool HasMessage => !string.IsNullOrWhiteSpace(Message);
    public bool CanRemove => _selectedImages.Count > 0 || SelectedImage != null;
    public bool CanMoveUp => SelectedImage != null && Images.IndexOf(SelectedImage) > 0;
    public bool CanMoveDown => SelectedImage != null && Images.IndexOf(SelectedImage) is int index &&
        index >= 0 && index < Images.Count - 1;
    public bool CanCreate => !IsBusy && Images.Count > 1 && Delay >= 10 && Delay <= 655350 &&
        RepeatCount >= 0 && RepeatCount <= ushort.MaxValue;
    public string ImageCountText => string.Format(Images.Count == 1
        ? Localization.Strings.ImageThumbnailerViewModel_One_image
        : Localization.Strings.ImageThumbnailerViewModel_Image_count, Images.Count);
    public string FrameDelayUnit => Localization.Strings.NetworkMonitorViewModel_Milliseconds
        .Replace("{0:0}", string.Empty, StringComparison.Ordinal).Trim();
    public string PreviewInfoText => string.Format(Localization.Strings.AnimatedGifMakerViewModel_Preview_info,
        Images.Count, (int)Delay);

    [RelayCommand]
    private async Task AddAsync()
    {
        if (SelectFilesRequested != null)
        {
            AddFiles(await SelectFilesRequested());
        }
    }

    public void AddFiles(IEnumerable<string>? files)
    {
        if (files == null)
        {
            return;
        }

        foreach (string file in files.Where(File.Exists))
        {
            Images.Add(file);
        }

        if (SelectedImage == null)
        {
            SelectedImage = Images.FirstOrDefault();
        }

        NotifyCollectionChanged();
    }

    public void SetSelectedImages(IEnumerable<string> files)
    {
        _selectedImages.Clear();
        foreach (string file in files)
        {
            _selectedImages.Add(file);
        }
        OnPropertyChanged(nameof(CanRemove));
    }

    [RelayCommand]
    private void Remove()
    {
        string[] files = _selectedImages.Count > 0
            ? _selectedImages.ToArray()
            : SelectedImage == null ? [] : [SelectedImage];

        foreach (string file in files)
        {
            Images.Remove(file);
        }

        _selectedImages.Clear();
        SelectedImage = Images.FirstOrDefault();
        NotifyCollectionChanged();
    }

    [RelayCommand]
    private void MoveUp()
    {
        if (SelectedImage == null)
        {
            return;
        }

        int index = Images.IndexOf(SelectedImage);
        if (index > 0)
        {
            Images.Move(index, index - 1);
            NotifyCollectionChanged();
        }
    }

    [RelayCommand]
    private void MoveDown()
    {
        if (SelectedImage == null)
        {
            return;
        }

        int index = Images.IndexOf(SelectedImage);
        if (index >= 0 && index < Images.Count - 1)
        {
            Images.Move(index, index + 1);
            NotifyCollectionChanged();
        }
    }

    [RelayCommand]
    private async Task CreateAsync()
    {
        if (!CanCreate || SelectOutputFileRequested == null)
        {
            return;
        }

        string firstImageName = Path.GetFileNameWithoutExtension(Images[0]);
        string? outputFilePath = await SelectOutputFileRequested(firstImageName + "_animated.gif");
        if (string.IsNullOrWhiteSpace(outputFilePath))
        {
            return;
        }

        string normalizedOutputFilePath = Path.ChangeExtension(outputFilePath, ".gif")!;
        string[] imageFiles = Images.ToArray();
        int delay = (int)Delay;
        bool loop = Loop;
        int repeatCount = (int)RepeatCount;

        IsBusy = true;
        Message = Localization.Strings.AnimatedGifMakerViewModel_Creating;
        try
        {
            await Task.Run(() => AnimatedGifMakerService.Create(normalizedOutputFilePath, imageFiles, delay,
                loop, repeatCount));
            OutputFilePath = normalizedOutputFilePath;
            Message = Localization.Strings.AnimatedGifMakerViewModel_Created;
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(AnimatedGifMakerViewModel), "Failed to create animated GIF.", ex);
            Message = string.Format(Localization.Strings.AnimatedGifMakerViewModel_Create_failed, ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void OpenOutput()
    {
        if (HasOutput)
        {
            FileHelpers.OpenFolderWithFile(OutputFilePath);
        }
    }

    partial void OnSelectedImageChanged(string? value) => NotifyStateChanged();

    partial void OnDelayChanged(decimal value)
    {
        NotifyOptionsChanged();
        RestartPreview();
    }

    partial void OnLoopChanged(bool value)
    {
        NotifyOptionsChanged();
        RestartPreview();
    }

    partial void OnRepeatCountChanged(decimal value)
    {
        NotifyOptionsChanged();
        RestartPreview();
    }

    partial void OnIsBusyChanged(bool value) => OnPropertyChanged(nameof(CanCreate));

    private void NotifyCollectionChanged()
    {
        OnPropertyChanged(nameof(HasImages));
        OnPropertyChanged(nameof(ImageCountText));
        OnPropertyChanged(nameof(PreviewInfoText));
        OutputFilePath = string.Empty;
        Message = string.Empty;
        NotifyStateChanged();
        QueuePreviewRefresh();
    }

    private void NotifyOptionsChanged()
    {
        OnPropertyChanged(nameof(CanCreate));
        OnPropertyChanged(nameof(PreviewInfoText));
        OutputFilePath = string.Empty;
        Message = string.Empty;
    }

    private void NotifyStateChanged()
    {
        OnPropertyChanged(nameof(CanCreate));
        OnPropertyChanged(nameof(CanRemove));
        OnPropertyChanged(nameof(CanMoveUp));
        OnPropertyChanged(nameof(CanMoveDown));
    }

    private void QueuePreviewRefresh()
    {
        _previewCancellationTokenSource?.Cancel();
        _previewCancellationTokenSource?.Dispose();
        _previewCancellationTokenSource = new CancellationTokenSource();
        _ = RefreshPreviewAsync(_previewCancellationTokenSource.Token);
    }

    private async Task RefreshPreviewAsync(CancellationToken cancellationToken)
    {
        int version = ++_previewVersion;
        if (_isDisposed || Images.Count < 2)
        {
            ReplacePreviewFrames([]);
            IsPreviewLoading = false;
            return;
        }

        IsPreviewLoading = true;
        try
        {
            await Task.Delay(100, cancellationToken);
            string[] imageFiles = Images.ToArray();
            IReadOnlyList<byte[]> frameBytes = await Task.Run(
                () => AnimatedGifMakerService.CreatePreviewFrames(imageFiles), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            List<AvaloniaBitmap> frames = new(frameBytes.Count);
            try
            {
                foreach (byte[] bytes in frameBytes)
                {
                    using MemoryStream stream = new(bytes, writable: false);
                    frames.Add(new AvaloniaBitmap(stream));
                }

                if (version == _previewVersion && !_isDisposed)
                {
                    ReplacePreviewFrames(frames);
                }
                else
                {
                    DisposeFrames(frames);
                }
            }
            catch
            {
                DisposeFrames(frames);
                throw;
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            if (version == _previewVersion && !_isDisposed)
            {
                ReplacePreviewFrames([]);
                Message = string.Format(Localization.Strings.AnimatedGifMakerViewModel_Preview_failed, ex.Message);
                ToolsDiagnostics.ReportWarning(nameof(AnimatedGifMakerViewModel),
                    "Failed to create animated GIF preview.", ex);
            }
        }
        finally
        {
            if (version == _previewVersion && !_isDisposed)
            {
                IsPreviewLoading = false;
            }
        }
    }

    private void ReplacePreviewFrames(List<AvaloniaBitmap> frames)
    {
        _previewTimer.Stop();
        PreviewImage = null;
        DisposeFrames(_previewFrames);
        _previewFrames.Clear();
        _previewFrames.AddRange(frames);
        RestartPreview();
    }

    private void RestartPreview()
    {
        _previewTimer.Stop();
        _previewFrameIndex = 0;
        _completedPreviewRepeats = 0;
        PreviewImage = _previewFrames.FirstOrDefault();

        if (_previewFrames.Count > 1)
        {
            _previewTimer.Interval = TimeSpan.FromMilliseconds(Math.Clamp((double)Delay, 10, 655350));
            _previewTimer.Start();
        }
    }

    private void OnPreviewTimerTick(object? sender, EventArgs e)
    {
        if (_previewFrameIndex < _previewFrames.Count - 1)
        {
            _previewFrameIndex++;
            PreviewImage = _previewFrames[_previewFrameIndex];
            return;
        }

        if (!Loop || RepeatCount > 0 && _completedPreviewRepeats >= (int)RepeatCount)
        {
            _previewTimer.Stop();
            return;
        }

        _completedPreviewRepeats++;
        _previewFrameIndex = 0;
        PreviewImage = _previewFrames[0];
    }

    private static void DisposeFrames(IEnumerable<AvaloniaBitmap> frames)
    {
        foreach (AvaloniaBitmap frame in frames)
        {
            frame.Dispose();
        }
    }

    public void Dispose()
    {
        _isDisposed = true;
        _previewVersion++;
        _previewTimer.Stop();
        _previewCancellationTokenSource?.Cancel();
        _previewCancellationTokenSource?.Dispose();
        _previewCancellationTokenSource = null;
        PreviewImage = null;
        DisposeFrames(_previewFrames);
        _previewFrames.Clear();
    }
}
