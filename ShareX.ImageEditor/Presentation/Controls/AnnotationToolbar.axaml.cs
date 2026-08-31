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
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.VisualTree;
using ShareX.ImageEditor.Core.Abstractions;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Presentation.ViewModels;
using System.ComponentModel;

namespace ShareX.ImageEditor.Presentation.Controls;

public partial class AnnotationToolbar : UserControl
{
    public static readonly StyledProperty<bool> ShowEditingActionsProperty =
        AvaloniaProperty.Register<AnnotationToolbar, bool>(nameof(ShowEditingActions), true);

    public static readonly StyledProperty<bool> ShowToolOptionsProperty =
        AvaloniaProperty.Register<AnnotationToolbar, bool>(nameof(ShowToolOptions), true);

    public static readonly StyledProperty<Thickness> MainToolbarBorderThicknessProperty =
        AvaloniaProperty.Register<AnnotationToolbar, Thickness>(nameof(MainToolbarBorderThickness), new Thickness(1));

    public static readonly StyledProperty<CornerRadius> MainToolbarCornerRadiusProperty =
        AvaloniaProperty.Register<AnnotationToolbar, CornerRadius>(nameof(MainToolbarCornerRadius), new CornerRadius(0, 0, 4, 4));

    private IAnnotationToolbarAdapter? _toolbarAdapter;
    private ContentControl _mainToolbarLeadingContentHost = null!;

    public event EventHandler<IBrush>? ColorChanged;
    public event EventHandler<IBrush>? FillColorChanged;
    public event EventHandler<IBrush>? TextColorChanged;
    public event EventHandler<int>? WidthChanged;
    public event EventHandler<BorderStyle>? BorderStyleChanged;
    public event EventHandler<int>? CornerRadiusChanged;
    public event EventHandler<float>? FontSizeChanged;
    public event EventHandler<string>? FontFamilyChanged;
    public event EventHandler<ArrowStyle>? ArrowStyleChanged;
    public event EventHandler<CursorType>? CursorTypeChanged;
    public event EventHandler<float>? StrengthChanged;
    public event EventHandler<float>? SpotlightBlurChanged;
    public event EventHandler<bool>? TextBoldChanged;
    public event EventHandler<bool>? TextItalicChanged;
    public event EventHandler<bool>? ShadowChanged;
    public event EventHandler? ShadowSettingsChanged;
    public event EventHandler<bool>? SpeechBalloonTailChanged;
    public event EventHandler<bool>? EffectEllipseChanged;
    public event EventHandler<Control>? FavoriteEffectsMenuRequested;

