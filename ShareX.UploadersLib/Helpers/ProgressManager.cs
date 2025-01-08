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

using ShareX.HelpersLib;
using System;
using System.Diagnostics;
using System.Linq;

namespace ShareX.UploadersLib
{
    public class ProgressManager
    {
        public long Position { get; private set; }
        public long Length { get; private set; }

        public double Percentage => (double)Position / Length * 100;

        public double Speed { get; private set; }

        public TimeSpan Elapsed => startTimer.Elapsed;

        public TimeSpan Remaining
        {
            get
            {
                if (Speed > 0)
                {
                    return TimeSpan.FromSeconds((Length - Position) / Speed);
                }

                return TimeSpan.Zero;
            }
        }

        private Stopwatch startTimer = new Stopwatch();
        private Stopwatch smoothTimer = new Stopwatch();
        private int smoothTime = 250;
        private long speedTest;
        private FixedSizedQueue<double> averageSpeed = new FixedSizedQueue<double>(10);

        public ProgressManager(long length, long position = 0)
        {
            Length = length;
            Position = position;
            startTimer.Start();
            smoothTimer.Start();
        }

        public bool UpdateProgress(long bytesRead)
        {
            Position += bytesRead;
            speedTest += bytesRead;

            if (Position >= Length)
            {
                startTimer.Stop();

                return true;
            }

            if (smoothTimer.ElapsedMilliseconds > smoothTime)
            {
                averageSpeed.Enqueue(speedTest / smoothTimer.Elapsed.TotalSeconds);

                Speed = averageSpeed.Average();

                speedTest = 0;
                smoothTimer.Reset();
                smoothTimer.Start();

                return true;
            }

            return false;
        }
    }
}