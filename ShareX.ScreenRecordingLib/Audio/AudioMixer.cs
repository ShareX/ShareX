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
namespace ShareX.ScreenRecordingLib.Audio;

/// <summary>Two continuous stereo streams aligned to the video QPC timeline. Missing packets become silence.</summary>
internal sealed class AudioMixer(RecordingClock clock)
{
    private sealed class StreamState
    {
        public long Segment = long.MinValue;
        public long NextFrame;
        public double Phase, Step = 1;
        public float[] Input = new float[1024 * 2];
    }

    public const int SampleRate = 48_000;
    private const int Capacity = SampleRate * 2;
    private const int FilterTaps = 32, FilterDelay = FilterTaps / 2, FilterPhases = 128;
    private const double TimestampJitter = 2; // Ignore sub-42 microsecond packet timestamp variations.
    private static readonly float[][] interpolation = CreateInterpolation();
    private readonly object sync = new();
    private readonly float[][] samples = [new float[Capacity * 2], new float[Capacity * 2]];
    private readonly long[][] positions = [Enumerable.Repeat(long.MinValue, Capacity).ToArray(), Enumerable.Repeat(long.MinValue, Capacity).ToArray()];
    private readonly StreamState[] streams = [new(), new()];
    private long consumed;
    private long discontinuities;
    public long Discontinuities => Interlocked.Read(ref discontinuities);
    public void MarkDiscontinuity() => Interlocked.Increment(ref discontinuities);

    public void Write(int source, long timestamp, int frames, ReadOnlySpan<float> packet, float gain, bool discontinuity)
    {
        if (frames <= 0 || !clock.TryMap(timestamp, out long relative, out long segment)) return;
        double first = (double)relative * SampleRate / TimeSpan.TicksPerSecond;
        lock (sync)
        {
            StreamState stream = streams[source];
            double error = first - (stream.NextFrame - stream.Phase / stream.Step);
            if (stream.Segment != segment || discontinuity || Math.Abs(error) > SampleRate * 0.05)
            {
                // Only start, resume, or a genuine capture gap may reposition the stream.
                // Never turn ordinary QPC rounding/jitter into dropped or zero-filled samples.
                stream.Segment = segment;
                stream.NextFrame = (long)Math.Round(first);
                stream.Phase = 0;
                stream.Step = 1;
                stream.Input.AsSpan(0, FilterTaps * 2).Clear();
            }
            else
            {
                // Follow slow endpoint clock drift by changing the interpolation rate smoothly.
                // Two frames of deadband keep ordinary WASAPI timestamp jitter sample-exact.
                double correction = Math.CopySign(Math.Max(0, Math.Abs(error) - TimestampJitter), error);
                double targetStep = Math.Clamp(1 - correction / (SampleRate * 2), 0.995, 1.005);
                stream.Step += (targetStep - stream.Step) * Math.Min(1, (double)frames / (SampleRate * 0.5));
            }
            if (stream.Input.Length < (frames + FilterTaps) * 2)
                Array.Resize(ref stream.Input, (frames + FilterTaps) * 2);
            Span<float> input = stream.Input.AsSpan(FilterTaps * 2, frames * 2);
            if (packet.IsEmpty) input.Clear();
            else packet.CopyTo(input);
            double phase = stream.Phase;
            while (phase < frames - FilterDelay)
            {
                int index = (int)Math.Floor(phase);
                float fraction = (float)(phase - index);
                long position = stream.NextFrame++;
                if (position >= consumed && position < consumed + Capacity)
                {
                    float left, right;
                    int offset = (index + FilterTaps) * 2;
                    if (fraction == 0)
                    {
                        // Preserve the exact original samples when no rate correction is needed.
                        left = stream.Input[offset];
                        right = stream.Input[offset + 1];
                    }
                    else
                    {
                        // A windowed-sinc filter avoids the treble loss of linear interpolation.
                        float filterPhase = fraction * FilterPhases;
                        int filterIndex = Math.Min((int)filterPhase, FilterPhases - 1);
                        float blend = filterPhase - filterIndex;
                        float[] firstFilter = interpolation[filterIndex], nextFilter = interpolation[filterIndex + 1];
                        offset -= (FilterDelay - 1) * 2;
                        left = right = 0;
                        for (int tap = 0; tap < FilterTaps; tap++, offset += 2)
                        {
                            float weight = firstFilter[tap] + (nextFilter[tap] - firstFilter[tap]) * blend;
                            left += stream.Input[offset] * weight;
                            right += stream.Input[offset + 1] * weight;
                        }
                    }
                    int slot = (int)(position % Capacity);
                    positions[source][slot] = position;
                    samples[source][slot * 2] = left * gain;
                    samples[source][slot * 2 + 1] = right * gain;
                }
                phase += stream.Step;
            }
            // Retain filter history and lookahead across packet boundaries (333 microseconds at 48 kHz).
            stream.Phase = phase - frames;
            stream.Input.AsSpan(frames * 2, FilterTaps * 2).CopyTo(stream.Input);
        }
    }

    private static float[][] CreateInterpolation()
    {
        float[][] filters = new float[FilterPhases + 1][];
        for (int phase = 0; phase <= FilterPhases; phase++)
        {
            float[] filter = filters[phase] = new float[FilterTaps];
            double sum = 0;
            for (int tap = 0; tap < FilterTaps; tap++)
            {
                double distance = tap - (FilterDelay - 1) - (double)phase / FilterPhases;
                double sinc = distance == 0 ? 1 : Math.Sin(Math.PI * distance) / (Math.PI * distance);
                double window = 0.42 + 0.5 * Math.Cos(Math.PI * distance / FilterDelay) + 0.08 * Math.Cos(2 * Math.PI * distance / FilterDelay);
                filter[tap] = (float)(sinc * window);
                sum += filter[tap];
            }
            for (int tap = 0; tap < FilterTaps; tap++) filter[tap] /= (float)sum;
        }
        return filters;
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