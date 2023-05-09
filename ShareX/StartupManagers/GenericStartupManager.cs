#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

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

#if !MicrosoftStore

using Microsoft.Win32;
using ShareX.HelpersLib;
using System;

namespace ShareX
{
    public abstract class GenericStartupManager : IStartupManager
    {
        public abstract string StartupTargetPath { get; }

        public StartupState State
        {
            get
            {
                if (ShortcutHelpers.CheckShortcut(Environment.SpecialFolder.Startup, "ShareX", StartupTargetPath))
                {
                    if (Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder",
                        "ShareX.lnk", null) is byte[] status && status.Length > 0 && status[0] == 3)
                    {
                        return StartupState.DisabledByUser;
                    }
                    else
                    {
                        return StartupState.Enabled;
                    }
                }
                else
                {
                    return StartupState.Disabled;
                }
            }
            set
            {
                if (value == StartupState.Enabled || value == StartupState.Disabled)
                {
                    ShortcutHelpers.SetShortcut(value == StartupState.Enabled, Environment.SpecialFolder.Startup, "ShareX", StartupTargetPath, "-silent");
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}

#endif