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
using ShareX.ImageEditor.Core.Abstractions;
using ShareX.ImageEditor.Core.Annotations;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace ShareX.ImageEditor.Presentation.ViewModels;

/// <summary>
/// Bridges <see cref="MainViewModel"/> to the core-facing toolbar contract.
/// </summary>
public sealed class EditorToolbarAdapter : IAnnotationToolbarAdapter
{
    private readonly MainViewModel _viewModel;
    private readonly ObservableCollection<MenuItem> _recentImageMenuItems = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public EditorToolbarAdapter(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        RecentImageMenuItems = new ReadOnlyObservableCollection<MenuItem>(_recentImageMenuItems);

        if (_viewModel.RecentImageFiles is INotifyCollectionChanged recentFiles)
        {
            recentFiles.CollectionChanged += OnRecentImageFilesChanged;
        }

        SyncRecentImageMenuItems();
    }

    public ReadOnlyObservableCollection<MenuItem> RecentImageMenuItems { get; }

    public EditorTool ActiveTool
    {
        get => _viewModel.ActiveTool;
        set => _viewModel.ActiveTool = value;
    }

    public string StrokeColor
    {
        get => _viewModel.SelectedColor;
        set => _viewModel.SelectedColor = value;
    }

    public string FillColor
    {
        get => _viewModel.FillColor;
        set => _viewModel.FillColor = value;
    }

    public string TextColor
    {
        get => _viewModel.TextColor;
        set => _viewModel.TextColor = value;
    }

    public string SelectedFontFamily
    {
        get => _viewModel.SelectedFontFamily;
        set => _viewModel.SelectedFontFamily = value;
    }

    public TextHorizontalAlignment SelectedTextHorizontalAlignment
    {
        get => _viewModel.SelectedTextHorizontalAlignment;
        set => _viewModel.SelectedTextHorizontalAlignment = value;
    }

    public BorderStyle SelectedBorderStyle
    {
        get => _viewModel.SelectedBorderStyle;
        set => _viewModel.SelectedBorderStyle = value;
    }

    public ArrowStyle SelectedArrowStyle
    {
        get => _viewModel.SelectedArrowStyle;
        set => _viewModel.SelectedArrowStyle = value;
    }

    public CursorType SelectedCursorType
    {
        get => _viewModel.SelectedCursorType;
        set => _viewModel.SelectedCursorType = value;
    }

    public StepType SelectedStepType
    {
        get => _viewModel.SelectedStepType;
        set => _viewModel.SelectedStepType = value;
    }

    public IReadOnlyList<string> AvailableFontFamilies => _viewModel.AvailableFontFamilies;

    public IReadOnlyList<TextHorizontalAlignment> AvailableTextHorizontalAlignments => _viewModel.AvailableTextHorizontalAlignments;

    public IReadOnlyList<BorderStyle> AvailableBorderStyles => _viewModel.AvailableBorderStyles;

    public IReadOnlyList<ArrowStyle> AvailableArrowStyles => _viewModel.AvailableArrowStyles;

    public IReadOnlyList<CursorType> AvailableCursorTypes => _viewModel.AvailableCursorTypes;

    public IReadOnlyList<int> AvailableStepStartNumbers => _viewModel.AvailableStepStartNumbers;

    public IReadOnlyList<StepType> AvailableStepTypes => _viewModel.AvailableStepTypes;

    public int StrokeWidth
    {
        get => _viewModel.StrokeWidth;
        set => _viewModel.StrokeWidth = value;
    }

    public int CornerRadius
    {
        get => _viewModel.CornerRadius;
        set => _viewModel.CornerRadius = value;
    }

    public float FontSize
    {
        get => _viewModel.FontSize;
        set => _viewModel.FontSize = value;
    }

    public int StepStartNumber
    {
        get => _viewModel.StepStartNumber;
        set => _viewModel.StepStartNumber = value;
    }

    public float EffectStrength
    {
        get => _viewModel.EffectStrength;
        set => _viewModel.EffectStrength = value;
    }

    public float EffectStrengthMaximum => _viewModel.EffectStrengthMaximum;

    public float SpotlightBlur
    {
        get => _viewModel.SpotlightBlur;
        set => _viewModel.SpotlightBlur = value;
    }

    public float SpotlightBlurMaximum => _viewModel.SpotlightBlurMaximum;

    public bool ShadowEnabled
    {
        get => _viewModel.ShadowEnabled;
        set => _viewModel.ShadowEnabled = value;
    }

    public IBrush ShadowColorBrush
    {
        get => _viewModel.ShadowColorBrush;
        set => _viewModel.ShadowColorBrush = value;
    }

    public double ShadowBlurRadius
    {
        get => _viewModel.ShadowBlurRadius;
        set => _viewModel.ShadowBlurRadius = value;
    }

