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
using System.Linq;

namespace HelpersLib
{
    public class CLIManager
    {
        public string[] Arguments { get; private set; }
        public List<CLICommand> Commands { get; private set; }
        public List<CLICommandAction> Actions { get; private set; }

        public CLIManager()
        {
            Commands = new List<CLICommand>();
            Actions = new List<CLICommandAction>();
        }

        public CLIManager(string[] arguments)
            : this()
        {
            Arguments = arguments;
        }

        public CLIManager(string arguments)
            : this()
        {
            CLIParser parser = new CLIParser();
            Arguments = parser.Parse(arguments).ToArray();
        }

        public bool Parse()
        {
            try
            {
                CLICommand lastCommand = null;

                foreach (string argument in Arguments)
                {
                    if (lastCommand == null || argument[0] == '-')
                    {
                        lastCommand = new CLICommand();

                        if (argument[0] == '-')
                        {
                            lastCommand.IsCommand = true;
                            lastCommand.Command = argument.Substring(1);
                        }
                        else
                        {
                            lastCommand.Command = argument;
                        }

                        Commands.Add(lastCommand);
                    }
                    else if (string.IsNullOrEmpty(lastCommand.Parameter))
                    {
                        lastCommand.Parameter = argument;
                    }
                    else
                    {
                        throw new Exception("Argument not starting with '-' or more than one parameter exist.");
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }

        public bool IsCommandExist(params string[] commands)
        {
            if (Commands != null && commands != null)
            {
                foreach (string command in commands.Where(x => !string.IsNullOrEmpty(x)))
                {
                    string command1 = command;

                    if (command1[0] == '-')
                    {
                        command1 = command1.Substring(1);
                    }

                    foreach (CLICommand command2 in Commands.Where(x => x != null && x.IsCommand && !string.IsNullOrEmpty(x.Command)))
                    {
                        if (command1.Equals(command2.Command, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
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