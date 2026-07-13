#region License Information (GPL v3)

/* ShareX - Copyright (c) 2007-2026 ShareX Team - GPL v3 */

#endregion License Information (GPL v3)

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using DrawingColor = System.Drawing.Color;
using DrawingPoint = System.Drawing.Point;
using DrawingSize = System.Drawing.Size;
using FormsPadding = System.Windows.Forms.Padding;

namespace ShareX.ImageEffectsLib;

public partial class ImageEffectOptionsWindow : Window
{
    private readonly ImageEffect _target;
    private readonly ImageEffect _workingCopy;
    private readonly List<Action> _committers = [];
    private StackPanel _editorPanel = null!;
    private TextBlock _validationText = null!;

    public ImageEffectOptionsWindow() : this(new Grayscale())
    {
    }

    public ImageEffectOptionsWindow(ImageEffect effect)
    {
        _target = effect;
        _workingCopy = effect.Copy();
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _editorPanel = this.FindControl<StackPanel>("EditorPanel")!;
        _validationText = this.FindControl<TextBlock>("ValidationText")!;
        this.FindControl<TextBlock>("EffectTitle")!.Text = effect.GetType().GetDescription();
        BuildEditors();
    }

    private void BuildEditors()
    {
        foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(_workingCopy).Cast<PropertyDescriptor>()
            .Where(x => x.IsBrowsable && !x.IsReadOnly).OrderBy(x => x.Category).ThenBy(x => x.DisplayName))
        {
            Grid row = new() { ColumnDefinitions = new ColumnDefinitions("170,*"), ColumnSpacing = 10, MinHeight = 36 };
            TextBlock label = new()
            {
                Text = property.DisplayName,
                FontWeight = FontWeight.Normal,
                VerticalAlignment = VerticalAlignment.Center
            };
            if (!string.IsNullOrWhiteSpace(property.Description)) ToolTip.SetTip(label, property.Description);
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
                Text = "This effect has no configurable properties.",
                FontWeight = FontWeight.Normal
            });
        }
    }

    private Control CreateEditor(PropertyDescriptor property)
    {
        object? value = property.GetValue(_workingCopy);
        Type type = property.PropertyType;

        if (type == typeof(bool))
        {
            ToggleSwitch toggle = new() { IsChecked = value as bool? ?? false, OnContent = string.Empty, OffContent = string.Empty,
                HorizontalAlignment = HorizontalAlignment.Right };
            _committers.Add(() => property.SetValue(_workingCopy, toggle.IsChecked == true));
            return toggle;
        }

        if (type.IsEnum && type.GetCustomAttribute<FlagsAttribute>() == null)
        {
            ComboBox combo = new() { ItemsSource = Enum.GetValues(type), SelectedItem = value };
            _committers.Add(() => property.SetValue(_workingCopy, combo.SelectedItem));
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
            _committers.Add(() => property.SetValue(_workingCopy,
                Convert.ChangeType(number.Value ?? 0, Nullable.GetUnderlyingType(type) ?? type, CultureInfo.InvariantCulture)));
            return number;
        }

        if (type == typeof(DrawingColor))
        {
            DrawingColor color = value is DrawingColor c ? c : DrawingColor.Transparent;
            Button button = CreateColorButton(color, selected => color = selected);
            _committers.Add(() => property.SetValue(_workingCopy, color));
            return button;
        }

        if (type == typeof(DrawingPoint))
        {
            DrawingPoint point = value is DrawingPoint p ? p : DrawingPoint.Empty;
            return CreatePairEditor(point.X, point.Y, "X", "Y", (x, y) => property.SetValue(_workingCopy, new DrawingPoint(x, y)));
        }

        if (type == typeof(DrawingSize))
        {
            DrawingSize size = value is DrawingSize s ? s : DrawingSize.Empty;
            return CreatePairEditor(size.Width, size.Height, "W", "H", (x, y) => property.SetValue(_workingCopy, new DrawingSize(x, y)));
        }

        if (type == typeof(FormsPadding))
        {
            FormsPadding padding = value is FormsPadding p ? p : FormsPadding.Empty;
            return CreatePaddingEditor(padding, result => property.SetValue(_workingCopy, result));
        }

        if (type == typeof(GradientInfo))
        {
            Button button = new() { Content = "Edit gradient..." };
            button.Click += async (_, _) =>
            {
                GradientInfo gradient = property.GetValue(_workingCopy) as GradientInfo ?? new GradientInfo();
                await new GradientOptionsWindow(gradient).ShowDialog(this);
            };
            return button;
        }

        TextBox textBox = new() { Text = ConvertPropertyToString(property, value), VerticalContentAlignment = VerticalAlignment.Center };
        string? editorName = property.Attributes.OfType<EditorAttribute>().FirstOrDefault()?.EditorTypeName;
        if (type == typeof(string) && editorName != null &&
            (editorName.Contains("FileNameEditor", StringComparison.Ordinal) || editorName.Contains("DirectoryNameEditor", StringComparison.Ordinal)))
        {
            Grid browseGrid = new() { ColumnDefinitions = new ColumnDefinitions("*,Auto"), ColumnSpacing = 8 };
            browseGrid.Children.Add(textBox);
            Button browse = new() { Content = "Browse..." };
            browse.Click += async (_, _) =>
            {
                if (editorName.Contains("DirectoryNameEditor", StringComparison.Ordinal))
                {
                    IReadOnlyList<Avalonia.Platform.Storage.IStorageFolder> folders = await StorageProvider.OpenFolderPickerAsync(
                        new Avalonia.Platform.Storage.FolderPickerOpenOptions { AllowMultiple = false, Title = $"Select {property.DisplayName}" });
                    if (folders.Count > 0) textBox.Text = folders[0].Path.LocalPath;
                }
                else
                {
                    IReadOnlyList<Avalonia.Platform.Storage.IStorageFile> files = await StorageProvider.OpenFilePickerAsync(
                        new Avalonia.Platform.Storage.FilePickerOpenOptions { AllowMultiple = false, Title = $"Select {property.DisplayName}" });
                    if (files.Count > 0) textBox.Text = files[0].Path.LocalPath;
                }
            };
            Grid.SetColumn(browse, 1);
            browseGrid.Children.Add(browse);
            _committers.Add(() => SetFromText(property, textBox.Text));
            return browseGrid;
        }

        _committers.Add(() => SetFromText(property, textBox.Text));
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
        TextBlock colorText = new()
        {
            FontWeight = FontWeight.Normal,
            VerticalAlignment = VerticalAlignment.Center
        };
        StackPanel content = new()
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8
        };
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

        void ApplySelectedColor()
        {
            DrawingColor color = ToDrawing(picker.Color);
            UpdateColorPreview(color);
            changed(color);
        }

        picker.PropertyChanged += (_, e) =>
        {
            if (e.Property == Avalonia.Controls.ColorView.ColorProperty)
            {
                ApplySelectedColor();
            }
        };
        flyout.Closed += (_, _) => ApplySelectedColor();
        button.Flyout = flyout;
        return button;
    }

    private Control CreatePairEditor(int first, int second, string firstLabel, string secondLabel, Action<int, int> commit)
    {
        Grid grid = new() { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto,*"), ColumnSpacing = 6 };
        NumericUpDown a = CreateIntegerInput(first);
        NumericUpDown b = CreateIntegerInput(second);
        grid.Children.Add(new TextBlock { Text = firstLabel, FontWeight = FontWeight.Normal, VerticalAlignment = VerticalAlignment.Center });
        Grid.SetColumn(a, 1); grid.Children.Add(a);
        TextBlock separator = new() { Text = secondLabel, FontWeight = FontWeight.Normal, VerticalAlignment = VerticalAlignment.Center };
        Grid.SetColumn(separator, 2); grid.Children.Add(separator);
        Grid.SetColumn(b, 3); grid.Children.Add(b);
        _committers.Add(() => commit((int)(a.Value ?? 0), (int)(b.Value ?? 0)));
        return grid;
    }

    private Control CreatePaddingEditor(FormsPadding padding, Action<FormsPadding> commit)
    {
        Grid grid = new() { ColumnDefinitions = new ColumnDefinitions("*,*,*,*"), ColumnSpacing = 6 };
        int[] values = [padding.Left, padding.Top, padding.Right, padding.Bottom];
        string[] labels = ["L", "T", "R", "B"];
        NumericUpDown[] inputs = new NumericUpDown[4];
        for (int i = 0; i < 4; i++)
        {
            StackPanel panel = new() { Spacing = 3 };
            panel.Children.Add(new TextBlock { Text = labels[i], FontWeight = FontWeight.Normal, HorizontalAlignment = HorizontalAlignment.Center });
            inputs[i] = CreateIntegerInput(values[i]);
            panel.Children.Add(inputs[i]);
            Grid.SetColumn(panel, i);
            grid.Children.Add(panel);
        }
        _committers.Add(() => commit(new FormsPadding((int)(inputs[0].Value ?? 0), (int)(inputs[1].Value ?? 0),
            (int)(inputs[2].Value ?? 0), (int)(inputs[3].Value ?? 0))));
        return grid;
    }

    private static NumericUpDown CreateIntegerInput(int value) => new()
    {
        Minimum = -1_000_000, Maximum = 1_000_000, Increment = 1, FormatString = "0", Value = value,
        ShowButtonSpinner = false
    };

    private void SetFromText(PropertyDescriptor property, string? text)
    {
        TypeConverter converter = property.Converter;
        object? value = converter.CanConvertFrom(typeof(string))
            ? converter.ConvertFromString(null, CultureInfo.CurrentCulture, text ?? string.Empty)
            : text;
        property.SetValue(_workingCopy, value);
    }

    private static string ConvertPropertyToString(PropertyDescriptor property, object? value)
    {
        try { return property.Converter.ConvertToString(null, CultureInfo.CurrentCulture, value) ?? string.Empty; }
        catch { return value?.ToString() ?? string.Empty; }
    }

    private void OnSaveClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            foreach (Action commit in _committers) commit();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(_workingCopy).Cast<PropertyDescriptor>().Where(x => !x.IsReadOnly))
                property.SetValue(_target, property.GetValue(_workingCopy));
            Close(true);
        }
        catch (Exception ex)
        {
            _validationText.Text = ex.Message;
        }
    }

    private void OnCancelClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close(false);

    private static bool IsNumeric(Type type) => Type.GetTypeCode(Nullable.GetUnderlyingType(type) ?? type) is
        TypeCode.Byte or TypeCode.SByte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32 or
        TypeCode.Int64 or TypeCode.UInt64 or TypeCode.Single or TypeCode.Double or TypeCode.Decimal;
    private static bool IsFloating(Type type) => Type.GetTypeCode(Nullable.GetUnderlyingType(type) ?? type) is
        TypeCode.Single or TypeCode.Double or TypeCode.Decimal;
    private static Avalonia.Media.Color ToAvalonia(DrawingColor color) => Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
    private static DrawingColor ToDrawing(Avalonia.Media.Color color) => DrawingColor.FromArgb(color.A, color.R, color.G, color.B);
}
