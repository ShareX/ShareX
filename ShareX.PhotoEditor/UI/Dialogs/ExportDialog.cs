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

using Avalonia.Controls;
using Avalonia.Media;
using ShareX.PhotoEditor.Tools;
using ShareX.PhotoEditor.UI.Commands;

namespace ShareX.PhotoEditor.UI.Dialogs;

public static class ExportDialog
{
    public sealed record Result(string Extension, int Quality, int Width, int Height);

    public static async Task<Result?> ShowAsync(Window owner, PhotoDocument doc)
    {
        string extension = ".png";
        int quality = 90;
        NumericUpDown width = DialogBuilder.Number(1, 30000, doc.Width);
        NumericUpDown height = DialogBuilder.Number(1, 30000, doc.Height);
        bool updating = false;
        width.ValueChanged += (_, e) =>
        {
            if (updating || e.NewValue is not decimal w) return;
            updating = true;
            height.Value = Math.Max(1, Math.Round(w * doc.Height / doc.Width));
            updating = false;
        };
        height.ValueChanged += (_, e) =>
        {
            if (updating || e.NewValue is not decimal h) return;
            updating = true;
            width.Value = Math.Max(1, Math.Round(h * doc.Width / doc.Height));
            updating = false;
        };
        Grid form = DialogBuilder.Form();
        List<Control> qualityRow = [];
        DialogBuilder.AddRow(form, "Format:", DialogBuilder.Combo([("PNG", ".png"), ("JPEG", ".jpg"), ("WebP", ".webp"), ("GIF", ".gif"), ("BMP", ".bmp"), ("TIFF", ".tif")], extension, v =>
        {
            extension = v;
            // Only lossy formats have a quality setting.
            foreach (Control control in qualityRow) control.IsVisible = extension is ".jpg" or ".webp";
        }));
        DialogBuilder.AddRow(form, "Quality:", DialogBuilder.SliderNumber(1, 100, quality, v => quality = (int)v));
        qualityRow.AddRange(form.Children.TakeLast(2).Cast<Control>());
        foreach (Control control in qualityRow) control.IsVisible = false;
        DialogBuilder.AddRow(form, "Width (px):", width);
        DialogBuilder.AddRow(form, "Height (px):", height);
        if (!await DialogBuilder.ShowAsync(owner, "Export As", form, 420, "Export...")) return null;
        return new Result(extension, quality, (int)(width.Value ?? doc.Width), (int)(height.Value ?? doc.Height));
    }
}
