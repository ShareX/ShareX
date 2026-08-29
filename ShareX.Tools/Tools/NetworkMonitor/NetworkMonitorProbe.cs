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

using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ShareX.Tools;

internal interface INetworkMonitorProbe
{
    Task<NetworkMonitorProbeResult> ProbeAsync(
        NetworkMonitorTargetItem target,
        TimeSpan timeout,
        CancellationToken cancellationToken);
}

internal sealed class NetworkMonitorProbe : INetworkMonitorProbe
{
    public async Task<NetworkMonitorProbeResult> ProbeAsync(
        NetworkMonitorTargetItem target,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        DateTimeOffset timestamp = DateTimeOffset.Now;
        if (!NetworkInterface.GetIsNetworkAvailable())
        {
            return new NetworkMonitorProbeResult(timestamp, false, null, target.DisplayName, string.Empty,
                Localization.Strings.NetworkMonitorProbe_No_network_interface);
        }

        Task<EndpointProbeResult>[] probes = target.Endpoints
            .Select(endpoint => ProbeEndpointAsync(endpoint, timeout, cancellationToken))
            .ToArray();
        EndpointProbeResult[] results = await Task.WhenAll(probes);
        EndpointProbeResult? fastest = results
            .Where(x => x.Success)
            .OrderBy(x => x.LatencyMilliseconds)
            .FirstOrDefault();

        if (fastest != null)
        {
            return new NetworkMonitorProbeResult(timestamp, true, fastest.LatencyMilliseconds,
                fastest.Endpoint.Name, fastest.Method, string.Empty);
        }

        string error = string.Join("; ", results
            .Select(x => x.ErrorMessage)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.Ordinal));
        return new NetworkMonitorProbeResult(timestamp, false, null, target.DisplayName, string.Empty,
            string.IsNullOrWhiteSpace(error) ? Localization.Strings.NetworkMonitorProbe_No_response : error);
    }

    private static async Task<EndpointProbeResult> ProbeEndpointAsync(
        NetworkMonitorEndpoint endpoint,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        string pingError = string.Empty;
        try
        {
            using Ping ping = new();
            Stopwatch stopwatch = Stopwatch.StartNew();
            PingReply reply = await ping.SendPingAsync(endpoint.Host, (int)timeout.TotalMilliseconds)
                .WaitAsync(timeout + TimeSpan.FromMilliseconds(250), cancellationToken);
            stopwatch.Stop();
            if (reply.Status == IPStatus.Success)
            {
                double latency = reply.RoundtripTime > 0 ? reply.RoundtripTime : stopwatch.Elapsed.TotalMilliseconds;
                return EndpointProbeResult.Succeeded(endpoint, latency, "ICMP");
            }

            pingError = reply.Status.ToString();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            pingError = ex.Message;
        }

        try
        {
            using TcpClient client = new();
            using CancellationTokenSource timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutSource.CancelAfter(timeout);
            Stopwatch stopwatch = Stopwatch.StartNew();
            await client.ConnectAsync(endpoint.Host, endpoint.TcpPort, timeoutSource.Token);
            stopwatch.Stop();
            return EndpointProbeResult.Succeeded(endpoint, stopwatch.Elapsed.TotalMilliseconds, "TCP");
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return EndpointProbeResult.Failed(endpoint,
                string.Format(Localization.Strings.NetworkMonitorProbe_Endpoint_timed_out, endpoint.Name));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            string reason = string.IsNullOrWhiteSpace(pingError) ? ex.Message : pingError;
            return EndpointProbeResult.Failed(endpoint,
                string.Format(Localization.Strings.NetworkMonitorProbe_Endpoint_failed, endpoint.Name, reason));
        }
    }

    private sealed record EndpointProbeResult(
        NetworkMonitorEndpoint Endpoint,
        bool Success,
        double LatencyMilliseconds,
        string Method,
        string ErrorMessage)
    {
        public static EndpointProbeResult Succeeded(NetworkMonitorEndpoint endpoint, double latency, string method) =>
            new(endpoint, true, latency, method, string.Empty);

        public static EndpointProbeResult Failed(NetworkMonitorEndpoint endpoint, string error) =>
            new(endpoint, false, 0, string.Empty, error);
    }
}
