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
using Avalonia.Media;
using Avalonia.Threading;
using ShareX.PhotoEditor.UI.Commands;
using ShareX.PhotoEditor.UI.Dialogs;

namespace ShareX.PhotoEditor.UI.Panels;

/// <summary>Context-sensitive properties of the document or the active layer (text, shape, adjustment, mask, smart object, frame, artboard).</summary>
public sealed class PropertiesPanel : EditorPanel
{
    private readonly StackPanel _root = new() { Spacing = 8, Margin = new Thickness(8) };
    private readonly DispatcherTimer _commitTimer;
    private string? _pendingHistory;

    public PropertiesPanel(PhotoEditorWindow editor) : base(editor)
    {
        Content = new ScrollViewer { Content = _root };
        // Slider drags produce many changes; record a single history state when they settle.
        _commitTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(600) };
        _commitTimer.Tick += (_, _) =>
        {
            _commitTimer.Stop();
            if (_pendingHistory != null && Doc != null)
            {
                Doc.History.Push(_pendingHistory);
                _pendingHistory = null;
                Editor.RefreshPanelsExcept(this);
            }
        };
    }

    public override string Id => "properties";

    public override string Title => Strings.EditorCommands_Properties;

    private void Changed(string history)
    {
        Editor.InvalidateCanvas();
        _pendingHistory = history;
        _commitTimer.Stop();
        _commitTimer.Start();
    }

    public override void Refresh()
    {
        _root.Children.Clear();
        PhotoDocument? doc = Doc;
        if (doc == null)
        {
            _root.Children.Add(Text(Strings.PropertiesPanel_NoDocument));
            return;
        }
        Layer? layer = doc.ActiveLayer;
        if (doc.EditingMask && layer?.Mask != null)
        {
            BuildMask(doc, layer);
            return;
        }
        switch (layer)
        {
            case TextLayer text: BuildText(text); break;
            case ShapeLayer shape: BuildShape(shape); break;
            case AdjustmentLayer adjustment: BuildAdjustment(doc, adjustment); break;
            case FillLayer fill: BuildFill(fill); break;
            case SmartObjectLayer smart: BuildSmart(smart); break;
            case FrameLayer frame: BuildFrame(frame); break;
            case GroupLayer { IsArtboard: true } artboard: BuildArtboard(artboard); break;
            case RasterLayer raster: BuildRaster(raster); break;
            default: BuildDocument(doc); break;
        }
        if (layer != null && layer is not AdjustmentLayer)
        {
            _root.Children.Add(new Separator());
            Button style = new() { Content = Strings.EditorCommands_LayerStyleEllipsis };
            style.Click += (_, _) => _ = EditorCommands.LayerStyle(Editor);
            _root.Children.Add(style);
        }
    }

    private void Header(string text) => _root.Children.Add(Text(text, 13, true));

    private void BuildDocument(PhotoDocument doc)
    {
        Header(Strings.PropertiesPanel_Document);
        _root.Children.Add(Text(string.Format(Strings.PropertiesPanel_DocumentSize, doc.Width, doc.Height)));
        _root.Children.Add(Text(string.Format(Strings.PropertiesPanel_Resolution, doc.Resolution)));
        Button imageSize = new() { Content = Strings.EditorCommands_ImageSizeEllipsis };
        imageSize.Click += (_, _) => Editor.Execute("image.imageSize");
        Button canvasSize = new() { Content = Strings.EditorCommands_CanvasSizeEllipsis };
        canvasSize.Click += (_, _) => Editor.Execute("image.canvasSize");
        _root.Children.Add(Row(imageSize, canvasSize));
    }

    private void BuildRaster(RasterLayer raster)
    {
        Header(raster.IsBackground ? Strings.LayerNames_Background : Strings.PropertiesPanel_PixelLayer);
        SKRectI bounds = raster.GetOpaqueBounds();
        _root.Children.Add(Text(bounds.IsEmpty ? Strings.PropertiesPanel_EmptyLayer : $"W: {bounds.Width} px   H: {bounds.Height} px\nX: {bounds.Left} px   Y: {bounds.Top} px"));
        if (raster.IsBackground)
        {
            Button convert = new() { Content = Strings.PropertiesPanel_ConvertToLayer };
            convert.Click += (_, _) => Editor.Execute("layer.fromBackground");
            _root.Children.Add(convert);
        }
        Button smart = new() { Content = Strings.EditorCommands_ConvertToSmartObject };
        smart.Click += (_, _) => Editor.Execute("layer.smart.convert");
        _root.Children.Add(smart);
        if (raster.Mask == null)
        {
            Button mask = new() { Content = Strings.PropertiesPanel_AddMask };
            mask.Click += (_, _) => EditorCommands.AddMask(Editor, MaskMode.RevealAll);
            _root.Children.Add(mask);
        }
    }

    private void BuildText(TextLayer text)
    {
        Header(Strings.PropertiesPanel_Character);
        Grid form = DialogBuilder.Form();
        TextBox content = new() { Text = text.Text, AcceptsReturn = true, MinHeight = 60, TextWrapping = TextWrapping.Wrap };
        content.TextChanged += (_, _) => { text.Text = content.Text ?? string.Empty; text.Invalidate(); Changed(Strings.PropertiesPanel_EditTypeLayer); };
        DialogBuilder.AddFull(form, content);
        ComboBox font = DialogBuilder.Combo(SKFontManager.Default.FontFamilies.OrderBy(f => f).Select(f => (f, f)), text.FontFamily, v => { text.FontFamily = v; text.Invalidate(); Changed(Strings.PropertiesPanel_SetFont); });
        DialogBuilder.AddRow(form, Strings.PropertiesPanel_FontColon, font);
        DialogBuilder.AddRow(form, Strings.PropertiesPanel_SizeColon, DialogBuilder.SliderNumber(1, 1296, text.FontSize, v => { text.FontSize = (float)v; text.Invalidate(); Changed(Strings.EditorCommands_SetFontSize); }, 0.5, "0.#"));
        DialogBuilder.AddRow(form, Strings.PropertiesPanel_LeadingColon, DialogBuilder.SliderNumber(0.5, 5, text.Leading, v => { text.Leading = (float)v; text.Invalidate(); Changed(Strings.PropertiesPanel_SetLeading); }, 0.05, "0.00"));
        DialogBuilder.AddRow(form, Strings.PropertiesPanel_TrackingColon, DialogBuilder.SliderNumber(-1000, 10000, text.Tracking, v => { text.Tracking = (float)v; text.Invalidate(); Changed(Strings.PropertiesPanel_SetTracking); }, 10));
        Button color = new() { Content = new Border { Width = 40, Height = 16, Background = DialogBuilder.ToBrush(text.Color) } };
        color.Click += async (_, _) =>
        {
            SKColor? c = await ColorPickerDialog.ShowAsync(Editor, text.Color, Strings.PropertiesPanel_TextColor, Editor);
            if (c is SKColor value) { text.Color = value; text.Invalidate(); Editor.Commit(Strings.PropertiesPanel_SetTextColor); }
        };
        DialogBuilder.AddRow(form, Strings.EditorCommands_ColorColon, color);
        CheckBox bold = new() { Content = Strings.PropertiesPanel_Bold, IsChecked = text.Bold };
        bold.IsCheckedChanged += (_, _) => { text.Bold = bold.IsChecked == true; text.Invalidate(); Changed(Strings.PropertiesPanel_Bold); };
        CheckBox italic = new() { Content = Strings.PropertiesPanel_Italic, IsChecked = text.Italic };
        italic.IsCheckedChanged += (_, _) => { text.Italic = italic.IsChecked == true; text.Invalidate(); Changed(Strings.PropertiesPanel_Italic); };
        CheckBox underline = new() { Content = Strings.PropertiesPanel_Underline, IsChecked = text.Underline };
        underline.IsCheckedChanged += (_, _) => { text.Underline = underline.IsChecked == true; text.Invalidate(); Changed(Strings.PropertiesPanel_Underline); };
        DialogBuilder.AddFull(form, Row(bold, italic, underline));
        _root.Children.Add(form);
        Header(Strings.PropertiesPanel_Paragraph);
        _root.Children.Add(DialogBuilder.Combo(Enum.GetValues<TextAlignment>().Select(a => (EnumNames.Get(a), a)), text.Alignment, v => { text.Alignment = v; text.Invalidate(); Editor.Commit(Strings.PropertiesPanel_AlignText); }));
        Button rasterize = new() { Content = Strings.PropertiesPanel_RasterizeType };
        rasterize.Click += (_, _) => Editor.Execute("layer.rasterize");
        Button shape = new() { Content = Strings.EditorCommands_ConvertToShape };
        shape.Click += (_, _) => Editor.Execute("type.convertToShape");
        _root.Children.Add(Row(rasterize, shape));
    }

    private void BuildShape(ShapeLayer shape)
    {
        Header(Strings.PropertiesPanel_ShapeProperties);
        SKRect bounds = shape.Path.GetBounds();
        _root.Children.Add(Text($"W: {bounds.Width:0.#} px   H: {bounds.Height:0.#} px\nX: {bounds.Left:0.#} px   Y: {bounds.Top:0.#} px"));
        Grid form = DialogBuilder.Form();
        Button fill = new() { Content = new Border { Width = 40, Height = 16, Background = shape.FillColor is SKColor f ? DialogBuilder.ToBrush(f) : Brushes.Transparent } };
        fill.Click += async (_, _) =>
        {
            SKColor? c = await ColorPickerDialog.ShowAsync(Editor, shape.FillColor ?? SKColors.Gray, Strings.EditorCommands_Fill, Editor);
            if (c is SKColor value) { shape.FillColor = value; Editor.Commit(Strings.PropertiesPanel_SetFill); }
        };
        CheckBox noFill = new() { Content = Strings.PropertiesPanel_None, IsChecked = shape.FillColor == null };
        noFill.IsCheckedChanged += (_, _) => { shape.FillColor = noFill.IsChecked == true ? null : SKColors.Gray; Editor.Commit(Strings.PropertiesPanel_SetFill); };
        DialogBuilder.AddRow(form, Strings.PropertiesPanel_FillColon, Row(fill, noFill));
        Button stroke = new() { Content = new Border { Width = 40, Height = 16, Background = shape.StrokeColor is SKColor s ? DialogBuilder.ToBrush(s) : Brushes.Transparent } };
        stroke.Click += async (_, _) =>
        {
            SKColor? c = await ColorPickerDialog.ShowAsync(Editor, shape.StrokeColor ?? SKColors.Black, Strings.EditorCommands_Stroke, Editor);
            if (c is SKColor value) { shape.StrokeColor = value; Editor.Commit(Strings.PropertiesPanel_SetStroke); }
        };
        CheckBox noStroke = new() { Content = Strings.PropertiesPanel_None, IsChecked = shape.StrokeColor == null };
        noStroke.IsCheckedChanged += (_, _) => { shape.StrokeColor = noStroke.IsChecked == true ? null : SKColors.Black; Editor.Commit(Strings.PropertiesPanel_SetStroke); };
        DialogBuilder.AddRow(form, Strings.PropertiesPanel_StrokeColon, Row(stroke, noStroke));
        DialogBuilder.AddRow(form, Strings.EditorCommands_WidthColon, DialogBuilder.SliderNumber(0, 200, shape.StrokeWidth, v => { shape.StrokeWidth = (float)v; Changed(Strings.PropertiesPanel_SetStrokeWidth); }, 0.5, "0.#"));
        DialogBuilder.AddRow(form, Strings.PropertiesPanel_AlignColon, DialogBuilder.Combo(Enum.GetValues<StrokePosition>().Select(p => (EnumNames.Get(p), p)), shape.StrokeAlignment, v => { shape.StrokeAlignment = v; Editor.Commit(Strings.PropertiesPanel_SetStrokeAlignment); }));
        CheckBox dashed = new() { Content = Strings.PropertiesPanel_Dashed, IsChecked = shape.Dashed };
        dashed.IsCheckedChanged += (_, _) => { shape.Dashed = dashed.IsChecked == true; Editor.Commit(Strings.PropertiesPanel_SetStrokeType); };
        DialogBuilder.AddFull(form, dashed);
        _root.Children.Add(form);
    }

    private void BuildAdjustment(PhotoDocument doc, AdjustmentLayer adjustment)
    {
        Header(AdjustmentSettings.GetName(adjustment.Settings.Kind));
        SKBitmap? histogram = Editor.View?.Composite;
        _root.Children.Add(AdjustmentEditor.Build(adjustment.Settings, () => Changed(string.Format(Strings.PropertiesPanel_ModifyAdjustment, AdjustmentSettings.GetName(adjustment.Settings.Kind))), histogram, Editor));
        CheckBox clip = new() { Content = Strings.PropertiesPanel_ClipToLayerBelow, IsChecked = adjustment.IsClipped };
        clip.IsCheckedChanged += (_, _) => { adjustment.IsClipped = clip.IsChecked == true; Editor.Commit(Strings.PropertiesPanel_ClippingMask); };
        _root.Children.Add(clip);
        _ = doc;
    }

    private void BuildFill(FillLayer fill)
    {
        Header(Strings.PropertiesPanel_FillLayer);
        _root.Children.Add(DialogBuilder.Combo(Enum.GetValues<FillKind>().Select(k => (EnumNames.Get(k), k)), fill.FillKind, v => { fill.FillKind = v; Editor.Commit(Strings.PropertiesPanel_ChangeFill); Refresh(); }));
        switch (fill.FillKind)
        {
            case FillKind.SolidColor:
            {
                Button color = new() { Content = new Border { Width = 60, Height = 20, Background = DialogBuilder.ToBrush(fill.Color) } };
                color.Click += async (_, _) =>
                {
                    SKColor? c = await ColorPickerDialog.ShowAsync(Editor, fill.Color, Strings.EditorCommands_SolidColor, Editor);
                    if (c is SKColor value) { fill.Color = value; Editor.Commit(Strings.PropertiesPanel_ChangeFill); Refresh(); }
                };
                _root.Children.Add(color);
                break;
            }
            case FillKind.Gradient:
            {
                Button edit = new() { Content = Strings.AdjustmentEditor_EditGradientEllipsis };
                edit.Click += async (_, _) =>
                {
                    GradientDefinition? g = await GradientEditorDialog.ShowAsync(Editor, fill.Gradient, Editor);
                    if (g != null) { g.Angle = fill.Gradient.Angle; fill.Gradient = g; Editor.Commit(Strings.PropertiesPanel_ChangeFill); }
                };
                _root.Children.Add(edit);
                _root.Children.Add(DialogBuilder.SliderNumber(-180, 180, fill.Gradient.Angle, v => { fill.Gradient.Angle = (float)v; Changed(Strings.PropertiesPanel_ChangeFill); }));
                break;
            }
            case FillKind.Pattern:
                _root.Children.Add(DialogBuilder.Combo(Editor.Patterns.Select(p => (p.Name, p)), fill.Pattern ?? Editor.Patterns[0], v => { fill.Pattern = v; Editor.Commit(Strings.PropertiesPanel_ChangeFill); }));
                _root.Children.Add(DialogBuilder.SliderNumber(1, 1000, fill.PatternScale * 100, v => { fill.PatternScale = (float)v / 100; Changed(Strings.PropertiesPanel_ChangeFill); }));
                fill.Pattern ??= Editor.Patterns[0];
                break;
        }
    }

    private void BuildSmart(SmartObjectLayer smart)
    {
        Header(Strings.PropertiesPanel_SmartObject);
        _root.Children.Add(Text($"Source: {smart.Source.Width} × {smart.Source.Height} px\nTransforms are non-destructive."));
        if (smart.Filters.Count > 0)
        {
            Header(Strings.PropertiesPanel_SmartFilters);
            foreach (SmartFilter filter in smart.Filters)
            {
                CheckBox enabled = new() { Content = filter.Name, IsChecked = filter.Enabled };
                enabled.IsCheckedChanged += (_, _) => { filter.Enabled = enabled.IsChecked == true; Editor.Commit(Strings.PropertiesPanel_ToggleSmartFilter); };
                Button remove = new() { Classes = { "icon" }, Content = new IconView(PhotoEditorIcons.Close, 10), Padding = new Thickness(5), VerticalAlignment = VerticalAlignment.Center };
                ToolTip.SetTip(remove, Strings.PropertiesPanel_DeleteSmartFilterTip);
                remove.Click += (_, _) => { smart.Filters.Remove(filter); Editor.Commit(Strings.PropertiesPanel_DeleteSmartFilter); Refresh(); };
                _root.Children.Add(Row(enabled, remove));
            }
        }
        Button reset = new() { Content = Strings.PropertiesPanel_ResetTransform };
        reset.Click += (_, _) => { smart.Transform = SKMatrix.Identity; Editor.Commit(Strings.PropertiesPanel_ResetTransform); };
        Button rasterize = new() { Content = Strings.EditorCommands_Rasterize };
        rasterize.Click += (_, _) => Editor.Execute("layer.rasterize");
        _root.Children.Add(Row(reset, rasterize));
    }

    private void BuildFrame(FrameLayer frame)
    {
        Header(Strings.PropertiesPanel_Frame);
        _root.Children.Add(Text($"W: {frame.Frame.Width:0} px   H: {frame.Frame.Height:0} px\n{(frame.Content != null ? Strings.PropertiesPanel_ContainsImage : Strings.PropertiesPanel_EmptyFrame)}"));
        Button place = new() { Content = Strings.PropertiesPanel_PlaceImageEllipsis };
        place.Click += (_, _) => EditorCommands.PlaceIntoFrame(Editor);
        _root.Children.Add(place);
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, Strings.PropertiesPanel_StrokeColon, DialogBuilder.SliderNumber(0, 100, frame.StrokeWidth, v => { frame.StrokeWidth = (float)v; if (frame.StrokeColor.Alpha == 0) frame.StrokeColor = SKColors.Black; Changed(Strings.PropertiesPanel_FrameStroke); }));
        _root.Children.Add(form);
    }

    private void BuildArtboard(GroupLayer artboard)
    {
        Header(Strings.PropertiesPanel_Artboard);
        SKRectI b = artboard.ArtboardBounds!.Value;
        Grid form = DialogBuilder.Form();
        NumericUpDown w = DialogBuilder.Number(1, 30000, b.Width), h = DialogBuilder.Number(1, 30000, b.Height), x = DialogBuilder.Number(-30000, 30000, b.Left), y = DialogBuilder.Number(-30000, 30000, b.Top);
        void Apply()
        {
            artboard.ArtboardBounds = SKRectI.Create((int)(x.Value ?? 0), (int)(y.Value ?? 0), (int)(w.Value ?? 1), (int)(h.Value ?? 1));
            Changed(Strings.PropertiesPanel_ResizeArtboard);
        }
        foreach (NumericUpDown box in new[] { w, h, x, y }) box.ValueChanged += (_, _) => Apply();
        DialogBuilder.AddRow(form, "W:", w);
        DialogBuilder.AddRow(form, "H:", h);
        DialogBuilder.AddRow(form, "X:", x);
        DialogBuilder.AddRow(form, "Y:", y);
        Button background = new() { Content = new Border { Width = 40, Height = 16, Background = DialogBuilder.ToBrush(artboard.ArtboardBackground) } };
        background.Click += async (_, _) =>
        {
            SKColor? c = await ColorPickerDialog.ShowAsync(Editor, artboard.ArtboardBackground, Strings.PropertiesPanel_ArtboardBackground, Editor);
            if (c is SKColor value) { artboard.ArtboardBackground = value; Editor.Commit(Strings.PropertiesPanel_ArtboardBackground); Refresh(); }
        };
        DialogBuilder.AddRow(form, Strings.PropertiesPanel_BackgroundColon, background);
        _root.Children.Add(form);
    }

    private void BuildMask(PhotoDocument doc, Layer layer)
    {
        Header(Strings.EditorCommands_LayerMask);
        _root.Children.Add(Text(Strings.PropertiesPanel_MaskEditHint));
        Button feather = new() { Content = Strings.EditorCommands_FeatherEllipsis };
        feather.Click += async (_, _) =>
        {
            string? text = await DialogBuilder.PromptAsync(Editor, Strings.PropertiesPanel_FeatherMask, Strings.PropertiesPanel_RadiusPxColon, "5");
            if (float.TryParse(text, out float radius) && radius > 0 && layer.Mask != null)
            {
                MaskOps.Blur(layer.GetWritableMask(doc.Width, doc.Height), radius);
                Editor.Commit(Strings.PropertiesPanel_FeatherMask);
            }
        };
        Button invert = new() { Content = Strings.PropertiesPanel_Invert };
        invert.Click += (_, _) =>
        {
            MaskOps.Invert(layer.GetWritableMask(doc.Width, doc.Height));
            Editor.Commit(Strings.PropertiesPanel_InvertMask);
        };
        Button toSelection = new() { Content = Strings.PropertiesPanel_LoadAsSelection };
        toSelection.Click += (_, _) =>
        {
            doc.SetSelection(layer.Mask!.Clone());
            Editor.Commit(Strings.EditorCommands_LoadSelection);
        };
        Button apply = new() { Content = Strings.PropertiesPanel_ApplyMask };
        apply.Click += (_, _) => Editor.Execute("layer.mask.apply");
        Button disable = new() { Content = layer.MaskEnabled ? Strings.EditorCommands_Disable : Strings.PropertiesPanel_Enable };
        disable.Click += (_, _) => { layer.MaskEnabled = !layer.MaskEnabled; Editor.Commit(layer.MaskEnabled ? Strings.EditorCommands_EnableLayerMask : Strings.EditorCommands_DisableLayerMask); };
        Button delete = new() { Content = Strings.PropertiesPanel_DeleteMask };
        delete.Click += (_, _) => Editor.Execute("layer.mask.delete");
        _root.Children.Add(Row(feather, invert));
        _root.Children.Add(Row(toSelection, apply));
        _root.Children.Add(Row(disable, delete));
    }
}
