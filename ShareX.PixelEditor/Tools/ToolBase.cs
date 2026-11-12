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
using ShareX.PixelEditor.UI;

namespace ShareX.PixelEditor.Tools;

public enum ToolId
{
    Move,
    RectSelect,
    EllipseSelect,
    Lasso,
    PolygonLasso,
    MagicWand,
    Pencil,
    Spray,
    Eraser,
    Shade,
    ColorReplace,
    Bucket,
    Gradient,
    Eyedropper,
    Line,
    Curve,
    Rectangle,
    Ellipse,
    Contour,
    Text,
    Slice,
    Hand,
    Zoom
}

/// <summary>A pointer position on the sprite: the pixel under the cursor and the exact position inside it.</summary>
public readonly record struct ToolPointer(int X, int Y, double Fx, double Fy, bool Right, KeyModifiers Modifiers)
{
    public bool Shift => Modifiers.HasFlag(KeyModifiers.Shift);

    public bool Control => Modifiers.HasFlag(KeyModifiers.Control);

    public bool Alt => Modifiers.HasFlag(KeyModifiers.Alt);

    public SKPointI Pixel => new(X, Y);
}

public abstract class ToolBase
{
    protected ToolBase(PixelEditorWindow editor, ToolId id, string name, string hint, string icon, Key shortcut)
    {
        Editor = editor;
        Id = id;
        Name = name;
        Hint = hint;
        Icon = icon;
        Shortcut = shortcut;
    }

    protected PixelEditorWindow Editor { get; }

    protected EditorSettings Settings => Editor.Settings;

    protected Sprite? Sprite => Editor.Sprite;

    public ToolId Id { get; }

    public string Name { get; }

    /// <summary>Shown in the status bar while the tool is active.</summary>
    public string Hint { get; }

    public string Icon { get; }

    public Key Shortcut { get; }

    public virtual StandardCursorType Cursor => StandardCursorType.Cross;

    /// <summary>The brush tip outlined under the cursor, or null for tools without one.</summary>
    public virtual SKPointI[]? PreviewTip => null;

    /// <summary>True while the tool holds uncommitted changes between pointer presses.</summary>
    public virtual bool HasPendingEdit => false;

    public virtual void Down(ToolPointer p)
    {
    }

    public virtual void Move(ToolPointer p)
    {
    }

    public virtual void Up(ToolPointer p)
    {
    }

    /// <summary>The pointer moved without a button held.</summary>
    public virtual void Hover(ToolPointer p)
    {
    }

    /// <summary>Handles a double-click. When it returns false the second click is an ordinary press, so quick taps still draw.</summary>
    public virtual bool DoubleClick(ToolPointer p) => false;

    public virtual bool KeyDown(Key key, KeyModifiers modifiers) => false;

    /// <summary>Abandons the edit in progress.</summary>
    public virtual void Cancel()
    {
    }

    public virtual void Activate()
    {
    }

    public virtual void Deactivate() => Cancel();

    public virtual void DrawOverlay(OverlayContext overlay)
    {
    }

    public virtual void BuildOptions(OptionsBuilder options)
    {
    }

    /// <summary>Snaps a pixel to the sprite grid when grid snapping is on.</summary>
    protected SKPointI SnapToGrid(SKPointI p)
    {
        if (!Settings.SnapToGrid) return p;
        int gw = Math.Max(1, Settings.GridWidth), gh = Math.Max(1, Settings.GridHeight);
        return new SKPointI((int)Math.Round(p.X / (double)gw) * gw, (int)Math.Round(p.Y / (double)gh) * gh);
    }

    /// <summary>Constrains a line end to the angles that make clean pixel lines: horizontal, vertical, diagonal and 2:1 slopes.</summary>
    protected static SKPointI ConstrainAngle(SKPointI from, SKPointI to)
    {
        int dx = to.X - from.X, dy = to.Y - from.Y, ax = Math.Abs(dx), ay = Math.Abs(dy);
        if (ax == 0 && ay == 0) return to;
        double angle = Math.Atan2(ay, ax) * 180 / Math.PI;
        int sx = Math.Sign(dx), sy = Math.Sign(dy);
        if (angle < 13.3) return new SKPointI(to.X, from.Y);
        if (angle < 35.8) return new SKPointI(from.X + sx * ax, from.Y + sy * (ax / 2));
        if (angle < 54.2) return new SKPointI(from.X + sx * Math.Max(ax, ay), from.Y + sy * Math.Max(ax, ay));
        if (angle < 76.7) return new SKPointI(from.X + sx * (ay / 2), from.Y + sy * ay);
        return new SKPointI(from.X, to.Y);
    }

