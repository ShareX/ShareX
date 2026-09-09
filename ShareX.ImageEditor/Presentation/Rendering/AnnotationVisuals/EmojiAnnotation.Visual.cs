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

using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ShareX.ImageEditor.Presentation.Emoji;
using SkiaSharp;
using static ShareX.ImageEditor.Presentation.Rendering.AnnotationVisualHelpers;
using System.Runtime.CompilerServices;

namespace ShareX.ImageEditor.Core.Annotations;

public partial class EmojiAnnotation
{
    private static readonly ConditionalWeakTable<Image, EmojiInteractiveRenderState> EmojiInteractiveRenderStates = new();
    private static readonly SemaphoreSlim EmojiInteractiveRenderThrottle = new(2, 2);

    public override Control CreateVisual()
    {
        var image = new Image
        {
            Tag = this
        };

        image.DetachedFromVisualTree += (_, _) => CancelQueuedEmojiRefresh(image);
        UpdateVisual(image);
        return image;
    }

    internal override void UpdateVisual(Image imageControl, bool useInteractiveRender = false)
    {
        if (IsDisposed) return;
        var imageBounds = GetBounds();
        int renderSize = Math.Max(1, (int)Math.Ceiling(Math.Max(imageBounds.Width, imageBounds.Height)));
        int targetBitmapSize = useInteractiveRender
            ? WindowsEmojiBitmapRenderer.GetInteractiveStickerSize(renderSize)
            : renderSize;
        bool hasUnicode = !string.IsNullOrWhiteSpace(UnicodeSequence);
        bool hasTargetBitmap = ImageBitmap != null
            && ImageBitmap.Width == targetBitmapSize
            && ImageBitmap.Height == targetBitmapSize;

        bool shouldQueueAsyncRefresh = hasUnicode
            && !hasTargetBitmap
            && imageControl.Source != null;

        if (shouldQueueAsyncRefresh)
        {
            QueueEmojiRefresh(this, imageControl, renderSize, targetBitmapSize, useInteractiveRender);
        }
        else
        {
            CancelQueuedEmojiRefresh(imageControl);
        }

        bool needsBitmapRefresh = hasUnicode
            && !hasTargetBitmap
            && !shouldQueueAsyncRefresh;

        if (needsBitmapRefresh)
        {
            var renderedBitmap = useInteractiveRender
                ? WindowsEmojiBitmapRenderer.RenderInteractiveStickerBitmap(UnicodeSequence, renderSize)
                : WindowsEmojiBitmapRenderer.RenderStickerBitmap(UnicodeSequence, renderSize);

            if (renderedBitmap != null)
            {
                SetImage(renderedBitmap);
            }
        }

        if (ImageBitmap != null && (needsBitmapRefresh || imageControl.Source == null))
        {
            ReplaceImageSource(imageControl, BitmapConversionHelpers.ToAvaloniBitmap(ImageBitmap));
        }

        Canvas.SetLeft(imageControl, imageBounds.Left);
        Canvas.SetTop(imageControl, imageBounds.Top);
        imageControl.Width = Math.Max(1, imageBounds.Width);
        imageControl.Height = Math.Max(1, imageBounds.Height);
        ApplyRotationTransform(imageControl, RotationAngle);
    }

    private static void QueueEmojiRefresh(EmojiAnnotation emojiAnnotation, Image imageControl, int renderSize, int targetBitmapSize, bool useInteractiveRender)
    {
        string unicodeSequence = emojiAnnotation.UnicodeSequence;
        if (string.IsNullOrWhiteSpace(unicodeSequence))
        {
            return;
        }

        EmojiInteractiveRenderState state = EmojiInteractiveRenderStates.GetOrCreateValue(imageControl);
        string requestKey = $"{unicodeSequence}:{(useInteractiveRender ? "interactive" : "exact")}:{targetBitmapSize}";
        bool shouldStartWorker = false;

        lock (state.SyncRoot)
        {
            if ((string.Equals(state.PendingRequestKey, requestKey, StringComparison.Ordinal) && ReferenceEquals(state.PendingAnnotation, emojiAnnotation))
                || (string.Equals(state.InFlightRequestKey, requestKey, StringComparison.Ordinal) && ReferenceEquals(state.InFlightAnnotation, emojiAnnotation)))
            {
                return;
            }

            state.UpdateVersion++;
            state.PendingRequestKey = requestKey;
            state.PendingUnicodeSequence = unicodeSequence;
            state.PendingRenderSize = renderSize;
            state.PendingAnnotation = emojiAnnotation;
            state.PendingUseInteractiveRender = useInteractiveRender;

            if (!state.IsWorkerRunning)
            {
                state.IsWorkerRunning = true;
                shouldStartWorker = true;
            }
        }

        if (shouldStartWorker)
        {
            _ = UpdateQueuedEmojiImageAsync(imageControl, state);
        }
    }

    private static void CancelQueuedEmojiRefresh(Image imageControl)
    {
        EmojiInteractiveRenderState state = EmojiInteractiveRenderStates.GetOrCreateValue(imageControl);
        lock (state.SyncRoot)
        {
            state.UpdateVersion++;
            state.PendingRequestKey = null;
            state.PendingUnicodeSequence = null;
            state.PendingRenderSize = 0;
            state.PendingAnnotation = null;
            state.PendingUseInteractiveRender = false;
        }
    }

