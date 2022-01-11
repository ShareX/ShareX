#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2022 ShareX Team

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
using System.Windows.Forms;

namespace ShareX
{
    public class CaptureRegion : CaptureBase
    {
        protected static RegionCaptureType lastRegionCaptureType = RegionCaptureType.Default;

        public RegionCaptureType RegionCaptureType { get; protected set; }

        public CaptureRegion()
        {
        }

        public CaptureRegion(RegionCaptureType regionCaptureType)
        {
            RegionCaptureType = regionCaptureType;
        }

        protected override TaskMetadata Execute(TaskSettings taskSettings)
        {
            switch (RegionCaptureType)
            {
                default:
                case RegionCaptureType.Default:
                    return ExecuteRegionCapture(taskSettings);
                case RegionCaptureType.Light:
                    return ExecuteRegionCaptureLight(taskSettings);
                case RegionCaptureType.Transparent:
                    return ExecuteRegionCaptureTransparent(taskSettings);
            }
        }

        protected TaskMetadata ExecuteRegionCapture(TaskSettings taskSettings)
        {
            TaskMetadata metadata = new TaskMetadata();

            RegionCaptureMode mode;

            if (taskSettings.AdvancedSettings.RegionCaptureDisableAnnotation)
            {
                mode = RegionCaptureMode.Default;
            }
            else
            {
                mode = RegionCaptureMode.Annotation;
            }

            Screenshot screenshot = TaskHelpers.GetScreenshot(taskSettings);
            screenshot.CaptureCursor = false;
            Bitmap bmp = screenshot.CaptureFullscreen();

            CursorData cursorData = null;

            if (taskSettings.CaptureSettings.ShowCursor)
            {
                cursorData = new CursorData();
            }

            using (RegionCaptureForm form = new RegionCaptureForm(mode, taskSettings.CaptureSettingsReference.SurfaceOptions, bmp))
            {
                if (cursorData != null && cursorData.IsVisible)
                {
                    form.AddCursor(cursorData.Handle, CaptureHelpers.ScreenToClient(cursorData.Position));
                }

                form.ShowDialog();

                metadata.Image = form.GetResultImage();

                if (metadata.Image != null)
                {
                    if (form.IsImageModified)
                    {
                        AllowAnnotation = false;
                    }

                    if (form.Result == RegionResult.Region)
                    {
                        WindowInfo windowInfo = form.GetWindowInfo();
                        metadata.UpdateInfo(windowInfo);
                    }

                    lastRegionCaptureType = RegionCaptureType.Default;
                }
            }

            return metadata;
        }

        protected TaskMetadata ExecuteRegionCaptureLight(TaskSettings taskSettings)
        {
            Bitmap bmp = null;

            using (RegionCaptureLightForm rectangleLight = new RegionCaptureLightForm(TaskHelpers.GetScreenshot(taskSettings)))
            {
                if (rectangleLight.ShowDialog() == DialogResult.OK)
                {
                    bmp = rectangleLight.GetAreaImage();

                    if (bmp != null)
                    {
                        lastRegionCaptureType = RegionCaptureType.Light;
                    }
                }
            }

            return new TaskMetadata(bmp);
        }

        protected TaskMetadata ExecuteRegionCaptureTransparent(TaskSettings taskSettings)
        {
            Bitmap bmp = null;

            using (RegionCaptureTransparentForm rectangleTransparent = new RegionCaptureTransparentForm())
            {
                if (rectangleTransparent.ShowDialog() == DialogResult.OK)
                {
                    bmp = rectangleTransparent.GetAreaImage(TaskHelpers.GetScreenshot(taskSettings));

                    if (bmp != null)
                    {
                        lastRegionCaptureType = RegionCaptureType.Transparent;
                    }
                }
            }

            return new TaskMetadata(bmp);
        }
    }
}