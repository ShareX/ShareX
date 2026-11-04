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

public static class ImageSizeDialog
{
    public sealed record Result(int Width, int Height, float Resolution, ResampleMethod Method);

    public static async Task<Result?> ShowAsync(Window owner, int width, int height, float resolution, ResampleMethod method)
    {
        NumericUpDown w = DialogBuilder.Number(1, 30000, width), h = DialogBuilder.Number(1, 30000, height);
        NumericUpDown percent = DialogBuilder.Number(1, 10000, 100, 1, "0.##");
        NumericUpDown dpi = DialogBuilder.Number(1, 10000, resolution);
        bool constrain = true, updating = false;
        void Sync(Action action)
        {
            if (updating) return;
            updating = true;
            action();
            updating = false;
        }
        w.ValueChanged += (_, e) => Sync(() =>
        {
            decimal value = e.NewValue ?? width;
            if (constrain) h.Value = Math.Max(1, Math.Round(value * height / width));
            percent.Value = Math.Round(value * 100 / width, 2);
        });
        h.ValueChanged += (_, e) => Sync(() =>
        {
            decimal value = e.NewValue ?? height;
            if (constrain) w.Value = Math.Max(1, Math.Round(value * width / height));
        });
        percent.ValueChanged += (_, e) => Sync(() =>
        {
            decimal p = e.NewValue ?? 100;
            w.Value = Math.Max(1, Math.Round(width * p / 100));
            h.Value = Math.Max(1, Math.Round(height * p / 100));
        });
        CheckBox link = new() { Content = "Constrain proportions", IsChecked = true };
        link.IsCheckedChanged += (_, _) => constrain = link.IsChecked == true;
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddFull(form, new TextBlock { Text = $"Current size: {width} × {height} px", Opacity = 0.8 });
        DialogBuilder.AddRow(form, "Width (px):", w);
        DialogBuilder.AddRow(form, "Height (px):", h);
        DialogBuilder.AddRow(form, "Percent:", percent);
        DialogBuilder.AddFull(form, link);
        DialogBuilder.AddRow(form, "Resolution (ppi):", dpi);
        DialogBuilder.AddRow(form, "Resample:", DialogBuilder.Combo(Enum.GetValues<ResampleMethod>().Select(m => (OptionsBuilder.SplitWords(m.ToString()), m)), method, v => method = v));
        if (!await DialogBuilder.ShowAsync(owner, "Image Size", form, 420)) return null;
        return new Result((int)(w.Value ?? width), (int)(h.Value ?? height), (float)(dpi.Value ?? (decimal)resolution), method);
    }
}
