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
using System.Linq;

namespace ShareX.Destinations;

/// <summary>Editing helpers for a route table. A default table always keeps its Other files route.</summary>
public static class DestinationRoutes
{
    public static DestinationRoute? Find(IEnumerable<DestinationRoute>? routes, string fileTypeId) =>
        routes?.FirstOrDefault(route => route.FileTypeId == fileTypeId);

    /// <summary>Adds or replaces the route for the file type.</summary>
    public static DestinationRoute Set(List<DestinationRoute> routes, string fileTypeId, Guid instanceId)
    {
        DestinationRoute? route = Find(routes, fileTypeId);

        if (route != null)
        {
            route.InstanceId = instanceId;
            return route;
        }

        route = new DestinationRoute(fileTypeId, instanceId);

        // Keep Other files last so the table reads in matching order.
        int otherIndex = routes.FindIndex(r => r.FileTypeId == PremadeFileTypes.OtherFiles);
        routes.Insert(otherIndex >= 0 ? otherIndex : routes.Count, route);
        return route;
    }

    /// <summary>Removes a route. The Other files route of a default table cannot be removed.</summary>
    public static bool Remove(List<DestinationRoute> routes, string fileTypeId, bool isDefaultTable = true)
    {
        if (isDefaultTable && fileTypeId == PremadeFileTypes.OtherFiles)
        {
            return false;
        }

        return routes.RemoveAll(route => route.FileTypeId == fileTypeId) > 0;
    }

    /// <summary>Removes a custom file type together with its routes in every table.</summary>
    public static void RemoveFileType(DestinationRoutingConfig config, string fileTypeId, params List<DestinationRoute>?[] tables)
    {
        if (PremadeFileTypes.IsPremade(fileTypeId))
        {
            throw new ArgumentException("Premade file types cannot be removed.", nameof(fileTypeId));
        }

        config.RemoveCustomFileType(fileTypeId);

        foreach (List<DestinationRoute>? table in tables)
        {
            table?.RemoveAll(route => route.FileTypeId == fileTypeId);
        }
    }

    /// <summary>
    /// Task routes replace default routes of the same file type, the rest is inherited.
    /// This is how a hotkey overrides only Videos and keeps everything else.
    /// </summary>
    public static List<(DestinationRoute Route, bool IsOverride)> Merge(IEnumerable<DestinationRoute>? defaultRoutes, IEnumerable<DestinationRoute>? taskRoutes)
    {
        List<(DestinationRoute Route, bool IsOverride)> merged = new List<(DestinationRoute, bool)>();
        List<DestinationRoute> overrides = taskRoutes?.ToList() ?? new List<DestinationRoute>();

        foreach (DestinationRoute route in defaultRoutes ?? Enumerable.Empty<DestinationRoute>())
        {
            DestinationRoute? replacement = overrides.FirstOrDefault(r => r.FileTypeId == route.FileTypeId);
            merged.Add(replacement != null ? (replacement, true) : (route, false));
        }

        foreach (DestinationRoute route in overrides)
        {
            if (!merged.Any(m => m.Route.FileTypeId == route.FileTypeId))
            {
                merged.Add((route, true));
            }
        }

        return merged;
    }

    /// <summary>Checks a default table: exactly one Other files route, no duplicate file types, known types and instances.</summary>
    public static IReadOnlyList<string> Validate(DestinationRoutingConfig config, IReadOnlyList<DestinationRoute> routes, bool isDefaultTable = true)
    {
        List<string> errors = new List<string>();

        if (isDefaultTable && routes.Count(route => route.FileTypeId == PremadeFileTypes.OtherFiles) != 1)
        {
            errors.Add("The route table must contain one Other files route.");
        }

        foreach (IGrouping<string, DestinationRoute> group in routes.GroupBy(route => route.FileTypeId).Where(g => g.Count() > 1))
        {
            errors.Add($"File type '{group.Key}' has more than one route.");
        }

        foreach (DestinationRoute route in routes)
        {
            FileTypeDefinition? fileType = config.FindFileType(route.FileTypeId);
            DestinationInstance? instance = config.FindInstance(route.InstanceId);

            if (fileType == null)
            {
                errors.Add($"File type '{route.FileTypeId}' does not exist.");
            }
            else if (instance == null)
            {
                errors.Add($"The {fileType.Name} route points to an instance that no longer exists.");
            }
            else if (!config.IsCompatible(instance, fileType))
            {
                errors.Add($"{instance.Name} does not accept {fileType.Name}.");
            }
        }

        return errors;
    }
}
