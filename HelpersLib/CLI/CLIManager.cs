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

using System;
using System.Collections.Generic;

namespace HelpersLib.CLI
{
    public class CLIManager
    {
        public List<CLICommand> Commands { get; set; }

        public List<CLICommandAction> Actions { get; set; }

        private string input;
        private CLIParser parser;

        public CLIManager(string text)
        {
            input = text;
            parser = new CLIParser();
        }

        public bool Parse()
        {
            bool result = false;

            try
            {
                List<string> commands = parser.Parse(input);

                CLICommand lastCommand = null;

                foreach (string cmd in commands)
                {
                    if (cmd[0] == '-')
                    {
                        string command = cmd.Substring(1);
                        lastCommand = new CLICommand(command);
                        Commands.Add(lastCommand);
                    }
                    else if (lastCommand != null && string.IsNullOrEmpty(lastCommand.Parameter))
                    {
                        lastCommand.Parameter = cmd;
                    }
                    else
                    {
                        throw new Exception("Command not starting with '-' or more than one parameter exist.");
                    }
                }

                result = true;
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return result;
        }

        public void ExecuteActions()
        {
            foreach (CLICommandAction action in Actions)
            {
                action.CheckCommands(Commands);
            }
        }
    }
}