#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Media.Imaging;
using ShareX.HelpersLib;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShareX;

public sealed record EnumOption<T>(T Value, string DisplayName)
{
    public override string ToString() => DisplayName;
}

public sealed record LanguageOption(SupportedLanguage Value, string DisplayName, Bitmap? Icon)
{
    public override string ToString() => DisplayName;
}

public sealed class ClipboardFormatItem : INotifyPropertyChanged
{
    public ClipboardFormat Model { get; }

    public string Description
    {
        get => Model.Description ?? string.Empty;
        set
        {
            if (Model.Description == value)
            {
                return;
            }

            Model.Description = value;
            OnPropertyChanged();
        }
    }

    public string Format
    {
        get => Model.Format ?? string.Empty;
        set
        {
            if (Model.Format == value)
            {
                return;
            }

            Model.Format = value;
            OnPropertyChanged();
        }
    }

    public ClipboardFormatItem(ClipboardFormat model)
    {
        Model = model;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
