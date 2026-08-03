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
using ShareX.UploadersLib.FileUploaders;
using System;
using System.Threading.Tasks;

namespace ShareX.UploadersLib;

public partial class PuushLoginWindow : Window
{
    private bool _isLoggingIn;

    public string? SubmittedApiKey { get; private set; }

    public PuushLoginWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Activate();
        EmailTextBox.Focus();
    }

    private async void OnLoginClick(object? sender, RoutedEventArgs e)
    {
        if (_isLoggingIn || !ValidateCredentials())
        {
            return;
        }

        string email = EmailTextBox.Text?.Trim() ?? string.Empty;
        string password = PasswordTextBox.Text ?? string.Empty;
        SetLoginState(true);

        try
        {
            string? apiKey = await Task.Run(() => new Puush().Login(email, password));
            if (!string.IsNullOrEmpty(apiKey))
            {
                SubmittedApiKey = apiKey;
                Close();
                return;
            }

            ShowError(Localization.Strings.PuushLoginWindow_Login_failed);
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            ShowError(exception.Message);
        }
        finally
        {
            SetLoginState(false);
        }
    }

    private bool ValidateCredentials()
    {
        bool hasEmail = !string.IsNullOrWhiteSpace(EmailTextBox.Text);
        bool hasPassword = !string.IsNullOrEmpty(PasswordTextBox.Text);

        EmailTextBox.Classes.Set("invalid", !hasEmail);
        PasswordTextBox.Classes.Set("invalid", !hasPassword);

        if (!hasEmail || !hasPassword)
        {
            ShowError(Localization.Strings.PuushLoginWindow_Enter_both_your_email_and_password);
            return false;
        }

        StatusText.IsVisible = false;
        return true;
    }

    private void SetLoginState(bool isLoggingIn)
    {
        _isLoggingIn = isLoggingIn;
        CredentialsCard.IsEnabled = !isLoggingIn;
        LoginButton.IsEnabled = !isLoggingIn;
        LoginButton.Content = isLoggingIn
            ? Localization.Strings.PuushLoginWindow_Logging_in
            : Localization.Strings.PuushLoginWindow_Login;
        LoginProgressBar.IsVisible = isLoggingIn;
    }

    private void ShowError(string message)
    {
        StatusText.Text = message;
        StatusText.IsVisible = true;
    }

    private void OnForgotPasswordClick(object? sender, RoutedEventArgs e) =>
        URLHelpers.OpenURL(Puush.PuushResetPasswordURL);

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();
}
