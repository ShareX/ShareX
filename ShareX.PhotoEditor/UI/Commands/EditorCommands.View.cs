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
using ShareX.PhotoEditor.UI.Dialogs;

namespace ShareX.PhotoEditor.UI.Commands;

public static partial class EditorCommands
{
    private static void RegisterView()
    {
        R("view.zoomIn", "Zoom In", "Ctrl+OemPlus, Ctrl+Add", w => Zoom(w, true), HasDoc);
        R("view.zoomOut", "Zoom Out", "Ctrl+OemMinus, Ctrl+Subtract", w => Zoom(w, false), HasDoc);
        R("view.fit", "Fit on Screen", "Ctrl+D0", w => { w.View!.FitOnScreen(); w.InvalidateOverlay(); }, HasDoc);
        R("view.fill", "Fill Screen", null, w => { w.View!.FillScreen(); w.InvalidateOverlay(); }, HasDoc);
        R("view.100", "100%", "Ctrl+D1, Ctrl+Alt+D0", w => { w.View!.ActualPixels(); w.InvalidateOverlay(); }, HasDoc);
        R("view.200", "200%", null, w => { w.View!.ZoomAt(2, Center(w)); w.InvalidateOverlay(); }, HasDoc);
        R("view.resetRotation", "Reset View Rotation", null, w => { w.View!.Rotation = 0; w.InvalidateOverlay(); }, HasDoc);
        R("view.channel.rgb", "RGB", "Ctrl+D2", w => SetChannelView(w, ChannelView.Composite), HasDoc, w => w.Document?.ChannelView == ChannelView.Composite);
        R("view.channel.red", "Red", "Ctrl+D3", w => SetChannelView(w, ChannelView.Red), HasDoc, w => w.Document?.ChannelView == ChannelView.Red);
        R("view.channel.green", "Green", "Ctrl+D4", w => SetChannelView(w, ChannelView.Green), HasDoc, w => w.Document?.ChannelView == ChannelView.Green);
        R("view.channel.blue", "Blue", "Ctrl+D5", w => SetChannelView(w, ChannelView.Blue), HasDoc, w => w.Document?.ChannelView == ChannelView.Blue);
        Toggle("view.extras", "Extras", "Ctrl+H", s => s.ShowSelectionEdges && s.ShowGuides, (s, v) => { s.ShowSelectionEdges = v; s.ShowGuides = v; s.ShowSlices = v; s.ShowNotes = v; s.ShowCount = v; });
        Toggle("view.show.selection", "Selection Edges", null, s => s.ShowSelectionEdges, (s, v) => s.ShowSelectionEdges = v);
        Toggle("view.show.grid", "Grid", "Ctrl+OemQuotes", s => s.ShowGrid, (s, v) => s.ShowGrid = v);
        Toggle("view.show.guides", "Guides", "Ctrl+OemSemicolon", s => s.ShowGuides, (s, v) => s.ShowGuides = v);
        Toggle("view.show.slices", "Slices", null, s => s.ShowSlices, (s, v) => s.ShowSlices = v);
        Toggle("view.show.notes", "Notes", null, s => s.ShowNotes, (s, v) => s.ShowNotes = v);
        Toggle("view.show.count", "Count", null, s => s.ShowCount, (s, v) => s.ShowCount = v);
        Toggle("view.show.pixelGrid", "Pixel Grid", null, s => s.ShowPixelGrid, (s, v) => s.ShowPixelGrid = v);
        Toggle("view.rulers", "Rulers", "Ctrl+R", s => s.ShowRulers, (s, v) => s.ShowRulers = v);
        Toggle("view.snap", "Snap", "Ctrl+Shift+OemSemicolon", s => s.Snap, (s, v) => s.Snap = v);
        Toggle("view.snap.guides", "Guides", null, s => s.SnapToGuides, (s, v) => s.SnapToGuides = v);
        Toggle("view.snap.grid", "Grid", null, s => s.SnapToGrid, (s, v) => s.SnapToGrid = v);
        Toggle("view.snap.document", "Document Bounds", null, s => s.SnapToDocument, (s, v) => s.SnapToDocument = v);
        Toggle("view.lockGuides", "Lock Guides", "Ctrl+Alt+OemSemicolon", s => s.LockGuides, (s, v) => s.LockGuides = v);
        R("view.clearGuides", "Clear Guides", null, w => { w.Document!.Guides.Clear(); w.Commit("Clear Guides"); }, w => w.Document?.Guides.Count > 0);
        R("view.newGuide", "New Guide...", null, w => _ = NewGuideAsync(w), HasDoc);
        R("view.newGuideLayout", "New Guide Layout...", null, w => _ = GuideLayoutAsync(w), HasDoc);
        R("view.guidesFromShape", "New Guides From Shape", null, GuidesFromShape, HasLayer);
    }

