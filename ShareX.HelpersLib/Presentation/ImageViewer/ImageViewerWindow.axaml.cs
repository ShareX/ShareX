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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using ShareX.AvaloniaUI.Theming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareX.HelpersLib;

public partial class ImageViewerWindow : Window
{
    private const double MinZoom = 0.1;
    private const double MaxZoom = 10;
    private const double ZoomFactor = 1.2;

    private readonly ImageViewerViewModel _viewModel = new();
    private readonly MatrixTransform _previewTransform = new() { Matrix = Matrix.Identity };
    private bool _closeOnDeactivate;

    public ImageViewerWindow()
    {
        Initialize();
        Opened += OnOpened;
    }

    public ImageViewerWindow(string filePath)
    {
        Initialize();
        _closeOnDeactivate = _viewModel.LoadFile(filePath);
    }

    public ImageViewerWindow(IReadOnlyList<string> filePaths, int selectedIndex)
    {
        Initialize();
        _closeOnDeactivate = _viewModel.LoadFiles(filePaths, selectedIndex);
    }

    public ImageViewerWindow(byte[] imageData, string? displayName = null)
    {
        Initialize();
        _closeOnDeactivate = _viewModel.LoadEncodedImage(imageData, displayName);
    }

    private void Initialize()
    {
        DataContext = _viewModel;
        AvaloniaXamlLoader.Load(this);
        this.FindControl<Image>("PreviewImage")!.RenderTransform = _previewTransform;
        _viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ImageViewerViewModel.CurrentImage))
            {
                _previewTransform.Matrix = Matrix.Identity;
            }
        };
        System.Drawing.Rectangle activeScreen = CaptureHelpers.GetActiveScreenBounds();
        WindowStartupLocation = WindowStartupLocation.Manual;
        Position = new PixelPoint(activeScreen.X, activeScreen.Y);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        Title = Localization.Strings.ImageViewerWindow_Title;
        KeyDown += OnKeyDown;
        Deactivated += OnDeactivated;
        Closed += (_, _) => _viewModel.Dispose();
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        if (!await SelectImageAsync())
        {
            Close();
        }
    }

    private async Task<bool> SelectImageAsync()
    {
        _closeOnDeactivate = false;
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = Localization.Strings.ImageViewerWindow_Open_image,
            AllowMultiple = false,
            FileTypeFilter = [FilePickerFileTypes.ImageAll]
        });

        string? filePath = files.FirstOrDefault()?.Path.LocalPath;
        bool loaded = !string.IsNullOrWhiteSpace(filePath) && _viewModel.LoadFile(filePath);
        _closeOnDeactivate = _viewModel.HasImage;
        Activate();
        Focus();
        return loaded;
    }

    private void OnPreviousClick(object? sender, RoutedEventArgs e) => _viewModel.Navigate(-1);
    private void OnNextClick(object? sender, RoutedEventArgs e) => _viewModel.Navigate(1);

    private void OnPreviewPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton is MouseButton.Left or MouseButton.Right)
        {
            Close();
            e.Handled = true;
        }
    }

    private void OnPreviewPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            if (_viewModel.HasImage && e.Delta.Y != 0 && sender is Control preview)
            {
                Matrix matrix = _previewTransform.Matrix;
                double zoom = Math.Clamp(matrix.M11 * Math.Pow(ZoomFactor, e.Delta.Y), MinZoom, MaxZoom);
                double scale = zoom / matrix.M11;
                Point pointerPosition = e.GetPosition(preview);
                _previewTransform.Matrix = new Matrix(zoom, 0, 0, zoom,
                    pointerPosition.X - (pointerPosition.X - matrix.M31) * scale,
                    pointerPosition.Y - (pointerPosition.Y - matrix.M32) * scale);
            }

            e.Handled = true;
            return;
        }

        if (e.Delta.Y > 0 && _viewModel.CanNavigateLeft)
        {
            _viewModel.Navigate(-1);
            e.Handled = true;
        }
        else if (e.Delta.Y < 0 && _viewModel.CanNavigateRight)
        {
            _viewModel.Navigate(1);
            e.Handled = true;
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Left:
                _viewModel.Navigate(-1);
                e.Handled = true;
                break;
            case Key.Right:
                _viewModel.Navigate(1);
                e.Handled = true;
                break;
            case Key.Escape:
            case Key.Enter:
            case Key.Space:
                Close();
                e.Handled = true;
                break;
        }
    }

    private void OnDeactivated(object? sender, EventArgs e)
    {
        if (_closeOnDeactivate)
        {
            Close();
        }
    }
}
