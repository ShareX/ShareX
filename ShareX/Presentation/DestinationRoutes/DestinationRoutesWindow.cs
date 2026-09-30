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

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using ShareX.Destinations;
using ShareX.HelpersLib;
using ShareX.Localization;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using MessageBox = ShareX.AvaloniaUI.MessageBox;
using MessageBoxButtons = ShareX.AvaloniaUI.MessageBoxButtons;
using MessageBoxResult = ShareX.AvaloniaUI.DialogResult;

namespace ShareX;

internal enum DestinationRoutesTab
{
    Routes,
    Instances
}

/// <summary>The destination settings window for routes: an Instances tab and a Routes tab with the custom file type editor.</summary>
internal sealed class DestinationRoutesWindow : Window
{
    private static DestinationRoutesWindow? _current;

    private readonly TaskSettings _settings;
    private readonly TabControl _tabs;
    private readonly StackPanel _routeRows = new() { Spacing = 6 };
    private readonly StackPanel _fileTypeRows = new() { Spacing = 6 };
    private readonly ComboBox _addRouteFileType = new() { MinWidth = 200 };
    private readonly ComboBox _addRouteInstance = new() { MinWidth = 240 };
    private readonly TextBlock _routeMessages = new() { TextWrapping = TextWrapping.Wrap, IsVisible = false };
    private readonly TextBox _newFileTypeName = new() { Width = 180 };
    private readonly TextBox _newFileTypeExtensions = new() { MinWidth = 260 };
    private readonly TextBlock _fileTypeMessage = new() { TextWrapping = TextWrapping.Wrap, IsVisible = false };
    private readonly ListBox _instanceList = new() { MinHeight = 300 };
    private readonly StackPanel _instanceDetails = new() { Spacing = 10 };
    private readonly ComboBox _addInstanceUploader = new() { HorizontalAlignment = HorizontalAlignment.Stretch };
    private bool _refreshing;

    private DestinationRoutingConfig Config => DestinationRouting.Config;

    private bool IsDefaultTable => DestinationRouting.IsDefaultTable(_settings);

    private DestinationRoutesWindow(TaskSettings settings, DestinationRoutesTab tab)
    {
        _settings = settings;
        Title = "ShareX - " + Strings.DestinationRoutesWindow_Title;
        Width = 1040;
        Height = 700;
        MinWidth = 860;
        MinHeight = 520;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        BindResource(this, BackgroundProperty, "ShareX.Brush.Background.Main");

        _tabs = new TabControl
        {
            Margin = new Thickness(14, 8, 14, 0),
            ItemsSource = new[]
            {
                new TabItem { Header = Strings.DestinationRoutesWindow_Routes, Content = BuildRoutesTab() },
                new TabItem { Header = Strings.DestinationRoutesWindow_Instances, Content = BuildInstancesTab() }
            },
            SelectedIndex = (int)tab
        };

        Button close = new() { Content = Strings.DestinationRoutesWindow_Close, MinWidth = 96, MinHeight = 32, IsCancel = true };
        close.Click += (_, _) => Close();

        DockPanel root = new();
        Control header = BuildHeader();
        DockPanel.SetDock(header, Dock.Top);
        StackPanel footer = new() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(18, 10, 18, 16) };
        footer.Children.Add(close);
        DockPanel.SetDock(footer, Dock.Bottom);
        root.Children.Add(header);
        root.Children.Add(footer);
        root.Children.Add(_tabs);
        Content = root;

        _addRouteFileType.SelectionChanged += (_, _) => RefreshAddRouteInstances();
        _instanceList.SelectionChanged += (_, _) =>
        {
            if (!_refreshing) RefreshInstanceDetails();
        };

