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
using Avalonia.Layout;
using ShareX.PhotoEditor.UI.Dialogs;
using ShareX.PixelEditor.UI.Commands;

namespace ShareX.PixelEditor.UI.Dialogs;

public static class NewSpriteDialog
{
    private static readonly (int W, int H)[] Sizes = [(16, 16), (32, 32), (48, 48), (64, 64), (128, 128), (256, 256), (320, 180), (640, 360)];

    public static async Task ShowAsync(PixelEditorWindow w)
    {
        EditorSettings settings = w.Settings;
        Form form = new(w);
        NumericUpDown? widthBox = null, heightBox = null;
        Func<int> width = form.Number(Strings.Options_Width, 1, Sprite.MaxSize, settings.NewWidth, 1, b => widthBox = b);
        Func<int> height = form.Number(Strings.Options_Height, 1, Sprite.MaxSize, settings.NewHeight, 1, b => heightBox = b);
        WrapPanel presets = new() { Orientation = Orientation.Horizontal };
        foreach ((int pw, int ph) in Sizes)
        {
            Button button = new() { Content = $"{pw}×{ph}", Margin = new Thickness(0, 0, 4, 4), Padding = new Thickness(7, 2) };
            button.Click += (_, _) =>
            {
                widthBox!.Value = pw;
                heightBox!.Value = ph;
            };
            presets.Children.Add(button);
        }
        form.AddRow(Strings.Dialog_Presets, presets);
        Func<bool> transparent = form.Check(Strings.Dialog_TransparentBackground, settings.NewTransparent);
        List<(string, string)> palettes = Palettes.Presets.Select(p => ($"{p.Name} ({p.Colors.Length})", p.Name)).ToList();
        Func<string> palette = form.Combo(Strings.Panel_Palette, palettes, palettes.Any(p => p.Item2 == settings.NewPalette) ? settings.NewPalette : palettes[0].Item2);
        if (!await form.ShowAsync(Strings.Dialog_NewSprite, 400, Strings.Dialog_Create)) return;
        settings.NewWidth = width();
        settings.NewHeight = height();
        settings.NewTransparent = transparent();
        settings.NewPalette = palette();
        int number = w.Views.Count(v => v.Sprite.FilePath == null) + 1;
        Sprite sprite = Sprite.Create(width(), height(), string.Format(Strings.Sprite_Untitled, number), UniqueName(), transparent() ? 0 : w.BackgroundColor);
        sprite.Palette = [.. Palettes.Presets.First(p => p.Name == palette()).Colors];
        w.OpenSprite(sprite, Strings.History_New);

        static string UniqueName() => $"{Strings.Layer_DefaultName} 1";
    }
}

public static class SpriteDialogs
{
    public static async Task SpriteSizeAsync(PixelEditorWindow w)
    {
        Sprite sprite = w.Sprite!;
        Form form = new(w);
        NumericUpDown? widthBox = null, heightBox = null, percentBox = null;
        bool syncing = false;
        Func<int> width = form.Number(Strings.Options_Width, 1, Sprite.MaxSize, sprite.Width, 1, b => widthBox = b);
        Func<int> height = form.Number(Strings.Options_Height, 1, Sprite.MaxSize, sprite.Height, 1, b => heightBox = b);
        Func<int> percent = form.Number(Strings.Dialog_Percent, 1, 6400, 100, 100, b => percentBox = b);
        Func<bool> keep = form.Check(Strings.Dialog_KeepProportions, true);
        Func<ResizeMethod> method = form.Combo(Strings.Dialog_Method, EnumNames.ResizeMethods, ResizeMethod.Nearest);
        form.Note(Strings.Dialog_ResizeNote);
        widthBox!.ValueChanged += (_, _) =>
        {
            if (syncing || !keep()) return;
            syncing = true;
            heightBox!.Value = Math.Clamp((int)Math.Round(width() * (double)sprite.Height / sprite.Width), 1, Sprite.MaxSize);
            percentBox!.Value = Math.Clamp((int)Math.Round(width() * 100.0 / sprite.Width), 1, 6400);
            syncing = false;
        };
        heightBox!.ValueChanged += (_, _) =>
        {
            if (syncing || !keep()) return;
            syncing = true;
            widthBox.Value = Math.Clamp((int)Math.Round(height() * (double)sprite.Width / sprite.Height), 1, Sprite.MaxSize);
            percentBox!.Value = Math.Clamp((int)Math.Round(height() * 100.0 / sprite.Height), 1, 6400);
            syncing = false;
        };
        percentBox!.ValueChanged += (_, _) =>
        {
            if (syncing) return;
            syncing = true;
            widthBox.Value = Math.Clamp((int)Math.Round(sprite.Width * percent() / 100.0), 1, Sprite.MaxSize);
            heightBox.Value = Math.Clamp((int)Math.Round(sprite.Height * percent() / 100.0), 1, Sprite.MaxSize);
            syncing = false;
        };
        if (!await form.ShowAsync(Strings.Command_SpriteSize.Replace("...", string.Empty), 400) || !ReferenceEquals(w.Sprite, sprite)) return;
        if (width() == sprite.Width && height() == sprite.Height) return;
        int newWidth = width(), newHeight = height();
        ResizeMethod chosen = method();
        await w.RunBusyAsync(Strings.Status_Resizing, async _ =>
        {
            await Task.Run(() => SpriteOps.Resize(sprite, newWidth, newHeight, chosen));
            w.Commit(Strings.Command_SpriteSize.Replace("...", string.Empty));
            w.View!.NeedsFit = true;
        });
    }

