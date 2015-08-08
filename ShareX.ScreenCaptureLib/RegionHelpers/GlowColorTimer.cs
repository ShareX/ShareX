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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ShareX.ScreenCaptureLib
{
    public class GlowTimer
    {
        public float Max = 1;
        public float Min = 0;
        public float Current = 0;

        private Stopwatch timer;
        private TimeSpan previousTime;
        private int direction = 1;
        private float increase = 0.75f;

        public GlowTimer()
        {
            timer = Stopwatch.StartNew();
        }

        public void Update()
        {
            TimeSpan totalElapsed = timer.Elapsed;
            TimeSpan elapsed = totalElapsed - previousTime;
            previousTime = totalElapsed;

            Current += (float)elapsed.TotalSeconds * increase * direction;

            if (Current > Max)
            {
                Current = Max - (Current - Max);
                direction = -1;
            }
            else if (Current < Min)
            {
                Current = Min + (Min - Current);
                direction = 1;
            }
        }

        public Color GetColor()
        {
            return ColorHelpers.Lerp(Color.FromArgb(30, 30, 30), Color.FromArgb(100, 100, 100), Current);
        }
    }
}