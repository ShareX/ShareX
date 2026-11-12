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
using Avalonia.Controls.Chrome;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ShareX.PhotoEditor.UI.Dialogs;
using ShareX.PhotoEditor.UI.Theme;
using ShareX.PixelEditor.UI.Commands;
using Path = System.IO.Path;
using Shape = Avalonia.Controls.Shapes.Path;

namespace ShareX.PixelEditor.UI;

public sealed partial class PixelEditorWindow
{
    private const double TitleBarHeight = 32;
    private const string MaximizeGlyph = "M0.5 0.5 H9.5 V9.5 H0.5 Z";
    private const string RestoreGlyph = "M0.5 2.5 H7.5 V9.5 H0.5 Z M2.5 2.5 V0.5 H9.5 V7.5 H7.5";

    private Border _titleBar = null!;
    private Border _menuHost = null!;
    private TextBlock _titleText = null!;
    private Button _maximizeButton = null!;
    private Shape _maximizeGlyph = null!;
    private bool _windowPlacementApplied, _windowPlacementSavePending;
    private WindowState _restoreWindowState = WindowState.Normal;

    // ------------------------------------------------------------------ sprites

    public void OpenSprite(Sprite sprite, string? historyName = null)
    {
        sprite.History.MaxStates = Settings.HistoryStates;
        if (sprite.Palette.Count == 0) sprite.Palette = [.. Palettes.Default];
        sprite.History.Initialize(historyName ?? Strings.History_Open);
        SpriteView view = new(sprite);
        _views.Add(view);
        ActivateView(view);
    }

    public void ActivateView(SpriteView? view)
    {
        if (_view == view) return;
        CommitFloating();
        StopPlayback();
        Tools.Active.Deactivate();
        _view = view;
        Tools.Active.Activate();
        _statusOverride = null;
        ApplyVisibility();
        RefreshPanels();
        RefreshOptions();
        InvalidateCanvas();
    }

    public async Task<bool> CloseViewAsync(SpriteView view)
    {
        if (IsBusy) return false;
        if (view.Sprite.IsDirty)
        {
            ActivateView(view);
            bool? save = await DialogBuilder.ConfirmAsync(this, Strings.Window_Name, string.Format(Strings.Message_SaveChanges, view.Sprite.DisplayName), Strings.Command_Save, Strings.Message_DontSave);
            if (save == null) return false;
            if (save == true && !await EditorCommands.SaveAsync(this, false)) return false;
        }
        if (_view == view)
        {
            CommitFloating();
            StopPlayback();
        }
        int index = _views.IndexOf(view);
        _views.Remove(view);
        view.Dispose();
        if (_view == view)
        {
            _view = null;
            SpriteView? next = _views.Count > 0 ? _views[Math.Clamp(index, 0, _views.Count - 1)] : null;
            if (next != null) ActivateView(next);
            else
            {
                ApplyVisibility();
                RefreshPanels();
                RefreshOptions();
            }
        }
        RefreshTabs();
        InvalidateCanvas();
        return true;
    }

