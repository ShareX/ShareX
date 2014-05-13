#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

// AForge Video for Windows Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com

using HelpersLib;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ScreenCaptureLib
{
    /// <summary>
    /// AVI files writing using Video for Windows interface.
    /// </summary>
    ///
    /// <remarks><para>The class allows to write AVI files using Video for Windows API.</para>
    ///
    /// <para>Sample usage:</para>
    /// <code>
    /// // instantiate AVI writer, use WMV3 codec
    /// AVIWriter writer = new AVIWriter( "wmv3" );
    /// // create new AVI file and open it
    /// writer.Open( "test.avi", 320, 240 );
    /// // create frame image
    /// Bitmap image = new Bitmap( 320, 240 );
    ///
    /// for ( int i = 0; i &lt; 240; i++ )
    /// {
    ///     // update image
    ///     image.SetPixel( i, i, Color.Red );
    ///     // add the image as a new frame of video file
    ///     writer.AddFrame( image );
    /// }
    /// writer.Close( );
    /// </code>
    /// </remarks>
    public class AVIWriter : IDisposable
    {
        // AVI file
        private IntPtr file;
        // video stream
        private IntPtr stream;
        // compressed stream
        private IntPtr streamCompressed;
        // buffer
        private IntPtr buffer = IntPtr.Zero;

        // width of video frames
        private int width;
        // height of vide frames
        private int height;
        // length of one line
        private int stride;
        // frame rate
        private int rate = 25;
        // current position
        private int position;

        // dummy object to lock for synchronization
        private readonly object sync = new object();

        public ScreencastOptions Options { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AVIWriter"/> class.
        /// </summary>
        ///
        /// <remarks>Initializes Video for Windows library.</remarks>
        public AVIWriter(ScreencastOptions options)
        {
            NativeMethods.AVIFileInit();
            Options = options;
            Open();
        }

        /// <summary>
        /// Destroys the instance of the <see cref="AVIWriter"/> class.
        /// </summary>
        ~AVIWriter()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose the object.
        /// </summary>
        ///
        /// <remarks>Frees unmanaged resources used by the object. The object becomes unusable
        /// after that.</remarks>
        public void Dispose()
        {
            Dispose(true);
            // remove me from the Finalization queue
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the object.
        /// </summary>
        ///
        /// <param name="disposing">Indicates if disposing was initiated manually.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
            }
            // close current AVI file if any opened and uninitialize AVI library
            Close();
            NativeMethods.AVIFileExit();
        }

        /// <summary>
        /// Create new AVI file and open it for writing.
        /// </summary>
        ///
        /// <param name="fileName">AVI file name to create.</param>
        /// <param name="width">Video width.</param>
        /// <param name="height">Video height.</param>
        ///
        /// <remarks><para>The method opens (creates) a video files, configure video codec and prepares
        /// the stream for saving video frames with a help of <see cref="AddFrame"/> method.</para></remarks>
        ///
        /// <exception cref="System.IO.IOException">Failed opening the specified file.</exception>
        /// <exception cref="VideoException">A error occurred while creating new video file. See exception message.</exception>
        /// <exception cref="OutOfMemoryException">Insufficient memory for internal buffer.</exception>
        /// <exception cref="ArgumentException">Video file resolution must be a multiple of two.</exception>
        public void Open()
        {
            // close previous file
            Close();

            this.width = Options.CaptureArea.Width;
            this.height = Options.CaptureArea.Height;

            // check width and height
            if (((width & 1) != 0) || ((height & 1) != 0))
            {
                throw new ArgumentException("Video file resolution must be a multiple of two.");
            }

            bool success = false;

            try
            {
                lock (sync)
                {
                    // calculate stride
                    stride = width * 3;
                    if ((stride % 4) != 0)
                        stride += (4 - stride % 4);

                    // create new file
                    if (NativeMethods.AVIFileOpen(out file, Options.OutputPath, OpenFileMode.Create | OpenFileMode.Write, IntPtr.Zero) != 0)
                        throw new IOException("Failed opening the specified file.");

                    this.rate = Options.ScreenRecordFPS;

                    // describe new stream
                    AVISTREAMINFO info = new AVISTREAMINFO();

                    info.type = NativeMethods.mmioFOURCC("vids");
                    info.handler = NativeMethods.mmioFOURCC("DIB ");
                    info.scale = 1;
                    info.rate = rate;
                    info.suggestedBufferSize = stride * height;

                    // create stream
                    if (NativeMethods.AVIFileCreateStream(file, out stream, ref info) != 0)
                        throw new Exception("Failed creating stream.");

                    if (Options.AVI.CompressOptions.handler == 0)
                    {
                        // describe compression options
                        Options.AVI.CompressOptions.handler = NativeMethods.mmioFOURCC("DIB ");
                    }

                    if (Options.ShowAVIOptionsDialog)
                    {
                        AVICOMPRESSOPTIONS options = new AVICOMPRESSOPTIONS();
                        options.handler = Options.AVI.CompressOptions.handler;
                        options.quality = Options.AVI.CompressOptions.quality;
                        options.flags = 8; // AVICOMPRESSF_VALID
                        int result = NativeMethods.AVISaveOptions(stream, ref options, Options.ParentWindow);
                        if (result == 1)
                        {
                            Options.AVI.CompressOptions = options;
                        }
                    }

                    // create compressed stream
                    if (NativeMethods.AVIMakeCompressedStream(out streamCompressed, stream, ref Options.AVI.CompressOptions, IntPtr.Zero) != 0)
                        throw new Exception("Failed creating compressed stream.");

                    // describe frame format
                    BITMAPINFOHEADER bitmapInfoHeader = new BITMAPINFOHEADER(width, height, 24);

                    // set frame format
                    if (NativeMethods.AVIStreamSetFormat(streamCompressed, 0, ref bitmapInfoHeader, Marshal.SizeOf(bitmapInfoHeader.GetType())) != 0)
                        throw new Exception("Failed setting format of the compressed stream.");

                    // alloc unmanaged memory for frame
                    buffer = Marshal.AllocHGlobal(stride * height);

                    if (buffer == IntPtr.Zero)
                    {
                        throw new OutOfMemoryException("Insufficient memory for internal buffer.");
                    }

                    position = 0;
                    success = true;
                }
            }
            finally
            {
                if (!success)
                {
                    Close();
                }
            }
        }

        /// <summary>
        /// Close video file.
        /// </summary>
        public void Close()
        {
            lock (sync)
            {
                // free unmanaged memory
                if (buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buffer);
                    buffer = IntPtr.Zero;
                }

                // release compressed stream
                if (streamCompressed != IntPtr.Zero)
                {
                    NativeMethods.AVIStreamRelease(streamCompressed);
                    streamCompressed = IntPtr.Zero;
                }

                // release stream
                if (stream != IntPtr.Zero)
                {
                    NativeMethods.AVIStreamRelease(stream);
                    stream = IntPtr.Zero;
                }

                // release file
                if (file != IntPtr.Zero)
                {
                    NativeMethods.AVIFileRelease(file);
                    file = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// Add new frame to the AVI file.
        /// </summary>
        ///
        /// <param name="frameImage">New frame image.</param>
        ///
        /// <remarks><para>The method adds new video frame to an opened video file. The width and heights
        /// of the frame should be the same as it was specified in <see cref="Open"/> method
        /// (see <see cref="Width"/> and <see cref="Height"/> properties).</para></remarks>
        ///
        /// <exception cref="System.IO.IOException">Thrown if no video file was open.</exception>
        /// <exception cref="ArgumentException">Bitmap size must be of the same as video size, which was specified on opening video file.</exception>
        /// <exception cref="VideoException">A error occurred while writing new video frame. See exception message.</exception>
        public void AddFrame(Bitmap frameImage)
        {
            lock (sync)
            {
                // check if AVI file was properly opened
                if (buffer == IntPtr.Zero)
                    throw new IOException("AVI file should be successfully opened before writing.");

                // check image dimension
                if ((frameImage.Width != width) || (frameImage.Height != height))
                    throw new ArgumentException("Bitmap size must be of the same as video size, which was specified on opening video file.");

                // lock bitmap data
                BitmapData imageData = frameImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                // copy image data
                int srcStride = imageData.Stride;
                int dstStride = stride;

                int src = imageData.Scan0.ToInt32() + srcStride * (height - 1);
                int dst = buffer.ToInt32();

                for (int y = 0; y < height; y++)
                {
                    NativeMethods.memcpy(dst, src, dstStride);
                    dst += dstStride;
                    src -= srcStride;
                }

                // unlock bitmap data
                frameImage.UnlockBits(imageData);

                // write to stream
                if (NativeMethods.AVIStreamWrite(streamCompressed, position, 1, buffer, stride * height, 0, IntPtr.Zero, IntPtr.Zero) != 0)
                {
                    throw new Exception("Failed adding frame.");
                }

                position++;
            }
        }
    }
}