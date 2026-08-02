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

using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ShareX.ImageEditor.Localization;

namespace ShareX.ImageEditor.Presentation.Emoji;

public sealed class EmojiCatalogEntry
{
    public string Name { get; init; } = string.Empty;

    public string Group { get; init; } = string.Empty;

    public string Unicode { get; init; } = string.Empty;

    public string[] Keywords { get; init; } = [];

    [JsonIgnore]
    public string Glyph => EmojiCatalogService.ToGlyph(Unicode);

    [JsonIgnore]
    public string DisplayName => EmojiCatalogLocalization.GetName(Unicode, Name);

    [JsonIgnore]
    public string DisplayGroup => EmojiCatalogLocalization.GetGroupName(Group);

    [JsonIgnore]
    private string SearchIndex => _searchIndex ??= BuildSearchIndex();

    private string? _searchIndex;

    public int GetSearchScore(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return int.MaxValue;
        }

        string search = searchText.Trim().ToLowerInvariant();
        string normalizedName = DisplayName.ToLowerInvariant();

        if (normalizedName.Equals(search, StringComparison.Ordinal))
        {
            return 0;
        }

        if (normalizedName.StartsWith(search, StringComparison.Ordinal))
        {
            return 1;
        }

        if (Keywords.Any(keyword => keyword.StartsWith(search, StringComparison.OrdinalIgnoreCase)))
        {
            return 2;
        }

        if (SearchIndex.Contains(search, StringComparison.Ordinal))
        {
            return 3;
        }

        return int.MaxValue;
    }

    private string BuildSearchIndex()
    {
        var builder = new StringBuilder(Name.Length + Group.Length + DisplayName.Length + DisplayGroup.Length + 32);
        builder.Append(DisplayName);
        builder.Append(' ');
        builder.Append(DisplayGroup);
        builder.Append(' ');
        builder.Append(Name);
        builder.Append(' ');
        builder.Append(Group);

        foreach (string keyword in Keywords)
        {
            builder.Append(' ');
            builder.Append(keyword);
        }

        return builder.ToString().ToLowerInvariant();
    }
}

public sealed record EmojiCatalogGroupOption(string Name, string DisplayName)
{
    public override string ToString() => DisplayName;
}

public sealed record EmojiSelectionRequest(string UnicodeSequence, string DisplayName);

public static class EmojiCatalogService
{
    private const string CatalogResourceName = "ShareX.ImageEditor.Assets.emoji-catalog.json";

    private static readonly string[] OrderedGroups =
    [
        "Smileys & Emotion",
        "People & Body",
        "Animals & Nature",
        "Food & Drink",
        "Travel & Places",
        "Activities",
        "Objects",
        "Symbols",
        "Flags"
    ];

    private static readonly Lazy<IReadOnlyList<EmojiCatalogEntry>> Catalog = new(LoadCatalog);

    public static IReadOnlyList<EmojiCatalogEntry> GetCatalog() => Catalog.Value;

    public static IReadOnlyList<string> GetGroups()
    {
        var catalog = GetCatalog();
        var presentGroups = new HashSet<string>(catalog.Select(entry => entry.Group), StringComparer.Ordinal);
        var result = new List<string>(OrderedGroups.Length);

        foreach (string group in OrderedGroups)
        {
            if (presentGroups.Contains(group))
            {
                result.Add(group);
            }
        }

        foreach (string group in presentGroups.OrderBy(static group => group, StringComparer.Ordinal))
        {
            if (!result.Contains(group, StringComparer.Ordinal))
            {
                result.Add(group);
            }
        }

        return result;
    }

    public static string ToGlyph(string unicodeSequence)
    {
        if (string.IsNullOrWhiteSpace(unicodeSequence))
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        string[] parts = unicodeSequence.Split([' ', '-'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (string part in parts)
        {
            if (!int.TryParse(part, System.Globalization.NumberStyles.HexNumber, null, out int codePoint))
            {
                continue;
            }

            builder.Append(char.ConvertFromUtf32(codePoint));
        }

        return builder.ToString();
    }

    private static IReadOnlyList<EmojiCatalogEntry> LoadCatalog()
    {
        Assembly assembly = typeof(EmojiCatalogService).Assembly;
        using Stream? stream = assembly.GetManifestResourceStream(CatalogResourceName);
        if (stream == null)
        {
            throw new InvalidOperationException($"Emoji catalog resource '{CatalogResourceName}' was not found.");
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        List<EmojiCatalogEntry>? entries = JsonSerializer.Deserialize<List<EmojiCatalogEntry>>(stream, options);
        if (entries == null || entries.Count == 0)
        {
            throw new InvalidOperationException("Emoji catalog is empty.");
        }

        return entries;
    }
}

public static class EmojiCatalogLocalization
{
    public static string GetName(string unicodeSequence, string fallbackName)
    {
        string key = $"EmojiCatalog_Name_{NormalizeUnicode(unicodeSequence)}";
        return Strings.ResourceManager.GetString(key, Strings.Culture) ?? fallbackName;
    }

    public static string GetGroupName(string groupName)
    {
        string? key = groupName switch
        {
            "Smileys & Emotion" => "EmojiCatalog_Group_SmileysEmotion",
            "People & Body" => "EmojiCatalog_Group_PeopleBody",
            "Animals & Nature" => "EmojiCatalog_Group_AnimalsNature",
            "Food & Drink" => "EmojiCatalog_Group_FoodDrink",
            "Travel & Places" => "EmojiCatalog_Group_TravelPlaces",
            "Activities" => "EmojiCatalog_Group_Activities",
            "Objects" => "EmojiCatalog_Group_Objects",
            "Symbols" => "EmojiCatalog_Group_Symbols",
            "Flags" => "EmojiCatalog_Group_Flags",
            _ => null
        };

        return key == null ? groupName : Strings.ResourceManager.GetString(key, Strings.Culture) ?? groupName;
    }

    private static string NormalizeUnicode(string unicodeSequence)
    {
        return unicodeSequence.Replace(' ', '_').Replace('-', '_').ToUpperInvariant();
    }
}
