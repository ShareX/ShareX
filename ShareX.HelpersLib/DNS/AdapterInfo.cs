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

using System;
using System.Collections.Generic;
using System.Management;

namespace ShareX.HelpersLib
{
    public class AdapterInfo : IDisposable
    {
        private ManagementObject adapter;

        public AdapterInfo(ManagementObject adapter)
        {
            this.adapter = adapter;
        }

        public static List<AdapterInfo> GetEnabledAdapters()
        {
            List<AdapterInfo> adapters = new List<AdapterInfo>();

            using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            using (ManagementObjectCollection moc = mc.GetInstances())
            {
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        adapters.Add(new AdapterInfo(mo));
                    }
                    else
                    {
                        mo.Dispose();
                    }
                }
            }

            return adapters;
        }

        public bool IsEnabled()
        {
            return (bool)adapter["IPEnabled"];
        }

        public string GetCaption()
        {
            return (string)adapter["Caption"];
        }

        public string GetDescription()
        {
            return (string)adapter["Description"];
        }

        public string[] GetDNS()
        {
            return (string[])adapter["DnsServerSearchOrder"];
        }

        public uint SetDNS(string primary, string secondary)
        {
            using (ManagementBaseObject parameters = adapter.GetMethodParameters("SetDNSServerSearchOrder"))
            {
                if (string.IsNullOrEmpty(primary))
                {
                    // Obtain DNS server address automatically
                    parameters["DNSServerSearchOrder"] = null;
                }
                else if (string.IsNullOrEmpty(secondary))
                {
                    parameters["DNSServerSearchOrder"] = new string[] { primary };
                }
                else
                {
                    parameters["DNSServerSearchOrder"] = new string[] { primary, secondary };
                }

                // http://msdn.microsoft.com/en-us/library/aa393295(v=vs.85).aspx
                using (ManagementBaseObject result = adapter.InvokeMethod("SetDNSServerSearchOrder", parameters, null))
                {
                    return (uint)result["ReturnValue"];
                }
            }
        }

        public uint SetDNSAutomatic()
        {
            return SetDNS(null, null);
        }

        public void Dispose()
        {
            if (adapter != null)
            {
                adapter.Dispose();
            }
        }

        public override string ToString()
        {
            return GetDescription();
        }
    }
}