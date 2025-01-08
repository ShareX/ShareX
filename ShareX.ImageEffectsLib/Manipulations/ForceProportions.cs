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
using System.Drawing.Design;

namespace ShareX.ImageEffectsLib
{
    [Description("Force proportions")]
    internal class ForceProportions : ImageEffect
    {
        private int proportionalWidth = 1;

        [DefaultValue(1)]
        public int ProportionalWidth
        {
            get
            {
                return proportionalWidth;
            }
            set
            {
                proportionalWidth = Math.Max(1, value);
            }
        }

        private int proportionalHeight = 1;

        [DefaultValue(1)]
        public int ProportionalHeight
        {
            get
            {
                return proportionalHeight;
            }
            set
            {
                proportionalHeight = Math.Max(1, value);
            }
        }

        public enum ForceProportionsMethod
        {
            Grow,
            Crop
        }

        [DefaultValue(ForceProportionsMethod.Grow)]
        public ForceProportionsMethod Method { get; set; } = ForceProportionsMethod.Grow;

        [DefaultValue(typeof(Color), "Transparent"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color GrowFillColor { get; set; } = Color.Transparent;

        public override Bitmap Apply(Bitmap bmp)
        {
            float currentRatio = bmp.Width / (float)bmp.Height;
            float targetRatio = proportionalWidth / (float)proportionalHeight;

            bool isTargetWider = targetRatio > currentRatio;

            int targetWidth = bmp.Width;
            int targetHeight = bmp.Height;
            int marginLeft = 0;
            int marginTop = 0;

            if (Method == ForceProportionsMethod.Crop)
            {
                if (isTargetWider)
                {
                    targetHeight = (int)Math.Round(bmp.Width / targetRatio);
                    marginTop = (bmp.Height - targetHeight) / 2;
                }
                else
                {
                    targetWidth = (int)Math.Round(bmp.Height * targetRatio);
                    marginLeft = (bmp.Width - targetWidth) / 2;
                }

                return ImageHelpers.CropBitmap(bmp, new Rectangle(marginLeft, marginTop, targetWidth, targetHeight));
            }
            else if (Method == ForceProportionsMethod.Grow)
            {
                if (isTargetWider)
                {
                    targetWidth = (int)Math.Round(bmp.Height * targetRatio);
                }
                else
                {
                    targetHeight = (int)Math.Round(bmp.Width / targetRatio);
                }

                return ImageHelpers.ResizeImage(bmp, targetWidth, targetHeight, false, true, GrowFillColor);
            }

            return bmp;
        }

        protected override string GetSummary()
        {
            return $"{ProportionalWidth}, {ProportionalHeight}";
        }
    }
}