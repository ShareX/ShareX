using ShareX.HelpersLib;
using ShareX.UploadersLib.FileUploaders;

namespace ShareX.UploadersLib.SharingServices
{
    public class PushbulletSharingService : SharingService
    {
        public override URLSharingServices EnumValue { get; } = URLSharingServices.Pushbullet;

        public override bool CheckConfig(UploadersConfig uploadersConfig)
        {
            var pushbulletSettings = uploadersConfig.PushbulletSettings;

            return pushbulletSettings != null 
                   && !string.IsNullOrEmpty(pushbulletSettings.UserAPIKey) 
                   && pushbulletSettings.DeviceList != null
                   && pushbulletSettings.DeviceList.IsValidIndex(pushbulletSettings.SelectedDevice);
        }

        public override void ShareURL(string url, UploadersConfig uploadersConfig)
        {
            new Pushbullet(uploadersConfig.PushbulletSettings).PushLink(url, "ShareX: URL Share");
        }
    }
}