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

/// <summary>B41: a pending clipboard clear must never delete a later selection or document.</summary>
public sealed class EditorAnnotationCutTests
{
    [Fact]
    public async Task CutCommitsBeforeClipboardClearAndDoesNotDeleteLaterSelection()
    {
        using var fixture = new CutFixture();
        var first = fixture.AddImage(SKColors.Red);
        var later = fixture.AddImage(SKColors.Blue);
        fixture.Core.Select(first);
        var clear = Pending();
        Task<bool> cut = fixture.Cut(first, () =>
        {
            Assert.DoesNotContain(first, fixture.Core.Annotations);
            Assert.True(first.IsDisposed);
            Assert.Single(fixture.Removed);
            return clear.Task;
        });
        Assert.False(cut.IsCompleted);
        fixture.Core.Select(later);
        clear.SetResult(true);
        Assert.True(await cut.WaitAsync(TimeSpan.FromSeconds(5)));
        Assert.Same(later, Assert.Single(fixture.Core.Annotations));
        Assert.Same(later, fixture.Core.SelectedAnnotation);
        Assert.Same(first, Assert.Single(fixture.Removed));
        Assert.Equal(SKColors.Red, Assert.IsType<ImageAnnotation>(fixture.Clipboard).ImageBitmap!.GetPixel(0, 0));
    }

    [Fact]
    public async Task UndoAndRedoRetainTheChosenImageAndAnIndependentClipboardClone()
    {
        using var fixture = new CutFixture();
        var original = fixture.AddImage(SKColors.Red);
        var other = fixture.AddImage(SKColors.Blue);
        fixture.Core.Select(original);
        SKBitmap bitmap = original.ImageBitmap!;
        Assert.True(await fixture.Cut(original, () => Task.CompletedTask));
        Assert.Equal(IntPtr.Zero, bitmap.Handle);
        var copy = Assert.IsType<ImageAnnotation>(fixture.Clipboard);
        Assert.NotSame(original, copy);
        Assert.Equal(SKColors.Red, copy.ImageBitmap!.GetPixel(0, 0));
        fixture.Core.Undo();
        var restored = Assert.IsType<ImageAnnotation>(fixture.Core.Annotations.Single(a => a.Id == original.Id));
        Assert.Equal(SKColors.Red, restored.ImageBitmap!.GetPixel(0, 0));
        Assert.Equal(2, fixture.Core.Annotations.Count);
        fixture.Core.Redo();
        Assert.Equal(other.Id, Assert.Single(fixture.Core.Annotations).Id);
        Assert.Equal(SKColors.Red, copy.ImageBitmap.GetPixel(0, 0));
    }

    [Theory]
    [InlineData("close")]
    [InlineData("unload")]
    [InlineData("document")]
    public async Task LeavingEditorReleasesClipboardWaitWithoutAnotherDeletion(string transition)
    {
        using var fixture = new CutFixture();
        var original = fixture.AddImage(SKColors.Red);
        var clear = Pending();
        Task<bool> cut = fixture.Cut(original, () => clear.Task);
        if (transition == "close")
        {
            fixture.Lifetime.Close();
            fixture.Core.Dispose();
        }
        else
        {
            fixture.Lifetime.Invalidate();
            if (transition == "document") fixture.Core.LoadImage(new SKBitmap(1, 1));
        }
        Assert.True(await cut.WaitAsync(TimeSpan.FromSeconds(5)));
        Assert.False(clear.Task.IsCompleted);
        Assert.Single(fixture.Removed);
        Assert.Empty(fixture.Errors);
        clear.SetException(new IOException("Old clipboard provider completed after the editor left."));
        await Assert.ThrowsAsync<IOException>(() => clear.Task);
        Assert.Empty(fixture.Errors);
    }

