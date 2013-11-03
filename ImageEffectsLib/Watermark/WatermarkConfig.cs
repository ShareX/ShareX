#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

using HelpersLib;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ImageEffectsLib
{
    public enum WatermarkPositionType
    {
        [Description("Top - Left")]
        TOP_LEFT,
        [Description("Top - Center")]
        TOP,
        [Description("Top - Right")]
        TOP_RIGHT,
        [Description("Center - Left")]
        LEFT,
        [Description("Center")]
        CENTER,
        [Description("Center - Right")]
        RIGHT,
        [Description("Bottom - Left")]
        BOTTOM_LEFT,
        [Description("Bottom - Center")]
        BOTTOM,
        [Description("Bottom - Right")]
        BOTTOM_RIGHT
    }

    public enum WatermarkType
    {
        [Description("Text")]
        TEXT,
        [Description("Image")]
        IMAGE
    }

    public class WatermarkConfig
    {
        public WatermarkType WatermarkMode = WatermarkType.TEXT;
        public WatermarkPositionType WatermarkPositionMode = WatermarkPositionType.BOTTOM_RIGHT;
        public int WatermarkOffset = 5;
        public bool WatermarkAddReflection = false;
        public bool WatermarkAutoHide = true;

        public string WatermarkText = "%h:%mi";
        public XmlFont WatermarkFont = new XmlFont("Arial", 8);
        public XmlColor WatermarkFontArgb = Color.White;

        public int WatermarkCornerRadius = 4;
        public XmlColor WatermarkGradient1Argb = Color.FromArgb(85, 85, 85);
        public XmlColor WatermarkGradient2Argb = Color.Black;
        public XmlColor WatermarkBorderArgb = Color.Black;
        public LinearGradientMode WatermarkGradientType = LinearGradientMode.Vertical;
        public bool WatermarkUseCustomGradient = false;
        public GradientData WatermarkGradient = new GradientData();

        public string WatermarkImageLocation = "";
        public bool WatermarkUseBorder = false;
        public int WatermarkImageScale = 100;
    }
}