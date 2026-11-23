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

namespace ShareX.ScreenRecordingLib.Audio;

/// <summary>
/// Copies a microphone that only feeds one channel of a stereo capture endpoint to both channels. Audio interface
/// drivers often expose their inputs only as stereo pairs, so a mono microphone on input 1 would otherwise be
/// recorded on the left side only. Stereo sources, and mono devices Windows already spreads over both channels,
/// pass through unchanged.
/// </summary>
internal sealed class MicrophoneMonoMixer
{
    private enum Layout { Stereo, Left, Right }

    // A channel is in use from -60 dBFS (RMS), and counts as unused while it stays 40 dB below the other channel.
    // Back to stereo only once the unused channel reaches 20 dB below the used one, so pauses never flip the layout.
    private const float ActivePower = 1e-6f, UnusedRatio = 1e-4f, StereoRatio = 1e-2f;
    private const float LevelWindow = AudioMixer.SampleRate * 0.25f;

    private float leftPower, rightPower;
    private Layout layout = Layout.Stereo;
    private float[] buffer = new float[1024 * 2];

    /// <summary>Returns the packet with its channels remapped. The result is only valid until the next call.</summary>
    public ReadOnlySpan<float> Process(ReadOnlySpan<float> packet)
    {
        int frames = packet.Length / 2;
        if (frames == 0) return packet;
        double left = 0, right = 0;
        for (int i = 0; i < frames * 2; i += 2)
        {
            left += packet[i] * packet[i];
            right += packet[i + 1] * packet[i + 1];
        }
        float weight = Math.Min(1, frames / LevelWindow);
        leftPower = Smooth(leftPower, (float)(left / frames), weight);
        rightPower = Smooth(rightPower, (float)(right / frames), weight);

        Layout previous = layout;
        layout = Detect();
        if (layout == Layout.Stereo && previous == Layout.Stereo) return packet;

        if (buffer.Length < frames * 2) Array.Resize(ref buffer, frames * 2);
        Span<float> output = buffer.AsSpan(0, frames * 2);
        for (int i = 0; i < frames; i++)
        {
            (float l, float r) = Map(layout, packet[i * 2], packet[i * 2 + 1]);
            if (previous != layout)
            {
                // Crossfade over the packet in which the layout changes, so the switch cannot click.
                (float pl, float pr) = Map(previous, packet[i * 2], packet[i * 2 + 1]);
                float t = (i + 1f) / frames;
                l = pl + (l - pl) * t;
                r = pr + (r - pr) * t;
            }
            output[i * 2] = l;
            output[i * 2 + 1] = r;
        }
        return output;
    }

    private Layout Detect()
    {
        if (layout == Layout.Stereo)
        {
            float louder = Math.Max(leftPower, rightPower), quieter = Math.Min(leftPower, rightPower);
            if (louder >= ActivePower && quieter <= louder * UnusedRatio) return leftPower > rightPower ? Layout.Left : Layout.Right;
            return Layout.Stereo;
        }
        float used = layout == Layout.Left ? leftPower : rightPower, unused = layout == Layout.Left ? rightPower : leftPower;
        return unused >= ActivePower && unused >= used * StereoRatio ? Layout.Stereo : layout;
    }

    private static float Smooth(float current, float target, float weight)
    {
        // A broken endpoint can deliver NaN or infinity; never let it stick in the level history.
        float next = current + (target - current) * weight;
        return float.IsFinite(next) ? next : 0;
    }

    private static (float Left, float Right) Map(Layout layout, float left, float right) => layout switch
    {
        Layout.Left => (left, left),
        Layout.Right => (right, right),
        _ => (left, right)
    };
}
