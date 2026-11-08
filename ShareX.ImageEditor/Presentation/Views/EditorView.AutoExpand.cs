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
using Avalonia.Threading;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Presentation.ViewModels;
using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.Views
{
    /// <summary>
    /// Automatic canvas expansion: when text or a shape goes past the image edge,
    /// the canvas grows so nothing is cut off (similar to Yandex Screenshots).
    /// </summary>
    public partial class EditorView
    {
        /// <summary>Free space left between the annotation and the new canvas edge.</summary>
        private const float AutoExpandMargin = 16f;

        /// <summary>While typing, grow in steps so the bitmap is not reallocated on every key press.</summary>
        private const int LiveExpandStep = 150;

        private readonly HashSet<Annotation> _pendingAutoExpandAnnotations = new();
        private bool _autoExpandScheduled;
        private bool _suppressScrollResetForAutoExpand;

        private bool IsAutoExpandCanvasEnabled =>
            DataContext is MainViewModel vm && vm.Options != null && vm.Options.AutoExpandCanvas;

        private SKColor AutoExpandCanvasColor
        {
            get
            {
                string? hex = (DataContext as MainViewModel)?.Options?.CanvasFillColorHex;
                return !string.IsNullOrWhiteSpace(hex) && SKColor.TryParse(hex, out SKColor color) ? color : SKColors.Transparent;
            }
        }

        /// <summary>
        /// Queue a check for the annotation that was just drawn, moved or resized.
        /// Several requests in the same UI turn are merged into one expansion (one undo step).
        /// </summary>
        internal void ScheduleAutoExpandCanvas(Annotation? annotation)
        {
            if (annotation == null || !IsAutoExpandCanvasEnabled || !Core.Editor.EditorCore.CanAutoExpandFor(annotation))
            {
                return;
            }

            _pendingAutoExpandAnnotations.Add(annotation);

            if (_autoExpandScheduled)
            {
                return;
            }

            _autoExpandScheduled = true;
            Dispatcher.UIThread.Post(RunScheduledAutoExpand, DispatcherPriority.Background);
        }

        private void RunScheduledAutoExpand()
        {
            _autoExpandScheduled = false;
            var targets = _pendingAutoExpandAnnotations.ToList();
            _pendingAutoExpandAnnotations.Clear();

            if (_workspaceDisposed || targets.Count == 0)
            {
                return;
            }

            // Text is still being typed: the live expansion handles it, the final fit happens on commit.
            if (_editorCore.IsLiveCanvasExpansionActive)
            {
                return;
            }

            ExpandCanvasToFit(targets);
        }

        private bool ExpandCanvasToFit(IReadOnlyCollection<Annotation> targets)
        {
            if (!IsAutoExpandCanvasEnabled || targets.Count == 0)
            {
                return false;
            }

            Guid? selectedId = (_selectionController.SelectedShape?.Tag as Annotation)?.Id;
            ScrollViewer? scrollViewer = this.FindControl<ScrollViewer>("CanvasScrollViewer");
            Vector scrollOffset = scrollViewer?.Offset ?? default;

            _suppressScrollResetForAutoExpand = true;

            bool expanded = _editorCore.AutoExpandCanvasToFit(targets, AutoExpandMargin, AutoExpandCanvasColor, out int addedLeft, out int addedTop);

            if (!expanded)
            {
                _suppressScrollResetForAutoExpand = false;
                return false;
            }

            bool shifted = addedLeft > 0 || addedTop > 0;

            // Runs after the posted ImageChanged / AnnotationsRestored handlers.
            Dispatcher.UIThread.Post(() =>
            {
                _suppressScrollResetForAutoExpand = false;

                if (_workspaceDisposed)
                {
                    return;
                }

                if (shifted)
                {
                    // Annotation controls were rebuilt: restore the selection.
                    if (selectedId.HasValue)
                    {
                        ReselectAnnotation(selectedId.Value);
                    }

                    // Content moved right/down: scroll by the same amount so the view does not jump.
                    if (scrollViewer != null)
                    {
                        double zoom = DataContext is MainViewModel vm && vm.Zoom > 0 ? vm.Zoom : 1.0;
                        scrollViewer.Offset = new Vector(scrollOffset.X + addedLeft * zoom, scrollOffset.Y + addedTop * zoom);
                    }
                }
                else
                {
                    _selectionController.UpdateSelectionHandles();
                }
            }, DispatcherPriority.Background);

            return true;
        }

        private void ReselectAnnotation(Guid id)
        {
            var canvas = this.FindControl<Canvas>("AnnotationCanvas");
            if (canvas == null) return;

            foreach (var child in canvas.Children)
            {
                if (child is Control control && control.Tag is Annotation annotation && annotation.Id == id)
                {
                    _selectionController.SetSelectedShape(control);
                    return;
                }
            }
        }

        /// <summary>
        /// Called while a new text box is being typed. Grows the canvas right/down so the text stays visible.
        /// Not recorded in history; <see cref="FinishLiveTextExpansion"/> replaces it with one undoable step.
        /// </summary>
        internal void OnLiveTextEditorBoundsChanged(TextBox textBox)
        {
            if (!IsAutoExpandCanvasEnabled || _editorCore.SourceImage == null || textBox.Parent == null)
            {
                return;
            }

            double left = Canvas.GetLeft(textBox);
            double top = Canvas.GetTop(textBox);
            if (double.IsNaN(left) || double.IsNaN(top))
            {
                return;
            }

            double right = left + textBox.Bounds.Width;
            double bottom = top + textBox.Bounds.Height;
            float width = _editorCore.CanvasSize.Width;
            float height = _editorCore.CanvasSize.Height;

            int addRight = right > width ? (int)Math.Ceiling(right - width) + LiveExpandStep : 0;
            int addBottom = bottom > height ? (int)Math.Ceiling(bottom - height) + (int)AutoExpandMargin : 0;

            if (addRight == 0 && addBottom == 0)
            {
                return;
            }

            _suppressScrollResetForAutoExpand = true;
            _editorCore.ExpandCanvasLive(addRight, addBottom, AutoExpandCanvasColor);
            Dispatcher.UIThread.Post(() => _suppressScrollResetForAutoExpand = false, DispatcherPriority.Background);
        }

        /// <summary>
        /// Called when typing of a new text ends. Drops the temporary expansion and,
        /// if the text was kept, grows the canvas to fit it exactly as one undoable step.
        /// </summary>
        internal void FinishLiveTextExpansion(Annotation? committedAnnotation)
        {
            bool hadLiveExpansion = _editorCore.EndLiveCanvasExpansion(notify: false);

            bool expanded = committedAnnotation != null && ExpandCanvasToFit(new[] { committedAnnotation });

            if (!expanded && hadLiveExpansion)
            {
                _suppressScrollResetForAutoExpand = true;
                _editorCore.NotifyImageReplaced();
                Dispatcher.UIThread.Post(() => _suppressScrollResetForAutoExpand = false, DispatcherPriority.Background);
            }
        }
    }
}
