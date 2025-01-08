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
        [Description("Flickr")]
        Flickr,
        [Description("Photobucket")]
        Photobucket,
        [Description("X")]
        Twitter,
        [Description("Chevereto")]
        Chevereto,
        [Description("vgy.me")]
        Vgyme,
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
        [Description("Paste.ee")]
        Paste_ee,
        [Description("GitHub Gist")]
        Gist,
        [Description("uPaste")]
        Upaste,
        [Description("Hastebin")]
        Hastebin,
        [Description("OneTimeSecret")]
        OneTimeSecret,
        [Description("Pastie")]
        Pastie,
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
        [Description("puush")]
        Puush,
        [Description("Box")]
        Box,
        [Description("MEGA")]
        Mega,
        [Description("Amazon S3")]
        AmazonS3,
        [Description("Google Cloud Storage")]
        GoogleCloudStorage,
        [Description("Azure Storage")]
        AzureStorage,
        [Description("Backblaze B2")]
        BackblazeB2,
        [Description("ownCloud / Nextcloud")]
        OwnCloud,
        [Description("MediaFire")]
        MediaFire,
        [Description("Pushbullet")]
        Pushbullet,
        [Description("SendSpace")]
        SendSpace,
        [Description("Hostr")]
        Localhostr,
        [Description("JIRA")]
        Jira,
        [Description("Lambda")]
        Lambda,
        [Description("Pomf")]
        Pomf,
        [Description("Uguu")]
        Uguu,
        [Description("Seafile")]
        Seafile,
        [Description("Streamable")]
        Streamable,
        [Description("s-ul")]
        Sul,
        [Description("LobFile")]
        Lithiio,
        [Description("transfer.sh")]
        Transfersh,
        [Description("Plik")]
        Plik,
        [Description("YouTube")]
        YouTube,
        [Description("Vault.ooo")]
        Vault_ooo,
        SharedFolder, // Localized
        Email, // Localized
        CustomFileUploader // Localized
    }

    [Description("URL shorteners"), DefaultValue(BITLY)]
    public enum UrlShortenerType
    {
        [Description("bit.ly")]
        BITLY,
        [Description("is.gd")]
        ISGD,
        [Description("v.gd")]
        VGD,
        [Description("tinyurl.com")]
        TINYURL,
        [Description("turl.ca")]
        TURL,
        [Description("yourls.org")]
        YOURLS,
        [Description("qr.net")]
        QRnet,
        [Description("vurl.com")]
        VURL,
        [Description("2.gp")]
        TwoGP,
        [Description("Polr")]
        Polr,
        [Description("Firebase Dynamic Links")]
        FirebaseDynamicLinks,
        [Description("Kutt")]
        Kutt,
        [Description("Zero Width Shortener")]
        ZeroWidthShortener,
        CustomURLShortener // Localized
    }

    [Description("URL sharing services"), DefaultValue(Twitter)]
    public enum URLSharingServices
    {
        Email, // Localized
        [Description("X")]
        Twitter,
        [Description("Facebook")]
        Facebook,
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
        Pushbullet,
        GoogleImageSearch, // Localized
        BingVisualSearch, // Localized
        CustomURLSharingService // Localized
    }

    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        PATCH,
        DELETE
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

    public enum CustomUploaderBody
    {
        [Description("No body")]
        None,
        [Description("Form data (multipart/form-data)")]
        MultipartFormData,
        [Description("Form URL encoded (application/x-www-form-urlencoded)")]
        FormURLEncoded,
        [Description("JSON (application/json)")]
        JSON,
        [Description("XML (application/xml)")]
        XML,
        [Description("Binary")]
        Binary
    }

    [Flags]
    public enum CustomUploaderDestinationType
    {
        [Description("None")]
        None = 0,
        ImageUploader = 1, // Localized
        TextUploader = 1 << 1, // Localized
        FileUploader = 1 << 2, // Localized
        URLShortener = 1 << 3, // Localized
        URLSharingService = 1 << 4 // Localized
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

    public enum YouTubeVideoPrivacy // Localized
    {
        Public,
        Unlisted,
        Private
    }

    public enum BoxShareAccessLevel
    {
        [Description("Public - People with the link")]
        Open,
        [Description("Company - People in your company")]
        Company,
        [Description("Collaborators - Invited people only")]
        Collaborators
    }
}