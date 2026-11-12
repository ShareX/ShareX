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

using ShareX.PixelEditor.UI.Dialogs;

namespace ShareX.PixelEditor.UI.Commands;

public static partial class EditorCommands
{
    // ------------------------------------------------------------------ effects

    /// <summary>
    /// Shows an effect dialog with a live preview on the canvas. The form gets a scope selector; the effect is
    /// reapplied from the last history state whenever a value changes and recorded when the dialog is accepted.
    /// </summary>
    private static async Task RunEffect(PixelEditorWindow w, string title, Func<Form, Func<PixelBuffer, bool[]?, PixelBuffer>> build)
    {
        Sprite? sprite = w.Sprite;
        if (sprite == null) return;
        if (sprite.ActiveLayer is not PixelLayer)
        {
            w.SetStatus(Strings.Status_SelectPixelLayer);
            return;
        }
        string name = title.Replace("...", string.Empty);
        Form form = new(w);
        Func<PixelBuffer, bool[]?, PixelBuffer> effect = build(form);
        Func<CelScope> scope = form.Combo(Strings.Dialog_ApplyTo, EnumNames.Scopes, sprite.RangeTo > sprite.RangeFrom ? CelScope.SelectedFrames : CelScope.Cel);
        bool changed = false;
        void Preview()
        {
            sprite.History.Revert();
            changed = SpriteOps.Apply(sprite, scope(), effect);
            w.InvalidateCanvas();
        }
        form.Changed += Preview;
        Preview();
        bool ok = await form.ShowAsync(name, 400);
        if (ok && changed) w.Commit(name);
        else
        {
            w.RevertEdit();
            w.RefreshPanels();
        }
    }

    private static void RegisterEffects()
    {
        R("fx.invert", Strings.Command_Invert, "Ctrl+I", w => Instant(w, Strings.Command_Invert, Effects.Invert), HasPixelLayer);
        R("fx.grayscale", Strings.Command_Grayscale, "Ctrl+Shift+U", w => Instant(w, Strings.Command_Grayscale, Effects.Grayscale), HasPixelLayer);
        R("fx.hueSaturation", Strings.Command_HueSaturation, "Ctrl+U", w => _ = RunEffect(w, Strings.Command_HueSaturation, form =>
        {
            Func<int> hue = form.Slider(Strings.Dialog_Hue, -180, 180, 0), saturation = form.Slider(Strings.Dialog_Saturation, -100, 100, 0), lightness = form.Slider(Strings.Dialog_Lightness, -100, 100, 0);
            return (b, mask) => Effects.Map(b, mask, c => Effects.HueSaturation(c, hue(), saturation(), lightness()));
        }), HasPixelLayer);
        R("fx.brightnessContrast", Strings.Command_BrightnessContrast, null, w => _ = RunEffect(w, Strings.Command_BrightnessContrast, form =>
        {
            Func<int> brightness = form.Slider(Strings.Dialog_Brightness, -100, 100, 0), contrast = form.Slider(Strings.Dialog_Contrast, -100, 100, 0);
            return (b, mask) => Effects.Map(b, mask, c => Effects.BrightnessContrast(c, brightness(), contrast()));
        }), HasPixelLayer);
        R("fx.posterize", Strings.Command_Posterize, null, w => _ = RunEffect(w, Strings.Command_Posterize, form =>
        {
            Func<int> levels = form.Slider(Strings.Dialog_Levels, 2, 32, 4);
            return (b, mask) => Effects.Map(b, mask, c => Effects.Posterize(c, levels()));
        }), HasPixelLayer);
        R("fx.replaceColor", Strings.Command_ReplaceColor, null, w => _ = RunEffect(w, Strings.Command_ReplaceColor, form =>
        {
            Func<uint> from = form.Color(Strings.Dialog_From, w.Foreground), to = form.Color(Strings.Dialog_To, w.Background);
            Func<int> tolerance = form.Slider(Strings.Options_Tolerance, 0, 255, 0);
            return (b, mask) => Effects.ReplaceColor(b, mask, from(), to(), tolerance());
        }), HasPixelLayer);
        R("fx.outline", Strings.Command_Outline, null, w => _ = RunEffect(w, Strings.Command_Outline, form =>
        {
            Func<uint> color = form.Color(Strings.Dialog_Color, w.Foreground);
            Func<bool> inside = form.Check(Strings.Dialog_OutlineInside, false), corners = form.Check(Strings.Dialog_OutlineCorners, false);
            return (b, mask) => Effects.Outline(b, mask, color(), inside(), corners());
        }), HasPixelLayer);
        R("fx.dropShadow", Strings.Command_DropShadow, null, w => _ = RunEffect(w, Strings.Command_DropShadow, form =>
        {
            Func<uint> color = form.Color(Strings.Dialog_Color, 0x80000000);
            Func<int> dx = form.Number(Strings.Dialog_OffsetX, -64, 64, 1), dy = form.Number(Strings.Dialog_OffsetY, -64, 64, 1);
            return (b, mask) => Effects.DropShadow(b, mask, color(), dx(), dy());
        }), HasPixelLayer);
        R("fx.alphaThreshold", Strings.Command_AlphaThreshold, null, w => _ = RunEffect(w, Strings.Command_AlphaThreshold, form =>
        {
            form.Note(Strings.Dialog_AlphaThreshold_Note);
            Func<int> threshold = form.Slider(Strings.Dialog_Threshold, 1, 255, 128);
            return (b, mask) => Effects.AlphaThreshold(b, mask, threshold());
        }), HasPixelLayer);
    }

