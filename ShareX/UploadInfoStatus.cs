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
using System.IO;

namespace ShareX
{
    public class UploadInfoStatus
    {
        public WorkerTask Task { get; private set; }

        public TaskInfo Info => Task.Info;

        public bool IsURLExist { get; private set; }
        public bool IsShortenedURLExist { get; private set; }
        public bool IsThumbnailURLExist { get; private set; }
        public bool IsDeletionURLExist { get; private set; }
        public bool IsFileURL { get; private set; }
        public bool IsImageURL { get; private set; }
        public bool IsTextURL { get; private set; }
        public bool IsFilePathValid { get; private set; }
        public bool IsFileExist { get; private set; }
        public bool IsThumbnailFilePathValid { get; private set; }
        public bool IsThumbnailFileExist { get; private set; }
        public bool IsImageFile { get; private set; }
        public bool IsTextFile { get; private set; }

        public UploadInfoStatus(WorkerTask task)
        {
            Task = task;
            Update();
        }

        public void Update()
        {
            if (Info.Result != null)
            {
                IsURLExist = !string.IsNullOrEmpty(Info.Result.URL);
                IsShortenedURLExist = !string.IsNullOrEmpty(Info.Result.ShortenedURL);
                IsThumbnailURLExist = !string.IsNullOrEmpty(Info.Result.ThumbnailURL);
                IsDeletionURLExist = !string.IsNullOrEmpty(Info.Result.DeletionURL);
                IsFileURL = IsURLExist && URLHelpers.IsFileURL(Info.Result.URL);
                IsImageURL = IsFileURL && FileHelpers.IsImageFile(Info.Result.URL);
                IsTextURL = IsFileURL && FileHelpers.IsTextFile(Info.Result.URL);
            }

            IsFilePathValid = !string.IsNullOrEmpty(Info.FilePath) && Path.HasExtension(Info.FilePath);
            IsFileExist = IsFilePathValid && File.Exists(Info.FilePath);
            IsThumbnailFilePathValid = !string.IsNullOrEmpty(Info.ThumbnailFilePath) && Path.HasExtension(Info.ThumbnailFilePath);
            IsThumbnailFileExist = IsThumbnailFilePathValid && File.Exists(Info.ThumbnailFilePath);
            IsImageFile = IsFileExist && FileHelpers.IsImageFile(Info.FilePath);
            IsTextFile = IsFileExist && FileHelpers.IsTextFile(Info.FilePath);
        }
    }
}