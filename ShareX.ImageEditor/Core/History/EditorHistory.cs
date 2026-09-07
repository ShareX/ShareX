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

using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Core.Editor;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.History;

/// <summary>
/// Manages undo/redo history for the editor using the Memento pattern.
/// Adapted from ShareX's ImageEditorHistory implementation.
/// </summary>
internal class EditorHistory : IDisposable
{
    public bool CanUndo => _undoMementoStack.Count > 0;
    public bool CanRedo => _redoMementoStack.Count > 0;

    /// <summary>
    /// Maximum number of canvas mementos (destructive operations) to keep.
    /// Canvas mementos contain full bitmap copies, so their count is limited separately.
    /// </summary>
    private const int MaxCanvasMementos = 5;

    /// <summary>
    /// Maximum total number of mementos, including canvas mementos.
    /// </summary>
    private const int MaxMementos = 50;

    private readonly EditorCore _editorCore;
    private readonly Stack<EditorMemento> _undoMementoStack = new();
    private readonly Stack<EditorMemento> _redoMementoStack = new();

    public EditorHistory(EditorCore editorCore)
    {
        _editorCore = editorCore;
    }

    /// <summary>
    /// Add a memento to the undo stack and clear redo stack
    /// </summary>
    private void AddMemento(EditorMemento memento)
    {
        _undoMementoStack.Push(memento);
        TrimUndoHistory();
        ClearStack(_redoMementoStack);
    }

    private void TrimUndoHistory()
    {
        // Stack enumeration runs from newest to oldest. Keep a contiguous history
        // so undo never skips an operation whose state has been discarded.
        EditorMemento[] mementos = _undoMementoStack.ToArray();
        int keepCount = 0;
        int canvasCount = 0;

        while (keepCount < mementos.Length && keepCount < MaxMementos)
        {
            if (mementos[keepCount].Canvas != null && ++canvasCount > MaxCanvasMementos)
            {
                break;
            }

            keepCount++;
        }

        if (keepCount == mementos.Length)
        {
            return;
        }

        _undoMementoStack.Clear();

        for (int i = keepCount - 1; i >= 0; i--)
        {
            _undoMementoStack.Push(mementos[i]);
        }

        for (int i = keepCount; i < mementos.Length; i++)
        {
            mementos[i].Dispose();
        }
    }

    /// <summary>
    /// Create a memento with full canvas bitmap (for destructive operations like crop/cutout)
    /// </summary>
    private EditorMemento GetMementoFromCanvas()
    {
        List<Annotation> annotations = _editorCore.GetAnnotationsSnapshot();
        SKBitmap? canvas = _editorCore.SourceImage?.Copy();
        Guid? selectedId = _editorCore.SelectedAnnotation?.Id;
        return new EditorMemento(annotations, _editorCore.CanvasSize, canvas, selectedId);
    }

    /// <summary>
    /// Create a memento with only annotations (for non-destructive annotation operations)
    /// </summary>
    private EditorMemento GetMementoFromAnnotations(Annotation? excludeAnnotation = null)
    {
        List<Annotation> annotations = _editorCore.GetAnnotationsSnapshot(excludeAnnotation);
        Guid? selectedId = _editorCore.SelectedAnnotation?.Id;
        return new EditorMemento(annotations, _editorCore.CanvasSize, null, selectedId);
    }

    /// <summary>
    /// Create a canvas memento before destructive operations (crop, cutout)
    /// </summary>
    public void CreateCanvasMemento()
    {
        EditorMemento memento = GetMementoFromCanvas();
        AddMemento(memento);
    }

    /// <summary>
    /// Create an annotations-only memento for non-destructive operations.
    /// Excludes crop while its region is still being drawn.
    /// </summary>
    /// <param name="excludeAnnotation">Optional annotation to exclude from the memento (to capture state before it was added)</param>
    /// <param name="force">Force memento creation regardless of active tool</param>
    public void CreateAnnotationsMemento(Annotation? excludeAnnotation = null, bool force = false)
    {
        // Skip memento creation for crop while its region is being drawn.
        // Crop commits through canvas history when confirmed.
        if (!force &&
            _editorCore.ActiveTool == EditorTool.Crop)
        {
            return;
        }

        EditorMemento memento = GetMementoFromAnnotations(excludeAnnotation);
        AddMemento(memento);
    }

    /// <summary>
    /// Undo the last operation
    /// </summary>
    public void Undo()
    {
        RestoreMemento(_undoMementoStack, _redoMementoStack);
    }

    /// <summary>
    /// Redo the last undone operation
    /// </summary>
    public void Redo()
    {
        RestoreMemento(_redoMementoStack, _undoMementoStack);
    }

    private void RestoreMemento(Stack<EditorMemento> source, Stack<EditorMemento> destination)
    {
        if (!source.TryPop(out EditorMemento? memento))
        {
            return;
        }

        using (memento)
        {
            destination.Push(memento.Canvas == null ? GetMementoFromAnnotations() : GetMementoFromCanvas());
            _editorCore.RestoreState(memento);
        }
    }

    /// <summary>
    /// Clear all history and dispose resources
    /// </summary>
    public void Dispose()
    {
        ClearStack(_undoMementoStack);
        ClearStack(_redoMementoStack);
    }

    private static void ClearStack(Stack<EditorMemento> stack)
    {
        while (stack.TryPop(out EditorMemento? memento))
        {
            memento.Dispose();
        }
    }
}