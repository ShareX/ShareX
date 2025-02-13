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
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public static class RegionCaptureTasks
    {
        public static Bitmap GetRegionImage(RegionCaptureOptions options = null)
        {
            RegionCaptureOptions newOptions = GetRegionCaptureOptions(options);

            using (RegionCaptureForm form = new RegionCaptureForm(RegionCaptureMode.Default, newOptions))
            {
                form.ShowDialog();

                return form.GetResultImage();
            }
        }

        public static Bitmap GetRegionImage(out Rectangle rect, RegionCaptureOptions options = null)
        {
            RegionCaptureOptions newOptions = GetRegionCaptureOptions(options);

            using (RegionCaptureForm form = new RegionCaptureForm(RegionCaptureMode.Default, newOptions))
            {
                form.ShowDialog();

                rect = form.GetSelectedRectangle();
                return form.GetResultImage();
            }
        }

        public static bool GetRectangleRegion(out Rectangle rect, RegionCaptureOptions options = null)
        {
            RegionCaptureOptions newOptions = GetRegionCaptureOptions(options);

            using (RegionCaptureForm form = new RegionCaptureForm(RegionCaptureMode.Default, newOptions))
            {
                form.ShowDialog();

                rect = form.GetSelectedRectangle();
            }

            return !rect.IsEmpty;
        }

        public static bool GetRectangleRegion(out Rectangle rect, out WindowInfo windowInfo, RegionCaptureOptions options = null)
        {
            RegionCaptureOptions newOptions = GetRegionCaptureOptions(options);

            using (RegionCaptureForm form = new RegionCaptureForm(RegionCaptureMode.Default, newOptions))
            {
                form.ShowDialog();

                rect = form.GetSelectedRectangle();
                windowInfo = form.GetWindowInfo();
            }

            return !rect.IsEmpty;
        }

        public static bool GetRectangleRegionTransparent(out Rectangle rect)
        {
            using (RegionCaptureTransparentForm regionCaptureTransparentForm = new RegionCaptureTransparentForm())
            {
                if (regionCaptureTransparentForm.ShowDialog() == DialogResult.OK)
                {
                    rect = regionCaptureTransparentForm.SelectionRectangle;
                    return true;
                }
            }

            rect = Rectangle.Empty;
            return false;
        }

        public static PointInfo GetPointInfo(RegionCaptureOptions options, Bitmap canvas = null)
        {
            RegionCaptureOptions newOptions = GetRegionCaptureOptions(options);
            newOptions.DetectWindows = false;
            newOptions.BackgroundDimStrength = 0;

            using (RegionCaptureForm form = new RegionCaptureForm(RegionCaptureMode.ScreenColorPicker, newOptions, canvas))
            {
                form.ShowDialog();

                if (form.Result == RegionResult.Region)
                {
                    PointInfo pointInfo = new PointInfo();
                    pointInfo.Position = form.CurrentPosition;
                    pointInfo.Color = form.ShapeManager.GetCurrentColor();
                    return pointInfo;
                }
            }

            return null;
        }

        public static SimpleWindowInfo GetWindowInfo(RegionCaptureOptions options)
        {
            RegionCaptureOptions newOptions = GetRegionCaptureOptions(options);
            newOptions.BackgroundDimStrength = 0;
            newOptions.ShowMagnifier = false;

            using (RegionCaptureForm form = new RegionCaptureForm(RegionCaptureMode.OneClick, newOptions))
            {
                form.ShowDialog();

                if (form.Result == RegionResult.Region)
                {
                    return form.SelectedWindow;
                }
            }

            return null;
        }

        public static void ShowScreenColorPickerDialog(RegionCaptureOptions options)
        {
            Color color = Color.Red;
            ColorPickerForm colorPickerForm = new ColorPickerForm(color, true, true, options.ColorPickerOptions);
            colorPickerForm.EnableScreenColorPickerButton(() => GetPointInfo(options));
            colorPickerForm.Show();
        }

        public static void ShowScreenRuler(RegionCaptureOptions options)
        {
            RegionCaptureOptions newOptions = GetRegionCaptureOptions(options);
            newOptions.QuickCrop = false;
            newOptions.UseLightResizeNodes = true;

            using (RegionCaptureForm form = new RegionCaptureForm(RegionCaptureMode.Ruler, newOptions))
            {
                form.ShowDialog();
            }
        }

        public static Bitmap ApplyRegionPathToImage(Bitmap bmp, GraphicsPath gp, out Rectangle resultArea)
        {
            if (bmp != null && gp != null)
            {
                Rectangle regionArea = Rectangle.Round(gp.GetBounds());
                Rectangle screenRectangle = CaptureHelpers.GetScreenBounds();
                resultArea = Rectangle.Intersect(regionArea, new Rectangle(0, 0, screenRectangle.Width, screenRectangle.Height));

                if (resultArea.IsValid())
                {
                    using (Bitmap bmpResult = bmp.CreateEmptyBitmap())
                    using (Graphics g = Graphics.FromImage(bmpResult))
                    using (TextureBrush brush = new TextureBrush(bmp))
                    {
                        g.PixelOffsetMode = PixelOffsetMode.Half;
                        g.SmoothingMode = SmoothingMode.HighQuality;

                        g.FillPath(brush, gp);

                        return ImageHelpers.CropBitmap(bmpResult, resultArea);
                    }
                }
            }

            resultArea = Rectangle.Empty;
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
                    DetectControls = options.DetectControls,
                    SnapSizes = options.SnapSizes,
                    ShowMagnifier = options.ShowMagnifier,
                    UseSquareMagnifier = options.UseSquareMagnifier,
                    MagnifierPixelCount = options.MagnifierPixelCount,
                    MagnifierPixelSize = options.MagnifierPixelSize,
                    ShowCrosshair = options.ShowCrosshair,
                    AnnotationOptions = options.AnnotationOptions,
                    ScreenColorPickerInfoText = options.ScreenColorPickerInfoText
                };
            }
        }
    }
}