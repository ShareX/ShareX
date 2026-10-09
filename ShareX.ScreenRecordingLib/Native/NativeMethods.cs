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

// SPDX-License-Identifier: GPL-3.0-or-later
using System.Runtime.InteropServices;

namespace ShareX.ScreenRecordingLib.Native;

internal static unsafe partial class NativeMethods
{
    [DllImport("ole32.dll", ExactSpelling = true)]
    public static extern HRESULT CoInitializeEx(void* reserved, COINIT flags);

    [DllImport("ole32.dll", ExactSpelling = true)]
    public static extern void CoUninitialize();

    [DllImport("d3d11.dll", ExactSpelling = true)]
    public static extern HRESULT CreateDirect3D11DeviceFromDXGIDevice(nint device, nint* inspectable);

    [DllImport("combase.dll", ExactSpelling = true)]
    public static extern HRESULT WindowsCreateString(char* value, uint length, nint* result);

    [DllImport("combase.dll", ExactSpelling = true)]
    public static extern HRESULT WindowsDeleteString(nint value);

    [DllImport("combase.dll", ExactSpelling = true)]
    public static extern HRESULT RoGetActivationFactory(nint className, Guid* iid, void** factory);

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumDisplayMonitors(nint dc, RECT* clip, delegate* unmanaged[Stdcall]<nint, nint, RECT*, nint, BOOL> callback, nint state);

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindow(nint window);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern nint MonitorFromWindow(nint window, MONITOR_FROM_FLAGS flags);

    [DllImport("avrt.dll", EntryPoint = "AvSetMmThreadCharacteristicsW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern nint AvSetMmThreadCharacteristics(string taskName, ref uint taskIndex);

    [DllImport("avrt.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AvRevertMmThreadCharacteristics(nint task);

    [DllImport("kernel32.dll", EntryPoint = "CreateWaitableTimerExW", ExactSpelling = true, SetLastError = true)]
    public static extern nint CreateWaitableTimerEx(void* attributes, char* name, uint flags, uint access);

    [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWaitableTimerEx(nint timer, long* dueTime, int period, void* completionRoutine, void* argument, void* wakeContext, uint tolerableDelay);
}
