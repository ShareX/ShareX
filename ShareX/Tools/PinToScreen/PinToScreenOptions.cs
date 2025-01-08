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

using System.Drawing;

namespace ShareX
{
    public class PinToScreenOptions
    {
        public int InitialScale { get; set; } = 100;
        public int ScaleStep { get; set; } = 10;
        public bool HighQualityScale { get; set; } = true;
        public int InitialOpacity { get; set; } = 100;
        public int OpacityStep { get; set; } = 10;
        public ContentAlignment Placement { get; set; } = ContentAlignment.BottomRight;
        public int PlacementOffset { get; set; } = 10;
        public bool TopMost { get; set; } = true;
        public bool KeepCenterLocation { get; set; } = true;
        public Color BackgroundColor { get; set; } = Color.White;
        public bool Shadow { get; set; } = true;
        public bool Border { get; set; } = true;
        public int BorderSize { get; set; } = 2;
        public Color BorderColor { get; set; } = Color.CornflowerBlue;
        public Size MinimizeSize { get; set; } = new Size(100, 100);
    }
}