    [Fact]
    public async Task OwnerChangeSuppressesLateClipboardErrorAndKeepsNewSelection()
    {
        using var fixture = new CutFixture();
        var first = fixture.AddImage(SKColors.Red);
        var second = fixture.AddImage(SKColors.Blue);
        fixture.Core.Select(first);
        var clear = Pending();
        Task<bool> cut = fixture.Cut(first, () => clear.Task);
        fixture.Current = false;
        fixture.Core.Select(second);
        clear.SetException(new IOException("Stale provider."));
        Assert.True(await cut);
        Assert.Empty(fixture.Errors);
        Assert.Same(second, Assert.Single(fixture.Core.Annotations));
        Assert.Same(second, fixture.Core.SelectedAnnotation);
    }

    [Fact]
    public async Task ClipboardFailureReportsOnceWithoutRollingBackAcceptedCut()
    {
        using var fixture = new CutFixture();
        var original = fixture.AddImage(SKColors.Red);
        Assert.True(await fixture.Cut(original, () => throw new IOException("Clipboard unavailable.")));
        Assert.Empty(fixture.Core.Annotations);
        Assert.Single(fixture.Removed);
        Assert.IsType<IOException>(Assert.Single(fixture.Errors));
        fixture.Core.Undo();
        Assert.Equal(original.Id, Assert.Single(fixture.Core.Annotations).Id);
    }

    [Theory]
    [InlineData("closed")]
    [InlineData("owner")]
    [InlineData("removed")]
    [InlineData("disposed")]
    public async Task InvalidCutTargetCannotPublishOrClearClipboard(string state)
    {
        using var fixture = new CutFixture();
        using var original = fixture.AddImage(SKColors.Red);
        if (state == "closed") fixture.Lifetime.Close();
        else if (state == "owner") fixture.Current = false;
        else if (state == "removed") fixture.Core.RemoveAnnotation(original);
        else original.Dispose();
        Assert.False(await fixture.Cut(original, () => throw new InvalidOperationException("Unexpected clipboard clear.")));
        Assert.Null(fixture.Clipboard);
        Assert.Empty(fixture.Removed);
        Assert.Empty(fixture.Errors);
    }

    [Fact]
    public async Task ClonePublicationFailureReleasesCloneAndDoesNotRemoveOriginal()
    {
        using var fixture = new CutFixture();
        var original = fixture.AddImage(SKColors.Red);
        ImageAnnotation? rejected = null;
        using var operation = fixture.Lifetime.Begin(() => true);
        await Assert.ThrowsAsync<InvalidOperationException>(() => EditorAnnotationCutController.CutAsync(
            fixture.Core, original, operation, copy =>
            {
                rejected = Assert.IsType<ImageAnnotation>(copy);
                throw new InvalidOperationException("Clipboard owner rejected clone.");
            }, _ => throw new InvalidOperationException(), () => throw new InvalidOperationException(), _ => throw new InvalidOperationException()));
        Assert.NotNull(rejected);
        Assert.True(rejected.IsDisposed);
        Assert.Null(rejected.ImageBitmap);
        Assert.Same(original, Assert.Single(fixture.Core.Annotations));
        Assert.Equal(SKColors.Red, original.ImageBitmap!.GetPixel(0, 0));
    }

    [Fact]
    public async Task VisualCleanupFailureStillReleasesRemovedImageAndRetainsUndo()
    {
        using var fixture = new CutFixture();
        var original = fixture.AddImage(SKColors.Red);
        using var operation = fixture.Lifetime.Begin(() => true);
        await Assert.ThrowsAsync<InvalidOperationException>(() => EditorAnnotationCutController.CutAsync(
            fixture.Core, original, operation, fixture.Publish,
            _ => throw new InvalidOperationException("Visual cleanup failed."), () => Task.CompletedTask, fixture.Errors.Add));
        Assert.True(original.IsDisposed);
        Assert.Empty(fixture.Core.Annotations);
        fixture.Core.Undo();
        var restored = Assert.IsType<ImageAnnotation>(Assert.Single(fixture.Core.Annotations));
        Assert.Equal(SKColors.Red, restored.ImageBitmap!.GetPixel(0, 0));
    }

