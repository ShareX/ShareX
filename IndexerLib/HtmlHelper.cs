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
using IndexerLib.Properties;
using System.IO;
using System.Text;

namespace IndexerLib
{
    public static class HtmlHelper
    {
        public static string GetCssStyle(string filePath)
        {
            string css;

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                css = File.ReadAllText(filePath, Encoding.UTF8);
            }
            else
            {
                css = Resources.IndexerDefault;
            }

            return string.Format("<style type=\"text/css\">\r\n{0}\r\n</style>", css);
        }

        public static string StartTag(string tag, string style = "", string otherFields = "")
        {
            string css = string.Empty;

            if (!string.IsNullOrEmpty(style))
            {
                css = string.Format(" style=\"{0}\"", style);
            }

            string fields = string.Empty;

            if (!string.IsNullOrEmpty(otherFields))
            {
                fields = " " + otherFields;
            }

            return string.Format("<{0}{2}{1}>", tag, fields, css);
        }

        public static string EndTag(string tag)
        {
            return string.Format("</{0}>", tag);
        }

        public static string Tag(string tag, string content, string style = "", string otherFields = "")
        {
            return StartTag(tag, style, otherFields) + Helpers.HtmlEncode(content) + EndTag(tag);
        }
    }
}