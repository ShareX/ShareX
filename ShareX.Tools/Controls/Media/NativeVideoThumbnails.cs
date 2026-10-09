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

using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.Runtime.InteropServices;
using Vortice.MediaFoundation;

namespace ShareX.Tools;

/// <summary>Uses one native decoder for a sparse filmstrip, independently of the playback engine.</summary>
internal static class NativeVideoThumbnails
{
    public static Task<IReadOnlyList<VideoTrimmerThumbnail>> CreateAsync(string path, double duration, CancellationToken token) =>
        Task.Run<IReadOnlyList<VideoTrimmerThumbnail>>(() =>
        {
            List<VideoTrimmerThumbnail> thumbnails = [];
            int com = CoInitializeEx(0, 0); // COINIT_MULTITHREADED; keep all reader operations on this worker.
            bool started = false;
            try
            {
                Marshal.ThrowExceptionForHR(com);
                token.ThrowIfCancellationRequested();
                MediaFactory.MFStartup(true).CheckError();
                started = true;
                using IMFAttributes attributes = MediaFactory.MFCreateAttributes(1);
                attributes.Set(SourceReaderAttributeKeys.EnableAdvancedVideoProcessing, 1u).CheckError();
                using IMFSourceReader reader = MediaFactory.MFCreateSourceReaderFromURL(path, attributes);
                reader.SetStreamSelection(SourceReaderIndex.AllStreams, false);
                reader.SetStreamSelection(SourceReaderIndex.FirstVideoStream, true);
                using IMFMediaType nativeType = reader.GetNativeMediaType(SourceReaderIndex.FirstVideoStream, 0);
                ulong size = nativeType.GetUInt64(MediaTypeAttributeKeys.FrameSize);
                int width = checked((int)(size >> 32)), height = checked((int)(size & uint.MaxValue));
                if (width <= 0 || height <= 0) throw new InvalidDataException();
                double scale = Math.Min(160d / width, 90d / height);
                int thumbnailWidth = Math.Max(2, (int)(width * scale) & ~1);
                int thumbnailHeight = Math.Max(2, (int)(height * scale) & ~1);
                using IMFMediaType outputType = MediaFactory.MFCreateMediaType();
                outputType.Set(MediaTypeAttributeKeys.MajorType, MediaTypeGuids.Video).CheckError();
                outputType.Set(MediaTypeAttributeKeys.Subtype, VideoFormatGuids.Rgb32).CheckError();
                outputType.Set(MediaTypeAttributeKeys.FrameSize, ((ulong)thumbnailWidth << 32) | (uint)thumbnailHeight).CheckError();
                outputType.Set(MediaTypeAttributeKeys.PixelAspectRatio, (1UL << 32) | 1).CheckError();
                reader.SetCurrentMediaType(SourceReaderIndex.FirstVideoStream, outputType);
                for (int i = 0; i < 12; i++)
                {
                    token.ThrowIfCancellationRequested();
                    double target = duration * i / 12;
                    reader.SetCurrentPosition((long)(target * 10000000));
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();
                        using IMFSample? sample = reader.ReadSample(SourceReaderIndex.FirstVideoStream, SourceReaderControlFlag.None,
                            out _, out SourceReaderFlag flags, out long timestamp);
                        if (sample != null && timestamp >= (long)(target * 10000000))
                        {
                            using IMFMediaType currentType = reader.GetCurrentMediaType(SourceReaderIndex.FirstVideoStream);
                            thumbnails.Add(new(target, CopyBitmap(sample, currentType)));
                            break;
                        }
                        if (flags.HasFlag(SourceReaderFlag.Error)) throw new InvalidDataException();
                        if (flags.HasFlag(SourceReaderFlag.EndOfStream)) break;
                    }
                }
                token.ThrowIfCancellationRequested();
                return thumbnails;
            }
            catch
            {
                foreach (var thumbnail in thumbnails) thumbnail.Image.Dispose();
                throw;
            }
            finally
            {
                if (started) MediaFactory.MFShutdown();
                if (com >= 0) CoUninitialize();
            }
        }, token);

    private static unsafe Bitmap CopyBitmap(IMFSample sample, IMFMediaType type)
    {
        ulong size = type.GetUInt64(MediaTypeAttributeKeys.FrameSize);
        int width = checked((int)(size >> 32)), height = checked((int)(size & uint.MaxValue));
        if (width is <= 0 or > 4096 || height is <= 0 or > 4096) throw new InvalidDataException();
        WriteableBitmap bitmap = new(new PixelSize(width, height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Opaque);
        try
        {
            using IMFMediaBuffer buffer = sample.ConvertToContiguousBuffer();
            using IMF2DBuffer? buffer2D = buffer.QueryInterfaceOrNull<IMF2DBuffer>();
            nint scanline;
            int stride;
            if (buffer2D != null) buffer2D.Lock2D(out scanline, out stride);
            else
            {
                stride = unchecked((int)type.GetUInt32(MediaTypeAttributeKeys.DefaultStride));
                buffer.Lock(out scanline, out _, out int length);
                if (Math.Abs((long)stride) * height > length)
                {
                    buffer.Unlock();
                    throw new InvalidDataException();
                }
                if (stride < 0) scanline += (height - 1) * -stride;
            }
            try
            {
                if (Math.Abs((long)stride) < width * 4) throw new InvalidDataException();
                using ILockedFramebuffer destination = bitmap.Lock();
                for (int y = 0; y < height; y++)
                {
                    new ReadOnlySpan<byte>((void*)(scanline + y * stride), width * 4).CopyTo(
                        new Span<byte>((void*)(destination.Address + y * destination.RowBytes), width * 4));
                }
            }
            finally
            {
                if (buffer2D != null) buffer2D.Unlock2D();
                else buffer.Unlock();
            }
            return bitmap;
        }
        catch { bitmap.Dispose(); throw; }
    }

    [DllImport("ole32.dll")]
    private static extern int CoInitializeEx(nint reserved, uint flags);
    [DllImport("ole32.dll")]
    private static extern void CoUninitialize();
}
