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

using System.IO;
using System.Xml.XPath;

namespace ShareX.UploadersLib
{
    // Example: {xml:/files/file[1]/url}
    // Example: {xml:{response}|/files/file[1]/url}
    internal class CustomUploaderFunctionXml : CustomUploaderFunction
    {
        public override string Name { get; } = "xml";

        public override int MinParameterCount { get; } = 1;

        public override string Call(ShareXCustomUploaderSyntaxParser parser, string[] parameters)
        {
            // https://www.w3schools.com/xml/xpath_syntax.asp
            string input, xpath;

            if (parameters.Length > 1)
            {
                // {xml:input|xpath}
                input = parameters[0];
                xpath = parameters[1];
            }
            else
            {
                // {xml:xpath}
                input = parser.ResponseInfo.ResponseText;
                xpath = parameters[0];
            }

            if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(xpath))
            {
                using (StringReader sr = new StringReader(input))
                {
                    XPathDocument doc = new XPathDocument(sr);
                    XPathNavigator nav = doc.CreateNavigator();
                    XPathNavigator node = nav.SelectSingleNode(xpath);

                    if (node != null)
                    {
                        return node.Value;
                    }
                }
            }

            return null;
        }
    }
}