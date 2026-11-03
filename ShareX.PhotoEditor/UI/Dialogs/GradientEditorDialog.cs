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
using ShareX.PhotoEditor.Tools;

namespace ShareX.PhotoEditor.UI.Dialogs;

/// <summary>Edits gradient presets and color stops.</summary>
public static class GradientEditorDialog
{
    public static async Task<GradientDefinition?> ShowAsync(Window owner, GradientDefinition initial, IEditorHost host)
    {
        GradientDefinition gradient = initial.Clone();
        Border preview = new() { Height = 36, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
        StackPanel stops = new() { Spacing = 6 };
        TextBox name = new() { Text = gradient.Name };

        void RefreshPreview()
        {
            LinearGradientBrush brush = new() { StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative), EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative) };
            foreach (GradientStop stop in gradient.Stops.OrderBy(s => s.Position))
            {
                brush.GradientStops.Add(new Avalonia.Media.GradientStop(Color.FromArgb(stop.Color.Alpha, stop.Color.Red, stop.Color.Green, stop.Color.Blue), stop.Position));
            }
            preview.Background = brush;
        }

        void RebuildStops()
        {
            stops.Children.Clear();
            for (int i = 0; i < gradient.Stops.Count; i++)
            {
                int index = i;
                GradientStop stop = gradient.Stops[i];
                Border swatch = new() { Width = 28, Height = 18, Background = DialogBuilder.ToBrush(stop.Color), BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
                Button color = new() { Content = swatch, Padding = new Thickness(2) };
                color.Click += async (_, _) =>
                {
                    SKColor? picked = await ColorPickerDialog.ShowAsync(owner, gradient.Stops[index].Color, "Stop Color", host);
                    if (picked is SKColor c)
                    {
                        gradient.Stops[index] = gradient.Stops[index] with { Color = c };
                        RebuildStops();
                    }
                };
                Slider position = new() { Minimum = 0, Maximum = 100, Value = stop.Position * 100, Width = 220 };
                position.ValueChanged += (_, e) =>
                {
                    gradient.Stops[index] = gradient.Stops[index] with { Position = (float)(e.NewValue / 100) };
                    RefreshPreview();
                };
                Button remove = new() { Content = "Delete", IsEnabled = gradient.Stops.Count > 2 };
                remove.Click += (_, _) =>
                {
                    gradient.Stops.RemoveAt(index);
                    RebuildStops();
                };
                StackPanel row = new() { Orientation = Orientation.Horizontal, Spacing = 8 };
                row.Children.Add(color);
                row.Children.Add(new TextBlock { Text = "Location", VerticalAlignment = VerticalAlignment.Center });
                row.Children.Add(position);
                row.Children.Add(remove);
                stops.Children.Add(row);
            }
            RefreshPreview();
        }

        ListBox presets = new() { Height = 120 };
        foreach (GradientDefinition preset in GradientDefinition.CreatePresets(host.Foreground, host.Background))
        {
            presets.Items.Add(new ListBoxItem { Content = preset.Name, Tag = preset });
        }
        presets.SelectionChanged += (_, _) =>
        {
            if (presets.SelectedItem is ListBoxItem { Tag: GradientDefinition preset })
            {
                gradient = preset.Clone();
                name.Text = gradient.Name;
                RebuildStops();
            }
        };
        Button addStop = new() { Content = "Add Stop" };
        addStop.Click += (_, _) =>
        {
            gradient.Stops.Add(new GradientStop(0.5f, gradient.Sample(0.5f)));
            RebuildStops();
        };

        StackPanel content = new() { Spacing = 10 };
        content.Children.Add(new TextBlock { Text = "Presets" });
        content.Children.Add(presets);
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Name:", name);
        content.Children.Add(form);
        content.Children.Add(preview);
        content.Children.Add(stops);
        content.Children.Add(addStop);
        RebuildStops();

        bool ok = await DialogBuilder.ShowAsync(owner, "Gradient Editor", content, 520);
        if (!ok) return null;
        gradient.Name = string.IsNullOrWhiteSpace(name.Text) ? "Custom" : name.Text;
        // Editing the stops of a foreground preset turns it into a fixed custom gradient.
        if (gradient.IsDynamic)
        {
            GradientStop[] expected = gradient.ResolveColors(host.Foreground, host.Background).Stops.ToArray();
            if (!gradient.Stops.SequenceEqual(expected) && expected.Length > 0) gradient.Name = "Custom";
        }
        return gradient;
    }
}
