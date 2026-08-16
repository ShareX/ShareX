#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

using ShareX.ScreenCaptureLib;
using ShareX.ScreenCaptureLib.Presentation.RegionCapture;
using System.Drawing;
using System.Threading.Tasks;

namespace ShareX
{
    public class CaptureLastRegion : CaptureRegion
    {
        protected override async Task<TaskMetadata> ExecuteAsync(TaskSettings taskSettings)
        {
            if (lastRegionCaptureType == RegionCaptureType.Default &&
                RegionCaptureIntegration.LastRegionRectangle.IsEmpty)
            {
                return await ExecuteRegionCaptureAvaloniaAsync(taskSettings);
            }

            return Execute(taskSettings);
        }

        protected override TaskMetadata Execute(TaskSettings taskSettings)
        {
            switch (lastRegionCaptureType)
            {
                default:
                case RegionCaptureType.Default:
                    if (!RegionCaptureIntegration.LastRegionRectangle.IsEmpty)
                    {
                        Bitmap bmp = TaskHelpers.GetScreenshot(taskSettings).CaptureRectangle(
                            RegionCaptureIntegration.LastRegionRectangle);
                        return new TaskMetadata(bmp);
                    }
                    return ExecuteRegionCapture(taskSettings);
                case RegionCaptureType.Light:
                    if (!RegionCaptureLightForm.LastScreenSelectionRectangle.IsEmpty)
                    {
                        Bitmap bmp = TaskHelpers.GetScreenshot(taskSettings).CaptureRectangle(RegionCaptureLightForm.LastScreenSelectionRectangle);
                        return new TaskMetadata(bmp);
                    }
                    else
                    {
                        return ExecuteRegionCaptureLight(taskSettings);
                    }
                case RegionCaptureType.Transparent:
                    if (!RegionCaptureLightForm.LastScreenSelectionRectangle.IsEmpty)
                    {
                        Bitmap bmp = TaskHelpers.GetScreenshot(taskSettings).CaptureRectangle(RegionCaptureLightForm.LastScreenSelectionRectangle);
                        return new TaskMetadata(bmp);
                    }
                    else
                    {
                        return ExecuteRegionCaptureTransparent(taskSettings);
                    }
            }
        }
    }
}
