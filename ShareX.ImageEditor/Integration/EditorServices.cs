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

using ShareX.ImageEditor.Integration.Diagnostics;
using System.Diagnostics;

namespace ShareX.ImageEditor.Integration;

/// <summary>
/// Service locator for Editor services that must be provided by the host application.
/// </summary>
public static class EditorServices
{
    private static readonly object DesktopWallpaperPrewarmLock = new();
    private static IDesktopWallpaperService? _desktopWallpaper;
    private static Task? _desktopWallpaperPrewarmTask;

    /// <summary>
    /// Clipboard service for copy/paste operations.
    /// Host applications should set this before using clipboard functionality.
    /// </summary>
    public static IClipboardService? Clipboard { get; set; }

    /// <summary>
    /// Optional diagnostics sink for exception/messages emitted by ImageEditor.
    /// </summary>
    public static IEditorDiagnosticsSink? Diagnostics { get; set; }

    /// <summary>
    /// Optional service for resolving the current desktop wallpaper path.
    /// Hosts should set this before creating editor view models when wallpaper
    /// background support is desired.
    /// </summary>
    public static IDesktopWallpaperService? DesktopWallpaper
    {
        get => _desktopWallpaper;
        set
        {
            if (ReferenceEquals(_desktopWallpaper, value))
            {
                return;
            }

            _desktopWallpaper = value;

            lock (DesktopWallpaperPrewarmLock)
            {
                _desktopWallpaperPrewarmTask = null;
            }
        }
    }

    /// <summary>
    /// Installs the built-in wallpaper service when the host has not provided one.
    /// </summary>
    public static void EnsureDefaultDesktopWallpaperService()
    {
        if (DesktopWallpaper != null)
        {
            return;
        }

        if (OperatingSystem.IsWindows())
        {
            DesktopWallpaper = new WindowsDesktopWallpaperService();
        }
        else if (OperatingSystem.IsLinux())
        {
            DesktopWallpaper = new LinuxDesktopWallpaperService();
        }
        else if (OperatingSystem.IsMacOS())
        {
            DesktopWallpaper = new MacOSDesktopWallpaperService();
        }
    }

    /// <summary>
    /// Starts an asynchronous wallpaper lookup so hosts do not need to prewarm it manually.
    /// </summary>
    public static void StartDesktopWallpaperPrewarm(string source)
    {
        IDesktopWallpaperService? desktopWallpaperService = DesktopWallpaper;
        if (desktopWallpaperService?.IsSupported != true ||
            !desktopWallpaperService.RequiresDesktopWallpaperPrewarm)
        {
            return;
        }

        lock (DesktopWallpaperPrewarmLock)
        {
            if (_desktopWallpaperPrewarmTask != null)
            {
                return;
            }

            _desktopWallpaperPrewarmTask = Task.Run(() =>
            {
                try
                {
                    ReportInformation(source, "Starting wallpaper background prewarm.");
                    desktopWallpaperService.PrewarmDesktopWallpaper();
                    ReportInformation(source, "Wallpaper background prewarm completed.");
                }
                catch (Exception ex)
                {
                    ReportWarning(source, $"Wallpaper background prewarm failed: {ex.Message}", ex);
                }
            });
        }
    }

    public static void ReportInformation(string source, string message)
    {
        ReportDiagnostic(EditorDiagnosticLevel.Information, source, message, null);
    }

    public static void ReportWarning(string source, string message, Exception? exception = null)
    {
        ReportDiagnostic(EditorDiagnosticLevel.Warning, source, message, exception);
    }

    public static void ReportError(string source, string message, Exception? exception = null)
    {
        ReportDiagnostic(EditorDiagnosticLevel.Error, source, message, exception);
    }

    /// <summary>
    /// Emits a debug-level diagnostic for effect UI / preview tracing. Uses <see cref="Diagnostics"/> when set;
    /// otherwise writes to <see cref="Debug"/> so standalone runs still get console output.
    /// </summary>
    public static void ReportDebug(string source, string message)
    {
        if (Diagnostics != null)
        {
            ReportDiagnostic(EditorDiagnosticLevel.Debug, source, message, null);
            return;
        }

        Debug.WriteLine($"[ImageEditor:Debug:{source}] {message}");
    }

    public static void ReportDiagnostic(EditorDiagnosticLevel level, string source, string message, Exception? exception = null)
    {
        IEditorDiagnosticsSink? sink = Diagnostics;
        if (sink == null)
        {
            return;
        }

        var diagnosticEvent = new EditorDiagnosticEvent(level, source, message, exception);

        try
        {
            sink.Report(diagnosticEvent);
        }
        catch
        {
            // Diagnostics must never break editor functionality.
        }
    }
}