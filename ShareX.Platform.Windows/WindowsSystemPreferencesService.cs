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
using System;
using System.IO;
using System.Runtime.Versioning;
using System.Security;

namespace ShareX.Platform.Windows;

/// <summary>Reads the same values WinForms' SystemInformation returns.</summary>
[SupportedOSPlatform("windows")]
public sealed unsafe class WindowsSystemPreferencesService : ISystemPreferencesService
{
    private readonly Func<RegistryHive, string, object?> readPolicy;

    public WindowsSystemPreferencesService()
        : this(ReadPolicy)
    {
    }

    internal WindowsSystemPreferencesService(Func<RegistryHive, string, object?> readPolicy)
    {
        this.readPolicy = readPolicy;
    }

    public int WheelScrollLines
    {
        get
        {
            uint lines;
            return Win32.SystemParametersInfo(Win32.SPI_GETWHEELSCROLLLINES, 0, &lines, 0) ? (int)lines : DefaultSystemPreferencesService.DefaultWheelScrollLines;
        }
    }

    /// <summary>HKLM first, then HKCU, skipping unreadable or invalid values just as v22's SystemOptions does.</summary>
    public object? GetPolicy(string name)
    {
        foreach (RegistryHive hive in new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser })
        {
            object? value = readPolicy(hive, name);

            if (name == "PersonalPath")
            {
                if (value is string path)
                {
                    return path;
                }
            }
            else if (name is "DisableUpdateCheck" or "DisableUpload" or "DisableLogging")
            {
                if (value != null)
                {
                    try
                    {
                        return Convert.ToBoolean(value);
                    }
                    catch (Exception e) when (e is InvalidCastException or FormatException or OverflowException)
                    {
                        // An invalid machine policy must still allow the user's valid policy to apply.
                    }
                }
            }
            else if (value != null)
            {
                return value;
            }
        }

        return null;
    }

    private static object? ReadPolicy(RegistryHive hive, string name)
    {
        try
        {
            using RegistryKey baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default);
            using RegistryKey? key = baseKey.OpenSubKey(@"SOFTWARE\ShareX");
            return key?.GetValue(name);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or SecurityException)
        {
            return null;
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

    private readonly object changedLock = new object();
    private EventHandler? changed;

    /// <summary>Theme and display changes, from SystemEvents as the tray icon has always listened to them.</summary>
    public event EventHandler? Changed
    {
        add
        {
            lock (changedLock)
            {
                if (changed == null)
                {
                    SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
                    SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
                }

                changed += value;
            }
        }
        remove
        {
            lock (changedLock)
            {
                changed -= value;

                if (changed == null)
                {
                    SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
                    SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
                }
            }
        }
    }

    private void OnUserPreferenceChanged(object? sender, UserPreferenceChangedEventArgs e) => changed?.Invoke(this, EventArgs.Empty);

    private void OnDisplaySettingsChanged(object? sender, EventArgs e) => changed?.Invoke(this, EventArgs.Empty);
}
