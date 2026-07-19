#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ShareX;

public partial class CustomUploaderKeyValueEditor : UserControl
{
    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<CustomUploaderKeyValueEditor, string>(nameof(Header), string.Empty);

    public static readonly StyledProperty<CustomUploaderKeyValueCollection?> RowsProperty =
        AvaloniaProperty.Register<CustomUploaderKeyValueEditor, CustomUploaderKeyValueCollection?>(nameof(Rows));

    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public CustomUploaderKeyValueCollection? Rows
    {
        get => GetValue(RowsProperty);
        set => SetValue(RowsProperty, value);
    }

    public CustomUploaderKeyValueEditor() => InitializeComponent();

    private void OnAddClick(object? sender, RoutedEventArgs e) => Rows?.AddNew();

    private void OnRemoveClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: CustomUploaderKeyValueRow row }) Rows?.Remove(row);
    }
}
