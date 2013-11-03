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
using System.ComponentModel;

namespace UploadersLib
{
    public class UploadersAPIKeys
    {
        #region Image Uploaders

        [Category("ImageShack"), DefaultValue(ApiKeys.ImageShackKey), Description("ImageShack Key")]
        public string ImageShackKey { get; set; }

        [Category("TinyPic"), DefaultValue(ApiKeys.TinyPicID), Description("TinyPic ID")]
        public string TinyPicID { get; set; }

        [Category("TinyPic"), DefaultValue(ApiKeys.TinyPicKey), Description("TinyPic Key")]
        public string TinyPicKey { get; set; }

        /*[Category("Imgur"), DefaultValue(ApiKeys.ImgurAnonymousKey), Description("Imgur Anonymous Key")]
        public string ImgurAnonymousKey { get; set; }

        [Category("Imgur"), DefaultValue(ApiKeys.ImgurConsumerKey), Description("Imgur Consumer Key")]
        public string ImgurConsumerKey { get; set; }

        [Category("Imgur"), DefaultValue(ApiKeys.ImgurConsumerSecret), Description("Imgur Consumer Secret")]
        public string ImgurConsumerSecret { get; set; }*/

        [Category("Imgur"), DefaultValue(ApiKeys.ImgurClientID), Description("Imgur Client ID")]
        public string ImgurClientID { get; set; }

        [Category("Imgur"), DefaultValue(ApiKeys.ImgurClientSecret), Description("Imgur Client Secret")]
        public string ImgurClientSecret { get; set; }

        [Category("Flickr"), DefaultValue(ApiKeys.FlickrKey), Description("Flickr Key")]
        public string FlickrKey { get; set; }

        [Category("Flickr"), DefaultValue(ApiKeys.FlickrSecret), Description("Flickr Secret")]
        public string FlickrSecret { get; set; }

        [Category("Photobucket"), DefaultValue(ApiKeys.PhotobucketConsumerKey), Description("Photobucket Consumer Key")]
        public string PhotobucketConsumerKey { get; set; }

        [Category("Photobucket"), DefaultValue(ApiKeys.PhotobucketConsumerSecret), Description("Photobucket Consumer Secret")]
        public string PhotobucketConsumerSecret { get; set; }

        [Category("Uploadscreenshot"), DefaultValue(ApiKeys.UploadScreenshotKey), Description("Uploadscreenshot Key")]
        public string UploadScreenshotKey { get; set; }

        [Category("ImageBam"), DefaultValue(ApiKeys.ImageBamKey), Description("ImageBam Key")]
        public string ImageBamKey { get; set; }

        [Category("ImageBam"), DefaultValue(ApiKeys.ImageBamSecret), Description("ImageBam Secret")]
        public string ImageBamSecret { get; set; }

        [Category("TwitSnaps"), DefaultValue(ApiKeys.TwitsnapsKey), Description("TwitSnaps Key")]
        public string TwitsnapsKey { get; set; }

        [Category("TwitPic"), DefaultValue(ApiKeys.TwitPicKey), Description("TwitPic Key")]
        public string TwitPicKey { get; set; }

        #endregion Image Uploaders

        #region File Uploaders

        [Category("Dropbox"), DefaultValue(ApiKeys.DropboxConsumerKey), Description("Dropbox Consumer Key")]
        public string DropboxConsumerKey { get; set; }

        [Category("Dropbox"), DefaultValue(ApiKeys.DropboxConsumerSecret), Description("Dropbox Consumer Secret")]
        public string DropboxConsumerSecret { get; set; }

        [Category("Box"), DefaultValue(ApiKeys.BoxKey), Description("Box Key")]
        public string BoxKey { get; set; }

        [Category("Minus"), DefaultValue(ApiKeys.MinusConsumerKey), Description("Minus Consumer Secret")]
        public string MinusConsumerKey { get; set; }

        [Category("Minus"), DefaultValue(ApiKeys.MinusConsumerSecret), Description("Minus Consumer Secret")]
        public string MinusConsumerSecret { get; set; }

        [Category("SendSpace"), DefaultValue(ApiKeys.SendSpaceKey), Description("SendSpace Key")]
        public string SendSpaceKey { get; set; }

        [Category("Drop.IO"), Browsable(false), DefaultValue(ApiKeys.DropIOKey), Description("Drop.IO Consumer Secret")]
        public string DropIOKey { get; set; }

        [Category("Ge.tt"), Browsable(false), DefaultValue(ApiKeys.Ge_ttKey), Description("Ge.tt Key")]
        public string Ge_ttKey { get; set; }

        [Category("Atlassian Jira"), DefaultValue(ApiKeys.JiraConsumerKey), Description("Atlassian Jira Consumer Key")]
        public string JiraConsumerKey { get; set; }

        #endregion File Uploaders

        #region Text Uploaders

        [Category("Pastebin"), DefaultValue(ApiKeys.PastebinKey), Description("Pastebin Consumer Secret")]
        public string PastebinKey { get; set; }

        [Category("Pastebin"), DefaultValue(ApiKeys.PastebinCaKey), Description("Pastebin Consumer Secret")]
        public string PastebinCaKey { get; set; }

        #endregion Text Uploaders

        #region URL Shorteners

        [Category("bit.ly"), DefaultValue(ApiKeys.BitlyLogin), Description("bit.ly Consumer Secret")]
        public string BitlyLogin { get; set; }

        [Category("bit.ly"), DefaultValue(ApiKeys.BitlyKey), Description("bit.ly Consumer Secret")]
        public string BitlyKey { get; set; }

        [Category("bit.ly"), DefaultValue(ApiKeys.BitlyConsumerKey), Description("bit.ly Consumer Secret")]
        public string BitlyConsumerKey { get; set; }

        [Category("bit.ly"), DefaultValue(ApiKeys.BitlyConsumerSecret), Description("bit.ly Consumer Secret")]
        public string BitlyConsumerSecret { get; set; }

        [Category("kl.am"), Browsable(false), DefaultValue(ApiKeys.KlamKey), Description("kl.am key")]
        public string KlamKey { get; set; }

        [Category("3.ly"), Browsable(false), DefaultValue(ApiKeys.ThreelyKey), Description("3.ly key")]
        public string ThreelyKey { get; set; }

        #endregion URL Shorteners

        #region Other Services

        [Category("Google"), DefaultValue(ApiKeys.GoogleClientID), Description("Google Client ID")]
        public string GoogleClientID { get; set; }

        [Category("Google"), DefaultValue(ApiKeys.GoogleClientSecret), Description("Google Client Secret")]
        public string GoogleClientSecret { get; set; }

        [Category("Google"), DefaultValue(ApiKeys.GoogleAPIKey), Description("Google API Key")]
        public string GoogleAPIKey { get; set; }

        [Category("Twitter"), DefaultValue(ApiKeys.TwitterConsumerKey), Description("Twitter Consumer Secret")]
        public string TwitterConsumerKey { get; set; }

        [Category("Twitter"), DefaultValue(ApiKeys.TwitterConsumerSecret), Description("Twitter Consumer Secret")]
        public string TwitterConsumerSecret { get; set; }

        [Category("Picnik"), DefaultValue(ApiKeys.PicnikKey), Description("Picnik Key")]
        public string PicnikKey { get; set; }

        #endregion Other Services

        public UploadersAPIKeys()
        {
            this.ApplyDefaultPropertyValues();
        }
    }
}