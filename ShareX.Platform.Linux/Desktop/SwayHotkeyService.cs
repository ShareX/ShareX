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
using ShareX.Platform.Linux.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShareX.Platform.Linux.Desktop;

/// <summary>
/// Global hotkeys on sway, whose portal has no GlobalShortcuts interface. Each hotkey is bound with sway's IPC as a "nop" command
/// and ShareX listens for sway's binding events to learn when it was pressed. Keys sway's configuration already binds are
/// reported as in use. The binds live only in the running session and are removed when ShareX exits.
/// </summary>
public sealed class SwayHotkeyService : IHotkeyService
{
    private const string CommandPrefix = "nop sharex-";

    private readonly ICommandRunner runner;
    private readonly object syncRoot = new object();
    private readonly Dictionary<int, (PlatformHotkey Hotkey, string Combination)> bound = new();
    private Process? listener;
    private bool disposed;

    public SwayHotkeyService(ICommandRunner runner)
    {
        this.runner = runner;
        Support = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SWAYSOCK")) && runner.Exists("swaymsg")
            ? FeatureSupport.Supported
            : FeatureSupport.NotSupported("ShareX cannot reach sway's IPC socket (SWAYSOCK).");
    }

    public FeatureSupport Support { get; }

    public event EventHandler<HotkeyPressedEventArgs>? HotkeyPressed;

    public HotkeyRegistrationStatus Register(int id, PlatformHotkey hotkey)
    {
        if (!Support.IsSupported)
        {
            return HotkeyRegistrationStatus.NotSupported;
        }

        if (ToCombination(hotkey) is not string combination)
        {
            return HotkeyRegistrationStatus.UnsupportedKey;
        }

        Unregister(id);

        if (Run("swaymsg", ["-t", "get_config"]) is string config && GetConfiguredCombinations(config).Contains(Normalize(combination)))
        {
            return HotkeyRegistrationStatus.InUse;
        }

        if (!IsSuccess(Run("swaymsg", ["bindsym", "--no-repeat", combination, CommandPrefix + id.ToString(CultureInfo.InvariantCulture)])))
        {
            return HotkeyRegistrationStatus.Failed;
        }

        lock (syncRoot)
        {
            bound[id] = (hotkey, combination);
        }

        EnsureListening();
        return HotkeyRegistrationStatus.Registered;
    }

    public bool Unregister(int id)
    {
        (PlatformHotkey Hotkey, string Combination) entry;

        lock (syncRoot)
        {
            if (!bound.Remove(id, out entry))
            {
                return false;
            }
        }

        Run("swaymsg", ["unbindsym", entry.Combination]);
        return true;
    }

    public void UnregisterAll()
    {
        int[] ids;

        lock (syncRoot)
        {
            ids = bound.Keys.ToArray();
        }

        foreach (int id in ids)
        {
            Unregister(id);
        }
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        UnregisterAll();

        try
        {
            if (listener is { HasExited: false })
            {
                listener.Kill();
            }
        }
        catch (InvalidOperationException)
        {
        }

        listener?.Dispose();
    }

