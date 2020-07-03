#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
using System.Drawing.Imaging;

namespace ShareX.ImageEffectsLib
{
    [Description("RGB Split")]
    internal class RGBSplit : ImageEffect
    {
        [DefaultValue(typeof(Point), "5, 0")]
        public Point OffsetRed { get; set; }
        [DefaultValue(typeof(Point), "0, 0")]
        public Point OffsetGreen { get; set; }
        [DefaultValue(typeof(Point), "-5, 0")]
        public Point OffsetBlue { get; set; }

        public override Bitmap Apply(Bitmap bmp)
        {
            Bitmap bmpResult = bmp.CreateEmptyBitmap();
            using (UnsafeBitmap source = new UnsafeBitmap(bmp, true, ImageLockMode.ReadOnly))
            using (UnsafeBitmap dest = new UnsafeBitmap(bmpResult, true, ImageLockMode.WriteOnly))
            {
                for (int y = 0; y < dest.Height; y++)
                {
                    for (int x = 0; x < dest.Width; x++)
                    {
                        ColorBgra colorR = source.GetPixel(MathHelpers.Clamp(x + OffsetRed.X, 0, dest.Width - 1), MathHelpers.Clamp(y + OffsetRed.Y, 0, dest.Height - 1));
                        ColorBgra colorG = source.GetPixel(MathHelpers.Clamp(x + OffsetGreen.X, 0, dest.Width - 1), MathHelpers.Clamp(y + OffsetGreen.Y, 0, dest.Height - 1));
                        ColorBgra colorB = source.GetPixel(MathHelpers.Clamp(x + OffsetBlue.X, 0, dest.Width - 1), MathHelpers.Clamp(y + OffsetBlue.Y, 0, dest.Height - 1));

                        byte colorR_alpha = colorR.Alpha;
                        byte colorG_alpha = colorG.Alpha;
                        byte colorB_alpha = colorB.Alpha;
                        byte colorA = (byte)((colorR_alpha / 3 + colorG_alpha / 3 + colorB_alpha / 3));

                        ColorBgra shiftedcolor = new ColorBgra((byte)(colorB.Blue * colorB_alpha / 255), (byte)(colorG.Green * colorG_alpha / 255), (byte)(colorR.Red * colorR_alpha / 255), colorA);
                        dest.SetPixel(x, y, shiftedcolor);
                    }
                }
            }
            return bmpResult;
        }
    }
}