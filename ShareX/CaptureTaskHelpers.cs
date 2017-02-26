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
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ShareX
{
    public static class CaptureTaskHelpers
    {
        private delegate CaptureData ScreenCaptureDelegate();

        private enum LastRegionCaptureType { Default, Light, Transparent }

        private static LastRegionCaptureType lastRegionCaptureType = LastRegionCaptureType.Default;

        public static void CaptureScreenshot(CaptureType captureType, TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            switch (captureType)
            {
                case CaptureType.Fullscreen:
                    CaptureFullscreen(taskSettings, autoHideForm);
                    break;
                case CaptureType.ActiveWindow:
                    CaptureActiveWindow(taskSettings, autoHideForm);
                    break;
                case CaptureType.ActiveMonitor:
                    CaptureActiveMonitor(taskSettings, autoHideForm);
                    break;
                case CaptureType.Region:
                    CaptureRegion(taskSettings, autoHideForm);
                    break;
                case CaptureType.CustomRegion:
                    CaptureCustomRegion(taskSettings, autoHideForm);
                    break;
                case CaptureType.LastRegion:
                    CaptureLastRegion(taskSettings, autoHideForm);
                    break;
            }
        }

        private static void DoCapture(ScreenCaptureDelegate capture, CaptureType captureType, TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            if (taskSettings.CaptureSettings.IsDelayScreenshot && taskSettings.CaptureSettings.DelayScreenshot > 0)
            {
                TaskEx.Run(() =>
                {
                    int sleep = (int)(taskSettings.CaptureSettings.DelayScreenshot * 1000);
                    Thread.Sleep(sleep);
                },
                () =>
                {
                    DoCaptureWork(capture, captureType, taskSettings, autoHideForm);
                });
            }
            else
            {
                DoCaptureWork(capture, captureType, taskSettings, autoHideForm);
            }
        }

        private static void DoCaptureWork(ScreenCaptureDelegate capture, CaptureType captureType, TaskSettings taskSettings, bool autoHideForm = true)
        {
            if (autoHideForm)
            {
                Program.MainForm.Hide();
                Thread.Sleep(250);
            }

            CaptureData captureData = null;

            try
            {
                captureData = capture();
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
            }
            finally
            {
                if (autoHideForm)
                {
                    Program.MainForm.ForceActivate();
                }

                AfterCapture(captureData, captureType, taskSettings);
            }
        }

        private static void AfterCapture(CaptureData captureData, CaptureType captureType, TaskSettings taskSettings)
        {
            if (captureData != null && captureData.Image != null)
            {
                if (taskSettings.GeneralSettings.PlaySoundAfterCapture)
                {
                    TaskHelpers.PlayCaptureSound(taskSettings);
                }

                if (taskSettings.AdvancedSettings.UseShareXForAnnotation && taskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.AnnotateImage))
                {
                    taskSettings.AfterCaptureJob = taskSettings.AfterCaptureJob.Remove(AfterCaptureTasks.AnnotateImage);

                    if (captureType != CaptureType.Region)
                    {
                        captureData.Image = TaskHelpers.AnnotateImageUsingShareX(captureData.Image, taskSettings);
                    }
                }

                if (taskSettings.ImageSettings.ImageEffectOnlyRegionCapture && !IsRegionCapture(captureType))
                {
                    taskSettings.AfterCaptureJob = taskSettings.AfterCaptureJob.Remove(AfterCaptureTasks.AddImageEffects);
                }

                UploadManager.RunImageTask(captureData.Image, taskSettings);
            }
        }

        private static bool IsRegionCapture(CaptureType captureType)
        {
            return captureType.HasFlagAny(CaptureType.Region, CaptureType.LastRegion);
        }

        public static void CaptureFullscreen(TaskSettings taskSettings, bool autoHideForm = true)
        {
            DoCapture(() =>
            {
                Image img = TaskHelpers.GetScreenshot(taskSettings).CaptureFullscreen();
                return new CaptureData(img);
            }, CaptureType.Fullscreen, taskSettings, autoHideForm);
        }

        public static void CaptureWindow(IntPtr handle, TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            autoHideForm = autoHideForm && handle != Program.MainForm.Handle;

            DoCapture(() =>
            {
                if (NativeMethods.IsIconic(handle))
                {
                    NativeMethods.RestoreWindow(handle);
                }

                NativeMethods.SetForegroundWindow(handle);
                Thread.Sleep(250);

                Image img;

                if (taskSettings.CaptureSettings.CaptureTransparent && !taskSettings.CaptureSettings.CaptureClientArea)
                {
                    img = TaskHelpers.GetScreenshot(taskSettings).CaptureWindowTransparent(handle);
                }
                else
                {
                    img = TaskHelpers.GetScreenshot(taskSettings).CaptureWindow(handle);
                }

                return new CaptureData(img);
            }, CaptureType.Window, taskSettings, autoHideForm);
        }

        public static void CaptureActiveWindow(TaskSettings taskSettings, bool autoHideForm = true)
        {
            DoCapture(() =>
            {
                Image img;
                string activeWindowTitle = NativeMethods.GetForegroundWindowText();
                string activeProcessName = null;

                using (Process process = NativeMethods.GetForegroundWindowProcess())
                {
                    if (process != null)
                    {
                        activeProcessName = process.ProcessName;
                    }
                }

                if (taskSettings.CaptureSettings.CaptureTransparent && !taskSettings.CaptureSettings.CaptureClientArea)
                {
                    img = TaskHelpers.GetScreenshot(taskSettings).CaptureActiveWindowTransparent();
                }
                else
                {
                    img = TaskHelpers.GetScreenshot(taskSettings).CaptureActiveWindow();
                }

                return new CaptureData()
                {
                    Image = img,
                    WindowTitle = activeWindowTitle,
                    ProcessName = activeProcessName
                };
            }, CaptureType.ActiveWindow, taskSettings, autoHideForm);
        }

        public static void CaptureMonitor(Rectangle rect, TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            DoCapture(() =>
            {
                Image img = TaskHelpers.GetScreenshot().CaptureRectangle(rect);
                return new CaptureData(img);
            }, CaptureType.Monitor, taskSettings, autoHideForm);
        }

        public static void CaptureActiveMonitor(TaskSettings taskSettings, bool autoHideForm)
        {
            DoCapture(() =>
            {
                Image img = TaskHelpers.GetScreenshot(taskSettings).CaptureActiveMonitor();
                return new CaptureData(img);
            }, CaptureType.ActiveMonitor, taskSettings, autoHideForm);
        }

        public static void CaptureCustomRegion(TaskSettings taskSettings, bool autoHideForm)
        {
            DoCapture(() =>
            {
                Rectangle regionBounds = taskSettings.CaptureSettings.CaptureCustomRegion;
                Image img = TaskHelpers.GetScreenshot(taskSettings).CaptureRectangle(regionBounds);
                return new CaptureData(img);
            }, CaptureType.CustomRegion, taskSettings, autoHideForm);
        }

        public static void CaptureRegion(TaskSettings taskSettings, bool autoHideForm = true)
        {
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

            DoCapture(() =>
            {
                CaptureData captureData = new CaptureData();

                try
                {
                    form.Config = taskSettings.CaptureSettingsReference.SurfaceOptions;
                    form.Prepare(TaskHelpers.GetScreenshot(taskSettings).CaptureFullscreen());
                    form.ShowDialog();

                    captureData.Image = form.GetResultImage();

                    if (captureData.Image != null)
                    {
                        if (form.Result == RegionResult.Region && taskSettings.UploadSettings.RegionCaptureUseWindowPattern)
                        {
                            WindowInfo windowInfo = form.GetWindowInfo();

                            if (windowInfo != null)
                            {
                                captureData.WindowTitle = windowInfo.Text;
                                captureData.ProcessName = windowInfo.ProcessName;
                            }
                        }

                        lastRegionCaptureType = LastRegionCaptureType.Default;
                    }
                }
                finally
                {
                    if (form != null)
                    {
                        form.Dispose();
                    }
                }

                return captureData;
            }, CaptureType.Region, taskSettings, autoHideForm);
        }

        public static void CaptureRectangleLight(TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            DoCapture(() =>
            {
                Image img = null;

                using (RegionCaptureLightForm rectangleLight = new RegionCaptureLightForm(TaskHelpers.GetScreenshot(taskSettings)))
                {
                    if (rectangleLight.ShowDialog() == DialogResult.OK)
                    {
                        img = rectangleLight.GetAreaImage();

                        if (img != null)
                        {
                            lastRegionCaptureType = LastRegionCaptureType.Light;
                        }
                    }
                }

                return new CaptureData(img);
            }, CaptureType.Region, taskSettings, autoHideForm);
        }

        public static void CaptureRectangleTransparent(TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            DoCapture(() =>
            {
                Image img = null;

                using (RegionCaptureTransparentForm rectangleTransparent = new RegionCaptureTransparentForm())
                {
                    if (rectangleTransparent.ShowDialog() == DialogResult.OK)
                    {
                        img = rectangleTransparent.GetAreaImage(TaskHelpers.GetScreenshot(taskSettings));

                        if (img != null)
                        {
                            lastRegionCaptureType = LastRegionCaptureType.Transparent;
                        }
                    }
                }

                return new CaptureData(img);
            }, CaptureType.Region, taskSettings, autoHideForm);
        }

        public static void CaptureLastRegion(TaskSettings taskSettings, bool autoHideForm = true)
        {
            switch (lastRegionCaptureType)
            {
                case LastRegionCaptureType.Default:
                    if (RegionCaptureForm.LastRegionFillPath != null)
                    {
                        DoCapture(() =>
                        {
                            using (Image screenshot = TaskHelpers.GetScreenshot(taskSettings).CaptureFullscreen())
                            {
                                Image img = RegionCaptureTasks.ApplyRegionPathToImage(screenshot, RegionCaptureForm.LastRegionFillPath);
                                return new CaptureData(img);
                            }
                        }, CaptureType.LastRegion, taskSettings, autoHideForm);
                    }
                    else
                    {
                        CaptureRegion(taskSettings, autoHideForm);
                    }
                    break;
                case LastRegionCaptureType.Light:
                    if (!RegionCaptureLightForm.LastSelectionRectangle0Based.IsEmpty)
                    {
                        DoCapture(() =>
                        {
                            using (Image screenshot = TaskHelpers.GetScreenshot(taskSettings).CaptureFullscreen())
                            {
                                Image img = ImageHelpers.CropImage(screenshot, RegionCaptureLightForm.LastSelectionRectangle0Based);
                                return new CaptureData(img);
                            }
                        }, CaptureType.LastRegion, taskSettings, autoHideForm);
                    }
                    else
                    {
                        CaptureRectangleLight(taskSettings, autoHideForm);
                    }
                    break;
                case LastRegionCaptureType.Transparent:
                    if (!RegionCaptureTransparentForm.LastSelectionRectangle0Based.IsEmpty)
                    {
                        DoCapture(() =>
                        {
                            using (Image screenshot = TaskHelpers.GetScreenshot(taskSettings).CaptureFullscreen())
                            {
                                Image img = ImageHelpers.CropImage(screenshot, RegionCaptureTransparentForm.LastSelectionRectangle0Based);
                                return new CaptureData(img);
                            }
                        }, CaptureType.LastRegion, taskSettings, autoHideForm);
                    }
                    else
                    {
                        CaptureRectangleTransparent(taskSettings, autoHideForm);
                    }
                    break;
            }
        }
    }
}