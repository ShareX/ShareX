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

using ShareX.HelpersLib;
using ShareX.PixelEditor.Tools;
using ShareX.PixelEditor.UI.Dialogs;

namespace ShareX.PixelEditor.UI.Commands;

/// <summary>Every command of the Pixel editor and the layout of its main menu.</summary>
public static partial class EditorCommands
{
    private static PixelBuffer? _clipboard;
    private static SKPointI _clipboardOrigin;
    private static int _clipboardHash;

    private static bool HasSprite(PixelEditorWindow w) => w.Sprite != null;

    private static bool HasSelection(PixelEditorWindow w) => w.Sprite?.Selection != null;

    private static bool HasPixelLayer(PixelEditorWindow w) => w.Sprite?.ActiveLayer is PixelLayer;

    private static void R(string id, string text, string? gesture, Action<PixelEditorWindow> execute, Func<PixelEditorWindow, bool>? canExecute = null,
        Func<PixelEditorWindow, bool>? isChecked = null, bool keepsFloating = false, bool stopsPlayback = true)
    {
        CommandRegistry.Register(new EditorCommand(id, text, gesture, execute, canExecute, isChecked) { KeepsFloating = keepsFloating, StopsPlayback = stopsPlayback });
    }

    public static void RegisterAll()
    {
        RegisterFile();
        RegisterEdit();
        RegisterSprite();
        RegisterLayer();
        RegisterFrame();
        RegisterSelect();
        RegisterEffects();
        RegisterView();
        RegisterPalette();
    }

    public static IReadOnlyList<MenuNode> MainMenu { get; } = BuildMenu();

