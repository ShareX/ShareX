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
using ShareX.HelpersLib;
using System;

namespace ShareX.UploadersLib;

public sealed record YouTubeVideoOptionsWindowResult(
    string Title,
    string Description,
    YouTubeVideoPrivacy Visibility);

public partial class YouTubeVideoOptionsWindow : Window
{
    public YouTubeVideoOptionsWindowResult? SubmittedResult { get; private set; }

    public YouTubeVideoOptionsWindow()
        : this(string.Empty, string.Empty, YouTubeVideoPrivacy.Private)
    {
    }

    public YouTubeVideoOptionsWindow(
        string? title,
        string? description,
        YouTubeVideoPrivacy visibility)
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        TitleTextBox.Text = title ?? string.Empty;
        DescriptionTextBox.Text = description ?? string.Empty;
        VisibilityComboBox.ItemsSource = Helpers.GetLocalizedEnumDescriptions<YouTubeVideoPrivacy>();
        VisibilityComboBox.SelectedIndex = (int)visibility;

        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Activate();
        TitleTextBox.Focus();
        TitleTextBox.SelectAll();
    }

    private void OnConfirmClick(object? sender, RoutedEventArgs e)
    {
        int selectedIndex = VisibilityComboBox.SelectedIndex;
        if (!Enum.IsDefined(typeof(YouTubeVideoPrivacy), selectedIndex))
        {
            selectedIndex = (int)YouTubeVideoPrivacy.Private;
        }

        SubmittedResult = new YouTubeVideoOptionsWindowResult(
            TitleTextBox.Text ?? string.Empty,
            DescriptionTextBox.Text ?? string.Empty,
            (YouTubeVideoPrivacy)selectedIndex);
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();
}
