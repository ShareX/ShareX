// ImageListView - A listview control for image files
// Copyright (C) 2009 Ozgur Ozcitak
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Ozgur Ozcitak (ozcitak@yahoo.com)

using System.Runtime.InteropServices;

namespace ShareX.ImageListView;

/// <summary>
/// Reads shell icons and shell file types.
/// </summary>
public partial class GDIExtractor : IExtractor
{
    #region Platform Invoke
    // GetFileAttributesEx
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetFileAttributesEx(string lpFileName,
        GET_FILEEX_INFO_LEVELS fInfoLevelId,
        out WIN32_FILE_ATTRIBUTE_DATA fileData);

    private enum GET_FILEEX_INFO_LEVELS
    {
        GetFileExInfoStandard,
        GetFileExMaxInfoLevel
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct WIN32_FILE_ATTRIBUTE_DATA
    {
        public FileAttributes dwFileAttributes;
        public FILETIME ftCreationTime;
        public FILETIME ftLastAccessTime;
        public FILETIME ftLastWriteTime;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct FILETIME
    {
        public uint dwLowDateTime;
        public uint dwHighDateTime;

        public DateTime Value
        {
            get
            {
                long longTime = (((long)dwHighDateTime) << 32) | ((uint)dwLowDateTime);
                return DateTime.FromFileTimeUtc(longTime);
            }
        }
    }
    // DestroyIcon
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyIcon(IntPtr hIcon);
    // SHGetFileInfo
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SHGetFileInfo(string pszPath, FileAttributes dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, SHGFI uFlags);
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct SHFILEINFO
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_TYPE)]
        public string szTypeName;
    };
    private const int MAX_PATH = 260;
    private const int MAX_TYPE = 80;
    [Flags]
    private enum SHGFI : uint
    {
        Icon = 0x000000100,
        DisplayName = 0x000000200,
        TypeName = 0x000000400,
        Attributes = 0x000000800,
        IconLocation = 0x000001000,
        ExeType = 0x000002000,
        SysIconIndex = 0x000004000,
        LinkOverlay = 0x000008000,
        Selected = 0x000010000,
        Attr_Specified = 0x000020000,
        LargeIcon = 0x000000000,
        SmallIcon = 0x000000001,
        OpenIcon = 0x000000002,
        ShellIconSize = 0x000000004,
        PIDL = 0x000000008,
        UseFileAttributes = 0x000000010,
        AddOverlays = 0x000000020,
        OverlayIndex = 0x000000040,
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Returns shell info.
    /// </summary>
    /// <param name="path">Filepath of image</param>
    public virtual ShellInfo GetShellInfo(string path)
    {
        ShellInfo info = new();

        try
        {
            SHFILEINFO shinfo = new();
            uint structSize = (uint)Marshal.SizeOf(shinfo);
            SHGFI flags = SHGFI.Icon | SHGFI.SmallIcon | SHGFI.TypeName | SHGFI.UseFileAttributes;

            // Get the small icon and shell file type
            IntPtr hImg = SHGetFileInfo(path, FileAttributes.Normal, out shinfo,
                structSize, flags);

            // Get mime type
            info.FileType = shinfo.szTypeName;

            // Get small icon 
            if (hImg != IntPtr.Zero && shinfo.hIcon != IntPtr.Zero)
            {
                using (Icon newIcon = System.Drawing.Icon.FromHandle(shinfo.hIcon))
                {
                    info.SmallIcon = newIcon.ToBitmap();
                }
                DestroyIcon(shinfo.hIcon);
            } else
                info.Error = new Exception("Error reading shell icon");

            // Get large icon
            hImg = SHGetFileInfo(path, FileAttributes.Normal, out shinfo,
                structSize, SHGFI.Icon | SHGFI.LargeIcon | SHGFI.UseFileAttributes);

            if (hImg != IntPtr.Zero && shinfo.hIcon != IntPtr.Zero)
            {
                using (Icon newIcon = System.Drawing.Icon.FromHandle(shinfo.hIcon))
                {
                    info.LargeIcon = newIcon.ToBitmap();
                }
                DestroyIcon(shinfo.hIcon);
            } else
                info.Error = new Exception("Error reading shell icon");
        } catch (Exception e)
        {
            info.Error = e;
        }

        return info;
    }
    #endregion
}
