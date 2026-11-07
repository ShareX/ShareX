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

public static class NewDocumentDialog
{
    public sealed record Result(string Name, int Width, int Height, float Resolution, SKColor Background);

    public static async Task<Result?> ShowAsync(Window owner, SKSizeI clipboard, SKColor backgroundColor)
    {
        string name = Strings.NewDocumentDialog_Untitled;
        NumericUpDown width = DialogBuilder.Number(1, 30000, clipboard.Width > 0 ? clipboard.Width : 1920);
        NumericUpDown height = DialogBuilder.Number(1, 30000, clipboard.Height > 0 ? clipboard.Height : 1080);
        NumericUpDown resolution = DialogBuilder.Number(1, 10000, 72);
        int background = 0;
        TextBox nameBox = new() { Text = name };
        List<(string, SKSizeI)> presets =
        [
            (Strings.EditorCommands_Clipboard, clipboard), (Strings.NewDocumentDialog_PresetDefault, new(1920, 1080)), ("HD 1280 × 720", new(1280, 720)), ("4K UHD 3840 × 2160", new(3840, 2160)),
            (Strings.NewDocumentDialog_PresetSquare, new(1080, 1080)), (Strings.NewDocumentDialog_PresetWeb, new(1366, 768)), (Strings.NewDocumentDialog_PresetLetter, new(2550, 3300)), (Strings.NewDocumentDialog_PresetA4, new(2480, 3508)),
            (Strings.NewDocumentDialog_PresetIcon, new(512, 512))
        ];
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, Strings.EditorCommands_NameColon, nameBox);
        DialogBuilder.AddRow(form, Strings.HdrToningEditor_PresetColon, DialogBuilder.Combo(presets.Where(p => p.Item2.Width > 0).Select(p => (p.Item1, p.Item2)), presets.First(p => p.Item2.Width > 0).Item2, v =>
        {
            width.Value = v.Width;
            height.Value = v.Height;
            resolution.Value = v.Width >= 2480 ? 300 : 72;
        }));
        DialogBuilder.AddRow(form, Strings.EditorCommands_WidthPxColon, width);
        DialogBuilder.AddRow(form, Strings.ExportDialog_HeightPxColon, height);
        DialogBuilder.AddRow(form, Strings.NewDocumentDialog_ResolutionColon, resolution);
        DialogBuilder.AddRow(form, Strings.PropertiesPanel_BackgroundColon, DialogBuilder.Combo([(Strings.NewDocumentDialog_White, 0), (Strings.NewDocumentDialog_Black, 1), (Strings.ColorPanel_BackgroundColor, 2), (Strings.NewDocumentDialog_Transparent, 3)], background, v => background = v));
        if (!await DialogBuilder.ShowAsync(owner, Strings.NewDocumentDialog_Title, form, 420, Strings.NewDocumentDialog_Create)) return null;
        SKColor fill = background switch { 1 => SKColors.Black, 2 => backgroundColor.WithAlpha(255), 3 => SKColors.Transparent, _ => SKColors.White };
        return new Result(string.IsNullOrWhiteSpace(nameBox.Text) ? Strings.NewDocumentDialog_Untitled : nameBox.Text, (int)(width.Value ?? 1), (int)(height.Value ?? 1), (float)(resolution.Value ?? 72), fill);
    }
}
