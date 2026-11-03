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
using Avalonia.Layout;

namespace ShareX.PhotoEditor.UI.Dialogs;

public static class CanvasSizeDialog
{
    public sealed record Result(int Width, int Height, CanvasAnchor Anchor, SKColor Extension);

    public static async Task<Result?> ShowAsync(PhotoEditorWindow owner, int width, int height, SKColor background)
    {
        NumericUpDown w = DialogBuilder.Number(1, 30000, width), h = DialogBuilder.Number(1, 30000, height);
        CheckBox relative = new() { Content = "Relative" };
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
        void BuildAnchors()
        {
            anchors.Children.Clear();
            foreach (CanvasAnchor a in Enum.GetValues<CanvasAnchor>())
            {
                CanvasAnchor captured = a;
                Button b = new() { Content = a == anchor ? "●" : "", Margin = new Thickness(1), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                b.Click += (_, _) => { anchor = captured; BuildAnchors(); };
                anchors.Children.Add(b);
            }
        }
        BuildAnchors();
        SKColor extension = background;
        int extensionMode = 0;
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddFull(form, new TextBlock { Text = $"Current size: {width} × {height} px", Opacity = 0.8 });
        DialogBuilder.AddRow(form, "Width (px):", w);
        DialogBuilder.AddRow(form, "Height (px):", h);
        DialogBuilder.AddFull(form, relative);
        DialogBuilder.AddRow(form, "Anchor:", anchors);
        DialogBuilder.AddRow(form, "Canvas extension color:", DialogBuilder.Combo([("Background", 0), ("Foreground", 1), ("White", 2), ("Black", 3), ("Gray", 4), ("Transparent", 5)], extensionMode, v => extensionMode = v));
        if (!await DialogBuilder.ShowAsync(owner, "Canvas Size", form, 440)) return null;
        int nw = (int)(w.Value ?? width), nh = (int)(h.Value ?? height);
        if (relative.IsChecked == true) { nw += width; nh += height; }
        extension = extensionMode switch { 1 => owner.Foreground, 2 => SKColors.White, 3 => SKColors.Black, 4 => new SKColor(128, 128, 128), 5 => SKColors.Transparent, _ => background };
        return new Result(Math.Max(1, nw), Math.Max(1, nh), anchor, extension);
    }
}
