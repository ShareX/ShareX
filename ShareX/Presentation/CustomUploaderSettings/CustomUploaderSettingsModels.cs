#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Newtonsoft.Json;
using ShareX.HelpersLib;
using ShareX.UploadersLib;
using ShareX.Localization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ShareX;

public abstract class CustomUploaderNotifyObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public sealed record CustomUploaderEditorSection(string Title, string Icon);

public sealed class CustomUploaderKeyValueRow : CustomUploaderNotifyObject
{
    private string _key;
    private string _value;
    private bool _hasDuplicateKey;

    internal Action? Changed { get; set; }

    public string Key
    {
        get => _key;
        set
        {
            if (SetField(ref _key, value ?? string.Empty)) Changed?.Invoke();
        }
    }

    public string Value
    {
        get => _value;
        set
        {
            if (SetField(ref _value, value ?? string.Empty)) Changed?.Invoke();
        }
    }

    public bool HasDuplicateKey
    {
        get => _hasDuplicateKey;
        internal set => SetField(ref _hasDuplicateKey, value);
    }

    public string? Error => HasDuplicateKey ? Strings.CustomUploaderSettingsWindow_DuplicateNamesNotAllowed : null;

    public CustomUploaderKeyValueRow(string key = "", string value = "")
    {
        _key = key;
        _value = value;
        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(HasDuplicateKey)) OnPropertyChanged(nameof(Error));
        };
    }
}

public sealed class CustomUploaderKeyValueCollection : ObservableCollection<CustomUploaderKeyValueRow>
{
    private readonly Action<Dictionary<string, string>> _setter;
    private bool _loading;

    public CustomUploaderKeyValueCollection(Dictionary<string, string>? values, Action<Dictionary<string, string>> setter)
    {
        _setter = setter;
        _loading = true;
        foreach ((string key, string value) in values ?? []) Add(new CustomUploaderKeyValueRow(key, value));
        _loading = false;
        Synchronize();
    }

    public void AddNew() => Add(new CustomUploaderKeyValueRow());

    protected override void InsertItem(int index, CustomUploaderKeyValueRow item)
    {
        item.Changed = Synchronize;
        base.InsertItem(index, item);
        Synchronize();
    }

    protected override void RemoveItem(int index)
    {
        this[index].Changed = null;
        base.RemoveItem(index);
        Synchronize();
    }

    protected override void ClearItems()
    {
        foreach (CustomUploaderKeyValueRow row in this) row.Changed = null;
        base.ClearItems();
        Synchronize();
    }

    private void Synchronize()
    {
        if (_loading) return;

        string[] duplicateKeys = this
            .Select(x => x.Key)
            .Where(x => !string.IsNullOrEmpty(x))
            .GroupBy(x => x, StringComparer.Ordinal)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToArray();

        Dictionary<string, string> result = new(StringComparer.Ordinal);
        foreach (CustomUploaderKeyValueRow row in this)
        {
            string key = row.Key;
            row.HasDuplicateKey = duplicateKeys.Contains(key, StringComparer.Ordinal);
            if (!string.IsNullOrEmpty(key) && !result.ContainsKey(key)) result.Add(key, row.Value);
        }

        _setter(result);
    }
}

public sealed class CustomUploaderEditorItem : CustomUploaderNotifyObject
{
    internal CustomUploaderItem Model { get; }

    public string Title => Model.ToString();
    public string HostName => URLHelpers.GetHostName(Model.RequestURL) ?? string.Empty;

