#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class ScrollingCaptureForm : BaseForm
    {
        public ScrollingCaptureOptions Options { get; set; }

        private WindowInfo selectedWindow;
        private int currentScrollCount;
        private List<Image> images = new List<Image>();

        public ScrollingCaptureForm(ScrollingCaptureOptions options)
        {
            Options = options;
            InitializeComponent();
            nudScrollDelay.Value = Options.ScrollDelay;
            nudMaximumScrollCount.Value = Options.MaximumScrollCount;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                Clean();
            }

            base.Dispose(disposing);
        }

        private void btnSelectHandle_Click(object sender, EventArgs e)
        {
            Hide();
            SimpleWindowInfo simpleWindowInfo;

            try
            {
                Thread.Sleep(250);
                simpleWindowInfo = GetWindowInfo();
            }
            finally
            {
                Show();
            }

            if (simpleWindowInfo != null)
            {
                selectedWindow = new WindowInfo(simpleWindowInfo.Handle);
                lblControlText.Text = selectedWindow.ClassName ?? String.Empty;
                btnCapture.Enabled = true;
            }
            else
            {
                btnCapture.Enabled = false;
            }
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            StartCapture();
        }

        private SimpleWindowInfo GetWindowInfo()
        {
            using (RectangleRegion surface = new RectangleRegion())
            {
                surface.OneClickMode = true;
                surface.Config.ForceWindowCapture = true;
                surface.Config.IncludeControls = true;
                surface.Config.UseDimming = false;
                surface.Config.ShowInfo = true;
                surface.Config.ShowMagnifier = false;
                surface.Config.ShowTips = false;
                surface.Prepare();
                surface.ShowDialog();

                if (surface.Result == SurfaceResult.Region)
                {
                    return surface.SelectedWindow;
                }
            }

            return null;
        }

        private void StartCapture()
        {
            btnCapture.Enabled = false;
            Clean();
            selectedWindow.Activate();
            captureTimer.Interval = Options.ScrollDelay;
            captureTimer.Start();
        }

        private void StopCapture()
        {
            captureTimer.Stop();
            btnCapture.Enabled = true;
            this.ShowActivate();
            tcScrollingCapture.SelectedTab = tpOutput;
            pbOutput.Image = ImageHelpers.CombineImages(images);
        }

        private void Clean()
        {
            currentScrollCount = 0;

            if (images != null)
            {
                foreach (Image image in images)
                {
                    if (image != null)
                    {
                        image.Dispose();
                    }
                }

                images.Clear();
            }
        }

        private void captureTimer_Tick(object sender, EventArgs e)
        {
            Screenshot.CaptureCursor = false;
            Image image = Screenshot.CaptureRectangle(selectedWindow.Rectangle);

            if (image != null)
            {
                images.Add(image);
            }

            currentScrollCount++;

            if (currentScrollCount == Options.MaximumScrollCount || IsScrollReachedBottom(selectedWindow.Handle))
            {
                StopCapture();
            }

            NativeMethods.SendMessage(selectedWindow.Handle, (int)WindowsMessages.VSCROLL, (int)ScrollBarCommands.SB_PAGEDOWN, 0);
        }

        private void nudScrollDelay_ValueChanged(object sender, EventArgs e)
        {
            Options.ScrollDelay = (int)nudScrollDelay.Value;
        }

        private void nudMaximumScrollCount_ValueChanged(object sender, EventArgs e)
        {
            Options.MaximumScrollCount = (int)nudMaximumScrollCount.Value;
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

            if (images.Count > 1)
            {
                bool result = ImageHelpers.IsImagesEqual((Bitmap)images[images.Count - 1], (Bitmap)images[images.Count - 2]);

                if (result)
                {
                    Image last = images[images.Count - 1];
                    images.Remove(last);
                    last.Dispose();
                }

                return result;
            }

            return false;
        }
    }
}