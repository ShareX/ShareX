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

using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ShareX.HelpersLib
{
    public static class XMLExtensions
    {
        public static XElement GetNode(this XContainer element, string path)
        {
            path = path.Trim().Trim('/');

            if (element != null && !string.IsNullOrEmpty(path))
            {
                XContainer lastElement = element;

                string[] splitPath = path.Split('/');

                if (splitPath.Length > 0)
                {
                    foreach (string name in splitPath)
                    {
                        if (name.Contains('|'))
                        {
                            string[] splitName = name.Split('|');

                            XContainer lastElement2 = null;

                            foreach (string name2 in splitName)
                            {
                                lastElement2 = lastElement.Element(name2);
                                if (lastElement2 != null) break;
                            }

                            lastElement = lastElement2;
                        }
                        else
                        {
                            lastElement = lastElement.Element(name);
                        }

                        if (lastElement == null) return null;
                    }

                    return (XElement)lastElement;
                }
            }

            return null;
        }

        public static XElement[] GetNodes(this XContainer element, string path)
        {
            path = path.Trim().Trim('/');

            if (element != null && !string.IsNullOrEmpty(path))
            {
                int index = path.LastIndexOf('/');

                if (index > -1)
                {
                    string leftPath = path.Left(index);
                    string lastPath = path.RemoveLeft(index + 1);

                    XElement lastNode = element.GetNode(leftPath);

                    if (lastNode != null)
                    {
                        return lastNode.Elements(lastPath).Where(x => x != null).ToArray();
                    }
                }
            }

            return null;
        }

        public static string GetValue(this XContainer element, string path, string defaultValue = null)
        {
            XElement xe = element.GetNode(path);

            if (xe != null) return xe.Value;

            return defaultValue;
        }

        public static XElement GetElement(this XElement xe, params string[] elements)
        {
            XElement result = null;

            if (xe != null && elements != null && elements.Length > 0)
            {
                result = xe;

                foreach (string element in elements)
                {
                    result = result.Element(element);
                    if (result == null) break;
                }
            }

            return result;
        }

        public static XElement GetElement(this XDocument xd, params string[] elements)
        {
            if (xd != null && elements != null && elements.Length > 0)
            {
                XElement result = xd.Root;

                if (result.Name == elements[0])
                {
                    for (int i = 1; i < elements.Length; i++)
                    {
                        result = result.Element(elements[i]);
                        if (result == null) break;
                    }

                    return result;
                }
            }

            return null;
        }

        public static string GetElementValue(this XElement xe, XName name)
        {
            if (xe != null)
            {
                XElement xeItem = xe.Element(name);
                if (xeItem != null) return xeItem.Value;
            }

            return "";
        }

        public static string GetAttributeValue(this XElement xe, string name)
        {
            if (xe != null)
            {
                XAttribute xaItem = xe.Attribute(name);
                if (xaItem != null) return xaItem.Value;
            }

            return "";
        }

        public static string GetAttributeFirstValue(this XElement xe, params string[] names)
        {
            string value;
            foreach (string name in names)
            {
                value = xe.GetAttributeValue(name);
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }

            return "";
        }

        public static XmlNode AppendElement(this XmlNode parent, string tagName)
        {
            return parent.AppendElement(tagName, null, false);
        }

        public static XmlNode AppendElement(this XmlNode parent, string tagName, string textContent, bool checkTextContent = true)
        {
            if (!checkTextContent || !string.IsNullOrEmpty(textContent))
            {
                XmlDocument xd;

                if (parent is XmlDocument document)
                {
                    xd = document;
                }
                else
                {
                    xd = parent.OwnerDocument;
                }

                XmlNode node = xd.CreateElement(tagName);
                parent.AppendChild(node);

                if (textContent != null)
                {
                    XmlNode content = xd.CreateTextNode(textContent);
                    node.AppendChild(content);
                }

                return node;
            }

            return null;
        }

        public static XmlNode PrependElement(this XmlNode parent, string tagName)
        {
            return parent.PrependElement(tagName, null, false);
        }

        public static XmlNode PrependElement(this XmlNode parent, string tagName, string textContent, bool checkTextContent = true)
        {
            if (!checkTextContent || !string.IsNullOrEmpty(textContent))
            {
                XmlDocument xd;

                if (parent is XmlDocument document)
                {
                    xd = document;
                }
                else
                {
                    xd = parent.OwnerDocument;
                }

                XmlNode node = xd.CreateElement(tagName);
                parent.PrependChild(node);

                if (textContent != null)
                {
                    XmlNode content = xd.CreateTextNode(textContent);
                    node.PrependChild(content);
                }

                return node;
            }

            return null;
        }

        public static void WriteElementIfNotEmpty(this XmlTextWriter writer, string name, string value)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
            {
                writer.WriteElementString(name, value);
            }
        }
    }
}