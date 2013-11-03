#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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
using ShareX.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using UpdateCheckerLib;
using UploadersLib;

namespace ShareX
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            Text = Program.FullTitle;
            Icon = Resources.ShareX_Icon;
            lblProductName.Text = Program.FullTitle;
            rtbCredits.Text += Program.AssemblyCopyright;

            UpdateChecker updateChecker = new UpdateChecker(Links.URL_UPDATE, Application.ProductName, Program.AssemblyVersion,
                ReleaseChannelType.Stable, Uploader.ProxyInfo.GetWebProxy());
            uclUpdate.CheckUpdate(updateChecker);
        }

        private void AboutForm_Shown(object sender, EventArgs e)
        {
            BringToFront();
            Activate();

            cLogo.Interval = 50;
            cLogo.Start();
        }

        private void AboutForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cLogo.Stop();
        }

        private void lblProjectPage_Click(object sender, EventArgs e)
        {
            Helpers.LoadBrowserAsync(Links.URL_WEBSITE);
        }

        private void lblBugs_Click(object sender, EventArgs e)
        {
            Helpers.LoadBrowserAsync(Links.URL_ISSUES);
        }

        private void pbBerkURL_Click(object sender, EventArgs e)
        {
            Helpers.LoadBrowserAsync(Links.URL_BERK);
        }

        private void pbBerkSteamURL_Click(object sender, EventArgs e)
        {
            Helpers.LoadBrowserAsync(Links.URL_BERK_STEAM);
        }

        private void pbMikeURL_Click(object sender, EventArgs e)
        {
            Helpers.LoadBrowserAsync(Links.URL_MIKE);
        }

        private void pbGregoire_Click(object sender, EventArgs e)
        {
            Helpers.LoadBrowserAsync(Links.URL_GREGOIRE);
        }

        private void rtbCredits_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
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
        private Color lineColor = Color.Black;
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
            }
        }

        private void cLogo_MouseDown(object sender, MouseEventArgs e)
        {
            isPaused = !isPaused;
        }

        private void cLogo_MouseMove(object sender, MouseEventArgs e)
        {
            lineColor = new HSB((double)e.X / (w - 1), 1, 1.0 - (double)e.Y / (h - 1));
        }

        private void cLogo_MouseLeave(object sender, EventArgs e)
        {
            lineColor = Color.Black;
        }

        #endregion Animation
    }
}