#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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

using System;
using System.Drawing;
using System.Windows.Forms;

namespace HistoryLib
{
    public partial class HistoryItemManager
    {
        public ContextMenuStrip cmsHistory;

        private ToolStripMenuItem tsmiOpen;
        private ToolStripMenuItem tsmiOpenURL;
        private ToolStripMenuItem tsmiOpenShortenedURL;
        private ToolStripMenuItem tsmiOpenThumbnailURL;
        private ToolStripMenuItem tsmiOpenDeletionURL;
        private ToolStripSeparator tssOpen1;
        private ToolStripMenuItem tsmiOpenFile;
        private ToolStripMenuItem tsmiOpenFolder;
        private ToolStripMenuItem tsmiCopy;
        private ToolStripMenuItem tsmiCopyURL;
        private ToolStripMenuItem tsmiCopyShortenedURL;
        private ToolStripMenuItem tsmiCopyThumbnailURL;
        private ToolStripMenuItem tsmiCopyDeletionURL;
        private ToolStripSeparator tssCopy1;
        private ToolStripMenuItem tsmiCopyFile;
        private ToolStripMenuItem tsmiCopyImage;
        private ToolStripMenuItem tsmiCopyText;
        private ToolStripSeparator tssCopy2;
        private ToolStripMenuItem tsmiCopyHTMLLink;
        private ToolStripMenuItem tsmiCopyHTMLImage;
        private ToolStripMenuItem tsmiCopyHTMLLinkedImage;
        private ToolStripSeparator tssCopy3;
        private ToolStripMenuItem tsmiCopyForumLink;
        private ToolStripMenuItem tsmiCopyForumImage;
        private ToolStripMenuItem tsmiCopyForumLinkedImage;
        private ToolStripSeparator tssCopy4;
        private ToolStripMenuItem tsmiCopyFilePath;
        private ToolStripMenuItem tsmiCopyFileName;
        private ToolStripMenuItem tsmiCopyFileNameWithExtension;
        private ToolStripMenuItem tsmiCopyFolder;
        private ToolStripMenuItem tsmiShow;
        private ToolStripMenuItem tsmiShowImagePreview;
        private ToolStripMenuItem tsmiShowMoreInfo;

