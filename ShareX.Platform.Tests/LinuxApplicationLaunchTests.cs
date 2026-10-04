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

    [Theory]
    [InlineData("relative/dir", "ShareX")]
    [InlineData("/opt/sharex", "../ShareX")]
    [InlineData("/opt/sharex", "..")]
    [InlineData("/opt/sharex", "")]
    public void GetExecutablePath_RejectsUnsafeInput(string directory, string name)
    {
        Assert.ThrowsAny<ArgumentException>(() => service.GetExecutablePath(directory, name));
    }

    [Fact]
    public void LaunchDetached_RejectsRelativePathsAndNullCharacters()
    {
        Assert.Throws<ArgumentException>(() => service.LaunchDetached("sh", []));
        Assert.Throws<ArgumentException>(() => service.LaunchDetached("/bin/sh", ["a\0b"]));
    }

    [LinuxFact]
    public void LaunchDetached_StartsInOwnSessionWithArgumentsAndNoInheritedStdio()
    {
        string directory = Path.Combine(Path.GetTempPath(), "sharex-launch-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string output = Path.Combine(directory, "out");

        try
        {
            // $0 is "/bin/sh" (argv[0]), then the arguments as passed. The script reports its session id, its stdin target and
            // whether SIGTERM is blocked or ignored, which the .NET runtime that started it may have set up.
            string script = """
                out="$1"; shift
                {
                  printf '%s\n' "$$"
                  cut -d' ' -f6 /proc/$$/stat
                  readlink /proc/$$/fd/0
                  grep -E '^Sig(Blk|Ign):' /proc/$$/status
                  for a in "$@"; do printf '[%s]\n' "$a"; done
                } > "$out.tmp" && mv "$out.tmp" "$out"
                """;
            int pid = service.LaunchDetached("/bin/sh", ["-c", script, "/bin/sh", output, "two words", "\"quoted\" $HOME `x`", ""]);

            SpinWait.SpinUntil(() => File.Exists(output), TimeSpan.FromSeconds(10));
            string[] lines = File.ReadAllLines(output);

            Assert.Equal(pid.ToString(), lines[0]);
            // Session id equals the process id: setsid took effect.
            Assert.Equal(pid.ToString(), lines[1]);
            Assert.Equal("/dev/null", lines[2]);
            // SIGTERM (bit 14) must be neither blocked nor ignored, so ShareX can still be closed normally. The shell itself blocks
            // SIGCHLD, so the masks are not compared whole.
            const ulong sigterm = 1UL << 14;
            Assert.All(lines.Skip(3).Take(2), line => Assert.Equal(0UL, Convert.ToUInt64(line.Split('\t')[1], 16) & sigterm));
            Assert.Equal(["[two words]", "[\"quoted\" $HOME `x`]", "[]"], lines.Skip(5));
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }
}
