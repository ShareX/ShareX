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
using ShareX.AvaloniaUI.Integration;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using System;

namespace ShareX.UploadersLib;

public partial class TextUploadWindow : Window
{
    private string? _submittedContent;
    private bool _selectInitialContent;

    public TextUploadWindow() : this(null)
    {
    }

    public TextUploadWindow(string? content)
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        if (!string.IsNullOrEmpty(content))
        {
            ContentTextBox.Text = content;
            _selectInitialContent = true;
        }

        UpdateCharacterCount();
        Opened += OnOpened;
    }

    public static Task<string?> ShowAsync(string? content = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        TaskCompletionSource<string?> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                TextUploadWindow window = new(content);
                window.Closed += (_, _) => completion.TrySetResult(window._submittedContent);
                window.Show();
            }
            catch (Exception exception)
            {
                completion.TrySetException(exception);
            }
        });

        return completion.Task;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Activate();
        ContentTextBox.Focus();

        // Start from the clipboard text when nothing was passed in. The window's own clipboard works on Windows, macOS, X11 and Wayland.
        if (string.IsNullOrEmpty(ContentTextBox.Text) && Clipboard != null)
        {
            try
            {
                string? text = await Clipboard.TryGetTextAsync();

                if (!string.IsNullOrEmpty(text))
                {
                    ContentTextBox.Text = text;
                    _selectInitialContent = true;
                }
            }
            catch (Exception)
            {
                // A clipboard that cannot be read is the same as an empty one.
            }
        }

        if (_selectInitialContent)
        {
            ContentTextBox.SelectAll();
        }
    }

    private void OnContentChanged(object? sender, TextChangedEventArgs e) => UpdateCharacterCount();

    private void UpdateCharacterCount()
    {
        int length = ContentTextBox.Text?.Length ?? 0;
        CharacterCountText.Text = string.Format(
            length == 1
                ? Localization.Strings.TextUploadWindow_Character_count_singular
                : Localization.Strings.TextUploadWindow_Character_count_plural,
            length);
    }

    private void OnUploadClick(object? sender, RoutedEventArgs e)
    {
        _submittedContent = ContentTextBox.Text ?? string.Empty;
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();
}
