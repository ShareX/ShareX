#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
    public static class SystemOptions
    {
        private const string RegistryPath = @"SOFTWARE\ShareX";

        public static bool DisableUpdateCheck { get; private set; }
        public static bool DisableUpload { get; private set; }
        public static bool DisableLogging { get; private set; }
        public static string PersonalPath { get; private set; }

        public static void UpdateSystemOptions()
        {
            DisableUpdateCheck = GetSystemOptionBoolean("DisableUpdateCheck");
            DisableUpload = GetSystemOptionBoolean("DisableUpload");
            DisableLogging = GetSystemOptionBoolean("DisableLogging");
            PersonalPath = GetSystemOptionString("PersonalPath");
        }

        private static bool GetSystemOptionBoolean(string name)
        {
            object value = RegistryHelpers.GetValue(RegistryPath, name, RegistryHive.LocalMachine);

            if (value != null)
            {
                try
                {
                    return Convert.ToBoolean(value);
                }
                catch
                {
                }
            }

            value = RegistryHelpers.GetValue(RegistryPath, name, RegistryHive.CurrentUser);

            if (value != null)
            {
                try
                {
                    return Convert.ToBoolean(value);
                }
                catch
                {
                }
            }

            return false;
        }

        private static string GetSystemOptionString(string name)
        {
            string value = RegistryHelpers.GetValueString(RegistryPath, name, RegistryHive.LocalMachine);

            if (value == null)
            {
                value = RegistryHelpers.GetValueString(RegistryPath, name, RegistryHive.CurrentUser);
            }

            return value;
        }
    }
}