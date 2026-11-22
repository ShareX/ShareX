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
using System.Diagnostics;
using System.Threading;

namespace ShareX.Platform.MacOS;

/// <summary>Waiting for a display to come back, for the time between a Mac waking and its screen being awake.</summary>
internal static class MacDisplayWait
{
    /// <summary>Polls <paramref name="isAvailable"/> until it is true or <paramref name="timeout"/> has passed.</summary>
    /// <param name="sleep">Pauses between polls; tests pass one that advances their clock.</param>
    /// <param name="elapsed">Time since the wait began.</param>
    public static bool WaitFor(Func<bool> isAvailable, TimeSpan timeout, TimeSpan pollInterval, Action<TimeSpan>? sleep = null, Func<TimeSpan>? elapsed = null)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        elapsed ??= () => stopwatch.Elapsed;
        sleep ??= Thread.Sleep;

        while (!isAvailable())
        {
            if (elapsed() >= timeout)
            {
                return false;
            }

            sleep(pollInterval);
        }

        return true;
    }
}
