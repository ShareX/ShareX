#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

using ShareX.Platform.Diagnostics;
using System;
using System.Linq;

namespace ShareX.Platform.MacOS;

public sealed class MacSystemInfoService : ISystemInfoService
{
    private readonly ICommandRunner runner;
    private readonly Lazy<string> name;
    private readonly Lazy<bool> admin;

    public MacSystemInfoService(ICommandRunner runner)
    {
        this.runner = runner;
        name = new Lazy<string>(() => "macOS " + (Run("sw_vers", "-productVersion") ?? Environment.OSVersion.Version.ToString()));
        admin = new Lazy<bool>(() => Run("id", "-Gn")?.Split(' ', StringSplitOptions.RemoveEmptyEntries).Contains("admin") == true);
    }

    public string OperatingSystemName => name.Value;

    public bool IsElevated => Environment.IsPrivilegedProcess;

    public bool IsAdministratorGroupMember => admin.Value;

    public bool IsTabletMode => false;

    public GpuAdapter? GetPreferredGpu() => null;

    private string? Run(string command, string argument)
    {
        try
        {
            CommandResult result = runner.RunAsync(command, [argument], timeout: TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
            return result.Success ? result.StandardOutputText.Trim() : null;
        }
        catch (Exception e) when (e is TimeoutException or System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            return null;
        }
    }
}
