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
using Avalonia.Media;
using ShareX.ImageEditor.Core.Annotations;
using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.Rendering;

internal static class AnnotationVisualHelpers
{
    internal static void ApplyBoundsControl(Control control, SKRect bounds, bool ensureMinimumSize)
    {
        double left = bounds.Left;
        double top = bounds.Top;
        double width = ensureMinimumSize ? Math.Max(1, bounds.Width) : bounds.Width;
        double height = ensureMinimumSize ? Math.Max(1, bounds.Height) : bounds.Height;

        Canvas.SetLeft(control, left);
        Canvas.SetTop(control, top);
        control.Width = width;
        control.Height = height;

        if (control.Tag is MagnifyAnnotation magnifyAnnotation)
        {
            control.Clip = magnifyAnnotation.IsEllipse
                ? new EllipseGeometry(new Rect(0, 0, Math.Max(1, width), Math.Max(1, height)))
                : null;
        }
    }

    internal static void ApplyRotationTransform(Control control, float rotationAngle, RelativePoint? transformOrigin = null)
    {
        if (rotationAngle != 0)
        {
            control.RenderTransformOrigin = transformOrigin ?? new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
            control.RenderTransform = new RotateTransform(rotationAngle);
        }
        else
        {
            control.RenderTransform = null;
        }
    }
}
