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
        public delegate Image ScreenCaptureDelegate();

        private enum LastRegionCaptureType { Default, Light, Transparent }

        private static LastRegionCaptureType lastRegionCaptureType = LastRegionCaptureType.Default;

        public static void CaptureScreenshot(CaptureType captureType, TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            switch (captureType)
            {
                case CaptureType.Fullscreen:
                    DoCapture(TaskHelpers.GetScreenshot(taskSettings).CaptureFullscreen, CaptureType.Fullscreen, taskSettings, autoHideForm);
                    break;
                case CaptureType.ActiveWindow:
                    CaptureActiveWindow(taskSettings, autoHideForm);
                    break;
                case CaptureType.ActiveMonitor:
                    DoCapture(TaskHelpers.GetScreenshot(taskSettings).CaptureActiveMonitor, CaptureType.ActiveMonitor, taskSettings, autoHideForm);
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

        public static void DoCapture(ScreenCaptureDelegate capture, CaptureType captureType, TaskSettings taskSettings = null, bool autoHideForm = true)
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

            Image img = null;

            try
            {
                img = capture();
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

                AfterCapture(img, captureType, taskSettings);
            }
        }

        private static void AfterCapture(Image img, CaptureType captureType, TaskSettings taskSettings)
        {
            if (img != null)
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
                        img = TaskHelpers.AnnotateImageUsingShareX(img, taskSettings);
                    }
                }

                if (taskSettings.ImageSettings.ImageEffectOnlyRegionCapture && !IsRegionCapture(captureType))
                {
                    taskSettings.AfterCaptureJob = taskSettings.AfterCaptureJob.Remove(AfterCaptureTasks.AddImageEffects);
                }

                UploadManager.RunImageTask(img, taskSettings);
            }
        }

        private static bool IsRegionCapture(CaptureType captureType)
        {
            return captureType.HasFlagAny(CaptureType.Region, CaptureType.LastRegion);
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

                img.Tag = new ImageTag
                {
                    WindowTitle = activeWindowTitle,
                    ProcessName = activeProcessName
                };

                return img;
            }, CaptureType.ActiveWindow, taskSettings, autoHideForm);
        }

        public static void CaptureCustomRegion(TaskSettings taskSettings, bool autoHideForm)
        {
            DoCapture(() =>
            {
                Rectangle regionBounds = taskSettings.CaptureSettings.CaptureCustomRegion;
                Image img = TaskHelpers.GetScreenshot(taskSettings).CaptureRectangle(regionBounds);

                return img;
            }, CaptureType.CustomRegion, taskSettings, autoHideForm);
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

                if (taskSettings.CaptureSettings.CaptureTransparent && !taskSettings.CaptureSettings.CaptureClientArea)
                {
                    return TaskHelpers.GetScreenshot(taskSettings).CaptureWindowTransparent(handle);
                }

                return TaskHelpers.GetScreenshot(taskSettings).CaptureWindow(handle);
            }, CaptureType.Window, taskSettings, autoHideForm);
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
                Image img = null;

                try
                {
                    form.Config = taskSettings.CaptureSettingsReference.SurfaceOptions;
                    form.Prepare(TaskHelpers.GetScreenshot(taskSettings).CaptureFullscreen());
                    form.ShowDialog();

                    img = form.GetResultImage();

                    if (img != null)
                    {
                        if (form.Result == RegionResult.Region && taskSettings.UploadSettings.RegionCaptureUseWindowPattern)
                        {
                            WindowInfo windowInfo = form.GetWindowInfo();

                            if (windowInfo != null)
                            {
                                img.Tag = new ImageTag
                                {
                                    WindowTitle = windowInfo.Text,
                                    ProcessName = windowInfo.ProcessName
                                };
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

                return img;
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

                return img;
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

                return img;
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
                                return RegionCaptureTasks.ApplyRegionPathToImage(screenshot, RegionCaptureForm.LastRegionFillPath);
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
                                return ImageHelpers.CropImage(screenshot, RegionCaptureLightForm.LastSelectionRectangle0Based);
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
                                return ImageHelpers.CropImage(screenshot, RegionCaptureTransparentForm.LastSelectionRectangle0Based);
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