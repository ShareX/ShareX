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
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Presentation.Controls;
using ShareX.ImageEditor.Presentation.Helpers;
using ShareX.ImageEditor.Presentation.Rendering;
using ShareX.ImageEditor.Presentation.ViewModels;

namespace ShareX.ImageEditor.Presentation.Views
{
    public partial class EditorView : UserControl
    {
        private void HookAnnotationToolbarEvents()
        {
            var toolbar = this.FindControl<AnnotationToolbar>("AnnotationToolbarControl");
            if (toolbar == null)
            {
                return;
            }

            toolbar.ColorChanged += OnColorChanged;
            toolbar.FillColorChanged += OnFillColorChanged;
            toolbar.TextColorChanged += OnTextColorChanged;
            toolbar.WidthChanged += OnWidthChanged;
            toolbar.BorderStyleChanged += OnBorderStyleChanged;
            toolbar.CornerRadiusChanged += OnCornerRadiusChanged;
            toolbar.FontSizeChanged += OnFontSizeChanged;
            toolbar.FontFamilyChanged += OnFontFamilyChanged;
            toolbar.ArrowStyleChanged += OnArrowStyleChanged;
            toolbar.CursorTypeChanged += OnCursorTypeChanged;
            toolbar.StrengthChanged += OnStrengthChanged;
            toolbar.SpotlightBlurChanged += OnSpotlightBlurChanged;
            toolbar.TextBoldChanged += OnToolbarTextBoldChanged;
            toolbar.TextItalicChanged += OnToolbarTextItalicChanged;
            toolbar.ShadowChanged += OnToolbarShadowChanged;
            toolbar.ShadowSettingsChanged += OnToolbarShadowSettingsChanged;
            toolbar.SpeechBalloonTailChanged += OnToolbarSpeechBalloonTailChanged;
            toolbar.EffectEllipseChanged += OnToolbarEffectEllipseChanged;
            toolbar.FavoriteEffectsMenuRequested += OnFavoriteEffectsMenuRequested;
        }

        private void UnhookAnnotationToolbarEvents()
        {
            var toolbar = this.FindControl<AnnotationToolbar>("AnnotationToolbarControl");
            if (toolbar == null)
            {
                return;
            }

            toolbar.ColorChanged -= OnColorChanged;
            toolbar.FillColorChanged -= OnFillColorChanged;
            toolbar.TextColorChanged -= OnTextColorChanged;
            toolbar.WidthChanged -= OnWidthChanged;
            toolbar.BorderStyleChanged -= OnBorderStyleChanged;
            toolbar.CornerRadiusChanged -= OnCornerRadiusChanged;
            toolbar.FontSizeChanged -= OnFontSizeChanged;
            toolbar.FontFamilyChanged -= OnFontFamilyChanged;
            toolbar.ArrowStyleChanged -= OnArrowStyleChanged;
            toolbar.CursorTypeChanged -= OnCursorTypeChanged;
            toolbar.StrengthChanged -= OnStrengthChanged;
            toolbar.SpotlightBlurChanged -= OnSpotlightBlurChanged;
            toolbar.TextBoldChanged -= OnToolbarTextBoldChanged;
            toolbar.TextItalicChanged -= OnToolbarTextItalicChanged;
            toolbar.ShadowChanged -= OnToolbarShadowChanged;
            toolbar.ShadowSettingsChanged -= OnToolbarShadowSettingsChanged;
            toolbar.SpeechBalloonTailChanged -= OnToolbarSpeechBalloonTailChanged;
            toolbar.EffectEllipseChanged -= OnToolbarEffectEllipseChanged;
            toolbar.FavoriteEffectsMenuRequested -= OnFavoriteEffectsMenuRequested;
        }

        private void OnFavoriteEffectsMenuRequested(object? sender, Control target)
        {
            if (DataContext is not MainViewModel vm)
            {
                return;
            }

            EffectBrowserPanel effectBrowserPanel = EnsureEffectBrowserPanel(vm);
            IReadOnlyList<MenuItem> favoriteMenuItems = effectBrowserPanel.CreateFavoriteMenuItems();
            if (favoriteMenuItems.Count == 0)
            {
                return;
            }

            var menu = new ContextMenu
            {
                Placement = PlacementMode.BottomEdgeAlignedLeft,
                PlacementTarget = target,
                ItemsSource = favoriteMenuItems,
                BorderBrush = (IBrush?)Resources["ShareX.Brush.Border"],
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(6, 2)
            };

            menu.Open(target);
        }

