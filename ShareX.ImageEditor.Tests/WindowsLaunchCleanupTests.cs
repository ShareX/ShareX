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
using System.Runtime.Versioning;
using Xunit;
using LaunchFiles = ShareX.ImageEditor.Tests.WindowsApplicationLaunchTests.LaunchFiles;

namespace ShareX.ImageEditor.Tests;

[Collection("Windows application verification")]
public sealed class WindowsLaunchCleanupTests
{
    [WindowsLaunchFact]
    [SupportedOSPlatform("windows")]
    public async Task CleanupWaitsForAnOwnedFileLockBeyondTheOldRetryWindow()
    {
        if (!OperatingSystem.IsWindows()) return;
        using LaunchFiles files = new();
        using FileStream locked = File.Open(files.ExecutablePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        TaskCompletionSource denied = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Task cleanup = Task.Run(() => LaunchFiles.DeleteDirectory(files.DirectoryPath, TimeSpan.FromSeconds(5),
            error => denied.TrySetResult()));
        try
        {
            await denied.Task.WaitAsync(TimeSpan.FromSeconds(2));
            await Task.Delay(2200); // The old cleanup exhausted its retry budget while this lock was still held.
            Assert.False(cleanup.IsCompleted);
            locked.Dispose();
            await cleanup.WaitAsync(TimeSpan.FromSeconds(3));
            Assert.False(Directory.Exists(files.DirectoryPath));
        }
        finally
        {
            locked.Dispose();
            await cleanup.WaitAsync(TimeSpan.FromSeconds(6));
        }
    }

    [WindowsLaunchFact]
    [SupportedOSPlatform("windows")]
    public void PersistentLockStillFailsWithItsTargetNativeErrorAndRecords()
    {
        if (!OperatingSystem.IsWindows()) return;
        using LaunchFiles files = new();
        string record = "{\"ProcessId\":12345,\"ChildProcessId\":23456,\"InJob\":true}";
        File.WriteAllText(Path.Combine(files.DirectoryPath, "parent.json"), record);
        using FileStream locked = File.Open(files.ExecutablePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        Stopwatch elapsed = Stopwatch.StartNew();

        IOException error = Assert.Throws<IOException>(() =>
            LaunchFiles.DeleteDirectory(files.DirectoryPath, TimeSpan.FromMilliseconds(150)));

        Assert.True(elapsed.Elapsed < TimeSpan.FromSeconds(2), error.ToString());
        Assert.Contains(files.DirectoryPath, error.Message);
        Assert.Contains("launch fixture.exe", error.Message);
        Assert.Contains("parent.json: " + record, error.Message);
        Assert.NotNull(error.InnerException);
        Assert.True(error.InnerException is IOException or UnauthorizedAccessException);
        Assert.Contains($"0x{error.InnerException.HResult:X8}", error.Message);
        Assert.True(File.Exists(files.ExecutablePath)); // A permanent lock must never be swallowed or treated as clean.
        locked.Dispose();
    }

    [WindowsLaunchFact]
    [SupportedOSPlatform("windows")]
    public void CleanupClearsReadOnlyFixtureFilesAndIsIdempotent()
    {
        if (!OperatingSystem.IsWindows()) return;
        using LaunchFiles files = new();
        string readOnly = Path.Combine(files.DirectoryPath, "synthetic-read-only.json");
        File.WriteAllText(readOnly, "generated fixture data");
        File.SetAttributes(readOnly, File.GetAttributes(readOnly) | FileAttributes.ReadOnly);

        files.Dispose();
        files.Dispose();

        Assert.False(Directory.Exists(files.DirectoryPath));
    }

    [WindowsLaunchFact]
    [SupportedOSPlatform("windows")]
    public void CleanupRequiresADirectGeneratedChildOfTheTemporaryRoot()
    {
        if (!OperatingSystem.IsWindows()) return;
        string root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(Path.GetTempPath()));
        string name = "ShareX launch fixture 日本語 " + Guid.NewGuid().ToString("N");
        string sibling = Path.Combine(root + "-sibling", name);
        string wrongName = Path.Combine(root, "unrelated-" + Guid.NewGuid().ToString("N"));
        string invalidId = Path.Combine(root, "ShareX launch fixture 日本語 not-a-fixture-id");
        string nested = Path.Combine(root, name, name);
        foreach (string candidate in new[] { sibling, wrongName, invalidId, nested })
        {
            Assert.Throws<InvalidOperationException>(() =>
                LaunchFiles.DeleteDirectory(candidate, TimeSpan.FromMilliseconds(150)));
        }
    }
}
