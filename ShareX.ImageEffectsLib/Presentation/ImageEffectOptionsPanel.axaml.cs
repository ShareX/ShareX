#region License Information (GPL v3)

/* ShareX - Copyright (c) 2007-2026 ShareX Team - GPL v3 */

#endregion License Information (GPL v3)

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ShareX.HelpersLib;
using ShareX.ImageEffectsLib.Localization;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using DrawingColor = System.Drawing.Color;
using DrawingPoint = System.Drawing.Point;
using DrawingSize = System.Drawing.Size;
using FormsPadding = System.Windows.Forms.Padding;

namespace ShareX.ImageEffectsLib;

public partial class ImageEffectOptionsPanel : UserControl
{
    private ImageEffect? _effect;
    private Action? _changed;
    private StackPanel _editorPanel = null!;
    private TextBlock _effectTitle = null!;
    private TextBlock _validationText = null!;

    public ImageEffectOptionsPanel()
    {
        AvaloniaXamlLoader.Load(this);
        _editorPanel = this.FindControl<StackPanel>("EditorPanel")!;
        _effectTitle = this.FindControl<TextBlock>("EffectTitle")!;
        _validationText = this.FindControl<TextBlock>("ValidationText")!;
        ShowEmptyState();
    }

    public void SetEffect(ImageEffect? effect, Action? changed)
    {
        _effect = effect;
        _changed = changed;
        _validationText.Text = string.Empty;
        _editorPanel.Children.Clear();

        if (effect == null)
        {
            ShowEmptyState();
            return;
        }

        _effectTitle.Text = string.Format(Localization.Strings.ImageEffectOptionsPanel_Effect_options_format,
            ImageEffectsLocalization.GetEffectName(effect.GetType()));
        BuildEditors();
    }

    private void ShowEmptyState()
    {
        _effectTitle.Text = Localization.Strings.ImageEffectOptionsPanel_Effect_options;
        _editorPanel.Children.Clear();
        _editorPanel.Children.Add(new TextBlock
        {
            Text = Localization.Strings.ImageEffectOptionsPanel_Select_an_effect,
            FontWeight = FontWeight.Normal
        });
    }

    private void BuildEditors()
    {
        if (_effect == null) return;

        foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(_effect).Cast<PropertyDescriptor>()
            .Where(x => x.IsBrowsable && !x.IsReadOnly))
        {
            Grid row = new() { ColumnDefinitions = new ColumnDefinitions("140,*"), ColumnSpacing = 10, MinHeight = 36 };
            TextBlock label = new()
            {
                Text = ImageEffectsLocalization.GetPropertyName(property),
                FontWeight = FontWeight.Normal,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };
            string description = ImageEffectsLocalization.GetPropertyDescription(_effect.GetType(), property);
            if (!string.IsNullOrWhiteSpace(description)) ToolTip.SetTip(label, description);
            row.Children.Add(label);

            Control editor = CreateEditor(property);
            Grid.SetColumn(editor, 1);
            row.Children.Add(editor);
            _editorPanel.Children.Add(row);
        }

