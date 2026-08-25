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

namespace ShareX.Tools;

public sealed record MetadataValue(string Group, string Tag, string Value);

public static class MetadataService
{
    public static async Task<IReadOnlyList<MetadataValue>> ReadMetadataAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        ValidateFile(filePath);

        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return MetadataReader.Read(filePath, cancellationToken);
        }, cancellationToken);
    }

    public static bool CanStripMetadata(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return false;
        }

        try
        {
            MetadataFormat format = MetadataFormatDetector.Detect(filePath);
            return format == MetadataFormat.IsoBaseMedia
                ? MetadataStripper.IsIsoVideoPath(filePath)
                : format.CanStripMetadata();
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    public static async Task StripMetadataAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ValidateFile(filePath);

        await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            MetadataStripper.Strip(filePath, cancellationToken);
        }, cancellationToken);
    }

    public static void StripFileMetadata(string filePath)
    {
        StripMetadataAsync(filePath).GetAwaiter().GetResult();
    }

    private static void ValidateFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(Localization.Strings.MetadataService_Selected_file_not_found, filePath);
        }
    }
}
