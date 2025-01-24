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
            DownloadFFmpeg = 1 << 7,
            CreateChecksumFile = 1 << 8,
            OpenOutputDirectory = 1 << 9,

            Release = CreateSetup | CreatePortable | DownloadFFmpeg | OpenOutputDirectory,
            Debug = CreateDebug | DownloadFFmpeg | OpenOutputDirectory,
            Steam = CreateSteamFolder | DownloadFFmpeg | OpenOutputDirectory,
            MicrosoftStore = CreateMicrosoftStoreFolder | CompileAppx | DownloadFFmpeg | OpenOutputDirectory,
            MicrosoftStoreDebug = CreateMicrosoftStoreDebugFolder | CompileAppx | DownloadFFmpeg | OpenOutputDirectory
        }

        private static SetupJobs Job { get; set; } = SetupJobs.Release;
        private static bool Silent { get; set; } = false;
        private static bool AppVeyor { get; set; } = false;

        private static string ParentDir;
        private static string Configuration;
        private static string AppVersion;
        private static string WindowsKitsDir;

        private static string SolutionPath => Path.Combine(ParentDir, "ShareX.sln");
        private static string BinDir => Path.Combine(ParentDir, "ShareX", "bin", Configuration);
        private static string SteamLauncherDir => Path.Combine(ParentDir, "ShareX.Steam", "bin", Configuration);
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

        private static string SetupPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-setup.exe");
        private static string RecorderDevicesSetupPath => Path.Combine(OutputDir, "Recorder-devices-setup.exe");
        private static string PortableZipPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-portable.zip");
        private static string DebugZipPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-debug.zip");
        private static string SteamUpdatesDir => Path.Combine(SteamOutputDir, "Updates");
        private static string SteamZipPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-Steam.zip");
        private static string MicrosoftStoreAppxPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}.appx");
        private static string MicrosoftStoreDebugAppxPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-debug.appx");
        private static string FFmpegPath => Path.Combine(OutputDir, "ffmpeg.exe");
        private static string MakeAppxPath => Path.Combine(WindowsKitsDir, "x64", "makeappx.exe");

        private const string InnoSetupCompilerPath = @"C:\Program Files (x86)\Inno Setup 6\ISCC.exe";
        private const string FFmpegVersion = "7.1";
        private static string FFmpegDownloadURL = $"https://github.com/ShareX/FFmpeg/releases/download/v{FFmpegVersion}/ffmpeg-{FFmpegVersion}-win64.zip";

        private static void Main(string[] args)
        {
            Console.WriteLine("ShareX setup started.");

            CheckArgs(args);

            Console.WriteLine("Job: " + Job);

            UpdatePaths();

            if (Directory.Exists(OutputDir))
            {
                Console.WriteLine("Cleaning output directory: " + OutputDir);

                Directory.Delete(OutputDir, true);
            }

            if (Job.HasFlag(SetupJobs.DownloadFFmpeg))
            {
                DownloadFFmpeg();
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

            if (AppVeyor)
            {
                FileHelpers.CopyAll(OutputDir, ParentDir);
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
            AppVeyor = cli.IsCommandExist("AppVeyor");

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

            Console.WriteLine("Configuration: " + Configuration);

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(ExecutablePath);
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

        private static void CompileSetup()
        {
            CompileISSFile("Recorder-devices-setup.iss");
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
                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        FileName = InnoSetupCompilerPath,
                        WorkingDirectory = InnoSetupDir,
                        Arguments = $"/Q \"{fileName}\"",
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

            FileHelpers.CopyFiles(Path.Combine(source, "ShareX.exe"), destination);
            FileHelpers.CopyFiles(Path.Combine(source, "ShareX.exe.config"), destination);
            FileHelpers.CopyFiles(source, destination, "*.dll");

            if (job == SetupJobs.CreateDebug || job == SetupJobs.CreateMicrosoftStoreDebugFolder)
            {
                FileHelpers.CopyFiles(source, destination, "*.pdb");
            }

            FileHelpers.CopyFiles(Path.Combine(ParentDir, "Licenses"), Path.Combine(destination, "Licenses"), "*.txt");

            if (job != SetupJobs.CreateMicrosoftStoreFolder && job != SetupJobs.CreateMicrosoftStoreDebugFolder)
            {
                if (!File.Exists(RecorderDevicesSetupPath))
                {
                    CompileISSFile("Recorder-devices-setup.iss");
                }

                FileHelpers.CopyFiles(RecorderDevicesSetupPath, destination);

                FileHelpers.CopyFiles(Path.Combine(source, "ShareX_NativeMessagingHost.exe"), destination);
                FileHelpers.CopyFiles(Path.Combine(source, "host-manifest-chrome.json"), destination);
                FileHelpers.CopyFiles(Path.Combine(source, "host-manifest-firefox.json"), destination);
            }

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