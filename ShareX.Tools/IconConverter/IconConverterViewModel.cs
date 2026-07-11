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
using ShareX.Tools.IconConverter;
using ShareX.AvaloniaUI.Imaging;
using SkiaSharp;

namespace ShareX.Tools.IconConverter;

public sealed partial class IconConverterViewModel : ViewModelBase, IDisposable
{
    private SKBitmap? _sourceBitmap;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasImage))]
    [NotifyCanExecuteChangedFor(nameof(ConvertCommand))]
    private string? _imagePath;

    [ObservableProperty]
    private Bitmap? _previewImage;

    [ObservableProperty]
    private string _imageDetails = "Drop an image here or choose a file.";

    [ObservableProperty]
    private string _statusText = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConvertCommand))]
    [NotifyCanExecuteChangedFor(nameof(BrowseCommand))]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedSize))]
    [NotifyCanExecuteChangedFor(nameof(ConvertCommand))]
    private bool _is16Selected = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedSize))]
    [NotifyCanExecuteChangedFor(nameof(ConvertCommand))]
    private bool _is32Selected = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedSize))]
    [NotifyCanExecuteChangedFor(nameof(ConvertCommand))]
    private bool _is48Selected = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedSize))]
    [NotifyCanExecuteChangedFor(nameof(ConvertCommand))]
    private bool _is64Selected = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedSize))]
    [NotifyCanExecuteChangedFor(nameof(ConvertCommand))]
    private bool _is128Selected = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedSize))]
    [NotifyCanExecuteChangedFor(nameof(ConvertCommand))]
    private bool _is256Selected = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsPalette8))]
    [NotifyPropertyChangedFor(nameof(IsTrueColor32))]
    private IconBitDepth _selectedBitDepth = IconBitDepth.TrueColor32;

    public Func<string, Task<string?>>? SelectImageFileRequested { get; set; }

    public Func<byte[], string?, Task<string?>>? SaveIconRequested { get; set; }

    public bool HasImage => _sourceBitmap != null;

    public bool HasSelectedSize => Is16Selected || Is32Selected || Is48Selected || Is64Selected || Is128Selected || Is256Selected;

    public bool IsPalette8
    {
        get => SelectedBitDepth == IconBitDepth.Palette8;
        set
        {
            if (value)
            {
                SelectedBitDepth = IconBitDepth.Palette8;
            }
        }
    }

    public bool IsTrueColor32
    {
        get => SelectedBitDepth == IconBitDepth.TrueColor32;
        set
        {
            if (value)
            {
                SelectedBitDepth = IconBitDepth.TrueColor32;
            }
        }
    }

    private bool CanBrowse() => !IsBusy;

    [RelayCommand(CanExecute = nameof(CanBrowse))]
    private async Task BrowseAsync()
    {
        if (SelectImageFileRequested == null)
        {
            StatusText = "Image picker is unavailable.";
            return;
        }

        string? filePath = await SelectImageFileRequested("Select image");
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            LoadImage(filePath);
        }
    }

    public void LoadImage(string filePath)
    {
        try
        {
            SKBitmap? bitmap = SKBitmap.Decode(filePath);
            if (bitmap == null)
            {
                StatusText = "The selected file could not be loaded as an image.";
                return;
            }

            Bitmap preview = BitmapConversionHelpers.ToAvaloniBitmap(bitmap);
            _sourceBitmap?.Dispose();
            PreviewImage?.Dispose();
            _sourceBitmap = bitmap;
            PreviewImage = preview;
            ImagePath = filePath;
            ImageDetails = $"{bitmap.Width} Ã— {bitmap.Height} pixels";
            StatusText = string.Empty;
        }
        catch (Exception ex)
        {
            StatusText = $"Failed to load image: {ex.Message}";
        }
    }

    private bool CanConvert() => HasImage && HasSelectedSize && !IsBusy;

    [RelayCommand(CanExecute = nameof(CanConvert))]
    private async Task ConvertAsync()
    {
        if (_sourceBitmap == null || SaveIconRequested == null)
        {
            StatusText = "Icon save dialog is unavailable.";
            return;
        }

        IsBusy = true;
        StatusText = "Creating icon...";

        try
        {
            int[] sizes = GetSelectedSizes();
            using SKBitmap source = _sourceBitmap.Copy();
            byte[] icon = await Task.Run(() => IconConverterService.Convert(source, sizes, SelectedBitDepth));
            string? savedPath = await SaveIconRequested(icon, ImagePath);

            StatusText = string.IsNullOrWhiteSpace(savedPath)
                ? string.Empty
                : $"Icon saved to {savedPath}";
        }
        catch (Exception ex)
        {
            StatusText = $"Failed to create icon: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private int[] GetSelectedSizes()
    {
        List<int> sizes = [];
        if (Is16Selected) sizes.Add(16);
        if (Is32Selected) sizes.Add(32);
        if (Is48Selected) sizes.Add(48);
        if (Is64Selected) sizes.Add(64);
        if (Is128Selected) sizes.Add(128);
        if (Is256Selected) sizes.Add(256);
        return sizes.ToArray();
    }

    public void Dispose()
    {
        _sourceBitmap?.Dispose();
        PreviewImage?.Dispose();
    }
}
