namespace ShareX.UploadersLib
{
    public interface IURLShortener
    {
        UploadResult ShortenURL(string url);
    }
}