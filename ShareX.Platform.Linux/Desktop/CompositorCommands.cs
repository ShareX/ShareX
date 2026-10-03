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
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShareX.Platform.Linux.Desktop;

/// <summary>Runs compositor IPC commands (hyprctl, swaymsg) and parses their JSON, treating any failure as "no answer".</summary>
internal sealed class CompositorCommands(ICommandRunner runner)
{
    public ICommandRunner Runner { get; } = runner;

    public bool Run(string command, IReadOnlyList<string> arguments, TimeSpan? timeout = null)
    {
        try
        {
            return Task.Run(() => Runner.RunAsync(command, arguments, timeout: timeout ?? TimeSpan.FromSeconds(2))).GetAwaiter().GetResult().Success;
        }
        catch (Exception e) when (e is TimeoutException or System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            return false;
        }
    }

    public string? ReadText(string command, IReadOnlyList<string> arguments)
    {
        try
        {
            CommandResult result = Task.Run(() => Runner.RunAsync(command, arguments, timeout: TimeSpan.FromSeconds(2))).GetAwaiter().GetResult();
            return result.Success ? result.StandardOutputText : null;
        }
        catch (Exception e) when (e is TimeoutException or System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            return null;
        }
    }

    public T? ReadJson<T>(string command, IReadOnlyList<string> arguments, Func<JsonElement, T> parse)
    {
        try
        {
            CommandResult result = Task.Run(() => Runner.RunAsync(command, arguments, timeout: TimeSpan.FromSeconds(5))).GetAwaiter().GetResult();

            if (result.Success)
            {
                using JsonDocument document = JsonDocument.Parse(result.StandardOutput);
                return parse(document.RootElement);
            }
        }
        catch (Exception e) when (e is JsonException or TimeoutException or System.ComponentModel.Win32Exception or InvalidOperationException or KeyNotFoundException or FormatException)
        {
        }

        return default;
    }

    public IReadOnlyList<T> ReadList<T>(string command, IReadOnlyList<string> arguments, Func<JsonElement, IReadOnlyList<T>> parse) =>
        ReadJson(command, arguments, parse) ?? Array.Empty<T>();
}
