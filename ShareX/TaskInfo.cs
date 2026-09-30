#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

using ShareX.Destinations;
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

        /// <summary>The route and instance used for the upload. Set when the upload starts.</summary>
        public RouteMatch Route { get; set; }

        /// <summary>An instance picked for this upload only, for example in the before upload window.</summary>
        public Guid? DestinationInstanceOverride { get; set; }

        /// <summary>The route this upload will use, or the one it used. Null when routing is unavailable.</summary>
        public RouteMatch GetRoute()
        {
            if (Route != null)
            {
                return Route;
            }

            if (DataType == EDataType.URL || ApplicationState.UploadersConfigOrNull == null || ApplicationState.DefaultTaskSettings == null)
            {
                return null;
            }

            try
            {
                RouteMatch route = DestinationRouting.Resolve(TaskSettings, DataType, FileName);

                if (route != null && DestinationInstanceOverride is Guid id && DestinationRouting.Config.FindInstance(id) is DestinationInstance instance &&
                    DestinationRouting.Config.IsCompatible(instance, route.FileType))
                {
                    route = route with { Instance = instance };
                }

                return route;
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                return null;
            }
        }

        public EDataType UploadDestination
        {
            get
            {
                RouteMatch route = Route;

                if (route != null)
                {
                    return route.Instance.Category switch
                    {
                        UploaderCategory.Image => EDataType.Image,
                        UploaderCategory.Text => EDataType.Text,
                        _ => EDataType.File
                    };
                }

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
                    if (DataType != EDataType.URL && GetRoute() is RouteMatch route)
                    {
                        return route.Instance.Name;
                    }

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

                return null;
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

                AddRouteTags(tags);

                if (tags.Count > 0)
                {
                    return tags;
                }
            }
            else if (Route != null)
            {
                Dictionary<string, string> tags = new Dictionary<string, string>();
                AddRouteTags(tags);
                return tags;
            }

            return null;
        }

        private void AddRouteTags(Dictionary<string, string> tags)
        {
            // Tags keep the history database format unchanged.
            if (Route != null)
            {
                tags["Route"] = Route.FileType.Name;
                tags["DestinationInstance"] = Route.Instance.Name;
                tags["DestinationInstanceId"] = Route.Instance.Id.ToString();
            }
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