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
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace ShareX.PhotoEditor.UI.Dialogs;

public static class LayerStyleDialog
{
    /// <summary>Edits the layer's style live; returns true when OK is pressed.</summary>
    public static async Task<bool> ShowAsync(PhotoEditorWindow owner, Layer layer)
    {
        LayerStyle style = layer.Style!;
        void Changed() => owner.InvalidateCanvas();
        Control ColorButton(Func<SKColor> get, Action<SKColor> set, string title)
        {
            Button button = new() { Content = new Border { Width = 40, Height = 16, Background = DialogBuilder.ToBrush(get()) } };
            button.Click += async (_, _) =>
            {
                Window? window = TopLevel.GetTopLevel(button) as Window;
                if (window == null) return;
                SKColor? picked = await ColorPickerDialog.ShowAsync(window, get(), title, owner);
                if (picked is SKColor c)
                {
                    set(c);
                    button.Content = new Border { Width = 40, Height = 16, Background = DialogBuilder.ToBrush(c) };
                    Changed();
                }
            };
            return button;
        }

        Grid blending = DialogBuilder.Form();
        DialogBuilder.AddRow(blending, "Blend Mode:", DialogBuilder.Combo(BlendModes.LayerModes.Select(m => (BlendModes.GetName(m), m)), layer.BlendMode, v => { layer.BlendMode = v; Changed(); }));
        DialogBuilder.AddRow(blending, "Opacity:", DialogBuilder.SliderNumber(0, 100, layer.Opacity * 100, v => { layer.Opacity = (float)v / 100; Changed(); }));
        DialogBuilder.AddRow(blending, "Fill Opacity:", DialogBuilder.SliderNumber(0, 100, layer.FillOpacity * 100, v => { layer.FillOpacity = (float)v / 100; Changed(); }));

        Grid shadow = DialogBuilder.Form();
        DialogBuilder.AddRow(shadow, "Color:", ColorButton(() => style.ShadowColor, c => style.ShadowColor = c, "Shadow Color"));
        DialogBuilder.AddRow(shadow, "Opacity:", DialogBuilder.SliderNumber(0, 100, style.ShadowOpacity * 100, v => { style.ShadowOpacity = (float)v / 100; Changed(); }));
        DialogBuilder.AddRow(shadow, "Angle:", DialogBuilder.SliderNumber(-180, 180, style.ShadowAngle, v => { style.ShadowAngle = (float)v; Changed(); }));
        DialogBuilder.AddRow(shadow, "Distance:", DialogBuilder.SliderNumber(0, 300, style.ShadowDistance, v => { style.ShadowDistance = (float)v; Changed(); }));
        DialogBuilder.AddRow(shadow, "Size:", DialogBuilder.SliderNumber(0, 250, style.ShadowSize, v => { style.ShadowSize = (float)v; Changed(); }));

        Grid inner = DialogBuilder.Form();
        DialogBuilder.AddRow(inner, "Color:", ColorButton(() => style.InnerShadowColor, c => style.InnerShadowColor = c, "Inner Shadow Color"));
        DialogBuilder.AddRow(inner, "Opacity:", DialogBuilder.SliderNumber(0, 100, style.InnerShadowOpacity * 100, v => { style.InnerShadowOpacity = (float)v / 100; Changed(); }));
        DialogBuilder.AddRow(inner, "Distance:", DialogBuilder.SliderNumber(0, 300, style.InnerShadowDistance, v => { style.InnerShadowDistance = (float)v; Changed(); }));
        DialogBuilder.AddRow(inner, "Size:", DialogBuilder.SliderNumber(0, 250, style.InnerShadowSize, v => { style.InnerShadowSize = (float)v; Changed(); }));

        Grid glow = DialogBuilder.Form();
        DialogBuilder.AddRow(glow, "Color:", ColorButton(() => style.GlowColor, c => style.GlowColor = c, "Glow Color"));
        DialogBuilder.AddRow(glow, "Opacity:", DialogBuilder.SliderNumber(0, 100, style.GlowOpacity * 100, v => { style.GlowOpacity = (float)v / 100; Changed(); }));
        DialogBuilder.AddRow(glow, "Size:", DialogBuilder.SliderNumber(0, 250, style.GlowSize, v => { style.GlowSize = (float)v; Changed(); }));

        Grid stroke = DialogBuilder.Form();
        DialogBuilder.AddRow(stroke, "Size:", DialogBuilder.SliderNumber(1, 250, style.StrokeSize, v => { style.StrokeSize = (float)v; Changed(); }));
        DialogBuilder.AddRow(stroke, "Position:", DialogBuilder.Combo(Enum.GetValues<StrokePosition>().Select(p => (p.ToString(), p)), style.StrokePosition, v => { style.StrokePosition = v; Changed(); }));
        DialogBuilder.AddRow(stroke, "Opacity:", DialogBuilder.SliderNumber(0, 100, style.StrokeOpacity * 100, v => { style.StrokeOpacity = (float)v / 100; Changed(); }));
        DialogBuilder.AddRow(stroke, "Color:", ColorButton(() => style.StrokeColor, c => style.StrokeColor = c, "Stroke Color"));

        Grid overlay = DialogBuilder.Form();
        DialogBuilder.AddRow(overlay, "Color:", ColorButton(() => style.OverlayColor, c => style.OverlayColor = c, "Overlay Color"));
        DialogBuilder.AddRow(overlay, "Opacity:", DialogBuilder.SliderNumber(0, 100, style.OverlayOpacity * 100, v => { style.OverlayOpacity = (float)v / 100; Changed(); }));

        // Reference-style layout: a list of styles with enable checkboxes on the left, settings on the right.
        (string Title, Func<bool>? Get, Action<bool>? Set, Grid Body)[] sections =
        [
            ("Blending Options", null, null, blending),
            ("Stroke", () => style.Stroke, v => style.Stroke = v, stroke),
            ("Inner Shadow", () => style.InnerShadow, v => style.InnerShadow = v, inner),
            ("Color Overlay", () => style.ColorOverlay, v => style.ColorOverlay = v, overlay),
            ("Outer Glow", () => style.OuterGlow, v => style.OuterGlow = v, glow),
            ("Drop Shadow", () => style.DropShadow, v => style.DropShadow = v, shadow)
        ];
        TextBlock detailTitle = new() { Classes = { "header" }, FontSize = 13, Margin = new Thickness(0, 0, 0, 10) };
        ContentControl detail = new();
        StackPanel list = new() { Spacing = 0 };
        List<Border> rows = [];
        void Select(int index)
        {
            for (int i = 0; i < rows.Count; i++) rows[i].Background = i == index ? new SolidColorBrush(Color.FromRgb(0x4B, 0x4B, 0x4B)) : Brushes.Transparent;
            detailTitle.Text = sections[index].Title;
            detail.Content = sections[index].Body;
        }
        for (int i = 0; i < sections.Length; i++)
        {
            int index = i;
            var section = sections[i];
            Control label;
            if (section.Get != null)
            {
                CheckBox check = new() { Content = section.Title, IsChecked = section.Get() };
                check.IsCheckedChanged += (_, _) => { section.Set!(check.IsChecked == true); Changed(); Select(index); };
                label = check;
            }
            else
            {
                label = new TextBlock { Text = section.Title, FontWeight = FontWeight.SemiBold, Margin = new Thickness(4, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            }
            Border row = new() { Child = label, Height = 30, Padding = new Thickness(8, 0), Cursor = new Cursor(StandardCursorType.Hand) };
            row.PointerPressed += (_, _) => Select(index);
            rows.Add(row);
            list.Children.Add(row);
        }
        Border sidebar = new()
        {
            Width = 180,
            Background = new SolidColorBrush(Color.FromRgb(0x28, 0x28, 0x28)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E)),
            BorderThickness = new Thickness(1),
            Child = list,
            Padding = new Thickness(0, 4)
        };
        StackPanel detailPanel = new() { Margin = new Thickness(16, 4, 4, 4), MinWidth = 380 };
        detailPanel.Children.Add(detailTitle);
        detailPanel.Children.Add(detail);
        Grid layout = new() { ColumnDefinitions = new ColumnDefinitions("Auto,*"), MinHeight = 340 };
        Grid.SetColumn(detailPanel, 1);
        layout.Children.Add(sidebar);
        layout.Children.Add(detailPanel);
        Select(0);
        return await DialogBuilder.ShowAsync(owner, "Layer Style", layout, 640);
    }
}
