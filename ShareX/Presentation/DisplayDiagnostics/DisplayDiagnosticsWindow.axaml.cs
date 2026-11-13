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
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ShareX.AvaloniaUI;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Localization;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace ShareX;

public partial class DisplayDiagnosticsWindow : Window
{
    private static readonly TimeSpan StatusDuration = TimeSpan.FromSeconds(4);

    private readonly DispatcherTimer _statusTimer;
    private string _report = string.Empty;

    public DisplayDiagnosticsWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        _statusTimer = new DispatcherTimer { Interval = StatusDuration };
        _statusTimer.Tick += (_, _) =>
        {
            _statusTimer.Stop();
            StatusPanel.Opacity = 0;
        };

        Opened += (_, _) => RefreshReport();
        Closed += (_, _) => _statusTimer.Stop();
    }

    private void RefreshReport()
    {
        DisplayDiagnosticsResult result;

        try
        {
            result = DisplayDiagnosticsCollector.Collect(this);
        }
        catch (Exception e)
        {
            DebugHelper.WriteException(e, "Display diagnostics failed.");
            result = new DisplayDiagnosticsResult { Text = $"Display diagnostics failed:{Environment.NewLine}{e}" };
        }

        _report = result.Text;
        ReportTextBox.Text = _report;
        ReportTextBox.CaretIndex = 0;
        LayoutView.SetMonitors(result.Monitors, result.CursorPosition);
    }

    private void OnRefreshClick(object? sender, RoutedEventArgs e)
    {
        RefreshReport();
    }

    private async void OnCopyClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_report) || Clipboard == null)
        {
            return;
        }

        await Clipboard.SetTextAsync(_report);
        ShowStatus(Strings.DisplayDiagnosticsWindow_Copied);
    }

    private async void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_report))
        {
            return;
        }

        try
        {
            string? filePath = await SaveReportAsync();
            if (filePath != null)
            {
                ShowStatus(string.Format(CultureInfo.CurrentCulture, Strings.DisplayDiagnosticsWindow_Saved0, filePath));
            }
        }
        catch (Exception ex)
        {
            DebugHelper.WriteException(ex, "Failed to save the display diagnostics report.");
            MessageBox.Show(this, ex.Message, Strings.DisplayDiagnosticsWindow_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task<string?> SaveReportAsync()
    {
        IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = Strings.DisplayDiagnosticsWindow_SaveDialogTitle,
            SuggestedFileName = $"ShareX-display-diagnostics-{DateTime.Now.ToString("yyyy-MM-dd-HHmmss", CultureInfo.InvariantCulture)}.txt",
            DefaultExtension = "txt",
            FileTypeChoices =
            [
                new FilePickerFileType(Strings.DisplayDiagnosticsWindow_TextFile) { Patterns = ["*.txt"] }
            ]
        });

        if (file == null)
        {
            return null;
        }

        string filePath = file.Path.LocalPath;
        await File.WriteAllTextAsync(filePath, _report);
        return filePath;
    }

    private void ShowStatus(string message)
    {
        StatusText.Text = message;
        StatusPanel.Opacity = 1;
        _statusTimer.Stop();
        _statusTimer.Start();
    }
}
