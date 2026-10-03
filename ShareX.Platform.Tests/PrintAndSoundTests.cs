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
using ShareX.Platform.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ShareX.Platform.Tests;

public class CupsPrintTests : IDisposable
{
    private readonly string temp = Path.Combine(Path.GetTempPath(), "sharex-print-test-" + Guid.NewGuid().ToString("N"));

    public CupsPrintTests() => Directory.CreateDirectory(temp);

    public void Dispose() => Directory.Delete(temp, true);

    [Fact]
    public void PrintArgumentsNamePrinterCopiesAndTitle()
    {
        List<string> arguments = CupsPrintService.CreatePrintArguments(new PrintOptions { PrinterName = "Office", Copies = 2 }, "Screenshot", ["/tmp/a.png", "/tmp/b.png"]);

        Assert.Equal(["-d", "Office", "-n", "2", "-t", "Screenshot", "-o", "fit-to-page", "--", "/tmp/a.png", "/tmp/b.png"], arguments);
    }

    [Fact]
    public void DefaultPrinterIsNotNamed()
    {
        List<string> arguments = CupsPrintService.CreatePrintArguments(new PrintOptions(), "", ["page.png"]);

        Assert.DoesNotContain("-d", arguments);
        Assert.Equal("ShareX", arguments[arguments.IndexOf("-t") + 1]);
        Assert.Equal("1", arguments[arguments.IndexOf("-n") + 1]);
    }

    [Theory]
    [InlineData("copies=1 media=iso_a4_210x297mm sides=one-sided", 827)]
    [InlineData("media=Letter", 850)]
    [InlineData("PageSize=A4 job-sheets=none", 827)]
    [InlineData("copies=1", null)]
    [InlineData("", null)]
    public void MediaComesFromLpoptions(string output, int? width)
    {
        Assert.Equal(width, CupsPrintService.ParseMedia(output)?.Width);
    }

    [Theory]
    [InlineData("en_US.UTF-8", 850)]
    [InlineData("en_CA.UTF-8", 850)]
    [InlineData("en_AU.UTF-8", 827)]
    [InlineData("de_DE.UTF-8", 827)]
    [InlineData(null, 827)]
    public void PaperFollowsLocaleWhenPrinterDoesNotSay(string? lang, int width)
    {
        CupsPrintService service = new CupsPrintService(new RecordingRunner(), LinuxDistribution.Unknown, name => name == "LANG" ? lang : null, temp);

        Assert.Equal(width, service.GetPageSetup(new PrintOptions()).Width);
    }

    [Fact]
    public void PrintSendsAllPagesAsOneJobAndCleansUp()
    {
        RecordingRunner runner = new RecordingRunner("lp");
        CupsPrintService service = new CupsPrintService(runner, LinuxDistribution.Unknown, _ => null, temp);

        service.Print(new PrintOptions { PrinterName = "Office" }, "Text", (setup, index) =>
        {
            Assert.Equal(PrintPageSetup.A4, setup);
            return new PrintedPage(new PixelBuffer(4, 4), index < 2);
        });

        (string command, IReadOnlyList<string> arguments, string[] files) = Assert.Single(runner.Calls);
        Assert.Equal("lp", command);
        Assert.Equal(3, files.Length);
        Assert.All(files, file => Assert.True(PngCodec.IsPng(File.ReadAllBytes(file))));
        Assert.Empty(Directory.GetFileSystemEntries(temp));
    }

    [Fact]
    public void FailedJobReportsLpError()
    {
        RecordingRunner runner = new RecordingRunner("lp") { Failure = "lp: The printer or class does not exist." };
        CupsPrintService service = new CupsPrintService(runner, LinuxDistribution.Unknown, _ => null, temp);

        IOException error = Assert.Throws<IOException>(() => service.Print(new PrintOptions(), "x", (_, _) => new PrintedPage(new PixelBuffer(1, 1), false)));
        Assert.Contains("does not exist", error.Message);
    }

    [Fact]
    public void MissingLpNamesThePackage()
    {
        LinuxDistribution debian = LinuxDistribution.Parse("ID=debian\nPRETTY_NAME=\"Debian\"\n");
        CupsPrintService service = new CupsPrintService(new RecordingRunner(), debian, _ => null, temp);

        Assert.False(service.Support.IsSupported);
        Assert.Contains("cups-client", service.Support.Reason);
    }
}

public class CommandLineSoundTests : IDisposable
{
    private readonly string temp = Path.Combine(Path.GetTempPath(), "sharex-sound-test-" + Guid.NewGuid().ToString("N"));

    public CommandLineSoundTests() => Directory.CreateDirectory(temp);

    public void Dispose() => Directory.Delete(temp, true);

    [Fact]
    public void UsesFirstInstalledPlayerAndDeletesTheTemporaryFile()
    {
        RecordingRunner runner = new RecordingRunner("paplay", "aplay");
        CommandLineSoundService service = new CommandLineSoundService(runner, ["pw-play", "paplay", "aplay"], FeatureSupport.NotSupported("none"), temp);

        service.Play(Encoding.ASCII.GetBytes("RIFF"));

        (string command, _, string[] files) = Assert.Single(runner.Calls);
        Assert.Equal("paplay", command);
        Assert.Equal("RIFF", Encoding.ASCII.GetString(File.ReadAllBytes(Assert.Single(files))));
        Assert.Empty(Directory.GetFileSystemEntries(temp));
    }

    [Fact]
    public void WithoutPlayerReportsReasonAndDoesNothing()
    {
        RecordingRunner runner = new RecordingRunner();
        CommandLineSoundService service = new CommandLineSoundService(runner, ["pw-play"], FeatureSupport.NotSupported("Install pipewire."), temp);

        service.Play([1, 2, 3]);

        Assert.Equal("Install pipewire.", service.Support.Reason);
        Assert.Empty(runner.Calls);
    }
}

/// <summary>Records commands and keeps a copy of the files they were given, which the services delete afterwards.</summary>
internal sealed class RecordingRunner(params string[] commands) : ICommandRunner
{
    private readonly string copies = Path.Combine(Path.GetTempPath(), "sharex-runner-" + Guid.NewGuid().ToString("N"));

    public List<(string Command, IReadOnlyList<string> Arguments, string[] Files)> Calls { get; } = new();

    public string? Failure { get; init; }

    public bool Exists(string command) => Array.IndexOf(commands, command) >= 0;

    public Task<CommandResult> RunAsync(string command, IReadOnlyList<string> arguments, byte[]? standardInput = null,
        TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(copies);
        string[] files = arguments.Where(File.Exists).Select(file =>
        {
            string copy = Path.Combine(copies, Path.GetFileName(file));
            File.Copy(file, copy);
            return copy;
        }).ToArray();

        Calls.Add((command, arguments.ToList(), files));
        return Task.FromResult(new CommandResult(Failure == null ? 0 : 1, [], Failure ?? ""));
    }

    public Task<int> RunForkingAsync(string command, IReadOnlyList<string> arguments, byte[]? standardInput = null,
        TimeSpan? timeout = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();
}
