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
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX
{
    public partial class ScreenRegionForm : Form
    {
        public event Action StopRequested;

        private Color color = Color.Red;
        private Rectangle borderRectangle;
        private Rectangle borderRectangle0Based;
        private Stopwatch Timer;

        public ScreenRegionForm(Rectangle regionRectangle)
        {
            InitializeComponent();

            borderRectangle = regionRectangle.RectangleOffset(1);
            borderRectangle0Based = new Rectangle(0, 0, borderRectangle.Width, borderRectangle.Height);

            Location = borderRectangle.Location;
            Size = new Size(borderRectangle.Width, borderRectangle.Height + pInfo.Height);
            pInfo.Location = new Point(Width - pInfo.Width, Height - pInfo.Height);

            Region region = new Region(ClientRectangle);
            region.Exclude(borderRectangle0Based.RectangleOffset(-1));
            region.Exclude(new Rectangle(0, pInfo.Location.Y, pInfo.Location.X, pInfo.Height));
            Region = region;
        }

        protected void OnStopRequested()
        {
            if (StopRequested != null)
            {
                StopRequested();
            }
        }

        public static ScreenRegionForm Start(Rectangle captureRectangle, Action stopRequested)
        {
            if (captureRectangle != CaptureHelpers.GetScreenBounds())
            {
                ScreenRegionForm regionForm = new ScreenRegionForm(captureRectangle);
                regionForm.StopRequested += stopRequested;
                regionForm.Show();
                return regionForm;
            }

            return null;
        }

        public void StartTimer()
        {
            this.color = Color.FromArgb(0, 255, 0);
            Refresh();

            Timer = Stopwatch.StartNew();
            timerRefresh.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (Pen pen1 = new Pen(Color.Black) { DashPattern = new float[] { 5, 5 } })
            using (Pen pen2 = new Pen(color) { DashPattern = new float[] { 5, 5 }, DashOffset = 5 })
            {
                e.Graphics.DrawRectangleProper(pen1, borderRectangle0Based);
                e.Graphics.DrawRectangleProper(pen2, borderRectangle0Based);
            }

            base.OnPaint(e);
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            if (!IsDisposed)
            {
                lblTimer.Text = Timer.Elapsed.ToString("mm\\:ss\\:ff");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            OnStopRequested();
        }
    }
}