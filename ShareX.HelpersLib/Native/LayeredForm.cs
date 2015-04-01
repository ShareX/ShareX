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

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class LayeredForm : Form
    {
        public LayeredForm()
        {
            InitializeComponent();
        }

        #region Windows Form Designer generated code

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(292, 273);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "LayeredForm";
            this.ShowInTaskbar = false;
            this.Text = "LayeredForm";
            this.TopMost = true;
            this.ResumeLayout(false);
        }

        #endregion Windows Form Designer generated code

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= (int)WindowStyles.WS_EX_LAYERED;
                return createParams;
            }
        }

        public void SelectBitmap(Bitmap bitmap)
        {
            SelectBitmap(bitmap, 255);
        }

        public void SelectBitmap(Bitmap bitmap, int opacity)
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
                POINT newLocation = new POINT(this.Left, this.Top);
                BLENDFUNCTION blend = new BLENDFUNCTION();
                blend.BlendOp = NativeMethods.AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = (byte)opacity;
                blend.AlphaFormat = NativeMethods.AC_SRC_ALPHA;

                NativeMethods.UpdateLayeredWindow(this.Handle, screenDc, ref newLocation, ref newSize, memDc, ref sourceLocation, 0, ref blend, NativeMethods.ULW_ALPHA);
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
    }
}