    public AnnotationToolbar()
    {
        InitializeComponent();
        _mainToolbarLeadingContentHost = this.FindControl<ContentControl>("MainToolbarLeadingContentHost")!;
        WireCompatibilityEvents();
        DataContextChanged += OnDataContextChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public bool ShowEditingActions
    {
        get => GetValue(ShowEditingActionsProperty);
        set => SetValue(ShowEditingActionsProperty, value);
    }

    public bool ShowToolOptions
    {
        get => GetValue(ShowToolOptionsProperty);
        set => SetValue(ShowToolOptionsProperty, value);
    }

    public Thickness MainToolbarBorderThickness
    {
        get => GetValue(MainToolbarBorderThicknessProperty);
        set => SetValue(MainToolbarBorderThicknessProperty, value);
    }

    public CornerRadius MainToolbarCornerRadius
    {
        get => GetValue(MainToolbarCornerRadiusProperty);
        set => SetValue(MainToolbarCornerRadiusProperty, value);
    }

    public Control? MainToolbarLeadingContent
    {
        get => _mainToolbarLeadingContentHost.Content as Control;
        set
        {
            _mainToolbarLeadingContentHost.Content = value;
            _mainToolbarLeadingContentHost.IsVisible = value != null;
        }
    }

    public void OpenFileMenu()
    {
        Button? fileButton = this.GetVisualDescendants()
            .OfType<Button>()
            .FirstOrDefault(button => button.DataContext is ToolbarCustomizationItemViewModel item && item.IsFileMenu);

        if (fileButton?.Flyout is FlyoutBase flyout)
        {
            flyout.ShowAt(fileButton);
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void WireCompatibilityEvents()
    {
        if (this.FindControl<ColorPickerDropdown>("StrokeColorPicker") is ColorPickerDropdown strokePicker)
        {
            strokePicker.ColorChanged += (_, brush) => ColorChanged?.Invoke(this, brush);
        }

        if (this.FindControl<ColorPickerDropdown>("FillColorPicker") is ColorPickerDropdown fillPicker)
        {
            fillPicker.ColorChanged += (_, brush) => FillColorChanged?.Invoke(this, brush);
        }

        if (this.FindControl<ColorPickerDropdown>("TextColorPicker") is ColorPickerDropdown textColorPicker)
        {
            textColorPicker.ColorChanged += (_, brush) => TextColorChanged?.Invoke(this, brush);
        }

        if (this.FindControl<WidthPickerDropdown>("StrokeWidthPicker") is WidthPickerDropdown widthPicker)
        {
            widthPicker.WidthChanged += (_, width) => WidthChanged?.Invoke(this, width);
        }

        if (this.FindControl<BorderStylePickerDropdown>("BorderStylePicker") is BorderStylePickerDropdown borderStylePicker)
        {
            borderStylePicker.BorderStyleChanged += (_, borderStyle) => BorderStyleChanged?.Invoke(this, borderStyle);
        }

        if (this.FindControl<CornerRadiusPickerDropdown>("CornerRadiusPicker") is CornerRadiusPickerDropdown cornerRadiusPicker)
        {
            cornerRadiusPicker.CornerRadiusChanged += (_, cornerRadius) => CornerRadiusChanged?.Invoke(this, cornerRadius);
        }

        if (this.FindControl<FontSizePickerDropdown>("FontSizePicker") is FontSizePickerDropdown fontSizePicker)
        {
            fontSizePicker.FontSizeChanged += (_, fontSize) => FontSizeChanged?.Invoke(this, fontSize);
        }

        if (this.FindControl<FontFamilyPickerDropdown>("FontFamilyPicker") is FontFamilyPickerDropdown fontFamilyPicker)
        {
            fontFamilyPicker.FontFamilyChanged += (_, fontFamily) => FontFamilyChanged?.Invoke(this, fontFamily);
        }

        if (this.FindControl<ArrowStylePickerDropdown>("ArrowStylePicker") is ArrowStylePickerDropdown arrowStylePicker)
        {
            arrowStylePicker.ArrowStyleChanged += (_, arrowStyle) => ArrowStyleChanged?.Invoke(this, arrowStyle);
        }

        if (this.FindControl<CursorTypePickerDropdown>("CursorTypePicker") is CursorTypePickerDropdown cursorTypePicker)
        {
            cursorTypePicker.CursorTypeChanged += (_, cursorType) => CursorTypeChanged?.Invoke(this, cursorType);
        }

        if (this.FindControl<StrengthSlider>("EffectStrengthSlider") is StrengthSlider strengthSlider)
        {
            strengthSlider.StrengthChanged += (_, strength) => StrengthChanged?.Invoke(this, strength);
        }

        if (this.FindControl<StrengthSlider>("SpotlightBlurSlider") is StrengthSlider spotlightBlurSlider)
        {
            spotlightBlurSlider.StrengthChanged += (_, blurAmount) => SpotlightBlurChanged?.Invoke(this, blurAmount);
        }
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        SetToolbarAdapter(DataContext as IAnnotationToolbarAdapter);
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        SetToolbarAdapter(null);
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        SetToolbarAdapter(DataContext as IAnnotationToolbarAdapter);
    }

    private void SetToolbarAdapter(IAnnotationToolbarAdapter? toolbarAdapter)
    {
        if (ReferenceEquals(_toolbarAdapter, toolbarAdapter))
        {
            return;
        }

        if (_toolbarAdapter != null)
        {
            _toolbarAdapter.PropertyChanged -= OnToolbarAdapterPropertyChanged;
        }

        _toolbarAdapter = toolbarAdapter;

        if (_toolbarAdapter != null)
        {
            _toolbarAdapter.PropertyChanged += OnToolbarAdapterPropertyChanged;
        }
    }

    private void OnToolbarAdapterPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(IAnnotationToolbarAdapter.ShadowColorBrush)
            or nameof(IAnnotationToolbarAdapter.ShadowBlurRadius)
            or nameof(IAnnotationToolbarAdapter.ShadowOpacity)
            or nameof(IAnnotationToolbarAdapter.ShadowOffsetX)
            or nameof(IAnnotationToolbarAdapter.ShadowOffsetY))
        {
            ShadowSettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnSelectToolClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: EditorTool tool } && DataContext is IAnnotationToolbarAdapter toolbar)
        {
            toolbar.SelectTool(tool);
        }
    }

