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
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class ScrollingCaptureForm : Form
    {
        public event Action<Bitmap> UploadRequested;

        public ScrollingCaptureOptions Options { get; private set; }

        private ScrollingCaptureManager manager;
        private Point dragStartPosition;

        public ScrollingCaptureForm(ScrollingCaptureOptions options)
        {
            Options = options;

            InitializeComponent();
            ShareXResources.ApplyTheme(this);

            manager = new ScrollingCaptureManager(Options);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                manager?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void ResetPictureBox()
        {
            Image temp = pbOutput.Image;
            pbOutput.Image = null;
            temp?.Dispose();
        }

        private async Task StartCapture()
        {
            WindowState = FormWindowState.Minimized;
            btnCapture.Enabled = false;
            btnUpload.Enabled = false;
            btnOptions.Enabled = false;
            lblResultSize.Text = "";
            ResetPictureBox();

            try
            {
                await manager.StartCapture();
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);

                e.ShowError();
            }

            btnCapture.Enabled = true;
            btnOptions.Enabled = true;

            if (manager.Result != null)
            {
                btnUpload.Enabled = true;
                pbOutput.Image = manager.Result;
                pOutput.AutoScrollPosition = new Point(0, 0);
                lblResultSize.Text = $"{manager.Result.Width}x{manager.Result.Height}";
            }

            this.ForceActivate();

            if (Options.AutoUpload)
            {
                UploadResult();
            }
        }

        private async Task SelectWindow()
        {
            WindowState = FormWindowState.Minimized;
            Thread.Sleep(250);

            if (manager.SelectWindow())
            {
                await StartCapture();
            }
            else
            {
                this.ForceActivate();
            }
        }

        private void UploadResult()
        {
            if (manager.Result != null)
            {
                OnUploadRequested((Bitmap)manager.Result.Clone());
            }
        }

        protected void OnUploadRequested(Bitmap bmp)
        {
            UploadRequested?.Invoke(bmp);
        }

        private async void ScrollingCaptureForm_Load(object sender, EventArgs e)
        {
            await SelectWindow();
        }

        private void ScrollingCaptureForm_Activated(object sender, EventArgs e)
        {
            manager.StopCapture();
        }

        private async void btnCapture_Click(object sender, EventArgs e)
        {
            await SelectWindow();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            UploadResult();
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            using (ScrollingCaptureOptionsForm scrollingCaptureOptionsForm = new ScrollingCaptureOptionsForm(Options))
            {
                scrollingCaptureOptionsForm.ShowDialog();
            }
        }

        private void pbOutput_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && (pOutput.HorizontalScroll.Visible || pOutput.VerticalScroll.Visible))
            {
                pOutput.Cursor = Cursors.SizeAll;
                dragStartPosition = e.Location;
            }
        }

        private void pbOutput_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && (pOutput.HorizontalScroll.Visible || pOutput.VerticalScroll.Visible))
            {
                Point scrollOffset = new Point(e.X - dragStartPosition.X, e.Y - dragStartPosition.Y);
                pOutput.AutoScrollPosition = new Point(-pOutput.AutoScrollPosition.X - scrollOffset.X, -pOutput.AutoScrollPosition.Y - scrollOffset.Y);
                pOutput.Update();
            }
        }

        private void pbOutput_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pOutput.Cursor = Cursors.Default;
            }
        }
    }
}