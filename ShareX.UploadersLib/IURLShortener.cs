namespace ShareX.UploadersLib
{
    public interface IURLShortener : IUploader
    {
        UploadResult ShortenURL(string url);
    }
}