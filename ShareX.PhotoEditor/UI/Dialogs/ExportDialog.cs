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
    public sealed record Result(string Extension, int Quality, int Width, int Height, bool Lossless);

    private static readonly (string Name, string Extension)[] Formats = [("PNG", ".png"), ("JPEG", ".jpg"), ("WebP", ".webp"), ("GIF", ".gif"), ("BMP", ".bmp"), ("TIFF", ".tif")];

    /// <summary>Shows the export options. Format, quality and lossless are remembered in <paramref name="settings"/>; the size always starts at the document size.</summary>
    public static async Task<Result?> ShowAsync(Window owner, PhotoDocument doc, EditorSettings settings)
    {
        string extension = Formats.Any(f => f.Extension == settings.ExportFormat) ? settings.ExportFormat : ".png";
        int quality = Math.Clamp(settings.ExportQuality, 1, 100);
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
        // Each optional row is its own grid so a hidden row collapses completely instead of leaving row spacing behind.
        StackPanel form = new() { Spacing = 8 };
        Grid.SetIsSharedSizeScope(form, true);
        Grid AddSection()
        {
            Grid section = DialogBuilder.Form();
            section.ColumnDefinitions[0].SharedSizeGroup = "Label";
            form.Children.Add(section);
            return section;
        }
        Grid formatSection = AddSection();
        Grid losslessSection = AddSection();
        Grid qualitySection = AddSection();
        Grid sizeSection = AddSection();
        CheckBox lossless = new() { Content = "Lossless", IsChecked = settings.ExportWebPLossless };
        void UpdateOptions()
        {
            losslessSection.IsVisible = extension == ".webp";
            // Only lossy encoding has a quality setting.
            qualitySection.IsVisible = extension == ".jpg" || (extension == ".webp" && lossless.IsChecked != true);
        }
        DialogBuilder.AddRow(formatSection, "Format:", DialogBuilder.Combo(Formats, extension, v =>
        {
            extension = v;
            UpdateOptions();
        }));
        lossless.IsCheckedChanged += (_, _) => UpdateOptions();
        DialogBuilder.AddFull(losslessSection, lossless);
        DialogBuilder.AddRow(qualitySection, "Quality:", DialogBuilder.SliderNumber(1, 100, quality, v => quality = (int)v));
        DialogBuilder.AddRow(sizeSection, "Width (px):", width);
        DialogBuilder.AddRow(sizeSection, "Height (px):", height);
        UpdateOptions();
        if (!await DialogBuilder.ShowAsync(owner, "Export As", form, 420, "Export...")) return null;
        settings.ExportFormat = extension;
        settings.ExportQuality = quality;
        settings.ExportWebPLossless = lossless.IsChecked == true;
        return new Result(extension, quality, (int)(width.Value ?? doc.Width), (int)(height.Value ?? doc.Height), extension == ".webp" && lossless.IsChecked == true);
    }
}
