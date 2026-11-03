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
using ShareX.PhotoEditor.Tools;
using ShareX.PhotoEditor.UI.Dialogs;

namespace ShareX.PhotoEditor.UI.Panels;

public sealed class BrushesPanel : EditorPanel
{
    private readonly StackPanel _root = new() { Spacing = 6, Margin = new Thickness(8) };

    public BrushesPanel(PhotoEditorWindow editor) : base(editor)
    {
        Content = new ScrollViewer { Content = _root };
    }

    public override string Id => "brushes";

    public override string Title => "Brushes";

    public override void Refresh()
    {
        _root.Children.Clear();
        if (Editor.Tools.Active is not BrushToolBase tool)
        {
            _root.Children.Add(new TextBlock { Text = "Select a painting tool to edit its brush.", TextWrapping = TextWrapping.Wrap });
            ListBox presets = PresetList(null);
            _root.Children.Add(presets);
            return;
        }
        _root.Children.Add(Text(tool.Name, 12, true));
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Size", DialogBuilder.SliderNumber(1, 2500, tool.Size, v => { tool.Size = (float)v; Editor.RefreshOptions(); Editor.InvalidateOverlay(); }));
        DialogBuilder.AddRow(form, "Hardness", DialogBuilder.SliderNumber(0, 100, tool.Tip.Hardness * 100, v => tool.Tip.Hardness = (float)v / 100));
        DialogBuilder.AddRow(form, "Spacing", DialogBuilder.SliderNumber(1, 1000, tool.Tip.Spacing * 100, v => tool.Tip.Spacing = (float)v / 100));
        DialogBuilder.AddRow(form, "Angle", DialogBuilder.SliderNumber(-180, 180, tool.Tip.Angle, v => { tool.Tip.Angle = (float)v; Editor.InvalidateOverlay(); }));
        DialogBuilder.AddRow(form, "Roundness", DialogBuilder.SliderNumber(5, 100, tool.Tip.Roundness * 100, v => { tool.Tip.Roundness = (float)v / 100; Editor.InvalidateOverlay(); }));
        DialogBuilder.AddRow(form, "Size Jitter", DialogBuilder.SliderNumber(0, 100, tool.Tip.SizeJitter * 100, v => tool.Tip.SizeJitter = (float)v / 100));
        DialogBuilder.AddRow(form, "Scatter", DialogBuilder.SliderNumber(0, 1000, tool.Tip.Scatter * 100, v => tool.Tip.Scatter = (float)v / 100));
        _root.Children.Add(form);
        CheckBox square = new() { Content = "Square tip", IsChecked = tool.Tip.Shape == BrushTipShape.Square };
        square.IsCheckedChanged += (_, _) => tool.Tip.Shape = square.IsChecked == true ? BrushTipShape.Square : BrushTipShape.Round;
        _root.Children.Add(square);
        CheckBox pressureSize = new() { Content = "Pen pressure controls size", IsChecked = tool.Tip.PressureSize };
        pressureSize.IsCheckedChanged += (_, _) => tool.Tip.PressureSize = pressureSize.IsChecked == true;
        CheckBox pressureOpacity = new() { Content = "Pen pressure controls flow", IsChecked = tool.Tip.PressureOpacity };
        pressureOpacity.IsCheckedChanged += (_, _) => tool.Tip.PressureOpacity = pressureOpacity.IsChecked == true;
        _root.Children.Add(pressureSize);
        _root.Children.Add(pressureOpacity);
        Button save = new() { Content = "New Brush Preset..." };
        save.Click += async (_, _) =>
        {
            string? name = await DialogBuilder.PromptAsync(Editor, "Brush Name", "Name:", $"{tool.Tip.Name} {tool.Size:0}");
            if (string.IsNullOrWhiteSpace(name)) return;
            BrushPreset preset = tool.Tip.Clone();
            preset.Name = name;
            preset.Size = tool.Size;
            Editor.BrushPresets.Add(preset);
            Refresh();
        };
        _root.Children.Add(save);
        _root.Children.Add(PresetList(tool));
    }

    private ListBox PresetList(BrushToolBase? tool)
    {
        ListBox list = new() { MaxHeight = 260 };
        foreach (BrushPreset preset in Editor.BrushPresets) list.Items.Add(new ListBoxItem { Content = $"{preset.Name} ({preset.Size:0} px)", Tag = preset });
        list.SelectionChanged += (_, _) =>
        {
            if (tool == null || list.SelectedItem is not ListBoxItem { Tag: BrushPreset preset }) return;
            tool.Tip = preset.Clone();
            tool.Size = preset.Size;
            Editor.RefreshOptions();
            Refresh();
        };
        return list;
    }
}
