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
using System.Diagnostics;
using System.Threading;

namespace ShareX.HelpersLib
{
    public class FPSManager
    {
        public event Action FPSUpdated;

        public int FPS { get; private set; }
        public int FPSLimit { get; set; }

        private int frameCount;
        private Stopwatch fpsTimer, frameTimer;

        public FPSManager()
        {
            fpsTimer = new Stopwatch();
            frameTimer = new Stopwatch();
        }

        public FPSManager(int fpsLimit) : this()
        {
            FPSLimit = fpsLimit;
        }

        protected void OnFPSUpdated()
        {
            FPSUpdated?.Invoke();
        }

        public void Update()
        {
            frameCount++;

            if (!fpsTimer.IsRunning)
            {
                fpsTimer.Start();
            }
            else if (fpsTimer.ElapsedMilliseconds >= 1000)
            {
                FPS = (int)Math.Round(frameCount / fpsTimer.Elapsed.TotalSeconds);

                OnFPSUpdated();

                frameCount = 0;
                fpsTimer.Restart();
            }

            if (FPSLimit > 0)
            {
                if (!frameTimer.IsRunning)
                {
                    frameTimer.Start();
                }
                else
                {
                    double currentFrameDuration = frameTimer.Elapsed.TotalMilliseconds;
                    double targetFrameDuration = 1000d / FPSLimit;

                    if (currentFrameDuration < targetFrameDuration)
                    {
                        int sleepDuration = (int)Math.Round(targetFrameDuration - currentFrameDuration);

                        if (sleepDuration > 0)
                        {
                            Thread.Sleep(sleepDuration);
                        }
                    }

                    frameTimer.Restart();
                }
            }
        }
    }
}