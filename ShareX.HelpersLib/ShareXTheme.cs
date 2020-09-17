#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace ShareX.HelpersLib
{
    public class ShareXTheme
    {
        public string Name { get; set; }

        private Color backgroundColor;

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                if (!value.IsEmpty) backgroundColor = value;
            }
        }

        private Color lightBackgroundColor;

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color LightBackgroundColor
        {
            get
            {
                return lightBackgroundColor;
            }
            set
            {
                if (!value.IsEmpty) lightBackgroundColor = value;
            }
        }

        private Color darkBackgroundColor;

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color DarkBackgroundColor
        {
            get
            {
                return darkBackgroundColor;
            }
            set
            {
                if (!value.IsEmpty) darkBackgroundColor = value;
            }
        }

        private Color textColor;

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color TextColor
        {
            get
            {
                return textColor;
            }
            set
            {
                if (!value.IsEmpty) textColor = value;
            }
        }

        private Color borderColor;

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                if (!value.IsEmpty) borderColor = value;
            }
        }

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color CheckerColor { get; set; }

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color CheckerColor2 { get; set; }

        public int CheckerSize { get; set; } = 15;

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color LinkColor { get; set; }

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color MenuHighlightColor { get; set; }

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color MenuHighlightBorderColor { get; set; }

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color MenuBorderColor { get; set; }

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color MenuCheckBackgroundColor { get; set; }

        public Font ContextMenuFont { get; set; } = new Font("Segoe UI", 10);

        public int ContextMenuOpacity { get; set; } = 100;

        [Browsable(false)]
        public double ContextMenuOpacityDouble => ContextMenuOpacity.Clamp(10, 100) / 100d;

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color SeparatorLightColor { get; set; }

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color SeparatorDarkColor { get; set; }

        [Browsable(false)]
        public bool IsDarkTheme => ColorHelpers.IsDarkColor(BackgroundColor);

        public ShareXTheme()
        {
        }

        public static ShareXTheme GetDarkTheme()
        {
            return new ShareXTheme()
            {
                Name = "Dark",
                BackgroundColor = Color.FromArgb(42, 47, 56),
                LightBackgroundColor = Color.FromArgb(52, 57, 65),
                DarkBackgroundColor = Color.FromArgb(28, 32, 38),
                TextColor = Color.FromArgb(235, 235, 235),
                BorderColor = Color.FromArgb(28, 32, 38),
                CheckerColor = Color.FromArgb(60, 60, 60),
                CheckerColor2 = Color.FromArgb(50, 50, 50),
                CheckerSize = 15,
                LinkColor = Color.FromArgb(166, 212, 255),
                MenuHighlightColor = Color.FromArgb(30, 34, 40),
                MenuHighlightBorderColor = Color.FromArgb(116, 129, 152),
                MenuBorderColor = Color.FromArgb(22, 26, 31),
                MenuCheckBackgroundColor = Color.FromArgb(56, 64, 75),
                ContextMenuOpacity = 90,
                SeparatorLightColor = Color.FromArgb(56, 64, 75),
                SeparatorDarkColor = Color.FromArgb(22, 26, 31)
            };
        }

        public static ShareXTheme GetLightTheme()
        {
            return new ShareXTheme()
            {
                Name = "Light",
                BackgroundColor = Color.FromArgb(242, 242, 242),
                LightBackgroundColor = Color.FromArgb(247, 247, 247),
                DarkBackgroundColor = Color.FromArgb(235, 235, 235),
                TextColor = Color.FromArgb(69, 69, 69),
                BorderColor = Color.FromArgb(201, 201, 201),
                CheckerColor = Color.FromArgb(247, 247, 247),
                CheckerColor2 = Color.FromArgb(235, 235, 235),
                CheckerSize = 15,
                LinkColor = Color.FromArgb(166, 212, 255),
                MenuHighlightColor = Color.FromArgb(247, 247, 247),
                MenuHighlightBorderColor = Color.FromArgb(96, 143, 226),
                MenuBorderColor = Color.FromArgb(201, 201, 201),
                MenuCheckBackgroundColor = Color.FromArgb(225, 233, 244),
                ContextMenuOpacity = 95,
                SeparatorLightColor = Color.FromArgb(253, 253, 253),
                SeparatorDarkColor = Color.FromArgb(189, 189, 189)
            };
        }

        public static List<ShareXTheme> GetPresets()
        {
            return new List<ShareXTheme>() { GetDarkTheme(), GetLightTheme() };
        }

        public override string ToString()
        {
            return Name;
        }
    }
}