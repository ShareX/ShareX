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

using ShareX.Platform.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.Json;

namespace ShareX.ImageEditor.Tests;

/// <summary>Entry point used only when launch tests execute a private copy of this test project's apphost.</summary>
internal static class NativeProcessFixture
{
    public static int Main(string[] args)
    {
        if (!OperatingSystem.IsWindows() || args.Length == 0) return 0;
        try
        {
            File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "fixture-started"), Environment.ProcessId.ToString());
            return Run(args);
        }
        catch (Exception error)
        {
            if (args.Length > 1 && Directory.Exists(args[1]))
                File.WriteAllText(Path.Combine(args[1], "fixture-error.txt"), error.ToString());
            return 1;
        }
    }

    [SupportedOSPlatform("windows")]
    private static int Run(string[] args)
    {
        switch (args[0])
        {
            case "--record":
            case "--record-wait":
                IntPtr checkedJob = args[0] == "--record-wait" ? OpenJobObject(4, false, args[4]) : IntPtr.Zero;
                if (args[0] == "--record-wait" && checkedJob == IntPtr.Zero) ThrowNativeError();
                bool inJob;
                try
                {
                    if (!IsProcessInJob(GetCurrentProcess(), checkedJob, out inJob)) ThrowNativeError();
                }
                finally
                {
                    if (checkedJob != IntPtr.Zero) CloseHandle(checkedJob);
                }
                WriteRecord(args[1], new LaunchRecord(Environment.ProcessId, args.Skip(args[0] == "--record" ? 2 : 5).ToArray(), inJob));
                if (args[0] == "--record-wait")
                {
                    WaitForFile(args[2]);
                    File.WriteAllText(args[3], "finished");
                }
                return 0;

            case "--job-parent":
                RunJobParent(args[1], args[2]);
                return 0;

            case "--handle-check":
                RunHandleCheck(args[1], args[2]);
                return 0;

            case "-NativeMessagingInput":
                // Synthetic browser payloads specify only this fixture's own output path.
                string payload = File.ReadAllText(args[1]);
                using (JsonDocument json = JsonDocument.Parse(payload))
                {
                    string output = json.RootElement.GetProperty("TestOutputFile").GetString()!;
                    WriteRecord(output, new BrowserLaunchRecord(Environment.ProcessId, args, payload));
                }
                File.Delete(args[1]);
                return 0;

            default:
                return 2;
        }
    }

    [SupportedOSPlatform("windows")]
    private static void RunJobParent(string directory, string executable)
    {
        string name = "ShareX-launch-fixture-" + Guid.NewGuid().ToString("N");
        IntPtr job = CreateJobObject(IntPtr.Zero, name);
        if (job == IntPtr.Zero) ThrowNativeError();
        try
        {
            JobLimits limits = new();
            limits.Basic.Flags = 0x00000800 | 0x00002000; // BREAKAWAY_OK | KILL_ON_JOB_CLOSE
            if (!SetInformationJobObject(job, 9, ref limits, (uint)Marshal.SizeOf<JobLimits>())) ThrowNativeError();
            // Only this generated parent process joins the job, never the test host or another application.
            if (!AssignProcessToJobObject(job, GetCurrentProcess())) ThrowNativeError();
            if (!IsProcessInJob(GetCurrentProcess(), job, out bool inJob) || !inJob)
                throw new InvalidOperationException("The fixture parent did not join its own job.");

            string record = Path.Combine(directory, "child.json");
            int child = new WindowsApplicationLaunchService().LaunchDetached(executable,
                ["--record-wait", record, Path.Combine(directory, "release"), Path.Combine(directory, "finished"), name]);
            WriteRecord(Path.Combine(directory, "parent.json"), new JobParentRecord(Environment.ProcessId, child, inJob));
            WaitForFile(record);
            File.WriteAllText(Path.Combine(directory, "parent-closing-job"), "closing");
        }
        finally
        {
            // This kills this parent. A correctly detached child survives to receive the release file from the test.
            CloseHandle(job);
        }
        File.WriteAllText(Path.Combine(directory, "parent-survived-job-close"), "unexpected");
    }

    [SupportedOSPlatform("windows")]
    private static void RunHandleCheck(string directory, string executable)
    {
        WindowsApplicationLaunchService service = new();
        using Process process = Process.GetCurrentProcess();
        for (int index = 0; index < 4; index++) LaunchOnce(index);
        FailOnce();
        CollectAndRefresh();
        int before = process.HandleCount;
        for (int index = 0; index < 16; index++) { LaunchOnce(index + 4); FailOnce(); }
        CollectAndRefresh();
        WriteRecord(Path.Combine(directory, "handles.json"), new HandleRecord(before, process.HandleCount));

        void LaunchOnce(int index)
        {
            string record = Path.Combine(directory, "handles-child-" + index + ".json");
            int id = service.LaunchDetached(executable, ["--record", record]);
            try
            {
                using Process child = Process.GetProcessById(id);
                if (!child.WaitForExit(20000))
                {
                    child.Kill();
                    throw new TimeoutException("Handle fixture child did not exit.");
                }
            }
            catch (ArgumentException) { }
            WaitForFile(record);
        }

        void FailOnce()
        {
            try { service.LaunchDetached(Path.Combine(directory, "missing.exe"), []); }
            catch (Win32Exception error) when (error.NativeErrorCode == 2) { return; }
            throw new InvalidOperationException("Missing executable did not fail with ERROR_FILE_NOT_FOUND.");
        }

        void CollectAndRefresh()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            process.Refresh();
        }
    }

    internal static void WriteRecord<T>(string path, T record)
    {
        File.WriteAllText(path + ".writing", JsonSerializer.Serialize(record));
        File.Move(path + ".writing", path, true);
    }

    internal static void WaitForFile(string path)
    {
        Stopwatch timeout = Stopwatch.StartNew();
        while (!File.Exists(path))
        {
            if (timeout.Elapsed > TimeSpan.FromSeconds(20)) throw new TimeoutException("Missing fixture file: " + path);
            Thread.Sleep(20);
        }
    }

    private static void ThrowNativeError() => throw new Win32Exception(Marshal.GetLastWin32Error());

    [StructLayout(LayoutKind.Sequential)]
    private struct BasicJobLimits
    {
        public long ProcessTime, JobTime;
        public uint Flags;
        public UIntPtr MinimumWorkingSet, MaximumWorkingSet;
        public uint ActiveProcessLimit;
        public UIntPtr Affinity;
        public uint PriorityClass, SchedulingClass;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct JobLimits
    {
        public BasicJobLimits Basic;
        public ulong ReadOperations, WriteOperations, OtherOperations, ReadBytes, WriteBytes, OtherBytes;
        public UIntPtr ProcessMemory, JobMemory, PeakProcessMemory, PeakJobMemory;
    }

    [DllImport("kernel32.dll", EntryPoint = "CreateJobObjectW", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    private static extern IntPtr CreateJobObject(IntPtr attributes, string? name);

    [DllImport("kernel32.dll", EntryPoint = "OpenJobObjectW", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    private static extern IntPtr OpenJobObject(uint access, [MarshalAs(UnmanagedType.Bool)] bool inheritHandle, string name);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetInformationJobObject(IntPtr job, int informationClass, ref JobLimits limits, uint length);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsProcessInJob(IntPtr process, IntPtr job, [MarshalAs(UnmanagedType.Bool)] out bool result);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr handle);
}

internal sealed record LaunchRecord(int ProcessId, string[] Arguments, bool InJob);
internal sealed record JobParentRecord(int ProcessId, int ChildProcessId, bool InJob);
internal sealed record HandleRecord(int Before, int After);
internal sealed record BrowserLaunchRecord(int ProcessId, string[] Arguments, string Payload);
