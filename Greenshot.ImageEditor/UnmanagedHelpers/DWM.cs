/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015 Thomas Braun, Jens Klingen, Robin Krom
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

using Microsoft.Win32;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GreenshotPlugin.UnmanagedHelpers
{
    // See: http://msdn.microsoft.com/en-us/library/aa969502(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential)]
    public struct DWM_THUMBNAIL_PROPERTIES
    {
        // A bitwise combination of DWM thumbnail constant values that indicates which members of this structure are set.
        public int dwFlags;
        // The area in the destination window where the thumbnail will be rendered.
        public RECT rcDestination;
        // The region of the source window to use as the thumbnail. By default, the entire window is used as the thumbnail.
        public RECT rcSource;
        // The opacity with which to render the thumbnail. 0 is fully transparent while 255 is fully opaque. The default value is 255.
        public byte opacity;
        // TRUE to make the thumbnail visible; otherwise, FALSE. The default is FALSE.
        public bool fVisible;
        // TRUE to use only the thumbnail source's client area; otherwise, FALSE. The default is FALSE.
        public bool fSourceClientAreaOnly;
        public RECT Destination
        {
            set
            {
                dwFlags |= DWM_TNP_RECTDESTINATION;
                rcDestination = value;
            }
        }
        public RECT Source
        {
            set
            {
                dwFlags |= DWM_TNP_RECTSOURCE;
                rcSource = value;
            }
        }
        public byte Opacity
        {
            set
            {
                dwFlags |= DWM_TNP_OPACITY;
                opacity = value;
            }
        }
        public bool Visible
        {
            set
            {
                dwFlags |= DWM_TNP_VISIBLE;
                fVisible = value;
            }
        }
        public bool SourceClientAreaOnly
        {
            set
            {
                dwFlags |= DWM_TNP_SOURCECLIENTAREAONLY;
                fSourceClientAreaOnly = value;
            }
        }
        // A value for the rcDestination member has been specified.
        public const int DWM_TNP_RECTDESTINATION = 0x00000001;
        // A value for the rcSource member has been specified.
        public const int DWM_TNP_RECTSOURCE = 0x00000002;
        // A value for the opacity member has been specified.
        public const int DWM_TNP_OPACITY = 0x00000004;
        // A value for the fVisible member has been specified.
        public const int DWM_TNP_VISIBLE = 0x00000008;
        // A value for the fSourceClientAreaOnly member has been specified.
        public const int DWM_TNP_SOURCECLIENTAREAONLY = 0x00000010;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DWM_BLURBEHIND
    {
        public DWM_BB dwFlags;
        public bool fEnable;
        public IntPtr hRgnBlur;
        public bool fTransitionOnMaximized;
    }

    /// <summary>
    /// Description of DWM.
    /// </summary>
    public class DWM
    {
        public static readonly uint DWM_EC_DISABLECOMPOSITION = 0;
        public static readonly uint DWM_EC_ENABLECOMPOSITION = 1;

        // DWM
        [DllImport("dwmapi", SetLastError = true)]
        public static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr src, out IntPtr thumb);

        [DllImport("dwmapi", SetLastError = true)]
        public static extern int DwmUnregisterThumbnail(IntPtr thumb);

        [DllImport("dwmapi", SetLastError = true)]
        public static extern int DwmQueryThumbnailSourceSize(IntPtr thumb, out SIZE size);

        [DllImport("dwmapi", SetLastError = true)]
        public static extern int DwmUpdateThumbnailProperties(IntPtr hThumb, ref DWM_THUMBNAIL_PROPERTIES props);

        // Deprecated as of Windows 8 Release Preview
        [DllImport("dwmapi", SetLastError = true)]
        public static extern int DwmIsCompositionEnabled(out bool enabled);

        [DllImport("dwmapi", SetLastError = true)]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT lpRect, int size);

        [DllImport("dwmapi", SetLastError = true)]
        public static extern int DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);

        [DllImport("dwmapi", SetLastError = true)]
        public static extern uint DwmEnableComposition(uint uCompositionAction);

        public static void EnableComposition()
        {
            DwmEnableComposition(DWM_EC_ENABLECOMPOSITION);
        }

        public static void DisableComposition()
        {
            DwmEnableComposition(DWM_EC_DISABLECOMPOSITION);
        }

        // Key to ColorizationColor for DWM
        private const string COLORIZATION_COLOR_KEY = @"SOFTWARE\Microsoft\Windows\DWM";

        /// <summary>
        /// Helper method for an easy DWM check
        /// </summary>
        /// <returns>bool true if DWM is available AND active</returns>
        public static bool isDWMEnabled()
        {
            // According to: http://technet.microsoft.com/en-us/subscriptions/aa969538%28v=vs.85%29.aspx
            // And: http://msdn.microsoft.com/en-us/library/windows/desktop/aa969510%28v=vs.85%29.aspx
            // DMW is always enabled on Windows 8! So return true and save a check! ;-)
            if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 2)
            {
                return true;
            }
            if (Environment.OSVersion.Version.Major >= 6)
            {
                bool dwmEnabled;
                DwmIsCompositionEnabled(out dwmEnabled);
                return dwmEnabled;
            }
            return false;
        }

        public static Color ColorizationColor
        {
            get
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(COLORIZATION_COLOR_KEY, false))
                {
                    if (key != null)
                    {
                        object dwordValue = key.GetValue("ColorizationColor");
                        if (dwordValue != null)
                        {
                            return Color.FromArgb((Int32)dwordValue);
                        }
                    }
                }
                return Color.White;
            }
        }
    }
}