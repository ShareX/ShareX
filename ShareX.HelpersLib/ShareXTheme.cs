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

using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class ShareXTheme
    {
        public Color BackgroundColor { get; set; }
        public Color BackgroundColor2 { get; set; }
        public Color TextColor { get; set; }
        public Color BorderColor { get; set; }
        public Color CheckerColor { get; set; }
        public Color CheckerColor2 { get; set; }
        public int CheckerSize { get; set; } = 15;
        public Color LinkColor { get; set; }

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
            BackgroundColor = Color.FromArgb(42, 47, 56);
            BackgroundColor2 = ColorHelpers.LighterColor(BackgroundColor, 0.05f);
            TextColor = Color.FromArgb(235, 235, 235);
            BorderColor = Color.FromArgb(28, 32, 38);
            CheckerColor = Color.FromArgb(60, 60, 60);
            CheckerColor2 = Color.FromArgb(50, 50, 50);
            LinkColor = Color.FromArgb(166, 212, 255);
        }
    }
}