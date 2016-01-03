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

using Newtonsoft.Json.Linq;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace ShareX.UploadersLib
{
    public class CustomUploaderItem
    {
        public string Name { get; set; }
        public CustomUploaderRequestType RequestType { get; set; }
        public string RequestURL { get; set; }
        public string FileFormName { get; set; }
        public Dictionary<string, string> Arguments { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public ResponseType ResponseType { get; set; }
        public List<string> RegexList { get; set; }
        public string URL { get; set; }
        public string ThumbnailURL { get; set; }
        public string DeletionURL { get; set; }

        private string response;
        private List<Match> regexResult;

        public CustomUploaderItem()
        {
        }

        public CustomUploaderItem(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public HttpMethod GetHttpMethod()
        {
            switch (RequestType)
            {
                default:
                case CustomUploaderRequestType.POST:
                    return HttpMethod.POST;
                case CustomUploaderRequestType.GET:
                    return HttpMethod.GET;
                case CustomUploaderRequestType.PUT:
                    return HttpMethod.PUT;
                case CustomUploaderRequestType.PATCH:
                    return HttpMethod.PATCH;
                case CustomUploaderRequestType.DELETE:
                    return HttpMethod.DELETE;
            }
        }

        public string GetRequestURL()
        {
            if (string.IsNullOrEmpty(RequestURL))
            {
                throw new Exception("'Request URL' must be not empty.");
            }

            return URLHelpers.FixPrefix(RequestURL);
        }

        public string GetFileFormName()
        {
            if (string.IsNullOrEmpty(FileFormName))
            {
                throw new Exception("'File form name' must be not empty.");
            }

            return FileFormName;
        }

        public Dictionary<string, string> GetArguments(string input = null)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();

            if (Arguments != null)
            {
                foreach (KeyValuePair<string, string> arg in Arguments)
                {
                    string value = arg.Value;

                    value = value.Replace("%input", "$input$"); // For backward compatibility
                    value = NameParser.Parse(NameParserType.Text, value);
                    value = value.Replace("$input$", input);

                    arguments.Add(arg.Key, value);
                }
            }

            return arguments;
        }

        public NameValueCollection GetHeaders()
        {
            if (Headers != null && Headers.Count > 0)
            {
                NameValueCollection collection = new NameValueCollection();

                foreach (KeyValuePair<string, string> header in Headers)
                {
                    collection.Add(header.Key, header.Value);
                }

                return collection;
            }

            return null;
        }

        public void ParseResponse(UploadResult result, bool isShortenedURL = false)
        {
            if (result != null && !string.IsNullOrEmpty(result.Response))
            {
                response = result.Response;
                ParseRegexList();

                string url;

                if (!string.IsNullOrEmpty(URL))
                {
                    url = ParseURL(URL);
                }
                else
                {
                    url = response;
                }

                if (isShortenedURL)
                {
                    result.ShortenedURL = url;
                }
                else
                {
                    result.URL = url;
                }

                result.ThumbnailURL = ParseURL(ThumbnailURL);
                result.DeletionURL = ParseURL(DeletionURL);
            }
        }

        private void ParseRegexList()
        {
            regexResult = new List<Match>();

            if (RegexList != null)
            {
                foreach (string regex in RegexList)
                {
                    regexResult.Add(Regex.Match(response, regex));
                }
            }
        }

        private string ParseURL(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            StringBuilder result = new StringBuilder();

            bool syntaxStart = false;
            CustomUploaderResponseParseType parseType = CustomUploaderResponseParseType.Regex;
            int syntaxStartIndex = 0;

            for (int i = 0; i < url.Length; i++)
            {
                if (url[i] == '$')
                {
                    if (!syntaxStart)
                    {
                        syntaxStart = true;

                        string syntaxCheck = url.Substring(i + 1);

                        if (syntaxCheck.StartsWith("regex:", StringComparison.InvariantCultureIgnoreCase)) // Example: $regex:1,1$
                        {
                            parseType = CustomUploaderResponseParseType.Regex;
                            syntaxStartIndex = i + 7;
                        }
                        else if (syntaxCheck.StartsWith("json:", StringComparison.InvariantCultureIgnoreCase)) // Example: $json:Files[0].URL$
                        {
                            parseType = CustomUploaderResponseParseType.Json;
                            syntaxStartIndex = i + 6;
                        }
                        else if (syntaxCheck.StartsWith("xml:", StringComparison.InvariantCultureIgnoreCase)) // Example: $xml:/Files/File[1]/URL$
                        {
                            parseType = CustomUploaderResponseParseType.Xml;
                            syntaxStartIndex = i + 5;
                        }
                        else
                        {
                            parseType = CustomUploaderResponseParseType.Regex;
                            syntaxStartIndex = i + 1;
                        }
                    }
                    else
                    {
                        string parseText = url.Substring(syntaxStartIndex, i - syntaxStartIndex).Trim();

                        if (!string.IsNullOrEmpty(parseText))
                        {
                            string resultText;

                            switch (parseType)
                            {
                                default:
                                case CustomUploaderResponseParseType.Regex:
                                    resultText = ParseRegexSyntax(parseText);
                                    break;
                                case CustomUploaderResponseParseType.Json:
                                    resultText = ParseJsonSyntax(parseText);
                                    break;
                                case CustomUploaderResponseParseType.Xml:
                                    resultText = ParseXmlSyntax(parseText);
                                    break;
                            }

                            if (!string.IsNullOrEmpty(resultText))
                            {
                                result.Append(resultText);
                            }
                        }

                        syntaxStart = false;
                    }
                }
                else if (!syntaxStart)
                {
                    result.Append(url[i]);
                }
            }

            return result.ToString();
        }

        private string ParseRegexSyntax(string syntax)
        {
            string regexIndexString = "";
            int regexIndex;
            bool isGroupRegex = false;
            int i;

            for (i = 0; i < syntax.Length; i++)
            {
                if (char.IsDigit(syntax[i]))
                {
                    regexIndexString += syntax[i];
                }
                else
                {
                    if (syntax[i] == ',')
                    {
                        isGroupRegex = true;
                    }

                    break;
                }
            }

            if (regexIndexString.Length > 0 && int.TryParse(regexIndexString, out regexIndex))
            {
                Match match = regexResult[regexIndex - 1];

                if (isGroupRegex && i + 1 < syntax.Length)
                {
                    string group = syntax.Substring(i + 1);
                    int groupNumber;

                    if (int.TryParse(group, out groupNumber))
                    {
                        return match.Groups[groupNumber].Value;
                    }

                    return match.Groups[group].Value;
                }

                return match.Value;
            }

            return null;
        }

        // http://goessner.net/articles/JsonPath/
        private string ParseJsonSyntax(string syntaxJsonPath)
        {
            return (string)JToken.Parse(response).SelectToken("$." + syntaxJsonPath);
        }

        // http://www.w3schools.com/xsl/xpath_syntax.asp
        // https://msdn.microsoft.com/en-us/library/ms256086(v=vs.110).aspx
        private string ParseXmlSyntax(string syntaxXPath)
        {
            using (StringReader sr = new StringReader(response))
            {
                XPathDocument doc = new XPathDocument(sr);
                XPathNavigator nav = doc.CreateNavigator();
                XPathNavigator node = nav.SelectSingleNode(syntaxXPath);

                if (node != null)
                {
                    return node.Value;
                }
            }

            return null;
        }
    }
}