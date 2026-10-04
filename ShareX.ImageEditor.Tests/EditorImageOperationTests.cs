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
using ShareX.ImageEditor.Presentation.Controllers;
using SkiaSharp;
using Xunit;

namespace ShareX.ImageEditor.Tests;

/// <summary>B40: delayed import providers must release their images without changing a different or closed document.</summary>
public sealed class EditorImageOperationTests
{
    [Fact]
    public async Task ClosedEditorDoesNotOpenProvider()
    {
        var lifetime = new EditorImageOperationLifetime();
        lifetime.Close();
        using var operation = lifetime.Begin(() => true);
        bool opened = false;
        Assert.False(await operation.RunOwnedAsync(_ =>
        {
            opened = true;
            return Task.FromResult<EditorImportedImage?>(null);
        }, _ => throw new InvalidOperationException("Closed editor published an image.")));
        Assert.False(opened);
    }

    [Theory]
    [InlineData("close")]
    [InlineData("unload")]
    [InlineData("document")]
    [InlineData("owner")]
    public async Task LateImageDoesNotReplaceCurrentDocument(string transition)
    {
        var lifetime = new EditorImageOperationLifetime();
        object originalOwner = new();
        object owner = originalOwner;
        using var core = new EditorCore();
        var original = Bitmap(SKColors.Blue);
        core.LoadImage(original);
        using var operation = lifetime.Begin(() => ReferenceEquals(owner, originalOwner));
        var provider = Pending<EditorImportedImage?>();
        Task<bool> import = operation.RunOwnedAsync(_ => provider.Task, image =>
        {
            core.LoadImage(image.TakeBitmap());
            return Task.FromResult(true);
        });
        if (transition == "close") lifetime.Close();
        else if (transition == "owner") owner = new object();
        else lifetime.Invalidate();
        var incoming = Bitmap(SKColors.Red);
        provider.SetResult(new EditorImportedImage(incoming));
        Assert.False(await import.WaitAsync(TimeSpan.FromSeconds(5)));
        Assert.Equal(IntPtr.Zero, incoming.Handle);
        Assert.Same(original, core.SourceImage);
        Assert.Equal(SKColors.Blue, core.SourceImage!.GetPixel(0, 0));
        Assert.Empty(core.Annotations);
    }

    [Fact]
    public async Task NewDocumentCanImportAfterPreviousOperationWasInvalidated()
    {
        var lifetime = new EditorImageOperationLifetime();
        using var previous = lifetime.Begin(() => true);
        var provider = Pending<EditorImportedImage?>();
        Task<bool> oldImport = previous.RunOwnedAsync(_ => provider.Task, _ => throw new InvalidOperationException());
        lifetime.Invalidate();
        using var current = lifetime.Begin(() => true);
        using var core = new EditorCore();
        var accepted = Bitmap(SKColors.Green);
        Assert.True(await current.RunOwnedAsync(_ => Task.FromResult<EditorImportedImage?>(new(accepted)), image =>
        {
            core.LoadImage(image.TakeBitmap());
            return Task.FromResult(true);
        }));
        var rejected = Bitmap(SKColors.Red);
        provider.SetResult(new EditorImportedImage(rejected));
        Assert.False(await oldImport);
        Assert.Equal(IntPtr.Zero, rejected.Handle);
        Assert.Same(accepted, core.SourceImage);
        Assert.NotEqual(IntPtr.Zero, accepted.Handle);
    }

    [Fact]
    public void InvalidatingClosedEditorCannotReopenIt()
    {
        var lifetime = new EditorImageOperationLifetime();
        lifetime.Close();
        lifetime.Invalidate();
        using var operation = lifetime.Begin(() => true);
        Assert.False(operation.IsCurrent);
    }

