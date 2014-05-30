#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

using HelpersLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.FtpClient;
using System.Windows.Forms;
using UploadersLib.FileUploaders;

namespace UploadersLib
{
    public partial class FTPClientForm : Form
    {
        private const string Root = "/";

        public FTP Client { get; set; }
        public FTPAccount Account { get; set; }

        private string currentDirectory;
        private ListViewItem tempSelected;

        public FTPClientForm(FTPAccount account)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            lblStatus.Text = string.Empty;
            lvFTPList.SubItemEndEditing += lvFTPList_SubItemEndEditing;

            FtpTrace.AddListener(new TextBoxTraceListener(txtDebug));

            Account = account;

            Client = new FTP(account);

            pgAccount.SelectedObject = Client.Account;
            Text = "ShareX FTP client - " + account.Name;
            lblConnecting.Text = "Connecting to " + account.FTPAddress;

            TaskEx.Run(() =>
            {
                Client.Connect();
            },
            () =>
            {
                pConnecting.Visible = false;
                Refresh();
                RefreshDirectory();
            });
        }

        #region Methods

        private void RefreshDirectory()
        {
            if (string.IsNullOrEmpty(currentDirectory))
            {
                currentDirectory = Root;
            }

            LoadDirectory(currentDirectory);
        }

        private void FillDirectories(string path)
        {
            List<string> paths = URLHelpers.GetPaths(path);
            paths.Insert(0, "/");

            cbDirectoryList.Items.Clear();
            foreach (string directory in paths)
            {
                cbDirectoryList.Items.Add(directory);
            }

            if (cbDirectoryList.Items.Count > 0)
            {
                cbDirectoryList.SelectedIndex = cbDirectoryList.Items.Count - 1;
            }
        }

        private void LoadDirectory(string path)
        {
            currentDirectory = path;
            FillDirectories(currentDirectory);

            List<FtpListItem> list = Client.GetListing(currentDirectory).OrderBy(x => x.Type != FtpFileSystemObjectType.Directory).ThenBy(x => x.Name).ToList();

            if (currentDirectory != Root)
            {
                list.Insert(0, new FtpListItem { FullName = "..", Type = FtpFileSystemObjectType.Link });
            }

            lvFTPList.Items.Clear();
            lvFTPList.SmallImageList = new ImageList { ColorDepth = ColorDepth.Depth32Bit };

            foreach (FtpListItem file in list)
            {
                if (file.Type == FtpFileSystemObjectType.Directory && (file.Name == "." || file.Name == ".."))
                {
                    continue;
                }

                ListViewItem lvi = new ListViewItem(file.Name);

                lvi.Tag = file;

                if (file.Type != FtpFileSystemObjectType.Link)
                {
                    if (file.Type == FtpFileSystemObjectType.File)
                    {
                        lvi.SubItems.Add(file.Size.ToString("N0"));
                    }
                    else
                    {
                        lvi.SubItems.Add(string.Empty);
                    }

                    lvi.SubItems.Add(IconReader.GetDisplayName(file.Name, file.Type == FtpFileSystemObjectType.Directory));
                    lvi.SubItems.Add(file.Modified.ToLocalTime().ToString());
                }

                string ext;
                if (file.Type == FtpFileSystemObjectType.Directory || file.Type == FtpFileSystemObjectType.Link)
                {
                    ext = "Directory";
                }
                else if (Path.HasExtension(file.Name))
                {
                    ext = Path.GetExtension(file.Name);
                }
                else
                {
                    ext = "File";
                }

                if (!lvFTPList.SmallImageList.Images.Keys.Contains(ext))
                {
                    Icon icon;
                    if (ext == "Directory")
                    {
                        icon = IconReader.GetFolderIcon(IconReader.IconSize.Small, IconReader.FolderType.Closed);
                    }
                    else
                    {
                        icon = IconReader.GetFileIcon(ext, IconReader.IconSize.Small, false);
                    }

                    if (icon != null)
                    {
                        lvFTPList.SmallImageList.Images.Add(ext, icon.ToBitmap());
                    }
                }

                if (lvFTPList.SmallImageList.Images.Keys.Contains(ext))
                {
                    lvi.ImageKey = ext;
                }

                lvFTPList.Items.Add(lvi);
            }

            CheckFiles(false);
        }

