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

using ShareX.HelpersLib;
using ShareX.UploadersLib.ImageUploaders;
using System;
using System.Windows.Forms;

namespace ShareX.UploadersLib.SharingServices
{
    public class TwitterSharingService : URLSharingService
    {
        public override URLSharingServices EnumValue { get; } = URLSharingServices.Twitter;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.TwitterOAuthInfoList != null && config.TwitterOAuthInfoList.IsValidIndex(config.TwitterSelectedAccount) &&
                OAuthInfo.CheckOAuth(config.TwitterOAuthInfoList[config.TwitterSelectedAccount]);
        }

        public override URLSharer CreateSharer(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new TwitterSharer(config);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpTwitter;
    }

    public sealed class TwitterSharer : URLSharer
    {
        private UploadersConfig config;

        public TwitterSharer(UploadersConfig config)
        {
            this.config = config;
        }

        public override UploadResult ShareURL(string url)
        {
            UploadResult result = new UploadResult { URL = url, IsURLExpected = false };

            OAuthInfo twitterOAuth = config.TwitterOAuthInfoList[config.TwitterSelectedAccount];

            if (config.TwitterSkipMessageBox)
            {
                try
                {
                    new Twitter(twitterOAuth).TweetMessage(url);
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                }
            }
            else
            {
                using (TwitterTweetForm twitter = new TwitterTweetForm(twitterOAuth, url))
                {
                    twitter.ShowDialog();
                }
            }

            //URLHelpers.OpenURL("https://twitter.com/intent/tweet?text=" + encodedUrl);

            return result;
        }
    }
}