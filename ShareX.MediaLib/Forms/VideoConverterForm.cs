#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.MediaLib
{
    public partial class VideoConverterForm : Form
    {
        public string FFmpegFilePath { get; private set; }
        public VideoConverterOptions Options { get; private set; }

        private bool ready;

        public VideoConverterForm(string ffmpegFilePath, VideoConverterOptions options)
        {
            FFmpegFilePath = ffmpegFilePath;
            Options = options;

            InitializeComponent();
            ShareXResources.ApplyTheme(this);

            cbVideoCodec.Items.AddRange(Helpers.GetEnumDescriptions<ConverterVideoCodecs>());
            cbVideoCodec.SelectedIndex = (int)Options.VideoCodec;
            nudVideoQuality.SetValue(Options.VideoQuality);

            ready = true;

            UpdateOptions();
        }

        private void UpdateOptions()
        {
            if (ready)
            {
                Options.InputFilePath = txtInputFilePath.Text;
                Options.OutputFolderPath = txtOutputFolder.Text;
                Options.OutputFileName = txtOutputFileName.Text;
                Options.VideoCodec = (ConverterVideoCodecs)cbVideoCodec.SelectedIndex;
                Options.VideoQuality = (int)nudVideoQuality.Value;

                txtCLI.Text = Options.GetFFmpegArgs();
            }
        }

        private bool StartEncoding()
        {
            bool result = false;

            if (!string.IsNullOrEmpty(Options.InputFilePath) && File.Exists(Options.InputFilePath) && !string.IsNullOrEmpty(Options.OutputFilePath))
            {
                using (FFmpegCLIManager manager = new FFmpegCLIManager(FFmpegFilePath))
                {
                    manager.ShowError = true;

                    string outputFilePath = Options.OutputFilePath;
                    string args = Options.GetFFmpegArgs();
                    result = manager.Run(args);

                    if (result)
                    {
                        Helpers.OpenFolderWithFile(outputFilePath);
                    }
                }
            }

            return result;
        }

        private async Task<bool> StartEncodingAsync()
        {
            return await Task.Run(StartEncoding);
        }

        private void txtInputFilePath_TextChanged(object sender, EventArgs e)
        {
            UpdateOptions();
        }

        private void btnInputFilePathBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    string filePath = ofd.FileName;
                    txtInputFilePath.Text = filePath;

                    if (string.IsNullOrEmpty(txtOutputFolder.Text))
                    {
                        txtOutputFolder.Text = Path.GetDirectoryName(filePath);
                    }

                    if (string.IsNullOrEmpty(txtOutputFileName.Text))
                    {
                        txtOutputFileName.Text = Path.GetFileNameWithoutExtension(filePath) + "-output";
                    }
                }
            }
        }

        private void txtOutputFolder_TextChanged(object sender, EventArgs e)
        {
            UpdateOptions();
        }

        private void txtOutputFileName_TextChanged(object sender, EventArgs e)
        {
            UpdateOptions();
        }

        private void cbVideoCodec_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateOptions();
        }

        private void nudVideoQuality_ValueChanged(object sender, EventArgs e)
        {
            UpdateOptions();
        }

        private async void btnEncode_Click(object sender, EventArgs e)
        {
            UpdateOptions();

            pbProgress.Visible = true;
            btnEncode.Visible = false;

            await StartEncodingAsync();

            btnEncode.Visible = true;
            pbProgress.Visible = false;
        }
    }
}