    private static async Task UpdateQueuedEmojiImageAsync(Image imageControl, EmojiInteractiveRenderState state)
    {
        try
        {
            while (true)
            {
                string? requestKey;
                string? unicodeSequence;
                int renderSize;
                int version;
                EmojiAnnotation? emojiAnnotation;
                bool useInteractiveRender;

                lock (state.SyncRoot)
                {
                    requestKey = state.PendingRequestKey;
                    unicodeSequence = state.PendingUnicodeSequence;
                    renderSize = state.PendingRenderSize;
                    version = state.UpdateVersion;
                    emojiAnnotation = state.PendingAnnotation;
                    useInteractiveRender = state.PendingUseInteractiveRender;

                    state.PendingRequestKey = null;
                    state.PendingUnicodeSequence = null;
                    state.PendingRenderSize = 0;
                    state.PendingAnnotation = null;
                    state.PendingUseInteractiveRender = false;
                    state.InFlightRequestKey = requestKey;
                    state.InFlightAnnotation = emojiAnnotation;
                }

                if (string.IsNullOrWhiteSpace(requestKey) || string.IsNullOrWhiteSpace(unicodeSequence) || renderSize <= 0 || emojiAnnotation == null)
                {
                    lock (state.SyncRoot)
                    {
                        state.InFlightRequestKey = null;
                        state.InFlightAnnotation = null;

                        if (state.PendingRequestKey == null)
                        {
                            state.IsWorkerRunning = false;
                            return;
                        }
                    }

                    continue;
                }

                await EmojiInteractiveRenderThrottle.WaitAsync();

                try
                {
                    bool skipRender;

                    lock (state.SyncRoot)
                    {
                        skipRender = emojiAnnotation.IsDisposed || version != state.UpdateVersion
                            || !string.Equals(state.InFlightRequestKey, requestKey, StringComparison.Ordinal)
                            || !ReferenceEquals(state.InFlightAnnotation, emojiAnnotation);
                    }

                    if (skipRender)
                    {
                        continue;
                    }

                    SKBitmap? renderedBitmap = null;
                    Bitmap? bitmapSource = null;

                    try
                    {
                        renderedBitmap = await Task.Run(() => useInteractiveRender
                            ? WindowsEmojiBitmapRenderer.RenderInteractiveStickerBitmap(unicodeSequence, renderSize)
                            : WindowsEmojiBitmapRenderer.RenderStickerBitmap(unicodeSequence, renderSize));
                        if (renderedBitmap == null)
                        {
                            continue;
                        }

                        bitmapSource = BitmapConversionHelpers.ToAvaloniBitmap(renderedBitmap);
                    }
                    catch
                    {
                        renderedBitmap?.Dispose();
                        bitmapSource?.Dispose();
                        continue;
                    }

                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        bool isCurrentRequest;

                        lock (state.SyncRoot)
                        {
                            isCurrentRequest = !emojiAnnotation.IsDisposed && version == state.UpdateVersion
                                && string.Equals(state.InFlightRequestKey, requestKey, StringComparison.Ordinal)
                                && ReferenceEquals(state.InFlightAnnotation, emojiAnnotation)
                                && ReferenceEquals(imageControl.Tag, emojiAnnotation);
                        }

                        if (!isCurrentRequest)
                        {
                            bitmapSource.Dispose();
                            renderedBitmap.Dispose();
                            return;
                        }

                        emojiAnnotation.SetImage(renderedBitmap);
                        ReplaceImageSource(imageControl, bitmapSource);
                    }, DispatcherPriority.Background);
                }
                finally
                {
                    EmojiInteractiveRenderThrottle.Release();
                }

                lock (state.SyncRoot)
                {
                    if (string.Equals(state.InFlightRequestKey, requestKey, StringComparison.Ordinal)
                        && ReferenceEquals(state.InFlightAnnotation, emojiAnnotation))
                    {
                        state.InFlightRequestKey = null;
                        state.InFlightAnnotation = null;
                    }

                    if (state.PendingRequestKey == null)
                    {
                        state.IsWorkerRunning = false;
                        return;
                    }
                }
            }
        }
        catch
        {
            bool shouldRestartWorker;

            lock (state.SyncRoot)
            {
                state.InFlightRequestKey = null;
                state.InFlightAnnotation = null;
                shouldRestartWorker = state.PendingRequestKey != null;
                state.IsWorkerRunning = shouldRestartWorker;
            }

            if (shouldRestartWorker)
            {
                _ = UpdateQueuedEmojiImageAsync(imageControl, state);
            }
        }
    }

    private sealed class EmojiInteractiveRenderState
    {
        public object SyncRoot { get; } = new();
        public bool IsWorkerRunning;
        public string? PendingRequestKey;
        public string? PendingUnicodeSequence;
        public int PendingRenderSize;
        public EmojiAnnotation? PendingAnnotation;
        public bool PendingUseInteractiveRender;
        public string? InFlightRequestKey;
        public EmojiAnnotation? InFlightAnnotation;
        public int UpdateVersion;
    }
}
