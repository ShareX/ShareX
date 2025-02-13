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

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShareX.HelpersLib
{
    public class GraphicsQualityManager : IDisposable
    {
        private CompositingQuality previousCompositingQuality;
        private InterpolationMode previousInterpolationMode;
        private SmoothingMode previousSmoothingMode;
        private PixelOffsetMode previousPixelOffsetMode;
        private Graphics g;

        public GraphicsQualityManager(Graphics g, bool highQuality)
        {
            this.g = g;

            previousCompositingQuality = g.CompositingQuality;
            previousInterpolationMode = g.InterpolationMode;
            previousSmoothingMode = g.SmoothingMode;
            previousPixelOffsetMode = g.PixelOffsetMode;

            if (highQuality)
            {
                SetHighQuality();
            }
            else
            {
                SetLowQuality();
            }
        }

        public void SetHighQuality()
        {
            if (g != null)
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
            }
        }

        public void SetLowQuality()
        {
            if (g != null)
            {
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            }
        }

        public void Dispose()
        {
            if (g != null)
            {
                g.CompositingQuality = previousCompositingQuality;
                g.InterpolationMode = previousInterpolationMode;
                g.SmoothingMode = previousSmoothingMode;
                g.PixelOffsetMode = previousPixelOffsetMode;
            }
        }
    }
}