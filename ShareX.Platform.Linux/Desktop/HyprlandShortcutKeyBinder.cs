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
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Linux.Desktop;

/// <summary>
/// Binds ShareX's hotkeys in Hyprland. Hyprland's GlobalShortcuts portal registers the shortcuts but leaves the keys to the user's
/// configuration, so ShareX adds runtime binds to them with hyprctl (Lua configurations through eval, classic ones through
/// keyword). Key combinations the configuration already uses are left alone and reported as in use. Nothing is written to the
/// configuration files; the binds disappear when ShareX exits, and are added again after the configuration reloads.
/// </summary>
internal sealed class HyprlandShortcutKeyBinder : IShortcutKeyBinder
{
    private const int ShiftMask = 1, ControlMask = 4, AltMask = 8, SuperMask = 64;

    private readonly ICommandRunner runner;
    private readonly string applicationId;
    private readonly object syncRoot = new object();
    private readonly List<(int Mask, string Key, string Combination)> bound = new();
    private IReadOnlyList<GlobalShortcut> applied = Array.Empty<GlobalShortcut>();
    private readonly CancellationTokenSource listening = new CancellationTokenSource();
    private bool? luaConfiguration;

    public HyprlandShortcutKeyBinder(ICommandRunner runner, string applicationId)
    {
        this.runner = runner;
        this.applicationId = applicationId;
        _ = Task.Run(() => ListenForReloadsAsync(listening.Token));
    }

