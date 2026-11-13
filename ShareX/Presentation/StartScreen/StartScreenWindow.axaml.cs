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
using Avalonia.Interactivity;
using Avalonia.Platform;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using System;
using System.IO;
using System.Threading.Tasks;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;
using DrawingBitmap = SkiaSharp.SKBitmap;

namespace ShareX;

public partial class StartScreenWindow : Window
{
    private static StartScreenWindow? _current;
    private readonly StartScreenViewModel _viewModel;
    private readonly AvaloniaBitmap _logoBitmap;

    public StartScreenWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        using DrawingBitmap logo = ShareXResources.Logo;
        using Stream logoStream = logo.GetStream();
        logoStream.Position = 0;
        _logoBitmap = new AvaloniaBitmap(logoStream);
        LogoImage.Source = _logoBitmap;

        _viewModel = new StartScreenViewModel();
        DataContext = _viewModel;

        Opened += (_, _) =>
        {
            FitToWorkingArea();
            Activate();
        };
        ThemeManager.ThemeChanged += OnThemeChanged;
        Closed += OnClosed;
    }

    // The window sizes to its content and cannot be resized, so on a short screen (such as 1080p at 150%) cap it
    // to the working area; the content then scrolls above the footer.
    private void FitToWorkingArea()
    {
        Screen? screen = Screens.ScreenFromWindow(this) ?? Screens.Primary;

        if (screen == null)
        {
            return;
        }

        double frameHeight = FrameSize is Size frame ? frame.Height - ClientSize.Height : 0;
        double available = screen.WorkingArea.Height / screen.Scaling - frameHeight;

        if (ClientSize.Height > available)
        {
            MaxHeight = available;
            Position = new PixelPoint(Position.X, screen.WorkingArea.Y);
        }
    }

    public Task ShowAsync()
    {
        TaskCompletionSource<bool> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Closed += (_, _) => completion.TrySetResult(true);
        _current = this;
        Show();
        return completion.Task;
    }

    internal static void ActivateCurrent() => _current?.Activate();
    internal static void CloseCurrent() => _current?.Close();

    private void OnThemeChanged(object? sender, Avalonia.Styling.ThemeVariant theme) =>
        Avalonia.Threading.Dispatcher.UIThread.Post(() => RequestedThemeVariant = theme);

    private void OnGetStartedClick(object? sender, RoutedEventArgs e) => Close();

    private void OnClosed(object? sender, EventArgs e)
    {
        if (ReferenceEquals(_current, this)) _current = null;
        ThemeManager.ThemeChanged -= OnThemeChanged;
        _logoBitmap.Dispose();
        ApplicationState.Settings.ShowStartScreen = false;
        // The start screen has its own icon pack choice, so the update prompt is not needed.
        ApplicationState.Settings.ShowIconPackPrompt = false;
        SettingManager.SaveApplicationConfigAsync();
    }
}
