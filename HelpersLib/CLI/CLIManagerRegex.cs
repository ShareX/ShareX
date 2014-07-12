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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace HelpersLib.CLI
{
    public enum ActionInput
    {
        None,
        Text,
        Number
    }

    public class CLIManagerRegex
    {
        public List<CLICommandRegex> Commands { get; set; }

        public Action<string> FilePathAction { get; set; }

        public bool Parse(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                text = text.Trim();

                foreach (CLICommandRegex command in Commands)
                {
                    if (command.Parse(text)) return true;
                }

                if (FilePathAction != null && File.Exists(text))
                {
                    FilePathAction(text);
                }
            }

            return false;
        }
    }

    public class CLICommandRegex
    {
        public string Commands;
        public ActionInput InputType;
        public Action DefaultAction;
        public Action<string> TextAction;
        public Action<int> NumberAction;

        private CLICommandRegex(string commands)
        {
            Commands = commands;
        }

        public CLICommandRegex(string commands, Action action)
            : this(commands)
        {
            InputType = ActionInput.None;
            DefaultAction = action;
        }

        public CLICommandRegex(string commands, Action<int> action)
            : this(commands)
        {
            InputType = ActionInput.Number;
            NumberAction = action;
        }

        public CLICommandRegex(string commands, Action<string> action)
            : this(commands)
        {
            InputType = ActionInput.Text;
            TextAction = action;
        }

        public bool Parse(string text)
        {
            string input;

            switch (InputType)
            {
                default:
                case ActionInput.None:
                    if (Parse(text, out input))
                    {
                        DefaultAction();
                        return true;
                    }
                    break;
                case ActionInput.Number:
                    if (Parse(text, out input))
                    {
                        if (!string.IsNullOrEmpty(input))
                        {
                            int num;
                            if (int.TryParse(input, out num))
                            {
                                NumberAction(num);
                                return true;
                            }
                        }
                    }
                    break;
                case ActionInput.Text:
                    if (Parse(text, out input))
                    {
                        if (!string.IsNullOrEmpty(input))
                        {
                            TextAction(input);
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

        private bool Parse(string text, out string input)
        {
            input = null;

            string inputPattern = @"(?<Input>\w+|\x22.+?\x22)";
            string pattern = string.Format(@"-+(?:{0})(?:\s+{1}|\s*=+\s*{1}|\s+|\z)", Commands, inputPattern);

            Match match = Regex.Match(text, pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            if (match.Success)
            {
                Group group = match.Groups["Input"];

                if (group.Success)
                {
                    input = group.Value;
                }
            }

            return match.Success;
        }
    }
}