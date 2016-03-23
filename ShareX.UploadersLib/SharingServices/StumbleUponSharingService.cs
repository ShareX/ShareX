namespace ShareX.UploadersLib.SharingServices
{
    public class StumbleUponSharingService : SimpleSharingService
    {
        public override URLSharingServices EnumValue { get; } = URLSharingServices.StumbleUpon;
        protected override string UrlFormatString { get; } = "http://www.stumbleupon.com/submit?url={0}";
    }
}