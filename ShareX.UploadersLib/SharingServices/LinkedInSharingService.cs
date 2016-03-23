namespace ShareX.UploadersLib.SharingServices
{
    public class LinkedInSharingService : SimpleSharingService
    {
        public override URLSharingServices EnumValue { get; } = URLSharingServices.LinkedIn;
        protected override string UrlFormatString { get; } = "https://www.linkedin.com/shareArticle?url={0}";
    }
}