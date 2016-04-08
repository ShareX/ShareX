using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

namespace ShareX.HelpersLib
{
    class ImageLoader
    {
        private bool Running = false;
        private Thread loadThread = null;
        private object loadLock = new object();

        private Queue<ImageLoaderTask> tasks = new Queue<ImageLoaderTask>();

        public void LoadImage(string filePath, Action<Image> callback)
        {
            lock (loadLock)
            {
                ImageLoaderTask task = new ImageLoaderTask(filePath, callback);
                tasks.Enqueue(task);
                if (!Running)
                {
                    Running = true;

                    loadThread = new Thread(new ThreadStart(thread_load));
                    loadThread.IsBackground = true;
                    loadThread.Start();
                }
            }
        }

        private void thread_load()
        {
            Stopwatch timeoutStopwatch = new Stopwatch();
            timeoutStopwatch.Start();

            while (Running)
            {
                lock(loadLock)
                {
                    if (tasks.Count != 0)
                    {
                        timeoutStopwatch.Restart();

                        ImageLoaderTask task = tasks.Dequeue();
                        task.Load();
                    }
                    else if (timeoutStopwatch.ElapsedMilliseconds > 10000)
                    {
                        Running = false;
                    }

                    Thread.Sleep(1); //Avoid high cpu usage
                }
            }
        }
    }
}
