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
using System.ComponentModel;
using System.Drawing;

namespace ShareX.ImageEffectsLib
{
    internal class Reflection : ImageEffect
    {
        private int percentage;

        [DefaultValue(20), Description("Reflection height size relative to screenshot height.\nValue need to be between 1 to 100.")]
        public int Percentage
        {
            get
            {
                return percentage;
            }
            set
            {
                percentage = value.Clamp(1, 100);
            }
        }

        private int maxAlpha;

        [DefaultValue(255), Description("Reflection transparency start from this value to MinAlpha.\nValue need to be between 0 to 255.")]
        public int MaxAlpha
        {
            get
            {
                return maxAlpha;
            }
            set
            {
                maxAlpha = value.Clamp(0, 255);
            }
        }

        private int minAlpha;

        [DefaultValue(0), Description("Reflection transparency start from MaxAlpha to this value.\nValue need to be between 0 to 255.")]
        public int MinAlpha
        {
            get
            {
                return minAlpha;
            }
            set
            {
                minAlpha = value.Clamp(0, 255);
            }
        }

        [DefaultValue(0), Description("Reflection start position will be: Screenshot height + Offset")]
        public int Offset { get; set; }

        [DefaultValue(false), Description("Adding skew to reflection from bottom left to bottom right.")]
        public bool Skew { get; set; }

        [DefaultValue(25), Description("How much pixel skew left to right.")]
        public int SkewSize { get; set; }

        public Reflection()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Bitmap Apply(Bitmap bmp)
        {
            return ImageHelpers.DrawReflection(bmp, Percentage, MaxAlpha, MinAlpha, Offset, Skew, SkewSize);
        }

        protected override string GetSummary()
        {
            return Percentage.ToString();
        }
    }
}