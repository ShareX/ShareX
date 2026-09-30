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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ShareX.Destinations;

/// <summary>A named configuration of an uploader, for example "S3 Media" and "S3 Backups" both using Amazon S3.</summary>
public sealed class DestinationInstance
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = "";

    public UploaderCategory Category { get; set; }

    /// <summary>The uploader enum member name, for example "Imgur", "AmazonS3" or "CustomFileUploader".</summary>
    public string Uploader { get; set; } = "";

    /// <summary>
    /// The FTP account or custom uploader this instance uses. Null keeps ShareX's per category selection
    /// (FTPSelectedImage/Text/File or Custom*UploaderSelected), which is how migrated configurations behave.
    /// </summary>
    public int? AccountIndex { get; set; }

    /// <summary>
    /// File type ids this instance may be routed to, or <see cref="PremadeFileTypes.AllFiles"/>.
    /// Null uses the default for <see cref="Category"/>: images, text or all files.
    /// </summary>
    public List<string>? AcceptedFileTypes { get; set; }

    /// <summary>
    /// Settings that differ from the shared uploader settings, keyed by UploadersConfig property name.
    /// Null for the default instance of each uploader, which uses the shared settings exactly as before.
    /// </summary>
    public JObject? Settings { get; set; }

    /// <summary>
    /// The instance created for an uploader during migration. It edits the shared settings and cannot be removed,
    /// so every uploader stays available.
    /// </summary>
    public bool IsDefault { get; set; }

    [JsonIgnore]
    public bool HasOwnSettings => Settings != null;

    public IReadOnlyList<string> GetAcceptedFileTypes() => AcceptedFileTypes ?? GetDefaultAcceptedFileTypes(Category);

    public static IReadOnlyList<string> GetDefaultAcceptedFileTypes(UploaderCategory category) => category switch
    {
        UploaderCategory.Image => [PremadeFileTypes.Images],
        UploaderCategory.Text => [PremadeFileTypes.Text],
        _ => [PremadeFileTypes.AllFiles]
    };

    /// <summary>True when both instances call the same uploader service with the same account selection.</summary>
    public bool IsSameUploader(DestinationInstance other) =>
        Category == other.Category && string.Equals(Uploader, other.Uploader, StringComparison.Ordinal) && AccountIndex == other.AccountIndex;

    /// <summary>A deep copy with a new id, including credentials. The copy never counts as the default instance.</summary>
    public DestinationInstance Duplicate(string name) => new DestinationInstance
    {
        Id = Guid.NewGuid(),
        Name = name,
        Category = Category,
        Uploader = Uploader,
        AccountIndex = AccountIndex,
        AcceptedFileTypes = AcceptedFileTypes != null ? new List<string>(AcceptedFileTypes) : null,
        Settings = (JObject?)Settings?.DeepClone(),
        IsDefault = false
    };

    public override string ToString() => Name;
}

/// <summary>Sends files of one file type to one instance.</summary>
public sealed class DestinationRoute
{
    public DestinationRoute()
    {
    }

    public DestinationRoute(string fileTypeId, Guid instanceId)
    {
        FileTypeId = fileTypeId;
        InstanceId = instanceId;
    }

    public string FileTypeId { get; set; } = "";

    public Guid InstanceId { get; set; }

    public DestinationRoute Clone() => new DestinationRoute(FileTypeId, InstanceId);

    public override string ToString() => $"{FileTypeId} -> {InstanceId}";
}
