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
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using ShareX.Localization;
using System;
using System.Threading.Tasks;

namespace ShareX;

public partial class URLUploadWindow : Window
{
    private string? _submittedURL;
    private readonly bool _selectInitialURL;

    public URLUploadWindow() : this(null)
    {
    }

    public URLUploadWindow(string? initialURL)
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        PromptText.Text = Strings.URLUploadWindow_Prompt;

        if (!string.IsNullOrEmpty(initialURL))
        {
            URLTextBox.Text = initialURL;
            _selectInitialURL = true;
        }

        UpdateValidation();
        Opened += OnOpened;
    }

    public static Task<string?> ShowAsync(string? initialURL = null)
    {
        TaskCompletionSource<string?> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                URLUploadWindow window = new(initialURL);
                window.Closed += (_, _) => completion.TrySetResult(window._submittedURL);
                window.Show();
            }
            catch (Exception exception)
            {
                completion.TrySetException(exception);
            }
        });

        return completion.Task;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Activate();
        URLTextBox.Focus();

        if (_selectInitialURL)
        {
            URLTextBox.SelectAll();
        }
    }

    private void OnURLChanged(object? sender, TextChangedEventArgs e) => UpdateValidation();

    private void UpdateValidation()
    {
        string url = URLTextBox.Text?.Trim() ?? string.Empty;
        bool isValid = URLInputValidation.IsSupported(url);
        UploadButton.IsEnabled = isValid;
        ValidationText.IsVisible = url.Length > 0 && !isValid;
    }

    private void OnUploadClick(object? sender, RoutedEventArgs e)
    {
        string url = URLTextBox.Text?.Trim() ?? string.Empty;
        if (!URLInputValidation.IsSupported(url))
        {
            UpdateValidation();
            return;
        }

        _submittedURL = url;
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();
}
