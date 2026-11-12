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

using ShareX.PhotoEditor.UI;

namespace ShareX.PhotoEditor.Tools;

/// <summary>Horizontal/vertical type tools and their type mask variants.</summary>
public sealed class TypeTool : ToolBase
{
    private readonly bool _vertical;
    private readonly bool _mask;
    private SKPoint _start;
    private SKPoint _current;
    private bool _dragging;

    private static readonly string[] FontFamilies = SKFontManager.Default.FontFamilies.OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToArray();

    public TypeTool(IEditorHost host, bool vertical, bool mask) : base(host)
    {
        _vertical = vertical;
        _mask = mask;
    }

    public override ToolId Id => (_vertical, _mask) switch
    {
        (false, false) => ToolId.HorizontalType,
        (true, false) => ToolId.VerticalType,
        (false, true) => ToolId.HorizontalTypeMask,
        _ => ToolId.VerticalTypeMask
    };

    public override string Name => _mask ? (_vertical ? Strings.TypeTools_VerticalTypeMaskTool : Strings.TypeTools_HorizontalTypeMaskTool) : (_vertical ? Strings.TypeTools_VerticalTypeTool : Strings.TypeTools_HorizontalTypeTool);

    public override string Hint => _mask
        ? Strings.TypeTools_MaskHint
        : Strings.TypeTools_Hint;

    public override ToolCursor Cursor => ToolCursor.Text;

    public override bool HasPendingOperation => Host.IsTextEditing;

    /// <summary>Shared character settings for all type tools.</summary>
    public static TextLayer Settings { get; } = new("settings") { FontFamily = "Segoe UI", FontSize = 48 };

    public bool IsMask => _mask;

    /// <summary>Switching to another tool commits the text being edited.</summary>
    public override void Commit() => Host.EndTextEditing(true);

