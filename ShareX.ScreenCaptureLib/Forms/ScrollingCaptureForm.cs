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
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class ScrollingCaptureForm : Form
    {
        private static readonly object lockObject = new object();

        private static ScrollingCaptureForm instance;

        public event Action<Bitmap> UploadRequested;
        public event Action PlayNotificationSound;

        public ScrollingCaptureOptions Options { get; private set; }

        private ScrollingCaptureManager manager;
        private Point dragStartPosition;

        private ScrollingCaptureForm(ScrollingCaptureOptions options)
        {
            Options = options;

            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            manager = new ScrollingCaptureManager(Options);
        }

        public static async Task StartStopScrollingCapture(ScrollingCaptureOptions options, Action<Bitmap> uploadRequested = null, Action playNotificationSound = null)
        {
            if (instance == null || instance.IsDisposed)
            {
                lock (lockObject)
                {
                    if (instance == null || instance.IsDisposed)
                    {
                        instance = new ScrollingCaptureForm(options);

                        if (uploadRequested != null)
                        {
                            instance.UploadRequested += uploadRequested;
                        }

                        if (playNotificationSound != null)
                        {
                            instance.PlayNotificationSound += playNotificationSound;
                        }

                        instance.Show();
                    }
                }
            }
            else
            {
                await instance.StartStopScrollingCapture();
            }
        }

        public async Task StartStopScrollingCapture()
        {
            if (manager.IsCapturing)
            {
                manager.StopCapture();
            }
            else
            {
                await SelectWindow();
            }
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
            btnCopy.Enabled = false;
            btnOptions.Enabled = false;
            lblResultSize.Text = "";
            ResetPictureBox();

            try
            {
                ScrollingCaptureStatus status = await manager.StartCapture();

                switch (status)
                {
                    case ScrollingCaptureStatus.Failed:
                        pbStatus.Image = Resources.control_record;
                        break;
                    case ScrollingCaptureStatus.PartiallySuccessful:
                        pbStatus.Image = Resources.control_record_yellow;
                        break;
                    case ScrollingCaptureStatus.Successful:
                        pbStatus.Image = Resources.control_record_green;
                        break;
                }

                OnPlayNotificationSound();
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);

                e.ShowError();
            }

            btnCapture.Enabled = true;
            btnOptions.Enabled = true;

            LoadImage(manager.Result);

            this.ForceActivate();

            if (Options.AutoUpload)
            {
                UploadResult();
            }
        }

        private void LoadImage(Bitmap bmp)
        {
            if (bmp != null)
            {
                btnUpload.Enabled = true;
                btnCopy.Enabled = true;
                pbOutput.Image = bmp;
                pOutput.AutoScrollPosition = new Point(0, 0);
                lblResultSize.Text = $"{bmp.Width}x{bmp.Height}";
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

        private void CopyResult()
        {
            if (manager.Result != null)
            {
                ClipboardHelpers.CopyImage(manager.Result);
            }
        }

        protected void OnUploadRequested(Bitmap bmp)
        {
            UploadRequested?.Invoke(bmp);
        }

        protected void OnPlayNotificationSound()
        {
            PlayNotificationSound?.Invoke();
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

        private void btnCopy_Click(object sender, EventArgs e)
        {
            CopyResult();
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            using (ScrollingCaptureOptionsForm scrollingCaptureOptionsForm = new ScrollingCaptureOptionsForm(Options))
            {
                scrollingCaptureOptionsForm.ShowDialog();
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.DocsScrollingScreenshot);
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