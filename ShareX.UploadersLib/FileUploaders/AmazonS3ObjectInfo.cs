#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

using System;

namespace ShareX.UploadersLib.FileUploaders
{
    public sealed class AmazonS3ObjectInfo
    {
        public string Key { get; }
        public bool IsFolder { get; }
        public long? Size { get; }
        public DateTimeOffset? LastModified { get; }

        public AmazonS3ObjectInfo(string key, bool isFolder, long? size = null, DateTimeOffset? lastModified = null)
        {
            Key = key;
            IsFolder = isFolder;
            Size = size;
            LastModified = lastModified;
        }
    }
}
