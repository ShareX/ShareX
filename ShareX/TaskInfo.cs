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
using ShareX.HistoryLib;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                switch (Job)
                {
                    case TaskJob.Job:
                        return TaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.UploadImageToHost);
                    case TaskJob.DataUpload:
                    case TaskJob.FileUpload:
                    case TaskJob.TextUpload:
                    case TaskJob.ShortenURL:
                    case TaskJob.ShareURL:
                    case TaskJob.DownloadUpload:
                        return true;
                }

                return false;
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
                    FileName = "";
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
        public TaskMetadata Metadata { get; set; }

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

                return "";
            }
        }

        public DateTime TaskStartTime { get; set; }
        public DateTime TaskEndTime { get; set; }

        public TimeSpan TaskDuration => TaskEndTime - TaskStartTime;

        public Stopwatch UploadDuration { get; set; }

        public UploadResult Result { get; set; }

        public TaskInfo(TaskSettings taskSettings)
        {
            if (taskSettings == null)
            {
                taskSettings = TaskSettings.GetDefaultTaskSettings();
            }

            TaskSettings = taskSettings;
            Metadata = new TaskMetadata();
            Result = new UploadResult();
        }

        public Dictionary<string, string> GetTags()
        {
            if (Metadata != null)
            {
                Dictionary<string, string> tags = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(Metadata.WindowTitle))
                {
                    tags.Add("WindowTitle", Metadata.WindowTitle);
                }

                if (!string.IsNullOrEmpty(Metadata.ProcessName))
                {
                    tags.Add("ProcessName", Metadata.ProcessName);
                }

                if (tags.Count > 0)
                {
                    return tags;
                }
            }

            return null;
        }

        public override string ToString()
        {
            string text = Result.ToString();

            if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(FilePath))
            {
                text = FilePath;
            }

            return text;
        }

        public HistoryItem GetHistoryItem()
        {
            return new HistoryItem
            {
                FileName = FileName,
                FilePath = FilePath,
                DateTime = TaskEndTime,
                Type = DataType.ToString(),
                Host = UploaderHost,
                URL = Result.URL,
                ThumbnailURL = Result.ThumbnailURL,
                DeletionURL = Result.DeletionURL,
                ShortenedURL = Result.ShortenedURL,
                Tags = GetTags()
            };
        }
    }
}