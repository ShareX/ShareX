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
using System;
using System.Drawing;
using System.Threading;

namespace ShareX
{
    public abstract class CaptureBase
    {
        public bool AllowAutoHideForm { get; set; } = true;

        protected abstract ImageInfo Execute(TaskSettings taskSettings);

        public void Capture(bool autoHideForm)
        {
            Capture(null, autoHideForm);
        }

        public void Capture(TaskSettings taskSettings = null, bool autoHideForm = false)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            if (taskSettings.CaptureSettings.IsDelayScreenshot && taskSettings.CaptureSettings.DelayScreenshot > 0)
            {
                int delay = (int)(taskSettings.CaptureSettings.DelayScreenshot * 1000);

                TaskEx.RunDelayed(() =>
                {
                    CaptureInternal(taskSettings, autoHideForm);
                }, delay);
            }
            else
            {
                CaptureInternal(taskSettings, autoHideForm);
            }
        }

        private void CaptureInternal(TaskSettings taskSettings, bool autoHideForm)
        {
            if (autoHideForm && AllowAutoHideForm)
            {
                Program.MainForm.Hide();
                Thread.Sleep(250);
            }

            ImageInfo imageInfo = null;

            try
            {
                imageInfo = Execute(taskSettings);
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
            }
            finally
            {
                if (autoHideForm && AllowAutoHideForm)
                {
                    Program.MainForm.ForceActivate();
                }

                AfterCapture(imageInfo, taskSettings);
            }
        }

        private void AfterCapture(ImageInfo imageInfo, TaskSettings taskSettings)
        {
            if (imageInfo != null && imageInfo.Image != null)
            {
                if (taskSettings.GeneralSettings.PlaySoundAfterCapture)
                {
                    TaskHelpers.PlayCaptureSound(taskSettings);
                }

                if (taskSettings.AdvancedSettings.UseShareXForAnnotation && taskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.AnnotateImage) &&
                    this.GetType() == typeof(CaptureRegion))
                {
                    taskSettings.AfterCaptureJob = taskSettings.AfterCaptureJob.Remove(AfterCaptureTasks.AnnotateImage);
                }

                if (taskSettings.ImageSettings.ImageEffectOnlyRegionCapture &&
                    this.GetType() != typeof(CaptureRegion) && this.GetType() != typeof(CaptureLastRegion))
                {
                    taskSettings.AfterCaptureJob = taskSettings.AfterCaptureJob.Remove(AfterCaptureTasks.AddImageEffects);
                }

                UploadManager.RunImageTask(imageInfo, taskSettings);
            }
        }

        protected ImageInfo CreateImageInfo()
        {
            return CreateImageInfo(Rectangle.Empty, null);
        }

        protected ImageInfo CreateImageInfo(Rectangle insideRect)
        {
            return CreateImageInfo(insideRect, "explorer");
        }

        protected ImageInfo CreateImageInfo(Rectangle insideRect, string ignoreProcess)
        {
            ImageInfo imageInfo = new ImageInfo();

            IntPtr handle = NativeMethods.GetForegroundWindow();
            WindowInfo windowInfo = new WindowInfo(handle);

            if ((ignoreProcess == null || !windowInfo.ProcessName.Equals(ignoreProcess, StringComparison.InvariantCultureIgnoreCase)) &&
                (insideRect.IsEmpty || windowInfo.Rectangle.Contains(insideRect)))
            {
                imageInfo.WindowTitle = windowInfo.Text;
                imageInfo.ProcessName = windowInfo.ProcessName;
            }

            return imageInfo;
        }
    }
}