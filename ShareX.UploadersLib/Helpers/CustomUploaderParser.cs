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

        public bool IsOutput { get; set; }
        public string Response { get; private set; }
        public List<Match> RegexMatches { get; private set; }

        public CustomUploaderParser()
        {
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
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            StringBuilder result = new StringBuilder();

            bool syntaxStart = false;
            int syntaxStartIndex = 0;
            bool escape = false;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == SyntaxChar && !escape)
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
                            string syntax = text.Substring(syntaxStartIndex, syntaxLength);
                            string syntaxResult = ParseSyntax(syntax);

                            if (!string.IsNullOrEmpty(syntaxResult))
                            {
                                result.Append(syntaxResult);
                            }
                        }
                    }

                    escape = false;
                }
                else if (text[i] == '\\' && !escape)
                {
                    escape = true;
                }
                else if (!syntaxStart)
                {
                    result.Append(text[i]);
                    escape = false;
                }
            }

            return result.ToString();
        }

        private string ParseSyntax(string syntax)
        {
            if (IsOutput)
            {
                if (syntax.Equals("response", StringComparison.InvariantCultureIgnoreCase)) // Example: $response$
                {
                    return Response;
                }
                else if (syntax.StartsWith("regex:", StringComparison.InvariantCultureIgnoreCase)) // Example: $regex:1|1$
                {
                    return ParseSyntaxRegex(syntax.Substring(6));
                }
                else if (syntax.StartsWith("json:", StringComparison.InvariantCultureIgnoreCase)) // Example: $json:Files[0].URL$
                {
                    return ParseSyntaxJson(syntax.Substring(5));
                }
                else if (syntax.StartsWith("xml:", StringComparison.InvariantCultureIgnoreCase)) // Example: $xml:/Files/File[1]/URL$
                {
                    return ParseSyntaxXml(syntax.Substring(4));
                }
            }

            if (syntax.StartsWith("random:", StringComparison.InvariantCultureIgnoreCase)) // Example: $random:domain1.com|domain2.com$
            {
                return ParseSyntaxRandom(syntax.Substring(7));
            }
            else if (syntax.StartsWith("select:", StringComparison.InvariantCultureIgnoreCase)) // Example: $select:domain1.com|domain2.com$
            {
                return ParseSyntaxSelect(syntax.Substring(7));
            }
            else if (syntax.Equals("prompt", StringComparison.InvariantCultureIgnoreCase)) // Example: prompt
            {
                return ParseSyntaxPrompt();
            }
            else if (syntax.StartsWith("prompt:", StringComparison.InvariantCultureIgnoreCase)) // Example: $prompt:default value$
            {
                return ParseSyntaxPrompt(syntax.Substring(7));
            }

            // Invalid syntax
            return null;
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

        private string ParseSyntaxPrompt(string defaultValue = null)
        {
            using (InputBox inputBox = new InputBox("ShareX - Prompt", defaultValue))
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