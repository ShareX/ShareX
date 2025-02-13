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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX
{
    public class UploadInfoManager
    {
        public UploadInfoStatus[] SelectedItems { get; private set; }

        public UploadInfoStatus SelectedItem
        {
            get
            {
                if (IsItemSelected)
                {
                    return SelectedItems[0];
                }

                return null;
            }
        }

        public bool IsItemSelected
        {
            get
            {
                return SelectedItems != null && SelectedItems.Length > 0;
            }
        }

        private UploadInfoParser parser;

        public UploadInfoManager()
        {
            parser = new UploadInfoParser();
        }

        public void UpdateSelectedItems(IEnumerable<WorkerTask> tasks)
        {
            if (tasks != null && tasks.Count() > 0)
            {
                SelectedItems = tasks.Where(x => x != null && x.Info != null).Select(x => new UploadInfoStatus(x)).ToArray();
            }
            else
            {
                SelectedItems = null;
            }
        }

        private void CopyTexts(IEnumerable<string> texts)
        {
            if (texts != null && texts.Count() > 0)
            {
                string urls = string.Join("\r\n", texts.ToArray());

                if (!string.IsNullOrEmpty(urls))
                {
                    ClipboardHelpers.CopyText(urls);
                }
            }
        }

        #region Open

        public void OpenURL()
        {
            if (IsItemSelected && SelectedItem.IsURLExist) URLHelpers.OpenURL(SelectedItem.Info.Result.URL);
        }

        public void OpenShortenedURL()
        {
            if (IsItemSelected && SelectedItem.IsShortenedURLExist) URLHelpers.OpenURL(SelectedItem.Info.Result.ShortenedURL);
        }

        public void OpenThumbnailURL()
        {
            if (IsItemSelected && SelectedItem.IsThumbnailURLExist) URLHelpers.OpenURL(SelectedItem.Info.Result.ThumbnailURL);
        }

        public void OpenDeletionURL()
        {
            if (IsItemSelected && SelectedItem.IsDeletionURLExist) URLHelpers.OpenURL(SelectedItem.Info.Result.DeletionURL);
        }

        public void OpenFile()
        {
            if (IsItemSelected && SelectedItem.IsFileExist) FileHelpers.OpenFile(SelectedItem.Info.FilePath);
        }

        public void OpenThumbnailFile()
        {
            if (IsItemSelected && SelectedItem.IsThumbnailFileExist) FileHelpers.OpenFile(SelectedItem.Info.ThumbnailFilePath);
        }

        public void OpenFolder()
        {
            if (IsItemSelected && SelectedItem.IsFileExist) FileHelpers.OpenFolderWithFile(SelectedItem.Info.FilePath);
        }

        public void TryOpen()
        {
            if (IsItemSelected)
            {
                SelectedItem.Update();

                if (SelectedItem.IsShortenedURLExist)
                {
                    URLHelpers.OpenURL(SelectedItem.Info.Result.ShortenedURL);
                }
                else if (SelectedItem.IsURLExist)
                {
                    URLHelpers.OpenURL(SelectedItem.Info.Result.URL);
                }
                else if (SelectedItem.IsFilePathValid)
                {
                    FileHelpers.OpenFile(SelectedItem.Info.FilePath);
                }
            }
        }

        #endregion Open

        #region Copy

        public void CopyURL()
        {
            if (IsItemSelected) CopyTexts(SelectedItems.Where(x => x.IsURLExist).Select(x => x.Info.Result.URL));
        }

        public void CopyShortenedURL()
        {
            if (IsItemSelected) CopyTexts(SelectedItems.Where(x => x.IsShortenedURLExist).Select(x => x.Info.Result.ShortenedURL));
        }

        public void CopyThumbnailURL()
        {
            if (IsItemSelected) CopyTexts(SelectedItems.Where(x => x.IsThumbnailURLExist).Select(x => x.Info.Result.ThumbnailURL));
        }

        public void CopyDeletionURL()
        {
            if (IsItemSelected) CopyTexts(SelectedItems.Where(x => x.IsDeletionURLExist).Select(x => x.Info.Result.DeletionURL));
        }

        public void CopyFile()
        {
            if (IsItemSelected && SelectedItem.IsFileExist) ClipboardHelpers.CopyFile(SelectedItem.Info.FilePath);
        }

        public void CopyImage()
        {
            if (IsItemSelected && SelectedItem.IsImageFile) ClipboardHelpers.CopyImageFromFile(SelectedItem.Info.FilePath);
        }

        public void CopyImageDimensions()
        {
            if (IsItemSelected && SelectedItem.IsImageFile)
            {
                Size size = ImageHelpers.GetImageFileDimensions(SelectedItem.Info.FilePath);
                if (!size.IsEmpty)
                {
                    ClipboardHelpers.CopyText($"{size.Width} x {size.Height}");
                }
            }
        }

        public void CopyText()
        {
            if (IsItemSelected && SelectedItem.IsTextFile) ClipboardHelpers.CopyTextFromFile(SelectedItem.Info.FilePath);
        }

        public void CopyThumbnailFile()
        {
            if (IsItemSelected && SelectedItem.IsThumbnailFileExist) ClipboardHelpers.CopyFile(SelectedItem.Info.ThumbnailFilePath);
        }

        public void CopyThumbnailImage()
        {
            if (IsItemSelected && SelectedItem.IsThumbnailFileExist) ClipboardHelpers.CopyImageFromFile(SelectedItem.Info.ThumbnailFilePath);
        }

        public void CopyHTMLLink()
        {
            if (IsItemSelected) CopyTexts(SelectedItems.Where(x => x.IsURLExist).Select(x => parser.Parse(x.Info, UploadInfoParser.HTMLLink)));
        }

        public void CopyHTMLImage()
        {
            if (IsItemSelected) CopyTexts(SelectedItems.Where(x => x.IsImageURL).Select(x => parser.Parse(x.Info, UploadInfoParser.HTMLImage)));
        }

        public void CopyHTMLLinkedImage()
        {
            if (IsItemSelected) CopyTexts(SelectedItems.Where(x => x.IsImageURL && x.IsThumbnailURLExist).Select(x => parser.Parse(x.Info, UploadInfoParser.HTMLLinkedImage)));
        }

        public void CopyForumLink()
        {
            if (IsItemSelected) CopyTexts(SelectedItems.Where(x => x.IsURLExist).Select(x => parser.Parse(x.Info, UploadInfoParser.ForumLink)));
        }

        public void CopyForumImage()
        {
            if (IsItemSelected) CopyTexts(SelectedItems.Where(x => x.IsImageURL).Select(x => parser.Parse(x.Info, UploadInfoParser.ForumImage)));
        }

        public void CopyForumLinkedImage()
        {
            if (IsItemSelected) CopyTexts(SelectedItems.Where(x => x.IsImageURL && x.IsThumbnailURLExist).Select(x => parser.Parse(x.Info, UploadInfoParser.ForumLinkedImage)));
        }

        public void CopyMarkdownLink()
        {
            if (IsItemSelected) CopyTexts(SelectedItems.Where(x => x.IsURLExist).Select(x => parser.Parse(x.Info, UploadInfoParser.MarkdownLink)));
        }

        public void CopyMarkdownImage()
        {
            if (IsItemSelected) CopyTexts(SelectedItems.Where(x => x.IsImageURL).Select(x => parser.Parse(x.Info, UploadInfoParser.MarkdownImage)));
        }

        public void CopyMarkdownLinkedImage()
        {
            if (IsItemSelected) CopyTexts(SelectedItems.Where(x => x.IsImageURL && x.IsThumbnailURLExist).Select(x => parser.Parse(x.Info, UploadInfoParser.MarkdownLinkedImage)));
        }

        public void CopyFilePath()
        {
            if (IsItemSelected) CopyTexts(SelectedItems.Where(x => x.IsFilePathValid).Select(x => x.Info.FilePath));
        }

        public void CopyFileName()
        {
            if (IsItemSelected) CopyTexts(SelectedItems.Where(x => x.IsFilePathValid).Select(x => Path.GetFileNameWithoutExtension(x.Info.FilePath)));
        }

        public void CopyFileNameWithExtension()
        {
            if (IsItemSelected) CopyTexts(SelectedItems.Where(x => x.IsFilePathValid).Select(x => Path.GetFileName(x.Info.FilePath)));
        }

        public void CopyFolder()
        {
            if (IsItemSelected) CopyTexts(SelectedItems.Where(x => x.IsFilePathValid).Select(x => Path.GetDirectoryName(x.Info.FilePath)));
        }

        public void CopyCustomFormat(string format)
        {
            if (!string.IsNullOrEmpty(format) && IsItemSelected) CopyTexts(SelectedItems.Where(x => x.IsURLExist).Select(x => parser.Parse(x.Info, format)));
        }

        public void TryCopy()
        {
            if (IsItemSelected)
            {
                if (SelectedItem.IsShortenedURLExist)
                {
                    CopyTexts(SelectedItems.Where(x => x.IsShortenedURLExist).Select(x => x.Info.Result.ShortenedURL));
                }
                else if (SelectedItem.IsURLExist)
                {
                    CopyTexts(SelectedItems.Where(x => x.IsURLExist).Select(x => x.Info.Result.URL));
                }
                else if (SelectedItem.IsFilePathValid)
                {
                    CopyTexts(SelectedItems.Where(x => x.IsFilePathValid).Select(x => x.Info.FilePath));
                }
            }
        }

        #endregion Copy

        #region Other

        public void ShowImagePreview()
        {
            if (IsItemSelected && SelectedItem.IsImageFile) ImageViewer.ShowImage(SelectedItem.Info.FilePath);
        }

        public void ShowErrors()
        {
            if (IsItemSelected)
            {
                SelectedItem.Task.ShowErrorWindow();
            }
        }

        public void StopUpload()
        {
            if (IsItemSelected)
            {
                foreach (WorkerTask task in SelectedItems.Select(x => x.Task))
                {
                    task?.Stop();
                }
            }
        }

        public void Upload()
        {
            if (IsItemSelected && SelectedItem.IsFileExist) UploadManager.UploadFile(SelectedItem.Info.FilePath);
        }

        public void Download()
        {
            if (IsItemSelected && SelectedItem.IsFileURL) UploadManager.DownloadFile(SelectedItem.Info.Result.URL);
        }

        public void EditImage()
        {
            if (IsItemSelected && SelectedItem.IsImageFile) TaskHelpers.AnnotateImageFromFile(SelectedItem.Info.FilePath);
        }

        public void BeautifyImage()
        {
            if (IsItemSelected && SelectedItem.IsImageFile) TaskHelpers.OpenImageBeautifier(SelectedItem.Info.FilePath);
        }

        public void AddImageEffects()
        {
            if (IsItemSelected && SelectedItem.IsImageFile) TaskHelpers.OpenImageEffects(SelectedItem.Info.FilePath);
        }

        public void PinToScreen()
        {
            if (IsItemSelected && SelectedItem.IsImageFile) TaskHelpers.PinToScreen(SelectedItem.Info.FilePath);
        }

        public void DeleteFiles()
        {
            if (IsItemSelected)
            {
                foreach (string filePath in SelectedItems.Select(x => x.Info.FilePath))
                {
                    FileHelpers.DeleteFile(filePath, true);
                }
            }
        }

        public void ShortenURL(UrlShortenerType urlShortener)
        {
            if (IsItemSelected && SelectedItem.IsURLExist) UploadManager.ShortenURL(SelectedItem.Info.Result.ToString(), urlShortener);
        }

        public void ShareURL(URLSharingServices urlSharingService)
        {
            if (IsItemSelected && SelectedItem.IsURLExist) UploadManager.ShareURL(SelectedItem.Info.Result.ToString(), urlSharingService);
        }

        public void SearchImageUsingGoogleLens()
        {
            if (IsItemSelected && SelectedItem.IsURLExist) TaskHelpers.SearchImageUsingGoogleLens(SelectedItem.Info.Result.URL);
        }

        public void SearchImageUsingBing()
        {
            if (IsItemSelected && SelectedItem.IsURLExist) TaskHelpers.SearchImageUsingBing(SelectedItem.Info.Result.URL);
        }

        public void ShowQRCode()
        {
            if (IsItemSelected && SelectedItem.IsURLExist) new QRCodeForm(SelectedItem.Info.Result.URL).Show();
        }

        public async Task OCRImage()
        {
            if (IsItemSelected && SelectedItem.IsImageFile) await TaskHelpers.OCRImage(SelectedItem.Info.FilePath);
        }

        public void CombineImages()
        {
            if (IsItemSelected)
            {
                IEnumerable<string> imageFiles = SelectedItems.Where(x => x.IsImageFile).Select(x => x.Info.FilePath);

                if (imageFiles.Count() > 1)
                {
                    TaskHelpers.OpenImageCombiner(imageFiles);
                }
            }
        }

        public void CombineImages(Orientation orientation)
        {
            if (IsItemSelected)
            {
                IEnumerable<string> imageFiles = SelectedItems.Where(x => x.IsImageFile).Select(x => x.Info.FilePath);

                if (imageFiles.Count() > 1)
                {
                    TaskHelpers.CombineImages(imageFiles, orientation);
                }
            }
        }

        public void ShowResponse()
        {
            if (IsItemSelected && SelectedItem.Info.Result != null)
            {
                ResponseForm.ShowInstance(SelectedItem.Info.Result);
            }
        }

        #endregion Other
    }
}