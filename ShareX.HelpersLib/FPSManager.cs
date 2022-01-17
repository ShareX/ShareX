#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2022 ShareX Team

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

namespace ShareX.HelpersLib
{
    public class FPSManager
    {
        public event Action FPSChanged;

        public int FPS { get; private set; }

        private int frameCount;
        private Stopwatch timer;

        public FPSManager()
        {
            timer = new Stopwatch();
        }

        protected void OnFPSChanged()
        {
            FPSChanged?.Invoke();
        }

        public void Update()
        {
            if (!timer.IsRunning)
            {
                timer.Start();
            }

            frameCount++;

            if (timer.ElapsedMilliseconds >= 1000)
            {
                FPS = (int)(frameCount / timer.Elapsed.TotalSeconds);

                OnFPSChanged();

                frameCount = 0;
                timer.Restart();
            }
        }
    }
}