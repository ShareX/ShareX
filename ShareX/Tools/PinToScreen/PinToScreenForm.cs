#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

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
using ShareX.Properties;
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

                    if (loaded)
                    {
                        AutoSizeForm();
                    }
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

        public bool Minimized { get; private set; }

        public PinToScreenOptions Options { get; private set; }

        private bool isDWMEnabled, loaded, dragging;
        private Point initialLocation;
        private Cursor openHandCursor, closedHandCursor;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= (int)WindowStyles.WS_EX_TOOLWINDOW;
                return createParams;
            }
        }

        private PinToScreenForm(PinToScreenOptions options)
        {
            Options = options;

            ImageScale = Options.InitialScale;
            ImageOpacity = Options.InitialOpacity;

            InitializeComponent();
            TopMost = Options.TopMost;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);

            openHandCursor = Helpers.CreateCursor(Resources.openhand);
            closedHandCursor = Helpers.CreateCursor(Resources.closedhand);
            SetHandCursor(false);

            isDWMEnabled = NativeMethods.IsDWMEnabled();

            loaded = true;
        }

        public PinToScreenForm(Image image, PinToScreenOptions options, Point? location = null) : this(options)
        {
            Image = image;
            AutoSizeForm();

            if (location != null)
            {
                Location = location.Value;
            }
            else
            {
                Rectangle rectScreen = CaptureHelpers.GetActiveScreenWorkingArea();
                Location = Helpers.GetPosition(Options.Placement, Options.PlacementOffset, rectScreen.Size, ImageSize);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                Image?.Dispose();
                openHandCursor?.Dispose();
                closedHandCursor?.Dispose();
            }

            base.Dispose(disposing);
        }

        public void SetHandCursor(bool grabbing)
        {
            if (grabbing)
            {
                if (Cursor != closedHandCursor)
                {
                    Cursor = closedHandCursor;
                }
            }
            else
            {
                if (Cursor != openHandCursor)
                {
                    Cursor = openHandCursor;
                }
            }
        }

        private void ResetImage()
        {
            ImageScale = 100;
            ImageOpacity = 100;
        }

        private void AutoSizeForm()
        {
            Size previousSize = Size;
            Size newSize;

            if (Minimized)
            {
                newSize = Options.MinimizeSize;
            }
            else
            {
                newSize = ImageSize;
            }

            Point newLocation = Location;
            SpecialWindowHandles insertAfter = Options.TopMost ? SpecialWindowHandles.HWND_TOPMOST : SpecialWindowHandles.HWND_TOP;
            SetWindowPosFlags flags = SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOSENDCHANGING;

            if (Options.KeepCenterLocation)
            {
                Point locationOffset = new Point((previousSize.Width - newSize.Width) / 2, (previousSize.Height - newSize.Height) / 2);
                newLocation = new Point(newLocation.X + locationOffset.X, newLocation.Y + locationOffset.Y);
            }
            else
            {
                flags |= SetWindowPosFlags.SWP_NOMOVE;
            }

            NativeMethods.SetWindowPos(Handle, (IntPtr)insertAfter, newLocation.X, newLocation.Y, newSize.Width, newSize.Height, flags);

            Refresh();
        }

        private void ToggleMinimize()
        {
            Minimized = !Minimized;

            if (ImageOpacity < 100)
            {
                if (Minimized)
                {
                    Opacity = 1f;
                }
                else
                {
                    Opacity = ImageOpacity / 100f;
                }
            }

            AutoSizeForm();
        }

        protected override void WndProc(ref Message m)
        {
            if (Options.ShowShadow && m.Msg == (int)WindowsMessages.NCPAINT && isDWMEnabled)
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
            Graphics g = e.Graphics;

            g.Clear(Options.BackgroundColor);

            if (Image != null)
            {
                if (Minimized)
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(Image, new Rectangle(0, 0, Options.MinimizeSize.Width, Options.MinimizeSize.Height),
                        0, 0, Options.MinimizeSize.Width, Options.MinimizeSize.Height, GraphicsUnit.Pixel);
                }
                else
                {
                    if (ImageScale == 100)
                    {
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.DrawImage(Image, 0, 0, Image.Width, Image.Height);
                    }
                    else
                    {
                        if (Options.HighQualityScale)
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
        }

        private void PinToScreenForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.Clicks > 1)
                {
                    ToggleMinimize();
                }
                else
                {
                    dragging = true;
                    initialLocation = e.Location;
                    SetHandCursor(true);
                }
            }
        }

        private void PinToScreenForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Location = new Point(Location.X + e.X - initialLocation.X, Location.Y + e.Y - initialLocation.Y);
                Update();
            }
        }

        private void PinToScreenForm_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    dragging = false;
                    SetHandCursor(false);
                    break;
                case MouseButtons.Right:
                    Close();
                    break;
            }

            if (!Minimized)
            {
                switch (e.Button)
                {
                    case MouseButtons.Middle:
                        ResetImage();
                        break;
                }
            }
        }

        private void PinToScreenForm_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!Minimized)
            {
                switch (ModifierKeys)
                {
                    case Keys.None:
                        if (e.Delta > 0)
                        {
                            ImageScale += Options.ScaleStep;
                        }
                        else if (e.Delta < 0)
                        {
                            ImageScale -= Options.ScaleStep;
                        }
                        break;
                    case Keys.Control:
                        if (e.Delta > 0)
                        {
                            ImageOpacity += Options.OpacityStep;
                        }
                        else if (e.Delta < 0)
                        {
                            ImageOpacity -= Options.OpacityStep;
                        }
                        break;
                }
            }
        }

        private void PinToScreenForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    Close();
                    break;
                case Keys.Control | Keys.C:
                    ClipboardHelpers.CopyImage(Image);
                    break;
            }

            if (!Minimized)
            {
                switch (e.KeyData)
                {
                    case Keys.Oemplus:
                    case Keys.Add:
                        ImageScale += Options.ScaleStep;
                        break;
                    case Keys.OemMinus:
                    case Keys.Subtract:
                        ImageScale -= Options.ScaleStep;
                        break;
                    case Keys.Control | Keys.Oemplus:
                    case Keys.Control | Keys.Add:
                        ImageOpacity += Options.OpacityStep;
                        break;
                    case Keys.Control | Keys.OemMinus:
                    case Keys.Control | Keys.Subtract:
                        ImageOpacity -= Options.OpacityStep;
                        break;
                }
            }
        }
    }
}