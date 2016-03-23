namespace ShareX.UploadersLib.SharingServices
{
    public class GooglePlusSharingService : SimpleSharingService
    {
        public override URLSharingServices EnumValue { get; } = URLSharingServices.GooglePlus;
        
        protected override string UrlFormatString { get; } = "https://plus.google.com/share?url={0}";
    }
}