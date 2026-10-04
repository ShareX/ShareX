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

using ShareX.Platform.Linux;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;

namespace ShareX.Platform.Tests;

public sealed class LinuxFactAttribute : FactAttribute
{
    public LinuxFactAttribute()
    {
        if (!OperatingSystem.IsLinux())
        {
            Skip = "Uses Linux process semantics.";
        }
    }
}

public class LinuxApplicationLaunchTests
{
    private readonly LinuxApplicationLaunchService service = new LinuxApplicationLaunchService();

    [Fact]
    public void GetExecutablePath_HasNoExtension()
    {
        Assert.Equal(Path.Combine(Path.GetTempPath(), "ShareX"), service.GetExecutablePath(Path.GetTempPath(), "ShareX"));
    }

    [Fact]
    public void GetExecutablePath_RejectsRelativeDirectory()
    {
        ArgumentException error = Assert.ThrowsAny<ArgumentException>(() => service.GetExecutablePath("relative/dir", "ShareX"));
        Assert.Equal("directory", error.ParamName);
    }

    // The directory is valid and absolute on every host, so only the name can be what is rejected.
    [Theory]
    [InlineData("../ShareX")]
    [InlineData("..")]
    [InlineData("")]
    public void GetExecutablePath_RejectsUnsafeName(string name)
    {
        ArgumentException error = Assert.ThrowsAny<ArgumentException>(() => service.GetExecutablePath(Path.GetTempPath(), name));
        Assert.Equal("applicationName", error.ParamName);
    }

    [Fact]
    public void LaunchDetached_RejectsRelativePath()
    {
        ArgumentException error = Assert.Throws<ArgumentException>(() => service.LaunchDetached("sh", []));
        Assert.Equal("executablePath", error.ParamName);
    }

    [Fact]
    public void LaunchDetached_RejectsNullCharacterInArgument()
    {
        string executable = Path.Combine(Path.GetTempPath(), "sharex-launch-validation");
        ArgumentException error = Assert.Throws<ArgumentException>(() => service.LaunchDetached(executable, ["a\0b"]));
        Assert.Equal("arguments", error.ParamName);
    }

    [LinuxFact]
    public void LaunchDetached_StartsInOwnSessionWithArgumentsAndNoInheritedStdio()
    {
        string directory = Path.Combine(Path.GetTempPath(), "sharex-launch-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string output = Path.Combine(directory, "out");

        try
        {
            // $0 is "/bin/sh" (argv[0]), then the arguments as passed. The script reports its session id and its stdin target.
            string script = """
                out="$1"; shift
                {
                  printf '%s\n' "$$"
                  cut -d' ' -f6 /proc/$$/stat
                  readlink /proc/$$/fd/0
                  for a in "$@"; do printf '[%s]\n' "$a"; done
                } > "$out.tmp" && mv "$out.tmp" "$out"
                """;
            // The source file may be checked out with CRLF line endings; the shell needs LF.
            script = script.ReplaceLineEndings("\n");
            int pid = service.LaunchDetached("/bin/sh", ["-c", script, "/bin/sh", output, "two words", "\"quoted\" $HOME `x`", ""]);

            SpinWait.SpinUntil(() => File.Exists(output), TimeSpan.FromSeconds(10));
            string[] lines = File.ReadAllLines(output);

            Assert.Equal(pid.ToString(), lines[0]);
            // Session id equals the process id: setsid took effect.
            Assert.Equal(pid.ToString(), lines[1]);
            Assert.Equal("/dev/null", lines[2]);
            Assert.Equal(["[two words]", "[\"quoted\" $HOME `x`]", "[]"], lines.Skip(3));
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    /// <summary>
    /// A program started directly (not a shell, which may block signals itself) has no blocked or ignored signals, so ShareX
    /// can be closed normally, and leads its own session.
    /// </summary>
    [LinuxFact]
    public void LaunchDetached_ChildHasDefaultSignalsAndOwnSession()
    {
        string output = Path.Combine(Path.GetTempPath(), "sharex-launch-status-" + Guid.NewGuid().ToString("N"));

        try
        {
            int pid = service.LaunchDetached("/bin/cp", ["/proc/self/status", output]);
            SpinWait.SpinUntil(() => File.Exists(output) && new FileInfo(output).Length > 0, TimeSpan.FromSeconds(10));
            Thread.Sleep(100);
            string[] status = File.ReadAllLines(output);
            string Field(string name) => status.First(line => line.StartsWith(name + ":", StringComparison.Ordinal)).Split('\t')[1].Trim();

            Assert.Equal(0UL, Convert.ToUInt64(Field("SigBlk"), 16));
            Assert.Equal(0UL, Convert.ToUInt64(Field("SigIgn"), 16));
            Assert.Equal(pid.ToString(), Field("NSsid").Split(' ', '\t')[0]);
        }
        finally
        {
            File.Delete(output);
        }
    }
}
