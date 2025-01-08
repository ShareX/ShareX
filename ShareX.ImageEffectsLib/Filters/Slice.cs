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

using ShareX.HelpersLib;
using System;
using System.ComponentModel;
using System.Drawing;

namespace ShareX.ImageEffectsLib
{
    [Description("Slice")]
    internal class Slice : ImageEffect
    {
        private int minSliceHeight;

        [DefaultValue(10)]
        public int MinSliceHeight
        {
            get
            {
                return minSliceHeight;
            }
            set
            {
                minSliceHeight = value.Max(1);
            }
        }

        private int maxSliceHeight;

        [DefaultValue(100)]
        public int MaxSliceHeight
        {
            get
            {
                return maxSliceHeight;
            }
            set
            {
                maxSliceHeight = value.Max(1);
            }
        }

        [DefaultValue(0)]
        public int MinSliceShift { get; set; }

        [DefaultValue(10)]
        public int MaxSliceShift { get; set; }

        public Slice()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Bitmap Apply(Bitmap bmp)
        {
            int minSliceHeight = Math.Min(MinSliceHeight, MaxSliceHeight);
            int maxSliceHeight = Math.Max(MinSliceHeight, MaxSliceHeight);
            int minSliceShift = Math.Min(MinSliceShift, MaxSliceShift);
            int maxSliceShift = Math.Max(MinSliceShift, MaxSliceShift);

            using (bmp)
            {
                return ImageHelpers.Slice(bmp, minSliceHeight, maxSliceHeight, minSliceShift, maxSliceShift);
            }
        }

        protected override string GetSummary()
        {
            return $"{MinSliceHeight}, {MaxSliceHeight}";
        }
    }
}