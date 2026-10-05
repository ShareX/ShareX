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

namespace ShareX.Platform.Linux;

/// <summary>Linux: the distribution's pretty name from os-release, root for elevation, and the sudo or wheel group for administrators.</summary>
public sealed class UnixSystemInfoService : ISystemInfoService
{
    private static readonly string[] AdministratorGroups = ["wheel", "sudo", "admin"];

    private readonly PlatformInfo info;
    private readonly ICommandRunner runner;
    private readonly Lazy<bool> administratorGroupMember;

    public UnixSystemInfoService(PlatformInfo info, ICommandRunner runner)
    {
        this.info = info;
        this.runner = runner;
        administratorGroupMember = new Lazy<bool>(ReadAdministratorGroupMember);
    }

    public string OperatingSystemName => info.Distribution?.PrettyName is { Length: > 0 } name ? name : System.Runtime.InteropServices.RuntimeInformation.OSDescription;

    public bool IsElevated => Environment.IsPrivilegedProcess;

    public bool IsAdministratorGroupMember => administratorGroupMember.Value;

    public bool IsTabletMode => false;

    public GpuAdapter? GetPreferredGpu() => null;

    public string GetSingleInstanceMutexName(string name) => @"Global\" + name + "-" + Environment.UserName;

    // .NET puts the socket in /tmp here, well inside the 108 character limit.
    public string GetSingleInstancePipeName(string name) => name;

    public string? GetModifierKeyName(HotkeyModifiers modifier) => modifier == HotkeyModifiers.Super ? "Super" : null;

    private bool ReadAdministratorGroupMember()
    {
        try
        {
            CommandResult result = runner.RunAsync("id", ["-Gn"], timeout: TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
            return result.Success && IsAdministratorGroupList(result.StandardOutputText);
        }
        catch (Exception e) when (e is TimeoutException or System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            return IsElevated;
        }
    }

    internal static bool IsAdministratorGroupList(string groups) =>
        groups.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Any(group => AdministratorGroups.Contains(group, StringComparer.Ordinal));
}
