#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
using ShareX.HelpersLib;
using System;

namespace ShareX
{
    public abstract class GenericStartupManager : IStartupManager
    {
        public abstract string StartupTargetPath { get; }

        public StartupTaskState State
        {
            get
            {
                byte[] status = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder",
                    "ShareX.lnk", null) as byte[];

                if (status != null && status.Length > 0 && status[0] == 3)
                {
                    return StartupTaskState.DisabledByUser;
                }

                return ShortcutHelpers.CheckShortcut(Environment.SpecialFolder.Startup, StartupTargetPath) ? StartupTaskState.Enabled : StartupTaskState.Disabled;
            }
            set
            {
                if (value == StartupTaskState.Enabled || value == StartupTaskState.Disabled)
                {
                    ShortcutHelpers.SetShortcut(value == StartupTaskState.Enabled, Environment.SpecialFolder.Startup, StartupTargetPath, "-silent");
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}