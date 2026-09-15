#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using ShareX.AvaloniaUI.Theming;
using Strings = ShareX.Tools.Localization.Strings;

namespace ShareX.Tools;

public sealed class MouseHighlighterWindow : Window
{
    private readonly MouseHighlighterOptions _options;
    private readonly Button _toggle;
    private readonly TextBlock _status;

    public MouseHighlighterWindow(MouseHighlighterOptions options, Action? configureHotkey, Action? settingsChanged)
    {
        _options = options;
        Title = "ShareX - " + Strings.MouseHighlighter_Title;
        Width = 620;
        Height = 690;
        MinWidth = 520;
        MinHeight = 460;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        this.Bind(BackgroundProperty, new DynamicResourceExtension("ShareX.Brush.Background.Main"));
        StackPanel panel = new() { Spacing = 14, Margin = new Thickness(20) };
        panel.Children.Add(new TextBlock { Text = Strings.MouseHighlighter_Title, FontSize = 22, FontWeight = FontWeight.SemiBold });
        _status = new TextBlock { TextWrapping = TextWrapping.Wrap, FontWeight = FontWeight.Normal };
        panel.Children.Add(_status);
        _toggle = new Button();
        _toggle.Click += (_, _) => Toggle();
        panel.Children.Add(_toggle);
        panel.Children.Add(new MouseHighlighterSettingsControl(options, settingsChanged));
        if (configureHotkey != null)
        {
            Button shortcut = new() { Content = Strings.MouseHighlighter_ConfigureShortcut };
            shortcut.Click += (_, _) => configureHotkey();
            panel.Children.Add(shortcut);
            panel.Children.Add(new TextBlock { Text = Strings.MouseHighlighter_ShortcutHelp, TextWrapping = TextWrapping.Wrap, FontWeight = FontWeight.Normal });
        }
        panel.Children.Add(new TextBlock { Text = Strings.MouseHighlighter_RecordingHelp, TextWrapping = TextWrapping.Wrap, FontWeight = FontWeight.Normal });
        Content = new ScrollViewer { Content = panel };
        MouseHighlighterManager.StateChanged += RefreshState;
        Closed += (_, _) => MouseHighlighterManager.StateChanged -= RefreshState;
        RefreshState();
    }

    private void Toggle()
    {
        try { MouseHighlighterManager.SetManualActive(!MouseHighlighterManager.IsManuallyActive, _options); }
        catch (Exception ex) { _status.Text = Strings.MouseHighlighter_StartFailed + " " + ex.Message; }
    }

    private void RefreshState()
    {
        if (MouseHighlighterManager.IsRecordingActive)
        {
            _toggle.Content = MouseHighlighterManager.IsManuallyActive ? Strings.MouseHighlighter_StopAfterRecording : Strings.MouseHighlighter_KeepAfterRecording;
            _status.Text = MouseHighlighterManager.IsManuallyActive ? Strings.MouseHighlighter_RecordingAndManualActive : Strings.MouseHighlighter_RecordingActive;
        }
        else
        {
            _toggle.Content = MouseHighlighterManager.IsManuallyActive ? Strings.MouseHighlighter_Stop : Strings.MouseHighlighter_Start;
            _status.Text = MouseHighlighterManager.IsManuallyActive ? Strings.MouseHighlighter_Active : Strings.MouseHighlighter_Inactive;
        }
    }
}
