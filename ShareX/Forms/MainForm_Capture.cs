#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

using HelpersLib;
using ScreenCaptureLib;
using ShareX.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX
{
    public partial class MainForm
    {
        private delegate Image ScreenCaptureDelegate();

        private void InitHotkeys()
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                if (Program.HotkeysConfig == null)
                {
                    Program.HotkeySettingsResetEvent.WaitOne();
                }

                Invoke(new MethodInvoker(() =>
                {
                    Program.HotkeyManager = new HotkeyManager(this, Program.HotkeysConfig.Hotkeys);
                    Program.HotkeyManager.HotkeyTrigger += HandleHotkeys;
                    DebugHelper.WriteLine("HotkeyManager started");

                    Program.WatchFolderManager = new WatchFolderManager();
                    DebugHelper.WriteLine("WatchFolderManager started");

                    UpdateWorkflowsMenu();
                }));
            });
        }

        private void HandleHotkeys(HotkeySettings hotkeySetting)
        {
            DebugHelper.WriteLine("Hotkey triggered: " + hotkeySetting);

            if (hotkeySetting.TaskSettings.Job == HotkeyType.None) return;

            HandleTask(hotkeySetting.TaskSettings);
        }

        private void HandleTask(TaskSettings taskSettings)
        {
            TaskSettings safeTaskSettings = TaskSettings.GetSafeTaskSettings(taskSettings);

            switch (safeTaskSettings.Job)
            {
                case HotkeyType.StopUploads:
                    TaskManager.StopAllTasks();
                    break;
                case HotkeyType.ClipboardUpload:
                    UploadManager.ClipboardUpload(safeTaskSettings);
                    break;
                case HotkeyType.ClipboardUploadWithContentViewer:
                    UploadManager.ClipboardUploadWithContentViewer(safeTaskSettings);
                    break;
                case HotkeyType.FileUpload:
                    UploadManager.UploadFile(safeTaskSettings);
                    break;
                case HotkeyType.DragDropUpload:
                    TaskHelpers.OpenDropWindow();
                    break;
                case HotkeyType.PrintScreen:
                    CaptureScreenshot(CaptureType.Screen, safeTaskSettings, false);
                    break;
                case HotkeyType.ActiveWindow:
                    CaptureScreenshot(CaptureType.ActiveWindow, safeTaskSettings, false);
                    break;
                case HotkeyType.ActiveMonitor:
                    CaptureScreenshot(CaptureType.ActiveMonitor, safeTaskSettings, false);
                    break;
                case HotkeyType.RectangleRegion:
                    CaptureScreenshot(CaptureType.Rectangle, safeTaskSettings, false);
                    break;
                case HotkeyType.WindowRectangle:
                    CaptureScreenshot(CaptureType.RectangleWindow, safeTaskSettings, false);
                    break;
                case HotkeyType.RoundedRectangleRegion:
                    CaptureScreenshot(CaptureType.RoundedRectangle, safeTaskSettings, false);
                    break;
                case HotkeyType.EllipseRegion:
                    CaptureScreenshot(CaptureType.Ellipse, safeTaskSettings, false);
                    break;
                case HotkeyType.TriangleRegion:
                    CaptureScreenshot(CaptureType.Triangle, safeTaskSettings, false);
                    break;
                case HotkeyType.DiamondRegion:
                    CaptureScreenshot(CaptureType.Diamond, safeTaskSettings, false);
                    break;
                case HotkeyType.PolygonRegion:
                    CaptureScreenshot(CaptureType.Polygon, safeTaskSettings, false);
                    break;
                case HotkeyType.FreeHandRegion:
                    CaptureScreenshot(CaptureType.Freehand, safeTaskSettings, false);
                    break;
                case HotkeyType.LastRegion:
                    CaptureScreenshot(CaptureType.LastRegion, safeTaskSettings, false);
                    break;
                case HotkeyType.ScreenRecorder:
                    TaskHelpers.DoScreenRecorder(safeTaskSettings);
                    break;
                case HotkeyType.AutoCapture:
                    TaskHelpers.OpenAutoCapture();
                    break;
                case HotkeyType.ScreenColorPicker:
                    TaskHelpers.OpenScreenColorPicker(safeTaskSettings);
                    break;
                case HotkeyType.Ruler:
                    TaskHelpers.OpenRuler();
                    break;
                case HotkeyType.FTPClient:
                    TaskHelpers.OpenFTPClient();
                    break;
                case HotkeyType.HashCheck:
                    TaskHelpers.OpenHashCheck();
                    break;
                case HotkeyType.IndexFolder:
                    TaskHelpers.OpenIndexFolder();
                    break;
                case HotkeyType.ImageEffects:
                    TaskHelpers.OpenImageEffects();
                    break;
            }
        }

        public void CaptureScreenshot(CaptureType captureType, TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            switch (captureType)
            {
                case CaptureType.Screen:
                    DoCapture(Screenshot.CaptureFullscreen, CaptureType.Screen, taskSettings, autoHideForm);
                    break;
                case CaptureType.ActiveWindow:
                    CaptureActiveWindow(taskSettings, autoHideForm);
                    break;
                case CaptureType.ActiveMonitor:
                    DoCapture(Screenshot.CaptureActiveMonitor, CaptureType.ActiveMonitor, taskSettings, autoHideForm);
                    break;
                case CaptureType.RectangleWindow:
                case CaptureType.Rectangle:
                case CaptureType.RoundedRectangle:
                case CaptureType.Ellipse:
                case CaptureType.Triangle:
                case CaptureType.Diamond:
                case CaptureType.Polygon:
                case CaptureType.Freehand:
                    CaptureRegion(captureType, taskSettings, autoHideForm);
                    break;
                case CaptureType.LastRegion:
                    CaptureLastRegion(taskSettings, autoHideForm);
                    break;
            }
        }

        private void DoCapture(ScreenCaptureDelegate capture, CaptureType captureType, TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            if (taskSettings.CaptureSettings.IsDelayScreenshot && taskSettings.CaptureSettings.DelayScreenshot > 0)
            {
                int sleep = (int)(taskSettings.CaptureSettings.DelayScreenshot * 1000);
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += (sender, e) => Thread.Sleep(sleep);
                bw.RunWorkerCompleted += (sender, e) => DoCaptureWork(capture, captureType, taskSettings, autoHideForm);
                bw.RunWorkerAsync();
            }
            else
            {
                DoCaptureWork(capture, captureType, taskSettings, autoHideForm);
            }
        }

        private void DoCaptureWork(ScreenCaptureDelegate capture, CaptureType captureType, TaskSettings taskSettings, bool autoHideForm = true)
        {
            if (autoHideForm)
            {
                Hide();
                Thread.Sleep(250);
            }

            Image img = null;

            try
            {
                Screenshot.CaptureCursor = taskSettings.CaptureSettings.ShowCursor;
                Screenshot.CaptureShadow = taskSettings.CaptureSettings.CaptureShadow;
                Screenshot.ShadowOffset = taskSettings.CaptureSettings.CaptureShadowOffset;
                Screenshot.CaptureClientArea = taskSettings.CaptureSettings.CaptureClientArea;
                Screenshot.AutoHideTaskbar = taskSettings.CaptureSettings.CaptureAutoHideTaskbar;

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
                    this.ShowActivate();
                }

                AfterCapture(img, captureType, taskSettings);
            }
        }

        private void AfterCapture(Image img, CaptureType captureType, TaskSettings taskSettings)
        {
            if (img != null)
            {
                if (taskSettings.GeneralSettings.PlaySoundAfterCapture)
                {
                    Helpers.PlaySoundAsync(Resources.CameraSound);
                }

                if (taskSettings.ImageSettings.ImageEffectOnlyRegionCapture && !IsRegionCapture(captureType))
                {
                    taskSettings.AfterCaptureJob = taskSettings.AfterCaptureJob.Remove(AfterCaptureTasks.AddImageEffects);
                }

                if (taskSettings.GeneralSettings.ShowAfterCaptureTasksForm)
                {
                    using (AfterCaptureForm afterCaptureForm = new AfterCaptureForm(img, taskSettings))
                    {
                        afterCaptureForm.ShowDialog();

                        switch (afterCaptureForm.Result)
                        {
                            case AfterCaptureFormResult.Continue:
                                taskSettings.AfterCaptureJob = afterCaptureForm.AfterCaptureTasks;
                                break;
                            case AfterCaptureFormResult.Copy:
                                taskSettings.AfterCaptureJob = AfterCaptureTasks.CopyImageToClipboard;
                                break;
                            case AfterCaptureFormResult.Cancel:
                                if (img != null) img.Dispose();
                                return;
                        }
                    }
                }

                UploadManager.RunImageTask(img, taskSettings);
            }
        }

        private bool IsRegionCapture(CaptureType captureType)
        {
            return captureType.HasFlagAny(CaptureType.RectangleWindow, CaptureType.Rectangle, CaptureType.RoundedRectangle, CaptureType.Ellipse,
                CaptureType.Triangle, CaptureType.Diamond, CaptureType.Polygon, CaptureType.Freehand, CaptureType.LastRegion);
        }

        private void CaptureActiveWindow(TaskSettings taskSettings, bool autoHideForm = true)
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
                    img = Screenshot.CaptureActiveWindowTransparent();
                }
                else
                {
                    img = Screenshot.CaptureActiveWindow();
                }

                img.Tag = new ImageTag
                {
                    ActiveWindowTitle = activeWindowTitle,
                    ActiveProcessName = activeProcessName
                };

                return img;
            }, CaptureType.ActiveWindow, taskSettings, autoHideForm);
        }

        private void CaptureWindow(IntPtr handle, TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            autoHideForm = autoHideForm && handle != Handle;

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
                    return Screenshot.CaptureWindowTransparent(handle);
                }

                return Screenshot.CaptureWindow(handle);
            }, CaptureType.Window, taskSettings, autoHideForm);
        }

        private void CaptureRegion(CaptureType captureType, TaskSettings taskSettings, bool autoHideForm = true)
        {
            Surface surface;

            switch (captureType)
            {
                default:
                case CaptureType.Rectangle:
                    if (taskSettings.AdvancedSettings.UseLightRectangleCrop)
                    {
                        CaptureLightRectangle(taskSettings, autoHideForm);
                        return;
                    }

                    surface = new RectangleRegion();
                    break;
                case CaptureType.RectangleWindow:
                    RectangleRegion rectangleRegion = new RectangleRegion();
                    rectangleRegion.AreaManager.WindowCaptureMode = true;
                    surface = rectangleRegion;
                    break;
                case CaptureType.RoundedRectangle:
                    surface = new RoundedRectangleRegion();
                    break;
                case CaptureType.Ellipse:
                    surface = new EllipseRegion();
                    break;
                case CaptureType.Triangle:
                    surface = new TriangleRegion();
                    break;
                case CaptureType.Diamond:
                    surface = new DiamondRegion();
                    break;
                case CaptureType.Polygon:
                    surface = new PolygonRegion();
                    break;
                case CaptureType.Freehand:
                    surface = new FreeHandRegion();
                    break;
            }

            DoCapture(() =>
            {
                Image img = null;
                Image screenshot = Screenshot.CaptureFullscreen();

                try
                {
                    surface.Config = taskSettings.CaptureSettings.SurfaceOptions;
                    surface.SurfaceImage = screenshot;
                    surface.Prepare();
                    surface.ShowDialog();

                    if (surface.Result == SurfaceResult.Region)
                    {
                        img = surface.GetRegionImage();
                        screenshot.Dispose();
                    }
                    else if (surface.Result == SurfaceResult.Fullscreen)
                    {
                        img = screenshot;
                    }
                }
                finally
                {
                    surface.Dispose();
                }

                return img;
            }, captureType, taskSettings, autoHideForm);
        }

        private void CaptureLightRectangle(TaskSettings taskSettings, bool autoHideForm = true)
        {
            DoCapture(() =>
            {
                Image img = null;

                using (RectangleLight rectangleLight = new RectangleLight())
                {
                    if (rectangleLight.ShowDialog() == DialogResult.OK)
                    {
                        img = rectangleLight.GetAreaImage();
                    }
                }

                return img;
            }, CaptureType.Rectangle, taskSettings, autoHideForm);
        }

        private void CaptureLastRegion(TaskSettings taskSettings, bool autoHideForm = true)
        {
            if (!taskSettings.AdvancedSettings.UseLightRectangleCrop && Surface.LastRegionFillPath != null)
            {
                DoCapture(() =>
                {
                    using (Image screenshot = Screenshot.CaptureFullscreen())
                    {
                        return ShapeCaptureHelpers.GetRegionImage(screenshot, Surface.LastRegionFillPath, Surface.LastRegionDrawPath, taskSettings.CaptureSettings.SurfaceOptions);
                    }
                }, CaptureType.LastRegion, taskSettings, autoHideForm);
            }
            else if (taskSettings.AdvancedSettings.UseLightRectangleCrop && !RectangleLight.LastSelectionRectangle0Based.IsEmpty)
            {
                DoCapture(() =>
                {
                    using (Image screenshot = Screenshot.CaptureFullscreen())
                    {
                        return ImageHelpers.CropImage(screenshot, RectangleLight.LastSelectionRectangle0Based);
                    }
                }, CaptureType.LastRegion, taskSettings, autoHideForm);
            }
            else
            {
                CaptureRegion(CaptureType.Rectangle, taskSettings, autoHideForm);
            }
        }

        private async void PrepareCaptureMenuAsync(ToolStripMenuItem tsmiWindow, EventHandler handlerWindow, ToolStripMenuItem tsmiMonitor, EventHandler handlerMonitor)
        {
            tsmiWindow.DropDownItems.Clear();

            WindowsList windowsList = new WindowsList();
            List<WindowInfo> windows = await TaskEx.Run(() => windowsList.GetVisibleWindowsList());

            if (windows != null)
            {
                foreach (WindowInfo window in windows)
                {
                    try
                    {
                        string title = window.Text.Truncate(50);
                        ToolStripItem tsi = tsmiWindow.DropDownItems.Add(title);
                        tsi.Tag = window;
                        tsi.Click += handlerWindow;

                        using (Icon icon = window.Icon)
                        {
                            if (icon != null && icon.Width > 0 && icon.Height > 0)
                            {
                                tsi.Image = icon.ToBitmap();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        DebugHelper.WriteException(e);
                    }
                }
            }

            tsmiMonitor.DropDownItems.Clear();

            Screen[] screens = Screen.AllScreens;

            for (int i = 0; i < screens.Length; i++)
            {
                Screen screen = screens[i];
                string text = string.Format("{0}. {1}x{2}", i + 1, screen.Bounds.Width, screen.Bounds.Height);
                ToolStripItem tsi = tsmiMonitor.DropDownItems.Add(text);
                tsi.Tag = screen.Bounds;
                tsi.Click += handlerMonitor;
            }

            tsmiWindow.Invalidate();
            tsmiMonitor.Invalidate();
        }

        #region Menu events

        private void tsmiFullscreen_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Screen);
        }

        private void tsddbCapture_DropDownOpening(object sender, EventArgs e)
        {
            PrepareCaptureMenuAsync(tsmiWindow, tsmiWindowItems_Click, tsmiMonitor, tsmiMonitorItems_Click);
        }

        private void tsmiWindowItems_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = (ToolStripItem)sender;
            WindowInfo wi = tsi.Tag as WindowInfo;
            if (wi != null)
            {
                CaptureWindow(wi.Handle);
            }
        }

        private void tsmiMonitorItems_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = (ToolStripItem)sender;
            Rectangle rectangle = (Rectangle)tsi.Tag;
            if (!rectangle.IsEmpty)
            {
                DoCapture(() => Screenshot.CaptureRectangle(rectangle), CaptureType.Monitor);
            }
        }

        private void tsmiWindowRectangle_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.RectangleWindow);
        }

        private void tsmiRectangle_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Rectangle);
        }

        private void tsmiRoundedRectangle_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.RoundedRectangle);
        }

        private void tsmiEllipse_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Ellipse);
        }

        private void tsmiTriangle_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Triangle);
        }

        private void tsmiDiamond_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Diamond);
        }

        private void tsmiPolygon_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Polygon);
        }

        private void tsmiFreeHand_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Freehand);
        }

        private void tsmiLastRegion_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.LastRegion);
        }

        #endregion Menu events

        #region Tray events

        private void tsmiTrayFullscreen_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Screen, null, false);
        }

        private void tsmiCapture_DropDownOpening(object sender, EventArgs e)
        {
            PrepareCaptureMenuAsync(tsmiTrayWindow, tsmiTrayWindowItems_Click, tsmiTrayMonitor, tsmiTrayMonitorItems_Click);
        }

        private void tsmiTrayWindowItems_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = (ToolStripItem)sender;
            WindowInfo wi = tsi.Tag as WindowInfo;
            if (wi != null)
            {
                CaptureWindow(wi.Handle, null, false);
            }
        }

        private void tsmiTrayMonitorItems_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = (ToolStripItem)sender;
            Rectangle rectangle = (Rectangle)tsi.Tag;
            if (!rectangle.IsEmpty)
            {
                DoCapture(() => Screenshot.CaptureRectangle(rectangle), CaptureType.Monitor, null, false);
            }
        }

        private void tsmiTrayWindowRectangle_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.RectangleWindow, null, false);
        }

        private void tsmiTrayRectangle_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Rectangle, null, false);
        }

        private void tsmiTrayRoundedRectangle_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.RoundedRectangle, null, false);
        }

        private void tsmiTrayEllipse_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Ellipse, null, false);
        }

        private void tsmiTrayTriangle_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Triangle, null, false);
        }

        private void tsmiTrayDiamond_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Diamond, null, false);
        }

        private void tsmiTrayPolygon_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Polygon, null, false);
        }

        private void tsmiTrayFreeHand_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Freehand, null, false);
        }

        private void tsmiTrayLastRegion_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.LastRegion, null, false);
        }

        #endregion Tray events
    }
}