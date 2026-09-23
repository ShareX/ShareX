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

namespace ShareX.UploadersLib;

public sealed record EmailWindowResult(string ToEmail, string Subject, string Body);

public partial class EmailWindow : Window
{
    public EmailWindowResult? SubmittedResult { get; private set; }

    public EmailWindow() : this(string.Empty, string.Empty, string.Empty)
    {
    }

    public EmailWindow(string? toEmail, string? subject, string? body)
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        ToEmailTextBox.Text = toEmail ?? string.Empty;
        SubjectTextBox.Text = subject ?? string.Empty;
        MessageTextBox.Text = body ?? string.Empty;

        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Activate();
        MessageTextBox.Focus();
        MessageTextBox.CaretIndex = MessageTextBox.Text?.Length ?? 0;
    }

    private void OnSendClick(object? sender, RoutedEventArgs e)
    {
        SubmittedResult = new EmailWindowResult(
            ToEmailTextBox.Text ?? string.Empty,
            SubjectTextBox.Text ?? string.Empty,
            MessageTextBox.Text ?? string.Empty);
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();
}