        private void OnColorChanged(object? sender, IBrush color)
        {
            if (DataContext is MainViewModel vm && color is SolidColorBrush solidBrush)
            {
                var hexColor = $"#{solidBrush.Color.A:X2}{solidBrush.Color.R:X2}{solidBrush.Color.G:X2}{solidBrush.Color.B:X2}";
                vm.SetColorCommand.Execute(hexColor);
            }
        }

        private void OnFillColorChanged(object? sender, IBrush color)
        {
            if (DataContext is MainViewModel vm && color is SolidColorBrush solidBrush)
            {
                var hexColor = $"#{solidBrush.Color.A:X2}{solidBrush.Color.R:X2}{solidBrush.Color.G:X2}{solidBrush.Color.B:X2}";
                vm.FillColor = hexColor;
            }
        }

        private void OnTextColorChanged(object? sender, IBrush color)
        {
            if (DataContext is MainViewModel vm && color is SolidColorBrush solidBrush)
            {
                var hexColor = $"#{solidBrush.Color.A:X2}{solidBrush.Color.R:X2}{solidBrush.Color.G:X2}{solidBrush.Color.B:X2}";
                vm.TextColor = hexColor;
            }
        }

        private void OnFontSizeChanged(object? sender, float fontSize)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.FontSize = fontSize;
                ApplySelectedFontSize(fontSize);
            }
        }

        private void OnFontFamilyChanged(object? sender, string fontFamily)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SelectedFontFamily = fontFamily;
                ApplySelectedFontFamily(fontFamily);
            }
        }

        private void OnBorderStyleChanged(object? sender, BorderStyle borderStyle)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SelectedBorderStyle = borderStyle;
                ApplySelectedBorderStyle(borderStyle);
            }
        }

        private void OnArrowStyleChanged(object? sender, ArrowStyle arrowStyle)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SelectedArrowStyle = arrowStyle;
                ApplySelectedArrowStyle(arrowStyle);
            }
        }

        private void OnCursorTypeChanged(object? sender, CursorType cursorType)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SelectedCursorType = cursorType;
                ApplySelectedCursorType(cursorType);
            }
        }

        private void OnStrengthChanged(object? sender, float strength)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.EffectStrength = strength;
                ApplySelectedEffectStrength(strength);
            }
        }

        private void OnSpotlightBlurChanged(object? sender, float blurAmount)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SpotlightBlur = blurAmount;
                ApplySelectedSpotlightBlur(blurAmount);
            }
        }

        private void OnShadowButtonClick(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.ShadowEnabled = !vm.ShadowEnabled;
                ApplySelectedShadowState(vm.ShadowEnabled);
            }
        }
        private void OnBoldButtonClick(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.TextBold = !vm.TextBold;
                ApplySelectedTextBold(vm.TextBold);
            }
        }

        private void OnItalicButtonClick(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.TextItalic = !vm.TextItalic;
                ApplySelectedTextItalic(vm.TextItalic);
            }
        }

        private void OnToolbarTextBoldChanged(object? sender, bool isBold)
        {
            ApplySelectedTextBold(isBold);
        }

        private void OnToolbarTextItalicChanged(object? sender, bool isItalic)
        {
            ApplySelectedTextItalic(isItalic);
        }

        private void OnToolbarShadowChanged(object? sender, bool isEnabled)
        {
            ApplySelectedShadowState(isEnabled);
        }

        private void OnToolbarShadowSettingsChanged(object? sender, EventArgs e)
        {
            ApplySelectedShadowSettings();
        }

        private void OnToolbarSpeechBalloonTailChanged(object? sender, bool isEnabled)
        {
            ApplySelectedSpeechBalloonTail(isEnabled);
        }

        private void OnToolbarEffectEllipseChanged(object? sender, bool isEllipse)
        {
            ApplySelectedEffectEllipse(isEllipse);
        }

        private void OnWidthChanged(object? sender, int width)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SetStrokeWidthCommand.Execute(width);
            }
        }

        private void OnCornerRadiusChanged(object? sender, int cornerRadius)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.CornerRadius = cornerRadius;
            }
        }

        private void OnZoomChanged(object? sender, double zoom)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.Zoom = zoom;
            }
        }

        private void OnZoomPickerZoomToFitRequested(object? sender, EventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.ZoomToFitCommand.Execute(null);
            }
        }

        private void OnSidebarScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            // TODO: Restore sidebar scrollbar overlay logic
        }

        private void OnToolbarScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            // TODO: Restore toolbar scrollbar overlay logic
        }

        // --- Restored from ref\3babd33_EditorView.axaml.cs lines 767-829 ---

        private void ApplySelectedColor(string colorHex)
        {
            var selected = _selectionController.SelectedShape;
            if (selected == null) return;

            // Ensure the annotation model property is updated so changes persist and effects render correctly
            if (selected.Tag is Annotation annotation)
            {
                annotation.StrokeColor = colorHex;
            }

            var brush = new SolidColorBrush(Color.Parse(colorHex));

            switch (selected)
            {
                case Shape shape:
                    shape.Stroke = brush;

                    if (shape is global::Avalonia.Controls.Shapes.Path path)
                    {
                        path.Fill = brush;
                    }
                    else if (selected is SpeechBalloonControl balloon)
                    {
                        balloon.InvalidateVisual();
                    }
                    break;
                case OutlinedTextControl textBox:
                    textBox.InvalidateVisual();
                    break;
                case SpeechBalloonControl balloonControl:
                    balloonControl.InvalidateVisual();
                    break;
                case StepControl stepControl:
                    stepControl.InvalidateVisual();
                    break;
            }

            // ISSUE-LIVE-UPDATE: Update active text editor if present
            _selectionController.UpdateActiveTextEditorProperties();
        }

        private void ApplySelectedFillColor(string colorHex)
        {
            var selected = _selectionController.SelectedShape;
            if (selected == null) return;

            // Ensure the annotation model property is updated so changes persist and effects render correctly
            if (selected.Tag is Annotation annotation)
            {
                annotation.FillColor = colorHex;
            }

            var brush = new SolidColorBrush(Color.Parse(colorHex));

            switch (selected)
            {
                case Shape shape:
                    if (shape.Tag is HighlightAnnotation)
                    {
                        OnRequestUpdateEffect(shape);
                        break;
                    }

                    if (shape is global::Avalonia.Controls.Shapes.Path path)
                    {
                        path.Fill = brush;
                    }
                    else if (shape is global::Avalonia.Controls.Shapes.Rectangle || shape is global::Avalonia.Controls.Shapes.Ellipse)
                    {
                        shape.Fill = brush;
                    }
                    break;
                case OutlinedTextControl textBox:
                    textBox.InvalidateVisual();
                    break;
                case SpeechBalloonControl balloonControl:
                    balloonControl.InvalidateVisual();
                    break;
                case StepControl stepControl:
                    stepControl.InvalidateVisual();
                    break;
            }

            // ISSUE-LIVE-UPDATE: Update active text editor if present
            _selectionController.UpdateActiveTextEditorProperties();
        }

        private void ApplySelectedTextColor(string colorHex)
        {
            var selected = _selectionController.SelectedShape;
            if (selected == null) return;

            if (selected.Tag is TextAnnotation textAnnotation)
            {
                textAnnotation.TextColor = colorHex;
            }
            else if (selected.Tag is SpeechBalloonAnnotation balloon)
            {
                balloon.TextColor = colorHex;
            }
            else if (selected.Tag is NumberAnnotation number)
            {
                number.TextColor = colorHex;
            }

            // Update UI
            if (selected is OutlinedTextControl outText)
            {
                outText.InvalidateVisual();
            }
            else if (selected is SpeechBalloonControl balloonControl)
            {
                balloonControl.InvalidateVisual();
            }
            else if (selected is StepControl stepControl)
            {
                stepControl.InvalidateVisual();
            }

            // ISSUE-LIVE-UPDATE: Update active text editor if present
            _selectionController.UpdateActiveTextEditorProperties();
        }

        private void ApplySelectedStrokeWidth(int width)
        {
            var selected = _selectionController.SelectedShape;
            if (selected == null) return;

            if (selected.Tag is Annotation annotation)
            {
                annotation.StrokeWidth = width;
            }

            if (selected.Tag is ArrowAnnotation arrowAnnotation
                && selected is global::Avalonia.Controls.Shapes.Path arrowPath)
            {
                AnnotationVisualFactory.UpdateVisualControl(arrowPath, arrowAnnotation);
                return;
            }

            switch (selected)
            {
                case Shape shape:
                    shape.StrokeThickness = width;
                    break;
                case OutlinedTextControl textBox:
                    textBox.InvalidateMeasure();
                    textBox.InvalidateVisual();
                    break;
                case SpeechBalloonControl balloon:
                    if (balloon.Annotation != null)
                    {
                        balloon.Annotation.StrokeWidth = width;
                        balloon.InvalidateVisual();
                    }
                    break;
                case StepControl stepControl:
                    if (stepControl.Annotation != null)
                    {
                        stepControl.Annotation.StrokeWidth = width;
                        stepControl.InvalidateVisual();
                    }
                    break;
            }
        }

        private void ApplySelectedCornerRadius(int cornerRadius)
        {
            var selected = _selectionController.SelectedShape;
            if (selected == null) return;

            int clampedRadius = Math.Max(0, cornerRadius);

            switch (selected.Tag)
            {
                case RectangleAnnotation rectangleAnnotation:
                    rectangleAnnotation.CornerRadius = clampedRadius;
                    if (selected is global::Avalonia.Controls.Shapes.Rectangle rectangle)
                    {
                        rectangle.RadiusX = clampedRadius;
                        rectangle.RadiusY = clampedRadius;
                    }
                    break;
                case SpeechBalloonAnnotation balloonAnnotation:
                    balloonAnnotation.CornerRadius = clampedRadius;
                    if (selected is SpeechBalloonControl balloonControl)
                    {
                        balloonControl.InvalidateVisual();
                    }
                    break;
            }
        }

        private void ApplySelectedFontSize(float fontSize)
        {
            var selected = _selectionController.SelectedShape;
            if (selected?.Tag is TextAnnotation textAnn)
            {
                textAnn.FontSize = fontSize;
                if (selected is OutlinedTextControl outlinedText)
                {
                    outlinedText.InvalidateMeasure();
                    outlinedText.InvalidateVisual();
                }
            }
            else if (selected?.Tag is NumberAnnotation numAnn)
            {
                numAnn.FontSize = fontSize;

                if (selected is StepControl stepControl)
                {
                    AnnotationVisualFactory.UpdateVisualControl(stepControl, numAnn);
                }
            }
            else if (selected?.Tag is SpeechBalloonAnnotation balloonAnn)
            {
                balloonAnn.FontSize = fontSize;
                if (selected is SpeechBalloonControl balloonControl)
                {
                    balloonControl.InvalidateVisual();
                }
                _selectionController.UpdateActiveTextEditorProperties();
            }
        }

        private void ApplySelectedTextHorizontalAlignment(TextHorizontalAlignment alignment)
        {
            var selected = _selectionController.SelectedShape;
            if (selected?.Tag is TextAnnotation textAnnotation)
            {
                textAnnotation.HorizontalAlignment = alignment;

                if (selected is OutlinedTextControl outlinedText)
                {
                    outlinedText.InvalidateVisual();
                }

                if (FindActiveTextEditor(textAnnotation) is TextBox textEditor)
                {
                    ApplyTextHorizontalAlignment(textEditor, alignment);
                }
            }
            else if (selected?.Tag is SpeechBalloonAnnotation balloonAnnotation)
            {
                balloonAnnotation.HorizontalAlignment = alignment;

                if (selected is SpeechBalloonControl balloonControl)
                {
                    balloonControl.InvalidateVisual();
                }

                if (FindActiveTextEditor(balloonAnnotation) is TextBox textEditor)
                {
                    ApplyTextHorizontalAlignment(textEditor, alignment);
                }
            }
        }

        private void ApplyStepTypeToAnnotations(StepType stepType)
        {
            bool hasSteps = false;

            foreach (var annotation in _editorCore.Annotations)
            {
                if (annotation is NumberAnnotation numberAnnotation)
                {
                    numberAnnotation.StepType = stepType;
                    hasSteps = true;
                }
            }

            if (!hasSteps)
            {
                return;
            }

            var canvas = this.FindControl<Canvas>("AnnotationCanvas");
            if (canvas == null)
            {
                return;
            }

            foreach (var child in canvas.Children)
            {
                if (child is StepControl stepControl && stepControl.Annotation != null)
                {
                    stepControl.Annotation.StepType = stepType;
                    AnnotationVisualFactory.UpdateVisualControl(stepControl, stepControl.Annotation);
                }
            }

            if (_selectionController.SelectedShape is StepControl)
            {
                _selectionController.UpdateSelectionHandles();
            }
        }

        private void ApplySelectedFontFamily(string fontFamily)
        {
            if (string.IsNullOrWhiteSpace(fontFamily))
            {
                return;
            }

            var selected = _selectionController.SelectedShape;

            if (selected?.Tag is TextAnnotation textAnnotation)
            {
                textAnnotation.FontFamily = fontFamily;

                if (selected is OutlinedTextControl outlinedText)
                {
                    outlinedText.InvalidateMeasure();
                    outlinedText.InvalidateVisual();
                }
            }
            else if (selected?.Tag is SpeechBalloonAnnotation balloonAnnotation)
            {
                balloonAnnotation.FontFamily = fontFamily;

                if (selected is SpeechBalloonControl balloonControl)
                {
                    balloonControl.InvalidateVisual();
                }

                _selectionController.UpdateActiveTextEditorProperties();
            }
        }

        private void ApplySelectedArrowStyle(ArrowStyle arrowStyle)
        {
            var selected = _selectionController.SelectedShape;

            if (selected?.Tag is ArrowAnnotation arrowAnnotation && selected is global::Avalonia.Controls.Shapes.Path arrowPath)
            {
                arrowAnnotation.Style = arrowStyle;
                AnnotationVisualFactory.UpdateVisualControl(arrowPath, arrowAnnotation);
                _selectionController.UpdateSelectionHandles();
            }
        }

        private void ApplySelectedBorderStyle(BorderStyle borderStyle)
        {
            var selected = _selectionController.SelectedShape;

            switch (selected?.Tag)
            {
                case RectangleAnnotation rectangleAnnotation when selected is global::Avalonia.Controls.Shapes.Rectangle rectangle:
                    rectangleAnnotation.BorderStyle = borderStyle;
                    rectangle.StrokeDashArray = BorderStyleDashHelper.CreateStrokeDashArray(borderStyle);
                    rectangle.StrokeLineCap = BorderStyleDashHelper.CreateStrokeLineCap(borderStyle);
                    break;
                case EllipseAnnotation ellipseAnnotation when selected is global::Avalonia.Controls.Shapes.Ellipse ellipse:
                    ellipseAnnotation.BorderStyle = borderStyle;
                    ellipse.StrokeDashArray = BorderStyleDashHelper.CreateStrokeDashArray(borderStyle);
                    ellipse.StrokeLineCap = BorderStyleDashHelper.CreateStrokeLineCap(borderStyle);
                    break;
                case LineAnnotation lineAnnotation when selected is global::Avalonia.Controls.Shapes.Path linePath:
                    lineAnnotation.BorderStyle = borderStyle;
                    linePath.StrokeDashArray = BorderStyleDashHelper.CreateStrokeDashArray(borderStyle);
                    linePath.StrokeLineCap = BorderStyleDashHelper.CreateStrokeLineCap(borderStyle);
                    break;
                case FreehandAnnotation freehandAnnotation when selected is global::Avalonia.Controls.Shapes.Path freehandPath:
                    freehandAnnotation.BorderStyle = borderStyle;
                    freehandPath.StrokeDashArray = BorderStyleDashHelper.CreateStrokeDashArray(borderStyle);
                    freehandPath.StrokeLineCap = BorderStyleDashHelper.CreateStrokeLineCap(borderStyle);
                    break;
            }
        }

        private void ApplySelectedCursorType(CursorType cursorType)
        {
            var selected = _selectionController.SelectedShape;

            if (selected?.Tag is CursorAnnotation cursorAnnotation && selected is Image cursorImage)
            {
                cursorAnnotation.CursorType = cursorType;

                var cursorBitmap = WindowsCursorBitmapRenderer.CreateAnnotationBitmap(cursorType);
                if (cursorBitmap != null)
                {
                    cursorAnnotation.SetImage(cursorBitmap);
                }

                AnnotationVisualFactory.UpdateVisualControl(cursorImage, cursorAnnotation);
                _selectionController.UpdateSelectionHandles();
            }
        }

        private void ApplySelectedEffectStrength(float strength)
        {
            var selected = _selectionController.SelectedShape;
            if (selected?.Tag is BaseEffectAnnotation effectAnn)
            {
                effectAnn.Amount = strength;
                OnRequestUpdateEffect(selected);
            }
            else if (selected?.Tag is SpotlightAnnotation spotlightAnn)
            {
                spotlightAnn.DarkenOpacity = (byte)Math.Clamp(
                    strength / MainViewModel.GetMaxEffectStrength(EditorTool.Spotlight) * 255,
                    0,
                    255);

                if (selected is SpotlightControl spotlightControl)
                {
                    RefreshSpotlightOverlay();
                }
            }
        }

        private void ApplySelectedSpotlightBlur(float blurAmount)
        {
            if (_selectionController.SelectedShape?.Tag is SpotlightAnnotation spotlightAnnotation)
            {
                spotlightAnnotation.BlurAmount = blurAmount;
                RefreshSpotlightOverlay();
            }
        }

        private void ApplySelectedEffectEllipse(bool isEllipse)
        {
            var selected = _selectionController.SelectedShape;

            if (selected?.Tag is MagnifyAnnotation magnifyAnnotation)
            {
                magnifyAnnotation.IsEllipse = isEllipse;
                selected.Clip = CreateEllipseClipIfNeeded(selected, isEllipse);
                OnRequestUpdateEffect(selected);
                _selectionController.UpdateSelectionHandles();
            }
            else if (selected?.Tag is SpotlightAnnotation spotlightAnnotation)
            {
                spotlightAnnotation.IsEllipse = isEllipse;
                RefreshSpotlightOverlay();
                _selectionController.UpdateSelectionHandles();
            }
        }

        private static Geometry? CreateEllipseClipIfNeeded(Control control, bool isEllipse)
        {
            if (!isEllipse)
            {
                return null;
            }

            double width = Math.Max(1, control.Width);
            double height = Math.Max(1, control.Height);
            if (double.IsNaN(width) || double.IsNaN(height))
            {
                width = Math.Max(1, control.Bounds.Width);
                height = Math.Max(1, control.Bounds.Height);
            }

            return new EllipseGeometry(new Rect(0, 0, width, height));
        }

        private void ApplySelectedShadowState(bool isEnabled)
        {
            var selected = _selectionController.SelectedShape;
            if (selected?.Tag is not Annotation annotation)
            {
                return;
            }

            annotation.ShadowEnabled = isEnabled;

            if (selected is Control control)
            {
                if (isEnabled)
                {
                    control.Effect = ShareX.ImageEditor.Presentation.Helpers.ShadowEffectHelper.CreateDropShadow(annotation);
                }
                else
                {
                    control.Effect = null;
                }
            }
        }

        private void ApplySelectedShadowSettings()
        {
            if (DataContext is not MainViewModel vm)
            {
                return;
            }

            var selected = _selectionController.SelectedShape;
            if (selected?.Tag is not Annotation annotation)
            {
                return;
            }

            annotation.ShadowColor = vm.Options.ShadowColorHex;
            annotation.ShadowBlurRadius = vm.Options.ShadowBlurRadius;
            annotation.ShadowOpacity = vm.Options.ShadowOpacity;
            annotation.ShadowOffsetX = vm.Options.ShadowOffsetX;
            annotation.ShadowOffsetY = vm.Options.ShadowOffsetY;

            if (selected is Control control && annotation.ShadowEnabled)
            {
                control.Effect = ShareX.ImageEditor.Presentation.Helpers.ShadowEffectHelper.CreateDropShadow(annotation);
            }
        }

        private void ApplySelectedSpeechBalloonTail(bool isEnabled)
        {
            if (_selectionController.SelectedShape is SpeechBalloonControl balloonControl &&
                balloonControl.Annotation is SpeechBalloonAnnotation balloonAnnotation)
            {
                balloonAnnotation.TailEnabled = isEnabled;
                balloonControl.InvalidateVisual();
                _selectionController.UpdateSelectionHandles();
                return;
            }

            if (_selectionController.SelectedShape is StepControl stepControl &&
                stepControl.Annotation is NumberAnnotation numberAnnotation)
            {
                numberAnnotation.TailEnabled = isEnabled;
                AnnotationVisualFactory.UpdateVisualControl(stepControl, numberAnnotation);
                _selectionController.UpdateSelectionHandles();
            }
        }

        private void ApplySelectedTextBold(bool isBold)
        {
            if (_selectionController.SelectedShape?.Tag is TextAnnotation textAnn)
            {
                textAnn.IsBold = isBold;
                if (_selectionController.SelectedShape is OutlinedTextControl outlinedText)
                {
                    outlinedText.InvalidateMeasure();
                    outlinedText.InvalidateVisual();
                }

                if (FindActiveTextEditor(textAnn) is TextBox textEditor)
                {
                    ApplyTextFontStyle(textEditor, textAnn.IsBold, textAnn.IsItalic);
                }
            }
            else if (_selectionController.SelectedShape?.Tag is SpeechBalloonAnnotation balloonAnnotation)
            {
                balloonAnnotation.IsBold = isBold;

                if (_selectionController.SelectedShape is SpeechBalloonControl balloonControl)
                {
                    balloonControl.InvalidateVisual();
                }

                if (FindActiveTextEditor(balloonAnnotation) is TextBox textEditor)
                {
                    ApplyTextFontStyle(textEditor, balloonAnnotation.IsBold, balloonAnnotation.IsItalic);
                }
            }
            else if (_selectionController.SelectedShape?.Tag is NumberAnnotation numberAnnotation)
            {
                numberAnnotation.IsBold = isBold;

                if (_selectionController.SelectedShape is StepControl stepControl)
                {
                    AnnotationVisualFactory.UpdateVisualControl(stepControl, numberAnnotation);
                    _selectionController.UpdateSelectionHandles();
                }
            }
        }

        private void ApplySelectedTextItalic(bool isItalic)
        {
            if (_selectionController.SelectedShape?.Tag is TextAnnotation textAnn)
            {
                textAnn.IsItalic = isItalic;
                if (_selectionController.SelectedShape is OutlinedTextControl outlinedText)
                {
                    outlinedText.InvalidateMeasure();
                    outlinedText.InvalidateVisual();
                }

                if (FindActiveTextEditor(textAnn) is TextBox textEditor)
                {
                    ApplyTextFontStyle(textEditor, textAnn.IsBold, textAnn.IsItalic);
                }
            }
            else if (_selectionController.SelectedShape?.Tag is SpeechBalloonAnnotation balloonAnnotation)
            {
                balloonAnnotation.IsItalic = isItalic;

                if (_selectionController.SelectedShape is SpeechBalloonControl balloonControl)
                {
                    balloonControl.InvalidateVisual();
                }

                if (FindActiveTextEditor(balloonAnnotation) is TextBox textEditor)
                {
                    ApplyTextFontStyle(textEditor, balloonAnnotation.IsBold, balloonAnnotation.IsItalic);
                }
            }
        }

        private static Color ApplyHighlightAlpha(Color baseColor)
        {
            return Color.FromArgb(0x55, baseColor.R, baseColor.G, baseColor.B);
        }

        private TextBox? FindActiveTextEditor(Annotation annotation)
        {
            var overlay = this.FindControl<Canvas>("OverlayCanvas");
            return overlay?.Children.OfType<TextBox>().FirstOrDefault(textBox => ReferenceEquals(textBox.Tag, annotation));
        }

        private static void ApplyTextHorizontalAlignment(TextBox textBox, TextHorizontalAlignment alignment)
        {
            textBox.TextAlignment = TextHorizontalAlignmentHelper.ToAvaloniaTextAlignment(alignment);
            textBox.HorizontalContentAlignment = TextHorizontalAlignmentHelper.ToHorizontalContentAlignment(alignment);
        }

        private static void ApplyTextFontStyle(TextBox textBox, bool isBold, bool isItalic)
        {
            textBox.FontWeight = isBold ? FontWeight.Bold : FontWeight.Normal;
            textBox.FontStyle = isItalic ? FontStyle.Italic : FontStyle.Normal;
        }
    }
}