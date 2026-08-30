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

public sealed class NetworkMonitorServices
{
    public string? LogFilePath { get; init; }
    public Action<string>? CopyText { get; init; }
    public Action<string>? OpenFile { get; init; }
}

public sealed record NetworkMonitorEndpoint(string Name, string Host);

public sealed record NetworkMonitorTargetItem(
    string DisplayName,
    IReadOnlyList<NetworkMonitorEndpoint> Endpoints);

public sealed record NetworkMonitorIntervalItem(string DisplayName, TimeSpan Interval);

public sealed record NetworkMonitorTimeRangeItem(string DisplayName, TimeSpan? Duration);

public sealed record NetworkMonitorProbeResult(
    DateTimeOffset Timestamp,
    bool Success,
    double? LatencyMilliseconds,
    string Endpoint,
    string Method,
    string ErrorMessage);

public sealed record NetworkMonitorSample(
    DateTimeOffset Timestamp,
    bool Success,
    double? LatencyMilliseconds);

public enum NetworkMonitorEventStatus
{
    Connected,
    Disconnected
}

public sealed class NetworkMonitorEvent
{
    public required DateTimeOffset Timestamp { get; init; }
    public required NetworkMonitorEventStatus Status { get; init; }

    public bool IsConnected => Status == NetworkMonitorEventStatus.Connected;
    public bool IsDisconnected => Status == NetworkMonitorEventStatus.Disconnected;
    public string TimestampText => Timestamp.LocalDateTime.ToString("G");
    public string StatusText => IsConnected
        ? Localization.Strings.NetworkMonitorViewModel_Connected
        : Localization.Strings.NetworkMonitorViewModel_Disconnected;
    public string LogText => $"{Timestamp.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)} - {Status}.";
}
