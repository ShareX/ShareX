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

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Linux.Desktop;

/// <summary>
/// Places one of ShareX's full-screen XWayland overlays on Hyprland. When a multi-monitor region capture overlay maps,
/// Hyprland clamps its initial position to the active output instead of leaving it at the virtual desktop's top-left
/// (issue #8867), and static move rules run before clamping. Watching Hyprland's event socket for <c>openwindow</c> and
/// issuing an absolute move afterwards puts only that overlay back at <paramref name="target"/>.
/// </summary>
internal static class HyprlandWindowPlacement
{
    private static readonly TimeSpan WatchTimeout = TimeSpan.FromSeconds(3);
    private static readonly TimeSpan SettleDelay = TimeSpan.FromMilliseconds(50);

    internal readonly record struct HyprlandClient(long Address, PlatformPoint Position, bool Floating, bool XWayland);

    /// <summary>Starts placing the overlay and returns at once; failures are only logged, since the window stays usable.</summary>
    public static void Place(CompositorCommands commands, HyprlandDispatcher dispatcher, int processId, string? title, PlatformRectangle target)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await PlaceAsync(commands, dispatcher, processId, title, new PlatformPoint(target.X, target.Y)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Hyprland window placement failed: {e.Message}");
            }
        });
    }

    /// <summary>
    /// Moves an already-mapped matching overlay window to <paramref name="target"/>. Returns whether the window was found
    /// (whether or not it needed to move).
    /// </summary>
    internal static bool TryPlaceExisting(CompositorCommands commands, HyprlandDispatcher dispatcher, int processId, string? title,
        PlatformPoint target, long? expectedAddress = null)
    {
        HyprlandClient? client = commands.ReadJson("hyprctl", ["clients", "-j"],
            root => FindMatchingClient(root, processId, title, expectedAddress));

        if (client == null)
        {
            return false;
        }

        if (client.Value.Position != target)
        {
            dispatcher.MoveWindow(client.Value.Address, target);
        }

        return true;
    }

    internal static HyprlandClient? FindMatchingClient(JsonElement root, int processId, string? title, long? expectedAddress = null)
    {
        if (root.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        foreach (JsonElement item in root.EnumerateArray())
        {
            if (!item.TryGetProperty("pid", out JsonElement pidElement) || pidElement.ValueKind != JsonValueKind.Number ||
                pidElement.GetInt32() != processId)
            {
                continue;
            }

            if (item.TryGetProperty("mapped", out JsonElement mapped) && mapped.ValueKind == JsonValueKind.False)
            {
                continue;
            }

            bool xwayland = item.TryGetProperty("xwayland", out JsonElement xw) && xw.ValueKind == JsonValueKind.True;

            if (!xwayland)
            {
                continue;
            }

            string? addressText = item.TryGetProperty("address", out JsonElement addr) ? addr.GetString() : null;

            if (string.IsNullOrEmpty(addressText))
            {
                continue;
            }

            ReadOnlySpan<char> hex = addressText.AsSpan();

            if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                hex = hex[2..];
            }

            if (hex.Length == 0 || !long.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long address))
            {
                continue;
            }

            if (expectedAddress.HasValue && address != expectedAddress.Value)
            {
                continue;
            }

            string clientTitle = item.TryGetProperty("title", out JsonElement titleElement) ? titleElement.GetString() ?? "" : "";

            if (title != null && !string.Equals(clientTitle, title, StringComparison.Ordinal))
            {
                continue;
            }

            if (!item.TryGetProperty("at", out JsonElement at) || at.ValueKind != JsonValueKind.Array || at.GetArrayLength() < 2)
            {
                continue;
            }

            bool floating = item.TryGetProperty("floating", out JsonElement fl) && fl.ValueKind == JsonValueKind.True;
            PlatformPoint position = new PlatformPoint(at[0].GetInt32(), at[1].GetInt32());
            return new HyprlandClient(address, position, floating, xwayland);
        }

        return null;
    }

    /// <summary>Parses the window address from a Hyprland socket2 <c>openwindow&gt;&gt;ADDRESS,WORKSPACE,CLASS,TITLE</c> line.</summary>
    internal static bool TryParseOpenWindowAddress(string eventLine, out long address)
    {
        address = 0;
        const string prefix = "openwindow>>";

        if (!eventLine.StartsWith(prefix, StringComparison.Ordinal))
        {
            return false;
        }

        ReadOnlySpan<char> payload = eventLine.AsSpan(prefix.Length);
        int comma = payload.IndexOf(',');
        ReadOnlySpan<char> hex = comma >= 0 ? payload[..comma] : payload;

        if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            hex = hex[2..];
        }

        return hex.Length > 0 && long.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out address);
    }

    private static async Task PlaceAsync(CompositorCommands commands, HyprlandDispatcher dispatcher, int processId, string? title, PlatformPoint target)
    {
        string? signature = Environment.GetEnvironmentVariable("HYPRLAND_INSTANCE_SIGNATURE");
        string? runtime = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");
        string? socketPath = !string.IsNullOrEmpty(signature) && !string.IsNullOrEmpty(runtime)
            ? Path.Combine(runtime, "hypr", signature, ".socket2.sock")
            : null;

        using CancellationTokenSource timeout = new CancellationTokenSource(WatchTimeout);
        Socket? socket = null;

        try
        {
            // Connect before querying clients -j so an openwindow event cannot slip through between the query and listening.
            if (socketPath != null)
            {
                try
                {
                    socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
                    await socket.ConnectAsync(new UnixDomainSocketEndPoint(socketPath), timeout.Token).ConfigureAwait(false);
                }
                catch (Exception e) when (e is IOException or SocketException or OperationCanceledException)
                {
                    socket?.Dispose();
                    socket = null;
                }
            }

            if (TryPlaceExisting(commands, dispatcher, processId, title, target))
            {
                // Hyprland can clamp a newly-mapped XWayland window once more on its initial configure; check again briefly.
                await Task.Delay(SettleDelay, timeout.Token).ConfigureAwait(false);
                TryPlaceExisting(commands, dispatcher, processId, title, target);
                return;
            }

            if (socket == null)
            {
                return;
            }

            using StreamReader reader = new StreamReader(new NetworkStream(socket, ownsSocket: false), Encoding.UTF8);

            while (!timeout.IsCancellationRequested && await reader.ReadLineAsync(timeout.Token).ConfigureAwait(false) is string line)
            {
                if (!TryParseOpenWindowAddress(line, out long openedAddress))
                {
                    continue;
                }

                if (TryPlaceExisting(commands, dispatcher, processId, title, target, openedAddress))
                {
                    await Task.Delay(SettleDelay, timeout.Token).ConfigureAwait(false);
                    TryPlaceExisting(commands, dispatcher, processId, title, target, openedAddress);
                    return;
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            socket?.Dispose();
        }
    }
}
