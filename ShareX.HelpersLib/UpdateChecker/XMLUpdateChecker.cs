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
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ShareX.HelpersLib
{
    public class XMLUpdateChecker : UpdateChecker
    {
        public string URL { get; private set; }
        public string ApplicationName { get; private set; }

        public XMLUpdateChecker(string url, string applicationName)
        {
            URL = url;
            ApplicationName = applicationName;
        }

        public override async Task CheckUpdateAsync()
        {
            try
            {
                string response = await WebHelpers.DownloadStringAsync(URL);

                using (StringReader sr = new StringReader(response))
                using (XmlTextReader xml = new XmlTextReader(sr))
                {
                    XDocument xd = XDocument.Load(xml);

                    if (xd != null)
                    {
                        string node;

                        switch (ReleaseType)
                        {
                            default:
                            case ReleaseChannelType.Stable:
                                node = "Stable";
                                break;
                            case ReleaseChannelType.Beta:
                                node = "Beta|Stable";
                                break;
                            case ReleaseChannelType.Dev:
                                node = "Dev|Beta|Stable";
                                break;
                        }

                        string path = string.Format("Update/{0}/{1}", ApplicationName, node);
                        XElement xe = xd.GetNode(path);

                        if (xe != null)
                        {
                            LatestVersion = new Version(xe.Element("Version").Value);
                            DownloadURL = xe.Element("URL").Value;
                            RefreshStatus();
                            return;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e, "XML update check failed");
            }

            Status = UpdateStatus.UpdateCheckFailed;
        }
    }
}