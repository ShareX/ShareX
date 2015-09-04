#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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

using Newtonsoft.Json;
using ShareX.HelpersLib;
using ShareX.Properties;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ShareX
{
    public partial class ChromeForm : BaseForm
    {
        public ChromeForm()
        {
            InitializeComponent();
        }

        private void CreateChromeHostManifest(string filepath)
        {
            var manifest = new
            {
                name = "com.getsharex.sharex",
                description = "ShareX",
                path = Program.ChromeHostPath,
                type = "stdio",
                allowed_origins = new string[] { "chrome-extension://nlkoigbdolhchiicbonbihbphgamnaoc/" }
            };

            string json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            File.WriteAllText(filepath, json, Encoding.UTF8);
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                CreateChromeHostManifest(Program.ChromeHostManifestPath);

                RegistryHelpers.RegisterChromeSupport(Program.ChromeHostManifestPath);

                MessageBox.Show(Resources.ChromeForm_btnRegister_Click_Chrome_support_enabled_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ShareX - " + Resources.Program_Run_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUnregister_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(Program.ChromeHostManifestPath))
                {
                    File.Delete(Program.ChromeHostManifestPath);
                }

                RegistryHelpers.UnregisterChromeSupport();

                MessageBox.Show(Resources.ChromeForm_btnUnregister_Click_Chrome_support_disabled_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ShareX - " + Resources.Program_Run_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnInstallExtension_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("https://chrome.google.com/webstore/detail/sharex/nlkoigbdolhchiicbonbihbphgamnaoc");
        }
    }
}