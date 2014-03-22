#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace HelpersLib
{
    public partial class DNSChangerForm : Form
    {
        public DNSChangerForm()
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            cbDNSType.Items.Add(new DNSInfo("Manual"));
            cbDNSType.Items.Add(new DNSInfo("Google Public DNS", "8.8.8.8", "8.8.4.4"));
            cbDNSType.Items.Add(new DNSInfo("OpenDNS", "208.67.222.222", "208.67.220.220"));
            cbDNSType.Items.Add(new DNSInfo("Level 3", "4.2.2.1", "4.2.2.2"));
            cbDNSType.Items.Add(new DNSInfo("Norton DNS", "199.85.126.10", "199.85.127.10"));
            cbDNSType.Items.Add(new DNSInfo("Comodo Secure DNS", "8.26.56.26", "8.20.247.20"));

            foreach (AdapterInfo adapter in AdapterInfo.GetEnabledAdapters())
            {
                cbAdapters.Items.Add(adapter);
            }

            if (cbAdapters.Items.Count > 0)
            {
                cbAdapters.SelectedIndex = 0;
            }
        }

        private void cbAdapters_SelectedIndexChanged(object sender, EventArgs e)
        {
            AdapterInfo adapter = cbAdapters.SelectedItem as AdapterInfo;

            if (adapter != null)
            {
                string[] dns = adapter.GetDNS();

                if (dns != null && dns.Length == 2)
                {
                    cbAutomatic.Checked = false;
                    txtPreferredDNS.Text = dns[0];
                    txtAlternateDNS.Text = dns[1];
                }
                else
                {
                    cbAutomatic.Checked = true;
                }

                cbDNSType.SelectedIndex = 0;
            }

            UpdateControls();
        }

        private void cbAutomatic_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void cbDNSType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDNSType.SelectedIndex > 0)
            {
                DNSInfo dnsInfo = cbDNSType.SelectedItem as DNSInfo;

                if (dnsInfo != null)
                {
                    txtPreferredDNS.Text = dnsInfo.PrimaryDNS;
                    txtAlternateDNS.Text = dnsInfo.SecondaryDNS;
                }
            }

            UpdateControls();
        }

        private void txtPreferredDNS_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void txtAlternateDNS_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void UpdateControls()
        {
            cbDNSType.Enabled = !cbAutomatic.Checked;
            txtPreferredDNS.Enabled = !cbAutomatic.Checked && cbDNSType.SelectedIndex == 0;
            txtAlternateDNS.Enabled = !cbAutomatic.Checked && cbDNSType.SelectedIndex == 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            AdapterInfo adapter = cbAdapters.SelectedItem as AdapterInfo;

            if (adapter != null)
            {
                bool result = false;

                if (cbAutomatic.Checked)
                {
                    result = adapter.SetDNSAutomatic();
                }
                else
                {
                    string primaryDNS = txtPreferredDNS.Text.Trim();
                    string secondaryDNS = txtAlternateDNS.Text.Trim();

                    if (Helpers.IsValidIPAddress(primaryDNS) && Helpers.IsValidIPAddress(secondaryDNS))
                    {
                        result = adapter.SetDNS(primaryDNS, secondaryDNS);
                    }
                }

                if (result)
                {
                    NativeMethods.DnsFlushResolverCache();
                    MessageBox.Show("DNS successfully set.", "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}