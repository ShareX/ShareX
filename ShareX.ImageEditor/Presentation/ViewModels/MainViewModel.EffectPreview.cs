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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.ImageEditor.Core.ImageEffects.Adjustments;
using ShareX.ImageEditor.Core.ImageEffects.Filters;
using ShareX.ImageEditor.Core.ImageEffects.Manipulations;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Presentation.Rendering;

namespace ShareX.ImageEditor.Presentation.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        // --- Edit Menu Commands ---

        [RelayCommand]
        private void Rotate90Clockwise()
        {
            if (_editorCore?.Rotate90Clockwise() == true)
            {
                ShowImageRotatedClockwiseNotification();
            }
        }

        [RelayCommand]
        private void Rotate90CounterClockwise()
        {
            if (_editorCore?.Rotate90CounterClockwise() == true)
            {
                ShowImageRotatedCounterClockwiseNotification();
            }
        }

        [RelayCommand]
        private void Rotate180()
        {
            if (_editorCore?.Rotate180() == true)
            {
                ShowImageRotated180Notification();
            }
        }

        [RelayCommand]
        private void FlipHorizontal()
        {
            if (_editorCore?.FlipHorizontal() == true)
            {
                ShowImageFlippedHorizontallyNotification();
            }
        }

        [RelayCommand]
        private void FlipVertical()
        {
            if (_editorCore?.FlipVertical() == true)
            {
                ShowImageFlippedVerticallyNotification();
            }
        }

        [RelayCommand]
        private void AutoCropImage()
        {
            AutoCrop(10);
        }

        public void AutoCrop(int tolerance)
        {
            if (_editorCore?.AutoCrop(tolerance) == true)
            {
                ShowImageAutoCroppedNotification();
            }
        }

        /// <summary>
        /// Resize the image to new dimensions with specified quality.
        /// </summary>
        public void ResizeImage(int newWidth, int newHeight, SkiaSharp.SKSamplingOptions sampling = default)
        {
            if (newWidth <= 0 || newHeight <= 0)
            {
                return;
            }

            if (_editorCore?.ResizeImage(newWidth, newHeight, sampling) == true)
            {
                ShowImageResizedNotification();
            }
        }

        /// <summary>
        /// Resize the canvas by adding padding around the image.
        /// </summary>
        public void ResizeCanvas(int top, int right, int bottom, int left, SkiaSharp.SKColor backgroundColor)
        {
            if (_editorCore?.ResizeCanvas(top, right, bottom, left, backgroundColor) == true)
            {
                ShowCanvasResizedNotification();
            }
        }

        public void RotateCustomAngle(float angle, bool autoResize = true)
        {
            if (_editorCore?.RotateCustomAngle(angle, autoResize) == true)
            {
                ShowImageRotatedCustomAngleNotification(angle);
            }
        }

        // --- Effects Menu Commands ---

        [RelayCommand]
        private void InvertColors()
        {
            ApplyOneShotEffect(img => new InvertImageEffect().Apply(img), "Inverted colors");
        }

        [RelayCommand]
        private void BlackAndWhite()
        {
            ApplyOneShotEffect(img => new BlackAndWhiteImageEffect().Apply(img), "Applied Black & White filter");
        }

        [RelayCommand]
        private void Sepia()
        {
            ApplyOneShotEffect(img => new SepiaImageEffect().Apply(img), "Applied Sepia filter");
        }

        [RelayCommand]
        private void Polaroid()
        {
            ApplyOneShotEffect(img => new PolaroidImageEffect().Apply(img), "Applied Polaroid filter");
        }

        [RelayCommand]
        private void EdgeDetect()
        {
            ApplyOneShotEffect(img => new EdgeDetectImageEffect().Apply(img), "Applied Edge detect filter");
        }

        [RelayCommand]
        private void Emboss()
        {
            ApplyOneShotEffect(img => new EmbossImageEffect().Apply(img), "Applied Emboss filter");
        }

        [RelayCommand]
        private void MeanRemoval()
        {
            ApplyOneShotEffect(img => new MeanRemovalImageEffect().Apply(img), "Applied Mean removal filter");
        }

        [RelayCommand]
        private void Smooth()
        {
            ApplyOneShotEffect(img => new SmoothImageEffect().Apply(img), "Applied Smooth filter");
        }

        private void ApplyOneShotEffect(Func<SkiaSharp.SKBitmap, SkiaSharp.SKBitmap> effect, string statusMessage)
        {
            if (_editorCore == null)
            {
                return;
            }

            if (_editorCore.ApplyImageEffect(effect))
            {
                ShowEffectAppliedNotification(statusMessage);
            }
        }

        // --- Effect Live Preview Logic ---

        private SkiaSharp.SKBitmap? _preEffectImage;
        private SkiaSharp.SKBitmap? _latestEffectPreviewImage;

        /// <summary>
        /// Called when an effect dialog opens to store the state before previewing.
        /// </summary>
        public void StartEffectPreview()
        {
            SkiaSharp.SKBitmap? source = GetBestAvailableSourceBitmap();
            if (source == null)
            {
                EditorServices.ReportDebug(nameof(MainViewModel), "StartEffectPreview: no source bitmap (GetBestAvailableSourceBitmap returned null).");
                return;
            }

            SkiaSharp.SKBitmap? copy = SafeCopyBitmap(source, "StartEffectPreview");
            if (copy == null)
            {
                EditorServices.ReportDebug(nameof(MainViewModel), $"StartEffectPreview: SafeCopyBitmap failed (source {source.Width}x{source.Height}).");
                return;
            }

            EditorServices.ReportDebug(
                nameof(MainViewModel),
                $"StartEffectPreview: captured {copy.Width}x{copy.Height} for live effect preview.");

            if (_latestEffectPreviewImage != null && !ReferenceEquals(_latestEffectPreviewImage, _preEffectImage))
            {
                _latestEffectPreviewImage.Dispose();
            }
            _latestEffectPreviewImage = null;

            // ISSUE-024 fix: Dispose previous bitmap before reassignment
            _preEffectImage?.Dispose();
            _preEffectImage = copy;

            _isPreviewingEffect = true;
            OnPropertyChanged(nameof(AreBackgroundEffectsActive));
            OnPropertyChanged(nameof(EffectiveCanvasBackground));
            RefreshSmartPaddingState();
        }

        /// <summary>
        /// Updates the displayed preview without committing changes to the source image or undo stack.
        /// </summary>
        public void UpdatePreviewImageOnly(SkiaSharp.SKBitmap preview, bool syncSourceState = false)
        {
            if (!IsBitmapAlive(preview))
            {
                return;
            }

            try
            {
                _isSyncingFromCore = true;

                // SIP-FIX: Calculate dimensions string BEFORE setting PreviewImage.
                // Setting PreviewImage can trigger bindings that might dispose the source
                // via EditorCore updates if not handled carefully.
                string dimStr = $"{preview.Width} x {preview.Height}";

                PreviewImage = BitmapConversionHelpers.ToAvaloniBitmap(preview);
                ImageDimensions = dimStr;

                if (syncSourceState)
                {
                    SkiaSharp.SKBitmap? sourceCopy = SafeCopyBitmap(preview, "UpdatePreviewImageOnly.SyncCurrent");
                    if (sourceCopy != null)
                    {
                        _currentSourceImage?.Dispose();
                        _currentSourceImage = sourceCopy;

                        if (!_isApplyingSmartPadding)
                        {
                            _originalSourceImage?.Dispose();
                            _originalSourceImage = SafeCopyBitmap(sourceCopy, "UpdatePreviewImageOnly.SyncOriginal");
                        }
                    }
                }
            }
            finally
            {
                _isSyncingFromCore = false;
            }
        }

        /// <summary>
        /// ISSUE-028 fix: Common logic for committing effects and cleaning up preview state.
        /// </summary>
        private bool CommitEffectAndCleanup(SkiaSharp.SKBitmap result, string statusMessage)
        {
            SkiaSharp.SKBitmap? preEffectImage = _preEffectImage;
            SkiaSharp.SKBitmap? latestPreviewImage = _latestEffectPreviewImage;
            _latestEffectPreviewImage = null;

            if (latestPreviewImage != null &&
                !ReferenceEquals(latestPreviewImage, result) &&
                !ReferenceEquals(latestPreviewImage, preEffectImage))
            {
                latestPreviewImage.Dispose();
            }

            bool applied = false;
            bool resultTransferred = false;

            if (_editorCore != null)
            {
                // SIP-FIX: Ensure EditorCore has the original clean image before applying the effect.
                // This ensures the memento captures the pre-effect state, not the preview/intermediate state.
                if (preEffectImage != null)
                {
                    var cleanState = preEffectImage.Copy();
                    if (cleanState != null)
                    {
                        _editorCore.UpdateSourceImage(cleanState);
                    }
                }

                applied = _editorCore.ApplyImageOperation(_ => result, clearAnnotations: false);
                resultTransferred = applied;

                // SIP-FIX: Ensure ViewModel state (_currentSourceImage) matches Core state after apply.
                if (applied)
                {
                    var syncCopy = SafeCopyBitmap(result, "CommitEffect.Sync");
                    if (syncCopy != null)
                    {
                        _currentSourceImage?.Dispose();
                        _currentSourceImage = syncCopy;

                        if (!_isApplyingSmartPadding)
                        {
                            _originalSourceImage?.Dispose();
                            _originalSourceImage = SafeCopyBitmap(syncCopy, "CommitEffect.SyncOriginal");
                        }
                    }
                }

                if (!applied)
                {
                    EditorServices.ReportDebug(
                        nameof(MainViewModel),
                        $"CommitEffectAndCleanup: EditorCore.ApplyImageOperation returned false (result {result.Width}x{result.Height}).");

                    if (!ReferenceEquals(result, preEffectImage))
                    {
                        result.Dispose();
                    }
                }
            }
            else
            {
                UpdatePreview(result, clearAnnotations: false);
                applied = true;
                resultTransferred = true;
            }

            if (!(resultTransferred && ReferenceEquals(preEffectImage, result)))
            {
                preEffectImage?.Dispose();
            }
            _preEffectImage = null;

            _isPreviewingEffect = false;
            OnPropertyChanged(nameof(AreBackgroundEffectsActive));
            OnPropertyChanged(nameof(EffectiveCanvasBackground));
            RefreshSmartPaddingState(ensureCache: AreBackgroundEffectsActive);

            return applied;
        }

        /// <summary>
        /// Commits the effect to the undo stack and updates the source image.
        /// </summary>
        public bool ApplyEffect(SkiaSharp.SKBitmap result, string statusMessage)
        {
            if (_preEffectImage == null) return false; // Should have been started
            return CommitEffectAndCleanup(result, statusMessage);
        }

        /// <summary>
        /// Cancels the preview and restores the original image view.
        /// </summary>
        public void CancelEffectPreview()
        {
            // SIP-FIX: Prioritize _preEffectImage (clean state) for cancellation.
            // GetBestAvailableSourceBitmap() might return the dirty/preview state from EditorCore.
            SkiaSharp.SKBitmap? source = _preEffectImage ?? GetBestAvailableSourceBitmap();

            if (source != null)
            {
                UpdatePreviewImageOnly(source, syncSourceState: true);
            }

            if (_latestEffectPreviewImage != null && !ReferenceEquals(_latestEffectPreviewImage, _preEffectImage))
            {
                _latestEffectPreviewImage.Dispose();
            }
            _latestEffectPreviewImage = null;

            _preEffectImage?.Dispose();
            _preEffectImage = null;

            // Restore Background Effects
            _isPreviewingEffect = false;
            OnPropertyChanged(nameof(AreBackgroundEffectsActive));
            OnPropertyChanged(nameof(EffectiveCanvasBackground));
            RefreshSmartPaddingState(ensureCache: AreBackgroundEffectsActive);
        }

        /// <summary>
        /// Restores EditorCore's source image to the clean pre-effect state.
        /// Must be called before direct EditorCore operations (AutoCrop, Crop, Resize, etc.)
        /// after a preview session, because preview updates may have pushed the already-effected
        /// image into EditorCore.SourceImage via the View's PropertyChanged sync path.
        /// </summary>
        public void RestorePreEffectSourceImage()
        {
            if (_preEffectImage != null && _editorCore != null)
            {
                var cleanCopy = _preEffectImage.Copy();
                if (cleanCopy != null)
                {
                    _editorCore.UpdateSourceImage(cleanCopy);
                }
            }
        }

        /// <summary>
        /// Ends dialog-driven effect preview without committing via effect delegates.
        /// Used when a dialog should apply through <c>EditorCore</c> to keep annotation transforms unified.
        /// </summary>
        public void EndEffectPreview()
        {
            _latestEffectPreviewImage?.Dispose();
            _latestEffectPreviewImage = null;

            _preEffectImage?.Dispose();
            _preEffectImage = null;

            _isPreviewingEffect = false;
            OnPropertyChanged(nameof(AreBackgroundEffectsActive));
            OnPropertyChanged(nameof(EffectiveCanvasBackground));
            RefreshSmartPaddingState(ensureCache: AreBackgroundEffectsActive);
        }

        /// <summary>
        /// Applies the effect function to the pre-effect image and updates the preview.
        /// </summary>
        public void PreviewEffect(Func<SkiaSharp.SKBitmap, SkiaSharp.SKBitmap> effect)
        {
            if (_preEffectImage == null)
            {
                EditorServices.ReportDebug(nameof(MainViewModel), "PreviewEffect: skipped (_preEffectImage is null).");
                return;
            }

            if (effect == null)
            {
                EditorServices.ReportDebug(nameof(MainViewModel), "PreviewEffect: skipped (effect delegate is null).");
                return;
            }

            SkiaSharp.SKBitmap? result = null;

            try
            {
                result = effect(_preEffectImage);
                if (result == null)
                {
                    EditorServices.ReportDebug(nameof(MainViewModel), "PreviewEffect: effect returned null bitmap.");
                    return;
                }

                if (!IsBitmapAlive(result))
                {
                    EditorServices.ReportDebug(nameof(MainViewModel), "PreviewEffect: effect returned dead bitmap; disposing.");
                    result.Dispose();
                    return;
                }

                // Normalize to a distinct owned instance if operation returns the source itself.
                if (ReferenceEquals(result, _preEffectImage))
                {
                    SkiaSharp.SKBitmap? copied = SafeCopyBitmap(result, "PreviewEffect.ReferenceEqual");
                    if (copied == null)
                    {
                        EditorServices.ReportDebug(nameof(MainViewModel), "PreviewEffect: SafeCopyBitmap failed for ReferenceEqual result.");
                        return;
                    }

                    result = copied;
                }

                EditorServices.ReportDebug(
                    nameof(MainViewModel),
                    $"PreviewEffect: ok source={_preEffectImage.Width}x{_preEffectImage.Height} -> result={result.Width}x{result.Height} refEqSource={ReferenceEquals(result, _preEffectImage)}");

                UpdatePreviewImageOnly(result, syncSourceState: false);

                if (_latestEffectPreviewImage != null && !ReferenceEquals(_latestEffectPreviewImage, _preEffectImage))
                {
                    _latestEffectPreviewImage.Dispose();
                }

                _latestEffectPreviewImage = result;
                result = null;
            }
            catch (Exception ex)
            {
                result?.Dispose();
                EditorServices.ReportError(nameof(MainViewModel), "Failed to render live effect preview.", ex);
            }
        }

        /// <summary>
        /// Applies the effect to the source image and commits to undo stack.
        /// </summary>
        public bool ApplyEffect(Func<SkiaSharp.SKBitmap, SkiaSharp.SKBitmap> effect, string statusMessage)
        {
            if (_preEffectImage == null)
            {
                EditorServices.ReportDebug(nameof(MainViewModel), $"ApplyEffect(Func): skipped (_preEffectImage null) status={statusMessage}");
                return false;
            }

            if (IsBitmapAlive(_latestEffectPreviewImage))
            {
                SkiaSharp.SKBitmap previewResult = _latestEffectPreviewImage!;
                EditorServices.ReportDebug(
                    nameof(MainViewModel),
                    $"ApplyEffect(Func): committing latest preview bitmap {previewResult.Width}x{previewResult.Height} status={statusMessage}");
                _latestEffectPreviewImage = null;
                return CommitEffectAndCleanup(previewResult, statusMessage);
            }

            _latestEffectPreviewImage?.Dispose();
            _latestEffectPreviewImage = null;

            EditorServices.ReportDebug(nameof(MainViewModel), $"ApplyEffect(Func): running effect fresh (no preview bitmap) status={statusMessage}");
            SkiaSharp.SKBitmap? result = effect(_preEffectImage);
            if (!IsBitmapAlive(result))
            {
                EditorServices.ReportDebug(nameof(MainViewModel), "ApplyEffect(Func): effect returned null or dead bitmap.");
                result?.Dispose();
                return false;
            }

            return CommitEffectAndCleanup(result!, statusMessage);
        }

        // --- Rotate Custom Angle Feature ---

        [ObservableProperty]
        private double _rotateAngleDegrees;

        [ObservableProperty]
        private bool _isRotateCustomAngleDialogOpen;

        [ObservableProperty]
        private bool _rotateAutoResize = true;

        private SkiaSharp.SKBitmap? _rotateCustomAngleOriginalBitmap;

        [RelayCommand]
        public void OpenRotateCustomAngleDialog()
        {
            SkiaSharp.SKBitmap? source = GetBestAvailableSourceBitmap();
            if (PreviewImage == null || source == null)
            {
                return;
            }

            // Snapshot the CURRENT state (including any previous edits)
            var current = source;
            if (current != null)
            {
                // ISSUE-024 fix: Dispose previous bitmap before reassignment
                _rotateCustomAngleOriginalBitmap?.Dispose();
                var copy = SafeCopyBitmap(current, "OpenRotateCustomAngleDialog");
                if (copy == null)
                {
                    return;
                }
                _rotateCustomAngleOriginalBitmap = copy;
                RotateAngleDegrees = 0;
                IsRotateCustomAngleDialogOpen = true;
            }
        }

        partial void OnRotateAngleDegreesChanged(double value)
        {
            RotateCustomAngleLiveApply();
        }

        partial void OnRotateAutoResizeChanged(bool value)
        {
            RotateCustomAngleLiveApply();
        }

        private void RotateCustomAngleLiveApply()
        {
            if (!IsRotateCustomAngleDialogOpen || _rotateCustomAngleOriginalBitmap == null) return;

            float angle = (float)Math.Clamp(RotateAngleDegrees, -360, 360);
            var effect = RotateImageEffect.Custom(angle, RotateAutoResize);

            var result = effect.Apply(_rotateCustomAngleOriginalBitmap);

            UpdatePreviewImageOnly(result, syncSourceState: false);
            result.Dispose();
        }

        [RelayCommand]
        public void CommitRotateCustomAngle()
        {
            if (_editorCore == null || _rotateCustomAngleOriginalBitmap == null)
            {
                return;
            }

            float angle = (float)Math.Clamp(RotateAngleDegrees, -360, 360);

            // Restore the original (pre-preview) bitmap to EditorCore before applying,
            // so the rotation is only applied once (preview had already rotated the display).
            var cleanState = _rotateCustomAngleOriginalBitmap.Copy();
            if (cleanState != null)
            {
                _editorCore.UpdateSourceImage(cleanState);
            }

            RotateCustomAngle(angle, RotateAutoResize);

            IsRotateCustomAngleDialogOpen = false;
            IsModalOpen = false;
            ModalContent = null;

            _rotateCustomAngleOriginalBitmap?.Dispose();
            _rotateCustomAngleOriginalBitmap = null;
        }

        [RelayCommand]
        public void CancelRotateCustomAngle()
        {
            if (_rotateCustomAngleOriginalBitmap != null)
            {
                // Restore the original (pre-rotation) bitmap to both display and EditorCore
                UpdatePreviewImageOnly(_rotateCustomAngleOriginalBitmap, syncSourceState: true);

                var cleanState = _rotateCustomAngleOriginalBitmap.Copy();
                if (cleanState != null)
                {
                    _editorCore?.UpdateSourceImage(cleanState);
                }

                _rotateCustomAngleOriginalBitmap.Dispose();
                _rotateCustomAngleOriginalBitmap = null;
            }

            IsRotateCustomAngleDialogOpen = false;
            IsModalOpen = false;
            ModalContent = null;
        }
    }
}