    private static List<MenuNode> BuildMenu()
    {
        MenuNode C(string id) => MenuNode.Cmd(id);
        MenuNode S = MenuNode.Separator;
        return
        [
            MenuNode.Sub(Strings.Menu_File,
                C("file.new"), C("file.open"),
                new MenuNode { Header = Strings.Menu_OpenRecent, Dynamic = RecentFiles },
                C("file.newFromClipboard"), S,
                C("file.close"), C("file.closeAll"), S,
                C("file.save"), C("file.saveAs"), S,
                C("file.export"), C("file.exportSheet"), S,
                C("file.importSheet"), C("file.importFrames"), C("file.duplicate"), S,
                C("file.upload"), C("file.copyImage"), S,
                C("file.exit")),
            MenuNode.Sub(Strings.Menu_Edit,
                C("edit.undo"), C("edit.redo"), S,
                C("edit.cut"), C("edit.copy"), C("edit.copyMerged"), C("edit.paste"), C("edit.pasteNewLayer"), C("edit.pasteNewSprite"), S,
                C("edit.clear"), C("edit.fill"), C("edit.stroke"), C("edit.defineBrush"), S,
                C("edit.transform"), C("edit.flipHorizontal"), C("edit.flipVertical"), C("edit.rotateCW"), C("edit.rotateCCW"), C("edit.rotate180"),
                MenuNode.Sub(Strings.Menu_Shift, C("edit.shiftLeft"), C("edit.shiftRight"), C("edit.shiftUp"), C("edit.shiftDown")), S,
                C("edit.shortcuts"), C("edit.preferences")),
            MenuNode.Sub(Strings.Menu_Sprite,
                C("sprite.size"), C("sprite.canvas"), S,
                C("sprite.crop"), C("sprite.trim"), S,
                C("sprite.rotateCW"), C("sprite.rotateCCW"), C("sprite.rotate180"), C("sprite.flipHorizontal"), C("sprite.flipVertical"), S,
                C("sprite.reduceColors")),
            MenuNode.Sub(Strings.Menu_Layer,
                C("layer.new"), C("layer.newGroup"), C("layer.duplicate"), C("layer.importImage"), C("layer.delete"), S,
                C("layer.properties"), C("layer.visible"), C("layer.lock"), C("layer.reference"), S,
                C("layer.mergeDown"), C("layer.flatten"), S,
                C("layer.moveUp"), C("layer.moveDown"), C("layer.moveToTop"), C("layer.moveToBottom"), S,
                C("layer.above"), C("layer.below")),
            MenuNode.Sub(Strings.Menu_Frame,
                C("frame.new"), C("frame.newEmpty"), C("frame.newLinked"), C("frame.delete"), S,
                C("frame.properties"), C("frame.reverse"), C("frame.moveLeft"), C("frame.moveRight"), S,
                MenuNode.Sub(Strings.Menu_Cel, C("cel.clear"), C("cel.link"), C("cel.unlink"), C("cel.properties")),
                MenuNode.Sub(Strings.Menu_Tags, C("frame.newTag"), C("frame.tagProperties"), C("frame.removeTag")), S,
                C("frame.play"), C("frame.loop"), C("frame.playTagOnly"), S,
                C("frame.first"), C("frame.previous"), C("frame.next"), C("frame.last"), S,
                C("frame.onionSkin"), C("frame.onionSettings")),
            MenuNode.Sub(Strings.Menu_Select,
                C("select.all"), C("select.none"), C("select.inverse"), S,
                C("select.byColor"), C("select.celContent"), S,
                C("select.grow"), C("select.shrink"), C("select.border")),
            MenuNode.Sub(Strings.Menu_Effects,
                C("fx.hueSaturation"), C("fx.brightnessContrast"), C("fx.invert"), C("fx.grayscale"), C("fx.posterize"), S,
                C("fx.replaceColor"), C("fx.outline"), C("fx.dropShadow"), C("fx.alphaThreshold")),
            MenuNode.Sub(Strings.Menu_View,
                C("view.zoomIn"), C("view.zoomOut"), C("view.fit"), C("view.actualSize"), S,
                C("view.pixelGrid"), C("view.grid"), C("view.snapToGrid"), C("view.gridSettings"), C("view.slices"), C("view.brushPreview"), S,
                MenuNode.Sub(Strings.Menu_Symmetry, C("view.symmetryHorizontal"), C("view.symmetryVertical"), C("view.symmetryReset"), C("view.symmetryAxis")),
                MenuNode.Sub(Strings.Menu_Tiled, C("view.tiled"), C("view.tiledX"), C("view.tiledY")), S,
                new MenuNode { Header = Strings.Menu_Panels, Dynamic = PanelItems },
                C("view.timeline"), C("view.toolbar"), C("view.options"), C("view.toolbarColumns"), C("view.resetPanels"), S,
                C("view.fullScreen")),
            MenuNode.Sub(Strings.Menu_Help, C("edit.shortcuts"))
        ];
    }

    private static IEnumerable<MenuNode> RecentFiles(PixelEditorWindow w)
    {
        foreach (string path in w.Settings.RecentFiles.Where(File.Exists).Take(12))
        {
            string captured = path;
            yield return new MenuNode { Header = path.Replace("_", "__"), Action = editor => OpenFile(editor, captured) };
        }
    }

    private static IEnumerable<MenuNode> PanelItems(PixelEditorWindow w)
    {
        foreach (Panels.EditorPanel panel in w.Panels.Panels)
        {
            string id = panel.Id;
            yield return new MenuNode { Header = panel.Title, Checked = w.Panels.IsPanelVisible(id), Action = editor => editor.Panels.TogglePanel(id) };
        }
    }

    // ------------------------------------------------------------------ file

    private static readonly (string, string[])[] OpenTypes =
    [
        (Strings.File_AllSupported, [SpriteFile.ProjectExtension, .. SpriteFile.ImageExtensions]),
        (Strings.File_Project, [SpriteFile.ProjectExtension]),
        (Strings.File_Images, SpriteFile.ImageExtensions)
    ];

