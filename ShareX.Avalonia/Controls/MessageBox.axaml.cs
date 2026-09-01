#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using ShareX.AvaloniaUI.Localization;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.AvaloniaUI;

public enum DialogResult
{
    None,
    OK,
    Cancel,
    Abort,
    Retry,
    Ignore,
    Yes,
    No,
    TryAgain,
    Continue
}

public enum MessageBoxButtons
{
    OK,
    OKCancel,
    AbortRetryIgnore,
    YesNoCancel,
    YesNo,
    RetryCancel,
    CancelTryContinue
}

public enum MessageBoxIcon
{
    None,
    Error,
    Hand = Error,
    Stop = Error,
    Question,
    Warning,
    Exclamation = Warning,
    Information,
    Asterisk = Information
}

public enum MessageBoxDefaultButton
{
    Button1,
    Button2,
    Button3,
    Button4
}

public static class MessageBox
{
    public static DialogResult Show(string? text) => Show(text, string.Empty);

    public static DialogResult Show(string? text, string? caption) =>
        Show(text, caption, MessageBoxButtons.OK);

    public static DialogResult Show(string? text, string? caption, MessageBoxButtons buttons) =>
        Show(text, caption, buttons, MessageBoxIcon.None);

    public static DialogResult Show(string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon icon) =>
        Show(text, caption, buttons, icon, MessageBoxDefaultButton.Button1);

    public static DialogResult Show(string? text, string? caption, MessageBoxButtons buttons,
        MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) =>
        Show(null, text, caption, buttons, icon, defaultButton);

    public static DialogResult Show(Window? owner, string? text, string? caption,
        MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None,
        MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
    {
        AvaloniaBootstrapper.EnsureInitialized();

        if (Dispatcher.UIThread.CheckAccess())
        {
            DialogResult result = DialogResult.None;
            DispatcherFrame frame = new();
            ShowCore(owner, text, caption, buttons, icon, defaultButton, value =>
            {
                result = value;
                frame.Continue = false;
            });
            Dispatcher.UIThread.PushFrame(frame);
            return result;
        }

        TaskCompletionSource<DialogResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                ShowCore(owner, text, caption, buttons, icon, defaultButton,
                    value => completion.TrySetResult(value));
            }
            catch (Exception exception)
            {
                completion.TrySetException(exception);
            }
        });
        return completion.Task.GetAwaiter().GetResult();
    }

    private static void ShowCore(Window? owner, string? text, string? caption, MessageBoxButtons buttons,
        MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, Action<DialogResult> completed)
    {
        MessageBoxWindow window = new(text, caption, buttons, icon, defaultButton);
        window.Closed += (_, _) => completed(window.Result);

        owner ??= FindOwner();
        if (owner is { IsVisible: true })
        {
            _ = window.ShowDialog(owner);
        }
        else
        {
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Topmost = true;
            window.Show();
        }
    }

    private static Window? FindOwner()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            return null;
        }

        return desktop.Windows.LastOrDefault(window =>
            window is not MessageBoxWindow && window.IsVisible && window.IsActive);
    }
}

internal partial class MessageBoxWindow : Window
{
    private static readonly Color ErrorColor = Color.Parse("#EF4444");
    private static readonly Color WarningColor = Color.Parse("#F59E0B");
    private readonly DialogResult _cancelResult;

    public DialogResult Result { get; private set; }