    public static async Task CanvasSizeAsync(PixelEditorWindow w)
    {
        Sprite sprite = w.Sprite!;
        Form form = new(w);
        Func<int> width = form.Number(Strings.Options_Width, 1, Sprite.MaxSize, sprite.Width);
        Func<int> height = form.Number(Strings.Options_Height, 1, Sprite.MaxSize, sprite.Height);
        // Anchor: which part of the old canvas stays in place.
        int anchorX = 1, anchorY = 1;
        Grid anchors = new() { ColumnDefinitions = new ColumnDefinitions("Auto,Auto,Auto"), RowDefinitions = new RowDefinitions("Auto,Auto,Auto"), HorizontalAlignment = HorizontalAlignment.Left };
        List<Button> buttons = [];
        void UpdateAnchors()
        {
            for (int i = 0; i < buttons.Count; i++) buttons[i].Classes.Set("checked", i % 3 == anchorX && i / 3 == anchorY);
        }
        for (int i = 0; i < 9; i++)
        {
            int ax = i % 3, ay = i / 3;
            Button button = new() { Classes = { "icon" }, Width = 24, Height = 24, Margin = new Thickness(1), Content = new Border { Width = 8, Height = 8, Background = Ui.TextBrush } };
            button.Click += (_, _) =>
            {
                anchorX = ax;
                anchorY = ay;
                UpdateAnchors();
            };
            Grid.SetColumn(button, ax);
            Grid.SetRow(button, ay);
            buttons.Add(button);
            anchors.Children.Add(button);
        }
        UpdateAnchors();
        form.AddRow(Strings.Dialog_Anchor, anchors);
        if (!await form.ShowAsync(Strings.Command_CanvasSize.Replace("...", string.Empty), 360) || !ReferenceEquals(w.Sprite, sprite)) return;
        if (width() == sprite.Width && height() == sprite.Height) return;
        int dx = (width() - sprite.Width) * anchorX / 2, dy = (height() - sprite.Height) * anchorY / 2;
        SpriteOps.ResizeCanvas(sprite, width(), height(), dx, dy);
        w.Commit(Strings.Command_CanvasSize.Replace("...", string.Empty));
        w.View!.NeedsFit = true;
    }

