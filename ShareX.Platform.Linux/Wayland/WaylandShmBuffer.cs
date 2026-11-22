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

#nullable enable

using System;
using System.Threading;

namespace ShareX.Platform.Linux.Wayland;

using static WaylandNative;

/// <summary>
/// A POSIX shared-memory backed <c>wl_buffer</c> (ARGB8888 premultiplied) for presenting frames or bitmap cursors to a
/// Wayland compositor without copying into intermediate PNGs.
/// </summary>
internal sealed unsafe class WaylandShmBuffer : IDisposable
{
    private static int nextShmId;

    private readonly IntPtr wlBuffer;
    private readonly IntPtr mappedPtr;
    private readonly nuint byteLength;
    private bool disposed;

    private WaylandShmBuffer(IntPtr wlBuffer, IntPtr mappedPtr, nuint byteLength, int width, int height, int stride)
    {
        this.wlBuffer = wlBuffer;
        this.mappedPtr = mappedPtr;
        this.byteLength = byteLength;
        Width = width;
        Height = height;
        Stride = stride;
    }

    public IntPtr WlBuffer => wlBuffer;

    public int Width { get; }

    public int Height { get; }

    public int Stride { get; }

    public bool Busy { get; set; }

    public static WaylandShmBuffer Create(IntPtr wlShm, uint shmVersion, int width, int height)
    {
        int stride = checked(width * 4);
        long size = checked((long)stride * height);
        int fd = CreateShmFile(size);

        IntPtr mapped = MAP_FAILED;
        try
        {
            mapped = Mmap(IntPtr.Zero, (nuint)size, PROT_READ | PROT_WRITE, MAP_SHARED, fd, 0);
            if (mapped == MAP_FAILED || mapped == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to mmap Wayland SHM buffer.");
            }

            WaylandProtocolInterfaces.InterfaceTable ifaces = WaylandProtocolInterfaces.All;
            WlArgument* poolArgs = stackalloc WlArgument[3];
            poolArgs[0].Pointer = IntPtr.Zero;
            poolArgs[1].Fd = fd;
            poolArgs[2].Int32 = checked((int)size);

            // wl_shm.create_pool (opcode 0)
            IntPtr pool = WlProxyMarshalArrayFlags(wlShm, 0, ifaces.WlShmPool, shmVersion, 0, poolArgs);
            if (pool == IntPtr.Zero)
            {
                throw new InvalidOperationException("wl_shm.create_pool failed.");
            }

            IntPtr buffer;
            try
            {
                uint poolVersion = WlProxyGetVersion(pool);
                WlArgument* bufArgs = stackalloc WlArgument[6];
                bufArgs[0].Pointer = IntPtr.Zero;
                bufArgs[1].Int32 = 0;
                bufArgs[2].Int32 = width;
                bufArgs[3].Int32 = height;
                bufArgs[4].Int32 = stride;
                bufArgs[5].UInt32 = WL_SHM_FORMAT_ARGB8888;

                // wl_shm_pool.create_buffer (opcode 0)
                buffer = WlProxyMarshalArrayFlags(pool, 0, ifaces.WlBuffer, poolVersion, 0, bufArgs);
                // wl_shm_pool.destroy (opcode 1) with WL_MARSHAL_FLAG_DESTROY
                WlProxyMarshalArrayFlags(pool, 1, null, poolVersion, WL_MARSHAL_FLAG_DESTROY, null);
            }
            catch
            {
                WlProxyDestroy(pool);
                throw;
            }

            if (buffer == IntPtr.Zero)
            {
                throw new InvalidOperationException("wl_shm_pool.create_buffer failed.");
            }

            return new WaylandShmBuffer(buffer, mapped, (nuint)size, width, height, stride);
        }
        catch
        {
            if (mapped != MAP_FAILED && mapped != IntPtr.Zero)
            {
                Munmap(mapped, (nuint)size);
            }

            throw;
        }
        finally
        {
            Close(fd);
        }
    }

    public void CopyFrom(byte[] source, int srcWidth, int srcHeight, int srcStride, PlatformRectangle slice)
    {
        fixed (byte* srcBase = source)
        {
            if (slice.Width == Width && slice.Height == Height)
            {
                int rowBytes = Math.Min(Stride, slice.Width * 4);
                for (int y = 0; y < Height; y++)
                {
                    byte* srcRow = srcBase + (long)(slice.Y + y) * srcStride + (long)slice.X * 4;
                    byte* dstRow = (byte*)mappedPtr + (long)y * Stride;
                    Buffer.MemoryCopy(srcRow, dstRow, Stride, rowBytes);
                }

                return;
            }

            // Fallback when wp_viewporter is unavailable and integer scaling differs from framebuffer slice.
            for (int y = 0; y < Height; y++)
            {
                int sy = Math.Clamp(slice.Y + y * slice.Height / Height, 0, srcHeight - 1);
                byte* srcRow = srcBase + (long)sy * srcStride;
                uint* dstRow = (uint*)((byte*)mappedPtr + (long)y * Stride);

                for (int x = 0; x < Width; x++)
                {
                    int sx = Math.Clamp(slice.X + x * slice.Width / Width, 0, srcWidth - 1);
                    dstRow[x] = ((uint*)srcRow)[sx];
                }
            }
        }
    }

    public void CopyFull(byte[] source, int srcStride)
    {
        fixed (byte* srcBase = source)
        {
            int rowBytes = Math.Min(Stride, srcStride);
            for (int y = 0; y < Height; y++)
            {
                Buffer.MemoryCopy(
                    srcBase + (long)y * srcStride,
                    (byte*)mappedPtr + (long)y * Stride,
                    Stride,
                    rowBytes);
            }
        }
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        if (wlBuffer != IntPtr.Zero)
        {
            // wl_buffer.destroy (opcode 0)
            WlProxyMarshalArrayFlags(wlBuffer, 0, null, WlProxyGetVersion(wlBuffer), WL_MARSHAL_FLAG_DESTROY, null);
        }

        if (mappedPtr != MAP_FAILED && mappedPtr != IntPtr.Zero)
        {
            Munmap(mappedPtr, byteLength);
        }
    }

    private static int CreateShmFile(long size)
    {
        int fd = MemfdCreate("sharex-wayland-overlay", MFD_CLOEXEC);
        if (fd >= 0)
        {
            if (Ftruncate(fd, size) == 0)
            {
                return fd;
            }

            Close(fd);
        }

        for (int attempt = 0; attempt < 8; attempt++)
        {
            string name = $"/sharex-wl-{Environment.ProcessId}-{Interlocked.Increment(ref nextShmId)}";
            int shmFd = ShmOpen(name, O_RDWR | O_CREAT | O_EXCL, 0x180);
            if (shmFd >= 0)
            {
                ShmUnlink(name);
                if (Ftruncate(shmFd, size) == 0)
                {
                    return shmFd;
                }

                Close(shmFd);
            }
        }

        throw new InvalidOperationException("Unable to allocate shared memory for Wayland overlay surface.");
    }
}
