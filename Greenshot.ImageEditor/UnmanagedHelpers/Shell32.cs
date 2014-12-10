/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2013  Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GreenshotPlugin.UnmanagedHelpers
{
    /// <summary>
    /// Description of Shell32.
    /// </summary>
    public static class Shell32
    {
        [DllImport("shell32")]
        public static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

        [DllImport("shell32", CharSet = CharSet.Auto)]
        internal static extern IntPtr ExtractAssociatedIcon(HandleRef hInst, StringBuilder iconPath, ref int index);

        /// <summary>
        /// Returns an icon representation of an image contained in the specified file.
        /// This function is identical to System.Drawing.Icon.ExtractAssociatedIcon, xcept this version works.
        /// See: http://stackoverflow.com/questions/1842226/how-to-get-the-associated-icon-from-a-network-share-file
        /// </summary>
        /// <param name="filePath">The path to the file that contains an image.</param>
        /// <returns>The System.Drawing.Icon representation of the image contained in the specified file.</returns>
        public static Icon ExtractAssociatedIcon(String filePath)
        {
            int index = 0;

            Uri uri;
            if (filePath == null)
            {
                throw new ArgumentException(String.Format("'{0}' is not valid for '{1}'", "null", "filePath"), "filePath");
            }
            try
            {
                uri = new Uri(filePath);
            }
            catch (UriFormatException)
            {
                filePath = Path.GetFullPath(filePath);
                uri = new Uri(filePath);
            }

            if (uri.IsFile)
            {
                if (File.Exists(filePath))
                {
                    StringBuilder iconPath = new StringBuilder(1024);
                    iconPath.Append(filePath);

                    IntPtr handle = ExtractAssociatedIcon(new HandleRef(null, IntPtr.Zero), iconPath, ref index);
                    if (handle != IntPtr.Zero)
                    {
                        return Icon.FromHandle(handle);
                    }
                }
            }
            return null;
        }
    }
}