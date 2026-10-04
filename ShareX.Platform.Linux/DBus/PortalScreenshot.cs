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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace ShareX.Platform.Linux.DBus;

/// <summary>org.freedesktop.portal.Screenshot, the supported way to capture the screen on GNOME and KDE Wayland sessions and inside Flatpak.</summary>
internal static class PortalScreenshot
{
    public const string Interface = "org.freedesktop.portal.Screenshot";

    /// <summary>Takes a screenshot and returns the PNG file content. The portal writes the file, which is deleted afterwards when it is in a temporary location.</summary>
    /// <param name="interactive">Let the user pick the area or window in the desktop's own dialog.</param>
    public static async Task<byte[]> CaptureAsync(bool interactive, CancellationToken cancellationToken)
    {
        // A portal backend that never answers must not hold the capture forever. Allow for a first-use permission dialog, and
        // longer when the user is picking an area.
        using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(interactive ? TimeSpan.FromMinutes(5) : TimeSpan.FromMinutes(1));

        try
        {
            return await CaptureCoreAsync(interactive, timeout.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (timeout.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("The desktop's screenshot service did not answer.");
        }
    }

    private static async Task<byte[]> CaptureCoreAsync(bool interactive, CancellationToken cancellationToken)
    {
        PortalResponse response = await DBusSession.CallPortalRequestAsync((bus, token) =>
            DBusSession.CreateMethodCall(bus, DBusSession.PortalBusName, DBusSession.PortalObjectPath, Interface, "Screenshot", "sa{sv}",
                (ref MessageWriter writer) =>
                {
                    writer.WriteString("");
                    writer.WriteDictionary(new Dictionary<string, VariantValue>
                    {
                        ["handle_token"] = token,
                        ["modal"] = true,
                        ["interactive"] = interactive
                    });
                }), cancellationToken).ConfigureAwait(false);

        if (response.Cancelled)
        {
            throw new OperationCanceledException("The screenshot was cancelled.");
        }

        if (!response.Success || !response.Results.TryGetValue("uri", out VariantValue uriValue))
        {
            throw new InvalidOperationException($"The screenshot portal failed with response {response.Code}.");
        }

        Uri uri = new Uri(uriValue.GetString());

        if (!uri.IsFile)
        {
            throw new InvalidOperationException($"The screenshot portal returned an unexpected location: {uri}");
        }

        string path = uri.LocalPath;
        byte[] png = await File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false);
        TryDeleteTemporary(path);
        return png;
    }

    private static void TryDeleteTemporary(string path)
    {
        // GNOME saves to ~/Pictures/Screenshots (the user may want it), KDE and wlroots backends to /tmp.
        string temp = Path.GetTempPath();

        if (path.StartsWith(temp, StringComparison.Ordinal) || path.StartsWith("/tmp/", StringComparison.Ordinal))
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
            }
        }
    }
}
