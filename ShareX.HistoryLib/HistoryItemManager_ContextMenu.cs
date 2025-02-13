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

using ShareX.HistoryLib.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HistoryLib
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
        private ToolStripMenuItem tsmiCopyMarkdownLink;
        private ToolStripMenuItem tsmiCopyMarkdownImage;
        private ToolStripMenuItem tsmiCopyMarkdownLinkedImage;
        private ToolStripSeparator tssCopy5;
        private ToolStripMenuItem tsmiCopyFilePath;
        private ToolStripMenuItem tsmiCopyFileName;
        private ToolStripMenuItem tsmiCopyFileNameWithExtension;
        private ToolStripMenuItem tsmiCopyFolder;

        private ToolStripMenuItem tsmiShowImagePreview;
        private ToolStripMenuItem tsmiUploadFile;
        private ToolStripMenuItem tsmiEditImage;
        private ToolStripMenuItem tsmiPinToScreen;
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
            tsmiCopyMarkdownLink = new ToolStripMenuItem();
            tsmiCopyMarkdownImage = new ToolStripMenuItem();
            tsmiCopyMarkdownLinkedImage = new ToolStripMenuItem();
            tssCopy5 = new ToolStripSeparator();
            tsmiCopyFilePath = new ToolStripMenuItem();
            tsmiCopyFileName = new ToolStripMenuItem();
            tsmiCopyFileNameWithExtension = new ToolStripMenuItem();
            tsmiCopyFolder = new ToolStripMenuItem();

            tsmiShowImagePreview = new ToolStripMenuItem();
            tsmiUploadFile = new ToolStripMenuItem();
            tsmiEditImage = new ToolStripMenuItem();
            tsmiPinToScreen = new ToolStripMenuItem();
            tsmiShowMoreInfo = new ToolStripMenuItem();

            cmsHistory.SuspendLayout();

            //
            // cmsHistory
            //
            cmsHistory.Items.AddRange(new ToolStripItem[]
            {
                tsmiOpen,
                tsmiCopy,
                tsmiShowImagePreview,
                tsmiUploadFile,
                tsmiEditImage,
                tsmiPinToScreen,
                tsmiShowMoreInfo
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
            tsmiOpen.Text = Resources.HistoryItemManager_InitializeComponent_Open;
            //
            // tsmiOpenURL
            //
            tsmiOpenURL.Name = "tsmiOpenURL";
            tsmiOpenURL.ShortcutKeyDisplayString = "Enter";
            tsmiOpenURL.Size = new Size(156, 22);
            tsmiOpenURL.Text = Resources.HistoryItemManager_InitializeComponent_URL;
            tsmiOpenURL.Click += tsmiOpenURL_Click;
            //
            // tsmiOpenShortenedURL
            //
            tsmiOpenShortenedURL.Name = "tsmiOpenShortenedURL";
            tsmiOpenShortenedURL.Size = new Size(156, 22);
            tsmiOpenShortenedURL.Text = Resources.HistoryItemManager_InitializeComponent_Shortened_URL;
            tsmiOpenShortenedURL.Click += tsmiOpenShortenedURL_Click;
            //
            // tsmiOpenThumbnailURL
            //
            tsmiOpenThumbnailURL.Name = "tsmiOpenThumbnailURL";
            tsmiOpenThumbnailURL.Size = new Size(156, 22);
            tsmiOpenThumbnailURL.Text = Resources.HistoryItemManager_InitializeComponent_Thumbnail_URL;
            tsmiOpenThumbnailURL.Click += tsmiOpenThumbnailURL_Click;
            //
            // tsmiOpenDeletionURL
            //
            tsmiOpenDeletionURL.Name = "tsmiOpenDeletionURL";
            tsmiOpenDeletionURL.Size = new Size(156, 22);
            tsmiOpenDeletionURL.Text = Resources.HistoryItemManager_InitializeComponent_Deletion_URL;
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
            tsmiOpenFile.ShortcutKeyDisplayString = "Ctrl+Enter";
            tsmiOpenFile.Size = new Size(156, 22);
            tsmiOpenFile.Text = Resources.HistoryItemManager_InitializeComponent_File;
            tsmiOpenFile.Click += tsmiOpenFile_Click;
            //
            // tsmiOpenFolder
            //
            tsmiOpenFolder.Name = "tsmiOpenFolder";
            tsmiOpenFolder.ShortcutKeyDisplayString = "Shift+Enter";
            tsmiOpenFolder.Size = new Size(156, 22);
            tsmiOpenFolder.Text = Resources.HistoryItemManager_InitializeComponent_Folder;
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
                tsmiCopyMarkdownLink,
                tsmiCopyMarkdownImage,
                tsmiCopyMarkdownLinkedImage,
                tssCopy5,
                tsmiCopyFilePath,
                tsmiCopyFileName,
                tsmiCopyFileNameWithExtension,
                tsmiCopyFolder
            });
            tsmiCopy.Name = "tsmiCopy";
            tsmiCopy.Size = new Size(127, 22);
            tsmiCopy.Text = Resources.HistoryItemManager_InitializeComponent_Copy;
            //
            // tsmiCopyURL
            //
            tsmiCopyURL.Name = "tsmiCopyURL";
            tsmiCopyURL.ShortcutKeyDisplayString = "Ctrl+C";
            tsmiCopyURL.Size = new Size(233, 22);
            tsmiCopyURL.Text = Resources.HistoryItemManager_InitializeComponent_URL;
            tsmiCopyURL.Click += tsmiCopyURL_Click;
            //
            // tsmiCopyShortenedURL
            //
            tsmiCopyShortenedURL.Name = "tsmiCopyShortenedURL";
            tsmiCopyShortenedURL.Size = new Size(233, 22);
            tsmiCopyShortenedURL.Text = Resources.HistoryItemManager_InitializeComponent_Shortened_URL;
            tsmiCopyShortenedURL.Click += tsmiCopyShortenedURL_Click;
            //
            // tsmiCopyThumbnailURL
            //
            tsmiCopyThumbnailURL.Name = "tsmiCopyThumbnailURL";
            tsmiCopyThumbnailURL.Size = new Size(233, 22);
            tsmiCopyThumbnailURL.Text = Resources.HistoryItemManager_InitializeComponent_Thumbnail_URL;
            tsmiCopyThumbnailURL.Click += tsmiCopyThumbnailURL_Click;
            //
            // tsmiCopyDeletionURL
            //
            tsmiCopyDeletionURL.Name = "tsmiCopyDeletionURL";
            tsmiCopyDeletionURL.Size = new Size(233, 22);
            tsmiCopyDeletionURL.Text = Resources.HistoryItemManager_InitializeComponent_Deletion_URL;
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
            tsmiCopyFile.ShortcutKeyDisplayString = "Shift+C";
            tsmiCopyFile.Size = new Size(233, 22);
            tsmiCopyFile.Text = Resources.HistoryItemManager_InitializeComponent_File;
            tsmiCopyFile.Click += tsmiCopyFile_Click;
            //
            // tsmiCopyImage
            //
            tsmiCopyImage.Name = "tsmiCopyImage";
            tsmiCopyImage.ShortcutKeyDisplayString = "Alt+C";
            tsmiCopyImage.Size = new Size(233, 22);
            tsmiCopyImage.Text = Resources.HistoryItemManager_InitializeComponent_Image;
            tsmiCopyImage.Click += tsmiCopyImage_Click;
            //
            // tsmiCopyText
            //
            tsmiCopyText.Name = "tsmiCopyText";
            tsmiCopyText.Size = new Size(233, 22);
            tsmiCopyText.Text = Resources.HistoryItemManager_InitializeComponent_Text;
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
            tsmiCopyHTMLLink.Text = Resources.HistoryItemManager_InitializeComponent_HTML_link;
            tsmiCopyHTMLLink.Click += tsmiCopyHTMLLink_Click;
            //
            // tsmiCopyHTMLImage
            //
            tsmiCopyHTMLImage.Name = "tsmiCopyHTMLImage";
            tsmiCopyHTMLImage.Size = new Size(233, 22);
            tsmiCopyHTMLImage.Text = Resources.HistoryItemManager_InitializeComponent_HTML_image;
            tsmiCopyHTMLImage.Click += tsmiCopyHTMLImage_Click;
            //
            // tsmiCopyHTMLLinkedImage
            //
            tsmiCopyHTMLLinkedImage.Name = "tsmiCopyHTMLLinkedImage";
            tsmiCopyHTMLLinkedImage.Size = new Size(233, 22);
            tsmiCopyHTMLLinkedImage.Text = Resources.HistoryItemManager_InitializeComponent_HTML_linked_image;
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
            tsmiCopyForumLink.Text = Resources.HistoryItemManager_InitializeComponent_Forum__BBCode__link;
            tsmiCopyForumLink.Click += tsmiCopyForumLink_Click;
            //
            // tsmiCopyForumImage
            //
            tsmiCopyForumImage.Name = "tsmiCopyForumImage";
            tsmiCopyForumImage.Size = new Size(233, 22);
            tsmiCopyForumImage.Text = Resources.HistoryItemManager_InitializeComponent_Forum__BBCode__image;
            tsmiCopyForumImage.Click += tsmiCopyForumImage_Click;
            //
            // tsmiCopyForumLinkedImage
            //
            tsmiCopyForumLinkedImage.Name = "tsmiCopyForumLinkedImage";
            tsmiCopyForumLinkedImage.Size = new Size(233, 22);
            tsmiCopyForumLinkedImage.Text = Resources.HistoryItemManager_InitializeComponent_Forum__BBCode__linked_image;
            tsmiCopyForumLinkedImage.Click += tsmiCopyForumLinkedImage_Click;
            //
            // tssCopy4
            //
            tssCopy4.Name = "tssCopy4";
            tssCopy4.Size = new Size(230, 6);
            //
            // tsmiCopyMarkdownLink
            //
            tsmiCopyMarkdownLink.Name = "tsmiCopyMarkdownLink";
            tsmiCopyMarkdownLink.Size = new Size(233, 22);
            tsmiCopyMarkdownLink.Text = Resources.HistoryItemManager_InitializeComponent_Markdown__link;
            tsmiCopyMarkdownLink.Click += tsmiCopyMarkdownLink_Click;
            //
            // tsmiCopyMarkdownImage
            //
            tsmiCopyMarkdownImage.Name = "tsmiCopyMarkdownImage";
            tsmiCopyMarkdownImage.Size = new Size(233, 22);
            tsmiCopyMarkdownImage.Text = Resources.HistoryItemManager_InitializeComponent_Markdown__image;
            tsmiCopyMarkdownImage.Click += tsmiCopyMarkdownImage_Click;
            //
            // tsmiCopyMarkdownLinkedImage
            //
            tsmiCopyMarkdownLinkedImage.Name = "tsmiCopyMarkdownLinkedImage";
            tsmiCopyMarkdownLinkedImage.Size = new Size(233, 22);
            tsmiCopyMarkdownLinkedImage.Text = Resources.HistoryItemManager_InitializeComponent_Markdown__linked_image;
            tsmiCopyMarkdownLinkedImage.Click += tsmiCopyMarkdownLinkedImage_Click;
            //
            // tssCopy5
            //
            tssCopy5.Name = "tssCopy5";
            tssCopy5.Size = new Size(230, 6);
            //
            // tsmiCopyFilePath
            //
            tsmiCopyFilePath.Name = "tsmiCopyFilePath";
            tsmiCopyFilePath.ShortcutKeyDisplayString = "Ctrl+Shift+C";
            tsmiCopyFilePath.Size = new Size(233, 22);
            tsmiCopyFilePath.Text = Resources.HistoryItemManager_InitializeComponent_File_path;
            tsmiCopyFilePath.Click += tsmiCopyFilePath_Click;
            //
            // tsmiCopyFileName
            //
            tsmiCopyFileName.Name = "tsmiCopyFileName";
            tsmiCopyFileName.Size = new Size(233, 22);
            tsmiCopyFileName.Text = Resources.HistoryItemManager_InitializeComponent_File_name;
            tsmiCopyFileName.Click += tsmiCopyFileName_Click;
            //
            // tsmiCopyFileNameWithExtension
            //
            tsmiCopyFileNameWithExtension.Name = "tsmiCopyFileNameWithExtension";
            tsmiCopyFileNameWithExtension.Size = new Size(233, 22);
            tsmiCopyFileNameWithExtension.Text = Resources.HistoryItemManager_InitializeComponent_File_name_with_extension;
            tsmiCopyFileNameWithExtension.Click += tsmiCopyFileNameWithExtension_Click;
            //
            // tsmiCopyFolder
            //
            tsmiCopyFolder.Name = "tsmiCopyFolder";
            tsmiCopyFolder.Size = new Size(233, 22);
            tsmiCopyFolder.Text = Resources.HistoryItemManager_InitializeComponent_Folder;
            tsmiCopyFolder.Click += tsmiCopyFolder_Click;
            //
            // tsmiShowImagePreview
            //
            tsmiShowImagePreview.Name = "tsmiShowImagePreview";
            tsmiShowImagePreview.Size = new Size(127, 22);
            tsmiShowImagePreview.Text = Resources.HistoryItemManager_InitializeComponent_Image_preview;
            tsmiShowImagePreview.Click += tsmiShowImagePreview_Click;
            //
            // tsmiUploadFile
            //
            tsmiUploadFile.Name = "tsmiUploadFile";
            tsmiUploadFile.ShortcutKeyDisplayString = "Ctrl+U";
            tsmiUploadFile.Size = new Size(127, 22);
            tsmiUploadFile.Text = Resources.HistoryItemManager_InitializeComponent_UploadFile;
            tsmiUploadFile.Click += tsmiUploadFile_Click;
            //
            // tsmiEditImage
            //
            tsmiEditImage.Name = "tsmiEditImage";
            tsmiEditImage.ShortcutKeyDisplayString = "Ctrl+E";
            tsmiEditImage.Size = new Size(127, 22);
            tsmiEditImage.Text = Resources.HistoryItemManager_InitializeComponent_EditImage;
            tsmiEditImage.Click += tsmiEditImage_Click;
            //
            // tsmiPinToScreen
            //
            tsmiPinToScreen.Name = "tsmiPinToScreen";
            tsmiPinToScreen.ShortcutKeyDisplayString = "Ctrl+P";
            tsmiPinToScreen.Size = new Size(127, 22);
            tsmiPinToScreen.Text = Resources.PinToScreen;
            tsmiPinToScreen.Click += tsmiPinToScreen_Click;
            //
            // tsmiShowMoreInfo
            //
            tsmiShowMoreInfo.Name = "tsmiShowMoreInfo";
            tsmiShowMoreInfo.Size = new Size(127, 22);
            tsmiShowMoreInfo.Text = Resources.HistoryItemManager_InitializeComponent_More_info;
            tsmiShowMoreInfo.Click += tsmiShowMoreInfo_Click;

            cmsHistory.ResumeLayout(false);
        }

        public void UpdateContextMenu(int itemCount)
        {
            cmsHistory.SuspendLayout();
            cmsHistory.Enabled = true;

            if (itemCount > 1)
            {
                // Open
                tsmiOpenURL.Enabled = false;
                tsmiOpenShortenedURL.Enabled = false;
                tsmiOpenThumbnailURL.Enabled = false;
                tsmiOpenDeletionURL.Enabled = false;

                tsmiOpenFile.Enabled = false;
                tsmiOpenFolder.Enabled = false;

                // Copy
                tsmiCopyURL.Enabled = true;
                tsmiCopyShortenedURL.Enabled = true;
                tsmiCopyThumbnailURL.Enabled = true;
                tsmiCopyDeletionURL.Enabled = true;

                tsmiCopyFile.Enabled = true;
                tsmiCopyImage.Enabled = false;
                tsmiCopyText.Enabled = false;

                tsmiCopyHTMLLink.Enabled = true;
                tsmiCopyHTMLImage.Enabled = true;
                tsmiCopyHTMLLinkedImage.Enabled = true;

                tsmiCopyForumLink.Enabled = true;
                tsmiCopyForumImage.Enabled = true;
                tsmiCopyForumLinkedImage.Enabled = true;

                tsmiCopyMarkdownLink.Enabled = true;
                tsmiCopyMarkdownImage.Enabled = true;
                tsmiCopyMarkdownLinkedImage.Enabled = true;

                tsmiCopyFilePath.Enabled = true;
                tsmiCopyFileName.Enabled = true;
                tsmiCopyFileNameWithExtension.Enabled = true;
                tsmiCopyFolder.Enabled = true;

                tsmiCopyFile.Text = Resources.HistoryItemManager_InitializeComponent_File + " (" + itemCount + ")";
                tsmiCopyURL.Text = Resources.HistoryItemManager_InitializeComponent_URL + " (" + itemCount + ")";
                tsmiCopyShortenedURL.Text = Resources.HistoryItemManager_InitializeComponent_Shortened_URL + " (" + itemCount + ")";
                tsmiCopyThumbnailURL.Text = Resources.HistoryItemManager_InitializeComponent_Thumbnail_URL + " (" + itemCount + ")";
                tsmiCopyDeletionURL.Text = Resources.HistoryItemManager_InitializeComponent_Deletion_URL + " (" + itemCount + ")";
                tsmiCopyHTMLLink.Text = Resources.HistoryItemManager_InitializeComponent_HTML_link + " (" + itemCount + ")";
                tsmiCopyHTMLImage.Text = Resources.HistoryItemManager_InitializeComponent_HTML_image + " (" + itemCount + ")";
                tsmiCopyHTMLLinkedImage.Text = Resources.HistoryItemManager_InitializeComponent_HTML_linked_image + " (" + itemCount + ")";
                tsmiCopyHTMLLinkedImage.Text = Resources.HistoryItemManager_InitializeComponent_HTML_linked_image + " (" + itemCount + ")";
                tsmiCopyForumLink.Text = Resources.HistoryItemManager_InitializeComponent_Forum__BBCode__link + " (" + itemCount + ")";
                tsmiCopyForumImage.Text = Resources.HistoryItemManager_InitializeComponent_Forum__BBCode__image + " (" + itemCount + ")";
                tsmiCopyForumLinkedImage.Text = Resources.HistoryItemManager_InitializeComponent_Forum__BBCode__linked_image + " (" + itemCount + ")";
                tsmiCopyMarkdownLink.Text = Resources.HistoryItemManager_InitializeComponent_Markdown__link + " (" + itemCount + ")";
                tsmiCopyMarkdownImage.Text = Resources.HistoryItemManager_InitializeComponent_Markdown__image + " (" + itemCount + ")";
                tsmiCopyMarkdownLinkedImage.Text = Resources.HistoryItemManager_InitializeComponent_Markdown__linked_image + " (" + itemCount + ")";
                tsmiCopyFilePath.Text = Resources.HistoryItemManager_InitializeComponent_File_path + " (" + itemCount + ")";
                tsmiCopyFileName.Text = Resources.HistoryItemManager_InitializeComponent_File_name + " (" + itemCount + ")";
                tsmiCopyFileNameWithExtension.Text = Resources.HistoryItemManager_InitializeComponent_File_name_with_extension + " (" + itemCount + ")";
                tsmiCopyFolder.Text = Resources.HistoryItemManager_InitializeComponent_Folder + " (" + itemCount + ")";

                // Other
                tsmiShowImagePreview.Enabled = false;
                tsmiUploadFile.Enabled = false;
                tsmiEditImage.Enabled = false;
                tsmiPinToScreen.Enabled = false;
                tsmiShowMoreInfo.Enabled = false;
            }
            else
            {
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

                tsmiCopyMarkdownLink.Enabled = IsURLExist;
                tsmiCopyMarkdownImage.Enabled = IsImageURL && IsURLExist;
                tsmiCopyMarkdownLinkedImage.Enabled = IsImageURL && IsThumbnailURLExist;

                tsmiCopyFilePath.Enabled = IsFilePathValid;
                tsmiCopyFileName.Enabled = IsFilePathValid;
                tsmiCopyFileNameWithExtension.Enabled = IsFilePathValid;
                tsmiCopyFolder.Enabled = IsFilePathValid;

                tsmiCopyFile.Text = Resources.HistoryItemManager_InitializeComponent_File;
                tsmiCopyURL.Text = Resources.HistoryItemManager_InitializeComponent_URL;
                tsmiCopyShortenedURL.Text = Resources.HistoryItemManager_InitializeComponent_Shortened_URL;
                tsmiCopyThumbnailURL.Text = Resources.HistoryItemManager_InitializeComponent_Thumbnail_URL;
                tsmiCopyDeletionURL.Text = Resources.HistoryItemManager_InitializeComponent_Deletion_URL;
                tsmiCopyHTMLLink.Text = Resources.HistoryItemManager_InitializeComponent_HTML_link;
                tsmiCopyHTMLImage.Text = Resources.HistoryItemManager_InitializeComponent_HTML_image;
                tsmiCopyHTMLLinkedImage.Text = Resources.HistoryItemManager_InitializeComponent_HTML_linked_image;
                tsmiCopyHTMLLinkedImage.Text = Resources.HistoryItemManager_InitializeComponent_HTML_linked_image;
                tsmiCopyForumLink.Text = Resources.HistoryItemManager_InitializeComponent_Forum__BBCode__link;
                tsmiCopyForumImage.Text = Resources.HistoryItemManager_InitializeComponent_Forum__BBCode__image;
                tsmiCopyForumLinkedImage.Text = Resources.HistoryItemManager_InitializeComponent_Forum__BBCode__linked_image;
                tsmiCopyMarkdownLink.Text = Resources.HistoryItemManager_InitializeComponent_Markdown__link;
                tsmiCopyMarkdownImage.Text = Resources.HistoryItemManager_InitializeComponent_Markdown__image;
                tsmiCopyMarkdownLinkedImage.Text = Resources.HistoryItemManager_InitializeComponent_Markdown__linked_image;
                tsmiCopyFilePath.Text = Resources.HistoryItemManager_InitializeComponent_File_path;
                tsmiCopyFileName.Text = Resources.HistoryItemManager_InitializeComponent_File_name;
                tsmiCopyFileNameWithExtension.Text = Resources.HistoryItemManager_InitializeComponent_File_name_with_extension;
                tsmiCopyFolder.Text = Resources.HistoryItemManager_InitializeComponent_Folder;

                // Other
                tsmiShowImagePreview.Enabled = IsImageFile;
                tsmiUploadFile.Enabled = uploadFile != null && IsFileExist;
                tsmiEditImage.Enabled = editImage != null && IsImageFile;
                tsmiPinToScreen.Enabled = pinToScreen != null && IsImageFile;
                tsmiShowMoreInfo.Enabled = true;
            }

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

        private void tsmiCopyMarkdownLinkedImage_Click(object sender, EventArgs e)
        {
            CopyMarkdownLinkedImage();
        }

        private void tsmiCopyMarkdownLink_Click(object sender, EventArgs e)
        {
            CopyMarkdownLink();
        }

        private void tsmiCopyMarkdownImage_Click(object sender, EventArgs e)
        {
            CopyMarkdownImage();
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

        private void tsmiUploadFile_Click(object sender, EventArgs e)
        {
            UploadFile();
        }

        private void tsmiEditImage_Click(object sender, EventArgs e)
        {
            EditImage();
        }

        private void tsmiPinToScreen_Click(object sender, EventArgs e)
        {
            PinToScreen();
        }

        private void tsmiShowMoreInfo_Click(object sender, EventArgs e)
        {
            ShowMoreInfo();
        }
    }
}