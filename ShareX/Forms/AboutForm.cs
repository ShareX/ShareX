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
using ShareX.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX
{
    public partial class AboutForm : Form
    {
        private EasterEggAboutAnimation easterEgg;
        private bool checkUpdate = false;

        public AboutForm()
        {
            InitializeComponent();
            lblProductName.Text = Program.Title;
            pbLogo.Image = ShareXResources.Logo;
            ShareXResources.ApplyTheme(this, true);

#if STEAM
            uclUpdate.Visible = false;
            lblBuild.Text = "Steam build";
            lblBuild.Visible = true;
#elif MicrosoftStore
            uclUpdate.Visible = false;
            lblBuild.Text = "Microsoft Store build";
            lblBuild.Visible = true;
#else
            if (!SystemOptions.DisableUpdateCheck)
            {
                uclUpdate.UpdateLoadingImage();
                checkUpdate = true;
            }
            else
            {
                uclUpdate.Visible = false;
            }
#endif

            rtbInfo.AppendLine(Resources.AboutForm_AboutForm_Links, FontStyle.Bold, 13);
            rtbInfo.AppendLine($@"{Resources.AboutForm_AboutForm_Website}: {Links.Website}
{Resources.AboutForm_AboutForm_Project_page}: {Links.GitHub}
{Resources.AboutForm_AboutForm_Changelog}: {Links.Changelog}
{Resources.AboutForm_AboutForm_Privacy_policy}: {Links.PrivacyPolicy}
", FontStyle.Regular);

            rtbInfo.AppendLine(Resources.AboutForm_AboutForm_Team, FontStyle.Bold, 13);
            rtbInfo.AppendLine($@"Jaex: {Links.Jaex}
McoreD: {Links.McoreD}
", FontStyle.Regular);

            rtbInfo.AppendLine(Resources.AboutForm_AboutForm_Translators, FontStyle.Bold, 13);
            rtbInfo.AppendLine($@"{Resources.AboutForm_AboutForm_Language_tr}: https://github.com/Jaex
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
{Resources.AboutForm_AboutForm_Language_fa_IR}: https://github.com/pourmand1376
{Resources.AboutForm_AboutForm_Language_pt_PT}: https://github.com/FarewellAngelina
{Resources.AboutForm_AboutForm_Language_ja_JP}: https://github.com/kanaxx
{Resources.AboutForm_AboutForm_Language_ro}: https://github.com/Edward205
{Resources.AboutForm_AboutForm_Language_pl}: https://github.com/RikoDEV
{Resources.AboutForm_AboutForm_Language_he_IL}: https://github.com/erelado
{Resources.AboutForm_AboutForm_Language_ar_YE}: https://github.com/OthmanAliModaes
", FontStyle.Regular);

            rtbInfo.AppendLine(Resources.AboutForm_AboutForm_Credits, FontStyle.Bold, 13);
            rtbInfo.AppendLine(@"Json.NET: https://github.com/JamesNK/Newtonsoft.Json
SSH.NET: https://github.com/sshnet/SSH.NET
Icons: http://p.yusukekamiyamane.com
ImageListView: https://github.com/oozcitak/imagelistview
FFmpeg: https://www.ffmpeg.org
Recorder devices: https://github.com/rdp/screen-capture-recorder-to-video-windows-free
FluentFTP: https://github.com/robinrodricks/FluentFTP
ZXing.Net: https://github.com/micjahn/ZXing.Net
MegaApiClient: https://github.com/gpailler/MegaApiClient
Inno Setup Dependency Installer: https://github.com/DomGries/InnoDependencyInstaller
Blob Emoji: http://blobs.gg
", FontStyle.Regular);

            rtbInfo.AppendText("Copyright (c) 2007-2025 ShareX Team", FontStyle.Bold, 13);

            easterEgg = new EasterEggAboutAnimation(cLogo, this);
        }

        private async void AboutForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();

            if (checkUpdate)
            {
                UpdateChecker updateChecker = Program.UpdateManager.CreateUpdateChecker();
                await uclUpdate.CheckUpdate(updateChecker);
            }
        }

        private void pbLogo_MouseDown(object sender, MouseEventArgs e)
        {
            easterEgg.Start();
            pbLogo.Visible = false;
            TaskHelpers.PlayNotificationSoundAsync(NotificationSound.ActionCompleted);
        }

        private void rtb_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            URLHelpers.OpenURL(e.LinkText);
        }

        private void btnShareXLicense_Click(object sender, EventArgs e)
        {
            FileHelpers.OpenFile(FileHelpers.GetAbsolutePath("Licenses\\ShareX_license.txt"));
        }

        private void btnLicenses_Click(object sender, EventArgs e)
        {
            FileHelpers.OpenFolder(FileHelpers.GetAbsolutePath("Licenses"));
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}