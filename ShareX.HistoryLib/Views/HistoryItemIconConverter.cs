#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Data.Converters;
using ShareX.AvaloniaUI.Theming;
using System;
using System.Globalization;

namespace ShareX.HistoryLib;

public sealed class HistoryItemIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not HistoryItem item)
        {
            return LucideIcons.file;
        }

        if (item.Favorite)
        {
            return LucideIcons.star;
        }

        return item.Type?.ToLowerInvariant() switch
        {
            "image" => LucideIcons.image,
            "text" => LucideIcons.file_text,
            "file" => LucideIcons.file,
            _ => LucideIcons.link
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
