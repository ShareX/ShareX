using System;
using ShareX.UploadersLib.Controls;

namespace ShareX.UploadersLib.TextUploaders.Paste_ee
{
    public partial class Paste_eeConfigControl : BaseConfigControl
    {
        private readonly UploadersConfig config;

        public Paste_eeConfigControl(UploadersConfig config)
        {
            this.config = config;

            InitializeComponent();

            txtPaste_eeUserAPIKey.Text = config.Paste_eeUserAPIKey;
        }

        private void txtPaste_eeUserAPIKey_TextChanged(object sender, EventArgs e)
        {
            config.Paste_eeUserAPIKey = txtPaste_eeUserAPIKey.Text;
        }
    }
}
