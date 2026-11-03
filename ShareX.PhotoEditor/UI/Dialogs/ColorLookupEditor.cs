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
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;

namespace ShareX.PhotoEditor.UI.Dialogs;

/// <summary>Parameter editor of the Color Lookup adjustment.</summary>
public static class ColorLookupEditor
{
    public static Control Build(AdjustmentSettings s, Action changed)
    {
        TextBlock name = new() { Text = s.Lut?.Name ?? "None", VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis };
        Button load = new() { Content = "Load 3D LUT..." };
        load.Click += async (_, _) =>
        {
            if (TopLevel.GetTopLevel(load) is not Window owner) return;
            IReadOnlyList<IStorageFile> files = await owner.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Load 3D LUT",
                AllowMultiple = false,
                FileTypeFilter = [new FilePickerFileType("3D LUT files") { Patterns = ["*.cube", "*.3dl"] }, FilePickerFileTypes.All]
            });
            string? path = files.FirstOrDefault()?.TryGetLocalPath();
            if (path == null) return;
            try
            {
                s.Lut = await Task.Run(() => ColorLut.Load(path));
            }
            catch (Exception ex) when (ex is IOException or InvalidDataException or UnauthorizedAccessException or FormatException or OverflowException or IndexOutOfRangeException)
            {
                await DialogBuilder.ShowMessageAsync(owner, "Color Lookup", $"Could not load the 3D LUT: {ex.Message}");
                return;
            }
            name.Text = s.Lut.Name;
            changed();
        };
        Grid row = new() { ColumnDefinitions = new ColumnDefinitions("*,Auto"), ColumnSpacing = 8 };
        Grid.SetColumn(load, 1);
        row.Children.Add(name);
        row.Children.Add(load);
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "3DLUT File:", row);
        return form;
    }
}
