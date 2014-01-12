#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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
using System.Windows.Forms;

namespace ImageEffectsLib
{
    [Description("Torn edge")]
    internal class TornEdge : ImageEffect
    {
        [DefaultValue(12)]
        public int ToothHeight { get; set; }

        [DefaultValue(20)]
        public int HorizontalToothRange { get; set; }

        [DefaultValue(20)]
        public int VerticalToothRange { get; set; }

        [DefaultValue(AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right)]
        public AnchorStyles Sides { get; set; }

        public TornEdge()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Image Apply(Image img)
        {
            if (Sides != AnchorStyles.None)
            {
                using (img)
                {
                    return ImageHelpers.CreateTornEdge(img, ToothHeight, HorizontalToothRange, VerticalToothRange, Sides);
                }
            }

            return img;
        }
    }
}