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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace ShareX.PhotoEditor.UI.Panels;

/// <summary>
/// Right-hand dock made of stacked panel groups. Each group has tabs, can collapse, and is resizable with splitters.
/// Panels can be moved between groups or closed through the tab context menu and the Window menu.
/// </summary>
public sealed class PanelDock : UserControl
{
    private readonly PhotoEditorWindow _editor;
    private readonly Dictionary<string, EditorPanel> _panels;
    private List<PanelGroupLayout> _layout = [];
    private readonly Grid _grid = new();

    public static List<PanelGroupLayout> DefaultLayout() =>
    [
        new() { Panels = ["color", "swatches"], Selected = "color", Height = 1.05 },
        new() { Panels = ["properties", "adjustments", "brushes"], Selected = "properties", Height = 1.25 },
        new() { Panels = ["layers", "channels", "paths"], Selected = "layers", Height = 1.6 },
        new() { Panels = ["history", "navigator", "info"], Selected = "history", Height = 0.9 }
    ];

    public PanelDock(PhotoEditorWindow editor, IEnumerable<EditorPanel> panels)
    {
        _editor = editor;
        _panels = panels.ToDictionary(p => p.Id);
        Content = _grid;
        Bind(BackgroundProperty, this.GetResourceObservable("ShareX.Brush.Background.Panel"));
        List<PanelGroupLayout> saved = editor.Settings.Panels;
        _layout = saved.Count > 0 && saved.SelectMany(g => g.Panels).All(_panels.ContainsKey) ? saved : DefaultLayout();
        Rebuild();
    }

    public IEnumerable<EditorPanel> Panels => _panels.Values;

    public bool IsPanelVisible(string id) => _layout.Any(g => g.Panels.Contains(id));

    public EditorPanel? Get(string id) => _panels.GetValueOrDefault(id);

    public void ResetLayout()
    {
        _layout = DefaultLayout();
        Rebuild();
    }

    /// <summary>Shows a panel (adding it to the last group if hidden) and selects its tab.</summary>
    public void ShowPanel(string id)
    {
        PanelGroupLayout? group = _layout.FirstOrDefault(g => g.Panels.Contains(id));
        if (group == null)
        {
            if (_layout.Count == 0) _layout.Add(new PanelGroupLayout());
            group = _layout[^1];
            group.Panels.Add(id);
        }
        group.Selected = id;
        group.Collapsed = false;
        Rebuild();
    }

    public void TogglePanel(string id)
    {
        PanelGroupLayout? group = _layout.FirstOrDefault(g => g.Panels.Contains(id));
        if (group == null)
        {
            ShowPanel(id);
            return;
        }
        if (group.Selected == id && !group.Collapsed)
        {
            ClosePanel(id);
            return;
        }
        ShowPanel(id);
    }

    private void ClosePanel(string id)
    {
        foreach (PanelGroupLayout group in _layout)
        {
            group.Panels.Remove(id);
            if (group.Selected == id) group.Selected = group.Panels.FirstOrDefault();
        }
        _layout.RemoveAll(g => g.Panels.Count == 0);
        Rebuild();
    }

    private void MovePanel(string id, PanelGroupLayout? target)
    {
        foreach (PanelGroupLayout group in _layout)
        {
            group.Panels.Remove(id);
            if (group.Selected == id) group.Selected = group.Panels.FirstOrDefault();
        }
        if (target == null)
        {
            target = new PanelGroupLayout();
            _layout.Add(target);
        }
        target.Panels.Add(id);
        target.Selected = id;
        _layout.RemoveAll(g => g.Panels.Count == 0);
        Rebuild();
    }

    public void SaveLayout()
    {
        // Capture the current splitter heights.
        int row = 0;
        foreach (PanelGroupLayout group in _layout)
        {
            if (row < _grid.RowDefinitions.Count && _grid.RowDefinitions[row].Height.IsStar) group.Height = _grid.RowDefinitions[row].ActualHeight / Math.Max(1, Bounds.Height) * _layout.Count;
            row += 2;
        }
        _editor.Settings.Panels = _layout;
    }

