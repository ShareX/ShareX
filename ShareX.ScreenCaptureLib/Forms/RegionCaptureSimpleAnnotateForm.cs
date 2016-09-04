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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public sealed class RegionCaptureSimpleAnnotateForm : Form
    {
        public static Rectangle LastSelectionRectangle0Based { get; private set; }

        public Rectangle ScreenRectangle { get; private set; }

        public Rectangle ScreenRectangle0Based => new Rectangle(0, 0, ScreenRectangle.Width, ScreenRectangle.Height);

        public Rectangle SelectionRectangle { get; private set; }

        public Rectangle SelectionRectangle0Based => new Rectangle(SelectionRectangle.X - ScreenRectangle.X, SelectionRectangle.Y - ScreenRectangle.Y,
            SelectionRectangle.Width, SelectionRectangle.Height);

        public Point CurrentMousePosition { get; private set; }

        public Point CurrentMousePosition0Based => new Point(CurrentMousePosition.X - ScreenRectangle.X, CurrentMousePosition.Y - ScreenRectangle.Y);

        public Point PreviousMousePosition { get; private set; }

        public Point PreviousMousePosition0Based => new Point(PreviousMousePosition.X - ScreenRectangle.X, PreviousMousePosition.Y - ScreenRectangle.Y);

        public RectangleAnnotateOptions Options { get; private set; }

        public RegionAnnotateMode Mode { get; private set; }

        public bool IsDrawingMode => Mode != RegionAnnotateMode.Capture && !isBusy;

        private Timer timer;
        private Image backgroundImage;
        private Pen borderDotPen, borderDotPen2, textBackgroundPenWhite, textBackgroundPenBlack;
        private Brush textBackgroundBrush;
        private Point positionOnClick;
        private bool isMouseDown, isBusy;
        private Stopwatch penTimer;
        private Font infoFont;

        public RegionCaptureSimpleAnnotateForm(Screenshot screenshot, RectangleAnnotateOptions options)
        {
            Options = options;

            backgroundImage = screenshot.CaptureFullscreen();
            borderDotPen = new Pen(Color.Black, 1);
            borderDotPen2 = new Pen(Color.White, 1);
            borderDotPen2.DashPattern = new float[] { 5, 5 };
            textBackgroundBrush = new SolidBrush(Color.FromArgb(75, Color.Black));
            textBackgroundPenWhite = new Pen(Color.FromArgb(50, Color.White));
            textBackgroundPenBlack = new Pen(Color.FromArgb(150, Color.Black));
            infoFont = new Font("Verdana", 9);
            penTimer = Stopwatch.StartNew();
            ScreenRectangle = CaptureHelpers.GetScreenBounds();

            InitializeComponent();
            Icon = ShareXResources.Icon;
            Cursor = Helpers.CreateCursor(Resources.Crosshair);

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
            if (textBackgroundBrush != null) textBackgroundBrush.Dispose();
            if (textBackgroundPenWhite != null) textBackgroundPenWhite.Dispose();
            if (textBackgroundPenBlack != null) textBackgroundPenBlack.Dispose();
            if (infoFont != null) infoFont.Dispose();

            base.Dispose(disposing);
        }

        private void RectangleAnnotate_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void RectangleAnnotate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D1)
            {
                Mode = RegionAnnotateMode.Capture;
            }
            else if (e.KeyCode == Keys.D2)
            {
                Mode = RegionAnnotateMode.Rectangle;
            }
            else if (e.KeyCode == Keys.D3)
            {
                Mode = RegionAnnotateMode.Pen;
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
            else if (e.KeyCode == Keys.F1)
            {
                Options.ShowTips = !Options.ShowTips;
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
            if (ModifierKeys == Keys.Control)
            {
                if (e.Delta > 0)
                {
                    if (Mode == RegionAnnotateMode.Rectangle)
                    {
                        Options.DrawingRectangleBorderSize++;
                    }
                    else if (Mode == RegionAnnotateMode.Pen)
                    {
                        Options.DrawingPenSize++;
                    }
                }
                else if (e.Delta < 0)
                {
                    if (Mode == RegionAnnotateMode.Rectangle)
                    {
                        Options.DrawingRectangleBorderSize--;
                    }
                    else if (Mode == RegionAnnotateMode.Pen)
                    {
                        Options.DrawingPenSize--;
                    }
                }
            }
            else
            {
                if (e.Delta > 0)
                {
                    Mode = Mode.Previous<RegionAnnotateMode>();
                }
                else if (e.Delta < 0)
                {
                    Mode = Mode.Next<RegionAnnotateMode>();
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
            if (Mode == RegionAnnotateMode.Pen && isMouseDown)
            {
                using (Graphics gImage = Graphics.FromImage(backgroundImage))
                {
                    gImage.SmoothingMode = SmoothingMode.HighQuality;
                    DrawLine(gImage);
                }
            }

            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.CompositingMode = CompositingMode.SourceCopy;
            g.DrawImage(backgroundImage, ScreenRectangle0Based);
            g.CompositingMode = CompositingMode.SourceOver;

            if (Mode == RegionAnnotateMode.Rectangle)
            {
                if (isMouseDown)
                {
                    DrawRectangle(g);
                }
                else
                {
                    DrawRectangleMarker(g);
                }
            }
            else if (Mode == RegionAnnotateMode.Pen)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                DrawDot(g);
                g.SmoothingMode = SmoothingMode.HighSpeed;
            }

            if (Options.ShowTips)
            {
                DrawTips(g, 10, 10);
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
                    borderDotPen2.DashOffset = (float)penTimer.Elapsed.TotalSeconds * -15;
                    g.DrawRectangleProper(borderDotPen2, SelectionRectangle0Based);
                }
            }
        }

        private void DrawInfoText(Graphics g, string text, Rectangle rect, int padding)
        {
            g.FillRectangle(textBackgroundBrush, rect.Offset(-2));
            g.DrawRectangleProper(textBackgroundPenBlack, rect.Offset(-1));
            g.DrawRectangleProper(textBackgroundPenWhite, rect);

            ImageHelpers.DrawTextWithShadow(g, text, rect.Offset(-padding).Location, infoFont, Brushes.White, Brushes.Black);
        }

        private void DrawTips(Graphics g, int offset, int padding)
        {
            StringBuilder sb = new StringBuilder();
            WriteTips(sb);
            string tipText = sb.ToString().Trim();

            Size textSize = g.MeasureString(tipText, infoFont).ToSize();
            int rectWidth = textSize.Width + padding * 2 + 2;
            int rectHeight = textSize.Height + padding * 2;
            Rectangle primaryScreenBounds = CaptureHelpers.GetPrimaryScreenBounds0Based();
            Rectangle textRectangle = new Rectangle(primaryScreenBounds.X + primaryScreenBounds.Width - rectWidth - offset, primaryScreenBounds.Y + offset, rectWidth, rectHeight);

            if (textRectangle.Offset(10).Contains(CurrentMousePosition0Based))
            {
                textRectangle.Y = primaryScreenBounds.Height - rectHeight - offset;
            }

            DrawInfoText(g, tipText, textRectangle, padding);
        }

        private void WriteTips(StringBuilder sb)
        {
            sb.AppendLine(Resources.RectangleRegion_WriteTips__F1__Hide_tips);

            sb.AppendLine();
            sb.AppendLine(Resources.RectangleAnnotate_WriteTips__Mouse_wheel__Swap_modes);
            if (Mode == RegionAnnotateMode.Capture) sb.Append("-> ");
            sb.AppendLine(Resources.RectangleAnnotate_WriteTips__1__Select_capture_mode);
            if (Mode == RegionAnnotateMode.Rectangle) sb.Append("-> ");
            sb.AppendLine(Resources.RectangleAnnotate_WriteTips__2__Select_rectangle_drawing_mode);
            if (Mode == RegionAnnotateMode.Pen) sb.Append("-> ");
            sb.AppendLine(Resources.RectangleAnnotate_WriteTips__3__Select_pen_drawing_mode);

            switch (Mode)
            {
                case RegionAnnotateMode.Rectangle:
                    sb.AppendLine();
                    sb.AppendLine(Resources.RectangleAnnotate_WriteTips__Shift__Change_border_color);
                    sb.AppendLine(Resources.RectangleAnnotate_WriteTips__Ctrl___Mouse_wheel__Change_border_size);
                    break;
                case RegionAnnotateMode.Pen:
                    sb.AppendLine();
                    sb.AppendLine(Resources.RectangleAnnotate_WriteTips__Shift__Change_pen_color);
                    sb.AppendLine(Resources.RectangleAnnotate_WriteTips__Ctrl___Mouse_wheel__Change_pen_size);
                    break;
            }

            sb.AppendLine();
            sb.AppendLine(Resources.RectangleRegion_WriteTips__Space__Fullscreen_capture);
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

            int offset = 10, padding = 3;
            Point pos = CurrentMousePosition0Based;
            Size textSize = g.MeasureString(infoText, infoFont).ToSize();
            DrawInfoText(g, infoText, new Rectangle(pos.X + offset, pos.Y + offset, textSize.Width + padding * 2, textSize.Height + padding * 2), padding);
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
            Rectangle rect = SelectionRectangle0Based.Offset(Options.DrawingRectangleBorderSize - 1);

            if (Options.DrawingRectangleShadow)
            {
                g.DrawRectangleShadow(rect.Offset(1), Color.DarkGray, 3, 128, 20, new Padding(1));
            }

            using (Pen pen = new Pen(Options.DrawingPenColor, Options.DrawingRectangleBorderSize) { Alignment = PenAlignment.Inset })
            {
                g.DrawRectangleProper(pen, rect);
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
            using (Brush brush = new SolidBrush(Options.DrawingPenColor))
            {
                Point pos = CurrentMousePosition0Based;
                Rectangle rect = new Rectangle((int)(pos.X - Options.DrawingPenSize / 2f), (int)(pos.Y - Options.DrawingPenSize / 2f), Options.DrawingPenSize, Options.DrawingPenSize);
                g.FillEllipse(brush, rect);
            }
        }

        private void DrawRectangleMarker(Graphics g)
        {
            using (Pen pen = new Pen(Options.DrawingPenColor, Options.DrawingRectangleBorderSize) { Alignment = PenAlignment.Inset })
            {
                Point pos = CurrentMousePosition0Based;
                int offset = 15;
                g.DrawRectangleProper(pen, new Rectangle(pos.X - offset, pos.Y - offset, offset * 2, offset * 2).Offset(Options.DrawingRectangleBorderSize - 1));
            }
        }
    }
}