        private void InitializeComponent()
        {
            cmsHistory = new ContextMenuStrip();
            tsmiOpen = new ToolStripMenuItem();
            tsmiOpenURL = new ToolStripMenuItem();
            tsmiOpenShortenedURL = new ToolStripMenuItem();
            tsmiOpenThumbnailURL = new ToolStripMenuItem();
            tsmiOpenDeletionURL = new ToolStripMenuItem();
            tssOpen1 = new ToolStripSeparator();
            tsmiOpenFile = new ToolStripMenuItem();
            tsmiOpenFolder = new ToolStripMenuItem();
            tsmiCopy = new ToolStripMenuItem();
            tsmiCopyURL = new ToolStripMenuItem();
            tsmiCopyShortenedURL = new ToolStripMenuItem();
            tsmiCopyThumbnailURL = new ToolStripMenuItem();
            tsmiCopyDeletionURL = new ToolStripMenuItem();
            tssCopy1 = new ToolStripSeparator();
            tsmiCopyFile = new ToolStripMenuItem();
            tsmiCopyImage = new ToolStripMenuItem();
            tsmiCopyText = new ToolStripMenuItem();
            tssCopy2 = new ToolStripSeparator();
            tsmiCopyHTMLLink = new ToolStripMenuItem();
            tsmiCopyHTMLImage = new ToolStripMenuItem();
            tsmiCopyHTMLLinkedImage = new ToolStripMenuItem();
            tssCopy3 = new ToolStripSeparator();
            tsmiCopyForumLink = new ToolStripMenuItem();
            tsmiCopyForumImage = new ToolStripMenuItem();
            tsmiCopyForumLinkedImage = new ToolStripMenuItem();
            tssCopy4 = new ToolStripSeparator();
            tsmiCopyFilePath = new ToolStripMenuItem();
            tsmiCopyFileName = new ToolStripMenuItem();
            tsmiCopyFileNameWithExtension = new ToolStripMenuItem();
            tsmiCopyFolder = new ToolStripMenuItem();
            tsmiShow = new ToolStripMenuItem();
            tsmiShowImagePreview = new ToolStripMenuItem();
            tsmiShowMoreInfo = new ToolStripMenuItem();
            cmsHistory.SuspendLayout();

            //
            // cmsHistory
            //
            cmsHistory.Items.AddRange(new ToolStripItem[]
            {
                tsmiOpen,
                tsmiCopy,
                tsmiShow
            });
            cmsHistory.Name = "cmsHistory";
            cmsHistory.ShowImageMargin = false;
            cmsHistory.Size = new Size(128, 92);
            cmsHistory.Enabled = false;
            //
            // tsmiOpen
            //
            tsmiOpen.DropDownItems.AddRange(new ToolStripItem[]
            {
                tsmiOpenURL,
                tsmiOpenShortenedURL,
                tsmiOpenThumbnailURL,
                tsmiOpenDeletionURL,
                tssOpen1,
                tsmiOpenFile,
                tsmiOpenFolder
            });
            tsmiOpen.Name = "tsmiOpen";
            tsmiOpen.Size = new Size(127, 22);
            tsmiOpen.Text = "Open";
            //
            // tsmiOpenURL
            //
            tsmiOpenURL.Name = "tsmiOpenURL";
            tsmiOpenURL.Size = new Size(156, 22);
            tsmiOpenURL.Text = "URL";
            tsmiOpenURL.Click += tsmiOpenURL_Click;
            //
            // tsmiOpenShortenedURL
            //
            tsmiOpenShortenedURL.Name = "tsmiOpenShortenedURL";
            tsmiOpenShortenedURL.Size = new Size(156, 22);
            tsmiOpenShortenedURL.Text = "Shortened URL";
            tsmiOpenShortenedURL.Click += tsmiOpenShortenedURL_Click;
            //
            // tsmiOpenThumbnailURL
            //
            tsmiOpenThumbnailURL.Name = "tsmiOpenThumbnailURL";
            tsmiOpenThumbnailURL.Size = new Size(156, 22);
            tsmiOpenThumbnailURL.Text = "Thumbnail URL";
            tsmiOpenThumbnailURL.Click += tsmiOpenThumbnailURL_Click;
            //
            // tsmiOpenDeletionURL
            //
            tsmiOpenDeletionURL.Name = "tsmiOpenDeletionURL";
            tsmiOpenDeletionURL.Size = new Size(156, 22);
            tsmiOpenDeletionURL.Text = "Deletion URL";
            tsmiOpenDeletionURL.Click += tsmiOpenDeletionURL_Click;
            //
            // tssOpen1
            //
            tssOpen1.Name = "tssOpen1";
            tssOpen1.Size = new Size(153, 6);
            //
            // tsmiOpenFile
            //
            tsmiOpenFile.Name = "tsmiOpenFile";
            tsmiOpenFile.Size = new Size(156, 22);
            tsmiOpenFile.Text = "File";
            tsmiOpenFile.Click += tsmiOpenFile_Click;
            //
            // tsmiOpenFolder
            //
            tsmiOpenFolder.Name = "tsmiOpenFolder";
            tsmiOpenFolder.Size = new Size(156, 22);
            tsmiOpenFolder.Text = "Folder";
            tsmiOpenFolder.Click += tsmiOpenFolder_Click;
            //
            // tsmiCopy
            //
            tsmiCopy.DropDownItems.AddRange(new ToolStripItem[]
            {
                tsmiCopyURL,
                tsmiCopyShortenedURL,
                tsmiCopyThumbnailURL,
                tsmiCopyDeletionURL,
                tssCopy1,
                tsmiCopyFile,
                tsmiCopyImage,
                tsmiCopyText,
                tssCopy2,
                tsmiCopyHTMLLink,
                tsmiCopyHTMLImage,
                tsmiCopyHTMLLinkedImage,
                tssCopy3,
                tsmiCopyForumLink,
                tsmiCopyForumImage,
                tsmiCopyForumLinkedImage,
                tssCopy4,
                tsmiCopyFilePath,
                tsmiCopyFileName,
                tsmiCopyFileNameWithExtension,
                tsmiCopyFolder
            });
            tsmiCopy.Name = "tsmiCopy";
            tsmiCopy.Size = new Size(127, 22);
            tsmiCopy.Text = "Copy";
            //
            // tsmiCopyURL
            //
            tsmiCopyURL.Name = "tsmiCopyURL";
            tsmiCopyURL.Size = new Size(233, 22);
            tsmiCopyURL.Text = "URL";
            tsmiCopyURL.Click += tsmiCopyURL_Click;
            //
            // tsmiCopyShortenedURL
            //
            tsmiCopyShortenedURL.Name = "tsmiCopyShortenedURL";
            tsmiCopyShortenedURL.Size = new Size(233, 22);
            tsmiCopyShortenedURL.Text = "Shortened URL";
            tsmiCopyShortenedURL.Click += tsmiCopyShortenedURL_Click;
            //
            // tsmiCopyThumbnailURL
            //
            tsmiCopyThumbnailURL.Name = "tsmiCopyThumbnailURL";
            tsmiCopyThumbnailURL.Size = new Size(233, 22);
            tsmiCopyThumbnailURL.Text = "Thumbnail URL";
            tsmiCopyThumbnailURL.Click += tsmiCopyThumbnailURL_Click;
            //
            // tsmiCopyDeletionURL
            //
            tsmiCopyDeletionURL.Name = "tsmiCopyDeletionURL";
            tsmiCopyDeletionURL.Size = new Size(233, 22);
            tsmiCopyDeletionURL.Text = "Deletion URL";
            tsmiCopyDeletionURL.Click += tsmiCopyDeletionURL_Click;
            //
            // tssCopy1
            //
            tssCopy1.Name = "tssCopy1";
            tssCopy1.Size = new Size(230, 6);
            //
            // tsmiCopyFile
            //
            tsmiCopyFile.Name = "tsmiCopyFile";
            tsmiCopyFile.Size = new Size(233, 22);
            tsmiCopyFile.Text = "File";
            tsmiCopyFile.Click += tsmiCopyFile_Click;
            //
            // tsmiCopyImage
            //
            tsmiCopyImage.Name = "tsmiCopyImage";
            tsmiCopyImage.Size = new Size(233, 22);
            tsmiCopyImage.Text = "Image";
            tsmiCopyImage.Click += tsmiCopyImage_Click;
            //
            // tsmiCopyText
            //
            tsmiCopyText.Name = "tsmiCopyText";
            tsmiCopyText.Size = new Size(233, 22);
            tsmiCopyText.Text = "Text";
            tsmiCopyText.Click += tsmiCopyText_Click;
            //
            // tssCopy2
            //
            tssCopy2.Name = "tssCopy2";
            tssCopy2.Size = new Size(230, 6);
            //
            // tsmiCopyHTMLLink
            //
            tsmiCopyHTMLLink.Name = "tsmiCopyHTMLLink";
            tsmiCopyHTMLLink.Size = new Size(233, 22);
            tsmiCopyHTMLLink.Text = "HTML link";
            tsmiCopyHTMLLink.Click += tsmiCopyHTMLLink_Click;
            //
            // tsmiCopyHTMLImage
            //
            tsmiCopyHTMLImage.Name = "tsmiCopyHTMLImage";
            tsmiCopyHTMLImage.Size = new Size(233, 22);
            tsmiCopyHTMLImage.Text = "HTML image";
            tsmiCopyHTMLImage.Click += tsmiCopyHTMLImage_Click;
            //
            // tsmiCopyHTMLLinkedImage
            //
            tsmiCopyHTMLLinkedImage.Name = "tsmiCopyHTMLLinkedImage";
            tsmiCopyHTMLLinkedImage.Size = new Size(233, 22);
            tsmiCopyHTMLLinkedImage.Text = "HTML linked image";
            tsmiCopyHTMLLinkedImage.Click += tsmiCopyHTMLLinkedImage_Click;
            //
            // tssCopy3
            //
            tssCopy3.Name = "tssCopy3";
            tssCopy3.Size = new Size(230, 6);
            //
            // tsmiCopyForumLink
            //
            tsmiCopyForumLink.Name = "tsmiCopyForumLink";
            tsmiCopyForumLink.Size = new Size(233, 22);
            tsmiCopyForumLink.Text = "Forum (BBCode) link";
            tsmiCopyForumLink.Click += tsmiCopyForumLink_Click;
            //
            // tsmiCopyForumImage
            //
            tsmiCopyForumImage.Name = "tsmiCopyForumImage";
            tsmiCopyForumImage.Size = new Size(233, 22);
            tsmiCopyForumImage.Text = "Forum (BBCode) image";
            tsmiCopyForumImage.Click += tsmiCopyForumImage_Click;
            //
            // tsmiCopyForumLinkedImage
            //
            tsmiCopyForumLinkedImage.Name = "tsmiCopyForumLinkedImage";
            tsmiCopyForumLinkedImage.Size = new Size(233, 22);
            tsmiCopyForumLinkedImage.Text = "Forum (BBCode) linked image";
            tsmiCopyForumLinkedImage.Click += tsmiCopyForumLinkedImage_Click;
            //
            // tssCopy4
            //
            tssCopy4.Name = "tssCopy4";
            tssCopy4.Size = new Size(230, 6);
            //
            // tsmiCopyFilePath
            //
            tsmiCopyFilePath.Name = "tsmiCopyFilePath";
            tsmiCopyFilePath.Size = new Size(233, 22);
            tsmiCopyFilePath.Text = "File path";
            tsmiCopyFilePath.Click += tsmiCopyFilePath_Click;
            //
            // tsmiCopyFileName
            //
            tsmiCopyFileName.Name = "tsmiCopyFileName";
            tsmiCopyFileName.Size = new Size(233, 22);
            tsmiCopyFileName.Text = "File name";
            tsmiCopyFileName.Click += tsmiCopyFileName_Click;
            //
            // tsmiCopyFileNameWithExtension
            //
            tsmiCopyFileNameWithExtension.Name = "tsmiCopyFileNameWithExtension";
            tsmiCopyFileNameWithExtension.Size = new Size(233, 22);
            tsmiCopyFileNameWithExtension.Text = "File name with extension";
            tsmiCopyFileNameWithExtension.Click += tsmiCopyFileNameWithExtension_Click;
            //
            // tsmiCopyFolder
            //
            tsmiCopyFolder.Name = "tsmiCopyFolder";
            tsmiCopyFolder.Size = new Size(233, 22);
            tsmiCopyFolder.Text = "Folder";
            tsmiCopyFolder.Click += tsmiCopyFolder_Click;
            //
            // tsmiShow
            //
            tsmiShow.DropDownItems.AddRange(new ToolStripItem[]
            {
                tsmiShowImagePreview,
                tsmiShowMoreInfo
            });
            tsmiShow.Name = "tsmiShow";
            tsmiShow.Size = new Size(127, 22);
            tsmiShow.Text = "Show";
            //
            // tsmiShowImagePreview
            //
            tsmiShowImagePreview.Name = "tsmiShowImagePreview";
            tsmiShowImagePreview.Size = new Size(127, 22);
            tsmiShowImagePreview.Text = "Image preview";
            tsmiShowImagePreview.Click += tsmiShowImagePreview_Click;
            //
            // tsmiShowMoreInfo
            //
            tsmiShowMoreInfo.Name = "tsmiShowMoreInfo";
            tsmiShowMoreInfo.Size = new Size(127, 22);
            tsmiShowMoreInfo.Text = "More info";
            tsmiShowMoreInfo.Click += tsmiShowMoreInfo_Click;

            cmsHistory.ResumeLayout(false);
        }

