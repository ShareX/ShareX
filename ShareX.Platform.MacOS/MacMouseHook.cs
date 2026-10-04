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

using ShareX.Platform.MacOS.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading;

namespace ShareX.Platform.MacOS;

/// <summary>
/// Follows the mouse by reading the pointer position and button state 100 times a second. Unlike an event tap this needs no
/// Accessibility or Input Monitoring permission and never delays other applications' input. Calls the listener on its own thread.
/// </summary>
[SupportedOSPlatform("macos")]
internal sealed class MacMouseHook : IDisposable
{
    private readonly IGlobalMouseListener listener;
    private readonly Thread thread;
    private volatile bool stopping;

    public MacMouseHook(IGlobalMouseListener listener)
    {
        this.listener = listener;
        thread = new Thread(Run) { IsBackground = true, Name = "ShareX macOS mouse" };
        thread.Start();
    }

    private void Run()
    {
        PlatformPoint last = new PlatformPoint(int.MinValue, int.MinValue);
        bool[] down = new bool[3];

        while (!stopping)
        {
            PlatformPoint position = GetPointerPosition();

            if (position != last)
            {
                listener.OnMove(position);
                last = position;
            }

            bool[] now =
            [
                CoreGraphics.CGEventSourceButtonState(CoreGraphics.kCGEventSourceStateCombinedSessionState, CoreGraphics.kCGMouseButtonLeft),
                CoreGraphics.CGEventSourceButtonState(CoreGraphics.kCGEventSourceStateCombinedSessionState, CoreGraphics.kCGMouseButtonCenter),
                CoreGraphics.CGEventSourceButtonState(CoreGraphics.kCGEventSourceStateCombinedSessionState, CoreGraphics.kCGMouseButtonRight)
            ];

            foreach (GlobalMouseButtonEvent change in GetButtonChanges(down, now, position, Stopwatch.GetTimestamp()))
            {
                listener.OnButton(change);
            }

            down = now;
            Thread.Sleep(10);
        }
    }

    /// <summary>The pointer in global display coordinates (points, origin at the top left of the main display).</summary>
    internal static PlatformPoint GetPointerPosition()
    {
        IntPtr evt = CoreGraphics.CGEventCreate(IntPtr.Zero);

        try
        {
            CoreGraphics.CGPoint point = CoreGraphics.CGEventGetLocation(evt);
            return new PlatformPoint((int)Math.Round(point.X), (int)Math.Round(point.Y));
        }
        finally
        {
            if (evt != IntPtr.Zero) CoreFoundation.CFRelease(evt);
        }
    }

    /// <summary>Presses and releases between two samples of the left, middle and right buttons.</summary>
    internal static IEnumerable<GlobalMouseButtonEvent> GetButtonChanges(bool[] previous, bool[] current, PlatformPoint position, long timestamp)
    {
        GlobalMouseButton[] buttons = [GlobalMouseButton.Primary, GlobalMouseButton.Middle, GlobalMouseButton.Secondary];

        for (int i = 0; i < buttons.Length; i++)
        {
            if (previous[i] != current[i])
            {
                yield return new GlobalMouseButtonEvent(buttons[i], current[i], position, timestamp);
            }
        }
    }

    public void Dispose()
    {
        stopping = true;

        if (Thread.CurrentThread != thread)
        {
            thread.Join(TimeSpan.FromSeconds(1));
        }
    }
}
