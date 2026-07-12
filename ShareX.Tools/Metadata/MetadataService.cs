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

using ShareX.HelpersLib;
using System.Diagnostics;

namespace ShareX.Tools;

public static class MetadataService
{
    public static string ExifToolPath => FileHelpers.GetAbsolutePath("exiftool.exe");

    public static async Task<string> ReadMetadataAsync(string filePath, CancellationToken cancellationToken = default)
    {
        return await RunExifToolAsync(filePath, ["-G", "-t", "-m", "-q"], cancellationToken);
    }

    public static async Task StripMetadataAsync(string filePath, CancellationToken cancellationToken = default)
    {
        await RunExifToolAsync(filePath, ["-all=", "-overwrite_original", "-q"], cancellationToken);
    }

    public static void StripFileMetadata(string filePath)
    {
        StripMetadataAsync(filePath).GetAwaiter().GetResult();
    }

    private static async Task<string> RunExifToolAsync(
        string filePath,
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(ExifToolPath))
        {
            throw new FileNotFoundException("ExifTool could not be found.", ExifToolPath);
        }
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The selected file could not be found.", filePath);
        }

        ProcessStartInfo startInfo = new()
        {
            FileName = ExifToolPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add(filePath);
        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start ExifTool.");
        Task<string> outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        Task<string> errorTask = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        string output = await outputTask;
        string error = await errorTask;

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(error)
                ? $"ExifTool exited with code {process.ExitCode}."
                : error.Trim());
        }

        return output;
    }
}
