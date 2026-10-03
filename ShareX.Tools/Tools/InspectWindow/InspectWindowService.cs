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

using ShareX.HelpersLib;
using ShareX.Platform;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;

namespace ShareX.Tools;

public sealed class InspectWindowListItem : IDisposable
{
    public IntPtr Handle { get; }
    public string Title { get; }
    public string ProcessName { get; }
    public AvaloniaBitmap? Icon { get; }
    public bool HasIcon => Icon != null;
    public string DisplayName => string.IsNullOrWhiteSpace(ProcessName) ? Title : $"{Title}  —  {ProcessName}";

    public InspectWindowListItem(IntPtr handle, string title, string processName, AvaloniaBitmap? icon)
    {
        Handle = handle;
        Title = title;
        ProcessName = processName;
        Icon = icon;
    }

    public void Dispose()
    {
        Icon?.Dispose();
    }

    public override string ToString() => Title;
}

/// <summary>Window lists, picking and icons for the inspect and borderless window tools, through the platform window services.</summary>
public static class InspectWindowService
{
    public static IntPtr GetWindowAtPoint(int x, int y, bool topLevel)
    {
        return new IntPtr(PlatformServices.Current.WindowManagement.GetWindowAt(new PlatformPoint(x, y), topLevel));
    }

    public static IReadOnlyList<InspectWindowListItem> GetVisibleWindows(IntPtr ignoredHandle)
    {
        List<InspectWindowListItem> windows = [];

        foreach (PlatformWindow window in PlatformServices.Current.Windows.GetWindows())
        {
            if (window.Handle == ignoredHandle.ToInt64() || window.IsMinimized || string.IsNullOrWhiteSpace(window.Title))
            {
                continue;
            }

            try
            {
                windows.Add(new InspectWindowListItem(new IntPtr(window.Handle), window.Title, window.ProcessName ?? string.Empty,
                    GetWindowIcon(new IntPtr(window.Handle))));
            }
            catch (Exception ex)
            {
                ToolsDiagnostics.ReportWarning(nameof(InspectWindowService), "Failed to inspect a window while building the window list.", ex);
            }
        }

        return windows.OrderBy(x => x.Title, StringComparer.CurrentCultureIgnoreCase).ToArray();
    }

    public static AvaloniaBitmap? GetWindowIcon(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
        {
            return null;
        }

        try
        {
            byte[]? png = PlatformServices.Current.WindowManagement.GetIcon(handle.ToInt64());
            return png != null ? new AvaloniaBitmap(new MemoryStream(png, writable: false)) : null;
        }
        catch
        {
            return null;
        }
    }
}
