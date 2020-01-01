#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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

        private Action<string> uploadFile, editImage;

        public HistoryItemManager(Action<string> uploadFile, Action<string> editImage)
        {
            this.uploadFile = uploadFile;
            this.editImage = editImage;

            InitializeComponent();
        }

        public HistoryItem UpdateSelectedHistoryItem()
        {
            HistoryItem = GetSelectedHistoryItem();

            if (HistoryItem != null)
            {
                IsURLExist = !string.IsNullOrEmpty(HistoryItem.URL);
                IsShortenedURLExist = !string.IsNullOrEmpty(HistoryItem.ShortenedURL);
                IsThumbnailURLExist = !string.IsNullOrEmpty(HistoryItem.ThumbnailURL);
                IsDeletionURLExist = !string.IsNullOrEmpty(HistoryItem.DeletionURL);
                IsImageURL = IsURLExist && Helpers.IsImageFile(HistoryItem.URL);
                IsTextURL = IsURLExist && Helpers.IsTextFile(HistoryItem.URL);
                IsFilePathValid = !string.IsNullOrEmpty(HistoryItem.FilePath) && Path.HasExtension(HistoryItem.FilePath);
                IsFileExist = IsFilePathValid && File.Exists(HistoryItem.FilePath);
                IsImageFile = IsFileExist && Helpers.IsImageFile(HistoryItem.FilePath);
                IsTextFile = IsFileExist && Helpers.IsTextFile(HistoryItem.FilePath);

                UpdateButtons();
            }
            else
            {
                cmsHistory.Enabled = false;
            }

            return HistoryItem;
        }

        private HistoryItem GetSelectedHistoryItem()
        {
            HistoryItem[] historyItems = OnGetHistoryItems();

            if (historyItems != null && historyItems.Length > 0)
            {
                UpdateTexts(historyItems.Length);

                return historyItems[0];
            }

            return null;
        }

        public HistoryItem[] OnGetHistoryItems()
        {
            if (GetHistoryItems != null)
            {
                return GetHistoryItems();
            }

            return null;
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
            if (HistoryItem != null && IsFileExist) Helpers.OpenFile(HistoryItem.FilePath);
        }

        public void OpenFolder()
        {
            if (HistoryItem != null && IsFileExist) Helpers.OpenFolderWithFile(HistoryItem.FilePath);
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
                    Helpers.OpenFile(HistoryItem.FilePath);
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
            if (HistoryItem != null && IsDeletionURLExist) ClipboardHelpers.CopyText(HistoryItem.DeletionURL);
        }

        public void CopyFile()
        {
            if (HistoryItem != null && IsFileExist) ClipboardHelpers.CopyFile(HistoryItem.FilePath);
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
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.URL)).Select(x => string.Format("<a href=\"{0}\">{0}</a>", x.URL)).ToArray();

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
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.URL) && Helpers.IsImageFile(x.URL)).Select(x => string.Format("<img src=\"{0}\"/>", x.URL)).ToArray();

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
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.URL) && Helpers.IsImageFile(x.URL) && !string.IsNullOrEmpty(x.ThumbnailURL)).Select(x => string.Format("<a href=\"{0}\"><img src=\"{1}\"/></a>", x.URL, x.ThumbnailURL)).ToArray();

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
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.URL) && Helpers.IsImageFile(x.URL)).Select(x => string.Format("[img]{0}[/img]", x.URL)).ToArray();

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
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.URL) && Helpers.IsImageFile(x.URL) && !string.IsNullOrEmpty(x.ThumbnailURL)).Select(x => string.Format("[url={0}][img]{1}[/img][/url]", x.URL, x.ThumbnailURL)).ToArray();

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
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.URL)).Select(x => string.Format("[{0}]({1})", x.FileName, x.URL)).ToArray();

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
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.URL) && Helpers.IsImageFile(x.URL)).Select(x => string.Format("![{0}]({1})", x.FileName, x.URL)).ToArray();

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
                string[] array = historyItems.Where(x => x != null && !string.IsNullOrEmpty(x.URL) && Helpers.IsImageFile(x.URL) && !string.IsNullOrEmpty(x.ThumbnailURL)).Select(x => string.Format("[![{0}]({1})]({2})", x.FileName, x.ThumbnailURL, x.URL)).ToArray();

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
            if (HistoryItem != null && IsFilePathValid) ClipboardHelpers.CopyText(HistoryItem.FilePath);
        }

        public void CopyFileName()
        {
            if (HistoryItem != null && IsFilePathValid) ClipboardHelpers.CopyText(Path.GetFileNameWithoutExtension(HistoryItem.FilePath));
        }

        public void CopyFileNameWithExtension()
        {
            if (HistoryItem != null && IsFilePathValid) ClipboardHelpers.CopyText(Path.GetFileName(HistoryItem.FilePath));
        }

        public void CopyFolder()
        {
            if (HistoryItem != null && IsFilePathValid) ClipboardHelpers.CopyText(Path.GetDirectoryName(HistoryItem.FilePath));
        }

        public void ShowImagePreview()
        {
            if (HistoryItem != null && IsImageFile) ImageViewer.ShowImage(HistoryItem.FilePath);
        }

        public void ShowMoreInfo()
        {
            new HistoryItemInfoForm(HistoryItem).Show();
        }

        public void UploadFile()
        {
            if (uploadFile != null && HistoryItem != null && IsFileExist) uploadFile(HistoryItem.FilePath);
        }

        public void EditImage()
        {
            if (editImage != null && HistoryItem != null && IsImageFile) editImage(HistoryItem.FilePath);
        }
    }
}