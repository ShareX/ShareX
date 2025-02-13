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

using ShareX.HelpersLib.Properties;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class DNSChangerForm : Form
    {
        public DNSChangerForm()
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            AddDNS(Resources.DNSChangerForm_DNSChangerForm_Manual);
            AddDNS("Google Public DNS", "8.8.8.8", "8.8.4.4"); // https://developers.google.com/speed/public-dns/
            AddDNS("OpenDNS", "208.67.222.222", "208.67.220.220"); // https://www.opendns.com
            AddDNS("Cloudflare", "1.1.1.1", "1.0.0.1"); // https://1.1.1.1
            AddDNS("Level 3 Communications", "4.2.2.1", "4.2.2.2"); // http://www.level3.com
            AddDNS("Comodo Secure DNS", "8.26.56.26", "8.20.247.20"); // https://www.comodo.com/secure-dns/
            AddDNS("DNS Advantage", "156.154.70.1", "156.154.71.1"); // https://www.security.neustar/dns-services/free-recursive-dns-service
            AddDNS("Yandex DNS", "77.88.8.2", "77.88.8.88"); // https://dns.yandex.com
            AddDNS("Quad9", "9.9.9.9"); // https://quad9.net

            foreach (AdapterInfo adapter in AdapterInfo.GetEnabledAdapters())
            {
                cbAdapters.Items.Add(adapter);
            }

            if (cbAdapters.Items.Count > 0)
            {
                cbAdapters.SelectedIndex = 0;
            }
        }

        private void AddDNS(string name, string primary = null, string secondary = null)
        {
            cbDNSType.Items.Add(new DNSInfo(name, primary, secondary));
        }

        private void cbAdapters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAdapters.SelectedItem is AdapterInfo adapter)
            {
                string[] dns = adapter.GetDNS();

                if (dns != null && dns.Length > 0)
                {
                    cbAutomatic.Checked = false;
                    txtPreferredDNS.Text = dns[0];

                    if (dns.Length > 1)
                    {
                        txtAlternateDNS.Text = dns[1];
                    }
                    else
                    {
                        txtAlternateDNS.Text = "";
                    }
                }
                else
                {
                    cbAutomatic.Checked = true;
                    txtPreferredDNS.Text = "";
                    txtAlternateDNS.Text = "";
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
            if (cbDNSType.SelectedIndex > 0 && cbDNSType.SelectedItem is DNSInfo dnsInfo)
            {
                txtPreferredDNS.Text = dnsInfo.PrimaryDNS;
                txtAlternateDNS.Text = dnsInfo.SecondaryDNS;
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

        private async Task SendPing(string ip)
        {
            if (!string.IsNullOrEmpty(ip))
            {
                btnPingPrimary.Enabled = btnPingSecondary.Enabled = false;

                await Task.Run(() =>
                {
                    PingResult pingResult = PingHelper.PingHost(ip);
                    MessageBox.Show(pingResult.ToString(), "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                });

                btnPingPrimary.Enabled = btnPingSecondary.Enabled = true;
            }
        }

        private async void btnPingPrimary_Click(object sender, EventArgs e)
        {
            await SendPing(txtPreferredDNS.Text);
        }

        private async void btnPingSecondary_Click(object sender, EventArgs e)
        {
            await SendPing(txtAlternateDNS.Text);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cbAdapters.SelectedItem is AdapterInfo adapter)
            {
                uint result;

                try
                {
                    if (cbAutomatic.Checked)
                    {
                        result = adapter.SetDNSAutomatic();
                    }
                    else
                    {
                        string primaryDNS = txtPreferredDNS.Text.Trim();
                        string secondaryDNS = txtAlternateDNS.Text.Trim();

                        if (Helpers.IsValidIPAddress(primaryDNS) && (string.IsNullOrEmpty(secondaryDNS) || Helpers.IsValidIPAddress(secondaryDNS)))
                        {
                            result = adapter.SetDNS(primaryDNS, secondaryDNS);
                        }
                        else
                        {
                            throw new Exception("Not valid IP address.");
                        }
                    }

                    if (result == 0)
                    {
                        NativeMethods.DnsFlushResolverCache();
                        MessageBox.Show(Resources.DNSChangerForm_btnSave_Click_DNS_successfully_set_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (result == 1)
                    {
                        MessageBox.Show(Resources.DNSChangerForm_btnSave_Click_DNS_successfully_set__Reboot_is_required_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (result > 1)
                    {
                        MessageBox.Show(Resources.DNSChangerForm_btnSave_Click_Setting_DNS_failed_with_error_code_ + " " + result, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Resources.DNSChangerForm_btnSave_Click_Setting_DNS_failed_ + "\r\n" + ex, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}