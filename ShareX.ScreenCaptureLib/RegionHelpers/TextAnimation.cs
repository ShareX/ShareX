#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
    internal class TextAnimation
    {
        public string Text { get; private set; }

        private double opacity;

        public double Opacity
        {
            get
            {
                return opacity;
            }
            private set
            {
                opacity = value.Between(0, 1);
            }
        }

        public TimeSpan Duration { get; private set; }
        public TimeSpan FadeDuration { get; private set; }

        public TimeSpan TotalDuration
        {
            get
            {
                return Duration + FadeDuration;
            }
        }

        public bool Active
        {
            get
            {
                return timer.IsRunning && timer.Elapsed <= TotalDuration;
            }
        }

        private Stopwatch timer = new Stopwatch();

        public TextAnimation(TimeSpan duration, TimeSpan fadeDuration)
        {
            Duration = duration;
            FadeDuration = fadeDuration;
        }

        public void Start(string text)
        {
            Text = text;
            Opacity = 1;
            timer.Restart();
        }

        public void Update()
        {
            if (Active)
            {
                Opacity = 1 - (timer.Elapsed - Duration).TotalMilliseconds / FadeDuration.TotalMilliseconds;

                if (Opacity == 0)
                {
                    timer.Stop();
                }
            }
        }
    }
}