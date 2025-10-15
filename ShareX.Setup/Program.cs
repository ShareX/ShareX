#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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

using Microsoft.Win32;
using ShareX.HelpersLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

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
            CreateDebug = 1 << 2,
            CreateSteamFolder = 1 << 3,
            CreateMicrosoftStoreFolder = 1 << 4,
            CreateMicrosoftStoreDebugFolder = 1 << 5,
            CompileAppx = 1 << 6,
            DownloadTools = 1 << 7,
            CreateChecksumFile = 1 << 8,
            OpenOutputDirectory = 1 << 9,
            CreateArm64 = 1 << 10,

            Release = CreateSetup | CreatePortable | DownloadTools | OpenOutputDirectory,
            ReleaseArm64 = CreateSetup | CreatePortable | DownloadTools | OpenOutputDirectory | CreateArm64,
            Debug = CreateDebug | DownloadTools | OpenOutputDirectory,
            Steam = CreateSteamFolder | DownloadTools | OpenOutputDirectory,
            MicrosoftStore = CreateMicrosoftStoreFolder | CompileAppx | DownloadTools | OpenOutputDirectory,
            MicrosoftStoreDebug = CreateMicrosoftStoreDebugFolder | CompileAppx | DownloadTools | OpenOutputDirectory
        }

        private static SetupJobs Job { get; set; } = SetupJobs.Release;
        private static bool Silent { get; set; } = false;

        // Initialize with safe defaults to avoid null usage before UpdatePaths() runs
        private static string ParentDir = Directory.GetCurrentDirectory();
        private static string Configuration = "Release"; // default until UpdatePaths() adjusts based on Job
        private static string AppVersion = "0.0.0"; // will be updated in UpdatePaths()
        private static string WindowsKitsDir = string.Empty; // will be updated when compiling Appx

        private static string SolutionPath => Path.Combine(ParentDir, "ShareX.sln");
        private const string WinArm64 = "win-arm64";
        private const string WinX64 = "win-x64";
        private static string CurrentWinArch => Job.HasFlag(SetupJobs.CreateArm64) ? WinArm64 : WinX64;

        private static string BinDir => Path.Combine(ParentDir, "ShareX", "bin", Configuration, CurrentWinArch);
        private static string SteamLauncherDir => Path.Combine(ParentDir, "ShareX.Steam", "bin", CurrentWinArch, Configuration);
        private static string ExecutablePath => Path.Combine(BinDir, "ShareX.exe");

        private static string OutputDir => Path.Combine(ParentDir, "Output");
        private static string PortableOutputDir => Path.Combine(OutputDir, "ShareX-portable");
        private static string DebugOutputDir => Path.Combine(OutputDir, "ShareX-debug");
        private static string SteamOutputDir => Path.Combine(OutputDir, "ShareX-Steam");
        private static string MicrosoftStoreOutputDir => Path.Combine(OutputDir, "ShareX-MicrosoftStore");
        private static string MicrosoftStoreDebugOutputDir => Path.Combine(OutputDir, "ShareX-MicrosoftStore-debug");

        private static string SetupDir => Path.Combine(ParentDir, "ShareX.Setup");
        private static string InnoSetupDir => Path.Combine(SetupDir, "InnoSetup");
        private static string MicrosoftStorePackageFilesDir => Path.Combine(SetupDir, "MicrosoftStore");

        private static string SetupPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-{CurrentWinArch}-setup.exe");
        private static string PortableZipPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-{CurrentWinArch}-portable.zip");
        private static string DebugZipPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-{CurrentWinArch}-debug.zip");
        private static string SteamUpdatesDir => Path.Combine(SteamOutputDir, "Updates");
        private static string SteamZipPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-Steam.zip");
        private static string MicrosoftStoreAppxPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}.appx");
        private static string MicrosoftStoreDebugAppxPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-debug.appx");
        private static string FFmpegPath => Path.Combine(OutputDir, "ffmpeg.exe");
        private static string RecorderDevicesSetupPath => Path.Combine(OutputDir, $"recorder-devices-{RecorderDevicesVersion}-setup.exe");
        private static string ExifToolPath => Path.Combine(OutputDir, "exiftool.exe");
        private static string MakeAppxPath => Path.Combine(WindowsKitsDir, "x64", "makeappx.exe");

        private const string InnoSetupCompilerPath = @"C:\Program Files (x86)\Inno Setup 6\ISCC.exe";
        private const string FFmpegVersion = "8.0";
        private static string FFmpegDownloadURL => $"https://github.com/ShareX/FFmpeg/releases/download/v{FFmpegVersion}/ffmpeg-{FFmpegVersion}-win-{(Job.HasFlag(SetupJobs.CreateArm64) ? "arm64" : "x64")}.zip";
        private const string RecorderDevicesVersion = "0.12.10";
        private static string RecorderDevicesDownloadURL = $"https://github.com/ShareX/RecorderDevices/releases/download/v{RecorderDevicesVersion}/recorder-devices-{RecorderDevicesVersion}-setup.exe";
        private const string ExifToolVersion = "13.29";
        private static string ExifToolDownloadURL = $"https://github.com/ShareX/ExifTool/releases/download/v{ExifToolVersion}/exiftool-{ExifToolVersion}-win64.zip";
        private static void Main(string[] args)
        {
            Console.WriteLine("ShareX setup started.");

            CheckArgs(args);

            UpdatePaths();

            Console.WriteLine("Job: " + Job);

            if (Directory.Exists(OutputDir))
            {
                Console.WriteLine("Cleaning output directory: " + OutputDir);

                Directory.Delete(OutputDir, true);
            }

            if (Job.HasFlag(SetupJobs.DownloadTools))
            {
                DownloadFFmpeg();
                DownloadRecorderDevices();
                DownloadExifTool();
            }

            if (Job.HasFlag(SetupJobs.CreateSetup))
            {
                CompileSetup();
            }

            if (Job.HasFlag(SetupJobs.CreatePortable))
            {
                CreateFolder(BinDir, PortableOutputDir, SetupJobs.CreatePortable);

                CreateZipFile(PortableOutputDir, PortableZipPath);
            }

            if (Job.HasFlag(SetupJobs.CreateDebug))
            {
                CreateFolder(BinDir, DebugOutputDir, SetupJobs.CreateDebug);

                CreateZipFile(DebugOutputDir, DebugZipPath);
            }

            if (Job.HasFlag(SetupJobs.CreateSteamFolder))
            {
                CreateSteamFolder();

                CreateZipFile(SteamOutputDir, SteamZipPath);
            }

            if (Job.HasFlag(SetupJobs.CreateMicrosoftStoreFolder))
            {
                CreateFolder(BinDir, MicrosoftStoreOutputDir, SetupJobs.CreateMicrosoftStoreFolder);

                if (Job.HasFlag(SetupJobs.CompileAppx))
                {
                    CompileAppx(MicrosoftStoreOutputDir, MicrosoftStoreAppxPath);
                }
            }
            if (Job.HasFlag(SetupJobs.CreateMicrosoftStoreDebugFolder))
            {
                CreateFolder(BinDir, MicrosoftStoreDebugOutputDir, SetupJobs.CreateMicrosoftStoreDebugFolder);

                if (Job.HasFlag(SetupJobs.CompileAppx))
                {
                    CompileAppx(MicrosoftStoreDebugOutputDir, MicrosoftStoreDebugAppxPath);
                }
            }

            if (!Silent && Job.HasFlag(SetupJobs.OpenOutputDirectory))
            {
                FileHelpers.OpenFolder(OutputDir, false);
            }

            Console.WriteLine("ShareX setup successfully completed.");
        }

        private static void CheckArgs(string[] args)
        {
            CLIManager cli = new CLIManager(args);
            cli.ParseCommands();

            Silent = cli.IsCommandExist("Silent");

            if (Silent)
            {
                Console.WriteLine("Silent: " + Silent);
            }

            CLICommand command = cli.GetCommand("Job");

            if (command != null)
            {
                string parameter = command.Parameter;

                if (Enum.TryParse(parameter, out SetupJobs job))
                {
                    Job = job;
                    Console.WriteLine("IsArm64: " + job.HasFlag(SetupJobs.CreateArm64));
                }
                else
                {
                    Console.WriteLine("Invalid job: " + parameter);

                    Environment.Exit(0);
                }
            }
        }

        private static void UpdatePaths()
        {
            ParentDir = Directory.GetCurrentDirectory();

            if (!File.Exists(SolutionPath))
            {
                Console.WriteLine("Invalid parent directory: " + ParentDir);

                ParentDir = FileHelpers.GetAbsolutePath(@"..\..\..\");

                if (!File.Exists(SolutionPath))
                {
                    Console.WriteLine("Invalid parent directory: " + ParentDir);

                    Environment.Exit(0);
                }
            }

            Console.WriteLine("Parent directory: " + ParentDir);

            UpdateBuildConfig();

            Console.WriteLine("Configuration: " + Configuration);
            Console.WriteLine("Architecture: " + CurrentWinArch);
            Console.WriteLine("ExecutablePath (expected): " + ExecutablePath);

            // Determine the correct executable path for reading version info.
            string versionSourcePath = ExecutablePath;
            if (!File.Exists(versionSourcePath))
            {
                string fallbackArch = Job.HasFlag(SetupJobs.CreateArm64) ? WinX64 : WinArm64;
                string fallbackBinDir = Path.Combine(ParentDir, "ShareX", "bin", Configuration, fallbackArch);
                string fallbackExe = Path.Combine(fallbackBinDir, "ShareX.exe");

                if (File.Exists(fallbackExe))
                {
                    Console.WriteLine($"Executable not found at expected path. Falling back to {fallbackArch} for version info: {fallbackExe}");
                    versionSourcePath = fallbackExe;

                    if (fallbackArch.Equals(WinArm64, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Switching job to ReleaseArm64 due to version detection fallback.");
                        Job = SetupJobs.ReleaseArm64;
                    }
                }
                else
                {
                    Console.WriteLine("Executable not found for version detection at either path:");
                    Console.WriteLine(" - Expected: " + ExecutablePath);
                    Console.WriteLine(" - Fallback: " + fallbackExe);
                }
            }

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(versionSourcePath);
            AppVersion = versionInfo.ProductVersion;

            Console.WriteLine("Application version: " + AppVersion);

            if (Job.HasFlag(SetupJobs.CompileAppx))
            {
                string sdkInstallationFolder = RegistryHelpers.GetValueString(@"SOFTWARE\WOW6432Node\Microsoft\Microsoft SDKs\Windows\v10.0",
                    "InstallationFolder", RegistryHive.LocalMachine);
                string sdkProductVersion = RegistryHelpers.GetValueString(@"SOFTWARE\WOW6432Node\Microsoft\Microsoft SDKs\Windows\v10.0",
                    "ProductVersion", RegistryHive.LocalMachine);
                WindowsKitsDir = Path.Combine(sdkInstallationFolder, "bin", Helpers.NormalizeVersion(sdkProductVersion).ToString());

                Console.WriteLine("Windows Kits directory: " + WindowsKitsDir);
            }
        }

        private static void UpdateBuildConfig()
        {
            if (Job.HasFlag(SetupJobs.CreateDebug))
            {
                Configuration = "Debug";
            }
            else if (Job.HasFlag(SetupJobs.CreateSteamFolder))
            {
                Configuration = "Steam";
            }
            else if (Job.HasFlag(SetupJobs.CreateMicrosoftStoreFolder))
            {
                Configuration = "MicrosoftStore";
            }
            else if (Job.HasFlag(SetupJobs.CreateMicrosoftStoreDebugFolder))
            {
                Configuration = "MicrosoftStoreDebug";
            }
            else
            {
                Configuration = "Release";
            }
        }

        private static void CompileSetup()
        {
            CompileISSFile("ShareX-setup.iss");
            CreateChecksumFile(SetupPath);
        }

        private static void CompileISSFile(string fileName)
        {
            if (File.Exists(InnoSetupCompilerPath))
            {
                Console.WriteLine("Compiling setup file: " + fileName);

                using (Process process = new Process())
                {
                    string arguments = Job.HasFlag(SetupJobs.CreateArm64) ? $"/Q /DArch=arm64 \"{fileName}\"" : $"/Q \"{fileName}\"";

                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        FileName = InnoSetupCompilerPath,
                        WorkingDirectory = InnoSetupDir,
                        Arguments = arguments,
                        UseShellExecute = false
                    };

                    process.StartInfo = psi;
                    process.Start();
                    process.WaitForExit();
                }

                Console.WriteLine("Setup file compiled: " + fileName);
            }
            else
            {
                Console.WriteLine("InnoSetup compiler is missing: " + InnoSetupCompilerPath);
            }
        }

        private static void CompileAppx(string contentDirectory, string outputPackageName)
        {
            Console.WriteLine("Compiling appx file: " + contentDirectory);

            using (Process process = new Process())
            {
                ProcessStartInfo psi = new ProcessStartInfo()
                {
                    FileName = MakeAppxPath,
                    Arguments = $"pack /d \"{contentDirectory}\" /p \"{outputPackageName}\" /l /o",
                    UseShellExecute = false
                };

                process.StartInfo = psi;
                process.Start();
                process.WaitForExit();
            }

            Console.WriteLine("Appx file compiled: " + outputPackageName);

            CreateChecksumFile(outputPackageName);
        }

        private static void CreateSteamFolder()
        {
            Console.WriteLine("Creating Steam folder: " + SteamOutputDir);

            if (Directory.Exists(SteamOutputDir))
            {
                Directory.Delete(SteamOutputDir, true);
            }

            Directory.CreateDirectory(SteamOutputDir);

            FileHelpers.CopyFiles(Path.Combine(SteamLauncherDir, "ShareX_Launcher.exe"), SteamOutputDir);
            FileHelpers.CopyFiles(Path.Combine(SteamLauncherDir, "steam_appid.txt"), SteamOutputDir);
            FileHelpers.CopyFiles(Path.Combine(SteamLauncherDir, "installscript.vdf"), SteamOutputDir);
            FileHelpers.CopyFiles(SteamLauncherDir, SteamOutputDir, "*.dll");

            CreateFolder(BinDir, SteamUpdatesDir, SetupJobs.CreateSteamFolder);
        }

        private static void CreateFolder(string source, string destination, SetupJobs job)
        {
            Console.WriteLine("Creating folder: " + destination);

            if (Directory.Exists(destination))
            {
                Directory.Delete(destination, true);
            }

            Directory.CreateDirectory(destination);

            FileHelpers.CopyFiles(source, destination, "*.exe");
            FileHelpers.CopyFiles(source, destination, "*.dll");
            FileHelpers.CopyFiles(source, destination, "*.json");

            if (job == SetupJobs.CreateDebug || job == SetupJobs.CreateMicrosoftStoreDebugFolder)
            {
                FileHelpers.CopyFiles(source, destination, "*.pdb");
            }

            FileHelpers.CopyFiles(Path.Combine(ParentDir, "Licenses"), Path.Combine(destination, "Licenses"), "*.txt");

            if (job != SetupJobs.CreateMicrosoftStoreFolder && job != SetupJobs.CreateMicrosoftStoreDebugFolder)
            {
                if (File.Exists(RecorderDevicesSetupPath))
                {
                    FileHelpers.CopyFiles(RecorderDevicesSetupPath, destination);
                }
            }

            FileHelpers.CopyFiles(Path.Combine(source, "ShareX_File_Icon.ico"), destination);

            foreach (string directory in Directory.GetDirectories(source))
            {
                string language = Path.GetFileName(directory);

                if (Regex.IsMatch(language, "^[a-z]{2}(?:-[A-Z]{2})?$"))
                {
                    FileHelpers.CopyFiles(Path.Combine(source, language), Path.Combine(destination, "Languages", language), "*.resources.dll");
                }
            }

            if (File.Exists(FFmpegPath))
            {
                FileHelpers.CopyFiles(FFmpegPath, destination);
            }

            if (File.Exists(ExifToolPath))
            {
                FileHelpers.CopyFiles(ExifToolPath, destination);
                FileHelpers.CopyAll(Path.Combine(OutputDir, "exiftool_files"), Path.Combine(destination, "exiftool_files"));
            }

            FileHelpers.CopyAll(Path.Combine(ParentDir, @"ShareX.ScreenCaptureLib\Stickers"), Path.Combine(destination, "Stickers"));

            if (job == SetupJobs.CreatePortable)
            {
                FileHelpers.CreateEmptyFile(Path.Combine(destination, "Portable"));
            }
            else if (job == SetupJobs.CreateMicrosoftStoreFolder || job == SetupJobs.CreateMicrosoftStoreDebugFolder)
            {
                FileHelpers.CopyAll(MicrosoftStorePackageFilesDir, destination);
            }

            Console.WriteLine("Folder created: " + destination);
        }

        private static void CreateZipFile(string source, string archivePath)
        {
            Console.WriteLine("Creating zip file: " + archivePath);

            ZipManager.Compress(source, archivePath);
            CreateChecksumFile(archivePath);
        }

        private static void DownloadFFmpeg()
        {
            if (!File.Exists(FFmpegPath))
            {
                string fileName = Path.GetFileName(FFmpegDownloadURL);
                string filePath = Path.Combine(OutputDir, fileName);

                Console.WriteLine("Downloading: " + FFmpegDownloadURL);
                WebHelpers.DownloadFileAsync(FFmpegDownloadURL, filePath).GetAwaiter().GetResult();

                Console.WriteLine("Extracting: " + filePath);
                ZipManager.Extract(filePath, OutputDir, false, entry => entry.Name.Equals("ffmpeg.exe", StringComparison.OrdinalIgnoreCase));
            }
        }

        private static void DownloadRecorderDevices()
        {
            if (!File.Exists(RecorderDevicesSetupPath))
            {
                string fileName = Path.GetFileName(RecorderDevicesDownloadURL);
                string filePath = Path.Combine(OutputDir, fileName);

                Console.WriteLine("Downloading: " + RecorderDevicesDownloadURL);
                WebHelpers.DownloadFileAsync(RecorderDevicesDownloadURL, filePath).GetAwaiter().GetResult();
            }
        }

        private static void DownloadExifTool()
        {
            if (!File.Exists(ExifToolPath))
            {
                string fileName = Path.GetFileName(ExifToolDownloadURL);
                string filePath = Path.Combine(OutputDir, fileName);

                Console.WriteLine("Downloading: " + ExifToolDownloadURL);
                WebHelpers.DownloadFileAsync(ExifToolDownloadURL, filePath).GetAwaiter().GetResult();

                Console.WriteLine("Extracting: " + filePath);
                ZipManager.Extract(filePath, OutputDir);
            }
        }

        private static void CreateChecksumFile(string filePath)
        {
            if (Job.HasFlag(SetupJobs.CreateChecksumFile))
            {
                Console.WriteLine("Creating checksum file: " + filePath);

                Helpers.CreateChecksumFile(filePath);
            }
        }
    }
}