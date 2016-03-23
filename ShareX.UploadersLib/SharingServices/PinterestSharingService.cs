namespace ShareX.UploadersLib.SharingServices
{
    public class PinterestSharingService : SimpleSharingService
    {
        public override URLSharingServices EnumValue { get; } = URLSharingServices.Pinterest;
        protected override string UrlFormatString { get; } = "http://pinterest.com/pin/create/button/?url={0}&media={0}";
    }
}