    public bool IsInUse(PlatformHotkey hotkey)
    {
        if (!TryGetKey(hotkey, out int mask, out string key, out _))
        {
            return false;
        }

        lock (syncRoot)
        {
            if (bound.Any(x => x.Mask == mask && string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
        }

        // A bind ShareX made itself (left behind by an instance that could not clean up) does not block the key.
        string? json = Run("hyprctl", ["binds", "-j"]);
        return json != null && ParseBinds(json, applicationId).Any(x => !x.IsShareX && x.Mask == mask && string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));
    }

    public void Apply(IReadOnlyList<GlobalShortcut> shortcuts)
    {
        lock (syncRoot)
        {
            RemoveBinds();
            RemoveStaleShareXBinds();
            applied = shortcuts.ToArray();

            foreach (GlobalShortcut shortcut in applied)
            {
                if (!TryGetKey(shortcut.Hotkey, out int mask, out string key, out string combination))
                {
                    continue;
                }

                string target = applicationId + ":" + shortcut.Id;
                bool ok = IsLuaConfiguration()
                    ? Eval($"hl.bind({Lua(combination)}, hl.dsp.global({Lua(target)}), {{ description = {Lua("ShareX: " + shortcut.Description)} }})")
                    : Run("hyprctl", ["keyword", "bind", $"{ClassicModifiers(mask)}, {key}, global, {target}"]) is string output && IsOk(output);

                if (ok)
                {
                    bound.Add((mask, key, combination));
                }
            }
        }
    }

    public void Clear()
    {
        lock (syncRoot)
        {
            RemoveBinds();
            applied = Array.Empty<GlobalShortcut>();
        }
    }

    public void Dispose()
    {
        listening.Cancel();
        Clear();
    }

    /// <summary>
    /// Removes binds an earlier ShareX made and could not remove (it was killed), so this instance can bind the same keys. Only
    /// binds recognised as ShareX's are touched: the user's own binds stay.
    /// </summary>
    private void RemoveStaleShareXBinds()
    {
        string? json = Run("hyprctl", ["binds", "-j"]);

        if (json == null)
        {
            return;
        }

        foreach (HyprlandBind bind in ParseBinds(json, applicationId).Where(x => x.IsShareX))
        {
            if (IsLuaConfiguration())
            {
                Eval($"hl.unbind({Lua(CombinationFor(bind.Mask, bind.Key))})");
            }
            else
            {
                Run("hyprctl", ["keyword", "unbind", $"{ClassicModifiers(bind.Mask)}, {bind.Key}"]);
            }
        }
    }

    /// <summary>The Lua combination text for a bind as hyprctl lists it, for example "CTRL + SHIFT + Print".</summary>
    internal static string CombinationFor(int mask, string key)
    {
        List<string> parts = new List<string>();
        if ((mask & SuperMask) != 0) parts.Add("SUPER");
        if ((mask & ControlMask) != 0) parts.Add("CTRL");
        if ((mask & AltMask) != 0) parts.Add("ALT");
        if ((mask & ShiftMask) != 0) parts.Add("SHIFT");
        parts.Add(key);
        return string.Join(" + ", parts);
    }

    private void RemoveBinds()
    {
        foreach ((int mask, string key, string combination) in bound)
        {
            if (IsLuaConfiguration())
            {
                Eval($"hl.unbind({Lua(combination)})");
            }
            else
            {
                Run("hyprctl", ["keyword", "unbind", $"{ClassicModifiers(mask)}, {key}"]);
            }
        }

        bound.Clear();
    }

    /// <summary>A configuration reload drops runtime binds; add ShareX's again.</summary>
    private async Task ListenForReloadsAsync(CancellationToken cancellationToken)
    {
        string? signature = Environment.GetEnvironmentVariable("HYPRLAND_INSTANCE_SIGNATURE");
        string? runtime = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");

        if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(runtime))
        {
            return;
        }

        string path = Path.Combine(runtime, "hypr", signature, ".socket2.sock");

        try
        {
            using Socket socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
            await socket.ConnectAsync(new UnixDomainSocketEndPoint(path), cancellationToken).ConfigureAwait(false);
            using StreamReader reader = new StreamReader(new NetworkStream(socket, ownsSocket: false), Encoding.UTF8);

            while (!cancellationToken.IsCancellationRequested && await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) is string line)
            {
                if (line.StartsWith("configreloaded>>", StringComparison.Ordinal))
                {
                    lock (syncRoot)
                    {
                        bound.Clear();
                        Apply(applied);
                    }
                }
            }
        }
        catch (Exception e) when (e is IOException or SocketException or OperationCanceledException or ObjectDisposedException)
        {
        }
    }

    private bool IsLuaConfiguration()
    {
        // Hyprland 0.56+ with a Lua configuration evaluates Lua; classic configurations take keywords instead.
        luaConfiguration ??= Eval("return true");
        return luaConfiguration.Value;
    }

    private bool Eval(string lua) => Run("hyprctl", ["eval", lua]) is string output && IsOk(output);

    private static bool IsOk(string output) => output.Trim().Equals("ok", StringComparison.OrdinalIgnoreCase);

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

    /// <summary>Hyprland's modifier mask, key name and "CTRL + SHIFT + Print" form of a hotkey.</summary>
    internal static bool TryGetKey(PlatformHotkey hotkey, out int mask, out string key, out string combination)
    {
        mask = 0;
        combination = "";

        if (!XKeyMap.TryGetKeysym(hotkey.KeyCode, out _, out key))
        {
            return false;
        }

        List<string> parts = new List<string>(5);
        if (hotkey.Modifiers.HasFlag(HotkeyModifiers.Super)) { parts.Add("SUPER"); mask |= SuperMask; }
        if (hotkey.Modifiers.HasFlag(HotkeyModifiers.Control)) { parts.Add("CTRL"); mask |= ControlMask; }
        if (hotkey.Modifiers.HasFlag(HotkeyModifiers.Alt)) { parts.Add("ALT"); mask |= AltMask; }
        if (hotkey.Modifiers.HasFlag(HotkeyModifiers.Shift)) { parts.Add("SHIFT"); mask |= ShiftMask; }
        parts.Add(key);
        combination = string.Join(" + ", parts);
        return true;
    }

    internal static string ClassicModifiers(int mask)
    {
        List<string> parts = new List<string>(4);
        if ((mask & SuperMask) != 0) parts.Add("SUPER");
        if ((mask & ControlMask) != 0) parts.Add("CTRL");
        if ((mask & AltMask) != 0) parts.Add("ALT");
        if ((mask & ShiftMask) != 0) parts.Add("SHIFT");
        return string.Join(" ", parts);
    }

    /// <summary>The key binds in the main submap, from hyprctl binds -j.</summary>
    /// <summary>A bind in the main submap. <see cref="IsShareX"/> marks binds ShareX made: described "ShareX: …" (Lua configurations) or a global shortcut of its application id (classic configurations).</summary>
    internal readonly record struct HyprlandBind(int Mask, string Key, bool IsShareX);

    internal static IReadOnlyList<HyprlandBind> ParseBinds(string json, string applicationId)
    {
        List<HyprlandBind> binds = new List<HyprlandBind>();

        try
        {
            using JsonDocument document = JsonDocument.Parse(json);

            foreach (JsonElement bind in document.RootElement.EnumerateArray())
            {
                if (bind.TryGetProperty("submap", out JsonElement submap) && submap.GetString() is { Length: > 0 })
                {
                    continue;
                }

                if (bind.TryGetProperty("mouse", out JsonElement mouse) && mouse.ValueKind == JsonValueKind.True)
                {
                    continue;
                }

                string? key = bind.TryGetProperty("key", out JsonElement keyElement) ? keyElement.GetString() : null;
                int mask = bind.TryGetProperty("modmask", out JsonElement maskElement) ? maskElement.GetInt32() : 0;
                string description = bind.TryGetProperty("description", out JsonElement d) ? d.GetString() ?? "" : "";
                string dispatcher = bind.TryGetProperty("dispatcher", out JsonElement di) ? di.GetString() ?? "" : "";
                string argument = bind.TryGetProperty("arg", out JsonElement a) ? a.GetString() ?? "" : "";
                bool isShareX = description.StartsWith("ShareX: ", StringComparison.Ordinal) ||
                    (dispatcher == "global" && argument.StartsWith(applicationId + ":", StringComparison.Ordinal));

                if (!string.IsNullOrEmpty(key))
                {
                    binds.Add(new HyprlandBind(mask, key, isShareX));
                }
            }
        }
        catch (JsonException)
        {
        }

        return binds;
    }

    internal static string Lua(string text) => "\"" + text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n") + "\"";
}
