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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class ScrollingCaptureLightForm : Form
    {
        public event Action<Bitmap> UploadRequested;

        public ScrollingCaptureOptions Options { get; private set; }
        public Bitmap Result { get; private set; }

        private List<Bitmap> images = new List<Bitmap>();
        private bool isCapturing, stopRequested;
        private int currentScrollCount, bestMatchCount, bestMatchIndex;
        private WindowInfo selectedWindow;
        private Rectangle selectedRectangle;
        private Point dragStartPosition;

        public ScrollingCaptureLightForm(ScrollingCaptureOptions options)
        {
            Options = options;

            InitializeComponent();
            ShareXResources.ApplyTheme(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();

                Reset();
            }

            base.Dispose(disposing);
        }

        private void Reset()
        {
            currentScrollCount = 0;

            ResetPictureBox();
            btnUpload.Enabled = false;

            if (images != null)
            {
                foreach (Bitmap bmp in images)
                {
                    bmp?.Dispose();
                }

                images.Clear();
            }

            if (Result != null)
            {
                Result.Dispose();
                Result = null;
            }
        }

        private void ResetPictureBox()
        {
            Image temp = pbOutput.Image;
            pbOutput.Image = null;
            temp?.Dispose();
        }

        private async Task StartCapture()
        {
            if (!isCapturing)
            {
                isCapturing = true;
                stopRequested = false;
                bestMatchCount = 0;
                bestMatchIndex = 0;

                // TODO: Translate
                btnCapture.Text = "Stop";
                WindowState = FormWindowState.Minimized;
                Reset();
                selectedWindow.Activate();

                await Task.Delay(Options.StartDelay);
                await CaptureJob();
            }
        }

        private void StopCapture()
        {
            stopRequested = true;
        }

        private void EndCapture()
        {
            // TODO: Translate
            btnCapture.Text = "Capture...";
            btnUpload.Enabled = Result != null;
            pbOutput.Image = Result;
            this.ForceActivate();

            isCapturing = false;
        }

        private async Task CaptureJob()
        {
            InputHelpers.SendKeyPress(VirtualKeyCode.HOME);
            NativeMethods.SendMessage(selectedWindow.Handle, (int)WindowsMessages.VSCROLL, (int)ScrollBarCommands.SB_TOP, 0);

            do
            {
                await Task.Delay(Options.ScrollDelay);

                Screenshot screenshot = new Screenshot()
                {
                    CaptureCursor = false
                };

                Bitmap bmp = screenshot.CaptureRectangle(selectedRectangle);

                if (bmp != null)
                {
                    images.Add(bmp);
                }

                InputHelpers.SendMouseWheel(-120 * 2);
                currentScrollCount++;

                if (images.Count > 0)
                {
                    Result = await CombineImagesAsync(Result, images[images.Count - 1]);
                }
            }
            while (!stopRequested && currentScrollCount <= Options.MaximumScrollCount && !IsScrollReachedBottom(selectedWindow.Handle));

            EndCapture();
        }

        private bool IsScrollReachedBottom(IntPtr handle)
        {
            SCROLLINFO scrollInfo = new SCROLLINFO();
            scrollInfo.cbSize = (uint)Marshal.SizeOf(scrollInfo);
            scrollInfo.fMask = (uint)(ScrollInfoMask.SIF_RANGE | ScrollInfoMask.SIF_PAGE | ScrollInfoMask.SIF_TRACKPOS);

            if (NativeMethods.GetScrollInfo(handle, (int)SBOrientation.SB_VERT, ref scrollInfo))
            {
                return scrollInfo.nMax == scrollInfo.nTrackPos + scrollInfo.nPage - 1;
            }

            return IsLastTwoImagesSame();
        }

        private bool IsLastTwoImagesSame()
        {
            bool result = false;

            if (images.Count > 1)
            {
                result = ImageHelpers.IsImagesEqual(images[images.Count - 1], images[images.Count - 2]);

                if (result)
                {
                    Bitmap last = images[images.Count - 1];
                    images.Remove(last);
                    last.Dispose();
                }
            }

            return result;
        }

        private async Task SelectWindow()
        {
            WindowState = FormWindowState.Minimized;
            Thread.Sleep(250);

            if (RegionCaptureTasks.GetRectangleRegion(out selectedRectangle, out selectedWindow, new RegionCaptureOptions()))
            {
                await StartCapture();
            }
            else
            {
                this.ForceActivate();
            }
        }

        private async Task<Bitmap> CombineImagesAsync(Bitmap result, Bitmap currentImage)
        {
            return await Task.Run(() => CombineImages(result, currentImage));
        }

        private Bitmap CombineImages(Bitmap result, Bitmap currentImage)
        {
            if (result == null)
            {
                return (Bitmap)currentImage.Clone();
            }

            int matchCount = 0;
            int matchIndex = 0;
            int matchLimit = currentImage.Height / 3;
            int ignoreSideOffset = Math.Max(50, currentImage.Width / 20);

            if (currentImage.Width < ignoreSideOffset * 3)
            {
                ignoreSideOffset = 0;
            }

            Rectangle rect = new Rectangle(ignoreSideOffset, result.Height - currentImage.Height, currentImage.Width - ignoreSideOffset * 2, currentImage.Height);

            BitmapData bdResult = result.LockBits(new Rectangle(0, 0, result.Width, result.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bdCurrentImage = currentImage.LockBits(new Rectangle(0, 0, currentImage.Width, currentImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = bdResult.Stride;
            int pixelSize = stride / result.Width;
            IntPtr resultScan0 = bdResult.Scan0 + pixelSize * ignoreSideOffset;
            IntPtr currentImageScan0 = bdCurrentImage.Scan0 + pixelSize * ignoreSideOffset;
            int rectBottom = rect.Bottom - 1;
            int compareLength = pixelSize * rect.Width;

            for (int currentImageY = currentImage.Height - 1; currentImageY >= 0 && matchCount < matchLimit; currentImageY--)
            {
                int currentMatchCount = 0;

                for (int y = 0; currentImageY - y >= 0 && currentMatchCount < matchLimit; y++)
                {
                    if (NativeMethods.memcmp(resultScan0 + ((rectBottom - y) * stride), currentImageScan0 + ((currentImageY - y) * stride), compareLength) == 0)
                    {
                        currentMatchCount++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (currentMatchCount > matchCount)
                {
                    matchCount = currentMatchCount;
                    matchIndex = currentImageY;
                }
            }

            result.UnlockBits(bdResult);
            currentImage.UnlockBits(bdCurrentImage);

            if (matchCount == 0 && bestMatchCount > 0)
            {
                matchCount = bestMatchCount;
                matchIndex = bestMatchIndex;
            }

            if (matchCount > 0)
            {
                int matchHeight = currentImage.Height - matchIndex - 1;

                if (matchHeight > 0)
                {
                    if (matchCount > bestMatchCount)
                    {
                        bestMatchCount = matchCount;
                        bestMatchIndex = matchIndex;
                    }

                    Bitmap newResult = new Bitmap(result.Width, result.Height + matchHeight);

                    using (Graphics g = Graphics.FromImage(newResult))
                    {
                        g.DrawImage(result, new Rectangle(0, 0, result.Width, result.Height),
                            new Rectangle(0, 0, result.Width, result.Height), GraphicsUnit.Pixel);
                        g.DrawImage(currentImage, new Rectangle(0, result.Height, currentImage.Width, matchHeight),
                            new Rectangle(0, matchIndex + 1, currentImage.Width, matchHeight), GraphicsUnit.Pixel);
                    }

                    result.Dispose();
                    result = newResult;
                }
            }

            return result;
        }

        private void UploadResult()
        {
            if (Result != null)
            {
                OnUploadRequested((Bitmap)Result.Clone());
            }
        }

        protected void OnUploadRequested(Bitmap bmp)
        {
            UploadRequested?.Invoke(bmp);
        }

        private async void ScrollingCaptureLightForm_Load(object sender, EventArgs e)
        {
            await SelectWindow();
        }

        private async void btnCapture_Click(object sender, EventArgs e)
        {
            if (isCapturing)
            {
                StopCapture();
            }
            else
            {
                await SelectWindow();
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            UploadResult();
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {

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