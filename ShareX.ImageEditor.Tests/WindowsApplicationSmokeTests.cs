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

using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Xunit;

namespace ShareX.ImageEditor.Tests;

[Collection("Windows application verification")]
public sealed class WindowsApplicationSmokeTests
{
    [WindowsApplicationSmokeFact]
    public async Task PortableApplicationStartsAndExitsThroughItsCommandLine()
    {
        if (!OperatingSystem.IsWindows()) return;
        using ApplicationFiles files = new();
        string log = await files.RunAsync("windows-application-startup.log", "-portable", "-multi", "-silent", "-NoHotkeys", "-ExitShareX");
        Assert.Contains("Personal path: " + files.SettingsPath, log);
        Assert.Contains("Personal path detection method: Portable CLI flag", log);
        Assert.Contains("HotkeyManager started.", log);
        Assert.Contains("WatchFolderManager started.", log);
        Assert.Contains("Startup time:", log);
        Assert.Contains("CommandLine: -ExitShareX", log);
        Assert.Contains("Hotkey host init finished.", log);
        Assert.Contains("ShareX closing.", log);
        Assert.Contains("ShareX closed.", log);
        using (FileStream history = File.Open(Path.Combine(files.SettingsPath, "History.db"), FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {
            byte[] header = new byte[16];
            history.ReadExactly(header);
            Assert.Equal("SQLite format 3\0", System.Text.Encoding.ASCII.GetString(header));
        }

        using JsonDocument settings = JsonDocument.Parse(File.ReadAllText(Path.Combine(files.SettingsPath, "ApplicationConfig.json")));
        Assert.False(settings.RootElement.GetProperty("ShowStartScreen").GetBoolean());
        Assert.False(settings.RootElement.GetProperty("AutoCheckUpdate").GetBoolean());
        Assert.True(settings.RootElement.GetProperty("DisableUpload").GetBoolean());
        Assert.True(settings.RootElement.GetProperty("DisableHotkeys").GetBoolean());
        using JsonDocument hotkeys = JsonDocument.Parse(File.ReadAllText(Path.Combine(files.SettingsPath, "HotkeysConfig.json")));
        Assert.Empty(hotkeys.RootElement.GetProperty("Hotkeys").EnumerateArray());
    }

    internal sealed class ApplicationFiles : IDisposable
    {
        private readonly string tempRoot = Path.GetFullPath(Path.GetTempPath());
        public string DirectoryPath { get; }
        public string SettingsPath => Path.Combine(DirectoryPath, "ShareX");

        public ApplicationFiles()
        {
            DirectoryPath = Path.GetFullPath(Path.Combine(tempRoot, "sharex-application-smoke-" + Guid.NewGuid().ToString("N")));
            Directory.CreateDirectory(DirectoryPath);
            try
            {
                string source = Path.GetFullPath(Environment.GetEnvironmentVariable("SHAREX_TEST_WINDOWS_APPLICATION_DIRECTORY")!);
                foreach (string file in Directory.EnumerateFiles(source))
                {
                    string name = Path.GetFileName(file);
                    string extension = Path.GetExtension(file);
                    if (extension is ".dll" or ".pdb" || name is "ShareX.exe" or "ShareX.deps.json" or "ShareX.runtimeconfig.json")
                        File.Copy(file, Path.Combine(DirectoryPath, name));
                }
                // Copy only build resource/native-library directories, never a portable settings folder.
                foreach (string directory in Directory.EnumerateDirectories(source))
                {
                    string name = Path.GetFileName(directory);
                    if (name.Equals("ShareX", StringComparison.OrdinalIgnoreCase)) continue;
                    if (name != "runtimes")
                    {
                        try { _ = CultureInfo.GetCultureInfo(name); }
                        catch (CultureNotFoundException) { continue; }
                    }
                    string pattern = name == "runtimes" ? "*.dll" : "*.resources.dll";
                    SearchOption search = name == "runtimes" ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                    foreach (string file in Directory.EnumerateFiles(directory, pattern, search))
                    {
                        string target = Path.Combine(DirectoryPath, Path.GetRelativePath(source, file));
                        Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                        File.Copy(file, target);
                    }
                }
                Directory.CreateDirectory(SettingsPath);
                File.WriteAllText(Path.Combine(SettingsPath, "ApplicationConfig.json"), """
                    {
                      "ShowStartScreen": false,
                      "AutoCheckUpdate": false,
                      "ShowTray": true,
                      "DisableUpload": true,
                      "DisableHotkeys": true,
                      "TrayLeftClickAction": "None",
                      "TrayLeftDoubleClickAction": "None",
                      "TrayMiddleClickAction": "None"
                    }
                    """);
                File.WriteAllText(Path.Combine(SettingsPath, "HotkeysConfig.json"), """{"Hotkeys": []}""");
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public string ReadLog()
        {
            string logs = Path.Combine(SettingsPath, "Logs");
            return Directory.Exists(logs)
                ? string.Join(Environment.NewLine, Directory.EnumerateFiles(logs, "ShareX-Log-*.txt").Select(File.ReadAllText))
                : "";
        }

        public async Task<string> RunAsync(string artifactName, params string[] arguments)
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = Path.Combine(DirectoryPath, "ShareX.exe"),
                WorkingDirectory = DirectoryPath,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            foreach (string argument in arguments) startInfo.ArgumentList.Add(argument);
            using Process process = Assert.IsType<Process>(Process.Start(startInfo));
            Task<string> output = process.StandardOutput.ReadToEndAsync();
            Task<string> error = process.StandardError.ReadToEndAsync();
            bool exited = false;
            try
            {
                await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));
                exited = true;
            }
            catch (TimeoutException)
            {
                // Only the process created by this fixture is terminated on timeout.
            }
            finally
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                    await process.WaitForExitAsync();
                }
            }
            string log = ReadLog();
            string diagnostics = $"Exit code: {process.ExitCode}{Environment.NewLine}{await output}{await error}{log}";
            string? artifacts = Environment.GetEnvironmentVariable("SHAREX_TEST_GRAPHICS_OUTPUT");
            if (!string.IsNullOrEmpty(artifacts))
            {
                Directory.CreateDirectory(artifacts);
                File.WriteAllText(Path.Combine(artifacts, artifactName), diagnostics);
            }
            Assert.True(exited, "The isolated application did not exit within 30 seconds." + Environment.NewLine + diagnostics);
            Assert.True(process.ExitCode == 0, diagnostics);
            return log;
        }

