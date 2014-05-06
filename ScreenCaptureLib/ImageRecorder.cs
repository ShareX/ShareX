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
    public abstract class ImageRecorder : IDisposable
    {
        public bool IsWorking { get; protected set; }
        public AVIOptions Options { get; set; }

        protected Task task;
        protected BlockingCollection<Image> imageQueue;
        protected int position;

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

        protected abstract void StartConsumerThread();

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