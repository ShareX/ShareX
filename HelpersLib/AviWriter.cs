#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace HelpersLib
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
        // quality
        private int quality;
        // frame rate
        private int rate = 25;
        // current position
        private int position;
        // codec used for video compression
        private string codec = "DIB ";

        // dummy object to lock for synchronization
        private readonly object sync = new object();

        /// <summary>
        /// Width of video frames.
        /// </summary>
        ///
        /// <remarks><para>The property specifies the width of video frames, which are acceptable
        /// by <see cref="AddFrame"/> method for saving, which is set in <see cref="Open"/>
        /// method.</para></remarks>
        public int Width
        {
            get
            {
                return (buffer != IntPtr.Zero) ? width : 0;
            }
        }

        /// <summary>
        /// Height of video frames.
        /// </summary>
        ///
        /// <remarks><para>The property specifies the height of video frames, which are acceptable
        /// by <see cref="AddFrame"/> method for saving, which is set in <see cref="Open"/>
        /// method.</para></remarks>
        public int Height
        {
            get
            {
                return (buffer != IntPtr.Zero) ? height : 0;
            }
        }

        /// <summary>
        /// Current position in video stream.
        /// </summary>
        ///
        /// <remarks><para>The property tell current position in video stream, which actually equals
        /// to the amount of frames added using <see cref="AddFrame"/> method.</para></remarks>
        public int Position
        {
            get
            {
                return position;
            }
        }

        /// <summary>
        /// Desired playing frame rate.
        /// </summary>
        ///
        /// <remarks><para>The property sets the video frame rate, which should be use during playing
        /// of the video to be saved.</para>
        ///
        /// <para><note>The property should be set befor opening new file to take effect.</note></para>
        ///
        /// <para>Default frame rate is set to <b>25</b>.</para></remarks>
        public int FrameRate
        {
            get
            {
                return rate;
            }
            set
            {
                rate = value;
            }
        }

        /// <summary>
        /// Codec used for video compression.
        /// </summary>
        ///
        /// <remarks><para>The property sets the FOURCC code of video compression codec, which needs to
        /// be used for video encoding.</para>
        ///
        /// <para><note>The property should be set befor opening new file to take effect.</note></para>
        ///
        /// <para>Default video codec is set <b>"DIB "</b>, which means no compression.</para></remarks>
        public string Codec
        {
            get
            {
                return codec;
            }
            set
            {
                codec = value;
            }
        }

        /// <summary>
        /// Compression video quality.
        /// </summary>
        ///
        /// <remarks><para>The property sets video quality used by codec in order to balance compression rate
        /// and image quality. The quality is measured usually in the [0, 100] range.</para>
        ///
        /// <para><note>The property should be set befor opening new file to take effect.</note></para>
        ///
        /// <para>Default value is set to <b>-1</b> - default compression quality of the codec.</para></remarks>
        public int Quality
        {
            get
            {
                return quality;
            }
            set
            {
                quality = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AVIWriter"/> class.
        /// </summary>
        ///
        /// <remarks>Initializes Video for Windows library.</remarks>
        public AVIWriter()
        {
            NativeMethods.AVIFileInit();
        }

        public AVIWriter(string output, int fps, int width, int height, bool showOptions = false)
            : this()
        {
            FrameRate = fps;
            Open(output, width, height, showOptions);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AVIWriter"/> class.
        /// </summary>
        ///
        /// <param name="codec">Codec to use for compression.</param>
        ///
        /// <remarks>Initializes Video for Windows library.</remarks>
        public AVIWriter(string codec)
            : this()
        {
            this.codec = codec;
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
        public void Open(string fileName, int width, int height, bool showOptions = false)
        {
            // close previous file
            Close();

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
                    if (NativeMethods.AVIFileOpen(out file, fileName, OpenFileMode.Create | OpenFileMode.Write, IntPtr.Zero) != 0)
                        throw new IOException("Failed opening the specified file.");

                    this.width = width;
                    this.height = height;

                    // describe new stream
                    AVISTREAMINFO info = new AVISTREAMINFO();

                    info.type = NativeMethods.mmioFOURCC("vids");
                    info.handler = NativeMethods.mmioFOURCC(codec);
                    info.scale = 1;
                    info.rate = rate;
                    info.suggestedBufferSize = stride * height;

                    // create stream
                    if (NativeMethods.AVIFileCreateStream(file, out stream, ref info) != 0)
                        throw new Exception("Failed creating stream.");

                    // describe compression options
                    AVICOMPRESSOPTIONS options = new AVICOMPRESSOPTIONS();

                    options.handler = NativeMethods.mmioFOURCC(codec);
                    options.quality = quality;

                    if (showOptions)
                    {
                        NativeMethods.AVISaveOptions(stream, ref options);
                    }

                    // create compressed stream
                    if (NativeMethods.AVIMakeCompressedStream(out streamCompressed, stream, ref options, IntPtr.Zero) != 0)
                        throw new Exception("Failed creating compressed stream.");

                    // describe frame format
                    BITMAPINFOHEADER bitmapInfoHeader = new BITMAPINFOHEADER();

                    bitmapInfoHeader.size = Marshal.SizeOf(bitmapInfoHeader.GetType());
                    bitmapInfoHeader.width = width;
                    bitmapInfoHeader.height = height;
                    bitmapInfoHeader.planes = 1;
                    bitmapInfoHeader.bitCount = 24;
                    bitmapInfoHeader.sizeImage = 0;
                    bitmapInfoHeader.compression = 0; // BI_RGB

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