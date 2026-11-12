#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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
using ShareX.ScreenCaptureLib.Presentation.RegionCapture;
using System.Drawing;
using System.Threading.Tasks;

namespace ShareX
{
    /// <summary>
    /// Region capture over the live desktop: the screen is not frozen while the region is drawn, and the region
    /// is captured after the selection window closes.
    /// </summary>
    public class CaptureRegionTransparent : CaptureBase
    {
        // Gives the compositor a frame to remove the selection window before the screen is captured.
        private const int CaptureDelay = 50;

        protected override TaskMetadata Execute(TaskSettings taskSettings)
        {
            return null;
        }

        protected override async Task<TaskMetadata> ExecuteAsync(TaskSettings taskSettings)
        {
            bool activeMonitorMode = taskSettings.CaptureSettings.RegionCaptureOptions.ActiveMonitorMode;
            Rectangle screenBounds = activeMonitorMode
                ? CaptureHelpers.GetActiveScreenBounds()
                : CaptureHelpers.GetScreenBounds();

            Rectangle? selection = await RegionCaptureIntegration.SelectTransparentRegionAsync(screenBounds, activeMonitorMode);

            if (selection is not Rectangle rect)
            {
                return null;
            }

            await Task.Delay(CaptureDelay);

            TaskMetadata metadata = CreateMetadata(rect);
            metadata.Image = TaskHelpers.GetScreenshot(taskSettings).CaptureRectangle(rect);
            return metadata;
        }
    }
}
