#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using ShareX.ImageEffectsLib;
using System.Collections.Generic;
using System.Drawing;

namespace ShareX.ScreenCaptureLib
{
    public class RegionCaptureOptions
    {
        public const int DefaultMinimumSize = 5;
        public const int MagnifierPixelCountMinimum = 3;
        public const int MagnifierPixelCountMaximum = 35;
        public const int MagnifierPixelSizeMinimum = 3;
        public const int MagnifierPixelSizeMaximum = 30;
        public const int SnapDistance = 30;
        public const int MoveSpeedMinimum = 1;
        public const int MoveSpeedMaximum = 10;

        public bool QuickCrop = true;
        public int MinimumSize = DefaultMinimumSize;
        public RegionCaptureAction RegionCaptureActionRightClick = RegionCaptureAction.RemoveShapeCancelCapture;
        public RegionCaptureAction RegionCaptureActionMiddleClick = RegionCaptureAction.SwapToolType;
        public RegionCaptureAction RegionCaptureActionX1Click = RegionCaptureAction.CaptureFullscreen;
        public RegionCaptureAction RegionCaptureActionX2Click = RegionCaptureAction.CaptureActiveMonitor;
        public bool DetectWindows = true;
        public bool DetectControls = true;
        // TEMP: For backward compatibility
        public bool UseDimming = true;
        public int BackgroundDimStrength = 10;
        public bool UseCustomInfoText = false;
        public string CustomInfoText = "X: $x, Y: $y$nR: $r, G: $g, B: $b$nHex: $hex"; // Formats: $x, $y, $r, $g, $b, $hex, $HEX, $n
        public List<SnapSize> SnapSizes = new List<SnapSize>()
        {
            new SnapSize(426, 240), // 240p
            new SnapSize(640, 360), // 360p
            new SnapSize(854, 480), // 480p
            new SnapSize(1280, 720), // 720p
            new SnapSize(1920, 1080) // 1080p
        };
        public bool ShowInfo = true;
        public bool ShowMagnifier = true;
        public bool UseSquareMagnifier = false;
        public int MagnifierPixelCount = 15; // Must be odd number like 11, 13, 15 etc.
        public int MagnifierPixelSize = 10;
        public bool ShowCrosshair = false;
        public bool UseLightResizeNodes = false;
        public bool EnableAnimations = true;
        public bool IsFixedSize = false;
        public Size FixedSize = new Size(250, 250);
        public bool ShowFPS = false;
        public int FPSLimit = 100;
        public int MenuIconSize = 0;
        public bool MenuLocked = false;
        public bool RememberMenuState = false;
        public bool MenuCollapsed = false;
        public Point MenuPosition = Point.Empty;
        public int InputDelay = 500;
        public bool SwitchToDrawingToolAfterSelection = false;
        public bool SwitchToSelectionToolAfterDrawing = false;
        public bool ActiveMonitorMode = false;

        // Annotation
        public AnnotationOptions AnnotationOptions = new AnnotationOptions();
        public ShapeType LastRegionTool = ShapeType.RegionRectangle;
        public ShapeType LastAnnotationTool = ShapeType.DrawingRectangle;
        public ShapeType LastEditorTool = ShapeType.DrawingRectangle;

        // Image editor
        public ImageEditorStartMode ImageEditorStartMode = ImageEditorStartMode.AutoSize;
        public WindowState ImageEditorWindowState = new WindowState();
        public bool ZoomToFitOnOpen = false;
        public bool EditorAutoCopyImage = false;
        public bool AutoCloseEditorOnTask = false;
        public bool ShowEditorPanTip = true;
        public ImageInterpolationMode ImageEditorResizeInterpolationMode = ImageInterpolationMode.Bicubic;
        public Size EditorNewImageSize = new Size(800, 600);
        public bool EditorNewImageTransparent = false;
        public Color EditorNewImageBackgroundColor = Color.White;
        public Color EditorCanvasColor = Color.Transparent;
        public List<ImageEffectPreset> ImageEffectPresets = new List<ImageEffectPreset>();
        public int SelectedImageEffectPreset = 0;

        // Color picker
        public ColorPickerOptions ColorPickerOptions = new ColorPickerOptions();

        // Screen color picker
        public string ScreenColorPickerInfoText = "";
    }
}