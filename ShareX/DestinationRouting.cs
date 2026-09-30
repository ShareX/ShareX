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

#nullable enable

using ShareX.Destinations;
using ShareX.HelpersLib;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX;

/// <summary>Upload routing for tasks: migration of the legacy destinations, route resolution and route edits.</summary>
internal static class DestinationRouting
{
    private static readonly object migrationLock = new object();

    internal static DestinationRoutingConfig Config => ApplicationState.UploadersConfig.DestinationRouting;

    /// <summary>Creates instances and default routes from the existing settings. Called after the uploader settings load.</summary>
    internal static void Migrate(UploadersConfig config, TaskSettings? defaultTaskSettings, IEnumerable<TaskSettings?>? hotkeyTaskSettings)
    {
        lock (migrationLock)
        {
            DestinationMigration.EnsureInstances(config);

            if (defaultTaskSettings != null)
            {
                EnsureDefaultRoutes(config, defaultTaskSettings);
            }

            foreach (TaskSettings? taskSettings in hotkeyTaskSettings ?? Enumerable.Empty<TaskSettings?>())
            {
                if (taskSettings != null)
                {
                    EnsureTaskRoutes(config, taskSettings);
                }
            }
        }
    }

    private static void EnsureDefaultRoutes(UploadersConfig config, TaskSettings taskSettings)
    {
        if (taskSettings.DestinationRoutes == null || taskSettings.DestinationRoutes.Count == 0)
        {
            taskSettings.DestinationRoutes = DestinationMigration.CreateRoutes(config, GetLegacy(taskSettings));
        }

        // Other files is permanent. If its instance is gone, fall back to the legacy file destination.
        DestinationRoute? other = DestinationRoutes.Find(taskSettings.DestinationRoutes, PremadeFileTypes.OtherFiles);

        if (other == null || config.DestinationRouting.FindInstance(other.InstanceId) == null)
        {
            DestinationInstance? files = config.DestinationRouting.FindDefaultInstance(UploaderCategory.File, taskSettings.FileDestination.ToString());

            if (files != null)
            {
                DestinationRoutes.Set(taskSettings.DestinationRoutes, PremadeFileTypes.OtherFiles, files.Id);
            }
        }
    }

    private static void EnsureTaskRoutes(UploadersConfig config, TaskSettings taskSettings)
    {
        if (taskSettings.DestinationRoutes != null)
        {
            return;
        }

        // A task that used its own destinations keeps them as overrides. Others inherit every default route.
        taskSettings.DestinationRoutes = taskSettings.UseDefaultDestinations
            ? new List<DestinationRoute>()
            : DestinationMigration.CreateRoutes(config, GetLegacy(taskSettings));
    }

    internal static LegacyDestinations GetLegacy(TaskSettings taskSettings) => new LegacyDestinations
    {
        ImageDestination = taskSettings.ImageDestination,
        ImageFileDestination = taskSettings.ImageFileDestination,
        TextDestination = taskSettings.TextDestination,
        TextFileDestination = taskSettings.TextFileDestination,
        FileDestination = taskSettings.FileDestination
    };

    private static void SetLegacy(TaskSettings taskSettings, LegacyDestinations legacy)
    {
        taskSettings.ImageDestination = legacy.ImageDestination;
        taskSettings.ImageFileDestination = legacy.ImageFileDestination;
        taskSettings.TextDestination = legacy.TextDestination;
        taskSettings.TextFileDestination = legacy.TextFileDestination;
        taskSettings.FileDestination = legacy.FileDestination;
    }

    private static void EnsureMigrated(TaskSettings taskSettings)
    {
        UploadersConfig config = ApplicationState.UploadersConfig;

        if (ApplicationState.DefaultTaskSettings?.DestinationRoutes == null || taskSettings.DestinationRoutes == null)
        {
            lock (migrationLock)
            {
                DestinationMigration.EnsureInstances(config);

                if (ApplicationState.DefaultTaskSettings != null)
                {
                    EnsureDefaultRoutes(config, ApplicationState.DefaultTaskSettings);
                }

                EnsureTaskRoutes(config, taskSettings);
            }
        }
    }

