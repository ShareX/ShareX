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

using ShareX.Platform;

namespace ShareX.Tools;

/// <summary>Toggles borderless windowed mode through <see cref="IWindowManagementService"/>, which remembers each window's original frame.</summary>
public static class BorderlessWindowManager
{
    public static bool ToggleBorderlessWindow(string windowTitle, bool useWorkingArea = false)
    {
        if (string.IsNullOrWhiteSpace(windowTitle))
        {
            return false;
        }

        // An exact title first, then any window whose title contains the text, as ShareX always searched.
        IReadOnlyList<PlatformWindow> windows = PlatformServices.Current.Windows.GetWindows();
        PlatformWindow? window = windows.FirstOrDefault(x => x.Title == windowTitle) ??
            windows.FirstOrDefault(x => x.Title.Contains(windowTitle, StringComparison.InvariantCultureIgnoreCase));

        if (window == null)
        {
            return false;
        }

        ToggleBorderlessWindow(new IntPtr(window.Handle), useWorkingArea);
        return true;
    }

    public static void ToggleBorderlessWindow(IntPtr handle, bool useWorkingArea = false)
    {
        PlatformServices.Current.WindowManagement.ToggleBorderless(handle.ToInt64(), useWorkingArea);
    }
}
