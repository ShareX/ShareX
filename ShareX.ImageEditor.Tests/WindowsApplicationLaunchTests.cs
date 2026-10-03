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
using ShareX.Platform.Windows;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Xunit;

namespace ShareX.ImageEditor.Tests;

[Collection("Windows application verification")]
public sealed class WindowsApplicationLaunchTests
{
    [Fact]
    public void UnsupportedLaunchReportsItsReasonWithoutStartingAnApplication()
    {
        IApplicationLaunchService service = new UnsupportedApplicationLaunchService("Fixture launch unavailable.");
        Assert.False(service.Support.IsSupported);
        Assert.Equal("Fixture launch unavailable.", service.Support.Reason);
        Assert.Equal(Path.Combine(Path.GetTempPath(), "ShareX"), service.GetExecutablePath(Path.GetTempPath(), "ShareX"));
        Assert.Equal(service.Support.Reason, Assert.Throws<PlatformNotSupportedException>(
            () => service.LaunchDetached("unused", [])).Message);
    }

    [WindowsLaunchFact]
    [SupportedOSPlatform("windows")]
    public void NativeLaunchRoundTripsUnicodeAndQuotedArguments()
    {
        if (!OperatingSystem.IsWindows()) return;
        using LaunchFiles files = new();
        WindowsApplicationLaunchService service = new();
        Assert.True(service.Support.IsSupported);
        Assert.Equal(Path.Combine(files.DirectoryPath, "ShareX.exe"), service.GetExecutablePath(files.DirectoryPath, "ShareX"));
        string[] arguments = ["", "simple", "two words", "tab\there", "İstanbul 日本語 😀", "\"quoted\"", "slashes\\\"quote", "ends in \\", "plain\\", "&|<>$%!"];
        string recordPath = Path.Combine(files.DirectoryPath, "arguments.json");
        int processId = service.LaunchDetached(files.ExecutablePath, ["--record", recordPath, .. arguments]);
        LaunchRecord record = files.ReadRecord<LaunchRecord>(recordPath);
        Assert.Equal(processId, record.ProcessId);
        Assert.Equal(arguments, record.Arguments);
        files.WaitForOwnedProcess(processId);
    }

    [WindowsLaunchFact]
    [SupportedOSPlatform("windows")]
    public void ChildSurvivesWhenItsOwnParentClosesAKillOnCloseJob()
    {
        if (!OperatingSystem.IsWindows()) return;
        using LaunchFiles files = new();
        using Process parent = files.StartParent();
        int childId = 0;
        try
        {
            JobParentRecord parentRecord = files.ReadRecord<JobParentRecord>(Path.Combine(files.DirectoryPath, "parent.json"));
            childId = parentRecord.ChildProcessId;
            Assert.Equal(parent.Id, parentRecord.ProcessId);
            Assert.True(parentRecord.InJob);
            LaunchRecord child = files.ReadRecord<LaunchRecord>(Path.Combine(files.DirectoryPath, "child.json"));
            Assert.Equal(childId, child.ProcessId);
            Assert.False(child.InJob);
            Assert.True(parent.WaitForExit(25000), files.Diagnostics());
            Assert.True(File.Exists(Path.Combine(files.DirectoryPath, "parent-closing-job")), files.Diagnostics());
            Assert.False(File.Exists(Path.Combine(files.DirectoryPath, "parent-survived-job-close")));
            File.WriteAllText(Path.Combine(files.DirectoryPath, "release"), "continue");
            NativeProcessFixture.WaitForFile(Path.Combine(files.DirectoryPath, "finished"));
            files.WaitForOwnedProcess(childId);
        }
        finally
        {
            if (!parent.HasExited) parent.Kill();
            if (childId > 0) files.StopOwnedProcess(childId);
        }
    }

    [WindowsLaunchFact]
    [SupportedOSPlatform("windows")]
    public void NativeProcessAndThreadHandlesAreReleasedOnSuccessAndFailure()
    {
        if (!OperatingSystem.IsWindows()) return;
        using LaunchFiles files = new();
        WindowsApplicationLaunchService service = new();
        using Process check = files.StartFixture("--handle-check");
        Assert.True(check.WaitForExit(25000), files.Diagnostics());
        Assert.True(check.ExitCode == 0, files.Diagnostics());
        HandleRecord handles = files.ReadRecord<HandleRecord>(Path.Combine(files.DirectoryPath, "handles.json"));
        Assert.True(handles.After <= handles.Before + 2, $"Handles before: {handles.Before}; after: {handles.After}");

        Assert.Throws<ArgumentException>(() => service.LaunchDetached("relative.exe", []));
        Assert.Throws<ArgumentException>(() => service.LaunchDetached(files.ExecutablePath, ["before\0after"]));
        Assert.Throws<ArgumentException>(() => service.LaunchDetached(files.ExecutablePath, [new string('x', 32767)]));
        Assert.Throws<ArgumentException>(() => service.GetExecutablePath("relative", "ShareX"));
        Assert.Throws<ArgumentException>(() => service.GetExecutablePath(files.DirectoryPath, "../ShareX"));

    }

