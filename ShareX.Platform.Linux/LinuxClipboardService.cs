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

using ShareX.Platform.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Linux;

/// <summary>Clipboard access through wl-clipboard on Wayland and xclip or xsel on X11.</summary>
/// <remarks>
/// GNOME's Mutter does not implement the data control protocol, so wl-copy briefly maps a surface to take focus there.
/// When a ShareX window is open, the UI toolkit's clipboard is preferable.
/// </remarks>
public sealed class LinuxClipboardService : IClipboardService
{
    internal enum Backend
    {
        None,
        WlClipboard,
        Xclip,
        Xsel
    }

    private const string UriListType = "text/uri-list";
    private const string PngType = "image/png";

    private readonly ICommandRunner runner;
    private readonly LinuxDistribution distribution;

    public LinuxClipboardService(PlatformInfo info, ICommandRunner runner)
    {
        this.runner = runner;
        distribution = info.Distribution ?? LinuxDistribution.Unknown;
        ActiveBackend = SelectBackend(info, runner);
    }

    internal Backend ActiveBackend { get; }

    public FeatureSupport Support => ActiveBackend switch
    {
        Backend.None => LinuxPackages.Missing(distribution, LinuxTool.WlClipboard, LinuxTool.Xclip),
        _ => FeatureSupport.Supported
    };

    internal static Backend SelectBackend(PlatformInfo info, ICommandRunner runner)
    {
        if (info.IsWayland && runner.Exists("wl-copy") && runner.Exists("wl-paste"))
        {
            return Backend.WlClipboard;
        }

        // Also used on Wayland through XWayland when wl-clipboard is missing.
        if (runner.Exists("xclip"))
        {
            return Backend.Xclip;
        }

        if (runner.Exists("xsel"))
        {
            return Backend.Xsel;
        }

        return Backend.None;
    }

    public Task<bool> SetTextAsync(string text, CancellationToken cancellationToken = default) =>
        SetAsync("text/plain;charset=utf-8", Encoding.UTF8.GetBytes(text), cancellationToken);

    public async Task<string?> GetTextAsync(CancellationToken cancellationToken = default)
    {
        byte[]? data = await GetAsync(null, cancellationToken).ConfigureAwait(false);
        return data != null ? Encoding.UTF8.GetString(data) : null;
    }

    public Task<bool> SetImageAsync(byte[] png, CancellationToken cancellationToken = default) => SetAsync(PngType, png, cancellationToken);

    public async Task<byte[]?> GetImageAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<string> types = await GetTypesAsync(cancellationToken).ConfigureAwait(false);

        if (!types.Contains(PngType))
        {
            return null;
        }

        byte[]? data = await GetAsync(PngType, cancellationToken).ConfigureAwait(false);
        return data != null && Imaging.PngCodec.IsPng(data) ? data : null;
    }

    public Task<bool> SetFilesAsync(IReadOnlyList<string> paths, CancellationToken cancellationToken = default)
    {
        string uriList = string.Concat(paths.Select(path => new Uri(path).AbsoluteUri + "\r\n"));
        return SetAsync(UriListType, Encoding.UTF8.GetBytes(uriList), cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetFilesAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<string> types = await GetTypesAsync(cancellationToken).ConfigureAwait(false);

        if (!types.Contains(UriListType))
        {
            return Array.Empty<string>();
        }

        byte[]? data = await GetAsync(UriListType, cancellationToken).ConfigureAwait(false);
        return data != null ? ParseUriList(Encoding.UTF8.GetString(data)) : Array.Empty<string>();
    }

    public async Task<bool> ClearAsync(CancellationToken cancellationToken = default)
    {
        switch (ActiveBackend)
        {
            case Backend.WlClipboard:
                return (await runner.RunAsync("wl-copy", ["--clear"], cancellationToken: cancellationToken).ConfigureAwait(false)).Success;
            case Backend.Xsel:
                return (await runner.RunAsync("xsel", ["--clipboard", "--clear"], cancellationToken: cancellationToken).ConfigureAwait(false)).Success;
            case Backend.Xclip:
                return await SetAsync("text/plain", Array.Empty<byte>(), cancellationToken).ConfigureAwait(false);
            default:
                return false;
        }
    }

    internal static IReadOnlyList<string> ParseUriList(string uriList)
    {
        List<string> paths = new List<string>();

        foreach (string rawLine in uriList.Split('\n'))
        {
            string line = rawLine.Trim();

            // Lines starting with # are comments. GNOME's x-special format prefixes the list with "copy" or "cut".
            if (line.Length == 0 || line[0] == '#' || line is "copy" or "cut")
            {
                continue;
            }

            if (Uri.TryCreate(line, UriKind.Absolute, out Uri? uri) && uri.IsFile)
            {
                paths.Add(uri.LocalPath);
            }
        }

        return paths;
    }

    private async Task<bool> SetAsync(string mimeType, byte[] data, CancellationToken cancellationToken)
    {
        (string command, string[] arguments) = ActiveBackend switch
        {
            Backend.WlClipboard => ("wl-copy", new[] { "--type", mimeType }),
            Backend.Xclip => ("xclip", new[] { "-selection", "clipboard", "-target", mimeType.Split(';')[0], "-in" }),
            // xsel only handles text.
            Backend.Xsel when mimeType.StartsWith("text/plain", StringComparison.Ordinal) => ("xsel", new[] { "--clipboard", "--input" }),
            _ => ("", Array.Empty<string>())
        };

        if (command.Length == 0)
        {
            return false;
        }

        try
        {
            return await runner.RunForkingAsync(command, arguments, data, TimeSpan.FromSeconds(10), cancellationToken).ConfigureAwait(false) == 0;
        }
        catch (Exception e) when (e is TimeoutException or System.ComponentModel.Win32Exception)
        {
            return false;
        }
    }

    private async Task<byte[]?> GetAsync(string? mimeType, CancellationToken cancellationToken)
    {
        (string command, string[] arguments) = ActiveBackend switch
        {
            Backend.WlClipboard => ("wl-paste", mimeType != null ? new[] { "--no-newline", "--type", mimeType } : new[] { "--no-newline" }),
            Backend.Xclip => ("xclip", mimeType != null ? new[] { "-selection", "clipboard", "-target", mimeType, "-out" } : new[] { "-selection", "clipboard", "-out" }),
            Backend.Xsel when mimeType == null => ("xsel", new[] { "--clipboard", "--output" }),
            _ => ("", Array.Empty<string>())
        };

        if (command.Length == 0)
        {
            return null;
        }

        try
        {
            CommandResult result = await runner.RunAsync(command, arguments, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.Success ? result.StandardOutput : null;
        }
        catch (Exception e) when (e is TimeoutException or System.ComponentModel.Win32Exception)
        {
            return null;
        }
    }

    private async Task<IReadOnlyList<string>> GetTypesAsync(CancellationToken cancellationToken)
    {
        (string command, string[] arguments) = ActiveBackend switch
        {
            Backend.WlClipboard => ("wl-paste", new[] { "--list-types" }),
            Backend.Xclip => ("xclip", new[] { "-selection", "clipboard", "-target", "TARGETS", "-out" }),
            _ => ("", Array.Empty<string>())
        };

        if (command.Length == 0)
        {
            return Array.Empty<string>();
        }

        try
        {
            CommandResult result = await runner.RunAsync(command, arguments, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.Success
                ? result.StandardOutputText.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                : Array.Empty<string>();
        }
        catch (Exception e) when (e is TimeoutException or System.ComponentModel.Win32Exception)
        {
            return Array.Empty<string>();
        }
    }
}