    public string Name
    {
        get => Model.Name ?? string.Empty;
        set
        {
            value ??= string.Empty;
            if (Model.Name == value) return;
            Model.Name = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Title));
        }
    }

    public bool IsImageUploader
    {
        get => HasDestination(CustomUploaderDestinationType.ImageUploader);
        set => SetDestination(CustomUploaderDestinationType.ImageUploader, value);
    }

    public bool IsTextUploader
    {
        get => HasDestination(CustomUploaderDestinationType.TextUploader);
        set => SetDestination(CustomUploaderDestinationType.TextUploader, value);
    }

    public bool IsFileUploader
    {
        get => HasDestination(CustomUploaderDestinationType.FileUploader);
        set => SetDestination(CustomUploaderDestinationType.FileUploader, value);
    }

    public bool IsURLShortener
    {
        get => HasDestination(CustomUploaderDestinationType.URLShortener);
        set => SetDestination(CustomUploaderDestinationType.URLShortener, value);
    }

    public bool IsURLSharingService
    {
        get => HasDestination(CustomUploaderDestinationType.URLSharingService);
        set => SetDestination(CustomUploaderDestinationType.URLSharingService, value);
    }

    public int RequestMethodIndex
    {
        get => (int)Model.RequestMethod;
        set
        {
            HttpMethod method = (HttpMethod)value;
            if (Model.RequestMethod == method) return;
            Model.RequestMethod = method;
            OnPropertyChanged();
        }
    }

    public string RequestURL
    {
        get => Model.RequestURL ?? string.Empty;
        set
        {
            value ??= string.Empty;
            if (Model.RequestURL == value) return;
            Model.RequestURL = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HostName));
            OnPropertyChanged(nameof(Title));
        }
    }

    public int BodyIndex
    {
        get => (int)Model.Body;
        set
        {
            CustomUploaderBody body = (CustomUploaderBody)value;
            if (Model.Body == body) return;
            Model.Body = body;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsArgumentsBody));
            OnPropertyChanged(nameof(IsMultipartBody));
            OnPropertyChanged(nameof(IsDataBody));
            OnPropertyChanged(nameof(IsJsonBody));
            OnPropertyChanged(nameof(IsXmlBody));
            OnPropertyChanged(nameof(HasNoBodyEditor));
        }
    }

    public bool IsArgumentsBody => Model.Body is CustomUploaderBody.MultipartFormData or CustomUploaderBody.FormURLEncoded;
    public bool IsMultipartBody => Model.Body == CustomUploaderBody.MultipartFormData;
    public bool IsDataBody => Model.Body is CustomUploaderBody.JSON or CustomUploaderBody.XML;
    public bool IsJsonBody => Model.Body == CustomUploaderBody.JSON;
    public bool IsXmlBody => Model.Body == CustomUploaderBody.XML;
    public bool HasNoBodyEditor => !IsArgumentsBody && !IsDataBody;

    public string FileFormName
    {
        get => Model.FileFormName ?? string.Empty;
        set => SetText(Model.FileFormName, value, x => Model.FileFormName = x);
    }

    public string Data
    {
        get => Model.Data ?? string.Empty;
        set => SetText(Model.Data, value, x => Model.Data = x);
    }

    public string URL
    {
        get => Model.URL ?? string.Empty;
        set => SetText(Model.URL, value, x => Model.URL = x);
    }

    public string ThumbnailURL
    {
        get => Model.ThumbnailURL ?? string.Empty;
        set => SetText(Model.ThumbnailURL, value, x => Model.ThumbnailURL = x);
    }

    public string DeletionURL
    {
        get => Model.DeletionURL ?? string.Empty;
        set => SetText(Model.DeletionURL, value, x => Model.DeletionURL = x);
    }

    public string ErrorMessage
    {
        get => Model.ErrorMessage ?? string.Empty;
        set => SetText(Model.ErrorMessage, value, x => Model.ErrorMessage = x);
    }

    public CustomUploaderKeyValueCollection Parameters { get; }
    public CustomUploaderKeyValueCollection Headers { get; }
    public CustomUploaderKeyValueCollection Arguments { get; }

    public CustomUploaderEditorItem(CustomUploaderItem model)
    {
        Model = model;
        Parameters = new CustomUploaderKeyValueCollection(model.Parameters, value => model.Parameters = value);
        Headers = new CustomUploaderKeyValueCollection(model.Headers, value => model.Headers = value);
        Arguments = new CustomUploaderKeyValueCollection(model.Arguments, value => model.Arguments = value);
    }

    public override string ToString() => Title;

    public void FormatData(Formatting formatting)
    {
        if (string.IsNullOrWhiteSpace(Data)) return;
        Data = Model.Body switch
        {
            CustomUploaderBody.JSON => Helpers.JSONFormat(Data, formatting),
            CustomUploaderBody.XML when formatting == Formatting.Indented => Helpers.XMLFormat(Data),
            _ => Data
        };
    }

    private bool HasDestination(CustomUploaderDestinationType type) => Model.DestinationType.HasFlag(type);

    private void SetDestination(CustomUploaderDestinationType type, bool enabled, [CallerMemberName] string? propertyName = null)
    {
        CustomUploaderDestinationType value = enabled ? Model.DestinationType | type : Model.DestinationType & ~type;
        if (Model.DestinationType == value) return;
        Model.DestinationType = value;
        OnPropertyChanged(propertyName);
    }

    private void SetText(string? current, string? value, Action<string> setter, [CallerMemberName] string? propertyName = null)
    {
        value ??= string.Empty;
        if (current == value) return;
        setter(value);
        OnPropertyChanged(propertyName);
    }
}
