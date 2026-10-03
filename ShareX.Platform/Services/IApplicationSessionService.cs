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

/// <summary>The end of the user's session, as the operating system announces it.</summary>
/// <param name="Restarting">True when the system will start ShareX again afterwards (Windows restart manager).</param>
public sealed class SessionEndingEventArgs(bool restarting) : EventArgs
{
    public bool Restarting { get; } = restarting;
}

/// <summary>Session end and restart registration for ShareX itself.</summary>
/// <remarks>
/// On Linux and macOS the session ends with SIGTERM, which the application handles directly, so <see cref="SessionEnding"/> is not
/// raised there.
/// </remarks>
public interface IApplicationSessionService
{
    /// <summary>Raised on the UI thread before the session ends. Handlers save state quickly; ShareX is then closed.</summary>
    event EventHandler<SessionEndingEventArgs>? SessionEnding;

    /// <summary>Whether <see cref="RegisterRestart"/> works (RegisterApplicationRestart on Windows).</summary>
    FeatureSupport RestartSupport { get; }

    /// <summary>Asks the system to start ShareX with <paramref name="arguments"/> when it restarts the session.</summary>
    void RegisterRestart(string arguments);
}

public sealed class UnsupportedApplicationSessionService(string reason) : IApplicationSessionService
{
    public event EventHandler<SessionEndingEventArgs>? SessionEnding
    {
        add { }
        remove { }
    }

    public FeatureSupport RestartSupport { get; } = FeatureSupport.NotSupported(reason);

    public void RegisterRestart(string arguments)
    {
    }
}
