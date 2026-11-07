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
using Avalonia.Layout;
using ShareX.AvaloniaUI.Theming;
using System.Collections.Generic;
using System.Linq;

namespace ShareX;

/// <summary>
/// A non-interactive copy of part of the main window sidebar, one of its menus or the tools menu (picked by
/// <see cref="Area"/>), drawn with the same style classes as the real ones so spacing changes show up as they are made.
/// </summary>
internal sealed class SpacingPreview : UserControl
{
    private const int SidebarSectionCount = 6;
    private const int MenuEntryCount = 7;

    public static readonly StyledProperty<SpacingArea> AreaProperty =
        AvaloniaProperty.Register<SpacingPreview, SpacingArea>(nameof(Area));

    private readonly Dictionary<SpacingArea, Control> _parts = new();

    public SpacingArea Area
    {
        get => GetValue(AreaProperty);
        set => SetValue(AreaProperty, value);
    }

    public SpacingPreview()
    {
        IsHitTestVisible = false;
        Focusable = false;

        MainMenuBuilder builder = new();
        IReadOnlyList<MainNavigationSection> sections = builder.BuildNavigation().Where(x => x.IsVisible).ToArray();

        // All parts share one cell and only the selected one is visible, so the preview is as tall as that part.
        Panel layout = new()
        {
            HorizontalAlignment = HorizontalAlignment.Left
        };

        _parts[SpacingArea.Sidebar] = CreateSidebar(sections);

        MainNavigationSection? menuSection = sections.FirstOrDefault(x => x.CreateChildren != null && x.CreateCategories == null);
        if (menuSection?.CreateChildren != null)
        {
            _parts[SpacingArea.Menus] = CreateMenu(menuSection.CreateChildren());
        }

        MainNavigationSection? toolSection = sections.FirstOrDefault(x => x.CreateCategories != null);
        if (toolSection?.CreateCategories != null)
        {
            _parts[SpacingArea.ToolMenus] = CreateToolMenu(toolSection.CreateCategories());
        }

        foreach (Control part in _parts.Values)
        {
            layout.Children.Add(part);
        }

        Content = layout;
        UpdateVisiblePart();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == AreaProperty)
        {
            UpdateVisiblePart();
        }
    }

    private void UpdateVisiblePart()
    {
        foreach ((SpacingArea area, Control part) in _parts)
        {
            part.IsVisible = area == Area;
        }
    }

    private static Control CreateSidebar(IReadOnlyList<MainNavigationSection> sections)
    {
        StackPanel panel = new()
        {
            Spacing = 0,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        panel.Bind(MarginProperty, panel.GetResourceObservable("ShareX.Density.Nav.PanelMargin"));

        int index = 0;
        foreach (MainNavigationSection section in sections.Take(SidebarSectionCount))
        {
            // Same separator position as the real sidebar.
            if (index == 4)
            {
                panel.Children.Add(new Separator { Margin = new Thickness(5, 2) });
            }

            panel.Children.Add(new Button
            {
                Classes = { "nav-button" },
                Content = MainWindow.CreateNavigationContent(section),
                Focusable = false
            });
            index++;
        }

        Border sidebar = new()
        {
            Child = panel,
            BorderThickness = new Thickness(0, 0, 1, 0),
            VerticalAlignment = VerticalAlignment.Top
        };
        sidebar.Bind(MinWidthProperty, sidebar.GetResourceObservable("ShareX.Density.Nav.MinWidth"));
        sidebar.Bind(Border.BackgroundProperty, sidebar.GetResourceObservable("ShareX.Brush.Background.Toolbar"));
        sidebar.Bind(Border.BorderBrushProperty, sidebar.GetResourceObservable("ShareX.Brush.Border"));
        return sidebar;
    }

    private static Control CreateMenu(IReadOnlyList<MainMenuEntry> entries)
    {
        ContextMenu menu = new()
        {
            VerticalAlignment = VerticalAlignment.Top,
            Focusable = false
        };

        foreach (MainMenuEntry entry in entries.Where(x => x.IsVisible).Take(MenuEntryCount))
        {
            if (entry.IsSeparator)
            {
                menu.Items.Add(new Separator { Classes = { "compact-menu-separator" } });
                continue;
            }

            MenuItem item = new()
            {
                Header = entry.Header,
                InputGesture = entry.InputGesture,
                Classes = { "compact-menu-item" },
                Focusable = false
            };

            if (!string.IsNullOrEmpty(entry.Icon))
            {
                item.Icon = MainWindow.CreateAccentMenuIcon(entry.Icon, 16);
            }

            // An empty placeholder child draws the submenu arrow without building the real submenu.
            if (entry.CreateChildren != null || entry.CreateCategories != null)
            {
                item.Items.Add(new MenuItem());
            }

            menu.Items.Add(item);
        }

        return menu;
    }

    private static Control CreateToolMenu(IReadOnlyList<MainMenuCategory> categories)
    {
        // The first category plus the start of the second one shows headers, buttons and the separator between them.
        List<MainMenuCategory> preview = categories.Take(1).ToList();
        if (categories.Count > 1)
        {
            preview.Add(new MainMenuCategory(categories[1].Header, categories[1].Entries.Take(3).ToArray()));
        }

        StackPanel content = new()
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        MainWindow.PopulateGridMenuCategories(content, preview, isEditing: false, execute: _ => { });

        FlyoutPresenter presenter = new()
        {
            Content = content,
            HorizontalAlignment = HorizontalAlignment.Left,
            Focusable = false
        };
        presenter.Classes.Add("tool-grid-flyout");
        return presenter;
    }
}
