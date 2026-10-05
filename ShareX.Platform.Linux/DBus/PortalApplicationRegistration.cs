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
using System.IO;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace ShareX.Platform.Linux.DBus;

/// <summary>The host Registry is only for applications that the portal does not identify as a sandbox.</summary>
internal static class PortalApplicationRegistration
{
    internal static bool HasSandboxIdentity()
    {
        // Match the portal's process checks, rather than SNAP/FLATPAK_ID environment variables
        // which a host application can inherit from a terminal or IDE installed as a Snap.
        if (File.Exists("/.flatpak-info"))
        {
            return true;
        }

        try
        {
            return IsSnapCgroup(File.ReadAllText("/proc/self/cgroup"));
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            // If process metadata is unavailable, let the portal decide and handle its reply.
            return false;
        }
    }

    internal static bool IsSnapCgroup(string cgroups)
    {
        foreach (string line in cgroups.Split('\n'))
        {
            int first = line.IndexOf(':');
            int second = first < 0 ? -1 : line.IndexOf(':', first + 1);

            if (second < 0)
            {
                continue;
            }

            string controller = line[(first + 1)..second];

            if ((controller.Length == 0 || controller is "freezer" or "name=systemd") &&
                line[(second + 1)..].Contains("/snap.", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    internal static async Task<PortalRegistrationResult> RegisterAsync(Func<Task> register, bool hasSandboxIdentity)
    {
        if (hasSandboxIdentity)
        {
            return new PortalRegistrationResult(null, false);
        }

        try
        {
            await register().ConfigureAwait(false);
            return new PortalRegistrationResult(null, false);
        }
        catch (DBusErrorReplyException e) when (e.ErrorName is "org.freedesktop.DBus.Error.UnknownMethod" or
            "org.freedesktop.DBus.Error.UnknownInterface" or "org.freedesktop.DBus.Error.ServiceUnknown")
        {
            // Older portals have no host Registry.
            return new PortalRegistrationResult(null, false);
        }
        catch (DBusErrorReplyException e)
        {
            bool sandbox = (e.ErrorName is "org.freedesktop.portal.Error.Failed" or "org.freedesktop.portal.Error.NotAllowed") &&
                IsSandboxRegistrationError(e.ErrorMessage);

            // A sandbox rejection means the portal already knows how to identify this process.
            // Other errors (including a missing sharex.desktop) must still be reported.
            return new PortalRegistrationResult(sandbox ? null : e.ErrorMessage, sandbox);
        }
    }

    private static bool IsSandboxRegistrationError(string message)
    {
        const string prefix = "Could not register app ID: ";
        string reason = message.StartsWith(prefix, StringComparison.Ordinal) ? message[prefix.Length..] : message;
        return reason is "Can't manually register a io.snapcraft application" or "Can't manually register a Snap application" or
            "Can't manually register a org.flatpak application" or "Can't manually register a Flatpak application";
    }
}

internal readonly record struct PortalRegistrationResult(string? Error, bool UseNewConnection);