    /// <summary>Makes a dragged box square.</summary>
    protected static SKPointI ConstrainSquare(SKPointI from, SKPointI to)
    {
        int dx = to.X - from.X, dy = to.Y - from.Y, size = Math.Max(Math.Abs(dx), Math.Abs(dy));
        return new SKPointI(from.X + (dx < 0 ? -size : size), from.Y + (dy < 0 ? -size : size));
    }
}

/// <summary>A group of related tools sharing one toolbar button.</summary>
public sealed class ToolGroup
{
    public ToolGroup(int section, params ToolBase[] tools)
    {
        Section = section;
        Tools = tools;
        Current = tools[0];
    }

    public int Section { get; }

    public IReadOnlyList<ToolBase> Tools { get; }

    /// <summary>The tool of the group shown on the toolbar: the one used last.</summary>
    public ToolBase Current { get; set; }
}

public sealed class ToolManager
{
    private readonly PixelEditorWindow _editor;

    public ToolManager(PixelEditorWindow editor)
    {
        _editor = editor;
        Groups =
        [
            new ToolGroup(0, new MoveTool(editor)),
            new ToolGroup(0, new RectSelectTool(editor), new EllipseSelectTool(editor)),
            new ToolGroup(0, new LassoTool(editor), new PolygonLassoTool(editor)),
            new ToolGroup(0, new MagicWandTool(editor)),
            new ToolGroup(1, new PencilTool(editor), new SprayTool(editor)),
            new ToolGroup(1, new EraserTool(editor)),
            new ToolGroup(1, new ShadeTool(editor), new ColorReplaceTool(editor)),
            new ToolGroup(1, new BucketTool(editor), new GradientTool(editor)),
            new ToolGroup(1, new EyedropperTool(editor)),
            new ToolGroup(2, new LineTool(editor), new CurveTool(editor)),
            new ToolGroup(2, new RectangleTool(editor), new EllipseTool(editor), new ContourTool(editor)),
            new ToolGroup(2, new TextTool(editor)),
            new ToolGroup(2, new SliceTool(editor)),
            new ToolGroup(3, new HandTool(editor)),
            new ToolGroup(3, new ZoomTool(editor))
        ];
        Active = Get(ToolId.Pencil);
    }

    public IReadOnlyList<ToolGroup> Groups { get; }

    public ToolBase Active { get; private set; }

    public IEnumerable<ToolBase> All => Groups.SelectMany(g => g.Tools);

    public event Action? ActiveChanged;

    public ToolBase Get(ToolId id) => All.First(t => t.Id == id);

    public ToolGroup GroupOf(ToolBase tool) => Groups.First(g => g.Tools.Contains(tool));

    public void Select(ToolId id) => Select(Get(id));

    public void Select(ToolBase tool)
    {
        if (tool == Active) return;
        Active.Deactivate();
        // A moved or pasted selection is dropped in place when another tool takes over.
        if (tool is not MoveTool) _editor.CommitFloating();
        Active = tool;
        GroupOf(tool).Current = tool;
        tool.Activate();
        ActiveChanged?.Invoke();
    }

    /// <summary>Selects the tool bound to a key; pressing the key again cycles through the tools sharing it.</summary>
    public bool HandleShortcut(Key key, KeyModifiers modifiers)
    {
        if (modifiers != KeyModifiers.None && modifiers != KeyModifiers.Shift) return false;
        List<ToolBase> matches = All.Where(t => t.Shortcut == key).ToList();
        if (matches.Count == 0) return false;
        int index = matches.IndexOf(Active);
        Select(matches[(index + 1) % matches.Count]);
        return true;
    }
}
