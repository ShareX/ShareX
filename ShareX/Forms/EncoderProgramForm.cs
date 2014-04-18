using HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX
{
    public partial class EncoderProgramForm : Form
    {
        public VideoEncoder encoder { get; private set; }

        public EncoderProgramForm()
            : this(new VideoEncoder())
        {
        }

        public EncoderProgramForm(VideoEncoder encoder)
        {
            this.encoder = encoder;
            InitializeComponent();
            txtName.Text = encoder.Name ?? "";
            txtPath.Text = encoder.Path ?? "";
            txtArguments.Text = encoder.Args ?? "";
            txtExtension.Text = encoder.OutputExtension ?? "";
        }

        private void btnPathBrowse_Click(object sender, EventArgs e)
        {
            Helpers.BrowseFile("ShareX - Choose encoder path", txtPath);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            encoder.Name = txtName.Text;
            encoder.Path = txtPath.Text;
            encoder.Args = txtArguments.Text;
            encoder.OutputExtension = txtExtension.Text;
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}