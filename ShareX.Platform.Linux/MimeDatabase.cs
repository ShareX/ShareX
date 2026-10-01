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
using System.Collections.Generic;
using System.IO;

namespace ShareX.Platform.Linux;

/// <summary>
/// Extension to MIME type lookups from the freedesktop.org shared-mime-info database (globs2) and the classic mime.types files.
/// </summary>
internal sealed class MimeDatabase
{
    public static MimeDatabase Default { get; } = new MimeDatabase(GetDefaultFiles());

    private readonly Lazy<Dictionary<string, string>> types;

    public MimeDatabase(IReadOnlyList<string> files)
    {
        types = new Lazy<Dictionary<string, string>>(() => Load(files));
    }

    public string? GetMimeType(string extension)
    {
        string key = extension.TrimStart('.').ToLowerInvariant();
        return key.Length > 0 && types.Value.TryGetValue(key, out string? type) ? type : null;
    }

    private static IReadOnlyList<string> GetDefaultFiles()
    {
        List<string> files = new List<string>();
        string? dataHome = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
        string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        files.Add(Path.Combine(string.IsNullOrEmpty(dataHome) ? Path.Combine(home, ".local", "share") : dataHome, "mime", "globs2"));

        string dataDirs = Environment.GetEnvironmentVariable("XDG_DATA_DIRS") is { Length: > 0 } dirs ? dirs : "/usr/local/share:/usr/share";

        foreach (string dir in dataDirs.Split(':', StringSplitOptions.RemoveEmptyEntries))
        {
            files.Add(Path.Combine(dir, "mime", "globs2"));
        }

        files.Add("/etc/mime.types");
        return files;
    }

    internal static Dictionary<string, string> Load(IReadOnlyList<string> files)
    {
        // The first file that defines an extension wins, so user and local databases override the system ones.
        Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (string file in files)
        {
            try
            {
                if (!File.Exists(file))
                {
                    continue;
                }

                foreach (string line in File.ReadLines(file))
                {
                    if (Path.GetFileName(file) == "globs2")
                    {
                        ParseGlobs2Line(line, result);
                    }
                    else
                    {
                        ParseMimeTypesLine(line, result);
                    }
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
            }
        }

        return result;
    }

    /// <summary>"weight:type:glob[:flags]", for example "50:image/png:*.png".</summary>
    internal static void ParseGlobs2Line(string line, Dictionary<string, string> result)
    {
        if (line.Length == 0 || line[0] == '#')
        {
            return;
        }

        string[] parts = line.Split(':');

        if (parts.Length >= 3 && parts[2].StartsWith("*.", StringComparison.Ordinal) && parts[2].IndexOfAny(['*', '?', '['], 2) < 0)
        {
            result.TryAdd(parts[2][2..].ToLowerInvariant(), parts[1]);
        }
    }

    /// <summary>"type ext ext ...", for example "image/png png".</summary>
    internal static void ParseMimeTypesLine(string line, Dictionary<string, string> result)
    {
        if (line.Length == 0 || line[0] == '#')
        {
            return;
        }

        string[] parts = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < parts.Length; i++)
        {
            result.TryAdd(parts[i].ToLowerInvariant(), parts[0]);
        }
    }
}
