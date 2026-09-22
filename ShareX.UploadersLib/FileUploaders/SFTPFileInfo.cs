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

namespace ShareX.UploadersLib.FileUploaders;

public sealed class SFTPFileInfo
{
    public string Name { get; }
    public bool IsDirectory { get; }
    public bool IsRegularFile { get; }
    public bool IsSymbolicLink { get; }
    public long Length { get; }
    public DateTime LastWriteTimeUtc { get; }

    public SFTPFileInfo(string name, bool isDirectory, bool isRegularFile, bool isSymbolicLink,
        long length, DateTime lastWriteTimeUtc)
    {
        Name = name;
        IsDirectory = isDirectory;
        IsRegularFile = isRegularFile;
        IsSymbolicLink = isSymbolicLink;
        Length = length;
        LastWriteTimeUtc = lastWriteTimeUtc;
    }
}
