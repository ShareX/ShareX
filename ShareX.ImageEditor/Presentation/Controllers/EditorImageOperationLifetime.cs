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

namespace ShareX.ImageEditor.Presentation.Controllers;

/// <summary>Owns cancellable image operations until they complete, even when the editor unloads or changes documents.</summary>
internal sealed class EditorImageOperationLifetime
{
    private readonly object sync = new();
    private readonly HashSet<Operation> operations = new();
    private int generation;
    private bool closed;

    public Operation Begin(Func<bool> contextIsCurrent)
    {
        lock (sync)
        {
            Operation operation = new(this, generation, contextIsCurrent);
            operations.Add(operation);
            return operation;
        }
    }

    public void Invalidate() => EndOperations(close: false);
    public void Close() => EndOperations(close: true);

    private void EndOperations(bool close)
    {
        Operation[] pending;
        lock (sync)
        {
            closed |= close;
            generation++;
            pending = operations.ToArray();
        }
        foreach (Operation operation in pending) operation.Cancel();
    }

    private bool IsCurrent(int version)
    {
        lock (sync) return !closed && generation == version;
    }

    private void Remove(Operation operation)
    {
        lock (sync) operations.Remove(operation);
    }

    internal sealed class Operation : IDisposable
    {
        private readonly EditorImageOperationLifetime owner;
        private readonly int version;
        private readonly Func<bool> contextIsCurrent;
        private readonly CancellationTokenSource cancellation = new();
        private readonly object sync = new();
        private bool disposed;
        public CancellationToken CancellationToken { get; }
        public bool IsCurrent => !Volatile.Read(ref disposed) && owner.IsCurrent(version) && contextIsCurrent();

        internal Operation(EditorImageOperationLifetime owner, int version, Func<bool> contextIsCurrent)
        {
            this.owner = owner;
            this.version = version;
            this.contextIsCurrent = contextIsCurrent;
            CancellationToken = cancellation.Token;
        }

        public async Task<bool> RunOwnedAsync(Func<CancellationToken, Task<EditorImportedImage?>> acquire,
            Func<EditorImportedImage, Task<bool>> publish)
        {
            bool publishing = false;
            try
            {
                if (!IsCurrent) return false;
                using EditorImportedImage? image = await acquire(CancellationToken);
                if (!IsCurrent || image == null) return false;
                publishing = true;
                return await publish(image);
            }
            catch (OperationCanceledException) when (CancellationToken.IsCancellationRequested)
            {
                return false;
            }
            catch (Exception) when (!publishing && !IsCurrent)
            {
                // A picker/clipboard provider may fail while returning after its owner has left.
                return false;
            }
        }

        internal void Cancel()
        {
            lock (sync) if (!disposed) cancellation.Cancel();
        }

        public async Task<bool> PublishAfterAsync(EditorImportedImage image, Func<Task> wait,
            Func<EditorImportedImage, bool> publish)
        {
            if (!IsCurrent) return false;
            await wait().WaitAsync(CancellationToken);
            return IsCurrent && publish(image);
        }

        public void Dispose()
        {
            lock (sync)
            {
                if (disposed) return;
                disposed = true;
                cancellation.Dispose();
            }
            owner.Remove(this);
        }
    }
}