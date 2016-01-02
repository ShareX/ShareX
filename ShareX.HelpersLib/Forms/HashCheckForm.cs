#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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
    public partial class HashCheckForm : BaseForm
    {
        private HashCheck hashCheck;
        private Translator translator;

        public HashCheckForm()
        {
            InitializeComponent();

            cbHashType.Items.AddRange(Helpers.GetEnumDescriptions<HashType>());
            cbHashType.SelectedIndex = (int)HashType.SHA1;

            hashCheck = new HashCheck();
            hashCheck.FileCheckProgressChanged += fileCheck_FileCheckProgressChanged;
            hashCheck.FileCheckCompleted += fileCheck_FileCheckCompleted;

            translator = new Translator();
        }

        #region File hash check

        private void btnFilePathBrowse_Click(object sender, EventArgs e)
        {
            Helpers.BrowseFile("ShareX - " + Resources.HashCheckForm_btnFilePathBrowse_Click_Choose_file_path, txtFilePath);
        }

        private void btnStartHashCheck_Click(object sender, EventArgs e)
        {
            if (hashCheck.IsWorking)
            {
                hashCheck.Stop();
            }
            else
            {
                HashType hashType = (HashType)cbHashType.SelectedIndex;

                if (hashCheck.Start(txtFilePath.Text, hashType))
                {
                    btnStartHashCheck.Text = Resources.Stop;
                    txtResult.Text = string.Empty;
                }
            }
        }

        private void fileCheck_FileCheckProgressChanged(float progress)
        {
            pbProgress.Value = (int)progress;
            lblProgressPercentage.Text = (int)progress + "%";
        }

        private void fileCheck_FileCheckCompleted(string result, bool cancelled)
        {
            btnStartHashCheck.Text = Resources.Start;
            txtResult.Text = result.ToUpperInvariant();
            if (!string.IsNullOrEmpty(txtTarget.Text))
            {
                txtTarget.Text = txtTarget.Text.ToUpperInvariant();
            }
        }

        private void txtResult_TextChanged(object sender, EventArgs e)
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
            }
            else
            {
                txtTarget.BackColor = Color.White;
            }
        }

        private void txtFilePath_DragEnter(object sender, DragEventArgs e)
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

        private void txtFilePath_DragDrop(object sender, DragEventArgs e)
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