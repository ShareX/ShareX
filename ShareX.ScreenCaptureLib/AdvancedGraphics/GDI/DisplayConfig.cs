using System;
using System.Runtime.InteropServices;

namespace ShareX.ScreenCaptureLib.AdvancedGraphics.GDI
{
    [Flags]
    public enum QDC_CONSTANT : uint
    {
        QDC_ALL_PATHS = 0x1,
        QDC_ONLY_ACTIVE_PATHS = 0x2,
        QDC_DATABASE_CURRENT = 0x4,
        QDC_VIRTUAL_MODE_AWARE = 0x10,
        QDC_INCLUDE_HMD = 0x20,
    }

    public enum DISPLAYCONFIG_DEVICE_INFO_TYPE : uint
    {
        DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME = 1,
        DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME = 2,
        DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE = 3,
        DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME = 4,
        DISPLAYCONFIG_DEVICE_INFO_SET_TARGET_PERSISTENCE = 5,
        DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE = 6,
        DISPLAYCONFIG_DEVICE_INFO_GET_SUPPORT_VIRTUAL_RESOLUTION = 7,
        DISPLAYCONFIG_DEVICE_INFO_SET_SUPPORT_VIRTUAL_RESOLUTION = 8,
        DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO = 9,
        DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE = 10,
        DISPLAYCONFIG_DEVICE_INFO_GET_SDR_WHITE_LEVEL = 11,
        DISPLAYCONFIG_DEVICE_INFO_FORCE_UINT32 = 0xFFFFFFFF,
    }

    public enum DISPLAYCONFIG_COLOR_ENCODING : uint
    {
        DISPLAYCONFIG_COLOR_ENCODING_RGB = 0,
        DISPLAYCONFIG_COLOR_ENCODING_YCBCR444 = 1,
        DISPLAYCONFIG_COLOR_ENCODING_YCBCR422 = 2,
        DISPLAYCONFIG_COLOR_ENCODING_YCBCR420 = 3,
        DISPLAYCONFIG_COLOR_ENCODING_INTENSITY = 4,
        DISPLAYCONFIG_COLOR_ENCODING_FORCE_UINT32 = 0xFFFFFFFF,
    }

    [Flags]
    public enum DISPLAYCONFIG_PATH_SOURCE_INFO_FLAGS
    {
        None = 0,
        DISPLAYCONFIG_SOURCE_IN_USE = 1,
    }

    public enum DISPLAYCONFIG_MODE_INFO_TYPE
    {
        DISPLAYCONFIG_MODE_INFO_TYPE_FORCE_UINT32 = -1,
        DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 1,
        DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 2,
        DISPLAYCONFIG_MODE_INFO_TYPE_DESKTOP_IMAGE = 3,
    }

    [Flags]
    public enum DISPLAYCONFIG_PATH_TARGET_INFO_FLAGS
    {
        None = 0,
        DISPLAYCONFIG_TARGET_IN_USE = 1,
        DISPLAYCONFIG_TARGET_FORCIBLE = 2,
        DISPLAYCONFIG_TARGET_FORCED_AVAILABILITY_BOOT = 4,
        DISPLAYCONFIG_TARGET_FORCED_AVAILABILITY_PATH = 8,
        DISPLAYCONFIG_TARGET_FORCED_AVAILABILITY_SYSTEM = 16,
    }

    public enum DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY
    {
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL = int.MinValue,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER = -1,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_FORCE_UINT32 = -1,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HD15 = 0,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SVIDEO = 1,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPOSITE_VIDEO = 2,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPONENT_VIDEO = 3,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DVI = 4,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HDMI = 5,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_LVDS = 6,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_D_JPN = 8,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDI = 9,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL = 10,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED = 11,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EXTERNAL = 12,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EMBEDDED = 13,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDTVDONGLE = 14,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_MIRACAST = 15,
    }
    public enum DISPLAYCONFIG_ROTATION
    {
        DISPLAYCONFIG_ROTATION_FORCE_UINT32 = -1,
        DISPLAYCONFIG_ROTATION_IDENTITY = 1,
        DISPLAYCONFIG_ROTATION_ROTATE90 = 2,
        DISPLAYCONFIG_ROTATION_ROTATE180 = 3,
        DISPLAYCONFIG_ROTATION_ROTATE270 = 4,
    }

    public enum DISPLAYCONFIG_SCANLINE_ORDERING
    {
        DISPLAYCONFIG_SCANLINE_ORDERING_FORCE_UINT32 = -1,
        DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED = 0,
        DISPLAYCONFIG_SCANLINE_ORDERING_PROGRESSIVE = 1,
        DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED = 2,
        DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST = 2,
        DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST = 3,
    }

    public enum DISPLAYCONFIG_SCALING
    {
        DISPLAYCONFIG_SCALING_FORCE_UINT32 = -1,
        DISPLAYCONFIG_SCALING_IDENTITY = 1,
        DISPLAYCONFIG_SCALING_CENTERED = 2,
        DISPLAYCONFIG_SCALING_STRETCHED = 3,
        DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX = 4,
        DISPLAYCONFIG_SCALING_CUSTOM = 5,
        DISPLAYCONFIG_SCALING_PREFERRED = 128,
    }

    [Flags]
    public enum AdvancedColorStatus : uint
    {
        AdvancedColorNone = 0,
        AdvancedColorSupported = 1 << 0,
        AdvancedColorEnabled = 1 << 1,
        WideColorEnforced = 1 << 2,
        AdvancedColorForceDisabled = 1 << 3,
    }

