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

using System.Text;

namespace ShareX.Tools;

internal sealed class NetworkMonitorEventLog
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);
    private readonly string? _filePath;
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    public NetworkMonitorEventLog(string? filePath)
    {
        _filePath = filePath;
    }

    public async Task AppendAsync(NetworkMonitorEvent entry, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_filePath))
        {
            return;
        }

        try
        {
            await _fileLock.WaitAsync(cancellationToken);
            try
            {
                EnsureFileExists();
                await File.AppendAllTextAsync(_filePath, entry.LogText + Environment.NewLine, Utf8NoBom, cancellationToken);
            }
            finally
            {
                _fileLock.Release();
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(NetworkMonitorEventLog), "Failed to write the network monitor event log.", ex);
        }
    }

    public async Task<bool> ClearAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_filePath))
        {
            return true;
        }

        try
        {
            await _fileLock.WaitAsync(cancellationToken);
            try
            {
                string? directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                await File.WriteAllTextAsync(_filePath, string.Empty, Utf8NoBom, cancellationToken);
                return true;
            }
            finally
            {
                _fileLock.Release();
            }
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(NetworkMonitorEventLog), "Failed to clear the network monitor event log.", ex);
            return false;
        }
    }

    public bool EnsureFileExists()
    {
        if (string.IsNullOrWhiteSpace(_filePath))
        {
            return false;
        }

        try
        {
            string? directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, string.Empty, Utf8NoBom);
            }
            return true;
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(NetworkMonitorEventLog), "Failed to create the network monitor event log.", ex);
            return false;
        }
    }
}
