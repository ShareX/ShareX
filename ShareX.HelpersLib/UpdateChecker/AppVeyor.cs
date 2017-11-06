#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;

namespace ShareX.HelpersLib
{
    public class AppVeyor
    {
        public string AccountName { get; set; }
        public string ProjectSlug { get; set; }
        public IWebProxy Proxy { get; set; }

        private const string APIURL = "https://ci.appveyor.com/api";

        public AppVeyorProject GetProjectByBranch(string branch = "master")
        {
            string url = $"{APIURL}/projects/{AccountName}/{ProjectSlug}/branch/{branch}";

            using (WebClient wc = new WebClient())
            {
                wc.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                wc.Headers.Add(HttpRequestHeader.UserAgent, ShareXResources.UserAgent);
                wc.Proxy = Proxy;

                string response = wc.DownloadString(url);

                if (!string.IsNullOrEmpty(response))
                {
                    return JsonConvert.DeserializeObject<AppVeyorProject>(response);
                }
            }

            return null;
        }

        public AppVeyorProjectArtifact[] GetArtifacts(string jobId)
        {
            string url = $"{APIURL}/buildjobs/{jobId}/artifacts";

            using (WebClient wc = new WebClient())
            {
                wc.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                wc.Headers.Add(HttpRequestHeader.UserAgent, ShareXResources.UserAgent);
                wc.Proxy = Proxy;

                string response = wc.DownloadString(url);

                if (!string.IsNullOrEmpty(response))
                {
                    return JsonConvert.DeserializeObject<AppVeyorProjectArtifact[]>(response);
                }
            }

            return null;
        }

        public string GetArtifactDownloadURL(string jobId, string fileName)
        {
            return $"{APIURL}/buildjobs/{jobId}/artifacts/{fileName}";
        }
    }

    public class AppVeyorProject
    {
        public AppVeyorProjectInfo project { get; set; }
        public AppVeyorProjectBuild build { get; set; }
    }

    public class AppVeyorProjectInfo
    {
    }

    public class AppVeyorProjectBuild
    {
        public AppVeyorProjectJob[] jobs { get; set; }
        public string version { get; set; }
        public string status { get; set; }
    }

    public class AppVeyorProjectJob
    {
        public string jobId { get; set; }
        public string name { get; set; }
        public string osType { get; set; }
        public string status { get; set; }
    }

    public class AppVeyorProjectArtifact
    {
        public string fileName { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public long size { get; set; }
    }
}