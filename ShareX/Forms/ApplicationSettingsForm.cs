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
using ScreenCaptureLib;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UploadersLib;

namespace ShareX
{
    public partial class ApplicationSettingsForm : Form
    {
        private bool loaded;
        private const int MaxBufferSizePower = 14;

        public ApplicationSettingsForm()
        {
            InitializeComponent();
            LoadSettings();
            loaded = true;
        }

        private void LoadSettings()
        {
            Icon = ShareXResources.Icon;

            // General
            cbShowTray.Checked = Program.Settings.ShowTray;
            cbSilentRun.Enabled = Program.Settings.ShowTray;
            cbSilentRun.Checked = Program.Settings.SilentRun;
            cbStartWithWindows.Checked = ShortcutHelpers.CheckShortcut(Environment.SpecialFolder.Startup); //RegistryHelper.CheckStartWithWindows();
            cbSendToMenu.Checked = ShortcutHelpers.CheckShortcut(Environment.SpecialFolder.SendTo);
            cbShellContextMenu.Checked = RegistryHelpers.CheckShellContextMenu();
            cbTrayIconProgressEnabled.Checked = Program.Settings.TrayIconProgressEnabled;
            cbTaskbarProgressEnabled.Enabled = TaskbarManager.IsPlatformSupported;
            cbTaskbarProgressEnabled.Checked = Program.Settings.TaskbarProgressEnabled;
            cbRememberMainFormPosition.Checked = Program.Settings.RememberMainFormPosition;
            cbRememberMainFormSize.Checked = Program.Settings.RememberMainFormSize;

            // Paths
            txtPersonalFolderPath.Text = Program.ReadPersonalPathConfig();
            UpdatePersonalFolderPathPreview();
            cbUseCustomScreenshotsPath.Checked = Program.Settings.UseCustomScreenshotsPath;
            txtCustomScreenshotsPath.Text = Program.Settings.CustomScreenshotsPath;
            txtSaveImageSubFolderPattern.Text = Program.Settings.SaveImageSubFolderPattern;
            NameParser.CreateCodesMenu(txtSaveImageSubFolderPattern, ReplacementVariables.t, ReplacementVariables.pn, ReplacementVariables.i,
                ReplacementVariables.width, ReplacementVariables.height, ReplacementVariables.n);

            // Proxy
            cbProxyMethod.Items.AddRange(Enum.GetNames(typeof(ProxyMethod)));
            cbProxyMethod.SelectedIndex = (int)Program.Settings.ProxySettings.ProxyMethod;
            txtProxyUsername.Text = Program.Settings.ProxySettings.Username;
            txtProxyPassword.Text = Program.Settings.ProxySettings.Password;
            txtProxyHost.Text = Program.Settings.ProxySettings.Host ?? string.Empty;
            nudProxyPort.Value = Program.Settings.ProxySettings.Port;
            UpdateProxyControls();

            // Upload
            nudUploadLimit.Value = Program.Settings.UploadLimit;

            for (int i = 0; i < MaxBufferSizePower; i++)
            {
                string size = ((long)(Math.Pow(2, i) * 1024)).ToSizeString(Program.Settings.BinaryUnits, 0);
                cbBufferSize.Items.Add(size);
            }

            cbBufferSize.SelectedIndex = Program.Settings.BufferSizePower.Between(0, MaxBufferSizePower);

            foreach (ClipboardFormat cf in Program.Settings.ClipboardContentFormats)
            {
                AddClipboardFormat(cf);
            }

            nudRetryUpload.Value = Program.Settings.MaxUploadFailRetry;
            chkUseSecondaryUploaders.Checked = Program.Settings.UseSecondaryUploaders;
            tlpBackupDestinations.Enabled = Program.Settings.UseSecondaryUploaders;

            Program.Settings.SecondaryImageUploaders.AddRange(Helpers.GetEnums<ImageDestination>().Where(n => Program.Settings.SecondaryImageUploaders.All(e => e != n)));
            Program.Settings.SecondaryTextUploaders.AddRange(Helpers.GetEnums<TextDestination>().Where(n => Program.Settings.SecondaryTextUploaders.All(e => e != n)));
            Program.Settings.SecondaryFileUploaders.AddRange(Helpers.GetEnums<FileDestination>().Where(n => Program.Settings.SecondaryFileUploaders.All(e => e != n)));

            Program.Settings.SecondaryImageUploaders.Where(n => Helpers.GetEnums<ImageDestination>().All(e => e != n)).ForEach(x => Program.Settings.SecondaryImageUploaders.Remove(x));
            Program.Settings.SecondaryTextUploaders.Where(n => Helpers.GetEnums<TextDestination>().All(e => e != n)).ForEach(x => Program.Settings.SecondaryTextUploaders.Remove(x));
            Program.Settings.SecondaryFileUploaders.Where(n => Helpers.GetEnums<FileDestination>().All(e => e != n)).ForEach(x => Program.Settings.SecondaryFileUploaders.Remove(x));

            Program.Settings.SecondaryImageUploaders.ForEach<ImageDestination>(x => lvSecondaryImageUploaders.Items.Add(new ListViewItem(x.GetDescription()) { Tag = x }));
            Program.Settings.SecondaryTextUploaders.ForEach<TextDestination>(x => lvSecondaryTextUploaders.Items.Add(new ListViewItem(x.GetDescription()) { Tag = x }));
            Program.Settings.SecondaryFileUploaders.ForEach<FileDestination>(x => lvSecondaryFileUploaders.Items.Add(new ListViewItem(x.GetDescription()) { Tag = x }));

            // Print
            cbDontShowPrintSettingDialog.Checked = Program.Settings.DontShowPrintSettingsDialog;
            cbPrintDontShowWindowsDialog.Checked = !Program.Settings.PrintSettings.ShowPrintDialog;

            // Advanced
            pgSettings.SelectedObject = Program.Settings;
        }

