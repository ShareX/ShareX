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

using ShareX.Platform;
using System.Collections.Generic;

namespace ShareX.Desktop.Commands;

public enum CommandKind
{
    /// <summary>Start the tray application, or do nothing when it is already running.</summary>
    Run,
    Capture,
    Upload,
    Editor,
    Quit,
    Status,
    /// <summary>Print what works on this system. Runs locally, without the tray application.</summary>
    Doctor,
    /// <summary>Print compositor key bindings for the capture commands. Runs locally.</summary>
    Hotkeys,
    Help,
    Version
}

public enum CaptureTarget
{
    Region,
    FullScreen,
    Screen
}

/// <summary>What happens after a capture. Null keeps the value from the settings file.</summary>
public sealed record AfterCaptureOverrides
{
    public bool? Save { get; init; }

    public bool? CopyImage { get; init; }

    public bool? Upload { get; init; }

    public bool? Edit { get; init; }

    public bool? Notify { get; init; }
}

/// <summary>One request to the application, from the command line or from another instance.</summary>
public sealed record DesktopCommand(CommandKind Kind)
{
    public CaptureTarget Target { get; init; }

    public IReadOnlyList<string> Files { get; init; } = [];

    public AfterCaptureOverrides AfterCapture { get; init; } = new AfterCaptureOverrides();

    /// <summary>True for commands the running application answers. The others are handled by the process that was started.</summary>
    public bool NeedsRunningInstance => Kind is CommandKind.Capture or CommandKind.Upload or CommandKind.Editor or CommandKind.Quit or CommandKind.Status;
}
