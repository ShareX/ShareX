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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX.Destinations;

/// <summary>A named set of file extensions that a route can target, for example "Videos" or a custom "PNG" type.</summary>
public sealed class FileTypeDefinition
{
    public FileTypeDefinition()
    {
    }

    public FileTypeDefinition(string id, string name, IEnumerable<string> extensions)
    {
        Id = id;
        Name = name;
        Extensions = NormalizeExtensions(extensions);
    }

    /// <summary>Premade types use fixed ids such as "images". Custom types use a GUID.</summary>
    public string Id { get; set; } = "";

    public string Name { get; set; } = "";

    /// <summary>Lower case extensions without the leading dot.</summary>
    public List<string> Extensions { get; set; } = new List<string>();

    [JsonIgnore]
    public bool IsPremade => PremadeFileTypes.IsPremade(Id);

    [JsonIgnore]
    public bool IsOtherFiles => Id == PremadeFileTypes.OtherFiles;

    public bool Contains(string? extension)
    {
        string? normalized = NormalizeExtension(extension);
        return normalized != null && Extensions.Contains(normalized, StringComparer.Ordinal);
    }

    public FileTypeDefinition Clone() => new FileTypeDefinition { Id = Id, Name = Name, Extensions = new List<string>(Extensions) };

    public override string ToString() => Name;

    /// <summary>Returns "png" for ".PNG", " png " or "png". Returns null for an empty value.</summary>
    public static string? NormalizeExtension(string? extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            return null;
        }

        string value = extension.Trim().TrimStart('*').TrimStart('.').Trim().ToLowerInvariant();
        return value.Length > 0 ? value : null;
    }

    /// <summary>Accepts a list or a single comma, semicolon or space separated string such as "jpg, jpeg".</summary>
    public static List<string> NormalizeExtensions(IEnumerable<string>? extensions) =>
        (extensions ?? Enumerable.Empty<string>())
            .SelectMany(value => (value ?? "").Split(new[] { ',', ';', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            .Select(NormalizeExtension)
            .OfType<string>()
            .Distinct(StringComparer.Ordinal)
            .ToList();
}

/// <summary>The premade file types. Other files is the catch all and is always last.</summary>
public static class PremadeFileTypes
{
    public const string Images = "images";
    public const string Videos = "videos";
    public const string Audio = "audio";
    public const string Text = "text";
    public const string Documents = "documents";
    public const string Archives = "archives";
    public const string OtherFiles = "other";

    /// <summary>Used in <see cref="DestinationInstance.AcceptedFileTypes"/> for instances that accept every file.</summary>
    public const string AllFiles = "*";

    // Images, Text and Videos start from the lists in FileHelpers. ShareX routes images and text by the task data type, which honours the
    // image and text extensions in the task's advanced settings, so these lists only decide matching when no data type is known.
    private static readonly FileTypeDefinition[] all =
    [
        new FileTypeDefinition(Images, "Images", ["jpg", "jpeg", "png", "gif", "bmp", "ico", "tif", "tiff", "webp", "avif", "heic", "heif", "jxl", "svg"]),
        new FileTypeDefinition(Videos, "Videos", ["mp4", "webm", "mkv", "avi", "vob", "ogv", "ogg", "mov", "qt", "wmv", "m4p", "m4v", "mpg", "mp2", "mpeg", "mpe", "mpv",
            "m2v", "flv", "f4v", "3gp", "ts", "m2ts"]),
        new FileTypeDefinition(Audio, "Audio", ["mp3", "wav", "flac", "aac", "m4a", "oga", "opus", "wma", "aiff", "aif", "alac", "mid", "midi"]),
        new FileTypeDefinition(Text, "Text", ["txt", "log", "nfo", "c", "cpp", "cc", "cxx", "h", "hpp", "hxx", "cs", "vb", "html", "htm", "xhtml", "xht", "xml",
            "css", "js", "php", "bat", "java", "lua", "py", "pl", "cfg", "ini", "dart", "go", "gohtml", "md", "json", "yaml", "yml", "csv", "sh", "rs"]),
        new FileTypeDefinition(Documents, "Documents", ["pdf", "doc", "docx", "odt", "rtf", "xls", "xlsx", "ods", "ppt", "pptx", "odp", "epub"]),
        new FileTypeDefinition(Archives, "Archives", ["zip", "7z", "rar", "tar", "gz", "tgz", "bz2", "xz", "zst", "iso"]),
        new FileTypeDefinition(OtherFiles, "Other files", [])
    ];

    /// <summary>Premade types in matching order, Other files last.</summary>
    public static IReadOnlyList<FileTypeDefinition> All => all;

    public static bool IsPremade(string? id) => id != null && all.Any(type => type.Id == id);

    public static FileTypeDefinition? Get(string? id) => all.FirstOrDefault(type => type.Id == id);
}
