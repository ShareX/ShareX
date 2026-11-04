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


namespace ShareX.PhotoEditor.Tools;

public enum PathToolMode
{
    Shape,
    Path
}

/// <summary>Shared helpers for tools that create or edit vector paths.</summary>
public abstract class PathToolBase : ToolBase
{
    protected PathToolBase(IEditorHost host) : base(host)
    {
    }

    public override ToolCursor Cursor => ToolCursor.Pen;

    protected float Tolerance => 6 / (float)(Host.View?.Zoom ?? 1);

    /// <summary>The path being edited: the active shape layer's path, the selected path, or the work path.</summary>
    protected VectorPath? GetTargetPath(bool create, PathToolMode mode = PathToolMode.Path)
    {
        PhotoDocument? doc = Doc;
        if (doc == null) return null;
        if (doc.ActiveLayer is ShapeLayer shape && (mode == PathToolMode.Shape || doc.ActivePath == null)) return shape.Path;
        if (doc.ActivePath != null) return doc.ActivePath.Path;
        if (doc.WorkPath != null)
        {
            doc.ActivePath = new NamedPath("Work Path", doc.WorkPath);
            return doc.WorkPath;
        }
        if (!create) return null;
        doc.WorkPath = new VectorPath();
        doc.ActivePath = new NamedPath("Work Path", doc.WorkPath);
        return doc.WorkPath;
    }

    protected void PathChanged()
    {
        if (Doc?.ActiveLayer is ShapeLayer) Host.InvalidateCanvas();
        else Host.InvalidateOverlay();
    }

    public override void DrawOverlay(OverlayContext context)
    {
        VectorPath? path = GetTargetPath(false);
        if (path == null || path.IsEmpty) return;
        using SKPath skPath = path.ToSKPath();
        context.Path(skPath);
        context.PathAnchors(path, SelectedAnchor);
    }

    protected virtual PathAnchor? SelectedAnchor => null;

    /// <summary>Creates a shape layer filled with the foreground color for a new path.</summary>
    protected ShapeLayer CreateShapeLayer(VectorPath path)
    {
        PhotoDocument doc = Doc!;
        int number = doc.AllLayers.OfType<ShapeLayer>().Count() + 1;
        ShapeLayer layer = new($"Shape {number}", path) { FillColor = Host.Foreground };
        doc.ActivePath = null;
        doc.AddLayer(layer);
        return layer;
    }
}