        RefreshAll();
        Closed += OnClosed;
    }

    public static void Show(TaskSettings settings, DestinationRoutesTab tab = DestinationRoutesTab.Routes)
    {
        SettingManager.WaitUploadersConfig();

        Dispatcher.UIThread.Post(() =>
        {
            if (_current != null && ReferenceEquals(_current._settings, settings))
            {
                _current._tabs.SelectedIndex = (int)tab;
                _current.Activate();
                return;
            }

            _current?.Close();
            _current = new DestinationRoutesWindow(settings, tab);
            _current.Show();
            _current.Activate();
        });
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        if (ReferenceEquals(_current, this))
        {
            _current = null;
        }

        DestinationRouting.UpdateLegacy(_settings);
        SettingManager.SaveApplicationConfigAsync();
        SettingManager.SaveUploadersConfigAsync();
        SettingManager.SaveHotkeysConfigAsync();
        MainWindowIntegration.RefreshMenus();
    }

    private Control BuildHeader()
    {
        TextBlock icon = new() { Text = LucideIcons.route, FontSize = 20, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        icon.Classes.Add("icon");
        BindResource(icon, TextBlock.ForegroundProperty, "ShareX.Brush.Accent.Start");
        Border iconBox = new() { Width = 40, Height = 40, CornerRadius = new CornerRadius(6), Child = icon };
        BindResource(iconBox, Border.BackgroundProperty, "ShareX.Brush.Background.Panel");

        string target = IsDefaultTable
            ? Strings.DestinationRoutesWindow_DefaultTaskSettings
            : string.IsNullOrEmpty(_settings.Description) ? _settings.Job.GetLocalizedDescription() : _settings.Description;

        TextBlock title = new() { Text = Strings.DestinationRoutesWindow_Title, FontSize = 19, FontWeight = FontWeight.SemiBold };
        BindResource(title, TextBlock.ForegroundProperty, "ShareX.Brush.Accent.Start");
        TextBlock subtitle = new() { Text = string.Format(Strings.DestinationRoutesWindow_RoutesFor, target), FontSize = 12 };
        BindResource(subtitle, TextBlock.ForegroundProperty, "ShareX.Brush.Text.Secondary");

        StackPanel text = new() { VerticalAlignment = VerticalAlignment.Center };
        text.Children.Add(title);
        text.Children.Add(subtitle);

        Grid grid = new() { ColumnDefinitions = new ColumnDefinitions("Auto,*"), ColumnSpacing = 10 };
        Grid.SetColumn(text, 1);
        grid.Children.Add(iconBox);
        grid.Children.Add(text);

        Border header = new() { Padding = new Thickness(18, 12), Child = grid };
        BindResource(header, Border.BackgroundProperty, "ShareX.Brush.Background.Toolbar");
        return header;
    }

    #region Routes

    private Control BuildRoutesTab()
    {
        StackPanel content = new() { Spacing = 10, Margin = new Thickness(4, 12, 4, 12) };

        content.Children.Add(Hint(IsDefaultTable ? Strings.DestinationRoutesWindow_RoutesHint
            : Strings.DestinationRoutesWindow_RoutesHint + " " + Strings.DestinationRoutesWindow_TaskRoutesHint));

        StackPanel routes = new() { Spacing = 6 };
        routes.Children.Add(RouteHeaderRow());
        routes.Children.Add(_routeRows);
        BindResource(_routeMessages, TextBlock.ForegroundProperty, "ShareX.Brush.Status.Error");
        routes.Children.Add(_routeMessages);

        Button addRoute = CompactButton(Strings.DestinationRoutesWindow_AddRoute, LucideIcons.circle_plus);
        addRoute.Click += (_, _) => AddRoute();
        StackPanel addRow = new() { Orientation = Orientation.Horizontal, Spacing = 8 };
        addRow.Children.Add(_addRouteFileType);
        addRow.Children.Add(Arrow());
        addRow.Children.Add(_addRouteInstance);
        addRow.Children.Add(addRoute);
        routes.Children.Add(addRow);

        content.Children.Add(Card(Strings.DestinationRoutesWindow_Routes, routes));

        _newFileTypeName.PlaceholderText = Strings.DestinationRoutesWindow_Name;
        _newFileTypeExtensions.PlaceholderText = "jpg, jpeg";
        Button addFileType = CompactButton(Strings.DestinationRoutesWindow_AddFileType, LucideIcons.circle_plus);
        addFileType.Click += (_, _) => AddFileType();
        StackPanel newFileType = new() { Orientation = Orientation.Horizontal, Spacing = 8 };
        newFileType.Children.Add(_newFileTypeName);
        newFileType.Children.Add(_newFileTypeExtensions);
        newFileType.Children.Add(addFileType);
        BindResource(_fileTypeMessage, TextBlock.ForegroundProperty, "ShareX.Brush.Status.Error");

        StackPanel fileTypes = new() { Spacing = 6 };
        fileTypes.Children.Add(Hint(Strings.DestinationRoutesWindow_CustomFileTypesHint));
        fileTypes.Children.Add(_fileTypeRows);
        fileTypes.Children.Add(newFileType);
        fileTypes.Children.Add(_fileTypeMessage);
        content.Children.Add(Card(Strings.DestinationRoutesWindow_CustomFileTypes, fileTypes));

        return new ScrollViewer { Content = content, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled };
    }

    private static Grid RouteGrid() => new() { ColumnDefinitions = new ColumnDefinitions("190,*,260,120,Auto"), ColumnSpacing = 10 };

    private Control RouteHeaderRow()
    {
        Grid grid = RouteGrid();
        AddCell(grid, SecondaryText(Strings.DestinationRoutesWindow_FileType, true), 0);
        AddCell(grid, SecondaryText(Strings.DestinationRoutesWindow_Extensions, true), 1);
        AddCell(grid, SecondaryText(Strings.DestinationRoutesWindow_Instance, true), 2);
        return grid;
    }

    private void RefreshRoutes()
    {
        _routeRows.Children.Clear();
        List<(DestinationRoute Route, bool IsOverride)> routes = DestinationRouting.GetEffectiveRoutes(_settings).ToList();
        HashSet<string> defaultTypes = DestinationRouting.GetDefaultRoutes().Select(x => x.FileTypeId).ToHashSet();

        foreach ((DestinationRoute route, bool isOverride) in routes)
        {
            FileTypeDefinition? fileType = Config.FindFileType(route.FileTypeId);

            if (fileType != null)
            {
                _routeRows.Children.Add(RouteRow(fileType, route, isOverride, defaultTypes.Contains(fileType.Id)));
            }
        }

        IReadOnlyList<string> errors = DestinationRoutes.Validate(Config, routes.Select(x => x.Route).ToList(), isDefaultTable: true);
        _routeMessages.Text = errors.Count > 0 ? Strings.DestinationRoutesWindow_RoutesNeedAttention + Environment.NewLine + string.Join(Environment.NewLine, errors) : "";
        _routeMessages.IsVisible = errors.Count > 0;

        HashSet<string> routed = routes.Select(x => x.Route.FileTypeId).ToHashSet();
        _addRouteFileType.ItemsSource = Config.GetFileTypes()
            .Where(type => !routed.Contains(type.Id))
            .Select(type => new ComboBoxItem { Content = DestinationRouting.GetFileTypeName(type), Tag = type })
            .ToList();
        _addRouteFileType.SelectedIndex = _addRouteFileType.ItemCount > 0 ? 0 : -1;
        RefreshAddRouteInstances();
    }

    private Control RouteRow(FileTypeDefinition fileType, DestinationRoute route, bool isOverride, bool hasDefault)
    {
        Grid grid = RouteGrid();

        StackPanel name = new() { Orientation = Orientation.Horizontal, Spacing = 7, VerticalAlignment = VerticalAlignment.Center };
        TextBlock icon = new() { Text = DestinationRouting.GetFileTypeIcon(fileType), VerticalAlignment = VerticalAlignment.Center };
        icon.Classes.Add("icon");
        name.Children.Add(icon);
        name.Children.Add(new TextBlock { Text = DestinationRouting.GetFileTypeName(fileType), VerticalAlignment = VerticalAlignment.Center, FontWeight = FontWeight.SemiBold });
        AddCell(grid, name, 0);

        string extensions = fileType.IsOtherFiles ? Strings.DestinationRoutesWindow_AllOtherExtensions : string.Join(", ", fileType.Extensions);
        TextBlock extensionText = SecondaryText(extensions, false);
        extensionText.TextTrimming = TextTrimming.CharacterEllipsis;
        ToolTip.SetTip(extensionText, extensions);
        AddCell(grid, extensionText, 1);

        ComboBox instances = new() { HorizontalAlignment = HorizontalAlignment.Stretch };
        List<ComboBoxItem> items = Config.GetCompatibleInstances(fileType)
            .Select(instance => new ComboBoxItem { Content = InstanceLabel(instance), Tag = instance })
            .ToList();

        if (Config.FindInstance(route.InstanceId) == null)
        {
            items.Insert(0, new ComboBoxItem { Content = Strings.MainMenuBuilder_RouteMissingInstance, Tag = null });
        }

        instances.ItemsSource = items;
        instances.SelectedItem = items.FirstOrDefault(item => (item.Tag as DestinationInstance)?.Id == route.InstanceId) ?? items.FirstOrDefault();
        instances.SelectionChanged += (_, _) =>
        {
            if (!_refreshing && instances.SelectedItem is ComboBoxItem { Tag: DestinationInstance instance } && instance.Id != route.InstanceId)
            {
                DestinationRouting.SetRoute(_settings, fileType.Id, instance);
                PostRefresh();
            }
        };
        AddCell(grid, instances, 2);

        if (!IsDefaultTable)
        {
            CheckBox useDefault = new()
            {
                Content = Strings.DestinationRoutesWindow_UseDefault,
                IsChecked = !isOverride,
                // Without a default route there is nothing to inherit, so the row can only be removed.
                IsEnabled = hasDefault || isOverride,
                VerticalAlignment = VerticalAlignment.Center
            };
            useDefault.IsCheckedChanged += (_, _) =>
            {
                if (_refreshing) return;

                if (useDefault.IsChecked == true)
                {
                    DestinationRouting.RemoveRoute(_settings, fileType.Id);
                }
                else if (Config.FindInstance(route.InstanceId) is DestinationInstance current)
                {
                    DestinationRouting.SetRoute(_settings, fileType.Id, current);
                }

                PostRefresh();
            };
            AddCell(grid, useDefault, 3);
        }

        if (IsDefaultTable && !fileType.IsOtherFiles)
        {
            Button remove = IconButton(LucideIcons.trash_2, Strings.DestinationRoutesWindow_Remove);
            remove.Click += (_, _) =>
            {
                DestinationRouting.RemoveRoute(_settings, fileType.Id);
                PostRefresh();
            };
            AddCell(grid, remove, 4);
        }

        return grid;
    }

    private void RefreshAddRouteInstances()
    {
        if (_addRouteFileType.SelectedItem is ComboBoxItem { Tag: FileTypeDefinition fileType })
        {
            _addRouteInstance.ItemsSource = Config.GetCompatibleInstances(fileType)
                .Select(instance => new ComboBoxItem { Content = InstanceLabel(instance), Tag = instance })
                .ToList();
            _addRouteInstance.SelectedIndex = _addRouteInstance.ItemCount > 0 ? 0 : -1;
        }
        else
        {
            _addRouteInstance.ItemsSource = null;
        }
    }

    private void AddRoute()
    {
        if (_addRouteFileType.SelectedItem is ComboBoxItem { Tag: FileTypeDefinition fileType } &&
            _addRouteInstance.SelectedItem is ComboBoxItem { Tag: DestinationInstance instance })
        {
            DestinationRouting.SetRoute(_settings, fileType.Id, instance);
            PostRefresh();
        }
    }

    private void RefreshFileTypes()
    {
        _fileTypeRows.Children.Clear();

        foreach (FileTypeDefinition fileType in Config.CustomFileTypes.ToList())
        {
            TextBox name = new() { Text = fileType.Name, Width = 180 };
            TextBox extensions = new() { Text = string.Join(", ", fileType.Extensions), MinWidth = 260 };
            Button remove = IconButton(LucideIcons.trash_2, Strings.DestinationRoutesWindow_Remove);

            name.LostFocus += (_, _) =>
            {
                string value = name.Text?.Trim() ?? "";

                if (value.Length > 0 && value != fileType.Name)
                {
                    fileType.Name = value;
                    PostRefresh();
                }
                else
                {
                    name.Text = fileType.Name;
                }
            };

            extensions.LostFocus += (_, _) =>
            {
                List<string> values = FileTypeDefinition.NormalizeExtensions([extensions.Text ?? ""]);

                if (values.Count > 0 && !values.SequenceEqual(fileType.Extensions))
                {
                    fileType.Extensions = values;
                    PostRefresh();
                }
                else
                {
                    extensions.Text = string.Join(", ", fileType.Extensions);
                }
            };

            remove.Click += (_, _) =>
            {
                DestinationRoutes.RemoveFileType(Config, fileType.Id, GetAllRouteTables());
                PostRefresh();
            };

            StackPanel row = new() { Orientation = Orientation.Horizontal, Spacing = 8 };
            row.Children.Add(name);
            row.Children.Add(extensions);
            row.Children.Add(remove);
            _fileTypeRows.Children.Add(row);
        }
    }

    private List<DestinationRoute>?[] GetAllRouteTables()
    {
        List<List<DestinationRoute>?> tables = [ApplicationState.DefaultTaskSettings.DestinationRoutes, _settings.DestinationRoutes];

        if (ApplicationState.HotkeysConfigOrNull?.Hotkeys != null)
        {
            tables.AddRange(ApplicationState.HotkeysConfig.Hotkeys.Select(hotkey => hotkey.TaskSettings?.DestinationRoutes));
        }

        return tables.Distinct().ToArray();
    }

    private void AddFileType()
    {
        try
        {
            Config.AddCustomFileType(_newFileTypeName.Text ?? "", [_newFileTypeExtensions.Text ?? ""]);
            _newFileTypeName.Text = "";
            _newFileTypeExtensions.Text = "";
            _fileTypeMessage.IsVisible = false;
            PostRefresh();
        }
        catch (ArgumentException)
        {
            _fileTypeMessage.Text = Strings.DestinationRoutesWindow_FileTypeInvalid;
            _fileTypeMessage.IsVisible = true;
        }
    }

    #endregion Routes

    #region Instances

    private Control BuildInstancesTab()
    {
        Button add = CompactButton(Strings.DestinationRoutesWindow_Add, LucideIcons.circle_plus);
        add.Click += (_, _) => AddInstance();

        _addInstanceUploader.ItemsSource = DestinationCatalog.GetUploaders()
            .Select(x => new ComboBoxItem
            {
                Content = $"{DestinationCatalog.GetUploaderName(x.Category, x.Uploader)} ({GetCategoryName(x.Category)})",
                Tag = x
            })
            .OrderBy(x => x.Content as string, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
        _addInstanceUploader.SelectedIndex = 0;

        Grid addRow = new() { ColumnDefinitions = new ColumnDefinitions("*,Auto"), ColumnSpacing = 8 };
        Grid.SetColumn(add, 1);
        addRow.Children.Add(_addInstanceUploader);
        addRow.Children.Add(add);

        DockPanel left = new() { LastChildFill = true };
        DockPanel.SetDock(addRow, Dock.Bottom);
        addRow.Margin = new Thickness(0, 8, 0, 0);
        left.Children.Add(addRow);
        left.Children.Add(_instanceList);

        Grid grid = new() { ColumnDefinitions = new ColumnDefinitions("340,*"), ColumnSpacing = 14, Margin = new Thickness(4, 12, 4, 12) };
        ScrollViewer details = new() { Content = _instanceDetails, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled };
        Grid.SetColumn(details, 1);
        grid.Children.Add(left);
        grid.Children.Add(details);
        return grid;
    }

    private void RefreshInstances(DestinationInstance? select = null)
    {
        DestinationInstance? selected = select ?? (_instanceList.SelectedItem as ListBoxItem)?.Tag as DestinationInstance;
        _refreshing = true;

        try
        {
            List<ListBoxItem> items = Config.Instances.Select(instance =>
            {
                StackPanel content = new() { Spacing = 1 };
                content.Children.Add(new TextBlock { Text = instance.Name, FontWeight = FontWeight.SemiBold });
                content.Children.Add(SecondaryText($"{DestinationCatalog.GetUploaderName(instance.Category, instance.Uploader)} · {GetSettingsSummary(instance)}", false));
                return new ListBoxItem { Content = content, Tag = instance };
            }).ToList();

            _instanceList.ItemsSource = items;
            _instanceList.SelectedItem = items.FirstOrDefault(item => item.Tag == selected) ?? items.FirstOrDefault();
        }
        finally
        {
            _refreshing = false;
        }

        RefreshInstanceDetails();
    }

    private DestinationInstance? SelectedInstance => (_instanceList.SelectedItem as ListBoxItem)?.Tag as DestinationInstance;

    private void RefreshInstanceDetails()
    {
        _instanceDetails.Children.Clear();
        DestinationInstance? instance = SelectedInstance;

        if (instance == null)
        {
            return;
        }

        TextBox name = new() { Text = instance.Name, MinWidth = 280 };
        Button rename = CompactButton(Strings.DestinationRoutesWindow_Rename, LucideIcons.pencil);
        rename.Click += (_, _) =>
        {
            if (!string.IsNullOrWhiteSpace(name.Text))
            {
                Config.Rename(instance, name.Text);
                PostRefresh(instance);
            }
        };
        StackPanel nameRow = new() { Orientation = Orientation.Horizontal, Spacing = 8 };
        nameRow.Children.Add(name);
        nameRow.Children.Add(rename);

        StackPanel general = new() { Spacing = 6 };
        general.Children.Add(nameRow);
        general.Children.Add(SecondaryText(string.Format(Strings.DestinationRoutesWindow_UploaderInfo,
            DestinationCatalog.GetUploaderName(instance.Category, instance.Uploader), GetCategoryName(instance.Category)), false));
        general.Children.Add(Hint(GetSettingsDescription(instance)));

        List<string> usedBy = GetRoutesUsing(instance);
        if (usedBy.Count > 0)
        {
            general.Children.Add(Hint(string.Format(Strings.DestinationRoutesWindow_UsedByRoutes, string.Join(", ", usedBy))));
        }

        Button duplicate = CompactButton(Strings.DestinationRoutesWindow_Duplicate, LucideIcons.copy_plus);
        duplicate.Click += (_, _) => PostRefresh(DestinationMigration.Duplicate(ApplicationState.UploadersConfig, instance));

        Button edit = CompactButton(Strings.DestinationRoutesWindow_EditSettings, LucideIcons.cloud_cog);
        edit.Click += (_, _) => EditSettings(instance);

        Button remove = CompactButton(Strings.DestinationRoutesWindow_Remove, LucideIcons.trash_2);
        remove.IsEnabled = Config.CanRemove(instance);
        remove.Click += (_, _) => RemoveInstance(instance);

        StackPanel actions = new() { Orientation = Orientation.Horizontal, Spacing = 8 };
        actions.Children.Add(duplicate);
        actions.Children.Add(edit);
        actions.Children.Add(remove);
        general.Children.Add(actions);

        if (instance.IsDefault)
        {
            general.Children.Add(Hint(Strings.DestinationRoutesWindow_DefaultInstanceHint));
        }

        _instanceDetails.Children.Add(Card(instance.Name, general));
        _instanceDetails.Children.Add(Card(Strings.DestinationRoutesWindow_AcceptedFileTypes, BuildAcceptedFileTypes(instance)));
    }

    private Control BuildAcceptedFileTypes(DestinationInstance instance)
    {
        StackPanel panel = new() { Spacing = 4 };
        panel.Children.Add(Hint(Strings.DestinationRoutesWindow_AcceptedFileTypesHint));

        IReadOnlyList<string> accepted = instance.GetAcceptedFileTypes();
        bool acceptsAll = accepted.Contains(PremadeFileTypes.AllFiles);
        List<CheckBox> typeChecks = new();

        CheckBox all = new() { Content = Strings.DestinationRoutesWindow_AllFiles, IsChecked = acceptsAll };
        panel.Children.Add(all);

        WrapPanel types = new() { Orientation = Orientation.Horizontal, Margin = new Thickness(20, 0, 0, 0) };

        foreach (FileTypeDefinition fileType in Config.GetFileTypes().Where(type => !type.IsOtherFiles))
        {
            CheckBox check = new()
            {
                Content = DestinationRouting.GetFileTypeName(fileType),
                IsChecked = acceptsAll || accepted.Contains(fileType.Id),
                IsEnabled = !acceptsAll,
                Tag = fileType.Id,
                Margin = new Thickness(0, 0, 14, 0)
            };
            check.IsCheckedChanged += (_, _) => Save();
            typeChecks.Add(check);
            types.Children.Add(check);
        }

        all.IsCheckedChanged += (_, _) =>
        {
            foreach (CheckBox check in typeChecks)
            {
                check.IsEnabled = all.IsChecked != true;
            }

            Save();
        };

        panel.Children.Add(types);
        return panel;

        void Save()
        {
            List<string> values = all.IsChecked == true
                ? [PremadeFileTypes.AllFiles]
                : typeChecks.Where(check => check.IsChecked == true).Select(check => (string)check.Tag!).ToList();

            instance.AcceptedFileTypes = values.SequenceEqual(DestinationInstance.GetDefaultAcceptedFileTypes(instance.Category)) ? null : values;
            RefreshRoutes();
        }
    }

    private void AddInstance()
    {
        if (_addInstanceUploader.SelectedItem is ComboBoxItem { Tag: ValueTuple<UploaderCategory, string> uploader })
        {
            PostRefresh(DestinationMigration.AddInstance(ApplicationState.UploadersConfig, uploader.Item1, uploader.Item2));
        }
    }

    private void RemoveInstance(DestinationInstance instance)
    {
        List<string> usedBy = GetRoutesUsing(instance);

        if (usedBy.Count > 0 && MessageBox.Show(string.Format(Strings.DestinationRoutesWindow_RemoveInUse, instance.Name, string.Join(", ", usedBy)),
            "ShareX - " + Strings.DestinationRoutesWindow_Title, MessageBoxButtons.YesNo) != MessageBoxResult.Yes)
        {
            return;
        }

        if (Config.Remove(instance))
        {
            PostRefresh();
        }
    }

    private void EditSettings(DestinationInstance instance)
    {
        IGenericUploaderService? service = DestinationCatalog.GetService(instance);

        if (DestinationCatalog.IsCustomUploader(instance))
        {
            TaskHelpers.OpenCustomUploaderSettingsWindow();
        }
        else if (!instance.HasOwnSettings || !DestinationCatalog.HasSettings(instance.Uploader))
        {
            // Default instances, FTP accounts and uploaders without settings edit the shared settings.
            TaskHelpers.OpenUploadersConfigWindow(service);
        }
        else
        {
            // A duplicate edits a copy of the settings, which is written back to the instance when the window closes.
            UploadersConfig copy = DestinationCatalog.CreateConfig(ApplicationState.UploadersConfig, instance);
            DestinationSettingsWindow window = new(copy, TaskHelpers.OpenRemoteStorageBrowser)
            {
                Title = $"ShareX - {instance.Name}"
            };

            if (service != null)
            {
                window.NavigateToService(service);
            }

            window.Closed += (_, _) =>
            {
                DestinationCatalog.CaptureSettings(copy, instance);
                SettingManager.SaveUploadersConfigAsync();
                PostRefresh(instance);
            };
            window.Show(this);
        }
    }

    private List<string> GetRoutesUsing(DestinationInstance instance)
    {
        List<string> names = new();

        foreach ((DestinationRoute route, _) in DestinationRouting.GetEffectiveRoutes(_settings))
        {
            if (route.InstanceId == instance.Id && Config.FindFileType(route.FileTypeId) is FileTypeDefinition fileType)
            {
                names.Add(DestinationRouting.GetFileTypeName(fileType));
            }
        }

        return names;
    }

    private string GetSettingsSummary(DestinationInstance instance)
    {
        if (instance.AccountIndex != null) return Strings.DestinationRoutesWindow_Account;
        return instance.HasOwnSettings ? Strings.DestinationRoutesWindow_OwnSettingsShort : Strings.DestinationRoutesWindow_SharedSettingsShort;
    }

    private string GetSettingsDescription(DestinationInstance instance)
    {
        string uploader = DestinationCatalog.GetUploaderName(instance.Category, instance.Uploader);

        if (instance.AccountIndex is int index)
        {
            return string.Format(Strings.DestinationRoutesWindow_AccountSettings, index + 1, uploader);
        }

        return string.Format(instance.HasOwnSettings ? Strings.DestinationRoutesWindow_OwnSettings : Strings.DestinationRoutesWindow_SharedSettings, uploader);
    }

    private static string GetCategoryName(UploaderCategory category) => category switch
    {
        UploaderCategory.Image => Strings.DestinationRoutesWindow_CategoryImage,
        UploaderCategory.Text => Strings.DestinationRoutesWindow_CategoryText,
        _ => Strings.DestinationRoutesWindow_CategoryFile
    };

    #endregion Instances

    private void RefreshAll(DestinationInstance? select = null)
    {
        _refreshing = true;

        try
        {
            RefreshRoutes();
            RefreshFileTypes();
        }
        finally
        {
            _refreshing = false;
        }

        RefreshInstances(select);
    }

    /// <summary>Rebuilds after the event that triggered the change has finished, so controls are not replaced while they raise events.</summary>
    private void PostRefresh(DestinationInstance? select = null) => Dispatcher.UIThread.Post(() => RefreshAll(select));

    private static string InstanceLabel(DestinationInstance instance) => instance.Name;

    private static void AddCell(Grid grid, Control control, int column)
    {
        Grid.SetColumn(control, column);
        grid.Children.Add(control);
    }

    private static TextBlock Arrow()
    {
        TextBlock arrow = new() { Text = LucideIcons.arrow_right, VerticalAlignment = VerticalAlignment.Center };
        arrow.Classes.Add("icon");
        return arrow;
    }

    private static TextBlock Hint(string text)
    {
        TextBlock hint = new() { Text = text, FontSize = 12, TextWrapping = TextWrapping.Wrap };
        BindResource(hint, TextBlock.ForegroundProperty, "ShareX.Brush.Text.Secondary");
        return hint;
    }

    private static TextBlock SecondaryText(string text, bool bold)
    {
        TextBlock block = new() { Text = text, FontSize = 12, VerticalAlignment = VerticalAlignment.Center, FontWeight = bold ? FontWeight.SemiBold : FontWeight.Normal };
        BindResource(block, TextBlock.ForegroundProperty, "ShareX.Brush.Text.Secondary");
        return block;
    }

    private static Border Card(string title, Control content)
    {
        StackPanel panel = new() { Spacing = 8 };
        panel.Children.Add(new TextBlock { Text = title, FontSize = 15, FontWeight = FontWeight.SemiBold });
        panel.Children.Add(content);

        Border card = new() { Child = panel, CornerRadius = new CornerRadius(6), Padding = new Thickness(12), BorderThickness = new Thickness(1) };
        BindResource(card, Border.BackgroundProperty, "ShareX.Brush.Background.Panel");
        BindResource(card, Border.BorderBrushProperty, "ShareX.Brush.Border");
        return card;
    }

    private static Button CompactButton(string text, string icon)
    {
        TextBlock iconBlock = new() { Text = icon, VerticalAlignment = VerticalAlignment.Center };
        iconBlock.Classes.Add("icon");
        StackPanel content = new() { Orientation = Orientation.Horizontal, Spacing = 6 };
        content.Children.Add(iconBlock);
        content.Children.Add(new TextBlock { Text = text, VerticalAlignment = VerticalAlignment.Center });
        return new Button { Content = content, MinHeight = 32, Padding = new Thickness(10, 4) };
    }

    private static Button IconButton(string icon, string toolTip)
    {
        TextBlock iconBlock = new() { Text = icon };
        iconBlock.Classes.Add("icon");
        Button button = new() { Content = iconBlock, MinHeight = 32, Padding = new Thickness(8, 4) };
        ToolTip.SetTip(button, toolTip);
        return button;
    }

    private static void BindResource(Control control, AvaloniaProperty property, string key) =>
        control.Bind(property, control.GetResourceObservable(key));
}
