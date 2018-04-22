## Requirements

* [Visual Studio 2017](https://www.visualstudio.com/downloads/)

## Code formatting

We are using [CodeMaid](http://www.codemaid.net/) for code formatting. You can import our project settings from [here](https://github.com/ShareX/ShareX/blob/master/CodeMaid.config). When you press ``Ctrl + S`` to save the current file, it will automatically format the code before saving.

**Tab spacing:** 4 spaces

**New line:** CR + LF (Windows)

## How to create a new uploader

In the interest of keeping the size of ShareX setup small and in order to maintain performance of ShareX, utilization of external libraries is discouraged.

The best way to understand how to make a new uploader is by going through existing uploader classes and checking how these uploaders work. There are many example uploaders and you can probably copy & paste the majority of code from them when creating new uploader.

In the `ShareX.UploadersLib` project, create a new uploader class file in the folder according to uploader type (ImageUploaders, TextUploaders, FileUploaders, URLShorteners). Inherit uploader according to uploader type (ImageUploader, TextUploader, FileUploader, URLShortener).

```csharp
namespace ShareX.UploadersLib.ImageUploaders
{
    ...

    public sealed class Imgur : ImageUploader, IOAuth2
    {
        public override UploadResult Upload(Stream stream, string fileName)
        {
            ...

            UploadResult result = UploadData(stream, "https://api.imgur.com/3/image", fileName, "image", args, headers);

            ...
        }
    }
}
```

After uploader class is ready, add uploader name to `ShareX.UploadersLib/Enum.cs` (ImageDestination, TextDestination, FileDestination, UrlShortenerType)

```csharp
public enum ImageDestination
{
    ...
    [Description("Imgur")]
    Imgur,
    ...
}
```

You'll also need to create an uploader service class which will be used by ShareX to initiate the uploader class and check the config to enable/disable destination.

```csharp
namespace ShareX.UploadersLib.ImageUploaders
{
    public class ImgurImageUploaderService : ImageUploaderService
    {
        public override ImageDestination EnumValue { get; } = ImageDestination.Imgur;

        public override Icon ServiceIcon => Resources.Imgur;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.ImgurAccountType == AccountType.Anonymous || OAuth2Info.CheckOAuth(config.ImgurOAuth2Info);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            if (config.ImgurOAuth2Info == null)
            {
                config.ImgurOAuth2Info = new OAuth2Info(APIKeys.ImgurClientID, APIKeys.ImgurClientSecret);
            }

            return new Imgur(config.ImgurOAuth2Info)
            {
                UploadMethod = config.ImgurAccountType
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpImgur;
    }

    ...

    public sealed class Imgur : ImageUploader, IOAuth2
    {
        ...
    }
}
```

**EnumValue** must be assigned so ShareX can know which uploader this service belongs to.

[Optional] **ServiceIcon** or **ServiceImage** only required if you're going to have a tab in the uploaders config window. Add the icon/image file to the `ShareX.UploadersLib\Favicons` folder and add it to the `ShareX.UploadersLib` project resources.

**CheckConfig** function is used to check validation of config. For example, if account type is user and account info is empty then destination will be disabled in destinations menu.

**CreateUploader** function is used by the ShareX task system to initiate uploader class with config.

[Optional] **GetUploadersConfigTabPage** function only required if you're going to have a tab in the uploaders config window. It will be used when the user tries to upload with missing or invalid account info. Then ShareX can open uploaders config window with the current uploader tab selected.

Both the service class and the uploader class should be in same file (Example: `Imgur.cs`) for consistency with other uploaders.

You can store uploader settings in `ShareX.UploadersLib/UploadersConfig.cs`.

```csharp
public class UploadersConfig : SettingsBase<UploadersConfig>
{
    ...

    // Imgur

    public AccountType ImgurAccountType = AccountType.Anonymous;
    public OAuth2Info ImgurOAuth2Info = null;

    ...
}
```

If uploader settings need to be configurable by user, then in the `ShareX.UploadersLib/Forms/UploadersConfigForm.cs` form, create a new tab for your uploader.

```csharp
public void LoadSettings(UploadersConfig uploadersConfig)
{
    ...

    // Imgur

    oauth2Imgur.Enabled = Config.ImgurAccountType == AccountType.User;

    if (OAuth2Info.CheckOAuth(Config.ImgurOAuth2Info))
    {
        oauth2Imgur.Status = OAuthLoginStatus.LoginSuccessful;
        btnImgurRefreshAlbumList.Enabled = true;
    }

    atcImgurAccountType.SelectedAccountType = Config.ImgurAccountType;

    ...
}

...

private void atcImgurAccountType_AccountTypeChanged(AccountType accountType)
{
    Config.ImgurAccountType = accountType;
    oauth2Imgur.Enabled = Config.ImgurAccountType == AccountType.User;
}

private void oauth2Imgur_OpenButtonClicked()
{
    ImgurAuthOpen();
}

private void oauth2Imgur_CompleteButtonClicked(string code)
{
    ImgurAuthComplete(code);
}

private void oauth2Imgur_ClearButtonClicked()
{
    Config.ImgurOAuth2Info = null;
}

private void oauth2Imgur_RefreshButtonClicked()
{
    ImgurAuthRefresh();
}
```
