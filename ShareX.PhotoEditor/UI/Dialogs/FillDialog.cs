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

public static class FillDialog
{
    public sealed class Result
    {
        public SKColor Color { get; set; }
        public PatternDefinition? Pattern { get; set; }
        public bool History { get; set; }
        public bool ContentAware { get; set; }
        public LayerBlendMode Mode { get; set; } = LayerBlendMode.Normal;
        public float Opacity { get; set; } = 1;
        public bool PreserveTransparency { get; set; }
    }

    public static async Task<Result?> ShowAsync(PhotoEditorWindow owner)
    {
        Result result = new() { Color = owner.Foreground };
        int contents = 0;
        PatternDefinition pattern = owner.Pattern ?? owner.Patterns[0];
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Contents:", DialogBuilder.Combo([("Foreground Color", 0), ("Background Color", 1), ("Color...", 2), ("Content-Aware", 3), ("Pattern", 4), ("History", 5), ("Black", 6), ("50% Gray", 7), ("White", 8)], contents, v => contents = v));
        DialogBuilder.AddRow(form, "Pattern:", DialogBuilder.Combo(owner.Patterns.Select(p => (p.Name, p)), pattern, v => pattern = v));
        DialogBuilder.AddRow(form, "Mode:", DialogBuilder.Combo(BlendModes.PaintModes.Select(m => (BlendModes.GetName(m), m)), result.Mode, v => result.Mode = v));
        DialogBuilder.AddRow(form, "Opacity (%):", DialogBuilder.SliderNumber(0, 100, 100, v => result.Opacity = (float)v / 100));
        CheckBox preserve = new() { Content = "Preserve Transparency" };
        preserve.IsCheckedChanged += (_, _) => result.PreserveTransparency = preserve.IsChecked == true;
        DialogBuilder.AddFull(form, preserve);
        if (!await DialogBuilder.ShowAsync(owner, "Fill", form, 420)) return null;
        switch (contents)
        {
            case 1: result.Color = owner.Background; break;
            case 2:
                SKColor? picked = await ColorPickerDialog.ShowAsync(owner, owner.Foreground, "Fill Color", owner);
                if (picked == null) return null;
                result.Color = picked.Value;
                break;
            case 3: result.ContentAware = true; break;
            case 4: result.Pattern = pattern; break;
            case 5: result.History = true; break;
            case 6: result.Color = SKColors.Black; break;
            case 7: result.Color = new SKColor(128, 128, 128); break;
            case 8: result.Color = SKColors.White; break;
        }
        return result;
    }
}
