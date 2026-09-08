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

using SkiaSharp;

namespace ShareX.ImageEditor.Core.Annotations;

/// <summary>
/// Line annotation
/// </summary>
public partial class LineAnnotation : Annotation, ICurvedSegmentAnnotation
{
    public override AnnotationCategory Category => AnnotationCategory.Shapes;
    public BorderStyle BorderStyle { get; set; } = BorderStyle.Solid;

    public SKPoint CurvePoint { get; set; }
    public bool CurvePointActivated { get; set; }

    public LineAnnotation()
    {
        ToolType = EditorTool.Line;
    }

    public override bool HitTest(SKPoint point, float tolerance = 5)
    {
        return CurvedSegmentHelper.DistanceToPath(this, point) <= tolerance;
    }

    public override SKRect GetBounds()
    {
        return CurvedSegmentHelper.GetBounds(this);
    }

    internal override void MoveBy(float deltaX, float deltaY)
    {
        base.MoveBy(deltaX, deltaY);
        CurvedSegmentHelper.OffsetCurvePoint(this, deltaX, deltaY);
    }
}