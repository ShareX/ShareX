using ShareX.HelpersLib;
using ShareX.UploadersLib;
using ShareX.UploadersLib.HelperClasses;
using ShareX.UploadersLib.ImageUploaders;
using System.IO;

namespace ShareX
{
    public class UploadImageWorkerTask : WorkerTask
    {
        public UploadImageWorkerTask(TaskSettings taskSettings) : base(taskSettings)
        {
        }

        public override UploadResult PerformTask(Stream stream, string fileName)
        {
            ImageUploader imageUploader = null;

            switch (Info.TaskSettings.ImageDestination)
            {
                case ImageDestination.Imgur:
                    if (Program.UploadersConfig.ImgurOAuth2Info == null)
                    {
                        Program.UploadersConfig.ImgurOAuth2Info = new OAuth2Info(APIKeys.ImgurClientID, APIKeys.ImgurClientSecret);
                    }

                    string albumID = null;

                    if (Program.UploadersConfig.ImgurUploadSelectedAlbum && Program.UploadersConfig.ImgurSelectedAlbum != null)
                    {
                        albumID = Program.UploadersConfig.ImgurSelectedAlbum.id;
                    }

                    imageUploader = new Imgur(Program.UploadersConfig.ImgurOAuth2Info)
                    {
                        UploadMethod = Program.UploadersConfig.ImgurAccountType,
                        DirectLink = Program.UploadersConfig.ImgurDirectLink,
                        ThumbnailType = Program.UploadersConfig.ImgurThumbnailType,
                        UseGIFV = Program.UploadersConfig.ImgurUseGIFV,
                        UploadAlbumID = albumID
                    };
                    break;
                case ImageDestination.ImageShack:
                    Program.UploadersConfig.ImageShackSettings.ThumbnailWidth = Info.TaskSettings.AdvancedSettings.ThumbnailPreferredWidth;
                    Program.UploadersConfig.ImageShackSettings.ThumbnailHeight = Info.TaskSettings.AdvancedSettings.ThumbnailPreferredHeight;
                    imageUploader = new ImageShackUploader(APIKeys.ImageShackKey, Program.UploadersConfig.ImageShackSettings);
                    break;
                case ImageDestination.TinyPic:
                    imageUploader = new TinyPicUploader(APIKeys.TinyPicID, APIKeys.TinyPicKey, Program.UploadersConfig.TinyPicAccountType, Program.UploadersConfig.TinyPicRegistrationCode);
                    break;
                case ImageDestination.Flickr:
                    imageUploader = new FlickrUploader(APIKeys.FlickrKey, APIKeys.FlickrSecret, Program.UploadersConfig.FlickrAuthInfo, Program.UploadersConfig.FlickrSettings);
                    break;
                case ImageDestination.Photobucket:
                    imageUploader = new Photobucket(Program.UploadersConfig.PhotobucketOAuthInfo, Program.UploadersConfig.PhotobucketAccountInfo);
                    break;
                case ImageDestination.Picasa:
                    imageUploader = new Picasa(Program.UploadersConfig.PicasaOAuth2Info)
                    {
                        AlbumID = Program.UploadersConfig.PicasaAlbumID
                    };
                    break;
                case ImageDestination.Twitter:
                    OAuthInfo twitterOAuth = Program.UploadersConfig.TwitterOAuthInfoList.ReturnIfValidIndex(Program.UploadersConfig.TwitterSelectedAccount);
                    imageUploader = new Twitter(twitterOAuth)
                    {
                        SkipMessageBox = Program.UploadersConfig.TwitterSkipMessageBox,
                        DefaultMessage = Program.UploadersConfig.TwitterDefaultMessage ?? string.Empty
                    };
                    break;
                case ImageDestination.Chevereto:
                    imageUploader = new Chevereto(Program.UploadersConfig.CheveretoWebsite, Program.UploadersConfig.CheveretoAPIKey)
                    {
                        DirectURL = Program.UploadersConfig.CheveretoDirectURL
                    };
                    break;
                case ImageDestination.Vgyme:
                    imageUploader = new VgymeUploader();
                    break;
                case ImageDestination.SomeImage:
                    imageUploader = new SomeImage(APIKeys.SomeImageKey);
                    break;
                case ImageDestination.CustomImageUploader:
                    CustomUploaderItem customUploader = GetCustomUploader(Program.UploadersConfig.CustomImageUploaderSelected);
                    if (customUploader != null)
                    {
                        imageUploader = new CustomImageUploader(customUploader);
                    }
                    break;
            }

            if (imageUploader != null)
            {
                PrepareUploader(imageUploader);
                return imageUploader.Upload(stream, fileName);
            }

            return null;
        }
    }
}