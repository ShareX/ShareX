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
using ShareX.Platform;
using ShareX.ScreenCaptureLib;
using ShareX.ScreenCaptureLib.Presentation.RegionCapture;
using SkiaSharp;
using System.Drawing;
using System.Threading.Tasks;
using Bitmap = SkiaSharp.SKBitmap;

namespace ShareX
{
    public class CaptureRegion : CaptureBase
    {
        protected override async Task<TaskMetadata> ExecuteAsync(TaskSettings taskSettings)
        {
            return await ExecuteRegionCaptureAvaloniaAsync(taskSettings);
        }

        protected async Task<TaskMetadata> ExecuteRegionCaptureAvaloniaAsync(TaskSettings taskSettings)
        {
            Screenshot screenshot = TaskHelpers.GetScreenshot(taskSettings);
            screenshot.CaptureCursor = false;

            bool activeMonitorMode = taskSettings.CaptureSettings.RegionCaptureOptions.ActiveMonitorMode;
            Rectangle screenBounds = activeMonitorMode
                ? CaptureHelpers.GetActiveScreenBounds()
                : CaptureHelpers.GetScreenBounds();

            SKBitmap frozenScreenshot = activeMonitorMode
                ? await screenshot.CaptureActiveMonitorAsync()
                : await screenshot.CaptureFullscreenAsync();

            if (frozenScreenshot == null)
            {
                return null;
            }

            if (screenBounds.IsEmpty)
            {
                // GNOME and KDE on Wayland do not reveal the screen layout; the capture covers the whole desktop.
                screenBounds = new Rectangle(0, 0, frozenScreenshot.Width, frozenScreenshot.Height);
            }

            SKBitmap cursorBitmap = null;
            Point cursorPosition = Point.Empty;

            // The cursor is drawn by the region capture editor so it can be moved or removed. Wayland does not reveal it.
            if (taskSettings.CaptureSettings.ShowCursor && PlatformServices.Current.ScreenCapture.CaptureCursor() is CursorCapture cursor)
            {
                cursorBitmap = PlatformImageConverter.ToSKBitmap(cursor.Image);
                cursorPosition = new Point(cursor.Position.X - screenBounds.X, cursor.Position.Y - screenBounds.Y);
            }

            AvaloniaRegionCaptureRequest request = new AvaloniaRegionCaptureRequest
            {
                Screenshot = frozenScreenshot,
                ScreenBounds = screenBounds,
                RegionCaptureOptions = taskSettings.CaptureSettingsReference.RegionCaptureOptions,
                ImageEditorOptions = taskSettings.ToolsSettingsReference.ImageEditorOptions,
                EnableAnnotations = !taskSettings.AdvancedSettings.RegionCaptureDisableAnnotation,
                CursorBitmap = cursorBitmap,
                CursorPosition = cursorPosition
            };

            AvaloniaRegionCaptureResult result = await RegionCaptureIntegration.CaptureAsync(request);
            if (result == null)
            {
                return null;
            }

            using (result.Image)
            {
                Bitmap output = result.Image.Copy();
                TaskMetadata metadata = new TaskMetadata(output);

                if (result.ImageModified)
                {
                    AllowAnnotation = false;
                }

                if (result.Window != null)
                {
                    metadata.UpdateInfo(result.Window);
                }

                return metadata;
            }
        }
    }
}
