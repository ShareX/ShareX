#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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
using System.Drawing;

namespace ShareX.ScreenCaptureLib
{
    public class ColorBlinkAnimation
    {
        public float Max { get; set; }
        public float Min { get; set; }
        public float Current { get; set; }
        public float Speed { get; set; }
        public Color FromColor { get; set; }
        public Color ToColor { get; set; }

        private Stopwatch timer;
        private TimeSpan previousTime;
        private int direction;

        public ColorBlinkAnimation()
        {
            Max = 1;
            Min = 0;
            Current = Min;
            Speed = 0.75f;
            FromColor = Color.FromArgb(30, 30, 30);
            ToColor = Color.FromArgb(100, 100, 100);

            timer = Stopwatch.StartNew();
            direction = 1;
        }

        public Color GetColor()
        {
            TimeSpan totalElapsed = timer.Elapsed;
            TimeSpan elapsed = totalElapsed - previousTime;
            previousTime = totalElapsed;

            Current += (float)elapsed.TotalSeconds * Speed * direction;

            if (Current > Max)
            {
                Current = Max; //Max - (Current - Max);
                direction = -1;
            }
            else if (Current < Min)
            {
                Current = Min; //Min + (Min - Current);
                direction = 1;
            }

            return ColorHelpers.Lerp(FromColor, ToColor, Current);
        }
    }
}