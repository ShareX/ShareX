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