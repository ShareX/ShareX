#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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

using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading;

namespace ShareX.ScreenCaptureLib
{
    public abstract class ImageCache : IDisposable
    {
        public bool IsWorking { get; protected set; }
        public ScreenRecordingOptions Options { get; set; }

        protected Thread task;
        protected BlockingCollection<Image> imageQueue;

        public ImageCache()
        {
            imageQueue = new BlockingCollection<Image>();
        }

        public void AddImageAsync(Image img)
        {
            if (!IsWorking)
            {
                StartConsumerThread();
            }

            imageQueue.Add(img);
        }

        protected virtual void StartConsumerThread()
        {
            if (!IsWorking)
            {
                IsWorking = true;

                task = new Thread(() =>
                {
                    try
                    {
                        while (!imageQueue.IsCompleted)
                        {
                            Image img = null;

                            try
                            {
                                img = imageQueue.Take();

                                if (img != null)
                                {
                                    //using (new DebugTimer("WriteFrame"))
                                    WriteFrame(img);
                                }
                            }
                            catch (InvalidOperationException)
                            {
                            }
                            finally
                            {
                                if (img != null) img.Dispose();
                            }
                        }
                    }
                    finally
                    {
                        IsWorking = false;
                    }
                });

                task.Start();
            }
        }

        protected abstract void WriteFrame(Image img);

        public void Finish()
        {
            if (IsWorking)
            {
                imageQueue.CompleteAdding();
                task.Join();
            }

            Dispose();
        }

        public virtual void Dispose()
        {
            if (imageQueue != null)
            {
                imageQueue.Dispose();
            }
        }
    }
}