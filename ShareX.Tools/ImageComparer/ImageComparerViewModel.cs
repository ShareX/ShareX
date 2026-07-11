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

using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.Tools;
using ShareX.AvaloniaUI.Imaging;
using SkiaSharp;
using System.Globalization;

namespace ShareX.Tools;

public enum ImageComparerMode
{
    Slider,
    DiffView
}

public sealed partial class ImageComparerViewModel : ViewModelBase, IDisposable
{
    private const int MaxPreviewDimension = 2048;
    private readonly ImageComparisonService _comparisonService = new();
    private SKBitmap? _image1Bitmap;
    private SKBitmap? _image2Bitmap;
    private ImageComparisonResult? _comparisonResult;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasImage1))]
    private string? _image1Path;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasImage2))]
    private string? _image2Path;

    [ObservableProperty]
    private Bitmap? _image1Preview;

    [ObservableProperty]
    private Bitmap? _image2Preview;

    [ObservableProperty]
    private Bitmap? _diffPreview;

    [ObservableProperty]
    private Bitmap? _displayedLeftImage;

    [ObservableProperty]
    private Bitmap? _displayedRightImage;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSliderMode))]
    [NotifyPropertyChangedFor(nameof(IsDiffViewMode))]
    [NotifyPropertyChangedFor(nameof(IsSliderComparisonVisible))]
    [NotifyPropertyChangedFor(nameof(IsDiffComparisonVisible))]
    private ImageComparerMode _selectedMode;

    [ObservableProperty]
    private string _statusText = "Select two images to compare.";

    [ObservableProperty]
    private string _similarityText = "Similarity: -";

    [ObservableProperty]
    private IBrush _similarityBrush = Brushes.White;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanShowComparison))]
    [NotifyPropertyChangedFor(nameof(IsSliderComparisonVisible))]
    [NotifyPropertyChangedFor(nameof(IsDiffComparisonVisible))]
    private bool _hasComparison;

    public Func<string, Task<string?>>? SelectImageFileRequested { get; set; }

    public bool HasImage1 => !string.IsNullOrEmpty(Image1Path);

    public bool HasImage2 => !string.IsNullOrEmpty(Image2Path);

    public bool CanShowComparison => HasComparison;

    public bool IsSliderMode
    {
        get => SelectedMode == ImageComparerMode.Slider;
        set
        {
            if (value)
            {
                SelectedMode = ImageComparerMode.Slider;
            }
        }
    }

    public bool IsDiffViewMode
    {
        get => SelectedMode == ImageComparerMode.DiffView;
        set
        {
            if (value)
            {
                SelectedMode = ImageComparerMode.DiffView;
            }
        }
    }

    public bool IsSliderComparisonVisible => CanShowComparison && SelectedMode == ImageComparerMode.Slider;

    public bool IsDiffComparisonVisible => CanShowComparison && SelectedMode == ImageComparerMode.DiffView;

    [RelayCommand]
    private async Task SelectImage1Async()
    {
        await SelectImageAsync(1);
    }

    [RelayCommand]
    private async Task SelectImage2Async()
    {
        await SelectImageAsync(2);
    }

    private async Task SelectImageAsync(int imageNumber)
    {
        if (SelectImageFileRequested == null)
        {
            StatusText = "Image picker is unavailable.";
            return;
        }

        string? filePath = await SelectImageFileRequested($"Select Image {imageNumber}");
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        try
        {
            SKBitmap? bitmap = SKBitmap.Decode(filePath);
            if (bitmap == null)
            {
                StatusText = "The selected file could not be loaded as an image.";
                return;
            }

            SetImage(imageNumber, filePath, bitmap);
            UpdateComparison();
        }
        catch (Exception ex)
        {
            StatusText = $"Failed to load image: {ex.Message}";
        }
    }

    private void SetImage(int imageNumber, string filePath, SKBitmap bitmap)
    {
        Bitmap preview = CreatePreviewBitmap(bitmap);

        if (imageNumber == 1)
        {
            _image1Bitmap?.Dispose();
            Image1Preview?.Dispose();
            _image1Bitmap = bitmap;
            Image1Preview = preview;
            Image1Path = filePath;
        }
        else
        {
            _image2Bitmap?.Dispose();
            Image2Preview?.Dispose();
            _image2Bitmap = bitmap;
            Image2Preview = preview;
            Image2Path = filePath;
        }
    }

    private void UpdateComparison()
    {
        HasComparison = false;
        _comparisonResult?.Dispose();
        _comparisonResult = null;
        DiffPreview?.Dispose();
        DiffPreview = null;

        if (_image1Bitmap == null || _image2Bitmap == null)
        {
            StatusText = "Select two images to compare.";
            SimilarityText = "Similarity: -";
            SimilarityBrush = Brushes.White;
            RefreshDisplayedImages();
            return;
        }

        _comparisonResult = _comparisonService.Compare(_image1Bitmap, _image2Bitmap);
        DiffPreview = BitmapConversionHelpers.ToAvaloniBitmap(_comparisonResult.DiffBitmap);

        double similarity = _comparisonResult.SimilarityPercentage;
        SimilarityText = string.Create(CultureInfo.InvariantCulture, $"Similarity: {similarity:0.##}%");
        SimilarityBrush = Math.Abs(similarity - 100d) < 0.0001
            ? new SolidColorBrush(Color.FromRgb(16, 185, 129))
            : Brushes.White;
        StatusText = $"{_comparisonResult.MatchingPixels:N0} of {_comparisonResult.TotalPixels:N0} pixels match.";
        HasComparison = true;

        RefreshDisplayedImages();
    }

    private void RefreshDisplayedImages()
    {
        DisplayedLeftImage = Image1Preview;
        DisplayedRightImage = Image2Preview;
    }

    private static Bitmap CreatePreviewBitmap(SKBitmap source)
    {
        int largestDimension = Math.Max(source.Width, source.Height);
        if (largestDimension <= MaxPreviewDimension)
        {
            return BitmapConversionHelpers.ToAvaloniBitmap(source);
        }

        double scale = MaxPreviewDimension / (double)largestDimension;
        SKImageInfo previewInfo = new(
            Math.Max(1, (int)Math.Round(source.Width * scale)),
            Math.Max(1, (int)Math.Round(source.Height * scale)),
            SKColorType.Bgra8888,
            SKAlphaType.Premul);
        using SKBitmap? preview = source.Resize(
            previewInfo,
            new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));

        return BitmapConversionHelpers.ToAvaloniBitmap(preview ?? source);
    }

    public void Dispose()
    {
        _image1Bitmap?.Dispose();
        _image2Bitmap?.Dispose();
        _comparisonResult?.Dispose();
        Image1Preview?.Dispose();
        Image2Preview?.Dispose();
        DiffPreview?.Dispose();
    }
}
