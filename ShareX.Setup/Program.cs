#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

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

            Stable = CreateSetup | CreatePortable | OpenOutputDirectory,
            Setup = CreateSetup | OpenOutputDirectory,
            Portable = CreatePortable | OpenOutputDirectory,
            Steam = CreateSteamFolder | OpenOutputDirectory,
            PortableApps = CreatePortableAppsFolder | OpenOutputDirectory,
            Beta = CreateSetup | UploadOutputFile
        }

        private static SetupJobs Job = SetupJobs.Stable;
        private static bool AppVeyor = false;

        private static string ParentDir => AppVeyor ? "" : @"..\..\..\";
        private static string BinDir => Path.Combine(ParentDir, "ShareX", "bin");
        private static string ReleaseDir => Path.Combine(BinDir, "Release");
        private static string DebugDir => Path.Combine(BinDir, "Debug");
        private static string SteamDir => Path.Combine(BinDir, "Steam");

        private static string InnoSetupDir => Path.Combine(ParentDir, @"ShareX.Setup\InnoSetup");
        private static string OutputDir => Path.Combine(InnoSetupDir, "Output");
        private static string PortableOutputDir => Path.Combine(OutputDir, "ShareX-portable");
        private static string SteamOutputDir => Path.Combine(OutputDir, "ShareX-Steam");
        private static string PortableAppsOutputDir => Path.Combine(ParentDir, @"..\PortableApps\ShareXPortable\App\ShareX");

        private static string SteamLauncherDir => Path.Combine(ParentDir, @"ShareX.Steam\bin\Release");
        private static string SteamUpdatesDir => Path.Combine(SteamOutputDir, "Updates");
        private static string ChromeDir => Path.Combine(ParentDir, @"ShareX.Chrome\bin\Release");

        private static string DebugExecutablePath => Path.Combine(DebugDir, "ShareX.exe");
        private static string InnoSetupCompilerPath = @"C:\Program Files (x86)\Inno Setup 5\ISCC.exe";
        private static string ZipPath = @"C:\Program Files\7-Zip\7z.exe";

        private static void Main(string[] args)
        {
            Console.WriteLine("ShareX setup started.");

            AppVeyor = CheckArgs(args, "-appveyor");

            if (AppVeyor)
            {
                Job = SetupJobs.CreateSetup | SetupJobs.CreatePortable;
            }

            Console.WriteLine("Setup job: " + Job);

            if (Job.HasFlag(SetupJobs.CreateSetup))
            {
                CompileSetup();
            }

            if (Job.HasFlag(SetupJobs.CreatePortable))
            {
                CreatePortable(PortableOutputDir, ReleaseDir);
            }

            if (Job.HasFlag(SetupJobs.CreateSteamFolder))
            {
                CreateSteamFolder();
            }

            if (Job.HasFlag(SetupJobs.CreatePortableAppsFolder))
            {
                CreatePortable(PortableAppsOutputDir, ReleaseDir);
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

        private static bool CheckArgs(string[] args, string check)
        {
            if (!string.IsNullOrEmpty(check))
            {
                foreach (string arg in args)
                {
                    if (!string.IsNullOrEmpty(arg) && arg.Equals(check, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void CompileSetup()
        {
            if (AppVeyor && !File.Exists(InnoSetupCompilerPath))
            {
                InstallInnoSetup();
            }

            if (File.Exists(InnoSetupCompilerPath))
            {
                CompileISSFile("Recorder-devices-setup.iss");
                CompileISSFile("ShareX-setup.iss");
            }
            else
            {
                Console.WriteLine("InnoSetup compiler is missing: " + InnoSetupCompilerPath);
            }
        }

        private static void InstallInnoSetup()
        {
            Console.WriteLine("Downloading InnoSetup.");

            string innoSetupURL = "http://files.jrsoftware.org/is/5/innosetup-5.5.9-unicode.exe";
            string innoSetupFilename = "innosetup-5.5.9-unicode.exe";

            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(innoSetupURL, innoSetupFilename);
            }

            Console.WriteLine("Installing InnoSetup.");

            Process.Start(innoSetupFilename, "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-").WaitForExit();

            Console.WriteLine("InnoSetup installed.");
        }

        private static void CompileISSFile(string filename)
        {
            Console.WriteLine("Compiling setup file: " + filename);

            ProcessStartInfo startInfo = new ProcessStartInfo(InnoSetupCompilerPath, $"\"{filename}\"");
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = Path.GetFullPath(InnoSetupDir);
            Process process = Process.Start(startInfo);
            process.WaitForExit();

            Console.WriteLine("Setup file is created.");
        }

        private static void CreateSteamFolder()
        {
            Console.WriteLine("Creating Steam folder:" + SteamOutputDir);

            if (Directory.Exists(SteamOutputDir))
            {
                Directory.Delete(SteamOutputDir, true);
            }

            Directory.CreateDirectory(SteamOutputDir);

            CopyFile(Path.Combine(SteamLauncherDir, "ShareX_Launcher.exe"), SteamOutputDir);
            CopyFile(Path.Combine(SteamLauncherDir, "steam_appid.txt"), SteamOutputDir);
            CopyFile(Path.Combine(SteamLauncherDir, "installscript.vdf"), SteamOutputDir);
            CopyFiles(SteamLauncherDir, "*.dll", SteamOutputDir);

            CreatePortable(SteamUpdatesDir, SteamDir);
        }

        private static void CreatePortable(string destination, string releaseDirectory)
        {
            Console.WriteLine("Creating portable: " + destination);

            if (Directory.Exists(destination))
            {
                Directory.Delete(destination, true);
            }

            Directory.CreateDirectory(destination);

            CopyFile(Path.Combine(releaseDirectory, "ShareX.exe"), destination);
            CopyFile(Path.Combine(releaseDirectory, "ShareX.exe.config"), destination);
            CopyFiles(releaseDirectory, "*.dll", destination);
            CopyFiles(Path.Combine(ParentDir, "Licenses"), "*.txt", Path.Combine(destination, "Licenses"));
            CopyFile(Path.Combine(OutputDir, "Recorder-devices-setup.exe"), destination);
            CopyFile(Path.Combine(ChromeDir, "ShareX_Chrome.exe"), destination);

            string[] languages = new string[] { "de", "es", "fr", "hu", "ko-KR", "nl-NL", "pt-BR", "ru", "tr", "vi-VN", "zh-CN" };

            foreach (string language in languages)
            {
                CopyFiles(Path.Combine(releaseDirectory, language), "*.resources.dll", Path.Combine(destination, "Languages", language));
            }

            if (destination.Equals(SteamUpdatesDir, StringComparison.InvariantCultureIgnoreCase))
            {
                // These git ignored
                CopyFile(Path.Combine(ParentDir, "Lib", "ffmpeg.exe"), destination);
                CopyFile(Path.Combine(ParentDir, "Lib", "ffmpeg-x64.exe"), destination);
            }
            else if (destination.Equals(PortableAppsOutputDir, StringComparison.InvariantCultureIgnoreCase))
            {
                File.Create(Path.Combine(destination, "PortableApps")).Dispose();
            }
            else
            {
                File.Create(Path.Combine(destination, "Portable")).Dispose();

                //FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(releaseDir, "ShareX.exe"));
                //string zipFilename = string.Format("ShareX-{0}.{1}.{2}-portable.zip", versionInfo.ProductMajorPart, versionInfo.ProductMinorPart, versionInfo.ProductBuildPart);
                string zipPath = Path.Combine(OutputDir, "ShareX-portable.zip");

                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }

                Zip(destination + "\\*", zipPath);

                if (Directory.Exists(destination))
                {
                    Directory.Delete(destination, true);
                }
            }

            Console.WriteLine("Portable created.");
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

        private static void CopyFiles(string[] files, string toFolder)
        {
            if (!Directory.Exists(toFolder))
            {
                Directory.CreateDirectory(toFolder);
            }

            foreach (string filepath in files)
            {
                string filename = Path.GetFileName(filepath);
                string dest = Path.Combine(toFolder, filename);
                File.Copy(filepath, dest);
            }
        }

        private static void CopyFile(string path, string toFolder)
        {
            CopyFiles(new string[] { path }, toFolder);
        }

        private static void CopyFiles(string directory, string searchPattern, string toFolder)
        {
            CopyFiles(Directory.GetFiles(directory, searchPattern), toFolder);
        }

        private static void Zip(string source, string target)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = ZipPath;
            startInfo.Arguments = string.Format("a -tzip \"{0}\" \"{1}\" -r -mx=9", target, source);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process process = Process.Start(startInfo);
            process.WaitForExit();
        }
    }
}