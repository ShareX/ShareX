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
using SkiaSharp;

namespace ShareX.ImageEditor.Core.History;

/// <summary>
/// Memento pattern implementation for storing editor state.
/// Stores annotation list, canvas size, and optionally a full canvas bitmap.
/// </summary>
internal class EditorMemento : IDisposable
{
    /// <summary>
    /// Duplicated list of annotations at this state
    /// </summary>
    public List<Annotation> Annotations { get; }

    /// <summary>
    /// Canvas size at this state
    /// </summary>
    public SKSize CanvasSize { get; }

    /// <summary>
    /// Optional full canvas bitmap (for destructive operations like crop/cutout)
    /// </summary>
    public SKBitmap? Canvas { get; private set; }

    /// <summary>
    /// ID of the selected annotation at this state (for undo/redo selection restoration)
    /// </summary>
    public Guid? SelectedAnnotationId { get; }

    /// <summary>
    /// Create a new memento
    /// </summary>
    /// <param name="annotations">Snapshot of annotations owned by this memento</param>
    /// <param name="canvasSize">Canvas size</param>
    /// <param name="canvas">Optional canvas bitmap for destructive operations</param>
    /// <param name="selectedAnnotationId">Optional selected annotation ID for selection restoration</param>
    public EditorMemento(List<Annotation> annotations, SKSize canvasSize, SKBitmap? canvas = null, Guid? selectedAnnotationId = null)
    {
        Annotations = annotations;
        CanvasSize = canvasSize;
        Canvas = canvas;
        SelectedAnnotationId = selectedAnnotationId;
    }

    public void Dispose()
    {
        // Restoring transfers the annotation references to the editor's own list.
        // Clear this list without disposing annotations that may now be in use.
        Annotations.Clear();

        Canvas?.Dispose();
        Canvas = null;
    }
}