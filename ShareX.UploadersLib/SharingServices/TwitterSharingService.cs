using System;
using System.Windows.Forms;
using ShareX.HelpersLib;
using ShareX.UploadersLib.HelperClasses;
using ShareX.UploadersLib.ImageUploaders;
using ShareX.UploadersLib.Properties;

namespace ShareX.UploadersLib.SharingServices
{
    public class TwitterSharingService : SharingService
    {
        public override URLSharingServices EnumValue { get; } = URLSharingServices.Twitter;
        public override bool CheckConfig(UploadersConfig config)
        {
            return config.TwitterOAuthInfoList != null
                   && config.TwitterOAuthInfoList.IsValidIndex(config.TwitterSelectedAccount)
                   && OAuthInfo.CheckOAuth(config.TwitterOAuthInfoList[config.TwitterSelectedAccount]);
        }

        public override void ShareURL(string url, UploadersConfig uploadersConfig)
        {
            if (!CheckConfig(uploadersConfig))
            {
                //URLHelpers.OpenURL("https://twitter.com/intent/tweet?text=" + encodedUrl);
                MessageBox.Show(Resources.TaskHelpers_TweetMessage_Unable_to_find_valid_Twitter_account_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            OAuthInfo twitterOAuth = uploadersConfig.TwitterOAuthInfoList[uploadersConfig.TwitterSelectedAccount];

            if (uploadersConfig.TwitterSkipMessageBox)
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
        }
    }
}