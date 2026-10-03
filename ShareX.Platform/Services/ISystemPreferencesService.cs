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

namespace ShareX.Platform;

/// <summary>User preferences the operating system owns, so ShareX's windows behave like every other window.</summary>
public interface ISystemPreferencesService
{
    /// <summary>Lines to scroll for one mouse wheel notch. Windows lets the user change it; elsewhere it is 3.</summary>
    int WheelScrollLines { get; }

    /// <summary>
    /// Whether the system surfaces that hold tray icons (the Windows task bar) use the light theme, so tray glyphs can be drawn in a
    /// contrasting colour. Null where the platform does not say; callers then follow ShareX's own theme.
    /// </summary>
    bool? SystemUsesLightTheme { get; }

    /// <summary>The size of small icons such as tray icons, in pixels. Windows scales it with the display; elsewhere it is 16.</summary>
    int SmallIconSize { get; }

    /// <summary>
    /// An administrator policy ShareX honours, such as "DisableUpdateCheck", "DisableUpload", "DisableLogging" or "PersonalPath",
    /// or null when it is not set. The machine wide value wins over the user's. Windows reads HKLM then HKCU\SOFTWARE\ShareX;
    /// Linux and macOS read a JSON object from the system policy file, then the user's.
    /// </summary>
    object? GetPolicy(string name);
}

/// <summary>The defaults used on platforms that do not expose these preferences, with policies from JSON files.</summary>
public sealed class DefaultSystemPreferencesService : ISystemPreferencesService
{
    private readonly string[] policyFiles;

    /// <param name="policyFiles">JSON objects of policy names and values, most important first (system, then user).</param>
    public DefaultSystemPreferencesService(params string[] policyFiles)
    {
        this.policyFiles = policyFiles;
    }

    public const int DefaultWheelScrollLines = 3;

    public int WheelScrollLines => DefaultWheelScrollLines;

    public const int DefaultSmallIconSize = 16;

    public bool? SystemUsesLightTheme => null;

    public int SmallIconSize => DefaultSmallIconSize;

    public object? GetPolicy(string name)
    {
        foreach (string file in policyFiles)
        {
            try
            {
                if (!System.IO.File.Exists(file))
                {
                    continue;
                }

                using System.Text.Json.JsonDocument document = System.Text.Json.JsonDocument.Parse(System.IO.File.ReadAllText(file));

                if (document.RootElement.ValueKind == System.Text.Json.JsonValueKind.Object &&
                    document.RootElement.TryGetProperty(name, out System.Text.Json.JsonElement value))
                {
                    return value.ValueKind switch
                    {
                        System.Text.Json.JsonValueKind.True => true,
                        System.Text.Json.JsonValueKind.False => false,
                        System.Text.Json.JsonValueKind.Number => value.GetInt64(),
                        System.Text.Json.JsonValueKind.String => value.GetString(),
                        _ => null
                    };
                }
            }
            catch (Exception e) when (e is System.IO.IOException or UnauthorizedAccessException or System.Text.Json.JsonException)
            {
                // An unreadable policy file must not stop ShareX; the next file or the default applies.
            }
        }

        return null;
    }
}
