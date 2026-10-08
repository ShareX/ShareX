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
            using SkiaSharp.SKBitmap decoded = CursorBitmapDecoder.Decode(cursorStream, renderScaling, out PixelPoint hotSpot);
            Bitmap bitmap = BitmapConversionHelpers.ToAvaloniBitmap(decoded);
            try
            {
                return new LoadedCursor(bitmap, new Cursor(bitmap, hotSpot));
            }
            catch
            {
                bitmap.Dispose();
                throw;
            }
        }

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