    /// <summary>Applies a per-color effect without a dialog: to the selected frames when a range is selected, else to the cel.</summary>
    private static void Instant(PixelEditorWindow w, string name, Func<uint, uint> map)
    {
        Sprite sprite = w.Sprite!;
        if (w.EditableLayer() == null) return;
        if (SpriteOps.Apply(sprite, sprite.RangeTo > sprite.RangeFrom ? CelScope.SelectedFrames : CelScope.Cel, (b, mask) => Effects.Map(b, mask, map))) w.Commit(name);
    }

    // ------------------------------------------------------------------ view

    private static void RegisterView()
    {
        void View(string id, string text, string? gesture, Action<PixelEditorWindow> action, Func<PixelEditorWindow, bool>? can = null, Func<PixelEditorWindow, bool>? isChecked = null)
        {
            R(id, text, gesture, w =>
            {
                action(w);
                w.InvalidateOverlay();
            }, can, isChecked, keepsFloating: true, stopsPlayback: false);
        }
        void Toggle(string id, string text, string? gesture, Func<EditorSettings, bool> get, Action<EditorSettings, bool> set)
        {
            View(id, text, gesture, w =>
            {
                set(w.Settings, !get(w.Settings));
                w.RefreshOptions();
            }, null, w => get(w.Settings));
        }
        View("view.zoomIn", Strings.Command_ZoomIn, "Ctrl+OemPlus,Ctrl+Add", w => w.View!.ZoomStep(true, w.View.ViewportCenter), HasSprite);
        View("view.zoomOut", Strings.Command_ZoomOut, "Ctrl+OemMinus,Ctrl+Subtract", w => w.View!.ZoomStep(false, w.View.ViewportCenter), HasSprite);
        View("view.fit", Strings.Command_FitOnScreen, "Ctrl+D0", w => w.View!.Fit(), HasSprite);
        View("view.actualSize", Strings.Command_ActualSize, "Ctrl+D1", w =>
        {
            w.View!.ZoomTo(1, w.View.ViewportCenter);
            w.View.Center();
        }, HasSprite);
        Toggle("view.pixelGrid", Strings.Command_PixelGrid, "Ctrl+Shift+OemQuotes", s => s.ShowPixelGrid, (s, v) => s.ShowPixelGrid = v);
        Toggle("view.grid", Strings.Command_Grid, "Ctrl+OemQuotes", s => s.ShowGrid, (s, v) => s.ShowGrid = v);
        Toggle("view.snapToGrid", Strings.Command_SnapToGrid, "Ctrl+Shift+OemSemicolon", s => s.SnapToGrid, (s, v) => s.SnapToGrid = v);
        Toggle("view.slices", Strings.Command_ShowSlices, null, s => s.ShowSlices, (s, v) => s.ShowSlices = v);
        Toggle("view.brushPreview", Strings.Command_BrushPreview, null, s => s.ShowBrushPreview, (s, v) => s.ShowBrushPreview = v);
        R("view.gridSettings", Strings.Command_GridSettings, null, w => _ = SpriteDialogs.GridSettingsAsync(w), keepsFloating: true, stopsPlayback: false);

        void Symmetry(PixelEditorWindow w, SymmetryMode axis)
        {
            bool horizontal = w.Settings.Symmetry is SymmetryMode.Horizontal or SymmetryMode.Both, vertical = w.Settings.Symmetry is SymmetryMode.Vertical or SymmetryMode.Both;
            if (axis == SymmetryMode.Horizontal) horizontal = !horizontal;
            else vertical = !vertical;
            w.Settings.Symmetry = horizontal && vertical ? SymmetryMode.Both : horizontal ? SymmetryMode.Horizontal : vertical ? SymmetryMode.Vertical : SymmetryMode.None;
            w.RefreshOptions();
        }
        View("view.symmetryHorizontal", Strings.Command_SymmetryHorizontal, null, w => Symmetry(w, SymmetryMode.Horizontal), null, w => w.Settings.Symmetry is SymmetryMode.Horizontal or SymmetryMode.Both);
        View("view.symmetryVertical", Strings.Command_SymmetryVertical, null, w => Symmetry(w, SymmetryMode.Vertical), null, w => w.Settings.Symmetry is SymmetryMode.Vertical or SymmetryMode.Both);
        View("view.symmetryReset", Strings.Command_SymmetryReset, null, w =>
        {
            w.View!.AxisX2 = w.Sprite!.Width;
            w.View.AxisY2 = w.Sprite.Height;
        }, HasSprite);
        R("view.symmetryAxis", Strings.Command_SymmetryAxis, null, w => _ = SpriteDialogs.SymmetryAxisAsync(w), HasSprite, keepsFloating: true, stopsPlayback: false);

        void Tiled(string id, string text, TiledMode mode)
        {
            View(id, text, null, w =>
            {
                w.Settings.Tiled = w.Settings.Tiled == mode ? TiledMode.None : mode;
                w.RefreshOptions();
            }, null, w => w.Settings.Tiled == mode);
        }
        Tiled("view.tiled", Strings.Command_TiledMode, TiledMode.Both);
        Tiled("view.tiledX", Strings.Command_TiledHorizontal, TiledMode.Horizontal);
        Tiled("view.tiledY", Strings.Command_TiledVertical, TiledMode.Vertical);

        void Layout(string id, string text, string? gesture, Func<EditorSettings, bool> get, Action<EditorSettings, bool> set)
        {
            View(id, text, gesture, w =>
            {
                set(w.Settings, !get(w.Settings));
                w.ApplyVisibility();
            }, null, w => get(w.Settings));
        }
        Layout("view.timeline", Strings.Command_ShowTimeline, "Ctrl+Shift+T", s => s.TimelineVisible, (s, v) => s.TimelineVisible = v);
        Layout("view.toolbar", Strings.Command_ShowToolbar, null, s => s.ToolbarVisible, (s, v) => s.ToolbarVisible = v);
        Layout("view.options", Strings.Command_ShowOptions, null, s => s.OptionsVisible, (s, v) => s.OptionsVisible = v);
        View("view.toolbarColumns", Strings.Command_ToolbarColumns, null, w => w.ToggleToolbarColumns(), null, w => w.Settings.ToolbarTwoColumns);
        View("view.resetPanels", Strings.Command_ResetPanels, null, w =>
        {
            w.Settings.RightDockVisible = w.Settings.TimelineVisible = w.Settings.ToolbarVisible = w.Settings.OptionsVisible = true;
            w.Panels.ResetLayout();
            w.ApplyVisibility();
        });
        View("view.fullScreen", Strings.Command_FullScreen, "F11", w => w.ToggleFullScreen());
    }

