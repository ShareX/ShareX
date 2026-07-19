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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ShareX;

public sealed record EnumOption<T>(T Value, string DisplayName)
{
    public override string ToString() => DisplayName;
}

public sealed class ClipboardFormatItem : INotifyPropertyChanged
{
    private readonly Action _changed;

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
            _changed();
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
            _changed();
        }
    }

    public ClipboardFormatItem(ClipboardFormat model, Action changed)
    {
        Model = model;
        _changed = changed;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public sealed record UploaderOption<T>(T Value, string DisplayName) where T : struct, Enum
{
    public override string ToString() => DisplayName;
}

public sealed record AdvancedEnumOption(object Value, string DisplayName)
{
    public override string ToString() => DisplayName;
}

public sealed record AdvancedSettingCategory(string Title, ObservableCollection<AdvancedSettingItem> Items);

public sealed class AdvancedSettingItem : INotifyPropertyChanged
{
    private readonly object _target;
    private readonly PropertyDescriptor _property;
    private readonly Action _changed;

    public string Name { get; }
    public string Category => _property.Category;
    public string Description => _property.Description;
    public bool IsBoolean => _property.PropertyType == typeof(bool);
    public bool IsInteger => _property.PropertyType == typeof(int);
    public bool IsText => _property.PropertyType == typeof(string);
    public bool IsEnum => _property.PropertyType.IsEnum;
    public IReadOnlyList<AdvancedEnumOption> EnumOptions { get; }

    public bool BoolValue
    {
        get => IsBoolean && _property.GetValue(_target) is bool value && value;
        set
        {
            if (IsBoolean)
            {
                SetValue(value);
            }
        }
    }

    public decimal NumberValue
    {
        get => IsInteger && _property.GetValue(_target) is int value
            ? Convert.ToDecimal(value, CultureInfo.InvariantCulture)
            : 0;
        set
        {
            if (IsInteger)
            {
                SetValue(decimal.ToInt32(value));
            }
        }
    }

    public string TextValue
    {
        get => IsText ? _property.GetValue(_target) as string ?? string.Empty : string.Empty;
        set
        {
            if (IsText)
            {
                SetValue(value);
            }
        }
    }

    public AdvancedEnumOption? SelectedEnumOption
    {
        get
        {
            if (!IsEnum)
            {
                return null;
            }

            object? value = _property.GetValue(_target);
            return EnumOptions.FirstOrDefault(x => Equals(x.Value, value));
        }
        set
        {
            if (IsEnum && value != null)
            {
                SetValue(value.Value);
            }
        }
    }

    public AdvancedSettingItem(object target, PropertyDescriptor property, Action changed)
    {
        _target = target;
        _property = property;
        _changed = changed;
        Name = Humanize(property.DisplayName);
        EnumOptions = property.PropertyType.IsEnum
            ? Enum.GetValues(property.PropertyType).Cast<object>()
                .Select(x => new AdvancedEnumOption(x, ((Enum)x).GetLocalizedDescription()))
                .ToArray()
            : [];
    }

    private void SetValue(object value)
    {
        if (Equals(_property.GetValue(_target), value))
        {
            return;
        }

        _property.SetValue(_target, value);
        OnPropertyChanged(nameof(BoolValue));
        OnPropertyChanged(nameof(NumberValue));
        OnPropertyChanged(nameof(TextValue));
        OnPropertyChanged(nameof(SelectedEnumOption));
        _changed();
    }

    private static string Humanize(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        List<char> result = new(value.Length + 8) { value[0] };

        for (int i = 1; i < value.Length; i++)
        {
            char current = value[i];
            char previous = value[i - 1];

            if (char.IsUpper(current) && !char.IsUpper(previous))
            {
                result.Add(' ');
            }

            result.Add(current);
        }

        return new string(result.ToArray());
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
