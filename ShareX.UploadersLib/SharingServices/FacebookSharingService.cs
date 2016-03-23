namespace ShareX.UploadersLib.SharingServices
{
    public class FacebookSharingService : SimpleSharingService
    {
        public override URLSharingServices EnumValue { get; } = URLSharingServices.Facebook;

        protected override string UrlFormatString { get; } = "https://www.facebook.com/sharer/sharer.php?u={0}";
    }
}