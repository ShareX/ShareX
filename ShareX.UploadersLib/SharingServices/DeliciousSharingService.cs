namespace ShareX.UploadersLib.SharingServices
{
    public class DeliciousSharingService : SimpleSharingService
    {
        public override URLSharingServices EnumValue { get; } = URLSharingServices.Delicious;
        protected override string UrlFormatString { get; } = "https://delicious.com/save?v=5&url={0}";
    }
}