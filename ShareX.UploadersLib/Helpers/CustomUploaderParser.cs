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

using Newtonsoft.Json.Linq;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.XPath;

namespace ShareX.UploadersLib
{
    public class CustomUploaderParser
    {
        public const char SyntaxChar = '$';
        public const char SyntaxParameterChar = '|';
        public const char SyntaxEscapeChar = '\\';

        public bool IsOutput { get; set; }
        public string Filename { get; private set; }
        public string Input { get; private set; }
        public string Response { get; private set; }
        public List<Match> RegexMatches { get; private set; }

        public CustomUploaderParser()
        {
            IsOutput = false;
        }

        public CustomUploaderParser(string filename, string input)
        {
            Filename = filename;
            Input = input;

            IsOutput = false;
        }

        public CustomUploaderParser(string response, List<string> regexList)
        {
            Response = response;

            RegexMatches = new List<Match>();

            if (regexList != null)
            {
                foreach (string regex in regexList)
                {
                    Match match = Regex.Match(response, regex);
                    RegexMatches.Add(match);
                }
            }

            IsOutput = true;
        }

        public string Parse(string text)
        {
            return Parse(text, IsOutput);
        }

        public string Parse(string text, bool isOutput)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbSyntax = new StringBuilder();
            bool escapeNext = false;
            bool parsingSyntax = false;

            for (int i = 0; i < text.Length; i++)
            {
                if (!escapeNext && text[i] == SyntaxChar)
                {
                    if (!parsingSyntax)
                    {
                        parsingSyntax = true;

                        sbSyntax.Clear();
                    }
                    else
                    {
                        parsingSyntax = false;

                        string syntax = sbSyntax.ToString();

                        if (!string.IsNullOrEmpty(syntax))
                        {
                            string syntaxResult = ParseSyntax(syntax, isOutput);

                            if (!string.IsNullOrEmpty(syntaxResult))
                            {
                                sbResult.Append(syntaxResult);
                            }
                        }
                    }
                }
                else if (!escapeNext && text[i] == SyntaxEscapeChar)
                {
                    escapeNext = true;
                }
                else
                {
                    escapeNext = false;

                    if (!parsingSyntax)
                    {
                        sbResult.Append(text[i]);
                    }
                    else
                    {
                        sbSyntax.Append(text[i]);
                    }
                }
            }

            return sbResult.ToString();
        }

        private string ParseSyntax(string syntax, bool isOutput)
        {
            string value;

            if (isOutput)
            {
                if (CheckKeyword(syntax, "response")) // Example: $response$
                {
                    return Response;
                }
                else if (CheckKeyword(syntax, "regex", out value)) // Examples: $regex:1$ $regex:1|1$ $regex:1|thumbnail$
                {
                    return ParseSyntaxRegex(value);
                }
                else if (CheckKeyword(syntax, "json", out value)) // Example: $json:Files[0].URL$
                {
                    return ParseSyntaxJson(value);
                }
                else if (CheckKeyword(syntax, "xml", out value)) // Example: $xml:/Files/File[1]/URL$
                {
                    return ParseSyntaxXml(value);
                }
            }

            if (CheckKeyword(syntax, "input")) // Example: $input$
            {
                return Input;
            }
            else if (CheckKeyword(syntax, "filename")) // Example: $filename$
            {
                return Filename;
            }
            else if (CheckKeyword(syntax, "random", out value)) // Example: $random:domain1.com|domain2.com$
            {
                return ParseSyntaxRandom(value);
            }
            else if (CheckKeyword(syntax, "select", out value)) // Example: $select:domain1.com|domain2.com$
            {
                return ParseSyntaxSelect(value);
            }
            else if (CheckKeyword(syntax, "prompt", out value)) // Examples: $prompt$ $prompt:title$ $prompt:title|default value$
            {
                return ParseSyntaxPrompt(value);
            }

            // Invalid syntax
            return null;
        }

        private bool CheckKeyword(string syntax, string keyword)
        {
            return CheckKeyword(syntax, keyword, out _);
        }

        private bool CheckKeyword(string syntax, string keyword, out string value)
        {
            value = null;

            if (syntax.Equals(keyword, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            else if (syntax.StartsWith(keyword + ":", StringComparison.InvariantCultureIgnoreCase))
            {
                value = syntax.Substring(keyword.Length + 1);
                return true;
            }

            return false;
        }

        private string ParseSyntaxRegex(string syntax)
        {
            if (!string.IsNullOrEmpty(syntax))
            {
                string regexIndexString = "";
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
                        if (syntax[i] == SyntaxParameterChar || syntax[i] == ',') // , for backward compatibility
                        {
                            isGroupRegex = true;
                        }

                        break;
                    }
                }

                if (regexIndexString.Length > 0 && int.TryParse(regexIndexString, out int regexIndex))
                {
                    Match match = RegexMatches[regexIndex - 1];

                    if (isGroupRegex && i + 1 < syntax.Length)
                    {
                        string group = syntax.Substring(i + 1);

                        if (int.TryParse(group, out int groupNumber))
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

        // http://goessner.net/articles/JsonPath/
        private string ParseSyntaxJson(string syntaxJsonPath)
        {
            if (!string.IsNullOrEmpty(syntaxJsonPath))
            {
                return (string)JToken.Parse(Response).SelectToken("$." + syntaxJsonPath);
            }

            return null;
        }

        // http://www.w3schools.com/xsl/xpath_syntax.asp
        // https://msdn.microsoft.com/en-us/library/ms256086(v=vs.110).aspx
        private string ParseSyntaxXml(string syntaxXPath)
        {
            if (!string.IsNullOrEmpty(syntaxXPath))
            {
                using (StringReader sr = new StringReader(Response))
                {
                    XPathDocument doc = new XPathDocument(sr);
                    XPathNavigator nav = doc.CreateNavigator();
                    XPathNavigator node = nav.SelectSingleNode(syntaxXPath);

                    if (node != null)
                    {
                        return node.Value;
                    }
                }
            }

            return null;
        }

        private string ParseSyntaxRandom(string syntax)
        {
            if (!string.IsNullOrEmpty(syntax))
            {
                string[] values = syntax.Split(SyntaxParameterChar);

                if (values.Length > 0)
                {
                    return values[MathHelpers.Random(values.Length - 1)];
                }
            }

            return null;
        }

        private string ParseSyntaxSelect(string syntax)
        {
            if (!string.IsNullOrEmpty(syntax))
            {
                string[] values = syntax.Split(SyntaxParameterChar).Where(x => !string.IsNullOrEmpty(x)).ToArray();

                if (values.Length > 0)
                {
                    using (ParserSelectForm form = new ParserSelectForm(values))
                    {
                        form.ShowDialog();
                        return form.SelectedText;
                    }
                }
            }

            return null;
        }

        private string ParseSyntaxPrompt(string syntax)
        {
            string title = "ShareX - Prompt", defaultValue = "";

            if (!string.IsNullOrEmpty(syntax))
            {
                if (syntax.Contains(SyntaxParameterChar))
                {
                    int index = syntax.IndexOf(SyntaxParameterChar);
                    title = syntax.Remove(index);
                    defaultValue = syntax.Substring(index + 1);
                }
                else
                {
                    title = syntax;
                }
            }

            using (InputBox inputBox = new InputBox(title, defaultValue))
            {
                if (inputBox.ShowDialog() == DialogResult.OK)
                {
                    return inputBox.InputText;
                }
            }

            return defaultValue;
        }
    }
}