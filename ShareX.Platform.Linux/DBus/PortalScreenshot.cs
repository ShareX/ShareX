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
