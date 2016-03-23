namespace ShareX.UploadersLib.SharingServices
{
    public class TumblrSharingService : SimpleSharingService
    {
        public override URLSharingServices EnumValue { get; } = URLSharingServices.Tumblr;
        protected override string UrlFormatString { get; } = "https://www.tumblr.com/share?v=3&u={0}";
    }
}