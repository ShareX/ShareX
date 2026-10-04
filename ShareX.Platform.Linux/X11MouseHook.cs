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

using ShareX.Platform.Linux.Native;
using System;
using System.Diagnostics;
using System.Threading;

namespace ShareX.Platform.Linux;

/// <summary>
/// Follows the mouse across an X11 desktop by asking the server for the pointer 100 times a second on a thread with its own
/// connection. Unlike grabbing, polling never takes input away from other applications. Calls the listener on that thread.
/// </summary>
internal sealed class X11MouseHook : IDisposable
{
    private readonly IGlobalMouseListener listener;
    private readonly Thread thread;
    private volatile bool stopping;

    public X11MouseHook(IGlobalMouseListener listener)
    {
        this.listener = listener;
        X11Display probe = X11Display.TryOpen() ?? throw new PlatformNotSupportedException("No X server is available.");
        thread = new Thread(() => Run(probe)) { IsBackground = true, Name = "ShareX X11 mouse" };
        thread.Start();
    }

    private void Run(X11Display display)
    {
        using (display)
        {
            PlatformPoint last = new PlatformPoint(int.MinValue, int.MinValue);
            uint lastMask = 0;

            while (!stopping)
            {
                if (X11.XQueryPointer(display.Display, display.Root, out _, out _, out int x, out int y, out _, out _, out uint mask) != 0)
                {
                    PlatformPoint position = new PlatformPoint(x, y);

                    if (position != last)
                    {
                        listener.OnMove(position);
                        last = position;
                    }

                    foreach (GlobalMouseButtonEvent change in GetButtonChanges(lastMask, mask, position, Stopwatch.GetTimestamp()))
                    {
                        listener.OnButton(change);
                    }

                    lastMask = mask;
                }

                Thread.Sleep(10);
            }
        }
    }

    /// <summary>The presses and releases between two pointer button masks (left, middle and right).</summary>
    internal static System.Collections.Generic.IEnumerable<GlobalMouseButtonEvent> GetButtonChanges(uint previous, uint current, PlatformPoint position, long timestamp)
    {
        (uint Mask, GlobalMouseButton Button)[] buttons =
        [
            (X11.Button1Mask, GlobalMouseButton.Primary),
            (X11.Button2Mask, GlobalMouseButton.Middle),
            (X11.Button3Mask, GlobalMouseButton.Secondary)
        ];

        foreach ((uint mask, GlobalMouseButton button) in buttons)
        {
            bool was = (previous & mask) != 0;
            bool now = (current & mask) != 0;

            if (was != now)
            {
                yield return new GlobalMouseButtonEvent(button, now, position, timestamp);
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