    private static void Toggle(string id, string text, string? gesture, Func<EditorSettings, bool> get, Action<EditorSettings, bool> set) =>
        R(id, text, gesture, w =>
        {
            set(w.Settings, !get(w.Settings));
            w.ApplyVisibility();
            w.InvalidateOverlay();
        }, null, w => get(w.Settings));

    private static Point Center(PhotoEditorWindow w) => new(w.View!.Viewport.Width / 2, w.View.Viewport.Height / 2);

    private static void Zoom(PhotoEditorWindow w, bool zoomIn)
    {
        w.View!.ZoomStep(zoomIn, Center(w));
        w.InvalidateOverlay();
        w.RefreshPanels();
    }

    private static async Task NewGuideAsync(PhotoEditorWindow w)
    {
        bool horizontal = true;
        double position = 0;
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Orientation:", DialogBuilder.Combo([("Horizontal", true), ("Vertical", false)], horizontal, v => horizontal = v));
        NumericUpDown number = DialogBuilder.Number(-30000, 30000, 0);
        number.ValueChanged += (_, e) => position = (double)(e.NewValue ?? 0);
        DialogBuilder.AddRow(form, "Position (px):", number);
        if (!await DialogBuilder.ShowAsync(w, "New Guide", form, 340)) return;
        w.Document!.Guides.Add(new Guide(horizontal, (float)position));
        w.Settings.ShowGuides = true;
        w.Commit("New Guide");
    }

    private static async Task GuideLayoutAsync(PhotoEditorWindow w)
    {
        int columns = 3, rows = 0;
        double gutter = 0, margin = 0;
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Columns:", DialogBuilder.SliderNumber(0, 50, columns, v => columns = (int)v));
        DialogBuilder.AddRow(form, "Rows:", DialogBuilder.SliderNumber(0, 50, rows, v => rows = (int)v));
        DialogBuilder.AddRow(form, "Gutter (px):", DialogBuilder.SliderNumber(0, 500, gutter, v => gutter = v));
        DialogBuilder.AddRow(form, "Margin (px):", DialogBuilder.SliderNumber(0, 1000, margin, v => margin = v));
        if (!await DialogBuilder.ShowAsync(w, "New Guide Layout", form, 420)) return;
        PhotoDocument doc = w.Document!;
        void AddAxis(int count, int size, bool horizontal)
        {
            if (count <= 0) return;
            double start = margin, end = size - margin;
            double cell = (end - start - gutter * (count - 1)) / count;
            doc.Guides.Add(new Guide(horizontal, (float)start));
            for (int i = 0; i < count; i++)
            {
                double a = start + i * (cell + gutter);
                if (i > 0) doc.Guides.Add(new Guide(horizontal, (float)a));
                if (gutter > 0 || i == count - 1) doc.Guides.Add(new Guide(horizontal, (float)(a + cell)));
            }
        }
        AddAxis(columns, doc.Width, false);
        AddAxis(rows, doc.Height, true);
        w.Settings.ShowGuides = true;
        w.Commit("New Guide Layout");
    }

