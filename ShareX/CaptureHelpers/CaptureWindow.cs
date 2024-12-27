#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2024 ShareX Team

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

using ShareX.HelpersLib.Native;

using System;
using System.Threading;

namespace ShareX.CaptureHelpers;

public class CaptureWindow : CaptureBase
{
    public IntPtr WindowHandle { get; protected set; }

    public CaptureWindow()
    {
    }

    public CaptureWindow(IntPtr windowHandle)
    {
        WindowHandle = windowHandle;

        AllowAutoHideForm = WindowHandle != Program.MainForm.Handle;
    }

    protected override TaskMetadata Execute(TaskSettings taskSettings)
    {
        WindowInfo windowInfo = new(WindowHandle);

        if (windowInfo.IsMinimized)
        {
            windowInfo.Restore();
            Thread.Sleep(250);
        }

        if (!windowInfo.IsActive)
        {
            windowInfo.Activate();
            Thread.Sleep(100);
        }

        TaskMetadata metadata = new();
        metadata.UpdateInfo(windowInfo);

        metadata.Image = taskSettings.CaptureSettings.CaptureTransparent && !taskSettings.CaptureSettings.CaptureClientArea
            ? TaskHelpers.GetScreenshot(taskSettings).CaptureWindowTransparent(WindowHandle)
            : TaskHelpers.GetScreenshot(taskSettings).CaptureWindow(WindowHandle);

        return metadata;
    }
}