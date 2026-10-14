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
using Avalonia.Input;
using Avalonia.Media;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Presentation.Controls;
using ShareX.ImageEditor.Presentation.Helpers;
using ShareX.ImageEditor.Presentation.Rendering;
using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.Controllers;

public partial class EditorSelectionController
{
    private TextBox? _balloonTextEditor;

    private void ShowSpeechBalloonTextEditor(SpeechBalloonControl balloonControl)
    {
        if (balloonControl.Annotation == null) return;

        // Use OverlayCanvas to ensure TextBox is on top of everything
        var overlay = _view.FindControl<Canvas>("OverlayCanvas");
        if (overlay == null) return;

        if (_balloonTextEditor != null)
        {
            // Remove from whichever parent it has
            (_balloonTextEditor.Parent as Panel)?.Children.Remove(_balloonTextEditor);
            _balloonTextEditor = null;
        }

        var annotation = balloonControl.Annotation;
        var balloonRect = GetLogicalRect(balloonControl);
        var balloonLeft = balloonRect.Left;
        var balloonTop = balloonRect.Top;
        var balloonWidth = balloonRect.Width;
        var balloonHeight = balloonRect.Height;

        // Check if balloon is too small (e.g. user just clicked without dragging)
        if (balloonWidth < 50 || balloonHeight < 30)
        {
            balloonWidth = Math.Max(balloonWidth, 200);
            balloonHeight = Math.Max(balloonHeight, 100);

            annotation.EndPoint = new SKPoint(
                annotation.StartPoint.X + (float)balloonWidth,
                annotation.StartPoint.Y + (float)balloonHeight
            );

            // Fix Tail Point if it was at 0,0 or default
            if (annotation.TailEnabled &&
                (!annotation.HasTailPoint ||
                (Math.Abs(annotation.TailPoint.X - annotation.StartPoint.X) < 1 && Math.Abs(annotation.TailPoint.Y - annotation.StartPoint.Y) < 1)))
            {
                annotation.SetTailPoint(annotation.GetDefaultTailPoint());
            }

            AnnotationVisualFactory.UpdateVisualControl(
                balloonControl,
                annotation,
                AnnotationVisualMode.Persisted,
                _view.EditorCore.CanvasSize.Width,
                _view.EditorCore.CanvasSize.Height);
            UpdateSelectionHandles();
        }

        var textBox = annotation.CreateTextEditor();

        // Keep the editor above annotation visuals and selection handles.
        textBox.SetValue(Panel.ZIndexProperty, 9999);
        ApplySpeechBalloonTextEditorLayout(textBox, annotation, balloonLeft, balloonTop, balloonWidth, balloonHeight);

        textBox.LostFocus += (s, args) =>
        {
            if (s is TextBox tb)
            {
                annotation.Text = tb.Text ?? string.Empty;
                balloonControl.InvalidateVisual();
                (tb.Parent as Panel)?.Children.Remove(tb); // Remove from OverlayCanvas
                _balloonTextEditor = null;
            }
        };

        AnnotationTextEditorHelper.AttachKeyHandlers(textBox, () => _view.Focus());

        overlay.Children.Add(textBox); // Add to OverlayCanvas
        _balloonTextEditor = textBox;
        textBox.Focus();
        textBox.CaretIndex = textBox.Text?.Length ?? 0; // Place caret at end
    }

    public void UpdateActiveTextEditorProperties()
    {
        if (_balloonTextEditor == null || _selectedShape is not SpeechBalloonControl balloonControl) return;
        if (balloonControl.Annotation is not SpeechBalloonAnnotation annotation) return;

        annotation.UpdateTextEditorAppearance(_balloonTextEditor);

        var bodyBounds = annotation.GetBounds();
        ApplySpeechBalloonTextEditorLayout(
            _balloonTextEditor,
            annotation,
            bodyBounds.Left,
            bodyBounds.Top,
            bodyBounds.Width,
            bodyBounds.Height);
    }

    private static void ApplySpeechBalloonTextEditorLayout(TextBox textBox, SpeechBalloonAnnotation annotation, double left, double top, double width, double height)
    {
        Canvas.SetLeft(textBox, ToOverlayCoordinate(left));
        Canvas.SetTop(textBox, ToOverlayCoordinate(top));
        textBox.Width = width;
        textBox.Height = height;

        if (annotation.RotationAngle != 0)
        {
            textBox.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
            textBox.RenderTransform = new RotateTransform(annotation.RotationAngle);
        }
        else
        {
            textBox.RenderTransform = null;
        }
    }

    private void ShowTextEditor(OutlinedTextControl textControl)
    {
        if (textControl.Annotation is not TextAnnotation annotation) return;

        var overlay = _view.FindControl<Canvas>("OverlayCanvas");
        if (overlay == null) return;

        // Hide the original control while editing
        textControl.IsVisible = false;

        var textBox = annotation.CreateTextEditor();

        var annotationBounds = annotation.GetBounds();
        Canvas.SetLeft(textBox, ToOverlayCoordinate(annotationBounds.Left));
        Canvas.SetTop(textBox, ToOverlayCoordinate(annotationBounds.Top));
        textBox.Width = Math.Max(20, annotationBounds.Width);
        textBox.Height = Math.Max(20, annotationBounds.Height);

        EventHandler<FocusChangedEventArgs>? lostFocusHandler = null;

        void CompleteEditing()
        {
            if (lostFocusHandler != null) textBox.LostFocus -= lostFocusHandler;

            annotation.Text = textBox.Text ?? string.Empty;

            // Remove from overlay
            overlay.Children.Remove(textBox);

            // Keep the existing annotation rectangle so wrapped text stays inside the resized bounds.
            textControl.IsVisible = true;
            textControl.InvalidateMeasure();
            textControl.InvalidateVisual();

            // Fire RequestUpdateEffect to save new state if needed
            RequestUpdateEffect?.Invoke(textControl);

            UpdateSelectionHandles();
            UpdateHoverOutline();
        }

        lostFocusHandler = (s, args) => CompleteEditing();

        textBox.LostFocus += lostFocusHandler;
        AnnotationTextEditorHelper.AttachKeyHandlers(textBox, () => _view.Focus());

        overlay.Children.Add(textBox);
        textBox.Focus();
        textBox.CaretIndex = textBox.Text?.Length ?? 0;
        textBox.SelectionStart = textBox.CaretIndex;
        textBox.SelectionEnd = textBox.CaretIndex;
    }
}
