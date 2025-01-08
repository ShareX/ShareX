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
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.HistoryLib
{
    public partial class HistoryItemManager
    {
        public delegate HistoryItem[] GetHistoryItemsEventHandler();

        public event GetHistoryItemsEventHandler GetHistoryItems;

        public HistoryItem HistoryItem { get; private set; }

        public bool IsURLExist { get; private set; }
        public bool IsShortenedURLExist { get; private set; }
        public bool IsThumbnailURLExist { get; private set; }
        public bool IsDeletionURLExist { get; private set; }
        public bool IsImageURL { get; private set; }
        public bool IsTextURL { get; private set; }
        public bool IsFilePathValid { get; private set; }
        public bool IsFileExist { get; private set; }
        public bool IsImageFile { get; private set; }
        public bool IsTextFile { get; private set; }

        private Action<string> uploadFile, editImage, pinToScreen;

        public HistoryItemManager(Action<string> uploadFile, Action<string> editImage, Action<string> pinToScreen, bool hideShowMoreInfoButton = false)
        {
            this.uploadFile = uploadFile;
            this.editImage = editImage;
            this.pinToScreen = pinToScreen;

            InitializeComponent();

            tsmiOpen.HideImageMargin();
            tsmiCopy.HideImageMargin();
            tsmiUploadFile.Visible = uploadFile != null;
            tsmiEditImage.Visible = editImage != null;
            tsmiPinToScreen.Visible = pinToScreen != null;
            tsmiShowMoreInfo.Visible = !hideShowMoreInfoButton;
        }

        public HistoryItem UpdateSelectedHistoryItem()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();

            if (historyItems != null && historyItems.Length > 0)
            {
                HistoryItem = historyItems[0];
            }
            else
            {
                HistoryItem = null;
            }

            if (HistoryItem != null)
            {
                IsURLExist = !string.IsNullOrEmpty(HistoryItem.URL);
                IsShortenedURLExist = !string.IsNullOrEmpty(HistoryItem.ShortenedURL);
                IsThumbnailURLExist = !string.IsNullOrEmpty(HistoryItem.ThumbnailURL);
                IsDeletionURLExist = !string.IsNullOrEmpty(HistoryItem.DeletionURL);
                IsImageURL = IsURLExist && FileHelpers.IsImageFile(HistoryItem.URL);
                IsTextURL = IsURLExist && FileHelpers.IsTextFile(HistoryItem.URL);
                IsFilePathValid = !string.IsNullOrEmpty(HistoryItem.FilePath) && Path.HasExtension(HistoryItem.FilePath);
                IsFileExist = IsFilePathValid && File.Exists(HistoryItem.FilePath);
                IsImageFile = IsFileExist && FileHelpers.IsImageFile(HistoryItem.FilePath);
                IsTextFile = IsFileExist && FileHelpers.IsTextFile(HistoryItem.FilePath);

                UpdateContextMenu(historyItems.Length);
            }
            else
            {
                cmsHistory.Enabled = false;
            }

            return HistoryItem;
        }

        public HistoryItem[] OnGetHistoryItems()
        {
            if (GetHistoryItems != null)
            {
                return GetHistoryItems();
            }

            return null;
        }

        public bool HandleKeyInput(KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                default:
                    return false;
                case Keys.Enter:
                    TryOpen();
                    break;
                case Keys.Control | Keys.Enter:
                    OpenFile();
                    break;
                case Keys.Shift | Keys.Enter:
                    OpenFolder();
                    break;
                case Keys.Control | Keys.C:
                    CopyURL();
                    break;
                case Keys.Shift | Keys.C:
                    CopyFile();
                    break;
                case Keys.Alt | Keys.C:
                    CopyImage();
                    break;
                case Keys.Control | Keys.Shift | Keys.C:
                    CopyFilePath();
                    break;
                case Keys.Control | Keys.U:
                    UploadFile();
                    break;
                case Keys.Control | Keys.E:
                    EditImage();
                    break;
                case Keys.Control | Keys.P:
                    PinToScreen();
                    break;
            }

            return true;
        }

        public void OpenURL()
        {
            if (HistoryItem != null && IsURLExist) URLHelpers.OpenURL(HistoryItem.URL);
        }

        public void OpenShortenedURL()
        {
            if (HistoryItem != null && IsShortenedURLExist) URLHelpers.OpenURL(HistoryItem.ShortenedURL);
        }

        public void OpenThumbnailURL()
        {
            if (HistoryItem != null && IsThumbnailURLExist) URLHelpers.OpenURL(HistoryItem.ThumbnailURL);
        }

        public void OpenDeletionURL()
        {
            if (HistoryItem != null && IsDeletionURLExist) URLHelpers.OpenURL(HistoryItem.DeletionURL);
        }

        public void OpenFile()
        {
            if (HistoryItem != null && IsFileExist) FileHelpers.OpenFile(HistoryItem.FilePath);
        }

        public void OpenFolder()
        {
            if (HistoryItem != null && IsFileExist) FileHelpers.OpenFolderWithFile(HistoryItem.FilePath);
        }

        public void TryOpen()
        {
            if (HistoryItem != null)
            {
                if (IsShortenedURLExist)
                {
                    URLHelpers.OpenURL(HistoryItem.ShortenedURL);
                }
                else if (IsURLExist)
                {
                    URLHelpers.OpenURL(HistoryItem.URL);
                }
                else if (IsFileExist)
                {
                    FileHelpers.OpenFile(HistoryItem.FilePath);
                }
            }
        }

        public void CopyURL()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();
            if (historyItems != null)
            {
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.URL)).Select(x => x.URL).ToArray();

                if (array != null && array.Length > 0)
                {
                    string urls = string.Join("\r\n", array);

                    if (!string.IsNullOrEmpty(urls))
                    {
                        ClipboardHelpers.CopyText(urls);
                    }
                }
            }
        }

        public void CopyShortenedURL()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();
            if (historyItems != null)
            {
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.ShortenedURL)).Select(x => x.ShortenedURL).ToArray();

                if (array != null && array.Length > 0)
                {
                    string shortenedURLs = string.Join("\r\n", array);

                    if (!string.IsNullOrEmpty(shortenedURLs))
                    {
                        ClipboardHelpers.CopyText(shortenedURLs);
                    }
                }
            }
        }

        public void CopyThumbnailURL()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();
            if (historyItems != null)
            {
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.ThumbnailURL)).Select(x => x.ThumbnailURL).ToArray();

                if (array != null && array.Length > 0)
                {
                    string thumbnailURLs = string.Join("\r\n", array);

                    if (!string.IsNullOrEmpty(thumbnailURLs))
                    {
                        ClipboardHelpers.CopyText(thumbnailURLs);
                    }
                }
            }
        }

        public void CopyDeletionURL()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();
            if (historyItems != null)
            {
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.DeletionURL)).Select(x => x.DeletionURL).ToArray();

                if (array != null && array.Length > 0)
                {
                    string deletionURLs = string.Join("\r\n", array);

                    if (!string.IsNullOrEmpty(deletionURLs))
                    {
                        ClipboardHelpers.CopyText(deletionURLs);
                    }
                }
            }
        }

        public void CopyFile()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();
            if (historyItems != null)
            {
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.FilePath) && Path.HasExtension(x.FilePath) &&
                    File.Exists(x.FilePath)).Select(x => x.FilePath).ToArray();

                if (array != null && array.Length > 0)
                {
                    ClipboardHelpers.CopyFile(array);
                }
            }
        }

        public void CopyImage()
        {
            if (HistoryItem != null && IsImageFile) ClipboardHelpers.CopyImageFromFile(HistoryItem.FilePath);
        }

        public void CopyText()
        {
            if (HistoryItem != null && IsTextFile) ClipboardHelpers.CopyTextFromFile(HistoryItem.FilePath);
        }

        public void CopyHTMLLink()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();
            if (historyItems != null)
            {
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.URL)).
                    Select(x => string.Format("<a href=\"{0}\">{0}</a>", x.URL)).ToArray();

                if (array != null && array.Length > 0)
                {
                    string htmlLinks = string.Join("\r\n", array);

                    if (!string.IsNullOrEmpty(htmlLinks))
                    {
                        ClipboardHelpers.CopyText(htmlLinks);
                    }
                }
            }
        }

        public void CopyHTMLImage()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();
            if (historyItems != null)
            {
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.URL) && FileHelpers.IsImageFile(x.URL)).
                    Select(x => string.Format("<img src=\"{0}\"/>", x.URL)).ToArray();

                if (array != null && array.Length > 0)
                {
                    string htmlImages = string.Join("\r\n", array);

                    if (!string.IsNullOrEmpty(htmlImages))
                    {
                        ClipboardHelpers.CopyText(htmlImages);
                    }
                }
            }
        }

        public void CopyHTMLLinkedImage()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();
            if (historyItems != null)
            {
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.URL) && FileHelpers.IsImageFile(x.URL) &&
                    !string.IsNullOrEmpty(x.ThumbnailURL)).Select(x => string.Format("<a href=\"{0}\"><img src=\"{1}\"/></a>", x.URL, x.ThumbnailURL)).ToArray();

                if (array != null && array.Length > 0)
                {
                    string htmlLinkedImages = string.Join("\r\n", array);

                    if (!string.IsNullOrEmpty(htmlLinkedImages))
                    {
                        ClipboardHelpers.CopyText(htmlLinkedImages);
                    }
                }
            }
        }

        public void CopyForumLink()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();
            if (historyItems != null)
            {
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.URL)).Select(x => string.Format("[url]{0}[/url]", x.URL)).ToArray();

                if (array != null && array.Length > 0)
                {
                    string forumLinks = string.Join("\r\n", array);

                    if (!string.IsNullOrEmpty(forumLinks))
                    {
                        ClipboardHelpers.CopyText(forumLinks);
                    }
                }
            }
        }

        public void CopyForumImage()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();
            if (historyItems != null)
            {
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.URL) && FileHelpers.IsImageFile(x.URL)).
                    Select(x => string.Format("[img]{0}[/img]", x.URL)).ToArray();

                if (array != null && array.Length > 0)
                {
                    string forumImages = string.Join("\r\n", array);

                    if (!string.IsNullOrEmpty(forumImages))
                    {
                        ClipboardHelpers.CopyText(forumImages);
                    }
                }
            }
        }

        public void CopyForumLinkedImage()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();
            if (historyItems != null)
            {
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.URL) && FileHelpers.IsImageFile(x.URL) &&
                    !string.IsNullOrEmpty(x.ThumbnailURL)).Select(x => string.Format("[url={0}][img]{1}[/img][/url]", x.URL, x.ThumbnailURL)).ToArray();

                if (array != null && array.Length > 0)
                {
                    string forumLinkedImages = string.Join("\r\n", array);

                    if (!string.IsNullOrEmpty(forumLinkedImages))
                    {
                        ClipboardHelpers.CopyText(forumLinkedImages);
                    }
                }
            }
        }

        public void CopyMarkdownLink()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();
            if (historyItems != null)
            {
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.URL)).
                    Select(x => string.Format("[{0}]({1})", x.FileName, x.URL)).ToArray();

                if (array != null && array.Length > 0)
                {
                    string markdownLinks = string.Join("\r\n", array);

                    if (!string.IsNullOrEmpty(markdownLinks))
                    {
                        ClipboardHelpers.CopyText(markdownLinks);
                    }
                }
            }
        }

        public void CopyMarkdownImage()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();
            if (historyItems != null)
            {
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.URL) && FileHelpers.IsImageFile(x.URL)).
                    Select(x => string.Format("![{0}]({1})", x.FileName, x.URL)).ToArray();

                if (array != null && array.Length > 0)
                {
                    string markdownImages = string.Join("\r\n", array);

                    if (!string.IsNullOrEmpty(markdownImages))
                    {
                        ClipboardHelpers.CopyText(markdownImages);
                    }
                }
            }
        }

        public void CopyMarkdownLinkedImage()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();
            if (historyItems != null)
            {
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.URL) && FileHelpers.IsImageFile(x.URL) &&
                    !string.IsNullOrEmpty(x.ThumbnailURL)).Select(x => string.Format("[![{0}]({1})]({2})", x.FileName, x.ThumbnailURL, x.URL)).ToArray();

                if (array != null && array.Length > 0)
                {
                    string markdownLinkedImages = string.Join("\r\n", array);

                    if (!string.IsNullOrEmpty(markdownLinkedImages))
                    {
                        ClipboardHelpers.CopyText(markdownLinkedImages);
                    }
                }
            }
        }

        public void CopyFilePath()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();
            if (historyItems != null)
            {
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.FilePath) && Path.HasExtension(x.FilePath) &&
                    File.Exists(x.FilePath)).Select(x => x.FilePath).ToArray();

                if (array != null && array.Length > 0)
                {
                    string filePaths = string.Join("\r\n", array);

                    if (!string.IsNullOrEmpty(filePaths))
                    {
                        ClipboardHelpers.CopyText(filePaths);
                    }
                }
            }
        }

        public void CopyFileName()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();
            if (historyItems != null)
            {
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.FilePath) && Path.HasExtension(x.FilePath)).
                    Select(x => Path.GetFileNameWithoutExtension(x.FilePath)).ToArray();

                if (array != null && array.Length > 0)
                {
                    string fileNames = string.Join("\r\n", array);

                    if (!string.IsNullOrEmpty(fileNames))
                    {
                        ClipboardHelpers.CopyText(fileNames);
                    }
                }
            }
        }

        public void CopyFileNameWithExtension()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();
            if (historyItems != null)
            {
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.FilePath) && Path.HasExtension(x.FilePath)).
                    Select(x => Path.GetFileName(x.FilePath)).ToArray();

                if (array != null && array.Length > 0)
                {
                    string fileNamesWithExtension = string.Join("\r\n", array);

                    if (!string.IsNullOrEmpty(fileNamesWithExtension))
                    {
                        ClipboardHelpers.CopyText(fileNamesWithExtension);
                    }
                }
            }
        }

        public void CopyFolder()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();
            if (historyItems != null)
            {
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.FilePath) && Path.HasExtension(x.FilePath)).
                    Select(x => Path.GetDirectoryName(x.FilePath)).ToArray();

                if (array != null && array.Length > 0)
                {
                    string folderPaths = string.Join("\r\n", array);

                    if (!string.IsNullOrEmpty(folderPaths))
                    {
                        ClipboardHelpers.CopyText(folderPaths);
                    }
                }
            }
        }

        public void ShowImagePreview()
        {
            if (HistoryItem != null && IsImageFile) ImageViewer.ShowImage(HistoryItem.FilePath);
        }

        public void UploadFile()
        {
            if (uploadFile != null && HistoryItem != null && IsFileExist) uploadFile(HistoryItem.FilePath);
        }

        public void EditImage()
        {
            if (editImage != null && HistoryItem != null && IsImageFile) editImage(HistoryItem.FilePath);
        }

        public void PinToScreen()
        {
            if (pinToScreen != null && HistoryItem != null && IsImageFile) pinToScreen(HistoryItem.FilePath);
        }

        public void ShowMoreInfo()
        {
            new HistoryItemInfoForm(HistoryItem).Show();
        }
    }
}