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
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX
{
    public class EasterEggAboutAnimation : IDisposable
    {
        public Canvas Canvas { get; private set; }
        public bool IsPaused { get; set; }
        public Size Size { get; set; } = new Size(200, 200);
        public int Step { get; set; } = 10;
        public int MinStep { get; set; } = 3;
        public int MaxStep { get; set; } = 35;
        public int Speed { get; set; } = 1;
        public Color Color { get; set; } = new HSB(0.0, 1.0, 0.9);
        public int ClickCount { get; private set; }

        private EasterEggBounce easterEggBounce;
        private int direction;

        public EasterEggAboutAnimation(Canvas canvas, Form form)
        {
            Canvas = canvas;
            Canvas.MouseDown += Canvas_MouseDown;
            Canvas.Draw += Canvas_Draw;

            easterEggBounce = new EasterEggBounce(form);
        }

        public void Start()
        {
            direction = Speed;
            Canvas.Start(50);
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            IsPaused = !IsPaused;

            if (!easterEggBounce.IsWorking)
            {
                ClickCount++;

                if (ClickCount >= 10)
                {
                    easterEggBounce.ApplyGravity = e.Button == MouseButtons.Left;
                    easterEggBounce.Start();
                }
            }
            else
            {
                easterEggBounce.Stop();
            }

            TaskHelpers.PlayNotificationSoundAsync(NotificationSound.ActionCompleted);
        }

        private void Canvas_Draw(Graphics g)
        {
            g.SetHighQuality();

            int halfWidth = Size.Width / 2;
            int halfHeight = Size.Height / 2;

            using (Matrix m = new Matrix())
            {
                m.RotateAt(45, new PointF(halfWidth, halfHeight));
                g.Transform = m;
            }

            using (Pen pen = new Pen(Color, 2))
            {
                for (int i = 0; i <= halfWidth; i += Step)
                {
                    g.DrawLine(pen, i, halfHeight, halfWidth, halfHeight - i); // Left top
                    g.DrawLine(pen, halfWidth, i, halfWidth + i, halfHeight); // Right top
                    g.DrawLine(pen, Size.Width - i, halfHeight, halfWidth, halfHeight + i); // Right bottom
                    g.DrawLine(pen, halfWidth, Size.Height - i, halfWidth - i, halfHeight); // Left bottom

                    /*
                    g.DrawLine(pen, i, mY, mX, mY - i); // Left top
                    g.DrawLine(pen, w - i, mY, mX, mY - i); // Right top
                    g.DrawLine(pen, w - i, mY, mX, mY + i); // Right bottom
                    g.DrawLine(pen, i, mY, mX, mY + i); // Left bottom
                    */

                    /*
                    g.DrawLine(pen, mX, i, i, mY); // Left top
                    g.DrawLine(pen, mX, i, w - i, mY); // Right top
                    g.DrawLine(pen, mX, h - i, w - i, mY); // Right bottom
                    g.DrawLine(pen, mX, h - i, i, mY); // Left bottom
                    */
                }

                //g.DrawLine(pen, mX, 0, mX, h);
            }

            if (!IsPaused)
            {
                if (Step + Speed > MaxStep)
                {
                    direction = -Speed;
                }
                else if (Step - Speed < MinStep)
                {
                    direction = Speed;
                }

                Step += direction;

                HSB hsb = Color;

                if (hsb.Hue >= 1)
                {
                    hsb.Hue = 0;
                }
                else
                {
                    hsb.Hue += 0.01;
                }

                Color = hsb;
            }
        }

        public void Dispose()
        {
            if (easterEggBounce != null)
            {
                easterEggBounce.Dispose();
            }
        }
    }
}