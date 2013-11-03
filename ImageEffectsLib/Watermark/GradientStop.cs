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
using System;
using System.Drawing;
using System.Globalization;

namespace ImageEffectsLib
{
    public class GradientStop
    {
        public Color Color { get; set; }
        public float Offset { get; set; }

        public GradientStop(Color color, float offset)
        {
            Color = color;
            Offset = offset;
        }

        public GradientStop(string color, string offset)
        {
            Color = ColorHelpers.ParseColor(color);

            if (Color == null)
            {
                throw new Exception("Color is unknown.");
            }

            float offset2;
            if (float.TryParse(offset, NumberStyles.Any, CultureInfo.InvariantCulture, out offset2))
            {
                Offset = offset2;
            }
            else
            {
                Offset = 0;
            }
        }
    }
}