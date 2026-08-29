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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace ShareX.Tools;

public sealed partial class NetworkMonitorViewModel : ViewModelBase, IDisposable
{
    private const int DisconnectFailureThreshold = 2;
    private const int MaximumVisibleEvents = 1000;
    private static readonly TimeSpan MaximumSampleAge = TimeSpan.FromHours(24);

    public static IReadOnlyList<NetworkMonitorTargetItem> Targets { get; } = CreateTargets();
    public static IReadOnlyList<NetworkMonitorIntervalItem> Intervals { get; } =
    [
        new("2 s", TimeSpan.FromSeconds(2)),
        new("5 s", TimeSpan.FromSeconds(5)),
        new("10 s", TimeSpan.FromSeconds(10)),
        new("30 s", TimeSpan.FromSeconds(30))
    ];
    public static IReadOnlyList<NetworkMonitorTimeRangeItem> TimeRanges { get; } =
    [
        new(Localization.Strings.NetworkMonitorViewModel_Last_5_minutes, TimeSpan.FromMinutes(5)),
        new(Localization.Strings.NetworkMonitorViewModel_Last_15_minutes, TimeSpan.FromMinutes(15)),
        new(Localization.Strings.NetworkMonitorViewModel_Last_hour, TimeSpan.FromHours(1)),
        new(Localization.Strings.NetworkMonitorViewModel_Last_6_hours, TimeSpan.FromHours(6)),
        new(Localization.Strings.NetworkMonitorViewModel_Session, null)
    ];

    private readonly NetworkMonitorServices _services;
    private readonly INetworkMonitorProbe _probe;
    private readonly NetworkMonitorEventLog _eventLog;
    private readonly SemaphoreSlim _probeLock = new(1, 1);
    private CancellationTokenSource? _monitoringCancellation;
    private Task? _monitoringTask;
    private int _monitoringGeneration;
    private bool? _connectedState;
    private DateTimeOffset _stateChangedAt;
    private int _consecutiveFailures;

    public ObservableCollection<NetworkMonitorEvent> Events { get; } = [];
    public ObservableCollection<NetworkMonitorSample> Samples { get; } = [];

    [ObservableProperty]
    private NetworkMonitorTargetItem _selectedTarget = Targets[0];

