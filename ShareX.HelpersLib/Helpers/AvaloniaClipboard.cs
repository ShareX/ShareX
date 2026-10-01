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

using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;
using ShareX.AvaloniaUI.Integration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;

namespace ShareX.HelpersLib;

internal static class AvaloniaClipboard
{
    private static IClipboard Clipboard => DesktopServices.GetWindow().Clipboard ??
        throw new PlatformNotSupportedException("Clipboard access is unavailable.");

    public static bool Clear() => DesktopServices.Run(async () => { await Clipboard.ClearAsync(); return true; });
    public static string GetText() => DesktopServices.Run(() => Clipboard.TryGetTextAsync());
    public static bool SetText(string text) => DesktopServices.Run(async () =>
    {
        await Clipboard.SetTextAsync(text);
        await Clipboard.FlushAsync();
        return true;
    });

    public static string[] GetFiles() => DesktopServices.Run(async () =>
        (await Clipboard.TryGetFilesAsync())?.Select(x => x.TryGetLocalPath()).Where(x => !string.IsNullOrEmpty(x)).ToArray());

    public static bool SetFiles(string[] paths) => DesktopServices.Run(async () =>
    {
        IStorageProvider storage = DesktopServices.GetWindow().StorageProvider;
        List<IStorageItem> files = new();
        foreach (string path in paths)
        {
            IStorageItem file = Directory.Exists(path)
                ? await storage.TryGetFolderFromPathAsync(path) : await storage.TryGetFileFromPathAsync(path);
            if (file != null) files.Add(file);
        }
        if (files.Count == 0) return false;
        await Clipboard.SetFilesAsync(files);
        await Clipboard.FlushAsync();
        return true;
    });

    public static bool SetImage(byte[] png, byte[] dib, string html) => DesktopServices.Run(async () =>
    {
        OwnedImageTransfer owned = CreateImageTransfer(png, dib, html);
        try { await Clipboard.SetDataAsync(owned); }
        catch { owned.Dispose(); throw; }
        // Ownership has passed to Avalonia even if persisting the clipboard fails.
        await Clipboard.FlushAsync();
        return true;
    });

    private static OwnedImageTransfer CreateImageTransfer(byte[] png, byte[] dib, string html)
    {
        using MemoryStream stream = new(png, writable: false);
        AvaloniaBitmap bitmap = new(stream);
        DataTransfer transfer = new();
        try
        {
            DataTransferItem item = new();
            item.SetBitmap(bitmap);
            item.Set(DataFormat.CreateBytesPlatformFormat(OperatingSystem.IsWindows() ? "PNG" : "image/png"), png);
            if (OperatingSystem.IsWindows())
                item.Set(DataFormat.CreateBytesPlatformFormat("CF_DIB"), dib);
            if (!string.IsNullOrEmpty(html))
                item.Set(DataFormat.CreateStringPlatformFormat(OperatingSystem.IsWindows() ? ClipboardDataFormats.Html : "text/html"), html);
            transfer.Add(item);
            return new OwnedImageTransfer(transfer, bitmap);
        }
        catch { ((IDisposable)transfer).Dispose(); bitmap.Dispose(); throw; }
    }

    public static bool Contains(string kind) => DesktopServices.Run(async () =>
    {
        IReadOnlyList<DataFormat> formats = await Clipboard.GetDataFormatsAsync();
        return kind switch
        {
            ClipboardDataFormats.Text => formats.Contains(DataFormat.Text),
            ClipboardDataFormats.FileDrop => formats.Contains(DataFormat.File),
            ClipboardDataFormats.Bitmap => formats.Contains(DataFormat.Bitmap) ||
                formats.Any(x => x.Identifier is "PNG" or "image/png" or "CF_DIB" or "CF_DIBV5"),
            _ => formats.Any(x => x.Identifier.Equals(kind, StringComparison.OrdinalIgnoreCase))
        };
    });

    public static byte[] GetImage() => DesktopServices.Run(async () =>
    {
        byte[] png = await Clipboard.TryGetValueAsync(DataFormat.CreateBytesPlatformFormat(OperatingSystem.IsWindows() ? "PNG" : "image/png"));
        if (png != null) return png;
        using AvaloniaBitmap bitmap = await Clipboard.TryGetBitmapAsync();
        if (bitmap == null) return null;
        using MemoryStream stream = new();
        bitmap.Save(stream, Avalonia.Media.Imaging.PngBitmapEncoderOptions.Default);
        return stream.ToArray();
    });

    public static ClipboardData Capture() => DesktopServices.Run(async () =>
    {
        ClipboardData snapshot = new();
        using IAsyncDataTransfer transfer = await Clipboard.TryGetDataAsync();
        if (transfer == null) return snapshot;
        foreach (DataFormat format in transfer.Formats)
        {
            string name = format == DataFormat.Text ? ClipboardDataFormats.UnicodeText :
                format == DataFormat.Bitmap ? ClipboardDataFormats.Bitmap :
                format == DataFormat.File ? ClipboardDataFormats.FileDrop : format.Identifier switch
                {
                    "CF_DIB" => ClipboardDataFormats.Dib,
                    "CF_DIBV5" => "Format17",
                    _ => format.Identifier
                };
            try
            {
                if (format == DataFormat.File)
                {
                    string[] paths = (await transfer.TryGetFilesAsync())?.Select(x => x.TryGetLocalPath())
                        .Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    if (paths != null) snapshot.SetData(name, paths);
                }
                else
                {
                    object value = await transfer.Items.First(x => x.Formats.Contains(format)).TryGetRawAsync(format);
                    if (value is AvaloniaBitmap bitmap)
                    {
                        using (bitmap)
                        using (MemoryStream stream = new())
                        {
                            bitmap.Save(stream, Avalonia.Media.Imaging.PngBitmapEncoderOptions.Default);
                            snapshot.SetData(name, stream.ToArray());
                        }
                    }
                    else if (value != null) snapshot.SetData(name, value);
                }
            }
            catch (Exception exception) { DebugHelper.WriteException(exception, $"Unable to read clipboard format {name}."); }
        }
        return snapshot;
    });

    // Avalonia owns a posted transfer until the clipboard changes. Keep its bitmap
    // alive for lazy requests (X11/Wayland), and release it with the transfer.
    private sealed class OwnedImageTransfer(DataTransfer transfer, AvaloniaBitmap bitmap) : IAsyncDataTransfer, IDataTransfer
    {
        public IReadOnlyList<DataFormat> Formats => transfer.Formats;
        IReadOnlyList<IAsyncDataTransferItem> IAsyncDataTransfer.Items => transfer.Items;
        IReadOnlyList<IDataTransferItem> IDataTransfer.Items => transfer.Items;
        private bool disposed;
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            ((IDisposable)transfer).Dispose();
            bitmap.Dispose();
        }
    }
}
