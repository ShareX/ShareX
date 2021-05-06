using BetterWin32Errors;
using System;
using System.Runtime.InteropServices;

namespace ShareX.ScreenCaptureLib.AdvancedGraphics.GDI
{
    public static class GdiInterop
    {
        [DllImport("User32.dll")]
        private static extern Win32Error DisplayConfigSetDeviceInfo(IntPtr requestPacket);
        public static Win32Error DisplayConfigSetDeviceInfo<T>(ref T displayConfig)
           where T : IDisplayConfigInfo
        {
            return WrapStructureAndCall(ref displayConfig, DisplayConfigSetDeviceInfo);
        }

        [DllImport("User32.dll")]
        private static extern Win32Error DisplayConfigGetDeviceInfo(IntPtr targetDeviceName);
        public static Win32Error DisplayConfigGetDeviceInfo<T>(ref T displayConfig)
          where T : IDisplayConfigInfo
        {
            return WrapStructureAndCall(ref displayConfig, DisplayConfigGetDeviceInfo);
        }

        [DllImport("User32.dll")]
        public static extern Win32Error GetDisplayConfigBufferSizes(QDC_CONSTANT flags,
            ref uint numPathArrayElements, ref uint numModeInfoArrayElements);

        [DllImport("User32.dll")]
        public static extern unsafe Win32Error QueryDisplayConfig(
           QDC_CONSTANT Flags,
           ref uint pNumPathArrayElements,
           [Out] DISPLAYCONFIG_PATH_INFO[] pPathInfoArray,
           ref uint pNumModeInfoArrayElements,
           [Out] DISPLAYCONFIG_MODE_INFO[] pModeInfoArray,
           IntPtr pCurrentTopologyId);

        public static Win32Error WrapStructureAndCall<T>(ref T displayConfig,
            Func<IntPtr, Win32Error> func)
            where T : IDisplayConfigInfo
        {
            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(displayConfig));
            Marshal.StructureToPtr(displayConfig, ptr, false);

            var retval = func(ptr);

            displayConfig = (T)Marshal.PtrToStructure(ptr, displayConfig.GetType());

            Marshal.FreeHGlobal(ptr);
            return retval;
        }
    }
}
