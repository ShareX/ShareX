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

namespace ShareX.ScreenCaptureLib
{
    public class RegionCaptureOptions
    {
        public const int DefaultMinimumSize = 5;
        public const int MagnifierPixelCountMinimum = 3;
        public const int MagnifierPixelCountMaximum = 35;
        public const int MagnifierPixelSizeMinimum = 3;
        public const int MagnifierPixelSizeMaximum = 30;
        public const int MoveSpeedMinimum = 1;
        public const int MoveSpeedMaximum = 10;

        public bool QuickCapture = true;
        public int MinimumSize = DefaultMinimumSize;
        public RegionCaptureAction RegionCaptureActionRightClick = RegionCaptureAction.RemoveShapeCancelCapture;
        public RegionCaptureAction RegionCaptureActionMiddleClick = RegionCaptureAction.SwapToolType;
        public RegionCaptureAction RegionCaptureActionX1Click = RegionCaptureAction.CaptureFullscreen;
        public RegionCaptureAction RegionCaptureActionX2Click = RegionCaptureAction.CaptureActiveMonitor;
        public bool DetectWindows = true;
        public bool DetectControls = true;
        public int BackgroundDimStrength = 20;
        public bool UseCustomInfoText = false;
        public string CustomInfoText = "X: $x, Y: $y$nR: $r, G: $g, B: $b$nHex: $hex"; // Formats: $x, $y, $r, $g, $b, $hex, $HEX, $n
        public bool ShowInfo = true;
        public bool ShowMagnifier = true;
        public bool UseSquareMagnifier = false;
        public int MagnifierPixelCount = 15; // Must be odd number like 11, 13, 15 etc.
        public int MagnifierPixelSize = 10;
        public bool ShowCenterCrosshair = false;
        public bool ShowCrosshair = false;
        public int InputDelay = 500;
        public bool ActiveMonitorMode = false;

        // Color picker
        public ColorPickerOptions ColorPickerOptions = new ColorPickerOptions();

        // Screen color picker
        public string ScreenColorPickerInfoText = "";
    }
}
