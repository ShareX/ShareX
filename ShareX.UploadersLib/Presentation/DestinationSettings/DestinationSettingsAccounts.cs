#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using ShareX.HelpersLib;
using ShareX.UploadersLib;
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
        "paste-ee" => DestinationSettingsPageBuilder.Card("Account",
            DestinationSettingsPageBuilder.ButtonRow(DestinationSettingsPageBuilder.Button("Get user key", () =>
                URLHelpers.OpenURL(UploaderConfigurationClient.GetPasteEeAuthorizationURL())))),
        "puush" => UserPasswordLogin("puush", () => _config.PuushAPIKey,
            (username, password) => new Puush().Login(username, password),
            value => _config.PuushAPIKey = value),
        "lobfile" => UserPasswordLogin("LobFile", () => _config.LithiioSettings.UserAPIKey,
            (username, password) => new LobFile().FetchAPIKey(username, password),
            value => _config.LithiioSettings.UserAPIKey = value),
        "pushbullet" => PushbulletAccount(),
        _ => null
    };

    private Control ImageShackAccount()
    {
        TextBlock status = DestinationSettingsPageBuilder.Hint(
            string.IsNullOrEmpty(_config.ImageShackSettings.Auth_token) ? "Not connected" : "Connected");
        Button login = DestinationSettingsPageBuilder.Button("Log in", () =>
        {
            try
            {
                status.Text = UploaderConfigurationClient.LoginImageShack(_config.ImageShackSettings)
                    ? "Connected"
                    : "Login failed";
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                status.Text = exception.Message;
            }
        });
        Button profile = DestinationSettingsPageBuilder.Button("Public profile", () =>
        {
            if (!string.IsNullOrWhiteSpace(_config.ImageShackSettings.Username))
                URLHelpers.OpenURL("https://imageshack.com/user/" + _config.ImageShackSettings.Username);
        });
        Button images = DestinationSettingsPageBuilder.Button("My images", () => URLHelpers.OpenURL("https://imageshack.com/my/images"));
        return DestinationSettingsPageBuilder.Card("Account", DestinationSettingsPageBuilder.Row("Status:", status),
            DestinationSettingsPageBuilder.ButtonRow(login, profile, images));
    }

    private Control PastebinAccount()
    {
        TextBlock status = DestinationSettingsPageBuilder.Hint(
            string.IsNullOrEmpty(_config.PastebinSettings.UserKey) ? "Not connected" : "Connected");
        Button login = DestinationSettingsPageBuilder.Button("Log in", () =>
        {
            try
            {
                status.Text = UploaderConfigurationClient.LoginPastebin(_config.PastebinSettings)
                    ? "Connected"
                    : "Login failed";
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                status.Text = exception.Message;
            }
        });
        Button register = DestinationSettingsPageBuilder.Button("Register", () => URLHelpers.OpenURL("https://pastebin.com/signup"));
        return DestinationSettingsPageBuilder.Card("Account", DestinationSettingsPageBuilder.Row("Status:", status),
            DestinationSettingsPageBuilder.ButtonRow(login, register));
    }

    private Control UserPasswordLogin(string serviceName, Func<string> getKey, Func<string, string, string?> login, Action<string> saveKey)
    {
        TextBox username = DestinationSettingsPageBuilder.Text(() => string.Empty, _ => { });
        TextBox password = DestinationSettingsPageBuilder.Text(() => string.Empty, _ => { });
        password.PasswordChar = '●';
        TextBox apiKey = DestinationSettingsPageBuilder.Text(getKey, saveKey);
        apiKey.PasswordChar = '●';
        TextBlock status = DestinationSettingsPageBuilder.Hint("Enter account credentials to retrieve the API key");
        Button connect = DestinationSettingsPageBuilder.Button("Log in", () =>
        {
            try
            {
                string? key = login(username.Text ?? string.Empty, password.Text ?? string.Empty);
                if (!string.IsNullOrWhiteSpace(key))
                {
                    ((DestinationValue<string>)apiKey.DataContext!).Value = key;
                    status.Text = "API key retrieved";
                    password.Text = string.Empty;
                }
                else
                {
                    status.Text = "Login failed";
                }
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                status.Text = exception.Message;
            }
        });
        return DestinationSettingsPageBuilder.Card(serviceName + " account",
            DestinationSettingsPageBuilder.Row("Email / username:", username),
            DestinationSettingsPageBuilder.Row("Password:", password),
            DestinationSettingsPageBuilder.Row("API key:", apiKey),
            DestinationSettingsPageBuilder.ButtonRow(connect), DestinationSettingsPageBuilder.Row("Status:", status));
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
                devices.Add(new DestinationChoice(item, item.Name ?? "Unnamed device"));
            }

            int index = devices.Count == 0 ? -1 : previousIndex.BetweenOrDefault(0, devices.Count - 1);
            device.SelectedIndex = index;
            _config.PushbulletSettings.SelectedDevice = index;
            status.Text = devices.Count == 0 ? "No devices loaded" : $"{devices.Count} device(s) loaded";
        }

        device.SelectionChanged += (_, _) => _config.PushbulletSettings.SelectedDevice = device.SelectedIndex;
        Button refresh = DestinationSettingsPageBuilder.Button("Refresh devices", () =>
        {
            try
            {
                _config.PushbulletSettings.DeviceList = new Pushbullet(_config.PushbulletSettings).GetDeviceList() ?? [];
                Reload();
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                status.Text = exception.Message;
            }
        });
        Reload();
        return DestinationSettingsPageBuilder.Card("Account and devices", DestinationSettingsPageBuilder.Row("API key:", apiKey),
            DestinationSettingsPageBuilder.Row("Device:", device), DestinationSettingsPageBuilder.ButtonRow(refresh),
            DestinationSettingsPageBuilder.Row("Status:", status));
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

        void UpdateStatus(string? message = null)
        {
            status.Text = message ?? (OAuth2Info.CheckOAuth(getInfo()) ? "Connected" : "Not connected");
        }

        Button open = DestinationSettingsPageBuilder.Button("Open authorization page", () =>
        {
            try
            {
                OAuth2Info info = createInfo();
                IOAuth2Basic uploader = createUploader(info);
                string url = uploader.GetAuthorizationURL();
                setInfo(string.IsNullOrEmpty(url) ? null : uploader.AuthInfo);
                if (!string.IsNullOrEmpty(url)) URLHelpers.OpenURL(url);
                UpdateStatus("Authorization page opened");
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                UpdateStatus(exception.Message);
            }
        });
        Button complete = DestinationSettingsPageBuilder.Button("Complete authorization", () =>
        {
            try
            {
                OAuth2Info? info = getInfo();
                string authorizationCode = code.Text ?? string.Empty;
                bool result = info != null && !string.IsNullOrWhiteSpace(authorizationCode) && createUploader(info).GetAccessToken(authorizationCode);
                UpdateStatus(result ? "Connected" : "Authorization failed");
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                UpdateStatus(exception.Message);
            }
        });
        Button refresh = DestinationSettingsPageBuilder.Button("Refresh token", () =>
        {
            try
            {
                bool result = getInfo() is { } info && createUploader(info) is IOAuth2 oauth && oauth.RefreshAccessToken();
                UpdateStatus(result ? "Connected" : "Token refresh failed");
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                UpdateStatus(exception.Message);
            }
        });
        Button clear = DestinationSettingsPageBuilder.Button("Disconnect", () => { setInfo(null); UpdateStatus(); });

        List<Control> controls =
        [
            DestinationSettingsPageBuilder.Row("Status:", status),
            DestinationSettingsPageBuilder.Row("Authorization code:", code),
            DestinationSettingsPageBuilder.ButtonRow(open, complete, refresh, clear)
        ];
        controls.AddRange(extras);
        UpdateStatus();
        return DestinationSettingsPageBuilder.Card("Account", controls.ToArray());
    }

    private Control LoopbackOAuth(
        Func<OAuth2Info?> getInfo,
        Func<OAuthUserInfo?> getUser,
        Action<OAuth2Info?, OAuthUserInfo?> setAccount,
        Func<OAuth2Info, IOAuth2Loopback> createOAuth)
    {
        TextBlock status = DestinationSettingsPageBuilder.Hint(string.Empty);
        void UpdateStatus() => status.Text = OAuth2Info.CheckOAuth(getInfo())
            ? "Connected" + (string.IsNullOrWhiteSpace(getUser()?.name) ? string.Empty : " as " + getUser()!.name)
            : "Not connected";

        Button connect = DestinationSettingsPageBuilder.Button("Connect account", () =>
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
                status.Text = exception.Message;
            }
        });
        Button disconnect = DestinationSettingsPageBuilder.Button("Disconnect", () => { setAccount(null, null); UpdateStatus(); });
        UpdateStatus();
        return DestinationSettingsPageBuilder.Card("Account",
            DestinationSettingsPageBuilder.Row("Status:", status),
            DestinationSettingsPageBuilder.ButtonRow(connect, disconnect));
    }

    private Control FlickrOAuth()
    {
        TextBlock status = DestinationSettingsPageBuilder.Hint(string.Empty);
        TextBox code = DestinationSettingsPageBuilder.Text(() => string.Empty, _ => { });
        void UpdateStatus(string? text = null) => status.Text = text ?? (OAuthInfo.CheckOAuth(_config.FlickrOAuthInfo) ? "Connected" : "Not connected");

        Button open = DestinationSettingsPageBuilder.Button("Open authorization page", () =>
        {
            try
            {
                OAuthInfo info = UploaderOAuthClientFactory.CreateFlickr();
                string url = new FlickrUploader(info).GetAuthorizationURL();
                if (!string.IsNullOrEmpty(url)) { _config.FlickrOAuthInfo = info; URLHelpers.OpenURL(url); }
                UpdateStatus("Authorization page opened");
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                UpdateStatus(exception.Message);
            }
        });
        Button complete = DestinationSettingsPageBuilder.Button("Complete authorization", () =>
        {
            try
            {
                bool result = _config.FlickrOAuthInfo != null && new FlickrUploader(_config.FlickrOAuthInfo).GetAccessToken(code.Text ?? string.Empty);
                UpdateStatus(result ? "Connected" : "Authorization failed");
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                UpdateStatus(exception.Message);
            }
        });
        Button clear = DestinationSettingsPageBuilder.Button("Disconnect", () => { _config.FlickrOAuthInfo = null; UpdateStatus(); });
        UpdateStatus();
        return DestinationSettingsPageBuilder.Card("Account", DestinationSettingsPageBuilder.Row("Status:", status),
            DestinationSettingsPageBuilder.Row("Verification code:", code), DestinationSettingsPageBuilder.ButtonRow(open, complete, clear));
    }

    private Control PhotobucketOAuth()
    {
        TextBlock status = DestinationSettingsPageBuilder.Hint(string.Empty);
        TextBox code = DestinationSettingsPageBuilder.Text(() => string.Empty, _ => { });
        void UpdateStatus(string? text = null) => status.Text = text ?? (OAuthInfo.CheckOAuth(_config.PhotobucketOAuthInfo) ? "Connected" : "Not connected");

        Button open = DestinationSettingsPageBuilder.Button("Open authorization page", () =>
        {
            try
            {
                OAuthInfo info = UploaderOAuthClientFactory.CreatePhotobucket();
                string url = new Photobucket(info).GetAuthorizationURL();
                if (!string.IsNullOrEmpty(url)) { _config.PhotobucketOAuthInfo = info; URLHelpers.OpenURL(url); }
                UpdateStatus("Authorization page opened");
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                UpdateStatus(exception.Message);
            }
        });
        Button complete = DestinationSettingsPageBuilder.Button("Complete authorization", () =>
        {
            try
            {
                if (_config.PhotobucketOAuthInfo == null) return;
                Photobucket uploader = new(_config.PhotobucketOAuthInfo);
                bool result = uploader.GetAccessToken(code.Text ?? string.Empty);
                if (result) _config.PhotobucketAccountInfo = uploader.GetAccountInfo();
                UpdateStatus(result ? "Connected" : "Authorization failed");
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                UpdateStatus(exception.Message);
            }
        });
        Button clear = DestinationSettingsPageBuilder.Button("Disconnect", () =>
        {
            _config.PhotobucketOAuthInfo = null; _config.PhotobucketAccountInfo = null; UpdateStatus();
        });
        UpdateStatus();
        return DestinationSettingsPageBuilder.Card("Account", DestinationSettingsPageBuilder.Row("Status:", status),
            DestinationSettingsPageBuilder.Row("Verification code:", code), DestinationSettingsPageBuilder.ButtonRow(open, complete, clear));
    }

    private OAuth2Info CreateOneDriveInfo()
    {
        return UploaderOAuthClientFactory.CreateOneDrive();
    }

}
