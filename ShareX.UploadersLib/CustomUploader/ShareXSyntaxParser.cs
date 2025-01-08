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

using System.Collections.Generic;
using System.Text;

namespace ShareX.UploadersLib
{
    public abstract class ShareXSyntaxParser
    {
        public virtual char SyntaxStart { get; } = '{';
        public virtual char SyntaxEnd { get; } = '}';
        public virtual char SyntaxParameterStart { get; } = ':';
        public virtual char SyntaxParameterDelimiter { get; } = '|';
        public virtual char SyntaxEscape { get; } = '\\';

        public virtual string Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            return Parse(text, false, 0, out _);
        }

        private string Parse(string text, bool isFunction, int startPosition, out int endPosition)
        {
            StringBuilder sbOutput = new StringBuilder();
            bool escape = false;
            int i;

            for (i = startPosition; i < text.Length; i++)
            {
                char c = text[i];

                if (!escape)
                {
                    if (c == SyntaxStart)
                    {
                        string parsed = Parse(text, true, i + 1, out i);
                        sbOutput.Append(parsed);
                        continue;
                    }
                    else if (c == SyntaxEnd || c == SyntaxParameterDelimiter)
                    {
                        break;
                    }
                    else if (c == SyntaxEscape)
                    {
                        escape = true;
                        continue;
                    }
                    else if (isFunction && c == SyntaxParameterStart)
                    {
                        List<string> parameters = new List<string>();

                        do
                        {
                            string parsed = Parse(text, false, i + 1, out i);
                            parameters.Add(parsed);
                        } while (i < text.Length && text[i] == SyntaxParameterDelimiter);

                        endPosition = i;

                        return CallFunction(sbOutput.ToString(), parameters.ToArray());
                    }
                }

                escape = false;
                sbOutput.Append(c);
            }

            endPosition = i;

            if (isFunction)
            {
                return CallFunction(sbOutput.ToString());
            }

            return sbOutput.ToString();
        }

        protected abstract string CallFunction(string functionName, string[] parameters = null);
    }
}