        public void UpdateTexts(int itemsCount)
        {
            if (itemsCount > 1)
            {
                tsmiCopyURL.Text = string.Format("URLs ({0})", itemsCount);
            }
            else
            {
                tsmiCopyURL.Text = "URL";
            }
        }

        public void UpdateButtons()
        {
            cmsHistory.SuspendLayout();
            cmsHistory.Enabled = true;

            // Open
            tsmiOpenURL.Enabled = IsURLExist;
            tsmiOpenShortenedURL.Enabled = IsShortenedURLExist;
            tsmiOpenThumbnailURL.Enabled = IsThumbnailURLExist;
            tsmiOpenDeletionURL.Enabled = IsDeletionURLExist;

            tsmiOpenFile.Enabled = IsFileExist;
            tsmiOpenFolder.Enabled = IsFileExist;

            // Copy
            tsmiCopyURL.Enabled = IsURLExist;
            tsmiCopyShortenedURL.Enabled = IsShortenedURLExist;
            tsmiCopyThumbnailURL.Enabled = IsThumbnailURLExist;
            tsmiCopyDeletionURL.Enabled = IsDeletionURLExist;

            tsmiCopyFile.Enabled = IsFileExist;
            tsmiCopyImage.Enabled = IsImageFile;
            tsmiCopyText.Enabled = IsTextFile;

            tsmiCopyHTMLLink.Enabled = IsURLExist;
            tsmiCopyHTMLImage.Enabled = IsImageURL;
            tsmiCopyHTMLLinkedImage.Enabled = IsImageURL && IsThumbnailURLExist;

            tsmiCopyForumLink.Enabled = IsURLExist;
            tsmiCopyForumImage.Enabled = IsImageURL && IsURLExist;
            tsmiCopyForumLinkedImage.Enabled = IsImageURL && IsThumbnailURLExist;

            tsmiCopyFilePath.Enabled = IsFilePathValid;
            tsmiCopyFileName.Enabled = IsFilePathValid;
            tsmiCopyFileNameWithExtension.Enabled = IsFilePathValid;
            tsmiCopyFolder.Enabled = IsFilePathValid;

            // Show
            tsmiShowImagePreview.Enabled = IsImageFile;

            cmsHistory.ResumeLayout();
        }

