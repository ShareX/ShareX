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

namespace ShareX.ImageEditor.Presentation.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private SkiaSharp.SKBitmap? _currentSourceImage;
        private SkiaSharp.SKBitmap? _originalSourceImage; // Backup for smart padding restore

        private static bool IsBitmapAlive(SkiaSharp.SKBitmap? bitmap)
        {
            return bitmap != null && bitmap.Handle != IntPtr.Zero;
        }

        private static SkiaSharp.SKBitmap? SafeCopyBitmap(SkiaSharp.SKBitmap? source, string context)
        {
            if (!IsBitmapAlive(source))
            {
                return null;
            }

            SkiaSharp.SKBitmap safeSource = source!;
            SkiaSharp.SKBitmap? copy = safeSource.Copy();
            if (copy == null || copy.Handle == IntPtr.Zero)
            {
                copy?.Dispose();
                return null;
            }

            return copy;
        }

        private SkiaSharp.SKBitmap? GetBestAvailableSourceBitmap()
        {
            SkiaSharp.SKBitmap? coreSource = _editorCore?.SourceImage;
            if (IsBitmapAlive(coreSource))
            {
                return coreSource;
            }

            if (IsBitmapAlive(_currentSourceImage))
            {
                return _currentSourceImage;
            }

            return null;
        }

        internal bool IsEffectPreviewActive => _isPreviewingEffect;

        internal SkiaSharp.SKBitmap? CreateSourceImageCopyForCore()
        {
            if (IsBitmapAlive(_currentSourceImage))
            {
                return SafeCopyBitmap(_currentSourceImage, "CreateSourceImageCopyForCore.Current");
            }

            SkiaSharp.SKBitmap? coreSource = _editorCore?.SourceImage;
            if (IsBitmapAlive(coreSource))
            {
                return SafeCopyBitmap(coreSource, "CreateSourceImageCopyForCore.Core");
            }

            return null;
        }

        /// <summary>
        /// Updates the preview image. **TAKES OWNERSHIP** of the bitmap parameter.
        /// </summary>
        /// <remarks>
        /// ISSUE-027: Ownership contract documentation
        /// - The bitmap parameter is stored directly in _currentSourceImage
        /// - The caller MUST NOT dispose the bitmap after calling this method
        /// - A backup copy is created for _originalSourceImage (for smart padding)
        /// - If the bitmap was created by the caller, ownership is fully transferred
        /// </remarks>
        /// <param name="image">Image bitmap (ownership transferred to ViewModel)</param>
        /// <param name="clearAnnotations">Whether to clear all annotations</param>
        public void UpdatePreview(SkiaSharp.SKBitmap image, bool clearAnnotations = true)
        {
            if (!IsBitmapAlive(image))
            {
                return;
            }

            // ISSUE-031 fix: Dispose old currentSourceImage before replacing (if different object)
            if (_currentSourceImage != null && _currentSourceImage != image)
            {
                _currentSourceImage.Dispose();
            }
            _currentSourceImage = image;

            // Update original backup first so smart padding uses the new image during PreviewImage change
            if (!_isApplyingSmartPadding)
            {
                _originalSourceImage?.Dispose();
                var copy = SafeCopyBitmap(image, "UpdatePreview");
                _originalSourceImage = copy;
            }

            // Store dimensions BEFORE conversion (ToAvaloniBitmap triggers property change that may dispose the bitmap)
            int width = image.Width;
            int height = image.Height;

            if (clearAnnotations)
            {
                RequestZoomToFitOnNextImageLoad();
            }

            // Convert SKBitmap to Avalonia Bitmap
            PreviewImage = BitmapConversionHelpers.ToAvaloniBitmap(image);
            ImageDimensions = $"{width} x {height}";

            // Reset view state for the new image
            Zoom = 1.0;
            if (clearAnnotations)
            {
                ClearAnnotationsRequested?.Invoke(this, EventArgs.Empty);
            }
            ResetNumberCounter();
        }

        public void CropImage(int x, int y, int width, int height)
        {
            if (_editorCore == null || width <= 0 || height <= 0)
            {
                return;
            }

            if (_editorCore.Crop(new SkiaSharp.SKRect(x, y, x + width, y + height)))
            {
                (int newWidth, int newHeight) = GetCurrentImageSize();
                ShowImageCroppedNotification(newWidth, newHeight);
            }
        }

        public void CutOutImage(int startPos, int endPos, bool isVertical)
        {
            if (_editorCore?.CutOut(startPos, endPos, isVertical) == true)
            {
                (int newWidth, int newHeight) = GetCurrentImageSize();
                ShowImageCutOutNotification(newWidth, newHeight);
            }
        }

        private (int Width, int Height) GetCurrentImageSize()
        {
            if (_editorCore?.SourceImage != null)
            {
                return (_editorCore.SourceImage.Width, _editorCore.SourceImage.Height);
            }

            return (0, 0);
        }
    }
}