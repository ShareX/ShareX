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

namespace ShareX.PhotoEditor.UI.Panels;

public sealed class InfoPanel : EditorPanel
{
    private static readonly IBrush DividerBrush = new SolidColorBrush(Color.FromRgb(0x47, 0x47, 0x47));

    private readonly Dictionary<string, TextBlock> _values = [];
    private readonly StackPanel _samplers = new() { Spacing = 2 };
    private readonly TextBlock _doc = new() { Classes = { "dim" }, FontSize = 11, TextWrapping = TextWrapping.Wrap };

    public InfoPanel(PhotoEditorWindow editor) : base(editor)
    {
        Grid quadrants = new() { ColumnDefinitions = new ColumnDefinitions("*,1,*"), RowDefinitions = new RowDefinitions("Auto,1,Auto") };
        AddCell(quadrants, 0, 0, "eyedropper", ["R", "G", "B", "A"]);
        AddCell(quadrants, 0, 2, null, ["H", "S", "Br", "#"]);
        AddCell(quadrants, 2, 0, "move", ["X", "Y"]);
        AddCell(quadrants, 2, 2, "rectangularmarquee", ["W", "H2"]);
        Border vertical = new() { Background = DividerBrush, Margin = new Thickness(0, 4) };
        Grid.SetColumn(vertical, 1);
        Grid.SetRowSpan(vertical, 3);
        Border horizontal = new() { Background = DividerBrush };
        Grid.SetRow(horizontal, 1);
        Grid.SetColumnSpan(horizontal, 3);
        quadrants.Children.Add(vertical);
        quadrants.Children.Add(horizontal);

        StackPanel root = new() { Margin = new Thickness(8, 6), Spacing = 8 };
        root.Children.Add(quadrants);
        root.Children.Add(_samplers);
        root.Children.Add(new Border { Height = 1, Background = DividerBrush });
        root.Children.Add(_doc);
        Content = new ScrollViewer { Content = root, HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled };
    }

    public override string Id => "info";

    public override string Title => "Info";

    private void AddCell(Grid parent, int row, int column, string? icon, string[] keys)
    {
        Grid cell = new() { ColumnDefinitions = new ColumnDefinitions("20,18,*"), Margin = new Thickness(column == 0 ? 0 : 8, 5, 4, 5) };
        for (int i = 0; i < keys.Length; i++)
        {
            cell.RowDefinitions.Add(new RowDefinition(new GridLength(17)));
            TextBlock label = new() { Text = keys[i].TrimEnd('2') switch { "Br" => "B", var k => k } + (keys[i] == "#" ? string.Empty : ":"), Classes = { "dim" }, VerticalAlignment = VerticalAlignment.Center };
            TextBlock value = new() { Text = string.Empty, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left };
            Grid.SetRow(label, i);
            Grid.SetColumn(label, 1);
            Grid.SetRow(value, i);
            Grid.SetColumn(value, 2);
            cell.Children.Add(label);
            cell.Children.Add(value);
            _values[keys[i]] = value;
        }
        if (icon != null)
        {
            Control image = ToolIcons.Create(icon, 14);
            image.VerticalAlignment = VerticalAlignment.Top;
            image.Margin = new Thickness(0, 1, 0, 0);
            image.Opacity = 0.8;
            Grid.SetRowSpan(image, keys.Length);
            cell.Children.Add(image);
        }
        Grid.SetRow(cell, row);
        Grid.SetColumn(cell, column);
        parent.Children.Add(cell);
    }

    private void Set(string key, string text) => _values[key].Text = text;

    public override void Refresh()
    {
        _samplers.Children.Clear();
        DocumentView? view = Editor.View;
        if (view?.Composite == null)
        {
            foreach (TextBlock value in _values.Values) value.Text = string.Empty;
            _doc.Text = string.Empty;
            return;
        }
        PhotoDocument doc = view.Document;
        SKPoint p = Editor.Canvas.LastDocPoint;
        int x = (int)MathF.Floor(p.X), y = (int)MathF.Floor(p.Y);
        if (x >= 0 && y >= 0 && x < doc.Width && y < doc.Height)
        {
            SKColor c = ColorMath.ToColor(ImageOps.GetPixel(view.Composite, x, y));
            ColorMath.RgbToHsv(c.Red / 255f, c.Green / 255f, c.Blue / 255f, out float h, out float s, out float v);
            Set("R", c.Red.ToString());
            Set("G", c.Green.ToString());
            Set("B", c.Blue.ToString());
            Set("A", c.Alpha.ToString());
            Set("H", $"{h:0}°");
            Set("S", $"{s * 100:0}%");
            Set("Br", $"{v * 100:0}%");
            Set("#", ColorMath.ToHex(c));
        }
        else
        {
            foreach (string key in new[] { "R", "G", "B", "A", "H", "S", "Br", "#" }) Set(key, string.Empty);
        }
        Set("X", $"{p.X:0.0}");
        Set("Y", $"{p.Y:0.0}");
        SKRectI sel = doc.SelectionBounds;
        Set("W", doc.Selection != null ? sel.Width.ToString() : string.Empty);
        Set("H2", doc.Selection != null ? sel.Height.ToString() : string.Empty);
        for (int i = 0; i < doc.Samplers.Count; i++)
        {
            ColorSamplerPoint sampler = doc.Samplers[i];
            SKColor c = ImageOps.SampleAverage(view.Composite, (int)sampler.Position.X, (int)sampler.Position.Y, 1);
            StackPanel row = new() { Orientation = Orientation.Horizontal, Spacing = 10 };
            row.Children.Add(new TextBlock { Text = $"#{i + 1}", Classes = { "dim" }, Width = 20 });
            row.Children.Add(new Border { Width = 12, Height = 12, Background = new SolidColorBrush(Color.FromRgb(c.Red, c.Green, c.Blue)), BorderBrush = DividerBrush, BorderThickness = new Thickness(1) });
            row.Children.Add(new TextBlock { Text = $"R: {c.Red}   G: {c.Green}   B: {c.Blue}" });
            _samplers.Children.Add(row);
        }
        _samplers.IsVisible = _samplers.Children.Count > 0;
        _doc.Text = $"Doc: {doc.Width} × {doc.Height} px @ {doc.Resolution:0} ppi\nLayers: {doc.AllLayers.Count()}   History states: {doc.History.Entries.Count}";
    }
}