    private void EnsureListening()
    {
        lock (syncRoot)
        {
            if (listener != null || disposed)
            {
                return;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo("swaymsg")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            foreach (string argument in new[] { "-m", "-r", "-t", "subscribe", "[\"binding\"]" })
            {
                startInfo.ArgumentList.Add(argument);
            }

            listener = Process.Start(startInfo);

            if (listener == null)
            {
                return;
            }

            listener.ErrorDataReceived += (_, _) => { };
            listener.OutputDataReceived += (_, e) =>
            {
                if (ParseBindingEvent(e.Data) is int id)
                {
                    PlatformHotkey hotkey;

                    lock (syncRoot)
                    {
                        if (!bound.TryGetValue(id, out (PlatformHotkey Hotkey, string Combination) entry))
                        {
                            return;
                        }

                        hotkey = entry.Hotkey;
                    }

                    HotkeyPressed?.Invoke(this, new HotkeyPressedEventArgs(id, hotkey));
                }
            };
            listener.BeginOutputReadLine();
            listener.BeginErrorReadLine();
        }
    }

    private string? Run(string command, IReadOnlyList<string> arguments)
    {
        try
        {
            CommandResult result = Task.Run(() => runner.RunAsync(command, arguments, timeout: TimeSpan.FromSeconds(3))).GetAwaiter().GetResult();
            return result.Success ? result.StandardOutputText : null;
        }
        catch (Exception e) when (e is TimeoutException or System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            return null;
        }
    }

    /// <summary>sway answers commands with [{"success": true}, ...].</summary>
    internal static bool IsSuccess(string? output)
    {
        if (string.IsNullOrEmpty(output))
        {
            return false;
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(output);
            return document.RootElement.ValueKind == JsonValueKind.Array && document.RootElement.EnumerateArray()
                .All(x => x.TryGetProperty("success", out JsonElement success) && success.ValueKind == JsonValueKind.True);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>The hotkey id of a binding event line for one of ShareX's nop commands, else null.</summary>
    internal static int? ParseBindingEvent(string? line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return null;
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(line);
            JsonElement root = document.RootElement;

            if (root.TryGetProperty("change", out JsonElement change) && change.GetString() == "run" &&
                root.TryGetProperty("binding", out JsonElement binding) && binding.TryGetProperty("command", out JsonElement command) &&
                command.GetString() is string text && text.StartsWith(CommandPrefix, StringComparison.Ordinal) &&
                int.TryParse(text.AsSpan(CommandPrefix.Length), NumberStyles.Integer, CultureInfo.InvariantCulture, out int id))
            {
                return id;
            }
        }
        catch (JsonException)
        {
        }

        return null;
    }

    /// <summary>sway's name for a hotkey, for example "Ctrl+Shift+Print".</summary>
    internal static string? ToCombination(PlatformHotkey hotkey)
    {
        if (!XKeyMap.TryGetKeysym(hotkey.KeyCode, out _, out string key))
        {
            return null;
        }

        List<string> parts = new List<string>(5);
        if (hotkey.Modifiers.HasFlag(HotkeyModifiers.Super)) parts.Add("Mod4");
        if (hotkey.Modifiers.HasFlag(HotkeyModifiers.Control)) parts.Add("Ctrl");
        if (hotkey.Modifiers.HasFlag(HotkeyModifiers.Alt)) parts.Add("Mod1");
        if (hotkey.Modifiers.HasFlag(HotkeyModifiers.Shift)) parts.Add("Shift");
        parts.Add(key);
        return string.Join("+", parts);
    }

    /// <summary>The key combinations bound in sway's configuration (from swaymsg -t get_config), normalised.</summary>
    internal static HashSet<string> GetConfiguredCombinations(string getConfigOutput)
    {
        HashSet<string> combinations = new HashSet<string>(StringComparer.Ordinal);
        string config;

        try
        {
            using JsonDocument document = JsonDocument.Parse(getConfigOutput);
            config = document.RootElement.TryGetProperty("config", out JsonElement text) ? text.GetString() ?? "" : "";
        }
        catch (JsonException)
        {
            return combinations;
        }

        Dictionary<string, string> variables = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (string rawLine in config.Split('\n'))
        {
            string[] words = rawLine.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length >= 3 && words[0] == "set" && words[1].StartsWith('$'))
            {
                variables[words[1]] = words[2];
                continue;
            }

            if (words.Length >= 2 && words[0] == "bindsym")
            {
                string? combination = words.Skip(1).FirstOrDefault(word => !word.StartsWith("--", StringComparison.Ordinal));

                if (combination != null)
                {
                    foreach ((string name, string value) in variables.OrderByDescending(x => x.Key.Length))
                    {
                        combination = combination.Replace(name, value, StringComparison.Ordinal);
                    }

                    combinations.Add(Normalize(combination));
                }
            }
        }

        return combinations;
    }

    /// <summary>Modifiers in a fixed order and canonical names, keys case-insensitive: "mod4+ctrl+mod1+shift+print".</summary>
    internal static string Normalize(string combination)
    {
        string[] order = ["mod4", "ctrl", "mod1", "shift"];
        List<string> parts = combination.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(part => part.ToLowerInvariant() switch
            {
                "control" => "ctrl",
                "alt" => "mod1",
                "super" or "logo" => "mod4",
                string other => other
            }).ToList();

        string key = parts.LastOrDefault() ?? "";
        IEnumerable<string> modifiers = order.Where(parts.Take(parts.Count - 1).Contains);
        return string.Join("+", modifiers.Append(key));
    }
}
