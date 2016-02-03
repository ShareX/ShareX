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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShareX.IndexerLib
{
    public class IndexerJson : Indexer
    {
        private JsonWriter jsonWriter;

        public IndexerJson(IndexerSettings indexerSettings) : base(indexerSettings)
        {
        }

        public override string Index(string folderPath)
        {
            FolderInfo folderInfo = GetFolderInfo(folderPath);
            folderInfo.Update();

            StringBuilder sbContent = new StringBuilder();

            using (StringWriter sw = new StringWriter(sbContent))
            using (jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;

                jsonWriter.WriteStartObject();
                IndexFolder(folderInfo);
                jsonWriter.WriteEndObject();
            }

            return sbContent.ToString();
        }

        protected override void IndexFolder(FolderInfo dir, int level = 0)
        {
            jsonWriter.WritePropertyName(dir.FolderName);
            jsonWriter.WriteStartArray();

            foreach (FolderInfo subdir in dir.Folders)
            {
                jsonWriter.WriteStartObject();
                IndexFolder(subdir);
                jsonWriter.WriteEndObject();
            }

            foreach (FileInfo fi in dir.Files)
            {
                jsonWriter.WriteValue(fi.Name);
            }

            jsonWriter.WriteEnd();
        }
    }
}