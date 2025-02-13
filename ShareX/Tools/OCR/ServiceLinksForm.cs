#region License Information (GPL v3)

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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ShareX
{
    public partial class ServiceLinksForm : Form
    {
        public List<ServiceLink> ServiceLinks { get; private set; }

        public ServiceLinksForm(List<ServiceLink> serviceLinks)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            ServiceLinks = serviceLinks;

            if (ServiceLinks != null && ServiceLinks.Count > 0)
            {
                cbServices.Items.AddRange(ServiceLinks.ToArray());
                cbServices.SelectedIndex = 0;
            }

            UpdateControls();
        }

        private void UpdateControls()
        {
            btnRemove.Enabled = cbServices.Enabled = txtName.Enabled = txtURL.Enabled = cbServices.SelectedItem != null;
        }

        private void cbServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbServices.SelectedItem is ServiceLink serviceLink)
            {
                txtName.Text = serviceLink.Name;
                txtURL.Text = serviceLink.URL;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ServiceLink serviceLink = new ServiceLink("Name", "https://example.com/search?q={0}");
            ServiceLinks.Add(serviceLink);

            cbServices.Items.Add(serviceLink);
            cbServices.SelectedIndex = cbServices.Items.Count - 1;
            UpdateControls();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (cbServices.SelectedItem is ServiceLink serviceLink)
            {
                ServiceLinks.Remove(serviceLink);

                cbServices.Items.Remove(serviceLink);
                cbServices.SelectedIndex = cbServices.Items.Count - 1;
                UpdateControls();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ServiceLinks.Clear();
            ServiceLinks.AddRange(OCROptions.DefaultServiceLinks);

            cbServices.Items.Clear();
            cbServices.Items.AddRange(ServiceLinks.ToArray());
            cbServices.SelectedIndex = 0;
            UpdateControls();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (cbServices.SelectedItem is ServiceLink serviceLink)
            {
                serviceLink.Name = txtName.Text;

                int index = cbServices.SelectedIndex;
                cbServices.Items[index] = cbServices.Items[index];
            }
        }

        private void txtURL_TextChanged(object sender, EventArgs e)
        {
            if (cbServices.SelectedItem is ServiceLink serviceLink)
            {
                serviceLink.URL = txtURL.Text;
            }
        }
    }
}