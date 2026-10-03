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

namespace ShareX.Platform;

/// <summary>A confirmed end of the user's session.</summary>
/// <param name="restarting">True when the system requests restart registration (Windows restart manager).</param>
public sealed class SessionEndingEventArgs(bool restarting) : EventArgs
{
    public bool Restarting { get; } = restarting;
}

/// <summary>Session end and restart registration for ShareX itself.</summary>
/// <remarks>
/// On Linux and macOS the session ends with SIGTERM, which the application handles directly, so <see cref="SessionEnding"/> is not
/// raised there.
/// </remarks>
public interface IApplicationSessionService : IDisposable
{
    /// <summary>
    /// Raised synchronously on the initializing UI thread while the system queries restart-related shutdown.
    /// Register restart here, before the query returns. Do not save or close: shutdown may still be cancelled.
    /// </summary>
    event EventHandler? RestartRequested;

    /// <summary>
    /// Raised synchronously on the initializing UI thread only when the system confirms session end.
    /// Handlers save state quickly. Cancelled queries do not raise this event.
    /// </summary>
    event EventHandler<SessionEndingEventArgs>? SessionEnding;

    /// <summary>Whether <see cref="RegisterRestart"/> works (RegisterApplicationRestart on Windows).</summary>
    FeatureSupport RestartSupport { get; }

    /// <summary>
    /// Starts session notifications on the application UI thread. Dispose on that same thread.
    /// Native callback exceptions are passed to <paramref name="onUnhandledException"/> for the host's
    /// exception handling; the callback must not throw across a native window procedure.
    /// Repeated initialization does not create another receiver.
    /// </summary>
    void Initialize(Action<Exception> onUnhandledException);

    /// <summary>Asks the system to start ShareX with <paramref name="arguments"/> when it restarts the session.</summary>
    void RegisterRestart(string arguments);
}

public sealed class UnsupportedApplicationSessionService(string reason) : IApplicationSessionService
{
    public event EventHandler? RestartRequested
    {
        add { }
        remove { }
    }

    public event EventHandler<SessionEndingEventArgs>? SessionEnding
    {
        add { }
        remove { }
    }

    public FeatureSupport RestartSupport { get; } = FeatureSupport.NotSupported(reason);

    public void Initialize(Action<Exception> onUnhandledException)
    {
    }

    public void RegisterRestart(string arguments)
    {
    }

    public void Dispose()
    {
    }
}
