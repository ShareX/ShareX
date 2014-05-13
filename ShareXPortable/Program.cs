#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ShareXPortable
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string parentDir = @"..\..\..\";
            string releaseDir = Path.Combine(parentDir, @"ShareX\bin\Release");
            string outputDir = Path.Combine(parentDir, "Output");
            string portableDir = Path.Combine(outputDir, "ShareX-portable");

            List<string> files = new List<string>();

            string[] endsWith = new string[] { ".exe", ".dll", ".css", ".txt" };
            string[] ignoreEndsWith = new string[] { ".vshost.exe" };

            foreach (string filepath in Directory.GetFiles(releaseDir))
            {
                if (endsWith.Any(x => filepath.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)) &&
                    ignoreEndsWith.All(x => !filepath.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)))
                {
                    files.Add(filepath);
                }
            }

            if (Directory.Exists(portableDir))
            {
                Directory.Delete(portableDir, true);
                Console.WriteLine("Directory.Delete: \"{0}\"", portableDir);
            }

            Directory.CreateDirectory(portableDir);
            Console.WriteLine("Directory.Create: \"{0}\"", portableDir);

            foreach (string filepath in files)
            {
                string filename = Path.GetFileName(filepath);
                string dest = Path.Combine(portableDir, filename);

                File.Copy(filepath, dest);
                Console.WriteLine("File.Copy: \"{0}\" -> \"{1}\"", filepath, dest);
            }

            File.WriteAllText(Path.Combine(portableDir, "PersonalPath.cfg"), "ShareX", Encoding.UTF8);
            Console.WriteLine("Created PersonalPath.cfg file.");

            //FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(releaseDir, "ShareX.exe"));
            //string zipFilename = string.Format("ShareX-{0}.{1}.{2}-portable.zip", versionInfo.ProductMajorPart, versionInfo.ProductMinorPart, versionInfo.ProductBuildPart);
            string zipPath = Path.Combine(outputDir, "ShareX-portable.zip");

            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
                Console.WriteLine("File.Delete: \"{0}\"", zipPath);
            }

            Zip(portableDir + "\\*.*", zipPath);
            Console.WriteLine("Zip: \"{0}\"", zipPath);

            if (Directory.Exists(portableDir))
            {
                Directory.Delete(portableDir, true);
                Console.WriteLine("Directory.Delete: \"{0}\"", portableDir);
            }

            Process.Start("explorer.exe", outputDir);
            Console.WriteLine("Done.");
            //Console.Read();
        }

        private static void Zip(string source, string target)
        {
            ProcessStartInfo p = new ProcessStartInfo();
            p.FileName = "7za.exe";
            p.Arguments = string.Format("a -tzip \"{0}\" \"{1}\" -mx=9", target, source);
            p.WindowStyle = ProcessWindowStyle.Hidden;
            Process process = Process.Start(p);
            process.WaitForExit();
        }
    }
}