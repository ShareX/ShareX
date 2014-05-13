#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

namespace ImageEffectsLib
{
    public class GradientStop
    {
        [DefaultValue(typeof(Color), "Black")]
        public Color Color { get; set; }

        private float offset;

        [DefaultValue(0f)]
        public float Offset
        {
            get
            {
                return offset;
            }
            set
            {
                if (value >= 0 || value <= 1)
                {
                    offset = value;
                }
            }
        }

        public GradientStop()
        {
            this.ApplyDefaultPropertyValues();
        }

        public GradientStop(Color color, float offset)
        {
            Color = color;
            Offset = offset;
        }
    }
}