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
using Avalonia.Interactivity;
using ShareX.AvaloniaUI.Theming;
using System;

namespace ShareX.HelpersLib;

public partial class InputBoxWindow : Window
{
    public string? SubmittedText { get; private set; }

    public InputBoxWindow() : this(Localization.Strings.InputBoxWindow_Default_title)
    {
    }

    public InputBoxWindow(
        string title,
        string? inputText = null,
        string? okText = null,
        string? cancelText = null)
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        Title = string.Format(Localization.Strings.InputBoxWindow_Title, title);
        InputTextBox.Text = inputText ?? string.Empty;
        OKButton.Content = string.IsNullOrEmpty(okText)
            ? Localization.Strings.MyMessageBox_MyMessageBox_OK
            : okText;
        CancelButton.Content = string.IsNullOrEmpty(cancelText)
            ? Localization.Strings.MyMessageBox_MyMessageBox_Cancel
            : cancelText;

        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Activate();
        InputTextBox.Focus();
        InputTextBox.SelectAll();
    }

    private void OnOKClick(object? sender, RoutedEventArgs e)
    {
        SubmittedText = InputTextBox.Text ?? string.Empty;
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();
}
