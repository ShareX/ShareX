#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

using CodeWorks.Properties;
using HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CodeWorks
{
    public partial class MainForm : Form
    {
        private readonly string folderPath = Path.GetFullPath(@"..\..\..");
        private readonly string[] ignoreFolders = new string[] { "bin", "obj", "Properties", "GreenshotImageEditor", "AviFile" };
        private readonly string[] ignoreFiles = new string[] { };
        private readonly string[] ignoreFilenamesEndsWith = new string[] { ".designer.cs" };
        private readonly string[] allowFilenamesEndsWith = new string[] { ".cs" };
        private readonly string licenseText = Resources.ShareXLicense;

        public MainForm()
        {
            InitializeComponent();
            txtFolderPath.Text = folderPath;
        }

        private void btnFindRegionAreas_Click(object sender, EventArgs e)
        {
            lvResults.Items.Clear();
            SearchFolderRegionAreas(txtFolderPath.Text);
            lblFileCount.Text = lvResults.Items.Count + " files.";
            SystemSounds.Exclamation.Play();
        }

        private void SearchFolderRegionAreas(string path)
        {
            if (IsValidFolder(path))
            {
                foreach (string folder in Directory.GetDirectories(path))
                {
                    SearchFolderRegionAreas(folder);
                }

                foreach (string file in Directory.GetFiles(path))
                {
                    CleanRegionAreas(file, "License Information (GPL v3)", licenseText);
                }
            }
        }

        private bool CleanRegionAreas(string path, string searchRegionName, string newRegionText)
        {
            if (IsValidFile(path))
            {
                TextInfo info = new TextInfo(path);
                RegionAreaManager regionAreaManager = new RegionAreaManager(info.DefaultText);
                List<RegionArea> regionAreas = regionAreaManager.GetRegionAreas();
                int offset = 0;

                foreach (RegionArea regionArea in regionAreas)
                {
                    if (regionArea.RegionName.IndexOf(searchRegionName, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        regionArea.RegionIndexOffset = offset;
                        string regionAreaTextRemoved = regionArea.RemoveRegionText();
                        offset += -regionArea.RegionLength;
                        regionAreaManager.Text = regionAreaTextRemoved;
                    }
                }

                info.NewText = newRegionText + "\r\n\r\n" + regionAreaManager.Text.Trim();

                if (info.IsDifferent)
                {
                    lvResults.Items.Add(path).Tag = info;
                    return true;
                }
            }

            return false;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            lvResults.Items.Clear();
            SearchFolder(txtFolderPath.Text);
            lblFileCount.Text = lvResults.Items.Count + " files.";
            SystemSounds.Exclamation.Play();
        }

        private void SearchFolder(string path)
        {
            if (IsValidFolder(path))
            {
                foreach (string folder in Directory.GetDirectories(path))
                {
                    SearchFolder(folder);
                }

                foreach (string file in Directory.GetFiles(path))
                {
                    CheckFile(file);
                }
            }
        }

        private bool IsValidFolder(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string foldername = Path.GetFileName(path).ToLowerInvariant();
                return ignoreFolders.All(x => !foldername.Equals(x, StringComparison.InvariantCultureIgnoreCase)) && Directory.Exists(path);
            }

            return false;
        }

        private bool IsValidFile(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string filename = Path.GetFileName(path).ToLowerInvariant();
                return Path.HasExtension(filename) && allowFilenamesEndsWith.Any(x => filename.EndsWith(x)) &&
                    ignoreFiles.All(x => !filename.Equals(x, StringComparison.InvariantCultureIgnoreCase)) &&
                    ignoreFilenamesEndsWith.All(x => !filename.EndsWith(x)) && File.Exists(path);
            }

            return false;
        }

        private bool CheckFile(string path)
        {
            if (IsValidFile(path))
            {
                TextInfo info = new TextInfo(path);
                info.NewText = CheckLicense(info.DefaultText);
                // result = RemoveDuplicateLines(result);

                if (info.IsDifferent)
                {
                    lvResults.Items.Add(path).Tag = info;
                    return true;
                }
            }

            return false;
        }

        private string CheckLicense(string text)
        {
            if (!text.StartsWith(licenseText))
            {
                return text.Insert(0, licenseText + "\r\n\r\n");
            }

            return null;
        }

        private string RemoveDuplicateLines(string text)
        {
            return Regex.Replace(text, @"(\r\n\s*){3,}", "\r\n\r\n", RegexOptions.Singleline);
        }

        private void lvResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count > 0)
            {
                TextInfo info = lvResults.SelectedItems[0].Tag as TextInfo;

                if (info != null)
                {
                    tbDefaultText.Text = info.DefaultText;
                    tbNewText.Text = info.NewText;
                }
            }
        }

        private void btnAddLicense_Click(object sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count > 0)
            {
                TextInfo info = lvResults.SelectedItems[0].Tag as TextInfo;

                if (info != null)
                {
                    info.WriteNewText();
                    lvResults.Items.Remove(lvResults.SelectedItems[0]);
                }
            }
        }

        private void btnAddLicenseAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lvResults.Items)
            {
                TextInfo info = lvi.Tag as TextInfo;

                if (info != null)
                {
                    info.WriteNewText();
                }
            }

            lvResults.Items.Clear();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
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

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop, false) as string[];
                txtFolderPath.Text = files[0];
            }
        }

        private void btnOrderLines_Click(object sender, EventArgs e)
        {
            string clipboard = Clipboard.GetText();
            clipboard = clipboard.Trim();
            string[] lines = new StringLineReader(clipboard).ReadAllLines();
            Array.Sort(lines);
            string result = string.Join("\r\n", lines);

            tbDefaultText.Text = clipboard;
            tbNewText.Text = result;
        }

        private void btnCopyToClipboard_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbNewText.Text))
            {
                Clipboard.SetText(tbNewText.Text);
            }
        }
    }
}