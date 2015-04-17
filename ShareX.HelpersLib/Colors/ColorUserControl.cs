#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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

using ShareX.HelpersLib.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public abstract class ColorUserControl : UserControl
    {
        public event ColorEventHandler ColorChanged;

        public bool drawCrosshair;

        protected Bitmap bmp;
        protected int ClientWidth;
        protected int ClientHeight;
        protected DrawStyle drawStyle;
        protected MyColor selectedColor;
        protected bool mouseDown;
        protected Point lastPos;
        protected Timer mouseMoveTimer;

        public MyColor SelectedColor
        {
            get
            {
                return selectedColor;
            }
            set
            {
                selectedColor = value;

                if (this is ColorBox)
                {
                    SetBoxMarker();
                }
                else
                {
                    SetSliderMarker();
                }

                Refresh();
            }
        }

        public DrawStyle DrawStyle
        {
            get
            {
                return drawStyle;
            }
            set
            {
                drawStyle = value;

                if (this is ColorBox)
                {
                    SetBoxMarker();
                }
                else
                {
                    SetSliderMarker();
                }

                Refresh();
            }
        }

        #region Component Designer generated code

        private IContainer components = null;

        protected virtual void Initialize()
        {
            SuspendLayout();

            DoubleBuffered = true;
            ClientWidth = this.ClientRectangle.Width;
            ClientHeight = this.ClientRectangle.Height;
            bmp = new Bitmap(ClientWidth, ClientHeight, PixelFormat.Format32bppArgb);
            SelectedColor = Color.Red;
            DrawStyle = DrawStyle.Hue;

            mouseMoveTimer = new Timer();
            mouseMoveTimer.Interval = 10;
            mouseMoveTimer.Tick += new EventHandler(MouseMoveTimer_Tick);

            ClientSizeChanged += new EventHandler(EventClientSizeChanged);
            MouseDown += new MouseEventHandler(EventMouseDown);
            MouseEnter += new EventHandler(EventMouseEnter);
            MouseUp += new MouseEventHandler(EventMouseUp);
            Paint += new PaintEventHandler(EventPaint);

            ResumeLayout(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                if (bmp != null) bmp.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion Component Designer generated code

        #region Events

        private void EventClientSizeChanged(object sender, EventArgs e)
        {
            ClientWidth = ClientRectangle.Width;
            ClientHeight = ClientRectangle.Height;
            if (bmp != null) bmp.Dispose();
            bmp = new Bitmap(ClientWidth, ClientHeight, PixelFormat.Format32bppArgb);
            DrawColors();
        }

        private void EventMouseDown(object sender, MouseEventArgs e)
        {
            drawCrosshair = true;
            mouseDown = true;
            mouseMoveTimer.Start();
        }

        private void EventMouseEnter(object sender, EventArgs e)
        {
            if (this is ColorBox)
            {
                using (MemoryStream cursorStream = new MemoryStream(Resources.Crosshair))
                {
                    Cursor = new Cursor(cursorStream);
                }
            }
        }

        private void EventMouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            mouseMoveTimer.Stop();
        }

        private void EventPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (!mouseDown)
            {
                if (SelectedColor.IsTransparent)
                {
                    if (bmp != null) bmp.Dispose();
                    bmp = (Bitmap)ImageHelpers.DrawCheckers(ClientWidth, ClientHeight);
                }

                DrawColors();
            }

            g.DrawImage(bmp, ClientRectangle);

            if (drawCrosshair)
            {
                DrawCrosshair(g);
            }
        }

        private void MouseMoveTimer_Tick(object sender, EventArgs e)
        {
            Point mousePosition = GetPoint(PointToClient(MousePosition));

            if (mouseDown && lastPos != mousePosition)
            {
                GetPointColor(mousePosition);
                OnColorChanged();
                Refresh();
            }
        }

        #endregion Events

        #region Protected Methods

        protected void OnColorChanged()
        {
            if (ColorChanged != null)
            {
                ColorChanged(this, new ColorEventArgs(SelectedColor, DrawStyle));
            }
        }

        protected void DrawColors()
        {
            switch (DrawStyle)
            {
                case DrawStyle.Hue:
                    DrawHue();
                    break;
                case DrawStyle.Saturation:
                    DrawSaturation();
                    break;
                case DrawStyle.Brightness:
                    DrawBrightness();
                    break;
                case DrawStyle.Red:
                    DrawRed();
                    break;
                case DrawStyle.Green:
                    DrawGreen();
                    break;
                case DrawStyle.Blue:
                    DrawBlue();
                    break;
            }
        }

        protected void SetBoxMarker()
        {
            switch (DrawStyle)
            {
                case DrawStyle.Hue:
                    lastPos.X = Round((ClientWidth - 1) * SelectedColor.HSB.Saturation);
                    lastPos.Y = Round((ClientHeight - 1) * (1.0 - SelectedColor.HSB.Brightness));
                    break;
                case DrawStyle.Saturation:
                    lastPos.X = Round((ClientWidth - 1) * SelectedColor.HSB.Hue);
                    lastPos.Y = Round((ClientHeight - 1) * (1.0 - SelectedColor.HSB.Brightness));
                    break;
                case DrawStyle.Brightness:
                    lastPos.X = Round((ClientWidth - 1) * SelectedColor.HSB.Hue);
                    lastPos.Y = Round((ClientHeight - 1) * (1.0 - SelectedColor.HSB.Saturation));
                    break;
                case DrawStyle.Red:
                    lastPos.X = Round((ClientWidth - 1) * (double)SelectedColor.RGBA.Blue / 255);
                    lastPos.Y = Round((ClientHeight - 1) * (1.0 - (double)SelectedColor.RGBA.Green / 255));
                    break;
                case DrawStyle.Green:
                    lastPos.X = Round((ClientWidth - 1) * (double)SelectedColor.RGBA.Blue / 255);
                    lastPos.Y = Round((ClientHeight - 1) * (1.0 - (double)SelectedColor.RGBA.Red / 255));
                    break;
                case DrawStyle.Blue:
                    lastPos.X = Round((ClientWidth - 1) * (double)SelectedColor.RGBA.Red / 255);
                    lastPos.Y = Round((ClientHeight - 1) * (1.0 - (double)SelectedColor.RGBA.Green / 255));
                    break;
            }

            lastPos = GetPoint(lastPos);
        }

        protected void GetBoxColor()
        {
            switch (DrawStyle)
            {
                case DrawStyle.Hue:
                    selectedColor.HSB.Saturation = (double)lastPos.X / (ClientWidth - 1);
                    selectedColor.HSB.Brightness = 1.0 - (double)lastPos.Y / (ClientHeight - 1);
                    selectedColor.HSBUpdate();
                    break;
                case DrawStyle.Saturation:
                    selectedColor.HSB.Hue = (double)lastPos.X / (ClientWidth - 1);
                    selectedColor.HSB.Brightness = 1.0 - (double)lastPos.Y / (ClientHeight - 1);
                    selectedColor.HSBUpdate();
                    break;
                case DrawStyle.Brightness:
                    selectedColor.HSB.Hue = (double)lastPos.X / (ClientWidth - 1);
                    selectedColor.HSB.Saturation = 1.0 - (double)lastPos.Y / (ClientHeight - 1);
                    selectedColor.HSBUpdate();
                    break;
                case DrawStyle.Red:
                    selectedColor.RGBA.Blue = Round(255 * (double)lastPos.X / (ClientWidth - 1));
                    selectedColor.RGBA.Green = Round(255 * (1.0 - (double)lastPos.Y / (ClientHeight - 1)));
                    selectedColor.RGBAUpdate();
                    break;
                case DrawStyle.Green:
                    selectedColor.RGBA.Blue = Round(255 * (double)lastPos.X / (ClientWidth - 1));
                    selectedColor.RGBA.Red = Round(255 * (1.0 - (double)lastPos.Y / (ClientHeight - 1)));
                    selectedColor.RGBAUpdate();
                    break;
                case DrawStyle.Blue:
                    selectedColor.RGBA.Red = Round(255 * (double)lastPos.X / (ClientWidth - 1));
                    selectedColor.RGBA.Green = Round(255 * (1.0 - (double)lastPos.Y / (ClientHeight - 1)));
                    selectedColor.RGBAUpdate();
                    break;
            }
        }

        protected void SetSliderMarker()
        {
            switch (DrawStyle)
            {
                case DrawStyle.Hue:
                    lastPos.Y = (ClientHeight - 1) - Round((ClientHeight - 1) * SelectedColor.HSB.Hue);
                    break;
                case DrawStyle.Saturation:
                    lastPos.Y = (ClientHeight - 1) - Round((ClientHeight - 1) * SelectedColor.HSB.Saturation);
                    break;
                case DrawStyle.Brightness:
                    lastPos.Y = (ClientHeight - 1) - Round((ClientHeight - 1) * SelectedColor.HSB.Brightness);
                    break;
                case DrawStyle.Red:
                    lastPos.Y = (ClientHeight - 1) - Round((ClientHeight - 1) * (double)SelectedColor.RGBA.Red / 255);
                    break;
                case DrawStyle.Green:
                    lastPos.Y = (ClientHeight - 1) - Round((ClientHeight - 1) * (double)SelectedColor.RGBA.Green / 255);
                    break;
                case DrawStyle.Blue:
                    lastPos.Y = (ClientHeight - 1) - Round((ClientHeight - 1) * (double)SelectedColor.RGBA.Blue / 255);
                    break;
            }
            lastPos = GetPoint(lastPos);
        }

        protected void GetSliderColor()
        {
            switch (DrawStyle)
            {
                case DrawStyle.Hue:
                    selectedColor.HSB.Hue = 1.0 - (double)lastPos.Y / (ClientHeight - 1);
                    selectedColor.HSBUpdate();
                    break;
                case DrawStyle.Saturation:
                    selectedColor.HSB.Saturation = 1.0 - (double)lastPos.Y / (ClientHeight - 1);
                    selectedColor.HSBUpdate();
                    break;
                case DrawStyle.Brightness:
                    selectedColor.HSB.Brightness = 1.0 - (double)lastPos.Y / (ClientHeight - 1);
                    selectedColor.HSBUpdate();
                    break;
                case DrawStyle.Red:
                    selectedColor.RGBA.Red = 255 - Round(255 * (double)lastPos.Y / (ClientHeight - 1));
                    selectedColor.RGBAUpdate();
                    break;
                case DrawStyle.Green:
                    selectedColor.RGBA.Green = 255 - Round(255 * (double)lastPos.Y / (ClientHeight - 1));
                    selectedColor.RGBAUpdate();
                    break;
                case DrawStyle.Blue:
                    selectedColor.RGBA.Blue = 255 - Round(255 * (double)lastPos.Y / (ClientHeight - 1));
                    selectedColor.RGBAUpdate();
                    break;
            }
        }

        protected abstract void DrawCrosshair(Graphics g);

        protected abstract void DrawHue();

        protected abstract void DrawSaturation();

        protected abstract void DrawBrightness();

        protected abstract void DrawRed();

        protected abstract void DrawGreen();

        protected abstract void DrawBlue();

        #endregion Protected Methods

        #region Protected Helpers

        protected void GetPointColor(Point point)
        {
            lastPos = point;
            if (this is ColorBox)
            {
                GetBoxColor();
            }
            else
            {
                GetSliderColor();
            }
        }

        protected Point GetPoint(Point point)
        {
            return new Point(point.X.Between(0, ClientWidth - 1), point.Y.Between(0, ClientHeight - 1));
        }

        protected int Round(double val)
        {
            int ret_val = (int)val;

            int temp = (int)(val * 100);

            if ((temp % 100) >= 50)
                ret_val += 1;

            return ret_val;
        }

        #endregion Protected Helpers
    }
}