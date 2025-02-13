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
using ShareX.UploadersLib;
using System;
using System.IO;
using System.Windows.Forms;

namespace ShareX
{
    public partial class AfterUploadForm : Form
    {
        public TaskInfo Info { get; private set; }

        private UploadInfoParser parser = new UploadInfoParser();

        private ListViewGroup lvgForums = new ListViewGroup("Forums");
        private ListViewGroup lvgHtml = new ListViewGroup("HTML");
        private ListViewGroup lvgWiki = new ListViewGroup("Wiki");
        private ListViewGroup lvgLocal = new ListViewGroup("Local");
        private ListViewGroup lvgCustom = new ListViewGroup("Custom");

        public AfterUploadForm(TaskInfo info)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            Info = info;

            if (Info.TaskSettings.AdvancedSettings.AutoCloseAfterUploadForm)
            {
                tmrClose.Start();
            }

            bool isFileExist = !string.IsNullOrEmpty(info.FilePath) && File.Exists(info.FilePath);

            if (info.DataType == EDataType.Image)
            {
                if (isFileExist)
                {
                    pbPreview.LoadImageFromFileAsync(info.FilePath);
                }
                else
                {
                    pbPreview.LoadImageFromURLAsync(info.Result.URL);
                }
            }

            Text = "ShareX - " + (isFileExist ? info.FilePath : info.FileName);

            lvClipboardFormats.Groups.Add(lvgForums);
            lvClipboardFormats.Groups.Add(lvgHtml);
            lvClipboardFormats.Groups.Add(lvgWiki);
            lvClipboardFormats.Groups.Add(lvgLocal);
            lvClipboardFormats.Groups.Add(lvgCustom);

            foreach (LinkFormatEnum type in Helpers.GetEnums<LinkFormatEnum>())
            {
                if (!FileHelpers.IsImageFile(Info.Result.URL) &&
                    (type == LinkFormatEnum.HTMLImage || type == LinkFormatEnum.HTMLLinkedImage ||
                    type == LinkFormatEnum.ForumImage || type == LinkFormatEnum.ForumLinkedImage ||
                    type == LinkFormatEnum.WikiImage || type == LinkFormatEnum.WikiLinkedImage))
                    continue;

                AddFormat(type.GetLocalizedDescription(), GetUrlByType(type));
            }

            if (FileHelpers.IsImageFile(Info.Result.URL))
            {
                foreach (ClipboardFormat cf in Program.Settings.ClipboardContentFormats)
                {
                    AddFormat(cf.Description, parser.Parse(Info, cf.Format), lvgCustom);
                }
            }
        }

        private void AddFormat(string description, string text, ListViewGroup group = null)
        {
            if (!string.IsNullOrEmpty(text))
            {
                ListViewItem lvi = new ListViewItem(description);

                if (group == null)
                {
                    if (description.Contains("HTML"))
                    {
                        lvi.Group = lvgHtml;
                    }
                    else if (description.Contains("Forums"))
                    {
                        lvi.Group = lvgForums;
                    }
                    else if (description.Contains("Local"))
                    {
                        lvi.Group = lvgLocal;
                    }
                    else if (description.Contains("Wiki"))
                    {
                        lvi.Group = lvgWiki;
                    }
                }
                else
                {
                    lvi.Group = group;
                }

                lvi.SubItems.Add(text);
                lvClipboardFormats.Items.Add(lvi);
                lvClipboardFormats.FillLastColumn();
            }
        }

        private void tmrClose_Tick(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCopyImage_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Info.FilePath) && FileHelpers.IsImageFile(Info.FilePath) && File.Exists(Info.FilePath))
            {
                ClipboardHelpers.CopyImageFromFile(Info.FilePath);
            }
        }

        private void btnCopyLink_Click(object sender, EventArgs e)
        {
            if (lvClipboardFormats.Items.Count > 0)
            {
                string url;

                if (lvClipboardFormats.SelectedItems.Count == 0)
                {
                    url = lvClipboardFormats.Items[0].SubItems[1].Text;
                }
                else
                {
                    url = lvClipboardFormats.SelectedItems[0].SubItems[1].Text;
                }

                if (!string.IsNullOrEmpty(url))
                {
                    ClipboardHelpers.CopyText(url);
                }
            }
        }

        private void btnOpenLink_Click(object sender, EventArgs e)
        {
            string url = Info.Result.URL;

            if (!string.IsNullOrEmpty(url))
            {
                URLHelpers.OpenURL(url);
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            FileHelpers.OpenFile(Info.FilePath);
        }

        private void btnFolderOpen_Click(object sender, EventArgs e)
        {
            FileHelpers.OpenFolderWithFile(Info.FilePath);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #region TaskInfo helper methods

        public string GetUrlByType(LinkFormatEnum type)
        {
            switch (type)
            {
                case LinkFormatEnum.URL:
                    return Info.Result.URL;
                case LinkFormatEnum.ShortenedURL:
                    return Info.Result.ShortenedURL;
                case LinkFormatEnum.ForumImage:
                    return parser.Parse(Info, UploadInfoParser.ForumImage);
                case LinkFormatEnum.HTMLImage:
                    return parser.Parse(Info, UploadInfoParser.HTMLImage);
                case LinkFormatEnum.WikiImage:
                    return parser.Parse(Info, UploadInfoParser.WikiImage);
                case LinkFormatEnum.ForumLinkedImage:
                    return parser.Parse(Info, UploadInfoParser.ForumLinkedImage);
                case LinkFormatEnum.HTMLLinkedImage:
                    return parser.Parse(Info, UploadInfoParser.HTMLLinkedImage);
                case LinkFormatEnum.WikiLinkedImage:
                    return parser.Parse(Info, UploadInfoParser.WikiLinkedImage);
                case LinkFormatEnum.ThumbnailURL:
                    return Info.Result.ThumbnailURL;
                case LinkFormatEnum.LocalFilePath:
                    return Info.FilePath;
                case LinkFormatEnum.LocalFilePathUri:
                    return GetLocalFilePathAsUri(Info.FilePath);
            }

            return Info.Result.URL;
        }

        public string GetLocalFilePathAsUri(string fp)
        {
            if (!string.IsNullOrEmpty(fp) && File.Exists(fp))
            {
                try
                {
                    return new Uri(fp).AbsoluteUri;
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                }
            }

            return "";
        }

        #endregion TaskInfo helper methods

        private void lvClipboardFormats_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && lvClipboardFormats.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvClipboardFormats.SelectedItems[0];
                string txt = lvi.SubItems[1].Text;
                if (!string.IsNullOrEmpty(txt))
                {
                    ClipboardHelpers.CopyText(txt);
                }
            }
        }
    }
}