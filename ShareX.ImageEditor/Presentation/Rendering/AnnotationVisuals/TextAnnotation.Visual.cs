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
using static ShareX.ImageEditor.Presentation.Rendering.AnnotationVisualHelpers;

namespace ShareX.ImageEditor.Core.Annotations;

public partial class TextAnnotation
{
    /// <summary>
    /// Creates the Avalonia visual for this annotation (TextBox for editing)
    /// </summary>
    public Control CreateVisual()
    {
        var control = new ShareX.ImageEditor.Presentation.Controls.OutlinedTextControl
        {
            Annotation = this,
            Tag = this,
            IsHitTestVisible = false
        };

        if (ShadowEnabled)
        {
            control.Effect = ShareX.ImageEditor.Presentation.Helpers.ShadowEffectHelper.CreateDropShadow(this);
        }

        return control;
    }

    internal void UpdateVisual(OutlinedTextControl textControl)
    {
        var textBounds = GetBounds();
        textControl.Annotation = this;
        Canvas.SetLeft(textControl, textBounds.Left);
        Canvas.SetTop(textControl, textBounds.Top);
        textControl.Width = Math.Max(1, textBounds.Width);
        textControl.Height = Math.Max(1, textBounds.Height);

        // The control renders text styling from the annotation; geometry changes still need invalidation.

        ApplyRotationTransform(textControl, RotationAngle);

        textControl.InvalidateVisual();
        textControl.InvalidateMeasure();
    }

    internal Control CreatePreviewVisual()
    {
        return new Rectangle
        {
            Stroke = new SolidColorBrush(Color.Parse(StrokeColor)),
            StrokeThickness = 1,
            StrokeDashArray = new AvaloniaList<double> { 4, 4 },
            Fill = new SolidColorBrush(Color.FromArgb(30, 255, 255, 255)),
            Tag = this
        };
    }
}
