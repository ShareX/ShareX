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

using ShareX.ImageEditor.Presentation.ViewModels;

namespace ShareX.ImageEditor.Presentation.Views
{
    public partial class EditorView
    {
        private MainViewModel? _subscribedViewModel;

        private void AttachViewModel(MainViewModel vm)
        {
            if (ReferenceEquals(_subscribedViewModel, vm)) return;

            DetachViewModel();
            _subscribedViewModel = vm;
            vm.AttachEditorCore(_editorCore);
            vm.DeleteRequested += OnDeleteRequested;
            vm.UndoRequested += OnUndoRequested;
            vm.RedoRequested += OnRedoRequested;
            vm.ClearAnnotationsRequested += OnClearAnnotationsRequested;
            vm.CutAnnotationRequested += OnCutRequested;
            vm.CopyAnnotationRequested += OnCopyRequested;
            vm.PasteRequested += OnPasteRequested;
            vm.DuplicateRequested += OnDuplicateRequested;
            vm.ZoomToFitRequested += OnZoomToFitRequested;
            vm.FlattenRequested += OnFlattenRequested;
            vm.ImageInsertionRequested += OnImageInsertionRequested;
            vm.EmojiInsertionRequested += OnEmojiInsertionRequested;
            vm.NewImageRequested += OnNewImageRequested;
            vm.OpenImageRequested += OnOpenImageRequested;
            vm.StartScreenRequested += OnStartScreenRequested;
            vm.LoadFromClipboardRequested += OnLoadFromClipboardRequested;
            vm.LoadFromUrlRequested += OnLoadFromUrlRequested;
            vm.LoadRecentFileRequested += OnLoadRecentFileRequested;
            vm.CopyRequested += OnCopyImageRequested;
            vm.SaveRequested += OnSaveRequested;
            vm.SaveAsRequested += OnSaveAsRequested;
            vm.FileMenuRequested += OnFileMenuRequested;
            vm.PropertyChanged += OnViewModelPropertyChanged;
            vm.DeselectRequested += OnDeselectRequested;
            vm.CanvasFocusRequested += OnCanvasFocusRequested;
        }

        private void DetachViewModel()
        {
            if (_subscribedViewModel is not { } vm) return;

            _subscribedViewModel = null;
            vm.DeleteRequested -= OnDeleteRequested;
            vm.UndoRequested -= OnUndoRequested;
            vm.RedoRequested -= OnRedoRequested;
            vm.ClearAnnotationsRequested -= OnClearAnnotationsRequested;
            vm.CutAnnotationRequested -= OnCutRequested;
            vm.CopyAnnotationRequested -= OnCopyRequested;
            vm.PasteRequested -= OnPasteRequested;
            vm.DuplicateRequested -= OnDuplicateRequested;
            vm.ZoomToFitRequested -= OnZoomToFitRequested;
            vm.FlattenRequested -= OnFlattenRequested;
            vm.ImageInsertionRequested -= OnImageInsertionRequested;
            vm.EmojiInsertionRequested -= OnEmojiInsertionRequested;
            vm.NewImageRequested -= OnNewImageRequested;
            vm.OpenImageRequested -= OnOpenImageRequested;
            vm.StartScreenRequested -= OnStartScreenRequested;
            vm.LoadFromClipboardRequested -= OnLoadFromClipboardRequested;
            vm.LoadFromUrlRequested -= OnLoadFromUrlRequested;
            vm.LoadRecentFileRequested -= OnLoadRecentFileRequested;
            vm.CopyRequested -= OnCopyImageRequested;
            vm.SaveRequested -= OnSaveRequested;
            vm.SaveAsRequested -= OnSaveAsRequested;
            vm.FileMenuRequested -= OnFileMenuRequested;
            vm.PropertyChanged -= OnViewModelPropertyChanged;
            vm.DeselectRequested -= OnDeselectRequested;
            vm.CanvasFocusRequested -= OnCanvasFocusRequested;
            vm.DetachEditorCore(_editorCore);
        }

        private void DetachParentWindow()
        {
            if (_parentWindow == null) return;

            _parentWindow.KeyDown -= OnKeyDown;
            _parentWindow.KeyUp -= OnKeyUp;
            _parentWindow.Activated -= OnWindowActivated;
            _parentWindow = null;
        }

        private void OnDeleteRequested(object? sender, EventArgs e) => PerformDelete();
        private void OnUndoRequested(object? sender, EventArgs e) => PerformUndo();
        private void OnRedoRequested(object? sender, EventArgs e) => PerformRedo();
        private void OnClearAnnotationsRequested(object? sender, EventArgs e) => ClearAllAnnotations();
    }
}