    // ------------------------------------------------------------------ palette

    private static void RegisterPalette()
    {
        R("palette.addForeground", Strings.Palette_AddColor, null, w =>
        {
            Sprite sprite = w.Sprite!;
            if (sprite.Palette.Contains(w.Foreground)) return;
            sprite.Palette.Add(w.Foreground);
            w.Commit(Strings.Palette_AddColor);
        }, HasSprite);
        R("palette.removeForeground", Strings.Palette_RemoveColor, null, w =>
        {
            int index = w.Sprite!.Palette.IndexOf(w.Foreground);
            if (index >= 0) RemovePaletteColor(w, index);
        }, w => w.Sprite?.Palette.Contains(w.Foreground) == true);
        R("palette.load", Strings.Palette_Load, null, async w =>
        {
            IReadOnlyList<string> files = await w.PickOpenFilesAsync(Strings.Palette_Load.Replace("...", string.Empty), false, (Strings.File_Palettes, Palettes.FileExtensions));
            if (files.Count == 0) return;
            try
            {
                SetPalette(w, Palettes.Load(files[0]), Strings.History_LoadPalette);
            }
            catch (Exception ex)
            {
                w.ShowMessage(string.Format(Strings.Message_CouldNotOpen, Path.GetFileName(files[0]), ex.Message));
            }
        }, HasSprite);
        R("palette.save", Strings.Palette_Save, null, async w =>
        {
            Sprite sprite = w.Sprite!;
            string? path = await w.PickSaveFileAsync(Strings.Palette_Save.Replace("...", string.Empty), Path.GetFileNameWithoutExtension(sprite.DisplayName) + ".gpl",
                ("GIMP Palette", [".gpl"]), ("HEX", [".hex"]), ("JASC Palette", [".pal"]), ("PNG", [".png"]));
            if (path == null) return;
            try
            {
                Palettes.Save(path, sprite.Palette, Path.GetFileNameWithoutExtension(path));
            }
            catch (Exception ex)
            {
                w.ShowMessage(string.Format(Strings.Message_CouldNotSave, Path.GetFileName(path), ex.Message));
            }
        }, w => w.Sprite?.Palette.Count > 0);
        R("palette.fromSprite", Strings.Palette_FromSprite, null, w =>
        {
            Sprite sprite = w.Sprite!;
            List<uint> colors = Palettes.FromImages(sprite.PixelLayers.SelectMany(l => l.Cels).Where(c => c != null).Select(c => c!.Buffer).Distinct(), 256);
            if (colors.Count > 0) SetPalette(w, colors, Strings.Palette_FromSprite);
        }, HasSprite);
        R("palette.remapSprite", Strings.Palette_RemapSprite, null, w => _ = SpriteDialogs.RemapToPaletteAsync(w), w => w.Sprite?.Palette.Count > 0);
        void Sort(string id, string text, string by)
        {
            R(id, text, null, w => SetPalette(w, Palettes.Sort(w.Sprite!.Palette, by), text), w => w.Sprite?.Palette.Count > 1);
        }
        Sort("palette.sortHue", Strings.Palette_SortHue, "hue");
        Sort("palette.sortBrightness", Strings.Palette_SortBrightness, "brightness");
        Sort("palette.sortSaturation", Strings.Palette_SortSaturation, "saturation");
        Sort("palette.reverse", Strings.Palette_Reverse, "reverse");
        R("palette.ramp", Strings.Palette_Ramp, null, async w =>
        {
            Form form = new(w);
            form.Note(Strings.Palette_Ramp_Note);
            Func<int> steps = form.Number(Strings.Palette_Ramp_Steps, 3, 32, 5);
            if (!await form.ShowAsync(Strings.Palette_Ramp.Replace("...", string.Empty), 340) || w.Sprite == null) return;
            foreach (uint color in Palettes.Ramp(w.Foreground, w.Background, steps()))
                if (!w.Sprite.Palette.Contains(color)) w.Sprite.Palette.Add(color);
            w.Commit(Strings.Palette_Ramp.Replace("...", string.Empty));
        }, HasSprite);
        R("palette.removeUnused", Strings.Palette_RemoveUnused, null, w =>
        {
            Sprite sprite = w.Sprite!;
            HashSet<uint> used = [];
            foreach (PixelBuffer buffer in sprite.PixelLayers.SelectMany(l => l.Cels).Where(c => c != null).Select(c => c!.Buffer).Distinct())
                foreach (uint p in buffer.Pixels)
                    if (p >> 24 != 0) used.Add(p & 0xFFFFFF);
            List<uint> kept = sprite.Palette.Where(c => used.Contains(c & 0xFFFFFF)).ToList();
            if (kept.Count != sprite.Palette.Count) SetPalette(w, kept, Strings.Palette_RemoveUnused);
        }, w => w.Sprite?.Palette.Count > 0);
    }