    [WindowsApplicationSmokeFact]
    [SupportedOSPlatform("windows")]
    public async Task NativeMessagingHostEchoesTheFramedPayloadAndLaunchesTheFixtureApplication()
    {
        if (!OperatingSystem.IsWindows()) return;
        using LaunchFiles files = new();
        string hostPath = files.CopyBrowserHost();
        string output = Path.Combine(files.DirectoryPath, "browser-result.json");
        string input = JsonSerializer.Serialize(new { TestOutputFile = output, Text = "İstanbul 日本語 😀 \"quoted\" \\ end" },
            new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        byte[] body = Encoding.UTF8.GetBytes(input);
        using Process host = StartHost(hostPath, files.DirectoryPath);
        using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(25));
        try
        {
            await host.StandardInput.BaseStream.WriteAsync(BitConverter.GetBytes(body.Length), timeout.Token);
            await host.StandardInput.BaseStream.WriteAsync(body, timeout.Token);
            await host.StandardInput.BaseStream.FlushAsync(timeout.Token);
            host.StandardInput.Close();
            byte[] prefix = new byte[4];
            await host.StandardOutput.BaseStream.ReadExactlyAsync(prefix, timeout.Token);
            Assert.Equal(body.Length, BitConverter.ToInt32(prefix));
            byte[] echoed = new byte[body.Length];
            await host.StandardOutput.BaseStream.ReadExactlyAsync(echoed, timeout.Token);
            Assert.Equal(body, echoed);
            await host.WaitForExitAsync(timeout.Token);
            Assert.Equal(0, host.ExitCode);
            BrowserLaunchRecord record = files.ReadRecord<BrowserLaunchRecord>(output);
            Assert.Equal(input, record.Payload);
            Assert.Equal(2, record.Arguments.Length);
            Assert.Equal("-NativeMessagingInput", record.Arguments[0]);
            Assert.True(Path.IsPathFullyQualified(record.Arguments[1]));
            Assert.Equal(".json", Path.GetExtension(record.Arguments[1]));
            files.WaitForOwnedProcess(record.ProcessId);
            Assert.False(File.Exists(record.Arguments[1]));
        }
        finally
        {
            if (!host.HasExited) { host.Kill(); await host.WaitForExitAsync(); }
        }
    }

