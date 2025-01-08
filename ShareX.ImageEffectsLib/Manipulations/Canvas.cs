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
using System.Windows.Forms;

namespace ShareX.ImageEffectsLib
{
    internal class Canvas : ImageEffect
    {
        [DefaultValue(typeof(Padding), "0, 0, 0, 0")]
        public Padding Margin { get; set; }

        [DefaultValue(CanvasMarginMode.AbsoluteSize), Description("How the margin around the canvas will be calculated."), TypeConverter(typeof(EnumDescriptionConverter))]
        public CanvasMarginMode MarginMode { get; set; }

        [DefaultValue(typeof(Color), "Transparent"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color Color { get; set; }

        public Canvas()
        {
            this.ApplyDefaultPropertyValues();
        }

        public enum CanvasMarginMode
        {
            AbsoluteSize,
            PercentageOfCanvas
        }

        public override Bitmap Apply(Bitmap bmp)
        {
            Padding canvasMargin;

            if (MarginMode == CanvasMarginMode.PercentageOfCanvas)
            {
                canvasMargin = new Padding();
                canvasMargin.Left = (int)Math.Round(Margin.Left / 100f * bmp.Width);
                canvasMargin.Right = (int)Math.Round(Margin.Right / 100f * bmp.Width);
                canvasMargin.Top = (int)Math.Round(Margin.Top / 100f * bmp.Height);
                canvasMargin.Bottom = (int)Math.Round(Margin.Bottom / 100f * bmp.Height);
            }
            else
            {
                canvasMargin = Margin;
            }

            Bitmap bmpResult = ImageHelpers.AddCanvas(bmp, canvasMargin, Color);

            if (bmpResult == null)
            {
                return bmp;
            }

            bmp.Dispose();
            return bmpResult;
        }

        protected override string GetSummary()
        {
            if (Margin.All == -1)
            {
                return $"{Margin.Left}, {Margin.Top}, {Margin.Right}, {Margin.Bottom}";
            }

            return Margin.All.ToString();
        }
    }
}