    public void Rebuild()
    {
        foreach (EditorPanel panel in _panels.Values)
        {
            if (panel.Parent is Panel parent) parent.Children.Remove(panel);
            else if (panel.Parent is ContentControl content) content.Content = null;
            else if (panel.Parent is Decorator decorator) decorator.Child = null;
        }
        _grid.Children.Clear();
        _grid.RowDefinitions.Clear();
        for (int i = 0; i < _layout.Count; i++)
        {
            PanelGroupLayout group = _layout[i];
            group.Selected ??= group.Panels.FirstOrDefault();
            _grid.RowDefinitions.Add(group.Collapsed ? new RowDefinition(GridLength.Auto) : new RowDefinition(new GridLength(Math.Max(0.2, group.Height), GridUnitType.Star)) { MinHeight = 60 });
            Control groupControl = BuildGroup(group);
            Grid.SetRow(groupControl, i * 2);
            _grid.Children.Add(groupControl);
            if (i < _layout.Count - 1)
            {
                _grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                GridSplitter splitter = new() { Height = 2, ResizeDirection = GridResizeDirection.Rows, Background = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E)) };
                Grid.SetRow(splitter, i * 2 + 1);
                _grid.Children.Add(splitter);
            }
        }
        foreach (PanelGroupLayout group in _layout)
        {
            if (group.Selected != null && _panels.TryGetValue(group.Selected, out EditorPanel? panel)) panel.Refresh();
        }
    }

    private ContextMenu CreatePanelMenu(PanelGroupLayout group, string id)
    {
        ContextMenu menu = new();
        foreach (PanelGroupLayout other in _layout.Where(g => g != group))
        {
            MenuItem move = new() { Header = $"Move to Group with {string.Join(", ", other.Panels.Select(p => _panels.GetValueOrDefault(p)?.Title))}" };
            PanelGroupLayout target = other;
            move.Click += (_, _) => MovePanel(id, target);
            menu.Items.Add(move);
        }
        MenuItem newGroup = new() { Header = "Move to New Group" };
        newGroup.Click += (_, _) => MovePanel(id, null);
        MenuItem collapse = new() { Header = group.Collapsed ? "Expand Panel Group" : "Collapse Panel Group" };
        collapse.Click += (_, _) => { group.Collapsed = !group.Collapsed; Rebuild(); };
        MenuItem close = new() { Header = "Close" };
        close.Click += (_, _) => ClosePanel(id);
        MenuItem closeGroup = new() { Header = "Close Tab Group" };
        closeGroup.Click += (_, _) => { foreach (string p in group.Panels.ToList()) ClosePanel(p); };
        if (menu.Items.Count > 0) menu.Items.Add(new Separator());
        menu.Items.Add(newGroup);
        menu.Items.Add(collapse);
        menu.Items.Add(new Separator());
        menu.Items.Add(close);
        menu.Items.Add(closeGroup);
        return menu;
    }

    private Control BuildGroup(PanelGroupLayout group)
    {
        IBrush strip = new SolidColorBrush(Color.FromRgb(0x28, 0x28, 0x28));
        IBrush chrome = new SolidColorBrush(Color.FromRgb(0x32, 0x32, 0x32));
        StackPanel tabs = new() { Orientation = Orientation.Horizontal };
        foreach (string id in group.Panels)
        {
            if (!_panels.TryGetValue(id, out EditorPanel? panel)) continue;
            bool selected = id == group.Selected && !group.Collapsed;
            // Tabs: the active tab blends into the panel body, inactive tabs are dim text on the strip.
            Border tab = new()
            {
                Padding = new Thickness(10, 0),
                Height = 26,
                Background = selected ? chrome : Brushes.Transparent,
                Cursor = new Cursor(StandardCursorType.Hand),
                Child = new TextBlock
                {
                    Text = panel.Title,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = selected ? Brushes.White : new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E)),
                    FontWeight = selected ? FontWeight.SemiBold : FontWeight.Normal
                }
            };
            tab.PointerEntered += (_, _) => { if (!selected) tab.Background = new SolidColorBrush(Color.FromRgb(0x2E, 0x2E, 0x2E)); };
            tab.PointerExited += (_, _) => { if (!selected) tab.Background = Brushes.Transparent; };
            tab.PointerPressed += (_, e) =>
            {
                if (e.GetCurrentPoint(tab).Properties.IsRightButtonPressed) return;
                if (e.ClickCount >= 2 || group.Selected == id && !group.Collapsed) group.Collapsed = !group.Collapsed;
                else
                {
                    group.Selected = id;
                    group.Collapsed = false;
                }
                Rebuild();
            };
            tab.ContextMenu = CreatePanelMenu(group, id);
            tabs.Children.Add(tab);
        }
        Button menuButton = new()
        {
            Classes = { "icon" },
            Content = new TextBlock { Text = "≡", FontSize = 15, Margin = new Thickness(0, -3, 0, 0), Foreground = new SolidColorBrush(Color.FromRgb(0xB0, 0xB0, 0xB0)) },
            Padding = new Thickness(6, 0),
            Height = 22,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 4, 0)
        };
        ToolTip.SetTip(menuButton, "Panel menu");
        if (group.Selected != null) menuButton.Click += (_, _) => CreatePanelMenu(group, group.Selected).Open(menuButton);
        DockPanel headerContent = new();
        DockPanel.SetDock(menuButton, Dock.Right);
        headerContent.Children.Add(menuButton);
        headerContent.Children.Add(tabs);
        Border header = new() { Child = headerContent, Background = strip, Height = 26 };
        DockPanel dock = new() { Background = chrome };
        DockPanel.SetDock(header, Dock.Top);
        dock.Children.Add(header);
        if (!group.Collapsed && group.Selected != null && _panels.TryGetValue(group.Selected, out EditorPanel? content))
        {
            dock.Children.Add(content);
        }
        return dock;
    }

    public void RefreshVisible(EditorPanel? except = null)
    {
        foreach (PanelGroupLayout group in _layout)
        {
            if (group.Collapsed || group.Selected == null) continue;
            if (_panels.TryGetValue(group.Selected, out EditorPanel? panel) && panel != except) panel.Refresh();
        }
    }
}
