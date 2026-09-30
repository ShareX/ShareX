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

namespace ShareX.Desktop.Commands;

public sealed record ParseResult(DesktopCommand? Command, string? Error)
{
    public bool IsSuccess => Command != null;
}

/// <summary>
/// sharex [capture region|fullscreen|screen] [upload FILE...] [editor [FILE]] [quit] [status] [doctor] [hotkeys] [--help] [--version]
/// Capture options: --upload, --no-upload, --edit, --no-save, --no-copy, --no-notify.
/// </summary>
public static class CommandLineParser
{
    public const string Usage = @"Usage: sharex [command] [options]

Commands:
  (none)                    Start the tray application
  capture region            Select an area and capture it
  capture fullscreen        Capture every screen
  capture screen            Capture the screen under the cursor
  upload FILE...            Upload files with the configured destinations
  editor [FILE]             Open the image editor
  status                    Show whether the application is running
  quit                      Stop the running application
  doctor                    Show what works on this system and what to install
  hotkeys                   Print key bindings for your compositor

Capture options:
  --upload / --no-upload    Upload the capture (default: from settings)
  --edit                    Open the capture in the editor
  --no-save                 Do not save the capture to a file
  --no-copy                 Do not copy the capture to the clipboard
  --no-notify               Do not show a notification

  -h, --help                Show this help
  -v, --version             Show the version
";

    public static ParseResult Parse(IReadOnlyList<string> args)
    {
        if (args.Count == 0)
        {
            return Ok(new DesktopCommand(CommandKind.Run));
        }

        string first = args[0];

        switch (first.ToLowerInvariant())
        {
            case "-h":
            case "--help":
            case "help":
                return Ok(new DesktopCommand(CommandKind.Help));
            case "-v":
            case "--version":
            case "version":
                return Ok(new DesktopCommand(CommandKind.Version));
            case "quit":
            case "exit":
                return NoArguments(args, CommandKind.Quit);
            case "status":
                return NoArguments(args, CommandKind.Status);
            case "doctor":
                return NoArguments(args, CommandKind.Doctor);
            case "hotkeys":
                return NoArguments(args, CommandKind.Hotkeys);
            case "capture":
                return ParseCapture(args);
            case "upload":
                return ParseFiles(args, CommandKind.Upload, required: true);
            case "editor":
            case "edit":
                return ParseFiles(args, CommandKind.Editor, required: false);
            default:
                // A bare file is what a file manager passes for "Open with", so treat it as an editor request.
                if (!first.StartsWith('-'))
                {
                    return ParseFiles(["editor", .. args], CommandKind.Editor, required: false);
                }

                return Fail($"Unknown option '{first}'.");
        }
    }

    private static ParseResult ParseCapture(IReadOnlyList<string> args)
    {
        if (args.Count < 2)
        {
            return Fail("capture needs a target: region, fullscreen or screen.");
        }

        CaptureTarget target;

        switch (args[1].ToLowerInvariant())
        {
            case "region":
            case "rectangle":
                target = CaptureTarget.Region;
                break;
            case "fullscreen":
            case "full":
            case "all":
                target = CaptureTarget.FullScreen;
                break;
            case "screen":
            case "monitor":
                target = CaptureTarget.Screen;
                break;
            default:
                return Fail($"Unknown capture target '{args[1]}'. Use region, fullscreen or screen.");
        }

        bool? save = null, copy = null, upload = null, edit = null, notify = null;

        for (int i = 2; i < args.Count; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--upload": upload = true; break;
                case "--no-upload": upload = false; break;
                case "--edit": edit = true; break;
                case "--no-save": save = false; break;
                case "--no-copy": copy = false; break;
                case "--no-notify": notify = false; break;
                default: return Fail($"Unknown capture option '{args[i]}'.");
            }
        }

        return Ok(new DesktopCommand(CommandKind.Capture)
        {
            Target = target,
            AfterCapture = new AfterCaptureOverrides { Save = save, CopyImage = copy, Upload = upload, Edit = edit, Notify = notify }
        });
    }

    private static ParseResult ParseFiles(IReadOnlyList<string> args, CommandKind kind, bool required)
    {
        List<string> files = new List<string>();

        for (int i = 1; i < args.Count; i++)
        {
            if (args[i].StartsWith('-') && args[i].Length > 1)
            {
                return Fail($"Unknown option '{args[i]}'.");
            }

            files.Add(args[i]);
        }

        if (required && files.Count == 0)
        {
            return Fail($"{args[0]} needs at least one file.");
        }

        return Ok(new DesktopCommand(kind) { Files = files });
    }

    private static ParseResult NoArguments(IReadOnlyList<string> args, CommandKind kind) =>
        args.Count > 1 ? Fail($"{args[0]} does not take arguments.") : Ok(new DesktopCommand(kind));

    private static ParseResult Ok(DesktopCommand command) => new ParseResult(command, null);

    private static ParseResult Fail(string error) => new ParseResult(null, error);
}
