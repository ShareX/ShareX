#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ShareX.UploadersLib
{
    public class CustomUploaderItem
    {
        public string Name { get; set; }
        public CustomUploaderRequestType RequestType { get; set; }
        public string RequestURL { get; set; }
        public string FileFormName { get; set; }
        public Dictionary<string, string> Arguments { get; set; }
        public ResponseType ResponseType { get; set; }
        public List<string> RegexList { get; set; }
        public string URL { get; set; }
        public string ThumbnailURL { get; set; }
        public string DeletionURL { get; set; }

        private List<Match> regexResult;

        public CustomUploaderItem()
        {
            Arguments = new Dictionary<string, string>();
            RegexList = new List<string>();
        }

        public CustomUploaderItem(string name)
            : this()
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

        public Dictionary<string, string> ParseArguments(string input = null)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> arg in Arguments)
            {
                string value = arg.Value;

                value = value.Replace("%input", "$input$"); // For backward compatibility
                value = NameParser.Parse(NameParserType.Text, value);
                value = value.Replace("$input$", input);

                arguments.Add(arg.Key, value);
            }

            return arguments;
        }

        public void ParseResponse(UploadResult result, bool isShortenedURL = false)
        {
            if (result != null && !string.IsNullOrEmpty(result.Response))
            {
                regexResult = ParseRegexList(result.Response);

                string url;

                if (!string.IsNullOrEmpty(URL))
                {
                    url = ParseURL(URL);
                }
                else
                {
                    url = result.Response;
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

        private List<Match> ParseRegexList(string response)
        {
            List<Match> result = new List<Match>();

            if (RegexList != null)
            {
                foreach (string regex in RegexList)
                {
                    result.Add(Regex.Match(response, regex));
                }
            }

            return result;
        }

        private string ParseURL(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            StringBuilder result = new StringBuilder();

            bool regexStart = false;
            int regexStartIndex = 0;

            for (int i = 0; i < url.Length; i++)
            {
                if (url[i] == '$')
                {
                    if (!regexStart)
                    {
                        regexStart = true;
                        regexStartIndex = i;
                    }
                    else
                    {
                        string regexResult = ParseRegexSyntax(url.Substring(regexStartIndex + 1, i - regexStartIndex - 1));

                        if (!string.IsNullOrEmpty(regexResult))
                        {
                            result.Append(regexResult);
                        }

                        regexStart = false;
                        continue;
                    }
                }

                if (!regexStart)
                {
                    result.Append(url[i]);
                }
            }

            return result.ToString();
        }

        private string ParseRegexSyntax(string text)
        {
            if (text.Length > 0)
            {
                int i = 0;
                string regexIndexString = "";
                int regexIndex;
                bool isGroupRegex = false;

                for (; i < text.Length; i++)
                {
                    if (char.IsDigit(text[i]))
                    {
                        regexIndexString += text[i];
                    }
                    else
                    {
                        if (text[i] == ',')
                        {
                            isGroupRegex = true;
                        }

                        break;
                    }
                }

                if (regexIndexString.Length > 0 && int.TryParse(regexIndexString, out regexIndex))
                {
                    Match match = regexResult[regexIndex - 1];

                    if (isGroupRegex && i + 1 < text.Length)
                    {
                        string group = text.Substring(i + 1);
                        int groupNumber;

                        if (int.TryParse(group, out groupNumber))
                        {
                            return match.Groups[groupNumber].Value;
                        }

                        return match.Groups[group].Value;
                    }

                    return match.Value;
                }
            }

            return null;
        }
    }
}