    public double ShadowOpacity
    {
        get => _viewModel.ShadowOpacity;
        set => _viewModel.ShadowOpacity = value;
    }

    public double ShadowOffsetX
    {
        get => _viewModel.ShadowOffsetX;
        set => _viewModel.ShadowOffsetX = value;
    }

    public double ShadowOffsetY
    {
        get => _viewModel.ShadowOffsetY;
        set => _viewModel.ShadowOffsetY = value;
    }

    public bool SpeechBalloonTail
    {
        get => _viewModel.SpeechBalloonTail;
        set => _viewModel.SpeechBalloonTail = value;
    }

    public bool EffectEllipse
    {
        get => _viewModel.EffectEllipse;
        set => _viewModel.EffectEllipse = value;
    }

    public bool TextBold
    {
        get => _viewModel.TextBold;
        set => _viewModel.TextBold = value;
    }

    public bool TextItalic
    {
        get => _viewModel.TextItalic;
        set => _viewModel.TextItalic = value;
    }

    public bool CanUndo => _viewModel.CanUndo;

    public bool CanRedo => _viewModel.CanRedo;

    public bool HasSelection => _viewModel.HasSelectedAnnotation;

    public bool HasAnnotations => _viewModel.HasAnnotations;

    public bool ShowBorderColor => _viewModel.ShowBorderColor;

    public bool ShowFillColor => _viewModel.ShowFillColor;

    public bool ShowTextColor => _viewModel.ShowTextColor;

    public bool ShowThickness => _viewModel.ShowThickness;

    public bool ShowFontSize => _viewModel.ShowFontSize;

    public bool ShowStepStartNumber => _viewModel.ShowStepStartNumber;

    public bool ShowStepType => _viewModel.ShowStepType;

    public bool ShowTextHorizontalAlignment => _viewModel.ShowTextHorizontalAlignment;

    public bool ShowFontFamily => _viewModel.ShowFontFamily;

    public bool ShowBorderStyle => _viewModel.ShowBorderStyle;

    public bool ShowArrowStyle => _viewModel.ShowArrowStyle;

    public bool ShowCursorType => _viewModel.ShowCursorType;

    public bool ShowCornerRadius => _viewModel.ShowCornerRadius;

    public bool ShowStrength => _viewModel.ShowStrength;

    public bool ShowSpotlightBlur => _viewModel.ShowSpotlightBlur;

    public bool ShowTextStyle => _viewModel.ShowTextStyle;

    public bool ShowTextItalic => _viewModel.ShowTextItalic;

    public bool ShowShadow => _viewModel.ShowShadow;

    public bool ShowSpeechBalloonTail => _viewModel.ShowSpeechBalloonTail;

    public bool ShowEffectEllipse => _viewModel.ShowEffectEllipse;

    public bool ShowToolOptions => _viewModel.ShowToolOptionsSeparator;

    public bool ShowToolOptionsSeparator => _viewModel.ShowToolOptionsSeparator;

    // Compatibility surface for existing toolbar bindings
    public IBrush SelectedColorBrush
    {
        get => _viewModel.SelectedColorBrush;
        set => _viewModel.SelectedColorBrush = value;
    }

    public IBrush FillColorBrush
    {
        get => _viewModel.FillColorBrush;
        set => _viewModel.FillColorBrush = value;
    }

    public IBrush TextColorBrush
    {
        get => _viewModel.TextColorBrush;
        set => _viewModel.TextColorBrush = value;
    }

    public string ActiveToolIcon => _viewModel.ActiveToolIcon;

    public string ActiveToolName => _viewModel.ActiveToolName;

    public bool IsSettingsPanelOpen
    {
        get => _viewModel.IsSettingsPanelOpen;
        set => _viewModel.IsSettingsPanelOpen = value;
    }

    public bool IsEffectsPanelOpen
    {
        get => _viewModel.IsEffectsPanelOpen;
        set => _viewModel.IsEffectsPanelOpen = value;
    }

    public bool IsEffectsButtonActive => _viewModel.IsEffectsPanelOpen;

    public double Zoom
    {
        get => _viewModel.Zoom;
        set => _viewModel.Zoom = value;
    }

    public ICommand SelectToolCommand => _viewModel.SelectToolCommand;

    public ICommand UndoCommand => _viewModel.UndoCommand;

    public ICommand RedoCommand => _viewModel.RedoCommand;

    public ICommand DeleteSelectedCommand => _viewModel.DeleteSelectedCommand;

    public ICommand ClearAnnotationsCommand => _viewModel.ClearAnnotationsCommand;

    public ICommand ToggleSettingsPanelCommand => _viewModel.ToggleSettingsPanelCommand;

    public ICommand ToggleEffectsPanelCommand => _viewModel.ToggleEffectsPanelCommand;

    public ICommand AutoCropImageCommand => _viewModel.AutoCropImageCommand;

