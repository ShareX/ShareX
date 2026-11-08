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

using ShareX.HelpersLib;
using System;
using System.Runtime.InteropServices;

// Windows display configuration (SDR white level), used by the HDR tone mapper. ShareX.Platform.Windows has its own copy for
// screenshots; it lives here rather than in ShareX.HelpersLib, which stays free of Windows APIs on the cross-platform branch.
namespace ShareX.ScreenRecordingLib.Video
{
    internal static class DisplayConfigHelper
    {
        private const uint QueryDisplayConfigOnlyActivePaths = 0x00000002;
        private const uint DisplayConfigGetSourceName = 1;
        private const uint DisplayConfigGetSdrWhiteLevel = 11;
        private const int DisplayConfigModeInfoSize = 64;

        public static float GetSdrWhiteScale(string deviceName)
        {
            try
            {
                if (GetDisplayConfigBufferSizes(QueryDisplayConfigOnlyActivePaths, out uint pathCount, out uint modeCount) != 0)
                {
                    return 1f;
                }

                DisplayConfigPathInfo[] paths = new DisplayConfigPathInfo[pathCount];
                IntPtr modes = Marshal.AllocHGlobal(checked((int)modeCount * DisplayConfigModeInfoSize));

                try
                {
                    if (QueryDisplayConfig(QueryDisplayConfigOnlyActivePaths, ref pathCount, paths,
                        ref modeCount, modes, IntPtr.Zero) != 0)
                    {
                        return 1f;
                    }

                    for (int i = 0; i < pathCount; i++)
                    {
                        DisplayConfigSourceDeviceName sourceName = new DisplayConfigSourceDeviceName
                        {
                            Header = new DisplayConfigDeviceInfoHeader
                            {
                                Type = DisplayConfigGetSourceName,
                                Size = (uint)Marshal.SizeOf<DisplayConfigSourceDeviceName>(),
                                AdapterId = paths[i].SourceInfo.AdapterId,
                                Id = paths[i].SourceInfo.Id
                            },
                            ViewGdiDeviceName = string.Empty
                        };

                        if (DisplayConfigGetDeviceInfo(ref sourceName) != 0 ||
                            !string.Equals(sourceName.ViewGdiDeviceName, deviceName, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        DisplayConfigSdrWhiteLevel whiteLevel = new DisplayConfigSdrWhiteLevel
                        {
                            Header = new DisplayConfigDeviceInfoHeader
                            {
                                Type = DisplayConfigGetSdrWhiteLevel,
                                Size = (uint)Marshal.SizeOf<DisplayConfigSdrWhiteLevel>(),
                                AdapterId = paths[i].TargetInfo.AdapterId,
                                Id = paths[i].TargetInfo.Id
                            }
                        };

                        if (DisplayConfigGetDeviceInfo(ref whiteLevel) == 0 && whiteLevel.SdrWhiteLevel > 0)
                        {
                            return Math.Clamp(whiteLevel.SdrWhiteLevel / 1000f, 1f, 125f);
                        }
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(modes);
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e, "Failed to query the HDR display SDR white level.");
            }

            return 1f;
        }

        [DllImport("user32.dll")]
        private static extern int GetDisplayConfigBufferSizes(uint flags, out uint pathCount, out uint modeCount);

        [DllImport("user32.dll")]
        private static extern int QueryDisplayConfig(uint flags, ref uint pathCount,
            [Out] DisplayConfigPathInfo[] paths, ref uint modeCount, IntPtr modes, IntPtr currentTopologyId);

        [DllImport("user32.dll", EntryPoint = "DisplayConfigGetDeviceInfo", CharSet = CharSet.Unicode)]
        private static extern int DisplayConfigGetDeviceInfo(ref DisplayConfigSourceDeviceName requestPacket);

        [DllImport("user32.dll", EntryPoint = "DisplayConfigGetDeviceInfo")]
        private static extern int DisplayConfigGetDeviceInfo(ref DisplayConfigSdrWhiteLevel requestPacket);

        [StructLayout(LayoutKind.Sequential)]
        private struct Luid
        {
            public uint LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigPathSourceInfo
        {
            public Luid AdapterId;
            public uint Id;
            public uint ModeInfoIdx;
            public uint StatusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigRational
        {
            public uint Numerator;
            public uint Denominator;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigPathTargetInfo
        {
            public Luid AdapterId;
            public uint Id;
            public uint ModeInfoIdx;
            public uint OutputTechnology;
            public uint Rotation;
            public uint Scaling;
            public DisplayConfigRational RefreshRate;
            public uint ScanLineOrdering;
            public int TargetAvailable;
            public uint StatusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigPathInfo
        {
            public DisplayConfigPathSourceInfo SourceInfo;
            public DisplayConfigPathTargetInfo TargetInfo;
            public uint Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigDeviceInfoHeader
        {
            public uint Type;
            public uint Size;
            public Luid AdapterId;
            public uint Id;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DisplayConfigSourceDeviceName
        {
            public DisplayConfigDeviceInfoHeader Header;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string ViewGdiDeviceName;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigSdrWhiteLevel
        {
            public DisplayConfigDeviceInfoHeader Header;
            public uint SdrWhiteLevel;
        }
    }
}
