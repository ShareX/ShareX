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

using ShareX.Platform.Linux.DBus;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace ShareX.Platform.Linux.Desktop;

/// <summary>
/// Places one of ShareX's own windows on KDE Plasma (Wayland) through a short-lived KWin script. KWin decides where an XWayland
/// window goes: it ignores the requested position of a window larger than one monitor and puts the region capture overlay on the
/// primary or active monitor instead, or anywhere when no monitor covers the desktop origin (issue #8867). A script may set
/// any window's frame geometry, so ShareX loads one through KWin's scripting D-Bus interface, lets it move the window, and unloads it.
/// </summary>
internal static class KWinWindowPlacement
{
    private const string KWinBusName = "org.kde.KWin";

    /// <summary>
    /// The script. <paramref name="target"/> and <paramref name="xDesktop"/> are XWayland (X11) coordinates; KWin places windows in
    /// its logical coordinates, which differ when XWayland applications scale themselves. Both cover the same monitors, so the script
    /// maps the target from the X11 desktop onto the union of KWin's outputs.
    /// </summary>
    internal static string BuildScript(int processId, string? caption, PlatformRectangle target, PlatformRectangle xDesktop)
    {
        string Rect(PlatformRectangle r) => string.Create(CultureInfo.InvariantCulture, $"{{x:{r.X},y:{r.Y},w:{r.Width},h:{r.Height}}}");

        StringBuilder script = new StringBuilder();
        script.Append("const pid=").Append(processId.ToString(CultureInfo.InvariantCulture)).Append(";\n");
        script.Append("const caption=").Append(caption == null ? "null" : JsonSerializer.Serialize(caption)).Append(";\n");
        script.Append("const target=").Append(Rect(target)).Append(";\n");
        script.Append("const desktop=").Append(Rect(xDesktop)).Append(";\n");
        script.Append("""
            function place(w) {
                if (!w || w.pid !== pid || (caption !== null && !String(w.caption).startsWith(caption))) return;
                let x1 = Infinity, y1 = Infinity, x2 = -Infinity, y2 = -Infinity;
                for (const s of workspace.screens) {
                    const g = s.geometry;
                    x1 = Math.min(x1, g.x); y1 = Math.min(y1, g.y);
                    x2 = Math.max(x2, g.x + g.width); y2 = Math.max(y2, g.y + g.height);
                }
                if (!isFinite(x1) || desktop.w <= 0 || desktop.h <= 0) return;
                const sx = (x2 - x1) / desktop.w, sy = (y2 - y1) / desktop.h;
                w.frameGeometry = {
                    x: Math.round(x1 + (target.x - desktop.x) * sx),
                    y: Math.round(y1 + (target.y - desktop.y) * sy),
                    width: Math.round(target.w * sx),
                    height: Math.round(target.h * sy)
                };
            }
            for (const w of workspace.windowList()) place(w);
            workspace.windowAdded.connect(place);

            """);
        return script.ToString();
    }

    /// <summary>Starts placing the window and returns at once; failures are only logged, since the window stays usable.</summary>
    public static void Place(int processId, string? caption, PlatformRectangle target, PlatformRectangle xDesktop)
    {
        string script = BuildScript(processId, caption, target, xDesktop);
        _ = Task.Run(async () =>
        {
            try
            {
                await RunScriptAsync(script).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"KWin window placement failed: {e.Message}");
            }
        });
    }

    private static async Task RunScriptAsync(string script)
    {
        string pluginName = "sharex-place-" + Guid.NewGuid().ToString("N");
        string path = Path.Combine(Path.GetTempPath(), pluginName + ".js");
        await File.WriteAllTextAsync(path, script).ConfigureAwait(false);

        try
        {
            DBusConnection bus = await DBusSession.GetConnectionAsync().ConfigureAwait(false);
            MessageBuffer load = DBusSession.CreateMethodCall(bus, KWinBusName, "/Scripting", "org.kde.kwin.Scripting", "loadScript", "ss",
                (ref MessageWriter writer) =>
                {
                    writer.WriteString(path);
                    writer.WriteString(pluginName);
                });
            int id = await bus.CallMethodAsync(load, static (Message message, object? state) => message.GetBodyReader().ReadInt32(), null)
                .ConfigureAwait(false);

            if (id < 0)
            {
                throw new InvalidOperationException("KWin did not load the placement script.");
            }

            try
            {
                MessageBuffer run = DBusSession.CreateMethodCall(bus, KWinBusName, "/Scripting/Script" + id.ToString(CultureInfo.InvariantCulture),
                    "org.kde.kwin.Script", "run", null, null);
                await bus.CallMethodAsync(run).ConfigureAwait(false);

                // KWin adds " <2>" to repeated captions, hence startsWith. XWayland windows can reach KWin a moment after Avalonia reports them open; the script also places them as they arrive.
                await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
            }
            finally
            {
                MessageBuffer unload = DBusSession.CreateMethodCall(bus, KWinBusName, "/Scripting", "org.kde.kwin.Scripting", "unloadScript", "s",
                    (ref MessageWriter writer) => writer.WriteString(pluginName));
                await bus.CallMethodAsync(unload).ConfigureAwait(false);
            }
        }
        finally
        {
            File.Delete(path);
        }
    }
}
