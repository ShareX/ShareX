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

using System.Collections.Generic;
using System.Linq;

namespace ShareX.Destinations;

/// <summary>What is being uploaded.</summary>
public sealed class RouteRequest
{
    /// <summary>The file extension, for example "png" for a screenshot or "txt" for clipboard text.</summary>
    public string? Extension { get; init; }

    /// <summary>
    /// A premade type the caller already knows, for example Images for a screenshot. It is used for rule 2 instead of an extension lookup,
    /// which keeps ShareX's configurable image and text extensions in charge.
    /// </summary>
    public string? KnownFileType { get; init; }

    /// <summary>Premade types the caller has ruled out, for example Images and Text for a file ShareX classified as a plain file.</summary>
    public IReadOnlyCollection<string>? ExcludedFileTypes { get; init; }

    public static RouteRequest ForFileName(string? fileName) => new RouteRequest { Extension = GetExtension(fileName) };

    public static string? GetExtension(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return null;
        }

        int dot = fileName.LastIndexOf('.');
        int separator = fileName.LastIndexOfAny(['/', '\\']);
        return dot > separator && dot < fileName.Length - 1 ? FileTypeDefinition.NormalizeExtension(fileName[(dot + 1)..]) : null;
    }
}

/// <summary>The route chosen for an upload.</summary>
/// <param name="IsOverride">True when the route came from the task or hotkey rather than the default table.</param>
public sealed record RouteMatch(FileTypeDefinition FileType, DestinationInstance Instance, DestinationRoute Route, bool IsOverride)
{
    /// <summary>For example "PNG → Imgur".</summary>
    public string Description => $"{FileType.Name} → {Instance.Name}";
}

/// <summary>Applies the matching rules: an exact custom type wins, then the premade type, then Other files.</summary>
public static class RouteResolver
{
    public static RouteMatch? Resolve(DestinationRoutingConfig config, IEnumerable<DestinationRoute>? defaultRoutes, IEnumerable<DestinationRoute>? taskRoutes,
        RouteRequest request)
    {
        // Routes whose file type or instance no longer exists are skipped. A broken override falls back to the default route of its type,
        // a broken default route falls through to the next rule.
        bool IsValid(DestinationRoute route) => config.FindFileType(route.FileTypeId) != null && config.FindInstance(route.InstanceId) != null;

        List<(FileTypeDefinition FileType, DestinationInstance Instance, DestinationRoute Route, bool IsOverride)> routes =
            DestinationRoutes.Merge(defaultRoutes?.Where(IsValid), taskRoutes?.Where(IsValid))
            .Select(m => (FileType: config.FindFileType(m.Route.FileTypeId), Instance: config.FindInstance(m.Route.InstanceId), m.Route, m.IsOverride))
            .Where(m => m.FileType != null && m.Instance != null)
            .Select(m => (m.FileType!, m.Instance!, m.Route, m.IsOverride))
            .ToList();

        string? extension = FileTypeDefinition.NormalizeExtension(request.Extension);

        // 1. A custom file type that lists the exact extension.
        if (extension != null)
        {
            foreach (var route in routes.Where(r => !r.FileType.IsPremade))
            {
                if (route.FileType.Contains(extension))
                {
                    return new RouteMatch(route.FileType, route.Instance, route.Route, route.IsOverride);
                }
            }
        }

        // 2. The premade file type, either known by the caller or found by extension.
        string? premadeId = request.KnownFileType;

        if (premadeId == null && extension != null)
        {
            premadeId = PremadeFileTypes.All
                .Where(type => !type.IsOtherFiles && request.ExcludedFileTypes?.Contains(type.Id) != true)
                .FirstOrDefault(type => type.Contains(extension))?.Id;
        }

        if (premadeId != null)
        {
            foreach (var route in routes.Where(r => r.FileType.Id == premadeId))
            {
                return new RouteMatch(route.FileType, route.Instance, route.Route, route.IsOverride);
            }
        }

        // 3. Other files.
        foreach (var route in routes.Where(r => r.FileType.IsOtherFiles))
        {
            return new RouteMatch(route.FileType, route.Instance, route.Route, route.IsOverride);
        }

        return null;
    }
}
