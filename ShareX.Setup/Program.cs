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
        private enum SetupType
        {
            Stable, // Build setup & create portable zip file
            BuildSetup, // Build setup
            CreatePortable, // Create portable zip file
            PortableApps, // Create PortableApps folder
            Beta, // Build setup & upload it using "Debug/ShareX.exe"
            Steam, // Create Steam folder
            AppVeyor
        }

        private static SetupType Setup = SetupType.Stable;

        private static readonly string binDir = Path.Combine(ParentDir, "ShareX", "bin");
        private static readonly string releaseDir = Path.Combine(binDir, "Release");
        private static readonly string debugDir = Path.Combine(binDir, "Debug");
        private static readonly string steamDir = Path.Combine(binDir, "Steam");
        private static readonly string debugPath = Path.Combine(debugDir, "ShareX.exe");
        private static readonly string innoSetupDir = Path.Combine(ParentDir, @"ShareX.Setup\InnoSetup");
        private static readonly string outputDir = Path.Combine(innoSetupDir, "Output");
        private static readonly string portableDir = Path.Combine(outputDir, "ShareX-portable");
        private static readonly string steamOutputDir = Path.Combine(outputDir, "ShareX");
        private static readonly string portableAppsDir = Path.Combine(ParentDir, @"..\PortableApps\ShareXPortable\App\ShareX");
        private static readonly string steamLauncherDir = Path.Combine(ParentDir, @"..\ShareX_Steam\ShareX_Steam\bin\Release");
        private static readonly string steamUpdatesDir = Path.Combine(steamOutputDir, "Updates");
        private static readonly string chromeReleaseDir = Path.Combine(ParentDir, @"..\ShareX_Chrome\ShareX_Chrome\bin\Release");
        private static readonly string innoSetupCompilerPath = @"C:\Program Files (x86)\Inno Setup 5\ISCC.exe";
        private static readonly string innoSetupScriptPath = Path.Combine(innoSetupDir, "ShareX-setup.iss");
        private static readonly string zipPath = @"C:\Program Files\7-Zip\7z.exe";

        private static string ParentDir => Setup == SetupType.AppVeyor ? "" : @"..\..\..\";
        private static string ReleaseDirectory => Setup == SetupType.Steam ? steamDir : releaseDir;

        private static void Main(string[] args)
        {
            Console.WriteLine("ShareX.Setup started.");

            if (CheckArgs(args, "-appveyor"))
            {
                Setup = SetupType.AppVeyor;
            }

            Console.WriteLine("Setup type: " + Setup);

            switch (Setup)
            {
                case SetupType.Stable:
                    CompileSetup();
                    CreatePortable(portableDir);
                    OpenOutputDirectory();
                    break;
                case SetupType.BuildSetup:
                    CompileSetup();
                    OpenOutputDirectory();
                    break;
                case SetupType.CreatePortable:
                    CreatePortable(portableDir);
                    OpenOutputDirectory();
                    break;
                case SetupType.PortableApps:
                    CreatePortable(portableAppsDir);
                    OpenOutputDirectory();
                    break;
                case SetupType.Beta:
                    CompileSetup();
                    UploadLatestFile();
                    break;
                case SetupType.Steam:
                    CreateSteamFolder();
                    OpenOutputDirectory();
                    break;
                case SetupType.AppVeyor:
                    CompileSetup();
                    break;
            }

            Console.WriteLine("ShareX.Setup successfully completed.");
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

        private static void OpenOutputDirectory()
        {
            Process.Start("explorer.exe", outputDir);
        }

        private static void UploadLatestFile()
        {
            FileInfo fileInfo = new DirectoryInfo(outputDir).GetFiles("*.exe").OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
            if (fileInfo != null)
            {
                Console.WriteLine("Uploading setup file...");
                Process.Start(debugPath, fileInfo.FullName);
            }
        }

        private static void CompileSetup()
        {
            if (Setup == SetupType.AppVeyor && !File.Exists(innoSetupCompilerPath))
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

            if (File.Exists(innoSetupCompilerPath))
            {
                Console.WriteLine("Compiling setup file.");

                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo(innoSetupCompilerPath, $"\"{innoSetupScriptPath}\"");
                startInfo.UseShellExecute = false;
                startInfo.WorkingDirectory = innoSetupDir;
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                Console.WriteLine("Setup file is created.");
            }
            else
            {
                Console.WriteLine("InnoSetup compiler is missing: " + innoSetupCompilerPath);
            }
        }

        private static void CreateSteamFolder()
        {
            if (Directory.Exists(steamOutputDir))
            {
                Directory.Delete(steamOutputDir, true);
            }

            Directory.CreateDirectory(steamOutputDir);

            CopyFile(Path.Combine(steamLauncherDir, "ShareX_Launcher.exe"), steamOutputDir);
            CopyFile(Path.Combine(steamLauncherDir, "steam_appid.txt"), steamOutputDir);
            CopyFile(Path.Combine(steamLauncherDir, "installscript.vdf"), steamOutputDir);
            CopyFiles(steamLauncherDir, "*.dll", steamOutputDir);

            CreatePortable(steamUpdatesDir);
        }

        private static void CreatePortable(string destination)
        {
            Console.WriteLine("Creating portable...");

            if (Directory.Exists(destination))
            {
                Directory.Delete(destination, true);
            }

            Directory.CreateDirectory(destination);

            CopyFile(Path.Combine(ReleaseDirectory, "ShareX.exe"), destination);
            CopyFile(Path.Combine(ReleaseDirectory, "ShareX.exe.config"), destination);
            CopyFiles(ReleaseDirectory, "*.dll", destination);
            CopyFiles(Path.Combine(ParentDir, "Licenses"), "*.txt", Path.Combine(destination, "Licenses"));
            CopyFile(Path.Combine(outputDir, "Recorder-devices-setup.exe"), destination);
            CopyFile(Path.Combine(chromeReleaseDir, "ShareX_Chrome.exe"), destination);

            string[] languages = new string[] { "de", "es", "fr", "hu", "ko-KR", "nl-NL", "pt-BR", "ru", "tr", "vi-VN", "zh-CN" };

            foreach (string language in languages)
            {
                CopyFiles(Path.Combine(ReleaseDirectory, language), "*.resources.dll", Path.Combine(destination, "Languages", language));
            }

            if (Setup == SetupType.Steam)
            {
                // These git ignored
                CopyFile(Path.Combine(ParentDir, "Lib", "ffmpeg.exe"), destination);
                CopyFile(Path.Combine(ParentDir, "Lib", "ffmpeg-x64.exe"), destination);
            }
            else if (Setup == SetupType.PortableApps)
            {
                File.Create(Path.Combine(destination, "PortableApps")).Dispose();
            }
            else
            {
                File.Create(Path.Combine(destination, "Portable")).Dispose();

                //FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(releaseDir, "ShareX.exe"));
                //string zipFilename = string.Format("ShareX-{0}.{1}.{2}-portable.zip", versionInfo.ProductMajorPart, versionInfo.ProductMinorPart, versionInfo.ProductBuildPart);
                string zipPath = Path.Combine(outputDir, "ShareX-portable.zip");

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
            ProcessStartInfo p = new ProcessStartInfo();
            p.FileName = zipPath;
            p.Arguments = string.Format("a -tzip \"{0}\" \"{1}\" -r -mx=9", target, source);
            p.WindowStyle = ProcessWindowStyle.Hidden;
            Process process = Process.Start(p);
            process.WaitForExit();
        }
    }
}