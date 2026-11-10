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
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using ShareX.AvaloniaUI.Theming;
using ShareX.OCR;

namespace ShareX.Tools;

public partial class OCRWindow : Window
{
    private const double MinResultPanelWidth = 300;

    private readonly OCRViewModel _viewModel;
    private readonly OCRRegionCaptureHandler _selectRegion;
    private readonly OCROptions _options;
    private bool _windowPlacementApplied;

    public string Result => _viewModel.Result;

    public OCRWindow()
        : this([], new OCROptions(), _ => Task.FromResult(new OCRResult([])), () => Task.FromResult<byte[]?>(null))
    {
    }

    public OCRWindow(
        byte[] imageData,
        OCROptions options,
        OCRRecognitionHandler recognize,
        OCRRegionCaptureHandler selectRegion,
        Action<OCROptions>? optionsChanged = null)
    {
        _selectRegion = selectRegion;
        _options = options;
        _viewModel = new OCRViewModel(imageData, options, recognize, optionsChanged);
        DataContext = _viewModel;
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        _viewModel.SelectRegionRequested = SelectRegionAsync;
        _viewModel.SelectImageRequested = SelectImageAsync;
        _viewModel.CopyTextRequested = CopyTextAsync;
        _viewModel.CloseRequested = Close;

        ApplySavedWindowState();
        Opened += OnOpened;
        Closing += (_, _) => SaveWindowState();
        Closed += (_, _) => _viewModel.Dispose();
        Resized += (_, _) => SaveNormalWindowBounds();
        PositionChanged += (_, _) => SaveNormalWindowBounds();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private ColumnDefinition ResultPanelColumn => this.FindControl<Grid>("ContentGrid")!.ColumnDefinitions[2];

    private ShareX.HelpersLib.WindowState SavedWindowState => _options.WindowState ??= new ShareX.HelpersLib.WindowState();

    private void ApplySavedWindowState()
    {
        ShareX.HelpersLib.WindowState savedState = SavedWindowState;

        if (!savedState.Size.IsEmpty)
        {
            Width = Math.Max(savedState.Size.Width, MinWidth);
            Height = Math.Max(savedState.Size.Height, MinHeight);
        }

        if (!savedState.Location.IsEmpty)
        {
            PixelPoint savedPosition = new(savedState.Location.X, savedState.Location.Y);

            // Only restore a position that is still on a connected screen.
            if (Screens.ScreenFromPoint(savedPosition) != null)
            {
                WindowStartupLocation = WindowStartupLocation.Manual;
                Position = savedPosition;
            }
        }

        if (_options.ResultPanelWidth > 0)
        {
            ResultPanelColumn.Width = new GridLength(Math.Max(_options.ResultPanelWidth, MinResultPanelWidth));
        }

        WindowState = savedState.IsMaximized ? Avalonia.Controls.WindowState.Maximized : Avalonia.Controls.WindowState.Normal;
    }

    private void SaveWindowState()
    {
        SaveNormalWindowBounds();
        SavedWindowState.IsMaximized = WindowState == Avalonia.Controls.WindowState.Maximized;

        double resultPanelWidth = ResultPanelColumn.ActualWidth;

        if (resultPanelWidth > 0)
        {
            _options.ResultPanelWidth = Math.Round(resultPanelWidth);
        }
    }

    // The normal size and position are kept while the window moves, so a maximized window restores to them next time.
    private void SaveNormalWindowBounds()
    {
        if (!_windowPlacementApplied || WindowState != Avalonia.Controls.WindowState.Normal || Bounds.Width <= 0 || Bounds.Height <= 0)
        {
            return;
        }

        SavedWindowState.Size = new System.Drawing.Size((int)Math.Round(Bounds.Width), (int)Math.Round(Bounds.Height));
        SavedWindowState.Location = new System.Drawing.Point(Position.X, Position.Y);
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        _windowPlacementApplied = true;
        Opened -= OnOpened;
        await _viewModel.StartAsync();
    }

    private async Task<byte[]?> SelectRegionAsync()
    {
        WindowState previousState = WindowState;

        try
        {
            WindowState = WindowState.Minimized;
            await Task.Delay(250);
            return await _selectRegion();
        }
        finally
        {
            WindowState = previousState;
            Activate();
        }
    }

    private async Task CopyTextAsync(string text)
    {
        if (Clipboard != null)
        {
            await Clipboard.SetTextAsync(text);
        }
    }

    private async Task<byte[]?> SelectImageAsync()
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = Localization.Strings.OCRWindow_Select_image_dialog,
            AllowMultiple = false,
            FileTypeFilter = [FilePickerFileTypes.ImageAll]
        });

        if (files.Count == 0)
        {
            return null;
        }

        await using Stream stream = await files[0].OpenReadAsync();
        using MemoryStream memoryStream = new();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}
