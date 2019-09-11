#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class ShareXTheme
    {
        public string Name { get; set; }

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color BackgroundColor { get; set; }

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color LightBackgroundColor { get; set; }

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color DarkBackgroundColor { get; set; }

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color TextColor { get; set; }

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color BorderColor { get; set; }

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

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color SeparatorLightColor { get; set; }

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color SeparatorDarkColor { get; set; }

        public ShareXTheme()
        {
            ApplyDarkColors();
        }

        public void ApplySystemColors()
        {
            BackgroundColor = SystemColors.Window;
            //BackgroundColor2 = 
            TextColor = SystemColors.ControlText;
            BorderColor = ProfessionalColors.SeparatorDark;
            CheckerColor = SystemColors.ControlLightLight;
            CheckerColor2 = SystemColors.ControlLight;
            //LinkColor = 
        }

        public void ApplyDarkColors()
        {
            Name = "Dark";
            BackgroundColor = Color.FromArgb(42, 47, 56);
            LightBackgroundColor = Color.FromArgb(52, 57, 65);
            DarkBackgroundColor = Color.FromArgb(28, 32, 38);
            TextColor = Color.FromArgb(235, 235, 235);
            BorderColor = Color.FromArgb(28, 32, 38);
            CheckerColor = Color.FromArgb(60, 60, 60);
            CheckerColor2 = Color.FromArgb(50, 50, 50);
            LinkColor = Color.FromArgb(166, 212, 255);
            MenuHighlightColor = Color.FromArgb(255, 30, 34, 40);
            MenuHighlightBorderColor = Color.FromArgb(255, 116, 129, 152);
            MenuBorderColor = Color.FromArgb(255, 22, 26, 31);
            MenuCheckBackgroundColor = Color.FromArgb(255, 56, 64, 75);
            SeparatorLightColor = Color.FromArgb(255, 56, 64, 75);
            SeparatorDarkColor = Color.FromArgb(255, 22, 26, 31);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}