    private static void GuidesFromShape(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        Layer layer = doc.ActiveLayer!;
        SKRect b = layer is RasterLayer r ? r.GetOpaqueBounds() : layer.GetContentBounds();
        if (b.IsEmpty) return;
        doc.Guides.AddRange([new Guide(false, b.Left), new Guide(false, b.MidX), new Guide(false, b.Right), new Guide(true, b.Top), new Guide(true, b.MidY), new Guide(true, b.Bottom)]);
        w.Settings.ShowGuides = true;
        w.Commit("New Guides From Shape");
    }

    private static void RegisterWindow()
    {
        foreach (var (id, title) in PanelIds)
        {
            string captured = id;
            R($"window.panel.{id}", title, id switch { "brushes" => "F5", "color" => "F6", "layers" => "F7", "info" => "F8", _ => null },
                w => w.Panels.TogglePanel(captured), null, w => w.Panels.IsPanelVisible(captured));
        }
        R("window.tools", "Tools", null, w => { w.Settings.ToolbarVisible = !w.Settings.ToolbarVisible; w.ApplyVisibility(); }, null, w => w.Settings.ToolbarVisible);
        R("window.toolsTwoColumns", "Two Column Toolbar", null, w => w.ToggleToolbarColumns(), null, w => w.Settings.ToolbarTwoColumns);
        R("window.options", "Options", null, w => { w.Settings.OptionsVisible = !w.Settings.OptionsVisible; w.ApplyVisibility(); }, null, w => w.Settings.OptionsVisible);
        R("window.dock", "Panels", null, w => { w.Settings.RightDockVisible = !w.Settings.RightDockVisible; w.ApplyVisibility(); }, null, w => w.Settings.RightDockVisible);
        R("window.workspace.reset", "Reset Essentials", null, w =>
        {
            w.Settings.ToolbarVisible = w.Settings.OptionsVisible = w.Settings.RightDockVisible = true;
            w.Settings.RightDockWidth = 300;
            w.Panels.ResetLayout();
            w.ApplyVisibility();
        });
        R("window.workspace.save", "Save Workspace", null, w => { w.SaveSettings(); w.SetStatus("Workspace saved."); });
        R("window.nextDocument", "Next Document", "Ctrl+Tab", w => CycleDocument(w, 1), w => w.Views.Count > 1);
        R("window.previousDocument", "Previous Document", "Ctrl+Shift+Tab", w => CycleDocument(w, -1), w => w.Views.Count > 1);
        R("help.about", "About Photo editor...", null, w => _ = DialogBuilder.ShowMessageAsync(w, "About Photo editor",
            "Photo editor for ShareX\n\nA layer-based image editor with selection, painting, retouching, vector, type and adjustment tools.\nPart of ShareX, licensed under the GNU General Public License v3."));
        R("help.shortcuts", "Keyboard Shortcuts...", null, w => _ = ShortcutsDialog.ShowAsync(w));
        R("help.tool", "Current Tool Help", "F1", w => _ = DialogBuilder.ShowMessageAsync(w, w.Tools.Active.Name, w.Tools.Active.Hint.Length > 0 ? w.Tools.Active.Hint : "No help is available for this tool."));
    }

    private static readonly (string Id, string Title)[] PanelIds =
    [
        ("adjustments", "Adjustments"), ("brushes", "Brushes"), ("channels", "Channels"), ("color", "Color"), ("history", "History"), ("info", "Info"),
        ("layers", "Layers"), ("navigator", "Navigator"), ("paths", "Paths"), ("properties", "Properties"), ("swatches", "Swatches")
    ];

    private static void CycleDocument(PhotoEditorWindow w, int delta)
    {
        int index = w.View != null ? w.Views.ToList().IndexOf(w.View) : 0;
        w.ActivateView(w.Views[((index + delta) % w.Views.Count + w.Views.Count) % w.Views.Count]);
    }

