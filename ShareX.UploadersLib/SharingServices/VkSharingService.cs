namespace ShareX.UploadersLib.SharingServices
{
    public class VkSharingService : SimpleSharingService
    {
        public override URLSharingServices EnumValue { get; } = URLSharingServices.VK;
        protected override string UrlFormatString { get; } = "http://vk.com/share.php?url={0}";
    }
}