        private void SettingsForm_Shown(object sender, EventArgs e)
        {
            this.ShowActivate();
        }

        private void SettingsForm_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.WritePersonalPathConfig(txtPersonalFolderPath.Text);
        }

        private void UpdateProxyControls()
        {
            switch (Program.Settings.ProxySettings.ProxyMethod)
            {
                case ProxyMethod.None:
                    txtProxyUsername.Enabled = txtProxyPassword.Enabled = txtProxyHost.Enabled = nudProxyPort.Enabled = false;
                    break;
                case ProxyMethod.Manual:
                    txtProxyUsername.Enabled = txtProxyPassword.Enabled = txtProxyHost.Enabled = nudProxyPort.Enabled = true;
                    break;
                case ProxyMethod.Automatic:
                    txtProxyUsername.Enabled = txtProxyPassword.Enabled = true;
                    txtProxyHost.Enabled = nudProxyPort.Enabled = false;
                    break;
            }
        }

        private void UpdatePersonalFolderPathPreview()
        {
            string personalPath = txtPersonalFolderPath.Text;

            if (string.IsNullOrEmpty(personalPath))
            {
                personalPath = Program.DefaultPersonalPath;
            }
            else
            {
                personalPath = Path.GetFullPath(personalPath);
            }

            lblPreviewPersonalFolderPath.Text = personalPath;
        }

        #region General

        private void cbShowTray_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.ShowTray = cbShowTray.Checked;

            if (loaded)
            {
                Program.MainForm.niTray.Visible = Program.Settings.ShowTray;
            }