    public static async Task ReduceColorsAsync(PixelEditorWindow w)
    {
        Sprite sprite = w.Sprite!;
        Form form = new(w);
        Func<int> count = form.Number(Strings.Dialog_ColorCount, 2, 256, Math.Min(16, Math.Max(2, sprite.Palette.Count)));
        Func<RemapDither> dither = form.Combo(Strings.Options_Dither, EnumNames.RemapDithers, RemapDither.None);
        Func<bool> binary = form.Check(Strings.Dialog_BinaryAlpha, true);
        form.Note(Strings.Dialog_ReduceColors_Note);
        if (!await form.ShowAsync(Strings.Command_ReduceColors.Replace("...", string.Empty), 400) || !ReferenceEquals(w.Sprite, sprite)) return;
        List<PixelBuffer> buffers = sprite.PixelLayers.SelectMany(l => l.Cels).Where(c => c != null).Select(c => c!.Buffer).Distinct().ToList();
        int colors = count();
        RemapDither mode = dither();
        bool binaryAlpha = binary();
        await w.RunBusyAsync(Strings.Status_ReducingColors, async _ =>
        {
            await Task.Run(() =>
            {
                List<uint> palette = Palettes.FromImages(buffers, colors);
                sprite.ReplaceContent(sprite.Width, sprite.Height, b => Palettes.Remap(b, palette, mode, binaryAlpha), s => s);
                sprite.Palette = palette;
            });
            w.Commit(Strings.Command_ReduceColors.Replace("...", string.Empty));
        });
    }

    public static async Task RemapToPaletteAsync(PixelEditorWindow w)
    {
        Sprite sprite = w.Sprite!;
        Form form = new(w);
        Func<RemapDither> dither = form.Combo(Strings.Options_Dither, EnumNames.RemapDithers, RemapDither.None);
        Func<bool> binary = form.Check(Strings.Dialog_BinaryAlpha, false);
        form.Note(Strings.Dialog_Remap_Note);
        if (!await form.ShowAsync(Strings.Palette_RemapSprite.Replace("...", string.Empty), 400) || !ReferenceEquals(w.Sprite, sprite)) return;
        List<uint> palette = [.. sprite.Palette];
        RemapDither mode = dither();
        bool binaryAlpha = binary();
        await w.RunBusyAsync(Strings.Status_ReducingColors, async _ =>
        {
            await Task.Run(() => sprite.ReplaceContent(sprite.Width, sprite.Height, b => Palettes.Remap(b, palette, mode, binaryAlpha), s => s));
            w.Commit(Strings.Palette_RemapSprite.Replace("...", string.Empty));
        });
    }

    public static async Task LayerPropertiesAsync(PixelEditorWindow w)
    {
        Sprite sprite = w.Sprite!;
        Layer layer = sprite.ActiveLayer!;
        int id = layer.Id;
        Form form = new(w);
        Func<string> name = form.Text(Strings.Dialog_Name, layer.Name);
        Func<int> opacity = form.Slider(Strings.Options_Opacity, 0, 100, (int)Math.Round(layer.Opacity * 100 / 255.0));
        Func<BlendMode> blend = form.Combo(Strings.Dialog_BlendMode, EnumNames.BlendModes, layer.Blend);
        Func<bool> visible = form.Check(Strings.Command_LayerVisible, layer.Visible), locked = form.Check(Strings.Command_LayerLock, layer.Locked);
        if (!await form.ShowAsync(Strings.Command_LayerProperties.Replace("...", string.Empty), 380) || !ReferenceEquals(w.Sprite, sprite)) return;
        // The layer object is replaced when history is restored, so look it up again.
        if (sprite.FindLayer(id) is not Layer target) return;
        target.Name = string.IsNullOrWhiteSpace(name()) ? target.Name : name().Trim();
        target.Opacity = (int)Math.Round(opacity() * 255 / 100.0);
        target.Blend = blend();
        target.Visible = visible();
        target.Locked = locked();
        sprite.Touch();
        w.Commit(Strings.Command_LayerProperties.Replace("...", string.Empty));
    }

    public static async Task FramePropertiesAsync(PixelEditorWindow w)
    {
        Sprite sprite = w.Sprite!;
        int from = sprite.RangeFrom, to = sprite.RangeTo;
        Form form = new(w);
        form.Note(from == to ? string.Format(Strings.Dialog_FrameNumber, from + 1) : string.Format(Strings.Dialog_FrameRange, from + 1, to + 1));
        Func<int> duration = form.Number(Strings.Dialog_DurationMs, 1, 65535, sprite.Frames[sprite.ActiveFrame].Duration, 10);
        Func<bool> all = form.Check(Strings.Dialog_AllFrames, false);
        if (!await form.ShowAsync(Strings.Command_FrameProperties.Replace("...", string.Empty), 340) || !ReferenceEquals(w.Sprite, sprite)) return;
        EditorCommands.SetFrameDuration(w, all() ? 0 : from, all() ? sprite.Frames.Count - 1 : to, duration());
    }