        if (_editorPanel.Children.Count == 0)
        {
            _editorPanel.Children.Add(new TextBlock
            {
                Text = Localization.Strings.ImageEffectOptionsPanel_No_configurable_properties,
                FontWeight = FontWeight.Normal
            });
        }
    }

    private Control CreateEditor(PropertyDescriptor property)
    {
        object? value = property.GetValue(_effect);
        Type type = property.PropertyType;

        if (type == typeof(bool))
        {
            ToggleSwitch toggle = new()
            {
                IsChecked = value as bool? ?? false,
                OnContent = string.Empty,
                OffContent = string.Empty,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            toggle.PropertyChanged += (_, e) =>
            {
                if (e.Property == ToggleButton.IsCheckedProperty) Apply(property, toggle.IsChecked == true);
            };
            return toggle;
        }

        if (type.IsEnum && type.GetCustomAttribute<FlagsAttribute>() == null)
        {
            object[] values = Enum.GetValues(type).Cast<object>().ToArray();
            ComboBox combo = new()
            {
                ItemsSource = values.Select(enumValue => ImageEffectsLocalization.GetEnumValue(type, enumValue)).ToArray(),
                SelectedIndex = Array.IndexOf(values, value),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontWeight = FontWeight.Normal,
                ItemTemplate = new FuncDataTemplate<string>((name, _) => new TextBlock
                {
                    Text = name,
                    FontWeight = FontWeight.Normal
                })
            };
            combo.SelectionChanged += (_, _) =>
            {
                if (combo.SelectedIndex >= 0) Apply(property, values[combo.SelectedIndex]);
            };
            return combo;
        }

        if (IsNumeric(type))
        {
            NumericUpDown number = new()
            {
                Minimum = -1_000_000,
                Maximum = 1_000_000,
                Increment = IsFloating(type) ? 0.1m : 1m,
                FormatString = IsFloating(type) ? "0.###" : "0",
                Value = Convert.ToDecimal(value, CultureInfo.InvariantCulture)
            };
            number.ValueChanged += (_, _) => ApplyConvertedNumber(property, type, number.Value ?? 0);
            return number;
        }

        if (type == typeof(DrawingColor))
        {
            DrawingColor color = value is DrawingColor c ? c : DrawingColor.Transparent;
            return CreateColorButton(color, selected => Apply(property, selected));
        }

        if (type == typeof(DrawingPoint))
        {
            DrawingPoint point = value is DrawingPoint p ? p : DrawingPoint.Empty;
            return CreatePairEditor(point.X, point.Y, Localization.Strings.ImageEffectOptionsPanel_X_short,
                Localization.Strings.ImageEffectOptionsPanel_Y_short,
                (x, y) => Apply(property, new DrawingPoint(x, y)));
        }

        if (type == typeof(DrawingSize))
        {
            DrawingSize size = value is DrawingSize s ? s : DrawingSize.Empty;
            return CreatePairEditor(size.Width, size.Height, Localization.Strings.ImageEffectOptionsPanel_Width_short,
                Localization.Strings.ImageEffectOptionsPanel_Height_short,
                (x, y) => Apply(property, new DrawingSize(x, y)));
        }

        if (type == typeof(FormsPadding))
        {
            FormsPadding padding = value is FormsPadding p ? p : FormsPadding.Empty;
            return CreatePaddingEditor(padding, result => Apply(property, result));
        }

        if (type == typeof(GradientInfo))
        {
            Button button = new() { Content = Localization.Strings.ImageEffectOptionsPanel_Edit_gradient };
            GradientInfo? existingGradient = property.GetValue(_effect) as GradientInfo;
            GradientInfo gradient = existingGradient ?? new GradientInfo();
            button.Click += (_, _) =>
            {
                if (_effect != null && property.GetValue(_effect) == null)
                {
                    Apply(property, gradient);
                }
            };
            Flyout flyout = new() { Content = new GradientOptionsPanel(gradient, NotifyChanged) };
            flyout.FlyoutPresenterClasses.Add("gradient-options-flyout");
            flyout.Closed += (_, _) => gradient.Sort();
            button.Flyout = flyout;
            return button;
        }

        TextBox textBox = new()
        {
            Text = ConvertPropertyToString(property, value),
            VerticalContentAlignment = VerticalAlignment.Center
        };
        textBox.TextChanged += (_, _) => SetFromText(property, textBox.Text);
        string? editorName = property.Attributes.OfType<EditorAttribute>().FirstOrDefault()?.EditorTypeName;
        if (type == typeof(string) && editorName != null &&
            (editorName.Contains("FileNameEditor", StringComparison.Ordinal) ||
             editorName.Contains("DirectoryNameEditor", StringComparison.Ordinal)))
        {
            Grid browseGrid = new() { ColumnDefinitions = new ColumnDefinitions("*,Auto"), ColumnSpacing = 8 };
            browseGrid.Children.Add(textBox);
            Button browse = new() { Content = Localization.Strings.ImageEffectOptionsPanel_Browse };
            browse.Click += async (_, _) =>
            {
                TopLevel? topLevel = TopLevel.GetTopLevel(this);
                if (topLevel == null) return;

                if (editorName.Contains("DirectoryNameEditor", StringComparison.Ordinal))
                {
                    IReadOnlyList<Avalonia.Platform.Storage.IStorageFolder> folders = await topLevel.StorageProvider.OpenFolderPickerAsync(
                        new Avalonia.Platform.Storage.FolderPickerOpenOptions
                        {
                            AllowMultiple = false,
                            Title = string.Format(Localization.Strings.ImageEffectOptionsPanel_Select_property,
                                ImageEffectsLocalization.GetPropertyName(property))
                        });
                    if (folders.Count > 0) textBox.Text = folders[0].Path.LocalPath;
                }
                else
                {
                    IReadOnlyList<Avalonia.Platform.Storage.IStorageFile> files = await topLevel.StorageProvider.OpenFilePickerAsync(
                        new Avalonia.Platform.Storage.FilePickerOpenOptions
                        {
                            AllowMultiple = false,
                            Title = string.Format(Localization.Strings.ImageEffectOptionsPanel_Select_property,
                                ImageEffectsLocalization.GetPropertyName(property))
                        });
                    if (files.Count > 0) textBox.Text = files[0].Path.LocalPath;
                }
            };
            Grid.SetColumn(browse, 1);
            browseGrid.Children.Add(browse);
            return browseGrid;
        }

        return textBox;
    }

    private Button CreateColorButton(DrawingColor initialColor, Action<DrawingColor> changed)
    {
        Border swatch = new()
        {
            Width = 28,
            Height = 24,
            CornerRadius = new CornerRadius(3),
            BorderBrush = new SolidColorBrush(Avalonia.Media.Color.FromRgb(112, 112, 112)),
            BorderThickness = new Thickness(1)
        };
        TextBlock colorText = new() { FontWeight = FontWeight.Normal, VerticalAlignment = VerticalAlignment.Center };
        StackPanel content = new() { Orientation = Orientation.Horizontal, Spacing = 8 };
        content.Children.Add(swatch);
        content.Children.Add(colorText);

        void UpdateColorPreview(DrawingColor color)
        {
            swatch.Background = new SolidColorBrush(ToAvalonia(color));
            colorText.Text = color.A == byte.MaxValue
                ? $"#{color.R:X2}{color.G:X2}{color.B:X2}"
                : $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        UpdateColorPreview(initialColor);
        Button button = new() { Content = content, HorizontalContentAlignment = HorizontalAlignment.Left };
        Avalonia.Controls.ColorView picker = new()
        {
            MinWidth = 320,
            Color = ToAvalonia(initialColor),
            IsAlphaVisible = true,
            IsColorPreviewVisible = true
        };
        Flyout flyout = new() { Content = picker };
        picker.PropertyChanged += (_, e) =>
        {
            if (e.Property == Avalonia.Controls.ColorView.ColorProperty)
            {
                DrawingColor color = ToDrawing(picker.Color);
                UpdateColorPreview(color);
                changed(color);
            }
        };
        button.Flyout = flyout;
        return button;
    }

    private Control CreatePairEditor(int first, int second, string firstLabel, string secondLabel, Action<int, int> changed)
    {
        Grid grid = new() { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto,*"), ColumnSpacing = 6 };
        NumericUpDown a = CreateIntegerInput(first);
        NumericUpDown b = CreateIntegerInput(second);
        grid.Children.Add(new TextBlock { Text = firstLabel, FontWeight = FontWeight.Normal, VerticalAlignment = VerticalAlignment.Center });
        Grid.SetColumn(a, 1);
        grid.Children.Add(a);
        TextBlock separator = new() { Text = secondLabel, FontWeight = FontWeight.Normal, VerticalAlignment = VerticalAlignment.Center };
        Grid.SetColumn(separator, 2);
        grid.Children.Add(separator);
        Grid.SetColumn(b, 3);
        grid.Children.Add(b);
        a.ValueChanged += (_, _) => changed((int)(a.Value ?? 0), (int)(b.Value ?? 0));
        b.ValueChanged += (_, _) => changed((int)(a.Value ?? 0), (int)(b.Value ?? 0));
        return grid;
    }

    private Control CreatePaddingEditor(FormsPadding padding, Action<FormsPadding> changed)
    {
        Grid grid = new() { ColumnDefinitions = new ColumnDefinitions("*,*,*,*"), ColumnSpacing = 6 };
        int[] values = [padding.Left, padding.Top, padding.Right, padding.Bottom];
        string[] labels =
        [
            Localization.Strings.ImageEffectOptionsPanel_Left_short,
            Localization.Strings.ImageEffectOptionsPanel_Top_short,
            Localization.Strings.ImageEffectOptionsPanel_Right_short,
            Localization.Strings.ImageEffectOptionsPanel_Bottom_short
        ];
        NumericUpDown[] inputs = new NumericUpDown[4];

        void ApplyPadding() => changed(new FormsPadding((int)(inputs[0].Value ?? 0), (int)(inputs[1].Value ?? 0),
            (int)(inputs[2].Value ?? 0), (int)(inputs[3].Value ?? 0)));

        for (int i = 0; i < inputs.Length; i++)
        {
            StackPanel panel = new() { Spacing = 3 };
            panel.Children.Add(new TextBlock
            {
                Text = labels[i],
                FontWeight = FontWeight.Normal,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            inputs[i] = CreateIntegerInput(values[i]);
            inputs[i].ValueChanged += (_, _) => ApplyPadding();
            panel.Children.Add(inputs[i]);
            Grid.SetColumn(panel, i);
            grid.Children.Add(panel);
        }
        return grid;
    }

    private static NumericUpDown CreateIntegerInput(int value) => new()
    {
        Minimum = -1_000_000,
        Maximum = 1_000_000,
        Increment = 1,
        FormatString = "0",
        Value = value,
        ShowButtonSpinner = false
    };

    private void SetFromText(PropertyDescriptor property, string? text)
    {
        try
        {
            TypeConverter converter = property.Converter;
            object? value = converter.CanConvertFrom(typeof(string))
                ? converter.ConvertFromString(null, CultureInfo.CurrentCulture, text ?? string.Empty)
                : text;
            Apply(property, value);
        }
        catch (Exception ex)
        {
            _validationText.Text = ex.Message;
        }
    }

    private void Apply(PropertyDescriptor property, object? value)
    {
        if (_effect == null) return;

        try
        {
            property.SetValue(_effect, value);
            _validationText.Text = string.Empty;
            NotifyChanged();
        }
        catch (Exception ex)
        {
            _validationText.Text = ex.Message;
        }
    }

    private void ApplyConvertedNumber(PropertyDescriptor property, Type type, decimal value)
    {
        try
        {
            Apply(property, Convert.ChangeType(value, Nullable.GetUnderlyingType(type) ?? type, CultureInfo.InvariantCulture));
        }
        catch (Exception ex)
        {
            _validationText.Text = ex.Message;
        }
    }

    private void NotifyChanged() => _changed?.Invoke();

    private static string ConvertPropertyToString(PropertyDescriptor property, object? value)
    {
        try { return property.Converter.ConvertToString(null, CultureInfo.CurrentCulture, value) ?? string.Empty; }
        catch { return value?.ToString() ?? string.Empty; }
    }

    private static bool IsNumeric(Type type) => Type.GetTypeCode(Nullable.GetUnderlyingType(type) ?? type) is
        TypeCode.Byte or TypeCode.SByte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32 or
        TypeCode.Int64 or TypeCode.UInt64 or TypeCode.Single or TypeCode.Double or TypeCode.Decimal;
    private static bool IsFloating(Type type) => Type.GetTypeCode(Nullable.GetUnderlyingType(type) ?? type) is
        TypeCode.Single or TypeCode.Double or TypeCode.Decimal;
    private static Avalonia.Media.Color ToAvalonia(DrawingColor color) => Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
    private static DrawingColor ToDrawing(Avalonia.Media.Color color) => DrawingColor.FromArgb(color.A, color.R, color.G, color.B);
}
