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

using System.Globalization;
using System.Text;

namespace ShareX.Tools;

internal sealed class NetworkMonitorEventLog
{
    private const string Header = "# ShareX Network Monitor event log";
    private readonly string? _filePath;
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    public NetworkMonitorEventLog(string? filePath)
    {
        _filePath = filePath;
    }

    public IReadOnlyList<NetworkMonitorEvent> Load(int maximumCount)
    {
        if (string.IsNullOrWhiteSpace(_filePath) || !File.Exists(_filePath))
        {
            return [];
        }

        try
        {
            return File.ReadLines(_filePath, Encoding.UTF8)
                .Select(Parse)
                .Where(x => x != null)
                .Cast<NetworkMonitorEvent>()
                .TakeLast(maximumCount)
                .Reverse()
                .ToArray();
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(NetworkMonitorEventLog), "Failed to read the network monitor event log.", ex);
            return [];
        }
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
                string duration = entry.PreviousStateDuration?.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture) ?? string.Empty;
                string details = Sanitize(entry.Details);
                string line = $"{entry.Timestamp:O}\t{entry.Status}\t{duration}\t{details}{Environment.NewLine}";
                await File.AppendAllTextAsync(_filePath, line, Encoding.UTF8, cancellationToken);
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
                await File.WriteAllTextAsync(_filePath, Header + Environment.NewLine, new UTF8Encoding(false), cancellationToken);
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
                File.WriteAllText(_filePath, Header + Environment.NewLine, new UTF8Encoding(false));
            }
            return true;
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(NetworkMonitorEventLog), "Failed to create the network monitor event log.", ex);
            return false;
        }
    }

    private static NetworkMonitorEvent? Parse(string line)
    {
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
        {
            return null;
        }

        string[] parts = line.Split('\t', 4);
        if (parts.Length < 2 ||
            !DateTimeOffset.TryParse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTimeOffset timestamp) ||
            !Enum.TryParse(parts[1], true, out NetworkMonitorEventStatus status))
        {
            return null;
        }

        TimeSpan? duration = null;
        if (parts.Length >= 3 && double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double milliseconds))
        {
            duration = TimeSpan.FromMilliseconds(Math.Max(0, milliseconds));
        }

        return new NetworkMonitorEvent
        {
            Timestamp = timestamp,
            Status = status,
            PreviousStateDuration = duration,
            Details = parts.Length >= 4 ? parts[3] : string.Empty
        };
    }

    private static string Sanitize(string value) => value
        .Replace('\t', ' ')
        .Replace('\r', ' ')
        .Replace('\n', ' ');
}
