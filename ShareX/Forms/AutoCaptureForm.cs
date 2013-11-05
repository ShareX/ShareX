#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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
using ScreenCapture;
using ShareX.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX
{
    public partial class AutoCaptureForm : Form
    {
        public bool IsRunning { get; private set; }
        public Rectangle CaptureRectangle { get; private set; }

        private Timer timer, statusTimer;
        private int delay, count, timeleft, percentage;
        private bool waitUploads;
        private Stopwatch stopwatch = new Stopwatch();

        public AutoCaptureForm()
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            niTray.Icon = Resources.clock_plus.ToIcon();

            timer = new Timer();
            timer.Tick += TimerTick;
            statusTimer = new Timer { Interval = 250 };
            statusTimer.Tick += (sender, e) => UpdateStatus();

            nudRepeatTime.Value = Program.Settings.AutoCaptureRepeatTime;
            cbAutoMinimize.Checked = Program.Settings.AutoCaptureMinimizeToTray;
            cbWaitUploads.Checked = Program.Settings.AutoCaptureWaitUpload;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (IsRunning)
            {
                if (waitUploads && TaskManager.IsBusy)
                {
                    timer.Interval = 1000;
                }
                else
                {
                    stopwatch.Reset();
                    stopwatch.Start();
                    timer.Interval = delay;
                    count++;
                    TakeScreenshot();
                }
            }
        }

        private void TakeScreenshot()
        {
            if (!CaptureRectangle.IsEmpty)
            {
                Image img = Screenshot.CaptureRectangle(CaptureRectangle);

                if (img != null)
                {
                    TaskSettings taskSettings = TaskSettings.GetDefaultTaskSettings();
                    taskSettings.UseDefaultAfterCaptureJob = false;
                    taskSettings.AfterCaptureJob = taskSettings.AfterCaptureJob.Remove(AfterCaptureTasks.AnnotateImage);
                    taskSettings.UseDefaultAdvancedSettings = false;
                    taskSettings.AdvancedSettings.DisableNotifications = true;

                    UploadManager.RunImageTask(img, taskSettings);
                }
            }
        }

        private void SelectRegion()
        {
            Rectangle rect;
            if (TaskHelpers.SelectRegion(out rect))
            {
                UpdateRegion(rect);
            }
        }

        private void UpdateRegion(Rectangle rect)
        {
            if (!rect.IsEmpty)
            {
                CaptureRectangle = rect;
                lblRegion.Text = string.Format("X: {0}, Y: {1}, Width: {2}, Height: {3}", CaptureRectangle.X, CaptureRectangle.Y,
                    CaptureRectangle.Width, CaptureRectangle.Height);
                btnExecute.Enabled = true;
            }
        }

        private void UpdateStatus()
        {
            if (IsRunning && !IsDisposed)
            {
                timeleft = Math.Max(0, delay - (int)stopwatch.ElapsedMilliseconds);
                percentage = (int)(100 - (double)timeleft / delay * 100);
                tspbBar.Value = percentage;
                string secondsLeft = (timeleft / 1000f).ToString("0.0");
                tsslStatus.Text = " Timeleft: " + secondsLeft + "s (" + percentage + "%) Total: " + count;
            }
        }

        public void Execute()
        {
            if (IsRunning)
            {
                IsRunning = false;
                tspbBar.Value = 0;
                stopwatch.Reset();
                btnExecute.Text = "Start";
            }
            else
            {
                IsRunning = true;
                btnExecute.Text = "Stop";
                timer.Interval = 1000;
                delay = (int)(Program.Settings.AutoCaptureRepeatTime * 1000);
                waitUploads = Program.Settings.AutoCaptureWaitUpload;

                if (Program.Settings.AutoCaptureMinimizeToTray)
                {
                    Visible = false;
                    niTray.Visible = true;
                }
            }

            timer.Enabled = IsRunning;
            statusTimer.Enabled = IsRunning;
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
            timer.Enabled = false;
            statusTimer.Enabled = false;
        }

        private void btnFullscreen_Click(object sender, EventArgs e)
        {
            UpdateRegion(CaptureHelpers.GetScreenBounds());
        }

        private void AutoCapture_Resize(object sender, EventArgs e)
        {
            if (Program.Settings.AutoCaptureMinimizeToTray && WindowState == FormWindowState.Minimized)
            {
                Visible = false;
                niTray.Visible = true;
            }
        }

        private void niTray_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                niTray.Visible = false;
                this.ShowActivate();
            }
        }
    }
}