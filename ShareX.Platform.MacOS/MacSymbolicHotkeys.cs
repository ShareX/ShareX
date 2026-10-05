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
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace ShareX.Platform.MacOS;

/// <summary>
/// macOS's own keyboard shortcuts (System Settings > Keyboard > Keyboard Shortcuts), which Carbon lets another application register
/// again without an error, so both would fire. The user's choices live in com.apple.symbolichotkeys; shortcuts the user never
/// changed are missing there, so the built-in screenshot shortcuts count as enabled unless the file says otherwise.
/// </summary>
internal static class MacSymbolicHotkeys
{
    // NSEvent modifier flags, as stored in the preferences.
    private const long NSShift = 0x20000, NSControl = 0x40000, NSOption = 0x80000, NSCommand = 0x100000;

    /// <summary>Screenshot shortcuts enabled by default: id, key code (kVK_ANSI_3 = 20, 4 = 21, 5 = 23), modifier flags.</summary>
    private static readonly (int Id, uint KeyCode, long Flags)[] Defaults =
    [
        (28, 20, NSCommand | NSShift),
        (29, 20, NSCommand | NSShift | NSControl),
        (30, 21, NSCommand | NSShift),
        (31, 21, NSCommand | NSShift | NSControl),
        (184, 23, NSCommand | NSShift)
    ];

    /// <summary>Reads the enabled shortcuts with `defaults export`; on failure only the defaults apply.</summary>
    public static IReadOnlySet<(uint KeyCode, uint CarbonModifiers)> Load(ICommandRunner runner)
    {
        string? plist = null;

        try
        {
            CommandResult result = runner.RunAsync("defaults", ["export", "com.apple.symbolichotkeys", "-"], timeout: TimeSpan.FromSeconds(5))
                .GetAwaiter().GetResult();
            plist = result.Success ? result.StandardOutputText : null;
        }
        catch (Exception e) when (e is InvalidOperationException or System.ComponentModel.Win32Exception or TimeoutException)
        {
        }

        return Parse(plist);
    }

    internal static IReadOnlySet<(uint KeyCode, uint CarbonModifiers)> Parse(string? plist)
    {
        Dictionary<int, (bool Enabled, uint KeyCode, long Flags)> shortcuts = Defaults.ToDictionary(d => d.Id, d => (true, d.KeyCode, d.Flags));

        if (!string.IsNullOrWhiteSpace(plist))
        {
            try
            {
                XElement? root = XDocument.Parse(plist).Root?.Element("dict");
                XElement? all = ValueOf(root, "AppleSymbolicHotKeys");

                foreach ((string key, XElement entry) in Pairs(all))
                {
                    if (!int.TryParse(key, NumberStyles.Integer, CultureInfo.InvariantCulture, out int id))
                    {
                        continue;
                    }

                    bool enabled = ValueOf(entry, "enabled")?.Name.LocalName is "true" or "integer" && ValueOf(entry, "enabled")?.Value is not "0";
                    long[] parameters = ValueOf(ValueOf(entry, "value"), "parameters")?.Elements("integer")
                        .Select(e => long.TryParse(e.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long v) ? v : -1).ToArray() ?? [];

                    // parameters: character, key code (65535 when unset), modifier flags.
                    if (parameters.Length == 3 && parameters[1] is >= 0 and < 65535)
                    {
                        shortcuts[id] = (enabled, (uint)parameters[1], parameters[2]);
                    }
                    else if (shortcuts.TryGetValue(id, out var known))
                    {
                        shortcuts[id] = (enabled, known.KeyCode, known.Flags);
                    }
                }
            }
            catch (System.Xml.XmlException)
            {
            }
        }

        return shortcuts.Values.Where(s => s.Enabled).Select(s => (s.KeyCode, ToCarbon(s.Flags))).ToHashSet();
    }

    private static uint ToCarbon(long flags)
    {
        uint result = 0;
        if ((flags & NSCommand) != 0) result |= Native.Carbon.cmdKey;
        if ((flags & NSShift) != 0) result |= Native.Carbon.shiftKey;
        if ((flags & NSOption) != 0) result |= Native.Carbon.optionKey;
        if ((flags & NSControl) != 0) result |= Native.Carbon.controlKey;
        return result;
    }

    private static XElement? ValueOf(XElement? dict, string key)
    {
        if (dict == null) return null;
        XElement? keyElement = dict.Elements("key").FirstOrDefault(k => k.Value == key);
        return keyElement?.ElementsAfterSelf().FirstOrDefault();
    }

    private static IEnumerable<(string Key, XElement Value)> Pairs(XElement? dict)
    {
        if (dict == null) yield break;
        foreach (XElement key in dict.Elements("key"))
        {
            if (key.ElementsAfterSelf().FirstOrDefault() is XElement value && value.Name.LocalName == "dict")
            {
                yield return (key.Value, value);
            }
        }
    }
}
