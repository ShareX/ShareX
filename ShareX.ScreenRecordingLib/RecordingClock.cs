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
using System.Diagnostics;

namespace ShareX.ScreenRecordingLib;

/// <summary>WGC SystemRelativeTime and WASAPI QPCPosition both use the system QPC epoch, in 100 ns.</summary>
internal sealed class RecordingClock
{
    private readonly object sync = new();
    private long origin, pauseStarted, pausedTime, segmentStart, stoppedElapsed;
    private bool started, paused, stopped;

    public static long Now => (long)((Int128)Stopwatch.GetTimestamp() * TimeSpan.TicksPerSecond / Stopwatch.Frequency);
    public bool IsPaused { get { lock (sync) return paused; } }
    public long Elapsed { get { lock (sync) return stopped ? stoppedElapsed : started ? (paused ? pauseStarted : Now) - origin - pausedTime : 0; } }

    public void Start(long timestamp)
    {
        lock (sync) { origin = segmentStart = timestamp; started = true; }
    }

    public void Pause()
    {
        lock (sync)
        {
            if (started && !paused && !stopped) { pauseStarted = Now; paused = true; }
        }
    }

    public void Resume()
    {
        lock (sync)
        {
            if (paused && !stopped)
            {
                segmentStart = Now;
                pausedTime += segmentStart - pauseStarted;
                paused = false;
            }
        }
    }

    public void Stop()
    {
        lock (sync)
        {
            if (stopped) return;
            stoppedElapsed = started ? (paused ? pauseStarted : Now) - origin - pausedTime : 0;
            stopped = true;
            paused = false;
        }
    }

    public bool TryMap(long timestamp, out long relative, out long segment)
    {
        lock (sync)
        {
            relative = timestamp - origin - pausedTime;
            segment = segmentStart;
            return started && !paused && !stopped && timestamp >= segmentStart;
        }
    }
}