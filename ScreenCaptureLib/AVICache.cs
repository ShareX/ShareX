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

using HelpersLib;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading.Tasks;

namespace ScreenCaptureLib
{
    public class AVICache : IDisposable
    {
        public bool IsWorking { get; private set; }
        public AVIOptions Options { get; set; }

        private AVIWriter aviWriter;
        private Task task;
        private BlockingCollection<Image> imageQueue;
        private int position;

        public AVICache(AVIOptions options)
        {
            Options = options;

            Helpers.CreateDirectoryIfNotExist(Options.OutputPath);
            aviWriter = new AVIWriter(Options);
            imageQueue = new BlockingCollection<Image>();
        }

        private void StartConsumerThread()
        {
            if (!IsWorking)
            {
                IsWorking = true;

                task = TaskEx.Run(() =>
                {
                    try
                    {
                        position = 0;

                        while (!imageQueue.IsCompleted)
                        {
                            Image img = null;

                            try
                            {
                                img = imageQueue.Take();

                                if (img != null)
                                {
                                    using (new DebugTimer("Frame saved"))
                                        aviWriter.AddFrame((Bitmap)img);

                                    position++;
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
            }
        }

        public void AddImageAsync(Image img)
        {
            if (!IsWorking)
            {
                StartConsumerThread();
            }

            /*if (imageQueue.Count > 0)
            {
                Debug.WriteLine("ImageQueue count: " + imageQueue.Count);
            }*/

            imageQueue.Add(img);
        }

        public void Finish()
        {
            if (IsWorking)
            {
                imageQueue.CompleteAdding();
                task.Wait();
            }

            Dispose();
        }

        public void Dispose()
        {
            if (aviWriter != null)
            {
                aviWriter.Dispose();
            }

            if (imageQueue != null)
            {
                imageQueue.Dispose();
            }
        }
    }
}