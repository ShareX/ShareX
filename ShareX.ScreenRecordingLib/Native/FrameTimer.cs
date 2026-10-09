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

using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
namespace ShareX.ScreenRecordingLib.Native;

/// <summary>Windows 11 high-resolution deadlines without changing system-wide timer resolution.</summary>
internal sealed unsafe class FrameTimer : WaitHandle
{
    public FrameTimer()
    {
        nint timer = NativeMethods.CreateWaitableTimerEx(null, null, 2, 0x001F0003);
        if (timer == 0) throw new Win32Exception();
        SafeWaitHandle = new SafeWaitHandle(timer, true);
    }

    public void Arm(long delayTicks)
    {
        long dueTime = -Math.Max(1, delayTicks); // relative 100 ns interval
        if (!NativeMethods.SetWaitableTimerEx(SafeWaitHandle.DangerousGetHandle(), &dueTime, 0, null, null, null, 0))
            throw new Win32Exception();
    }
}