    private void RefreshTabs()
    {
        _shownZoom = _view?.Zoom ?? 0;
        _tabs.Children.Clear();
        foreach (SpriteView view in _views)
        {
            bool active = view == _view;
            SpriteView captured = view;
            Sprite sprite = view.Sprite;
            string text = $"{sprite.DisplayName} @ {view.Zoom * 100:0.#}%{(sprite.IsDirty ? " *" : string.Empty)}";
            Button close = new() { Classes = { "icon" }, Content = new IconView(PixelEditorIcons.Close, 10), Padding = new Thickness(3), Width = 16, Height = 16, VerticalAlignment = VerticalAlignment.Center, Opacity = active ? 1 : 0.6 };
            ToolTip.SetTip(close, Strings.Command_Close);
            close.Click += async (_, e) =>
            {
                e.Handled = true;
                await CloseViewAsync(captured);
            };
            StackPanel content = new() { Orientation = Orientation.Horizontal, Spacing = 8, VerticalAlignment = VerticalAlignment.Center };
            content.Children.Add(new TextBlock { Text = text, VerticalAlignment = VerticalAlignment.Center, Foreground = active ? Brushes.White : Ui.DimBrush });
            content.Children.Add(close);
            Border tab = new()
            {
                Child = content,
                Padding = new Thickness(12, 0, 6, 0),
                Height = 28,
                Margin = new Thickness(0, 0, 1, 0),
                Background = active ? Ui.Chrome : new SolidColorBrush(Color.FromRgb(0x2A, 0x2A, 0x2A)),
                BorderBrush = active ? PhotoEditorTheme.AccentBorderBrush : Brushes.Transparent,
                BorderThickness = new Thickness(0, active ? 2 : 0, 0, 0),
                Cursor = new Cursor(StandardCursorType.Hand)
            };
            ToolTip.SetTip(tab, sprite.FilePath ?? sprite.Title);
            tab.PointerPressed += (_, e) =>
            {
                if (e.GetCurrentPoint(tab).Properties.IsMiddleButtonPressed)
                {
                    _ = CloseViewAsync(captured);
                    return;
                }
                ActivateView(captured);
            };
            _tabs.Children.Add(tab);
        }
    }

    private void UpdateTitle()
    {
        Title = Sprite != null ? string.Format(Strings.Window_TitleWithSprite, Sprite.DisplayName) : Strings.Window_Title;
        _titleText.Text = Sprite != null ? Sprite.DisplayName + (Sprite.IsDirty ? " *" : string.Empty) : Strings.Window_Name;
    }

    // ------------------------------------------------------------------ file dialogs

    private async Task<IStorageFolder?> StartFolderAsync()
    {
        string? folder = Sprite?.FilePath != null ? Path.GetDirectoryName(Sprite.FilePath) : Services.DefaultFolder;
        return string.IsNullOrEmpty(folder) ? null : await StorageProvider.TryGetFolderFromPathAsync(folder);
    }

