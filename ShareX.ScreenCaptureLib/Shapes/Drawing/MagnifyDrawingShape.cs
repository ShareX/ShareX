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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShareX.ScreenCaptureLib
{
    public class MagnifyDrawingShape : EllipseDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingMagnify;

        public int MagnifyStrength { get; set; } = 200;
        public ImageInterpolationMode ImageInterpolationMode { get; set; }

        public MagnifyDrawingShape()
        {
            ForceProportionalResizing = true;
        }

        public override void OnConfigLoad()
        {
            base.OnConfigLoad();
            MagnifyStrength = AnnotationOptions.MagnifyStrength;
            ImageInterpolationMode = AnnotationOptions.ImageInterpolationMode;
        }

        public override void OnConfigSave()
        {
            base.OnConfigSave();
            AnnotationOptions.MagnifyStrength = MagnifyStrength;
            AnnotationOptions.ImageInterpolationMode = ImageInterpolationMode;
        }

        public override void OnDraw(Graphics g)
        {
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.InterpolationMode = ImageHelpers.GetInterpolationMode(ImageInterpolationMode);

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddEllipse(Rectangle);
                g.SetClip(gp);

                float magnify = Math.Max(MagnifyStrength, 100) / 100f;
                int newWidth = (int)(Rectangle.Width / magnify);
                int newHeight = (int)(Rectangle.Height / magnify);

                g.DrawImage(Manager.Form.Canvas, Rectangle,
                    new RectangleF(Rectangle.X + (Rectangle.Width / 2) - (newWidth / 2) - Manager.Form.CanvasRectangle.X + Manager.RenderOffset.X,
                    Rectangle.Y + (Rectangle.Height / 2) - (newHeight / 2) - Manager.Form.CanvasRectangle.Y + Manager.RenderOffset.Y,
                    newWidth, newHeight), GraphicsUnit.Pixel);

                g.ResetClip();
            }

            g.PixelOffsetMode = PixelOffsetMode.Default;
            g.InterpolationMode = InterpolationMode.Bilinear;

            DrawEllipse(g);
        }
    }
}