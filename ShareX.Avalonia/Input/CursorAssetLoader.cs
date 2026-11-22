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
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ShareX.AvaloniaUI.Imaging;
using ShareX.Platform;
using System.Runtime.CompilerServices;

namespace ShareX.AvaloniaUI.Input
{
    public static class CursorAssetLoader
    {
        public enum CustomCursorKind
        {
            ClosedHand,
            Crosshair,
            OpenHand
        }

        private static readonly Uri ClosedHandCursorUri = new("avares://ShareX.Avalonia/Assets/closedhand.cur");
        private static readonly Uri CrosshairCursorUri = new("avares://ShareX.Avalonia/Assets/Crosshair.cur");
        private static readonly Uri OpenHandCursorUri = new("avares://ShareX.Avalonia/Assets/openhand.cur");
        private static readonly Cursor FallbackClosedHandCursor = new(StandardCursorType.SizeAll);
        private static readonly Cursor FallbackCrosshairCursor = new(StandardCursorType.Cross);
        private static readonly Cursor FallbackOpenHandCursor = new(StandardCursorType.Hand);
        private static readonly object CursorCacheSyncRoot = new();
        private static readonly Dictionary<(CustomCursorKind CursorKind, int ScaleKey), LoadedCursor?> CursorCache = new();
        private static readonly ConditionalWeakTable<Cursor, OverlayCursor> OverlayCursorMap = new();

        public static Cursor GetCrosshairCursor()
            => GetCrosshairCursor(1.0);

        public static Cursor GetCrosshairCursor(double renderScaling)
        {
            return GetCursor(CustomCursorKind.Crosshair, renderScaling);
        }

        public static Cursor GetOpenHandCursor()
            => GetOpenHandCursor(1.0);

        public static Cursor GetOpenHandCursor(double renderScaling)
        {
            return GetCursor(CustomCursorKind.OpenHand, renderScaling);
        }

        public static Cursor GetClosedHandCursor()
            => GetClosedHandCursor(1.0);

        public static Cursor GetClosedHandCursor(double renderScaling)
        {
            return GetCursor(CustomCursorKind.ClosedHand, renderScaling);
        }

        public static Cursor GetCursor(CustomCursorKind cursorKind, double renderScaling = 1.0)
        {
            if (PlatformServices.IsInitialized && PlatformServices.Current.Windows.PrefersSystemCursors)
            {
                return GetFallbackCursor(cursorKind);
            }

            return TryLoadCursor(cursorKind, renderScaling)?.Cursor ?? GetFallbackCursor(cursorKind);
        }

        private static LoadedCursor? TryLoadCursor(CustomCursorKind cursorKind, double renderScaling)
        {
            int scaleKey = NormalizeScaleKey(renderScaling);

            lock (CursorCacheSyncRoot)
            {
                if (!CursorCache.TryGetValue((cursorKind, scaleKey), out LoadedCursor? loadedCursor))
                {
                    loadedCursor = TryLoadCursorCore(GetCursorUri(cursorKind), scaleKey / 100.0);
                    CursorCache[(cursorKind, scaleKey)] = loadedCursor;
                }

                return loadedCursor;
            }
        }

        private static LoadedCursor? TryLoadCursorCore(Uri cursorUri, double renderScaling)
        {
            try
            {
                using Stream cursorStream = AssetLoader.Open(cursorUri);
                return LoadCursor(cursorStream, renderScaling);
            }
            catch
            {
                return null;
            }
        }

        private static LoadedCursor LoadCursor(Stream cursorStream, double renderScaling)
        {
            double normalizedScale = NormalizeScaleKey(renderScaling) / 100.0;
            using SkiaSharp.SKBitmap decoded = CursorBitmapDecoder.Decode(cursorStream, renderScaling, out PixelPoint hotSpot);
            Bitmap bitmap = BitmapConversionHelpers.ToAvaloniBitmap(decoded);
            try
            {
                Cursor cursor = new(bitmap, hotSpot);
                OverlayCursorMap.AddOrUpdate(cursor, CreateOverlayCursor(decoded, hotSpot, normalizedScale));
                return new LoadedCursor(bitmap, cursor);
            }
            catch
            {
                bitmap.Dispose();
                throw;
            }
        }

