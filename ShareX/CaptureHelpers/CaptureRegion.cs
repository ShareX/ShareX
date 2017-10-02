#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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

        protected override ImageInfo Execute(TaskSettings taskSettings)
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

        protected ImageInfo ExecuteRegionCapture(TaskSettings taskSettings)
        {
            ImageInfo imageInfo = new ImageInfo();

            RegionCaptureMode mode;

            if (taskSettings.AdvancedSettings.RegionCaptureDisableAnnotation)
            {
                mode = RegionCaptureMode.Default;
            }
            else
            {
                mode = RegionCaptureMode.Annotation;
            }

            RegionCaptureForm form = new RegionCaptureForm(mode);

            try
            {
                form.Config = taskSettings.CaptureSettingsReference.SurfaceOptions;
                Screenshot screenshot = TaskHelpers.GetScreenshot(taskSettings);
                screenshot.CaptureCursor = false;
                Image img = screenshot.CaptureFullscreen();

                CursorData cursorData = null;

                try
                {
                    if (taskSettings.CaptureSettings.ShowCursor)
                    {
                        cursorData = new CursorData();
                    }

                    form.Prepare(img);

                    if (cursorData != null && cursorData.IsValid)
                    {
                        form.AddCursor(cursorData.Handle, cursorData.Position);
                    }
                }
                finally
                {
                    if (cursorData != null)
                    {
                        cursorData.Dispose();
                    }
                }

                form.ShowDialog();

                imageInfo.Image = form.GetResultImage();

                if (imageInfo.Image != null)
                {
                    if (form.IsAnnotated)
                    {
                        AllowAnnotation = false;
                    }

                    if (form.Result == RegionResult.Region && taskSettings.UploadSettings.RegionCaptureUseWindowPattern)
                    {
                        WindowInfo windowInfo = form.GetWindowInfo();
                        imageInfo.UpdateInfo(windowInfo);
                    }

                    lastRegionCaptureType = RegionCaptureType.Default;
                }
            }
            finally
            {
                if (form != null)
                {
                    form.Dispose();
                }
            }

            return imageInfo;
        }

        protected ImageInfo ExecuteRegionCaptureLight(TaskSettings taskSettings)
        {
            Image img = null;

            using (RegionCaptureLightForm rectangleLight = new RegionCaptureLightForm(TaskHelpers.GetScreenshot(taskSettings)))
            {
                if (rectangleLight.ShowDialog() == DialogResult.OK)
                {
                    img = rectangleLight.GetAreaImage();

                    if (img != null)
                    {
                        lastRegionCaptureType = RegionCaptureType.Light;
                    }
                }
            }

            return new ImageInfo(img);
        }

        protected ImageInfo ExecuteRegionCaptureTransparent(TaskSettings taskSettings)
        {
            Image img = null;

            using (RegionCaptureTransparentForm rectangleTransparent = new RegionCaptureTransparentForm())
            {
                if (rectangleTransparent.ShowDialog() == DialogResult.OK)
                {
                    img = rectangleTransparent.GetAreaImage(TaskHelpers.GetScreenshot(taskSettings));

                    if (img != null)
                    {
                        lastRegionCaptureType = RegionCaptureType.Transparent;
                    }
                }
            }

            return new ImageInfo(img);
        }
    }
}