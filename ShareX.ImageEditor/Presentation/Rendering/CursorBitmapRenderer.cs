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

using Avalonia.Media.Imaging;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Localization;
using SkiaSharp;
using System.Buffers.Binary;
using ShareX.Platform;

namespace ShareX.ImageEditor.Presentation.Rendering
{
    internal static class CursorBitmapRenderer
    {
        private static readonly object SyncRoot = new();
        private static readonly Dictionary<CursorType, SKBitmap?> AnnotationBitmapCache = new();
        private static readonly Dictionary<(CursorType CursorType, int PreviewSize), Bitmap?> PreviewBitmapCache = new();
        private static ISystemGraphicsService? cachedGraphics;

        private static void RefreshGraphicsCache()
        {
            ISystemGraphicsService? graphics = PlatformServices.IsInitialized ? PlatformServices.Current.Graphics : null;
            if (ReferenceEquals(cachedGraphics, graphics)) return;
            cachedGraphics = graphics;
            foreach (SKBitmap? bitmap in AnnotationBitmapCache.Values) bitmap?.Dispose();
            AnnotationBitmapCache.Clear();
            // Controls borrow previews; clearing references must not dispose their images.
            PreviewBitmapCache.Clear();
        }
        // These cursor images previously came from a runtime WinForms reflection lookup.
        private static readonly IReadOnlyDictionary<CursorType, string> BundledCursors = new Dictionary<CursorType, string>
        {
            [CursorType.HSplit] = "hsplit",
            [CursorType.VSplit] = "vsplit",
            [CursorType.NoMove2D] = "nomove2d",
            [CursorType.NoMoveHoriz] = "nomoveh",
            [CursorType.NoMoveVert] = "nomovev",
            [CursorType.PanEast] = "east",
            [CursorType.PanNE] = "ne",
            [CursorType.PanNorth] = "north",
            [CursorType.PanNW] = "nw",
            [CursorType.PanSE] = "se",
            [CursorType.PanSouth] = "south",
            [CursorType.PanSW] = "sw",
            [CursorType.PanWest] = "west",
        };
        public static SKBitmap? CreateAnnotationBitmap(CursorType cursorType)
        {
            lock (SyncRoot)
            {
                RefreshGraphicsCache();
                return GetCachedAnnotationBitmap(cursorType)?.Copy();
            }
        }

        public static FeatureSupport GetSupport(CursorType cursorType)
        {
            lock (SyncRoot)
            {
                RefreshGraphicsCache();
                if (!BundledCursors.ContainsKey(cursorType))
                {
                    FeatureSupport support = cachedGraphics?.CursorSupport ??
                        FeatureSupport.NotSupported(Strings.CursorBitmapRenderer_CursorImagesUnavailable);
                    if (!support.IsSupported) return support;
                }

                return GetCachedAnnotationBitmap(cursorType) != null ? FeatureSupport.Supported :
                    FeatureSupport.NotSupported(Strings.CursorBitmapRenderer_CursorUnavailableInTheme);
            }
        }

        private static SKBitmap? GetCachedAnnotationBitmap(CursorType cursorType)
        {
            if (!AnnotationBitmapCache.TryGetValue(cursorType, out SKBitmap? bitmap))
            {
                bitmap = RenderCursorBitmap(cursorType, cachedGraphics);
                AnnotationBitmapCache[cursorType] = bitmap;
            }
            return bitmap;
        }

        public static Bitmap? GetPreviewBitmap(CursorType cursorType, int previewSize = 28)
        {
            lock (SyncRoot)
            {
                RefreshGraphicsCache();
                var cacheKey = (cursorType, previewSize);

                if (!PreviewBitmapCache.TryGetValue(cacheKey, out Bitmap? previewBitmap))
                {
                    previewBitmap = CreatePreviewBitmap(cursorType, previewSize);
                    PreviewBitmapCache[cacheKey] = previewBitmap;
                }

                return previewBitmap;
            }
        }

