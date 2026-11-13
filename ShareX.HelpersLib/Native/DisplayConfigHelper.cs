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

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ShareX.HelpersLib
{
    public static class DisplayConfigHelper
    {
        private const uint QueryDisplayConfigOnlyActivePaths = 0x00000002;
        private const uint DisplayConfigGetSourceName = 1;
        private const uint DisplayConfigGetTargetName = 2;
        private const uint DisplayConfigGetAdvancedColorInfo = 9;
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

        /// <summary>Lists the active display paths: the monitor behind each GDI source, its connection and its colour state.</summary>
        public static List<DisplayTargetInfo> GetDisplayTargets()
        {
            List<DisplayTargetInfo> targets = new List<DisplayTargetInfo>();

            if (!OperatingSystem.IsWindows() ||
                GetDisplayConfigBufferSizes(QueryDisplayConfigOnlyActivePaths, out uint pathCount, out uint modeCount) != 0)
            {
                return targets;
            }

            DisplayConfigPathInfo[] paths = new DisplayConfigPathInfo[pathCount];
            IntPtr modes = Marshal.AllocHGlobal(checked((int)modeCount * DisplayConfigModeInfoSize));

            try
            {
                if (QueryDisplayConfig(QueryDisplayConfigOnlyActivePaths, ref pathCount, paths, ref modeCount, modes, IntPtr.Zero) != 0)
                {
                    return targets;
                }

                for (int i = 0; i < pathCount; i++)
                {
                    DisplayConfigPathInfo path = paths[i];
                    DisplayTargetInfo target = new DisplayTargetInfo
                    {
                        OutputTechnology = GetOutputTechnologyName(path.TargetInfo.OutputTechnology),
                        Rotation = path.TargetInfo.Rotation switch { 2 => 90, 3 => 180, 4 => 270, _ => 0 },
                        RefreshRate = path.TargetInfo.RefreshRate.Denominator > 0
                            ? (double)path.TargetInfo.RefreshRate.Numerator / path.TargetInfo.RefreshRate.Denominator
                            : 0
                    };

                    DisplayConfigSourceDeviceName sourceName = new DisplayConfigSourceDeviceName
                    {
                        Header = CreateHeader(DisplayConfigGetSourceName, Marshal.SizeOf<DisplayConfigSourceDeviceName>(),
                            path.SourceInfo.AdapterId, path.SourceInfo.Id),
                        ViewGdiDeviceName = string.Empty
                    };

                    if (DisplayConfigGetDeviceInfo(ref sourceName) == 0)
                    {
                        target.SourceDeviceName = sourceName.ViewGdiDeviceName;
                    }

                    DisplayConfigTargetDeviceName targetName = new DisplayConfigTargetDeviceName
                    {
                        Header = CreateHeader(DisplayConfigGetTargetName, Marshal.SizeOf<DisplayConfigTargetDeviceName>(),
                            path.TargetInfo.AdapterId, path.TargetInfo.Id),
                        MonitorFriendlyDeviceName = string.Empty,
                        MonitorDevicePath = string.Empty
                    };

                    if (DisplayConfigGetDeviceInfo(ref targetName) == 0)
                    {
                        target.FriendlyName = targetName.MonitorFriendlyDeviceName;
                    }

                    DisplayConfigAdvancedColorInfo colorInfo = new DisplayConfigAdvancedColorInfo
                    {
                        Header = CreateHeader(DisplayConfigGetAdvancedColorInfo, Marshal.SizeOf<DisplayConfigAdvancedColorInfo>(),
                            path.TargetInfo.AdapterId, path.TargetInfo.Id)
                    };

                    if (DisplayConfigGetDeviceInfo(ref colorInfo) == 0)
                    {
                        target.AdvancedColorSupported = (colorInfo.Value & 0x1) != 0;
                        target.AdvancedColorEnabled = (colorInfo.Value & 0x2) != 0;
                        target.BitsPerColorChannel = (int)colorInfo.BitsPerColorChannel;
                    }

                    DisplayConfigSdrWhiteLevel whiteLevel = new DisplayConfigSdrWhiteLevel
                    {
                        Header = CreateHeader(DisplayConfigGetSdrWhiteLevel, Marshal.SizeOf<DisplayConfigSdrWhiteLevel>(),
                            path.TargetInfo.AdapterId, path.TargetInfo.Id)
                    };

                    if (DisplayConfigGetDeviceInfo(ref whiteLevel) == 0 && whiteLevel.SdrWhiteLevel > 0)
                    {
                        // SdrWhiteLevel is in thousandths of 80 nits.
                        target.SdrWhiteLevelNits = whiteLevel.SdrWhiteLevel * 80 / 1000;
                    }

                    targets.Add(target);
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e, "Failed to query the display configuration.");
            }
            finally
            {
                Marshal.FreeHGlobal(modes);
            }

            return targets;
        }

        private static DisplayConfigDeviceInfoHeader CreateHeader(uint type, int size, Luid adapterId, uint id) =>
            new DisplayConfigDeviceInfoHeader { Type = type, Size = (uint)size, AdapterId = adapterId, Id = id };

        private static string GetOutputTechnologyName(uint value) => value switch
        {
            0xFFFFFFFF => "Other",
            0 => "VGA",
            1 => "S-Video",
            2 => "Composite video",
            3 => "Component video",
            4 => "DVI",
            5 => "HDMI",
            6 => "LVDS",
            8 => "D-Jpn",
            9 => "SDI",
            10 => "DisplayPort",
            11 => "Embedded DisplayPort",
            12 => "UDI",
            13 => "Embedded UDI",
            14 => "SDTV dongle",
            15 => "Miracast",
            16 => "Indirect (wired)",
            17 => "Indirect (virtual)",
            18 => "DisplayPort (USB tunnel)",
            0x80000000 => "Internal",
            _ => $"Unknown ({value})"
        };

        [DllImport("user32.dll")]
        private static extern int GetDisplayConfigBufferSizes(uint flags, out uint pathCount, out uint modeCount);

        [DllImport("user32.dll")]
        private static extern int QueryDisplayConfig(uint flags, ref uint pathCount,
            [Out] DisplayConfigPathInfo[] paths, ref uint modeCount, IntPtr modes, IntPtr currentTopologyId);

        [DllImport("user32.dll", EntryPoint = "DisplayConfigGetDeviceInfo", CharSet = CharSet.Unicode)]
        private static extern int DisplayConfigGetDeviceInfo(ref DisplayConfigSourceDeviceName requestPacket);

        [DllImport("user32.dll", EntryPoint = "DisplayConfigGetDeviceInfo")]
        private static extern int DisplayConfigGetDeviceInfo(ref DisplayConfigSdrWhiteLevel requestPacket);

        [DllImport("user32.dll", EntryPoint = "DisplayConfigGetDeviceInfo", CharSet = CharSet.Unicode)]
        private static extern int DisplayConfigGetDeviceInfo(ref DisplayConfigTargetDeviceName requestPacket);

        [DllImport("user32.dll", EntryPoint = "DisplayConfigGetDeviceInfo")]
        private static extern int DisplayConfigGetDeviceInfo(ref DisplayConfigAdvancedColorInfo requestPacket);

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

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DisplayConfigTargetDeviceName
        {
            public DisplayConfigDeviceInfoHeader Header;
            public uint Flags;
            public uint OutputTechnology;
            public ushort EdidManufactureId;
            public ushort EdidProductCodeId;
            public uint ConnectorInstance;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string MonitorFriendlyDeviceName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string MonitorDevicePath;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigAdvancedColorInfo
        {
            public DisplayConfigDeviceInfoHeader Header;
            public uint Value;
            public uint ColorEncoding;
            public uint BitsPerColorChannel;
        }
    }

    public sealed class DisplayTargetInfo
    {
        /// <summary>GDI device name of the source, such as \\.\DISPLAY1.</summary>
        public string SourceDeviceName { get; set; } = string.Empty;
        public string FriendlyName { get; set; } = string.Empty;
        public string OutputTechnology { get; set; } = string.Empty;
        /// <summary>Clockwise rotation in degrees.</summary>
        public int Rotation { get; set; }
        public double RefreshRate { get; set; }
        public bool AdvancedColorSupported { get; set; }
        public bool AdvancedColorEnabled { get; set; }
        public int BitsPerColorChannel { get; set; }
        public uint SdrWhiteLevelNits { get; set; }
    }
}