        public void Dispose()
        {
            string target = Path.GetFullPath(DirectoryPath);
            if (Path.GetDirectoryName(target) != Path.TrimEndingDirectorySeparator(tempRoot) ||
                !Path.GetFileName(target).StartsWith("sharex-application-smoke-", StringComparison.Ordinal))
                throw new InvalidOperationException("Refusing to remove a directory outside this fixture's temporary root.");
            // A terminated renderer can release its native DLL mappings just after the process handle signals.
            for (int attempt = 0; Directory.Exists(target); attempt++)
            {
                try { Directory.Delete(target, recursive: true); }
                catch (Exception exception) when (attempt < 20 && exception is IOException or UnauthorizedAccessException)
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}

[CollectionDefinition("Windows application verification", DisableParallelization = true)]
public sealed class WindowsApplicationVerificationCollection { }

public sealed class WindowsApplicationSmokeFactAttribute : FactAttribute
{
    public WindowsApplicationSmokeFactAttribute()
    {
        if (!OperatingSystem.IsWindows()) Skip = "Requires the native Windows application.";
        else if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SHAREX_TEST_WINDOWS_APPLICATION_DIRECTORY")))
            Skip = "Set SHAREX_TEST_WINDOWS_APPLICATION_DIRECTORY after building the real application.";
    }
}