    [Fact]
    public async Task DocumentChangeDuringPublicationRejectsRemovalAndClipboardClear()
    {
        using var fixture = new CutFixture();
        var original = fixture.AddImage(SKColors.Red);
        using var operation = fixture.Lifetime.Begin(() => true);
        Assert.False(await EditorAnnotationCutController.CutAsync(fixture.Core, original, operation, copy =>
        {
            fixture.Publish(copy);
            fixture.Lifetime.Invalidate();
        }, _ => throw new InvalidOperationException(), () => throw new InvalidOperationException(), _ => throw new InvalidOperationException()));
        Assert.Same(original, Assert.Single(fixture.Core.Annotations));
        Assert.False(original.IsDisposed);
    }

    [Fact]
    public async Task StepRenumberingAndUndoUseTheExistingCoreRemovalPath()
    {
        using var fixture = new CutFixture();
        var first = new NumberAnnotation { Number = 1 };
        var second = new NumberAnnotation { Number = 2 };
        fixture.Core.AddAnnotation(first);
        fixture.Core.AddAnnotation(second);
        fixture.Core.Select(first);
        Assert.True(await fixture.Cut(first, () => Task.CompletedTask));
        Assert.Same(second, Assert.Single(fixture.Core.Annotations));
        Assert.Equal(1, second.Number);
        fixture.Core.Undo();
        Assert.Equal([1, 2], fixture.Core.Annotations.Cast<NumberAnnotation>().Select(a => a.Number));
    }

    [Fact]
    public async Task OverlappingClipboardClearsCannotRepeatEitherCut()
    {
        using var fixture = new CutFixture();
        var first = fixture.AddImage(SKColors.Red);
        var second = fixture.AddImage(SKColors.Blue);
        var firstClear = Pending();
        var secondClear = Pending();
        fixture.Core.Select(first);
        Task<bool> one = fixture.Cut(first, () => firstClear.Task);
        fixture.Core.Select(second);
        Task<bool> two = fixture.Cut(second, () => secondClear.Task);
        Assert.Empty(fixture.Core.Annotations);
        secondClear.SetResult(true);
        Assert.True(await two);
        firstClear.SetResult(true);
        Assert.True(await one);
        Assert.Equal([first, second], fixture.Removed);
        Assert.Equal(SKColors.Blue, Assert.IsType<ImageAnnotation>(fixture.Clipboard).ImageBitmap!.GetPixel(0, 0));
    }

    private static TaskCompletionSource<bool> Pending() => new(TaskCreationOptions.RunContinuationsAsynchronously);

    private sealed class CutFixture : IDisposable
    {
        public EditorCore Core { get; } = new();
        public EditorImageOperationLifetime Lifetime { get; } = new();
        public bool Current { get; set; } = true;
        public Annotation? Clipboard { get; private set; }
        public List<Annotation> Removed { get; } = new();
        public List<Exception> Errors { get; } = new();

        public ImageAnnotation AddImage(SKColor color)
        {
            var bitmap = new SKBitmap(2, 1);
            bitmap.Erase(color);
            var annotation = new ImageAnnotation { EndPoint = new SKPoint(2, 1) };
            annotation.SetImage(bitmap);
            Core.AddAnnotation(annotation);
            return annotation;
        }

        public void Publish(Annotation copy)
        {
            (Clipboard as IDisposable)?.Dispose();
            Clipboard = copy;
        }

        public async Task<bool> Cut(Annotation annotation, Func<Task> clear)
        {
            using var operation = Lifetime.Begin(() => Current);
            return await EditorAnnotationCutController.CutAsync(Core, annotation, operation, Publish, Removed.Add, clear, Errors.Add);
        }

        public void Dispose()
        {
            Lifetime.Close();
            Core.Dispose();
            (Clipboard as IDisposable)?.Dispose();
        }
    }
}