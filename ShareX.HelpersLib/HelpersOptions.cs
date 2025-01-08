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

using System.Collections.Generic;
using System.Drawing;

namespace ShareX.HelpersLib
{
    public static class HelpersOptions
    {
        public const int RecentColorsMax = 32;

        public static ProxyInfo CurrentProxy { get; set; } = new ProxyInfo();
        public static bool AcceptInvalidSSLCertificates { get; set; } = false;
        public static bool DefaultCopyImageFillBackground { get; set; } = true;
        public static bool UseAlternativeClipboardCopyImage { get; set; } = false;
        public static bool UseAlternativeClipboardGetImage { get; set; } = false;
        public static bool RotateImageByExifOrientationData { get; set; } = true;
        public static string BrowserPath { get; set; } = "";
        public static List<Color> RecentColors { get; set; } = new List<Color>();
        public static string LastSaveDirectory { get; set; } = "";
        public static bool URLEncodeIgnoreEmoji { get; set; } = false;
        public static Dictionary<string, string> ShareXSpecialFolders { get; set; } = new Dictionary<string, string>();
        public static bool DevMode { get; set; } = false;
    }
}