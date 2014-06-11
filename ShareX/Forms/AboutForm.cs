#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

using HelpersLib;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            Text = Program.Title;
            lblProductName.Text = Program.Title;

            rtbShareXInfo.AddContextMenu();
            rtbCredits.AddContextMenu();

            uclUpdate.CheckUpdate(TaskHelpers.CheckUpdate);
        }

        private void AboutForm_Shown(object sender, EventArgs e)
        {
            this.ShowActivate();

            cLogo.Start(50);
        }

        private void lblProductName_Click(object sender, EventArgs e)
        {
            Helpers.OpenURL(Links.URL_VERSION_HISTORY);
        }

        private void pbBerkURL_Click(object sender, EventArgs e)
        {
            Helpers.OpenURL(Links.URL_BERK);
        }

        private void pbBerkSteamURL_Click(object sender, EventArgs e)
        {
            Helpers.OpenURL(Links.URL_BERK_STEAM);
        }

        private void pbMikeURL_Click(object sender, EventArgs e)
        {
            Helpers.OpenURL(Links.URL_MIKE);
        }

        private void pbMikeSteamURL_Click(object sender, EventArgs e)
        {
            Helpers.OpenURL(Links.URL_MIKE_STEAM);
        }

        private void rtb_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Helpers.OpenURL(e.LinkText);
        }

        #region Animation

        private const int w = 200;
        private const int h = w;
        private const int mX = w / 2;
        private const int mY = h / 2;
        private const int minStep = 3;
        private const int maxStep = 30;
        private const int speed = 1;
        private int step = 10;
        private int direction = speed;
        private Color lineColor = new HSB(0d, 1d, 0.9d);
        private bool isPaused;

        private void cLogo_Draw(Graphics g)
        {
            g.SetHighQuality();

            using (Matrix m = new Matrix())
            {
                m.RotateAt(45, new PointF(mX, mY));
                g.Transform = m;
            }

            using (Pen pen = new Pen(lineColor))
            {
                for (int i = 0; i <= mX; i += step)
                {
                    g.DrawLine(pen, i, mY, mX, mY + i); // Left top
                    g.DrawLine(pen, i, mY, mX, mY - i); // Left bottom
                    g.DrawLine(pen, w - i, mY, mX, mY - i); // Right top
                    g.DrawLine(pen, w - i, mY, mX, mY + i); // Right bottom
                }

                g.DrawLine(pen, mX, mY, mX, mY + mX); // Left top
                g.DrawLine(pen, mX, mY, mX, mY - mX); // Left bottom
                g.DrawLine(pen, w - mX, mY, mX, mY - mX); // Right top
                g.DrawLine(pen, w - mX, mY, mX, mY + mX); // Right bottom
            }

            if (!isPaused)
            {
                if (step + speed > maxStep)
                {
                    direction = -speed;
                }
                else if (step - speed < minStep)
                {
                    direction = speed;
                }

                step += direction;

                HSB hsb = lineColor;

                if (hsb.Hue >= 1)
                {
                    hsb.Hue = 0;
                }
                else
                {
                    hsb.Hue += 0.01;
                }

                lineColor = hsb;
            }
        }

        private void cLogo_MouseDown(object sender, MouseEventArgs e)
        {
            isPaused = !isPaused;
        }

        #endregion Animation
    }
}