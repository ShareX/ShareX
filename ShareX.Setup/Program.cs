#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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

using ShareX.HelpersLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ShareX.Setup
{
    internal class Program
    {
        [Flags]
        private enum SetupJobs
        {
            None = 0,
            CreateSetup = 1,
            CreatePortable = 1 << 1,
            CreateSteamFolder = 1 << 2,
            CreatePortableAppsFolder = 1 << 3,
            OpenOutputDirectory = 1 << 4,
            UploadOutputFile = 1 << 5,
            CreateWindowsStoreFolder = 1 << 6,
            CreateWindowsStoreDebugFolder = 1 << 7,
            CompileAppx = 1 << 8,
            DownloadFFmpeg = 1 << 9,

            Stable = CreateSetup | CreatePortable | OpenOutputDirectory,
            Setup = CreateSetup | OpenOutputDirectory,
            Portable = CreatePortable | OpenOutputDirectory,
            Steam = CreateSteamFolder | OpenOutputDirectory,
            WindowsStore = CreateWindowsStoreFolder,
            WindowsStoreDebug = CreateWindowsStoreDebugFolder,
            PortableApps = CreatePortableAppsFolder | OpenOutputDirectory,
            Beta = CreateSetup | UploadOutputFile,
            AppVeyorRelease = CreateSetup | CreatePortable,
            AppVeyorSteam = CreateSteamFolder,
            AppVeyorWindowsStore = CreateWindowsStoreFolder | CompileAppx,
            AppVeyorSteamRelease = AppVeyorSteam | DownloadFFmpeg,
            AppVeyorWindowsStoreRelease = AppVeyorWindowsStore | DownloadFFmpeg
        }

        private static SetupJobs Job = SetupJobs.Stable;
        private static bool AppVeyor = false;

        private static string ParentDir => AppVeyor ? "." : @"..\..\..\";
        private static string BinDir => Path.Combine(ParentDir, "ShareX", "bin");
        private static string ReleaseDir => Path.Combine(BinDir, "Release");
        private static string DebugDir => Path.Combine(BinDir, "Debug");
        private static string DebugExecutablePath => Path.Combine(DebugDir, "ShareX.exe");
        private static string SteamDir => Path.Combine(BinDir, "Steam");
        private static string WindowsStoreDir => Path.Combine(BinDir, "WindowsStore");
        private static string WindowsStoreDebugDir => Path.Combine(BinDir, "WindowsStoreDebug");

        private static string OutputDir => Path.Combine(ParentDir, "Output");
        private static string PortableOutputDir => Path.Combine(OutputDir, "ShareX-portable");
        private static string SteamOutputDir => Path.Combine(OutputDir, "ShareX-Steam");
        private static string WindowsStoreOutputDir => Path.Combine(OutputDir, "ShareX-WindowsStore");

        private static string SetupDir => Path.Combine(ParentDir, "ShareX.Setup");
        private static string InnoSetupDir => Path.Combine(SetupDir, "InnoSetup");
        private static string WindowsStorePackageFilesDir => Path.Combine(SetupDir, "WindowsStore");
        private static string PortableAppsOutputDir => Path.Combine(ParentDir, @"..\PortableApps\ShareXPortable\App\ShareX");

        private static string SteamLauncherDir => Path.Combine(ParentDir, @"ShareX.Steam\bin\Release");
        private static string SteamUpdatesDir => Path.Combine(SteamOutputDir, "Updates");
        private static string NativeMessagingHostDir => Path.Combine(ParentDir, @"ShareX.NativeMessagingHost\bin\Release");
        private static string RecorderDevicesSetupPath => Path.Combine(OutputDir, "Recorder-devices-setup.exe");
        private static string WindowsStoreAppxPath => Path.Combine(OutputDir, "ShareX.appx");

        public static string InnoSetupCompilerPath = @"C:\Program Files (x86)\Inno Setup 6\ISCC.exe";
        public static string FFmpeg32bit => Path.Combine(OutputDir, "ffmpeg.exe");
        public static string FFmpeg64bit => Path.Combine(OutputDir, "ffmpeg-x64.exe");
        public static string MakeAppxPath = @"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x64\makeappx.exe";

        private static void Main(string[] args)
        {
            Console.WriteLine("ShareX setup started.");

            if (SetupHelpers.CheckArguments(args, "-AppVeyorRelease"))
            {
                AppVeyor = true;
                Job = SetupJobs.AppVeyorRelease;
            }
            else if (SetupHelpers.CheckArguments(args, "-AppVeyorSteam"))
            {
                AppVeyor = true;
                Job = SetupJobs.AppVeyorSteam;
            }
            else if (SetupHelpers.CheckArguments(args, "-AppVeyorWindowsStore"))
            {
                AppVeyor = true;
                Job = SetupJobs.AppVeyorWindowsStore;
            }
            else if (SetupHelpers.CheckArguments(args, "-AppVeyorSteamRelease"))
            {
                AppVeyor = true;
                Job = SetupJobs.AppVeyorSteamRelease;
            }
            else if (SetupHelpers.CheckArguments(args, "-AppVeyorWindowsStoreRelease"))
            {
                AppVeyor = true;
                Job = SetupJobs.AppVeyorWindowsStoreRelease;
            }

            Console.WriteLine("Setup job: " + Job);

            if (Directory.Exists(OutputDir))
            {
                Console.WriteLine("Cleaning output directory: " + OutputDir);

                Directory.Delete(OutputDir, true);
            }

            if (Job.HasFlag(SetupJobs.CreateSetup))
            {
                CompileSetup();
            }

            if (Job.HasFlag(SetupJobs.CreatePortable))
            {
                CreateFolder(ReleaseDir, PortableOutputDir, SetupJobs.CreatePortable);
            }

            if (Job.HasFlag(SetupJobs.CreateSteamFolder))
            {
                CreateSteamFolder();

                if (Job.HasFlag(SetupJobs.DownloadFFmpeg))
                {
                    CopyFFmpeg(SteamUpdatesDir, true, true);
                }
            }

            if (Job.HasFlag(SetupJobs.CreateWindowsStoreFolder))
            {
                CreateFolder(WindowsStoreDir, WindowsStoreOutputDir, SetupJobs.CreateWindowsStoreFolder);

                if (Job.HasFlag(SetupJobs.DownloadFFmpeg))
                {
                    CopyFFmpeg(WindowsStoreOutputDir, false, true);
                }
            }

            if (Job.HasFlag(SetupJobs.CreateWindowsStoreDebugFolder))
            {
                CreateFolder(WindowsStoreDebugDir, WindowsStoreOutputDir, SetupJobs.CreateWindowsStoreDebugFolder);
            }

            if (Job.HasFlag(SetupJobs.CompileAppx))
            {
                using (Process process = new Process())
                {
                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        FileName = MakeAppxPath,
                        Arguments = $"pack /d \"{WindowsStoreOutputDir}\" /p \"{WindowsStoreAppxPath}\" /l /o",
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    };

                    process.OutputDataReceived += (s, e) => Console.WriteLine(e.Data);
                    process.StartInfo = psi;
                    process.Start();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                }

                Directory.Delete(WindowsStoreOutputDir, true);
            }

            if (Job.HasFlag(SetupJobs.CreatePortableAppsFolder))
            {
                CreateFolder(ReleaseDir, PortableAppsOutputDir, SetupJobs.CreatePortableAppsFolder);
            }

            if (AppVeyor)
            {
                Helpers.CopyAll(OutputDir, ParentDir);
            }

            if (Job.HasFlag(SetupJobs.OpenOutputDirectory))
            {
                OpenOutputDirectory();
            }

            if (Job.HasFlag(SetupJobs.UploadOutputFile))
            {
                UploadLatestFile();
            }

            Console.WriteLine("ShareX setup successfully completed.");
        }

        private static void CompileSetup()
        {
            CompileISSFile("Recorder-devices-setup.iss");
            CompileISSFile("ShareX-setup.iss");
        }

        private static void CompileISSFile(string filename)
        {
            if (File.Exists(InnoSetupCompilerPath))
            {
                Console.WriteLine("Compiling setup file: " + filename);

                using (Process process = new Process())
                {
                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        FileName = InnoSetupCompilerPath,
                        WorkingDirectory = Path.GetFullPath(InnoSetupDir),
                        Arguments = $"\"{filename}\"",
                        UseShellExecute = false
                    };

                    process.StartInfo = psi;
                    process.Start();
                    process.WaitForExit();
                }

                Console.WriteLine("Setup file is created.");
            }
            else
            {
                Console.WriteLine("InnoSetup compiler is missing: " + InnoSetupCompilerPath);
            }
        }

        private static void CreateSteamFolder()
        {
            Console.WriteLine("Creating Steam folder:" + SteamOutputDir);

            if (Directory.Exists(SteamOutputDir))
            {
                Directory.Delete(SteamOutputDir, true);
            }

            Directory.CreateDirectory(SteamOutputDir);

            SetupHelpers.CopyFile(Path.Combine(SteamLauncherDir, "ShareX_Launcher.exe"), SteamOutputDir);
            SetupHelpers.CopyFile(Path.Combine(SteamLauncherDir, "steam_appid.txt"), SteamOutputDir);
            SetupHelpers.CopyFile(Path.Combine(SteamLauncherDir, "installscript.vdf"), SteamOutputDir);
            SetupHelpers.CopyFiles(SteamLauncherDir, "*.dll", SteamOutputDir);

            CreateFolder(SteamDir, SteamUpdatesDir, SetupJobs.CreateSteamFolder);
        }

        private static void CreateFolder(string source, string destination, SetupJobs job)
        {
            Console.WriteLine("Creating folder: " + destination);

            if (Directory.Exists(destination))
            {
                Directory.Delete(destination, true);
            }

            Directory.CreateDirectory(destination);

            SetupHelpers.CopyFile(Path.Combine(source, "ShareX.exe"), destination);
            SetupHelpers.CopyFile(Path.Combine(source, "ShareX.exe.config"), destination);
            SetupHelpers.CopyFiles(source, "*.dll", destination);

            if (job == SetupJobs.CreateWindowsStoreDebugFolder)
            {
                SetupHelpers.CopyFiles(source, "*.pdb", destination);
            }

            SetupHelpers.CopyFiles(Path.Combine(ParentDir, "Licenses"), "*.txt", Path.Combine(destination, "Licenses"));

            if (job != SetupJobs.CreateWindowsStoreFolder && job != SetupJobs.CreateWindowsStoreDebugFolder)
            {
                if (!File.Exists(RecorderDevicesSetupPath))
                {
                    CompileISSFile("Recorder-devices-setup.iss");
                }

                SetupHelpers.CopyFile(RecorderDevicesSetupPath, destination);

                SetupHelpers.CopyFile(Path.Combine(NativeMessagingHostDir, "ShareX_NativeMessagingHost.exe"), destination);
            }

            string[] languages = new string[] { "de", "es", "es-MX", "fa-IR", "fr", "hu", "id-ID", "it-IT", "ko-KR", "nl-NL", "pt-BR", "pt-PT", "ru", "tr", "uk", "vi-VN", "zh-CN", "zh-TW" };

            foreach (string language in languages)
            {
                SetupHelpers.CopyFiles(Path.Combine(source, language), "*.resources.dll", Path.Combine(destination, "Languages", language));
            }

            Helpers.CopyAll(Path.Combine(ParentDir, @"ShareX.ScreenCaptureLib\Stickers"), Path.Combine(destination, "Stickers"));

            if (job == SetupJobs.CreatePortableAppsFolder)
            {
                Helpers.CreateEmptyFile(Path.Combine(destination, "PortableApps"));
            }
            else if (job == SetupJobs.CreateWindowsStoreFolder || job == SetupJobs.CreateWindowsStoreDebugFolder)
            {
                Helpers.CopyAll(WindowsStorePackageFilesDir, destination);
            }
            else if (job == SetupJobs.CreatePortable)
            {
                Helpers.CreateEmptyFile(Path.Combine(destination, "Portable"));

                //FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(releaseDir, "ShareX.exe"));
                //string zipFilename = string.Format("ShareX-{0}.{1}.{2}-portable.zip", versionInfo.ProductMajorPart, versionInfo.ProductMinorPart, versionInfo.ProductBuildPart);
                string zipPath = Path.Combine(OutputDir, "ShareX-portable.zip");
                ZipManager.Compress(Path.GetFullPath(destination), Path.GetFullPath(zipPath));

                Directory.Delete(destination, true);
            }

            Console.WriteLine("Folder created.");
        }

        private static void CopyFFmpeg(string destination, bool include32bit, bool include64bit)
        {
            if (include32bit)
            {
                if (!File.Exists(FFmpeg32bit))
                {
                    string filePath = SetupHelpers.DownloadFile("https://github.com/ShareX/FFmpeg/releases/download/v4.3.1/ffmpeg-4.3.1-win32.zip");
                    ZipManager.Extract(filePath, ".", false, entry => entry.Name.Equals("ffmpeg.exe", StringComparison.OrdinalIgnoreCase), 100_000_000);
                    File.Move("ffmpeg.exe", FFmpeg32bit);
                }

                SetupHelpers.CopyFile(FFmpeg32bit, destination);
            }

            if (include64bit)
            {
                if (!File.Exists(FFmpeg64bit))
                {
                    string filePath = SetupHelpers.DownloadFile("https://github.com/ShareX/FFmpeg/releases/download/v4.3.1/ffmpeg-4.3.1-win64.zip");
                    ZipManager.Extract(filePath, ".", false, entry => entry.Name.Equals("ffmpeg.exe", StringComparison.OrdinalIgnoreCase), 100_000_000);
                    File.Move("ffmpeg.exe", FFmpeg64bit);
                }

                SetupHelpers.CopyFile(FFmpeg64bit, destination);
            }
        }

        private static void OpenOutputDirectory()
        {
            Process.Start(OutputDir);
        }

        private static void UploadLatestFile()
        {
            FileInfo fileInfo = new DirectoryInfo(OutputDir).GetFiles("*.exe").OrderByDescending(f => f.LastWriteTime).FirstOrDefault();

            if (fileInfo != null)
            {
                Console.WriteLine("Uploading setup file.");
                Process.Start(DebugExecutablePath, fileInfo.FullName);
            }
        }
    }
}