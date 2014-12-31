#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2015 ShareX Developers

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
using System.ComponentModel;
using System.Drawing;

namespace ShareX.ScreenCaptureLib
{
    public class SurfaceOptions
    {
        [DefaultValue(true), Description("Allow screenshot capture as soon as the mouse is released. This disables the ability to capture multiple shapes and to move and/or resize them.")]
        public bool QuickCrop { get; set; }

        [DefaultValue(true), Description("Show coordinate and size information.")]
        public bool ShowInfo { get; set; }

        [DefaultValue(true), Description("Show magnifier.")]
        public bool ShowMagnifier { get; set; }

        [DefaultValue(15), Description("Number of pixels in magnifier. Must be odd number like 11, 13, 15 etc.")]
        public int MagnifierPixelCount { get; set; }

        [DefaultValue(10), Description("Size of pixels in magnifier.")]
        public int MagnifierPixelSize { get; set; }

        [DefaultValue(true), Description("Show screen wide crosshair.")]
        public bool ShowCrosshair { get; set; }

        [DefaultValue(true), Description("Cropping move will dim the screen during selection. Can be intensive at high resolutions.")]
        public bool UseDimming { get; set; }

        [DefaultValue(false), Description("Show frames per second.")]
        public bool ShowFPS { get; set; }

        [DefaultValue(false), Description("Result image will have border around the shape(s).")]
        public bool DrawBorder { get; set; }

        [DefaultValue(false), Description("You can use window capture mode in all rectangle type shapes. Also includes window client area.")]
        public bool ForceWindowCapture { get; set; }

        [DefaultValue(false), Description("If window capture mode enabled this setting will also allow to capture window controls.")]
        public bool IncludeControls { get; set; }

        [DefaultValue(1), Description("Number of pixels to move shape at each arrow key stroke.")]
        public int MinMoveSpeed { get; set; }

        [DefaultValue(5), Description("Number of pixels to move shape at each arrow key stroke while holding Ctrl key.")]
        public int MaxMoveSpeed { get; set; }

        [DefaultValue(false), Description("Fixed shape size.")]
        public bool IsFixedSize { get; set; }

        [DefaultValue(typeof(Size), "250, 250"), Description("Fixed shape size.")]
        public Size FixedSize { get; set; }

        public SurfaceOptions()
        {
            this.ApplyDefaultPropertyValues();
        }
    }
}