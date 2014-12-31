#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2015 ShareX Developers

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

        public HistoryItemManager()
        {
            InitializeComponent();
        }

        public HistoryRefreshInfoResult RefreshInfo()
        {
            HistoryItem tempHistoryItem = GetSelectedHistoryItem();

            if (tempHistoryItem != null)
            {
                if (HistoryItem != tempHistoryItem)
                {
                    HistoryItem = tempHistoryItem;

                    IsURLExist = !string.IsNullOrEmpty(HistoryItem.URL);
                    IsShortenedURLExist = !string.IsNullOrEmpty(HistoryItem.ShortenedURL);
                    IsThumbnailURLExist = !string.IsNullOrEmpty(HistoryItem.ThumbnailURL);
                    IsDeletionURLExist = !string.IsNullOrEmpty(HistoryItem.DeletionURL);
                    IsImageURL = IsURLExist && Helpers.IsImageFile(HistoryItem.URL);
                    IsTextURL = IsURLExist && Helpers.IsTextFile(HistoryItem.URL);
                    IsFilePathValid = !string.IsNullOrEmpty(HistoryItem.Filepath) && Path.HasExtension(HistoryItem.Filepath);
                    IsFileExist = IsFilePathValid && File.Exists(HistoryItem.Filepath);
                    IsImageFile = IsFileExist && Helpers.IsImageFile(HistoryItem.Filepath);
                    IsTextFile = IsFileExist && Helpers.IsTextFile(HistoryItem.Filepath);

                    UpdateButtons();
                    return HistoryRefreshInfoResult.Success;
                }

                return HistoryRefreshInfoResult.Same;
            }

            cmsHistory.Enabled = false;
            return HistoryRefreshInfoResult.Invalid;
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
            if (HistoryItem != null && IsFileExist) URLHelpers.OpenURL(HistoryItem.Filepath);
        }

        public void OpenFolder()
        {
            if (HistoryItem != null && IsFileExist) Helpers.OpenFolderWithFile(HistoryItem.Filepath);
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
                    URLHelpers.OpenURL(HistoryItem.Filepath);
                }
            }
        }

        public void CopyURL()
        {
            if (HistoryItem != null && IsURLExist)
            {
                HistoryItem[] historyItems = OnGetHistoryItems();

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
            if (HistoryItem != null && IsShortenedURLExist) ClipboardHelpers.CopyText(HistoryItem.ShortenedURL);
        }

        public void CopyThumbnailURL()
        {
            if (HistoryItem != null && IsThumbnailURLExist) ClipboardHelpers.CopyText(HistoryItem.ThumbnailURL);
        }

        public void CopyDeletionURL()
        {
            if (HistoryItem != null && IsDeletionURLExist) ClipboardHelpers.CopyText(HistoryItem.DeletionURL);
        }

        public void CopyFile()
        {
            if (HistoryItem != null && IsFileExist) ClipboardHelpers.CopyFile(HistoryItem.Filepath);
        }

        public void CopyImage()
        {
            if (HistoryItem != null && IsImageFile) ClipboardHelpers.CopyImageFromFile(HistoryItem.Filepath);
        }

        public void CopyText()
        {
            if (HistoryItem != null && IsTextFile) ClipboardHelpers.CopyTextFromFile(HistoryItem.Filepath);
        }

        public void CopyHTMLLink()
        {
            if (HistoryItem != null && IsURLExist)
            {
                HistoryItem[] historyItems = OnGetHistoryItems();

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
            if (HistoryItem != null && IsImageURL) ClipboardHelpers.CopyText(string.Format("<img src=\"{0}\"/>", HistoryItem.URL));
        }

        public void CopyHTMLLinkedImage()
        {
            if (HistoryItem != null && IsImageURL && IsThumbnailURLExist)
            {
                ClipboardHelpers.CopyText(string.Format("<a href=\"{0}\"><img src=\"{1}\"/></a>", HistoryItem.URL, HistoryItem.ThumbnailURL));
            }
        }

        public void CopyForumLink()
        {
            if (HistoryItem != null && IsURLExist) ClipboardHelpers.CopyText(string.Format("[url]{0}[/url]", HistoryItem.URL));
        }

        public void CopyForumImage()
        {
            if (HistoryItem != null && IsImageURL) ClipboardHelpers.CopyText(string.Format("[img]{0}[/img]", HistoryItem.URL));
        }

        public void CopyForumLinkedImage()
        {
            if (HistoryItem != null && IsImageURL && IsThumbnailURLExist)
            {
                ClipboardHelpers.CopyText(string.Format("[url={0}][img]{1}[/img][/url]", HistoryItem.URL, HistoryItem.ThumbnailURL));
            }
        }

        public void CopyFilePath()
        {
            if (HistoryItem != null && IsFilePathValid) ClipboardHelpers.CopyText(HistoryItem.Filepath);
        }

        public void CopyFileName()
        {
            if (HistoryItem != null && IsFilePathValid) ClipboardHelpers.CopyText(Path.GetFileNameWithoutExtension(HistoryItem.Filepath));
        }

        public void CopyFileNameWithExtension()
        {
            if (HistoryItem != null && IsFilePathValid) ClipboardHelpers.CopyText(Path.GetFileName(HistoryItem.Filepath));
        }

        public void CopyFolder()
        {
            if (HistoryItem != null && IsFilePathValid) ClipboardHelpers.CopyText(Path.GetDirectoryName(HistoryItem.Filepath));
        }

        public void ShowImagePreview()
        {
            if (HistoryItem != null && IsImageFile) ImageViewer.ShowImage(HistoryItem.Filepath);
        }

        public void ShowMoreInfo()
        {
            new HistoryItemInfoForm(HistoryItem).Show();
        }
    }
}