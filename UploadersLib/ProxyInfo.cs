#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

using HelpersLib;
using Starksoft.Net.Proxy;
using System.Net;

namespace UploadersLib
{
    public class ProxyInfo
    {
        public ProxyMethod ProxyMethod { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public ProxyType ProxyType { get; set; }

        public ProxyInfo()
        {
            ProxyMethod = ProxyMethod.Manual;
            ProxyType = ProxyType.HTTP;
        }

        public ProxyInfo(string username, string password, string host, int port)
            : this()
        {
            Username = username;
            Password = password;
            Host = host;
            Port = port;
        }

        public bool IsValidProxy()
        {
            if (ProxyMethod == ProxyMethod.Manual)
            {
                return !string.IsNullOrEmpty(Host) && Port > 0;
            }

            if (ProxyMethod == ProxyMethod.Automatic)
            {
                WebProxy systemProxy = Helpers.GetDefaultWebProxy();

                if (systemProxy != null && systemProxy.Address != null && !string.IsNullOrEmpty(systemProxy.Address.Host) && systemProxy.Address.Port > 0)
                {
                    Host = systemProxy.Address.Host;
                    Port = systemProxy.Address.Port;
                    ProxyType = ProxyType.HTTP;
                    return true;
                }
            }

            return false;
        }

        public IWebProxy GetWebProxy()
        {
            if (IsValidProxy())
            {
                NetworkCredential credentials = new NetworkCredential(Username, Password);
                string address = string.Format("{0}:{1}", Host, Port);
                return new WebProxy(address, true, null, credentials);
            }

            return null;
        }

        public IProxyClient GetProxyClient()
        {
            if (IsValidProxy())
            {
                Starksoft.Net.Proxy.ProxyType proxyType;

                switch (ProxyType)
                {
                    case ProxyType.HTTP:
                        proxyType = Starksoft.Net.Proxy.ProxyType.Http;
                        break;
                    case ProxyType.SOCKS4:
                        proxyType = Starksoft.Net.Proxy.ProxyType.Socks4;
                        break;
                    case ProxyType.SOCKS4a:
                        proxyType = Starksoft.Net.Proxy.ProxyType.Socks4a;
                        break;
                    case ProxyType.SOCKS5:
                        proxyType = Starksoft.Net.Proxy.ProxyType.Socks5;
                        break;
                    default:
                        proxyType = Starksoft.Net.Proxy.ProxyType.None;
                        break;
                }

                ProxyClientFactory proxy = new ProxyClientFactory();
                return proxy.CreateProxyClient(proxyType, Host, Port, Username, Password);
            }

            return null;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}:{2} ({3})", Username, Host, Port, ProxyType.ToString());
        }
    }
}