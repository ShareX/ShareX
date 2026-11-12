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
using ShareX.PhotoEditor.UI.Dialogs;

namespace ShareX.PixelEditor.UI.Dialogs;

/// <summary>A labeled form for the editor's dialogs. Each field returns a function that reads its current value.</summary>
public sealed class Form
{
    private readonly Window _owner;

    public Form(Window owner)
    {
        _owner = owner;
    }

    public Grid Grid { get; } = DialogBuilder.Form();

    /// <summary>Raised when any field changes, for live previews.</summary>
    public event Action? Changed;

    public void RaiseChanged() => Changed?.Invoke();

    public Func<int> Number(string label, int min, int max, int value, int increment = 1, Action<NumericUpDown>? configure = null)
    {
        NumericUpDown box = DialogBuilder.Number(min, max, value, increment);
        box.ShowButtonSpinner = true;
        box.Width = 124;
        box.ValueChanged += (_, _) => Changed?.Invoke();
        configure?.Invoke(box);
        DialogBuilder.AddRow(Grid, label, box);
        return () => (int)(box.Value ?? value);
    }

    public Func<int> Slider(string label, int min, int max, int value)
    {
        int current = value;
        DialogBuilder.AddRow(Grid, label, DialogBuilder.SliderNumber(min, max, value, v =>
        {
            current = (int)Math.Round(v);
            Changed?.Invoke();
        }));
        return () => current;
    }

    public Func<bool> Check(string label, bool value)
    {
        CheckBox box = new() { Content = label, IsChecked = value };
        box.IsCheckedChanged += (_, _) => Changed?.Invoke();
        DialogBuilder.AddFull(Grid, box);
        return () => box.IsChecked == true;
    }

    public Func<T> Combo<T>(string label, IEnumerable<(string Text, T Value)> items, T value)
    {
        T current = value;
        ComboBox combo = DialogBuilder.Combo(items, value, v =>
        {
            current = v;
            Changed?.Invoke();
        });
        DialogBuilder.AddRow(Grid, label, combo);
        return () => current;
    }

    public Func<string> Text(string label, string value, bool multiline = false)
    {
        TextBox box = new() { Text = value, AcceptsReturn = multiline, MinHeight = multiline ? 80 : 0, TextWrapping = multiline ? TextWrapping.Wrap : TextWrapping.NoWrap };
        box.TextChanged += (_, _) => Changed?.Invoke();
        if (Grid.RowDefinitions.Count == 0)
        {
            box.AttachedToVisualTree += (_, _) =>
            {
                box.Focus();
                box.SelectAll();
            };
        }
        DialogBuilder.AddRow(Grid, label, box);
        return () => box.Text ?? string.Empty;
    }

    public Func<uint> Color(string label, uint value)
    {
        uint current = value;
        Border fill = new() { Background = Ui.Brush(value) };
        Border swatch = new()
        {
            Width = 48,
            Height = 22,
            HorizontalAlignment = HorizontalAlignment.Left,
            Background = Ui.Checker(0xFFFFFFFF, 0xFFC8C8C8, 4),
            BorderBrush = Ui.Divider,
            BorderThickness = new Thickness(1),
            Child = fill,
            Cursor = new Cursor(StandardCursorType.Hand)
        };
        swatch.PointerPressed += async (_, _) =>
        {
            Window owner = TopLevel.GetTopLevel(swatch) as Window ?? _owner;
            uint? picked = await Ui.PickColorAsync(owner, current, label);
            if (picked is not uint color) return;
            current = color;
            fill.Background = Ui.Brush(color);
            Changed?.Invoke();
        };
        DialogBuilder.AddRow(Grid, label, swatch);
        return () => current;
    }

    public void Note(string text)
    {
        DialogBuilder.AddFull(Grid, new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap, Classes = { "dim" }, MaxWidth = 380, HorizontalAlignment = HorizontalAlignment.Left });
    }

    public void Add(Control control) => DialogBuilder.AddFull(Grid, control);

    public void AddRow(string label, Control control) => DialogBuilder.AddRow(Grid, label, control);

    public Task<bool> ShowAsync(string title, double width = 380, string? okText = null, Func<bool>? validate = null) =>
        DialogBuilder.ShowAsync(_owner, title, Grid, width, okText, validate);
}
