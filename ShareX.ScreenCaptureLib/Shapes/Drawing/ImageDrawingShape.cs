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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShareX.ScreenCaptureLib
{
    public class ImageDrawingShape : BaseDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingImage;

        public Image Image { get; protected set; }
        public ImageInterpolationMode ImageInterpolationMode { get; protected set; }

        public override BaseShape Duplicate()
        {
            Image imageTemp = Image;
            Image = null;
            ImageDrawingShape shape = (ImageDrawingShape)base.Duplicate();
            shape.Image = imageTemp.CloneSafe();
            Image = imageTemp;
            return shape;
        }

        public override void OnConfigLoad()
        {
            ImageInterpolationMode = AnnotationOptions.ImageInterpolationMode;
        }

        public override void OnConfigSave()
        {
            AnnotationOptions.ImageInterpolationMode = ImageInterpolationMode;
        }

        public void SetImage(Image img, bool centerImage)
        {
            Dispose();

            Image = img;

            if (Image != null)
            {
                PointF location;
                Size size = Image.Size;

                if (centerImage)
                {
                    location = new PointF(Rectangle.X - (size.Width / 2), Rectangle.Y - (size.Height / 2));
                }
                else
                {
                    location = Rectangle.Location;
                }

                Rectangle = new RectangleF(location, size);
            }
        }

        public override void OnDraw(Graphics g)
        {
            DrawImage(g);
        }

        protected void DrawImage(Graphics g)
        {
            if (Image != null)
            {
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.InterpolationMode = ImageHelpers.GetInterpolationMode(ImageInterpolationMode);

                g.DrawImage(Image, Rectangle);

                g.PixelOffsetMode = PixelOffsetMode.Default;
                g.InterpolationMode = InterpolationMode.Bilinear;
            }
        }

        public override void OnMoved()
        {
            /*if (Manager.Form.IsEditorMode)
            {
                Manager.AutoResizeCanvas();
            }*/
        }

        public override void Dispose()
        {
            if (Image != null)
            {
                Image.Dispose();
            }
        }
    }
}