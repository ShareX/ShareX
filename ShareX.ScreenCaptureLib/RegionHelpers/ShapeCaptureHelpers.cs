#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2015 ShareX Developers

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
    public static class ShapeCaptureHelpers
    {
        public static Image GetRegionImage(Image surfaceImage, GraphicsPath regionFillPath, GraphicsPath regionDrawPath, SurfaceOptions options)
        {
            if (regionFillPath != null)
            {
                Image img;

                Rectangle regionArea = Rectangle.Round(regionFillPath.GetBounds());
                Rectangle screenRectangle = CaptureHelpers.GetScreenBounds0Based();
                Rectangle newRegionArea = Rectangle.Intersect(regionArea, screenRectangle);

                using (GraphicsPath gp = (GraphicsPath)regionFillPath.Clone())
                {
                    MoveGraphicsPath(gp, -Math.Max(0, regionArea.X), -Math.Max(0, regionArea.Y));
                    img = ImageHelpers.CropImage(surfaceImage, newRegionArea, gp);

                    if (options.DrawBorder)
                    {
                        GraphicsPath gpOutline = regionDrawPath ?? regionFillPath;

                        using (GraphicsPath gp2 = (GraphicsPath)gpOutline.Clone())
                        {
                            MoveGraphicsPath(gp2, -Math.Max(0, regionArea.X), -Math.Max(0, regionArea.Y));
                            img = ImageHelpers.DrawOutline(img, gp2);
                        }
                    }
                }

                return img;
            }

            return null;
        }

        private static void MoveGraphicsPath(GraphicsPath gp, int x, int y)
        {
            using (Matrix matrix = new Matrix())
            {
                gp.CloseFigure();
                matrix.Translate(x, y);
                gp.Transform(matrix);
            }
        }
    }
}