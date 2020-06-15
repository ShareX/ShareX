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
    public class ImageEffectPackager
    {
        public string EffectJson { get; set; }
        public string EffectName { get; set; }
        public string AssetsFolderPath { get; set; }

        public void Package()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.DefaultExt = "sxie";
                sfd.FileName = EffectName + ".sxie";
                sfd.Filter = "ShareX image effect (*.sxie)|*.sxie";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Package(sfd.FileName);
                }
            }
        }

        public void Package(string outputFilePath)
        {
            if (!string.IsNullOrEmpty(outputFilePath))
            {
                string outputFolder = Path.GetDirectoryName(outputFilePath);
                Helpers.CreateDirectory(outputFolder);

                string jsonFileName = "ImageEffect.json";
                string jsonFilePath = Path.Combine(outputFolder, jsonFileName);
                File.WriteAllText(jsonFilePath, EffectJson, Encoding.UTF8);

                try
                {
                    Dictionary<string, string> files = new Dictionary<string, string>();
                    files.Add(jsonFilePath, jsonFileName);

                    if (!string.IsNullOrEmpty(AssetsFolderPath) && Directory.Exists(AssetsFolderPath))
                    {
                        int entryNamePosition = AssetsFolderPath.Length + 1;

                        foreach (string assetPath in Directory.EnumerateFiles(AssetsFolderPath, "*.*", SearchOption.AllDirectories).Where(x => Helpers.IsImageFile(x)))
                        {
                            string entryName = assetPath.Substring(entryNamePosition);
                            files.Add(assetPath, entryName);
                        }
                    }

                    ZipManager.Compress(outputFilePath, files);
                }
                finally
                {
                    File.Delete(jsonFilePath);
                }
            }
        }
    }
}