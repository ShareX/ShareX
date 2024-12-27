﻿#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2024 ShareX Team

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

using ShareX.HelpersLib;
using ShareX.HelpersLib.Helpers;

using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShareX.ScreenCaptureLib.Shapes.Drawing;

public class FreehandArrowDrawingShape : FreehandDrawingShape
{
    public override ShapeType ShapeType { get; } = ShapeType.DrawingFreehandArrow;

    public ArrowHeadDirection ArrowHeadDirection { get; set; }

    public override void OnConfigLoad()
    {
        base.OnConfigLoad();
        ArrowHeadDirection = AnnotationOptions.ArrowHeadDirection;
    }

    public override void OnConfigSave()
    {
        base.OnConfigSave();
        AnnotationOptions.ArrowHeadDirection = ArrowHeadDirection;
    }

    protected override Pen CreatePen(Color borderColor, int borderSize, BorderStyle borderStyle)
    {
        using GraphicsPath gp = new();
        int arrowWidth = 2, arrowHeight = 6, arrowCurve = 1;
        gp.AddLine(new Point(0, 0), new Point(-arrowWidth, -arrowHeight));
        gp.AddCurve(new Point[] { new(-arrowWidth, -arrowHeight), new(0, -arrowHeight + arrowCurve), new(arrowWidth, -arrowHeight) });
        gp.CloseFigure();

        CustomLineCap lineCap = new(gp, null)
        {
            BaseInset = arrowHeight - arrowCurve
        };

        Pen pen = new(borderColor, borderSize);

        if (ArrowHeadDirection == ArrowHeadDirection.Both && MathHelpers.Distance(positions[0], positions[positions.Count - 1]) > arrowHeight * borderSize * 2)
        {
            pen.CustomEndCap = pen.CustomStartCap = lineCap;
        } else if (ArrowHeadDirection == ArrowHeadDirection.Start)
        {
            pen.CustomStartCap = lineCap;
        } else
        {
            pen.CustomEndCap = lineCap;
        }

        pen.LineJoin = LineJoin.Round;
        pen.DashStyle = (DashStyle)borderStyle;
        return pen;
    }
}