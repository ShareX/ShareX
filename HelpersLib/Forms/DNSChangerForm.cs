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
        private string tempPrimaryDNS, tempSecondaryDNS;

        public DNSChangerForm()
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            foreach (AdapterInfo adapter in GetEnabledAdapters())
            {
                cbAdapters.Items.Add(adapter);
            }

            if (cbAdapters.Items.Count > 0)
            {
                cbAdapters.SelectedIndex = 0;
            }

            cbDNSType.Items.Add(new DNSInfo("Custom"));
            cbDNSType.Items.Add(new DNSInfo("Google Public DNS", "8.8.8.8", "8.8.4.4"));
            cbDNSType.Items.Add(new DNSInfo("OpenDNS", "208.67.222.222", "208.67.220.220"));
            cbDNSType.Items.Add(new DNSInfo("Level 3", "4.2.2.1", "4.2.2.2"));
            cbDNSType.Items.Add(new DNSInfo("Norton DNS", "199.85.126.10", "199.85.127.10"));
            cbDNSType.Items.Add(new DNSInfo("Comodo Secure DNS", "8.26.56.26", "8.20.247.20"));
            cbDNSType.SelectedIndex = 0;
        }

        private List<AdapterInfo> GetEnabledAdapters()
        {
            List<AdapterInfo> adapters = new List<AdapterInfo>();

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = TRUE"))
            {
                foreach (ManagementObject mo in searcher.Get())
                {
                    adapters.Add(new AdapterInfo(mo));
                }
            }

            return adapters;
        }

        private void cbAdapters_SelectedIndexChanged(object sender, EventArgs e)
        {
            AdapterInfo adapter = cbAdapters.SelectedItem as AdapterInfo;

            if (adapter != null)
            {
                string[] dns = adapter.GetDNS();

                if (dns != null)
                {
                    tempPrimaryDNS = dns[0];
                    txtPreferredDNS.Text = tempPrimaryDNS;

                    tempSecondaryDNS = dns[1];
                    txtAlternateDNS.Text = tempSecondaryDNS;
                }
            }

            btnSave.Enabled = false;
        }

        private bool CheckDNSChanged()
        {
            string primaryDNS = txtPreferredDNS.Text.Trim();
            string secondaryDNS = txtAlternateDNS.Text.Trim();

            return !string.IsNullOrEmpty(primaryDNS) && !string.IsNullOrEmpty(secondaryDNS) && (primaryDNS != tempPrimaryDNS || secondaryDNS != tempSecondaryDNS);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string primaryDNS = txtPreferredDNS.Text.Trim();
            string secondaryDNS = txtAlternateDNS.Text.Trim();

            if (!string.IsNullOrEmpty(primaryDNS) && !string.IsNullOrEmpty(secondaryDNS))
            {
                AdapterInfo adapter = cbAdapters.SelectedItem as AdapterInfo;

                if (adapter != null)
                {
                    bool result = adapter.SetDNS(primaryDNS, secondaryDNS);

                    if (result)
                    {
                        tempPrimaryDNS = txtPreferredDNS.Text;
                        tempSecondaryDNS = txtAlternateDNS.Text;
                        btnSave.Enabled = false;
                        MessageBox.Show("DNS successfully set.", "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtPreferredDNS_TextChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = CheckDNSChanged();
        }

        private void txtAlternateDNS_TextChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = CheckDNSChanged();
        }

        private void cbDNSType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDNSType.SelectedIndex == 0)
            {
                txtPreferredDNS.Enabled = true;
                txtAlternateDNS.Enabled = true;
            }
            else if (cbDNSType.SelectedIndex > 0)
            {
                txtPreferredDNS.Enabled = false;
                txtAlternateDNS.Enabled = false;

                DNSInfo dnsInfo = cbDNSType.SelectedItem as DNSInfo;

                if (dnsInfo != null)
                {
                    txtPreferredDNS.Text = dnsInfo.PrimaryDNS;
                    txtAlternateDNS.Text = dnsInfo.SecondaryDNS;
                }
            }
        }
    }
}