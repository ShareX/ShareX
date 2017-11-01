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

namespace ShareX.HelpersLib
{
    public class GitHubUpdateChecker : UpdateChecker
    {
        public string Owner { get; private set; }
        public string Repo { get; private set; }
        public bool IncludePreRelease { get; set; }
        public bool IsPreRelease { get; private set; }

        private const string APIURL = "https://api.github.com";

        private string ReleasesURL => $"{APIURL}/repos/{Owner}/{Repo}/releases";

        public GitHubUpdateChecker(string owner, string repo)
        {
            Owner = owner;
            Repo = repo;
        }

        public override void CheckUpdate()
        {
            try
            {
                GitHubRelease latestRelease = GetLatestRelease(IncludePreRelease);

                if (UpdateReleaseInfo(latestRelease, IsPortable, IsPortable ? true : false))
                {
                    RefreshStatus();
                    return;
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e, "GitHub update check failed");
            }

            Status = UpdateStatus.UpdateCheckFailed;
        }

        public string GetLatestDownloadURL(bool includePreRelease, bool isPortable, bool isBrowserDownloadURL)
        {
            try
            {
                GitHubRelease latestRelease = GetLatestRelease(includePreRelease);

                if (UpdateReleaseInfo(latestRelease, isPortable, isBrowserDownloadURL))
                {
                    return DownloadURL;
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return null;
        }

        private List<GitHubRelease> GetReleases()
        {
            using (WebClient wc = new WebClient())
            {
                wc.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                wc.Headers.Add(HttpRequestHeader.UserAgent, ShareXResources.UserAgent);
                wc.Proxy = Proxy;

                string response = wc.DownloadString(ReleasesURL);

                if (!string.IsNullOrEmpty(response))
                {
                    return JsonConvert.DeserializeObject<List<GitHubRelease>>(response);
                }
            }

            return null;
        }

        private GitHubRelease GetLatestRelease(bool includePreRelease)
        {
            GitHubRelease latestRelease = null;

            List<GitHubRelease> releases = GetReleases();

            if (releases != null && releases.Count > 0)
            {
                if (includePreRelease)
                {
                    latestRelease = releases[0];
                }
                else
                {
                    latestRelease = releases.FirstOrDefault(x => !x.prerelease);
                }
            }

            return latestRelease;
        }

        private bool UpdateReleaseInfo(GitHubRelease release, bool isPortable, bool isBrowserDownloadURL)
        {
            if (release != null && !string.IsNullOrEmpty(release.tag_name) && release.tag_name.Length > 1 && release.tag_name[0] == 'v')
            {
                LatestVersion = new Version(release.tag_name.Substring(1));

                if (release.assets != null && release.assets.Count > 0)
                {
                    string endsWith;

                    if (isPortable)
                    {
                        endsWith = "portable.zip";
                    }
                    else
                    {
                        endsWith = ".exe";
                    }

                    foreach (GitHubAsset asset in release.assets)
                    {
                        if (asset != null && !string.IsNullOrEmpty(asset.name) && asset.name.EndsWith(endsWith, StringComparison.InvariantCultureIgnoreCase))
                        {
                            Filename = asset.name;

                            if (isBrowserDownloadURL)
                            {
                                DownloadURL = asset.browser_download_url;
                            }
                            else
                            {
                                DownloadURL = asset.url;
                            }

                            IsPreRelease = release.prerelease;

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private class GitHubRelease
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

        private class GitHubAsset
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
            public string browser_download_url { get; set; }
        }
    }
}