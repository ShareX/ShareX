// SPDX-License-Identifier: GPL-3.0-or-later
using System.ComponentModel;
using Microsoft.Win32.SafeHandles;
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