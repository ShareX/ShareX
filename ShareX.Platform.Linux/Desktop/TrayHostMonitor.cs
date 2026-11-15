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

namespace ShareX.Platform.Linux.Desktop;

/// <summary>
/// Whether a tray host (a StatusNotifierWatcher) is there for ShareX's icon, ignoring the moments a desktop takes it away.
/// </summary>
/// <remarks>
/// GNOME switches extensions off while the screen is locked, the AppIndicator extension that hosts tray icons among them, and
/// switches them on again after unlocking. ShareX shows its main window when the tray host goes away, so that it stays
/// reachable, and so it opened its window at every unlock. A host that was there counts as there while the screen is locked and
/// for <see cref="grace"/> afterwards.
/// </remarks>
internal sealed class TrayHostMonitor
{
    private readonly Func<bool> hostPresent;
    private readonly Func<bool> screenLocked;
    private readonly Func<DateTime> now;
    private readonly TimeSpan grace;
    private readonly object syncRoot = new object();
    private bool hadHost;
    private DateTime? missingSince;

    public TrayHostMonitor(Func<bool> hostPresent, Func<bool> screenLocked, Func<DateTime>? now = null, TimeSpan? grace = null)
    {
        this.hostPresent = hostPresent;
        this.screenLocked = screenLocked;
        this.now = now ?? (() => DateTime.UtcNow);
        this.grace = grace ?? TimeSpan.FromSeconds(20);
    }

    public bool IsHostAvailable()
    {
        lock (syncRoot)
        {
            if (hostPresent())
            {
                hadHost = true;
                missingSince = null;
                return true;
            }

            if (!hadHost)
            {
                return false;
            }

            DateTime time = now();

            if (screenLocked())
            {
                // The wait starts when the screen is unlocked.
                missingSince = time;
                return true;
            }

            missingSince ??= time;

            if (time - missingSince.Value < grace)
            {
                return true;
            }

            hadHost = false;
            missingSince = null;
            return false;
        }
    }
}
