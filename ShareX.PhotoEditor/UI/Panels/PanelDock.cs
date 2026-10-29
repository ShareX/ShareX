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
        new() { Panels = ["color", "swatches"], Selected = "color", Height = 0.9 },
        new() { Panels = ["properties", "adjustments", "brushes"], Selected = "properties", Height = 1.3 },
        new() { Panels = ["layers", "channels", "paths"], Selected = "layers", Height = 1.6 },
        new() { Panels = ["history", "navigator", "info"], Selected = "history", Height = 1 }
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
                GridSplitter splitter = new() { Height = 4, ResizeDirection = GridResizeDirection.Rows, Background = new SolidColorBrush(Color.FromArgb(60, 0, 0, 0)) };
                Grid.SetRow(splitter, i * 2 + 1);
                _grid.Children.Add(splitter);
            }
        }
        foreach (PanelGroupLayout group in _layout)
        {
            if (group.Selected != null && _panels.TryGetValue(group.Selected, out EditorPanel? panel)) panel.Refresh();
        }
    }

    private Control BuildGroup(PanelGroupLayout group)
    {
        StackPanel tabs = new() { Orientation = Orientation.Horizontal };
        foreach (string id in group.Panels)
        {
            if (!_panels.TryGetValue(id, out EditorPanel? panel)) continue;
            bool selected = id == group.Selected;
            Button tab = new()
            {
                Content = panel.Title,
                Padding = new Thickness(8, 3),
                FontSize = 12,
                FontWeight = selected ? FontWeight.SemiBold : FontWeight.Normal,
                Background = selected ? new SolidColorBrush(Color.FromArgb(50, 255, 255, 255)) : Brushes.Transparent,
                CornerRadius = new CornerRadius(3, 3, 0, 0)
            };
            tab.Click += (_, _) =>
            {
                if (group.Selected == id) group.Collapsed = !group.Collapsed;
                else
                {
                    group.Selected = id;
                    group.Collapsed = false;
                }
                Rebuild();
            };
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
            MenuItem close = new() { Header = "Close" };
            close.Click += (_, _) => ClosePanel(id);
            MenuItem collapse = new() { Header = group.Collapsed ? "Expand Group" : "Collapse Group" };
            collapse.Click += (_, _) => { group.Collapsed = !group.Collapsed; Rebuild(); };
            menu.Items.Add(new Separator());
            menu.Items.Add(newGroup);
            menu.Items.Add(collapse);
            menu.Items.Add(close);
            tab.ContextMenu = menu;
            tabs.Children.Add(tab);
        }
        Border header = new()
        {
            Child = tabs,
            Background = new SolidColorBrush(Color.FromArgb(40, 0, 0, 0)),
            BorderBrush = new SolidColorBrush(Color.FromArgb(60, 0, 0, 0)),
            BorderThickness = new Thickness(0, 0, 0, 1)
        };
        DockPanel dock = new();
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
