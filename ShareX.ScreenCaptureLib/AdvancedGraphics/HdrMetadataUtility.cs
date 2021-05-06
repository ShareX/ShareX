using System;
using ShareX.ScreenCaptureLib.AdvancedGraphics.GDI;

namespace ShareX.ScreenCaptureLib.AdvancedGraphics
{
    public static class HdrMetadataUtility
    {
        public static ShaderHdrMetadata GetTestSettings()
        {
            return new ShaderHdrMetadata { EnableHdrProcessing = true, MonHdrDispNits = 800.0f, MonSdrDispNits = 270.0f, ExposureLevel = 1.0f };
        }

        public static ShaderHdrMetadata GetHdrMetadataForMonitor(string deviceName)
        {
            var err = BetterWin32Errors.Win32Error.ERROR_SUCCESS;
            // TODO: also query monitor HDR white point brightness (currently hardcoded to 800.0f)
            var hdrMetadata = new ShaderHdrMetadata { EnableHdrProcessing = false, MonSdrDispNits = 80.0f, ExposureLevel = 1.0f, MonHdrDispNits = 800.0f };
            var monAdvColorInfo = DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO.CreateGet();
            var monSdrWhiteLevel = DISPLAYCONFIG_SDR_WHITE_LEVEL.CreateGet();
            uint numPathArrayElements = 0;
            uint numModeInfoArrayElements = 0;

            err = GdiInterop.GetDisplayConfigBufferSizes(QDC_CONSTANT.QDC_ONLY_ACTIVE_PATHS,
                ref numPathArrayElements, ref numModeInfoArrayElements);
            if (err != BetterWin32Errors.Win32Error.ERROR_SUCCESS)
            {
                throw new System.ComponentModel.Win32Exception((int)err);
            }

            var displayPathInfoArray = new DISPLAYCONFIG_PATH_INFO[numPathArrayElements];
            var displayModeInfoArray = new DISPLAYCONFIG_MODE_INFO[numModeInfoArrayElements];

            err = GdiInterop.QueryDisplayConfig(QDC_CONSTANT.QDC_ONLY_ACTIVE_PATHS,
                    ref numPathArrayElements, displayPathInfoArray,
                    ref numModeInfoArrayElements, displayModeInfoArray, IntPtr.Zero);
            if (err != BetterWin32Errors.Win32Error.ERROR_SUCCESS)
            {
                throw new System.ComponentModel.Win32Exception((int)err);
            }

            for (uint pathIdx = 0; pathIdx < numPathArrayElements; pathIdx++)
            {
                DISPLAYCONFIG_SOURCE_DEVICE_NAME srcName = DISPLAYCONFIG_SOURCE_DEVICE_NAME.CreateGet();
                srcName.header.adapterId.HighPart = displayPathInfoArray[pathIdx].sourceInfo.adapterId.HighPart;
                srcName.header.adapterId.LowPart = displayPathInfoArray[pathIdx].sourceInfo.adapterId.LowPart;
                srcName.header.id = displayPathInfoArray[pathIdx].sourceInfo.id;

                err = GdiInterop.DisplayConfigGetDeviceInfo(ref srcName);
                if (err != BetterWin32Errors.Win32Error.ERROR_SUCCESS)
                {
                    throw new System.ComponentModel.Win32Exception((int)err);
                }

                if (srcName.DeviceName == deviceName)
                {
                    // If matches, proceed to query color information
                    monAdvColorInfo.header.adapterId.HighPart = displayPathInfoArray[pathIdx].targetInfo.adapterId.HighPart;
                    monAdvColorInfo.header.adapterId.LowPart = displayPathInfoArray[pathIdx].targetInfo.adapterId.LowPart;
                    monAdvColorInfo.header.id = displayPathInfoArray[pathIdx].targetInfo.id;

                    monSdrWhiteLevel.header.adapterId.HighPart = displayPathInfoArray[pathIdx].targetInfo.adapterId.HighPart;
                    monSdrWhiteLevel.header.adapterId.LowPart = displayPathInfoArray[pathIdx].targetInfo.adapterId.LowPart;
                    monSdrWhiteLevel.header.id = displayPathInfoArray[pathIdx].targetInfo.id;

                    err = GdiInterop.DisplayConfigGetDeviceInfo(ref monAdvColorInfo);
                    if (err != BetterWin32Errors.Win32Error.ERROR_SUCCESS)
                    {
                        throw new System.ComponentModel.Win32Exception((int)err);
                    }

                    hdrMetadata.EnableHdrProcessing = (monAdvColorInfo.AdvancedColorStatus & AdvancedColorStatus.AdvancedColorEnabled) == AdvancedColorStatus.AdvancedColorEnabled;
                    if (hdrMetadata.EnableHdrProcessing)
                    {
                        err = GdiInterop.DisplayConfigGetDeviceInfo(ref monSdrWhiteLevel);
                        if (err != BetterWin32Errors.Win32Error.ERROR_SUCCESS)
                        {
                            throw new System.ComponentModel.Win32Exception((int)err);
                        }

                        hdrMetadata.MonSdrDispNits = monSdrWhiteLevel.SDRWhiteLevelInNits;
                    }

                    break;
                }
            }

            return hdrMetadata;
        }
    }
}
