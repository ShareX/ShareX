#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace ScreenCaptureLib
{
    public class HardDiskCache : IDisposable
    {
        public string CachePath { get; private set; }

        public int Count
        {
            get
            {
                if (indexList != null)
                {
                    return indexList.Count;
                }

                return 0;
            }
        }

        private List<LocationInfo> indexList;
        private BlockingCollection<Image> imageQueue;
        private bool isWorking;
        private Task task;

        public HardDiskCache(string cachePath)
        {
            CachePath = cachePath;
            indexList = new List<LocationInfo>();
            imageQueue = new BlockingCollection<Image>();
            StartConsumerThread();
        }

        private void StartConsumerThread()
        {
            if (!isWorking)
            {
                isWorking = true;

                task = TaskEx.Run(() =>
                {
                    Helpers.CreateDirectoryIfNotExist(CachePath);

                    using (FileStream fsCache = new FileStream(CachePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        while (!imageQueue.IsCompleted)
                        {
                            Image img = null;

                            try
                            {
                                img = imageQueue.Take();

                                if (img != null)
                                {
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        img.Save(ms, ImageFormat.Bmp);
                                        long position = fsCache.Position;
                                        ms.CopyStreamTo(fsCache);
                                        indexList.Add(new LocationInfo(position, fsCache.Length - position));
                                    }
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

                    isWorking = false;
                });
            }
        }

        public void AddImageAsync(Image img)
        {
            if (isWorking)
            {
                imageQueue.Add(img);
            }
        }

        public void Finish()
        {
            imageQueue.CompleteAdding();
            task.Wait();
        }

        public IEnumerable<Image> GetImageEnumerator()
        {
            if (!isWorking && File.Exists(CachePath) && indexList != null && indexList.Count > 0)
            {
                using (FileStream fsCache = new FileStream(CachePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    foreach (LocationInfo index in indexList)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            fsCache.CopyStreamTo(ms, (int)index.Location, (int)index.Length);
                            yield return Image.FromStream(ms);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            if (imageQueue != null)
            {
                imageQueue.Dispose();
            }
        }
    }
}