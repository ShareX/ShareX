#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using ShareX.HelpersLib;
using ShareX.UploadersLib.FileUploaders;
using ShareX.UploadersLib.ImageUploaders;
using ShareX.UploadersLib.TextUploaders;
using ShareX.UploadersLib.URLShorteners;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ShareX.UploadersLib;

internal sealed class DestinationSettingsAccounts
{
    private readonly UploadersConfig _config;

    public DestinationSettingsAccounts(UploadersConfig config) => _config = config;

    public Control? Build(string pageId) => pageId switch
    {
        "imgur" => BasicOAuth2(
            () => _config.ImgurOAuth2Info,
            value => _config.ImgurOAuth2Info = value,
            info => new Imgur(info),
            UploaderOAuthClientFactory.CreateImgur),
        "gist" => BasicOAuth2(() => _config.GistOAuth2Info, value => _config.GistOAuth2Info = value,
            info => new GitHubGist(info), UploaderOAuthClientFactory.CreateGitHub),
        "dropbox" => BasicOAuth2(() => _config.DropboxOAuth2Info, value => _config.DropboxOAuth2Info = value,
            info => new Dropbox(info), UploaderOAuthClientFactory.CreateDropbox),
        "onedrive" => BasicOAuth2(() => _config.OneDriveV2OAuth2Info, value => _config.OneDriveV2OAuth2Info = value,
            info => new OneDrive(info), CreateOneDriveInfo),
        "box" => BasicOAuth2(() => _config.BoxOAuth2Info, value => _config.BoxOAuth2Info = value,
            info => new Box(info), UploaderOAuthClientFactory.CreateBox),
        "mega" => MegaAccount(),
        "bitly" => BasicOAuth2(() => _config.BitlyOAuth2Info, value => _config.BitlyOAuth2Info = value,
            info => new BitlyURLShortener(info), UploaderOAuthClientFactory.CreateBitly),
        "google-drive" => LoopbackOAuth(
            () => _config.GoogleDriveOAuth2Info,
            () => _config.GoogleDriveUserInfo,
            (info, user) => { _config.GoogleDriveOAuth2Info = info; _config.GoogleDriveUserInfo = user; },
            info => new GoogleDrive(info).OAuth2),
        "youtube" => LoopbackOAuth(
            () => _config.YouTubeOAuth2Info,
            () => _config.YouTubeUserInfo,
            (info, user) => { _config.YouTubeOAuth2Info = info; _config.YouTubeUserInfo = user; },
            info => new YouTube(info).OAuth2),
        "google-cloud-storage" => LoopbackOAuth(
            () => _config.GoogleCloudStorageOAuth2Info,
            () => _config.GoogleCloudStorageUserInfo,
            (info, user) => { _config.GoogleCloudStorageOAuth2Info = info; _config.GoogleCloudStorageUserInfo = user; },
            info => new GoogleCloudStorage(info).OAuth2),
        "flickr" => FlickrOAuth(),
        "photobucket" => PhotobucketOAuth(),
        "imageshack" => ImageShackAccount(),
        "pastebin" => PastebinAccount(),
        "paste-ee" => DestinationSettingsPageBuilder.Card(Localization.Strings.DestinationSettings_Account,
            DestinationSettingsPageBuilder.ButtonRow(DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Get_user_key, () =>
                URLHelpers.OpenURL(UploaderConfigurationClient.GetPasteEeAuthorizationURL())))),
        "puush" => UserPasswordLogin("puush", () => _config.PuushAPIKey,
            (username, password) => new Puush().LoginAsync(username, password),
            value => _config.PuushAPIKey = value),
        "lobfile" => UserPasswordLogin("LobFile", () => _config.LithiioSettings.UserAPIKey,
            (username, password) => new LobFile().FetchAPIKeyAsync(username, password),
            value => _config.LithiioSettings.UserAPIKey = value),
        "pushbullet" => PushbulletAccount(),
        _ => null
    };

    private Control MegaAccount()
    {
        bool IsConnected() => !string.IsNullOrWhiteSpace(_config.MegaSessionID) &&
            !string.IsNullOrWhiteSpace(_config.MegaMasterKey);

        TextBlock status = Status(IsConnected()
            ? Localization.Strings.DestinationSettings_Connected
            : Localization.Strings.DestinationSettings_Not_connected, IsConnected());

        void Disconnect()
        {
            _config.MegaSessionID = string.Empty;
            _config.MegaMasterKey = string.Empty;
            _config.MegaSelectedFolder = Mega.RootFolder;
            SetStatus(status, Localization.Strings.DestinationSettings_Not_connected, false);
        }

        TextBox email = DestinationSettingsPageBuilder.Text(() => _config.MegaEmail, value =>
        {
            if (!string.Equals(_config.MegaEmail, value, StringComparison.Ordinal)) Disconnect();
            _config.MegaEmail = value;
        });
        TextBox password = DestinationSettingsPageBuilder.Text(() => _config.MegaPassword, value =>
        {
            if (!string.Equals(_config.MegaPassword, value, StringComparison.Ordinal)) Disconnect();
            _config.MegaPassword = value;
        });
        password.PasswordChar = '●';

        Button login = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Log_in, async () =>
        {
            try
            {
                Mega mega = new(email.Text ?? string.Empty, password.Text ?? string.Empty);
                MegaSessionInfo session;

                try
                {
                    session = await mega.LoginAsync();
                }
                catch (MegaApiException exception) when (exception.ErrorCode == -26)
                {
                    string? code = InputBoxWindowIntegration.Show(
                        Localization.Strings.DestinationSettings_Enter_current_two_factor_authentication_code);
                    if (code == null) return;
                    session = await mega.LoginAsync(code);
                }

                ((DestinationValue<string>)password.DataContext!).Value = string.Empty;
                _config.MegaSessionID = session.SessionID;
                _config.MegaMasterKey = session.MasterKey;
                SetStatus(status, Localization.Strings.DestinationSettings_Connected, true);
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                SetStatus(status, exception.Message, false);
            }
        });
        Button disconnect = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Disconnect, Disconnect);

        return DestinationSettingsPageBuilder.Card(Localization.Strings.DestinationSettings_Account,
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_Field_Email, email),
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_Field_Password, password),
            DestinationSettingsPageBuilder.ButtonRow(login, disconnect),
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_Status, status));
    }

    private Control ImageShackAccount()
    {
        bool isConnected = !string.IsNullOrEmpty(_config.ImageShackSettings.Auth_token);
        TextBlock status = Status(
            !isConnected
                ? Localization.Strings.DestinationSettings_Not_connected
                : Localization.Strings.DestinationSettings_Connected, isConnected);
        Button login = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Log_in, async () =>
        {
            try
            {
                bool result = await UploaderConfigurationClient.LoginImageShackAsync(_config.ImageShackSettings);
                SetStatus(status, result
                    ? Localization.Strings.DestinationSettings_Connected
                    : Localization.Strings.DestinationSettings_Login_failed, result);
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                SetStatus(status, exception.Message, false);
            }
        });
        Button profile = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Public_profile, () =>
        {
            if (!string.IsNullOrWhiteSpace(_config.ImageShackSettings.Username))
                URLHelpers.OpenURL("https://imageshack.com/user/" + _config.ImageShackSettings.Username);
        });
        Button images = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_My_images,
            () => URLHelpers.OpenURL("https://imageshack.com/my/images"));
        return DestinationSettingsPageBuilder.Card(Localization.Strings.DestinationSettings_Account,
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_Status, status),
            DestinationSettingsPageBuilder.ButtonRow(login, profile, images));
    }

    private Control PastebinAccount()
    {
        bool isConnected = !string.IsNullOrEmpty(_config.PastebinSettings.UserKey);
        TextBlock status = Status(
            !isConnected
                ? Localization.Strings.DestinationSettings_Not_connected
                : Localization.Strings.DestinationSettings_Connected, isConnected);
        Button login = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Log_in, async () =>
        {
            try
            {
                bool result = await UploaderConfigurationClient.LoginPastebinAsync(_config.PastebinSettings);
                SetStatus(status, result
                    ? Localization.Strings.DestinationSettings_Connected
                    : Localization.Strings.DestinationSettings_Login_failed, result);
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                SetStatus(status, exception.Message, false);
            }
        });
        Button register = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Register,
            () => URLHelpers.OpenURL("https://pastebin.com/signup"));
        return DestinationSettingsPageBuilder.Card(Localization.Strings.DestinationSettings_Account,
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_Status, status),
            DestinationSettingsPageBuilder.ButtonRow(login, register));
    }

    private Control UserPasswordLogin(string serviceName, Func<string> getKey, Func<string, string, Task<string?>> login, Action<string> saveKey)
    {
        TextBox username = DestinationSettingsPageBuilder.Text(() => string.Empty, _ => { });
        TextBox password = DestinationSettingsPageBuilder.Text(() => string.Empty, _ => { });
        password.PasswordChar = '●';
        TextBox apiKey = DestinationSettingsPageBuilder.Text(getKey, saveKey);
        apiKey.PasswordChar = '●';
        TextBlock status = Status(Localization.Strings.DestinationSettings_Enter_credentials_to_retrieve_API_key);
        Button connect = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Log_in, async () =>
        {
            try
            {
                string? key = await login(username.Text ?? string.Empty, password.Text ?? string.Empty);
                if (!string.IsNullOrWhiteSpace(key))
                {
                    ((DestinationValue<string>)apiKey.DataContext!).Value = key;
                    SetStatus(status, Localization.Strings.DestinationSettings_API_key_retrieved, true);
                    password.Text = string.Empty;
                }
                else
                {
                    SetStatus(status, Localization.Strings.DestinationSettings_Login_failed, false);
                }
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                SetStatus(status, exception.Message, false);
            }
        });
        return DestinationSettingsPageBuilder.Card(string.Format(Localization.Strings.DestinationSettings_Service_account, serviceName),
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_Email_or_username, username),
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_Password, password),
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_API_key, apiKey),
            DestinationSettingsPageBuilder.ButtonRow(connect),
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_Status, status));
    }

    private Control PushbulletAccount()
    {
        ObservableCollection<DestinationChoice> devices = new();
        TextBox apiKey = DestinationSettingsPageBuilder.Text(() => _config.PushbulletSettings.UserAPIKey,
            value => _config.PushbulletSettings.UserAPIKey = value);
        apiKey.PasswordChar = '●';
        ComboBox device = new() { ItemsSource = devices };
        device.Classes.Add("form-control");
        TextBlock status = DestinationSettingsPageBuilder.Hint(string.Empty);

        void Reload()
        {
            int previousIndex = _config.PushbulletSettings.SelectedDevice;
            devices.Clear();
            foreach (PushbulletDevice item in _config.PushbulletSettings.DeviceList ?? [])
            {
                devices.Add(new DestinationChoice(item, item.Name ?? Localization.Strings.DestinationSettings_Unnamed_device));
            }

            int index = devices.Count == 0 ? -1 : previousIndex.BetweenOrDefault(0, devices.Count - 1);
            device.SelectedIndex = index;
            _config.PushbulletSettings.SelectedDevice = index;
            status.Text = devices.Count == 0
                ? Localization.Strings.DestinationSettings_No_devices_loaded
                : string.Format(devices.Count == 1
                    ? Localization.Strings.DestinationSettings_Device_count_singular
                    : Localization.Strings.DestinationSettings_Device_count_plural, devices.Count);
        }

        device.SelectionChanged += (_, _) => _config.PushbulletSettings.SelectedDevice = device.SelectedIndex;
        Button refresh = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Refresh_devices, async () =>
        {
            try
            {
                _config.PushbulletSettings.DeviceList = await new Pushbullet(_config.PushbulletSettings).GetDeviceListAsync() ?? [];
                Reload();
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                status.Text = exception.Message;
            }
        });
        Reload();
        return DestinationSettingsPageBuilder.Card(Localization.Strings.DestinationSettings_Account_and_devices,
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_API_key, apiKey),
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_Device, device),
            DestinationSettingsPageBuilder.ButtonRow(refresh),
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_Status, status));
    }

    private Control BasicOAuth2(
        Func<OAuth2Info?> getInfo,
        Action<OAuth2Info?> setInfo,
        Func<OAuth2Info, IOAuth2Basic> createUploader,
        Func<OAuth2Info> createInfo,
        params Control[] extras)
    {
        TextBlock status = DestinationSettingsPageBuilder.Hint(string.Empty);
        TextBox code = DestinationSettingsPageBuilder.Text(() => string.Empty, _ => { });

        void UpdateStatus(string? message = null, bool? connected = null)
        {
            if (message == null)
            {
                bool isConnected = OAuth2Info.CheckOAuth(getInfo());
                SetStatus(status, isConnected
                    ? Localization.Strings.DestinationSettings_Connected
                    : Localization.Strings.DestinationSettings_Not_connected, isConnected);
            }
            else
            {
                SetStatus(status, message, connected);
            }
        }

        Button open = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Open_authorization_page, async () =>
        {
            try
            {
                OAuth2Info info = createInfo();
                IOAuth2Basic uploader = createUploader(info);
                string url = await uploader.GetAuthorizationURLAsync();
                setInfo(string.IsNullOrEmpty(url) ? null : uploader.AuthInfo);
                if (!string.IsNullOrEmpty(url)) URLHelpers.OpenURL(url);
                UpdateStatus(Localization.Strings.DestinationSettings_Authorization_page_opened);
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                UpdateStatus(exception.Message, false);
            }
        });
        Button complete = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Complete_authorization, async () =>
        {
            try
            {
                OAuth2Info? info = getInfo();
                string authorizationCode = code.Text ?? string.Empty;
                bool result = info != null && !string.IsNullOrWhiteSpace(authorizationCode) &&
                    await createUploader(info).GetAccessTokenAsync(authorizationCode);
                UpdateStatus(result
                    ? Localization.Strings.DestinationSettings_Connected
                    : Localization.Strings.DestinationSettings_Authorization_failed, result);
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                UpdateStatus(exception.Message, false);
            }
        });
        Button refresh = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Refresh_token, async () =>
        {
            try
            {
                bool result = getInfo() is { } info && createUploader(info) is IOAuth2 oauth && await oauth.RefreshAccessTokenAsync();
                UpdateStatus(result
                    ? Localization.Strings.DestinationSettings_Connected
                    : Localization.Strings.DestinationSettings_Token_refresh_failed, result);
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                UpdateStatus(exception.Message, false);
            }
        });
        Button clear = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Disconnect,
            () => { setInfo(null); UpdateStatus(); });

        List<Control> controls =
        [
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_Status, status),
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_Authorization_code, code),
            DestinationSettingsPageBuilder.ButtonRow(open, complete, refresh, clear)
        ];
        controls.AddRange(extras);
        UpdateStatus();
        return DestinationSettingsPageBuilder.Card(Localization.Strings.DestinationSettings_Account, controls.ToArray());
    }

    private Control LoopbackOAuth(
        Func<OAuth2Info?> getInfo,
        Func<OAuthUserInfo?> getUser,
        Action<OAuth2Info?, OAuthUserInfo?> setAccount,
        Func<OAuth2Info, IOAuth2Loopback> createOAuth)
    {
        TextBlock status = DestinationSettingsPageBuilder.Hint(string.Empty);
        void UpdateStatus()
        {
            bool isConnected = OAuth2Info.CheckOAuth(getInfo());
            SetStatus(status, isConnected
                ? (string.IsNullOrWhiteSpace(getUser()?.name)
                    ? Localization.Strings.DestinationSettings_Connected
                    : string.Format(Localization.Strings.DestinationSettings_Connected_as, getUser()!.name))
                : Localization.Strings.DestinationSettings_Not_connected, isConnected);
        }

        Button connect = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Connect_account, () =>
        {
            try
            {
                OAuth2Info info = UploaderOAuthClientFactory.CreateGoogle();
                OAuthListenerWindowResult? result =
                    OAuthListenerWindowIntegration.Show(createOAuth(info));
                setAccount(result?.OAuth2Info, result?.UserInfo);
                UpdateStatus();
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                SetStatus(status, exception.Message, false);
            }
        });
        Button disconnect = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Disconnect,
            () => { setAccount(null, null); UpdateStatus(); });
        UpdateStatus();
        return DestinationSettingsPageBuilder.Card(Localization.Strings.DestinationSettings_Account,
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_Status, status),
            DestinationSettingsPageBuilder.ButtonRow(connect, disconnect));
    }

    private Control FlickrOAuth()
    {
        TextBlock status = DestinationSettingsPageBuilder.Hint(string.Empty);
        TextBox code = DestinationSettingsPageBuilder.Text(() => string.Empty, _ => { });
        void UpdateStatus(string? text = null, bool? connected = null)
        {
            bool isConnected = OAuthInfo.CheckOAuth(_config.FlickrOAuthInfo);
            SetStatus(status, text ?? (isConnected
                ? Localization.Strings.DestinationSettings_Connected
                : Localization.Strings.DestinationSettings_Not_connected), text == null ? isConnected : connected);
        }

        Button open = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Open_authorization_page, async () =>
        {
            try
            {
                OAuthInfo info = UploaderOAuthClientFactory.CreateFlickr();
                string url = await new FlickrUploader(info).GetAuthorizationURLAsync();
                if (!string.IsNullOrEmpty(url)) { _config.FlickrOAuthInfo = info; URLHelpers.OpenURL(url); }
                UpdateStatus(Localization.Strings.DestinationSettings_Authorization_page_opened);
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                UpdateStatus(exception.Message, false);
            }
        });
        Button complete = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Complete_authorization, async () =>
        {
            try
            {
                bool result = _config.FlickrOAuthInfo != null &&
                    await new FlickrUploader(_config.FlickrOAuthInfo).GetAccessTokenAsync(code.Text ?? string.Empty);
                UpdateStatus(result
                    ? Localization.Strings.DestinationSettings_Connected
                    : Localization.Strings.DestinationSettings_Authorization_failed, result);
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                UpdateStatus(exception.Message, false);
            }
        });
        Button clear = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Disconnect,
            () => { _config.FlickrOAuthInfo = null; UpdateStatus(); });
        UpdateStatus();
        return DestinationSettingsPageBuilder.Card(Localization.Strings.DestinationSettings_Account,
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_Status, status),
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_Verification_code, code),
            DestinationSettingsPageBuilder.ButtonRow(open, complete, clear));
    }

    private Control PhotobucketOAuth()
    {
        TextBlock status = DestinationSettingsPageBuilder.Hint(string.Empty);
        TextBox code = DestinationSettingsPageBuilder.Text(() => string.Empty, _ => { });
        void UpdateStatus(string? text = null, bool? connected = null)
        {
            bool isConnected = OAuthInfo.CheckOAuth(_config.PhotobucketOAuthInfo);
            SetStatus(status, text ?? (isConnected
                ? Localization.Strings.DestinationSettings_Connected
                : Localization.Strings.DestinationSettings_Not_connected), text == null ? isConnected : connected);
        }

        Button open = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Open_authorization_page, async () =>
        {
            try
            {
                OAuthInfo info = UploaderOAuthClientFactory.CreatePhotobucket();
                string url = await new Photobucket(info).GetAuthorizationURLAsync();
                if (!string.IsNullOrEmpty(url)) { _config.PhotobucketOAuthInfo = info; URLHelpers.OpenURL(url); }
                UpdateStatus(Localization.Strings.DestinationSettings_Authorization_page_opened);
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                UpdateStatus(exception.Message, false);
            }
        });
        Button complete = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Complete_authorization, async () =>
        {
            try
            {
                if (_config.PhotobucketOAuthInfo == null) return;
                Photobucket uploader = new(_config.PhotobucketOAuthInfo);
                bool result = await uploader.GetAccessTokenAsync(code.Text ?? string.Empty);
                if (result) _config.PhotobucketAccountInfo = uploader.GetAccountInfo();
                UpdateStatus(result
                    ? Localization.Strings.DestinationSettings_Connected
                    : Localization.Strings.DestinationSettings_Authorization_failed, result);
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                UpdateStatus(exception.Message, false);
            }
        });
        Button clear = DestinationSettingsPageBuilder.Button(Localization.Strings.DestinationSettings_Disconnect, () =>
        {
            _config.PhotobucketOAuthInfo = null; _config.PhotobucketAccountInfo = null; UpdateStatus();
        });
        UpdateStatus();
        return DestinationSettingsPageBuilder.Card(Localization.Strings.DestinationSettings_Account,
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_Status, status),
            DestinationSettingsPageBuilder.Row(Localization.Strings.DestinationSettings_Verification_code, code),
            DestinationSettingsPageBuilder.ButtonRow(open, complete, clear));
    }

    private OAuth2Info CreateOneDriveInfo()
    {
        return UploaderOAuthClientFactory.CreateOneDrive();
    }

    private static TextBlock Status(string text, bool? connected = null)
    {
        TextBlock status = DestinationSettingsPageBuilder.Hint(text);
        SetStatus(status, text, connected);
        return status;
    }

    private static void SetStatus(TextBlock status, string text, bool? connected = null)
    {
        status.Text = text;
        status.Classes.Set("connection-connected", connected == true);
        status.Classes.Set("connection-disconnected", connected == false);
    }

}