    private static void RegisterFile()
    {
        R("file.new", Strings.Command_New, "Ctrl+N", w => _ = NewSpriteDialog.ShowAsync(w));
        R("file.open", Strings.Command_Open, "Ctrl+O", async w =>
        {
            foreach (string path in await w.PickOpenFilesAsync(Strings.Command_Open.Replace("...", string.Empty), true, OpenTypes)) OpenFile(w, path);
        });
        R("file.newFromClipboard", Strings.Command_NewFromClipboard, "Ctrl+Shift+N", NewFromClipboard);
        R("file.close", Strings.Command_Close, "Ctrl+W", w => _ = w.CloseViewAsync(w.View!), HasSprite);
        R("file.closeAll", Strings.Command_CloseAll, "Ctrl+Shift+W", async w =>
        {
            foreach (SpriteView view in w.Views.ToList())
                if (!await w.CloseViewAsync(view)) break;
        }, HasSprite);
        R("file.save", Strings.Command_Save, "Ctrl+S", w => _ = SaveAsync(w, false), HasSprite);
        R("file.saveAs", Strings.Command_SaveAs, "Ctrl+Shift+S", w => _ = SaveAsync(w, true), HasSprite);
        R("file.export", Strings.Command_Export, "Ctrl+Shift+E", w => _ = ExportDialog.ShowAsync(w), HasSprite);
        R("file.exportSheet", Strings.Command_ExportSheet, null, w => _ = SheetDialogs.ExportAsync(w), HasSprite);
        R("file.importSheet", Strings.Command_ImportSheet, null, async w =>
        {
            IReadOnlyList<string> files = await w.PickOpenFilesAsync(Strings.Command_ImportSheet.Replace("...", string.Empty), false, (Strings.File_Images, SpriteFile.ImageExtensions));
            if (files.Count > 0) await SheetDialogs.ImportAsync(w, files[0]);
        });
        R("file.importFrames", Strings.Command_ImportFrames, null, async w =>
        {
            IReadOnlyList<string> files = await w.PickOpenFilesAsync(Strings.Command_ImportFrames.Replace("...", string.Empty), true, (Strings.File_Images, SpriteFile.ImageExtensions));
            if (files.Count == 0) return;
            try
            {
                // One frame per image, in name order; the first image sets the sprite size.
                List<PixelBuffer> images = [];
                foreach (string file in files.OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
                {
                    using SKBitmap bitmap = SkiaImageHelpers.LoadImage(file) ?? throw new InvalidDataException(Path.GetFileName(file));
                    images.Add(PixelBuffer.FromBitmap(bitmap));
                }
                Sprite sprite = new(images[0].Width, images[0].Height) { Title = Path.GetFileNameWithoutExtension(files[0]) };
                PixelLayer layer = new() { Name = $"{Strings.Layer_DefaultName} 1" };
                foreach (PixelBuffer image in images)
                {
                    PixelBuffer cel = new(sprite.Width, sprite.Height);
                    cel.Blit(image, (sprite.Width - image.Width) / 2, (sprite.Height - image.Height) / 2, false);
                    layer.Cels.Add(cel.IsEmpty ? null : new Cel(new CelImage(cel)));
                }
                GroupLayer root = new();
                layer.Parent = root;
                root.Children.Add(layer);
                sprite.Adopt(root, images.Select(_ => new FrameInfo()).ToList(), []);
                List<uint> colors = Palettes.FromImages(images, 257);
                if (colors.Count is > 0 and <= 256) sprite.Palette = colors;
                w.OpenSprite(sprite);
                sprite.History.MarkModified();
                w.RefreshPanels();
            }
            catch (Exception ex)
            {
                w.ShowMessage(string.Format(Strings.Message_OperationFailed, ex.Message));
            }
        });
        R("file.duplicate", Strings.Command_DuplicateSprite, null, w =>
        {
            Sprite source = w.Sprite!;
            Sprite copy = new(source.Width, source.Height) { Title = string.Format(Strings.Layer_CopyName, Path.GetFileNameWithoutExtension(source.DisplayName)) };
            copy.Restore(source.Capture());
            w.OpenSprite(copy);
            copy.History.MarkModified();
            w.RefreshPanels();
        }, HasSprite);
        R("file.upload", Strings.Command_Upload, null, w =>
        {
            using SKBitmap bitmap = FlattenBitmap(w.Sprite!, w.Sprite!.ActiveFrame, w.Settings.ExportScale);
            w.Services.UploadImage?.Invoke(bitmap);
        }, w => w.Sprite != null && w.Services.UploadImage != null);
        R("file.copyImage", Strings.Command_CopyImage, null, w =>
        {
            using SKBitmap bitmap = FlattenBitmap(w.Sprite!, w.Sprite!.ActiveFrame, w.Settings.ExportScale);
            ClipboardHelpers.CopyImage(bitmap);
            w.SetStatus(Strings.Status_Copied);
        }, HasSprite);
        R("file.exit", Strings.Command_Exit, "Ctrl+Q", w => w.Close());
    }

    /// <summary>The flattened frame as a premultiplied bitmap, scaled up with crisp pixels.</summary>
    public static SKBitmap FlattenBitmap(Sprite sprite, int frame, int scale)
    {
        PixelBuffer flat = Compositor.Flatten(sprite, frame);
        if (scale > 1) flat = flat.ScaleNearest(flat.Width * scale, flat.Height * scale);
        return ToPremultiplied(flat);
    }

    private static SKBitmap ToPremultiplied(PixelBuffer buffer)
    {
        using SKBitmap straight = buffer.ToBitmap();
        SKBitmap result = new(new SKImageInfo(buffer.Width, buffer.Height, SKColorType.Bgra8888, SKAlphaType.Premul));
        straight.CopyTo(result);
        return result;
    }

    public static void OpenFile(PixelEditorWindow w, string path)
    {
        SpriteView? open = w.Views.FirstOrDefault(v => string.Equals(v.Sprite.FilePath, path, StringComparison.OrdinalIgnoreCase));
        if (open != null)
        {
            w.ActivateView(open);
            return;
        }
        try
        {
            Sprite sprite = SpriteFile.Load(path, Strings.Layer_DefaultName);
            // Images bring their own colors as the palette when there are few enough.
            if (sprite.Palette.Count == 0)
            {
                List<uint> colors = Palettes.FromImages(sprite.PixelLayers.SelectMany(l => l.Cels).Where(c => c != null).Select(c => c!.Buffer).Distinct(), 257);
                if (colors.Count is > 0 and <= 256) sprite.Palette = colors;
            }
            w.OpenSprite(sprite);
            w.Settings.AddRecentFile(path);
        }
        catch (Exception ex)
        {
            w.ShowMessage(string.Format(Strings.Message_CouldNotOpen, Path.GetFileName(path), ex.Message));
        }
    }

    public static void OpenBitmap(PixelEditorWindow w, SKBitmap bitmap, string title)
    {
        if (bitmap.Width > Sprite.MaxSize || bitmap.Height > Sprite.MaxSize)
        {
            w.ShowMessage(string.Format(Strings.Message_TooLarge, Sprite.MaxSize));
            return;
        }
        Sprite sprite = Sprite.FromBuffer(PixelBuffer.FromBitmap(bitmap), title, Strings.Layer_DefaultName);
        w.OpenSprite(sprite);
        sprite.History.MarkModified();
        w.RefreshPanels();
    }

    public static SKBitmap? TryGetClipboardImage()
    {
        try
        {
            return ClipboardHelpers.TryGetImage();
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static void NewFromClipboard(PixelEditorWindow w)
    {
        using SKBitmap? image = TryGetClipboardImage();
        if (image == null)
        {
            w.ShowMessage(Strings.Message_ClipboardEmpty);
            return;
        }
        OpenBitmap(w, image, Strings.Sprite_Clipboard);
    }

    private static readonly (string, string[])[] SaveTypes =
    [
        (Strings.File_Project, [SpriteFile.ProjectExtension]),
        ("PNG", [".png"]),
        ("GIF", [".gif"]),
        ("WebP", [".webp"]),
        ("JPEG", [".jpg", ".jpeg"])
    ];

    /// <summary>Saves the sprite. Images that cannot hold layers and frames go through Save As with the project format suggested.</summary>
    public static async Task<bool> SaveAsync(PixelEditorWindow w, bool saveAs)
    {
        Sprite? sprite = w.Sprite;
        if (sprite == null) return false;
        w.CommitFloating();
        string? path = sprite.FilePath;
        bool simple = sprite.Frames.Count == 1 && sprite.Root.Children.Count == 1 && sprite.Root.Children[0] is PixelLayer { Opacity: 255, Blend: BlendMode.Normal };
        bool animatedOnly = sprite.Root.Children.Count == 1 && sprite.Root.Children[0] is PixelLayer { Opacity: 255, Blend: BlendMode.Normal };
        bool canOverwrite = path != null && (SpriteFile.IsProject(path) || simple && Path.GetExtension(path).ToLowerInvariant() is ".png" or ".webp" or ".jpg" or ".jpeg"
            || animatedOnly && Path.GetExtension(path).Equals(".gif", StringComparison.OrdinalIgnoreCase));
        if (saveAs || !canOverwrite)
        {
            string name = Path.GetFileNameWithoutExtension(path ?? sprite.Title) + (path != null && canOverwrite ? Path.GetExtension(path) : SpriteFile.ProjectExtension);
            path = await w.PickSaveFileAsync(Strings.Command_SaveAs.Replace("...", string.Empty), name, SaveTypes);
            if (path == null) return false;
        }
        try
        {
            if (SpriteFile.IsProject(path!)) SpriteFile.SaveProject(sprite, path!);
            else if (Path.GetExtension(path!).Equals(".gif", StringComparison.OrdinalIgnoreCase) && sprite.Frames.Count > 1)
                SpriteFile.ExportGif(sprite, path!, new ExportOptions { Frames = Enumerable.Range(0, sprite.Frames.Count).ToList() });
            else SpriteFile.ExportImage(sprite, path!, sprite.ActiveFrame, new ExportOptions());
        }
        catch (Exception ex)
        {
            w.ShowMessage(string.Format(Strings.Message_CouldNotSave, Path.GetFileName(path), ex.Message));
            return false;
        }
        sprite.FilePath = path;
        sprite.History.MarkSaved();
        w.Settings.AddRecentFile(path!);
        w.RefreshPanels();
        w.SetStatus(string.Format(Strings.Status_Saved, Path.GetFileName(path)));
        return true;
    }

    // ------------------------------------------------------------------ edit

    private static void RegisterEdit()
    {
        R("edit.undo", Strings.Command_Undo, "Ctrl+Z", w =>
        {
            if (w.Sprite!.History.Undo()) w.AfterHistoryChange();
        }, w => w.Sprite?.History.CanUndo == true);
        R("edit.redo", Strings.Command_Redo, "Ctrl+Y,Ctrl+Shift+Z", w =>
        {
            if (w.Sprite!.History.Redo()) w.AfterHistoryChange();
        }, w => w.Sprite?.History.CanRedo == true);
        R("edit.cut", Strings.Command_Cut, "Ctrl+X", w => Copy(w, false, true), HasPixelLayer);
        R("edit.copy", Strings.Command_Copy, "Ctrl+C", w => Copy(w, false, false), HasPixelLayer);
        R("edit.copyMerged", Strings.Command_CopyMerged, "Ctrl+Shift+C", w => Copy(w, true, false), HasSprite);
        R("edit.paste", Strings.Command_Paste, "Ctrl+V", w => Paste(w, false));
        R("edit.pasteNewLayer", Strings.Command_PasteNewLayer, "Ctrl+Shift+V", w => Paste(w, true), HasSprite);
        R("edit.pasteNewSprite", Strings.Command_PasteNewSprite, "Ctrl+Alt+V", NewFromClipboard);
        R("edit.clear", Strings.Command_Clear, "Delete,Back", w => Clear(w, Strings.Command_Clear), HasPixelLayer);
        R("edit.fill", Strings.Command_Fill, "Alt+Back", w => FillSelection(w, false), HasPixelLayer);
        R("edit.stroke", Strings.Command_Stroke, null, w => FillSelection(w, true), w => HasPixelLayer(w) && HasSelection(w));
        R("edit.defineBrush", Strings.Command_DefineBrush, "Ctrl+B", w =>
        {
            Sprite sprite = w.Sprite!;
            SelectionMask selection = sprite.Selection!;
            PixelBuffer? buffer = sprite.ActiveLayer is PixelLayer layer ? sprite.GetBuffer(layer, sprite.ActiveFrame) : null;
            SKRectI bounds = selection.Bounds;
            List<SKPointI> all = [], drawn = [];
            for (int y = bounds.Top; y < bounds.Bottom; y++)
            {
                for (int x = bounds.Left; x < bounds.Right; x++)
                {
                    if (!selection[x, y]) continue;
                    SKPointI point = new(x - bounds.Left - bounds.Width / 2, y - bounds.Top - bounds.Height / 2);
                    all.Add(point);
                    if (buffer != null && buffer[x, y] >> 24 != 0) drawn.Add(point);
                }
            }
            // The drawn pixels inside the selection shape the brush; an empty selection area gives its outline shape.
            w.CustomTip = (drawn.Count > 0 ? drawn : all).ToArray();
            if (w.Tools.Active is not PaintToolBase) w.Tools.Select(ToolId.Pencil);
            w.SetSelection(null);
            w.RefreshOptions();
        }, HasSelection);
        R("edit.transform", Strings.Command_Transform, "Ctrl+T", w =>
        {
            if (w.Floating == null && !w.BeginFloating(false)) return;
            w.Tools.Select(ToolId.Move);
            w.RefreshOptions();
        }, HasPixelLayer, keepsFloating: true);
        R("edit.flipHorizontal", Strings.Command_FlipHorizontal, "Shift+H", w => TransformCel(w, f => f.FlipX = !f.FlipX, b => b.FlipHorizontal(), Strings.Command_FlipHorizontal), HasPixelLayer, keepsFloating: true);
        R("edit.flipVertical", Strings.Command_FlipVertical, "Shift+V", w => TransformCel(w, f => f.FlipY = !f.FlipY, b => b.FlipVertical(), Strings.Command_FlipVertical), HasPixelLayer, keepsFloating: true);
        R("edit.rotateCW", Strings.Command_Rotate90CW, null, w => TransformCel(w, f => f.Angle = (f.Angle + 90) % 360, null, Strings.Command_Rotate90CW), HasPixelLayer, keepsFloating: true);
        R("edit.rotateCCW", Strings.Command_Rotate90CCW, null, w => TransformCel(w, f => f.Angle = (f.Angle + 270) % 360, null, Strings.Command_Rotate90CCW), HasPixelLayer, keepsFloating: true);
        R("edit.rotate180", Strings.Command_Rotate180, null, w => TransformCel(w, f => f.Angle = (f.Angle + 180) % 360, b => b.Rotate90(2), Strings.Command_Rotate180), HasPixelLayer, keepsFloating: true);
        void Shift(string id, string text, string gesture, int dx, int dy)
        {
            R(id, text, gesture, w =>
            {
                Sprite sprite = w.Sprite!;
                if (w.EditableLayer() == null) return;
                // Pixels leaving one side come back in on the other, inside the selection bounds or the whole cel.
                SKRectI area = sprite.Selection?.Bounds ?? sprite.Bounds;
                bool changed = SpriteOps.Apply(sprite, sprite.RangeTo > sprite.RangeFrom ? CelScope.SelectedFrames : CelScope.Cel, (b, _) =>
                {
                    PixelBuffer result = b.Clone();
                    for (int y = area.Top; y < area.Bottom; y++)
                    {
                        for (int x = area.Left; x < area.Right; x++)
                        {
                            int sx = area.Left + ((x - area.Left - dx) % area.Width + area.Width) % area.Width, sy = area.Top + ((y - area.Top - dy) % area.Height + area.Height) % area.Height;
                            result[x, y] = b[sx, sy];
                        }
                    }
                    return result;
                });
                if (changed) w.Commit(text);
            }, HasPixelLayer);
        }
        Shift("edit.shiftLeft", Strings.Command_ShiftLeft, "Ctrl+Shift+Left", -1, 0);
        Shift("edit.shiftRight", Strings.Command_ShiftRight, "Ctrl+Shift+Right", 1, 0);
        Shift("edit.shiftUp", Strings.Command_ShiftUp, "Ctrl+Shift+Up", 0, -1);
        Shift("edit.shiftDown", Strings.Command_ShiftDown, "Ctrl+Shift+Down", 0, 1);
        R("edit.preferences", Strings.Command_Preferences, "Ctrl+K", w => _ = PreferencesDialog.ShowAsync(w), stopsPlayback: false);
        R("edit.shortcuts", Strings.Command_Shortcuts, "F1", w => _ = ShortcutsDialog.ShowAsync(w), stopsPlayback: false);
        R("color.swap", Strings.Toolbar_SwitchColors, "X", w => w.SwapColors(), keepsFloating: true, stopsPlayback: false);
        R("color.default", Strings.Toolbar_DefaultColors, "D", w =>
        {
            w.Foreground = Argb.Black;
            w.Background = Argb.White;
        }, keepsFloating: true, stopsPlayback: false);
        R("tool.brushSmaller", Strings.Command_BrushSmaller, "OemOpenBrackets", w => ChangeBrushSize(w, -1), keepsFloating: true, stopsPlayback: false);
        R("tool.brushLarger", Strings.Command_BrushLarger, "OemCloseBrackets", w => ChangeBrushSize(w, 1), keepsFloating: true, stopsPlayback: false);
    }

    private static void ChangeBrushSize(PixelEditorWindow w, int delta)
    {
        if (w.Tools.Active is EraserTool) w.Settings.EraserSize = Math.Clamp(w.Settings.EraserSize + delta, 1, 64);
        else w.Settings.BrushSize = Math.Clamp(w.Settings.BrushSize + delta, 1, 64);
        w.RefreshOptions();
        w.Canvas.InvalidateVisual();
    }

    /// <summary>Flips or rotates the moved pixels, the selection, or (without a selection) the whole cel.</summary>
    private static void TransformCel(PixelEditorWindow w, Action<Floating> change, Func<PixelBuffer, PixelBuffer>? whole, string name)
    {
        Sprite sprite = w.Sprite!;
        if (w.Floating is Floating floating)
        {
            change(floating);
            w.RenderFloating();
            w.RefreshOptions();
            return;
        }
        if (sprite.Selection == null && whole != null)
        {
            if (w.EditableLayer() == null) return;
            if (SpriteOps.Apply(sprite, CelScope.Cel, (b, _) => whole(b))) w.Commit(name);
            return;
        }
        if (!w.BeginFloating(false)) return;
        change(w.Floating!);
        w.RenderFloating();
        w.CommitFloating();
    }

    private static void Copy(PixelEditorWindow w, bool merged, bool cut)
    {
        Sprite sprite = w.Sprite!;
        PixelBuffer? source = merged ? Compositor.Flatten(sprite, sprite.ActiveFrame, true) : sprite.ActiveLayer is PixelLayer layer ? sprite.GetBuffer(layer, sprite.ActiveFrame) : null;
        SKRectI? bounds = sprite.Selection?.Bounds ?? source?.ContentBounds();
        if (source == null || bounds is not SKRectI area || area.IsEmpty)
        {
            w.SetStatus(Strings.Status_NothingToCopy);
            return;
        }
        PixelBuffer copy = source.Crop(area);
        if (sprite.Selection is SelectionMask selection)
        {
            for (int y = 0; y < copy.Height; y++)
                for (int x = 0; x < copy.Width; x++)
                    if (!selection[area.Left + x, area.Top + y]) copy.Pixels[y * copy.Width + x] = 0;
        }
        _clipboard = copy;
        _clipboardOrigin = new SKPointI(area.Left, area.Top);
        try
        {
            using SKBitmap bitmap = ToPremultiplied(copy);
            ClipboardHelpers.CopyImage(bitmap);
            _clipboardHash = ClipboardSignature();
        }
        catch (Exception)
        {
            _clipboardHash = 0;
        }
        if (cut) Clear(w, Strings.Command_Cut);
        else w.SetStatus(Strings.Status_Copied);
    }

    private static int ClipboardSignature()
    {
        using SKBitmap? image = TryGetClipboardImage();
        return image == null ? 0 : HashCode.Combine(image.Width, image.Height, image.GetPixel(image.Width / 2, image.Height / 2));
    }

    private static void Paste(PixelEditorWindow w, bool newLayer)
    {
        PixelBuffer? image = null;
        SKPointI? origin = null;
        using (SKBitmap? system = TryGetClipboardImage())
        {
            // Our own copy is used while it is still what the system clipboard holds, since it keeps exact pixels and position.
            if (_clipboard != null && (system == null || HashCode.Combine(system.Width, system.Height, system.GetPixel(system.Width / 2, system.Height / 2)) == _clipboardHash))
            {
                image = _clipboard.Clone();
                origin = _clipboardOrigin;
            }
            else if (system != null)
            {
                if (w.Sprite == null)
                {
                    OpenBitmap(w, system, Strings.Sprite_Clipboard);
                    return;
                }
                image = PixelBuffer.FromBitmap(system);
            }
        }
        if (image == null)
        {
            w.ShowMessage(Strings.Message_ClipboardEmpty);
            return;
        }
        Sprite? sprite = w.Sprite;
        if (sprite == null)
        {
            w.OpenSprite(Sprite.FromBuffer(image, Strings.Sprite_Clipboard, Strings.Layer_DefaultName));
            return;
        }
        if (newLayer || sprite.ActiveLayer is not PixelLayer)
        {
            PixelLayer layer = sprite.CreatePixelLayer(UniqueLayerName(sprite, Strings.Layer_DefaultName));
            sprite.AddLayer(layer, sprite.ActiveLayer);
            sprite.ActiveLayer = layer;
        }
        if (w.EditableLayer() == null) return;
        bool fits = origin is SKPointI o && o.X >= 0 && o.Y >= 0 && o.X + image.Width <= sprite.Width && o.Y + image.Height <= sprite.Height;
        int x = fits ? origin!.Value.X : (sprite.Width - image.Width) / 2, y = fits ? origin!.Value.Y : (sprite.Height - image.Height) / 2;
        w.PasteFloating(image, x, y, Strings.Command_Paste);
        w.RefreshPanels();
    }

    /// <summary>Erases the selected pixels of the active cel, or the whole cel without a selection.</summary>
    private static void Clear(PixelEditorWindow w, string name)
    {
        Sprite sprite = w.Sprite!;
        PixelLayer? layer = w.EditableLayer();
        if (layer == null || sprite.GetBuffer(layer, sprite.ActiveFrame) == null) return;
        if (sprite.Selection is SelectionMask selection)
        {
            PixelBuffer target = sprite.GetWritable(layer, sprite.ActiveFrame);
            for (int i = 0; i < target.Pixels.Length; i++)
                if (selection.Data[i]) target.Pixels[i] = 0;
        }
        else
        {
            sprite.SetBuffer(layer, sprite.ActiveFrame, null);
        }
        w.Commit(name);
    }

    private static void FillSelection(PixelEditorWindow w, bool stroke)
    {
        Sprite sprite = w.Sprite!;
        Painter? painter = w.CreatePainter(false);
        if (painter == null) return;
        painter.Symmetry = SymmetryMode.None;
        painter.TiledX = painter.TiledY = false;
        if (stroke)
        {
            painter.Clip = sprite.Selection!.Border(1);
        }
        Raster.Rect(0, 0, sprite.Width - 1, sprite.Height - 1, true, painter.Pixel);
        if (painter.TotalDirty.IsEmpty) w.RevertEdit();
        else w.Commit(stroke ? Strings.Command_Stroke : Strings.Command_Fill);
    }

    public static string UniqueLayerName(Sprite sprite, string baseName)
    {
        HashSet<string> names = sprite.AllLayers.Select(l => l.Name).ToHashSet();
        for (int i = 1; ; i++)
        {
            string name = $"{baseName} {i}";
            if (!names.Contains(name)) return name;
        }
    }
}
