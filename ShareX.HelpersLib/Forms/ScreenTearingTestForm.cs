#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class ScreenTearingTestForm : Form
    {
        private Rectangle screenRectangle, screenRectangle0Based;
        private Timer drawTimer;
        private Stopwatch animationTime;
        private TimeSpan lastElapsed;
        private int rectangleSize = 50;
        private float animationSpeed = 500, minSpeed = 100, maxSpeed = 2000, speedChange = 50;
        private float currentPosition;

        public ScreenTearingTestForm()
        {
            screenRectangle = CaptureHelpers.GetScreenBounds();
            screenRectangle0Based = new Rectangle(0, 0, screenRectangle.Width, screenRectangle.Height);

            SuspendLayout();

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            StartPosition = FormStartPosition.Manual;
            Bounds = screenRectangle;
            Cursor = Cursors.Hand;
            FormBorderStyle = FormBorderStyle.None;
            Icon = ShareXResources.Icon;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            Text = "ShareX - Screen tearing test";
            ShowInTaskbar = false;
            TopMost = true;

            ResumeLayout(false);

            animationTime = Stopwatch.StartNew();

            drawTimer = new Timer { Interval = 5 };
            drawTimer.Tick += timer_Tick;
            drawTimer.Start();
        }

        protected override void OnShown(EventArgs e)
        {
            this.ForceActivate();

            base.OnShown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }

            base.OnKeyUp(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                animationSpeed = (animationSpeed + speedChange).Between(minSpeed, maxSpeed);
            }
            else if (e.Delta < 0)
            {
                animationSpeed = (animationSpeed - speedChange).Between(minSpeed, maxSpeed);
            }

            base.OnMouseWheel(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            Close();

            base.OnMouseUp(e);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Refresh();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighSpeed;

            g.Clear(Color.White);

            int nextPosition = rectangleSize * 2;
            int startOffset = (int)(currentPosition % nextPosition);

            for (int x = startOffset - rectangleSize; x < screenRectangle.Width; x += nextPosition)
            {
                g.FillRectangle(Brushes.Black, x, 0, rectangleSize, screenRectangle.Height);
            }

            TimeSpan elapsed = animationTime.Elapsed - lastElapsed;

            currentPosition += (float)(elapsed.TotalSeconds * animationSpeed);

            lastElapsed = animationTime.Elapsed;
        }

        protected override void Dispose(bool disposing)
        {
            if (drawTimer != null) drawTimer.Dispose();

            base.Dispose(disposing);
        }
    }
}