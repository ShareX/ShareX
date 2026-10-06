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
using ShareX.Platform.Linux.DBus;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ShareX.Platform.Tests;

public class PortalApplicationDesktopEntryTests
{
    [UnixFact]
    public void NativeBuildGetsAHiddenCompleteIdentityWithoutChangingOtherDesktopFiles()
    {
        WithDirectory(root =>
        {
            string executable = CreateFile(root, "ShareX", "synthetic executable");
            string dataHome = Path.Combine(root, "data");
            Assert.True(PortalApplicationDesktopEntry.EnsureExists(dataHome, root, executable, ""));
            string content = File.ReadAllText(EntryPath(dataHome));
            var entry = DesktopEntry.Parse(content);
            Assert.Equal("ShareX", entry["Name"]);
            Assert.Equal("Application", entry["Type"]);
            Assert.Equal("true", entry["NoDisplay"]);
            Assert.Equal("false", entry["Terminal"]);
            Assert.Equal(executable, Assert.Single(DesktopEntry.ParseExec(entry["Exec"])));
            Assert.DoesNotContain("Hidden=", content);
            Assert.DoesNotContain("MimeType=", content);
            Assert.DoesNotContain("Actions=", content);
            Assert.Equal(new[] { EntryPath(dataHome) }, Directory.GetFiles(dataHome, "*", SearchOption.AllDirectories));
        });
    }

    [UnixFact]
    public void FrameworkDependentBuildIncludesItsAssemblyInTheExecCommand()
    {
        WithDirectory(root =>
        {
            string host = CreateFile(root, "dotnet", "synthetic host");
            string assembly = CreateFile(root, "ShareX.dll", "synthetic application");
            string dataHome = Path.Combine(root, "data");
            Assert.True(PortalApplicationDesktopEntry.EnsureExists(dataHome, root, host, assembly));
            var entry = DesktopEntry.Parse(File.ReadAllText(EntryPath(dataHome)));
            Assert.Equal(new[] { host, assembly }, DesktopEntry.ParseExec(entry["Exec"]));
        });
    }

    [UnixFact]
    public void ExecHasBothDesktopValueAndArgumentEscaping()
    {
        WithDirectory(root =>
        {
            string executable = CreateFile(root, "ShareX $`\\\" 100%", "synthetic executable");
            string dataHome = Path.Combine(root, "data");
            Assert.True(PortalApplicationDesktopEntry.EnsureExists(dataHome, root, executable, ""));
            var entry = DesktopEntry.Parse(File.ReadAllText(EntryPath(dataHome)));
            Assert.Equal(DesktopEntry.EscapeValue(DesktopEntry.QuoteArgument(executable)), entry["Exec"]);
            // The key-value parser in GIO first unescapes doubled backslashes, then Exec quoting.
            string unescaped = entry["Exec"].Replace("\\\\", "\\");
            Assert.Equal(executable, Assert.Single(DesktopEntry.ParseExec(unescaped)));
        });
    }

