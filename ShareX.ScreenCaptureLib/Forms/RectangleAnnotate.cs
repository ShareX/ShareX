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
using ShareX.ScreenCaptureLib.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class RectangleAnnotate : BaseForm
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

        private bool cursorShown = true;

        public bool CursorShown
        {
            get
            {
                return cursorShown;
            }
            private set
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

        public RectangleAnnotateOptions Options { get; private set; }

        public RegionAnnotateMode Mode { get; private set; }

        public bool IsDrawingMode => Mode != RegionAnnotateMode.Capture && !isBusy;

        private Timer timer;
        private Image backgroundImage;
        private Pen borderDotPen, borderDotPen2;
        private Point positionOnClick;
        private bool isMouseDown, isBusy;
        private Stopwatch penTimer;
        private Font infofont, tipFont;

        public RectangleAnnotate(RectangleAnnotateOptions options)
        {
            Options = options;

            backgroundImage = Screenshot.CaptureFullscreen();
            borderDotPen = new Pen(Color.Black, 1);
            borderDotPen2 = new Pen(Color.White, 1);
            borderDotPen2.DashPattern = new float[] { 5, 5 };
            infofont = new Font("Arial", 15);
            tipFont = new Font("Arial", 13);
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

        private void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            StartPosition = FormStartPosition.Manual;
            Bounds = ScreenRectangle;
            FormBorderStyle = FormBorderStyle.None;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            Text = "ShareX - " + Resources.RectangleAnnotate_InitializeComponent_Rectangle_capture_annotate;
            ShowInTaskbar = false;
            TopMost = true;

            Shown += RectangleAnnotate_Shown;
            MouseDown += RectangleAnnotate_MouseDown;
            MouseUp += RectangleAnnotate_MouseUp;
            MouseWheel += RectangleAnnotate_MouseWheel;
            KeyDown += RectangleAnnotate_KeyDown;
            KeyUp += RectangleAnnotate_KeyUp;

            ResumeLayout(false);
        }

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
            if (infofont != null) infofont.Dispose();
            if (tipFont != null) tipFont.Dispose();

            base.Dispose(disposing);
        }

        private void RectangleAnnotate_Shown(object sender, EventArgs e)
        {
            this.ShowActivate();
        }

        private void RectangleAnnotate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D1)
            {
                Mode = RegionAnnotateMode.Capture;
            }
            else if (e.KeyCode == Keys.D2)
            {
                Mode = RegionAnnotateMode.Pen;
            }
            else if (e.KeyCode == Keys.D3)
            {
                Mode = RegionAnnotateMode.Rectangle;
            }
            else if (e.KeyCode == Keys.ControlKey)
            {
                switch (Mode)
                {
                    case RegionAnnotateMode.Capture:
                        Mode = RegionAnnotateMode.Pen;
                        break;
                    case RegionAnnotateMode.Pen:
                        Mode = RegionAnnotateMode.Rectangle;
                        break;
                    case RegionAnnotateMode.Rectangle:
                        Mode = RegionAnnotateMode.Capture;
                        break;
                }
            }
            else if (e.KeyCode == Keys.ShiftKey && IsDrawingMode)
            {
                try
                {
                    isBusy = true;
                    Options.DrawingPenColor = ColorPickerForm.GetColor(Options.DrawingPenColor);
                }
                finally
                {
                    isBusy = false;
                }
            }
        }

        private void RectangleAnnotate_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
            else if (e.KeyCode == Keys.Space)
            {
                DoSelection(ScreenRectangle);
            }
        }

        private void RectangleAnnotate_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                positionOnClick = CaptureHelpers.GetCursorPosition();
                isMouseDown = true;
            }
        }

        private void RectangleAnnotate_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && isMouseDown)
            {
                isMouseDown = false;

                switch (Mode)
                {
                    case RegionAnnotateMode.Capture:
                        DoSelection(SelectionRectangle);
                        break;
                    case RegionAnnotateMode.Rectangle:
                        AddRectangle();
                        break;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (isMouseDown)
                {
                    isMouseDown = false;
                }
                else
                {
                    Close();
                }
            }
        }

        private void RectangleAnnotate_MouseWheel(object sender, MouseEventArgs e)
        {
            if (IsDrawingMode)
            {
                if (e.Delta > 0)
                {
                    Options.DrawingPenSize++;
                }
                else if (e.Delta < 0)
                {
                    Options.DrawingPenSize--;
                }
            }
        }

        private void DoSelection(Rectangle rect)
        {
            SelectionRectangle = rect;

            if (SelectionRectangle0Based.Width > 0 && SelectionRectangle0Based.Height > 0)
            {
                LastSelectionRectangle0Based = SelectionRectangle0Based;
                DialogResult = DialogResult.OK;
            }

            Close();
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
            g.SmoothingMode = SmoothingMode.HighQuality;

            if (Mode == RegionAnnotateMode.Pen && isMouseDown)
            {
                using (Graphics gImage = Graphics.FromImage(backgroundImage))
                {
                    gImage.SmoothingMode = SmoothingMode.HighQuality;
                    DrawLine(gImage);
                }
            }

            g.CompositingMode = CompositingMode.SourceCopy;
            g.DrawImage(backgroundImage, ScreenRectangle0Based);
            g.CompositingMode = CompositingMode.SourceOver;

            if (Mode == RegionAnnotateMode.Pen)
            {
                DrawDot(g);
            }
            else if (Mode == RegionAnnotateMode.Rectangle && isMouseDown)
            {
                g.SmoothingMode = SmoothingMode.HighSpeed;
                DrawRectangle(g);
            }

            if (Options.ShowTips)
            {
                DrawTips(g);
            }

            if (!IsDrawingMode)
            {
                if (Options.ShowInfo)
                {
                    DrawInfo(g);
                }

                if (isMouseDown)
                {
                    g.DrawRectangleProper(borderDotPen, SelectionRectangle0Based);
                    borderDotPen2.DashOffset = (float)penTimer.Elapsed.TotalSeconds * -10;
                    g.DrawRectangleProper(borderDotPen2, SelectionRectangle0Based);
                }
            }
        }

        private void DrawTips(Graphics g)
        {
            int offset = 10;
            int padding = 3;

            string tipText;

            if (IsDrawingMode)
            {
                tipText = Resources.RectangleAnnotate_DrawTips_Drawing_mode_on;
            }
            else
            {
                tipText = Resources.RectangleAnnotate_DrawTips_Drawing_mode_off;
            }

            Size textSize = g.MeasureString(tipText, tipFont).ToSize();
            int rectWidth = textSize.Width + padding * 2;
            int rectHeight = textSize.Height + padding * 2;
            Rectangle primaryScreenBounds = CaptureHelpers.GetPrimaryScreenBounds0Based();
            Rectangle textRectangle = new Rectangle(primaryScreenBounds.X + (primaryScreenBounds.Width / 2) - (rectWidth / 2), primaryScreenBounds.Y + offset, rectWidth, rectHeight);

            if (textRectangle.Offset(10).Contains(CurrentMousePosition0Based))
            {
                textRectangle.Y = primaryScreenBounds.Height - rectHeight - offset;
            }

            using (Brush brush = new SolidBrush(Color.FromArgb(175, Color.White)))
            using (Pen pen = new Pen(Color.FromArgb(175, Color.Black)))
            {
                g.DrawRoundedRectangle(brush, pen, textRectangle, 5);
            }

            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(tipText, tipFont, Brushes.Black, textRectangle, sf);
            }
        }

        private void DrawInfo(Graphics g)
        {
            string infoText;

            if (isMouseDown)
            {
                infoText = string.Format("X: {0} Y: {1}\r\n{2} x {3}", SelectionRectangle.X, SelectionRectangle.Y, SelectionRectangle.Width, SelectionRectangle.Height);
            }
            else
            {
                infoText = string.Format("X: {0} Y: {1}", CurrentMousePosition.X, CurrentMousePosition.Y);
            }

            int offset = 10;
            Point position = new Point(CurrentMousePosition0Based.X + offset, CurrentMousePosition0Based.Y + offset);

            ImageHelpers.DrawTextWithShadow(g, infoText, position, infofont, Color.White, Color.Black);
        }

        private void DrawLine(Graphics g)
        {
            if (CurrentMousePosition0Based == PreviousMousePosition0Based)
            {
                DrawDot(g);
            }
            else
            {
                using (Pen pen = new Pen(Options.DrawingPenColor, Options.DrawingPenSize) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    g.DrawLine(pen, PreviousMousePosition0Based, CurrentMousePosition0Based);
                }
            }
        }

        private void DrawRectangle(Graphics g)
        {
            using (Pen pen = new Pen(Options.DrawingPenColor, Options.DrawingPenSize) { Alignment = PenAlignment.Inset })
            {
                g.DrawRectangleProper(pen, SelectionRectangle.Offset(Options.DrawingPenSize - 1));
            }
        }

        private void AddRectangle()
        {
            using (Graphics g = Graphics.FromImage(backgroundImage))
            {
                g.SmoothingMode = SmoothingMode.HighSpeed;
                DrawRectangle(g);
            }
        }

        private void DrawDot(Graphics g)
        {
            Point pos = CurrentMousePosition0Based;

            using (Brush brush = new SolidBrush(Options.DrawingPenColor))
            {
                RectangleF rect = new RectangleF(pos.X - Options.DrawingPenSize / 2f, pos.Y - Options.DrawingPenSize / 2f, Options.DrawingPenSize, Options.DrawingPenSize);
                g.FillEllipse(brush, rect);
            }
        }
    }
}