    [Fact]
    public async Task AcceptedNewDocumentInvalidatesOtherImportsWithoutDisposingItsOwnImage()
    {
        var lifetime = new EditorImageOperationLifetime();
        using var other = lifetime.Begin(() => true);
        var provider = Pending<EditorImportedImage?>();
        Task<bool> previous = other.RunOwnedAsync(_ => provider.Task, _ => throw new InvalidOperationException());
        using var acceptedOperation = lifetime.Begin(() => true);
        using var core = new EditorCore();
        var accepted = Bitmap(SKColors.Green);
        Assert.True(await acceptedOperation.RunOwnedAsync(_ => Task.FromResult<EditorImportedImage?>(new(accepted)), image =>
        {
            lifetime.Invalidate();
            core.LoadImage(image.TakeBitmap());
            return Task.FromResult(true);
        }));
        var stale = Bitmap(SKColors.Red);
        provider.SetResult(new EditorImportedImage(stale));
        Assert.False(await previous);
        Assert.Equal(IntPtr.Zero, stale.Handle);
        Assert.Equal(SKColors.Green, core.SourceImage!.GetPixel(0, 0));
        Assert.True(acceptedOperation.CancellationToken.IsCancellationRequested);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task StreamOpeningAfterCloseOrOwnerChangeIsDisposedWithoutBeingRead(bool ownerChanged)
    {
        var lifetime = new EditorImageOperationLifetime();
        bool current = true;
        using var operation = lifetime.Begin(() => current);
        var provider = Pending<Stream>();
        using var stream = new ImportStream(Png());
        Task<bool> import = operation.RunOwnedAsync(_ => EditorImageFileReader.ReadAsync(() => provider.Task, operation, null),
            _ => throw new InvalidOperationException());
        if (ownerChanged) current = false;
        else lifetime.Close();
        provider.SetResult(stream);
        Assert.False(await import.WaitAsync(TimeSpan.FromSeconds(5)));
        Assert.True(stream.Disposed);
        Assert.Equal(0, stream.Reads);
    }

    [Fact]
    public async Task CloseCancelsBlockedStreamReadAndReleasesStream()
    {
        var lifetime = new EditorImageOperationLifetime();
        using var operation = lifetime.Begin(() => true);
        using var stream = new ImportStream(Png(), block: true);
        Task<bool> import = operation.RunOwnedAsync(_ => EditorImageFileReader.ReadAsync(
            () => Task.FromResult<Stream>(stream), operation, null), _ => throw new InvalidOperationException());
        await stream.ReadStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        lifetime.Close();
        Assert.False(await import.WaitAsync(TimeSpan.FromSeconds(5)));
        Assert.True(stream.Disposed);
        Assert.Equal(1, stream.Reads);
    }

    [Fact]
    public async Task InvalidImageReleasesStreamAndIsNotPublished()
    {
        var lifetime = new EditorImageOperationLifetime();
        using var operation = lifetime.Begin(() => true);
        using var stream = new ImportStream([1, 2, 3, 4]);
        Assert.False(await operation.RunOwnedAsync(_ => EditorImageFileReader.ReadAsync(
            () => Task.FromResult<Stream>(stream), operation, null), _ => throw new InvalidOperationException()));
        Assert.True(stream.Disposed);
    }

    [Fact]
    public async Task DownloadTimeoutCancelsBodyReadAndIsReportedWhileEditorIsOpen()
    {
        var lifetime = new EditorImageOperationLifetime();
        using var operation = lifetime.Begin(() => true);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(operation.CancellationToken);
        using var stream = new ImportStream(Png(), block: true);
        Task<bool> import = operation.RunOwnedAsync(_ => EditorImageFileReader.ReadAsync(
            () => Task.FromResult<Stream>(stream), operation, null, timeout.Token), _ => Task.FromResult(true));
        await stream.ReadStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        timeout.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => import.WaitAsync(TimeSpan.FromSeconds(5)));
        Assert.True(operation.IsCurrent);
        Assert.True(stream.Disposed);
    }

