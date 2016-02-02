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
using ShareX.IndexerLib.Properties;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ShareX.IndexerLib
{
    public class IndexerHtml : Indexer
    {
        public IndexerHtml(IndexerSettings indexerSettings)
            : base(indexerSettings)
        {
        }

        public override string Index(string folderPath)
        {
            StringBuilder sbHtmlIndex = new StringBuilder();
            sbHtmlIndex.AppendLine("<!DOCTYPE html>");
            sbHtmlIndex.AppendLine(HtmlHelper.StartTag("html"));
            sbHtmlIndex.AppendLine(HtmlHelper.StartTag("head"));
            sbHtmlIndex.AppendLine("<meta charset=\"UTF-8\">");
            sbHtmlIndex.AppendLine("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">");
            sbHtmlIndex.AppendLine(HtmlHelper.Tag("title", "Index for " + Path.GetFileName(folderPath)));
            sbHtmlIndex.AppendLine(GetCssStyle());
            sbHtmlIndex.AppendLine(HtmlHelper.EndTag("head"));
            sbHtmlIndex.AppendLine(HtmlHelper.StartTag("body"));
            string index = base.Index(folderPath).Trim();
            sbHtmlIndex.AppendLine(index);
            if (config.AddFooter) sbHtmlIndex.AppendLine(HtmlHelper.StartTag("div") + GetFooter() + HtmlHelper.EndTag("div"));
            sbHtmlIndex.AppendLine(HtmlHelper.EndTag("body"));
            sbHtmlIndex.AppendLine(HtmlHelper.EndTag("html"));
            return sbHtmlIndex.ToString().Trim();
        }

        protected override void IndexFolder(FolderInfo dir, int level)
        {
            sbContent.AppendLine(GetFolderNameRow(dir, level));

            string divClass = level > 0 ? "FolderBorder" : "MainFolderBorder";
            sbContent.AppendLine(HtmlHelper.StartTag("div", "", "class=\"" + divClass + "\""));

            if (dir.Files.Count > 0)
            {
                sbContent.AppendLine(HtmlHelper.StartTag("ul"));

                foreach (FileInfo fi in dir.Files)
                {
                    sbContent.AppendLine(GetFileNameRow(fi, level));
                }

                sbContent.AppendLine(HtmlHelper.EndTag("ul"));
            }

            foreach (FolderInfo subdir in dir.Folders)
            {
                IndexFolder(subdir, level + 1);
            }

            sbContent.AppendLine(HtmlHelper.EndTag("div"));
        }

        protected override string GetFolderNameRow(FolderInfo dir, int level)
        {
            string folderNameRow = "";

            if (!dir.IsEmpty)
            {
                if (config.ShowSizeInfo)
                {
                    folderNameRow += dir.Size.ToSizeString(config.BinaryUnits) + " ";
                }

                folderNameRow += "(";

                if (dir.TotalFileCount > 0)
                {
                    folderNameRow += dir.TotalFileCount + " file" + (dir.TotalFileCount > 1 ? "s" : "");
                }

                if (dir.TotalFolderCount > 0)
                {
                    if (dir.TotalFileCount > 0)
                    {
                        folderNameRow += ", ";
                    }

                    folderNameRow += dir.TotalFolderCount + " folder" + (dir.TotalFolderCount > 1 ? "s" : "");
                }

                folderNameRow += ")";
                folderNameRow = " " + HtmlHelper.Tag("span", folderNameRow, "", "class=\"FolderInfo\"");
            }

            int heading = (level + 1).Between(1, 6);

            return HtmlHelper.StartTag("h" + heading) + URLHelpers.HtmlEncode(dir.FolderName) + folderNameRow + HtmlHelper.EndTag("h" + heading);
        }

        protected override string GetFileNameRow(FileInfo fi, int level)
        {
            string fileNameRow = HtmlHelper.StartTag("li") + URLHelpers.HtmlEncode(fi.Name);

            if (config.ShowSizeInfo)
            {
                fileNameRow += " " + HtmlHelper.Tag("span", fi.Length.ToSizeString(config.BinaryUnits), "", "class=\"FileSize\"");
            }

            fileNameRow += HtmlHelper.EndTag("li");

            return fileNameRow;
        }

        protected override string GetFooter()
        {
            return string.Format("Generated by {0} on {1}.", string.Format("<a href=\"{0}\">{1} {2}</a>", Links.URL_WEBSITE, Application.ProductName, Application.ProductVersion),
                DateTime.UtcNow.ToString("yyyy-MM-dd 'at' HH:mm:ss 'UTC'"));
        }

        private string GetCssStyle()
        {
            string css;

            if (config.UseCustomCSSFile && !string.IsNullOrEmpty(config.CustomCSSFilePath) && File.Exists(config.CustomCSSFilePath))
            {
                css = File.ReadAllText(config.CustomCSSFilePath, Encoding.UTF8);
            }
            else
            {
                css = Resources.IndexerDefault;
            }

            return $"<style type=\"text/css\">\r\n{css}\r\n</style>";
        }
    }
}