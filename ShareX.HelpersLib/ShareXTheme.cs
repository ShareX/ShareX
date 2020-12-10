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
                if (!value.IsTransparent()) backgroundColor = value;
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
                if (!value.IsTransparent()) lightBackgroundColor = value;
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
                if (!value.IsTransparent()) darkBackgroundColor = value;
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
                if (!value.IsTransparent()) textColor = value;
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
                if (!value.IsTransparent()) borderColor = value;
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

        private ShareXTheme()
        {
            Name = "Dark";
            BackgroundColor = Color.FromArgb(42, 47, 56);
            LightBackgroundColor = Color.FromArgb(52, 57, 65);
            DarkBackgroundColor = Color.FromArgb(28, 32, 38);
            TextColor = Color.FromArgb(235, 235, 235);
            BorderColor = Color.FromArgb(28, 32, 38);
            CheckerColor = Color.FromArgb(60, 60, 60);
            CheckerColor2 = Color.FromArgb(50, 50, 50);
            CheckerSize = 15;
            LinkColor = Color.FromArgb(166, 212, 255);
            MenuHighlightColor = Color.FromArgb(30, 34, 40);
            MenuHighlightBorderColor = Color.FromArgb(116, 129, 152);
            MenuBorderColor = Color.FromArgb(22, 26, 31);
            MenuCheckBackgroundColor = Color.FromArgb(56, 64, 75);
            ContextMenuOpacity = 100;
            SeparatorLightColor = Color.FromArgb(56, 64, 75);
            SeparatorDarkColor = Color.FromArgb(22, 26, 31);
        }

        public static ShareXTheme DarkTheme => new ShareXTheme();

        public static ShareXTheme LightTheme => new ShareXTheme()
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
            ContextMenuOpacity = 100,
            SeparatorLightColor = Color.FromArgb(253, 253, 253),
            SeparatorDarkColor = Color.FromArgb(189, 189, 189)
        };

        // https://www.nordtheme.com
        public static ShareXTheme NordDarkTheme => new ShareXTheme()
        {
            Name = "Nord Dark",
            BackgroundColor = Color.FromArgb(46, 52, 64),
            LightBackgroundColor = Color.FromArgb(59, 66, 82),
            DarkBackgroundColor = Color.FromArgb(38, 44, 57),
            TextColor = Color.FromArgb(229, 233, 240),
            BorderColor = Color.FromArgb(30, 38, 54),
            CheckerColor = Color.FromArgb(46, 52, 64),
            CheckerColor2 = Color.FromArgb(36, 42, 54),
            CheckerSize = 15,
            LinkColor = Color.FromArgb(136, 192, 208),
            MenuHighlightColor = Color.FromArgb(36, 42, 54),
            MenuHighlightBorderColor = Color.FromArgb(24, 30, 42),
            MenuBorderColor = Color.FromArgb(24, 30, 42),
            MenuCheckBackgroundColor = Color.FromArgb(59, 66, 82),
            ContextMenuOpacity = 100,
            SeparatorLightColor = Color.FromArgb(59, 66, 82),
            SeparatorDarkColor = Color.FromArgb(30, 38, 54)
        };

        // https://www.nordtheme.com
        public static ShareXTheme NordLightTheme => new ShareXTheme()
        {
            Name = "Nord Light",
            BackgroundColor = Color.FromArgb(229, 233, 240),
            LightBackgroundColor = Color.FromArgb(236, 239, 244),
            DarkBackgroundColor = Color.FromArgb(216, 222, 233),
            TextColor = Color.FromArgb(59, 66, 82),
            BorderColor = Color.FromArgb(207, 216, 233),
            CheckerColor = Color.FromArgb(229, 233, 240),
            CheckerColor2 = Color.FromArgb(216, 222, 233),
            CheckerSize = 15,
            LinkColor = Color.FromArgb(106, 162, 178),
            MenuHighlightColor = Color.FromArgb(236, 239, 244),
            MenuHighlightBorderColor = Color.FromArgb(207, 216, 233),
            MenuBorderColor = Color.FromArgb(216, 222, 233),
            MenuCheckBackgroundColor = Color.FromArgb(229, 233, 240),
            ContextMenuOpacity = 100,
            SeparatorLightColor = Color.FromArgb(236, 239, 244),
            SeparatorDarkColor = Color.FromArgb(207, 216, 233)
        };

        // https://draculatheme.com
        public static ShareXTheme DraculaTheme => new ShareXTheme()
        {
            Name = "Dracula",
            BackgroundColor = Color.FromArgb(40, 42, 54),
            LightBackgroundColor = Color.FromArgb(68, 71, 90),
            DarkBackgroundColor = Color.FromArgb(36, 38, 48),
            TextColor = Color.FromArgb(248, 248, 242),
            BorderColor = Color.FromArgb(33, 35, 43),
            CheckerColor = Color.FromArgb(40, 42, 54),
            CheckerColor2 = Color.FromArgb(36, 38, 48),
            CheckerSize = 15,
            LinkColor = Color.FromArgb(98, 114, 164),
            MenuHighlightColor = Color.FromArgb(36, 38, 48),
            MenuHighlightBorderColor = Color.FromArgb(255, 121, 198),
            MenuBorderColor = Color.FromArgb(33, 35, 43),
            MenuCheckBackgroundColor = Color.FromArgb(45, 47, 61),
            ContextMenuOpacity = 100,
            SeparatorLightColor = Color.FromArgb(45, 47, 61),
            SeparatorDarkColor = Color.FromArgb(33, 35, 43)
        };

        public static List<ShareXTheme> GetDefaultThemes()
        {
            return new List<ShareXTheme>() { DarkTheme, LightTheme, NordDarkTheme, NordLightTheme, DraculaTheme };
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
