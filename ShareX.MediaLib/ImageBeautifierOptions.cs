#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShareX.MediaLib
{
    public class ImageBeautifierOptions
    {
        public int Margin { get; set; } = 80;
        public int Padding { get; set; } = 40;
        public bool SmartPadding { get; set; } = true;
        public int RoundedCorner { get; set; } = 20;
        public int ShadowSize { get; set; } = 30;
        public GradientInfo Background { get; set; } = new GradientInfo(Color.FromArgb(255, 81, 47), Color.FromArgb(221, 36, 118))
        {
            Type = LinearGradientMode.ForwardDiagonal
        };
    }
}