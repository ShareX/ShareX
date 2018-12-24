#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
        private const int w = 200;
        private const int h = w;
        private const int mX = w / 2;
        private const int mY = h / 2;
        private const int minStep = 3;
        private const int maxStep = 35;
        private const int speed = 1;

        public Canvas Canvas { get; private set; }
        public bool IsPaused { get; set; }
        public int Step { get; set; } = 10;
        public int Direction { get; set; } = speed;
        public Color Color { get; set; } = new HSB(0d, 1d, 0.9d);
        public int ClickCount { get; private set; }

        private EasterEggBounce easterEggBounce;

        public EasterEggAboutAnimation(Canvas canvas, Form form)
        {
            Canvas = canvas;
            Canvas.MouseDown += Canvas_MouseDown;
            Canvas.Draw += Canvas_Draw;

            easterEggBounce = new EasterEggBounce(form);
        }

        public void Start()
        {
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
        }

        private void Canvas_Draw(Graphics g)
        {
            g.SetHighQuality();

            using (Matrix m = new Matrix())
            {
                m.RotateAt(45, new PointF(mX, mY));
                g.Transform = m;
            }

            using (Pen pen = new Pen(Color, 2))
            {
                for (int i = 0; i <= mX; i += Step)
                {
                    g.DrawLine(pen, i, mY, mX, mY - i); // Left top
                    g.DrawLine(pen, mX, i, mX + i, mY); // Right top
                    g.DrawLine(pen, w - i, mY, mX, mY + i); // Right bottom
                    g.DrawLine(pen, mX, h - i, mX - i, mY); // Left bottom

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
                if (Step + speed > maxStep)
                {
                    Direction = -speed;
                }
                else if (Step - speed < minStep)
                {
                    Direction = speed;
                }

                Step += Direction;

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