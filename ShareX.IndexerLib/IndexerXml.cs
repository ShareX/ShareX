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
using System.Text;
using System.Xml;

namespace ShareX.IndexerLib
{
    public class IndexerXml : Indexer
    {
        protected XmlWriter xmlWriter;

        public IndexerXml(IndexerSettings indexerSettings) : base(indexerSettings)
        {
        }

        public override string Index(string folderPath)
        {
            FolderInfo folderInfo = GetFolderInfo(folderPath);
            folderInfo.Update();

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Encoding = new UTF8Encoding(false);
            xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
            xmlWriterSettings.Indent = true;

            using (MemoryStream ms = new MemoryStream())
            {
                using (xmlWriter = XmlWriter.Create(ms, xmlWriterSettings))
                {
                    xmlWriter.WriteStartDocument();
                    IndexFolder(folderInfo);
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                }

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        protected override void IndexFolder(FolderInfo dir, int level = 0)
        {
            xmlWriter.WriteStartElement("Folder");

            if (settings.UseAttribute)
            {
                xmlWriter.WriteAttributeString("Name", dir.FolderName);

                if (settings.ShowSizeInfo && !dir.IsEmpty)
                {
                    xmlWriter.WriteAttributeString("Size", dir.Size.ToSizeString(settings.BinaryUnits));
                }
            }
            else
            {
                xmlWriter.WriteElementString("Name", dir.FolderName);

                if (settings.ShowSizeInfo && !dir.IsEmpty)
                {
                    xmlWriter.WriteElementString("Size", dir.Size.ToSizeString(settings.BinaryUnits));
                }
            }

            if (dir.Files.Count > 0)
            {
                xmlWriter.WriteStartElement("Files");

                foreach (FileInfo fi in dir.Files)
                {
                    xmlWriter.WriteStartElement("File");

                    if (settings.UseAttribute)
                    {
                        xmlWriter.WriteAttributeString("Name", fi.Name);

                        if (settings.ShowSizeInfo)
                        {
                            xmlWriter.WriteAttributeString("Size", fi.Length.ToSizeString(settings.BinaryUnits));
                        }
                    }
                    else
                    {
                        xmlWriter.WriteElementString("Name", fi.Name);

                        if (settings.ShowSizeInfo)
                        {
                            xmlWriter.WriteElementString("Size", fi.Length.ToSizeString(settings.BinaryUnits));
                        }
                    }

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }

            if (dir.Folders.Count > 0)
            {
                xmlWriter.WriteStartElement("Folders");

                foreach (FolderInfo subdir in dir.Folders)
                {
                    IndexFolder(subdir);
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }
    }
}