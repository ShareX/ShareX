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
using Avalonia.Input;
using Avalonia.Interactivity;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Localization;
using System;
using System.IO;

namespace ShareX;

public partial class FileExistWindow : Window
{
    private readonly string _originalFileName;
    private readonly string _uniqueFilePath;

    public string FilePath { get; private set; }

    public FileExistWindow() : this(Path.Combine(Path.GetTempPath(), "ShareX.png"))
    {
    }

    public FileExistWindow(string filePath)
    {
        FilePath = filePath;
        _originalFileName = Path.GetFileNameWithoutExtension(filePath);
        _uniqueFilePath = FileHelpers.GetUniqueFilePath(filePath);

        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        ExistingPathText.Text = filePath;
        ToolTip.SetTip(ExistingPathText, filePath);
        NewNameTextBox.Text = _originalFileName;
        NewNameActionTitle.Text = Strings.FileExistWindow_UseNewName;
        OverwriteActionTitle.Text = Strings.FileExistWindow_Overwrite;
        UniqueNameActionTitle.Text = Strings.FileExistWindow_UseUniqueName;
        CancelActionTitle.Text = Strings.FileExistWindow_Cancel;
        OverwriteFileText.Text = Path.GetFileName(filePath);
        ToolTip.SetTip(OverwriteFileText, filePath);
        UniqueNameFileText.Text = Path.GetFileName(_uniqueFilePath);
        ToolTip.SetTip(UniqueNameFileText, _uniqueFilePath);
        UpdateNewNameAction();

        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Activate();
        NewNameTextBox.Focus();
        NewNameTextBox.SelectAll();
    }

    private string GetNewFileName()
    {
        string newFileName = NewNameTextBox.Text ?? string.Empty;
        return string.IsNullOrEmpty(newFileName) ? string.Empty : newFileName + Path.GetExtension(FilePath);
    }

    private void UpdateNewNameAction()
    {
        if (NewNameButton == null)
        {
            return;
        }

        string newName = NewNameTextBox.Text ?? string.Empty;
        NewNameButton.IsEnabled = !string.IsNullOrEmpty(newName) &&
            !newName.Equals(_originalFileName, StringComparison.OrdinalIgnoreCase);
        NewNameFileText.Text = GetNewFileName();
        ToolTip.SetTip(NewNameFileText, GetNewFileName());
    }

    private void UseNewFileName()
    {
        string newFileName = GetNewFileName();
        if (!string.IsNullOrEmpty(newFileName))
        {
            FilePath = Path.Combine(Path.GetDirectoryName(FilePath)!, newFileName);
            Close();
        }
    }

    private void Cancel()
    {
        FilePath = string.Empty;
        Close();
    }

    private void OnNewNameChanged(object? sender, TextChangedEventArgs e) => UpdateNewNameAction();

    private void OnUseNewNameClick(object? sender, RoutedEventArgs e) => UseNewFileName();

    private void OnOverwriteClick(object? sender, RoutedEventArgs e) => Close();

    private void OnUseUniqueNameClick(object? sender, RoutedEventArgs e)
    {
        FilePath = _uniqueFilePath;
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Cancel();

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            string newName = NewNameTextBox.Text ?? string.Empty;
            if (!string.IsNullOrEmpty(newName))
            {
                if (newName.Equals(_originalFileName, StringComparison.OrdinalIgnoreCase))
                {
                    Close();
                }
                else
                {
                    UseNewFileName();
                }
            }

            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            Cancel();
            e.Handled = true;
        }
    }
}