    public ICommand Rotate90ClockwiseCommand => _viewModel.Rotate90ClockwiseCommand;

    public ICommand Rotate90CounterClockwiseCommand => _viewModel.Rotate90CounterClockwiseCommand;

    public ICommand Rotate180Command => _viewModel.Rotate180Command;

    public ICommand FlipHorizontalCommand => _viewModel.FlipHorizontalCommand;

    public ICommand FlipVerticalCommand => _viewModel.FlipVerticalCommand;

    public bool ShowFileMenu => _viewModel.ShowFileMenu;

    public ReadOnlyObservableCollection<string> RecentImageFiles => _viewModel.RecentImageFiles;

    public ReadOnlyObservableCollection<ToolbarCustomizationItemViewModel> ToolbarItems => _viewModel.ToolbarItems;
    public ReadOnlyObservableCollection<ToolbarCustomizationItemViewModel> VisibleToolbarItems => _viewModel.VisibleToolbarItems;

    public bool HasRecentImageFiles => _viewModel.HasRecentImageFiles;

    public ICommand NewImageCommand => _viewModel.NewImageCommand;

    public ICommand OpenImageCommand => _viewModel.OpenImageCommand;

    public ICommand OpenRecentImageCommand => _viewModel.OpenRecentImageCommand;

    public ICommand SaveCommand => _viewModel.SaveCommand;

    public ICommand SaveAsCommand => _viewModel.SaveAsCommand;

    public ICommand ExitEditorCommand => _viewModel.ExitEditorCommand;

    public void ExecuteToolbarItem(ToolbarCustomizationItemViewModel item) => _viewModel.ExecuteToolbarItem(item);

    private void OnRecentImageFilesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SyncRecentImageMenuItems();
    }

    private void SyncRecentImageMenuItems()
    {
        _recentImageMenuItems.Clear();

        foreach (string filePath in _viewModel.RecentImageFiles)
        {
            var menuItem = new MenuItem
            {
                Header = new TextBlock
                {
                    Text = filePath,
                    MaxWidth = 300,
                    TextTrimming = TextTrimming.LeadingCharacterEllipsis
                },
                Command = _viewModel.OpenRecentImageCommand,
                CommandParameter = filePath
            };

            menuItem.SetValue(ToolTip.TipProperty, filePath);
            _recentImageMenuItems.Add(menuItem);
        }
    }

    public void SelectTool(EditorTool tool) => _viewModel.SelectToolCommand.Execute(tool);

    public void Undo() => _viewModel.UndoCommand.Execute(null);

    public void Redo() => _viewModel.RedoCommand.Execute(null);

    public void DeleteSelection() => _viewModel.DeleteSelectedCommand.Execute(null);

    public void ClearSelection() => _viewModel.ClearAnnotationsCommand.Execute(null);

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.PropertyName))
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            return;
        }

        PropertyChanged?.Invoke(this, e);

        switch (e.PropertyName)
        {
            case nameof(MainViewModel.SelectedColor):
                OnPropertyChanged(nameof(StrokeColor));
                OnPropertyChanged(nameof(SelectedColorBrush));
                break;
            case nameof(MainViewModel.FillColor):
                OnPropertyChanged(nameof(FillColor));
                OnPropertyChanged(nameof(FillColorBrush));
                break;
            case nameof(MainViewModel.TextColor):
                OnPropertyChanged(nameof(TextColor));
                OnPropertyChanged(nameof(TextColorBrush));
                break;
            case nameof(MainViewModel.ShadowColor):
                OnPropertyChanged(nameof(ShadowColorBrush));
                break;
            case nameof(MainViewModel.HasSelectedAnnotation):
                OnPropertyChanged(nameof(HasSelection));
                break;
            case nameof(MainViewModel.HasAnnotations):
                OnPropertyChanged(nameof(HasAnnotations));
                break;
            case nameof(MainViewModel.ShowTextColor):
                OnPropertyChanged(nameof(ShowTextColor));
                break;
            case nameof(MainViewModel.ShowToolOptionsSeparator):
                OnPropertyChanged(nameof(ShowToolOptions));
                OnPropertyChanged(nameof(ShowToolOptionsSeparator));
                break;
            case nameof(MainViewModel.IsEffectsPanelOpen):
                OnPropertyChanged(nameof(IsEffectsPanelOpen));
                OnPropertyChanged(nameof(IsEffectsButtonActive));
                break;
            case nameof(MainViewModel.IsSettingsPanelOpen):
                OnPropertyChanged(nameof(IsSettingsPanelOpen));
                break;
            case nameof(MainViewModel.ShowFileMenu):
                OnPropertyChanged(nameof(ShowFileMenu));
                break;
            case nameof(MainViewModel.HasRecentImageFiles):
                OnPropertyChanged(nameof(HasRecentImageFiles));
                break;
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
