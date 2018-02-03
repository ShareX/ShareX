#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace ShareX.UploadersLib
{
    public class CustomUploaderItem
    {
        [DefaultValue("")]
        public string Name { get; set; }

        public bool ShouldSerializeName() => !string.IsNullOrEmpty(Name) && Name != URLHelpers.GetHostName(RequestURL);

        [DefaultValue(CustomUploaderDestinationType.None)]
        public CustomUploaderDestinationType DestinationType { get; set; }

        [DefaultValue(CustomUploaderRequestType.POST)]
        public CustomUploaderRequestType RequestType { get; set; }

        [DefaultValue("")]
        public string RequestURL { get; set; }

        [DefaultValue("")]
        public string FileFormName { get; set; }

        [DefaultValue(null)]
        public Dictionary<string, string> Arguments { get; set; }

        public bool ShouldSerializeArguments() => Arguments != null && Arguments.Count > 0;

        [DefaultValue(null)]
        public Dictionary<string, string> Headers { get; set; }

        public bool ShouldSerializeHeaders() => Headers != null && Headers.Count > 0;

        [DefaultValue(ResponseType.Text)]
        public ResponseType ResponseType { get; set; }

        [DefaultValue(null)]
        public List<string> RegexList { get; set; }

        public bool ShouldSerializeRegexList() => RegexList != null && RegexList.Count > 0;

        [DefaultValue("")]
        public string URL { get; set; }

        [DefaultValue("")]
        public string ThumbnailURL { get; set; }

        [DefaultValue("")]
        public string DeletionURL { get; set; }

        private string response;
        private List<Match> regexResult;

        public CustomUploaderItem()
        {
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                return Name;
            }

            string name = URLHelpers.GetHostName(RequestURL);

            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }

            return "Name";
        }

        public string GetFileName()
        {
            return ToString() + ".sxcu";
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
                throw new Exception(Resources.CustomUploaderItem_GetRequestURL_RequestURLMustBeConfigured);
            }

            string url = ParseURL(RequestURL, false);

            return URLHelpers.FixPrefix(url);
        }

        public string GetFileFormName()
        {
            if (string.IsNullOrEmpty(FileFormName))
            {
                throw new Exception(Resources.CustomUploaderItem_GetFileFormName_FileFormNameMustBeConfigured);
            }

            return FileFormName;
        }

        public Dictionary<string, string> GetArguments(CustomUploaderArgumentInput input)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();

            if (Arguments != null)
            {
                foreach (KeyValuePair<string, string> arg in Arguments)
                {
                    arguments.Add(arg.Key, input.Parse(arg.Value));
                }
            }

            return arguments;
        }

        public NameValueCollection GetHeaders(CustomUploaderArgumentInput input)
        {
            if (Headers != null && Headers.Count > 0)
            {
                NameValueCollection collection = new NameValueCollection();

                foreach (KeyValuePair<string, string> header in Headers)
                {
                    collection.Add(header.Key, input.Parse(header.Value));
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
                    url = ParseURL(URL, true);
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

                result.ThumbnailURL = ParseURL(ThumbnailURL, true);
                result.DeletionURL = ParseURL(DeletionURL, true);
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

        public string ParseURL(string url, bool output)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "";
            }

            StringBuilder result = new StringBuilder();

            bool syntaxStart = false;
            int syntaxStartIndex = 0;
            bool escape = false;

            for (int i = 0; i < url.Length; i++)
            {
                if (url[i] == '$' && !escape)
                {
                    if (!syntaxStart)
                    {
                        syntaxStart = true;
                        syntaxStartIndex = i + 1;
                    }
                    else
                    {
                        syntaxStart = false;
                        int syntaxLength = i - syntaxStartIndex;

                        if (syntaxLength > 0)
                        {
                            string syntax = url.Substring(syntaxStartIndex, syntaxLength);
                            string syntaxResult = ParseSyntax(syntax, output);

                            if (!string.IsNullOrEmpty(syntaxResult))
                            {
                                result.Append(syntaxResult);
                            }
                        }
                    }

                    escape = false;
                }
                else if (url[i] == '\\' && !escape)
                {
                    escape = true;
                }
                else if (!syntaxStart)
                {
                    result.Append(url[i]);
                    escape = false;
                }
            }

            return result.ToString();
        }

        private string ParseSyntax(string syntax, bool output)
        {
            CustomUploaderResponseParseType parseType;

            if (syntax.Equals("response", StringComparison.InvariantCultureIgnoreCase)) // Example: $response$
            {
                return response;
            }
            else if (syntax.StartsWith("regex:", StringComparison.InvariantCultureIgnoreCase)) // Example: $regex:1,1$
            {
                parseType = CustomUploaderResponseParseType.Regex;
                syntax = syntax.Substring(6);
            }
            else if (syntax.StartsWith("json:", StringComparison.InvariantCultureIgnoreCase)) // Example: $json:Files[0].URL$
            {
                parseType = CustomUploaderResponseParseType.Json;
                syntax = syntax.Substring(5);
            }
            else if (syntax.StartsWith("xml:", StringComparison.InvariantCultureIgnoreCase)) // Example: $xml:/Files/File[1]/URL$
            {
                parseType = CustomUploaderResponseParseType.Xml;
                syntax = syntax.Substring(4);
            }
            else if (syntax.StartsWith("random:", StringComparison.InvariantCultureIgnoreCase)) // Example: $random:domain1.com|domain2.com$
            {
                parseType = CustomUploaderResponseParseType.Random;
                syntax = syntax.Substring(7);
            }
            else // Example: $1,1$
            {
                parseType = CustomUploaderResponseParseType.Regex;
            }

            if (!string.IsNullOrEmpty(syntax))
            {
                if (output)
                {
                    switch (parseType)
                    {
                        case CustomUploaderResponseParseType.Regex:
                            return ParseRegexSyntax(syntax);
                        case CustomUploaderResponseParseType.Json:
                            return ParseJsonSyntax(syntax);
                        case CustomUploaderResponseParseType.Xml:
                            return ParseXmlSyntax(syntax);
                    }
                }

                if (parseType == CustomUploaderResponseParseType.Random)
                {
                    return ParseRandomSyntax(syntax);
                }
            }

            return null;
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
            return Helpers.ParseJSON(response, syntaxJsonPath);
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

        private string ParseRandomSyntax(string syntax)
        {
            string[] values = syntax.Split('|');

            if (values.Length > 0)
            {
                return values[MathHelpers.Random(values.Length - 1)];
            }

            return "";
        }
    }
}