    public enum DISPLAYCONFIG_PIXELFORMAT
    {
        DISPLAYCONFIG_PIXELFORMAT_FORCE_UINT32 = -1,
        DISPLAYCONFIG_PIXELFORMAT_8BPP = 1,
        DISPLAYCONFIG_PIXELFORMAT_16BPP = 2,
        DISPLAYCONFIG_PIXELFORMAT_24BPP = 3,
        DISPLAYCONFIG_PIXELFORMAT_32BPP = 4,
        DISPLAYCONFIG_PIXELFORMAT_NONGDI = 5,
    }

    public enum DISPLAYCONFIG_PATH
    {
        DISPLAYCONFIG_PATH_ACTIVE = 0x00000001,
        DISPLAYCONFIG_PATH_PREFERRED_UNSCALED = 0x00000004,
        DISPLAYCONFIG_PATH_SUPPORT_VIRTUAL_MODE = 0x00000008,
    }

    public interface IDisplayConfigInfo
    {
        // Nothing here
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_DEVICE_INFO_HEADER : IDisplayConfigInfo
    {
        public DISPLAYCONFIG_DEVICE_INFO_TYPE type;
        public uint size;
        public LUID adapterId;
        public uint id;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO : IDisplayConfigInfo
    {
        public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
        public AdvancedColorStatus AdvancedColorStatus;
        public DISPLAYCONFIG_COLOR_ENCODING ColorEncoding;
        public uint BitsPerColorChannel;

        public static DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO CreateGet()
        {
            DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO s = new DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO();
            s.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO;
            s.header.size = (uint)Marshal.SizeOf(s);
            return s;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_SDR_WHITE_LEVEL : IDisplayConfigInfo
    {
        public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
        public uint SDRWhiteLevel;

        public float SDRWhiteLevelInNits => (SDRWhiteLevel / 1000) * 80;

        public static DISPLAYCONFIG_SDR_WHITE_LEVEL CreateGet()
        {
            DISPLAYCONFIG_SDR_WHITE_LEVEL s = new DISPLAYCONFIG_SDR_WHITE_LEVEL();
            s.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SDR_WHITE_LEVEL;
            s.header.size = (uint)Marshal.SizeOf(s);
            return s;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DISPLAYCONFIG_SOURCE_DEVICE_NAME : IDisplayConfigInfo
    {
        public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = GdiInteropConstants.CCHDEVICENAME)]
        public string DeviceName;

        public static DISPLAYCONFIG_SOURCE_DEVICE_NAME CreateGet()
        {
            DISPLAYCONFIG_SOURCE_DEVICE_NAME s = new DISPLAYCONFIG_SOURCE_DEVICE_NAME();
            s.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME;
            s.header.size = (uint)Marshal.SizeOf(s);
            return s;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_PATH_SOURCE_INFO
    {
        public LUID adapterId;
        public uint id;
        public uint modeInfoIdx;
        public DISPLAYCONFIG_PATH_SOURCE_INFO_FLAGS statusFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_RATIONAL
    {
        public uint Numerator;
        public uint Denominator;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_PATH_TARGET_INFO
    {
        public LUID adapterId;
        public uint id;
        public uint modeInfoIdx;
        public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
        public DISPLAYCONFIG_ROTATION rotation;
        public DISPLAYCONFIG_SCALING scaling;
        public DISPLAYCONFIG_RATIONAL refreshRate;
        public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
        public bool targetAvailable;
        public DISPLAYCONFIG_PATH_TARGET_INFO_FLAGS statusFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_PATH_INFO
    {
        public DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo;
        public DISPLAYCONFIG_PATH_TARGET_INFO targetInfo;
        public DISPLAYCONFIG_PATH flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_2DREGION
    {
        public uint cx;
        public uint cy;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_ADDITIONAL_SIGNAL_INFO
    {
        public ushort videoStandard;

        public int vSyncFreqDivider { get; set; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO
    {
        public ulong pixelRate;
        public DISPLAYCONFIG_RATIONAL hSyncFreq;
        public DISPLAYCONFIG_RATIONAL vSyncFreq;
        public DISPLAYCONFIG_2DREGION activeSize;
        public DISPLAYCONFIG_2DREGION totalSize;
        public DISPLAYCONFIG_ADDITIONAL_SIGNAL_INFO AdditionalSignalInfo;
        public uint videoStandard;
        public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_TARGET_MODE
    {
        public DISPLAYCONFIG_VIDEO_SIGNAL_INFO targetVideoSignalInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_SOURCE_MODE
    {
        public uint width;
        public uint height;
        public DISPLAYCONFIG_PIXELFORMAT pixelFormat;
        public POINT position;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_DESKTOP_IMAGE_INFO
    {
        public POINT PathSourceSize;
        public RECT DesktopImageRegion;
        public RECT DesktopImageClip;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_MODE_INFO
    {
        public DISPLAYCONFIG_MODE_INFO_TYPE infoType;
        public uint id;
        public LUID adapterId;
        public DISPLAYCONFIG_TARGET_MODE targetMode;
        public DISPLAYCONFIG_SOURCE_MODE sourceMode;
        public DISPLAYCONFIG_DESKTOP_IMAGE_INFO desktopImageInfo;
    }
}
