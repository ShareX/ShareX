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

using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using System;

namespace ShareX.HelpersLib;

public partial class OutputBoxWindow : Window
{
    private readonly bool _scrollToEnd;

    public OutputBoxWindow() : this(string.Empty, Localization.Strings.OutputBoxWindow_Default_title)
    {
    }

    public OutputBoxWindow(string text, string title, bool scrollToEnd = false)
    {
        _scrollToEnd = scrollToEnd;

        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        Title = string.Format(Localization.Strings.OutputBoxWindow_Title, title);
        OutputTextBox.Text = text;

        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Activate();
        OutputTextBox.Focus();

        Dispatcher.UIThread.Post(() =>
        {
            OutputTextBox.CaretIndex = _scrollToEnd
                ? OutputTextBox.Text?.Length ?? 0
                : 0;
        }, DispatcherPriority.Loaded);
    }

    private async void OnCopyAllClick(object? sender, RoutedEventArgs e)
    {
        if (Clipboard != null && !string.IsNullOrEmpty(OutputTextBox.Text))
        {
            await Clipboard.SetTextAsync(OutputTextBox.Text);
        }
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Close();
}
