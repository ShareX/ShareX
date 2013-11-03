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
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace UpdateCheckerLib
{
    public class UpdateChecker
    {
        public string URL { get; private set; }
        public string ApplicationName { get; private set; }
        public Version ApplicationVersion { get; private set; }
        public ReleaseChannelType ReleaseChannel { get; private set; }
        public IWebProxy Proxy { get; private set; }
        public UpdateInfo UpdateInfo { get; private set; }

        public UpdateChecker(string url, string applicationName, Version applicationVersion,
            ReleaseChannelType channel = ReleaseChannelType.Stable, IWebProxy proxy = null)
        {
            URL = url;
            ApplicationName = applicationName;
            ApplicationVersion = applicationVersion;
            ReleaseChannel = channel;
            Proxy = proxy;
        }

        public bool CheckUpdate()
        {
            UpdateInfo = new UpdateInfo(ReleaseChannel);
            UpdateInfo.CurrentVersion = ApplicationVersion;

            try
            {
                RequestCachePolicy cachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

                using (WebClient wc = new WebClient { Proxy = Proxy, CachePolicy = cachePolicy })
                using (MemoryStream ms = new MemoryStream(wc.DownloadData(URL)))
                using (XmlTextReader xml = new XmlTextReader(ms))
                {
                    XDocument xd = XDocument.Load(xml);

                    if (xd != null)
                    {
                        string node;

                        switch (ReleaseChannel)
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
                            UpdateInfo.LatestVersion = new Version(xe.GetValue("Version"));
                            UpdateInfo.URL = xe.GetValue("URL");

                            string date = xe.GetValue("Date");
                            if (!string.IsNullOrEmpty(date))
                            {
                                DateTime dateTime;
                                if (DateTime.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                                {
                                    UpdateInfo.Date = dateTime;
                                }
                            }

                            UpdateInfo.Summary = xe.GetValue("Summary");

                            if (UpdateInfo.IsUpdateRequired)
                            {
                                UpdateInfo.Status = UpdateStatus.UpdateRequired;

                                if (!string.IsNullOrEmpty(UpdateInfo.Summary) && UpdateInfo.Summary.IsValidUrl())
                                {
                                    try
                                    {
                                        wc.Encoding = Encoding.UTF8;
                                        UpdateInfo.Summary = wc.DownloadString(UpdateInfo.Summary.Trim());
                                    }
                                    catch (Exception ex)
                                    {
                                        DebugHelper.WriteException(ex);
                                    }
                                }
                            }
                            else
                            {
                                UpdateInfo.Status = UpdateStatus.UpToDate;
                            }

                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
            }

            UpdateInfo.Status = UpdateStatus.UpdateCheckFailed;

            return false;
        }
    }
}