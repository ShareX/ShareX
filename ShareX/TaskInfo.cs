#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using ShareX.HistoryLib;
using ShareX.UploadersLib;
using System;
using System.IO;

namespace ShareX
{
    public class TaskInfo
    {
        public TaskSettings TaskSettings { get; set; }

        public string Status { get; set; }
        public TaskJob Job { get; set; }

        public bool IsUploadJob
        {
            get
            {
                return Job != TaskJob.Job || TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.UploadImageToHost);
            }
        }

        public ProgressManager Progress { get; set; }

        private string filePath;

        public string FilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;

                if (string.IsNullOrEmpty(filePath))
                {
                    FileName = string.Empty;
                }
                else
                {
                    FileName = Path.GetFileName(filePath);
                }
            }
        }

        public string FileName { get; set; }
        public string ThumbnailFilePath { get; set; }
        public EDataType DataType { get; set; }

        public EDataType UploadDestination
        {
            get
            {
                if ((DataType == EDataType.Image && TaskSettings.ImageDestination == ImageDestination.FileUploader) ||
                    (DataType == EDataType.Text && TaskSettings.TextDestination == TextDestination.FileUploader))
                {
                    return EDataType.File;
                }

                return DataType;
            }
        }

        public string UploaderHost
        {
            get
            {
                if (IsUploadJob)
                {
                    switch (UploadDestination)
                    {
                        case EDataType.Image:
                            return TaskSettings.ImageDestination.GetLocalizedDescription();
                        case EDataType.Text:
                            return TaskSettings.TextDestination.GetLocalizedDescription();
                        case EDataType.File:
                            switch (DataType)
                            {
                                case EDataType.Image:
                                    return TaskSettings.ImageFileDestination.GetLocalizedDescription();
                                case EDataType.Text:
                                    return TaskSettings.TextFileDestination.GetLocalizedDescription();
                                default:
                                case EDataType.File:
                                    return TaskSettings.FileDestination.GetLocalizedDescription();
                            }
                        case EDataType.URL:
                            if (Job == TaskJob.ShareURL)
                            {
                                return TaskSettings.URLSharingServiceDestination.GetLocalizedDescription();
                            }

                            return TaskSettings.URLShortenerDestination.GetLocalizedDescription();
                    }
                }

                return string.Empty;
            }
        }

        public DateTime StartTime { get; set; }
        public DateTime UploadTime { get; set; }

        public TimeSpan UploadDuration
        {
            get
            {
                return UploadTime - StartTime;
            }
        }

        public UploadResult Result { get; set; }

        public TaskInfo(TaskSettings taskSettings)
        {
            if (taskSettings == null)
            {
                taskSettings = TaskSettings.GetDefaultTaskSettings();
            }

            TaskSettings = taskSettings;
            Result = new UploadResult();
        }

        public override string ToString()
        {
            string text = "";

            if (!string.IsNullOrEmpty(Result.ToString()))
            {
                text = Result.ToString();
            }
            else if (!string.IsNullOrEmpty(FilePath))
            {
                text = FilePath;
            }

            return text;
        }

        public HistoryItem GetHistoryItem()
        {
            return new HistoryItem
            {
                Filename = FileName,
                Filepath = FilePath,
                DateTime = UploadTime,
                Type = DataType.ToString(),
                Host = UploaderHost,
                URL = Result.URL,
                ThumbnailURL = Result.ThumbnailURL,
                DeletionURL = Result.DeletionURL,
                ShortenedURL = Result.ShortenedURL
            };
        }
    }
}