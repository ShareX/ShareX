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

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class ScreenTearingTestForm : Form
    {
        public ScreenTearingTestMode Mode { get; set; } = ScreenTearingTestMode.VerticalLines;

        private Rectangle screenRectangle;
        private Stopwatch animationTime;
        private TimeSpan lastElapsed;
        private int rectangleSize = 50;
        private float animationSpeed = 500, minSpeed = 100, maxSpeed = 2000, speedChange = 50, currentPosition;

        public ScreenTearingTestForm()
        {
            screenRectangle = CaptureHelpers.GetScreenBounds();

            SuspendLayout();

            AutoScaleMode = AutoScaleMode.None;
            StartPosition = FormStartPosition.Manual;
            Bounds = screenRectangle;
            Cursor = Cursors.Hand;
            FormBorderStyle = FormBorderStyle.None;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            Text = "ShareX - Screen tearing test";
            ShowInTaskbar = false;
            TopMost = true;

            ResumeLayout(false);

            ShareXResources.ApplyTheme(this, true);

            animationTime = Stopwatch.StartNew();
        }

        protected override void OnShown(EventArgs e)
        {
            this.ForceActivate();

            base.OnShown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (Mode == ScreenTearingTestMode.VerticalLines)
            {
                Mode = ScreenTearingTestMode.HorizontalLines;
            }
            else
            {
                Close();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                animationSpeed = (animationSpeed + speedChange).Clamp(minSpeed, maxSpeed);
            }
            else if (e.Delta < 0)
            {
                animationSpeed = (animationSpeed - speedChange).Clamp(minSpeed, maxSpeed);
            }

            base.OnMouseWheel(e);
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

            if (Mode == ScreenTearingTestMode.VerticalLines)
            {
                for (int x = startOffset - rectangleSize; x < screenRectangle.Width; x += nextPosition)
                {
                    g.FillRectangle(Brushes.Black, x, 0, rectangleSize, screenRectangle.Height);
                }
            }
            else if (Mode == ScreenTearingTestMode.HorizontalLines)
            {
                for (int y = startOffset - rectangleSize; y < screenRectangle.Height; y += nextPosition)
                {
                    g.FillRectangle(Brushes.Black, 0, y, screenRectangle.Width, rectangleSize);
                }
            }

            TimeSpan elapsed = animationTime.Elapsed - lastElapsed;

            currentPosition += (float)(elapsed.TotalSeconds * animationSpeed);

            lastElapsed = animationTime.Elapsed;

            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}