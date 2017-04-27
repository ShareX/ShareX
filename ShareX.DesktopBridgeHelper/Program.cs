#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace DesktopBridgeHelper
{
    internal class Program
    {
        private const string TaskID = "ShareX";

        private static int Main(string[] args)
        {
            return MainAsync(args).GetAwaiter().GetResult();
        }

        private async static Task<int> MainAsync(string[] args)
        {
            try
            {
                if (args.Length > 0)
                {
                    string argument = args[0];

                    if (argument.Equals("-StartupState", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return (int)await GetStartupState();
                    }
                    else if (argument.Equals("-StartupEnable", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return (int)await SetStartupState(true);
                    }
                    else if (argument.Equals("-StartupDisable", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return (int)await SetStartupState(false);
                    }
                }
                else
                {
                    string path = GetAbsolutePath("ShareX.exe");
                    Process.Start(path, "-silent");
                    return 0;
                }
            }
            catch
            {
            }

            return -1;
        }

        private async static Task<StartupTaskState> GetStartupState()
        {
            StartupTask startupTask = await StartupTask.GetAsync(TaskID);
            return startupTask.State;
        }

        private async static Task<StartupTaskState> SetStartupState(bool enable)
        {
            StartupTask startupTask = await StartupTask.GetAsync(TaskID);

            if (enable)
            {
                return await startupTask.RequestEnableAsync();
            }
            else
            {
                startupTask.Disable();
                return StartupTaskState.Disabled;
            }
        }

        private static string GetAbsolutePath(string path)
        {
            if (!Path.IsPathRooted(path)) // Is relative path?
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }

            return Path.GetFullPath(path);
        }
    }
}