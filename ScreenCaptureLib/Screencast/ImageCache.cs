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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenCaptureLib
{
    public abstract class ImageCache : IDisposable
    {
        public bool IsWorking { get; protected set; }
        public AVIOptions Options { get; set; }

        protected Task task;
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

                task = TaskEx.Run(() =>
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
            }
        }

        protected abstract void WriteFrame(Image img);

        public void Finish()
        {
            if (IsWorking)
            {
                imageQueue.CompleteAdding();
                task.Wait();
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