    [Fact]
    public async Task FileReaderPreservesPixelsAndPortableSourcePath()
    {
        var lifetime = new EditorImageOperationLifetime();
        using var operation = lifetime.Begin(() => true);
        using var stream = new ImportStream(Png());
        string path = Path.Combine(Path.GetTempPath(), "synthetic-import.png");
        using var image = await EditorImageFileReader.ReadAsync(() => Task.FromResult<Stream>(stream), operation, path);
        Assert.NotNull(image);
        Assert.True(stream.Disposed);
        Assert.Equal(path, image.SourceFilePath);
        Assert.Equal(SKColors.Red, image.Bitmap.GetPixel(0, 0));
        Assert.Equal(2, image.Bitmap.Width);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RejectedOrFailedPlacementReleasesUntransferredImage(bool fail)
    {
        var lifetime = new EditorImageOperationLifetime();
        using var operation = lifetime.Begin(() => true);
        var bitmap = Bitmap(SKColors.Red);
        Task<bool> import = operation.RunOwnedAsync(_ => Task.FromResult<EditorImportedImage?>(new(bitmap)), _ =>
            fail ? throw new InvalidOperationException("Visual construction failed.") : Task.FromResult(false));
        if (fail) await Assert.ThrowsAsync<InvalidOperationException>(() => import);
        else Assert.False(await import);
        Assert.Equal(IntPtr.Zero, bitmap.Handle);
    }

    [Fact]
    public async Task AcceptedBitmapBelongsToCoreUntilCoreIsDisposed()
    {
        var lifetime = new EditorImageOperationLifetime();
        using var operation = lifetime.Begin(() => true);
        using var core = new EditorCore();
        var bitmap = Bitmap(SKColors.Red);
        Assert.True(await operation.RunOwnedAsync(_ => Task.FromResult<EditorImportedImage?>(new(bitmap)), image =>
        {
            core.LoadImage(image.TakeBitmap());
            return Task.FromResult(true);
        }));
        Assert.Equal(SKColors.Red, core.SourceImage!.GetPixel(0, 0));
        core.Dispose();
        Assert.Equal(IntPtr.Zero, bitmap.Handle);
    }

    [Fact]
    public async Task NotificationFailureAfterTransferDoesNotDisposeCoreImage()
    {
        var lifetime = new EditorImageOperationLifetime();
        using var operation = lifetime.Begin(() => true);
        using var core = new EditorCore();
        var bitmap = Bitmap(SKColors.Red);
        await Assert.ThrowsAsync<InvalidOperationException>(() => operation.RunOwnedAsync(
            _ => Task.FromResult<EditorImportedImage?>(new(bitmap)), image =>
            {
                core.LoadImage(image.TakeBitmap());
                throw new InvalidOperationException("Notification failed after acceptance.");
            }));
        Assert.Equal(SKColors.Red, core.SourceImage!.GetPixel(0, 0));
        Assert.NotEqual(IntPtr.Zero, bitmap.Handle);
    }

    [Fact]
    public async Task CloseReleasesPlacementWaitEvenIfDispatcherNeverRuns()
    {
        var lifetime = new EditorImageOperationLifetime();
        using var operation = lifetime.Begin(() => true);
        var bitmap = Bitmap(SKColors.Red);
        var dispatcher = Pending<bool>();
        bool published = false;
        Task<bool> import = operation.RunOwnedAsync(_ => Task.FromResult<EditorImportedImage?>(new(bitmap)), image =>
            operation.PublishAfterAsync(image, () => dispatcher.Task, _ => published = true));
        lifetime.Close();
        Assert.False(await import.WaitAsync(TimeSpan.FromSeconds(5)));
        Assert.False(published);
        Assert.False(dispatcher.Task.IsCompleted);
        Assert.Equal(IntPtr.Zero, bitmap.Handle);
    }

    [Fact]
    public async Task OwnerChangeDuringPlacementWaitRejectsInsertion()
    {
        var lifetime = new EditorImageOperationLifetime();
        bool current = true;
        using var operation = lifetime.Begin(() => current);
        var dispatcher = Pending<bool>();
        var bitmap = Bitmap(SKColors.Red);
        Task<bool> import = operation.RunOwnedAsync(_ => Task.FromResult<EditorImportedImage?>(new(bitmap)), image =>
            operation.PublishAfterAsync(image, () => dispatcher.Task, _ => throw new InvalidOperationException()));
        current = false;
        dispatcher.SetResult(true);
        Assert.False(await import);
        Assert.Equal(IntPtr.Zero, bitmap.Handle);
    }

    [Fact]
    public async Task PlacementPublishesAnnotationAfterWaitAndTransfersItsImage()
    {
        var lifetime = new EditorImageOperationLifetime();
        using var operation = lifetime.Begin(() => true);
        using var core = new EditorCore();
        core.LoadImage(Bitmap(SKColors.Blue));
        var dispatcher = Pending<bool>();
        var bitmap = Bitmap(SKColors.Red);
        Task<bool> import = operation.RunOwnedAsync(_ => Task.FromResult<EditorImportedImage?>(new(bitmap)), image =>
            operation.PublishAfterAsync(image, () => dispatcher.Task, owned =>
            {
                var annotation = new ImageAnnotation();
                annotation.SetImage(owned.TakeBitmap());
                core.AddAnnotation(annotation);
                return true;
            }));
        Assert.Empty(core.Annotations);
        dispatcher.SetResult(true);
        Assert.True(await import.WaitAsync(TimeSpan.FromSeconds(5)));
        var inserted = Assert.IsType<ImageAnnotation>(Assert.Single(core.Annotations));
        Assert.Equal(SKColors.Red, inserted.ImageBitmap!.GetPixel(0, 0));
        core.Dispose();
        Assert.Equal(IntPtr.Zero, bitmap.Handle);
    }

    [Fact]
    public async Task RemovedReplacementTargetCannotBeResurrectedByPicker()
    {
        var lifetime = new EditorImageOperationLifetime();
        using var core = new EditorCore();
        using var annotation = new ImageAnnotation();
        core.AddAnnotation(annotation);
        using var operation = lifetime.Begin(() => !annotation.IsDisposed && core.Annotations.Contains(annotation));
        var provider = Pending<EditorImportedImage?>();
        Task<bool> import = operation.RunOwnedAsync(_ => provider.Task, image =>
        {
            annotation.SetImage(image.TakeBitmap());
            return Task.FromResult(true);
        });
        core.ClearAll();
        annotation.Dispose();
        var bitmap = Bitmap(SKColors.Red);
        provider.SetResult(new EditorImportedImage(bitmap));
        Assert.False(await import);
        Assert.Null(annotation.ImageBitmap);
        Assert.Equal(IntPtr.Zero, bitmap.Handle);
    }

    [Fact]
    public async Task ValidReplacementTransfersBitmapAndReleasesPreviousImage()
    {
        var lifetime = new EditorImageOperationLifetime();
        using var annotation = new ImageAnnotation();
        var previous = Bitmap(SKColors.Blue);
        annotation.SetImage(previous);
        using var operation = lifetime.Begin(() => !annotation.IsDisposed);
        var replacement = Bitmap(SKColors.Red);
        Assert.True(await operation.RunOwnedAsync(_ => Task.FromResult<EditorImportedImage?>(new(replacement)), image =>
        {
            annotation.SetImage(image.TakeBitmap());
            return Task.FromResult(true);
        }));
        Assert.Equal(IntPtr.Zero, previous.Handle);
        Assert.Equal(SKColors.Red, annotation.ImageBitmap!.GetPixel(0, 0));
        annotation.Dispose();
        Assert.Equal(IntPtr.Zero, replacement.Handle);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ProviderFailureIsSuppressedOnlyWhenItsContextIsStale(bool stale)
    {
        var lifetime = new EditorImageOperationLifetime();
        using var operation = lifetime.Begin(() => true);
        var provider = Pending<EditorImportedImage?>();
        Task<bool> import = operation.RunOwnedAsync(_ => provider.Task, _ => throw new InvalidOperationException());
        if (stale) lifetime.Invalidate();
        provider.SetException(new IOException("Picker provider failed."));
        if (stale) Assert.False(await import);
        else await Assert.ThrowsAsync<IOException>(() => import);
    }

    [Fact]
    public async Task ActiveProviderCancellationIsReported()
    {
        var lifetime = new EditorImageOperationLifetime();
        using var operation = lifetime.Begin(() => true);
        await Assert.ThrowsAsync<OperationCanceledException>(() => operation.RunOwnedAsync(
            _ => throw new OperationCanceledException(), _ => Task.FromResult(true)));
    }

    [Fact]
    public async Task ClosingCancelsAllPendingImports()
    {
        var lifetime = new EditorImageOperationLifetime();
        using var first = lifetime.Begin(() => true);
        using var second = lifetime.Begin(() => true);
        async Task<EditorImportedImage?> Wait(CancellationToken token)
        {
            await Task.Delay(Timeout.Infinite, token);
            return null;
        }
        Task<bool> one = first.RunOwnedAsync(Wait, _ => Task.FromResult(true));
        Task<bool> two = second.RunOwnedAsync(Wait, _ => Task.FromResult(true));
        lifetime.Close();
        Assert.False(await one.WaitAsync(TimeSpan.FromSeconds(5)));
        Assert.False(await two.WaitAsync(TimeSpan.FromSeconds(5)));
    }

    private static TaskCompletionSource<T> Pending<T>() => new(TaskCreationOptions.RunContinuationsAsynchronously);

    private static SKBitmap Bitmap(SKColor color)
    {
        var bitmap = new SKBitmap(2, 2);
        bitmap.Erase(color);
        return bitmap;
    }

    private static byte[] Png()
    {
        using var bitmap = Bitmap(SKColors.Red);
        using var image = SKImage.FromBitmap(bitmap);
        using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
        return encoded.ToArray();
    }

    private sealed class ImportStream(byte[] bytes, bool block = false) : Stream
    {
        private readonly MemoryStream data = new(bytes);
        public TaskCompletionSource<bool> ReadStarted { get; } = Pending<bool>();
        public bool Disposed { get; private set; }
        public int Reads { get; private set; }
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => data.Length;
        public override long Position { get => data.Position; set => throw new NotSupportedException(); }
        public override int Read(byte[] buffer, int offset, int count) => data.Read(buffer, offset, count);
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            Reads++;
            ReadStarted.TrySetResult(true);
            if (block) await Task.Delay(Timeout.Infinite, cancellationToken);
            return await data.ReadAsync(buffer, cancellationToken);
        }
        public override void Flush() => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            if (disposing) data.Dispose();
            base.Dispose(disposing);
        }
    }
}