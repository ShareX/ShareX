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
            const string parentDir = @"..\..\..\";
            string releaseDir = Path.Combine(parentDir, @"ShareX\bin\Release");
            string outputDir = Path.Combine(parentDir, @"InnoSetup\Output");
            string portableDir = Path.Combine(outputDir, "ShareX-portable");

            var endsWith = new string[] { ".exe", ".dll", ".css", ".txt" };
            var ignoreEndsWith = new string[] { ".vshost.exe" };

            var files = Directory.GetFiles (releaseDir).Where (filepath => endsWith.Any (x => filepath.EndsWith (x, StringComparison.InvariantCultureIgnoreCase)) && ignoreEndsWith.All (x => !filepath.EndsWith (x, StringComparison.InvariantCultureIgnoreCase))).ToList ( );

            if (Directory.Exists(portableDir))
            {
                Directory.Delete(portableDir, true);
                Console.WriteLine("Directory.Delete: \"{0}\"", portableDir);
            }

            Directory.CreateDirectory(portableDir);
            Console.WriteLine("Directory.Create: \"{0}\"", portableDir);

            foreach (var filepath in files)
            {
                var filename = Path.GetFileName(filepath);
                var dest = Path.Combine(portableDir, filename);

                File.Copy(filepath, dest);
                Console.WriteLine("File.Copy: \"{0}\" -> \"{1}\"", filepath, dest);
            }

            File.WriteAllText(Path.Combine(portableDir, "PersonalPath.cfg"), "ShareX", Encoding.UTF8);
            Console.WriteLine("Created PersonalPath.cfg file.");

            //FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(releaseDir, "ShareX.exe"));
            //string zipFilename = string.Format("ShareX-{0}.{1}.{2}-portable.zip", versionInfo.ProductMajorPart, versionInfo.ProductMinorPart, versionInfo.ProductBuildPart);
            var zipPath = Path.Combine(outputDir, "ShareX-portable.zip");

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
        }

        private static void Zip(string source, string target)
        {
            var p = new ProcessStartInfo
                {
                FileName = "7za.exe",
                Arguments = string.Format ("a -tzip \"{0}\" \"{1}\" -mx=9", target, source),
                WindowStyle = ProcessWindowStyle.Hidden
                };
            var process = Process.Start(p);
            if (process != null) process.WaitForExit();
        }
    }
}