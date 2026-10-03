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

using System.Globalization;

namespace ShareX.Platform.Linux.Desktop;

/// <summary>
/// Hyprland dispatchers in the form the running configuration accepts: Hyprland 0.56+ with a Lua configuration takes
/// <c>hl.dsp</c> expressions, classic configurations take "dispatch name arguments".
/// </summary>
internal sealed class HyprlandDispatcher(CompositorCommands commands)
{
    private bool? lua;

    /// <summary>A Lua configuration evaluates Lua; a classic one does not.</summary>
    public bool IsLuaConfiguration => lua ??= commands.ReadText("hyprctl", ["eval", "return true"])?.Trim() == "ok";

    public bool Focus(long windowHandle) => Dispatch(
        $"hl.dsp.focus({{ window = {Lua(Address(windowHandle))} }})", ["focuswindow", Address(windowHandle)]);

    public bool MoveCursor(PlatformPoint position) => Dispatch(
        string.Create(CultureInfo.InvariantCulture, $"hl.dsp.cursor.move({{ x = {position.X}, y = {position.Y} }})"),
        ["movecursor", position.X.ToString(CultureInfo.InvariantCulture), position.Y.ToString(CultureInfo.InvariantCulture)]);

    /// <summary>Toggles "pinned" (shown on every workspace, above tiled windows) for a floating window.</summary>
    public bool TogglePin(long windowHandle) => Dispatch(
        $"hl.dsp.window.pin({{ window = {Lua(Address(windowHandle))} }})", ["pin", Address(windowHandle)]);

    /// <summary>Toggles fullscreen, or maximised (keeping bars and gaps) when <paramref name="maximized"/> is true.</summary>
    public bool ToggleFullscreen(long windowHandle, bool maximized)
    {
        if (IsLuaConfiguration)
        {
            return Ok(commands.ReadText("hyprctl", ["dispatch",
                $"hl.dsp.window.fullscreen({{ window = {Lua(Address(windowHandle))}, mode = {Lua(maximized ? "maximized" : "fullscreen")} }})"]));
        }

        return commands.Run("hyprctl", ["--batch", $"dispatch focuswindow {Address(windowHandle)}; dispatch fullscreen {(maximized ? 1 : 0)}"]);
    }

    private bool Dispatch(string luaExpression, string[] classic) =>
        IsLuaConfiguration ? Ok(commands.ReadText("hyprctl", ["dispatch", luaExpression])) : Ok(commands.ReadText("hyprctl", ["dispatch", .. classic]));

    private static bool Ok(string? output) => output?.Trim() == "ok";

    internal static string Address(long handle) => "address:0x" + handle.ToString("x", CultureInfo.InvariantCulture);

    private static string Lua(string text) => HyprlandShortcutKeyBinder.Lua(text);
}