        private void tsmiOpenURL_Click(object sender, EventArgs e)
        {
            OpenURL();
        }

        private void tsmiOpenShortenedURL_Click(object sender, EventArgs e)
        {
            OpenShortenedURL();
        }

        private void tsmiOpenThumbnailURL_Click(object sender, EventArgs e)
        {
            OpenThumbnailURL();
        }

        private void tsmiOpenDeletionURL_Click(object sender, EventArgs e)
        {
            OpenDeletionURL();
        }

        private void tsmiOpenFile_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void tsmiOpenFolder_Click(object sender, EventArgs e)
        {
            OpenFolder();
        }

        private void tsmiCopyURL_Click(object sender, EventArgs e)
        {
            CopyURL();
        }

        private void tsmiCopyShortenedURL_Click(object sender, EventArgs e)
        {
            CopyShortenedURL();
        }

        private void tsmiCopyThumbnailURL_Click(object sender, EventArgs e)
        {
            CopyThumbnailURL();
        }

        private void tsmiCopyDeletionURL_Click(object sender, EventArgs e)
        {
            CopyDeletionURL();
        }

        private void tsmiCopyFile_Click(object sender, EventArgs e)
        {
            CopyFile();
        }

        private void tsmiCopyImage_Click(object sender, EventArgs e)
        {
            CopyImage();
        }

