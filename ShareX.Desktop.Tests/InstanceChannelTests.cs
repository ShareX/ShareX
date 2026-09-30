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
using ShareX.Desktop.Ipc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ShareX.Desktop.Tests;

public class InstanceChannelTests
{
    private static string NewPipeName() => "sharex-test-" + Guid.NewGuid().ToString("N");

    [Fact]
    public async Task Command_TravelsToTheInstance_AndTheAnswerComesBack()
    {
        string pipe = NewPipeName();
        List<DesktopCommand> received = new List<DesktopCommand>();

        await using InstanceServer server = InstanceChannel.TryStartServer(pipe, (command, _) =>
        {
            received.Add(command);
            return Task.FromResult(new CommandResponse(true, "done " + command.Kind));
        })!;

        DesktopCommand sent = new DesktopCommand(CommandKind.Capture)
        {
            Target = CaptureTarget.FullScreen,
            Files = ["a.png"],
            AfterCapture = new AfterCaptureOverrides { Upload = true, Save = false }
        };
        CommandResponse? response = await InstanceChannel.TrySendAsync(pipe, sent, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

        Assert.NotNull(response);
        Assert.True(response!.Ok);
        Assert.Equal("done Capture", response.Message);
        DesktopCommand got = Assert.Single(received);
        Assert.Equal(CaptureTarget.FullScreen, got.Target);
        Assert.Equal(new[] { "a.png" }, got.Files);
        Assert.True(got.AfterCapture.Upload);
        Assert.False(got.AfterCapture.Save);
        Assert.Null(got.AfterCapture.CopyImage);
    }

    [Fact]
    public async Task OnlyOneInstanceCanOwnThePipe()
    {
        string pipe = NewPipeName();

        await using InstanceServer first = InstanceChannel.TryStartServer(pipe, (_, _) => Task.FromResult(new CommandResponse(true, "x")))!;

        Assert.Null(InstanceChannel.TryStartServer(pipe, (_, _) => Task.FromResult(new CommandResponse(true, "y"))));
    }

    [Fact]
    public async Task NoInstance_MeansNull()
    {
        Assert.Null(await InstanceChannel.TrySendAsync(NewPipeName(), new DesktopCommand(CommandKind.Status), TimeSpan.FromMilliseconds(200), TimeSpan.FromSeconds(1)));
    }

    [Fact]
    public async Task AFailingHandler_ReportsTheErrorInsteadOfHangingTheClient()
    {
        string pipe = NewPipeName();

        await using InstanceServer server = InstanceChannel.TryStartServer(pipe, (_, _) => throw new InvalidOperationException("boom"))!;
        CommandResponse? response = await InstanceChannel.TrySendAsync(pipe, new DesktopCommand(CommandKind.Status), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

        Assert.False(response!.Ok);
        Assert.Equal("boom", response.Message);
    }

    [Fact]
    public async Task ASlowCommand_DoesNotBlockTheNextOne()
    {
        string pipe = NewPipeName();
        TaskCompletionSource release = new TaskCompletionSource();

        await using InstanceServer server = InstanceChannel.TryStartServer(pipe, async (command, _) =>
        {
            if (command.Kind == CommandKind.Upload)
            {
                await release.Task;
            }

            return new CommandResponse(true, command.Kind.ToString());
        })!;

        Task<CommandResponse?> slow = InstanceChannel.TrySendAsync(pipe, new DesktopCommand(CommandKind.Upload), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10));
        CommandResponse? quick = await InstanceChannel.TrySendAsync(pipe, new DesktopCommand(CommandKind.Status), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

        Assert.Equal("Status", quick!.Message);
        Assert.False(slow.IsCompleted);
        release.SetResult();
        Assert.Equal("Upload", (await slow)!.Message);
    }

    [Theory]
    [InlineData("mike", "mike")]
    [InlineData("DOMAIN\\user name", "DOMAIN_user_name")]
    public void PipeNames_AreSafeForEveryOs(string user, string expected) => Assert.Equal(expected, InstanceChannel.Sanitize(user));
}