        public static OverlayCursor ToOverlayCursor(Cursor? cursor)
        {
            if (cursor == null)
            {
                return OverlayCursor.Default;
            }

            if (OverlayCursorMap.TryGetValue(cursor, out OverlayCursor? custom))
            {
                return custom;
            }

            if (Enum.TryParse(cursor.ToString(), ignoreCase: true, out StandardCursorType standardType))
            {
                return OverlayCursor.FromStandard(MapStandardCursor(standardType));
            }

            return OverlayCursor.Default;
        }

        private static unsafe OverlayCursor CreateOverlayCursor(SkiaSharp.SKBitmap decoded, PixelPoint hotSpot, double scale)
        {
            int width = decoded.Width;
            int height = decoded.Height;
            int stride = checked(width * 4);
            byte[] pixels = new byte[checked(stride * height)];
            SkiaSharp.SKImageInfo info = new(width, height, SkiaSharp.SKColorType.Bgra8888, SkiaSharp.SKAlphaType.Premul);

            fixed (byte* ptr = pixels)
            {
                using SkiaSharp.SKPixmap? pixmap = decoded.PeekPixels();
                if (pixmap == null || !pixmap.ReadPixels(info, (IntPtr)ptr, stride))
                {
                    return OverlayCursor.FromStandard(OverlayCursorShape.Crosshair);
                }
            }

            return OverlayCursor.FromBitmap(pixels, width, height, hotSpot.X, hotSpot.Y, scale);
        }

        private static OverlayCursorShape MapStandardCursor(StandardCursorType cursorType) => cursorType switch
        {
            StandardCursorType.Arrow => OverlayCursorShape.Default,
            StandardCursorType.Ibeam => OverlayCursorShape.Text,
            StandardCursorType.Wait => OverlayCursorShape.Wait,
            StandardCursorType.Cross => OverlayCursorShape.Crosshair,
            StandardCursorType.SizeWestEast => OverlayCursorShape.EWResize,
            StandardCursorType.SizeNorthSouth => OverlayCursorShape.NSResize,
            StandardCursorType.SizeAll => OverlayCursorShape.Move,
            StandardCursorType.No => OverlayCursorShape.NotAllowed,
            StandardCursorType.Hand => OverlayCursorShape.Pointer,
            StandardCursorType.AppStarting => OverlayCursorShape.Progress,
            StandardCursorType.Help => OverlayCursorShape.Help,
            StandardCursorType.TopSide => OverlayCursorShape.NResize,
            StandardCursorType.BottomSide => OverlayCursorShape.SResize,
            StandardCursorType.LeftSide => OverlayCursorShape.WResize,
            StandardCursorType.RightSide => OverlayCursorShape.EResize,
            StandardCursorType.TopLeftCorner => OverlayCursorShape.NWResize,
            StandardCursorType.TopRightCorner => OverlayCursorShape.NEResize,
            StandardCursorType.BottomLeftCorner => OverlayCursorShape.SWResize,
            StandardCursorType.BottomRightCorner => OverlayCursorShape.SEResize,
            StandardCursorType.DragMove => OverlayCursorShape.Grabbing,
            StandardCursorType.DragCopy => OverlayCursorShape.Copy,
            StandardCursorType.DragLink => OverlayCursorShape.Alias,
            StandardCursorType.None => OverlayCursorShape.None,
            _ => OverlayCursorShape.Default
        };

        private static Cursor GetFallbackCursor(CustomCursorKind cursorKind)
        {
            return cursorKind switch
            {
                CustomCursorKind.ClosedHand => FallbackClosedHandCursor,
                CustomCursorKind.Crosshair => FallbackCrosshairCursor,
                _ => FallbackOpenHandCursor
            };
        }

        private static Uri GetCursorUri(CustomCursorKind cursorKind)
        {
            return cursorKind switch
            {
                CustomCursorKind.ClosedHand => ClosedHandCursorUri,
                CustomCursorKind.Crosshair => CrosshairCursorUri,
                _ => OpenHandCursorUri
            };
        }

        private static int NormalizeScaleKey(double renderScaling)
        {
            double safeRenderScaling = double.IsFinite(renderScaling) && renderScaling > 1.0
                ? renderScaling
                : 1.0;

            return Math.Max(100, (int)Math.Round(safeRenderScaling * 100.0));
        }

        private sealed class LoadedCursor
        {
            public LoadedCursor(Bitmap bitmap, Cursor cursor)
            {
                Bitmap = bitmap;
                Cursor = cursor;
            }

            public Bitmap Bitmap { get; }
            public Cursor Cursor { get; }
        }
    }
}
