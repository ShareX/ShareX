#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace UploadersLib.HelperClasses
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
        public bool AutoUseResponse { get; set; }

        public string Input { get; set; }

        public string ResultURL { get; private set; }
        public string ResultThumbnailURL { get; private set; }
        public string ResultDeletionURL { get; private set; }

        private List<Match> regexResult;

        public CustomUploaderItem()
        {
            Arguments = new Dictionary<string, string>();
            RegexList = new List<string>();
            AutoUseResponse = true;
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

        public Dictionary<string, string> ParseArguments()
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> arg in Arguments)
            {
                string value = arg.Value;

                if (!string.IsNullOrEmpty(Input))
                {
                    value = value.Replace("%input", Input);
                    value = value.Replace("$input$", Input);
                }

                NameParser parser = new NameParser(NameParserType.Text);
                value = parser.Parse(value);

                arguments.Add(arg.Key, value);
            }

            return arguments;
        }

        public void Parse(string response)
        {
            ResultURL = "";
            ResultThumbnailURL = "";
            ResultDeletionURL = "";
            regexResult = new List<Match>();

            foreach (string regex in RegexList)
            {
                regexResult.Add(Regex.Match(response, regex));
            }

            if (!string.IsNullOrEmpty(URL))
            {
                ResultURL = ParseURL(URL);
            }
            else if (AutoUseResponse)
            {
                ResultURL = response;
            }

            if (!string.IsNullOrEmpty(ThumbnailURL))
            {
                ResultThumbnailURL = ParseURL(ThumbnailURL);
            }

            if (!string.IsNullOrEmpty(DeletionURL))
            {
                ResultDeletionURL = ParseURL(DeletionURL);
            }
        }

        private string ParseURL(string url)
        {
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

                        return match.Groups[@group].Value;
                    }

                    return match.Value;
                }
            }

            return null;
        }
    }
}