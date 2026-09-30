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

using ShareX.Platform;
using System.Text;

namespace ShareX.Desktop.Diagnostics;

/// <summary>
/// Compositors that have no global shortcut API for ordinary applications (Hyprland, Sway) start commands from key bindings,
/// so ShareX ships the commands and prints the lines to paste into the compositor's configuration.
/// </summary>
public static class HotkeyBindings
{
    public static string Report(PlatformInfo info, string executable)
    {
        StringBuilder text = new StringBuilder();

        switch (info.DesktopEnvironment)
        {
            case DesktopEnvironment.Hyprland:
                text.AppendLine("# Add to ~/.config/hypr/hyprland.conf (or a file it sources), then run: hyprctl reload");
                text.AppendLine($"bind = , Print, exec, {executable} capture region");
                text.AppendLine($"bind = SHIFT, Print, exec, {executable} capture fullscreen");
                text.AppendLine($"bind = CTRL, Print, exec, {executable} capture region --upload");
                break;
            case DesktopEnvironment.Sway:
                text.AppendLine("# Add to ~/.config/sway/config, then run: swaymsg reload");
                text.AppendLine($"bindsym Print exec {executable} capture region");
                text.AppendLine($"bindsym Shift+Print exec {executable} capture fullscreen");
                text.AppendLine($"bindsym Ctrl+Print exec {executable} capture region --upload");
                break;
            default:
                text.AppendLine("Create custom keyboard shortcuts in your desktop's settings that run:");
                text.AppendLine($"  {executable} capture region");
                text.AppendLine($"  {executable} capture fullscreen");
                text.AppendLine($"  {executable} capture region --upload");
                break;
        }

        return text.ToString();
    }
}
