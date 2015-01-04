#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ShareX.Setup
{
    internal class Program
    {
        private enum SetupType
        {
            Stable, // Build setup + create portable zip file
            Beta // Build setup + upload it using "Debug/ShareX.exe"
        }

        private const SetupType Setup = SetupType.Beta;

        private static string parentDir = @"..\..\..\";
        private static string binDir = Path.Combine(parentDir, @"ShareX\bin");
        private static string releaseDir = Path.Combine(binDir, "Release");
        private static string debugDir = Path.Combine(binDir, "Debug");
        private static string releasePath = Path.Combine(releaseDir, "ShareX.exe");
        private static string debugPath = Path.Combine(debugDir, "ShareX.exe");
        private static string outputDir = Path.Combine(parentDir, @"InnoSetup\Output");
        private static string portableDir = Path.Combine(outputDir, "ShareX-portable");
        private static string innoSetupPath = @"C:\Program Files (x86)\Inno Setup 5\ISCC.exe";
        private static string innoSetupScriptPath = Path.Combine(parentDir, @"InnoSetup\ShareX setup.iss");

        private static void Main(string[] args)
        {
            switch (Setup)
            {
                case SetupType.Stable:
                    CompileSetup();
                    CreatePortable();
                    OpenOutputDirectory();
                    break;
                case SetupType.Beta:
                    CompileSetup();
                    UploadLatestFile();
                    break;
            }

            Console.WriteLine("Done.");
            //Console.Read();
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
            Console.WriteLine("Compiling setup...");
            Process.Start(innoSetupPath, string.Format("\"{0}\"", innoSetupScriptPath)).WaitForExit();
            Console.WriteLine("Setup file created.");
        }

        private static void CreatePortable()
        {
            Console.WriteLine("Creating portable...");

            if (Directory.Exists(portableDir))
            {
                Directory.Delete(portableDir, true);
            }

            Directory.CreateDirectory(portableDir);

            List<string> files = new List<string>();

            string[] endsWith = new string[] { "ShareX.exe", "ShareX.exe.config", ".dll", ".css", ".txt" };
            string[] ignoreEndsWith = new string[] { };

            foreach (string filepath in Directory.GetFiles(releaseDir))
            {
                if (endsWith.Any(x => filepath.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)) &&
                    ignoreEndsWith.All(x => !filepath.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)))
                {
                    files.Add(filepath);
                }
            }

            CopyFiles(files, portableDir);

            string[] languages = new string[] { "tr", "de", "fr" };

            foreach (string language in languages)
            {
                CopyFiles(Path.Combine(releaseDir, language + "\\*.resources.dll"), Path.Combine(portableDir, "Languages\\" + language));
            }

            File.WriteAllText(Path.Combine(portableDir, "PersonalPath.cfg"), "ShareX", Encoding.UTF8);

            //FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(releaseDir, "ShareX.exe"));
            //string zipFilename = string.Format("ShareX-{0}.{1}.{2}-portable.zip", versionInfo.ProductMajorPart, versionInfo.ProductMinorPart, versionInfo.ProductBuildPart);
            string zipPath = Path.Combine(outputDir, "ShareX-portable.zip");

            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }

            Zip(portableDir + "\\*.*", zipPath);

            if (Directory.Exists(portableDir))
            {
                Directory.Delete(portableDir, true);
            }

            Console.WriteLine("Portable created.");
        }

        private static void CopyFiles(IEnumerable files, string toFolder)
        {
            foreach (string filepath in files)
            {
                string filename = Path.GetFileName(filepath);
                string dest = Path.Combine(toFolder, filename);

                File.Copy(filepath, dest);
            }
        }

        private static void CopyFiles(string path, string toFolder)
        {
            string directory = Path.GetDirectoryName(path);
            string filename = Path.GetFileName(path);
            if (!Directory.Exists(toFolder)) Directory.CreateDirectory(toFolder);
            CopyFiles(Directory.GetFiles(directory, filename), toFolder);
        }

        private static void Zip(string source, string target)
        {
            ProcessStartInfo p = new ProcessStartInfo();
            p.FileName = "7za.exe";
            p.Arguments = string.Format("a -tzip \"{0}\" \"{1}\" -r -mx=9", target, source);
            p.WindowStyle = ProcessWindowStyle.Hidden;
            Process process = Process.Start(p);
            process.WaitForExit();
        }
    }
}