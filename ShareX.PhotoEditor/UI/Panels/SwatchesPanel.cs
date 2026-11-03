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

namespace ShareX.PhotoEditor.UI.Panels;

public sealed class SwatchesPanel : EditorPanel
{
    private readonly WrapPanel _wrap = new() { Margin = new Thickness(6) };

    public static readonly string[] DefaultSwatches =
    [
        "#000000", "#404040", "#808080", "#C0C0C0", "#FFFFFF", "#FF0000", "#FF8000", "#FFFF00", "#80FF00", "#00FF00", "#00FF80", "#00FFFF",
        "#0080FF", "#0000FF", "#8000FF", "#FF00FF", "#FF0080", "#800000", "#804000", "#808000", "#408000", "#008000", "#008040", "#008080",
        "#004080", "#000080", "#400080", "#800080", "#800040", "#F4CCCC", "#FCE5CD", "#FFF2CC", "#D9EAD3", "#D0E0E3", "#CFE2F3", "#D9D2E9",
        "#EAD1DC", "#E06666", "#F6B26B", "#FFD966", "#93C47D", "#76A5AF", "#6FA8DC", "#8E7CC3", "#C27BA0"
    ];

    public SwatchesPanel(PhotoEditorWindow editor) : base(editor)
    {
        StackPanel footer = new() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(4) };
        footer.Children.Add(IconButton("plus", "Create new swatch of foreground color", () =>
        {
            Editor.Settings.Swatches.Add("#" + ColorMath.ToHex(Editor.Foreground));
            Refresh();
        }));
        DockPanel root = new();
        DockPanel.SetDock(footer, Dock.Bottom);
        root.Children.Add(footer);
        root.Children.Add(new ScrollViewer { Content = _wrap });
        Content = root;
    }

    public override string Id => "swatches";

    public override string Title => "Swatches";

    public override void Refresh()
    {
        _wrap.Children.Clear();
        if (Editor.Settings.Swatches.Count == 0) Editor.Settings.Swatches.AddRange(DefaultSwatches);
        foreach (string hex in Editor.Settings.Swatches.ToList())
        {
            if (!ColorMath.TryParseHex(hex, out SKColor color)) continue;
            Border swatch = new() { Width = 18, Height = 18, Margin = new Thickness(1), Background = DialogBuilder.ToBrush(color), BorderBrush = Brushes.Black, BorderThickness = new Thickness(0.5), Cursor = new Cursor(StandardCursorType.Hand) };
            ToolTip.SetTip(swatch, $"#{ColorMath.ToHex(color)} — click: foreground, Alt/Ctrl-click: background, right-click: delete");
            swatch.PointerPressed += (_, e) =>
            {
                if (e.GetCurrentPoint(swatch).Properties.IsRightButtonPressed)
                {
                    Editor.Settings.Swatches.Remove(hex);
                    Refresh();
                }
                else if (e.KeyModifiers.HasFlag(KeyModifiers.Alt) || e.KeyModifiers.HasFlag(KeyModifiers.Control)) Editor.Background = color;
                else Editor.Foreground = color;
            };
            _wrap.Children.Add(swatch);
        }
    }
}