    public static async Task CelPropertiesAsync(PixelEditorWindow w)
    {
        Sprite sprite = w.Sprite!;
        Cel? cel = sprite.GetCel((PixelLayer)sprite.ActiveLayer!, sprite.ActiveFrame);
        if (cel == null) return;
        Form form = new(w);
        Func<int> opacity = form.Slider(Strings.Options_Opacity, 0, 100, (int)Math.Round(cel.Opacity * 100 / 255.0));
        form.Changed += () =>
        {
            cel.Opacity = (int)Math.Round(opacity() * 255 / 100.0);
            sprite.Touch();
            w.InvalidateCanvas();
        };
        if (await form.ShowAsync(Strings.Command_CelProperties.Replace("...", string.Empty), 360)) w.Commit(Strings.Command_CelProperties.Replace("...", string.Empty));
        else
        {
            w.RevertEdit();
            w.RefreshPanels();
        }
    }

    private static readonly uint[] TagColors = [0xFF4A90D9, 0xFF3FB950, 0xFFE5534B, 0xFFD29922, 0xFFA371F7, 0xFF39C5CF, 0xFFDB61A2, 0xFF8B949E];

    /// <summary>Creates a tag over the selected frames, or edits an existing one.</summary>
    public static async Task TagAsync(PixelEditorWindow w, Tag? existing)
    {
        Sprite sprite = w.Sprite!;
        int index = existing != null ? sprite.Tags.IndexOf(existing) : -1;
        Form form = new(w);
        Func<string> name = form.Text(Strings.Dialog_Name, existing?.Name ?? string.Format(Strings.Tag_DefaultName, sprite.Tags.Count + 1));
        Func<int> from = form.Number(Strings.Dialog_FromFrame, 1, sprite.Frames.Count, (existing?.From ?? sprite.RangeFrom) + 1);
        Func<int> to = form.Number(Strings.Dialog_ToFrame, 1, sprite.Frames.Count, (existing?.To ?? sprite.RangeTo) + 1);
        Func<TagDirection> direction = form.Combo(Strings.Dialog_Direction, EnumNames.Directions, existing?.Direction ?? TagDirection.Forward);
        Func<int> repeat = form.Number(Strings.Dialog_Repeat, 1, 99, Math.Max(1, existing?.Repeat ?? 1));
        Func<uint> color = form.Color(Strings.Dialog_Color, existing?.Color ?? TagColors[sprite.Tags.Count % TagColors.Length]);
        string title = existing != null ? Strings.Command_TagProperties.Replace("...", string.Empty) : Strings.Command_NewTag.Replace("...", string.Empty);
        if (!await form.ShowAsync(title, 380) || !ReferenceEquals(w.Sprite, sprite)) return;
        Tag tag = index >= 0 && index < sprite.Tags.Count ? sprite.Tags[index] : new Tag();
        tag.Name = string.IsNullOrWhiteSpace(name()) ? tag.Name : name().Trim();
        tag.From = Math.Min(from(), to()) - 1;
        tag.To = Math.Max(from(), to()) - 1;
        tag.Direction = direction();
        tag.Repeat = repeat();
        tag.Color = Argb.Opaque(color());
        if (!sprite.Tags.Contains(tag)) sprite.Tags.Add(tag);
        sprite.Tags.Sort((a, b) => a.From != b.From ? a.From.CompareTo(b.From) : b.To.CompareTo(a.To));
        w.Commit(title);
    }

