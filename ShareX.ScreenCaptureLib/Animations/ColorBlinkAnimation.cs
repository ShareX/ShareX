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
using System.Drawing;

namespace ShareX.ScreenCaptureLib
{
    internal class ColorBlinkAnimation : BaseAnimation
    {
        public Color FromColor { get; set; }
        public Color ToColor { get; set; }
        public TimeSpan Duration { get; set; }

        public Color CurrentColor { get; set; }

        private bool backward;

        public override bool Update()
        {
            if (IsActive)
            {
                base.Update();

                float amount = (float)Timer.Elapsed.Ticks / Duration.Ticks;

                if (backward)
                {
                    amount = 1 - amount;
                }

                if (amount > 1)
                {
                    amount = 1;
                    backward = true;
                    Start();
                }
                else if (amount < 0)
                {
                    amount = 0;
                    backward = false;
                    Start();
                }

                CurrentColor = ColorHelpers.Lerp(FromColor, ToColor, amount);
            }

            return IsActive;
        }
    }
}