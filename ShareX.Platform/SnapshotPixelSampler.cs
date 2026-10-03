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

using ShareX.Platform.Imaging;
using System;

namespace ShareX.Platform;

/// <summary>Reads screen pixels, live or from a capture taken when the session started.</summary>
public interface IScreenPixelSampler : IDisposable
{
    /// <summary>True when every read shows the screen as it is now; false when reads come from <see cref="Snapshot"/>.</summary>
    bool IsLive { get; }

    /// <summary>The captured screen when <see cref="IsLive"/> is false, covering <see cref="Bounds"/>; otherwise null.</summary>
    PixelBuffer? Snapshot { get; }

    /// <summary>The desktop area the sampler covers, in screen coordinates.</summary>
    PlatformRectangle Bounds { get; }

    /// <summary>
    /// The pixels of <paramref name="area"/> (screen coordinates), one per screen coordinate. Parts outside <see cref="Bounds"/>
    /// are transparent. Null when the pixels cannot be read.
    /// </summary>
    PixelBuffer? Read(PlatformRectangle area);
}

/// <summary>Samples a full screen capture. Where the capture has more pixels than screen coordinates (scaled Wayland outputs), each
/// screen coordinate reads the nearest captured pixel.</summary>
public sealed class SnapshotPixelSampler : IScreenPixelSampler
{
    private readonly double scaleX;
    private readonly double scaleY;

    public SnapshotPixelSampler(ScreenCaptureResult capture)
        : this(capture.Pixels ?? PngCodec.Decode(capture.Png), capture.Bounds)
    {
    }

    public SnapshotPixelSampler(PixelBuffer snapshot, PlatformRectangle bounds)
    {
        Snapshot = snapshot;
        Bounds = bounds.IsEmpty ? new PlatformRectangle(0, 0, snapshot.Width, snapshot.Height) : bounds;
        scaleX = snapshot.Width / (double)Bounds.Width;
        scaleY = snapshot.Height / (double)Bounds.Height;
    }

    public bool IsLive => false;

    public PixelBuffer Snapshot { get; }

    PixelBuffer? IScreenPixelSampler.Snapshot => Snapshot;

    public PlatformRectangle Bounds { get; }

    public PixelBuffer? Read(PlatformRectangle area)
    {
        if (area.Width <= 0 || area.Height <= 0)
        {
            return null;
        }

        PixelBuffer result = new PixelBuffer(area.Width, area.Height);
        byte[] source = Snapshot.Pixels;
        byte[] target = result.Pixels;

        for (int y = 0; y < area.Height; y++)
        {
            int screenY = area.Y + y - Bounds.Y;
            int sourceY = (int)Math.Floor((screenY + 0.5) * scaleY);

            if (screenY < 0 || sourceY < 0 || sourceY >= Snapshot.Height)
            {
                continue;
            }

            for (int x = 0; x < area.Width; x++)
            {
                int screenX = area.X + x - Bounds.X;
                int sourceX = (int)Math.Floor((screenX + 0.5) * scaleX);

                if (screenX < 0 || sourceX < 0 || sourceX >= Snapshot.Width)
                {
                    continue;
                }

                Buffer.BlockCopy(source, (sourceY * Snapshot.Width + sourceX) * 4, target, (y * area.Width + x) * 4, 4);
            }
        }

        return result;
    }

    public void Dispose()
    {
    }
}
