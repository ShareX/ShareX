#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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

using System.ComponentModel;

namespace ShareX.UploadersLib
{
    [Description("Image uploaders"), DefaultValue(Imgur)]
    public enum ImageDestination
    {
        [Description("Chevereto")]
        Chevereto,
        [Description("Flickr")]
        Flickr,
        [Description("Google Photos (Picasa)")]
        Picasa,
        [Description("ImageShack")]
        ImageShack,
        [Description("Imgland")]
        Imgland,
        [Description("Imgur")]
        Imgur,
        [Description("Photobucket")]
        Photobucket,
        [Description("SLiMG")]
        Slimg,
        [Description("SomeImage")]
        SomeImage,
        [Description("TinyPic")]
        TinyPic,
        [Description("Twitter")]
        Twitter,
        [Description("vgy.me")]
        Vgyme,
        CustomImageUploader, // Localized
        FileUploader // Localized
    }

    [Description("Text uploaders"), DefaultValue(Pastebin)]
    public enum TextDestination
    {
        [Description("GitHub Gist")]
        Gist,
        [Description("Hastebin")]
        Hastebin,
        [Description("OneTimeSecret")]
        OneTimeSecret,
        [Description("Paste.ee")]
        Paste_ee,
        [Description("Paste2")]
        Paste2,
        [Description("Pastebin")]
        Pastebin,
        [Description("Pastee.org")]
        Pastee,
        [Description("Slexy")]
        Slexy,
        [Description("uPaste")]
        Upaste,
        CustomTextUploader, // Localized
        FileUploader // Localized
    }

    [Description("File uploaders"), DefaultValue(Dropbox)]
    public enum FileDestination
    {
        [Description("Amazon S3")]
        AmazonS3,
        [Description("Box")]
        Box,
        [Description("Dropbox")]
        Dropbox,
        [Description("Dropfile")]
        Dropfile,
        [Description("FTP")]
        FTP,
        [Description("Ge.tt")]
        Ge_tt,
        [Description("Gfycat")]
        Gfycat,
        [Description("Google Drive")]
        GoogleDrive,
        [Description("Hostr")]
        Localhostr,
        [Description("JIRA")]
        Jira,
        [Description("Lambda")]
        Lambda,
        [Description("Lithiio")]
        Lithiio,
        [Description("MediaFire")]
        MediaFire,
        [Description("MEGA")]
        Mega,
        [Description("Minus")]
        Minus,
        [Description("OneDrive")]
        OneDrive,
        [Description("ownCloud")]
        OwnCloud,
        [Description("Pomf")]
        Pomf,
        [Description("Pushbullet")]
        Pushbullet,
        [Description("puush")]
        Puush,
        [Description("s-ul")]
        Sul,
        [Description("Seafile")]
        Seafile,
        [Description("SendSpace")]
        SendSpace,
        [Description("Streamable")]
        Streamable,
        [Description("transfer.sh")]
        Transfersh,
        [Description("Uguu")]
        Uguu,
        [Description("Up1")]
        Up1,
        [Description("VideoBin")]
        VideoBin,
        SharedFolder, // Localized
        Email, // Localized
        CustomFileUploader // Localized
    }

    [Description("URL shorteners"), DefaultValue(BITLY)]
    public enum UrlShortenerType
    {
        [Description("2.gp")]
        TwoGP,
        [Description("adf.ly")]
        AdFly,
        [Description("bit.ly")]
        BITLY,
        [Description("coinurl.com")]
        CoinURL,
        [Description("goo.gl")]
        Google,
        [Description("is.gd")]
        ISGD,
        [Description("Polr")]
        Polr,
        [Description("qr.net")]
        QRnet,
        [Description("tinyurl.com")]
        TINYURL,
        [Description("turl.ca")]
        TURL,
        [Description("v.gd")]
        VGD,
        [Description("vurl.com")]
        VURL,
        [Description("yourls.org")]
        YOURLS,
        CustomURLShortener // Localized
    }

    [Description("URL sharing services"), DefaultValue(Twitter)]
    public enum URLSharingServices
    {
        Email, // Localized
        [Description("Delicious")]
        Delicious,
        [Description("Facebook")]
        Facebook,
        [Description("Google+")]
        GooglePlus,
        [Description("LinkedIn")]
        LinkedIn,
        [Description("Pinterest")]
        Pinterest,
        [Description("Pushbullet")]
        Pushbullet
        [Description("Reddit")]
        Reddit,
        [Description("StumbleUpon")]
        StumbleUpon,
        [Description("Tumblr")]
        Tumblr,
        [Description("Twitter")]
        Twitter,
        [Description("VK")]
        VK,
    }

    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        PATCH,
        DELETE
    }

    public enum ResponseType // Localized
    {
        Text,
        RedirectionURL,
        Headers,
        LocationHeader
    }

    public enum FTPProtocol
    {
        [Description("FTP")]
        FTP,
        [Description("FTPS (FTP over SSL)")]
        FTPS,
        [Description("SFTP (SSH FTP)")]
        SFTP
    }

    public enum BrowserProtocol
    {
        [Description("http://")]
        http,
        [Description("https://")]
        https,
        [Description("ftp://")]
        ftp,
        [Description("ftps://")]
        ftps,
        [Description("file://")]
        file
    }

    public enum Privacy
    {
        Public,
        Private
    }

    public enum AccountType
    {
        [Description("Anonymous")]
        Anonymous,
        [Description("User")]
        User
    }

    public enum LinkFormatEnum
    {
        [Description("Full URL")]
        URL,
        [Description("Full Image for Forums")]
        ForumImage,
        [Description("Full Image as HTML")]
        HTMLImage,
        [Description("Full Image for Wiki")]
        WikiImage,
        [Description("Shortened URL")]
        ShortenedURL,
        [Description("Linked Thumbnail for Forums")]
        ForumLinkedImage,
        [Description("Linked Thumbnail as HTML")]
        HTMLLinkedImage,
        [Description("Linked Thumbnail for Wiki")]
        WikiLinkedImage,
        [Description("Thumbnail")]
        ThumbnailURL,
        [Description("Local File path")]
        LocalFilePath,
        [Description("Local File path as URI")]
        LocalFilePathUri
    }

    public enum CustomUploaderType
    {
        Image,
        Text,
        File,
        URL
    }

    public enum CustomUploaderRequestType
    {
        POST,
        GET,
        PUT,
        PATCH,
        DELETE
    }

    public enum CustomUploaderResponseParseType
    {
        Regex,
        Json,
        Xml
    }

    public enum FTPSEncryption
    {
        /// <summary>
        /// Connection starts in plain text and encryption is enabled with the AUTH command immediately after the server greeting.
        /// </summary>
        Explicit,
        /// <summary>
        /// Encryption is used from the start of the connection, port 990
        /// </summary>
        Implicit
    }

    public enum OAuthLoginStatus
    {
        LoginRequired,
        LoginSuccessful,
        LoginFailed
    }

    public enum URLType
    {
        URL,
        ThumbnailURL,
        DeletionURL
    }
}