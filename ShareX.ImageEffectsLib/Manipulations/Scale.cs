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
    internal class Scale : ImageEffect
    {
        [DefaultValue(100f), Description("Use width percentage as 0 to maintain aspect ratio by automatically adjusting width.")]
        public float WidthPercentage { get; set; }

        [DefaultValue(0f), Description("Use height percentage as 0 to maintain aspect ratio by automatically adjusting height.")]
        public float HeightPercentage { get; set; }

        public Scale()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Bitmap Apply(Bitmap bmp)
        {
            if (WidthPercentage <= 0 && HeightPercentage <= 0)
            {
                return bmp;
            }

            int width = (int)Math.Round(WidthPercentage / 100 * bmp.Width);
            int height = (int)Math.Round(HeightPercentage / 100 * bmp.Height);
            Size size = ImageHelpers.ApplyAspectRatio(width, height, bmp);

            return ImageHelpers.ResizeImage(bmp, size);
        }

        protected override string GetSummary()
        {
            string summary = WidthPercentage.ToString();

            if (WidthPercentage > 0)
            {
                summary += "%";
            }

            summary += ", " + HeightPercentage.ToString();

            if (HeightPercentage > 0)
            {
                summary += "%";
            }

            return summary;
        }
    }
}