#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

using HelpersLib;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenCapture
{
    public partial class RegionCapturePreview : Form
    {
        public Image Result
        {
            get
            {
                return result;
            }
            private set
            {
                result = value;

                if (result != null)
                {
                    pbResult.Image = result;
                    Text = string.Format("Region Capture: {0}x{1}", result.Width, result.Height);
                }
            }
        }

        public SurfaceOptions SurfaceConfig { get; set; }

        private Image screenshot;
        private Image result;

        public RegionCapturePreview()
            : this(new SurfaceOptions())
        {
        }

        public RegionCapturePreview(SurfaceOptions surfaceConfig)
        {
            InitializeComponent();

            screenshot = Screenshot.CaptureFullscreen();
            SurfaceConfig = surfaceConfig;
            pgSurfaceConfig.SelectedObject = SurfaceConfig;
        }

        private void CaptureRegion(Surface surface)
        {
            pbResult.Image = null;

            try
            {
                surface.Config = SurfaceConfig;
                surface.SurfaceImage = screenshot;
                surface.Prepare();
                surface.ShowDialog();

                if (surface.Result == SurfaceResult.Region)
                {
                    Result = surface.GetRegionImage();
                }
                else if (surface.Result == SurfaceResult.Fullscreen)
                {
                    Result = screenshot;
                }
            }
            finally
            {
                surface.Dispose();
            }
        }

        private void CaptureLastRegion()
        {
            if (Surface.LastRegionFillPath != null)
            {
                Result = ShapeCaptureHelpers.GetRegionImage(screenshot, Surface.LastRegionFillPath, Surface.LastRegionDrawPath, SurfaceConfig);
            }
            else
            {
                CaptureRegion(new RectangleRegion());
            }
        }

        private void tsbFullscreen_Click(object sender, EventArgs e)
        {
            Result = screenshot;
        }

        private void tsbWindowRectangle_Click(object sender, EventArgs e)
        {
            RectangleRegion rectangleRegion = new RectangleRegion();
            rectangleRegion.AreaManager.WindowCaptureMode = true;
            CaptureRegion(rectangleRegion);
        }

        private void tsbRectangle_Click(object sender, EventArgs e)
        {
            CaptureRegion(new RectangleRegion());
        }

        private void tsbRoundedRectangle_Click(object sender, EventArgs e)
        {
            CaptureRegion(new RoundedRectangleRegion());
        }

        private void tsbEllipse_Click(object sender, EventArgs e)
        {
            CaptureRegion(new EllipseRegion());
        }

        private void tsbTriangle_Click(object sender, EventArgs e)
        {
            CaptureRegion(new TriangleRegion());
        }

        private void tsbDiamond_Click(object sender, EventArgs e)
        {
            CaptureRegion(new DiamondRegion());
        }

        private void tsbPolygon_Click(object sender, EventArgs e)
        {
            CaptureRegion(new PolygonRegion());
        }

        private void tsbFreeHand_Click(object sender, EventArgs e)
        {
            CaptureRegion(new FreeHandRegion());
        }

        private void tsbLastRegion_Click(object sender, EventArgs e)
        {
            CaptureLastRegion();
        }

        private void RegionCapturePreview_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Result != null)
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                DialogResult = DialogResult.Cancel;
            }
        }

        private void btnClipboardCopy_Click(object sender, EventArgs e)
        {
            if (Result != null)
            {
                ClipboardHelpers.CopyImage(Result);
            }
        }
    }
}