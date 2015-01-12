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

using System.ComponentModel;

namespace ShareX.UploadersLib
{
    [Description("Image uploaders"), DefaultValue(Imgur)]
    public enum ImageDestination
    {
        [Description("Imgur")]
        Imgur,
        [Description("ImageShack")]
        ImageShack,
        [Description("TinyPic")]
        TinyPic,
        [Description("Flickr")]
        Flickr,
        [Description("Photobucket")]
        Photobucket,
        [Description("Picasa")]
        Picasa,
        [Description("Twitter")]
        Twitter,
        [Description("Chevereto")]
        Chevereto,
        [Description("Hızlı Resim")]
        HizliResim,
        CustomImageUploader, // Localized
        FileUploader // Localized
    }

    [Description("Text uploaders"), DefaultValue(Pastebin)]
    public enum TextDestination
    {
        [Description("Pastebin")]
        Pastebin,
        [Description("Paste2")]
        Paste2,
        [Description("Slexy")]
        Slexy,
        [Description("Pastee.org")]
        Pastee,
        [Description("Paste.ee")]
        Paste_ee,
        [Description("GitHub Gist")]
        Gist,
        [Description("uPaste")]
        Upaste,
        [Description("Hastebin")]
        Hastebin,
        CustomTextUploader, // Localized
        FileUploader // Localized
    }

    [Description("File uploaders"), DefaultValue(Dropbox)]
    public enum FileDestination
    {
        [Description("Dropbox")]
        Dropbox,
        [Description("FTP")]
        FTP,
        [Description("OneDrive")]
        OneDrive,
        [Description("Google Drive")]
        GoogleDrive,
        [Description("Copy")]
        Copy,
        [Description("Hubic")]
        Hubic,
        [Description("Box")]
        Box,
        [Description("MEGA")]
        Mega,
        [Description("Amazon S3")]
        AmazonS3,
        [Description("ownCloud")]
        OwnCloud,
        [Description("MediaFire")]
        MediaFire,
        [Description("Pomf")]
        Pomf,
        [Description("Gfycat")]
        Gfycat,
        [Description("Pushbullet")]
        Pushbullet,
        [Description("MediaCrush")]
        MediaCrush,
        [Description("RapidShare")]
        RapidShare,
        [Description("SendSpace")]
        SendSpace,
        [Description("Minus")]
        Minus,
        [Description("Ge.tt")]
        Ge_tt,
        [Description("Hostr")]
        Localhostr,
        [Description("JIRA")]
        Jira,
        SharedFolder, // Localized
        Email, // Localized
        CustomFileUploader // Localized
    }

    [Description("URL shorteners"), DefaultValue(BITLY)]
    public enum UrlShortenerType
    {
        [Description("bit.ly")]
        BITLY,
        [Description("goo.gl")]
        Google,
        [Description("is.gd")]
        ISGD,
        [Description("tinyurl.com")]
        TINYURL,
        [Description("turl.ca")]
        TURL,
        [Description("yourls.org")]
        YOURLS,
        [Description("nl.cm")]
        NLCM,
        [Description("adf.ly")]
        AdFly,
        CustomURLShortener // Localized
    }

    [Description("URL sharing services"), DefaultValue(Twitter)]
    public enum URLSharingServices
    {
        Email, // Localized
        [Description("Twitter")]
        Twitter,
        [Description("Facebook")]
        Facebook,
        [Description("Google+")]
        GooglePlus,
        [Description("Reddit")]
        Reddit,
        [Description("Pinterest")]
        Pinterest,
        [Description("Tumblr")]
        Tumblr,
        [Description("LinkedIn")]
        LinkedIn,
        [Description("StumbleUpon")]
        StumbleUpon,
        [Description("Delicious")]
        Delicious,
        [Description("VK")]
        VK,
        [Description("Pushbullet")]
        Pushbullet
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
        Headers
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

    public enum ServerProtocol
    {
        [Description("ftp://")]
        ftp,
        [Description("ftps://")]
        ftps
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
}