﻿#region License Information (GPL v3)

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
using System.Runtime.InteropServices;
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

        private static string ParentDir;
        private static string Configuration;
        private static string AppVersion;
        private static string WindowsKitsDir;

        private static List<Architecture> SupportedArchitectures = new List<Architecture>
        {
            Architecture.X64,
            Architecture.Arm64
        };


        private static string GetArchName(Architecture arch) =>
            arch switch
            {
                Architecture.Arm64 => "arm64",
                Architecture.X64 => "x64",
                Architecture.X86 => "x86",
                _ => throw new NotImplementedException($"Arch {arch} not implemented")
            };

        private static string SolutionPath => Path.Combine(ParentDir, "ShareX.sln");

        private static string GetBinDir(Architecture arch) => Path.Combine(ParentDir, "ShareX", "bin", Configuration, $"win-{GetArchName(arch)}", "publish");
        private static string SteamLauncherDir => Path.Combine(ParentDir, "ShareX.Steam", "bin", Configuration);
        private static string GetExecutablePath(Architecture arch) => Path.Combine(GetBinDir(arch), "ShareX.exe");

        private static string OutputDir => Path.Combine(ParentDir, "Output");
        private static string DependenciesDir => Path.Combine(OutputDir, "Dependencies");
        private static string GetPortableOutputDir(Architecture arch) => Path.Combine(OutputDir, $"ShareX-portable-{GetArchName(arch)}");
        private static string DebugOutputDir => Path.Combine(OutputDir, "ShareX-debug");
        private static string SteamOutputDir => Path.Combine(OutputDir, "ShareX-Steam");
        private static string GetMicrosoftStoreOutputDir(Architecture arch) => Path.Combine(OutputDir, $"ShareX-MicrosoftStore-{GetArchName(arch)}");
        private static string GetMicrosoftStoreDebugOutputDir(Architecture arch) => Path.Combine(OutputDir, $"ShareX-MicrosoftStore-debug-{GetArchName(arch)}");

        private static string SetupDir => Path.Combine(ParentDir, "ShareX.Setup");
        private static string InnoSetupDir => Path.Combine(SetupDir, "InnoSetup");
        private static string MicrosoftStorePackageFilesDir => Path.Combine(SetupDir, "MicrosoftStore", "Resources");
        private static string GetMicrosoftStoreManifestDir(Architecture arch) => Path.Combine(SetupDir, "MicrosoftStore", GetArchName(arch));

        private static string GetSetupPath(Architecture arch) => Path.Combine(OutputDir, $"ShareX-{AppVersion}-setup-{GetArchName(arch)}.exe");
        private static string GetPortableZipPath(Architecture arch) => Path.Combine(OutputDir, $"ShareX-{AppVersion}-portable-{GetArchName(arch)}.zip");
        private static string DebugZipPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-debug.zip");
        private static string SteamUpdatesDir => Path.Combine(SteamOutputDir, "Updates");
        private static string SteamZipPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-Steam.zip");
        private static string MicrosoftStoreAppxDir => Path.Combine(OutputDir, "Microstore-Appx");
        private static string GetMicrosoftStoreAppxPath(Architecture arch) => Path.Combine( MicrosoftStoreAppxDir, $"ShareX-{AppVersion}-{GetArchName(arch)}.appx");
        private static string MicrosoftStoreBundlePath => Path.Combine(OutputDir, $"ShareX-{AppVersion}.appxbundle");
        private static string MicrosoftStoreDebugAppxDir => Path.Combine(OutputDir, "Microstore-Appx-Debug");
        private static string GetMicrosoftStoreDebugAppxPath(Architecture arch) => Path.Combine(MicrosoftStoreDebugAppxDir, $"ShareX-{AppVersion}-debug-{GetArchName(arch)}.appx");
        private static string MicrosoftStoreBundleDebugPath => Path.Combine(OutputDir, $"ShareX-{AppVersion}-debug.appxbundle");
        private static string FFmpegPath => Path.Combine(OutputDir, "ffmpeg.exe");
        private static string RecorderDevicesSetupPath => Path.Combine(OutputDir, $"recorder-devices-{RecorderDevicesVersion}-setup.exe");
        private static string ExifToolPath => Path.Combine(OutputDir, "exiftool.exe");
        private static string MakeAppxPath => Path.Combine(WindowsKitsDir, "x64", "makeappx.exe");

        private const string InnoSetupCompilerPath = @"C:\Program Files (x86)\Inno Setup 6\ISCC.exe";
        private const string FFmpegVersion = "7.1";
        private static string FFmpegDownloadURL = $"https://github.com/ShareX/FFmpeg/releases/download/v{FFmpegVersion}/ffmpeg-{FFmpegVersion}-win64.zip";
        private const string RecorderDevicesVersion = "0.12.10";
        private static string RecorderDevicesDownloadURL = $"https://github.com/ShareX/RecorderDevices/releases/download/v{RecorderDevicesVersion}/recorder-devices-{RecorderDevicesVersion}-setup.exe";
        private const string ExifToolVersion = "13.29";
        private static string ExifToolDownloadURL = $"https://github.com/ShareX/ExifTool/releases/download/v{ExifToolVersion}/exiftool-{ExifToolVersion}-win64.zip";

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
                SupportedArchitectures.ForEach(arch =>
                {
                    CreateFolder(GetBinDir(arch), GetPortableOutputDir(arch), arch, SetupJobs.CreatePortable);
                    
                    CreateZipFile(GetPortableOutputDir(arch), GetPortableZipPath(arch));
                });
            }

            if (Job.HasFlag(SetupJobs.CreateDebug))
            {
                SupportedArchitectures.ForEach(arch =>
                {
                    CreateFolder(GetBinDir(arch), DebugOutputDir, arch, SetupJobs.CreateDebug);

                    CreateZipFile(DebugOutputDir, DebugZipPath);
                });
               
            }

            if (Job.HasFlag(SetupJobs.CreateSteamFolder))
            {
                CreateSteamFolder();

                CreateZipFile(SteamOutputDir, SteamZipPath);
            }

            if (Job.HasFlag(SetupJobs.CreateMicrosoftStoreFolder))
            {
                SupportedArchitectures.ForEach(arch =>
                {
                    CreateFolder(GetBinDir(arch), GetMicrosoftStoreOutputDir(arch), arch, SetupJobs.CreateMicrosoftStoreFolder);

                    if (Job.HasFlag(SetupJobs.CompileAppx))
                    {
                        CompileAppx(GetMicrosoftStoreOutputDir(arch), GetMicrosoftStoreAppxPath(arch));
                    }
                });
                CompileAppxBundle(MicrosoftStoreAppxDir, MicrosoftStoreBundlePath);
            }

            if (Job.HasFlag(SetupJobs.CreateMicrosoftStoreDebugFolder))
            {
                SupportedArchitectures.ForEach(arch =>
                {
                    CreateFolder(GetBinDir(arch), GetMicrosoftStoreDebugOutputDir(arch), arch, SetupJobs.CreateMicrosoftStoreDebugFolder);

                    if (Job.HasFlag(SetupJobs.CompileAppx))
                    {
                        CompileAppx(GetMicrosoftStoreDebugOutputDir(arch), GetMicrosoftStoreDebugAppxPath(arch));
                    }
                });
                
                CompileAppxBundle(MicrosoftStoreDebugAppxDir, MicrosoftStoreBundleDebugPath);
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

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(GetExecutablePath(Architecture.X64));
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
            SupportedArchitectures.ForEach(arch =>
            {
                CompileISSFile($"ShareX-setup-{arch}.iss");
                CreateChecksumFile(GetSetupPath(arch));
            });
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

        private static void CompileAppxBundle(string contentDirectory, string outputPackageName)
        {
            Console.WriteLine("Compiling appx file: " + contentDirectory);

            using (Process process = new Process())
            {
                ProcessStartInfo psi = new ProcessStartInfo()
                {
                    FileName = MakeAppxPath,
                    Arguments = $"bundle /d \"{contentDirectory}\" /p \"{outputPackageName}\"",
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

            CreateFolder(GetBinDir(Architecture.X64), SteamUpdatesDir, Architecture.X64, SetupJobs.CreateSteamFolder);
        }

        private static void CreateFolder(string source, string destination, Architecture architecture, SetupJobs job)
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
                    FileHelpers.CopyFiles(Path.Combine(source, language), Path.Combine(destination, language), "*.resources.dll");
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
                FileHelpers.CopyAll(GetMicrosoftStoreManifestDir(architecture), destination);
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
                string filePath = Path.Combine(DependenciesDir, fileName);

                Console.WriteLine("Downloading: " + FFmpegDownloadURL);
                WebHelpers.DownloadFileAsync(FFmpegDownloadURL, filePath).GetAwaiter().GetResult();

                Console.WriteLine("Extracting: " + filePath);
                ZipManager.Extract(filePath, DependenciesDir, false, entry => entry.Name.Equals("ffmpeg.exe", StringComparison.OrdinalIgnoreCase));
            }
        }

        private static void DownloadRecorderDevices()
        {
            if (!File.Exists(RecorderDevicesSetupPath))
            {
                string fileName = Path.GetFileName(RecorderDevicesDownloadURL);
                string filePath = Path.Combine(DependenciesDir, fileName);

                Console.WriteLine("Downloading: " + RecorderDevicesDownloadURL);
                WebHelpers.DownloadFileAsync(RecorderDevicesDownloadURL, filePath).GetAwaiter().GetResult();
            }
        }

        private static void DownloadExifTool()
        {
            if (!File.Exists(ExifToolPath))
            {
                string fileName = Path.GetFileName(ExifToolDownloadURL);
                string filePath = Path.Combine(DependenciesDir, fileName);

                Console.WriteLine("Downloading: " + ExifToolDownloadURL);
                WebHelpers.DownloadFileAsync(ExifToolDownloadURL, filePath).GetAwaiter().GetResult();

                Console.WriteLine("Extracting: " + filePath);
                ZipManager.Extract(filePath, DependenciesDir);
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