    public static async Task OnionSettingsAsync(PixelEditorWindow w)
    {
        EditorSettings s = w.Settings;
        (bool, int, int, int, bool) before = (s.OnionSkin, s.OnionPrevious, s.OnionNext, s.OnionOpacity, s.OnionTint);
        Form form = new(w);
        Func<bool> enabled = form.Check(Strings.Command_OnionSkin, s.OnionSkin);
        Func<int> previous = form.Number(Strings.Dialog_OnionPrevious, 0, 5, s.OnionPrevious), next = form.Number(Strings.Dialog_OnionNext, 0, 5, s.OnionNext);
        Func<int> opacity = form.Slider(Strings.Options_Opacity, 5, 100, s.OnionOpacity);
        Func<bool> tint = form.Check(Strings.Dialog_OnionTint, s.OnionTint);
        void Apply((bool, int, int, int, bool) v)
        {
            (s.OnionSkin, s.OnionPrevious, s.OnionNext, s.OnionOpacity, s.OnionTint) = v;
            w.InvalidateCanvas();
        }
        form.Changed += () => Apply((enabled(), previous(), next(), opacity(), tint()));
        if (!await form.ShowAsync(Strings.Command_OnionSettings.Replace("...", string.Empty), 380)) Apply(before);
        w.RefreshTimeline();
        w.RefreshOptions();
    }

    public static async Task GridSettingsAsync(PixelEditorWindow w)
    {
        EditorSettings s = w.Settings;
        (bool, int, int, string) before = (s.ShowGrid, s.GridWidth, s.GridHeight, s.GridColor);
        Form form = new(w);
        Func<bool> show = form.Check(Strings.Command_Grid, true);
        Func<int> width = form.Number(Strings.Options_Width, 1, 1024, s.GridWidth), height = form.Number(Strings.Options_Height, 1, 1024, s.GridHeight);
        Func<uint> color = form.Color(Strings.Dialog_Color, EditorSettings.ParseColor(s.GridColor, 0xFF00A0FF));
        void Apply((bool, int, int, string) v)
        {
            (s.ShowGrid, s.GridWidth, s.GridHeight, s.GridColor) = v;
            w.Canvas.InvalidateVisual();
        }
        form.Changed += () => Apply((show(), width(), height(), "#" + Argb.ToHex(color())));
        Apply((true, s.GridWidth, s.GridHeight, s.GridColor));
        if (!await form.ShowAsync(Strings.Command_GridSettings.Replace("...", string.Empty), 340)) Apply(before);
        w.RefreshOptions();
    }

    public static async Task SymmetryAxisAsync(PixelEditorWindow w)
    {
        SpriteView view = w.View!;
        (int, int) before = (view.AxisX2, view.AxisY2);
        Form form = new(w);
        form.Note(Strings.Dialog_SymmetryAxis_Note);
        // Axes are edited in half pixels so they can run through a pixel column or between two.
        Func<int> x = form.Number(Strings.Dialog_AxisX, 0, view.Sprite.Width * 2, view.AxisX2), y = form.Number(Strings.Dialog_AxisY, 0, view.Sprite.Height * 2, view.AxisY2);
        form.Changed += () =>
        {
            (view.AxisX2, view.AxisY2) = (x(), y());
            w.Canvas.InvalidateVisual();
        };
        if (!await form.ShowAsync(Strings.Command_SymmetryAxis.Replace("...", string.Empty), 360)) (view.AxisX2, view.AxisY2) = before;
        w.Canvas.InvalidateVisual();
    }
}

public static class PreferencesDialog
{
    private static readonly (string Name, string Color1, string Color2)[] Checkers =
    [
        ("Medium", "#8C8C8C", "#A6A6A6"), ("Light", "#FFFFFF", "#CCCCCC"), ("Dark", "#3A3A3A", "#4A4A4A"), ("Black", "#000000", "#1A1A1A"), ("White", "#FFFFFF", "#FFFFFF")
    ];

