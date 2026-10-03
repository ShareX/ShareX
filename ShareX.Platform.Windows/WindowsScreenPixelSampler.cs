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

namespace ShareX.Platform.Windows;

/// <summary>Live GDI reads for the colour picker, without capturing its layered overlay.</summary>
internal sealed class WindowsScreenPixelSampler : IScreenPixelSampler
{
    private readonly Func<PlatformRectangle, PixelBuffer> capture;
    private bool disposed;

    internal WindowsScreenPixelSampler(PlatformRectangle bounds)
        : this(bounds, area => WindowsScreenCaptureService.Capture(area, false, false, false))
    {
    }

    internal WindowsScreenPixelSampler(PlatformRectangle bounds, Func<PlatformRectangle, PixelBuffer> capture)
    {
        Bounds = bounds;
        this.capture = capture;
    }

    public bool IsLive => true;
    public PixelBuffer? Snapshot => null;
    public PlatformRectangle Bounds { get; }

    public PixelBuffer? Read(PlatformRectangle area)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        if (area.IsEmpty) return null;

        PlatformRectangle clipped = area.Intersect(Bounds);
        PixelBuffer result = new(area.Width, area.Height);
        if (clipped.IsEmpty) return result;

        PixelBuffer pixels;
        try
        {
            pixels = capture(clipped);
        }
        catch (InvalidOperationException)
        {
            return null;
        }

        int offsetX = clipped.X - area.X;
        int offsetY = clipped.Y - area.Y;
        for (int y = 0; y < clipped.Height; y++)
        {
            Buffer.BlockCopy(pixels.Pixels, y * pixels.Stride, result.Pixels,
                (y + offsetY) * result.Stride + offsetX * 4, clipped.Width * 4);
        }

        return result;
    }

    public void Dispose() => disposed = true;
}
