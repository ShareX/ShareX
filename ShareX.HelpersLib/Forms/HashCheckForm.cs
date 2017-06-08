﻿#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
        private HashCheck hashCheck;
        private Translator translator;

        public HashCheckForm()
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            cbHashType.Items.AddRange(Helpers.GetEnumDescriptions<HashType>());
            cbHashType.SelectedIndex = (int)HashType.SHA1;

            hashCheck = new HashCheck();
            hashCheck.FileCheckProgressChanged += fileCheck_FileCheckProgressChanged;
            hashCheck.FileCheckCompleted += fileCheck_FileCheckCompleted;

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
            }
            else
            {
                txtTarget.BackColor = SystemColors.Window;
            }
        }

        private void btnFilePathBrowse_Click(object sender, EventArgs e)
        {
            Helpers.BrowseFile(txtFilePath);
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
                    txtResult.Text = "";
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
        }

        private void txtResult_TextChanged(object sender, EventArgs e)
        {
            UpdateResult();
        }

        private void txtTarget_TextChanged(object sender, EventArgs e)
        {
            string target = txtTarget.Text;

            if (!string.IsNullOrEmpty(target))
            {
                txtTarget.Text = target.RemoveWhiteSpaces().ToUpperInvariant();
                txtTarget.Select(txtTarget.TextLength, 0);
            }

            UpdateResult();
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
    }
}