    private void OnToolbarItemClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: ToolbarCustomizationItemViewModel item } &&
            DataContext is EditorToolbarAdapter toolbar)
        {
            toolbar.ExecuteToolbarItem(item);
            e.Handled = true;
        }
    }

    private void OnUndoClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is IAnnotationToolbarAdapter toolbar)
        {
            toolbar.Undo();
        }
    }

    private void OnRedoClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is IAnnotationToolbarAdapter toolbar)
        {
            toolbar.Redo();
        }
    }

    private void OnDeleteClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is IAnnotationToolbarAdapter toolbar)
        {
            toolbar.DeleteSelection();
        }
    }

    private void OnClearClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is IAnnotationToolbarAdapter toolbar)
        {
            toolbar.ClearSelection();
        }
    }

    private void OnToolbarItemPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control { DataContext: ToolbarCustomizationItemViewModel item } control ||
            item.Id != ToolbarCustomizationItemViewModel.ImageEffectsItemId)
        {
            return;
        }

        PointerPointProperties properties = e.GetCurrentPoint(control).Properties;
        if (!properties.IsRightButtonPressed)
        {
            return;
        }

        FavoriteEffectsMenuRequested?.Invoke(this, control);
        e.Handled = true;
    }

    private void OnTextBoldClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is IAnnotationToolbarAdapter toolbar)
        {
            toolbar.TextBold = !toolbar.TextBold;
            TextBoldChanged?.Invoke(this, toolbar.TextBold);
        }
    }

    private void OnTextItalicClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is IAnnotationToolbarAdapter toolbar)
        {
            toolbar.TextItalic = !toolbar.TextItalic;
            TextItalicChanged?.Invoke(this, toolbar.TextItalic);
        }
    }

    private void OnShadowClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is IAnnotationToolbarAdapter toolbar)
        {
            toolbar.ShadowEnabled = !toolbar.ShadowEnabled;
            ShadowChanged?.Invoke(this, toolbar.ShadowEnabled);
        }
    }

    private void OnShadowPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control)
        {
            return;
        }

        PointerPointProperties properties = e.GetCurrentPoint(control).Properties;
        if (!properties.IsRightButtonPressed)
        {
            return;
        }

        if (this.FindControl<Popup>("ShadowOptionsPopup") is Popup popup)
        {
            popup.IsOpen = true;
            e.Handled = true;
        }
    }

    private void OnSpeechBalloonTailClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is IAnnotationToolbarAdapter toolbar)
        {
            toolbar.SpeechBalloonTail = !toolbar.SpeechBalloonTail;
            SpeechBalloonTailChanged?.Invoke(this, toolbar.SpeechBalloonTail);
        }
    }

    private void OnEffectEllipseClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is IAnnotationToolbarAdapter toolbar)
        {
            toolbar.EffectEllipse = !toolbar.EffectEllipse;
            EffectEllipseChanged?.Invoke(this, toolbar.EffectEllipse);
        }
    }

}
