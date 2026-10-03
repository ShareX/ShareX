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

using ShareX.AvaloniaUI.Windows;
using ShareX.Platform;
using ShareX.Tools;

namespace ShareX;

/// <summary>Capabilities used by task menus. Saved tasks stay intact when a platform cannot execute them.</summary>
internal static class TaskFeatureSupport
{
    public static FeatureSupport Get(HotkeyType task) => task switch
    {
        HotkeyType.PrintScreen or HotkeyType.ActiveMonitor or HotkeyType.RectangleRegion or
        HotkeyType.CustomRegion or HotkeyType.LastRegion or HotkeyType.AutoCapture or
        HotkeyType.StartAutoCapture or HotkeyType.Ruler or HotkeyType.PinToScreenFromScreen or
        HotkeyType.QRCodeDecodeFromScreen or HotkeyType.QRCodeScanRegion => PlatformServices.Current.ScreenCapture.Support,

        HotkeyType.ScreenColorPicker => ScreenColorPickerAvailability.Support,

        HotkeyType.ActiveWindow or HotkeyType.CustomWindow => Require(
            PlatformServices.Current.ScreenCapture.Support, PlatformServices.Current.Windows.Support),

        HotkeyType.ScrollingCapture => ScrollingCapture,

        HotkeyType.ScreenRecorder or HotkeyType.ScreenRecorderCustomRegion or HotkeyType.StartScreenRecorder or
        HotkeyType.ScreenRecorderGIF or HotkeyType.ScreenRecorderGIFCustomRegion or HotkeyType.StartScreenRecorderGIF => Require(
            PlatformServices.Current.ScreenRecording.Support, PlatformServices.Current.ScreenCapture.Support),

        HotkeyType.ScreenRecorderActiveWindow or HotkeyType.ScreenRecorderGIFActiveWindow => Require(
            PlatformServices.Current.ScreenRecording.Support, PlatformServices.Current.ScreenCapture.Support,
            PlatformServices.Current.Windows.Support),

        HotkeyType.MouseHighlighter => MouseHighlighterManager.Support,
        HotkeyType.InspectWindow => PlatformServices.Current.WindowManagement.GetSupport(WindowManagementFeature.Inspect),
        HotkeyType.ActiveWindowTopMost => PlatformServices.Current.WindowManagement.GetSupport(WindowManagementFeature.TopMost),
        HotkeyType.BorderlessWindow or HotkeyType.ActiveWindowBorderless => PlatformServices.Current.WindowManagement.GetSupport(WindowManagementFeature.Borderless),
        // OCR tasks also accept image files; capture support is checked separately on the screen OCR menu.
        HotkeyType.OCR => PlatformServices.Current.Ocr.Support,
        _ => FeatureSupport.Supported
    };

    public static FeatureSupport Require(params FeatureSupport[] features)
    {
        foreach (FeatureSupport feature in features)
        {
            if (!feature.IsSupported)
            {
                return feature;
            }
        }
        return FeatureSupport.Supported;
    }

    private static FeatureSupport ScrollingCapture
    {
        get
        {
            IPlatformServices platform = PlatformServices.Current;
            IInputService input = platform.Input;
            FeatureSupport scroll = input.MouseWheelSupport.IsSupported || input.KeyboardSupport.IsSupported || input.WindowScrollSupport.IsSupported
                ? FeatureSupport.Supported : input.MouseWheelSupport;
            return Require(platform.ScreenCapture.Support, platform.Windows.Support, scroll);
        }
    }
}
