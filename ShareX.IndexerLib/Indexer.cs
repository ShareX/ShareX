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

using System;
using System.IO;

namespace ShareX.IndexerLib
{
    public abstract class Indexer
    {
        protected IndexerSettings settings = null;

        protected Indexer(IndexerSettings indexerSettings)
        {
            settings = indexerSettings;
        }

        public static string Index(string folderPath, IndexerSettings settings)
        {
            Indexer indexer = null;

            switch (settings.Output)
            {
                case IndexerOutput.Html:
                    indexer = new IndexerHtml(settings);
                    break;
                case IndexerOutput.Txt:
                    indexer = new IndexerText(settings);
                    break;
                case IndexerOutput.Xml:
                    indexer = new IndexerXml(settings);
                    break;
                case IndexerOutput.Json:
                    indexer = new IndexerJson(settings);
                    break;
            }

            return indexer.Index(folderPath);
        }

        public abstract string Index(string folderPath);

        protected abstract void IndexFolder(FolderInfo dir, int level = 0);

        protected FolderInfo GetFolderInfo(string folderPath, int level = 0)
        {
            FolderInfo folderInfo = new FolderInfo(folderPath);

            if (settings.MaxDepthLevel == 0 || level < settings.MaxDepthLevel)
            {
                try
                {
                    DirectoryInfo currentDirectoryInfo = new DirectoryInfo(folderPath);

                    foreach (DirectoryInfo directoryInfo in currentDirectoryInfo.EnumerateDirectories())
                    {
                        if (settings.SkipHiddenFolders && directoryInfo.Attributes.HasFlag(FileAttributes.Hidden))
                        {
                            continue;
                        }

                        FolderInfo subFolderInfo = GetFolderInfo(directoryInfo.FullName, level + 1);
                        folderInfo.Folders.Add(subFolderInfo);
                        subFolderInfo.Parent = folderInfo;
                    }

                    foreach (FileInfo fileInfo in currentDirectoryInfo.EnumerateFiles())
                    {
                        if (settings.SkipHiddenFiles && fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                        {
                            continue;
                        }

                        folderInfo.Files.Add(fileInfo);
                    }

                    folderInfo.Files.Sort((x, y) => x.Name.CompareTo(y.Name));
                }
                catch (UnauthorizedAccessException)
                {
                }
            }

            return folderInfo;
        }
    }
}