        private void tsmiCopyText_Click(object sender, EventArgs e)
        {
            CopyText();
        }

        private void tsmiCopyHTMLLink_Click(object sender, EventArgs e)
        {
            CopyHTMLLink();
        }

        private void tsmiCopyHTMLImage_Click(object sender, EventArgs e)
        {
            CopyHTMLImage();
        }

        private void tsmiCopyHTMLLinkedImage_Click(object sender, EventArgs e)
        {
            CopyHTMLLinkedImage();
        }

        private void tsmiCopyForumLink_Click(object sender, EventArgs e)
        {
            CopyForumLink();
        }

        private void tsmiCopyForumImage_Click(object sender, EventArgs e)
        {
            CopyForumImage();
        }

        private void tsmiCopyForumLinkedImage_Click(object sender, EventArgs e)
        {
            CopyForumLinkedImage();
        }

        private void tsmiCopyFilePath_Click(object sender, EventArgs e)
        {
            CopyFilePath();
        }

        private void tsmiCopyFileName_Click(object sender, EventArgs e)
        {
            CopyFileName();
        }

        private void tsmiCopyFileNameWithExtension_Click(object sender, EventArgs e)
        {
            CopyFileNameWithExtension();
        }

        private void tsmiCopyFolder_Click(object sender, EventArgs e)
        {
            CopyFolder();
        }

        private void tsmiShowImagePreview_Click(object sender, EventArgs e)
        {
            ShowImagePreview();
        }

        private void tsmiShowMoreInfo_Click(object sender, EventArgs e)
        {
            ShowMoreInfo();
        }
    }
}