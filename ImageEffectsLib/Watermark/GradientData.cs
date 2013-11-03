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

using System.Drawing.Drawing2D;

namespace ImageEffectsLib
{
    public class GradientData
    {
        public string Data { get; set; }
        public LinearGradientMode Type { get; set; }

        public GradientData()
        {
            Data = "255,68,120,194\t0\n255,13,58,122\t0.5\n255,6,36,78\t0.5\n255,12,76,159\t1";
            Type = LinearGradientMode.Vertical;
        }

        public GradientData(string data, LinearGradientMode type)
        {
            Data = data;
            Type = type;
        }
    }
}