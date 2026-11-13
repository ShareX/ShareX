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

using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ShareX.PixelEditor.Tools;

namespace ShareX.PixelEditor.UI;

/// <summary>Brushes made from selections, kept in the settings so they can be picked again later.</summary>
public static class BrushLibrary
{
    private const int MaxBrushes = 32;

    public static string Encode(IEnumerable<SKPointI> tip) =>
        string.Join(" ", tip.Select(p => p.X.ToString(CultureInfo.InvariantCulture) + "," + p.Y.ToString(CultureInfo.InvariantCulture)));

    public static SKPointI[]? Decode(string text)
    {
        List<SKPointI> points = [];
        foreach (string part in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            string[] xy = part.Split(',');
            if (xy.Length != 2 || !int.TryParse(xy[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int x) || !int.TryParse(xy[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int y)) return null;
            points.Add(new SKPointI(x, y));
        }
        return points.Count > 0 ? [.. points] : null;
    }

    /// <summary>Puts a brush first in the library, removing an identical one.</summary>
    public static void Add(EditorSettings settings, SKPointI[] tip)
    {
        string encoded = Encode(tip);
        settings.Brushes.Remove(encoded);
        settings.Brushes.Insert(0, encoded);
        if (settings.Brushes.Count > MaxBrushes) settings.Brushes.RemoveRange(MaxBrushes, settings.Brushes.Count - MaxBrushes);
    }

    private static SKRectI Bounds(SKPointI[] tip) =>
        new(tip.Min(p => p.X), tip.Min(p => p.Y), tip.Max(p => p.X) + 1, tip.Max(p => p.Y) + 1);

    /// <summary>A picture of a brush shape, scaled to fit a square.</summary>
    public static Control Preview(SKPointI[] tip, double size)
    {
        SKRectI bounds = Bounds(tip);
        PixelBuffer buffer = new(bounds.Width, bounds.Height);
        foreach (SKPointI p in tip) buffer.Pixels[(p.Y - bounds.Top) * bounds.Width + p.X - bounds.Left] = 0xFFE0E0E0;
        double scale = Math.Min(size / bounds.Width, size / bounds.Height);
        if (scale >= 1) scale = Math.Floor(scale);
        Image image = new() { Source = Ui.ToBitmap(buffer), Width = Math.Max(1, bounds.Width * scale), Height = Math.Max(1, bounds.Height * scale), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        RenderOptions.SetBitmapInterpolationMode(image, BitmapInterpolationMode.None);
        return new Border { Width = size, Height = size, Child = image };
    }

    /// <summary>The options bar button that shows the brush in use and opens the library.</summary>
    public static Control CreatePicker(PixelEditorWindow editor)
    {
        SKPointI[] current = editor.CustomTip ?? Raster.BrushTip(Math.Min(editor.Settings.BrushSize, 16), editor.Settings.BrushRound);
        StackPanel content = new() { Orientation = Orientation.Horizontal, Spacing = 4 };
        content.Children.Add(Preview(current, 16));
        content.Children.Add(new IconView(PixelEditorIcons.ChevronDown, 10) { VerticalAlignment = VerticalAlignment.Center });
        Button button = new() { Content = content, Padding = new Thickness(5, 3), VerticalAlignment = VerticalAlignment.Center };
        ToolTip.SetTip(button, Strings.Options_Brushes);
        button.Click += (_, _) => BuildMenu(editor).Open(button);
        return button;
    }

    private static ContextMenu BuildMenu(PixelEditorWindow editor)
    {
        ContextMenu menu = new();
        void Pick(SKPointI[]? tip)
        {
            editor.CustomTip = tip;
            if (editor.Tools.Active is not PaintToolBase) editor.Tools.Select(ToolId.Pencil);
            editor.RefreshOptions();
            editor.Canvas.InvalidateVisual();
        }
        MenuItem standard = new() { Header = Strings.Options_StandardBrush, Icon = Preview(Raster.BrushTip(5, editor.Settings.BrushRound), 16) };
        standard.Click += (_, _) => Pick(null);
        menu.Items.Add(standard);
        List<string> brushes = editor.Settings.Brushes;
        if (brushes.Count > 0) menu.Items.Add(new Separator());
        string? currentCode = editor.CustomTip != null ? Encode(editor.CustomTip) : null;
        foreach (string code in brushes.ToList())
        {
            SKPointI[]? tip = Decode(code);
            if (tip == null) continue;
            SKRectI bounds = Bounds(tip);
            MenuItem item = new()
            {
                Header = $"{bounds.Width} × {bounds.Height}",
                Icon = Preview(tip, 24),
                ToggleType = MenuItemToggleType.Radio,
                IsChecked = code == currentCode
            };
            item.Click += (_, _) => Pick(tip);
            menu.Items.Add(item);
        }
        menu.Items.Add(new Separator());
        MenuItem define = new() { Header = Strings.Command_DefineBrush, IsEnabled = editor.Sprite?.Selection != null };
        define.Click += (_, _) => editor.Execute("edit.defineBrush");
        menu.Items.Add(define);
        MenuItem remove = new() { Header = Strings.Options_RemoveBrush, IsEnabled = currentCode != null && brushes.Contains(currentCode) };
        remove.Click += (_, _) =>
        {
            if (currentCode != null) brushes.Remove(currentCode);
            Pick(null);
        };
        menu.Items.Add(remove);
        return menu;
    }
}
