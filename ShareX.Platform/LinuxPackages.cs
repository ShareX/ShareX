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
using System.Linq;

namespace ShareX.Platform;

/// <summary>Helper programs ShareX can use on Linux.</summary>
public enum LinuxTool
{
    /// <summary>wl-copy and wl-paste.</summary>
    WlClipboard,
    Xclip,
    Xsel,
    /// <summary>notify-send.</summary>
    Libnotify,
    Grim,
    Slurp,
    FFmpeg,
    /// <summary>xdg-open.</summary>
    XdgUtils,
    /// <summary>xdg-desktop-portal plus a backend for the current desktop.</summary>
    XdgDesktopPortal,
    WfRecorder,
    /// <summary>Tesseract OCR with at least one language's data.</summary>
    Tesseract,
    /// <summary>The CUPS lp, lpstat and lpoptions commands.</summary>
    Cups,
    /// <summary>pw-play, PipeWire's sound player.</summary>
    PipeWire,
    /// <summary>GStreamer's PipeWire plugin (pipewiresrc), for recording through the ScreenCast portal.</summary>
    GStreamerPipeWire
}

/// <summary>Maps optional helper programs to package names for each distribution family.</summary>
public static class LinuxPackages
{
    private static readonly Dictionary<LinuxTool, string> Commands = new Dictionary<LinuxTool, string>
    {
        [LinuxTool.WlClipboard] = "wl-copy",
        [LinuxTool.Xclip] = "xclip",
        [LinuxTool.Xsel] = "xsel",
        [LinuxTool.Libnotify] = "notify-send",
        [LinuxTool.Grim] = "grim",
        [LinuxTool.Slurp] = "slurp",
        [LinuxTool.FFmpeg] = "ffmpeg",
        [LinuxTool.XdgUtils] = "xdg-open",
        [LinuxTool.XdgDesktopPortal] = "xdg-desktop-portal",
        [LinuxTool.WfRecorder] = "wf-recorder",
        [LinuxTool.Tesseract] = "tesseract",
        [LinuxTool.Cups] = "lp",
        [LinuxTool.PipeWire] = "pw-play",
        [LinuxTool.GStreamerPipeWire] = "gst-launch-1.0"
    };

    /// <summary>The executable name used to check whether the tool is installed.</summary>
    public static string GetCommand(LinuxTool tool) => Commands[tool];

    public static string GetPackageName(LinuxTool tool, LinuxDistributionFamily family)
    {
        return (tool, family) switch
        {
            (LinuxTool.WlClipboard, _) => "wl-clipboard",
            (LinuxTool.Libnotify, LinuxDistributionFamily.Debian) => "libnotify-bin",
            (LinuxTool.Libnotify, LinuxDistributionFamily.Arch) => "libnotify",
            (LinuxTool.Libnotify, LinuxDistributionFamily.OpenSuse) => "libnotify-tools",
            (LinuxTool.Libnotify, LinuxDistributionFamily.NixOS) => "libnotify",
            (LinuxTool.Libnotify, _) => "libnotify",
            (LinuxTool.FFmpeg, LinuxDistributionFamily.Fedora) => "ffmpeg-free",
            (LinuxTool.XdgUtils, _) => "xdg-utils",
            (LinuxTool.XdgDesktopPortal, _) => "xdg-desktop-portal",
            (LinuxTool.WfRecorder, _) => "wf-recorder",
            (LinuxTool.Tesseract, LinuxDistributionFamily.Debian) => "tesseract-ocr",
            (LinuxTool.Tesseract, LinuxDistributionFamily.OpenSuse) => "tesseract-ocr",
            (LinuxTool.Tesseract, LinuxDistributionFamily.Arch) => "tesseract tesseract-data-eng",
            (LinuxTool.Cups, LinuxDistributionFamily.Debian) => "cups-client",
            (LinuxTool.Cups, LinuxDistributionFamily.Fedora) => "cups-client",
            (LinuxTool.Cups, LinuxDistributionFamily.OpenSuse) => "cups-client",
            (LinuxTool.Cups, _) => "cups",
            (LinuxTool.PipeWire, LinuxDistributionFamily.Debian) => "pipewire-bin",
            (LinuxTool.PipeWire, LinuxDistributionFamily.Fedora) => "pipewire-utils",
            (LinuxTool.PipeWire, _) => "pipewire",
            (LinuxTool.GStreamerPipeWire, LinuxDistributionFamily.Debian) => "gstreamer1.0-pipewire",
            (LinuxTool.GStreamerPipeWire, LinuxDistributionFamily.Fedora) => "pipewire-gstreamer",
            (LinuxTool.GStreamerPipeWire, LinuxDistributionFamily.OpenSuse) => "gstreamer-plugin-pipewire",
            (LinuxTool.GStreamerPipeWire, _) => "gst-plugin-pipewire",
            _ => Commands[tool]
        };
    }

    /// <summary>
    /// A user facing reason for <see cref="FeatureSupport"/> that names the missing package for this distribution. It never
    /// contains an install command: the dependency rules in AGENTS.md leave installing optional programs to the user.
    /// </summary>
    public static FeatureSupport Missing(LinuxDistribution distribution, params LinuxTool[] tools)
    {
        string names = string.Join(" or ", tools.Select(tool => GetPackageName(tool, distribution.Family)).Distinct());
        return FeatureSupport.NotSupported($"This needs {names}, which was not found on this system.");
    }
}
