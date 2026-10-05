// SPDX-License-Identifier: GPL-3.0-or-later
namespace ShareX.ScreenRecordingLib.Audio;

/// <summary>Two bounded stereo rings aligned to the video QPC timeline. Missing packets become silence.</summary>
internal sealed class AudioMixer(RecordingClock clock)
{
    public const int SampleRate = 48_000;
    private const int Capacity = SampleRate * 2;
    private readonly object sync = new();
    private readonly float[][] samples = [new float[Capacity * 2], new float[Capacity * 2]];
    private readonly long[][] positions = [Enumerable.Repeat(long.MinValue, Capacity).ToArray(), Enumerable.Repeat(long.MinValue, Capacity).ToArray()];
    private long consumed;
    private long discontinuities;
    public long Discontinuities => Interlocked.Read(ref discontinuities);
    public void MarkDiscontinuity() => Interlocked.Increment(ref discontinuities);

    public void Write(int source, long timestamp, ReadOnlySpan<float> packet, float gain)
    {
        if (!clock.TryMap(timestamp, out long relative)) return;
        long first = (long)((Int128)relative * SampleRate / TimeSpan.TicksPerSecond);
        lock (sync)
        {
            for (int i = 0; i < packet.Length / 2; i++)
            {
                long position = first + i;
                if (position < consumed || position >= consumed + Capacity) continue;
                int slot = (int)(position % Capacity);
                positions[source][slot] = position;
                samples[source][slot * 2] = packet[i * 2] * gain;
                samples[source][slot * 2 + 1] = packet[i * 2 + 1] * gain;
            }
        }
    }

    public void Read(long firstFrame, Span<short> destination)
    {
        lock (sync)
        {
            for (int i = 0; i < destination.Length / 2; i++)
            {
                long position = firstFrame + i;
                int slot = (int)(position % Capacity);
                for (int channel = 0; channel < 2; channel++)
                {
                    float value = 0;
                    for (int source = 0; source < 2; source++)
                        if (positions[source][slot] == position) value += samples[source][slot * 2 + channel];
                    // Clipping is deterministic, and NaN from a broken endpoint cannot propagate into PCM.
                    destination[i * 2 + channel] = float.IsFinite(value) ? (short)Math.Clamp((int)(value * 32767), -32768, 32767) : (short)0;
                }
            }
            consumed = firstFrame + destination.Length / 2;
        }
    }
}