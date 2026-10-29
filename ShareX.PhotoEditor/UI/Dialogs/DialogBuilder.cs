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
using ShareX.AvaloniaUI.Theming;

namespace ShareX.PhotoEditor.UI.Dialogs;

/// <summary>Builds simple themed modal dialogs in code.</summary>
public static class DialogBuilder
{
    public static Window CreateWindow(string title, double width, double? height = null)
    {
        Window window = new()
        {
            Title = title,
            Width = width,
            SizeToContent = height == null ? SizeToContent.Height : SizeToContent.Manual,
            CanResize = height != null,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ShowInTaskbar = false,
            RequestedThemeVariant = ThemeManager.GetCurrentTheme()
        };
        if (height is double h) window.Height = h;
        window.Bind(Window.BackgroundProperty, window.GetResourceObservable("ShareX.Brush.Background.Main"));
        return window;
    }

    /// <summary>Shows content with OK and Cancel buttons. Returns true when OK is pressed and validation passes.</summary>
    public static async Task<bool> ShowAsync(Window owner, string title, Control content, double width = 420, string okText = "OK", Func<bool>? validate = null, IEnumerable<Control>? extraButtons = null)
    {
        Window window = CreateWindow(title, width);
        bool result = false;
        Button ok = new() { Content = okText, IsDefault = true, MinWidth = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        Button cancel = new() { Content = "Cancel", IsCancel = true, MinWidth = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
        ok.Click += (_, _) =>
        {
            if (validate != null && !validate()) return;
            result = true;
            window.Close();
        };
        cancel.Click += (_, _) => window.Close();
        StackPanel buttons = new() { Orientation = Orientation.Horizontal, Spacing = 8, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 12, 0, 0) };
        if (extraButtons != null) foreach (Control c in extraButtons) buttons.Children.Add(c);
        buttons.Children.Add(ok);
        buttons.Children.Add(cancel);
        DockPanel root = new() { Margin = new Thickness(16) };
        DockPanel.SetDock(buttons, Dock.Bottom);
        root.Children.Add(buttons);
        root.Children.Add(content);
        window.Content = root;
        window.KeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape) window.Close();
        };
        await window.ShowDialog(owner);
        return result;
    }

    public static Grid Form()
    {
        return new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*"), ColumnSpacing = 10, RowSpacing = 8 };
    }

    public static void AddRow(Grid grid, string label, Control control)
    {
        int row = grid.RowDefinitions.Count;
        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        TextBlock text = new() { Text = label, VerticalAlignment = VerticalAlignment.Center };
        Grid.SetRow(text, row);
        Grid.SetRow(control, row);
        Grid.SetColumn(control, 1);
        grid.Children.Add(text);
        grid.Children.Add(control);
    }

    public static void AddFull(Grid grid, Control control)
    {
        int row = grid.RowDefinitions.Count;
        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        Grid.SetRow(control, row);
        Grid.SetColumnSpan(control, 2);
        grid.Children.Add(control);
    }

    public static NumericUpDown Number(double min, double max, double value, double increment = 1, string format = "0")
    {
        return new NumericUpDown
        {
            Minimum = (decimal)min,
            Maximum = (decimal)max,
            Value = (decimal)Math.Clamp(value, min, max),
            Increment = (decimal)increment,
            FormatString = format,
            MinWidth = 110
        };
    }

    /// <summary>Slider with a synchronized number box.</summary>
    public static Control SliderNumber(double min, double max, double value, Action<double> changed, double step = 1, string format = "0")
    {
        Slider slider = new() { Minimum = min, Maximum = max, Value = value, MinWidth = 180, VerticalAlignment = VerticalAlignment.Center };
        NumericUpDown box = Number(min, max, value, step, format);
        box.Width = 72;
        box.ShowButtonSpinner = false;
        box.MinWidth = 60;
        bool syncing = false;
        slider.ValueChanged += (_, e) =>
        {
            if (syncing) return;
            syncing = true;
            double v = step >= 1 ? Math.Round(e.NewValue) : Math.Round(e.NewValue / step) * step;
            box.Value = (decimal)v;
            syncing = false;
            changed(v);
        };
        box.ValueChanged += (_, e) =>
        {
            if (syncing || e.NewValue is not decimal d) return;
            syncing = true;
            slider.Value = (double)d;
            syncing = false;
            changed((double)d);
        };
        Grid grid = new() { ColumnDefinitions = new ColumnDefinitions("*,Auto"), ColumnSpacing = 8 };
        Grid.SetColumn(box, 1);
        grid.Children.Add(slider);
        grid.Children.Add(box);
        return grid;
    }

    public static ComboBox Combo<T>(IEnumerable<(string Text, T Value)> items, T selected, Action<T> changed)
    {
        List<(string Text, T Value)> list = items.ToList();
        ComboBox combo = new() { MinWidth = 160, HorizontalAlignment = HorizontalAlignment.Stretch };
        foreach (var item in list) combo.Items.Add(new ComboBoxItem { Content = item.Text });
        combo.SelectedIndex = Math.Max(0, list.FindIndex(i => Equals(i.Value, selected)));
        combo.SelectionChanged += (_, _) =>
        {
            if (combo.SelectedIndex >= 0) changed(list[combo.SelectedIndex].Value);
        };
        return combo;
    }

    public static async Task<string?> PromptAsync(Window owner, string title, string label, string initial)
    {
        TextBox box = new() { Text = initial, AcceptsReturn = label.Contains("Author", StringComparison.Ordinal), MinHeight = label.Contains("Author", StringComparison.Ordinal) ? 100 : 0, TextWrapping = TextWrapping.Wrap };
        StackPanel panel = new() { Spacing = 8 };
        panel.Children.Add(new TextBlock { Text = label });
        panel.Children.Add(box);
        box.AttachedToVisualTree += (_, _) =>
        {
            box.Focus();
            box.SelectAll();
        };
        bool ok = await ShowAsync(owner, title, panel, 400);
        return ok ? box.Text ?? string.Empty : null;
    }

    public static async Task ShowMessageAsync(Window owner, string title, string text)
    {
        Window window = CreateWindow(title, 440);
        Button ok = new() { Content = "OK", IsDefault = true, IsCancel = true, MinWidth = 80, HorizontalAlignment = HorizontalAlignment.Right };
        ok.Click += (_, _) => window.Close();
        StackPanel panel = new() { Margin = new Thickness(16), Spacing = 12 };
        panel.Children.Add(new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap });
        panel.Children.Add(ok);
        window.Content = panel;
        await window.ShowDialog(owner);
    }

    public static async Task<bool?> ConfirmAsync(Window owner, string title, string text, string yes = "Yes", string no = "No", bool showCancel = true)
    {
        Window window = CreateWindow(title, 460);
        bool? result = null;
        StackPanel buttons = new() { Orientation = Orientation.Horizontal, Spacing = 8, HorizontalAlignment = HorizontalAlignment.Right };
        Button yesButton = new() { Content = yes, IsDefault = true, MinWidth = 80 };
        Button noButton = new() { Content = no, MinWidth = 80 };
        Button cancelButton = new() { Content = "Cancel", IsCancel = true, MinWidth = 80 };
        yesButton.Click += (_, _) => { result = true; window.Close(); };
        noButton.Click += (_, _) => { result = false; window.Close(); };
        cancelButton.Click += (_, _) => window.Close();
        buttons.Children.Add(yesButton);
        buttons.Children.Add(noButton);
        if (showCancel) buttons.Children.Add(cancelButton);
        StackPanel panel = new() { Margin = new Thickness(16), Spacing = 14 };
        panel.Children.Add(new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap });
        panel.Children.Add(buttons);
        window.Content = panel;
        await window.ShowDialog(owner);
        return result;
    }

    public static IBrush ToBrush(SKColor c) => new SolidColorBrush(Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue));
}