    public static async Task ShowAsync(PixelEditorWindow w)
    {
        EditorSettings s = w.Settings;
        Form form = new(w);
        string[] names = [Strings.Checker_Medium, Strings.Checker_Light, Strings.Checker_Dark, Strings.Checker_Black, Strings.Checker_White];
        int current = Math.Max(0, Array.FindIndex(Checkers, c => c.Color1 == s.CheckerColor1 && c.Color2 == s.CheckerColor2));
        Func<int> checker = form.Combo(Strings.Preferences_Checker, names.Select((n, i) => (n, i)), current);
        Func<int> checkerSize = form.Number(Strings.Preferences_CheckerSize, 1, 64, s.CheckerSize);
        Func<int> gridZoom = form.Number(Strings.Preferences_PixelGridZoom, 2, 64, s.PixelGridZoom);
        Func<int> history = form.Number(Strings.Preferences_HistoryStates, 10, 1000, s.HistoryStates, 10);
        Func<int> cell = form.Number(Strings.Preferences_TimelineCell, 16, 48, s.TimelineCellSize);
        Func<bool> preview = form.Check(Strings.Command_BrushPreview, s.ShowBrushPreview);
        Func<int> autoSave = form.Number(Strings.Preferences_AutoSave, 0, 60, s.AutoSaveMinutes);
        Func<bool> remember = form.Check(Strings.Preferences_RememberWindow, s.RememberWindowState);
        if (!await form.ShowAsync(Strings.Command_Preferences.Replace("...", string.Empty), 420)) return;
        (s.CheckerColor1, s.CheckerColor2) = (Checkers[checker()].Color1, Checkers[checker()].Color2);
        s.CheckerSize = checkerSize();
        s.PixelGridZoom = gridZoom();
        s.HistoryStates = history();
        s.TimelineCellSize = cell();
        s.ShowBrushPreview = preview();
        s.RememberWindowState = remember();
        s.AutoSaveMinutes = autoSave();
        w.RestartAutoSave();
        foreach (SpriteView view in w.Views) view.Sprite.History.MaxStates = s.HistoryStates;
        w.SaveSettings();
        w.RefreshPanels();
        w.InvalidateCanvas();
    }
}

/// <summary>Lists every command and tool with its shortcut; shortcuts of commands can be changed.</summary>
public static class ShortcutsDialog
{
    public static async Task ShowAsync(PixelEditorWindow w)
    {
        Dictionary<string, TextBox> boxes = [];
        StackPanel list = new() { Spacing = 2, Margin = new Thickness(0, 0, 8, 0) };
        void Header(string text) => list.Children.Add(new TextBlock { Text = text, Classes = { "header" }, Margin = new Thickness(0, 8, 0, 2) });
        void Row(string text, Control value)
        {
            Grid row = new() { ColumnDefinitions = new ColumnDefinitions("*,150"), ColumnSpacing = 8 };
            row.Children.Add(new TextBlock { Text = text, VerticalAlignment = VerticalAlignment.Center, TextTrimming = Avalonia.Media.TextTrimming.CharacterEllipsis });
            Grid.SetColumn(value, 1);
            row.Children.Add(value);
            list.Children.Add(row);
        }
        Header(Strings.Shortcuts_Tools);
        foreach (Tools.ToolBase tool in w.Tools.All) Row(tool.Name, new TextBlock { Text = tool.Shortcut.ToString(), Classes = { "dim" }, VerticalAlignment = VerticalAlignment.Center });
        foreach ((string text, string keys) in new[]
        {
            (Strings.Shortcuts_Pan, "Space, " + Strings.Shortcuts_MiddleButton), (Strings.Shortcuts_Zoom, Strings.Shortcuts_Wheel), (Strings.Shortcuts_PickColor, "Alt"),
            (Strings.Shortcuts_StraightLine, "Shift"), (Strings.Shortcuts_BackgroundColor, Strings.Shortcuts_RightButton), (Strings.Shortcuts_HidePanels, "Tab")
        })
        {
            Row(text, new TextBlock { Text = keys, Classes = { "dim" }, VerticalAlignment = VerticalAlignment.Center });
        }
        Header(Strings.Shortcuts_Commands);
        foreach (EditorCommand command in CommandRegistry.Commands.Values.OrderBy(c => c.Id.Split('.')[0]).ThenBy(c => c.Text))
        {
            TextBox box = new() { Text = CommandRegistry.GetGesture(command.Id, w.Settings) ?? string.Empty };
            boxes[command.Id] = box;
            Row(command.Text.Replace("...", string.Empty).Replace("_", string.Empty), box);
        }
        Button reset = new() { Classes = { "secondary" }, Content = Strings.Shortcuts_Reset };
        reset.Click += (_, _) =>
        {
            foreach ((string id, TextBox box) in boxes) box.Text = CommandRegistry.Get(id)?.DefaultGesture ?? string.Empty;
        };
        DockPanel content = new() { Height = 520 };
        TextBlock note = new() { Text = Strings.Shortcuts_Note, Classes = { "dim" }, TextWrapping = Avalonia.Media.TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 8) };
        DockPanel.SetDock(note, Dock.Top);
        content.Children.Add(note);
        content.Children.Add(new ScrollViewer { Content = list });
        if (!await DialogBuilder.ShowAsync(w, Strings.Command_Shortcuts.Replace("...", string.Empty), content, 520, null, null, [reset])) return;
        foreach ((string id, TextBox box) in boxes)
        {
            string text = box.Text?.Trim() ?? string.Empty, standard = CommandRegistry.Get(id)?.DefaultGesture ?? string.Empty;
            if (text == standard) w.Settings.Shortcuts.Remove(id);
            else if (text.Length == 0 || text.Split(',').All(part => CommandRegistry.ParseGesture(part) != null)) w.Settings.Shortcuts[id] = text;
        }
        w.SaveSettings();
        w.RebuildMenu();
        w.RefreshPanels();
    }
}

