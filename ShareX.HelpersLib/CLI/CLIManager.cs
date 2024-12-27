﻿#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2024 ShareX Team

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
using System.Text;

namespace ShareX.HelpersLib.CLI;

public class CLIManager
{
    public string[] Arguments { get; private set; }
    public List<CLICommand> Commands { get; private set; }
    public List<CLICommandAction> Actions { get; private set; }

    public CLIManager()
    {
        Commands = [];
        Actions = [];
    }

    public CLIManager(string[] arguments) : this()
    {
        Arguments = arguments;
    }

    public CLIManager(string arguments) : this()
    {
        Arguments = ParseCLI(arguments);
    }

    public bool ParseCommands()
    {
        try
        {
            CLICommand lastCommand = null;

            foreach (string argument in Arguments)
            {
                if (lastCommand == null || argument[0] == '-')
                {
                    CLICommand command = new();

                    if (argument[0] == '-')
                    {
                        command.IsCommand = true;
                        command.Command = argument[1..];
                        lastCommand = command;
                    } else
                    {
                        command.Command = argument;
                    }

                    Commands.Add(command);
                } else
                {
                    lastCommand.Parameter = argument;
                    lastCommand = null;
                }
            }

            return true;
        } catch (Exception e)
        {
            DebugHelper.WriteException(e);
        }

        return false;
    }

    private static string[] ParseCLI(string arguments)
    {
        List<string> commands = [];

        bool inDoubleQuotes = false;

        for (int i = 0, start = 0; i < arguments.Length; i++)
        {
            if (!inDoubleQuotes && char.IsWhiteSpace(arguments[i]) || inDoubleQuotes && arguments[i] == '"')
            {
                string command = arguments[start..i ];

                if (!string.IsNullOrEmpty(command))
                {
                    commands.Add(command);
                }

                if (inDoubleQuotes) inDoubleQuotes = false;
                start = i + 1;
            } else if (arguments[i] == '"')
            {
                inDoubleQuotes = true;
                start = i + 1;
            }
        }

        return [.. commands];
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
                    command1 = command1[1..];
                }

                foreach (CLICommand command2 in Commands.Where(x => x != null && x.IsCommand))
                {
                    if (command2.CheckCommand(command1))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public CLICommand GetCommand(string command)
    {
        return Commands.Find(x => x.CheckCommand(command));
    }

    public string GetParameter(string command)
    {
        CLICommand cliCommand = GetCommand(command);

        return cliCommand?.Parameter;
    }

    public void ExecuteActions()
    {
        foreach (CLICommandAction action in Actions)
        {
            action.CheckCommands(Commands);
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        foreach (CLICommand command in Commands)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }

            sb.Append(command);
        }

        return sb.ToString();
    }
}