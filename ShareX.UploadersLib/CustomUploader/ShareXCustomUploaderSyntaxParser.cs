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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX.UploadersLib
{
    public class ShareXCustomUploaderSyntaxParser : ShareXSyntaxParser
    {
        private static IEnumerable<CustomUploaderFunction> Functions = Helpers.GetInstances<CustomUploaderFunction>();

        public string FileName { get; set; }
        public string Input { get; set; }
        public ResponseInfo ResponseInfo { get; set; }
        public bool URLEncode { get; set; } // Only URL encodes file name and input
        public bool UseNameParser { get; set; }
        public NameParserType NameParserType { get; set; } = NameParserType.Text;

        public ShareXCustomUploaderSyntaxParser()
        {
        }

        public ShareXCustomUploaderSyntaxParser(CustomUploaderInput input)
        {
            FileName = input.FileName;
            Input = input.Input;
        }

        public override string Parse(string text)
        {
            if (UseNameParser && !string.IsNullOrEmpty(text))
            {
                NameParser nameParser = new NameParser(NameParserType);
                EscapeHelper escapeHelper = new EscapeHelper();
                escapeHelper.KeepEscapeCharacter = true;
                text = escapeHelper.Parse(text, nameParser.Parse);
            }

            return base.Parse(text);
        }

        protected override string CallFunction(string functionName, string[] parameters = null)
        {
            if (string.IsNullOrEmpty(functionName))
            {
                throw new Exception("Function name cannot be empty.");
            }

            foreach (CustomUploaderFunction function in Functions)
            {
                if (function.Name.Equals(functionName, StringComparison.OrdinalIgnoreCase) ||
                    (function.Aliases != null && function.Aliases.Any(x => x.Equals(functionName, StringComparison.OrdinalIgnoreCase))))
                {
                    if (function.MinParameterCount > 0 && (parameters == null || parameters.Length < function.MinParameterCount))
                    {
                        throw new Exception($"Minimum parameter count for function \"{function.Name}\" is {function.MinParameterCount}.");
                    }

                    return function.Call(this, parameters);
                }
            }

            throw new Exception("Invalid function name: " + functionName);
        }
    }
}