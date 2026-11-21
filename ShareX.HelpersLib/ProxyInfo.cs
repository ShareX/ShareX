#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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
using System.Net;

namespace ShareX.HelpersLib
{
    public class ProxyInfo
    {
        public ProxyMethod ProxyMethod { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public ProxyInfo()
        {
            ProxyMethod = ProxyMethod.Manual;
        }

        public bool IsValidProxy()
        {
            return ProxyMethod == ProxyMethod.Manual && !string.IsNullOrEmpty(Host) && Port > 0;
        }

        public IWebProxy GetWebProxy()
        {
            try
            {
                if (IsValidProxy())
                {
                    NetworkCredential credentials = new NetworkCredential(Username, Password);
                    string address = string.Format("{0}:{1}", Host, Port);
                    return new WebProxy(address, true, null, credentials);
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e, "GetWebProxy failed.");
            }

            return null;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}:{2}", Username, Host, Port);
        }
    }
}