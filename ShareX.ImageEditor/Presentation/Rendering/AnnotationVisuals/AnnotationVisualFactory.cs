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
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Presentation.Controls;

using static ShareX.ImageEditor.Presentation.Rendering.AnnotationVisualHelpers;

namespace ShareX.ImageEditor.Presentation.Rendering;

/// <summary>
/// Indicates how an annotation visual is used by the host.
/// </summary>
public enum AnnotationVisualMode
{
    Persisted,
    Preview
}

/// <summary>
/// Shared factory/synchronizer for annotation visuals used by editor and region-capture hosts.
/// </summary>
public static class AnnotationVisualFactory
{
    /// <summary>
    /// Creates the visual control for the provided annotation.
    /// </summary>
    public static Control? CreateVisualControl(Annotation annotation, AnnotationVisualMode mode = AnnotationVisualMode.Persisted)
    {
        ArgumentNullException.ThrowIfNull(annotation);

        if (mode == AnnotationVisualMode.Preview)
        {
            return annotation switch
            {
                TextAnnotation text => text.CreatePreviewVisual(),
                SpeechBalloonAnnotation balloon => balloon.CreatePreviewVisual(),
                SpotlightAnnotation spotlight => spotlight.CreatePreviewVisual(),
                BlurAnnotation blur => blur.CreatePreviewVisual(),
                PixelateAnnotation pixelate => pixelate.CreatePreviewVisual(),
                MagnifyAnnotation magnify => magnify.CreatePreviewVisual(),
                HighlightAnnotation highlight => highlight.CreatePreviewVisual(),
                _ => CreatePersistedVisualControl(annotation)
            };
        }

        return CreatePersistedVisualControl(annotation);
    }

    /// <summary>
    /// Updates an existing visual control to reflect the annotation's current geometry and position.
    /// </summary>
    public static void UpdateVisualControl(
        Control control,
        Annotation annotation,
        AnnotationVisualMode mode = AnnotationVisualMode.Persisted,
        double canvasWidth = 0,
        double canvasHeight = 0,
        bool useInteractiveEmojiRender = false)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(annotation);

        bool ensureMinimumSize = mode == AnnotationVisualMode.Preview;

        switch (annotation)
        {
            case RectangleAnnotation rectangle when control is Rectangle rectangleControl:
                rectangle.UpdateVisual(rectangleControl, ensureMinimumSize);
                break;

            case EllipseAnnotation ellipseAnnotation when control is Ellipse ellipseControl:
                ellipseAnnotation.UpdateVisual(ellipseControl, ensureMinimumSize);
                break;

            case LineAnnotation lineAnnotation when control is Avalonia.Controls.Shapes.Path linePath:
                lineAnnotation.UpdateVisual(linePath);
                break;

            case ArrowAnnotation arrow when control is Avalonia.Controls.Shapes.Path arrowPath:
                arrow.UpdateVisual(arrowPath);
                break;

            case FreehandAnnotation freehand when control is Avalonia.Controls.Shapes.Path freehandPath:
                freehand.UpdateVisual(freehandPath);
                break;

            case NumberAnnotation number when control is StepControl stepControl:
                number.UpdateVisual(stepControl, ensureMinimumSize);
                break;

            case TextAnnotation text when mode == AnnotationVisualMode.Preview && control is Rectangle:
                ApplyBoundsControl(control, text.GetBounds(), ensureMinimumSize: true);
                break;

            case TextAnnotation text when control is OutlinedTextControl textControl:
                text.UpdateVisual(textControl);
                break;

            case SpeechBalloonAnnotation balloon when mode == AnnotationVisualMode.Preview && control is Rectangle balloonPreview:
                balloon.UpdatePreviewVisual(balloonPreview);
                break;

            case SpeechBalloonAnnotation balloon when control is SpeechBalloonControl balloonControl:
                balloon.UpdateVisual(balloonControl, ensureMinimumSize);
                break;

            case BaseEffectAnnotation effectAnnotation when control is Shape effectControl:
                effectAnnotation.UpdateVisual(effectControl, ensureMinimumSize);
                break;

            case SpotlightAnnotation spotlight when mode == AnnotationVisualMode.Preview && control is Shape:
                ApplyBoundsControl(control, spotlight.GetBounds(), ensureMinimumSize: true);
                break;

            case SpotlightAnnotation spotlight when control is SpotlightControl spotlightControl:
                spotlight.UpdateVisual(spotlightControl, canvasWidth, canvasHeight);
                break;

            case CursorAnnotation cursorAnnotation when control is Image cursorControl:
                cursorAnnotation.UpdateVisual(cursorControl);
                break;

            case EmojiAnnotation emojiAnnotation when control is Image emojiControl:
                emojiAnnotation.UpdateVisual(emojiControl, useInteractiveEmojiRender);
                break;

            case ImageAnnotation imageAnnotation when control is Image imageControl:
                imageAnnotation.UpdateVisual(imageControl);
                break;

            default:
                ApplyBoundsControl(control, annotation.GetBounds(), ensureMinimumSize);
                break;
        }
    }

    private static Control? CreatePersistedVisualControl(Annotation annotation)
    {
        return annotation switch
        {
            SmartEraserAnnotation smartEraser => smartEraser.CreateVisual(),
            RectangleAnnotation rect => rect.CreateVisual(),
            EllipseAnnotation ellipse => ellipse.CreateVisual(),
            LineAnnotation line => line.CreateVisual(),
            ArrowAnnotation arrow => arrow.CreateVisual(),
            TextAnnotation text => text.CreateVisual(),
            SpeechBalloonAnnotation balloon => balloon.CreateVisual(),
            NumberAnnotation number => number.CreateVisual(),
            BlurAnnotation blur => blur.CreateVisual(),
            PixelateAnnotation pixelate => pixelate.CreateVisual(),
            MagnifyAnnotation magnify => magnify.CreateVisual(),
            HighlightAnnotation highlight => highlight.CreateVisual(),
            SpotlightAnnotation spotlight => spotlight.CreateVisual(),
            FreehandAnnotation freehand => freehand.CreateVisual(),
            CursorAnnotation cursor => cursor.CreateVisual(),
            EmojiAnnotation emoji => emoji.CreateVisual(),
            ImageAnnotation image => image.CreateVisual(),
            _ => null
        };
    }
}
