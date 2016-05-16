#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public static class RegionCaptureHelpers
    {
        public static Image GetRegionImage()
        {
            using (RectangleRegionForm form = new RectangleRegionForm(RectangleRegionMode.Default))
            {
                form.Config.ShowTips = false;

                form.Prepare();
                form.ShowDialog();

                return form.GetResultImage();
            }
        }

        public static bool GetRectangleRegion(out Rectangle rect)
        {
            using (RectangleRegionForm form = new RectangleRegionForm(RectangleRegionMode.Default))
            {
                form.Config.ShowTips = false;

                form.Prepare();
                form.ShowDialog();

                if (form.Result == RegionResult.Region)
                {
                    if (form.ShapeManager.IsCurrentRegionValid)
                    {
                        rect = CaptureHelpers.ClientToScreen(form.ShapeManager.CurrentRectangle);
                        return true;
                    }
                }
                else if (form.Result == RegionResult.Fullscreen)
                {
                    rect = CaptureHelpers.GetScreenBounds();
                    return true;
                }
                else if (form.Result == RegionResult.Monitor)
                {
                    Screen[] screens = Screen.AllScreens;

                    if (form.MonitorIndex < screens.Length)
                    {
                        Screen screen = screens[form.MonitorIndex];
                        rect = screen.Bounds;
                        return true;
                    }
                }
                else if (form.Result == RegionResult.ActiveMonitor)
                {
                    rect = CaptureHelpers.GetActiveScreenBounds();
                    return true;
                }
            }

            rect = Rectangle.Empty;
            return false;
        }

        public static PointInfo GetPointInfo()
        {
            using (RectangleRegionForm form = new RectangleRegionForm(RectangleRegionMode.ScreenColorPicker))
            {
                form.Config.DetectWindows = false;
                form.Config.ShowTips = false;
                form.Config.UseDimming = false;

                form.Prepare();
                form.ShowDialog();

                if (form.Result == RegionResult.Region)
                {
                    PointInfo pointInfo = new PointInfo();
                    pointInfo.Position = form.CurrentPosition;
                    pointInfo.Color = form.CurrentColor;
                    return pointInfo;
                }
            }

            return null;
        }

        public static SimpleWindowInfo GetWindowInfo()
        {
            using (RectangleRegionForm form = new RectangleRegionForm(RectangleRegionMode.OneClick))
            {
                form.Config.UseDimming = false;
                form.Config.ShowMagnifier = false;
                form.Config.ShowTips = false;

                form.Prepare();
                form.ShowDialog();

                if (form.Result == RegionResult.Region)
                {
                    return form.SelectedWindow;
                }
            }

            return null;
        }

        public static void ShowScreenRuler()
        {
            using (RectangleRegionForm form = new RectangleRegionForm(RectangleRegionMode.Ruler))
            {
                form.Config.QuickCrop = false;
                form.Config.ShowTips = false;

                form.Prepare();
                form.ShowDialog();
            }
        }

        public static Image ApplyRegionPathToImage(Image backgroundImage, GraphicsPath regionFillPath, SurfaceOptions options)
        {
            if (backgroundImage != null && regionFillPath != null)
            {
                Image img;

                Rectangle regionArea = Rectangle.Round(regionFillPath.GetBounds());
                Rectangle screenRectangle = CaptureHelpers.GetScreenBounds0Based();
                Rectangle newRegionArea = Rectangle.Intersect(regionArea, screenRectangle);

                using (GraphicsPath gp = (GraphicsPath)regionFillPath.Clone())
                {
                    using (Matrix matrix = new Matrix())
                    {
                        gp.CloseFigure();
                        matrix.Translate(-Math.Max(0, regionArea.X), -Math.Max(0, regionArea.Y));
                        gp.Transform(matrix);
                    }

                    img = ImageHelpers.CropImage(backgroundImage, newRegionArea, gp);
                }

                return img;
            }

            return null;
        }
    }
}