        private void CheckFiles(bool selected)
        {
            ListViewItem[] list;

            if (selected)
            {
                list = lvFTPList.SelectedItems.Cast<ListViewItem>().ToArray();
            }
            else
            {
                list = lvFTPList.Items.Cast<ListViewItem>().ToArray();
            }

            List<FtpListItem> items = new List<FtpListItem>();
            FtpListItem item;

            foreach (ListViewItem lvi in list)
            {
                item = lvi.Tag as FtpListItem;

                if (item != null)
                {
                    items.Add(item);
                }
            }

            string isSelected = selected ? "Selected " : string.Empty;
            int filesCount = items.Count(x => x.Type == FtpFileSystemObjectType.File);
            string file = filesCount > 1 ? "files" : "file";
            int directoriesCount = items.Count(x => x.Type == FtpFileSystemObjectType.Directory);
            string directory = directoriesCount > 1 ? "directories" : "directory";
            string totalSize = items.Where(x => x.Type == FtpFileSystemObjectType.File).Sum(x => x.Size).ToString("N0");

            lblStatus.Text = string.Format("{0}{1} {2} and {3} {4}. Total size: {5} bytes", isSelected, filesCount, file, directoriesCount, directory, totalSize);
        }

        private void FTPDownload(bool openDirectory)
        {
            if (lvFTPList.SelectedItems.Count > 0)
            {
                FtpListItem checkDirectory = lvFTPList.SelectedItems[0].Tag as FtpListItem;

                if (openDirectory && checkDirectory != null)
                {
                    if (checkDirectory.Type == FtpFileSystemObjectType.Link && checkDirectory.Name == "..")
                    {
                        FTPNavigateBack();
                        return;
                    }

                    if (checkDirectory.Type == FtpFileSystemObjectType.Directory)
                    {
                        LoadDirectory(checkDirectory.FullName);
                        return;
                    }
                }

                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.RootFolder = Environment.SpecialFolder.Desktop;

                if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(fbd.SelectedPath))
                {
                    List<FtpListItem> list = new List<FtpListItem>();
                    foreach (ListViewItem lvi in lvFTPList.SelectedItems)
                    {
                        FtpListItem file = lvi.Tag as FtpListItem;
                        if (file != null)
                        {
                            list.Add(file);
                        }
                    }

                    Client.DownloadFiles(list, fbd.SelectedPath);
                }
            }
        }

        private void FTPRename()
        {
            if (lvFTPList.SelectedItems.Count > 0)
            {
                FtpListItem file = lvFTPList.SelectedItems[0].Tag as FtpListItem;
                if (file != null && file.Type != FtpFileSystemObjectType.Link)
                {
                    lvFTPList.StartEditing(txtRename, lvFTPList.SelectedItems[0], 0);
                    int offset = 23;
                    txtRename.Left += offset;
                    txtRename.Width -= offset;
                }
            }
        }

        private void FTPDelete()
        {
            if (lvFTPList.SelectedItems.Count > 0)
            {
                Client.DeleteFiles(lvFTPList.SelectedItems.Cast<ListViewItem>().Select(x => x.Tag as FtpListItem));
                RefreshDirectory();
            }
        }

        private void FTPCreateDirectory()
        {
            using (InputBox ib = new InputBox { Text = "Create directory", Question = "Please enter the name of the directory which should be created:" })
            {
                ib.ShowDialog();
                BringToFront();
                if (ib.DialogResult == DialogResult.OK)
                {
                    Client.CreateDirectory(Helpers.CombineURL(currentDirectory, ib.InputText));
                    RefreshDirectory();
                }
            }
        }

        private void FTPNavigateBack()
        {
            if (currentDirectory != Root && currentDirectory.Contains('/'))
            {
                LoadDirectory(currentDirectory.Substring(0, currentDirectory.LastIndexOf('/')));
            }
        }

        private string GetURL(FtpListItem file)
        {
            if (file != null && file.Type == FtpFileSystemObjectType.File)
            {
                FTPAccount accountClone = Account.Clone();
                accountClone.SubFolderPath = currentDirectory;
                accountClone.HttpHomePathAutoAddSubFolderPath = true;
                return accountClone.GetUriPath(file.Name);
            }

            return null;
        }

        #endregion Methods

        #region Events

        private void lvFTPList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            FTPDownload(true);
        }

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FTPDownload(false);
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FTPRename();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FTPDelete();
        }

        private void createDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FTPCreateDirectory();
        }

        private void lvFTPList_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (lvFTPList.SelectedItems.Count > 0)
            {
                List<string> filenames = new List<string>();
                foreach (ListViewItem lvi in lvFTPList.SelectedItems)
                {
                    FtpListItem file = lvi.Tag as FtpListItem;
                    if (file != null && !string.IsNullOrEmpty(file.Name))
                    {
                        filenames.Add(file.Name);
                    }
                }

                if (filenames.Count > 0)
                {
                    lvFTPList.DoDragDrop(filenames.ToArray(), DragDropEffects.Move);
                }
            }
        }

        private void lvFTPList_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string[])))
            {
                Point point = lvFTPList.PointToClient(new Point(e.X, e.Y));
                ListViewItem lvi = lvFTPList.GetItemAt(point.X, point.Y);
                if (lvi != null && e.AllowedEffect == DragDropEffects.Move)
                {
                    if (tempSelected != null && tempSelected != lvi)
                    {
                        tempSelected.Selected = false;
                    }

                    FtpListItem file = lvi.Tag as FtpListItem;
                    if (file != null && file.Type == FtpFileSystemObjectType.Directory)
                    {
                        lvi.Selected = true;
                        tempSelected = lvi;
                        e.Effect = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void lvFTPList_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string[])))
            {
                Point point = lvFTPList.PointToClient(new Point(e.X, e.Y));
                ListViewItem lvi = lvFTPList.GetItemAt(point.X, point.Y);
                if (lvi != null && e.AllowedEffect == DragDropEffects.Move)
                {
                    if (tempSelected != null && tempSelected != lvi)
                    {
                        tempSelected.Selected = false;
                    }

                    FtpListItem file = lvi.Tag as FtpListItem;
                    if (file != null && file.Type == FtpFileSystemObjectType.Directory)
                    {
                        string[] filenames = e.Data.GetData(typeof(string[])) as string[];
                        if (filenames != null)
                        {
                            int renameCount = 0;
                            foreach (string filename in filenames)
                            {
                                if (file.Name != filename)
                                {
                                    string path = Helpers.CombineURL(currentDirectory, filename);
                                    string movePath = string.Empty;
                                    if (file.Type == FtpFileSystemObjectType.Link)
                                    {
                                        if (file.Name == ".")
                                        {
                                            movePath = URLHelpers.AddSlash(filename, SlashType.Prefix, 2);
                                        }
                                        else if (file.Name == "..")
                                        {
                                            movePath = URLHelpers.AddSlash(filename, SlashType.Prefix);
                                        }
                                    }
                                    else
                                    {
                                        movePath = Helpers.CombineURL(file.FullName, filename);
                                    }

                                    if (!string.IsNullOrEmpty(movePath))
                                    {
                                        Client.Rename(path, movePath);
                                        renameCount++;
                                    }
                                }
                            }

                            if (renameCount > 0)
                            {
                                RefreshDirectory();
                            }
                        }
                    }
                }

                tempSelected = null;
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files != null)
                {
                    Client.UploadFiles(files, currentDirectory);
                    RefreshDirectory();
                }
            }
        }

        private void lvFTPList_SubItemEndEditing(object sender, SubItemEndEditingEventArgs e)
        {
            if (lvFTPList.SelectedItems.Count > 0 && !e.Cancel && !string.IsNullOrEmpty(e.DisplayText))
            {
                FtpListItem file = (FtpListItem)lvFTPList.SelectedItems[0].Tag;
                if (file.Name != e.DisplayText)
                {
                    Client.Rename(file.FullName, Helpers.CombineURL(currentDirectory, e.DisplayText));
                    RefreshDirectory();
                }
            }
        }

        private void lvFTPList_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool enabled = false;

            if (lvFTPList.SelectedItems.Count > 0)
            {
                FtpListItem file = lvFTPList.SelectedItems[0].Tag as FtpListItem;

                if (file != null)
                {
                    enabled = file.Type != FtpFileSystemObjectType.Link;
                }
            }

            downloadToolStripMenuItem.Enabled = renameToolStripMenuItem.Enabled = deleteToolStripMenuItem.Enabled =
                copyURLsToClipboardToolStripMenuItem.Enabled = openURLToolStripMenuItem.Enabled = enabled;

            CheckFiles(lvFTPList.SelectedItems.Count > 0);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshDirectory();
        }

        private void cbDirectoryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDirectoryList.Items.Count > 0)
            {
                string path = cbDirectoryList.SelectedItem.ToString();
                if (currentDirectory != path)
                {
                    LoadDirectory(path);
                }
            }
        }

        private void copyURLsToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();

            foreach (ListViewItem lvi in lvFTPList.SelectedItems)
            {
                FtpListItem file = lvi.Tag as FtpListItem;
                string url = GetURL(file);
                if (!string.IsNullOrEmpty(url))
                {
                    list.Add(url);
                }
            }

            string clipboard = string.Join("\r\n", list.ToArray());

            if (!string.IsNullOrEmpty(clipboard))
            {
                ClipboardHelpers.CopyText(clipboard);
            }
        }

        private void openURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvFTPList.SelectedItems.Count > 0)
            {
                FtpListItem file = lvFTPList.SelectedItems[0].Tag as FtpListItem;
                string url = GetURL(file);
                Helpers.OpenURL(url);
            }
        }

        private void FTPClient_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshDirectory();
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Client.Disconnect();
            lvFTPList.Items.Clear();
        }

        private void txtConsoleWrite_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Client.SendCommand(txtConsoleWrite.Text);
                txtConsoleWrite.Clear();
            }
        }

        private void FTPClient2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Client != null)
            {
                Client.Dispose();
            }
        }

        #endregion Events
    }
}