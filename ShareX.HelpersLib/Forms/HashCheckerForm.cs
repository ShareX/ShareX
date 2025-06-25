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

using ShareX.HelpersLib.Properties;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class HashCheckerForm : Form
    {
        public event Action PlayNotificationSound;

        public bool CompareTwoFiles { get; private set; }

        private HashChecker hashChecker;

        public HashCheckerForm(string filePath = null)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            UpdateCompareControls();
            UpdateCheckButton();
            cbHashType.Items.AddRange(Helpers.GetEnumDescriptions<HashType>());
            cbHashType.SelectedIndex = (int)HashType.SHA256;

            hashChecker = new HashChecker();
            hashChecker.FileCheckProgressChanged += fileCheck_FileCheckProgressChanged;

            txtResult.SupportSelectAll();
            txtTarget.SupportSelectAll();

            if (!string.IsNullOrEmpty(filePath))
            {
                txtFilePath.Text = filePath;
            }
        }

        protected void OnPlayNotificationSound()
        {
            PlayNotificationSound?.Invoke();
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

        private void UpdateCheckButton()
        {
            btnStartHashCheck.Enabled = !string.IsNullOrEmpty(txtFilePath.Text) && (!CompareTwoFiles || !string.IsNullOrEmpty(txtFilePath2.Text));
        }

        private void UpdateResult()
        {
            if (!string.IsNullOrEmpty(txtResult.Text) && !string.IsNullOrEmpty(txtTarget.Text))
            {
                if (txtResult.Text.Equals(txtTarget.Text, StringComparison.OrdinalIgnoreCase))
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

        private async Task StartHashCheck()
        {
            if (hashChecker.IsWorking)
            {
                hashChecker.Stop();
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
                string result = await hashChecker.Start(filePath, hashType);

                if (!string.IsNullOrEmpty(result))
                {
                    txtResult.Text = result.ToUpperInvariant();

                    if (CompareTwoFiles)
                    {
                        string filePath2 = txtFilePath2.Text;
                        string result2 = await hashChecker.Start(filePath2, hashType);

                        if (!string.IsNullOrEmpty(result2))
                        {
                            txtTarget.Text = result2.ToUpperInvariant();
                        }
                    }

                    OnPlayNotificationSound();
                }

                btnStartHashCheck.Text = Resources.Check;
            }
        }

        private async void HashCheckerForm_Shown(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtFilePath.Text))
            {
                btnStartHashCheck.Focus();

                await StartHashCheck();
            }
        }

        private void HashCheckerForm_DragEnter(object sender, DragEventArgs e)
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

        private void HashCheckerForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) && e.Data.GetData(DataFormats.FileDrop, false) is string[] files && files.Length > 0)
            {
                txtFilePath.Text = files[0];
            }
        }

        private void txtFilePath_TextChanged(object sender, EventArgs e)
        {
            UpdateCheckButton();
        }

        private void btnFilePathBrowse_Click(object sender, EventArgs e)
        {
            FileHelpers.BrowseFile(txtFilePath);
        }

        private void cbCompareTwoFiles_CheckedChanged(object sender, EventArgs e)
        {
            CompareTwoFiles = cbCompareTwoFiles.Checked;
            UpdateCompareControls();
            UpdateCheckButton();
        }

        private void txtFilePath2_TextChanged(object sender, EventArgs e)
        {
            UpdateCheckButton();
        }

        private void txtFilePath2_DragEnter(object sender, DragEventArgs e)
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

        private void txtFilePath2_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) && e.Data.GetData(DataFormats.FileDrop, false) is string[] files && files.Length > 0)
            {
                txtFilePath2.Text = files[0];
            }
        }

        private void btnFilePathBrowse2_Click(object sender, EventArgs e)
        {
            FileHelpers.BrowseFile(txtFilePath2);
        }

        private async void btnStartHashCheck_Click(object sender, EventArgs e)
        {
            await StartHashCheck();
        }

        private void fileCheck_FileCheckProgressChanged(float progress)
        {
            pbProgress.Value = (int)Math.Round(progress);
        }

        private void txtResult_TextChanged(object sender, EventArgs e)
        {
            UpdateResult();
        }

        private void txtTarget_TextChanged(object sender, EventArgs e)
        {
            UpdateResult();
        }
    }
}