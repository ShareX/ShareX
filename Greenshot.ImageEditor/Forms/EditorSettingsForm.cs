using Greenshot.Configuration;
using Greenshot.IniFile;
using GreenshotPlugin.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Greenshot
{
    public partial class EditorSettingsForm : Form
    {
        private static CoreConfiguration coreConfiguration = IniConfig.GetIniSection<CoreConfiguration>();
        private static EditorConfiguration editorConfiguration = IniConfig.GetIniSection<EditorConfiguration>();

        public EditorSettingsForm()
        {
            InitializeComponent();
            Icon = GreenshotResources.getGreenshotIcon();
            LoadSettings();
        }

        private void LoadSettings()
        {
            nudIconSize.Value = (int)Math.Round(coreConfiguration.IconSize.Width / 16.0) * 16;
            cbMatchSizeToCapture.Checked = editorConfiguration.MatchSizeToCapture;
            cbMaximizeWhenLargeImage.Checked = editorConfiguration.MaximizeWhenLargeImage;
            cbSuppressSaveDialogAtClose.Checked = editorConfiguration.SuppressSaveDialogAtClose;
            cbRememberLastDrawingMode.Checked = editorConfiguration.RememberLastDrawingMode;
        }

        private void SaveSettings()
        {
            coreConfiguration.IconSize = new Size((int)nudIconSize.Value, (int)nudIconSize.Value);
            editorConfiguration.MatchSizeToCapture = cbMatchSizeToCapture.Checked;
            editorConfiguration.MaximizeWhenLargeImage = cbMaximizeWhenLargeImage.Checked;
            editorConfiguration.SuppressSaveDialogAtClose = cbSuppressSaveDialogAtClose.Checked;
            editorConfiguration.RememberLastDrawingMode = cbRememberLastDrawingMode.Checked;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }
    }
}