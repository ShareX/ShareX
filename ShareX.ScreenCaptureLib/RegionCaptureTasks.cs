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
using System.IO;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public static class RegionCaptureTasks
    {
        public static Image GetRegionImage(RegionCaptureOptions options)
        {
            using (RegionCaptureForm form = new RegionCaptureForm(RegionCaptureMode.Default))
            {
                form.Config = GetRegionCaptureOptions(options);
                form.Config.ShowTips = false;

                form.Prepare();
                form.ShowDialog();

                return form.GetResultImage();
            }
        }

        public static bool GetRectangleRegion(out Rectangle rect, RegionCaptureOptions options)
        {
            using (RegionCaptureForm form = new RegionCaptureForm(RegionCaptureMode.Default))
            {
                form.Config = GetRegionCaptureOptions(options);
                form.Config.ShowTips = false;

                form.Prepare();
                form.ShowDialog();

                if (form.Result == RegionResult.Region)
                {
                    if (form.ShapeManager.IsCurrentShapeValid)
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

        public static PointInfo GetPointInfo(RegionCaptureOptions options)
        {
            using (RegionCaptureForm form = new RegionCaptureForm(RegionCaptureMode.ScreenColorPicker))
            {
                form.Config = GetRegionCaptureOptions(options);
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

        public static SimpleWindowInfo GetWindowInfo(RegionCaptureOptions options)
        {
            using (RegionCaptureForm form = new RegionCaptureForm(RegionCaptureMode.OneClick))
            {
                form.Config = GetRegionCaptureOptions(options);
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

        public static void ShowScreenRuler(RegionCaptureOptions options)
        {
            using (RegionCaptureForm form = new RegionCaptureForm(RegionCaptureMode.Ruler))
            {
                form.Config = GetRegionCaptureOptions(options);
                form.Config.QuickCrop = false;
                form.Config.ShowTips = false;

                form.Prepare();
                form.ShowDialog();
            }
        }

        public static void AnnotateImage(Image img, string filePath, RegionCaptureOptions options,
            Action<Image> afterCaptureTasksRequested,
            Action<Image, string> saveImageRequested,
            Action<Image, string> saveImageAsRequested,
            Action<Image> copyImageRequested,
            Action<Image> uploadImageRequested,
            Action<Image> printImageRequested)
        {
            using (RegionCaptureForm form = new RegionCaptureForm(RegionCaptureMode.Editor))
            {
                form.ImageFilePath = filePath;

                form.Config = GetRegionCaptureOptions(options);
                form.Config.DetectWindows = false;
                form.Config.ShowTips = false;
                form.Config.UseDimming = false;

                form.Prepare(img);
                form.ShowDialog();

                if (form.Result != RegionResult.Close)
                {
                    Image result = form.GetResultImage();

                    switch (form.Result)
                    {
                        default:
                            result.Dispose();
                            break;
                        case RegionResult.AnnotateRunAfterCaptureTasks:
                            afterCaptureTasksRequested(result);
                            break;
                        case RegionResult.AnnotateSaveImage:
                            saveImageRequested(result, form.ImageFilePath);
                            result.Dispose();
                            break;
                        case RegionResult.AnnotateSaveImageAs:
                            saveImageAsRequested(result, form.ImageFilePath);
                            result.Dispose();
                            break;
                        case RegionResult.AnnotateCopyImage:
                            copyImageRequested(result);
                            result.Dispose();
                            break;
                        case RegionResult.AnnotateUploadImage:
                            uploadImageRequested(result);
                            break;
                        case RegionResult.AnnotatePrintImage:
                            printImageRequested(result);
                            result.Dispose();
                            break;
                    }
                }
            }
        }

        public static Image ApplyRegionPathToImage(Image img, GraphicsPath gp)
        {
            if (img != null && gp != null)
            {
                Rectangle regionArea = Rectangle.Round(gp.GetBounds());
                Rectangle screenRectangle = CaptureHelpers.GetScreenBounds0Based();
                regionArea = Rectangle.Intersect(regionArea, screenRectangle);

                if (regionArea.IsValid())
                {
                    using (Bitmap bmp = img.CreateEmptyBitmap())
                    using (Graphics g = Graphics.FromImage(bmp))
                    using (TextureBrush brush = new TextureBrush(img))
                    {
                        g.PixelOffsetMode = PixelOffsetMode.Half;
                        g.SmoothingMode = SmoothingMode.HighQuality;

                        g.FillPath(brush, gp);

                        return ImageHelpers.CropBitmap(bmp, regionArea);
                    }
                }
            }

            return null;
        }

        private static RegionCaptureOptions GetRegionCaptureOptions(RegionCaptureOptions options)
        {
            if (options == null)
            {
                return new RegionCaptureOptions();
            }
            else
            {
                return new RegionCaptureOptions()
                {
                    SnapSizes = options.SnapSizes,
                    ShowMagnifier = options.ShowMagnifier,
                    UseSquareMagnifier = options.UseSquareMagnifier,
                    MagnifierPixelCount = options.MagnifierPixelCount,
                    MagnifierPixelSize = options.MagnifierPixelSize,
                    ShowCrosshair = options.ShowCrosshair,
                    AnnotationOptions = options.AnnotationOptions,
                    ShowMenuTip = options.ShowMenuTip
                };
            }
        }
    }
}