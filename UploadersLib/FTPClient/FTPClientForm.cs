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
using Starksoft.Net.Ftp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace UploadersLib
{
    public partial class FTPClientForm : Form
    {
        private const string Root = "/";

        public FTP FTPAdapter { get; set; }
        public FTPAccount Account { get; set; }

        private string currentDirectory;
        private ListViewItem tempSelected;

        public FTPClientForm(FTPAccount account)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            lblStatus.Text = string.Empty;
            lvFTPList.SubItemEndEditing += lvFTPList_SubItemEndEditing;

            Account = account;

            FTPAdapter = new FTP(account);
            FTPAdapter.Client.ClientRequest += Client_ClientRequest;
            FTPAdapter.Client.ServerResponse += Client_ServerResponse;
            FTPAdapter.Client.OpenAsyncCompleted += Client_OpenAsyncCompleted;

            pgAccount.SelectedObject = FTPAdapter.Account;
            Text = "FTP Client - " + account.Name;
            lblConnecting.Text = "Connecting to " + account.FTPAddress;

            FTPAdapter.Client.OpenAsync(account.Username, account.Password);
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
            List<string> paths = FTPHelpers.GetPaths(path);
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

            List<FtpItem> list = FTPAdapter.GetDirList(currentDirectory).
                OrderBy(x => x.ItemType != FtpItemType.Directory).ThenBy(x => x.Name).ToList();

            list.Insert(0, new FtpItem("..", DateTime.Now, 0, null, null, FtpItemType.Unknown, null));

            lvFTPList.Items.Clear();
            lvFTPList.SmallImageList = new ImageList { ColorDepth = ColorDepth.Depth32Bit };

            foreach (FtpItem file in list)
            {
                if (file.ItemType == FtpItemType.Directory && (file.Name == "." || file.Name == ".."))
                {
                    continue;
                }

                ListViewItem lvi = new ListViewItem(file.Name);

                lvi.Tag = file;

                if (file.ItemType != FtpItemType.Unknown)
                {
                    if (file.ItemType == FtpItemType.File)
                    {
                        lvi.SubItems.Add(file.Size.ToString("N0"));
                    }
                    else
                    {
                        lvi.SubItems.Add(string.Empty);
                    }

                    lvi.SubItems.Add(IconReader.GetDisplayName(file.Name, file.ItemType == FtpItemType.Directory));
                    lvi.SubItems.Add(file.Modified.ToLocalTime().ToString());
                    lvi.SubItems.Add(file.Attributes);
                }

                string ext;
                if (file.ItemType == FtpItemType.Directory || file.ItemType == FtpItemType.Unknown)
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

            List<FtpItem> items = new List<FtpItem>();
            FtpItem item;

            foreach (ListViewItem lvi in list)
            {
                item = lvi.Tag as FtpItem;

                if (item != null)
                {
                    items.Add(item);
                }
            }

            string isSelected = selected ? "Selected " : string.Empty;
            int filesCount = items.Count(x => x.ItemType == FtpItemType.File);
            string file = filesCount > 1 ? "files" : "file";
            int directoriesCount = items.Count(x => x.ItemType == FtpItemType.Directory);
            string directory = directoriesCount > 1 ? "directories" : "directory";
            string totalSize = items.Where(x => x.ItemType == FtpItemType.File).Sum(x => x.Size).ToString("N0");

            lblStatus.Text = string.Format("{0}{1} {2} and {3} {4}. Total size: {5} bytes",
                isSelected, filesCount, file, directoriesCount, directory, totalSize);
        }

        private void FTPDownload(bool openDirectory)
        {
            if (lvFTPList.SelectedItems.Count > 0)
            {
                FtpItem checkDirectory = lvFTPList.SelectedItems[0].Tag as FtpItem;

                if (openDirectory && checkDirectory != null)
                {
                    if (checkDirectory.ItemType == FtpItemType.Unknown && checkDirectory.Name == "..")
                    {
                        FTPNavigateBack();
                        return;
                    }

                    if (checkDirectory.ItemType == FtpItemType.Directory)
                    {
                        LoadDirectory(checkDirectory.FullPath);
                        return;
                    }
                }

                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.RootFolder = Environment.SpecialFolder.Desktop;

                if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(fbd.SelectedPath))
                {
                    FtpItemCollection list = new FtpItemCollection();
                    foreach (ListViewItem lvi in lvFTPList.SelectedItems)
                    {
                        FtpItem file = lvi.Tag as FtpItem;
                        if (file != null)
                        {
                            list.Add(file);
                        }
                    }

                    FTPAdapter.DownloadFiles(list, fbd.SelectedPath);
                }
            }
        }

        private void FTPRename()
        {
            if (lvFTPList.SelectedItems.Count > 0)
            {
                FtpItem file = lvFTPList.SelectedItems[0].Tag as FtpItem;
                if (file != null && file.ItemType != FtpItemType.Unknown)
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
                FTPAdapter.DeleteFiles(lvFTPList.SelectedItems.Cast<ListViewItem>().Select(x => x.Tag as FtpItem));
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
                    FTPAdapter.MakeDirectory(Helpers.CombineURL(currentDirectory, ib.InputText));
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

        private void AddConsoleMessage(string text, Color color)
        {
            this.InvokeSafe(() =>
            {
                text = string.Format("{0} - {1}\r\n", DateTime.Now.ToLongTimeString(), text);
                rtbConsole.AppendText(text);
                rtbConsole.SelectionStart = rtbConsole.TextLength - text.Length + 1;
                rtbConsole.SelectionLength = text.Length;
                rtbConsole.SelectionColor = color;
                rtbConsole.ScrollToCaret();
            });
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
                    FtpItem file = lvi.Tag as FtpItem;
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

                    FtpItem file = lvi.Tag as FtpItem;
                    if (file != null && file.ItemType == FtpItemType.Directory)
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

                    FtpItem file = lvi.Tag as FtpItem;
                    if (file != null && file.ItemType == FtpItemType.Directory)
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
                                    if (file.ItemType == FtpItemType.Unknown)
                                    {
                                        if (file.Name == ".")
                                        {
                                            movePath = FTPHelpers.AddSlash(filename, FTPHelpers.SlashType.Prefix, 2);
                                        }
                                        else if (file.Name == "..")
                                        {
                                            movePath = FTPHelpers.AddSlash(filename, FTPHelpers.SlashType.Prefix);
                                        }
                                    }
                                    else
                                    {
                                        movePath = Helpers.CombineURL(file.FullPath, filename);
                                    }

                                    if (!string.IsNullOrEmpty(movePath))
                                    {
                                        FTPAdapter.Rename(path, movePath);
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
                    FTPAdapter.UploadFiles(files, currentDirectory);
                    RefreshDirectory();
                }
            }
        }

        private void lvFTPList_SubItemEndEditing(object sender, SubItemEndEditingEventArgs e)
        {
            if (lvFTPList.SelectedItems.Count > 0 && !e.Cancel && !string.IsNullOrEmpty(e.DisplayText))
            {
                FtpItem file = (FtpItem)lvFTPList.SelectedItems[0].Tag;
                if (file.Name != e.DisplayText)
                {
                    FTPAdapter.Rename(file.FullPath, Helpers.CombineURL(currentDirectory, e.DisplayText));
                    RefreshDirectory();
                }
            }
        }

        private void lvFTPList_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool enabled = false;

            if (lvFTPList.SelectedItems.Count > 0)
            {
                FtpItem file = lvFTPList.SelectedItems[0].Tag as FtpItem;

                if (file != null)
                {
                    enabled = file.ItemType != FtpItemType.Unknown;
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
            string path;
            List<string> list = new List<string>();

            foreach (ListViewItem lvi in lvFTPList.SelectedItems)
            {
                FtpItem file = lvi.Tag as FtpItem;
                if (file != null && file.ItemType == FtpItemType.File)
                {
                    path = Helpers.CombineURL(Account.HttpHomePath, file.FullPath);
                    list.Add(path);
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
                FtpItem file = lvFTPList.SelectedItems[0].Tag as FtpItem;
                if (file != null && file.ItemType == FtpItemType.File)
                {
                    Task.Run(() => Process.Start(Account.GetUriPath("@" + file.FullPath)));
                }
            }
        }

        private void FTPClient_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Client_OpenAsyncCompleted(object sender, OpenAsyncCompletedEventArgs e)
        {
            pConnecting.Visible = false;
            Refresh();
            RefreshDirectory();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshDirectory();
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FTPAdapter.Disconnect();
            lvFTPList.Items.Clear();
        }

        private void Client_ClientRequest(object sender, FtpRequestEventArgs e)
        {
            AddConsoleMessage(e.Request.Text, Color.Blue);
        }

        private void Client_ServerResponse(object sender, FtpResponseEventArgs e)
        {
            AddConsoleMessage(e.Response.RawText, Color.DarkGreen);
        }

        private void txtConsoleWrite_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FTPAdapter.SendCommand(txtConsoleWrite.Text);
                txtConsoleWrite.Clear();
            }
        }

        private void FTPClient2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FTPAdapter != null)
            {
                FTPAdapter.Dispose();
            }
        }

        #endregion Events
    }
}