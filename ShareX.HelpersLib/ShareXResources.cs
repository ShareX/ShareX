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
using SkiaSharp;

namespace ShareX.HelpersLib
{
    /// <summary>Application name and user agent. The WinForms theme, icon and logo are in ShareX.HelpersLib.Windows.</summary>
    public static class ShareXResources
    {
        public static string Name { get; set; } = "ShareX";

        public static string UserAgent
        {
            get
            {
                return $"{Name}/{Helpers.GetApplicationVersion()}";
            }
        }

        public static bool IsDarkTheme => Theme.IsDarkTheme;

        public static bool UseWhiteIcon { get; set; }

        public static byte[] IconBytes => UseWhiteIcon ? Resources.ShareX_Icon_White : Resources.ShareX_Icon;

        private static SKBitmap logo = SkiaImageHelpers.ByteArrayToBitmap(Resources.ShareX_Logo);

        public static SKBitmap Logo
        {
            get
            {
                return logo.Copy();
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
