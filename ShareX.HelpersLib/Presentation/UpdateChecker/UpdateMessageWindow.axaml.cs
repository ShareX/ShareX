#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using ShareX.AvaloniaUI.Theming;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.HelpersLib;

public partial class UpdateMessageWindow : Window
{
    private DialogResult _result;

    public static bool IsOpen { get; private set; }

    public bool ActivateWindow { get; }
    public string MessageText { get; }
    public bool ShowChangelog { get; }
    public string DialogTitle => Localization.Strings.UpdateMessageBox_UpdateMessageBox_update_is_available;
    public string YesText => Localization.Strings.MyMessageBox_MyMessageBox_Yes;
    public string NoText => Localization.Strings.MyMessageBox_MyMessageBox_No;

    public UpdateMessageWindow()
    {
        ActivateWindow = true;
        MessageText = string.Empty;
        ShowChangelog = true;
        InitializeWindow();
    }

    private UpdateMessageWindow(UpdateChecker updateChecker, bool activateWindow)
    {
        ActivateWindow = activateWindow;
        MessageText = BuildMessage(updateChecker);
        ShowChangelog = !updateChecker.IsDev;

        InitializeWindow();
    }

    private void InitializeWindow()
    {
        InitializeComponent();
        DataContext = this;
        Title = DialogTitle;
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        ShowActivated = ActivateWindow;

        Opened += OnOpened;
        Closing += (_, _) =>
        {
            if (_result == DialogResult.None) _result = DialogResult.No;
        };
    }

    public static async Task<DialogResult> StartAsync(UpdateChecker? updateChecker, bool activateWindow = true)
    {
        if (updateChecker == null || updateChecker.Status != UpdateStatus.UpdateAvailable)
        {
            return DialogResult.None;
        }

        IsOpen = true;

        try
        {
            DialogResult result = await ShowWindowAsync(updateChecker, activateWindow);
            if (result == DialogResult.Yes)
            {
                await updateChecker.DownloadUpdateAsync();
            }

            return result;
        }
        finally
        {
            IsOpen = false;
        }
    }

    private static Task<DialogResult> ShowWindowAsync(UpdateChecker updateChecker, bool activateWindow)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        TaskCompletionSource<DialogResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                UpdateMessageWindow window = new(updateChecker, activateWindow);
                window.Closed += (_, _) => completion.TrySetResult(window._result);
                window.Show();
            }
            catch (Exception exception)
            {
                completion.TrySetException(exception);
            }
        });

        return completion.Task;
    }

    private static string BuildMessage(UpdateChecker updateChecker)
    {
        StringBuilder text = new();
        string productName = System.Windows.Forms.Application.ProductName ?? "ShareX";

        text.AppendLine(Helpers.SafeStringFormat(
            updateChecker.IsPortable
                ? Localization.Strings.UpdateMessageBox_UpdateMessageBox_Portable
                : Localization.Strings.UpdateMessageBox_UpdateMessageBox_,
            productName));
        text.AppendLine();
        text.Append(Localization.Strings.UpdateMessageBox_UpdateMessageBox_CurrentVersion);
        text.Append(": ");
        text.Append(updateChecker.CurrentVersion);
        text.AppendLine();
        text.Append(Localization.Strings.UpdateMessageBox_UpdateMessageBox_LatestVersion);
        text.Append(": ");
        text.Append(updateChecker.LatestVersion);
        if (updateChecker.IsDev) text.Append(" Dev");
        if (updateChecker is GitHubUpdateChecker { IsPreRelease: true })
            text.Append($" ({Localization.Strings.UpdateChannel_PreRelease})");
        return text.ToString();
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        if (ActivateWindow)
        {
            Activate();
            return;
        }

        WindowState = Avalonia.Controls.WindowState.Minimized;
        IntPtr handle = TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
        if (handle != IntPtr.Zero) NativeMethods.FlashWindowEx(handle, 10);
    }

    private void OnViewChangelogClick(object? sender, RoutedEventArgs e) => URLHelpers.OpenURL(Links.Changelog);

    private void OnYesClick(object? sender, RoutedEventArgs e)
    {
        _result = DialogResult.Yes;
        Close();
    }

    private void OnNoClick(object? sender, RoutedEventArgs e)
    {
        _result = DialogResult.No;
        Close();
    }
}
