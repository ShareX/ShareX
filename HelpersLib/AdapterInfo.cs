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
using System.Linq;
using System.Management;
using System.Text;

namespace HelpersLib
{
    public class AdapterInfo : IDisposable
    {
        private ManagementObject adapter;

        public AdapterInfo(ManagementObject adapter)
        {
            this.adapter = adapter;
        }

        public bool IsEnabled()
        {
            return (bool)adapter["IPEnabled"];
        }

        public string GetDescription()
        {
            return (string)adapter["Description"];
        }

        public string[] GetDNS()
        {
            return (string[])adapter["DnsServerSearchOrder"];
        }

        public bool SetDNS(string primary, string secondary)
        {
            if (IsEnabled())
            {
                try
                {
                    using (ManagementBaseObject parameters = adapter.GetMethodParameters("SetDNSServerSearchOrder"))
                    {
                        if (parameters != null)
                        {
                            if (primary == null || secondary == null)
                            {
                                // Obtain DNS server address automatically
                                parameters["DNSServerSearchOrder"] = null;
                            }
                            else
                            {
                                parameters["DNSServerSearchOrder"] = new string[] { primary, secondary };
                            }

                            using (ManagementBaseObject result = adapter.InvokeMethod("SetDNSServerSearchOrder", parameters, null))
                            {
                                return (uint)result["ReturnValue"] == 0;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }

            return false;
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