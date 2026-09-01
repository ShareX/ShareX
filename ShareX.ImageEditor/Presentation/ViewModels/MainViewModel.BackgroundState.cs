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
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Localization;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace ShareX.ImageEditor.Presentation.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        public enum CanvasBackgroundMode
        {
            Gradient,
            Color,
            Transparent,
            Image,
            Wallpaper
        }

        public sealed class BackgroundModeOption
        {
            public required CanvasBackgroundMode Mode { get; init; }
            public required string DisplayName { get; init; }

            public override string ToString() => DisplayName;
        }

        private Bitmap? _backgroundBitmap;
        private bool _isInitializingBackgroundSettings;

        public ObservableCollection<BackgroundModeOption> BackgroundModeOptions { get; }

        [ObservableProperty]
        private BackgroundModeOption? _selectedBackgroundModeOption;

        [ObservableProperty]
        private string _backgroundColorHex = "#FFFFFFFF";

        [ObservableProperty]
        private string _backgroundGradientColor1Hex = "#FFFF7E5F";

        [ObservableProperty]
        private string _backgroundGradientColor2Hex = "#FFFEB47B";

        [ObservableProperty]
        private string? _backgroundImagePath;

        public bool IsGradientBackgroundModeSelected => SelectedBackgroundMode == CanvasBackgroundMode.Gradient;
        public bool IsColorBackgroundModeSelected => SelectedBackgroundMode == CanvasBackgroundMode.Color;
        public bool IsImageBackgroundModeSelected => SelectedBackgroundMode == CanvasBackgroundMode.Image;
        public bool HasBackgroundImagePath => !string.IsNullOrWhiteSpace(BackgroundImagePath);

        public IBrush BackgroundColorBrush
        {
            get => new SolidColorBrush(Color.Parse(BackgroundColorHex));
            set
            {
                if (value is SolidColorBrush solidBrush)
                {
                    BackgroundColorHex = $"#{solidBrush.Color.A:X2}{solidBrush.Color.R:X2}{solidBrush.Color.G:X2}{solidBrush.Color.B:X2}";
                }
            }
        }

        public Color BackgroundColorValue
        {
            get => Color.Parse(BackgroundColorHex);
            set => BackgroundColorHex = $"#{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}";
        }

        public Color BackgroundGradientColor1Value
        {
            get => Color.Parse(BackgroundGradientColor1Hex);
            set => BackgroundGradientColor1Hex = $"#{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}";
        }

        public Color BackgroundGradientColor2Value
        {
            get => Color.Parse(BackgroundGradientColor2Hex);
            set => BackgroundGradientColor2Hex = $"#{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}";
        }

        public IBrush BackgroundGradientBrush => CreateGradientBrush(BackgroundGradientColor1Value, BackgroundGradientColor2Value);

        partial void OnSelectedBackgroundModeOptionChanged(BackgroundModeOption? value)
        {
            if (_isInitializingBackgroundSettings)
            {
                return;
            }

            Options.BackgroundType = SelectedBackgroundMode.ToString();
            OnPropertyChanged(nameof(IsGradientBackgroundModeSelected));
            OnPropertyChanged(nameof(IsColorBackgroundModeSelected));
            OnPropertyChanged(nameof(IsImageBackgroundModeSelected));
            ApplySelectedBackgroundMode();
        }

        partial void OnBackgroundColorHexChanged(string value)
        {
            if (_isInitializingBackgroundSettings)
            {
                return;
            }

            Options.BackgroundColorHex = value;
            OnPropertyChanged(nameof(BackgroundColorBrush));
            OnPropertyChanged(nameof(BackgroundColorValue));

            if (SelectedBackgroundMode == CanvasBackgroundMode.Color)
            {
                ApplyColorBackground(BackgroundColorValue);
            }
        }

        partial void OnBackgroundGradientColor1HexChanged(string value)
        {
            if (_isInitializingBackgroundSettings)
            {
                return;
            }

            Options.BackgroundGradientColor1Hex = value;
            OnPropertyChanged(nameof(BackgroundGradientColor1Value));
            OnPropertyChanged(nameof(BackgroundGradientBrush));

            if (SelectedBackgroundMode == CanvasBackgroundMode.Gradient)
            {
                ApplyGradientBackground();
            }
        }

        partial void OnBackgroundGradientColor2HexChanged(string value)
        {
            if (_isInitializingBackgroundSettings)
            {
                return;
            }

            Options.BackgroundGradientColor2Hex = value;
            OnPropertyChanged(nameof(BackgroundGradientColor2Value));
            OnPropertyChanged(nameof(BackgroundGradientBrush));

            if (SelectedBackgroundMode == CanvasBackgroundMode.Gradient)
            {
                ApplyGradientBackground();
            }
        }

        partial void OnBackgroundImagePathChanged(string? value)
        {
            if (_isInitializingBackgroundSettings)
            {
                return;
            }

            Options.BackgroundImagePath = value ?? string.Empty;
            OnPropertyChanged(nameof(HasBackgroundImagePath));

            if (SelectedBackgroundMode == CanvasBackgroundMode.Image)
            {
                ApplyImageBackground(value);
            }
        }

        public void SetBackgroundImagePath(string? filePath)
        {
            bool isSamePath = string.Equals(BackgroundImagePath, filePath, StringComparison.Ordinal);
            BackgroundImagePath = filePath;

            if (isSamePath && SelectedBackgroundMode == CanvasBackgroundMode.Image)
            {
                ApplyImageBackground(filePath);
            }
        }

        private CanvasBackgroundMode SelectedBackgroundMode =>
            SelectedBackgroundModeOption?.Mode ?? CanvasBackgroundMode.Transparent;

        private void ApplySelectedBackgroundMode()
        {
            switch (SelectedBackgroundMode)
            {
                case CanvasBackgroundMode.Gradient:
                    ApplyGradientBackground();
                    break;
                case CanvasBackgroundMode.Color:
                    ApplyColorBackground(BackgroundColorValue);
                    break;
                case CanvasBackgroundMode.Transparent:
                    ApplyTransparentBackground();
                    break;
                case CanvasBackgroundMode.Image:
                    ApplyImageBackground(BackgroundImagePath);
                    break;
                case CanvasBackgroundMode.Wallpaper:
                    ApplyWallpaperBackground();
                    break;
            }
        }

        private void ApplyGradientBackground()
        {
            SetCanvasBackground(CreateGradientBrush(BackgroundGradientColor1Value, BackgroundGradientColor2Value));
        }

        private void ApplyColorBackground(Color color)
        {
            SetCanvasBackground(new SolidColorBrush(color));
        }

        private void ApplyTransparentBackground()
        {
            SetCanvasBackground(Brushes.Transparent);
        }

        private void ApplyImageBackground(string? filePath)
        {
            if (!TryCreateImageBrushFromPath(filePath, DesktopWallpaperLayout.Fill, out ImageBrush? brush, out Bitmap? bitmap))
            {
                SetCanvasBackground(Brushes.Transparent);
                return;
            }

            SetCanvasBackground(brush!, bitmap);
        }

        private void ApplyWallpaperBackground()
        {
            IDesktopWallpaperService? desktopWallpaperService = EditorServices.DesktopWallpaper;
            if (desktopWallpaperService?.IsSupported != true ||
                !desktopWallpaperService.TryGetDesktopWallpaper(out DesktopWallpaperInfo? wallpaper) ||
                wallpaper == null)
            {
                EditorServices.ReportWarning(nameof(MainViewModel), "Failed to locate the current desktop wallpaper.");
                SetCanvasBackground(Brushes.Transparent);
                return;
            }

            EditorServices.ReportInformation(
                nameof(MainViewModel),
                $"Attempting wallpaper background from '{wallpaper.Path}' with layout '{wallpaper.Layout}'.");

            if (!TryCreateImageBrushFromPath(wallpaper.Path, wallpaper.Layout, out ImageBrush? brush, out Bitmap? bitmap))
            {
                SetCanvasBackground(Brushes.Transparent);
                return;
            }

            EditorServices.ReportInformation(nameof(MainViewModel), "Wallpaper background applied.");
            SetCanvasBackground(brush!, bitmap);
        }

        private void SetCanvasBackground(IBrush brush, Bitmap? ownedBitmap = null)
        {
            Bitmap? previousBitmap = _backgroundBitmap;
            _backgroundBitmap = ownedBitmap;
            CanvasBackground = brush;
            OnPropertyChanged(nameof(EffectiveCanvasBackground));

            if (previousBitmap != null && !ReferenceEquals(previousBitmap, ownedBitmap))
            {
                previousBitmap.Dispose();
            }
        }

        private BackgroundModeOption FindBackgroundModeOption(CanvasBackgroundMode mode)
        {
            return BackgroundModeOptions.FirstOrDefault(option => option.Mode == mode) ?? BackgroundModeOptions[0];
        }

        private void InitializeBackgroundSettingsFromOptions()
        {
            _isInitializingBackgroundSettings = true;

            try
            {
                BackgroundMargin = Math.Max(0, _options.BackgroundMargin);
                BackgroundPadding = Math.Max(0, _options.BackgroundPadding);
                BackgroundSmartPadding = _options.BackgroundSmartPadding;
                BackgroundRoundedCorner = Math.Max(0, _options.BackgroundRoundedCorner);
                BackgroundShadowRadius = Math.Max(0, _options.BackgroundShadowRadius);
                BackgroundGradientColor1Hex = NormalizeBackgroundColorHex(
                    _options.BackgroundGradientColor1Hex,
                    Color.FromArgb(255, 255, 126, 95));
                BackgroundGradientColor2Hex = NormalizeBackgroundColorHex(
                    _options.BackgroundGradientColor2Hex,
                    Color.FromArgb(255, 254, 180, 123));
                BackgroundColorHex = NormalizeBackgroundColorHex(
                    _options.BackgroundColorHex,
                    Color.FromArgb(255, 34, 34, 34));
                BackgroundImagePath = string.IsNullOrWhiteSpace(_options.BackgroundImagePath) ? null : _options.BackgroundImagePath;
                SelectedBackgroundModeOption = FindBackgroundModeOption(ParseBackgroundMode(_options.BackgroundType));
            }
            finally
            {
                _isInitializingBackgroundSettings = false;
            }

            Options.BackgroundMargin = BackgroundMargin;
            Options.BackgroundPadding = BackgroundPadding;
            Options.BackgroundSmartPadding = BackgroundSmartPadding;
            Options.BackgroundRoundedCorner = BackgroundRoundedCorner;
            Options.BackgroundShadowRadius = BackgroundShadowRadius;
            Options.BackgroundGradientColor1Hex = BackgroundGradientColor1Hex;
            Options.BackgroundGradientColor2Hex = BackgroundGradientColor2Hex;
            Options.BackgroundColorHex = BackgroundColorHex;
            Options.BackgroundImagePath = BackgroundImagePath ?? string.Empty;
            Options.BackgroundType = SelectedBackgroundMode.ToString();
        }

        private static CanvasBackgroundMode ParseBackgroundMode(string? backgroundType)
        {
            return Enum.TryParse(backgroundType, ignoreCase: true, out CanvasBackgroundMode mode)
                ? mode
                : CanvasBackgroundMode.Transparent;
        }

        private static string NormalizeBackgroundColorHex(string? colorHex, Color fallbackColor)
        {
            try
            {
                Color color = !string.IsNullOrWhiteSpace(colorHex)
                    ? Color.Parse(colorHex)
                    : fallbackColor;

                return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
            }
            catch
            {
                return $"#{fallbackColor.A:X2}{fallbackColor.R:X2}{fallbackColor.G:X2}{fallbackColor.B:X2}";
            }
        }

        private static ObservableCollection<BackgroundModeOption> BuildBackgroundModeOptions()
        {
            ObservableCollection<BackgroundModeOption> options =
            [
                new() { Mode = CanvasBackgroundMode.Gradient, DisplayName = Strings.MainViewModel_BackgroundModeGradient },
                new() { Mode = CanvasBackgroundMode.Color, DisplayName = Strings.MainViewModel_BackgroundModeColor },
                new() { Mode = CanvasBackgroundMode.Transparent, DisplayName = Strings.MainViewModel_BackgroundModeTransparent },
                new() { Mode = CanvasBackgroundMode.Image, DisplayName = Strings.MainViewModel_BackgroundModeImage }
            ];

            if (EditorServices.DesktopWallpaper?.IsSupported == true)
            {
                options.Add(new BackgroundModeOption { Mode = CanvasBackgroundMode.Wallpaper, DisplayName = Strings.MainViewModel_BackgroundModeWallpaper });
            }

            return options;
        }

        private static bool TryCreateImageBrushFromPath(string? filePath, DesktopWallpaperLayout layout, out ImageBrush? brush, out Bitmap? bitmap)
        {
            brush = null;
            bitmap = null;

            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return false;
            }

            try
            {
                bitmap = LoadBitmapFromPath(filePath);
                brush = CreateImageBrush(bitmap, layout);

                return true;
            }
            catch (Exception ex)
            {
                bitmap?.Dispose();
                bitmap = null;
                EditorServices.ReportWarning(nameof(MainViewModel), $"Failed to load background image '{filePath}'.", ex);
                return false;
            }
        }

        private static Bitmap LoadBitmapFromPath(string filePath)
        {
            try
            {
                using FileStream stream = File.OpenRead(filePath);
                return new Bitmap(stream);
            }
            catch (Exception avaloniaException)
            {
                try
                {
                    using FileStream stream = File.OpenRead(filePath);
                    using SKBitmap? skBitmap = SKBitmap.Decode(stream);
                    if (skBitmap != null)
                    {
                        return BitmapConversionHelpers.ToAvaloniBitmap(skBitmap);
                    }
                }
                catch (Exception skiaException)
                {
                    throw new AggregateException(
                        $"Failed to decode background image '{filePath}' with both Avalonia and Skia.",
                        avaloniaException,
                        skiaException);
                }

                throw new InvalidOperationException(
                    $"Failed to decode background image '{filePath}' with Avalonia, and Skia returned no bitmap.",
                    avaloniaException);
            }
        }

        private static ImageBrush CreateImageBrush(Bitmap bitmap, DesktopWallpaperLayout layout)
        {
            ImageBrush brush = new ImageBrush(bitmap);

            switch (layout)
            {
                case DesktopWallpaperLayout.Fill:
                    brush.Stretch = Stretch.UniformToFill;
                    break;
                case DesktopWallpaperLayout.Fit:
                    brush.Stretch = Stretch.Uniform;
                    break;
                case DesktopWallpaperLayout.Stretch:
                    brush.Stretch = Stretch.Fill;
                    break;
                case DesktopWallpaperLayout.Center:
                    brush.Stretch = Stretch.None;
                    brush.AlignmentX = AlignmentX.Center;
                    brush.AlignmentY = AlignmentY.Center;
                    break;
                case DesktopWallpaperLayout.Tile:
                    brush.Stretch = Stretch.None;
                    brush.TileMode = TileMode.Tile;
                    brush.SourceRect = new Avalonia.RelativeRect(0, 0, 1, 1, Avalonia.RelativeUnit.Relative);
                    brush.DestinationRect = new Avalonia.RelativeRect(0, 0, bitmap.Size.Width, bitmap.Size.Height, Avalonia.RelativeUnit.Absolute);
                    break;
                case DesktopWallpaperLayout.Span:
                    brush.Stretch = Stretch.Fill;
                    break;
                default:
                    brush.Stretch = Stretch.UniformToFill;
                    break;
            }

            return brush;
        }
    }
}
