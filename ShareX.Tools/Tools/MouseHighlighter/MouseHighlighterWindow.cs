#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using ShareX.AvaloniaUI.Theming;
using ShareX.Tools.Localization;

namespace ShareX.Tools;

public sealed class MouseHighlighterWindow : Window
{
    private readonly MouseHighlighterOptions _options;
    private readonly Button _toggle;

    public MouseHighlighterWindow(MouseHighlighterOptions options, Action? settingsChanged)
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
        _toggle = new Button();
        _toggle.Click += (_, _) => Toggle();
        panel.Children.Add(_toggle);
        panel.Children.Add(CreateRecordingTip());
        panel.Children.Add(new MouseHighlighterSettingsControl(options, settingsChanged));
        Content = new ScrollViewer { Content = panel };
        MouseHighlighterManager.StateChanged += RefreshState;
        Closed += (_, _) => MouseHighlighterManager.StateChanged -= RefreshState;
        RefreshState();
    }

    private static Control CreateRecordingTip()
    {
        TextBlock icon = new()
        {
            Text = LucideIcons.info,
            FontSize = 18,
            FontWeight = FontWeight.Normal,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top
        };
        icon.Bind(TextBlock.FontFamilyProperty, new DynamicResourceExtension("ShareX.FontFamily.Icon"));
        icon.Bind(TextBlock.ForegroundProperty, new DynamicResourceExtension("ShareX.Brush.Accent.Start"));

        TextBlock text = new()
        {
            Text = Strings.MouseHighlighter_RecordingHelp,
            TextWrapping = TextWrapping.Wrap,
            FontWeight = FontWeight.Normal,
            Margin = new Thickness(10, 0, 0, 0)
        };
        text.Bind(TextBlock.ForegroundProperty, new DynamicResourceExtension("ShareX.Brush.Text.Secondary"));
        Grid.SetColumn(text, 1);

        Grid content = new() { ColumnDefinitions = new ColumnDefinitions("Auto,*") };
        content.Children.Add(icon);
        content.Children.Add(text);

        Border tip = new()
        {
            Child = content,
            Padding = new Thickness(12),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6)
        };
        tip.Bind(Border.BackgroundProperty, new DynamicResourceExtension("ShareX.Brush.Background.Panel"));
        tip.Bind(Border.BorderBrushProperty, new DynamicResourceExtension("ShareX.Brush.Border"));
        return tip;
    }

    private void Toggle()
    {
        try { MouseHighlighterManager.SetManualActive(!MouseHighlighterManager.IsManuallyActive, _options); }
        catch (Exception ex)
        {
            ShareX.AvaloniaUI.MessageBox.Show(Strings.MouseHighlighter_StartFailed + " " + ex.Message,
                "ShareX", ShareX.AvaloniaUI.MessageBoxButtons.OK, ShareX.AvaloniaUI.MessageBoxIcon.Error);
        }
    }

    private void RefreshState()
    {
        if (MouseHighlighterManager.IsRecordingActive)
        {
            _toggle.Content = MouseHighlighterManager.IsManuallyActive ? Strings.MouseHighlighter_StopAfterRecording : Strings.MouseHighlighter_KeepAfterRecording;
        }
        else
        {
            _toggle.Content = MouseHighlighterManager.IsManuallyActive ? Strings.MouseHighlighter_Stop : Strings.MouseHighlighter_Start;
        }
    }
}