        private static Bitmap? CreatePreviewBitmap(CursorType cursorType, int previewSize)
        {
            using SKBitmap? annotationBitmap = CreateAnnotationBitmap(cursorType);
            if (annotationBitmap == null)
            {
                return null;
            }

            int canvasSize = Math.Max(16, previewSize);

            using var previewBitmap = new SKBitmap(new SKImageInfo(canvasSize, canvasSize, SKColorType.Bgra8888, SKAlphaType.Premul));
            using var canvas = new SKCanvas(previewBitmap);
            using var paint = new SKPaint { IsAntialias = true };

            canvas.Clear(SKColors.Transparent);

            float maxWidth = Math.Max(1, canvasSize - 4f);
            float maxHeight = Math.Max(1, canvasSize - 4f);
            float scale = Math.Min(maxWidth / annotationBitmap.Width, maxHeight / annotationBitmap.Height);

            if (float.IsNaN(scale) || float.IsInfinity(scale) || scale <= 0)
            {
                scale = 1f;
            }

            float drawWidth = annotationBitmap.Width * scale;
            float drawHeight = annotationBitmap.Height * scale;
            float left = (canvasSize - drawWidth) / 2f;
            float top = (canvasSize - drawHeight) / 2f;

            canvas.DrawBitmap(annotationBitmap, new SKRect(left, top, left + drawWidth, top + drawHeight), paint);
            canvas.Flush();

            return BitmapConversionHelpers.ToAvaloniBitmap(previewBitmap);
        }

        internal static SKBitmap? RenderCursorBitmap(CursorType cursorType, ISystemGraphicsService? graphics)
        {
            if (BundledCursors.TryGetValue(cursorType, out string? asset))
            {
                using Stream? source = typeof(CursorBitmapRenderer).Assembly.GetManifestResourceStream(
                    $"ShareX.ImageEditor.Assets.Cursors.{asset}.cur");
                if (source == null) return null;
                using MemoryStream stream = new();
                source.CopyTo(stream);
                byte[] data = stream.ToArray();
                // CUR and ICO share their DIB payload. Replace the cursor directory's
                // hotspot fields with icon planes/depth, then let Skia decode its mask.
                int offset = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(18));
                BinaryPrimitives.WriteInt16LittleEndian(data.AsSpan(2), 1);
                BinaryPrimitives.WriteInt16LittleEndian(data.AsSpan(10), 1);
                BinaryPrimitives.WriteInt16LittleEndian(data.AsSpan(12), BinaryPrimitives.ReadInt16LittleEndian(data.AsSpan(offset + 14)));
                return SKBitmap.Decode(data);
            }

            if (graphics == null || !graphics.CursorSupport.IsSupported || ToSystemCursor(cursorType) is not SystemCursor standard)
                return null;
            return graphics.GetSystemCursor(standard) is { } cursor
                ? SystemGraphicsBitmapConversion.ToSkBitmap(cursor.Image) : null;
        }

        internal static SystemCursor? ToSystemCursor(CursorType cursor) => cursor switch
        {
            CursorType.AppStarting => SystemCursor.AppStarting,
            CursorType.Arrow or CursorType.Default => SystemCursor.Arrow,
            CursorType.Cross => SystemCursor.Cross,
            CursorType.Hand => SystemCursor.Hand,
            CursorType.Help => SystemCursor.Help,
            CursorType.IBeam => SystemCursor.IBeam,
            CursorType.No => SystemCursor.No,
            CursorType.SizeAll => SystemCursor.SizeAll,
            CursorType.SizeNESW => SystemCursor.SizeNESW,
            CursorType.SizeNS => SystemCursor.SizeNS,
            CursorType.SizeNWSE => SystemCursor.SizeNWSE,
            CursorType.SizeWE => SystemCursor.SizeWE,
            CursorType.UpArrow => SystemCursor.UpArrow,
            CursorType.WaitCursor => SystemCursor.Wait,
            _ => null
        };
    }
}
