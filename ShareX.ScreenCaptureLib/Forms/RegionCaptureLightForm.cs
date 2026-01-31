#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public sealed class RegionCaptureLightForm : Form
    {
        private const int MinimumRectangleSize = 5;

        public static Rectangle LastSelectionRectangle { get; private set; }
        public static Rectangle LastScreenSelectionRectangle { get; private set; }

        public Rectangle ScreenRectangle { get; private set; }
        public Rectangle SelectionRectangle { get; private set; }
        public Rectangle ScreenSelectionRectangle { get; private set; }

        private Bitmap backgroundImage;
        private TextureBrush backgroundBrush;
        private Pen borderDotPen, borderDotPen2;
        private Point positionOnClick;
        private bool isMouseDown;
        private bool isTransparentBackground;

        public RegionCaptureLightForm(Bitmap background, bool activeMonitorMode = false)
        {
            borderDotPen = new Pen(Color.White, 1);
            borderDotPen2 = new Pen(Color.Black, 1);
            borderDotPen2.DashPattern = new float[] { 5, 5 };

            if (activeMonitorMode)
            {
                ScreenRectangle = CaptureHelpers.GetActiveScreenBounds();

                Helpers.LockCursorToWindow(this);
            }
            else
            {
                ScreenRectangle = CaptureHelpers.GetScreenBounds();
            }

            isTransparentBackground = background == null;

            if (!isTransparentBackground)
            {
                backgroundImage = background;
                backgroundBrush = new TextureBrush(backgroundImage);
            }
            else
            {
                backgroundImage = new Bitmap(ScreenRectangle.Width, ScreenRectangle.Height, PixelFormat.Format32bppArgb);
            }

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            AutoScaleMode = AutoScaleMode.None;
            StartPosition = FormStartPosition.Manual;
            Bounds = ScreenRectangle;
            FormBorderStyle = FormBorderStyle.None;

            if (!isTransparentBackground)
            {
                SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
                Text = "ShareX - " + Resources.RectangleLight_InitializeComponent_Rectangle_capture_light;
            }
            else
            {
                Text = "ShareX - " + Resources.RectangleTransparent_RectangleTransparent_Rectangle_capture_transparent;
            }
            ShowInTaskbar = false;
#if !DEBUG
            TopMost = true;
#endif

            Shown += RegionCaptureLightForm_Shown;
            KeyUp += RegionCaptureLightForm_KeyUp;
            MouseDown += RegionCaptureLightForm_MouseDown;
            MouseUp += RegionCaptureLightForm_MouseUp;
            MouseMove += RegionCaptureLightForm_MouseMove;

            ResumeLayout(false);

            Icon = ShareXResources.Icon;
            Cursor = Helpers.CreateCursor(Resources.Crosshair);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;

                if (isTransparentBackground)
                {
                    createParams.ExStyle |= (int)WindowStyles.WS_EX_LAYERED;
                }

                return createParams;
            }
        }

        protected override void Dispose(bool disposing)
        {
            backgroundImage?.Dispose();
            backgroundBrush?.Dispose();
            borderDotPen?.Dispose();
            borderDotPen2?.Dispose();

            base.Dispose(disposing);
        }

        public void SelectBitmap(Bitmap bitmap, int opacity = 255)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new ApplicationException("The bitmap must be 32bpp with alpha-channel.");
            }

            IntPtr screenDc = NativeMethods.GetDC(IntPtr.Zero);
            IntPtr memDc = NativeMethods.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr hOldBitmap = IntPtr.Zero;

            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                hOldBitmap = NativeMethods.SelectObject(memDc, hBitmap);

                SIZE newSize = new SIZE(bitmap.Width, bitmap.Height);
                POINT sourceLocation = new POINT(0, 0);
                POINT newLocation = new POINT(Left, Top);
                BLENDFUNCTION blend = new BLENDFUNCTION();
                blend.BlendOp = NativeConstants.AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = (byte)opacity;
                blend.AlphaFormat = NativeConstants.AC_SRC_ALPHA;

                NativeMethods.UpdateLayeredWindow(Handle, screenDc, ref newLocation, ref newSize, memDc, ref sourceLocation, 0, ref blend, NativeConstants.ULW_ALPHA);
            }
            finally
            {
                NativeMethods.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    NativeMethods.SelectObject(memDc, hOldBitmap);
                    NativeMethods.DeleteObject(hBitmap);
                }
                NativeMethods.DeleteDC(memDc);
            }
        }

        public Bitmap GetAreaImage()
        {
            Rectangle rect = SelectionRectangle;

            if (rect.Width > 0 && rect.Height > 0)
            {
                if (rect.X == 0 && rect.Y == 0 && rect.Width == backgroundImage.Width && rect.Height == backgroundImage.Height)
                {
                    return (Bitmap)backgroundImage.Clone();
                }

                return ImageHelpers.CropBitmap(backgroundImage, rect);
            }

            return null;
        }

        public Bitmap GetAreaImage(Screenshot screenshot)
        {
            Rectangle rect = ScreenSelectionRectangle;

            if (rect.Width > 0 && rect.Height > 0)
            {
                return screenshot.CaptureRectangle(rect);
            }

            return null;
        }

        private void DrawDottedRectangle(Graphics g, Pen pen1, Pen pen2, Rectangle rect)
        {
            g.DrawRectangleProper(pen1, rect);
            g.DrawLine(pen2, rect.X, rect.Y, rect.Right - 1, rect.Y);
            g.DrawLine(pen2, rect.X, rect.Y, rect.X, rect.Bottom - 1);
            g.DrawLine(pen2, rect.Right - 1, rect.Y, rect.Right - 1, rect.Bottom - 1);
            g.DrawLine(pen2, rect.X, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1);
        }

        private void RegionCaptureLightForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void RegionCaptureLightForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void RegionCaptureLightForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                positionOnClick = e.Location;
                isMouseDown = true;
            }
        }

        private void RegionCaptureLightForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isMouseDown && SelectionRectangle.Width > MinimumRectangleSize && SelectionRectangle.Height > MinimumRectangleSize)
                {
                    LastSelectionRectangle = SelectionRectangle;
                    LastScreenSelectionRectangle = new Rectangle(LastSelectionRectangle.X + ScreenRectangle.X,
                        LastSelectionRectangle.Y + ScreenRectangle.Y, LastSelectionRectangle.Width, LastSelectionRectangle.Height);
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
                    Render();
                }
                else
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
            }
        }

        private void RegionCaptureLightForm_MouseMove(object sender, MouseEventArgs e)
        {
            SelectionRectangle = CaptureHelpers.CreateRectangle(positionOnClick.X, positionOnClick.Y, e.X, e.Y);
            ScreenSelectionRectangle = new Rectangle(SelectionRectangle.X + ScreenRectangle.X, SelectionRectangle.Y + ScreenRectangle.Y,
                SelectionRectangle.Width, SelectionRectangle.Height);

            Render();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!isTransparentBackground)
            {
                Graphics g = e.Graphics;
                g.CompositingMode = CompositingMode.SourceCopy;
                g.SmoothingMode = SmoothingMode.None;
                g.FillRectangle(backgroundBrush, 0, 0, ScreenRectangle.Width, ScreenRectangle.Height);

                if (isMouseDown && SelectionRectangle.Width > MinimumRectangleSize && SelectionRectangle.Height > MinimumRectangleSize)
                {
                    DrawDottedRectangle(g, borderDotPen, borderDotPen2, SelectionRectangle);
                }
            }
        }

        private void Render()
        {
            if (isTransparentBackground)
            {
                UpdateLayeredSurface();
            }
            else
            {
                Invalidate();
            }
        }

        private void UpdateLayeredSurface()
        {
            using (Graphics g = Graphics.FromImage(backgroundImage))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.SmoothingMode = SmoothingMode.None;
                g.Clear(Color.FromArgb(1, 0, 0, 0));

                if (isMouseDown && SelectionRectangle.Width > MinimumRectangleSize && SelectionRectangle.Height > MinimumRectangleSize)
                {
                    DrawDottedRectangle(g, borderDotPen, borderDotPen2, SelectionRectangle);
                }
            }

            SelectBitmap(backgroundImage);
        }
    }
}