    public async Task<IReadOnlyList<string>> PickOpenFilesAsync(string title, bool multiple, params (string Name, string[] Extensions)[] types)
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = multiple,
            SuggestedStartLocation = await StartFolderAsync(),
            FileTypeFilter = [.. types.Select(t => new FilePickerFileType(t.Name) { Patterns = t.Extensions.Select(x => "*" + x).ToArray() }), FilePickerFileTypes.All]
        });
        return files.Select(f => f.TryGetLocalPath()).Where(p => p != null).Select(p => p!).ToList();
    }

    public async Task<string?> PickSaveFileAsync(string title, string suggestedName, params (string Name, string[] Extensions)[] types)
    {
        List<FilePickerFileType> choices = types.Select(t => new FilePickerFileType(t.Name) { Patterns = t.Extensions.Select(x => "*" + x).ToArray() }).ToList();
        string extension = Path.GetExtension(suggestedName);
        int match = Array.FindIndex(types, t => t.Extensions.Contains(extension, StringComparer.OrdinalIgnoreCase));
        IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = suggestedName,
            SuggestedStartLocation = await StartFolderAsync(),
            DefaultExtension = match >= 0 ? extension : types[0].Extensions[0],
            SuggestedFileType = match >= 0 ? choices[match] : choices[0],
            ShowOverwritePrompt = true,
            FileTypeChoices = choices
        });
        return file?.TryGetLocalPath();
    }

    // ------------------------------------------------------------------ window lifetime

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == WindowStateProperty && change.GetNewValue<WindowState>() != WindowState.Minimized)
        {
            _restoreWindowState = change.GetNewValue<WindowState>();
        }
    }

    /// <summary>Shows the window, restores it when minimized and brings it to the front.</summary>
    public void BringToFront()
    {
        if (!IsVisible) Show();
        if (WindowState == WindowState.Minimized) WindowState = _restoreWindowState;
        Activate();
    }

    private async void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (IsBusy)
        {
            e.Cancel = true;
            return;
        }
        if (_closingConfirmed)
        {
            SaveSettings();
            return;
        }
        CommitFloating();
        if (_views.Any(v => v.Sprite.IsDirty))
        {
            e.Cancel = true;
            foreach (SpriteView view in _views.ToList())
            {
                if (!await CloseViewAsync(view)) return;
            }
            _closingConfirmed = true;
            Close();
            return;
        }
        SaveSettings();
        _view = null;
        foreach (SpriteView view in _views) view.Dispose();
        _views.Clear();
    }

    private void ApplySavedWindowState()
    {
        if (!Settings.RememberWindowState) return;
        ShareX.HelpersLib.WindowState saved = Settings.WindowState;
        if (!saved.Size.IsEmpty)
        {
            Width = saved.Size.Width;
            Height = saved.Size.Height;
        }
        if (!saved.Location.IsEmpty)
        {
            PixelPoint position = new(saved.Location.X, saved.Location.Y);
            if (Screens.ScreenFromPoint(position) != null)
            {
                WindowStartupLocation = WindowStartupLocation.Manual;
                Position = position;
            }
        }
        if (saved.IsMaximized) WindowState = WindowState.Maximized;
    }

    private void QueueWindowPlacementSave()
    {
        if (!_windowPlacementApplied || _windowPlacementSavePending) return;
        _windowPlacementSavePending = true;
        // Maximizing moves the window before its state changes, so wait for the state to settle.
        Dispatcher.UIThread.Post(() =>
        {
            _windowPlacementSavePending = false;
            SaveNormalWindowPlacement();
        }, DispatcherPriority.Background);
    }

    private void SaveNormalWindowPlacement()
    {
        if (!Settings.RememberWindowState || WindowState != WindowState.Normal || !IsVisible || Bounds.Width <= 0 || Bounds.Height <= 0) return;
        Settings.WindowState.Size = new System.Drawing.Size((int)Math.Round(Bounds.Width), (int)Math.Round(Bounds.Height));
        Settings.WindowState.Location = new System.Drawing.Point(Position.X, Position.Y);
    }

    public void SaveSettings()
    {
        if (Settings.RememberWindowState)
        {
            SaveNormalWindowPlacement();
            if (WindowState is WindowState.Normal or WindowState.Maximized) Settings.WindowState.IsMaximized = WindowState == WindowState.Maximized;
        }
        _dock.SaveLayout();
        if (_dock.IsVisible && _dockColumn.ActualWidth > 0) Settings.RightDockWidth = _dockColumn.ActualWidth;
        if (_timeline.IsVisible && _timelineRow.ActualHeight > 0) Settings.TimelineHeight = _timelineRow.ActualHeight;
        Settings.Save(Services.SettingsFilePath);
    }

    // ------------------------------------------------------------------ title bar

    /// <summary>Builds the custom title bar: icon and menu on the left, sprite name centered, caption buttons on the right.</summary>
    private Border CreateTitleBar()
    {
        ExtendClientAreaToDecorationsHint = true;
        ExtendClientAreaTitleBarHeightHint = TitleBarHeight;
        WindowDecorations = WindowDecorations.BorderOnly;

        Image icon = new() { Width = 16, Height = 16, Margin = new Thickness(10, 0, 4, 0), Source = LoadTitleBarIcon(), IsHitTestVisible = false, VerticalAlignment = VerticalAlignment.Center };
        RenderOptions.SetBitmapInterpolationMode(icon, BitmapInterpolationMode.HighQuality);
        _menuHost = new Border { Child = CreateMenu(), VerticalAlignment = VerticalAlignment.Center };
        StackPanel left = new() { Orientation = Orientation.Horizontal };
        left.Children.Add(icon);
        left.Children.Add(_menuHost);

        _titleText = new TextBlock { TextTrimming = TextTrimming.CharacterEllipsis, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(12, 0), Text = Strings.Window_Name, Classes = { "dim" } };

        Button minimize = CreateCaptionButton(Strings.TitleBar_Minimize, "M0 5.5 H10", WindowDecorationsElementRole.MinimizeButton);
        minimize.Click += (_, _) => WindowState = WindowState.Minimized;
        Button maximize = CreateCaptionButton(Strings.TitleBar_Maximize, MaximizeGlyph, WindowDecorationsElementRole.MaximizeButton);
        _maximizeButton = maximize;
        _maximizeGlyph = (Shape)maximize.Content!;
        maximize.Click += (_, _) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        Button close = CreateCaptionButton(Strings.Command_Close, "M0 0 L10 10 M10 0 L0 10", WindowDecorationsElementRole.CloseButton);
        close.Classes.Add("close");
        close.Click += (_, _) => Close();
        StackPanel right = new() { Orientation = Orientation.Horizontal };
        right.Children.Add(minimize);
        right.Children.Add(maximize);
        right.Children.Add(close);

        TitleBarPanel content = new();
        content.Children.Add(left);
        content.Children.Add(_titleText);
        content.Children.Add(right);
        Border bar = new() { Height = TitleBarHeight, Child = content, Background = Ui.Chrome };
        WindowDecorationProperties.SetElementRole(bar, WindowDecorationsElementRole.TitleBar);

        PropertyChanged += (_, e) =>
        {
            if (e.Property == WindowStateProperty) UpdateMaximizeGlyph();
        };
        UpdateMaximizeGlyph();
        return bar;
    }

    private void UpdateMaximizeGlyph()
    {
        bool maximized = WindowState == WindowState.Maximized;
        _maximizeGlyph.Data = Geometry.Parse(maximized ? RestoreGlyph : MaximizeGlyph);
        ToolTip.SetTip(_maximizeButton, maximized ? Strings.TitleBar_Restore : Strings.TitleBar_Maximize);
    }

    private static Button CreateCaptionButton(string tip, string glyph, WindowDecorationsElementRole role)
    {
        // Stroke comes from the caption style so the close button can switch it on hover.
        Shape path = new() { Data = Geometry.Parse(glyph), Width = 10, Height = 10, StrokeThickness = 1, Stretch = Stretch.None };
        Button button = new() { Classes = { "caption" }, Content = path, Focusable = false };
        ToolTip.SetTip(button, tip);
        WindowDecorationProperties.SetElementRole(button, role);
        return button;
    }

    private Menu CreateMenu()
    {
        Menu menu = MenuBuilder.Build(this);
        menu.Background = Brushes.Transparent;
        // The menu sits inside the title bar, so it must take its own input instead of dragging the window.
        WindowDecorationProperties.SetElementRole(menu, WindowDecorationsElementRole.User);
        return menu;
    }

    public void RebuildMenu() => _menuHost.Child = CreateMenu();

    private static Bitmap? LoadTitleBarIcon()
    {
        try
        {
            using SKBitmap bitmap = ShareX.HelpersLib.SkiaImageHelpers.ByteArrayToBitmap(ShareX.HelpersLib.ShareXResources.IconBytes);
            using SKData data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
            using Stream stream = data.AsStream();
            return new Bitmap(stream);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>Lays out the left, center and right title bar parts, keeping the center part centered on the window when it fits.</summary>
    private sealed class TitleBarPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            double width = 0, height = 0;
            foreach (Control child in Children)
            {
                child.Measure(new Size(double.PositiveInfinity, availableSize.Height));
                width += child.DesiredSize.Width;
                height = Math.Max(height, child.DesiredSize.Height);
            }
            return new Size(double.IsInfinity(availableSize.Width) ? width : availableSize.Width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Control left = Children[0], center = Children[1], right = Children[2];
            double leftWidth = Math.Min(left.DesiredSize.Width, finalSize.Width);
            double rightWidth = Math.Min(right.DesiredSize.Width, finalSize.Width - leftWidth);
            left.Arrange(new Rect(0, 0, leftWidth, finalSize.Height));
            right.Arrange(new Rect(finalSize.Width - rightWidth, 0, rightWidth, finalSize.Height));
            double space = Math.Max(0, finalSize.Width - leftWidth - rightWidth);
            double centerWidth = Math.Min(center.DesiredSize.Width, space);
            double x = Math.Clamp((finalSize.Width - centerWidth) / 2, leftWidth, leftWidth + space - centerWidth);
            center.Arrange(new Rect(x, 0, centerWidth, finalSize.Height));
            return finalSize;
        }
    }
}
