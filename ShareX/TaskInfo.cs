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

using HelpersLib;
using HistoryLib;
using System;
using System.IO;
using UploadersLib;
using UploadersLib.HelperClasses;

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
                return Job != TaskJob.ImageJob || TaskSettings.SafeAfterTasks.AfterCaptureJobsTemp.HasFlag(AfterCaptureTasks.UploadImageToHost);
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
                FileName = Path.GetFileName(filePath);
            }
        }

        public string FileName { get; set; }
        public string ThumbnailFilePath { get; set; }
        public EDataType DataType { get; set; }

        public EDataType UploadDestination
        {
            get
            {
                if ((DataType == EDataType.Image && TaskSettings.SafeDestinations.ImageDestination == ImageDestination.FileUploader) ||
                    (DataType == EDataType.Text && TaskSettings.SafeDestinations.TextDestination == TextDestination.FileUploader))
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
                switch (UploadDestination)
                {
                    case EDataType.Image:
                        return TaskSettings.SafeDestinations.ImageDestination.GetDescription();
                    case EDataType.Text:
                        return TaskSettings.SafeDestinations.TextDestination.GetDescription();
                    case EDataType.File:
                        switch (DataType)
                        {
                            case EDataType.Image:
                                return TaskSettings.SafeDestinations.ImageFileDestination.GetDescription();
                            case EDataType.Text:
                                return TaskSettings.SafeDestinations.TextFileDestination.GetDescription();
                            default:
                            case EDataType.File:
                                return TaskSettings.SafeDestinations.FileDestination.GetDescription();
                        }
                    case EDataType.URL:
                        return TaskSettings.SafeDestinations.URLShortenerDestination.GetDescription();
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

        public HistoryItem GetHistoryItem()
        {
            return new HistoryItem
            {
                Filename = FileName,
                Filepath = FilePath,
                DateTimeUtc = UploadTime,
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