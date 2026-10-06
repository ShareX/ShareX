// SPDX-License-Identifier: GPL-3.0-or-later
namespace ShareX.ScreenRecordingLib.Native;

internal static partial class NativeMethods
{
    // Optional ICodecAPI properties from codecapi.h; Vortice does not currently wrap this interface.
    public static readonly Guid CODECAPI_AVEncCommonLowLatency = new("9d3ecd55-89e8-490a-970a-0c9548d5a56e");
    public static readonly Guid CODECAPI_AVEncMPVDefaultBPictureCount = new("8d390aac-dc5c-4200-b57f-814d04babab2");
}
