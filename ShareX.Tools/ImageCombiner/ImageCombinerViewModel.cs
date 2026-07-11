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

using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.Tools.Infrastructure;
using System.Collections.ObjectModel;

namespace ShareX.Tools;

public sealed partial class ImageCombinerViewModel : ViewModelBase, IDisposable
{
    private readonly ImageCombinerSettings _settings;
    private readonly ImageCombinerServices _services;
    private readonly Action<ImageCombinerSettings>? _settingsChanged;
    private readonly HashSet<string> _selectedImages = [];
    private int _previewVersion;

    public ObservableCollection<string> Images { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanMoveUp))]
    [NotifyPropertyChangedFor(nameof(CanMoveDown))]
    private string? _selectedImage;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsVertical))]
    [NotifyPropertyChangedFor(nameof(AlignmentOptions))]
    private bool _isHorizontal;

    [ObservableProperty]
    private int _selectedAlignmentIndex;

    [ObservableProperty]
    private decimal _space;

    [ObservableProperty]
    private decimal _wrapAfter;

    [ObservableProperty]
    private bool _autoFillBackground;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPreview))]
    private Bitmap? _previewImage;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusText = "Add at least two images to combine.";

    public ImageCombinerViewModel(
        ImageCombinerSettings settings,
        ImageCombinerServices services,
        Action<ImageCombinerSettings>? settingsChanged = null,
        IEnumerable<string>? imageFiles = null)
    {
        _settings = settings;
        _services = services;
        _settingsChanged = settingsChanged;
        _isHorizontal = settings.Orientation == ImageCombinerOrientation.Horizontal;
        _selectedAlignmentIndex = (int)settings.Alignment;
        _space = settings.Space;
        _wrapAfter = settings.WrapAfter;
        _autoFillBackground = settings.AutoFillBackground;

        AddFiles(imageFiles);
    }

    public Func<Task<IReadOnlyList<string>?>>? SelectFilesRequested { get; set; }

    public bool IsVertical
    {
        get => !IsHorizontal;
        set
        {
            if (value)
            {
                IsHorizontal = false;
            }
        }
    }

    public IReadOnlyList<string> AlignmentOptions => IsHorizontal
        ? ["Top", "Center", "Bottom"]
        : ["Left", "Center", "Right"];

    public bool HasPreview => PreviewImage != null;
    public bool HasImages => Images.Count > 0;
    public bool CanCombine => Images.Count > 1 && !IsBusy;
    public bool CanRemove => _selectedImages.Count > 0 || SelectedImage != null;
    public bool CanMoveUp => SelectedImage != null && Images.IndexOf(SelectedImage) > 0;
    public bool CanMoveDown => SelectedImage != null && Images.IndexOf(SelectedImage) is int index && index >= 0 && index < Images.Count - 1;
    public string ImageCountText => $"{Images.Count} image{(Images.Count == 1 ? string.Empty : "s")}";

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

        CollectionChanged();
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
        SelectedImage = null;
        CollectionChanged();
    }

    [RelayCommand]
    private void MoveUp()
    {
        if (SelectedImage == null) return;
        int index = Images.IndexOf(SelectedImage);
        if (index > 0)
        {
            Images.Move(index, index - 1);
            CollectionChanged();
        }
    }

    [RelayCommand]
    private void MoveDown()
    {
        if (SelectedImage == null) return;
        int index = Images.IndexOf(SelectedImage);
        if (index >= 0 && index < Images.Count - 1)
        {
            Images.Move(index, index + 1);
            CollectionChanged();
        }
    }

    [RelayCommand]
    private async Task CombineAsync()
    {
        if (!CanCombine)
        {
            return;
        }

        IsBusy = true;
        StatusText = "Combining images...";
        try
        {
            await _services.ProcessAsync(CreateRequest());
            StatusText = "Images combined and sent to the configured after-capture workflow.";
        }
        catch (Exception ex)
        {
            StatusText = $"Failed to combine images: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            NotifyStateChanged();
        }
    }

    partial void OnSelectedImageChanged(string? value) => NotifyStateChanged();

    partial void OnIsHorizontalChanged(bool value)
    {
        OnPropertyChanged(nameof(IsVertical));
        OnPropertyChanged(nameof(AlignmentOptions));
        OptionsChanged();
    }

    partial void OnSelectedAlignmentIndexChanged(int value) => OptionsChanged();
    partial void OnSpaceChanged(decimal value) => OptionsChanged();
    partial void OnWrapAfterChanged(decimal value) => OptionsChanged();
    partial void OnAutoFillBackgroundChanged(bool value) => OptionsChanged();

    private void OptionsChanged()
    {
        _settings.Orientation = IsHorizontal ? ImageCombinerOrientation.Horizontal : ImageCombinerOrientation.Vertical;
        _settings.Alignment = (ImageCombinerAlignment)Math.Clamp(SelectedAlignmentIndex, 0, 2);
        _settings.Space = (int)Space;
        _settings.WrapAfter = (int)WrapAfter;
        _settings.AutoFillBackground = AutoFillBackground;
        _settingsChanged?.Invoke(_settings);
        _ = RefreshPreviewAsync();
    }

    private void CollectionChanged()
    {
        OnPropertyChanged(nameof(ImageCountText));
        OnPropertyChanged(nameof(HasImages));
        NotifyStateChanged();
        _ = RefreshPreviewAsync();
    }

    private void NotifyStateChanged()
    {
        OnPropertyChanged(nameof(CanCombine));
        OnPropertyChanged(nameof(CanRemove));
        OnPropertyChanged(nameof(CanMoveUp));
        OnPropertyChanged(nameof(CanMoveDown));
    }

    private ImageCombineRequest CreateRequest()
    {
        ImageCombinerSettings settings = new ImageCombinerSettings
        {
            Orientation = IsHorizontal ? ImageCombinerOrientation.Horizontal : ImageCombinerOrientation.Vertical,
            Alignment = (ImageCombinerAlignment)Math.Clamp(SelectedAlignmentIndex, 0, 2),
            Space = (int)Space,
            WrapAfter = (int)WrapAfter,
            AutoFillBackground = AutoFillBackground
        };
        return new ImageCombineRequest(Images.ToArray(), settings);
    }

    private async Task RefreshPreviewAsync()
    {
        int version = ++_previewVersion;
        if (Images.Count < 2)
        {
            PreviewImage?.Dispose();
            PreviewImage = null;
            StatusText = "Add at least two images to combine.";
            return;
        }

        try
        {
            byte[]? png = await _services.CreatePreviewAsync(CreateRequest());
            if (version != _previewVersion || png == null) return;
            using MemoryStream stream = new MemoryStream(png);
            Bitmap bitmap = new Bitmap(stream);
            PreviewImage?.Dispose();
            PreviewImage = bitmap;
            StatusText = $"Previewing {ImageCountText}.";
        }
        catch (Exception ex)
        {
            if (version == _previewVersion)
            {
                StatusText = $"Preview failed: {ex.Message}";
            }
        }
    }

    public void Dispose()
    {
        PreviewImage?.Dispose();
    }
}
