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
using ShareX.Properties;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX
{
    public partial class AboutForm : Form
    {
        private EasterEggBounce easterEggBounce;

        public AboutForm()
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            lblProductName.Text = Program.Title;
            pbLogo.Image = ShareXResources.Logo;

            rtbShareXInfo.AddContextMenu();
            rtbCredits.AddContextMenu();

#if STEAM || WindowsStore
            uclUpdate.Visible = false;
            lblBuild.Visible = true;

            if (Program.Build == ShareXBuild.Steam)
            {
                lblBuild.Text = "Steam build";
            }
            else if (Program.Build == ShareXBuild.MicrosoftStore)
            {
                lblBuild.Text = "Microsoft Store build";
            }
#else
            if (!Program.PortableApps)
            {
                UpdateChecker updateChecker = Program.UpdateManager.CreateUpdateChecker();
                uclUpdate.CheckUpdate(updateChecker);
            }
            else
            {
                uclUpdate.Visible = false;
            }
#endif

            lblTeam.Text = "ShareX Team:";
            lblBerk.Text = "Jaex (Berk)";
            lblMike.Text = "McoreD (Michael Delpach)";

            rtbShareXInfo.Text = $@"{Resources.AboutForm_AboutForm_Website}: {Links.URL_WEBSITE}
{Resources.AboutForm_AboutForm_Project_page}: {Links.URL_PROJECT}
{Resources.AboutForm_AboutForm_Changelog}: {Links.URL_CHANGELOG}";

            rtbCredits.Text = $@"{Resources.AboutForm_AboutForm_Contributors}:

https://github.com/ShareX/ShareX/graphs/contributors

{Resources.AboutForm_AboutForm_Translators}:

{Resources.AboutForm_AboutForm_Language_tr}: https://github.com/Jaex & https://github.com/muratmoon
{Resources.AboutForm_AboutForm_Language_de}: https://github.com/Starbug2 & https://github.com/Kaeltis
{Resources.AboutForm_AboutForm_Language_fr}: https://github.com/nwies & https://github.com/Shadorc
{Resources.AboutForm_AboutForm_Language_zh_CH}: https://github.com/jiajiechan
{Resources.AboutForm_AboutForm_Language_hu}: https://github.com/devBluestar
{Resources.AboutForm_AboutForm_Language_ko_KR}: https://github.com/123jimin
{Resources.AboutForm_AboutForm_Language_es}: https://github.com/ovnisoftware
{Resources.AboutForm_AboutForm_Language_nl_NL}: https://github.com/canihavesomecoffee
{Resources.AboutForm_AboutForm_Language_pt_BR}: https://github.com/RockyTV & https://github.com/athosbr99
{Resources.AboutForm_AboutForm_Language_vi_VN}: https://github.com/thanhpd
{Resources.AboutForm_AboutForm_Language_ru}: https://github.com/L1Q
{Resources.AboutForm_AboutForm_Language_zh_TW}: https://github.com/alantsai
{Resources.AboutForm_AboutForm_Language_it_IT}: https://github.com/pjammo
{Resources.AboutForm_AboutForm_Language_uk}: https://github.com/6c6c6
{Resources.AboutForm_AboutForm_Language_id_ID}: https://github.com/Nicedward
{Resources.AboutForm_AboutForm_Language_es_MX}: https://github.com/absay

{Resources.AboutForm_AboutForm_External_libraries}:

Json.NET: https://github.com/JamesNK/Newtonsoft.Json
SSH.NET: https://github.com/sshnet/SSH.NET
Icons: http://p.yusukekamiyamane.com
ImageListView: https://github.com/oozcitak/imagelistview
FFmpeg: https://www.ffmpeg.org
Zeranoe FFmpeg: https://ffmpeg.zeranoe.com/builds
DirectShow video and audio device: https://github.com/rdp/screen-capture-recorder-to-video-windows-free
FluentFTP: https://github.com/robinrodricks/FluentFTP
Steamworks.NET: https://github.com/rlabrecque/Steamworks.NET
OCR Space: https://ocr.space
ZXing.Net: https://github.com/micjahn/ZXing.Net
MegaApiClient: https://github.com/gpailler/MegaApiClient
Blob Emoji: http://blobs.gg

Copyright (c) 2007-2018 ShareX Team";

            easterEggBounce = new EasterEggBounce(this);
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
            if (!easterEggBounce.IsWorking)
            {
                isPaused = !isPaused;

                clickCount++;

                if (clickCount >= 10)
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

        #endregion Animation
    }
}