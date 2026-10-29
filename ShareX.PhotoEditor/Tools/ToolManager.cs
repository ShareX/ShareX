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

using Avalonia.Input;
using ShareX.PhotoEditor.UI;

namespace ShareX.PhotoEditor.Tools;

/// <summary>A toolbar slot holding one or more tool variants that share a flyout.</summary>
public sealed class ToolGroup
{
    public ToolGroup(string name, IEnumerable<ToolBase> tools)
    {
        Name = name;
        Tools = tools.ToList();
        Current = Tools[0];
    }

    public string Name { get; }

    public List<ToolBase> Tools { get; }

    public ToolBase Current { get; set; }
}

/// <summary>Owns all tool instances, the toolbar grouping and keyboard shortcuts.</summary>
public sealed class ToolManager
{
    private readonly Dictionary<ToolId, ToolBase> _tools = [];

    public ToolManager(IEditorHost host)
    {
        foreach (ToolBase tool in ToolRegistry.CreateAll(host)) _tools[tool.Id] = tool;
        DefaultGroups = ToolRegistry.DefaultLayout.Select(g => new ToolGroup(g.Name, g.Tools.Select(id => _tools[id]))).ToList();
        Groups = DefaultGroups.ToList();
        Active = _tools[ToolId.Move];
    }

    public IReadOnlyList<ToolGroup> DefaultGroups { get; }

    public List<ToolGroup> Groups { get; private set; }

    public ToolBase Active { get; private set; }

    /// <summary>The tool to return to after a temporary (spring-loaded) tool is released.</summary>
    public ToolBase? Previous { get; private set; }

    public event Action? ActiveChanged;

    public IEnumerable<ToolBase> All => _tools.Values;

    public ToolBase Get(ToolId id) => _tools[id];

    public T Get<T>() where T : ToolBase => _tools.Values.OfType<T>().First();

    public static char? GetShortcut(ToolId id) => ToolRegistry.Shortcuts.TryGetValue(id, out char key) ? key : null;

    public ToolGroup? FindGroup(ToolBase tool) => Groups.FirstOrDefault(g => g.Tools.Contains(tool));

    public void Select(ToolId id) => Select(_tools[id]);

    public void Select(ToolBase tool)
    {
        if (tool == Active) return;
        Active.Deactivate();
        Previous = Active;
        Active = tool;
        ToolGroup? group = FindGroup(tool);
        if (group != null) group.Current = tool;
        tool.Activate();
        ActiveChanged?.Invoke();
    }

    /// <summary>Applies a customized toolbar layout from settings (Edit Toolbar).</summary>
    public void ApplyLayout(EditorSettings settings)
    {
        if (settings.Toolbar.Count == 0 && settings.HiddenTools.Count == 0)
        {
            Groups = DefaultGroups.Select(g => new ToolGroup(g.Name, g.Tools)).ToList();
            return;
        }
        List<ToolGroup> groups = [];
        HashSet<ToolId> used = [];
        IEnumerable<ToolbarGroupLayout> layout = settings.Toolbar.Count > 0
            ? settings.Toolbar
            : DefaultGroups.Select(g => new ToolbarGroupLayout { Tools = g.Tools.Select(t => t.Id.ToString()).ToList() });
        foreach (ToolbarGroupLayout groupLayout in layout)
        {
            List<ToolBase> tools = [];
            foreach (string name in groupLayout.Tools)
            {
                if (Enum.TryParse(name, out ToolId id) && _tools.TryGetValue(id, out ToolBase? tool) && used.Add(id) && !settings.HiddenTools.Contains(name))
                {
                    tools.Add(tool);
                }
            }
            if (tools.Count > 0) groups.Add(new ToolGroup(tools[0].Name, tools));
        }
        // Tools missing from a saved layout (for example after an update) go to their default group.
        foreach (ToolGroup defaultGroup in DefaultGroups)
        {
            foreach (ToolBase tool in defaultGroup.Tools.Where(t => !used.Contains(t.Id) && !settings.HiddenTools.Contains(t.Id.ToString())))
            {
                ToolGroup? target = groups.FirstOrDefault(g => g.Tools.Any(t => defaultGroup.Tools.Contains(t)));
                if (target != null) target.Tools.Add(tool);
                else groups.Add(new ToolGroup(defaultGroup.Name, [tool]));
            }
        }
        Groups = groups;
    }

    /// <summary>Handles single-letter tool shortcuts; Shift cycles through the tools of a group.</summary>
    public bool HandleShortcut(Key key, KeyModifiers modifiers)
    {
        if (modifiers.HasFlag(KeyModifiers.Control) || modifiers.HasFlag(KeyModifiers.Alt)) return false;
        if (key < Key.A || key > Key.Z) return false;
        char letter = (char)('A' + (key - Key.A));
        List<ToolGroup> matching = Groups.Where(g => g.Tools.Any(t => GetShortcut(t.Id) == letter)).ToList();
        if (matching.Count == 0) return false;
        bool cycle = modifiers.HasFlag(KeyModifiers.Shift);
        ToolGroup? currentGroup = FindGroup(Active);
        if (cycle && currentGroup != null && matching.Contains(currentGroup))
        {
            List<ToolBase> candidates = currentGroup.Tools.Where(t => GetShortcut(t.Id) == letter).ToList();
            int index = candidates.IndexOf(Active);
            Select(candidates[(index + 1) % candidates.Count]);
            return true;
        }
        ToolGroup group = matching.Contains(currentGroup!) ? currentGroup! : matching[0];
        ToolBase target = GetShortcut(group.Current.Id) == letter ? group.Current : group.Tools.First(t => GetShortcut(t.Id) == letter);
        Select(target);
        return true;
    }
}
