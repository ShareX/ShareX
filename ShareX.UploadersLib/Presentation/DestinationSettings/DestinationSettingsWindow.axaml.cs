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

using Avalonia.Controls;
using ShareX.AvaloniaUI.Theming;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX.UploadersLib;

public partial class DestinationSettingsWindow : Window
{
    private DestinationSettingsViewModel? _viewModel;
    private IReadOnlyDictionary<string, Control> _pages = new Dictionary<string, Control>();

    public DestinationSettingsWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
    }

    public DestinationSettingsWindow(UploadersConfig config, Action? openRemoteStorageBrowser = null) : this()
    {
        _viewModel = new DestinationSettingsViewModel();
        DataContext = _viewModel;
        _pages = new DestinationSettingsPageBuilder(config, openRemoteStorageBrowser).BuildPages();

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
