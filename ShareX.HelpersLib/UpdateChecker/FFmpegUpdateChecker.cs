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

namespace ShareX.HelpersLib
{
    public class FFmpegUpdateChecker : GitHubUpdateChecker
    {
        public FFmpegArchitecture Architecture { get; private set; }

        public FFmpegUpdateChecker(string owner, string repo) : base(owner, repo)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                Architecture = FFmpegArchitecture.win64;
            }
            else
            {
                Architecture = FFmpegArchitecture.win32;
            }
        }

        public FFmpegUpdateChecker(string owner, string repo, FFmpegArchitecture architecture) : base(owner, repo)
        {
            Architecture = architecture;
        }

        protected override bool UpdateReleaseInfo(GitHubRelease release, bool isPortable, bool isBrowserDownloadURL)
        {
            if (release != null && !string.IsNullOrEmpty(release.tag_name) && release.tag_name.Length > 1 && release.tag_name[0] == 'v')
            {
                LatestVersion = new Version(release.tag_name.Substring(1));

                if (release.assets != null && release.assets.Length > 0)
                {
                    string endsWith;

                    switch (Architecture)
                    {
                        default:
                        case FFmpegArchitecture.win64:
                            endsWith = "win64.zip";
                            break;
                        case FFmpegArchitecture.win32:
                            endsWith = "win32.zip";
                            break;
                        case FFmpegArchitecture.macos64:
                            endsWith = "macos64.zip";
                            break;
                    }

                    foreach (GitHubAsset asset in release.assets)
                    {
                        if (asset != null && !string.IsNullOrEmpty(asset.name) && asset.name.EndsWith(endsWith, StringComparison.OrdinalIgnoreCase))
                        {
                            FileName = asset.name;

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
    }
}