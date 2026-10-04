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

namespace ShareX.ImageEditor.Presentation.Controllers;

/// <summary>Commits a cut before waiting for the toolkit clipboard, leaving no delayed deletion behind.</summary>
internal static class EditorAnnotationCutController
{
    public static async Task<bool> CutAsync(EditorCore core, Annotation annotation,
        EditorImageOperationLifetime.Operation operation, Action<Annotation> publishClipboard,
        Action<Annotation> removeVisual, Func<Task> clearClipboard, Action<Exception> reportFailure)
    {
        bool CanCut() => operation.IsCurrent && core.Annotations.Contains(annotation) &&
            annotation is not ImageAnnotation { IsDisposed: true };
        if (!CanCut()) return false;

        Annotation? copy = annotation.Clone();
        try
        {
            publishClipboard(copy);
            copy = null; // The annotation clipboard owns the clone after successful publication.
        }
        finally
        {
            (copy as IDisposable)?.Dispose();
        }
        if (!CanCut()) return false;

        // RemoveAnnotation snapshots undo state and targets this object, independent of later selection.
        try
        {
            core.RemoveAnnotation(annotation);
        }
        finally
        {
            if (!core.Annotations.Contains(annotation))
            {
                try { if (operation.IsCurrent) removeVisual(annotation); }
                finally { (annotation as IDisposable)?.Dispose(); }
            }
        }

        if (operation.IsCurrent)
        {
            Task? pendingClear = null;
            try
            {
                pendingClear = clearClipboard();
                await pendingClear.WaitAsync(operation.CancellationToken);
            }
            catch (OperationCanceledException) when (operation.CancellationToken.IsCancellationRequested)
            {
                // ClearAsync has no cancellation API; release the editor's wait and observe any late failure without UI.
                if (pendingClear != null) _ = ObserveLateClearAsync(pendingClear);
            }
            catch (Exception ex)
            {
                if (operation.IsCurrent) reportFailure(ex);
            }
        }
        return true;
    }

    private static async Task ObserveLateClearAsync(Task pending)
    {
        try { await pending.ConfigureAwait(false); }
        catch { } // The editor has left; there is no current owner to receive this clipboard error.
    }
}