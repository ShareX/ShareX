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

using System;

namespace ShareX.HelpersLib
{
    public class CLICommand
    {
        public string Command { get; set; }
        public string Parameter { get; set; }
        public bool IsCommand { get; set; } // Starts with hyphen?

        public CLICommand(string command = null, string parameter = null)
        {
            Command = command;
            Parameter = parameter;
        }

        public bool CheckCommand(string command, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            return !string.IsNullOrEmpty(Command) && Command.Equals(command, comparisonType);
        }

        public override string ToString()
        {
            string text = "";

            if (IsCommand)
            {
                text += "-";
            }

            text += Command;

            if (!string.IsNullOrEmpty(Parameter))
            {
                text += " \"" + Parameter + "\"";
            }

            return text;
        }
    }
}