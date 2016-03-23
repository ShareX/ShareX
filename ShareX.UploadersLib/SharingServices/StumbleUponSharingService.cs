namespace ShareX.UploadersLib.SharingServices
{
    public class StumbleUponSharingService : SimpleSharingService
    {
        public override URLSharingServices EnumValue { get; } = URLSharingServices.StumbleUpon;
        protected override string UrlFormatString { get; } = "https://delicious.com/save?v=5&url={0}";
    }
}