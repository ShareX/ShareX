#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using ShareX.AvaloniaUI.Controls;
using ShareX.HelpersLib;
using ShareX.UploadersLib.FileUploaders;
using ShareX.UploadersLib.ImageUploaders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ShareX.UploadersLib;

internal sealed class DestinationSettingsPageBuilder
{
    private readonly UploadersConfig _config;
    private readonly Dictionary<IList, ObservableCollection<object>> _listItems = [];

    public DestinationSettingsPageBuilder(UploadersConfig config)
    {
        _config = config;
    }

    public IReadOnlyDictionary<string, Control> BuildPages()
    {
        Dictionary<string, Control> pages = [];
        PropertyInfo[] properties = typeof(UploadersConfig).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        foreach (DestinationCategoryDefinition category in DestinationSettingsViewModel.Categories)
        {
            foreach (DestinationPageDefinition definition in category.Pages)
            {
                PropertyInfo[] pageProperties = properties
                    .Where(property => definition.Prefixes.Any(prefix => property.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
                    .Where(IsEditableConfigProperty)
                    .ToArray();
                pages.Add(definition.Id, BuildPage(definition, category.Icon, pageProperties));
            }
        }

        return pages;
    }

    private Control BuildPage(DestinationPageDefinition definition, string icon, PropertyInfo[] properties)
    {
        List<Control> cards = [];
        Control? accountCard = new DestinationSettingsAccounts(_config).Build(definition.Id);
        if (accountCard != null)
        {
            cards.Add(accountCard);
        }

        if (definition.Id == "imgur")
        {
            cards.Add(BuildImgurAlbumsCard());
        }
        else if (definition.Id == "photobucket")
        {
            cards.Add(BuildPhotobucketAlbumsCard());
        }
        else if (definition.Id == "onedrive")
        {
            cards.Add(BuildOneDriveFolderCard());
        }
        else if (definition.Id == "box")
        {
            cards.Add(BuildBoxFolderCard());
        }
        else if (definition.Id == "mega")
        {
            cards.Add(BuildMegaFolderTreeCard());
        }
        else if (definition.Id == "amazon-s3")
        {
            cards.AddRange(BuildAmazonS3Cards());
        }
        else if (definition.Id == "img-fish")
        {
            cards.Add(BuildImgFishCard());
        }

        List<Control> simpleEditors = [];
        foreach (PropertyInfo property in properties)
        {
            DestinationMember member = new(property);
            if (ShouldSkipMember(definition.Id, member))
            {
                continue;
            }

            object? value = member.GetValue(_config);
            Type type = Nullable.GetUnderlyingType(member.ValueType) ?? member.ValueType;
            string label = GetTopLevelLabel(member.Name, definition.Prefixes);

            if (IsSimpleType(type) || IsStringList(type))
            {
                simpleEditors.Add(EditorRow(_config, member, label));
            }
            else if (value is IList list)
            {
                cards.Add(Card(label, BuildListEditor(list, member, _config)));
            }
            else if (value != null)
            {
                Control[] nested = GetEditableMembers(value.GetType()).Select(x => EditorRow(value, x, FormatLabel(x.Name))).ToArray();
                if (nested.Length > 0)
                {
                    cards.Add(Card(label is "Settings" or "Uploader"
                        ? Localization.Strings.DestinationSettings_Settings
                        : label, nested));
                }
            }
        }

        if (simpleEditors.Count > 0)
        {
            cards.Add(Card(Localization.Strings.DestinationSettings_Settings, simpleEditors.ToArray()));
        }

        if (cards.Count == 0)
        {
            cards.Add(Card(Localization.Strings.DestinationSettings_Settings,
                Hint(Localization.Strings.DestinationSettings_No_additional_configuration_required)));
        }

        return Page(definition.Id, definition.Title, icon, cards.ToArray());
    }

    private IEnumerable<Control> BuildAmazonS3Cards()
    {
        AmazonS3Settings settings = _config.AmazonS3Settings;
        TextBlock preview = Hint(string.Empty);

        void UpdatePreview()
        {
            preview.Text = new AmazonS3(settings).GetPreviewURL();
        }

        TextBox accessKey = Text(() => settings.AccessKeyID, value => settings.AccessKeyID = value);
        Button accessKeyOpen = Button("...", () =>
            URLHelpers.OpenURL("https://console.aws.amazon.com/iam/home?#security_credential"));

        TextBox secretKey = Text(() => settings.SecretAccessKey, value => settings.SecretAccessKey = value);
        secretKey.PasswordChar = '●';

        TextBox endpoint = Text(() => settings.Endpoint, value =>
        {
            settings.Endpoint = value;
            UpdatePreview();
        });
        TextBox region = Text(() => settings.Region, value =>
        {
            settings.Region = value;
            UpdatePreview();
        });

        ComboBox endpoints = new() { ItemsSource = AmazonS3.Endpoints };
        endpoints.Classes.Add("form-control");
        endpoints.SelectedItem = AmazonS3.Endpoints.FirstOrDefault(x =>
            x.Endpoint.Equals(settings.Endpoint, StringComparison.OrdinalIgnoreCase));
        endpoints.SelectionChanged += (_, _) =>
        {
            if (endpoints.SelectedItem is AmazonS3Endpoint selected)
            {
                ((DestinationValue<string>)endpoint.DataContext!).Value = selected.Endpoint;
                ((DestinationValue<string>)region.DataContext!).Value = selected.Region;
            }
        };

        TextBox bucket = Text(() => settings.Bucket, value =>
        {
            settings.Bucket = value;
            UpdatePreview();
        });
        Button bucketOpen = Button("...", () => URLHelpers.OpenURL("https://console.aws.amazon.com/s3/home"));

        TextBox objectPrefix = Text(() => settings.ObjectPrefix, value =>
        {
            settings.ObjectPrefix = value;
            UpdatePreview();
        });
        TextBox customDomain = Text(() => settings.CustomDomain, value =>
        {
            settings.CustomDomain = value;
            UpdatePreview();
        });
        customDomain.IsEnabled = settings.UseCustomCNAME;
        CheckBox useCustomDomain = Check(FormatLabel(nameof(settings.UseCustomCNAME)), () => settings.UseCustomCNAME, value =>
        {
            settings.UseCustomCNAME = value;
            customDomain.IsEnabled = value;
            UpdatePreview();
        });

        CheckBox usePathStyle = Check(FormatLabel(nameof(settings.UsePathStyle)), () => settings.UsePathStyle,
            value => settings.UsePathStyle = value);
        ComboBox storageClass = EnumCombo(typeof(AmazonS3StorageClass), settings.StorageClass,
            value => settings.StorageClass = (AmazonS3StorageClass)value);
        Button storageClassHelp = Button("?", () => URLHelpers.OpenURL("https://aws.amazon.com/s3/storage-classes/"));
        CheckBox signedPayload = Check(FormatLabel(nameof(settings.SignedPayload)), () => settings.SignedPayload,
            value => settings.SignedPayload = value);
        CheckBox useMultipartUpload = Check(FormatLabel(nameof(settings.UseMultipartUpload)), () => settings.UseMultipartUpload,
            value => settings.UseMultipartUpload = value);
        CheckBox publicAcl = Check(FormatLabel(nameof(settings.SetPublicACL)), () => settings.SetPublicACL,
            value => settings.SetPublicACL = value);
        CheckBox removeImageExtension = Check(FormatLabel("Image"), () => settings.RemoveExtensionImage, value =>
        {
            settings.RemoveExtensionImage = value;
            UpdatePreview();
        });
        CheckBox removeVideoExtension = Check(FormatLabel("Video"), () => settings.RemoveExtensionVideo, value =>
        {
            settings.RemoveExtensionVideo = value;
            UpdatePreview();
        });
        CheckBox removeTextExtension = Check(FormatLabel("Text"), () => settings.RemoveExtensionText, value =>
        {
            settings.RemoveExtensionText = value;
            UpdatePreview();
        });

        UpdatePreview();

        yield return Card(Localization.Strings.DestinationSettings_Settings,
            Row(FormatLabel(nameof(settings.AccessKeyID)) + ":", EditorWithButton(accessKey, accessKeyOpen)),
            Row(FormatLabel(nameof(settings.SecretAccessKey)) + ":", secretKey),
            Row(FormatLabel("Endpoints") + ":", endpoints),
            Row(FormatLabel(nameof(settings.Endpoint)) + ":", endpoint),
            Row(FormatLabel(nameof(settings.Region)) + ":", region),
            Row(FormatLabel(nameof(settings.Bucket)) + ":", EditorWithButton(bucket, bucketOpen)),
            Row(FormatLabel(nameof(settings.ObjectPrefix)) + ":", objectPrefix),
            useCustomDomain,
            Row(FormatLabel(nameof(settings.CustomDomain)) + ":", customDomain),
            PreviewRow(preview));

        yield return Card(FormatLabel("Advanced"),
            Row(FormatLabel(nameof(settings.StorageClass)) + ":", EditorWithButton(storageClass, storageClassHelp)),
            signedPayload,
            useMultipartUpload,
            publicAcl,
            usePathStyle,
            Row(FormatLabel("RemoveFileExtensionOn") + ":",
                HorizontalControls(removeImageExtension, removeVideoExtension, removeTextExtension)));
    }

    private Control BuildImgFishCard()
    {
        ImgFishSettings settings = _config.ImgFishSettings;
        TextBox apiKey = Text(() => settings.APIKey, value => settings.APIKey = value);
        apiKey.PasswordChar = '●';
        NumericUpDown fileIDLength = Number(settings.FileIDLength, value => settings.FileIDLength = (int)value, typeof(int));
        fileIDLength.Minimum = ImgFishSettings.MinFileIDLength;
        fileIDLength.Maximum = ImgFishSettings.MaxFileIDLength;

        return Card(Localization.Strings.DestinationSettings_Settings,
            Row(FormatLabel(nameof(settings.APIKey)) + ":", apiKey),
            Row(FormatLabel(nameof(settings.FileIDLength)) + ":", fileIDLength));
    }

    private Control BuildImgurAlbumsCard()
    {
        ObservableCollection<DestinationChoice> albums = new();
        ComboBox album = new() { ItemsSource = albums };
        album.Classes.Add("form-control");
        TextBlock status = Hint(string.Empty);

        DestinationChoice? GetSelectedChoice() => albums.FirstOrDefault(x =>
            x.Value is ImgurAlbumData candidate && candidate.id == _config.ImgurSelectedAlbum?.id);

        void LoadAlbums(IEnumerable<ImgurAlbumData>? source)
        {
            string? selectedId = _config.ImgurSelectedAlbum?.id;
            albums.Clear();
            foreach (ImgurAlbumData item in source ?? [])
            {
                albums.Add(new DestinationChoice(item, string.IsNullOrWhiteSpace(item.title) ? item.id : item.title));
            }

            DestinationChoice? selected = albums.FirstOrDefault(x => x.Value is ImgurAlbumData candidate && candidate.id == selectedId);
            album.SelectedItem = selected;
            _config.ImgurSelectedAlbum = selected?.Value as ImgurAlbumData;
            status.Text = albums.Count == 0
                ? Localization.Strings.DestinationSettings_No_albums_loaded
                : string.Format(albums.Count == 1
                    ? Localization.Strings.DestinationSettings_Album_count_loaded_singular
                    : Localization.Strings.DestinationSettings_Album_count_loaded_plural, albums.Count);
        }

        album.SelectionChanged += (_, _) => _config.ImgurSelectedAlbum = (album.SelectedItem as DestinationChoice)?.Value as ImgurAlbumData;
        Button refresh = Button(Localization.Strings.DestinationSettings_Refresh_albums, async () =>
        {
            try
            {
                if (!OAuth2Info.CheckOAuth(_config.ImgurOAuth2Info))
                {
                    status.Text = Localization.Strings.DestinationSettings_Connect_Imgur_account_first;
                    return;
                }

                _config.ImgurAlbumList = await new Imgur(_config.ImgurOAuth2Info).GetAlbumsAsync();
                LoadAlbums(_config.ImgurAlbumList);
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                status.Text = exception.Message;
            }
        });

        LoadAlbums(_config.ImgurAlbumList);
        album.SelectedItem = GetSelectedChoice();
        return Card(Localization.Strings.DestinationSettings_Albums,
            Row(Localization.Strings.DestinationSettings_Upload_to_album, album),
            Row(Localization.Strings.DestinationSettings_Status, status), ButtonRow(refresh));
    }

    private Control BuildPhotobucketAlbumsCard()
    {
        ObservableCollection<string> albums = new();
        ListBox list = new() { ItemsSource = albums, MinHeight = 100, MaxHeight = 180 };
        list.Classes.Add("settings-list");
        TextBox albumPath = Text(() => _config.PhotobucketAccountInfo?.AlbumID ?? string.Empty, value =>
        {
            if (_config.PhotobucketAccountInfo != null) _config.PhotobucketAccountInfo.AlbumID = value;
        });
        TextBlock status = Hint(string.Empty);

        void Reload()
        {
            albums.Clear();
            if (_config.PhotobucketAccountInfo == null)
            {
                status.Text = Localization.Strings.DestinationSettings_Connect_Photobucket_account_first;
                return;
            }

            ((DestinationValue<string>)albumPath.DataContext!).Value = _config.PhotobucketAccountInfo.AlbumID ?? string.Empty;
            foreach (string item in _config.PhotobucketAccountInfo.AlbumList) albums.Add(item);
            list.SelectedIndex = _config.PhotobucketAccountInfo.ActiveAlbumID.BetweenOrDefault(0, Math.Max(0, albums.Count - 1));
            status.Text = albums.Count == 0
                ? Localization.Strings.DestinationSettings_No_albums_saved
                : string.Format(albums.Count == 1
                    ? Localization.Strings.DestinationSettings_Album_count_singular
                    : Localization.Strings.DestinationSettings_Album_count_plural, albums.Count);
        }

        list.SelectionChanged += (_, _) =>
        {
            if (_config.PhotobucketAccountInfo != null && list.SelectedIndex >= 0)
            {
                _config.PhotobucketAccountInfo.ActiveAlbumID = list.SelectedIndex;
            }
        };
        Button add = Button(Localization.Strings.DestinationSettings_Add_path, () =>
        {
            PhotobucketAccountInfo? account = _config.PhotobucketAccountInfo;
            string path = albumPath.Text ?? string.Empty;
            if (account == null || string.IsNullOrWhiteSpace(path) || account.AlbumList.Contains(path)) return;
            account.AlbumList.Add(path);
            Reload();
            list.SelectedIndex = albums.Count - 1;
        });
        Button remove = Button(Localization.Strings.DestinationSettings_Remove, () =>
        {
            PhotobucketAccountInfo? account = _config.PhotobucketAccountInfo;
            if (account == null || account.AlbumList.Count <= 1 || list.SelectedIndex < 0) return;
            account.AlbumList.RemoveAt(list.SelectedIndex);
            account.ActiveAlbumID = Math.Min(account.ActiveAlbumID, account.AlbumList.Count - 1);
            Reload();
        });
        Button refresh = Button(Localization.Strings.DestinationSettings_Refresh, Reload);
        Reload();
        return Card(Localization.Strings.DestinationSettings_Albums,
            Row(Localization.Strings.DestinationSettings_Album_path, albumPath), list,
            ButtonRow(add, remove, refresh), Row(Localization.Strings.DestinationSettings_Status, status));
    }

    private Control BuildOneDriveFolderCard() => BuildRemoteFolderCard(
        OneDrive.RootFolder,
        () => _config.OneDriveV2SelectedFolder ?? OneDrive.RootFolder,
        value => _config.OneDriveV2SelectedFolder = value,
        async value => (await new OneDrive(_config.OneDriveV2OAuth2Info).GetPathInfoAsync(value.id))?.value,
        value => value.name);

    private Control BuildBoxFolderCard() => BuildRemoteFolderCard(
        Box.RootFolder,
        () => _config.BoxSelectedFolder ?? Box.RootFolder,
        value => _config.BoxSelectedFolder = value,
        async value => (await new Box(_config.BoxOAuth2Info).GetFilesAsync(value))?.entries?.Where(x => x.type == "folder"),
        value => value.name);

    private Control BuildMegaFolderTreeCard()
    {
        MegaFolderInfo selectedFolder = _config.MegaSelectedFolder ?? Mega.RootFolder;
        TextBlock selectedStatus = Hint(string.IsNullOrWhiteSpace(selectedFolder.Name)
            ? Localization.Strings.DestinationSettings_Root_folder
            : selectedFolder.Name);
        HashSet<TreeViewItem> loadedItems = [];
        HashSet<TreeViewItem> loadingItems = [];

        TreeViewItem CreateItem(MegaFolderInfo folder)
        {
            ObservableCollection<TreeViewItem> children = [new TreeViewItem { Header = "…", IsEnabled = false }];
            TreeViewItem item = new()
            {
                Header = string.IsNullOrWhiteSpace(folder.Name)
                    ? Localization.Strings.DestinationSettings_Root_folder
                    : folder.Name,
                DataContext = folder,
                ItemsSource = children
            };
            item.Expanded += async (_, _) => await LoadChildrenAsync(item, children);

            if ((!string.IsNullOrWhiteSpace(folder.ID) && folder.ID == selectedFolder.ID) ||
                (string.IsNullOrWhiteSpace(folder.ID) && string.IsNullOrWhiteSpace(selectedFolder.ID)))
            {
                item.IsSelected = true;
            }

            return item;
        }

        async Task LoadChildrenAsync(TreeViewItem item, ObservableCollection<TreeViewItem> children)
        {
            if (loadedItems.Contains(item) || !loadingItems.Add(item)) return;

            try
            {
                if (item.DataContext is not MegaFolderInfo folder) return;

                if (string.IsNullOrWhiteSpace(_config.MegaSessionID) || string.IsNullOrWhiteSpace(_config.MegaMasterKey))
                {
                    throw new InvalidOperationException(Localization.Strings.DestinationSettings_Not_connected);
                }

                children.Clear();
                Mega mega = new(_config.MegaSessionID, Mega.FromBase64URL(_config.MegaMasterKey));
                IReadOnlyList<MegaFolderInfo> folders = await mega.GetFoldersAsync(folder);
                foreach (MegaFolderInfo child in folders)
                {
                    children.Add(CreateItem(child));
                }

                loadedItems.Add(item);
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                children.Clear();
                children.Add(new TreeViewItem { Header = "…", IsEnabled = false });
                loadedItems.Remove(item);
            }
            finally
            {
                loadingItems.Remove(item);
            }
        }

        TreeViewItem rootItem = CreateItem(Mega.RootFolder);
        ObservableCollection<TreeViewItem> roots = [rootItem];
        TreeView tree = new() { ItemsSource = roots, MinHeight = 180, MaxHeight = 320 };
        tree.Classes.Add("settings-tree");
        tree.SelectionChanged += (_, _) =>
        {
            if (tree.SelectedItem is TreeViewItem { DataContext: MegaFolderInfo folder })
            {
                _config.MegaSelectedFolder = folder;
                selectedFolder = folder;
                selectedStatus.Text = string.IsNullOrWhiteSpace(folder.Name)
                    ? Localization.Strings.DestinationSettings_Root_folder
                    : folder.Name;
            }
        };

        return Card(Localization.Strings.DestinationSettings_Upload_folder,
            Row(Localization.Strings.DestinationSettings_Selected_folder, selectedStatus), tree);
    }

    private Control BuildRemoteFolderCard<T>(
        T root,
        Func<T> getSelected,
        Action<T> setSelected,
        Func<T, Task<IEnumerable<T>?>> getChildren,
        Func<T, string?> getName) where T : class
    {
        ObservableCollection<DestinationChoice> folders = new();
        ListBox list = new() { ItemsSource = folders, MinHeight = 130, MaxHeight = 220 };
        list.Classes.Add("settings-list");
        TextBlock selectedStatus = Hint(getName(getSelected()) ?? Localization.Strings.DestinationSettings_Root_folder);
        TextBlock browseStatus = Hint(Localization.Strings.DestinationSettings_Select_Refresh_to_load_folders);
        Stack<T> history = new();
        T currentFolder = root;

        async Task LoadAsync(T folder)
        {
            try
            {
                folders.Clear();
                foreach (T child in await getChildren(folder) ?? [])
                {
                    folders.Add(new DestinationChoice(child, getName(child) ?? Localization.Strings.DestinationSettings_Unnamed_folder));
                }

                browseStatus.Text = folders.Count == 0
                    ? Localization.Strings.DestinationSettings_No_child_folders
                    : string.Format(folders.Count == 1
                        ? Localization.Strings.DestinationSettings_Folder_count_singular
                        : Localization.Strings.DestinationSettings_Folder_count_plural, folders.Count);
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception);
                browseStatus.Text = exception.Message;
            }
        }

        list.SelectionChanged += (_, _) =>
        {
            if ((list.SelectedItem as DestinationChoice)?.Value is T selected)
            {
                setSelected(selected);
                selectedStatus.Text = getName(selected) ?? Localization.Strings.DestinationSettings_Unnamed_folder;
            }
        };

        Button refresh = Button(Localization.Strings.DestinationSettings_Refresh, () => LoadAsync(currentFolder));
        Button open = Button(Localization.Strings.DestinationSettings_Open_folder, async () =>
        {
            if ((list.SelectedItem as DestinationChoice)?.Value is T selected)
            {
                history.Push(currentFolder);
                currentFolder = selected;
                await LoadAsync(currentFolder);
            }
        });
        Button back = Button(Localization.Strings.DestinationSettings_Back, async () =>
        {
            if (history.Count > 0)
            {
                currentFolder = history.Pop();
                await LoadAsync(currentFolder);
            }
        });
        Button rootButton = Button(Localization.Strings.DestinationSettings_Root, async () =>
        {
            history.Clear();
            currentFolder = root;
            setSelected(root);
            selectedStatus.Text = getName(root) ?? Localization.Strings.DestinationSettings_Root_folder;
            await LoadAsync(root);
        });

        return Card(Localization.Strings.DestinationSettings_Upload_folder,
            Row(Localization.Strings.DestinationSettings_Selected_folder, selectedStatus), list,
            ButtonRow(refresh, open, back, rootButton),
            Row(Localization.Strings.DestinationSettings_Browser_status, browseStatus));
    }

    private Control EditorRow(object owner, DestinationMember member, string label, Action? valueChanged = null)
    {
        Type type = Nullable.GetUnderlyingType(member.ValueType) ?? member.ValueType;
        Control editor = CreateEditor(owner, member, type, label, valueChanged);
        string? description = member.GetAttribute<DescriptionAttribute>()?.Description;

        Control row = type == typeof(bool) ? editor : Row(label + ":", editor);
        if (!string.IsNullOrWhiteSpace(description))
        {
            ToolTip.SetTip(row, description);
        }

        return row;
    }

    private Control CreateEditor(object owner, DestinationMember member, Type type, string label, Action? valueChanged)
    {
        if (type == typeof(bool))
        {
            return Check(label, () => Convert.ToBoolean(member.GetValue(owner)), value =>
            {
                member.SetValue(owner, value);
                valueChanged?.Invoke();
            });
        }

        if (type == typeof(string))
        {
            TextBox text = Text(
                () => member.GetValue(owner)?.ToString() ?? string.Empty,
                value =>
                {
                    member.SetValue(owner, value);
                    valueChanged?.Invoke();
                });
            if (IsSecret(member))
            {
                text.PasswordChar = '●';
            }
            return text;
        }

        if (type.IsEnum)
        {
            return EnumCombo(type, member.GetValue(owner), value =>
            {
                member.SetValue(owner, value);
                valueChanged?.Invoke();
            });
        }

        if (type == typeof(int) && TryCreateSelectionEditor(owner, member, out ComboBox selectionEditor))
        {
            return selectionEditor;
        }

        if (IsNumericType(type))
        {
            decimal current = Convert.ToDecimal(member.GetValue(owner) ?? 0);
            return Number(current, value =>
            {
                member.SetValue(owner, ConvertNumeric(value, type));
                valueChanged?.Invoke();
            }, type);
        }

        if (IsStringList(type))
        {
            return Text(
                () => string.Join(", ", (IEnumerable<string>?)member.GetValue(owner) ?? []),
                value =>
                {
                    member.SetValue(owner, value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList());
                    valueChanged?.Invoke();
                });
        }

        object? nested = member.GetValue(owner);
        if (nested != null)
        {
            Expander expander = new()
            {
                Header = string.Format(Localization.Strings.DestinationSettings_Edit_item, label),
                IsExpanded = false
            };
            StackPanel panel = new() { Spacing = 4 };
            foreach (DestinationMember child in GetEditableMembers(nested.GetType()))
            {
                panel.Children.Add(EditorRow(nested, child, FormatLabel(child.Name)));
            }
            expander.Content = panel;
            return expander;
        }

        return Hint(Localization.Strings.DestinationSettings_Not_configured);
    }

    private bool TryCreateSelectionEditor(object owner, DestinationMember member, out ComboBox editor)
    {
        IList? items = null;

        if (ReferenceEquals(owner, _config) && member.Name.StartsWith("FTPSelected", StringComparison.Ordinal))
        {
            items = _config.FTPAccountList;
        }
        else if (ReferenceEquals(owner, _config) && member.Name.StartsWith("LocalhostSelected", StringComparison.Ordinal))
        {
            items = _config.LocalhostAccountList;
        }
        else if (ReferenceEquals(owner, _config.PushbulletSettings) && member.Name == nameof(_config.PushbulletSettings.SelectedDevice))
        {
            items = new ArrayList(_config.PushbulletSettings.DeviceList
                .Select(device => new DestinationChoice(device,
                    device.Name ?? Localization.Strings.DestinationSettings_Unnamed_device))
                .ToArray());
        }

        if (items == null)
        {
            editor = null!;
            return false;
        }

        int current = Convert.ToInt32(member.GetValue(owner) ?? 0);
        int selectedIndex = items.Count == 0 ? -1 : current.BetweenOrDefault(0, items.Count - 1);
        if (selectedIndex != current)
        {
            member.SetValue(owner, selectedIndex);
        }

        IEnumerable displayedItems = ReferenceEquals(items, _config.FTPAccountList) || ReferenceEquals(items, _config.LocalhostAccountList)
            ? GetObservableList(items)
            : items;
        DestinationValue<int> value = new(selectedIndex, index => member.SetValue(owner, index));
        editor = new ComboBox { ItemsSource = displayedItems, DataContext = value };
        editor.Classes.Add("form-control");
        editor.Bind(SelectingItemsControl.SelectedIndexProperty, new Binding(nameof(DestinationValue<int>.Value))
        { Source = value, Mode = BindingMode.TwoWay });
        return true;
    }

    private Control BuildListEditor(IList list, DestinationMember member, object owner)
    {
        Type itemType = member.ValueType.IsGenericType ? member.ValueType.GetGenericArguments()[0] : typeof(object);

        if (itemType == typeof(string))
        {
            return Text(
                () => string.Join(", ", list.Cast<object>()),
                value =>
                {
                    list.Clear();
                    foreach (string item in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)) list.Add(item);
                });
        }

        ObservableCollection<object> items = GetObservableList(list);
        ListBox listBox = new() { ItemsSource = items, MinHeight = 150, MaxHeight = 230 };
        listBox.Classes.Add("settings-list");
        StackPanel details = new() { Spacing = 4 };

        void ShowSelected()
        {
            details.Children.Clear();
            if (listBox.SelectedItem is not { } selected) return;

            Action? valueChanged = null;
            TextBlock? preview = null;
            if (selected is FTPAccount account)
            {
                TextBlock ftpPreview = Hint(account.PreviewHttpPath);
                preview = ftpPreview;
                valueChanged = () => ftpPreview.Text = account.PreviewHttpPath;
            }

            bool previewAdded = false;
            foreach (DestinationMember child in GetEditableMembers(selected.GetType()))
            {
                details.Children.Add(EditorRow(selected, child, FormatLabel(child.Name), valueChanged));
                if (preview != null && child.Name == nameof(FTPAccount.HttpHomePathNoExtension))
                {
                    details.Children.Add(PreviewRow(preview));
                    previewAdded = true;
                }
            }

            if (preview != null && !previewAdded)
            {
                details.Children.Add(PreviewRow(preview));
            }
        }

        listBox.SelectionChanged += (_, _) => ShowSelected();

        Button add = Button(Localization.Strings.DestinationSettings_Add, () =>
        {
            object? item = Activator.CreateInstance(itemType);
            if (item == null) return;
            list.Add(item);
            items.Add(item);
            listBox.SelectedItem = item;
        });
        Button duplicate = Button(Localization.Strings.DestinationSettings_Duplicate, () =>
        {
            if (listBox.SelectedItem is not { } selected) return;
            object? copy = CloneObject(selected);
            if (copy == null) return;
            list.Add(copy);
            items.Add(copy);
            listBox.SelectedItem = copy;
        });
        Button remove = Button(Localization.Strings.DestinationSettings_Remove, () =>
        {
            if (listBox.SelectedItem is not { } selected) return;
            list.Remove(selected);
            items.Remove(selected);
            details.Children.Clear();
        });

        StackPanel result = new() { Spacing = 6 };
        result.Children.Add(listBox);
        result.Children.Add(ButtonRow(add, duplicate, remove));
        result.Children.Add(details);
        if (items.Count > 0) listBox.SelectedIndex = 0;
        return result;
    }

