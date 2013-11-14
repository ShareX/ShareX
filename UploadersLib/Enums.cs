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

using System.ComponentModel;

namespace UploadersLib
{
    [Description("Image uploaders")]
    public enum ImageDestination
    {
        [Description("imgur.com")]
        Imgur,
        [Description("imageshack.us")]
        ImageShack,
        [Description("tinypic.com")]
        TinyPic,
        [Description("flickr.com")]
        Flickr,
        [Description("photobucket.com")]
        Photobucket,
        [Description("picasaweb.google.com")]
        Picasa,
        [Description("uploadscreenshot.com")]
        UploadScreenshot,
        [Description("twitpic.com")]
        Twitpic,
        [Description("twitsnaps.com")]
        Twitsnaps,
        [Description("yfrog.com")]
        yFrog,
        [Description("imm.io")]
        Immio,
        [Description("Custom image uploader")]
        CustomImageUploader,
        [Description("File uploader")]
        FileUploader
    }

    [Description("Text uploaders")]
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
        [Description("Custom text uploader")]
        CustomTextUploader,
        [Description("File uploader")]
        FileUploader
    }

    [Description("File uploaders")]
    public enum FileDestination
    {
        [Description("dropbox.com")]
        Dropbox,
        [Description("mega.co.nz")]
        Mega,
        [Description("FTP Server")]
        FTP,
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

    [Description("URL shorteners")]
    public enum UrlShortenerType
    {
        [Description("goo.gl")]
        Google,
        [Description("bit.ly")]
        BITLY,
        [Description("j.mp")]
        Jmp,
        [Description("is.gd")]
        ISGD,
        [Description("tinyurl.com")]
        TINYURL,
        [Description("turl.ca")]
        TURL,
        [Description("Custom URL shortener")]
        CustomURLShortener
    }

    [Description("Social networking services")]
    public enum SocialNetworkingService
    {
        [Description("twitter.com")]
        Twitter
    }

    public enum HttpMethod
    {
        [Description("GET")]
        Get,
        [Description("POST")]
        Post,
        [Description("DELETE")]
        Delete
    }

    public enum ResponseType
    {
        [Description("Response text")]
        Text,
        [Description("Redirection URL")]
        RedirectionURL
    }

    public enum ProxyMethod
    {
        None,
        Manual,
        Automatic
    }

    public enum ProxyType
    {
        [Description("HTTP Proxy")]
        HTTP,
        [Description("SOCKS v4 Proxy")]
        SOCKS4,
        [Description("SOCKS v4a Proxy")]
        SOCKS4a,
        [Description("SOCKS v5 Proxy")]
        SOCKS5
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
        Http,
        [Description("https://")]
        Https,
        [Description("ftp://")]
        Ftp,
        [Description("ftps://")]
        Ftps,
        [Description("file://")]
        File
    }

    public enum ServerProtocol
    {
        [Description("ftp://")]
        Ftp,
        [Description("ftps://")]
        Ftps
    }

    public enum LinkType
    {
        URL,
        ThumbnailURL,
        DeletionLink,
        FULLIMAGE_TINYURL
    }

    public enum URLType
    {
        URL,
        ThumbnailURL,
        DeletionURL
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

    public enum OutputEnum
    {
        [Description("Clipboard")]
        Clipboard,
        [Description("File")]
        LocalDisk,
        [Description("Upload")]
        RemoteHost,
        [Description("E-mail")]
        Email,
        [Description("Printer")]
        Printer,
        [Description("Shared folder")]
        SharedFolder
    }

    public enum ClipboardContentEnum
    {
        [Description("Image or Text")]
        Data,
        [Description("Local file path")]
        Local,
        [Description("Uploaded URL")]
        Remote,
        [Description("Text using OCR")]
        OCR
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

    public enum FtpSecurityProtocol
    {
        None = 0,
        Tls1Explicit = 1,
        Tls1OrSsl3Explicit = 2,
        Ssl3Explicit = 3,
        Ssl2Explicit = 4,
        Tls1Implicit = 5,
        Tls1OrSsl3Implicit = 6,
        Ssl3Implicit = 7,
        Ssl2Implicit = 8,
    }
}