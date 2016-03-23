namespace ShareX.UploadersLib.SharingServices
{
    public class RedditSharingService : SimpleSharingService
    {
        public override URLSharingServices EnumValue { get; } = URLSharingServices.Reddit;
        protected override string UrlFormatString { get; } = "http://www.reddit.com/submit?url={0}";
    }
}