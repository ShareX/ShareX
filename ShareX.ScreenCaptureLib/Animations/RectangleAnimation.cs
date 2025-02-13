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
    internal class RectangleAnimation : BaseAnimation
    {
        public RectangleF FromRectangle { get; set; }
        public RectangleF ToRectangle { get; set; }
        public TimeSpan Duration { get; set; }

        public RectangleF CurrentRectangle { get; private set; }

        public override bool Update()
        {
            if (IsActive)
            {
                base.Update();

                float amount = (float)Timer.Elapsed.Ticks / Duration.Ticks;
                amount = Math.Min(amount, 1);

                float x = MathHelpers.Lerp(FromRectangle.X, ToRectangle.X, amount);
                float y = MathHelpers.Lerp(FromRectangle.Y, ToRectangle.Y, amount);
                float width = MathHelpers.Lerp(FromRectangle.Width, ToRectangle.Width, amount);
                float height = MathHelpers.Lerp(FromRectangle.Height, ToRectangle.Height, amount);

                CurrentRectangle = new RectangleF(x, y, width, height);

                if (amount >= 1)
                {
                    Stop();
                }
            }

            return IsActive;
        }
    }
}