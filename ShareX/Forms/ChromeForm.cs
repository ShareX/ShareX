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

using ShareX.HelpersLib;
using ShareX.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX
{
    public partial class ChromeForm : Form
    {
        public ChromeForm()
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                string manifest = Encoding.UTF8.GetString(Resources.Chrome_host_manifest);
                manifest = manifest.Replace("{path}", Program.ChromeHostPath);
                File.WriteAllText(Program.ChromeHostManifestPath, manifest, Encoding.UTF8);

                RegistryHelpers.RegisterChromeSupport(Program.ChromeHostManifestPath);
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ShareX - " + Resources.Program_Run_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnInstallExtension_Click(object sender, EventArgs e)
        {
        }
    }
}