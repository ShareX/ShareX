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

/// <summary>Instances and custom file types. Stored with the uploader settings because instances carry credentials.</summary>
public sealed class DestinationRoutingConfig
{
    public List<DestinationInstance> Instances { get; set; } = new List<DestinationInstance>();

    public List<FileTypeDefinition> CustomFileTypes { get; set; } = new List<FileTypeDefinition>();

    public DestinationInstance? FindInstance(Guid id) => Instances.FirstOrDefault(instance => instance.Id == id);

    public DestinationInstance? FindDefaultInstance(UploaderCategory category, string uploader) =>
        Instances.FirstOrDefault(instance => instance.IsDefault && instance.Category == category &&
            string.Equals(instance.Uploader, uploader, StringComparison.Ordinal) && instance.AccountIndex == null);

    /// <summary>Premade types first, then custom types, in the order they are matched within each group.</summary>
    public IEnumerable<FileTypeDefinition> GetFileTypes() => PremadeFileTypes.All.Take(PremadeFileTypes.All.Count - 1)
        .Concat(CustomFileTypes)
        .Append(PremadeFileTypes.Get(PremadeFileTypes.OtherFiles)!);

    public FileTypeDefinition? FindFileType(string? id) => PremadeFileTypes.Get(id) ?? CustomFileTypes.FirstOrDefault(type => type.Id == id);

    /// <summary>Whether the instance can be picked for a route of this file type.</summary>
    public bool IsCompatible(DestinationInstance instance, FileTypeDefinition fileType)
    {
        IReadOnlyList<string> accepted = instance.GetAcceptedFileTypes();

        if (accepted.Contains(PremadeFileTypes.AllFiles))
        {
            return true;
        }

        if (fileType.IsOtherFiles)
        {
            return false;
        }

        if (accepted.Contains(fileType.Id))
        {
            return true;
        }

        // A custom type such as "PNG" fits an instance that accepts images when every extension is an image extension.
        if (!fileType.IsPremade && fileType.Extensions.Count > 0)
        {
            return fileType.Extensions.All(extension => accepted.Select(FindFileType).Any(type => type != null && type.Contains(extension)));
        }

        return false;
    }

    public IEnumerable<DestinationInstance> GetCompatibleInstances(FileTypeDefinition fileType) =>
        Instances.Where(instance => IsCompatible(instance, fileType));

    public FileTypeDefinition AddCustomFileType(string name, IEnumerable<string> extensions)
    {
        FileTypeDefinition fileType = new FileTypeDefinition(Guid.NewGuid().ToString("N"), name.Trim(), extensions);

        if (fileType.Name.Length == 0)
        {
            throw new ArgumentException("A file type needs a name.", nameof(name));
        }

        if (fileType.Extensions.Count == 0)
        {
            throw new ArgumentException("A file type needs at least one extension.", nameof(extensions));
        }

        CustomFileTypes.Add(fileType);
        return fileType;
    }

    /// <summary>Removes a custom file type. Routes for it must be removed by the caller, <see cref="DestinationRoutes.RemoveFileType"/> does it.</summary>
    public bool RemoveCustomFileType(string id) => CustomFileTypes.RemoveAll(type => type.Id == id) > 0;

    public DestinationInstance Add(DestinationInstance instance)
    {
        Instances.Add(instance);
        return instance;
    }

    /// <summary>Copies the instance, including its credentials, and inserts the copy after it.</summary>
    public DestinationInstance Duplicate(DestinationInstance instance, string? name = null)
    {
        DestinationInstance copy = instance.Duplicate(name ?? GetUniqueName(instance.Name));
        int index = Instances.IndexOf(instance);
        Instances.Insert(index >= 0 ? index + 1 : Instances.Count, copy);
        return copy;
    }

    public void Rename(DestinationInstance instance, string name)
    {
        string trimmed = name.Trim();

        if (trimmed.Length == 0)
        {
            throw new ArgumentException("An instance needs a name.", nameof(name));
        }

        instance.Name = trimmed;
    }

    /// <summary>Default instances cannot be removed. Routes to a removed instance fall through to the next matching rule.</summary>
    public bool CanRemove(DestinationInstance instance) => !instance.IsDefault;

    public bool Remove(DestinationInstance instance) => CanRemove(instance) && Instances.Remove(instance);

    /// <summary>"S3 Media (2)" style names so duplicates stay distinguishable.</summary>
    public string GetUniqueName(string baseName)
    {
        HashSet<string> names = new HashSet<string>(Instances.Select(instance => instance.Name), StringComparer.OrdinalIgnoreCase);

        for (int i = 2; ; i++)
        {
            string name = $"{baseName} ({i})";

            if (!names.Contains(name))
            {
                return name;
            }
        }
    }
}
