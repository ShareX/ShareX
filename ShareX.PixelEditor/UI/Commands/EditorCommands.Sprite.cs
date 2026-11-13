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
    // ------------------------------------------------------------------ sprite

    private static void RegisterSprite()
    {
        R("sprite.size", Strings.Command_SpriteSize, "Ctrl+Alt+I", w => _ = SpriteDialogs.SpriteSizeAsync(w), HasSprite);
        R("sprite.canvas", Strings.Command_CanvasSize, "Ctrl+Alt+C", w => _ = SpriteDialogs.CanvasSizeAsync(w), HasSprite);
        R("sprite.crop", Strings.Command_Crop, null, w =>
        {
            SKRectI area = w.Sprite!.Selection!.Bounds;
            SpriteOps.Crop(w.Sprite, area);
            w.Sprite.Selection = null;
            w.Commit(Strings.Command_Crop);
            w.View!.NeedsFit = true;
        }, HasSelection);
        R("sprite.trim", Strings.Command_Trim, null, w =>
        {
            if (SpriteOps.ContentBounds(w.Sprite!) is not SKRectI bounds || bounds == w.Sprite!.Bounds) return;
            SpriteOps.Crop(w.Sprite, bounds);
            w.Commit(Strings.Command_Trim);
            w.View!.NeedsFit = true;
        }, HasSprite);
        void Whole(string id, string text, Action<Sprite> action)
        {
            R(id, text, null, w =>
            {
                action(w.Sprite!);
                w.Commit(text);
                w.View!.NeedsFit = true;
            }, HasSprite);
        }
        Whole("sprite.rotateCW", Strings.Command_Rotate90CW, s => SpriteOps.Rotate(s, 1));
        Whole("sprite.rotateCCW", Strings.Command_Rotate90CCW, s => SpriteOps.Rotate(s, 3));
        Whole("sprite.rotate180", Strings.Command_Rotate180, s => SpriteOps.Rotate(s, 2));
        Whole("sprite.flipHorizontal", Strings.Command_FlipHorizontal, s => SpriteOps.Flip(s, true));
        Whole("sprite.flipVertical", Strings.Command_FlipVertical, s => SpriteOps.Flip(s, false));
        R("sprite.reduceColors", Strings.Command_ReduceColors, null, w => _ = SpriteDialogs.ReduceColorsAsync(w), HasSprite);
    }

    // ------------------------------------------------------------------ layers

    private static void RegisterLayer()
    {
        R("layer.new", Strings.Command_NewLayer, "Shift+N", w =>
        {
            Sprite sprite = w.Sprite!;
            PixelLayer layer = sprite.CreatePixelLayer(UniqueLayerName(sprite, Strings.Layer_DefaultName));
            sprite.AddLayer(layer, sprite.ActiveLayer);
            sprite.ActiveLayer = layer;
            w.Commit(Strings.Command_NewLayer);
        }, HasSprite);
        R("layer.newGroup", Strings.Command_NewGroup, "Ctrl+G", w =>
        {
            Sprite sprite = w.Sprite!;
            GroupLayer group = sprite.CreateGroup(UniqueLayerName(sprite, Strings.Layer_GroupName));
            Layer? active = sprite.ActiveLayer;
            sprite.AddLayer(group, active);
            // The active layer moves into the new group.
            if (active != null)
            {
                sprite.RemoveLayer(active);
                sprite.InsertLayer(active, group, 0);
                sprite.ActiveLayer = active;
            }
            else sprite.ActiveLayer = group;
            w.Commit(Strings.Command_NewGroup);
        }, HasSprite);
        R("layer.duplicate", Strings.Command_DuplicateLayer, "Ctrl+J", w =>
        {
            Sprite sprite = w.Sprite!;
            SpriteOps.Duplicate(sprite, sprite.ActiveLayer!, string.Format(Strings.Layer_CopyName, sprite.ActiveLayer!.Name));
            w.Commit(Strings.Command_DuplicateLayer);
        }, w => w.Sprite?.ActiveLayer != null);
        R("layer.importImage", Strings.Command_ImportImage, null, async w =>
        {
            IReadOnlyList<string> files = await w.PickOpenFilesAsync(Strings.Command_ImportImage.Replace("...", string.Empty), false, (Strings.File_Images, SpriteFile.ImageExtensions));
            Sprite? sprite = w.Sprite;
            if (files.Count == 0 || sprite == null) return;
            PixelBuffer image;
            try
            {
                using SKBitmap bitmap = ShareX.HelpersLib.SkiaImageHelpers.LoadImage(files[0]) ?? throw new InvalidDataException();
                image = PixelBuffer.FromBitmap(bitmap);
            }
            catch (Exception ex)
            {
                w.ShowMessage(string.Format(Strings.Message_CouldNotOpen, Path.GetFileName(files[0]), ex.Message));
                return;
            }
            // The image arrives on its own layer as a movable selection, so it can be placed and scaled first.
            PixelLayer layer = sprite.CreatePixelLayer(Path.GetFileNameWithoutExtension(files[0]));
            sprite.AddLayer(layer, sprite.ActiveLayer);
            sprite.ActiveLayer = layer;
            w.PasteFloating(image, (sprite.Width - image.Width) / 2, (sprite.Height - image.Height) / 2, Strings.Command_ImportImage.Replace("...", string.Empty));
            w.RefreshPanels();
        }, HasSprite);
        R("layer.delete", Strings.Command_DeleteLayer, null, w =>
        {
            Sprite sprite = w.Sprite!;
            Layer layer = sprite.ActiveLayer!;
            List<Layer> rows = sprite.Root.Rows(true).ToList();
            int index = rows.IndexOf(layer);
            sprite.RemoveLayer(layer);
            List<Layer> remaining = sprite.Root.Rows(true).ToList();
            sprite.ActiveLayer = remaining.Count > 0 ? remaining[Math.Clamp(index, 0, remaining.Count - 1)] : null;
            w.Commit(Strings.Command_DeleteLayer);
        }, w => w.Sprite?.ActiveLayer != null);
        R("layer.properties", Strings.Command_LayerProperties, null, w => _ = SpriteDialogs.LayerPropertiesAsync(w), w => w.Sprite?.ActiveLayer != null);
        R("layer.visible", Strings.Command_LayerVisible, null, w => ToggleLayerVisible(w, w.Sprite!.ActiveLayer!), w => w.Sprite?.ActiveLayer != null, w => w.Sprite?.ActiveLayer?.Visible == true);
        R("layer.lock", Strings.Command_LayerLock, null, w => ToggleLayerLocked(w, w.Sprite!.ActiveLayer!), w => w.Sprite?.ActiveLayer != null, w => w.Sprite?.ActiveLayer?.Locked == true);
        R("layer.reference", Strings.Command_LayerReference, null, w =>
        {
            PixelLayer layer = (PixelLayer)w.Sprite!.ActiveLayer!;
            layer.IsReference = !layer.IsReference;
            w.Sprite.Touch();
            w.Commit(Strings.Command_LayerReference);
        }, HasPixelLayer, w => w.Sprite?.ActiveLayer is PixelLayer { IsReference: true });
        R("layer.mergeDown", Strings.Command_MergeDown, "Ctrl+E", w =>
        {
            Sprite sprite = w.Sprite!;
            if (sprite.ActiveLayer is GroupLayer group)
            {
                SpriteOps.FlattenGroup(sprite, group);
                w.Commit(Strings.Command_MergeDown);
            }
            else if (SpriteOps.MergeDown(sprite, sprite.ActiveLayer!) != null) w.Commit(Strings.Command_MergeDown);
            else w.SetStatus(Strings.Status_NothingToMerge);
        }, w => w.Sprite?.ActiveLayer != null);
        R("layer.flatten", Strings.Command_Flatten, null, w =>
        {
            SpriteOps.Flatten(w.Sprite!, Strings.Layer_Flattened);
            w.Commit(Strings.Command_Flatten);
        }, HasSprite);
        R("layer.moveUp", Strings.Command_LayerMoveUp, "Ctrl+OemCloseBrackets", w => MoveLayer(w, 1, false), w => w.Sprite?.ActiveLayer != null);
        R("layer.moveDown", Strings.Command_LayerMoveDown, "Ctrl+OemOpenBrackets", w => MoveLayer(w, -1, false), w => w.Sprite?.ActiveLayer != null);
        R("layer.moveToTop", Strings.Command_LayerMoveToTop, "Ctrl+Shift+OemCloseBrackets", w => MoveLayer(w, 1, true), w => w.Sprite?.ActiveLayer != null);
        R("layer.moveToBottom", Strings.Command_LayerMoveToBottom, "Ctrl+Shift+OemOpenBrackets", w => MoveLayer(w, -1, true), w => w.Sprite?.ActiveLayer != null);
        R("layer.above", Strings.Command_LayerAbove, "Up", w => SelectNeighbor(w, -1), HasSprite, stopsPlayback: false);
        R("layer.below", Strings.Command_LayerBelow, "Down", w => SelectNeighbor(w, 1), HasSprite, stopsPlayback: false);
    }

    private static void SelectNeighbor(PixelEditorWindow w, int direction)
    {
        List<Layer> rows = w.Sprite!.Root.Rows().ToList();
        if (rows.Count == 0) return;
        int index = w.Sprite.ActiveLayer != null ? rows.IndexOf(w.Sprite.ActiveLayer) : -1;
        w.SetActiveLayer(rows[Math.Clamp(index + direction, 0, rows.Count - 1)]);
    }

    /// <summary>Moves the active layer one step in the stack. At the edge of a group it leaves the group; next to a group it enters it.</summary>
    private static void MoveLayer(PixelEditorWindow w, int direction, bool toEnd)
    {
        Sprite sprite = w.Sprite!;
        Layer layer = sprite.ActiveLayer!;
        GroupLayer parent = layer.Parent ?? sprite.Root;
        int index = parent.Children.IndexOf(layer);
        if (toEnd)
        {
            int target = direction > 0 ? parent.Children.Count - 1 : 0;
            if (target == index) return;
            parent.Children.RemoveAt(index);
            parent.Children.Insert(target, layer);
        }
        else
        {
            int target = index + direction;
            if (target < 0 || target >= parent.Children.Count)
            {
                // Step out of the group, landing just above or below it.
                if (parent.Parent is not GroupLayer grand) return;
                int groupIndex = grand.Children.IndexOf(parent);
                parent.Children.RemoveAt(index);
                layer.Parent = grand;
                grand.Children.Insert(direction > 0 ? groupIndex + 1 : groupIndex, layer);
            }
            else if (parent.Children[target] is GroupLayer group && !group.Collapsed && group != layer)
            {
                parent.Children.RemoveAt(index);
                layer.Parent = group;
                group.Children.Insert(direction > 0 ? 0 : group.Children.Count, layer);
            }
            else
            {
                parent.Children.RemoveAt(index);
                parent.Children.Insert(target, layer);
            }
        }
        sprite.Touch();
        w.Commit(Strings.History_ReorderLayer);
    }

    public static void ToggleLayerVisible(PixelEditorWindow w, Layer layer)
    {
        w.CommitFloating();
        if (w.LastModifiers.HasFlag(Avalonia.Input.KeyModifiers.Alt))
        {
            // Alt+click shows only this layer; again brings the others back.
            List<Layer> others = w.Sprite!.AllLayers.Where(l => l != layer && !(l is GroupLayer ancestor && layer.IsDescendantOf(ancestor)) && !(layer is GroupLayer group && l.IsDescendantOf(group))).ToList();
            bool solo = others.Any(l => l.Visible);
            foreach (Layer other in others) other.Visible = !solo;
            for (Layer? l = layer; l != null; l = l.Parent) l.Visible = true;
            w.Sprite.Touch();
            w.Commit(Strings.History_SoloLayer);
            return;
        }
        layer.Visible = !layer.Visible;
        w.Sprite!.Touch();
        w.Commit(layer.Visible ? Strings.History_ShowLayer : Strings.History_HideLayer);
    }

    public static void ToggleLayerLocked(PixelEditorWindow w, Layer layer)
    {
        w.CommitFloating();
        layer.Locked = !layer.Locked;
        w.Commit(layer.Locked ? Strings.History_LockLayer : Strings.History_UnlockLayer);
    }

    // ------------------------------------------------------------------ frames, cels and tags

    private static void RegisterFrame()
    {
        R("frame.new", Strings.Command_NewFrame, "Alt+N", w => NewFrame(w, true, false, Strings.Command_NewFrame), HasSprite);
        R("frame.newEmpty", Strings.Command_NewEmptyFrame, "Alt+B", w => NewFrame(w, false, false, Strings.Command_NewEmptyFrame), HasSprite);
        R("frame.newLinked", Strings.Command_NewLinkedFrame, "Alt+M", w => NewFrame(w, true, true, Strings.Command_NewLinkedFrame), HasSprite);
        R("frame.delete", Strings.Command_DeleteFrame, "Alt+C", w =>
        {
            Sprite sprite = w.Sprite!;
            sprite.RemoveFrames(sprite.RangeFrom, sprite.RangeTo);
            w.Commit(Strings.Command_DeleteFrame);
        }, w => w.Sprite?.Frames.Count > 1);
        R("frame.properties", Strings.Command_FrameProperties, "Alt+P", w => _ = SpriteDialogs.FramePropertiesAsync(w), HasSprite);
        R("frame.reverse", Strings.Command_ReverseFrames, null, w =>
        {
            Sprite sprite = w.Sprite!;
            sprite.ReverseFrames(sprite.RangeFrom, sprite.RangeTo);
            w.Commit(Strings.Command_ReverseFrames);
        }, w => w.Sprite != null && w.Sprite.RangeTo > w.Sprite.RangeFrom);
        R("frame.moveLeft", Strings.Command_MoveFrameLeft, "Alt+Left", w => MoveFrames(w, -1), w => w.Sprite?.RangeFrom > 0);
        R("frame.moveRight", Strings.Command_MoveFrameRight, "Alt+Right", w => MoveFrames(w, 1), w => w.Sprite != null && w.Sprite.RangeTo < w.Sprite.Frames.Count - 1);

        R("frame.first", Strings.Command_FirstFrame, "Home", w => w.GoToFrame(0), HasSprite);
        R("frame.previous", Strings.Command_PreviousFrame, "Left,OemComma", w => w.GoToFrame((w.Sprite!.ActiveFrame - 1 + w.Sprite.Frames.Count) % w.Sprite.Frames.Count), HasSprite);
        R("frame.next", Strings.Command_NextFrame, "Right,OemPeriod", w => w.GoToFrame((w.Sprite!.ActiveFrame + 1) % w.Sprite.Frames.Count), HasSprite);
        R("frame.last", Strings.Command_LastFrame, "End", w => w.GoToFrame(w.Sprite!.Frames.Count - 1), HasSprite);
        R("frame.play", Strings.Command_Play, "Enter", w => w.TogglePlayback(), w => w.Sprite?.Frames.Count > 1, stopsPlayback: false);
        R("frame.loop", Strings.Command_Loop, null, w =>
        {
            w.Settings.LoopPlayback = !w.Settings.LoopPlayback;
            w.RefreshTimeline();
        }, null, w => w.Settings.LoopPlayback, stopsPlayback: false);
        R("frame.playTagOnly", Strings.Command_PlayTagOnly, null, w =>
        {
            w.Settings.PlayTagOnly = !w.Settings.PlayTagOnly;
            w.RefreshPanels();
        }, null, w => w.Settings.PlayTagOnly);
        R("frame.onionSkin", Strings.Command_OnionSkin, "F3", w =>
        {
            w.Settings.OnionSkin = !w.Settings.OnionSkin;
            w.InvalidateCanvas();
            w.RefreshTimeline();
            w.RefreshOptions();
        }, null, w => w.Settings.OnionSkin, keepsFloating: true, stopsPlayback: false);
        R("frame.onionSettings", Strings.Command_OnionSettings, null, w => _ = SpriteDialogs.OnionSettingsAsync(w), stopsPlayback: false);

        R("frame.newTag", Strings.Command_NewTag, null, w => _ = SpriteDialogs.TagAsync(w, null), HasSprite);
        R("frame.tagProperties", Strings.Command_TagProperties, null, w => _ = SpriteDialogs.TagAsync(w, w.Sprite!.TagAt(w.Sprite.ActiveFrame)), w => w.Sprite?.TagAt(w.Sprite.ActiveFrame) != null);
        R("frame.removeTag", Strings.Command_RemoveTag, null, w =>
        {
            Sprite sprite = w.Sprite!;
            sprite.Tags.Remove(sprite.TagAt(sprite.ActiveFrame)!);
            w.Commit(Strings.Command_RemoveTag);
        }, w => w.Sprite?.TagAt(w.Sprite.ActiveFrame) != null);

        R("cel.clear", Strings.Command_ClearCel, null, w =>
        {
            Sprite sprite = w.Sprite!;
            PixelLayer? layer = w.EditableLayer();
            if (layer == null) return;
            for (int f = sprite.RangeFrom; f <= sprite.RangeTo; f++) sprite.SetBuffer(layer, f, null);
            w.Commit(Strings.Command_ClearCel);
        }, HasPixelLayer);
        R("cel.link", Strings.Command_LinkCels, null, w =>
        {
            Sprite sprite = w.Sprite!;
            SpriteOps.Link(sprite, (PixelLayer)sprite.ActiveLayer!, sprite.RangeFrom, sprite.RangeTo);
            w.Commit(Strings.Command_LinkCels);
        }, w => HasPixelLayer(w) && w.Sprite!.RangeTo > w.Sprite.RangeFrom);
        R("cel.unlink", Strings.Command_UnlinkCels, null, w =>
        {
            Sprite sprite = w.Sprite!;
            for (int f = sprite.RangeFrom; f <= sprite.RangeTo; f++) SpriteOps.Unlink(sprite, (PixelLayer)sprite.ActiveLayer!, f);
            w.Commit(Strings.Command_UnlinkCels);
        }, w => HasPixelLayer(w) && Enumerable.Range(w.Sprite!.RangeFrom, w.Sprite.RangeTo - w.Sprite.RangeFrom + 1).Any(f => w.Sprite.IsLinked((PixelLayer)w.Sprite.ActiveLayer!, f)));
        R("cel.properties", Strings.Command_CelProperties, null, w => _ = SpriteDialogs.CelPropertiesAsync(w), w => HasPixelLayer(w) && w.Sprite!.GetCel((PixelLayer)w.Sprite.ActiveLayer!, w.Sprite.ActiveFrame) != null);
    }

    private static void NewFrame(PixelEditorWindow w, bool copy, bool linked, string name)
    {
        Sprite sprite = w.Sprite!;
        int index = sprite.ActiveFrame + 1;
        sprite.InsertFrame(index, copy ? sprite.ActiveFrame : null, linked);
        sprite.SetActiveFrame(index);
        w.Commit(name);
        w.GoToFrame(index);
    }

    private static void MoveFrames(PixelEditorWindow w, int direction)
    {
        Sprite sprite = w.Sprite!;
        int active = sprite.ActiveFrame - sprite.RangeFrom, from = sprite.RangeFrom, to = sprite.RangeTo;
        sprite.MoveFrames(from, to, from + direction);
        sprite.SetRange(from + direction, to + direction, from + direction + active);
        w.Commit(Strings.History_MoveFrames);
    }

    public static void SetFrameDuration(PixelEditorWindow w, int from, int to, int duration)
    {
        Sprite? sprite = w.Sprite;
        if (sprite == null) return;
        duration = Math.Clamp(duration, 1, 65535);
        bool changed = false;
        for (int f = from; f <= to && f < sprite.Frames.Count; f++)
        {
            if (sprite.Frames[f].Duration == duration) continue;
            sprite.Frames[f].Duration = duration;
            changed = true;
        }
        if (changed) w.Commit(Strings.History_FrameDuration);
    }

    // ------------------------------------------------------------------ select

    private static void RegisterSelect()
    {
        R("select.all", Strings.Command_SelectAll, "Ctrl+A", w => w.SetSelection(SelectionMask.All(w.Sprite!.Width, w.Sprite.Height)), HasSprite);
        R("select.none", Strings.Command_Deselect, "Ctrl+D", w => w.SetSelection(null), HasSelection);
        R("select.inverse", Strings.Command_InvertSelection, "Ctrl+Shift+I", w => w.SetSelection(w.Sprite!.Selection?.Invert() ?? SelectionMask.All(w.Sprite.Width, w.Sprite.Height)), HasSprite);
        R("select.byColor", Strings.Command_SelectByColor, null, w => SelectByColor(w, w.ForegroundColor), HasSprite);
        R("select.celContent", Strings.Command_SelectCelContent, null, w =>
        {
            Sprite sprite = w.Sprite!;
            PixelBuffer? buffer = sprite.GetBuffer((PixelLayer)sprite.ActiveLayer!, sprite.ActiveFrame);
            if (buffer == null)
            {
                w.SetSelection(null);
                return;
            }
            SelectionMask mask = new(sprite.Width, sprite.Height, buffer.Pixels.Select(p => p >> 24 != 0).ToArray());
            w.SetSelection(mask.IsEmpty ? null : mask);
        }, HasPixelLayer);
        void Modify(string id, string text, Func<SelectionMask, int, SelectionMask?> change)
        {
            R(id, text, null, async w =>
            {
                Form form = new(w);
                Func<int> amount = form.Number(Strings.Dialog_Pixels, 1, 256, 1);
                if (!await form.ShowAsync(text.Replace("...", string.Empty), 300) || w.Sprite?.Selection == null) return;
                w.SetSelection(change(w.Sprite.Selection, amount()));
            }, HasSelection);
        }
        Modify("select.grow", Strings.Command_GrowSelection, (s, n) => s.Grow(n));
        Modify("select.shrink", Strings.Command_ShrinkSelection, (s, n) => s.Grow(-n));
        Modify("select.border", Strings.Command_BorderSelection, (s, n) => s.Border(n));
    }

    /// <summary>Selects every pixel of a color in the active cel (or in the whole image when the active layer has no pixels).</summary>
    public static void SelectByColor(PixelEditorWindow w, uint color)
    {
        Sprite? sprite = w.Sprite;
        if (sprite == null) return;
        w.CommitFloating();
        PixelBuffer source = sprite.ActiveLayer is PixelLayer layer && sprite.GetBuffer(layer, sprite.ActiveFrame) is PixelBuffer buffer ? buffer : Compositor.Flatten(sprite, sprite.ActiveFrame, true);
        SelectionMask mask = new(sprite.Width, sprite.Height, source.Pixels.Select(p => p >> 24 != 0 && (p & 0xFFFFFF) == (color & 0xFFFFFF)).ToArray());
        w.SetSelection(mask.IsEmpty ? null : mask);
        if (mask.IsEmpty) w.SetStatus(Strings.Status_ColorNotFound);
    }
}
