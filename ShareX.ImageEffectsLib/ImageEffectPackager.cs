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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.ImageEffectsLib
{
    public static class ImageEffectPackager
    {
        private const string ConfigFileName = "Config.json";

        public static string Package(string outputFilePath, string configJson, string assetsFolderPath)
        {
            if (!string.IsNullOrEmpty(outputFilePath))
            {
                string outputFolder = Path.GetDirectoryName(outputFilePath);
                Helpers.CreateDirectory(outputFolder);

                string configFilePath = Path.Combine(outputFolder, ConfigFileName);
                File.WriteAllText(configFilePath, configJson, Encoding.UTF8);

                try
                {
                    Dictionary<string, string> files = new Dictionary<string, string>();
                    files.Add(configFilePath, ConfigFileName);

                    if (!string.IsNullOrEmpty(assetsFolderPath) && Directory.Exists(assetsFolderPath))
                    {
                        int entryNamePosition = assetsFolderPath.Length + 1;

                        foreach (string assetPath in Directory.EnumerateFiles(assetsFolderPath, "*.*", SearchOption.AllDirectories).Where(x => Helpers.IsImageFile(x)))
                        {
                            string entryName = assetPath.Substring(entryNamePosition);
                            files.Add(assetPath, entryName);
                        }
                    }

                    ZipManager.Compress(outputFilePath, files);
                }
                finally
                {
                    File.Delete(configFilePath);
                }

                return outputFilePath;
            }

            return null;
        }

        public static string ExtractPackage(string packageFilePath, string imageEffectsFolderPath)
        {
            if (!string.IsNullOrEmpty(packageFilePath) && File.Exists(packageFilePath) && !string.IsNullOrEmpty(imageEffectsFolderPath))
            {
                string packageName = Path.GetFileNameWithoutExtension(packageFilePath);

                if (!string.IsNullOrEmpty(packageName) && !packageName.StartsWith("."))
                {
                    string destination = Path.Combine(imageEffectsFolderPath, packageName);

                    if (Directory.Exists(destination))
                    {
                        // TODO: Translate
                        if (MessageBox.Show($"Destination folder already exists:\r\n\"{destination}\"\r\n\r\nWould you like to overwrite it?", "ShareX - " +
                            "Image effect packager", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            Directory.Delete(destination, true);
                        }
                        else
                        {
                            return null;
                        }
                    }

                    ZipManager.Extract(packageFilePath, destination);

                    return Path.Combine(destination, ConfigFileName);
                }
            }

            return null;
        }
    }
}