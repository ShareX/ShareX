#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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

using ShareX.HelpersLib.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class HashCheckForm : Form
    {
        public bool CompareTwoFiles { get; private set; }

        private HashCheck hashCheck;
        private Translator translator;

        public HashCheckForm()
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this);

            UpdateCompareControls();
            cbHashType.Items.AddRange(Helpers.GetEnumDescriptions<HashType>());
            cbHashType.SelectedIndex = (int)HashType.SHA1;

            hashCheck = new HashCheck();
            hashCheck.FileCheckProgressChanged += fileCheck_FileCheckProgressChanged;

            translator = new Translator();
        }

        #region File hash check

        private void UpdateResult()
        {
            if (!string.IsNullOrEmpty(txtResult.Text) && !string.IsNullOrEmpty(txtTarget.Text))
            {
                if (txtResult.Text.Equals(txtTarget.Text, StringComparison.InvariantCultureIgnoreCase))
                {
                    txtTarget.BackColor = Color.FromArgb(200, 255, 200);
                }
                else
                {
                    txtTarget.BackColor = Color.FromArgb(255, 200, 200);
                }

                txtTarget.ForeColor = SystemColors.ControlText;
            }
            else
            {
                txtTarget.BackColor = txtResult.BackColor;
                txtTarget.ForeColor = txtResult.ForeColor;
            }
        }

        private void UpdateCompareControls()
        {
            lblFilePath2.Enabled = txtFilePath2.Enabled = btnFilePathBrowse2.Enabled = CompareTwoFiles;

            if (CompareTwoFiles)
            {
                lblResult.Text = Resources.ResultOfFirstFile;
                lblTarget.Text = Resources.ResultOfSecondFile;
            }
            else
            {
                lblResult.Text = Resources.Result;
                lblTarget.Text = Resources.Target;
            }

            txtTarget.ReadOnly = CompareTwoFiles;
        }

        private void btnFilePathBrowse_Click(object sender, EventArgs e)
        {
            Helpers.BrowseFile(txtFilePath);
        }

        private void btnFilePathBrowse2_Click(object sender, EventArgs e)
        {
            Helpers.BrowseFile(txtFilePath2);
        }

        private void cbCompareTwoFiles_CheckedChanged(object sender, EventArgs e)
        {
            CompareTwoFiles = cbCompareTwoFiles.Checked;
            UpdateCompareControls();
        }

        private async void btnStartHashCheck_Click(object sender, EventArgs e)
        {
            if (hashCheck.IsWorking)
            {
                hashCheck.Stop();
            }
            else
            {
                btnStartHashCheck.Text = Resources.Stop;
                pbProgress.Value = 0;
                txtResult.Text = "";

                if (CompareTwoFiles)
                {
                    txtTarget.Text = "";
                }

                HashType hashType = (HashType)cbHashType.SelectedIndex;

                string filePath = txtFilePath.Text;
                string result = await hashCheck.Start(filePath, hashType);

                if (!string.IsNullOrEmpty(result))
                {
                    txtResult.Text = result.ToUpperInvariant();

                    if (CompareTwoFiles)
                    {
                        string filePath2 = txtFilePath2.Text;
                        string result2 = await hashCheck.Start(filePath2, hashType);

                        if (!string.IsNullOrEmpty(result2))
                        {
                            txtTarget.Text = result2.ToUpperInvariant();
                        }
                    }
                }

                btnStartHashCheck.Text = Resources.Start;
            }
        }

        private void fileCheck_FileCheckProgressChanged(float progress)
        {
            pbProgress.Value = (int)progress;
            lblProgressPercentage.Text = (int)progress + "%";
        }

        private void txtResult_TextChanged(object sender, EventArgs e)
        {
            UpdateResult();
        }

        private void txtTarget_TextChanged(object sender, EventArgs e)
        {
            UpdateResult();
        }

        private void txtResult_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                txtResult.SelectAll();
            }
        }

        private void txtTarget_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                txtTarget.SelectAll();
            }
        }

        private void tpFileHashCheck_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void tpFileHashCheck_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop, false) as string[];

                if (files != null && files.Length > 0)
                {
                    txtFilePath.Text = files[0];
                }
            }
        }

        #endregion File hash check

        #region Text conversions

        private void FillConversionInfo()
        {
            FillConversionInfo(translator.Text);
        }

        private void FillConversionInfo(string text)
        {
            if (translator != null)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    translator.EncodeText(text);
                    txtHashCheckText.Text = translator.Text;
                    txtHashCheckBinary.Text = translator.BinaryText;
                    txtHashCheckHex.Text = translator.HexadecimalText;
                    txtHashCheckASCII.Text = translator.ASCIIText;
                    txtHashCheckBase64.Text = translator.Base64;
                    txtHashCheckHash.Text = translator.HashToString();
                }
                else
                {
                    translator.Clear();
                    txtHashCheckText.Text = txtHashCheckBinary.Text = txtHashCheckHex.Text = txtHashCheckASCII.Text = txtHashCheckBase64.Text = txtHashCheckHash.Text = "";
                }
            }
        }

        private void btnHashCheckCopyAll_Click(object sender, EventArgs e)
        {
            if (translator != null)
            {
                string text = translator.ToString();

                if (!string.IsNullOrEmpty(text))
                {
                    ClipboardHelpers.CopyText(text);
                }
            }
        }

        private void btnHashCheckEncodeText_Click(object sender, EventArgs e)
        {
            FillConversionInfo(txtHashCheckText.Text);
        }

        private void btnHashCheckDecodeBinary_Click(object sender, EventArgs e)
        {
            string binary = txtHashCheckBinary.Text;
            translator.DecodeBinary(binary);
            FillConversionInfo();
            txtHashCheckBinary.Text = binary;
        }

        private void btnHashCheckDecodeHex_Click(object sender, EventArgs e)
        {
            string hex = txtHashCheckHex.Text;
            translator.DecodeHex(hex);
            FillConversionInfo();
            txtHashCheckHex.Text = hex;
        }

        private void btnHashCheckDecodeASCII_Click(object sender, EventArgs e)
        {
            string ascii = txtHashCheckASCII.Text;
            translator.DecodeASCII(ascii);
            FillConversionInfo();
            txtHashCheckASCII.Text = ascii;
        }

        private void btnHashCheckDecodeBase64_Click(object sender, EventArgs e)
        {
            string base64 = txtHashCheckBase64.Text;
            translator.DecodeBase64(base64);
            FillConversionInfo();
            txtHashCheckBase64.Text = base64;
        }

        #endregion Text conversions
    }
}