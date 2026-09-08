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

using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using ShareX.ImageEditor.Presentation.Helpers;
using static ShareX.ImageEditor.Presentation.Rendering.AnnotationVisualHelpers;

namespace ShareX.ImageEditor.Core.Annotations;

public partial class EllipseAnnotation
{
    /// <summary>
    /// Creates the Avalonia visual for this annotation
    /// </summary>
    public Control CreateVisual()
    {
        var strokeBrush = new SolidColorBrush(Color.Parse(StrokeColor));
        IBrush fillBrush = string.IsNullOrEmpty(FillColor) || FillColor == "#00000000"
            ? Brushes.Transparent
            : new SolidColorBrush(Color.Parse(FillColor));
        var ellipse = new Avalonia.Controls.Shapes.Ellipse
        {
            Stroke = strokeBrush,
            StrokeThickness = StrokeWidth,
            StrokeDashArray = BorderStyleDashHelper.CreateStrokeDashArray(BorderStyle),
            StrokeLineCap = BorderStyleDashHelper.CreateStrokeLineCap(BorderStyle),
            Fill = fillBrush,
            Tag = this,
            Width = 0,
            Height = 0
        };

        if (ShadowEnabled)
        {
            ellipse.Effect = ShareX.ImageEditor.Presentation.Helpers.ShadowEffectHelper.CreateDropShadow(this);
        }

        return ellipse;
    }

    internal void UpdateVisual(Ellipse ellipseControl, bool ensureMinimumSize)
    {
        ellipseControl.StrokeDashArray = BorderStyleDashHelper.CreateStrokeDashArray(BorderStyle);
        ellipseControl.StrokeLineCap = BorderStyleDashHelper.CreateStrokeLineCap(BorderStyle);
        ApplyBoundsControl(ellipseControl, GetBounds(), ensureMinimumSize);
        ApplyRotationTransform(ellipseControl, RotationAngle);
    }
}
