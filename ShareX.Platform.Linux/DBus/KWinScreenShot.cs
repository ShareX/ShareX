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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using ShareX.Platform.Imaging;
using Tmds.DBus.Protocol;

namespace ShareX.Platform.Linux.DBus;

/// <summary>
/// KWin's org.kde.KWin.ScreenShot2, which KDE Plasma's own Spectacle uses. Unlike the portal it captures without a dialog, but
/// only for applications whose installed desktop entry lists the interface in X-KDE-DBUS-Restricted-Interfaces. KWin finds that
/// entry by comparing its Exec program with the caller's executable, so it works for installed ShareX, not for "dotnet ShareX.dll".
/// </summary>
internal static class KWinScreenShot
{
    public const string BusName = "org.kde.KWin.ScreenShot2";
    public const string ObjectPath = "/org/kde/KWin/ScreenShot2";
    public const string DesktopEntryKey = "X-KDE-DBUS-Restricted-Interfaces";

    // Set after KWin refused once (no authorised desktop entry, or no KWin), so later captures go straight to the portal.
    private static volatile bool unavailable;

    public static bool IsAvailable => !unavailable;

    /// <summary>Captures the whole desktop in device pixels. Returns null when KWin is missing or refuses; the caller then uses the portal.</summary>
    public static async Task<PixelBuffer?> TryCaptureWorkspaceAsync(bool includeCursor, CancellationToken cancellationToken)
    {
        if (unavailable)
        {
            return null;
        }

        try
        {
            DBusConnection bus = await DBusSession.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            return await CaptureWorkspaceAsync(bus, includeCursor, cancellationToken).ConfigureAwait(false);
        }
        catch (DBusErrorReplyException e) when (e.ErrorName != "org.kde.KWin.ScreenShot2.Error.Cancelled")
        {
            // org.kde.KWin.ScreenShot2.Error.NoAuthorized (no desktop entry allows ShareX), ServiceUnknown or UnknownMethod (an
            // older KWin or another compositor). None of these change during the session.
            unavailable = true;
            return null;
        }
        catch (Exception e) when (e is InvalidDataException or TimeoutException or IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    internal static async Task<PixelBuffer> CaptureWorkspaceAsync(DBusConnection bus, bool includeCursor, CancellationToken cancellationToken)
    {
        string directory = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR") is { Length: > 0 } runtime ? runtime : Path.GetTempPath();
        string path = Path.Combine(directory, "sharex-kwin-" + Guid.NewGuid().ToString("N") + ".raw");

        try
        {
            KWinImage image;

            // KWin writes the pixels to the file descriptor it is given. The message may own the handle it carries, so this handle
            // is used only for that and the file is read back separately.
            using (SafeFileHandle target = new FileStream(path, new FileStreamOptions
                {
                    Mode = FileMode.CreateNew,
                    Access = FileAccess.Write,
                    Share = FileShare.ReadWrite,
                    UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite
                }).SafeFileHandle)
            {
                MessageBuffer call = DBusSession.CreateMethodCall(bus, BusName, ObjectPath, BusName, "CaptureWorkspace", "a{sv}h",
                    (ref MessageWriter writer) =>
                    {
                        writer.WriteDictionary(new Dictionary<string, VariantValue>
                        {
                            ["include-cursor"] = includeCursor,
                            ["native-resolution"] = true
                        });
                        writer.WriteHandle(target);
                    });

                image = await bus.CallMethodAsync(call, static (Message message, object? state) =>
                    KWinImage.FromResults(message.GetBodyReader().ReadDictionaryOfStringToVariantValue()), null).WaitAsync(TimeSpan.FromSeconds(10), cancellationToken)
                    .ConfigureAwait(false);
            }

            long length = (long)image.Stride * image.Height;
            byte[] data = await ReadWhenWrittenAsync(path, length, TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
            return DecodeRaw(data, image);
        }
        finally
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

    // KWin answers before its worker thread has finished writing.
    private static async Task<byte[]> ReadWhenWrittenAsync(string path, long length, TimeSpan timeout, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        while (new FileInfo(path).Length < length)
        {
            if (stopwatch.Elapsed > timeout)
            {
                throw new TimeoutException("KWin did not finish writing the screenshot.");
            }

            await Task.Delay(20, cancellationToken).ConfigureAwait(false);
        }

        return await File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false);
    }

    // QImage::Format values KWin uses. On little endian machines the 32 bit formats are BGRA in memory, the 8888 ones RGBA.
    private const uint FormatRgb32 = 4, FormatArgb32 = 5, FormatArgb32Premultiplied = 6;
    private const uint FormatRgbx8888 = 16, FormatRgba8888 = 17, FormatRgba8888Premultiplied = 18;

    internal static PixelBuffer DecodeRaw(byte[] data, KWinImage image)
    {
        bool bgra = image.Format is FormatRgb32 or FormatArgb32 or FormatArgb32Premultiplied;
        bool rgba = image.Format is FormatRgbx8888 or FormatRgba8888 or FormatRgba8888Premultiplied;

        if ((!bgra && !rgba) || image.Width == 0 || image.Height == 0 || image.Stride < image.Width * 4 ||
            data.LongLength < (long)image.Stride * image.Height)
        {
            throw new InvalidDataException($"KWin returned an image this version of ShareX cannot read (format {image.Format}).");
        }

        PixelBuffer buffer = new PixelBuffer((int)image.Width, (int)image.Height);
        int rowBytes = buffer.Stride;

        for (int y = 0; y < buffer.Height; y++)
        {
            Buffer.BlockCopy(data, (int)(y * image.Stride), buffer.Pixels, y * rowBytes, rowBytes);
        }

        if (rgba)
        {
            byte[] pixels = buffer.Pixels;

            for (int i = 0; i < pixels.Length; i += 4)
            {
                (pixels[i], pixels[i + 2]) = (pixels[i + 2], pixels[i]);
            }
        }

        // A screenshot of the desktop is opaque; this also undoes premultiplication, which only differs where alpha is below 255.
        buffer.MakeOpaque();
        return buffer;
    }
}

/// <summary>The description KWin returns for the pixels it writes ("type" is "raw").</summary>
internal readonly record struct KWinImage(uint Width, uint Height, uint Stride, uint Format)
{
    public static KWinImage FromResults(Dictionary<string, VariantValue> results)
    {
        if (!results.TryGetValue("type", out VariantValue type) || type.GetString() != "raw")
        {
            throw new InvalidDataException("KWin returned a screenshot in an unknown form.");
        }

        return new KWinImage(GetUInt32(results, "width"), GetUInt32(results, "height"), GetUInt32(results, "stride"), GetUInt32(results, "format"));
    }

    private static uint GetUInt32(Dictionary<string, VariantValue> results, string key) =>
        results.TryGetValue(key, out VariantValue value) ? value.GetUInt32() : throw new InvalidDataException($"KWin's screenshot has no {key}.");
}
