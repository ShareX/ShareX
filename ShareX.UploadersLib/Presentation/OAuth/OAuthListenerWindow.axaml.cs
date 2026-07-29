#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using Avalonia.Interactivity;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using System;
using System.Threading.Tasks;

namespace ShareX.UploadersLib;

public sealed record OAuthListenerWindowResult(OAuth2Info OAuth2Info, OAuthUserInfo? UserInfo);

public partial class OAuthListenerWindow : Window
{
    private readonly IOAuth2Loopback _oauth;
    private OAuthListener? _listener;
    private bool _isClosed;

    public OAuthListenerWindowResult? SubmittedResult { get; private set; }

    public OAuthListenerWindow() : this(new NullOAuth2Loopback())
    {
    }

    public OAuthListenerWindow(IOAuth2Loopback oauth)
    {
        _oauth = oauth;

        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        Opened += OnOpened;
        Closed += OnClosed;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Activate();
        await ConnectAsync();

        if (!_isClosed)
        {
            Close();
        }
    }

    private async Task ConnectAsync()
    {
        try
        {
            _listener = new OAuthListener(_oauth);
            bool connected = await _listener.ConnectAsync();

            if (connected && !_isClosed)
            {
                OAuthUserInfo? userInfo = await Task.Run(_oauth.GetUserInfo);
                SubmittedResult = new OAuthListenerWindowResult(_oauth.AuthInfo, userInfo);
            }
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
        }
        finally
        {
            _listener?.Dispose();
            _listener = null;
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _isClosed = true;
        _listener?.Dispose();
        _listener = null;
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();

    private sealed class NullOAuth2Loopback : IOAuth2Loopback
    {
        public OAuth2Info AuthInfo { get; } = new(string.Empty, string.Empty);
        public string RedirectURI { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;

        public string GetAuthorizationURL() => string.Empty;
        public bool GetAccessToken(string code) => false;
        public bool RefreshAccessToken() => false;
        public bool CheckAuthorization() => false;
        public OAuthUserInfo? GetUserInfo() => null;
    }
}
