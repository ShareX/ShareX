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

using ShareX.HelpersLib;
using ShareX.ScreenCaptureLib.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class RectangleLightForm : Form
    {
        public static Rectangle LastSelectionRectangle0Based { get; private set; }

        public Rectangle ScreenRectangle { get; private set; }

        public Rectangle ScreenRectangle0Based
        {
            get
            {
                return new Rectangle(0, 0, ScreenRectangle.Width, ScreenRectangle.Height);
            }
        }

        public Rectangle SelectionRectangle { get; private set; }

        public Rectangle SelectionRectangle0Based
        {
            get
            {
                return new Rectangle(SelectionRectangle.X - ScreenRectangle.X, SelectionRectangle.Y - ScreenRectangle.Y, SelectionRectangle.Width, SelectionRectangle.Height);
            }
        }

        private Timer timer;
        private Image backgroundImage;
        private TextureBrush backgroundBrush;
        private Pen borderDotPen, borderDotPen2;
        private Point currentPosition, positionOnClick;
        private bool isMouseDown;
        private Stopwatch penTimer;

        public RectangleLightForm()
        {
            backgroundImage = Screenshot.CaptureFullscreen();
            backgroundBrush = new TextureBrush(backgroundImage);
            borderDotPen = new Pen(Color.Black, 1);
            borderDotPen2 = new Pen(Color.White, 1);
            borderDotPen2.DashPattern = new float[] { 5, 5 };
            penTimer = Stopwatch.StartNew();
            ScreenRectangle = CaptureHelpers.GetScreenBounds();

            InitializeComponent();
            Icon = ShareXResources.Icon;

            using (MemoryStream cursorStream = new MemoryStream(Resources.Crosshair))
            {
                Cursor = new Cursor(cursorStream);
            }

            timer = new Timer { Interval = 10 };
            timer.Tick += timer_Tick;
            timer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (timer != null) timer.Dispose();
            if (backgroundImage != null) backgroundImage.Dispose();
            if (backgroundBrush != null) backgroundBrush.Dispose();
            if (borderDotPen != null) borderDotPen.Dispose();
            if (borderDotPen2 != null) borderDotPen2.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            StartPosition = FormStartPosition.Manual;
            Bounds = ScreenRectangle;
            FormBorderStyle = FormBorderStyle.None;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            Text = "ShareX - " + Resources.RectangleLight_InitializeComponent_Rectangle_capture_light;
            ShowInTaskbar = false;
            TopMost = true;

            Shown += RectangleLight_Shown;
            KeyUp += RectangleLight_KeyUp;
            MouseDown += RectangleLight_MouseDown;
            MouseUp += RectangleLight_MouseUp;

            ResumeLayout(false);
        }

        private void RectangleLight_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void RectangleLight_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void RectangleLight_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                positionOnClick = CaptureHelpers.GetCursorPosition();
                isMouseDown = true;
            }
        }

        private void RectangleLight_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isMouseDown)
                {
                    if (SelectionRectangle0Based.Width > 0 && SelectionRectangle0Based.Height > 0)
                    {
                        LastSelectionRectangle0Based = SelectionRectangle0Based;
                        DialogResult = DialogResult.OK;
                    }

                    Close();
                }
            }
            else
            {
                if (isMouseDown)
                {
                    isMouseDown = false;
                    Refresh();
                }
                else
                {
                    Close();
                }
            }
        }

        public Image GetAreaImage()
        {
            Rectangle rect = SelectionRectangle0Based;

            if (rect.Width > 0 && rect.Height > 0)
            {
                if (rect.X == 0 && rect.Y == 0 && rect.Width == backgroundImage.Width && rect.Height == backgroundImage.Height)
                {
                    return (Image)backgroundImage.Clone();
                }

                return ImageHelpers.CropImage(backgroundImage, rect);
            }

            return null;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            currentPosition = CaptureHelpers.GetCursorPosition();
            SelectionRectangle = CaptureHelpers.CreateRectangle(positionOnClick.X, positionOnClick.Y, currentPosition.X, currentPosition.Y);

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
            g.CompositingMode = CompositingMode.SourceCopy;
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.FillRectangle(backgroundBrush, ScreenRectangle0Based);

            if (isMouseDown)
            {
                borderDotPen2.DashOffset = (float)penTimer.Elapsed.TotalSeconds * -15;
                g.DrawRectangleProper(borderDotPen, SelectionRectangle0Based);
                g.DrawRectangleProper(borderDotPen2, SelectionRectangle0Based);
            }
        }
    }
}