            cbSilentRun.Enabled = Program.Settings.ShowTray;
        }

        private void cbSilentRun_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.SilentRun = cbSilentRun.Checked;
        }

        private void cbStartWithWindows_CheckedChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                //RegistryHelper.SetStartWithWindows(cbStartWithWindows.Checked);
                ShortcutHelpers.SetShortcut(cbStartWithWindows.Checked, Environment.SpecialFolder.Startup, "-silent");
            }
        }

        private void cbSendToMenu_CheckedChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                ShortcutHelpers.SetShortcut(cbSendToMenu.Checked, Environment.SpecialFolder.SendTo);
            }
        }

        private void cbShellContextMenu_CheckedChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                RegistryHelpers.SetShellContextMenu(cbShellContextMenu.Checked);
            }
        }

        private void cbTrayIconProgressEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.TrayIconProgressEnabled = cbTrayIconProgressEnabled.Checked;
        }

        private void cbTaskbarProgressEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.TaskbarProgressEnabled = cbTaskbarProgressEnabled.Checked;

            if (loaded)
            {
                TaskbarManager.Enabled = Program.Settings.TaskbarProgressEnabled;
            }
        }

        private void cbRememberMainFormPosition_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.RememberMainFormPosition = cbRememberMainFormPosition.Checked;
        }

        private void cbRememberMainFormSize_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.RememberMainFormSize = cbRememberMainFormSize.Checked;
        }

        #endregion General

        #region Paths

        private void txtPersonalFolderPath_TextChanged(object sender, EventArgs e)
        {
            UpdatePersonalFolderPathPreview();
        }

        private void btnBrowsePersonalFolderPath_Click(object sender, EventArgs e)
        {
            Helpers.BrowseFolder("Choose ShareX personal folder path", txtPersonalFolderPath, Program.PersonalPath);
        }

        private void btnOpenPersonalFolder_Click(object sender, EventArgs e)
        {
            Helpers.OpenFolder(lblPreviewPersonalFolderPath.Text);
        }

        private void cbUseCustomScreenshotsPath_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.UseCustomScreenshotsPath = cbUseCustomScreenshotsPath.Checked;
            lblSaveImageSubFolderPatternPreview.Text = Program.ScreenshotsFolder;
        }

        private void txtCustomScreenshotsPath_TextChanged(object sender, EventArgs e)
        {
            Program.Settings.CustomScreenshotsPath = txtCustomScreenshotsPath.Text;
            lblSaveImageSubFolderPatternPreview.Text = Program.ScreenshotsFolder;
        }

        private void btnBrowseCustomScreenshotsPath_Click(object sender, EventArgs e)
        {
            Helpers.BrowseFolder("Choose screenshots folder path", txtCustomScreenshotsPath, Program.PersonalPath);
        }

        private void txtSaveImageSubFolderPattern_TextChanged(object sender, EventArgs e)
        {
            Program.Settings.SaveImageSubFolderPattern = txtSaveImageSubFolderPattern.Text;
            lblSaveImageSubFolderPatternPreview.Text = Program.ScreenshotsFolder;
        }

        private void btnOpenScreenshotsFolder_Click(object sender, EventArgs e)
        {
            Helpers.OpenFolder(lblSaveImageSubFolderPatternPreview.Text);
        }

        #endregion Paths

        #region Proxy

        private void cbProxyMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Settings.ProxySettings.ProxyMethod = (ProxyMethod)cbProxyMethod.SelectedIndex;

            if (Program.Settings.ProxySettings.ProxyMethod == ProxyMethod.Automatic)
            {
                Program.Settings.ProxySettings.IsValidProxy();
                txtProxyHost.Text = Program.Settings.ProxySettings.Host ?? string.Empty;
                nudProxyPort.Value = Program.Settings.ProxySettings.Port;
            }

            UpdateProxyControls();
        }

        private void txtProxyUsername_TextChanged(object sender, EventArgs e)
        {
            Program.Settings.ProxySettings.Username = txtProxyUsername.Text;
        }

        private void txtProxyPassword_TextChanged(object sender, EventArgs e)
        {
            Program.Settings.ProxySettings.Password = txtProxyPassword.Text;
        }

        private void txtProxyHost_TextChanged(object sender, EventArgs e)
        {
            Program.Settings.ProxySettings.Host = txtProxyHost.Text;
        }

        private void nudProxyPort_ValueChanged(object sender, EventArgs e)
        {
            Program.Settings.ProxySettings.Port = (int)nudProxyPort.Value;
        }

        #endregion Proxy

        #region Upload

        private void nudUploadLimit_ValueChanged(object sender, EventArgs e)
        {
            Program.Settings.UploadLimit = (int)nudUploadLimit.Value;
        }

        private void cbBufferSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Settings.BufferSizePower = cbBufferSize.SelectedIndex;
        }

        private void AddClipboardFormat(ClipboardFormat cf)
        {
            ListViewItem lvi = new ListViewItem(cf.Description ?? "");
            lvi.Tag = cf;
            lvi.SubItems.Add(cf.Format ?? "");
            lvClipboardFormats.Items.Add(lvi);
        }

        private void ClipboardFormatsEditSelected()
        {
            if (lvClipboardFormats.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvClipboardFormats.SelectedItems[0];
                ClipboardFormat cf = lvi.Tag as ClipboardFormat;
                using (ClipboardFormatForm form = new ClipboardFormatForm(cf))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        lvi.Text = form.ClipboardFormat.Description ?? "";
                        lvi.Tag = form.ClipboardFormat;
                        lvi.SubItems[1].Text = form.ClipboardFormat.Format ?? "";
                    }
                }
            }
        }

        private void lvClipboardFormats_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ClipboardFormatsEditSelected();
            }
        }

        private void btnAddClipboardFormat_Click(object sender, EventArgs e)
        {
            using (ClipboardFormatForm form = new ClipboardFormatForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ClipboardFormat cf = form.ClipboardFormat;
                    Program.Settings.ClipboardContentFormats.Add(cf);
                    AddClipboardFormat(cf);
                }
            }
        }

        private void btnClipboardFormatEdit_Click(object sender, EventArgs e)
        {
            ClipboardFormatsEditSelected();
        }

        private void btnClipboardFormatRemove_Click(object sender, EventArgs e)
        {
            if (lvClipboardFormats.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvClipboardFormats.SelectedItems[0];
                ClipboardFormat cf = lvi.Tag as ClipboardFormat;
                Program.Settings.ClipboardContentFormats.Remove(cf);
                lvClipboardFormats.Items.Remove(lvi);
            }
        }

        private void chkUseSecondaryUploaders_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.UseSecondaryUploaders = chkUseSecondaryUploaders.Checked;
            tlpBackupDestinations.Enabled = Program.Settings.UseSecondaryUploaders;
        }

        private void nudRetryUpload_ValueChanged(object sender, EventArgs e)
        {
            Program.Settings.MaxUploadFailRetry = (int)nudRetryUpload.Value;
        }

        private void lvSecondaryUploaders_MouseUp(object sender, MouseEventArgs e)
        {
            Program.Settings.SecondaryImageUploaders = lvSecondaryImageUploaders.Items.Cast<ListViewItem>().Select(x => (ImageDestination)x.Tag).ToList();
            Program.Settings.SecondaryTextUploaders = lvSecondaryTextUploaders.Items.Cast<ListViewItem>().Select(x => (TextDestination)x.Tag).ToList();
            Program.Settings.SecondaryFileUploaders = lvSecondaryFileUploaders.Items.Cast<ListViewItem>().Select(x => (FileDestination)x.Tag).ToList();
        }

        #endregion Upload

        #region Print

        private void cbDontShowPrintSettingDialog_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.DontShowPrintSettingsDialog = cbDontShowPrintSettingDialog.Checked;
        }

        private void btnShowImagePrintSettings_Click(object sender, EventArgs e)
        {
            using (Image testImage = Screenshot.CaptureActiveMonitor())
            using (PrintForm printForm = new PrintForm(testImage, Program.Settings.PrintSettings, true))
            {
                printForm.ShowDialog();
            }
        }

        private void cbPrintDontShowWindowsDialog_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.PrintSettings.ShowPrintDialog = !cbPrintDontShowWindowsDialog.Checked;
        }

        #endregion Print
    }
}