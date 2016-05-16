#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;

namespace ShareX.ScreenCaptureLib
{
    public class SurfaceOptions
    {
        [DefaultValue(true), Description("Allow screenshot capture as soon as the mouse is released. This disables the ability to capture multiple shapes and to move and/or resize them.")]
        public bool QuickCrop { get; set; }

        [DefaultValue(true), Description("If annotation is disabled then right click will cancel screen capture instead of opening options menu.")]
        public bool AnnotationEnabled { get; set; }

        [DefaultValue(true), Description("Allows selection of window regions in region capture.")]
        public bool DetectWindows { get; set; }

        [DefaultValue(true), Description("If detect windows setting is chosen, this setting will also allow detecting window controls.")]
        public bool DetectControls { get; set; }

        [DefaultValue(true), Description("Show coordinate and size information.")]
        public bool ShowInfo { get; set; }

        [DefaultValue(false), Description("Allows to show your custom info text near cursor. This way you can show color info too.")]
        public bool UseCustomInfoText { get; set; }

        [DefaultValue("X: $x, Y: $y\r\nR: $r, G: $g, B: $b\r\nHex: $hex"), Description("Show this custom info when color info setting is enabled. Formats: $x, $y, $r, $g, $b, $hex"),
            Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string CustomInfoText { get; set; }

        [DefaultValue(true), Description("Show hotkeys/tips.")]
        public bool ShowTips { get; set; }

        [DefaultValue(true), Description("Show magnifier.")]
        public bool ShowMagnifier { get; set; }

        [DefaultValue(false), Description("Use square or circle shape magnifier.")]
        public bool UseSquareMagnifier { get; set; }

        [DefaultValue(15), Description("Number of pixels in magnifier. Must be odd number like 11, 13, 15 etc.")]
        public int MagnifierPixelCount { get; set; }

        [DefaultValue(10), Description("Size of pixels in magnifier.")]
        public int MagnifierPixelSize { get; set; }

        [DefaultValue(false), Description("Show screen wide crosshair.")]
        public bool ShowCrosshair { get; set; }

        [DefaultValue(true), Description("Capturing screen will dim the screen outside selected area. This may impact on the startup time of the capture process at high resolutions.")]
        public bool UseDimming { get; set; }

        [DefaultValue(false), Description("Show frames per second.")]
        public bool ShowFPS { get; set; }

        [DefaultValue(1), Description("Number of pixels to move shape at each arrow key stroke.")]
        public int MinMoveSpeed { get; set; }

        [DefaultValue(10), Description("Number of pixels to move shape at each arrow key stroke while holding Ctrl key.")]
        public int MaxMoveSpeed { get; set; }

        [DefaultValue(false), Description("Fixed shape size.")]
        public bool IsFixedSize { get; set; }

        [DefaultValue(typeof(Size), "250, 250"), Description("Fixed shape size.")]
        public Size FixedSize { get; set; }

        [DefaultValue(20), Description("How much region size must be close to snap size for it to snap.")]
        public int SnapDistance { get; set; }

        [Description("How close to a snap size you must be for it to snap.")]
        public List<SnapSize> SnapSizes { get; set; }

        public bool ShowMenuTip = true;

        public ShapeType CurrentShapeType = ShapeType.RegionRectangle;
        public Color ShapeBorderColor = Color.Red;
        public int ShapeBorderSize = 2;
        public Color ShapeFillColor = Color.FromArgb(0, 0, 0, 0);
        public int ShapeRoundedRectangleRadius = 15;
        public int ShapeBlurRadius = 15;
        public int ShapePixelateSize = 7;
        public Color ShapeHighlightColor = Color.Yellow;

        public SurfaceOptions()
        {
            this.ApplyDefaultPropertyValues();

            SnapSizes = new List<SnapSize>()
            {
                new SnapSize(426, 240), // 240p
                new SnapSize(640, 360), // 360p
                new SnapSize(854, 480), // 480p
                new SnapSize(1280, 720), // 720p
                new SnapSize(1920, 1080), // 1080p
                new SnapSize(2560, 1440), // 1440p
                new SnapSize(3840, 2160) // 2160p
            };
        }
    }
}