    private static IReadOnlyList<MenuNode> BuildMenu()
    {
        MenuNode C(string id) => MenuNode.Cmd(id);
        MenuNode S = MenuNode.Separator;
        return
        [
            MenuNode.Sub("_File",
                C("file.new"), C("file.open"), new MenuNode { Header = "Open Recent", Dynamic = RecentFiles }, S,
                C("file.newFromClipboard"), C("file.newFromRegion"), C("file.newFromScreen"), S,
                C("file.close"), C("file.closeAll"), C("file.save"), C("file.saveAs"), C("file.saveCopy"), C("file.revert"), S,
                MenuNode.Sub("Export", C("file.quickExport"), C("file.exportAs"), S, C("file.exportSlices"), C("file.exportLayers")),
                C("file.place"), S,
                MenuNode.Sub("Share with ShareX", C("file.runTasks"), C("file.upload"), C("file.pin")),
                C("file.print"), S, C("file.exit")),
            MenuNode.Sub("_Edit",
                C("edit.undo"), C("edit.redo"), C("edit.stepBackward"), C("edit.stepForward"), S, C("edit.fade"), S,
                C("edit.cut"), C("edit.copy"), C("edit.copyMerged"), C("edit.paste"),
                MenuNode.Sub("Paste Special", C("edit.pasteInPlace"), C("edit.pasteInto"), C("edit.pasteNew")), C("edit.clear"), S,
                C("edit.fill"), C("edit.stroke"), C("edit.contentAwareFill"), S,
                C("edit.freeTransform"),
                MenuNode.Sub("Transform", C("edit.transform.again"), S, C("edit.transform.scale"), C("edit.transform.rotate"), C("edit.transform.skew"), C("edit.transform.distort"), C("edit.transform.perspective"), S,
                    C("edit.transform.rotate180"), C("edit.transform.rotate90cw"), C("edit.transform.rotate90ccw"), S, C("edit.transform.flipH"), C("edit.transform.flipV")), S,
                C("edit.defineBrush"), C("edit.definePattern"), C("edit.defineShape"), S,
                MenuNode.Sub("Purge", C("edit.purge.history"), C("edit.purge.clipboard")), S,
                C("edit.toolbar"), C("edit.shortcuts"), C("edit.preferences")),
            MenuNode.Sub("_Image",
                MenuNode.Sub("Mode", C("image.mode.grayscale")), S,
                MenuNode.Sub("Adjustments", AdjustmentMenu.Select(a => C($"image.adjust.{a.Kind}")).Append(S).Append(C("image.adjust.shadowsHighlights")).Append(C("image.adjust.equalize")).ToArray()), S,
                C("image.autoTone"), C("image.autoContrast"), C("image.autoColor"), S,
                C("image.imageSize"), C("image.canvasSize"),
                MenuNode.Sub("Image Rotation", C("image.rotate180"), C("image.rotate90cw"), C("image.rotate90ccw"), C("image.rotateArbitrary"), S, C("image.flipH"), C("image.flipV")),
                C("image.crop"), C("image.trim"), C("image.revealAll"), S, C("image.duplicate")),
            MenuNode.Sub("_Layer",
                MenuNode.Sub("New", C("layer.new"), C("layer.newGroup"), S, C("layer.fromBackground"), C("layer.toBackground"), S, C("layer.newViaCopy"), C("layer.newViaCut")),
                C("layer.duplicate"), C("layer.delete"), C("layer.rename"), S,
                MenuNode.Sub("Layer Style", C("layer.style"), S, C("layer.style.copy"), C("layer.style.paste"), C("layer.style.clear"), C("layer.rasterizeStyle")),
                MenuNode.Sub("New Fill Layer", C("layer.fill.solid"), C("layer.fill.gradient"), C("layer.fill.pattern")),
                MenuNode.Sub("New Adjustment Layer", Enum.GetValues<AdjustmentKind>().Select(k => C($"layer.adjust.{k}")).ToArray()), S,
                MenuNode.Sub("Layer Mask", C("layer.mask.revealAll"), C("layer.mask.hideAll"), C("layer.mask.revealSelection"), C("layer.mask.hideSelection"), C("layer.mask.fromTransparency"), S,
                    C("layer.mask.delete"), C("layer.mask.apply"), S, C("layer.mask.toggle"), C("layer.mask.edit")),
                C("layer.clip"), S,
                MenuNode.Sub("Smart Objects", C("layer.smart.convert"), C("layer.rasterize")),
                C("layer.rasterize"), S,
                C("layer.group"), C("layer.ungroup"), C("layer.hide"), S,
                MenuNode.Sub("Arrange", C("layer.arrange.front"), C("layer.arrange.forward"), C("layer.arrange.backward"), C("layer.arrange.back")),
                MenuNode.Sub("Align", C("layer.align.left"), C("layer.align.hcenter"), C("layer.align.right"), S, C("layer.align.top"), C("layer.align.vcenter"), C("layer.align.bottom")),
                C("layer.lock"), S,
                C("layer.mergeDown"), C("layer.mergeVisible"), C("layer.stampVisible"), C("layer.flatten")),
            MenuNode.Sub("_Type",
                MenuNode.Sub("Orientation", C("type.horizontal"), C("type.vertical")),
                MenuNode.Sub("Anti-Alias", Enum.GetValues<TextAntiAlias>().Select(a => C($"type.aa.{a}")).ToArray()),
                C("type.sizeUp"), C("type.sizeDown"), S,
                C("type.createWorkPath"), C("type.convertToShape"), C("type.rasterize")),
            MenuNode.Sub("_Select",
                C("select.all"), C("select.deselect"), C("select.reselect"), C("select.inverse"), S, C("select.allLayers"), S,
                C("select.colorRange"), C("select.subject"), S, C("select.selectAndMask"),
                MenuNode.Sub("Modify", C("select.modify.border"), C("select.modify.smooth"), C("select.modify.expand"), C("select.modify.contract"), C("select.modify.feather")), S,
                C("select.grow"), C("select.similar"), S, C("select.transform"), S, C("select.quickMask"), S, C("select.load"), C("select.save"), C("paths.fromSelection")),
            new MenuNode
            {
                Header = "Fil_ter",
                Children = new[] { C("filter.last"), S, C("filter.convertSmart"), S }.Concat(FilterCategoryNodes()).Append(S).ToList(),
                Dynamic = w => [new MenuNode { Header = "Effects Library", Children = LibraryNodes(w).ToList() }]
            },
            MenuNode.Sub("_View",
                C("view.zoomIn"), C("view.zoomOut"), C("view.fit"), C("view.fill"), C("view.100"), C("view.200"), C("view.resetRotation"), S,
                MenuNode.Sub("Channel", C("view.channel.rgb"), C("view.channel.red"), C("view.channel.green"), C("view.channel.blue")),
                C("view.screenMode"), S, C("view.extras"),
                MenuNode.Sub("Show", C("view.show.selection"), C("view.show.grid"), C("view.show.guides"), C("view.show.slices"), C("view.show.notes"), C("view.show.count"), C("view.show.pixelGrid")), S,
                C("view.rulers"), S, C("view.snap"), MenuNode.Sub("Snap To", C("view.snap.guides"), C("view.snap.grid"), C("view.snap.document")), S,
                C("view.lockGuides"), C("view.clearGuides"), C("view.newGuide"), C("view.newGuideLayout"), C("view.guidesFromShape")),
            new MenuNode
            {
                Header = "_Window",
                Children = new[] { MenuNode.Sub("Workspace", C("window.workspace.reset"), C("window.workspace.save")), S }
                    .Concat(PanelIds.Select(p => C($"window.panel.{p.Id}")))
                    .Concat([S, C("window.options"), C("window.tools"), C("window.toolsTwoColumns"), C("window.dock"), S, C("window.nextDocument"), C("window.previousDocument"), S]).ToList(),
                Dynamic = w => w.Views.Select(v => new MenuNode { Header = v.Document.DisplayName, Action = win => win.ActivateView(v) })
            },
            MenuNode.Sub("_Help", C("help.tool"), C("help.shortcuts"), S, C("help.about"))
        ];
    }
}
