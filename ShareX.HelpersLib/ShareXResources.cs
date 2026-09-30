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

using ShareX.HelpersLib.Properties;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;

namespace ShareX.HelpersLib
{
    public static class ShareXResources
    {
        public static string Name { get; set; } = "ShareX";

        public static string UserAgent => $"{Name}/{Helpers.GetApplicationVersion()}";

        public static bool IsDarkTheme => Theme.IsDarkTheme;

        private static bool useWhiteIcon;

        public static bool UseWhiteIcon
        {
            get
            {
                return useWhiteIcon;
            }
            set
            {
                if (useWhiteIcon != value)
                {
                    useWhiteIcon = value;

                    if (useWhiteIcon)
                    {
                        Icon = Resources.ShareX_Icon_White;
                    }
                    else
                    {
                        Icon = Resources.ShareX_Icon;
                    }
                }
            }
        }

        private static Icon icon = Resources.ShareX_Icon;

        public static Icon Icon
        {
            get
            {
                return icon.CloneSafe();
            }
            set
            {
                if (icon != value)
                {
                    icon?.Dispose();
                    icon = value;
                }
            }
        }

        private static Bitmap logo = Resources.ShareX_Logo;

        public static Bitmap Logo
        {
            get
            {
                return logo.CloneSafe();
            }
            set
            {
                if (logo != value)
                {
                    logo?.Dispose();
                    logo = value;
                }
            }
        }

        public static ShareXTheme Theme { get; set; } = ShareXTheme.DarkTheme;

    }
}
