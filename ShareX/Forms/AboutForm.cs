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
using ShareX.Properties;
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
            lblProductName.Text = Program.Title;
            pbLogo.Image = ShareXResources.Logo;

            rtbShareXInfo.AddContextMenu();
            rtbCredits.AddContextMenu();

#if STEAM
            uclUpdate.Visible = false;
#else
            pbSteam.Visible = false;
            lblSteamBuild.Visible = false;

            if (!Program.IsPortableApps)
            {
                uclUpdate.CheckUpdate(TaskHelpers.CheckUpdate);
            }
            else
            {
                uclUpdate.Visible = false;
            }
#endif

            lblTeam.Text = "ShareX Team:";
            lblBerk.Text = "Jaex (Berk)";
            lblMike.Text = "mcored (Michael Delpach)";

            rtbShareXInfo.Text = string.Format(@"{0}: {1}
{2}: {3}
{4}: {5}
{6}: {7}",
Resources.AboutForm_AboutForm_Website, Links.URL_WEBSITE, Resources.AboutForm_AboutForm_Project_page, Links.URL_PROJECT, Resources.AboutForm_AboutForm_Issues, Links.URL_ISSUES,
Resources.AboutForm_AboutForm_Changelog, Links.URL_CHANGELOG);

            rtbCredits.Text = string.Format(@"{0}:

https://github.com/ShareX/ShareX/graphs/contributors

Mega, Gist and Jira support: https://github.com/gpailler
Web site: https://github.com/dmxt
Amazon S3 and DreamObjects support: https://github.com/alanedwardes
Gfycat support: https://github.com/Dinnerbone
AdFly support: https://github.com/LRNAB
MediaFire support: https://github.com/michalx2
Pushbullet support: https://github.com/BallisticLingonberries
Lambda support: https://github.com/mstojcevich
VideoBin support: https://github.com/corey-/
Up1 support: https://github.com/Upload
CoinURL, QRnet, VURL, 2gp, SomeImage, OneTimeSecret, Polr support: https://github.com/DanielMcAssey
Seafile support: https://github.com/zikeji
Streamable support: https://github.com/streamablevideo
s-ul support: https://github.com/corin12355
Imgland support: https://github.com/jibcore
Lithiio support: https://github.com/lithium720

{1}:

Turkish: https://github.com/Jaex & https://github.com/muratmoon
German: https://github.com/Starbug2 & https://github.com/Kaeltis
French: https://github.com/nwies & https://github.com/Shadorc
Simplified Chinese: https://github.com/jiajiechan
Hungarian: https://github.com/devBluestar
Korean: https://github.com/123jimin
Spanish: https://github.com/ovnisoftware
Dutch: https://github.com/canihavesomecoffee
Portuguese (Brazil): https://github.com/RockyTV & https://github.com/athosbr99
Vietnamese: https://github.com/thanhpd
Russian: https://github.com/L1Q

{2}:

Greenshot Image Editor: https://bitbucket.org/greenshot/greenshot
Json.NET: https://github.com/JamesNK/Newtonsoft.Json
SSH.NET: https://sshnet.codeplex.com
Icons: http://p.yusukekamiyamane.com
ImageListView: https://github.com/oozcitak/imagelistview
FFmpeg: http://www.ffmpeg.org
Zeranoe FFmpeg: http://ffmpeg.zeranoe.com/builds
7-Zip: http://www.7-zip.org
SevenZipSharp: https://sevenzipsharp.codeplex.com
DirectShow video and audio device: https://github.com/rdp/screen-capture-recorder-to-video-windows-free
QrCode.Net: https://qrcodenet.codeplex.com
System.Net.FtpClient: https://netftp.codeplex.com
AWS SDK: http://aws.amazon.com/sdk-for-net/
CLR Security: http://clrsecurity.codeplex.com
Steamworks.NET: https://github.com/rlabrecque/Steamworks.NET
OCR Space: http://ocr.space

Copyright (c) 2007-2016 ShareX Team", Resources.AboutForm_AboutForm_Contributors, Resources.AboutForm_AboutForm_Translators, Resources.AboutForm_AboutForm_External_libraries);
        }

        private void AboutForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void pbLogo_MouseDown(object sender, MouseEventArgs e)
        {
            cLogo.Start(50);
            pbLogo.Visible = false;
        }

        private void pbSteam_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.URL_STEAM);
        }

        private void pbBerkURL_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.URL_BERK);
        }

        private void pbMikeURL_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.URL_MIKE);
        }

        private void rtb_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            URLHelpers.OpenURL(e.LinkText);
        }

        private void btnShareXLicense_Click(object sender, EventArgs e)
        {
            Helpers.OpenFile(Helpers.GetAbsolutePath("Licenses\\ShareX_license.txt"));
        }

        private void btnLicenses_Click(object sender, EventArgs e)
        {
            Helpers.OpenFolder(Helpers.GetAbsolutePath("Licenses"));
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region Animation

        private const int w = 200;
        private const int h = w;
        private const int mX = w / 2;
        private const int mY = h / 2;
        private const int minStep = 3;
        private const int maxStep = 35;
        private const int speed = 1;
        private int step = 10;
        private int direction = speed;
        private Color lineColor = new HSB(0d, 1d, 0.9d);
        private bool isPaused;
        private int clickCount;

        private void cLogo_Draw(Graphics g)
        {
            g.SetHighQuality();

            using (Matrix m = new Matrix())
            {
                m.RotateAt(45, new PointF(mX, mY));
                g.Transform = m;
            }

            using (Pen pen = new Pen(lineColor, 2))
            {
                for (int i = 0; i <= mX; i += step)
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
            if (!isEasterEggStarted)
            {
                isPaused = !isPaused;

                clickCount++;

                if (clickCount >= 10)
                {
                    isEasterEggStarted = true;
                    RunEasterEgg();
                }
            }
            else
            {
                if (bounceTimer != null)
                {
                    bounceTimer.Stop();
                }

                isEasterEggStarted = false;
            }
        }

        #endregion Animation

        #region Easter egg

        private bool isEasterEggStarted;
        private Rectangle screenRect;
        private Timer bounceTimer;
        private const int windowGravityPower = 3;
        private const int windowBouncePower = -50;
        private const int windowSpeed = 20;
        private Point windowVelocity = new Point(windowSpeed, windowGravityPower);

        private void RunEasterEgg()
        {
            screenRect = CaptureHelpers.GetScreenWorkingArea();

            bounceTimer = new Timer();
            bounceTimer.Interval = 20;
            bounceTimer.Tick += bounceTimer_Tick;
            bounceTimer.Start();
        }

        private void bounceTimer_Tick(object sender, EventArgs e)
        {
            if (!IsDisposed)
            {
                int x = Left + windowVelocity.X;
                int windowRight = screenRect.X + screenRect.Width - 1 - Width;

                if (x <= screenRect.X)
                {
                    x = screenRect.X;
                    windowVelocity.X = windowSpeed;
                }
                else if (x >= windowRight)
                {
                    x = windowRight;
                    windowVelocity.X = -windowSpeed;
                }

                int y = Top + windowVelocity.Y;
                int windowBottom = screenRect.Y + screenRect.Height - 1 - Height;

                if (y >= windowBottom)
                {
                    y = windowBottom;
                    windowVelocity.Y = windowBouncePower.RandomAdd(-10, 10);
                }
                else
                {
                    windowVelocity.Y += windowGravityPower;
                }

                Location = new Point(x, y);
            }
        }

        #endregion Easter egg
    }
}