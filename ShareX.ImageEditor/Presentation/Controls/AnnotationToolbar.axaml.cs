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
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ShareX.ImageEditor.Core.Abstractions;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.AvaloniaUI.Theming;
using ShareX.ImageEditor.Presentation.ViewModels;
using ShareX.ImageEditor.Presentation.Views;
using System.ComponentModel;

namespace ShareX.ImageEditor.Presentation.Controls;

public partial class AnnotationToolbar : UserControl
{
    private const double AccentForegroundDarkSwitchRatio = 1.75;

    public static readonly StyledProperty<bool> ShowEditingActionsProperty =
        AvaloniaProperty.Register<AnnotationToolbar, bool>(nameof(ShowEditingActions), true);

    private readonly SolidColorBrush? _activeBrush;
    private readonly SolidColorBrush? _activeForegroundBrush;
    private IPlatformSettings? _platformSettings;
    private IAnnotationToolbarAdapter? _toolbarAdapter;

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
        _activeBrush = Resources["AnnotationToolbarActiveBrush"] as SolidColorBrush;
        _activeForegroundBrush = Resources["AnnotationToolbarActiveForegroundBrush"] as SolidColorBrush;
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
        ThemeManager.ThemeChanged += OnThemeChanged;
        SetToolbarAdapter(DataContext as IAnnotationToolbarAdapter);
        RefreshPlatformColorTracking();
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        ThemeManager.ThemeChanged -= OnThemeChanged;
        SetPlatformSettings(null);
        SetToolbarAdapter(null);
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        SetToolbarAdapter(DataContext as IAnnotationToolbarAdapter);

        if (IsLoaded)
        {
            RefreshPlatformColorTracking();
        }
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

    private void OnThemeChanged(object? sender, Avalonia.Styling.ThemeVariant theme)
    {
        Dispatcher.UIThread.Post(() => UpdateAccentBrushes());
    }

    public void RefreshAccentBrushes()
    {
        UpdateAccentBrushes(_platformSettings?.GetColorValues());
    }

    private void RefreshPlatformColorTracking()
    {
        SetPlatformSettings(ShouldUseSystemAccentColor()
            ? this.GetPlatformSettings() ?? Application.Current?.PlatformSettings
            : null);

        UpdateAccentBrushes(_platformSettings?.GetColorValues());
    }

    private void SetPlatformSettings(IPlatformSettings? platformSettings)
    {
        if (ReferenceEquals(_platformSettings, platformSettings))
        {
            return;
        }

        if (_platformSettings != null)
        {
            _platformSettings.ColorValuesChanged -= OnPlatformColorValuesChanged;
        }

        _platformSettings = platformSettings;

        if (_platformSettings != null)
        {
            _platformSettings.ColorValuesChanged += OnPlatformColorValuesChanged;
        }
    }

    private void OnPlatformColorValuesChanged(object? sender, PlatformColorValues colorValues)
    {
        Dispatcher.UIThread.Post(() => UpdateAccentBrushes(colorValues));
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

    private void OnOpenOptionsClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is IAnnotationToolbarAdapter toolbar &&
            toolbar.OpenOptionsPanelCommand.CanExecute(null))
        {
            toolbar.OpenOptionsPanelCommand.Execute(null);
            e.Handled = true;
        }
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

    private void UpdateAccentBrushes(PlatformColorValues? colorValues = null)
    {
        if (_activeBrush == null || _activeForegroundBrush == null)
        {
            return;
        }

        Color accentColor = ResolveAccentColor(colorValues);
        if (accentColor.A == 0)
        {
            return;
        }

        _activeBrush.Color = accentColor;
        _activeForegroundBrush.Color = GetAccentForegroundColor(accentColor);
    }

