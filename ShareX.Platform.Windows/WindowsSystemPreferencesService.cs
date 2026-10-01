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

using Microsoft.Win32;
using ShareX.Platform.Windows.Native;
using System.Runtime.Versioning;

namespace ShareX.Platform.Windows;

/// <summary>Reads the same values WinForms' SystemInformation returns.</summary>
[SupportedOSPlatform("windows")]
public sealed unsafe class WindowsSystemPreferencesService : ISystemPreferencesService
{
    public int WheelScrollLines
    {
        get
        {
            uint lines;
            return Win32.SystemParametersInfo(Win32.SPI_GETWHEELSCROLLLINES, 0, &lines, 0) ? (int)lines : DefaultSystemPreferencesService.DefaultWheelScrollLines;
        }
    }

    public int SmallIconSize
    {
        get
        {
            int size = Win32.GetSystemMetrics(49); // SM_CXSMICON
            return size > 0 ? size : DefaultSystemPreferencesService.DefaultSmallIconSize;
        }
    }

    /// <summary>The task bar theme, which Windows keeps apart from the app theme. Missing means light, as on older Windows 10.</summary>
    public bool? SystemUsesLightTheme
    {
        get
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            return key?.GetValue("SystemUsesLightTheme") is not int value || value != 0;
        }
    }
}