    [ObservableProperty]
    private NetworkMonitorIntervalItem _selectedInterval = Intervals[1];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ChartRange))]
    private NetworkMonitorTimeRangeItem _selectedTimeRange = TimeRanges[1];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanStart))]
    [NotifyPropertyChangedFor(nameof(CanStop))]
    [NotifyCanExecuteChangedFor(nameof(StartMonitoringCommand))]
    [NotifyCanExecuteChangedFor(nameof(StopMonitoringCommand))]
    private bool _isMonitoring;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsConnected))]
    [NotifyPropertyChangedFor(nameof(IsDisconnected))]
    private string _statusText = Localization.Strings.NetworkMonitorViewModel_Checking;

    [ObservableProperty]
    private string _statusDetails = Localization.Strings.NetworkMonitorViewModel_Waiting_for_first_check;

    [ObservableProperty]
    private string _currentLatencyText = "—";

    [ObservableProperty]
    private string _averageLatencyText = "—";

    [ObservableProperty]
    private string _availabilityText = "—";

    [ObservableProperty]
    private string _disconnectCountText = "0";

    [ObservableProperty]
    private bool _hasEvents;

    public bool IsConnected => _connectedState == true;
    public bool IsDisconnected => _connectedState == false;
    public bool CanStart => !IsMonitoring;
    public bool CanStop => IsMonitoring;
    public bool CanOpenLog => _services.OpenFile != null && !string.IsNullOrWhiteSpace(_services.LogFilePath);
    public TimeSpan? ChartRange => SelectedTimeRange.Duration;

    public NetworkMonitorViewModel(NetworkMonitorServices services)
        : this(services, new NetworkMonitorProbe())
    {
    }

    internal NetworkMonitorViewModel(NetworkMonitorServices services, INetworkMonitorProbe probe)
    {
        _services = services;
        _probe = probe;
        _eventLog = new NetworkMonitorEventLog(services.LogFilePath);

        foreach (NetworkMonitorEvent entry in _eventLog.Load(MaximumVisibleEvents))
        {
            Events.Add(entry);
        }
        UpdateEventSummary();
    }

    public void Start()
    {
        if (IsMonitoring)
        {
            return;
        }

        _monitoringCancellation?.Cancel();
        _monitoringCancellation?.Dispose();
        _monitoringCancellation = new CancellationTokenSource();
        int generation = ++_monitoringGeneration;
        IsMonitoring = true;
        _monitoringTask = MonitorAsync(_monitoringCancellation.Token, generation);
    }

    [RelayCommand(CanExecute = nameof(CanStart))]
    private void StartMonitoring() => Start();

    [RelayCommand(CanExecute = nameof(CanStop))]
    private void StopMonitoring()
    {
        if (!IsMonitoring)
        {
            return;
        }

        _monitoringCancellation?.Cancel();
        IsMonitoring = false;
        if (_connectedState == null)
        {
            StatusText = Localization.Strings.NetworkMonitorViewModel_Paused;
            StatusDetails = Localization.Strings.NetworkMonitorViewModel_Monitoring_paused;
        }
    }

    [RelayCommand]
    private void CopyAll()
    {
        if (_services.CopyText == null)
        {
            return;
        }

        StringBuilder text = new();
        foreach (NetworkMonitorEvent entry in Events.Reverse())
        {
            text.Append(entry.TimestampText).Append('\t')
                .Append(entry.StatusText).Append('\t')
                .Append(entry.DurationText).Append('\t')
                .AppendLine(entry.Details);
        }
        _services.CopyText(text.ToString().TrimEnd());
    }

    [RelayCommand]
    private void OpenLogFile()
    {
        if (_services.OpenFile != null && !string.IsNullOrWhiteSpace(_services.LogFilePath) && _eventLog.EnsureFileExists())
        {
            _services.OpenFile(_services.LogFilePath);
        }
    }

    private async Task MonitorAsync(CancellationToken cancellationToken, int generation)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ProbeOnceAsync(cancellationToken);
                await Task.Delay(SelectedInterval.Interval, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(NetworkMonitorViewModel), "Network monitoring stopped unexpectedly.", ex);
            StatusDetails = string.Format(Localization.Strings.NetworkMonitorViewModel_Monitoring_failed, ex.Message);
        }
        finally
        {
            if (_monitoringGeneration == generation)
            {
                IsMonitoring = false;
            }
        }
    }

    private async Task ProbeOnceAsync(CancellationToken cancellationToken)
    {
        if (!await _probeLock.WaitAsync(0, cancellationToken))
        {
            return;
        }

        try
        {
            NetworkMonitorProbeResult result = await _probe.ProbeAsync(SelectedTarget, TimeSpan.FromSeconds(2), cancellationToken);
            Samples.Add(new NetworkMonitorSample(result.Timestamp, result.Success, result.LatencyMilliseconds));
            TrimSamples(result.Timestamp);
            UpdateSampleSummary(result);
            await UpdateConnectionStateAsync(result, cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(NetworkMonitorViewModel), "Network probe failed.", ex);
            StatusDetails = string.Format(Localization.Strings.NetworkMonitorViewModel_Monitoring_failed, ex.Message);
        }
        finally
        {
            _probeLock.Release();
        }
    }

    private async Task UpdateConnectionStateAsync(NetworkMonitorProbeResult result, CancellationToken cancellationToken)
    {
        if (result.Success)
        {
            _consecutiveFailures = 0;
            if (_connectedState != true)
            {
                await AddTransitionAsync(NetworkMonitorEventStatus.Connected, result, cancellationToken);
            }
            else
            {
                StatusText = Localization.Strings.NetworkMonitorViewModel_Connected;
            }
            return;
        }

        _consecutiveFailures++;
        if (_consecutiveFailures >= DisconnectFailureThreshold && _connectedState != false)
        {
            await AddTransitionAsync(NetworkMonitorEventStatus.Disconnected, result, cancellationToken);
        }
        else if (_connectedState == null)
        {
            StatusText = Localization.Strings.NetworkMonitorViewModel_Checking;
        }
    }

    private async Task AddTransitionAsync(
        NetworkMonitorEventStatus status,
        NetworkMonitorProbeResult result,
        CancellationToken cancellationToken)
    {
        DateTimeOffset changedAt = result.Timestamp;
        TimeSpan? duration = _connectedState == null ? null : changedAt - _stateChangedAt;
        _connectedState = status == NetworkMonitorEventStatus.Connected;
        _stateChangedAt = changedAt;
        StatusText = _connectedState.Value
            ? Localization.Strings.NetworkMonitorViewModel_Connected
            : Localization.Strings.NetworkMonitorViewModel_Disconnected;
        OnPropertyChanged(nameof(IsConnected));
        OnPropertyChanged(nameof(IsDisconnected));

        string details = result.Success
            ? string.Format(Localization.Strings.NetworkMonitorViewModel_Connected_via, result.Endpoint, result.Method)
            : result.ErrorMessage;
        NetworkMonitorEvent entry = new()
        {
            Timestamp = changedAt,
            Status = status,
            PreviousStateDuration = duration,
            Details = details
        };

        Events.Insert(0, entry);
        while (Events.Count > MaximumVisibleEvents)
        {
            Events.RemoveAt(Events.Count - 1);
        }
        UpdateEventSummary();
        await _eventLog.AppendAsync(entry, cancellationToken);
    }

    private void UpdateSampleSummary(NetworkMonitorProbeResult result)
    {
        if (result.Success && result.LatencyMilliseconds != null)
        {
            CurrentLatencyText = string.Format(CultureInfo.CurrentCulture,
                Localization.Strings.NetworkMonitorViewModel_Milliseconds, result.LatencyMilliseconds.Value);
            StatusDetails = string.Format(Localization.Strings.NetworkMonitorViewModel_Reply_from,
                result.Endpoint, result.Method);
        }
        else
        {
            CurrentLatencyText = "—";
            StatusDetails = result.ErrorMessage;
        }

        NetworkMonitorSample[] visible = GetVisibleSamples().ToArray();
        UpdateAggregateSummary(visible);
    }

    private void UpdateAggregateSummary(IReadOnlyCollection<NetworkMonitorSample> visible)
    {
        double[] successfulLatencies = visible
            .Where(x => x.Success && x.LatencyMilliseconds != null)
            .Select(x => x.LatencyMilliseconds!.Value)
            .ToArray();
        AverageLatencyText = successfulLatencies.Length > 0
            ? string.Format(CultureInfo.CurrentCulture, Localization.Strings.NetworkMonitorViewModel_Milliseconds, successfulLatencies.Average())
            : "—";
        AvailabilityText = visible.Count > 0
            ? string.Format(CultureInfo.CurrentCulture, Localization.Strings.NetworkMonitorViewModel_Percentage,
                visible.Count(x => x.Success) * 100d / visible.Count)
            : "—";
    }

    partial void OnSelectedTimeRangeChanged(NetworkMonitorTimeRangeItem value)
    {
        UpdateAggregateSummary(GetVisibleSamples().ToArray());
    }

    private IEnumerable<NetworkMonitorSample> GetVisibleSamples()
    {
        if (SelectedTimeRange.Duration == null)
        {
            return Samples;
        }

        DateTimeOffset cutoff = DateTimeOffset.Now - SelectedTimeRange.Duration.Value;
        return Samples.Where(x => x.Timestamp >= cutoff);
    }

    private void TrimSamples(DateTimeOffset now)
    {
        DateTimeOffset cutoff = now - MaximumSampleAge;
        while (Samples.Count > 0 && Samples[0].Timestamp < cutoff)
        {
            Samples.RemoveAt(0);
        }
    }

    private void UpdateEventSummary()
    {
        HasEvents = Events.Count > 0;
        DisconnectCountText = Events.Count(x => x.IsDisconnected).ToString(CultureInfo.CurrentCulture);
    }

    private static IReadOnlyList<NetworkMonitorTargetItem> CreateTargets()
    {
        NetworkMonitorEndpoint cloudflare = new("Cloudflare", "1.1.1.1", 443);
        NetworkMonitorEndpoint google = new("Google DNS", "8.8.8.8", 53);
        NetworkMonitorEndpoint quad9 = new("Quad9", "9.9.9.9", 53);
        return
        [
            new(Localization.Strings.NetworkMonitorViewModel_Automatic_recommended, [cloudflare, google, quad9]),
            new("Cloudflare (1.1.1.1)", [cloudflare]),
            new("Google DNS (8.8.8.8)", [google]),
            new("Quad9 (9.9.9.9)", [quad9])
        ];
    }

    public void Dispose()
    {
        _monitoringCancellation?.Cancel();
        _monitoringCancellation?.Dispose();
    }
}