    public override void Cancel() => Host.EndTextEditing(false);

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        if (Host.IsTextEditing)
        {
            Host.EndTextEditing(true);
            return;
        }
        if (!_mask)
        {
            TextLayer? hit = Doc.AllLayers.OfType<TextLayer>().LastOrDefault(t => t.IsEffectivelyVisible && t.GetContentBounds().Contains(input.Doc));
            if (hit != null)
            {
                Doc.ActiveLayer = hit;
                Host.RefreshPanels();
                Host.BeginTextEditing(hit, false);
                return;
            }
        }
        _start = _current = input.Doc;
        _dragging = true;
    }

    public override void PointerMove(ToolInput input)
    {
        if (!_dragging) return;
        _current = input.Doc;
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input)
    {
        if (!_dragging || Doc == null) return;
        _dragging = false;
        SKRect box = DragRect(_start, _current, false, false);
        TextLayer layer = new(_mask ? Strings.TypeTools_TypeMask : Strings.LayerNames_NewTypeLayer);
        Settings.CopyTextPropertiesTo(layer);
        layer.Text = string.Empty;
        layer.Color = Host.Foreground;
        layer.Vertical = _vertical;
        layer.Transform = SKMatrix.Identity;
        if (box.Width > 8 && box.Height > 8)
        {
            layer.Position = new SKPoint(box.Left, box.Top);
            layer.BoxSize = box.Size;
        }
        else
        {
            using SKFont font = layer.CreateFont();
            // A click places the baseline at the pointer, like the reference workflow.
            layer.Position = new SKPoint(_start.X, _start.Y + font.Metrics.Ascent);
            layer.BoxSize = SKSize.Empty;
        }
        Doc.AddLayer(layer);
        Host.InvalidateOverlay();
        Host.BeginTextEditing(layer, true);
    }

    /// <summary>Called by the editor when text editing ends for a layer created or edited by this tool.</summary>
    public bool FinishMaskEditing(TextLayer layer, bool commit)
    {
        if (!_mask || Doc == null) return false;
        if (commit && !string.IsNullOrWhiteSpace(layer.Text))
        {
            using SKPath path = layer.GetTextPath();
            PixelSurface mask = MaskOps.FromPath(path, Doc.Width, Doc.Height, layer.AntiAlias != TextAntiAlias.None);
            Doc.RemoveLayer(layer);
            Doc.SetSelection(MaskOps.Combine(Doc.Selection, mask, SelectionMode.Replace));
            PushHistory(Strings.TypeTools_TypeMask);
        }
        else
        {
            Doc.RemoveLayer(layer);
        }
        Host.InvalidateCanvas();
        return true;
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (_dragging && SKPoint.Distance(_start, _current) > 4) context.Rect(DragRect(_start, _current, false, false));
        if (Host.IsTextEditing && Doc?.ActiveLayer is TextLayer text && text.IsParagraph)
        {
            context.Rect(SKRect.Create(text.Position, text.BoxSize), OverlayContext.AccentPen);
        }
    }

    /// <summary>Applies a change to the shared settings and to the text layer being edited (or the active text layer).</summary>
    private void Change(Action<TextLayer> apply, string historyName)
    {
        apply(Settings);
        if (Doc?.ActiveLayer is TextLayer layer)
        {
            apply(layer);
            layer.Invalidate();
            if (!Host.IsTextEditing) PushHistory(historyName);
            Host.InvalidateCanvas();
        }
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        TextLayer current = Doc?.ActiveLayer as TextLayer ?? Settings;
        options.IconButton(PhotoEditorIcons.TextOrientation, Strings.TypeTools_ToggleOrientationTip, () => Change(t => t.Vertical = !t.Vertical, Strings.EditorCommands_ChangeTextOrientation));
        options.Combo(null, FontFamilies.Select(f => (f, f)), () => current.FontFamily, v => Change(t => t.FontFamily = v, Strings.PropertiesPanel_SetFont), 170);
        options.Combo(null, [(Strings.TypeTools_Regular, (false, false)), (Strings.PropertiesPanel_Bold, (true, false)), (Strings.PropertiesPanel_Italic, (false, true)), (Strings.TypeTools_BoldItalic, (true, true))],
            () => (current.Bold, current.Italic), v => Change(t => { t.Bold = v.Item1; t.Italic = v.Item2; }, Strings.TypeTools_SetFontStyle), 100);
        options.Number(Strings.PropertiesPanel_SizeColon, 1, 1296, () => current.FontSize, v => Change(t => t.FontSize = (float)v, Strings.EditorCommands_SetFontSize), "0.#", "px", 1, 64);
        options.EnumCombo("aa", () => current.AntiAlias, v => Change(t => t.AntiAlias = v, Strings.TypeTools_SetAntiAlias), 92);
        options.Separator();
        options.IconButton(PhotoEditorIcons.TextAlignLeft, Strings.TypeTools_LeftAlignTip, () => Change(t => t.Alignment = TextAlignment.Left, Strings.PropertiesPanel_AlignText), current.Alignment == TextAlignment.Left);
        options.IconButton(PhotoEditorIcons.TextAlignCenter, Strings.TypeTools_CenterTip, () => Change(t => t.Alignment = TextAlignment.Center, Strings.PropertiesPanel_AlignText), current.Alignment == TextAlignment.Center);
        options.IconButton(PhotoEditorIcons.TextAlignRight, Strings.TypeTools_RightAlignTip, () => Change(t => t.Alignment = TextAlignment.Right, Strings.PropertiesPanel_AlignText), current.Alignment == TextAlignment.Right);
        if (!_mask) options.ColorButton(Strings.TypeTools_TextColorTip, () => current.Color, c => { Host.Foreground = c; Change(t => t.Color = c, Strings.PropertiesPanel_SetTextColor); });
        options.Separator();
        options.Number(Strings.PropertiesPanel_TrackingColon, -1000, 10000, () => current.Tracking, v => Change(t => t.Tracking = (float)v, Strings.PropertiesPanel_SetTracking), "0", null, 10, 60);
        options.Number(Strings.PropertiesPanel_LeadingColon, 0.5, 5, () => current.Leading, v => Change(t => t.Leading = (float)v, Strings.PropertiesPanel_SetLeading), "0.00", null, 0.05, 56);
        options.IconButton(PhotoEditorIcons.FauxBold, Strings.TypeTools_FauxBold, () => Change(t => t.FauxBold = !t.FauxBold, Strings.TypeTools_FauxBold), current.FauxBold);
        options.IconButton(PhotoEditorIcons.FauxItalic, Strings.TypeTools_FauxItalic, () => Change(t => t.FauxItalic = !t.FauxItalic, Strings.TypeTools_FauxItalic), current.FauxItalic);
        options.IconButton(PhotoEditorIcons.Underline, Strings.PropertiesPanel_Underline, () => Change(t => t.Underline = !t.Underline, Strings.PropertiesPanel_Underline), current.Underline);
        options.IconButton(PhotoEditorIcons.Strikethrough, Strings.TypeTools_Strikethrough, () => Change(t => t.Strikethrough = !t.Strikethrough, Strings.TypeTools_Strikethrough), current.Strikethrough);
        options.IconButton(PhotoEditorIcons.AllCaps, Strings.TypeTools_AllCaps, () => Change(t => t.AllCaps = !t.AllCaps, Strings.TypeTools_AllCaps), current.AllCaps);
        if (Host.IsTextEditing)
        {
            options.Separator();
            options.Button(Strings.DialogBuilder_Cancel, () => Host.EndTextEditing(false), Strings.TypeTools_CancelTip);
            options.Button(Strings.Tools_Commit, () => Host.EndTextEditing(true), Strings.TypeTools_CommitTip);
        }
    }
}
