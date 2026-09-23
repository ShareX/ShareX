#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

namespace ShareX.UploadersLib;

public static class UploaderOAuthClientFactory
{
    public static OAuth2Info CreateImgur() => new(APIKeys.ImgurClientID, APIKeys.ImgurClientSecret);
    public static OAuth2Info CreateGitHub() => new(APIKeys.GitHubID, APIKeys.GitHubSecret);
    public static OAuth2Info CreateDropbox() => new(APIKeys.DropboxConsumerKey, APIKeys.DropboxConsumerSecret);
    public static OAuth2Info CreateOneDrive() => new(APIKeys.OneDriveClientID, APIKeys.OneDriveClientSecret)
    {
        Proof = new OAuth2ProofKey(OAuth2ChallengeMethod.SHA256)
    };
    public static OAuth2Info CreateBox() => new(APIKeys.BoxClientID, APIKeys.BoxClientSecret);
    public static OAuth2Info CreateBitly() => new(APIKeys.BitlyClientID, APIKeys.BitlyClientSecret);
    public static OAuth2Info CreateGoogle() => new(APIKeys.GoogleClientID, APIKeys.GoogleClientSecret);
    public static OAuthInfo CreateFlickr() => new(APIKeys.FlickrKey, APIKeys.FlickrSecret);
    public static OAuthInfo CreatePhotobucket() => new(APIKeys.PhotobucketConsumerKey, APIKeys.PhotobucketConsumerSecret);
}
