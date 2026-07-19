#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
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
