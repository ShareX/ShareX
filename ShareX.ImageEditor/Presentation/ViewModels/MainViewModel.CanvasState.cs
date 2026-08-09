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

using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using ShareX.ImageEditor.Integration;
using System.Collections.ObjectModel;

namespace ShareX.ImageEditor.Presentation.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private Color SamplePixelColor(Bitmap bitmap, int x, int y)
        {
            // Optimization: If we have the source SKBitmap (which we usually do for the main image),
            // use it directly instead of round-tripping.
            var pixelSize = bitmap.PixelSize;
            if (_currentSourceImage != null &&
                _currentSourceImage.Width == pixelSize.Width &&
                _currentSourceImage.Height == pixelSize.Height)
            {
                if (x < 0 || y < 0 || x >= _currentSourceImage.Width || y >= _currentSourceImage.Height)
                    return Colors.Transparent;

                var skColor = _currentSourceImage.GetPixel(x, y);
                return Color.FromArgb(skColor.Alpha, skColor.Red, skColor.Green, skColor.Blue);
            }

            // Fallback to fast conversion if we don't have the source match
            using var skBitmap = BitmapConversionHelpers.ToSKBitmap(bitmap);
            if (skBitmap == null || x < 0 || y < 0 || x >= skBitmap.Width || y >= skBitmap.Height)
            {
                return Colors.Transparent;
            }

            var pixel = skBitmap.GetPixel(x, y);
            return Color.FromArgb(pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue);
        }

        private void InvalidateSmartPaddingCache()
        {
            _smartPaddingCropInsets = new Thickness(0);
            _smartPaddingCacheValid = false;
        }

        private void DisableBackgroundSmartPaddingIfUnavailable()
        {
            if (!BackgroundSmartPadding)
            {
                return;
            }

            _suppressSmartPaddingChangeHandling = true;

            try
            {
                BackgroundSmartPadding = false;
            }
            finally
            {
                _suppressSmartPaddingChangeHandling = false;
            }

            Options.BackgroundSmartPadding = false;
        }

        private void EnsureSmartPaddingCache(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                InvalidateSmartPaddingCache();
            }

            if (_smartPaddingCacheValid)
            {
                return;
            }

            if (_originalSourceImage == null || PreviewImage == null)
            {
                InvalidateSmartPaddingCache();
                return;
            }

            if (_isApplyingSmartPadding)
            {
                return;
            }

            _isApplyingSmartPadding = true;
            try
            {
                var skBitmap = _originalSourceImage;
                if (skBitmap == null)
                {
                    InvalidateSmartPaddingCache();
                    return;
                }

                var targetColor = skBitmap.GetPixel(0, 0);
                const int tolerance = 30;

                int minX = skBitmap.Width;
                int minY = skBitmap.Height;
                int maxX = 0;
                int maxY = 0;

                unsafe
                {
                    byte* ptr = (byte*)skBitmap.GetPixels().ToPointer();
                    int width = skBitmap.Width;
                    int height = skBitmap.Height;
                    int rowBytes = skBitmap.RowBytes;
                    int bpp = skBitmap.BytesPerPixel;

                    byte tR = targetColor.Red;
                    byte tG = targetColor.Green;
                    byte tB = targetColor.Blue;
                    byte tA = targetColor.Alpha;

                    if (bpp == 4)
                    {
                        bool isBgra = skBitmap.ColorType == SkiaSharp.SKColorType.Bgra8888;

                        for (int y = 0; y < height; y++)
                        {
                            byte* row = ptr + (y * rowBytes);
                            bool rowHasContent = false;

                            for (int x = 0; x < width; x++)
                            {
                                byte r;
                                byte g;
                                byte b;
                                byte a;

                                if (isBgra)
                                {
                                    b = row[x * 4 + 0];
                                    g = row[x * 4 + 1];
                                    r = row[x * 4 + 2];
                                    a = row[x * 4 + 3];
                                }
                                else
                                {
                                    r = row[x * 4 + 0];
                                    g = row[x * 4 + 1];
                                    b = row[x * 4 + 2];
                                    a = row[x * 4 + 3];
                                }

                                if (Math.Abs(r - tR) > tolerance ||
                                    Math.Abs(g - tG) > tolerance ||
                                    Math.Abs(b - tB) > tolerance ||
                                    Math.Abs(a - tA) > tolerance)
                                {
                                    if (x < minX) minX = x;
                                    if (x > maxX) maxX = x;
                                    if (y < minY) minY = y;
                                    if (y > maxY) maxY = y;
                                    rowHasContent = true;
                                    break;
                                }
                            }

                            if (rowHasContent)
                            {
                                for (int x = width - 1; x >= 0; x--)
                                {
                                    byte r;
                                    byte g;
                                    byte b;
                                    byte a;

                                    if (isBgra)
                                    {
                                        b = row[x * 4 + 0];
                                        g = row[x * 4 + 1];
                                        r = row[x * 4 + 2];
                                        a = row[x * 4 + 3];
                                    }
                                    else
                                    {
                                        r = row[x * 4 + 0];
                                        g = row[x * 4 + 1];
                                        b = row[x * 4 + 2];
                                        a = row[x * 4 + 3];
                                    }

                                    if (Math.Abs(r - tR) > tolerance ||
                                        Math.Abs(g - tG) > tolerance ||
                                        Math.Abs(b - tB) > tolerance ||
                                        Math.Abs(a - tA) > tolerance)
                                    {
                                        if (x > maxX) maxX = x;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int y = 0; y < height; y++)
                        {
                            bool rowHasContent = false;

                            for (int x = 0; x < width; x++)
                            {
                                var pixel = skBitmap.GetPixel(x, y);
                                if (Math.Abs(pixel.Red - tR) > tolerance ||
                                    Math.Abs(pixel.Green - tG) > tolerance ||
                                    Math.Abs(pixel.Blue - tB) > tolerance ||
                                    Math.Abs(pixel.Alpha - tA) > tolerance)
                                {
                                    if (x < minX) minX = x;
                                    if (x > maxX) maxX = x;
                                    if (y < minY) minY = y;
                                    if (y > maxY) maxY = y;
                                    rowHasContent = true;
                                    break;
                                }
                            }

                            if (rowHasContent)
                            {
                                for (int x = width - 1; x >= 0; x--)
                                {
                                    var pixel = skBitmap.GetPixel(x, y);
                                    if (Math.Abs(pixel.Red - tR) > tolerance ||
                                        Math.Abs(pixel.Green - tG) > tolerance ||
                                        Math.Abs(pixel.Blue - tB) > tolerance ||
                                        Math.Abs(pixel.Alpha - tA) > tolerance)
                                    {
                                        if (x > maxX) maxX = x;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                if (minX > maxX || minY > maxY)
                {
                    _smartPaddingCropInsets = new Thickness(0);
                }
                else
                {
                    int cropRight = Math.Max(0, skBitmap.Width - maxX - 1);
                    int cropBottom = Math.Max(0, skBitmap.Height - maxY - 1);
                    _smartPaddingCropInsets = new Thickness(minX, minY, cropRight, cropBottom);
                }

                _smartPaddingCacheValid = true;

                if (!HasDetectedSmartPadding)
                {
                    DisableBackgroundSmartPaddingIfUnavailable();
                }
            }
            catch (Exception ex)
            {
                EditorServices.ReportError(nameof(MainViewModel), "Failed to apply smart padding crop.", ex);
                InvalidateSmartPaddingCache();
            }
            finally
            {
                _isApplyingSmartPadding = false;
            }
        }

        [RelayCommand]
        private void ApplyGradientPreset(GradientPreset preset)
        {
            bool wasGradientMode = SelectedBackgroundModeOption?.Mode == CanvasBackgroundMode.Gradient;
            BackgroundGradientColor1Value = preset.Color1;
            BackgroundGradientColor2Value = preset.Color2;

            if (!wasGradientMode)
            {
                SelectedBackgroundModeOption = FindBackgroundModeOption(CanvasBackgroundMode.Gradient);
            }
        }

        private void UpdateCanvasProperties()
        {
            if (AreBackgroundEffectsActive)
            {
                CanvasPadding = CalculateOutputPadding(BackgroundMargin, TargetOutputAspectRatio);
                CanvasShadow = new BoxShadows(ShareX.ImageEditor.Presentation.Helpers.ShadowEffectHelper.CreateCanvasShadow(_options.ShadowColor, BackgroundShadowRadius));
                CanvasCornerRadius = Math.Max(0, BackgroundRoundedCorner);
            }
            else
            {
                CanvasPadding = new Thickness(0);
                CanvasShadow = new BoxShadows(); // No shadow
                CanvasCornerRadius = 0;
            }
        }

        private Thickness CalculateOutputPadding(double basePadding, double? targetAspectRatio)
        {
            double imageWidth = SmartPaddingViewportWidth;
            double imageHeight = SmartPaddingViewportHeight;

            if (imageWidth <= 0 || imageHeight <= 0 || !targetAspectRatio.HasValue)
            {
                return new Thickness(basePadding);
            }

            double totalWidth = imageWidth + (basePadding * 2);
            double totalHeight = imageHeight + (basePadding * 2);
            double currentAspect = totalWidth / totalHeight;
            double target = targetAspectRatio.Value;

            double extraX = 0;
            double extraY = 0;

            const double epsilon = 0.0001;
            if (currentAspect > target + epsilon)
            {
                // Too wide, add vertical padding
                double requiredHeight = totalWidth / target;
                double addHeight = Math.Max(0, requiredHeight - totalHeight);
                extraY = addHeight / 2;
            }
            else if (currentAspect + epsilon < target)
            {
                // Too tall, add horizontal padding
                double requiredWidth = totalHeight * target;
                double addWidth = Math.Max(0, requiredWidth - totalWidth);
                extraX = addWidth / 2;
            }

            return new Thickness(basePadding + extraX, basePadding + extraY, basePadding + extraX, basePadding + extraY);
        }

        private static double? ParseAspectRatio(string ratio)
        {
            if (string.IsNullOrWhiteSpace(ratio) || string.Equals(ratio, OutputRatioAuto, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var parts = ratio.Split(':');
            if (parts.Length == 2 &&
                double.TryParse(parts[0], out var w) &&
                double.TryParse(parts[1], out var h) &&
                w > 0 && h > 0)
            {
                return w / h;
            }

            return null;
        }

        private static ObservableCollection<GradientPreset> BuildGradientPresets()
        {
            static LinearGradientBrush Make(string start, string end) => CreateGradientBrush(Color.Parse(start), Color.Parse(end));

            // Ordered so the most common day-to-day gradients stay near the top.
            return new ObservableCollection<GradientPreset>
            {
                new() { Name = "Sunset Glow", Brush = Make("#FF7E5F", "#FEB47B") },
                new() { Name = "Ocean Breeze", Brush = Make("#2193B0", "#6DD5ED") },
                new() { Name = "Skyline", Brush = Make("#00C6FF", "#0072FF") },
                new() { Name = "Peach Bloom", Brush = Make("#ED4264", "#FFEDBC") },
                new() { Name = "Mint Fresh", Brush = Make("#11998E", "#38EF7D") },
                new() { Name = "Royal Blue", Brush = Make("#396AFC", "#2948FF") },
                new() { Name = "Warm Flame", Brush = Make("#FF9A9E", "#FAD0C4") },
                new() { Name = "Coral Candy", Brush = Make("#FF9966", "#FF5E62") },
                new() { Name = "Mango", Brush = Make("#FFE259", "#FFA751") },
                new() { Name = "Violet Dream", Brush = Make("#4776E6", "#8E54E9") },
                new() { Name = "Ruby Fire", Brush = Make("#DD2476", "#FF512F") },
                new() { Name = "Aqua Marine", Brush = Make("#1A2980", "#26D0CE") },
                new() { Name = "Instagram", Brush = Make("#833AB4", "#FD1D1D") },
                new() { Name = "Aurora Green", Brush = Make("#00F260", "#0575E6") },
                new() { Name = "Citrus Pop", Brush = Make("#F7971E", "#FFD200") },
                new() { Name = "Arctic Sky", Brush = Make("#74EBD5", "#9FACE6") },
                new() { Name = "Cherry Rush", Brush = Make("#EB3349", "#F45C43") },
                new() { Name = "Lavender Sky", Brush = Make("#E0C3FC", "#8EC5FC") },
                new() { Name = "Midnight City", Brush = Make("#232526", "#414345") },
                new() { Name = "Deep Space", Brush = Make("#000428", "#004E92") },
                new() { Name = "Lagoon", Brush = Make("#43C6AC", "#191654") },
                new() { Name = "Rose Gold", Brush = Make("#EECDA3", "#EF629F") },
                new() { Name = "Emerald Water", Brush = Make("#348F50", "#56B4D3") },
                new() { Name = "Lemon Lime", Brush = Make("#56AB2F", "#A8E063") },
                new() { Name = "Pink Haze", Brush = Make("#EC008C", "#FC6767") },
                new() { Name = "Slate", Brush = Make("#BDC3C7", "#2C3E50") },
                new() { Name = "Carbon Steel", Brush = Make("#485563", "#29323C") },
                new() { Name = "Purple Bliss", Brush = Make("#8E2DE2", "#4A00E0") },
                new() { Name = "Ember", Brush = Make("#CB2D3E", "#EF473A") },
                new() { Name = "Soft Lilac", Brush = Make("#C471F5", "#FA71CD") },
                new() { Name = "Blue Hour", Brush = Make("#355C7D", "#6C5B7B") },
                new() { Name = "Golden Hour", Brush = Make("#F3904F", "#FFD194") },
                new() { Name = "Watermelon", Brush = Make("#FF5F6D", "#FFC371") },
                new() { Name = "Sea Foam", Brush = Make("#4CB8C4", "#3CD3AD") },
                new() { Name = "Twilight Plum", Brush = Make("#41295A", "#2F0743") },
                new() { Name = "Candy Grape", Brush = Make("#B24592", "#F15F79") },
                new() { Name = "Fresh Mint", Brush = Make("#76B852", "#8DC26F") },
                new() { Name = "Polar Night", Brush = Make("#283048", "#859398") },
                new() { Name = "Tropical Sun", Brush = Make("#F6D365", "#FDA085") },
                new() { Name = "Blue Raspberry", Brush = Make("#00B4DB", "#0083B0") },
                new() { Name = "Rosewater", Brush = Make("#E55D87", "#5FC3E4") },
                new() { Name = "Copper Glow", Brush = Make("#B79891", "#94716B") },
                new() { Name = "Aurora Violet", Brush = Make("#654EA3", "#EAAFC8") },
                new() { Name = "Apple Mint", Brush = Make("#5A3F37", "#2C7744") },
                new() { Name = "Sunrise Peach", Brush = Make("#F857A6", "#FF5858") },
                new() { Name = "Iceberg", Brush = Make("#C9FFBF", "#FFAFBD") },
                new() { Name = "Nightfall", Brush = Make("#141E30", "#243B55") },
                new() { Name = "Electric Cyan", Brush = Make("#0575E6", "#00F260") },
                new() { Name = "Velvet Rose", Brush = Make("#DA4453", "#89216B") },
                new() { Name = "Desert Sand", Brush = Make("#BA8B02", "#181818") },
                new() { Name = "Cosmic Fusion", Brush = Make("#FF00CC", "#333399") },
                new() { Name = "Deep Ocean", Brush = Make("#360033", "#0B8793") },
                new() { Name = "Northern Lights", Brush = Make("#43E97B", "#38F9D7") },
                new() { Name = "Neon Glow", Brush = Make("#4FACFE", "#00F2FE") },
                new() { Name = "Burning Orange", Brush = Make("#FF416C", "#FF4B2B") },
                new() { Name = "Mystic Purple", Brush = Make("#9D50BB", "#6E48AA") },
                new() { Name = "Golden Sunset", Brush = Make("#F12711", "#F5AF19") },
                new() { Name = "Midnight Dark", Brush = Make("#2C3E50", "#3498DB") },
                new() { Name = "Cool Sky", Brush = Make("#2980B9", "#6DD5FA") },
                new() { Name = "Purple Love", Brush = Make("#CC2B5E", "#753A88") }
            };
        }

        private static LinearGradientBrush CreateGradientBrush(Color color1, Color color2)
        {
            return new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new Avalonia.Media.GradientStop(color1, 0),
                    new Avalonia.Media.GradientStop(color2, 1)
                }
            };
        }
    }
}