    /// <summary>The default route table. It always contains Other files.</summary>
    internal static List<DestinationRoute> GetDefaultRoutes()
    {
        EnsureMigrated(ApplicationState.DefaultTaskSettings);
        return ApplicationState.DefaultTaskSettings.DestinationRoutes!;
    }

    /// <summary>Routes a task adds or overrides, or null when it uses the default routes.</summary>
    internal static List<DestinationRoute>? GetTaskRoutes(TaskSettings taskSettings)
    {
        if (taskSettings.UseDefaultDestinations || ReferenceEquals(taskSettings, ApplicationState.DefaultTaskSettings))
        {
            return null;
        }

        EnsureMigrated(taskSettings);
        return taskSettings.DestinationRoutes;
    }

    /// <summary>The routes the task uses, with task overrides applied.</summary>
    internal static IReadOnlyList<(DestinationRoute Route, bool IsOverride)> GetEffectiveRoutes(TaskSettings taskSettings) =>
        DestinationRoutes.Merge(GetDefaultRoutes(), GetTaskRoutes(taskSettings));

    internal static RouteRequest CreateRequest(EDataType dataType, string? fileName) => new RouteRequest
    {
        Extension = RouteRequest.GetExtension(fileName),
        KnownFileType = dataType switch
        {
            EDataType.Image => PremadeFileTypes.Images,
            EDataType.Text => PremadeFileTypes.Text,
            _ => null
        },
        // ShareX already decided this is neither an image nor text using the task's extension lists.
        ExcludedFileTypes = dataType == EDataType.File ? [PremadeFileTypes.Images, PremadeFileTypes.Text] : null
    };

    internal static RouteMatch? Resolve(TaskSettings taskSettings, EDataType dataType, string? fileName) =>
        RouteResolver.Resolve(Config, GetDefaultRoutes(), GetTaskRoutes(taskSettings), CreateRequest(dataType, fileName));

    /// <summary>The table a route edit writes to: the task's own routes, or the default table.</summary>
    private static List<DestinationRoute> GetEditableRoutes(TaskSettings taskSettings)
    {
        if (ReferenceEquals(taskSettings, ApplicationState.DefaultTaskSettings) || taskSettings.UseDefaultDestinations)
        {
            return GetDefaultRoutes();
        }

        EnsureMigrated(taskSettings);
        return taskSettings.DestinationRoutes!;
    }

    internal static bool IsDefaultTable(TaskSettings taskSettings) =>
        ReferenceEquals(taskSettings, ApplicationState.DefaultTaskSettings) || taskSettings.UseDefaultDestinations;

    internal static void SetRoute(TaskSettings taskSettings, string fileTypeId, DestinationInstance instance)
    {
        List<DestinationRoute> routes = GetEditableRoutes(taskSettings);
        DestinationRoutes.Set(routes, fileTypeId, instance.Id);
        UpdateLegacy(taskSettings);
    }

    internal static bool RemoveRoute(TaskSettings taskSettings, string fileTypeId)
    {
        List<DestinationRoute> routes = GetEditableRoutes(taskSettings);
        bool removed = DestinationRoutes.Remove(routes, fileTypeId, IsDefaultTable(taskSettings));
        UpdateLegacy(taskSettings);
        return removed;
    }

    /// <summary>Keeps the legacy fields in step with the routes they can express.</summary>
    internal static void UpdateLegacy(TaskSettings taskSettings)
    {
        TaskSettings target = IsDefaultTable(taskSettings) ? ApplicationState.DefaultTaskSettings : taskSettings;
        LegacyDestinations legacy = GetLegacy(target);
        IEnumerable<DestinationRoute> routes = target == ApplicationState.DefaultTaskSettings
            ? GetDefaultRoutes()
            : GetEffectiveRoutes(target).Select(x => x.Route);
        DestinationMigration.UpdateLegacy(ApplicationState.UploadersConfig, routes, legacy);
        SetLegacy(target, legacy);
    }

    /// <summary>The instance a route uses, for menus: "Videos: Dropbox".</summary>
    internal static string GetRouteLabel(FileTypeDefinition fileType, DestinationInstance? instance) =>
        instance != null ? $"{fileType.Name}: {instance.Name}" : fileType.Name;

    internal static IEnumerable<DestinationInstance> GetCompatibleInstances(FileTypeDefinition fileType) =>
        Config.GetCompatibleInstances(fileType);
}
