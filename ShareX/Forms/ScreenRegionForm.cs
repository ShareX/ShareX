#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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
    public partial class ScreenRegionForm : Form
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

        public ScreenRegionForm(Rectangle regionRectangle)
        {
            InitializeComponent();

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

        private void ScreenRegionForm_Shown(object sender, EventArgs e)
        {
            this.ShowActivate();
        }

        protected void OnStopRequested()
        {
            if (StopRequested != null)
            {
                StopRequested();
            }
        }

        public static ScreenRegionForm Show(Rectangle captureRectangle, Action stopRequested, float duration = 0)
        {
            ScreenRegionForm regionForm = new ScreenRegionForm(captureRectangle);
            regionForm.StopRequested += stopRequested;

            if (duration > 0)
            {
                regionForm.IsCountdown = true;
                regionForm.Countdown = TimeSpan.FromSeconds(duration);
            }

            regionForm.UpdateTimer();
            regionForm.Show();
            return regionForm;
        }

        public void StartTimer()
        {
            borderColor = Color.FromArgb(0, 255, 0);
            btnStop.Text = Resources.AutoCaptureForm_Execute_Stop;
            Refresh();

            Timer.Start();
            timerRefresh.Start();
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

        public void StartStop()
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

        private void btnStop_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                StartStop();
            }
        }

        private void btnAbort_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                AbortRequested = true;
                StartStop();
            }
        }
    }
}