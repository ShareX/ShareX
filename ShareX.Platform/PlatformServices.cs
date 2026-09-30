#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

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
