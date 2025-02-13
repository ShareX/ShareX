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

using ShareX.HelpersLib;
using ShareX.ScreenCaptureLib.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public sealed class RegionCaptureTransparentForm : LayeredForm
    {
        private const int MinimumRectangleSize = 3;

        public static Rectangle LastSelectionRectangle0Based { get; private set; }

        public Rectangle ScreenRectangle { get; private set; }
        public Rectangle ScreenRectangle0Based => new Rectangle(0, 0, ScreenRectangle.Width, ScreenRectangle.Height);
        public Rectangle SelectionRectangle { get; private set; }
        public Rectangle SelectionRectangle0Based => new Rectangle(SelectionRectangle.X - ScreenRectangle.X,
            SelectionRectangle.Y - ScreenRectangle.Y, SelectionRectangle.Width, SelectionRectangle.Height);
        private Rectangle PreviousSelectionRectangle { get; set; }
        private Rectangle PreviousSelectionRectangle0Based => new Rectangle(PreviousSelectionRectangle.X - ScreenRectangle.X,
            PreviousSelectionRectangle.Y - ScreenRectangle.Y, PreviousSelectionRectangle.Width, PreviousSelectionRectangle.Height);

        private Timer timer;
        private Bitmap backgroundImage;
        private Graphics gBackgroundImage;
        private Pen clearPen, borderDotPen, borderDotPen2;
        private Point currentPosition, positionOnClick;
        private bool isMouseDown;
        private Stopwatch penTimer;

        public RegionCaptureTransparentForm(bool activeMonitorMode = false)
        {
            clearPen = new Pen(Color.FromArgb(1, 0, 0, 0));
            borderDotPen = new Pen(Color.Black, 1);
            borderDotPen2 = new Pen(Color.White, 1);
            borderDotPen2.DashPattern = new float[] { 5, 5 };
            penTimer = Stopwatch.StartNew();

            if (activeMonitorMode)
            {
                ScreenRectangle = CaptureHelpers.GetActiveScreenBounds();

                Helpers.LockCursorToWindow(this);
            }
            else
            {
                ScreenRectangle = CaptureHelpers.GetScreenBounds();
            }

            backgroundImage = new Bitmap(ScreenRectangle.Width, ScreenRectangle.Height);
            gBackgroundImage = Graphics.FromImage(backgroundImage);
            gBackgroundImage.InterpolationMode = InterpolationMode.NearestNeighbor;
            gBackgroundImage.SmoothingMode = SmoothingMode.HighSpeed;
            gBackgroundImage.CompositingMode = CompositingMode.SourceCopy;
            gBackgroundImage.CompositingQuality = CompositingQuality.HighSpeed;
            gBackgroundImage.Clear(Color.FromArgb(1, 0, 0, 0));

            StartPosition = FormStartPosition.Manual;
            Bounds = ScreenRectangle;
            Text = "ShareX - " + Resources.RectangleTransparent_RectangleTransparent_Rectangle_capture_transparent;
            Cursor = Helpers.CreateCursor(Resources.Crosshair);
            TopMost = true;

            Shown += RectangleTransparent_Shown;
            KeyUp += RectangleTransparent_KeyUp;
            MouseDown += RectangleTransparent_MouseDown;
            MouseUp += RectangleTransparent_MouseUp;

            timer = new Timer { Interval = 10 };
            timer.Tick += timer_Tick;
            timer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (timer != null) timer.Dispose();
            if (clearPen != null) clearPen.Dispose();
            if (borderDotPen != null) borderDotPen.Dispose();
            if (borderDotPen2 != null) borderDotPen2.Dispose();
            if (gBackgroundImage != null) gBackgroundImage.Dispose();
            if (backgroundImage != null) backgroundImage.Dispose();

            base.Dispose(disposing);
        }

        private void RectangleTransparent_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void RectangleTransparent_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void RectangleTransparent_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                positionOnClick = CaptureHelpers.GetCursorPosition();
                isMouseDown = true;
            }
        }

        private void RectangleTransparent_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isMouseDown && SelectionRectangle0Based.Width > MinimumRectangleSize && SelectionRectangle0Based.Height > MinimumRectangleSize)
                {
                    LastSelectionRectangle0Based = SelectionRectangle0Based;
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    isMouseDown = false;
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
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
            }
        }

        public Bitmap GetAreaImage(Screenshot screenshot)
        {
            Rectangle rect = SelectionRectangle0Based;

            if (rect.Width > 0 && rect.Height > 0)
            {
                return screenshot.CaptureRectangle(SelectionRectangle);
            }

            return null;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            currentPosition = CaptureHelpers.GetCursorPosition();
            PreviousSelectionRectangle = SelectionRectangle;
            SelectionRectangle = CaptureHelpers.CreateRectangle(positionOnClick.X, positionOnClick.Y, currentPosition.X, currentPosition.Y);

            UpdateBackgroundImage();
        }

        private void UpdateBackgroundImage()
        {
            // Clear previous rectangle selection
            gBackgroundImage.DrawRectangleProper(clearPen, PreviousSelectionRectangle0Based);

            if (isMouseDown && SelectionRectangle0Based.Width > MinimumRectangleSize && SelectionRectangle0Based.Height > MinimumRectangleSize)
            {
                borderDotPen2.DashOffset = (float)penTimer.Elapsed.TotalSeconds * -15;

                gBackgroundImage.DrawRectangleProper(borderDotPen, SelectionRectangle0Based);
                gBackgroundImage.DrawRectangleProper(borderDotPen2, SelectionRectangle0Based);
            }

            SelectBitmap(backgroundImage);
        }
    }
}