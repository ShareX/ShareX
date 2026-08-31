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
using ShareX.ImageEditor.Core.Annotations;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ShareX.ImageEditor.Core.Abstractions;

/// <summary>
/// Core-facing contract for annotation toolbar state and actions.
/// </summary>
public interface IAnnotationToolbarAdapter : INotifyPropertyChanged
{
    EditorTool ActiveTool { get; set; }
    string StrokeColor { get; set; }
    string FillColor { get; set; }
    string TextColor { get; set; }
    string SelectedFontFamily { get; set; }
    TextHorizontalAlignment SelectedTextHorizontalAlignment { get; set; }
    BorderStyle SelectedBorderStyle { get; set; }
    ArrowStyle SelectedArrowStyle { get; set; }
    CursorType SelectedCursorType { get; set; }
    StepType SelectedStepType { get; set; }
    IReadOnlyList<string> AvailableFontFamilies { get; }
    IReadOnlyList<TextHorizontalAlignment> AvailableTextHorizontalAlignments { get; }
    IReadOnlyList<BorderStyle> AvailableBorderStyles { get; }
    IReadOnlyList<ArrowStyle> AvailableArrowStyles { get; }
    IReadOnlyList<CursorType> AvailableCursorTypes { get; }
    IReadOnlyList<int> AvailableStepStartNumbers { get; }
    IReadOnlyList<StepType> AvailableStepTypes { get; }
    IBrush SelectedColorBrush { get; set; }
    IBrush FillColorBrush { get; set; }
    IBrush TextColorBrush { get; set; }
    int StrokeWidth { get; set; }
    int CornerRadius { get; set; }
    float FontSize { get; set; }
    int StepStartNumber { get; set; }
    float EffectStrength { get; set; }
    float EffectStrengthMaximum { get; }
    float SpotlightBlur { get; set; }
    float SpotlightBlurMaximum { get; }
    bool ShadowEnabled { get; set; }
    IBrush ShadowColorBrush { get; set; }
    double ShadowBlurRadius { get; set; }
    double ShadowOpacity { get; set; }
    double ShadowOffsetX { get; set; }
    double ShadowOffsetY { get; set; }
    bool SpeechBalloonTail { get; set; }
    bool EffectEllipse { get; set; }
    bool TextBold { get; set; }
    bool TextItalic { get; set; }
    string ActiveToolIcon { get; }
    string ActiveToolName { get; }
    bool CanUndo { get; }
    bool CanRedo { get; }
    bool HasSelection { get; }
    bool HasAnnotations { get; }
    bool ShowBorderColor { get; }
    bool ShowFillColor { get; }
    bool ShowTextColor { get; }
    bool ShowThickness { get; }
    bool ShowFontSize { get; }
    bool ShowStepStartNumber { get; }
    bool ShowStepType { get; }
    bool ShowTextHorizontalAlignment { get; }
    bool ShowFontFamily { get; }
    bool ShowBorderStyle { get; }
    bool ShowArrowStyle { get; }
    bool ShowCursorType { get; }
    bool ShowCornerRadius { get; }
    bool ShowStrength { get; }
    bool ShowSpotlightBlur { get; }
    bool ShowTextStyle { get; }
    bool ShowTextItalic { get; }
    bool ShowShadow { get; }
    bool ShowSpeechBalloonTail { get; }
    bool ShowEffectEllipse { get; }
    bool ShowToolOptions { get; }
    bool ShowToolOptionsSeparator { get; }
    ReadOnlyObservableCollection<MenuItem> RecentImageMenuItems { get; }
    ReadOnlyObservableCollection<string> RecentImageFiles { get; }
    bool HasRecentImageFiles { get; }
    bool IsEffectsButtonActive { get; }
    ICommand NewImageCommand { get; }
    ICommand OpenImageCommand { get; }
    ICommand SaveCommand { get; }
    ICommand SaveAsCommand { get; }
    ICommand ExitEditorCommand { get; }
    ICommand OpenRecentImageCommand { get; }
    void SelectTool(EditorTool tool);
    void Undo();
    void Redo();
    void DeleteSelection();
    void ClearSelection();
}
