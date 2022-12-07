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

using ShareX.HelpersLib;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX
{
    public sealed class EasterEggBounce : IDisposable
    {
        public Form Form { get; }
        public bool IsWorking { get; private set; }
        public Rectangle BounceRectangle { get; }
        public int Speed { get; set; } = 20;
        public bool ApplyGravity { get; set; } = true;
        public int GravityPower { get; set; } = 3;
        public int BouncePower { get; set; } = 50;

        private readonly Timer timer;
        private Point velocity;

        public EasterEggBounce(Form form)
        {
            var t = new Timer { Interval = 20 };
            t.Tick += bounceTimer_Tick;

            Form = form ?? throw new ArgumentNullException(nameof(form));
            timer = t;
            BounceRectangle = CaptureHelpers.GetScreenWorkingArea();
        }

        public void Start()
        {
            if (IsWorking) return;
            
            IsWorking = true;

            velocity = new Point(RandomFast.Pick(-Speed, Speed), ApplyGravity ? GravityPower : RandomFast.Pick(-Speed, Speed));
            timer.Start();
        }

        public void Stop()
        {
            if (!IsWorking) return;
            
            if (timer != null)
            {
                timer.Stop();
            }

            IsWorking = false;
        }

        private void bounceTimer_Tick(object sender, EventArgs e)
        {
            if (Form == null || Form.IsDisposed) return;
            
            int x = Form.Left + velocity.X;
            int windowRight = BounceRectangle.X + BounceRectangle.Width - Form.Width - 1;

            if (x <= BounceRectangle.X)
            {
                x = BounceRectangle.X;
                velocity.X = Speed;
            }
            else if (x >= windowRight)
            {
                x = windowRight;
                velocity.X = -Speed;
            }

            int y = Form.Top + velocity.Y;
            int windowBottom = BounceRectangle.Y + BounceRectangle.Height - Form.Height - 1;

            if (ApplyGravity)
            {
                if (y >= windowBottom)
                {
                    y = windowBottom;
                    velocity.Y = -BouncePower + RandomFast.Next(-10, 10);
                }
                else
                {
                    velocity.Y += GravityPower;
                }
            }
            else
            {
                if (y <= BounceRectangle.Y)
                {
                    y = BounceRectangle.Y;
                    velocity.Y = Speed;
                }
                else if (y >= windowBottom)
                {
                    y = windowBottom;
                    velocity.Y = -Speed;
                }
            }

            Form.Location = new Point(x, y);
        }

        public void Dispose()
        {
            Stop();

            if (timer != null)
            {
                timer.Dispose();
            }
        }
    }
}