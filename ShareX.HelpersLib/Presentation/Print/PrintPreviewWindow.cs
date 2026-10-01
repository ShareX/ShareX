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
using Avalonia.Layout;
using Avalonia.Media;
using ShareX.AvaloniaUI.Integration;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;

namespace ShareX.HelpersLib;

internal sealed class PrintPreviewWindow : Window
{
    private readonly Func<int, (SKBitmap Bitmap, bool HasMore)> render;
    private readonly Image image = new() { Stretch = Stretch.Uniform };
    private readonly TextBlock pageNumber = new() { VerticalAlignment = VerticalAlignment.Center };
    private readonly Button previous = new() { Content = "←" };
    private readonly Button next = new() { Content = "→" };
    private AvaloniaBitmap bitmap;
    private int page;

    private PrintPreviewWindow(Func<int, (SKBitmap Bitmap, bool HasMore)> render, Func<bool> print)
    {
        this.render = render;
        Title = Localization.Strings.PrintWindow_Preview.TrimEnd('.');
        Width = 850;
        Height = 900;
        MinWidth = 400;
        MinHeight = 300;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        Button printButton = new() { Content = Localization.Strings.PrintForm_LoadSettings_Print };
        printButton.Click += (_, _) => { if (print()) Close(); };
        Button close = new() { Content = Localization.Strings.OutputBoxWindow_Close };
        close.Click += (_, _) => Close();
        previous.Click += (_, _) => { if (page > 0) { page--; UpdatePage(); } };
        next.Click += (_, _) => { page++; UpdatePage(); };
        StackPanel buttons = new() { Orientation = Orientation.Horizontal, Spacing = 8, Margin = new Thickness(8) };
        buttons.Children.Add(previous);
        buttons.Children.Add(pageNumber);
        buttons.Children.Add(next);
        buttons.Children.Add(printButton);
        buttons.Children.Add(close);
        DockPanel.SetDock(buttons, Dock.Top);
        DockPanel panel = new();
        panel.Children.Add(buttons);
        panel.Children.Add(new Border { Background = Brushes.DimGray, Padding = new Thickness(12), Child = image });
        Content = panel;
        Closed += (_, _) => { image.Source = null; bitmap?.Dispose(); };
        UpdatePage();
    }

    private void UpdatePage()
    {
        var rendered = render(page);
        using SKBitmap source = rendered.Bitmap;
        AvaloniaBitmap replacement = null;
        if (source != null)
        {
            using MemoryStream stream = new();
            source.Save(stream, SKEncodedImageFormat.Png);
            stream.Position = 0;
            replacement = new AvaloniaBitmap(stream);
        }
        image.Source = replacement;
        bitmap?.Dispose();
        bitmap = replacement;
        pageNumber.Text = (page + 1).ToString();
        previous.IsEnabled = page > 0;
        next.IsEnabled = rendered.HasMore;
    }

    public static void ShowPreview(Func<int, (SKBitmap Bitmap, bool HasMore)> render, Func<bool> print) => DesktopServices.Run(async () =>
    {
        PrintPreviewWindow preview = new(render, print);
        Window owner = DesktopServices.GetWindow();
        if (owner.IsVisible) await preview.ShowDialog<bool>(owner);
        else
        {
            TaskCompletionSource<bool> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
            preview.Closed += (_, _) => completion.TrySetResult(true);
            preview.Show();
            await completion.Task;
        }
        return true;
    });
}
