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
using ScreenCaptureLib.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace ScreenCaptureLib
{
    public class RectangleAnnotate : Form
    {
        public static Rectangle LastSelectionRectangle0Based { get; set; }

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

        public Point CurrentMousePosition { get; private set; }

        public Point CurrentMousePosition0Based
        {
            get
            {
                return new Point(CurrentMousePosition.X - ScreenRectangle.X, CurrentMousePosition.Y - ScreenRectangle.Y);
            }
        }

        public Point PreviousMousePosition { get; private set; }

        public Point PreviousMousePosition0Based
        {
            get
            {
                return new Point(PreviousMousePosition.X - ScreenRectangle.X, PreviousMousePosition.Y - ScreenRectangle.Y);
            }
        }

        public bool ShowRectangleInfo { get; set; }

        private bool cursorShown = true;

        public bool CursorShown
        {
            get
            {
                return cursorShown;
            }
            set
            {
                if (cursorShown == value)
                {
                    return;
                }

                if (value)
                {
                    Cursor.Show();
                }
                else
                {
                    Cursor.Hide();
                }

                cursorShown = value;
            }
        }

        private Timer timer;
        private Image backgroundImage;
        private Pen borderDotPen, borderDotPen2;
        private Point positionOnClick;
        private bool isMouseDown, isCtrlDown;
        private Stopwatch penTimer;
        private int drawingPenSize = 7;
        private Color drawingPenColor = Color.FromArgb(255, 0, 0);

        public RectangleAnnotate()
        {
            backgroundImage = Screenshot.CaptureFullscreen();
            borderDotPen = new Pen(Color.Black, 1);
            borderDotPen2 = new Pen(Color.White, 1);
            borderDotPen2.DashPattern = new float[] { 5, 5 };
            penTimer = Stopwatch.StartNew();
            ScreenRectangle = CaptureHelpers.GetScreenBounds();

            InitializeComponent();

            using (MemoryStream cursorStream = new MemoryStream(Resources.Crosshair))
            {
                Cursor = new Cursor(cursorStream);
            }

            timer = new Timer { Interval = 10 };
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (timer != null) timer.Dispose();
            if (backgroundImage != null) backgroundImage.Dispose();
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
            Text = "ShareX - Rectangle Capture Annotate";
            ShowInTaskbar = false;
            TopMost = true;

            Shown += RectangleAnnotate_Shown;
            MouseDown += RectangleAnnotate_MouseDown;
            MouseUp += RectangleAnnotate_MouseUp;
            MouseWheel += RectangleAnnotate_MouseWheel;
            KeyDown += RectangleAnnotate_KeyDown;
            KeyUp += RectangleAnnotate_KeyUp;
            FormClosing += RectangleAnnotate_FormClosing;

            ResumeLayout(false);
        }

        private void RectangleAnnotate_Shown(object sender, EventArgs e)
        {
            this.ShowActivate();
        }

        private void RectangleAnnotate_FormClosing(object sender, FormClosingEventArgs e)
        {
            CursorShown = true;
        }

        private void RectangleAnnotate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                isCtrlDown = true;
                CursorShown = false;
            }
        }

        private void RectangleAnnotate_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }

            if (e.KeyCode == Keys.ControlKey)
            {
                isCtrlDown = false;
                CursorShown = true;
            }
        }

        private void RectangleAnnotate_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                positionOnClick = CaptureHelpers.GetCursorPosition();
                isMouseDown = true;
            }
            else if (isMouseDown)
            {
                isMouseDown = false;
                Refresh();
            }
            else
            {
                Close();
            }
        }

        private void RectangleAnnotate_MouseUp(object sender, MouseEventArgs e)
        {
            if (isMouseDown && e.Button == MouseButtons.Left)
            {
                if (isCtrlDown)
                {
                    isMouseDown = false;
                }
                else
                {
                    if (SelectionRectangle0Based.Width > 0 && SelectionRectangle0Based.Height > 0)
                    {
                        LastSelectionRectangle0Based = SelectionRectangle0Based;
                        DialogResult = DialogResult.OK;
                    }

                    Close();
                }
            }
        }

        private void RectangleAnnotate_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                drawingPenSize++;
            }
            else if (e.Delta < 0)
            {
                drawingPenSize--;
            }

            drawingPenSize = drawingPenSize.Between(1, 100);
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
            PreviousMousePosition = CurrentMousePosition;
            CurrentMousePosition = CaptureHelpers.GetCursorPosition();
            SelectionRectangle = CaptureHelpers.CreateRectangle(positionOnClick.X, positionOnClick.Y, CurrentMousePosition.X, CurrentMousePosition.Y);
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

            if (isCtrlDown && isMouseDown)
            {
                using (Graphics gBackgroundImage = Graphics.FromImage(backgroundImage))
                using (Pen penDrawing = new Pen(drawingPenColor, drawingPenSize) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    gBackgroundImage.DrawLine(penDrawing, PreviousMousePosition0Based, CurrentMousePosition0Based);
                }
            }

            g.DrawImage(backgroundImage, ScreenRectangle0Based);

            if (isCtrlDown)
            {
                using (Pen penDrawing = new Pen(drawingPenColor, drawingPenSize) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                using (Brush brushDrawing = new SolidBrush(drawingPenColor))
                {
                    Rectangle rectCursor = new Rectangle(CurrentMousePosition0Based.X - drawingPenSize / 2, CurrentMousePosition0Based.Y - drawingPenSize / 2, drawingPenSize, drawingPenSize);
                    g.FillEllipse(brushDrawing, rectCursor);
                    g.DrawEllipse(Pens.Black, rectCursor);
                }
            }

            if (isMouseDown && !isCtrlDown)
            {
                if (ShowRectangleInfo)
                {
                    int offset = 10;
                    Point position = new Point(CurrentMousePosition0Based.X + offset, CurrentMousePosition0Based.Y + offset);

                    using (Font font = new Font("Arial", 17, FontStyle.Bold))
                    {
                        ImageHelpers.DrawTextWithOutline(g, string.Format("{0}, {1}\r\n{2} x {3}", SelectionRectangle.X, SelectionRectangle.Y,
                            SelectionRectangle.Width, SelectionRectangle.Height), position, font, Color.White, Color.Black, 3);
                    }
                }

                g.DrawRectangleProper(borderDotPen, SelectionRectangle0Based);
                borderDotPen2.DashOffset = (int)(penTimer.Elapsed.TotalMilliseconds / 100) % 10;
                g.DrawRectangleProper(borderDotPen2, SelectionRectangle0Based);
            }
        }
    }
}