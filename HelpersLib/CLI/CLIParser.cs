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

using System.Collections.Generic;

namespace HelpersLib
{
    public class CLIParser
    {
        private string input;
        private int index;

        public List<string> Parse(string text)
        {
            input = text;

            List<string> commands = new List<string>();

            string command;

            for (index = 0; index < input.Length; index++)
            {
                command = ParseUntilWhiteSpace();

                if (!string.IsNullOrEmpty(command))
                {
                    commands.Add(command);
                }
            }

            return commands;
        }

        private string ParseUntilWhiteSpace()
        {
            int start = index;

            for (; index < input.Length; index++)
            {
                if (char.IsWhiteSpace(input[index]))
                {
                    break;
                }

                if (input[index] == '"' && (index + 1) < input.Length)
                {
                    return ParseUntilDoubleQuotes();
                }
            }

            return input.Substring(start, index - start);
        }

        private string ParseUntilDoubleQuotes()
        {
            int start = ++index;

            for (; index < input.Length; index++)
            {
                if (input[index] == '"')
                {
                    break;
                }
            }

            return input.Substring(start, index - start);
        }
    }
}