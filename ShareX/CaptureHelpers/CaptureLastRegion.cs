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
using ShareX.ScreenCaptureLib;
using System.Drawing;

namespace ShareX
{
    public class CaptureLastRegion : CaptureRegion
    {
        protected override TaskMetadata Execute(TaskSettings taskSettings)
        {
            switch (lastRegionCaptureType)
            {
                default:
                case RegionCaptureType.Default:
                    if (RegionCaptureForm.LastRegionFillPath != null)
                    {
                        using (Bitmap screenshot = TaskHelpers.GetScreenshot(taskSettings).CaptureFullscreen())
                        {
                            Bitmap bmp = RegionCaptureTasks.ApplyRegionPathToImage(screenshot, RegionCaptureForm.LastRegionFillPath, out _);
                            return new TaskMetadata(bmp);
                        }
                    }
                    else
                    {
                        return ExecuteRegionCapture(taskSettings);
                    }
                case RegionCaptureType.Light:
                    if (!RegionCaptureLightForm.LastSelectionRectangle0Based.IsEmpty)
                    {
                        using (Bitmap screenshot = TaskHelpers.GetScreenshot(taskSettings).CaptureFullscreen())
                        {
                            Bitmap bmp = ImageHelpers.CropBitmap(screenshot, RegionCaptureLightForm.LastSelectionRectangle0Based);
                            return new TaskMetadata(bmp);
                        }
                    }
                    else
                    {
                        return ExecuteRegionCaptureLight(taskSettings);
                    }
                case RegionCaptureType.Transparent:
                    if (!RegionCaptureTransparentForm.LastSelectionRectangle0Based.IsEmpty)
                    {
                        using (Bitmap screenshot = TaskHelpers.GetScreenshot(taskSettings).CaptureFullscreen())
                        {
                            Bitmap bmp = ImageHelpers.CropBitmap(screenshot, RegionCaptureTransparentForm.LastSelectionRectangle0Based);
                            return new TaskMetadata(bmp);
                        }
                    }
                    else
                    {
                        return ExecuteRegionCaptureTransparent(taskSettings);
                    }
            }
        }
    }
}