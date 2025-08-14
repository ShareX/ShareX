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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.HistoryLib.Forms
{
    public partial class HistoryImportForm : Form
    {
        public HistoryManagerSQLite HistoryManager { get; private set; }
        public List<HistoryItem> HistoryItems { get; private set; }

        public HistoryImportForm(HistoryManagerSQLite historyManager, List<HistoryItem> historyItems)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            HistoryManager = historyManager;
            HistoryItems = historyItems;
        }

        private void ImportFolder(string folderPath, bool onlyImportImageFiles, bool skipDuplicateFiles)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories);

            if (onlyImportImageFiles)
            {
                files = files.Where(FileHelpers.IsImageFile);
            }

            if (files.Count() == 0)
            {
                return;
            }

            files = files.OrderBy(File.GetLastWriteTime);

            List<HistoryItem> historyItems = new List<HistoryItem>();

            foreach (string file in files)
            {
                if (skipDuplicateFiles && HistoryItems.Any(x => x.FilePath.Equals(file, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                string fileName = Path.GetFileName(file);
                DateTime dateTime = File.GetLastWriteTime(file);
                string type;

                if (FileHelpers.IsImageFile(file))
                {
                    type = "Image";
                }
                else if (FileHelpers.IsTextFile(file))
                {
                    type = "Text";
                }
                else
                {
                    type = "File";
                }

                HistoryItem historyItem = new HistoryItem()
                {
                    FileName = fileName,
                    FilePath = file,
                    DateTime = dateTime,
                    Type = type
                };

                historyItems.Add(historyItem);
            }

            if (historyItems.Count > 0)
            {
                HistoryManager.AppendHistoryItems(historyItems);

                DialogResult = DialogResult.OK;
                Close();
            }

            // TODO: Translate
            MessageBox.Show(string.Format("Successfully imported {0} files.", historyItems.Count),
                "ShareX - Import complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void txtFolderPath_TextChanged(object sender, EventArgs e)
        {
            btnImport.Enabled = !string.IsNullOrWhiteSpace(txtFolderPath.Text);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FileHelpers.BrowseFolder(txtFolderPath);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            ImportFolder(txtFolderPath.Text, cbOnlyImportImageFiles.Checked, cbSkipDuplicates.Checked);
        }
    }
}