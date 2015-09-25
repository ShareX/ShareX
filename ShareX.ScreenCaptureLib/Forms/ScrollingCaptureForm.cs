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

        public ScrollingCaptureForm(ScrollingCaptureOptions options)
        {
            Options = options;
            InitializeComponent();
            nudScrollDelay.Value = Options.ScrollDelay;
            nudMaximumScrollCount.Value = Options.MaximumScrollCount;
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
                lblControlText.Text = selectedWindow.ClassName ?? string.Empty;
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
            currentScrollCount = 0;
            selectedWindow.Activate();
            captureTimer.Interval = Options.ScrollDelay;
            captureTimer.Start();
        }

        private void StopCapture()
        {
            captureTimer.Stop();
            btnCapture.Enabled = true;
        }

        private void captureTimer_Tick(object sender, EventArgs e)
        {
            NativeMethods.SendMessage(selectedWindow.Handle, (int)WindowsMessages.VSCROLL, (int)ScrollBarCommands.SB_PAGEDOWN, 0);

            currentScrollCount++;

            if (currentScrollCount == Options.MaximumScrollCount)
            {
                StopCapture();
            }
        }

        private void nudScrollDelay_ValueChanged(object sender, EventArgs e)
        {
            Options.ScrollDelay = (int)nudScrollDelay.Value;
        }

        private void nudMaximumScrollCount_ValueChanged(object sender, EventArgs e)
        {
            Options.MaximumScrollCount = (int)nudMaximumScrollCount.Value;
        }
    }
}