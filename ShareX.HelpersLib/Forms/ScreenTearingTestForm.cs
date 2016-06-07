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
        private TextureBrush brush;
        private Timer timer;
        private Stopwatch stopwatch;
        private TimeSpan lastElapsed;
        private float animationSpeed = 200;

        public ScreenTearingTestForm()
        {
            screenRectangle = CaptureHelpers.GetScreenBounds();
            screenRectangle0Based = new Rectangle(0, 0, screenRectangle.Width, screenRectangle.Height);

            SuspendLayout();

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            StartPosition = FormStartPosition.Manual;
            Bounds = screenRectangle;
            FormBorderStyle = FormBorderStyle.None;
            Icon = ShareXResources.Icon;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            Text = "ShareX - Screen tearing test";
            ShowInTaskbar = false;
            TopMost = true;

            ResumeLayout(false);

            brush = CreateVerticalLineBrush(50, screenRectangle.Height, Color.Black, Color.White);

            stopwatch = Stopwatch.StartNew();

            timer = new Timer { Interval = 10 };
            timer.Tick += timer_Tick;
            timer.Start();
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
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.SmoothingMode = SmoothingMode.HighSpeed;

            g.FillRectangle(brush, screenRectangle0Based);

            TimeSpan elapsed = stopwatch.Elapsed - lastElapsed;

            float x = (float)(elapsed.TotalSeconds * animationSpeed);
            brush.TranslateTransform(x, 0);

            lastElapsed = stopwatch.Elapsed;
        }

        private TextureBrush CreateVerticalLineBrush(int size, int height, Color color1, Color color2)
        {
            using (Bitmap bmp = new Bitmap(size * 2, height))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.HighSpeed;

                using (Brush brush1 = new SolidBrush(color1))
                {
                    g.FillRectangle(brush1, 0, 0, size, height);
                }

                using (Brush brush2 = new SolidBrush(color2))
                {
                    g.FillRectangle(brush2, size, 0, size, height);
                }

                return new TextureBrush(bmp, WrapMode.Tile);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (timer != null) timer.Dispose();
            if (brush != null) brush.Dispose();

            base.Dispose(disposing);
        }
    }
}