    [UnixTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void ExistingUserAndSystemEntriesAreRetainedByteForByte(bool systemEntry)
    {
        WithDirectory(root =>
        {
            string dataHome = Path.Combine(root, "data");
            string systemHome = Path.Combine(root, "system");
            string entryPath = EntryPath(systemEntry ? systemHome : dataHome);
            Directory.CreateDirectory(Path.GetDirectoryName(entryPath)!);
            const string original = "[Desktop Entry]\nName=Customized ShareX\nHidden=true\n";
            File.WriteAllText(entryPath, original);
            Assert.True(PortalApplicationDesktopEntry.EnsureExists(dataHome, systemHome, null, ""));
            Assert.Equal(original, File.ReadAllText(entryPath));
            if (systemEntry) Assert.False(Directory.Exists(dataHome));
        });
    }

    [UnixFact]
    public void LaterSystemSearchDirectoriesAreChecked()
    {
        WithDirectory(root =>
        {
            string dataHome = Path.Combine(root, "data");
            string systemHome = Path.Combine(root, "system");
            Directory.CreateDirectory(Path.GetDirectoryName(EntryPath(systemHome))!);
            File.WriteAllText(EntryPath(systemHome), "existing entry");
            Assert.True(PortalApplicationDesktopEntry.EnsureExists(dataHome,
                Path.Combine(root, "empty") + Path.PathSeparator + systemHome, null, ""));
            Assert.False(Directory.Exists(dataHome));
        });
    }

    [UnixFact]
    public void RepeatedPreparationDoesNotRewriteTheEntry()
    {
        WithDirectory(root =>
        {
            string executable = CreateFile(root, "ShareX", "synthetic executable");
            string dataHome = Path.Combine(root, "data");
            Assert.True(PortalApplicationDesktopEntry.EnsureExists(dataHome, root, executable, ""));
            string original = File.ReadAllText(EntryPath(dataHome));
            File.SetLastWriteTimeUtc(EntryPath(dataHome), new DateTime(2020, 1, 1));
            DateTime timestamp = File.GetLastWriteTimeUtc(EntryPath(dataHome));
            Assert.True(PortalApplicationDesktopEntry.EnsureExists(dataHome, root, executable, ""));
            Assert.Equal(original, File.ReadAllText(EntryPath(dataHome)));
            Assert.Equal(timestamp, File.GetLastWriteTimeUtc(EntryPath(dataHome)));
        });
    }

    [UnixFact]
    public void GeneratedIdentityFollowsTheCurrentBuildAfterThePreviousExecutableIsRemoved()
    {
        WithDirectory(root =>
        {
            string executable = CreateFile(root, "ShareX", "previous executable");
            string dataHome = Path.Combine(root, "data");
            Assert.True(PortalApplicationDesktopEntry.EnsureExists(dataHome, root, executable, ""));
            File.Delete(executable);
            string replacement = CreateFile(root, "ShareX-new", "current executable");
            Assert.True(PortalApplicationDesktopEntry.EnsureExists(dataHome, root, replacement, ""));
            var entry = DesktopEntry.Parse(File.ReadAllText(EntryPath(dataHome)));
            Assert.Equal(replacement, Assert.Single(DesktopEntry.ParseExec(entry["Exec"])));
            Assert.Equal("true", entry["X-ShareX-PortalIdentity"]);
            Assert.Equal(new[] { EntryPath(dataHome) }, Directory.GetFiles(dataHome, "*", SearchOption.AllDirectories));
        });
    }

    [UnixTheory]
    [InlineData("Name=ShareX", "Name=My ShareX")]
    [InlineData("NoDisplay=true", "NoDisplay=false")]
    public void CustomizedGeneratedIdentityIsNotReplaced(string before, string after)
    {
        WithDirectory(root =>
        {
            string executable = CreateFile(root, "ShareX", "previous executable");
            string dataHome = Path.Combine(root, "data");
            Assert.True(PortalApplicationDesktopEntry.EnsureExists(dataHome, root, executable, ""));
            string customized = File.ReadAllText(EntryPath(dataHome)).Replace(before, after);
            File.WriteAllText(EntryPath(dataHome), customized);
            string replacement = CreateFile(root, "ShareX-new", "current executable");
            Assert.True(PortalApplicationDesktopEntry.EnsureExists(dataHome, root, replacement, ""));
            Assert.Equal(customized, File.ReadAllText(EntryPath(dataHome)));
        });
    }

    [UnixTheory]
    [InlineData("relative")]
    [InlineData(null)]
    [InlineData("")]
    public void MissingOrRelativeExecutableDoesNotPublishAnInvalidIdentity(string? executable)
    {
        WithDirectory(root =>
        {
            string dataHome = Path.Combine(root, "data");
            Assert.False(PortalApplicationDesktopEntry.EnsureExists(dataHome, root, executable, ""));
            Assert.False(Directory.Exists(dataHome));
        });
    }

    [UnixFact]
    public void MissingManagedAssemblyDoesNotPublishADotnetOnlyIdentity()
    {
        WithDirectory(root =>
        {
            string host = CreateFile(root, "dotnet", "synthetic host");
            string dataHome = Path.Combine(root, "data");
            Assert.False(PortalApplicationDesktopEntry.EnsureExists(dataHome, root, host, Path.Combine(root, "missing.dll")));
            Assert.False(Directory.Exists(dataHome));
        });
    }

    [UnixFact]
    public void UnwritableIdentityLocationRemainsUnavailableAndDoesNotThrow()
    {
        WithDirectory(root =>
        {
            string executable = CreateFile(root, "ShareX", "synthetic executable");
            string dataHome = CreateFile(root, "data", "a file blocks the directory");
            Assert.False(PortalApplicationDesktopEntry.EnsureExists(dataHome, root, executable, ""));
            Assert.Equal("a file blocks the directory", File.ReadAllText(dataHome));
        });
    }

    [UnixFact]
    public async Task SandboxRegistrationDoesNotPrepareAHostDesktopIdentity()
    {
        bool prepared = false;
        PortalRegistrationResult result = await PortalApplicationRegistration.RegisterAsync(
            () => throw new InvalidOperationException("Sandbox must not use host Registry"), true, () => prepared = true);
        Assert.False(prepared);
        Assert.Null(result.Error);
    }

    [UnixFact]
    public async Task DesktopIdentityIsReadyBeforeHostRegistration()
    {
        bool prepared = false;
        PortalRegistrationResult result = await PortalApplicationRegistration.RegisterAsync(() =>
        {
            Assert.True(prepared);
            return Task.CompletedTask;
        }, false, () => prepared = true);
        Assert.Null(result.Error);
    }

    private static string EntryPath(string dataHome) => Path.Combine(dataHome, "applications", "sharex.desktop");

    private static string CreateFile(string root, string name, string content)
    {
        string path = Path.Combine(root, name);
        File.WriteAllText(path, content);
        return path;
    }

    private static void WithDirectory(Action<string> test)
    {
        string root = Path.Combine(Path.GetTempPath(), "sharex-portal-identity-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try { test(root); }
        finally { Directory.Delete(root, recursive: true); }
    }
}
