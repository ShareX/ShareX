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

using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using ShareX.ImageEditor.Presentation.Controls;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.Annotations;

public partial class SpotlightAnnotation
{
    /// <summary>
    /// Creates the Avalonia visual for this annotation.
    /// </summary>
    public Control CreateVisual()
    {
        return new SpotlightControl
        {
            Annotation = this,
            IsHitTestVisible = false,
            Tag = this
        };
    }

    internal void UpdateVisual(SpotlightControl spotlightControl, double canvasWidth, double canvasHeight)
    {
        if (canvasWidth > 0 && canvasHeight > 0)
        {
            CanvasSize = new SKSize((float)canvasWidth, (float)canvasHeight);
        }

        spotlightControl.Annotation = this;
        Canvas.SetLeft(spotlightControl, 0);
        Canvas.SetTop(spotlightControl, 0);
        spotlightControl.Width = Math.Max(1, CanvasSize.Width);
        spotlightControl.Height = Math.Max(1, CanvasSize.Height);
        spotlightControl.InvalidateVisual();
    }

    internal Control CreatePreviewVisual()
    {
        Shape shape = IsEllipse ? new Ellipse() : new Rectangle();

        shape.Fill = Brushes.Transparent;
        shape.Stroke = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255));
        shape.StrokeThickness = 2;
        shape.StrokeDashArray = new AvaloniaList<double> { 6, 3 };
        shape.Tag = this;

        return shape;
    }
}