    private ObservableCollection<object> GetObservableList(IList list)
    {
        if (!_listItems.TryGetValue(list, out ObservableCollection<object>? items))
        {
            items = new ObservableCollection<object>(list.Cast<object>());
            _listItems.Add(list, items);
        }

        return items;
    }

    private static object? CloneObject(object source)
    {
        MethodInfo? clone = source.GetType().GetMethod("Clone", BindingFlags.Instance | BindingFlags.Public, Type.EmptyTypes);
        if (clone != null) return clone.Invoke(source, null);

        object? result = Activator.CreateInstance(source.GetType());
        if (result == null) return null;
        foreach (DestinationMember member in GetEditableMembers(source.GetType()))
        {
            member.SetValue(result, member.GetValue(source));
        }
        return result;
    }

    private static IEnumerable<DestinationMember> GetEditableMembers(Type type)
    {
        IEnumerable<DestinationMember> properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => x.CanRead && x.CanWrite && x.GetIndexParameters().Length == 0)
            .Select(x => new DestinationMember(x));
        IEnumerable<DestinationMember> fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => !x.IsInitOnly)
            .Select(x => new DestinationMember(x));
        return properties.Concat(fields)
            .Where(x => x.GetAttribute<BrowsableAttribute>()?.Browsable != false)
            .Where(x => !IsAuthenticationMember(x))
            .Where(x => IsSimpleType(Nullable.GetUnderlyingType(x.ValueType) ?? x.ValueType) || IsStringList(x.ValueType));
    }

    private static bool IsEditableConfigProperty(PropertyInfo property) =>
        property.CanRead && property.CanWrite && property.GetIndexParameters().Length == 0 &&
        property.GetCustomAttribute<BrowsableAttribute>()?.Browsable != false;

    private static bool ShouldSkipMember(string pageId, DestinationMember member)
    {
        if (IsAuthenticationMember(member)) return true;

        return pageId switch
        {
            "imgur" => member.Name is nameof(UploadersConfig.ImgurSelectedAlbum) or nameof(UploadersConfig.ImgurAlbumList),
            "photobucket" => member.Name == nameof(UploadersConfig.PhotobucketAccountInfo),
            "pushbullet" => member.Name == nameof(UploadersConfig.PushbulletSettings),
            "puush" => member.Name == nameof(UploadersConfig.PuushAPIKey),
            "lobfile" => member.Name == nameof(UploadersConfig.LithiioSettings),
            "onedrive" => member.Name == nameof(UploadersConfig.OneDriveV2SelectedFolder),
            "box" => member.Name == nameof(UploadersConfig.BoxSelectedFolder),
            "mega" => member.Name is nameof(UploadersConfig.MegaEmail) or nameof(UploadersConfig.MegaPassword) or
                nameof(UploadersConfig.MegaSelectedFolder),
            "amazon-s3" => member.Name == nameof(UploadersConfig.AmazonS3Settings),
            "img-fish" => member.Name == nameof(UploadersConfig.ImgFishSettings),
            _ => false
        };
    }

    private static bool IsAuthenticationMember(DestinationMember member) =>
        member.Name.Contains("OAuth", StringComparison.OrdinalIgnoreCase) ||
        member.Name.Equals("Auth_token", StringComparison.OrdinalIgnoreCase) ||
        member.Name.Equals("UserKey", StringComparison.OrdinalIgnoreCase) ||
        member.ValueType == typeof(OAuthUserInfo);

    private static bool IsSecret(DestinationMember member) =>
        member.GetAttributes().Any(x => x.GetType().Name == "JsonEncryptAttribute") ||
        member.Name.Contains("Password", StringComparison.OrdinalIgnoreCase) ||
        member.Name.Contains("Secret", StringComparison.OrdinalIgnoreCase) ||
        member.Name.EndsWith("Token", StringComparison.OrdinalIgnoreCase) ||
        member.Name.EndsWith("APIKey", StringComparison.OrdinalIgnoreCase) ||
        member.Name.EndsWith("AccessKey", StringComparison.OrdinalIgnoreCase);

    private static bool IsSimpleType(Type type) => type == typeof(string) || type == typeof(bool) || type.IsEnum || IsNumericType(type);
    private static bool IsStringList(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) && type.GetGenericArguments()[0] == typeof(string);
    private static bool IsNumericType(Type type) => type == typeof(sbyte) || type == typeof(byte) || type == typeof(short) || type == typeof(ushort) ||
        type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong) ||
        type == typeof(float) || type == typeof(double) || type == typeof(decimal);

    private static object ConvertNumeric(decimal value, Type type)
    {
        if (type == typeof(sbyte)) return (sbyte)value;
        if (type == typeof(byte)) return (byte)value;
        if (type == typeof(short)) return (short)value;
        if (type == typeof(ushort)) return (ushort)value;
        if (type == typeof(int)) return (int)value;
        if (type == typeof(uint)) return (uint)value;
        if (type == typeof(long)) return (long)value;
        if (type == typeof(ulong)) return (ulong)value;
        if (type == typeof(float)) return (float)value;
        if (type == typeof(double)) return (double)value;
        return value;
    }

    private static string GetTopLevelLabel(string name, IEnumerable<string> prefixes)
    {
        if (name == nameof(UploadersConfig.LocalhostAccountList)) return Localization.Strings.DestinationSettings_Accounts;

        string? prefix = prefixes.OrderByDescending(x => x.Length).FirstOrDefault(x => name.StartsWith(x, StringComparison.OrdinalIgnoreCase));
        string remainder = prefix == null ? name : name[prefix.Length..];
        return string.IsNullOrEmpty(remainder) ? Localization.Strings.DestinationSettings_Settings : FormatLabel(remainder);
    }

    private static string FormatLabel(string value)
    {
        value = value.Replace('_', ' ');
        string result = Regex.Replace(value, "([A-Z]+)([A-Z][a-z])", "$1 $2");
        result = Regex.Replace(result, "([a-z0-9])([A-Z])", "$1 $2");
        result = result.Trim();
        string resourceName = "DestinationSettings_Field_" + Regex.Replace(result, "[^A-Za-z0-9]+", "_");
        return Localization.Strings.ResourceManager.GetString(resourceName) ?? result;
    }

    private ScrollViewer Page(string id, string title, string icon, params Control[] controls)
    {
        StackPanel content = new()
        {
            Margin = new Thickness(28, 24, 28, 32),
            MaxWidth = 780,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        Grid header = new() { ColumnDefinitions = new ColumnDefinitions("Auto,*"), ColumnSpacing = 9 };
        header.Classes.Add("page-title");
        SettingsSearch.SetIsPageTitle(header, true);
        TextBlock iconBlock = new() { Text = icon };
        iconBlock.Classes.Add("icon"); iconBlock.Classes.Add("page-title-icon");
        TextBlock titleBlock = new() { Text = title };
        titleBlock.Classes.Add("page-title-text"); Grid.SetColumn(titleBlock, 1);
        header.Children.Add(iconBlock); header.Children.Add(titleBlock); content.Children.Add(header);
        foreach (Control control in controls) content.Children.Add(control);
        ScrollViewer page = new() { Content = content, IsVisible = false };
        SettingsSearch.SetPageId(page, id);
        return page;
    }

    internal static Border Card(string title, params Control[] controls)
    {
        StackPanel panel = new() { Spacing = 4 };
        TextBlock heading = new() { Text = title }; heading.Classes.Add("section-title"); panel.Children.Add(heading);
        foreach (Control control in controls) panel.Children.Add(control);
        Border card = new() { Child = panel }; card.Classes.Add("section-card"); SettingsSearch.SetIsPanel(card, true);
        return card;
    }

    internal static Grid Row(string label, Control editor)
    {
        Grid row = new() { ColumnDefinitions = new ColumnDefinitions("210,*"), ColumnSpacing = 8 };
        row.Children.Add(new TextBlock { Text = label, FontWeight = FontWeight.Normal, VerticalAlignment = VerticalAlignment.Center });
        Grid.SetColumn(editor, 1); row.Children.Add(editor); return row;
    }

    private static Grid PreviewRow(TextBlock preview)
    {
        Grid row = Row(FormatLabel("URLPreview") + ":", preview);
        row.Classes.Add("preview-row");
        return row;
    }

    private static Grid EditorWithButton(Control editor, Button button)
    {
        Grid grid = new() { ColumnDefinitions = new ColumnDefinitions("*,Auto"), ColumnSpacing = 6 };
        grid.Children.Add(editor);
        Grid.SetColumn(button, 1);
        grid.Children.Add(button);
        return grid;
    }

    private static StackPanel HorizontalControls(params Control[] controls)
    {
        StackPanel panel = new() { Orientation = Orientation.Horizontal, Spacing = 12 };
        foreach (Control control in controls) panel.Children.Add(control);
        return panel;
    }

    internal static CheckBox Check(string text, Func<bool> getter, Action<bool> setter)
    {
        DestinationValue<bool> value = new(getter(), setter);
        CheckBox check = new() { Content = text, DataContext = value }; check.Classes.Add("setting");
        check.Bind(ToggleButton.IsCheckedProperty, new Binding(nameof(DestinationValue<bool>.Value)) { Source = value, Mode = BindingMode.TwoWay });
        return check;
    }

    internal static TextBox Text(Func<string> getter, Action<string> setter)
    {
        DestinationValue<string> value = new(getter(), setter);
        TextBox text = new() { DataContext = value }; text.Classes.Add("form-control");
        text.Bind(TextBox.TextProperty, new Binding(nameof(DestinationValue<string>.Value))
        { Source = value, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
        return text;
    }

    private static NumericUpDown Number(decimal current, Action<decimal> setter, Type type)
    {
        DestinationValue<decimal?> value = new(current, x => setter(x ?? 0));
        decimal min = type == typeof(sbyte) ? sbyte.MinValue : type == typeof(byte) ? byte.MinValue :
            type == typeof(short) ? short.MinValue : type == typeof(ushort) ? ushort.MinValue :
            type == typeof(int) ? int.MinValue : type == typeof(uint) ? uint.MinValue :
            type == typeof(long) ? long.MinValue : type == typeof(ulong) ? ulong.MinValue : -1_000_000_000;
        decimal max = type == typeof(sbyte) ? sbyte.MaxValue : type == typeof(byte) ? byte.MaxValue :
            type == typeof(short) ? short.MaxValue : type == typeof(ushort) ? ushort.MaxValue :
            type == typeof(int) ? int.MaxValue : type == typeof(uint) ? uint.MaxValue :
            type == typeof(long) ? long.MaxValue : type == typeof(ulong) ? ulong.MaxValue : 1_000_000_000;
        NumericUpDown number = new() { DataContext = value, Minimum = min, Maximum = max };
        number.Classes.Add("form-control");
        number.Bind(NumericUpDown.ValueProperty, new Binding(nameof(DestinationValue<decimal?>.Value)) { Source = value, Mode = BindingMode.TwoWay });
        return number;
    }

    private static ComboBox EnumCombo(Type enumType, object? current, Action<object> setter)
    {
        DestinationChoice[] choices = Enum.GetValues(enumType).Cast<Enum>()
            .Select(value => new DestinationChoice(value,
                value.GetLocalizedDescription(Localization.Strings.ResourceManager))).ToArray();
        DestinationChoice selected = choices.FirstOrDefault(x => Equals(x.Value, current)) ?? choices[0];
        DestinationValue<DestinationChoice> binding = new(selected, value => setter(value.Value));
        ComboBox combo = new() { ItemsSource = choices, DataContext = binding }; combo.Classes.Add("form-control");
        combo.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(nameof(DestinationValue<DestinationChoice>.Value))
        { Source = binding, Mode = BindingMode.TwoWay });
        return combo;
    }

    internal static Button Button(string text, Action action)
    {
        Button button = new() { Content = text }; button.Classes.Add("compact"); button.Click += (_, _) => action(); return button;
    }

    internal static Button Button(string text, Func<Task> action)
    {
        Button button = new() { Content = text };
        button.Classes.Add("compact");
        button.Click += async (_, _) => await action();
        return button;
    }

    internal static StackPanel ButtonRow(params Button[] buttons)
    {
        StackPanel panel = new() { Orientation = Orientation.Horizontal, Spacing = 6 };
        foreach (Button button in buttons) panel.Children.Add(button); return panel;
    }

    internal static TextBlock Hint(string text)
    {
        TextBlock hint = new() { Text = text }; hint.Classes.Add("hint"); return hint;
    }
}

internal sealed class DestinationValue<T> : INotifyPropertyChanged
{
    private T _value;
    private readonly Action<T> _setter;
    public T Value
    {
        get => _value;
        set
        {
            if (EqualityComparer<T>.Default.Equals(_value, value)) return;
            _value = value; _setter(value); PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    public DestinationValue(T value, Action<T> setter) { _value = value; _setter = setter; }
}

internal sealed record DestinationChoice(object Value, string Title)
{
    public override string ToString() => Title;
}

internal sealed class DestinationMember
{
    private readonly PropertyInfo? _property;
    private readonly FieldInfo? _field;
    public string Name => _property?.Name ?? _field!.Name;
    public Type ValueType => _property?.PropertyType ?? _field!.FieldType;
    public DestinationMember(PropertyInfo property) => _property = property;
    public DestinationMember(FieldInfo field) => _field = field;
    public object? GetValue(object owner) => _property?.GetValue(owner) ?? _field?.GetValue(owner);
    public void SetValue(object owner, object? value) { if (_property != null) _property.SetValue(owner, value); else _field!.SetValue(owner, value); }
    public T? GetAttribute<T>() where T : Attribute => _property?.GetCustomAttribute<T>() ?? _field?.GetCustomAttribute<T>();
    public IEnumerable<Attribute> GetAttributes() => _property?.GetCustomAttributes().Cast<Attribute>() ?? _field!.GetCustomAttributes().Cast<Attribute>();
}
