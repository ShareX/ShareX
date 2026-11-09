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
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Path = Avalonia.Controls.Shapes.Path;
using Avalonia.Layout;
using Avalonia.Media;

namespace ShareX.PhotoEditor.UI.Dialogs;

public static class CanvasSizeDialog
{
    public sealed record Result(int Width, int Height, CanvasAnchor Anchor, SKColor Extension);

    public static async Task<Result?> ShowAsync(PhotoEditorWindow owner, int width, int height, SKColor background)
    {
        NumericUpDown w = DialogBuilder.Number(1, 30000, width), h = DialogBuilder.Number(1, 30000, height);
        CheckBox relative = new() { Content = Strings.CanvasSizeDialog_Relative };
        relative.IsCheckedChanged += (_, _) =>
        {
            bool r = relative.IsChecked == true;
            w.Minimum = r ? -30000 : 1;
            h.Minimum = r ? -30000 : 1;
            w.Value = r ? 0 : width;
            h.Value = r ? 0 : height;
        };
        CanvasAnchor anchor = CanvasAnchor.Center;
        UniformGrid anchors = new() { Rows = 3, Columns = 3, Width = 96, Height = 96, HorizontalAlignment = HorizontalAlignment.Left };
        List<Button> anchorButtons = [];
        foreach (CanvasAnchor a in Enum.GetValues<CanvasAnchor>())
        {
            Button b = new() { Margin = new Thickness(1), Padding = new Thickness(0), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center };
            b.Click += (_, _) => { anchor = a; UpdateAnchors(); };
            anchorButtons.Add(b);
            anchors.Children.Add(b);
        }
        // Like Photoshop: the cells next to the anchor show which way the canvas grows (or shrinks, pointing inward).
        void UpdateAnchors()
        {
            bool r = relative.IsChecked == true;
            double nw = w.Value is { } wv ? (double)wv : (r ? 0 : width), nh = h.Value is { } hv ? (double)hv : (r ? 0 : height);
            int growX = (r ? nw < 0 : nw < width) ? -1 : 1, growY = (r ? nh < 0 : nh < height) ? -1 : 1;
            int ax = (int)anchor % 3, ay = (int)anchor / 3;
            for (int i = 0; i < anchorButtons.Count; i++)
            {
                int dx = i % 3 - ax, dy = i / 3 - ay;
                anchorButtons[i].Content = dx == 0 && dy == 0 ? "●" : Math.Abs(dx) > 1 || Math.Abs(dy) > 1 ? null : CreateArrow(dx * growX, dy * growY);
            }
        }
        w.ValueChanged += (_, _) => UpdateAnchors();
        h.ValueChanged += (_, _) => UpdateAnchors();
        relative.IsCheckedChanged += (_, _) => UpdateAnchors();
        UpdateAnchors();
        SKColor extension = background;
        int extensionMode = 0;
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddFull(form, new TextBlock { Text = string.Format(Strings.CanvasSizeDialog_CurrentSize, width, height), Opacity = 0.8 });
        DialogBuilder.AddRow(form, Strings.EditorCommands_WidthPxColon, w);
        DialogBuilder.AddRow(form, Strings.ExportDialog_HeightPxColon, h);
        DialogBuilder.AddFull(form, relative);
        DialogBuilder.AddRow(form, Strings.CanvasSizeDialog_AnchorColon, anchors);
        DialogBuilder.AddRow(form, Strings.CanvasSizeDialog_ExtensionColorColon, DialogBuilder.Combo([(Strings.LayerNames_Background, 0), (Strings.CanvasSizeDialog_Foreground, 1), (Strings.NewDocumentDialog_White, 2), (Strings.NewDocumentDialog_Black, 3), (Strings.CanvasSizeDialog_Gray, 4), (Strings.NewDocumentDialog_Transparent, 5)], extensionMode, v => extensionMode = v));
        if (!await DialogBuilder.ShowAsync(owner, Strings.EditorCommands_CanvasSize, form, 440)) return null;
        int nw = (int)(w.Value ?? width), nh = (int)(h.Value ?? height);
        if (relative.IsChecked == true) { nw += width; nh += height; }
        extension = extensionMode switch { 1 => owner.Foreground, 2 => SKColors.White, 3 => SKColors.Black, 4 => new SKColor(128, 128, 128), 5 => SKColors.Transparent, _ => background };
        return new Result(Math.Max(1, nw), Math.Max(1, nh), anchor, extension);
    }

    /// <summary>Arrow drawn pointing right on a 12 px grid, then rotated toward (dx, dy).</summary>
    private static Control CreateArrow(int dx, int dy)
    {
        Path path = new()
        {
            Data = Geometry.Parse("M1 5 H7 V2 L11 6 L7 10 V7 H1 Z"),
            Width = 12,
            Height = 12,
            RenderTransform = new RotateTransform(Math.Atan2(dy, dx) * 180 / Math.PI),
            RenderTransformOrigin = RelativePoint.Center,
            IsHitTestVisible = false
        };
        path.Bind(Shape.FillProperty, path.GetResourceObservable("ShareX.Brush.Text"));
        return path;
    }
}