    [WindowsApplicationSmokeFact]
    [SupportedOSPlatform("windows")]
    public async Task NativeMessagingHostDoesNotLaunchAnApplicationForAnEmptyMessage()
    {
        if (!OperatingSystem.IsWindows()) return;
        using LaunchFiles files = new();
        string hostPath = files.CopyBrowserHost();
        using Process host = StartHost(hostPath, files.DirectoryPath);
        using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(25));
        try
        {
            await host.StandardInput.BaseStream.WriteAsync(new byte[4], timeout.Token);
            host.StandardInput.Close();
            string output = await host.StandardOutput.ReadToEndAsync(timeout.Token);
            await host.WaitForExitAsync(timeout.Token);
            Assert.Equal(0, host.ExitCode);
            Assert.Empty(output);
            Assert.False(File.Exists(Path.Combine(files.DirectoryPath, "fixture-started")));
        }
        finally
        {
            if (!host.HasExited) { host.Kill(); await host.WaitForExitAsync(); }
        }
    }

    [SupportedOSPlatform("windows")]
    private static Process StartHost(string executable, string workingDirectory)
    {
        ProcessStartInfo start = new(executable)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            WorkingDirectory = workingDirectory,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        start.ArgumentList.Add("chrome-extension://synthetic-sharex-fixture/");
        return Assert.IsType<Process>(Process.Start(start));
    }

    [SupportedOSPlatform("windows")]
    internal sealed class LaunchFiles : IDisposable
    {
        public string DirectoryPath { get; } = Path.Combine(Path.GetTempPath(), "ShareX launch fixture 日本語 " + Guid.NewGuid().ToString("N"));
        public string ExecutablePath => Path.Combine(DirectoryPath, "launch fixture.exe");

        public LaunchFiles()
        {
            Directory.CreateDirectory(DirectoryPath);
            try
            {
                // Copy only this build's test fixtures. No installed application or user settings are read.
                foreach (string file in Directory.EnumerateFiles(AppContext.BaseDirectory))
                {
                    if (Path.GetExtension(file) is ".dll" or ".json")
                        File.Copy(file, Path.Combine(DirectoryPath, Path.GetFileName(file)));
                }
                string apphost = Path.Combine(AppContext.BaseDirectory, "ShareX.ImageEditor.Tests.exe");
                byte[] bytes = File.ReadAllBytes(apphost);
                int pe = BitConverter.ToInt32(bytes, 0x3c);
                Assert.Equal(0x00004550, BitConverter.ToInt32(bytes, pe));
                // Make the private fixture copy a GUI apphost so native launch cannot create a console window.
                byte[] subsystem = BitConverter.GetBytes((ushort)2);
                subsystem.CopyTo(bytes, pe + 24 + 68);
                File.WriteAllBytes(ExecutablePath, bytes);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public Process StartParent() => StartFixture("--job-parent");

        public string CopyBrowserHost()
        {
            string source = Environment.GetEnvironmentVariable("SHAREX_TEST_WINDOWS_APPLICATION_DIRECTORY")!;
            foreach (string file in Directory.EnumerateFiles(source))
            {
                string name = Path.GetFileName(file);
                if (Path.GetExtension(file) is ".dll" or ".json" || name == "ShareX_NativeMessagingHost.exe")
                    File.Copy(file, Path.Combine(DirectoryPath, name), true);
            }
            File.Copy(ExecutablePath, Path.Combine(DirectoryPath, "ShareX.exe"));
            return Path.Combine(DirectoryPath, "ShareX_NativeMessagingHost.exe");
        }

        public Process StartFixture(string mode)
        {
            ProcessStartInfo start = new(ExecutablePath)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = DirectoryPath
            };
            foreach (string argument in new[] { mode, DirectoryPath, ExecutablePath }) start.ArgumentList.Add(argument);
            return Assert.IsType<Process>(Process.Start(start));
        }

        public T ReadRecord<T>(string path)
        {
            try { NativeProcessFixture.WaitForFile(path); }
            catch (TimeoutException) { throw new Xunit.Sdk.XunitException(Diagnostics()); }
            return JsonSerializer.Deserialize<T>(File.ReadAllText(path))!;
        }

        public string Diagnostics() => File.Exists(Path.Combine(DirectoryPath, "fixture-error.txt"))
            ? File.ReadAllText(Path.Combine(DirectoryPath, "fixture-error.txt")) : "The generated process fixture did not complete: " + DirectoryPath;

        public void WaitForOwnedProcess(int id)
        {
            try
            {
                using Process process = Process.GetProcessById(id);
                if (!process.WaitForExit(25000))
                {
                    process.Kill();
                    Assert.Fail(Diagnostics());
                }
            }
            catch (ArgumentException) { } // The child can exit before the test opens its process handle.
        }

        public void StopOwnedProcess(int id)
        {
            try
            {
                using Process process = Process.GetProcessById(id);
                if (!string.Equals(process.MainModule?.FileName, ExecutablePath, StringComparison.OrdinalIgnoreCase)) return;
                if (!process.HasExited) process.Kill();
                process.WaitForExit(5000);
            }
            catch (ArgumentException) { }
        }

        public void Dispose()
        {
            if (!Directory.Exists(DirectoryPath)) return;
            string target = Path.GetFullPath(DirectoryPath);
            if (!target.StartsWith(Path.GetFullPath(Path.GetTempPath()), StringComparison.OrdinalIgnoreCase) ||
                !Path.GetFileName(target).StartsWith("ShareX launch fixture 日本語 ", StringComparison.Ordinal))
                throw new InvalidOperationException("The fixture cleanup path is outside its temporary directory.");
            foreach (string file in Directory.EnumerateFiles(target, "*", SearchOption.AllDirectories))
                File.SetAttributes(file, File.GetAttributes(file) & ~FileAttributes.ReadOnly);
            for (int attempt = 0; ; attempt++)
            {
                try { Directory.Delete(target, true); return; }
                catch (Exception error) when (attempt < 20 && error is IOException or UnauthorizedAccessException)
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}

internal sealed class WindowsLaunchFactAttribute : FactAttribute
{
    public WindowsLaunchFactAttribute()
    {
        if (!OperatingSystem.IsWindows()) Skip = "Native application launch is verified on Windows.";
    }
}