    public MessageBoxWindow(string? text, string? caption, MessageBoxButtons buttons,
        MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        Title = caption ?? string.Empty;
        MessageText.Text = text ?? string.Empty;
        ConfigureIcon(icon);

        (DialogResult Result, string Text)[] buttonDefinitions = GetButtonDefinitions(buttons);
        int defaultButtonIndex = Math.Min((int)defaultButton, buttonDefinitions.Length - 1);
        _cancelResult = GetCancelResult(buttons);

        for (int index = 0; index < buttonDefinitions.Length; index++)
        {
            (DialogResult result, string buttonText) = buttonDefinitions[index];
            Button button = new()
            {
                Content = buttonText,
                MinWidth = 88,
                Height = 36,
                IsDefault = index == defaultButtonIndex,
                IsCancel = result == _cancelResult
            };

            if (index == defaultButtonIndex)
            {
                button.Classes.Add("active");
            }

            button.Click += (_, _) => CloseWithResult(result);
            ButtonPanel.Children.Add(button);

            if (index == defaultButtonIndex)
            {
                Opened += (_, _) => button.Focus();
            }
        }

        Closing += OnClosing;
    }

    private void ConfigureIcon(MessageBoxIcon icon)
    {
        string? iconText = icon switch
        {
            MessageBoxIcon.Error => LucideIcons.circle_x,
            MessageBoxIcon.Question => LucideIcons.circle_question_mark,
            MessageBoxIcon.Warning => LucideIcons.triangle_alert,
            MessageBoxIcon.Information => LucideIcons.info,
            _ => null
        };

        if (iconText == null)
        {
            IconContainer.IsVisible = false;
            return;
        }

        IconText.Text = iconText;
        IconText.Foreground = icon switch
        {
            MessageBoxIcon.Error => new SolidColorBrush(ErrorColor),
            MessageBoxIcon.Warning => new SolidColorBrush(WarningColor),
            _ => GetAccentBrush()
        };
    }

    private IBrush GetAccentBrush() =>
        this.FindResource("ShareX.Brush.Accent.Start") as IBrush ?? Brushes.DodgerBlue;

    private static (DialogResult Result, string Text)[] GetButtonDefinitions(MessageBoxButtons buttons) => buttons switch
    {
        MessageBoxButtons.OKCancel =>
        [
            (DialogResult.OK, Strings.MessageBox_OK),
            (DialogResult.Cancel, Strings.MessageBox_Cancel)
        ],
        MessageBoxButtons.AbortRetryIgnore =>
        [
            (DialogResult.Abort, Strings.MessageBox_Abort),
            (DialogResult.Retry, Strings.MessageBox_Retry),
            (DialogResult.Ignore, Strings.MessageBox_Ignore)
        ],
        MessageBoxButtons.YesNoCancel =>
        [
            (DialogResult.Yes, Strings.MessageBox_Yes),
            (DialogResult.No, Strings.MessageBox_No),
            (DialogResult.Cancel, Strings.MessageBox_Cancel)
        ],
        MessageBoxButtons.YesNo =>
        [
            (DialogResult.Yes, Strings.MessageBox_Yes),
            (DialogResult.No, Strings.MessageBox_No)
        ],
        MessageBoxButtons.RetryCancel =>
        [
            (DialogResult.Retry, Strings.MessageBox_Retry),
            (DialogResult.Cancel, Strings.MessageBox_Cancel)
        ],
        MessageBoxButtons.CancelTryContinue =>
        [
            (DialogResult.Cancel, Strings.MessageBox_Cancel),
            (DialogResult.TryAgain, Strings.MessageBox_Try_again),
            (DialogResult.Continue, Strings.MessageBox_Continue)
        ],
        _ => [(DialogResult.OK, Strings.MessageBox_OK)]
    };

    private static DialogResult GetCancelResult(MessageBoxButtons buttons) => buttons switch
    {
        MessageBoxButtons.OK => DialogResult.OK,
        MessageBoxButtons.OKCancel or MessageBoxButtons.YesNoCancel or MessageBoxButtons.RetryCancel or
            MessageBoxButtons.CancelTryContinue => DialogResult.Cancel,
        _ => DialogResult.None
    };

    private void CloseWithResult(DialogResult result)
    {
        Result = result;
        Close();
    }

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (Result != DialogResult.None)
        {
            return;
        }

        if (_cancelResult == DialogResult.None)
        {
            e.Cancel = true;
        }
        else
        {
            Result = _cancelResult;
        }
    }
}
