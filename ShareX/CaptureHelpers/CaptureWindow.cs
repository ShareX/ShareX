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
using System.Linq;
using System;
using System.Threading.Tasks;

namespace ShareX
{
    public class CaptureWindow : CaptureBase
    {
        public IntPtr WindowHandle { get; protected set; }

        public CaptureWindow()
        {
        }

        public CaptureWindow(IntPtr windowHandle)
        {
            WindowHandle = windowHandle;

            IntPtr mainWindowHandle = MainWindowIntegration.WindowHandle;
            AllowAutoHideForm = mainWindowHandle == IntPtr.Zero || WindowHandle != mainWindowHandle;
        }

        protected override async Task<TaskMetadata> ExecuteAsync(TaskSettings taskSettings)
        {
            IWindowService windows = PlatformServices.Current.Windows;
            long handle = WindowHandle.ToInt64();

            if (windows.GetWindows().FirstOrDefault(x => x.Handle == handle)?.IsMinimized == true)
            {
                windows.RestoreWindow(handle);
                await Task.Delay(250);
            }

            if (windows.GetActiveWindowHandle() != handle)
            {
                windows.ActivateWindow(handle);
                await Task.Delay(100);
            }

            TaskMetadata metadata = new TaskMetadata();
            metadata.UpdateInfo(PlatformServices.Current.WindowManagement.GetDetails(handle));

            if (taskSettings.CaptureSettings.CaptureTransparent && !taskSettings.CaptureSettings.CaptureClientArea)
            {
                metadata.Image = await TaskHelpers.GetScreenshot(taskSettings).CaptureWindowTransparentAsync(WindowHandle);
            }
            else
            {
                metadata.Image = await TaskHelpers.GetScreenshot(taskSettings).CaptureWindowAsync(WindowHandle);
            }

            return metadata;
        }
    }
}
