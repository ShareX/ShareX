#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ShareX
{
    public enum ScreenRecordState
    {
        Waiting, BeforeStart, AfterStart, AfterStop
    }

    public partial class ScreenRecordForm : BaseForm
    {
        public event Action StopRequested;

        public bool IsRecording { get; private set; }
        public bool IsCountdown { get; set; }
        public TimeSpan Countdown { get; set; }
        public Stopwatch Timer { get; private set; }
        public ManualResetEvent RecordResetEvent { get; set; }
        public bool AbortRequested { get; private set; }

        private Color borderColor = Color.Red;
        private Rectangle borderRectangle;
        private Rectangle borderRectangle0Based;
        private bool activateWindow;
        private float duration;

        public ScreenRecordForm(Rectangle regionRectangle, bool activateWindow = true, float duration = 0)
        {
            InitializeComponent();
            niTray.Icon = ShareXResources.Icon;

            this.activateWindow = activateWindow;
            this.duration = duration;

            borderRectangle = regionRectangle.Offset(1);
            borderRectangle0Based = new Rectangle(0, 0, borderRectangle.Width, borderRectangle.Height);

            Location = borderRectangle.Location;
            int windowWidth = Math.Max(borderRectangle.Width, pInfo.Width);
            Size = new Size(windowWidth, borderRectangle.Height + pInfo.Height + 1);
            pInfo.Location = new Point(0, borderRectangle.Height + 1);

            Region region = new Region(ClientRectangle);
            region.Exclude(borderRectangle0Based.Offset(-1));
            region.Exclude(new Rectangle(0, borderRectangle.Height, windowWidth, 1));
            if (borderRectangle.Width < pInfo.Width)
            {
                region.Exclude(new Rectangle(borderRectangle.Width, 0, pInfo.Width - borderRectangle.Width, borderRectangle.Height));
            }
            else if (borderRectangle.Width > pInfo.Width)
            {
                region.Exclude(new Rectangle(pInfo.Width, borderRectangle.Height + 1, borderRectangle.Width - pInfo.Width, pInfo.Height));
            }
            Region = region;

            Timer = new Stopwatch();
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return !activateWindow;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= (int)WindowStyles.WS_EX_TOPMOST;
                return createParams;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                if (RecordResetEvent != null)
                {
                    RecordResetEvent.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private void ScreenRegionForm_Shown(object sender, EventArgs e)
        {
            if (activateWindow)
            {
                this.ShowActivate();
            }
        }

        protected void OnStopRequested()
        {
            if (StopRequested != null)
            {
                StopRequested();
            }
        }

        public static ScreenRecordForm Show(Rectangle captureRectangle, Action stopRequested, bool activateWindow, float duration = 0)
        {
            ScreenRecordForm regionForm = new ScreenRecordForm(captureRectangle, activateWindow, duration);

            Thread thread = new Thread(() =>
            {
                regionForm.StopRequested += stopRequested;
                regionForm.UpdateTimer();
                regionForm.ShowDialog();
            });

            thread.Start();

            return regionForm;
        }

        public void StartCountdown(int milliseconds)
        {
            IsCountdown = true;
            Countdown = TimeSpan.FromMilliseconds(milliseconds);

            lblTimer.ForeColor = Color.Yellow;

            Timer.Start();
            timerRefresh.Start();
            UpdateTimer();
        }

        public void StartRecordingTimer()
        {
            IsCountdown = duration > 0;
            Countdown = TimeSpan.FromSeconds(duration);

            lblTimer.ForeColor = Color.White;
            borderColor = Color.FromArgb(0, 255, 0);
            btnStart.Text = Resources.AutoCaptureForm_Execute_Stop;
            Refresh();

            Timer.Reset();
            Timer.Start();
            timerRefresh.Start();
            UpdateTimer();
            IsRecording = true;
        }

        private void UpdateTimer()
        {
            if (!IsDisposed)
            {
                TimeSpan timer;

                if (IsCountdown)
                {
                    timer = Countdown - Timer.Elapsed;
                    if (timer.Ticks < 0) timer = TimeSpan.Zero;
                }
                else
                {
                    timer = Timer.Elapsed;
                }

                lblTimer.Text = timer.ToString("mm\\:ss\\:ff");
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (Pen pen1 = new Pen(Color.Black) { DashPattern = new float[] { 5, 5 } })
            using (Pen pen2 = new Pen(borderColor) { DashPattern = new float[] { 5, 5 }, DashOffset = 5 })
            {
                e.Graphics.DrawRectangleProper(pen1, borderRectangle0Based);
                e.Graphics.DrawRectangleProper(pen2, borderRectangle0Based);
            }

            base.OnPaint(e);
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            UpdateTimer();
        }

        private void btnStart_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                StartStopRecording();
            }
        }

        private void btnAbort_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                AbortRecording();
            }
        }

        public void StartStopRecording()
        {
            if (IsRecording)
            {
                OnStopRequested();
            }
            else if (RecordResetEvent != null)
            {
                RecordResetEvent.Set();
            }
        }

        public void AbortRecording()
        {
            AbortRequested = true;
            StartStopRecording();
        }

        public void ChangeState(ScreenRecordState state)
        {
            this.InvokeSafe(() =>
            {
                switch (state)
                {
                    case ScreenRecordState.Waiting:
                        string trayTextWaiting = "ShareX - " + Resources.ScreenRecordForm_StartRecording_Waiting___;
                        niTray.Text = trayTextWaiting.Truncate(63);
                        niTray.Icon = Resources.control_record_yellow.ToIcon();
                        cmsMain.Enabled = false;
                        niTray.Visible = true;
                        break;
                    case ScreenRecordState.BeforeStart:
                        string trayTextBeforeStart = "ShareX - " + Resources.ScreenRecordForm_StartRecording_Click_tray_icon_to_start_recording_;
                        niTray.Text = trayTextBeforeStart.Truncate(63);
                        tsmiStart.Text = Resources.AutoCaptureForm_Execute_Start;
                        cmsMain.Enabled = true;
                        break;
                    case ScreenRecordState.AfterStart:
                        string trayTextAfterStart = "ShareX - " + Resources.ScreenRecordForm_StartRecording_Click_tray_icon_to_stop_recording_;
                        niTray.Text = trayTextAfterStart.Truncate(63);
                        niTray.Icon = Resources.control_record.ToIcon();
                        tsmiStart.Text = Resources.AutoCaptureForm_Execute_Stop;
                        StartRecordingTimer();
                        break;
                    case ScreenRecordState.AfterStop:
                        Hide();
                        string trayTextAfterStop = "ShareX - " + Resources.ScreenRecordForm_StartRecording_Encoding___;
                        niTray.Text = trayTextAfterStop.Truncate(63);
                        niTray.Icon = Resources.camcorder_pencil.ToIcon();
                        cmsMain.Enabled = false;
                        break;
                }
            });
        }

        public void ChangeStateProgress(int progress)
        {
            niTray.Text = string.Format("ShareX - {0} ({1}%)", Resources.ScreenRecordForm_StartRecording_Encoding___, progress);
        }
    }
}