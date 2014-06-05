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

using System.ComponentModel;

namespace UploadersLib
{
    [Description("Image uploaders"), DefaultValue(ImageDestination.Imgur)]
    public enum ImageDestination
    {
        [Description("imgur.com")]
        Imgur,
        [Description("imageshack.com")]
        ImageShack,
        [Description("tinypic.com")]
        TinyPic,
        [Description("flickr.com")]
        Flickr,
        [Description("photobucket.com")]
        Photobucket,
        [Description("picasaweb.google.com")]
        Picasa,
        [Description("twitpic.com")]
        Twitpic,
        [Description("twitsnaps.com")]
        Twitsnaps,
        [Description("yfrog.com")]
        yFrog,
        [Description("Custom image uploader")]
        CustomImageUploader,
        [Description("File uploader")]
        FileUploader
    }

    [Description("Text uploaders"), DefaultValue(TextDestination.Pastebin)]
    public enum TextDestination
    {
        [Description("pastebin.com")]
        Pastebin,
        [Description("pastebin.ca")]
        PastebinCA,
        [Description("paste2.org")]
        Paste2,
        [Description("slexy.org")]
        Slexy,
        [Description("pastee.org")]
        Pastee,
        [Description("paste.ee")]
        Paste_ee,
        [Description("gist.github.com")]
        Gist,
        [Description("upaste.me")]
        Upaste,
        [Description("Custom text uploader")]
        CustomTextUploader,
        [Description("File uploader")]
        FileUploader
    }

    [Description("File uploaders"), DefaultValue(FileDestination.Dropbox)]
    public enum FileDestination
    {
        [Description("dropbox.com")]
        Dropbox,
        [Description("copy.com")]
        Copy,
        [Description("FTP Server")]
        FTP,
        [Description("gfycat.com")]
        Gfycat,
        [Description("mega.co.nz")]
        Mega,
        [Description("s3.amazon.com")]
        AmazonS3,
        [Description("pushbullet.com")]
        Pushbullet,
        [Description("mediacru.sh")]
        MediaCrush,
        [Description("drive.google.com")]
        GoogleDrive,
        [Description("rapidshare.com")]
        RapidShare,
        [Description("sendspace.com")]
        SendSpace,
        [Description("minus.com")]
        Minus,
        [Description("box.com")]
        Box,
        [Description("ge.tt")]
        Ge_tt,
        [Description("hostr.co")]
        Localhostr,
        [Description("Shared folder")]
        SharedFolder,
        [Description("Email")]
        Email,
        [Description("Jira")]
        Jira,
        [Description("Custom file uploader")]
        CustomFileUploader
    }

    [Description("URL shorteners"), DefaultValue(UrlShortenerType.BITLY)]
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
        [Description("Custom URL shortener")]
        CustomURLShortener
    }

    [Description("Social networking services"), DefaultValue(SocialNetworkingService.Twitter)]
    public enum SocialNetworkingService
    {
        [Description("twitter.com")]
        Twitter
    }

    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public enum ResponseType
    {
        [Description("Response text")]
        Text,
        [Description("Redirection URL")]
        RedirectionURL,
        [Description("Response headers")]
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
        GET
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
}