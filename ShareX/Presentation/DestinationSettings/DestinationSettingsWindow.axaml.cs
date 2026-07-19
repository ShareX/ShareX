#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using ShareX.AvaloniaUI.Theming;
using ShareX.UploadersLib;
using System.Collections.Generic;
using System.Linq;

namespace ShareX;

public partial class DestinationSettingsWindow : Window
{
    private DestinationSettingsViewModel? _viewModel;
    private IReadOnlyDictionary<string, Control> _pages = new Dictionary<string, Control>();

    public DestinationSettingsWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
    }

    public DestinationSettingsWindow(UploadersConfig config) : this()
    {
        _viewModel = new DestinationSettingsViewModel();
        DataContext = _viewModel;
        _pages = new DestinationSettingsPageBuilder(config).BuildPages();

        foreach (Control page in _pages.Values)
        {
            SettingsPages.Children.Add(page);
        }

        _viewModel.SelectedPageChanged += SelectPage;
        SelectPage(_viewModel.SelectedNavigationItem?.Id);
        Opened += (_, _) => Navigation.RefreshFilter();
    }

    public void NavigateToService(IUploaderService service)
    {
        DestinationPageDefinition? definition = DestinationSettingsViewModel.Categories
            .SelectMany(x => x.Pages)
            .FirstOrDefault(x => x.MatchesService(service.ServiceName, service.ServiceIdentifier));
        _viewModel?.NavigateTo(definition?.Id);
    }

    private void SelectPage(string? pageId)
    {
        foreach ((string id, Control page) in _pages)
        {
            page.IsVisible = id == pageId;
        }
    }
}