/// <summary>Name, color, bounds, 9-slice center and pivot of a slice.</summary>
public static class SliceDialog
{
    public static async Task<bool> ShowAsync(PixelEditorWindow w, Slice slice)
    {
        Sprite? sprite = w.Sprite;
        if (sprite == null) return false;
        Form form = new(w);
        SKRectI b = slice.Bounds;
        Func<string> name = form.Text(Strings.Dialog_Name, slice.Name);
        Func<uint> color = form.Color(Strings.Dialog_Color, slice.Color);
        Func<int> x = form.Number("X", -Sprite.MaxSize, Sprite.MaxSize, b.Left), y = form.Number("Y", -Sprite.MaxSize, Sprite.MaxSize, b.Top);
        Func<int> width = form.Number(Strings.Options_Width, 1, Sprite.MaxSize, b.Width), height = form.Number(Strings.Options_Height, 1, Sprite.MaxSize, b.Height);
        SKRectI center = slice.Center ?? SKRectI.Create(Math.Max(0, b.Width / 3), Math.Max(0, b.Height / 3), Math.Max(1, b.Width - b.Width / 3 * 2), Math.Max(1, b.Height - b.Height / 3 * 2));
        Func<bool> nine = form.Check(Strings.Slice_NineSlice, slice.Center != null);
        Func<int> cx = form.Number(Strings.Slice_CenterX, 0, Sprite.MaxSize, center.Left), cy = form.Number(Strings.Slice_CenterY, 0, Sprite.MaxSize, center.Top);
        Func<int> cw = form.Number(Strings.Slice_CenterWidth, 1, Sprite.MaxSize, center.Width), ch = form.Number(Strings.Slice_CenterHeight, 1, Sprite.MaxSize, center.Height);
        SKPointI pivot = slice.Pivot ?? new SKPointI(b.Width / 2, b.Height);
        Func<bool> hasPivot = form.Check(Strings.Slice_Pivot, slice.Pivot != null);
        Func<int> px = form.Number(Strings.Slice_PivotX, -Sprite.MaxSize, Sprite.MaxSize, pivot.X), py = form.Number(Strings.Slice_PivotY, -Sprite.MaxSize, Sprite.MaxSize, pivot.Y);
        form.Note(Strings.Slice_Note);
        if (!await form.ShowAsync(Strings.Slice_Properties, 380) || !ReferenceEquals(w.Sprite, sprite) || !sprite.Slices.Contains(slice)) return false;
        if (!string.IsNullOrWhiteSpace(name())) slice.Name = name().Trim();
        slice.Color = Argb.Opaque(color());
        slice.Bounds = SKRectI.Create(x(), y(), width(), height());
        slice.Center = nine() ? SKRectI.Create(Math.Min(cx(), width() - 1), Math.Min(cy(), height() - 1), Math.Clamp(cw(), 1, width() - Math.Min(cx(), width() - 1)), Math.Clamp(ch(), 1, height() - Math.Min(cy(), height() - 1))) : null;
        slice.Pivot = hasPivot() ? new SKPointI(px(), py()) : null;
        w.Commit(Strings.History_RenameSlice);
        return true;
    }
}