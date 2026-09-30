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

using ShareX.Desktop.Commands;
using System;
using System.IO;
using Xunit;

namespace ShareX.Desktop.Tests;

public class CommandLineParserTests
{
    private static DesktopCommand Ok(params string[] args)
    {
        ParseResult result = CommandLineParser.Parse(args);
        Assert.True(result.IsSuccess, result.Error);
        return result.Command!;
    }

    [Fact]
    public void NoArguments_RunsTheTrayApplication() => Assert.Equal(CommandKind.Run, Ok().Kind);

    [Theory]
    [InlineData("region", CaptureTarget.Region)]
    [InlineData("rectangle", CaptureTarget.Region)]
    [InlineData("fullscreen", CaptureTarget.FullScreen)]
    [InlineData("FULL", CaptureTarget.FullScreen)]
    [InlineData("screen", CaptureTarget.Screen)]
    public void Capture_ParsesTheTarget(string name, CaptureTarget expected)
    {
        DesktopCommand command = Ok("capture", name);

        Assert.Equal(CommandKind.Capture, command.Kind);
        Assert.Equal(expected, command.Target);
        Assert.True(command.NeedsRunningInstance);
    }

    [Fact]
    public void Capture_ParsesOverridesAndLeavesTheRestUnset()
    {
        DesktopCommand command = Ok("capture", "region", "--upload", "--no-save", "--edit");

        Assert.True(command.AfterCapture.Upload);
        Assert.False(command.AfterCapture.Save);
        Assert.True(command.AfterCapture.Edit);
        Assert.Null(command.AfterCapture.CopyImage);
        Assert.Null(command.AfterCapture.Notify);
    }

    [Theory]
    [InlineData("capture")]
    [InlineData("capture", "window")]
    [InlineData("capture", "region", "--bogus")]
    [InlineData("upload")]
    [InlineData("quit", "now")]
    [InlineData("--bogus")]
    public void InvalidCommandLines_AreRejectedWithAMessage(params string[] args)
    {
        ParseResult result = CommandLineParser.Parse(args);

        Assert.False(result.IsSuccess);
        Assert.False(string.IsNullOrWhiteSpace(result.Error));
    }

    [Fact]
    public void Upload_CollectsFiles()
    {
        DesktopCommand command = Ok("upload", "a.png", "b.txt");

        Assert.Equal(new[] { "a.png", "b.txt" }, command.Files);
    }

    [Fact]
    public void ABareFile_OpensTheEditor_LikeOpenWithInAFileManager()
    {
        DesktopCommand command = Ok("/tmp/shot.png");

        Assert.Equal(CommandKind.Editor, command.Kind);
        Assert.Equal(new[] { "/tmp/shot.png" }, command.Files);
    }

    [Theory]
    [InlineData("doctor", CommandKind.Doctor, false)]
    [InlineData("hotkeys", CommandKind.Hotkeys, false)]
    [InlineData("config", CommandKind.Config, false)]
    [InlineData("history", CommandKind.History, true)]
    [InlineData("--help", CommandKind.Help, false)]
    [InlineData("-v", CommandKind.Version, false)]
    [InlineData("quit", CommandKind.Quit, true)]
    [InlineData("status", CommandKind.Status, true)]
    [InlineData("editor", CommandKind.Editor, true)]
    public void SimpleCommands(string arg, CommandKind kind, bool needsInstance)
    {
        DesktopCommand command = Ok(arg);

        Assert.Equal(kind, command.Kind);
        Assert.Equal(needsInstance, command.NeedsRunningInstance);
    }

    [Fact]
    public void FilePaths_AreMadeAbsoluteBeforeTheyReachTheRunningInstance()
    {
        DesktopCommand command = Program.WithAbsolutePaths(CommandLineParser.Parse(["upload", "shot.png", "/abs/b.txt"]).Command!);

        Assert.Equal(Path.Combine(Environment.CurrentDirectory, "shot.png"), command.Files[0]);
        Assert.Equal(Path.GetFullPath("/abs/b.txt"), command.Files[1]);
    }

    [Fact]
    public void CommandsWithoutFiles_AreUnchanged()
    {
        DesktopCommand command = new DesktopCommand(CommandKind.Status);

        Assert.Same(command, Program.WithAbsolutePaths(command));
    }
}
