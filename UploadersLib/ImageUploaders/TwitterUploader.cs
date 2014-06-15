#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UploadersLib.HelperClasses;
using UploadersLib.SocialServices;

namespace UploadersLib.ImageUploaders
{
    public sealed class TwitterUploader : ImageUploader
    {
        public OAuthInfo AuthInfo { get; set; }

        public TwitterUploader(OAuthInfo oauth)
        {
            AuthInfo = oauth;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            using (TwitterMsg twitterMsg = new TwitterMsg())
            {
                twitterMsg.Length = Twitter.MessageMediaLimit;

                if (twitterMsg.ShowDialog() == DialogResult.OK)
                {
                    Twitter twitter = new Twitter(AuthInfo);
                    return twitter.TweetMessageWithMedia(twitterMsg.Message, stream, fileName);
                }
            }

            return new UploadResult() { IsURLExpected = false };
        }
    }
}