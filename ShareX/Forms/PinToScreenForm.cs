#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2022 ShareX Team

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
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ShareX
{
    public partial class PinToScreenForm : Form
    {
        public Image Image { get; private set; }

        private int imageScale = 100;

        public int ImageScale
        {
            get
            {
                return imageScale;
            }
            set
            {
                int newImageScale = value.Clamp(20, 500);

                if (imageScale != newImageScale)
                {
                    imageScale = newImageScale;

                    UpdateImage();
                }
            }
        }

        public Size ImageSize
        {
            get
            {
                return new Size((int)Math.Round(Image.Width * (ImageScale / 100f)), (int)Math.Round(Image.Height * (ImageScale / 100f)));
            }
        }

        private int imageOpacity = 100;

        public int ImageOpacity
        {
            get
            {
                return imageOpacity;
            }
            set
            {
                int newImageOpacity = value.Clamp(10, 100);

                if (imageOpacity != newImageOpacity)
                {
                    imageOpacity = newImageOpacity;

                    Opacity = imageOpacity / 100f;
                }
            }
        }

        public bool KeepCenterLocation { get; set; } = true;
        public bool HighQualityScale { get; set; } = true;
        public int ScaleStep { get; set; } = 10;
        public int OpacityStep { get; set; } = 10;
        public bool ShowShadow { get; set; } = true;

        private bool isDWMEnabled;

        private PinToScreenForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);

            isDWMEnabled = NativeMethods.IsDWMEnabled();
        }

        private PinToScreenForm(Image image) : this()
        {
            Image = image;

            UpdateImage();
            CenterForm();
        }

        public static void PinToScreen(Image image)
        {
            if (image != null)
            {
                PinToScreenForm form = new PinToScreenForm(image);
                form.Show();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                Image?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void UpdateImage()
        {
            AutoSizeForm();
            Refresh();
        }

        private void ResetImage()
        {
            ImageScale = 100;
            ImageOpacity = 100;
        }

        private void CenterForm()
        {
            Rectangle rectScreen = CaptureHelpers.GetActiveScreenWorkingArea();
            Location = new Point(rectScreen.X + (rectScreen.Width / 2) - (ImageSize.Width / 2), rectScreen.Y + (rectScreen.Height / 2) - (ImageSize.Height / 2));
        }

        private void AutoSizeForm()
        {
            Size previousSize = Size;
            Size newSize = ImageSize;
            Point newLocation = Location;
            SetWindowPosFlags flags = SetWindowPosFlags.SWP_NOACTIVATE;

            if (KeepCenterLocation)
            {
                Point locationOffset = new Point((previousSize.Width - newSize.Width) / 2, (previousSize.Height - newSize.Height) / 2);
                newLocation = new Point(newLocation.X + locationOffset.X, newLocation.Y + locationOffset.Y);
            }
            else
            {
                flags |= SetWindowPosFlags.SWP_NOMOVE;
            }

            NativeMethods.SetWindowPos(Handle, (IntPtr)SpecialWindowHandles.HWND_TOPMOST, newLocation.X, newLocation.Y, newSize.Width, newSize.Height, flags);
        }

        protected override void WndProc(ref Message m)
        {
            if (ShowShadow && m.Msg == (int)WindowsMessages.NCPAINT && isDWMEnabled)
            {
                NativeMethods.SetNCRenderingPolicy(Handle, DwmNCRenderingPolicy.Enabled);

                MARGINS margins = new MARGINS()
                {
                    bottomHeight = 1,
                    leftWidth = 1,
                    rightWidth = 1,
                    topHeight = 1
                };

                NativeMethods.DwmExtendFrameIntoClientArea(Handle, ref margins);
            }

            base.WndProc(ref m);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Image != null)
            {
                Graphics g = e.Graphics;

                if (ImageScale == 100)
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(Image, 0, 0);
                }
                else
                {
                    if (HighQualityScale)
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    }
                    else
                    {
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    }

                    using (ImageAttributes ia = new ImageAttributes())
                    {
                        ia.SetWrapMode(WrapMode.TileFlipXY);
                        g.DrawImage(Image, new Rectangle(0, 0, ImageSize.Width, ImageSize.Height), 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel, ia);
                    }
                }
            }
        }

        private void PinToScreenForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(Handle, (uint)WindowsMessages.NCLBUTTONDOWN, (IntPtr)WindowHitTestRegions.HTCAPTION, IntPtr.Zero);
            }
        }

        private void PinToScreenForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Close();
            }
            else if (e.Button == MouseButtons.Middle)
            {
                ResetImage();
            }
        }

        private void PinToScreenForm_MouseWheel(object sender, MouseEventArgs e)
        {
            switch (ModifierKeys)
            {
                case Keys.Control:
                    if (e.Delta > 0)
                    {
                        ImageOpacity += OpacityStep;
                    }
                    else if (e.Delta < 0)
                    {
                        ImageOpacity -= OpacityStep;
                    }
                    break;
                case Keys.None:
                    if (e.Delta > 0)
                    {
                        ImageScale += ScaleStep;
                    }
                    else if (e.Delta < 0)
                    {
                        ImageScale -= ScaleStep;
                    }
                    break;
            }
        }
    }
}