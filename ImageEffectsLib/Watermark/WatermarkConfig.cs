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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ImageEffectsLib
{
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
        public PositionType WatermarkPositionMode = PositionType.Bottom_Right;
        public int WatermarkOffset = 5;
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
        public List<GradientStop> WatermarkGradientList = new List<GradientStop>();

        public string WatermarkImageLocation = "";
    }
}