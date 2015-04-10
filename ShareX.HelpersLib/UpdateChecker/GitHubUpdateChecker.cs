#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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

namespace ShareX.HelpersLib
{
    public class GitHubUpdateChecker : UpdateChecker
    {
        public string Owner { get; private set; }
        public string Repo { get; private set; }
        public bool IncludePreRelease { get; set; }

        private const string APIURL = "https://api.github.com";

        private string ReleasesURL
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

        public override void CheckUpdate()
        {
            try
            {
                List<GitHubRelease> releases = GetReleases();

                if (releases != null && releases.Count > 0)
                {
                    GitHubRelease latestRelease;

                    if (IncludePreRelease)
                    {
                        latestRelease = releases[0];
                    }
                    else
                    {
                        latestRelease = releases.FirstOrDefault(x => !x.prerelease);
                    }

                    if (latestRelease != null && !string.IsNullOrEmpty(latestRelease.tag_name) && latestRelease.tag_name.Length > 1 && latestRelease.tag_name[0] == 'v')
                    {
                        LatestVersion = new Version(latestRelease.tag_name.Substring(1));

                        if (latestRelease.assets != null && latestRelease.assets.Count > 0)
                        {
                            foreach (GitHubAsset asset in latestRelease.assets)
                            {
                                if (asset != null && !string.IsNullOrEmpty(asset.name) && asset.name.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    Filename = asset.name;
                                    DownloadURL = asset.url;
                                    RefreshStatus();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e, "GitHub update check failed");
            }

            Status = UpdateStatus.UpdateCheckFailed;
        }

        public string GetLatestDownloadURL()
        {
            List<GitHubRelease> releases = GetReleases();

            if (releases != null && releases.Count > 0)
            {
                return GetDownloadURL(releases[0]);
            }

            return null;
        }

        private string GetDownloadURL(GitHubRelease release)
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

        private List<GitHubRelease> GetReleases()
        {
            using (WebClient wc = new WebClient())
            {
                wc.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                wc.Headers.Add("user-agent", "ShareX");
                wc.Proxy = Proxy;

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
        public string updated_at { get; set; }
    }
}