    private Color ResolveAccentColor(PlatformColorValues? colorValues)
    {
        if (TryGetConfiguredAccentColor(out Color configuredAccentColor))
        {
            return configuredAccentColor;
        }

        Color accentColor = default;

        if (ShouldUseSystemAccentColor())
        {
            accentColor = colorValues?.AccentColor1 ?? default;
            if (accentColor.A == 0 &&
                Application.Current?.TryGetResource("SystemAccentColor", ActualThemeVariant, out object? resourceValue) == true)
            {
                accentColor = resourceValue switch
                {
                    Color color => color,
                    SolidColorBrush brush => brush.Color,
                    _ => default
                };
            }

            if (accentColor.A != 0)
            {
                return accentColor;
            }
        }

        if (TryGetResourceColor("ShareX.Color.Accent.Start", out Color resourceAccentColor))
        {
            return resourceAccentColor;
        }

        return accentColor;
    }

    private bool ShouldUseSystemAccentColor()
    {
        return GetOwnerViewModel()?.ThemeOptions.UseSystemAccentColor ?? true;
    }

    private bool TryGetConfiguredAccentColor(out Color accentColor)
    {
        if (!ShouldUseSystemAccentColor() &&
            GetOwnerViewModel() is MainViewModel { ThemeOptions.AccentColorHex: var accentColorHex } &&
            Color.TryParse(accentColorHex, out accentColor) &&
            accentColor.A != 0)
        {
            return true;
        }

        accentColor = default;
        return false;
    }

    private MainViewModel? GetOwnerViewModel()
    {
        return this.FindAncestorOfType<EditorView>()?.DataContext as MainViewModel;
    }

    private bool TryGetResourceColor(string resourceKey, out Color color)
    {
        if (TryGetResource(resourceKey, ActualThemeVariant, out object? resourceValue))
        {
            switch (resourceValue)
            {
                case Color resourceColor when resourceColor.A != 0:
                    color = resourceColor;
                    return true;
                case SolidColorBrush brush when brush.Color.A != 0:
                    color = brush.Color;
                    return true;
            }
        }

        color = default;
        return false;
    }

    private Color GetAccentForegroundColor(Color accentColor)
    {
        Color lightForeground = GetThemeColor(
            ThemeManager.ShareXDark,
            "ShareX.Color.Text",
            Color.Parse("#D8DADB"));

        Color darkForeground = GetThemeColor(
            ThemeManager.ShareXLight,
            "ShareX.Color.Text",
            Color.Parse("#4E4E4E"));

        double lightContrast = GetContrastRatio(lightForeground, accentColor);
        double darkContrast = GetContrastRatio(darkForeground, accentColor);

        return darkContrast >= lightContrast * AccentForegroundDarkSwitchRatio
            ? darkForeground
            : lightForeground;
    }

    private Color GetThemeColor(Avalonia.Styling.ThemeVariant theme, string resourceKey, Color fallback)
    {
        if (!Resources.TryGetResource(resourceKey, theme, out object? resourceValue))
        {
            return fallback;
        }

        return resourceValue switch
        {
            Color color => color,
            SolidColorBrush brush => brush.Color,
            _ => fallback
        };
    }

    private static double GetContrastRatio(Color firstColor, Color secondColor)
    {
        double firstLuminance = GetRelativeLuminance(firstColor);
        double secondLuminance = GetRelativeLuminance(secondColor);

        double lighter = Math.Max(firstLuminance, secondLuminance);
        double darker = Math.Min(firstLuminance, secondLuminance);

        return (lighter + 0.05) / (darker + 0.05);
    }

    private static double GetRelativeLuminance(Color color)
    {
        double red = LinearizeColorChannel(color.R);
        double green = LinearizeColorChannel(color.G);
        double blue = LinearizeColorChannel(color.B);

        return (0.2126 * red) + (0.7152 * green) + (0.0722 * blue);
    }

    private static double LinearizeColorChannel(byte channel)
    {
        double normalized = channel / 255.0;

        return normalized <= 0.03928
            ? normalized / 12.92
            : Math.Pow((normalized + 0.055) / 1.055, 2.4);
    }
}
