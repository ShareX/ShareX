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
using ShareX.PhotoEditor.Tools;

namespace ShareX.PhotoEditor.UI.Dialogs;

public static class ToolbarCustomizeDialog
{
    /// <summary>Lets the user reorder tools, regroup them and hide unused ones.</summary>
    public static async Task ShowAsync(PhotoEditorWindow owner)
    {
        List<List<ToolId>> groups = owner.Tools.Groups.Select(g => g.Tools.Select(t => t.Id).ToList()).ToList();
        List<ToolId> hidden = owner.Settings.HiddenTools.Select(n => Enum.TryParse(n, out ToolId id) ? id : (ToolId?)null).Where(i => i != null).Select(i => i!.Value).ToList();
        StackPanel toolbar = new() { Spacing = 4 };
        WrapPanel extra = new();
        (int Group, int Index)? selected = null;

        void Build()
        {
            toolbar.Children.Clear();
            extra.Children.Clear();
            for (int g = 0; g < groups.Count; g++)
            {
                WrapPanel row = new();
                for (int i = 0; i < groups[g].Count; i++)
                {
                    int gi = g, ii = i;
                    ToolBase tool = owner.Tools.Get(groups[g][i]);
                    bool isSelected = selected == (gi, ii);
                    Button b = new()
                    {
                        Content = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4, Children = { ToolIcons.Create(tool.Id, 16), new TextBlock { Text = tool.Name.Replace(" Tool", string.Empty), FontSize = 11 } } },
                        Margin = new Thickness(1),
                        Background = isSelected ? new SolidColorBrush(Color.FromRgb(0x4B, 0x4B, 0x4B)) : Brushes.Transparent
                    };
                    b.Click += (_, _) => { selected = (gi, ii); Build(); };
                    row.Children.Add(b);
                }
                toolbar.Children.Add(new Border { BorderBrush = Brushes.Gray, BorderThickness = new Thickness(0, 0, 0, 1), Padding = new Thickness(2), Child = row });
            }
            foreach (ToolId id in hidden)
            {
                ToolId captured = id;
                Button b = new() { Content = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4, Children = { ToolIcons.Create(id, 16), new TextBlock { Text = owner.Tools.Get(id).Name, FontSize = 11 } } }, Margin = new Thickness(1) };
                ToolTip.SetTip(b, "Click to restore to the toolbar");
                b.Click += (_, _) =>
                {
                    hidden.Remove(captured);
                    groups.Add([captured]);
                    Build();
                };
                extra.Children.Add(b);
            }
        }

        void Act(Action<int, int> action)
        {
            if (selected is not (int g, int i)) return;
            action(g, i);
            groups.RemoveAll(x => x.Count == 0);
            Build();
        }
        Button up = new() { Content = "Move Group Up" };
        up.Click += (_, _) => Act((g, i) => { if (g > 0) { (groups[g - 1], groups[g]) = (groups[g], groups[g - 1]); selected = (g - 1, i); } });
        Button down = new() { Content = "Move Group Down" };
        down.Click += (_, _) => Act((g, i) => { if (g < groups.Count - 1) { (groups[g + 1], groups[g]) = (groups[g], groups[g + 1]); selected = (g + 1, i); } });
        Button left = new() { Content = "Move Earlier in Group" };
        left.Click += (_, _) => Act((g, i) => { if (i > 0) { (groups[g][i - 1], groups[g][i]) = (groups[g][i], groups[g][i - 1]); selected = (g, i - 1); } });
        Button split = new() { Content = "Own Group" };
        split.Click += (_, _) => Act((g, i) => { ToolId id = groups[g][i]; groups[g].RemoveAt(i); groups.Insert(g + 1, [id]); selected = (g + 1, 0); });
        Button merge = new() { Content = "Merge With Group Above" };
        merge.Click += (_, _) => Act((g, i) => { if (g > 0) { ToolId id = groups[g][i]; groups[g].RemoveAt(i); groups[g - 1].Add(id); selected = null; } });
        Button hide = new() { Content = "Hide Tool" };
        hide.Click += (_, _) => Act((g, i) => { if (groups[g][i] != ToolId.Move) { hidden.Add(groups[g][i]); groups[g].RemoveAt(i); selected = null; } });
        Button restore = new() { Content = "Restore Defaults" };
        restore.Click += (_, _) =>
        {
            groups = owner.Tools.DefaultGroups.Select(g => g.Tools.Select(t => t.Id).ToList()).ToList();
            hidden.Clear();
            selected = null;
            Build();
        };
        Build();
        WrapPanel actions = new() { Children = { up, down, left, split, merge, hide, restore } };
        foreach (Control c in actions.Children) c.Margin = new Thickness(2);
        StackPanel content = new() { Spacing = 8 };
        content.Children.Add(new TextBlock { Text = "Toolbar (each row is a toolbar slot with its flyout tools). Select a tool, then use the buttons to rearrange.", TextWrapping = TextWrapping.Wrap });
        content.Children.Add(new ScrollViewer { Content = toolbar, Height = 360 });
        content.Children.Add(actions);
        content.Children.Add(new TextBlock { Text = "Extra Tools (hidden from the toolbar):" });
        content.Children.Add(extra);
        if (!await DialogBuilder.ShowAsync(owner, "Customize Toolbar", content, 760, "Done")) return;
        owner.Settings.Toolbar = groups.Select(g => new ToolbarGroupLayout { Tools = g.Select(t => t.ToString()).ToList() }).ToList();
        owner.Settings.HiddenTools = hidden.Select(t => t.ToString()).ToList();
        owner.RebuildToolbar();
        owner.SaveSettings();
    }
}
