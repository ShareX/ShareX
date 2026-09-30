using System;
using System.Threading;

namespace ShareX.Platform;

/// <summary>Holds the platform implementation chosen by the application at start up.</summary>
/// <example>
/// <code>
/// PlatformServices.Initialize(new WindowsPlatformServices());
/// string folder = PlatformServices.Current.Paths.GetDefaultPersonalFolder("ShareX");
/// </code>
/// </example>
public static class PlatformServices
{
    private static IPlatformServices? current;

    public static bool IsInitialized => Volatile.Read(ref current) != null;

    /// <summary>The active implementation. Throws when <see cref="Initialize"/> has not been called.</summary>
    public static IPlatformServices Current => Volatile.Read(ref current) ??
        throw new InvalidOperationException("Platform services are not initialized. Call PlatformServices.Initialize at application start up.");

    /// <summary>Sets the active implementation. May only be called once per process.</summary>
    public static void Initialize(IPlatformServices services)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (Interlocked.CompareExchange(ref current, services, null) != null)
        {
            throw new InvalidOperationException("Platform services are already initialized.");
        }
    }

    /// <summary>Disposes and clears the active implementation. Intended for application shut down and tests.</summary>
    public static void Shutdown()
    {
        Interlocked.Exchange(ref current, null)?.Dispose();
    }
}
