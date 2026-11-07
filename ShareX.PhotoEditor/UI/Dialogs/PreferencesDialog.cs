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

public static class PreferencesDialog
{
    public static async Task ShowAsync(PhotoEditorWindow owner)
    {
        EditorSettings s = owner.Settings;
        int history = s.HistoryStates, checker = s.CheckerSize, subdivisions = s.GridSubdivisions;
        float grid = s.GridSpacing;
        ResampleMethod resample = s.DefaultResample;
        string guideColor = s.GuideColor;
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, Strings.PreferencesDialog_HistoryStatesColon, DialogBuilder.SliderNumber(5, 1000, history, v => history = (int)v));
        DialogBuilder.AddRow(form, Strings.PreferencesDialog_ImageInterpolationColon, DialogBuilder.Combo(Enum.GetValues<ResampleMethod>().Select(m => (EnumNames.Get(m), m)), resample, v => resample = v));
        DialogBuilder.AddRow(form, Strings.PreferencesDialog_TransparencyGridSizeColon, DialogBuilder.Combo([(Strings.Filters_OptionSmall, 4), (Strings.Filters_OptionMedium, 8), (Strings.Filters_OptionLarge, 16)], checker, v => checker = v));
        DialogBuilder.AddRow(form, Strings.PreferencesDialog_GridlineEveryColon, DialogBuilder.SliderNumber(1, 2000, grid, v => grid = (float)v));
        DialogBuilder.AddRow(form, Strings.PreferencesDialog_GridSubdivisionsColon, DialogBuilder.SliderNumber(1, 100, subdivisions, v => subdivisions = (int)v));
        TextBox guide = new() { Text = guideColor };
        DialogBuilder.AddRow(form, Strings.PreferencesDialog_GuideColorColon, guide);
        CheckBox rememberWindow = new() { Content = Strings.PreferencesDialog_RememberWindow, IsChecked = s.RememberWindowState };
        DialogBuilder.AddFull(form, rememberWindow);
        if (!await DialogBuilder.ShowAsync(owner, Strings.PreferencesDialog_Title, form, 460)) return;
        s.HistoryStates = history;
        s.CheckerSize = checker;
        s.GridSpacing = grid;
        s.GridSubdivisions = subdivisions;
        s.DefaultResample = resample;
        s.RememberWindowState = rememberWindow.IsChecked == true;
        if (ColorMath.TryParseHex(guide.Text, out _)) s.GuideColor = guide.Text!;
        foreach (DocumentView view in owner.Views) view.Document.History.MaxStates = history;
        owner.SaveSettings();
        owner.InvalidateCanvas();
    }
}
