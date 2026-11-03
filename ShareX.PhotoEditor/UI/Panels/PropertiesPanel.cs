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

    public override string Title => "Properties";

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
            _root.Children.Add(Text("No document"));
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
            Button style = new() { Content = "Layer Style..." };
            style.Click += (_, _) => _ = EditorCommands.LayerStyle(Editor);
            _root.Children.Add(style);
        }
    }

    private void Header(string text) => _root.Children.Add(Text(text, 13, true));

    private void BuildDocument(PhotoDocument doc)
    {
        Header("Document");
        _root.Children.Add(Text($"W: {doc.Width} px   H: {doc.Height} px"));
        _root.Children.Add(Text($"Resolution: {doc.Resolution:0} ppi"));
        Button imageSize = new() { Content = "Image Size..." };
        imageSize.Click += (_, _) => Editor.Execute("image.imageSize");
        Button canvasSize = new() { Content = "Canvas Size..." };
        canvasSize.Click += (_, _) => Editor.Execute("image.canvasSize");
        _root.Children.Add(Row(imageSize, canvasSize));
    }

    private void BuildRaster(RasterLayer raster)
    {
        Header(raster.IsBackground ? "Background" : "Pixel Layer");
        SKRectI bounds = raster.GetOpaqueBounds();
        _root.Children.Add(Text(bounds.IsEmpty ? "Empty layer" : $"W: {bounds.Width} px   H: {bounds.Height} px\nX: {bounds.Left} px   Y: {bounds.Top} px"));
        if (raster.IsBackground)
        {
            Button convert = new() { Content = "Convert to Layer" };
            convert.Click += (_, _) => Editor.Execute("layer.fromBackground");
            _root.Children.Add(convert);
        }
        Button smart = new() { Content = "Convert to Smart Object" };
        smart.Click += (_, _) => Editor.Execute("layer.smart.convert");
        _root.Children.Add(smart);
        if (raster.Mask == null)
        {
            Button mask = new() { Content = "Add Mask" };
            mask.Click += (_, _) => EditorCommands.AddMask(Editor, MaskMode.RevealAll);
            _root.Children.Add(mask);
        }
    }

    private void BuildText(TextLayer text)
    {
        Header("Character");
        Grid form = DialogBuilder.Form();
        TextBox content = new() { Text = text.Text, AcceptsReturn = true, MinHeight = 60, TextWrapping = TextWrapping.Wrap };
        content.TextChanged += (_, _) => { text.Text = content.Text ?? string.Empty; text.Invalidate(); Changed("Edit Type Layer"); };
        DialogBuilder.AddFull(form, content);
        ComboBox font = DialogBuilder.Combo(SKFontManager.Default.FontFamilies.OrderBy(f => f).Select(f => (f, f)), text.FontFamily, v => { text.FontFamily = v; text.Invalidate(); Changed("Set Font"); });
        DialogBuilder.AddRow(form, "Font:", font);
        DialogBuilder.AddRow(form, "Size:", DialogBuilder.SliderNumber(1, 1296, text.FontSize, v => { text.FontSize = (float)v; text.Invalidate(); Changed("Set Font Size"); }, 0.5, "0.#"));
        DialogBuilder.AddRow(form, "Leading:", DialogBuilder.SliderNumber(0.5, 5, text.Leading, v => { text.Leading = (float)v; text.Invalidate(); Changed("Set Leading"); }, 0.05, "0.00"));
        DialogBuilder.AddRow(form, "Tracking:", DialogBuilder.SliderNumber(-1000, 10000, text.Tracking, v => { text.Tracking = (float)v; text.Invalidate(); Changed("Set Tracking"); }, 10));
        Button color = new() { Content = new Border { Width = 40, Height = 16, Background = DialogBuilder.ToBrush(text.Color) } };
        color.Click += async (_, _) =>
        {
            SKColor? c = await ColorPickerDialog.ShowAsync(Editor, text.Color, "Text Color", Editor);
            if (c is SKColor value) { text.Color = value; text.Invalidate(); Editor.Commit("Set Text Color"); }
        };
        DialogBuilder.AddRow(form, "Color:", color);
        CheckBox bold = new() { Content = "Bold", IsChecked = text.Bold };
        bold.IsCheckedChanged += (_, _) => { text.Bold = bold.IsChecked == true; text.Invalidate(); Changed("Bold"); };
        CheckBox italic = new() { Content = "Italic", IsChecked = text.Italic };
        italic.IsCheckedChanged += (_, _) => { text.Italic = italic.IsChecked == true; text.Invalidate(); Changed("Italic"); };
        CheckBox underline = new() { Content = "Underline", IsChecked = text.Underline };
        underline.IsCheckedChanged += (_, _) => { text.Underline = underline.IsChecked == true; text.Invalidate(); Changed("Underline"); };
        DialogBuilder.AddFull(form, Row(bold, italic, underline));
        _root.Children.Add(form);
        Header("Paragraph");
        _root.Children.Add(DialogBuilder.Combo(Enum.GetValues<TextAlignment>().Select(a => (a.ToString(), a)), text.Alignment, v => { text.Alignment = v; text.Invalidate(); Editor.Commit("Align Text"); }));
        Button rasterize = new() { Content = "Rasterize Type" };
        rasterize.Click += (_, _) => Editor.Execute("layer.rasterize");
        Button shape = new() { Content = "Convert to Shape" };
        shape.Click += (_, _) => Editor.Execute("type.convertToShape");
        _root.Children.Add(Row(rasterize, shape));
    }

    private void BuildShape(ShapeLayer shape)
    {
        Header("Shape Properties");
        SKRect bounds = shape.Path.GetBounds();
        _root.Children.Add(Text($"W: {bounds.Width:0.#} px   H: {bounds.Height:0.#} px\nX: {bounds.Left:0.#} px   Y: {bounds.Top:0.#} px"));
        Grid form = DialogBuilder.Form();
        Button fill = new() { Content = new Border { Width = 40, Height = 16, Background = shape.FillColor is SKColor f ? DialogBuilder.ToBrush(f) : Brushes.Transparent } };
        fill.Click += async (_, _) =>
        {
            SKColor? c = await ColorPickerDialog.ShowAsync(Editor, shape.FillColor ?? SKColors.Gray, "Fill", Editor);
            if (c is SKColor value) { shape.FillColor = value; Editor.Commit("Set Fill"); }
        };
        CheckBox noFill = new() { Content = "None", IsChecked = shape.FillColor == null };
        noFill.IsCheckedChanged += (_, _) => { shape.FillColor = noFill.IsChecked == true ? null : SKColors.Gray; Editor.Commit("Set Fill"); };
        DialogBuilder.AddRow(form, "Fill:", Row(fill, noFill));
        Button stroke = new() { Content = new Border { Width = 40, Height = 16, Background = shape.StrokeColor is SKColor s ? DialogBuilder.ToBrush(s) : Brushes.Transparent } };
        stroke.Click += async (_, _) =>
        {
            SKColor? c = await ColorPickerDialog.ShowAsync(Editor, shape.StrokeColor ?? SKColors.Black, "Stroke", Editor);
            if (c is SKColor value) { shape.StrokeColor = value; Editor.Commit("Set Stroke"); }
        };
        CheckBox noStroke = new() { Content = "None", IsChecked = shape.StrokeColor == null };
        noStroke.IsCheckedChanged += (_, _) => { shape.StrokeColor = noStroke.IsChecked == true ? null : SKColors.Black; Editor.Commit("Set Stroke"); };
        DialogBuilder.AddRow(form, "Stroke:", Row(stroke, noStroke));
        DialogBuilder.AddRow(form, "Width:", DialogBuilder.SliderNumber(0, 200, shape.StrokeWidth, v => { shape.StrokeWidth = (float)v; Changed("Set Stroke Width"); }, 0.5, "0.#"));
        DialogBuilder.AddRow(form, "Align:", DialogBuilder.Combo(Enum.GetValues<StrokePosition>().Select(p => (p.ToString(), p)), shape.StrokeAlignment, v => { shape.StrokeAlignment = v; Editor.Commit("Set Stroke Alignment"); }));
        CheckBox dashed = new() { Content = "Dashed", IsChecked = shape.Dashed };
        dashed.IsCheckedChanged += (_, _) => { shape.Dashed = dashed.IsChecked == true; Editor.Commit("Set Stroke Type"); };
        DialogBuilder.AddFull(form, dashed);
        _root.Children.Add(form);
    }

    private void BuildAdjustment(PhotoDocument doc, AdjustmentLayer adjustment)
    {
        Header(AdjustmentSettings.GetName(adjustment.Settings.Kind));
        SKBitmap? histogram = Editor.View?.Composite;
        _root.Children.Add(AdjustmentEditor.Build(adjustment.Settings, () => Changed("Modify " + AdjustmentSettings.GetName(adjustment.Settings.Kind)), histogram, Editor));
        CheckBox clip = new() { Content = "Clip to layer below", IsChecked = adjustment.IsClipped };
        clip.IsCheckedChanged += (_, _) => { adjustment.IsClipped = clip.IsChecked == true; Editor.Commit("Clipping Mask"); };
        _root.Children.Add(clip);
        _ = doc;
    }

    private void BuildFill(FillLayer fill)
    {
        Header("Fill Layer");
        _root.Children.Add(DialogBuilder.Combo(Enum.GetValues<FillKind>().Select(k => (OptionsBuilder.SplitWords(k.ToString()), k)), fill.FillKind, v => { fill.FillKind = v; Editor.Commit("Change Fill"); Refresh(); }));
        switch (fill.FillKind)
        {
            case FillKind.SolidColor:
            {
                Button color = new() { Content = new Border { Width = 60, Height = 20, Background = DialogBuilder.ToBrush(fill.Color) } };
                color.Click += async (_, _) =>
                {
                    SKColor? c = await ColorPickerDialog.ShowAsync(Editor, fill.Color, "Solid Color", Editor);
                    if (c is SKColor value) { fill.Color = value; Editor.Commit("Change Fill"); Refresh(); }
                };
                _root.Children.Add(color);
                break;
            }
            case FillKind.Gradient:
            {
                Button edit = new() { Content = "Edit Gradient..." };
                edit.Click += async (_, _) =>
                {
                    GradientDefinition? g = await GradientEditorDialog.ShowAsync(Editor, fill.Gradient, Editor);
                    if (g != null) { g.Angle = fill.Gradient.Angle; fill.Gradient = g; Editor.Commit("Change Fill"); }
                };
                _root.Children.Add(edit);
                _root.Children.Add(DialogBuilder.SliderNumber(-180, 180, fill.Gradient.Angle, v => { fill.Gradient.Angle = (float)v; Changed("Change Fill"); }));
                break;
            }
            case FillKind.Pattern:
                _root.Children.Add(DialogBuilder.Combo(Editor.Patterns.Select(p => (p.Name, p)), fill.Pattern ?? Editor.Patterns[0], v => { fill.Pattern = v; Editor.Commit("Change Fill"); }));
                _root.Children.Add(DialogBuilder.SliderNumber(1, 1000, fill.PatternScale * 100, v => { fill.PatternScale = (float)v / 100; Changed("Change Fill"); }));
                fill.Pattern ??= Editor.Patterns[0];
                break;
        }
    }

    private void BuildSmart(SmartObjectLayer smart)
    {
        Header("Smart Object");
        _root.Children.Add(Text($"Source: {smart.Source.Width} × {smart.Source.Height} px\nTransforms are non-destructive."));
        if (smart.Filters.Count > 0)
        {
            Header("Smart Filters");
            foreach (SmartFilter filter in smart.Filters)
            {
                CheckBox enabled = new() { Content = filter.Name, IsChecked = filter.Enabled };
                enabled.IsCheckedChanged += (_, _) => { filter.Enabled = enabled.IsChecked == true; Editor.Commit("Toggle Smart Filter"); };
                Button remove = new() { Content = "✕", Padding = new Thickness(4, 0) };
                remove.Click += (_, _) => { smart.Filters.Remove(filter); Editor.Commit("Delete Smart Filter"); Refresh(); };
                _root.Children.Add(Row(enabled, remove));
            }
        }
        Button reset = new() { Content = "Reset Transform" };
        reset.Click += (_, _) => { smart.Transform = SKMatrix.Identity; Editor.Commit("Reset Transform"); };
        Button rasterize = new() { Content = "Rasterize" };
        rasterize.Click += (_, _) => Editor.Execute("layer.rasterize");
        _root.Children.Add(Row(reset, rasterize));
    }

    private void BuildFrame(FrameLayer frame)
    {
        Header("Frame");
        _root.Children.Add(Text($"W: {frame.Frame.Width:0} px   H: {frame.Frame.Height:0} px\n{(frame.Content != null ? "Contains an image" : "Empty frame")}"));
        Button place = new() { Content = "Place Image..." };
        place.Click += (_, _) => EditorCommands.PlaceIntoFrame(Editor);
        _root.Children.Add(place);
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Stroke:", DialogBuilder.SliderNumber(0, 100, frame.StrokeWidth, v => { frame.StrokeWidth = (float)v; if (frame.StrokeColor.Alpha == 0) frame.StrokeColor = SKColors.Black; Changed("Frame Stroke"); }));
        _root.Children.Add(form);
    }

    private void BuildArtboard(GroupLayer artboard)
    {
        Header("Artboard");
        SKRectI b = artboard.ArtboardBounds!.Value;
        Grid form = DialogBuilder.Form();
        NumericUpDown w = DialogBuilder.Number(1, 30000, b.Width), h = DialogBuilder.Number(1, 30000, b.Height), x = DialogBuilder.Number(-30000, 30000, b.Left), y = DialogBuilder.Number(-30000, 30000, b.Top);
        void Apply()
        {
            artboard.ArtboardBounds = SKRectI.Create((int)(x.Value ?? 0), (int)(y.Value ?? 0), (int)(w.Value ?? 1), (int)(h.Value ?? 1));
            Changed("Resize Artboard");
        }
        foreach (NumericUpDown box in new[] { w, h, x, y }) box.ValueChanged += (_, _) => Apply();
        DialogBuilder.AddRow(form, "W:", w);
        DialogBuilder.AddRow(form, "H:", h);
        DialogBuilder.AddRow(form, "X:", x);
        DialogBuilder.AddRow(form, "Y:", y);
        Button background = new() { Content = new Border { Width = 40, Height = 16, Background = DialogBuilder.ToBrush(artboard.ArtboardBackground) } };
        background.Click += async (_, _) =>
        {
            SKColor? c = await ColorPickerDialog.ShowAsync(Editor, artboard.ArtboardBackground, "Artboard Background", Editor);
            if (c is SKColor value) { artboard.ArtboardBackground = value; Editor.Commit("Artboard Background"); Refresh(); }
        };
        DialogBuilder.AddRow(form, "Background:", background);
        _root.Children.Add(form);
    }

    private void BuildMask(PhotoDocument doc, Layer layer)
    {
        Header("Layer Mask");
        _root.Children.Add(Text("Painting tools now edit the mask: black hides, white reveals."));
        Button feather = new() { Content = "Feather..." };
        feather.Click += async (_, _) =>
        {
            string? text = await DialogBuilder.PromptAsync(Editor, "Feather Mask", "Radius (px):", "5");
            if (float.TryParse(text, out float radius) && radius > 0 && layer.Mask != null)
            {
                MaskOps.Blur(layer.GetWritableMask(doc.Width, doc.Height), radius);
                Editor.Commit("Feather Mask");
            }
        };
        Button invert = new() { Content = "Invert" };
        invert.Click += (_, _) =>
        {
            MaskOps.Invert(layer.GetWritableMask(doc.Width, doc.Height));
            Editor.Commit("Invert Mask");
        };
        Button toSelection = new() { Content = "Load as Selection" };
        toSelection.Click += (_, _) =>
        {
            doc.SetSelection(layer.Mask!.Clone());
            Editor.Commit("Load Selection");
        };
        Button apply = new() { Content = "Apply Mask" };
        apply.Click += (_, _) => Editor.Execute("layer.mask.apply");
        Button disable = new() { Content = layer.MaskEnabled ? "Disable" : "Enable" };
        disable.Click += (_, _) => { layer.MaskEnabled = !layer.MaskEnabled; Editor.Commit(layer.MaskEnabled ? "Enable Layer Mask" : "Disable Layer Mask"); };
        Button delete = new() { Content = "Delete Mask" };
        delete.Click += (_, _) => Editor.Execute("layer.mask.delete");
        _root.Children.Add(Row(feather, invert));
        _root.Children.Add(Row(toSelection, apply));
        _root.Children.Add(Row(disable, delete));
    }
}
