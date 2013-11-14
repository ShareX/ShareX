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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;

namespace HelpersLib
{
    public class GitHubUpdateChecker
    {
        private const string APIURL = "https://api.github.com";

        public IWebProxy Proxy { get; set; }
        public string Owner { get; set; }
        public string Repo { get; set; }

        public string ReleasesURL
        {
            get
            {
                return string.Format("{0}/repos/{1}/{2}/releases", APIURL, Owner, Repo);
            }
        }

        public GitHubUpdateChecker(string owner, string repo)
        {
            Owner = owner;
            Repo = repo;
        }

        public void CheckUpdate(Version currentVersion)
        {
            List<GitHubRelease> releases = GetReleases();

            if (releases != null && releases.Count > 0)
            {
                GitHubRelease latestRelease = releases[0];

                if (latestRelease != null && !string.IsNullOrEmpty(latestRelease.tag_name) && latestRelease.tag_name[0] == 'v')
                {
                    Version latestVersion = new Version(latestRelease.tag_name.Substring(1));
                    bool isUpdateExist = Helpers.CheckVersion(latestVersion, currentVersion);

                    if (isUpdateExist)
                    {
                        string downloadURL = GetDownloadURL(latestRelease);
                    }
                }
            }
        }

        public string GetDownloadURL(GitHubRelease release)
        {
            if (release.assets != null && release.assets.Count > 0)
            {
                GitHubAsset asset = release.assets[0];

                if (asset != null && !string.IsNullOrEmpty(asset.name))
                {
                    return string.Format("https://github.com/{0}/{1}/releases/download/{2}/{3}", Owner, Repo, release.tag_name, asset.name);
                }
            }

            return null;
        }

        public List<GitHubRelease> GetReleases()
        {
            RequestCachePolicy cachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

            using (WebClient wc = new WebClient { Proxy = Proxy, CachePolicy = cachePolicy })
            {
                string response = wc.DownloadString(ReleasesURL);

                if (!string.IsNullOrEmpty(response))
                {
                    return JsonConvert.DeserializeObject<List<GitHubRelease>>(response);
                }
            }

            return null;
        }
    }

    public class GitHubRelease
    {
        public string url { get; set; }
        public string assets_url { get; set; }
        public string upload_url { get; set; }
        public string html_url { get; set; }
        public int id { get; set; }
        public string tag_name { get; set; }
        public string target_commitish { get; set; }
        public string name { get; set; }
        public string body { get; set; }
        public bool draft { get; set; }
        public bool prerelease { get; set; }
        public string created_at { get; set; }
        public string published_at { get; set; }
        public List<GitHubAsset> assets { get; set; }
        public string tarball_url { get; set; }
        public string zipball_url { get; set; }
    }

    public class GitHubAsset
    {
        public string url { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string label { get; set; }
        public string content_type { get; set; }
        public string state { get; set; }
        public int size { get; set; }
        public int download_count { get; set; }
        public string created_at { get; set; }
        public string published_at { get; set; }
    }
}