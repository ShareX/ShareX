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

using System;
using System.Threading;

namespace ShareX.HelpersLib
{
    public enum OwnedWaitResult
    {
        Signaled,
        TimedOut,
        OwnerClosed
    }

    /// <summary>
    /// Waits on a handle that belongs to a window (or another owner) which may close and dispose it while the wait runs. Waits in
    /// short slices and stops when the owner reports it is closed or the handle is gone, instead of waiting forever.
    /// </summary>
    public static class OwnedWait
    {
        private const int SliceMilliseconds = 50;

        /// <param name="timeoutMilliseconds">Timeout.Infinite to wait until signaled or closed.</param>
        public static OwnedWaitResult Wait(WaitHandle handle, Func<bool> ownerClosed, int timeoutMilliseconds = Timeout.Infinite)
        {
            long deadline = timeoutMilliseconds == Timeout.Infinite ? long.MaxValue : Environment.TickCount64 + timeoutMilliseconds;

            while (true)
            {
                if (ownerClosed())
                {
                    return OwnedWaitResult.OwnerClosed;
                }

                long remaining = deadline - Environment.TickCount64;

                if (remaining <= 0)
                {
                    return OwnedWaitResult.TimedOut;
                }

                try
                {
                    if (handle.WaitOne((int)Math.Min(SliceMilliseconds, remaining)))
                    {
                        return OwnedWaitResult.Signaled;
                    }
                }
                catch (ObjectDisposedException)
                {
                    return OwnedWaitResult.OwnerClosed;
                }
            }
        }
    }
}