    public static void SetPalette(PixelEditorWindow w, IEnumerable<uint> colors, string historyName)
    {
        if (w.Sprite == null) return;
        w.CommitFloating();
        w.Sprite.Palette = colors.Distinct().ToList();
        w.Commit(historyName.Replace("...", string.Empty));
    }

    public static void RemovePaletteColor(PixelEditorWindow w, int index)
    {
        if (w.Sprite == null || index < 0 || index >= w.Sprite.Palette.Count) return;
        w.CommitFloating();
        w.Sprite.Palette.RemoveAt(index);
        w.Commit(Strings.Palette_RemoveColor);
    }

    /// <summary>Changes a palette color and recolors the pixels that used it, like an indexed image.</summary>
    public static async void EditPaletteColor(PixelEditorWindow w, int index)
    {
        Sprite? sprite = w.Sprite;
        if (sprite == null || index < 0 || index >= sprite.Palette.Count) return;
        uint? picked = await Ui.PickColorAsync(w, sprite.Palette[index], Strings.Palette_EditColor.Replace("...", string.Empty));
        if (picked is uint color && ReferenceEquals(w.Sprite, sprite) && index < sprite.Palette.Count) ReplacePaletteColor(w, index, color);
    }

    public static void ReplacePaletteColor(PixelEditorWindow w, int index, uint color)
    {
        Sprite? sprite = w.Sprite;
        if (sprite == null || index < 0 || index >= sprite.Palette.Count || sprite.Palette[index] == color) return;
        w.CommitFloating();
        uint old = sprite.Palette[index];
        sprite.Palette[index] = color;
        HashSet<CelImage> done = [];
        foreach (PixelLayer layer in sprite.PixelLayers)
        {
            if (layer.IsEffectivelyLocked) continue;
            for (int f = 0; f < sprite.Frames.Count; f++)
            {
                Cel? cel = layer.Cels[f];
                if (cel == null || !done.Add(cel.Image) || !cel.Buffer.Pixels.Any(p => p >> 24 != 0 && (p & 0xFFFFFF) == (old & 0xFFFFFF))) continue;
                PixelBuffer target = sprite.GetWritable(layer, f);
                for (int i = 0; i < target.Pixels.Length; i++)
                {
                    uint p = target.Pixels[i];
                    // Pixels keep their own alpha unless the palette color itself is translucent.
                    if (p >> 24 != 0 && (p & 0xFFFFFF) == (old & 0xFFFFFF)) target.Pixels[i] = Argb.A(color) == 255 ? (color & 0xFFFFFF) | (p & 0xFF000000) : color;
                }
            }
        }
        if (w.Foreground == old) w.Foreground = color;
        if (w.Background == old) w.Background = color;
        w.Commit(Strings.History_EditPaletteColor);
    }
}
