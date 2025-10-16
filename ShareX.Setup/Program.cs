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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

            Release = CreateSetup | CreatePortable | DownloadTools | OpenOutputDirectory,
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
        private static string TargetPlatform = WinX64;
        private static bool IsArm64 => TargetPlatform == WinArm64;
        private static string CurrentPlatform => TargetPlatform;
        private static string DetectedExecutablePath;
        private static bool IsConfigurationOverridden;
        private static bool IsPlatformOverridden;

        private static string BinDir => Path.Combine(ParentDir, "ShareX", "bin", Configuration, CurrentPlatform);
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

        private static string SetupPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-{CurrentPlatform}-setup.exe");
        private static string PortableZipPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-{CurrentPlatform}-portable.zip");
        private static string DebugZipPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-{CurrentPlatform}-debug.zip");
        private static string SteamUpdatesDir => Path.Combine(SteamOutputDir, "Updates");
        private static string SteamZipPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-{CurrentPlatform}-Steam.zip");
        private static string MicrosoftStoreAppxPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-{CurrentPlatform}.appx");
        private static string MicrosoftStoreDebugAppxPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-{CurrentPlatform}-debug.appx");
        private static string FFmpegPath => Path.Combine(OutputDir, "ffmpeg.exe");
        private static string RecorderDevicesSetupPath => Path.Combine(OutputDir, $"recorder-devices-{RecorderDevicesVersion}-setup.exe");
        private static string ExifToolPath => Path.Combine(OutputDir, "exiftool.exe");
        private static string MakeAppxPath => Path.Combine(WindowsKitsDir, "x64", "makeappx.exe");

        private const string InnoSetupCompilerPath = @"C:\Program Files (x86)\Inno Setup 6\ISCC.exe";
        private const string FFmpegVersion = "8.0";
        private static string FFmpegDownloadURL => $"https://github.com/ShareX/FFmpeg/releases/download/v{FFmpegVersion}/ffmpeg-{FFmpegVersion}-win-{(IsArm64 ? "arm64" : "x64")}.zip";
        private const string RecorderDevicesVersion = "0.12.10";
        private static string RecorderDevicesDownloadURL = $"https://github.com/ShareX/RecorderDevices/releases/download/v{RecorderDevicesVersion}/recorder-devices-{RecorderDevicesVersion}-setup.exe";
        private const string ExifToolVersion = "13.29";
        private static string ExifToolDownloadURL = $"https://github.com/ShareX/ExifTool/releases/download/v{ExifToolVersion}/exiftool-{ExifToolVersion}-win64.zip";
        private static void Main(string[] args)
        {
            Console.WriteLine("ShareX setup started.");

            CheckArgs(args);

            InitializeEnvironment();

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

            CLICommand configurationCommand = cli.GetCommand("Configuration");

            if (configurationCommand != null)
            {
                string configurationValue = configurationCommand.Parameter?.Trim();

                if (string.IsNullOrWhiteSpace(configurationValue))
                {
                    Console.WriteLine("Configuration argument requires a value.");
                    Environment.Exit(1);
                }

                if (!TryMapConfigurationToJob(configurationValue, out SetupJobs overrideJob, out string normalizedConfiguration))
                {
                    Console.WriteLine("Invalid configuration: " + configurationValue);
                    Environment.Exit(1);
                }

                Job = overrideJob;
                Configuration = normalizedConfiguration;
                IsConfigurationOverridden = true;
                Console.WriteLine("Using configuration override: " + Configuration);
            }

            CLICommand platformCommand = cli.GetCommand("Platform");

            if (platformCommand != null)
            {
                string platformValue = platformCommand.Parameter?.Trim();

                if (string.IsNullOrWhiteSpace(platformValue))
                {
                    Console.WriteLine("Platform argument requires a value.");
                    Environment.Exit(1);
                }

                if (platformValue.Equals(WinX64, StringComparison.OrdinalIgnoreCase))
                {
                    TargetPlatform = WinX64;
                    IsPlatformOverridden = true;
                }
                else if (platformValue.Equals(WinArm64, StringComparison.OrdinalIgnoreCase))
                {
                    TargetPlatform = WinArm64;
                    IsPlatformOverridden = true;
                }
                else
                {
                    Console.WriteLine("Invalid platform: " + platformValue);
                    Environment.Exit(1);
                }

                Console.WriteLine("Using platform override: " + TargetPlatform);
            }

            if (cli.IsCommandExist("Job"))
            {
                Console.WriteLine("Job argument detected but ignored; configuration will be auto-detected.");
            }
        }

        private static void UpdatePaths()
        {
            Console.WriteLine("Parent directory: " + ParentDir);

            UpdateBuildConfig();

            Console.WriteLine("Configuration: " + Configuration);
            Console.WriteLine("Platform: " + CurrentPlatform);
            Console.WriteLine("ExecutablePath (expected): " + ExecutablePath);

            string versionSourcePath = ResolveVersionSourcePath();

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

        private static void InitializeEnvironment()
        {
            ResolveParentDirectory();
            EnsureShareXBuildIfOverrides();
            DetectExecutablePath();
        }

        private static void EnsureShareXBuildIfOverrides()
        {
            try
            {
                if (!IsPlatformOverridden || !IsConfigurationOverridden)
                {
                    return;
                }

                string shareXBinRoot = Path.Combine(ParentDir, "ShareX", "bin");
                string expectedExe = Path.Combine(shareXBinRoot, Configuration, TargetPlatform, "ShareX.exe");

                if (File.Exists(expectedExe))
                {
                    // Already built; record path and return.
                    DetectedExecutablePath = expectedExe;
                    return;
                }

                Console.WriteLine($"Expected executable not found for overrides ({Configuration}/{TargetPlatform}). Attempting to build ShareX early...");
                bool built = TryBuildShareX(Configuration, TargetPlatform);
                if (built && File.Exists(expectedExe))
                {
                    DetectedExecutablePath = expectedExe;
                    Console.WriteLine($"Early build completed. Using executable: {expectedExe}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during early build attempt: " + ex);
            }
        }

        private static void ResolveParentDirectory()
        {
            string startingDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo current = new DirectoryInfo(startingDirectory);

            while (current != null)
            {
                string candidate = Path.Combine(current.FullName, "ShareX.sln");

                if (File.Exists(candidate))
                {
                    ParentDir = current.FullName;
                    return;
                }

                current = current.Parent;
            }

            Console.WriteLine("ShareX.sln could not be located starting from: " + startingDirectory);
            Environment.Exit(1);
        }

        private static void DetectExecutablePath()
        {
            string shareXBinRoot = Path.Combine(ParentDir, "ShareX", "bin");

            if (!Directory.Exists(shareXBinRoot))
            {
                Console.WriteLine("ShareX binary directory not found: " + shareXBinRoot);
                DetectedExecutablePath = Path.Combine(shareXBinRoot, Configuration, TargetPlatform, "ShareX.exe");
                return;
            }

            string[] executables = Directory.GetFiles(shareXBinRoot, "ShareX.exe", SearchOption.AllDirectories);

            if (executables.Length == 0)
            {
                Console.WriteLine("ShareX executable not found under: " + shareXBinRoot);
                DetectedExecutablePath = Path.Combine(shareXBinRoot, Configuration, TargetPlatform, "ShareX.exe");
                return;
            }

            if (IsPlatformOverridden)
            {
                string match = FindExecutable(executables, TargetPlatform, IsConfigurationOverridden ? Configuration : null);

                if (match != null)
                {
                    DetectedExecutablePath = match;
                    Console.WriteLine($"Using provided platform {TargetPlatform}: {match}");

                    if (!IsConfigurationOverridden)
                    {
                        TryUpdateJobFromExecutablePath(match);
                    }

                    return;
                }

                // If both platform and configuration are explicitly overridden, attempt to build the ShareX project.
                if (IsConfigurationOverridden)
                {
                    Console.WriteLine($"ShareX executable for {TargetPlatform}/{Configuration} not found. Attempting to build ShareX...");
                    bool buildOk = TryBuildShareX(Configuration, TargetPlatform);
                    string expectedAfterBuild = Path.Combine(shareXBinRoot, Configuration, TargetPlatform, "ShareX.exe");
                    if (buildOk && File.Exists(expectedAfterBuild))
                    {
                        DetectedExecutablePath = expectedAfterBuild;
                        Console.WriteLine($"Build completed. Using executable: {expectedAfterBuild}");
                        return;
                    }
                }

                Console.WriteLine($"ShareX executable for {TargetPlatform} not found. Using expected output path.");
                DetectedExecutablePath = Path.Combine(shareXBinRoot, Configuration, TargetPlatform, "ShareX.exe");
                return;
            }

            if (IsConfigurationOverridden)
            {
                string match = FindExecutable(executables, null, Configuration);

                if (match != null)
                {
                    DetectedExecutablePath = match;
                    string detectedPlatform = ExtractPlatformFromExecutablePath(match);

                    if (!string.IsNullOrEmpty(detectedPlatform))
                    {
                        TargetPlatform = detectedPlatform;
                    }

                    Console.WriteLine($"Detected executable for configuration {Configuration}: {match}");
                    return;
                }
            }

            string arm64Executable = FindExecutable(executables, WinArm64, null);

            if (arm64Executable != null)
            {
                TargetPlatform = WinArm64;
                DetectedExecutablePath = arm64Executable;
                Console.WriteLine($"Detected ShareX executable for {WinArm64}: {arm64Executable}");
                if (!IsConfigurationOverridden)
                {
                    TryUpdateJobFromExecutablePath(arm64Executable);
                }
                return;
            }

            string x64Executable = FindExecutable(executables, WinX64, null);

            TargetPlatform = WinX64;
            DetectedExecutablePath = x64Executable ?? Path.Combine(shareXBinRoot, Configuration, WinX64, "ShareX.exe");

            if (x64Executable != null)
            {
                Console.WriteLine($"Detected ShareX executable for {WinX64}: {x64Executable}");
                if (!IsConfigurationOverridden)
                {
                    TryUpdateJobFromExecutablePath(x64Executable);
                }
            }
            else
            {
                Console.WriteLine($"Defaulting platform to {WinX64}. Expected executable path: {DetectedExecutablePath}");
                if (!IsConfigurationOverridden)
                {
                    TryUpdateJobFromExecutablePath(DetectedExecutablePath);
                }
            }
        }

        private static string ResolveVersionSourcePath()
        {
            if (!string.IsNullOrEmpty(DetectedExecutablePath) && File.Exists(DetectedExecutablePath))
            {
                return DetectedExecutablePath;
            }

            string expectedExecutablePath = ExecutablePath;

            if (!File.Exists(expectedExecutablePath))
            {
                string fallbackPlatform = IsArm64 ? WinX64 : WinArm64;
                string fallbackBinDir = Path.Combine(ParentDir, "ShareX", "bin", Configuration, fallbackPlatform);
                string fallbackExe = Path.Combine(fallbackBinDir, "ShareX.exe");

                if (File.Exists(fallbackExe))
                {
                    Console.WriteLine($"Executable not found at expected path. Falling back to {fallbackPlatform} for version info: {fallbackExe}");
                    return fallbackExe;
                }

                Console.WriteLine("Executable not found for version detection at either path:");
                Console.WriteLine(" - Expected: " + expectedExecutablePath);
                Console.WriteLine(" - Fallback: " + fallbackExe);
            }

            return expectedExecutablePath;
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

        private static bool TryBuildShareX(string configuration, string platform)
        {
            try
            {
                string runtime = platform; // expects values like "win-x64" or "win-arm64"
                string csproj = Path.Combine(ParentDir, "ShareX", "ShareX.csproj");
                if (!File.Exists(csproj))
                {
                    Console.WriteLine("ShareX.csproj not found: " + csproj);
                    return false;
                }

                // dotnet restore -r <rid> ShareX\ShareX.csproj
                using (Process restore = new Process())
                {
                    restore.StartInfo = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = $"restore -r {runtime} \"{csproj}\"",
                        WorkingDirectory = ParentDir,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    restore.Start();
                    restore.WaitForExit();
                    if (restore.ExitCode != 0)
                    {
                        Console.WriteLine("dotnet restore failed:\n" + restore.StandardError.ReadToEnd());
                        return false;
                    }
                }

                // dotnet build -c <configuration> -r <rid> --no-restore ShareX\ShareX.csproj
                using (Process build = new Process())
                {
                    build.StartInfo = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = $"build -c {configuration} -r {runtime} --no-restore \"{csproj}\"",
                        WorkingDirectory = ParentDir,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    build.Start();
                    build.WaitForExit();
                    if (build.ExitCode != 0)
                    {
                        Console.WriteLine("dotnet build failed:\n" + build.StandardError.ReadToEnd());
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while building ShareX: " + ex);
                return false;
            }
        }

        private static void CompileISSFile(string fileName)
        {
            if (File.Exists(InnoSetupCompilerPath))
            {
                Console.WriteLine("Compiling setup file: " + fileName);

                using (Process process = new Process())
                {
                    string arguments = IsArm64 ? $"/Q /DArch=arm64 \"{fileName}\"" : $"/Q \"{fileName}\"";

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

            string steamLauncherDir = ResolveSteamLauncherDirectory();

            if (string.IsNullOrEmpty(steamLauncherDir))
            {
                Console.WriteLine("Steam launcher directory could not be determined. Aborting Steam artifact creation.");
                Environment.Exit(1);
            }

            FileHelpers.CopyFiles(Path.Combine(steamLauncherDir, "ShareX_Launcher.exe"), SteamOutputDir);
            FileHelpers.CopyFiles(Path.Combine(steamLauncherDir, "steam_appid.txt"), SteamOutputDir);
            FileHelpers.CopyFiles(Path.Combine(steamLauncherDir, "installscript.vdf"), SteamOutputDir);
            FileHelpers.CopyFiles(steamLauncherDir, SteamOutputDir, "*.dll");

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

        private static string FindExecutable(IEnumerable<string> executables, string requiredPlatform, string preferredConfiguration)
        {
            IEnumerable<string> candidates = executables ?? Array.Empty<string>();

            if (!string.IsNullOrEmpty(requiredPlatform))
            {
                candidates = candidates.Where(path => PathContainsSegment(path, requiredPlatform));
            }

            string[] candidateArray = candidates as string[] ?? candidates.ToArray();

            if (candidateArray.Length == 0)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(preferredConfiguration))
            {
                string preferredMatch = candidateArray.FirstOrDefault(path => PathContainsSegment(path, preferredConfiguration));

                if (preferredMatch != null)
                {
                    return preferredMatch;
                }
            }

            return candidateArray.OrderBy(path => path.Length).FirstOrDefault();
        }

        private static string ExtractPlatformFromExecutablePath(string executablePath)
        {
            if (string.IsNullOrEmpty(executablePath))
            {
                return null;
            }

            if (PathContainsSegment(executablePath, WinArm64))
            {
                return WinArm64;
            }

            if (PathContainsSegment(executablePath, WinX64))
            {
                return WinX64;
            }

            return null;
        }

        private static bool TryUpdateJobFromExecutablePath(string executablePath)
        {
            if (IsConfigurationOverridden)
            {
                return false;
            }

            if (string.IsNullOrEmpty(executablePath) || !File.Exists(executablePath))
            {
                return false;
            }

            string configurationName = TryExtractConfigurationFromExecutablePath(executablePath);

            if (string.IsNullOrEmpty(configurationName))
            {
                return false;
            }

            if (!TryMapConfigurationToJob(configurationName, out SetupJobs detectedJob, out string normalizedConfiguration))
            {
                return false;
            }

            if (Job != detectedJob)
            {
                Job = detectedJob;
                Configuration = normalizedConfiguration;
                Console.WriteLine($"Detected setup job: {Job}");
            }

            return true;
        }

        private static string TryExtractConfigurationFromExecutablePath(string executablePath)
        {
            FileInfo exeInfo = new FileInfo(executablePath);

            DirectoryInfo platformDirectory = exeInfo.Directory;
            DirectoryInfo configurationDirectory = platformDirectory?.Parent;
            DirectoryInfo binDirectory = configurationDirectory?.Parent;

            if (platformDirectory == null || configurationDirectory == null || binDirectory == null)
            {
                return null;
            }

            if (!binDirectory.Name.Equals("bin", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return configurationDirectory.Name;
        }

        private static bool TryMapConfigurationToJob(string configurationName, out SetupJobs job, out string normalizedConfiguration)
        {
            job = SetupJobs.None;
            normalizedConfiguration = null;

            if (string.IsNullOrWhiteSpace(configurationName))
            {
                return false;
            }

            string trimmed = configurationName.Trim();

            if (trimmed.Equals("Release", StringComparison.OrdinalIgnoreCase))
            {
                job = SetupJobs.Release;
                normalizedConfiguration = "Release";
                return true;
            }

            if (trimmed.Equals("Debug", StringComparison.OrdinalIgnoreCase))
            {
                job = SetupJobs.Debug;
                normalizedConfiguration = "Debug";
                return true;
            }

            if (trimmed.Equals("Steam", StringComparison.OrdinalIgnoreCase))
            {
                job = SetupJobs.Steam;
                normalizedConfiguration = "Steam";
                return true;
            }

            if (trimmed.Equals("MicrosoftStore", StringComparison.OrdinalIgnoreCase))
            {
                job = SetupJobs.MicrosoftStore;
                normalizedConfiguration = "MicrosoftStore";
                return true;
            }

            if (trimmed.Equals("MicrosoftStoreDebug", StringComparison.OrdinalIgnoreCase))
            {
                job = SetupJobs.MicrosoftStoreDebug;
                normalizedConfiguration = "MicrosoftStoreDebug";
                return true;
            }

            return false;
        }

        private static string ResolveSteamLauncherDirectory()
        {
            string steamBinRoot = Path.Combine(ParentDir, "ShareX.Steam", "bin");

            if (!Directory.Exists(steamBinRoot))
            {
                Console.WriteLine("Steam binaries directory not found: " + steamBinRoot);
                return null;
            }

            string[] launcherPaths = Directory.GetFiles(steamBinRoot, "ShareX_Launcher.exe", SearchOption.AllDirectories);

            if (launcherPaths.Length == 0)
            {
                Console.WriteLine("ShareX_Launcher.exe not found under: " + steamBinRoot);
                return null;
            }

            string preferredLauncher = launcherPaths.FirstOrDefault(path =>
                PathContainsSegment(path, CurrentPlatform) && PathContainsSegment(path, Configuration));

            preferredLauncher ??= launcherPaths.FirstOrDefault(path => PathContainsSegment(path, CurrentPlatform));
            preferredLauncher ??= launcherPaths.FirstOrDefault();

            string launcherDir = Path.GetDirectoryName(preferredLauncher);

            if (!string.IsNullOrEmpty(launcherDir))
            {
                Console.WriteLine("Detected Steam launcher directory: " + launcherDir);
            }

            return launcherDir;
        }

        private static bool PathContainsSegment(string path, string segment)
        {
            if (string.IsNullOrEmpty(segment))
            {
                return false;
            }

            string[] parts = path.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Any(part => part.Equals(segment, StringComparison.OrdinalIgnoreCase));
        }
    }
}