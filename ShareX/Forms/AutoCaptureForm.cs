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
using ShareX.Properties;
using ShareX.ScreenCaptureLib;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX
{
    public partial class AutoCaptureForm : Form
    {
        private static AutoCaptureForm instance;

        public static AutoCaptureForm Instance
        {
            get
            {
                if (instance == null || instance.IsDisposed)
                {
                    instance = new AutoCaptureForm();
                }

                return instance;
            }
        }

        public static bool IsRunning { get; private set; }
        public TaskSettings TaskSettings { get; internal set; }

        private bool isLoaded;
        private Timer statusTimer;
        private System.Timers.Timer screenshotTimer;
        private int delay, count, timeleft, percentage;
        private bool waitUploads;
        private Stopwatch stopwatch = new Stopwatch();
        private Rectangle customRegion;

        private AutoCaptureForm()
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);
            niTray.Icon = Resources.clock.ToIcon();

            screenshotTimer = new System.Timers.Timer();
            screenshotTimer.SynchronizingObject = this;
            screenshotTimer.Elapsed += screenshotTimer_Elapsed;

            statusTimer = new Timer { Interval = 250 };
            statusTimer.Tick += (sender, e) => UpdateStatus();

            customRegion = Program.Settings.AutoCaptureRegion;
            UpdateRegion();
            nudRepeatTime.SetValue(Program.Settings.AutoCaptureRepeatTime);
            cbAutoMinimize.Checked = Program.Settings.AutoCaptureMinimizeToTray;
            cbWaitUploads.Checked = Program.Settings.AutoCaptureWaitUpload;

            isLoaded = true;
        }

        private void screenshotTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (IsRunning)
            {
                if (waitUploads && TaskManager.IsBusy)
                {
                    screenshotTimer.Interval = 1000;
                }
                else
                {
                    stopwatch.Reset();
                    stopwatch.Start();
                    screenshotTimer.Interval = delay;
                    count++;
                    TakeScreenshot();
                }
            }
        }

        private void TakeScreenshot()
        {
            Rectangle rect = Program.Settings.AutoCaptureRegion;

            if (!rect.IsEmpty)
            {
                Bitmap bmp = TaskHelpers.GetScreenshot(TaskSettings).CaptureRectangle(rect);

                if (bmp != null)
                {
                    TaskSettings.AfterCaptureJob = TaskSettings.AfterCaptureJob.Remove(AfterCaptureTasks.AnnotateImage);
                    TaskSettings.GeneralSettings.PlaySoundAfterUpload = false;
                    TaskSettings.GeneralSettings.PlaySoundAfterAction = false;
                    TaskSettings.GeneralSettings.ShowToastNotificationAfterTaskCompleted = false;

                    UploadManager.RunImageTask(bmp, TaskSettings, true, true);
                }
            }
        }

        private void SelectRegion()
        {
            if (RegionCaptureTasks.GetRectangleRegion(out Rectangle rect, TaskSettings.CaptureSettings.SurfaceOptions))
            {
                Program.Settings.AutoCaptureRegion = rect;
                UpdateRegion();
            }
        }

        private void UpdateRegion()
        {
            Rectangle rect = Program.Settings.AutoCaptureRegion;

            if (!rect.IsEmpty)
            {
                lblRegion.Text = string.Format(Resources.AutoCaptureForm_UpdateRegion_X___0___Y___1___Width___2___Height___3_, rect.X, rect.Y, rect.Width, rect.Height);
                btnExecute.Enabled = true;
            }
        }

        private void UpdateStatus()
        {
            if (IsRunning && !IsDisposed)
            {
                timeleft = Math.Max(0, delay - (int)stopwatch.ElapsedMilliseconds);
                percentage = (int)(100 - ((double)timeleft / delay * 100));
                tspbBar.Value = percentage;
                string secondsLeft = (timeleft / 1000f).ToString("0.0");
                tsslStatus.Text = " " + string.Format(Resources.AutoCaptureForm_UpdateStatus_Timeleft___0_s___1____Total___2_, secondsLeft, percentage, count);
            }
        }

        public void Execute()
        {
            if (IsRunning)
            {
                IsRunning = false;
                tspbBar.Value = 0;
                stopwatch.Reset();
                btnExecute.Text = Resources.AutoCaptureForm_Execute_Start;
            }
            else
            {
                IsRunning = true;
                btnExecute.Text = Resources.AutoCaptureForm_Execute_Stop;
                screenshotTimer.Interval = 1000;
                delay = (int)(Program.Settings.AutoCaptureRepeatTime * 1000);
                waitUploads = Program.Settings.AutoCaptureWaitUpload;

                if (Program.Settings.AutoCaptureMinimizeToTray)
                {
                    Visible = false;
                    niTray.Visible = true;
                }
            }

            screenshotTimer.Enabled = IsRunning;
            statusTimer.Enabled = IsRunning;
        }

        private void rbFullscreen_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoaded && rbFullscreen.Checked)
            {
                customRegion = Program.Settings.AutoCaptureRegion;
                Program.Settings.AutoCaptureRegion = CaptureHelpers.GetScreenBounds();
                UpdateRegion();
                btnRegion.Enabled = false;
            }
        }

        private void rbCustomRegion_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoaded && rbCustomRegion.Checked)
            {
                Program.Settings.AutoCaptureRegion = customRegion;
                UpdateRegion();
                btnRegion.Enabled = true;
            }
        }

        private void btnRegion_Click(object sender, EventArgs e)
        {
            SelectRegion();
        }

        private void nudDuration_ValueChanged(object sender, EventArgs e)
        {
            Program.Settings.AutoCaptureRepeatTime = nudRepeatTime.Value;
        }

        private void cbAutoMinimize_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.AutoCaptureMinimizeToTray = cbAutoMinimize.Checked;
        }

        private void cbWaitUploads_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.AutoCaptureWaitUpload = cbWaitUploads.Checked;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            Execute();
        }

        private void AutoCapture_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsRunning = false;
            screenshotTimer.Enabled = false;
            statusTimer.Enabled = false;
        }

        private void AutoCapture_Resize(object sender, EventArgs e)
        {
            if (Program.Settings.AutoCaptureMinimizeToTray && WindowState == FormWindowState.Minimized)
            {
                Visible = false;
                niTray.Visible = true;
            }
        }

        private void niTray_MouseClick(object sender, MouseEventArgs e)
        {
            niTray.Visible = false;
            this.ForceActivate();
        }
    }
}