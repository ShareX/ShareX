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

namespace ShareX.ScreenCaptureLib
{
    internal class OpacityAnimation : BaseAnimation
    {
        private double opacity;

        public double Opacity
        {
            get
            {
                return opacity;
            }
            private set
            {
                opacity = value.Clamp(0, 1);
            }
        }

        public TimeSpan FadeInDuration { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan FadeOutDuration { get; set; }

        public TimeSpan TotalDuration => FadeInDuration + Duration + FadeOutDuration;

        public override bool Update()
        {
            if (IsActive)
            {
                if (Timer.Elapsed < FadeInDuration)
                {
                    Opacity = Timer.Elapsed.TotalMilliseconds / FadeInDuration.TotalMilliseconds;
                }
                else
                {
                    Opacity = 1 - ((Timer.Elapsed - (FadeInDuration + Duration)).TotalMilliseconds / FadeOutDuration.TotalMilliseconds);
                }

                if (Opacity == 0)
                {
                    Timer.Stop();
                }
            }

            return IsActive;
        }
    }
}