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
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Core.ImageEffects.Manipulations;
using ShareX.ImageEditor.Presentation.Controls;
using ShareX.ImageEditor.Presentation.Effects;
using ShareX.ImageEditor.Presentation.ViewModels;
using ShareX.ImageEditor.Presentation.Views.Dialogs;
using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.Views
{
    public partial class EditorView : UserControl
    {
        // --- Edit Menu Event Handlers ---

        private void OnAutoCropImageRequested(object? sender, EventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.AutoCropImageCommand.Execute(null);
                vm.CloseEffectsPanelCommand.Execute(null);
            }
        }

        private void OnRotate90CWRequested(object? sender, EventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.Rotate90ClockwiseCommand.Execute(null);
                vm.CloseEffectsPanelCommand.Execute(null);
            }
        }

        private void OnRotate90CCWRequested(object? sender, EventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.Rotate90CounterClockwiseCommand.Execute(null);
                vm.CloseEffectsPanelCommand.Execute(null);
            }
        }

        private void OnRotate180Requested(object? sender, EventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.Rotate180Command.Execute(null);
                vm.CloseEffectsPanelCommand.Execute(null);
            }
        }

        private void OnFlipHorizontalRequested(object? sender, EventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.FlipHorizontalCommand.Execute(null);
                vm.CloseEffectsPanelCommand.Execute(null);
            }
        }

        private void OnFlipVerticalRequested(object? sender, EventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.FlipVerticalCommand.Execute(null);
                vm.CloseEffectsPanelCommand.Execute(null);
            }
        }

        // --- Immediate effects (no dialog) ---

        private void OnInvertRequested(object? sender, EventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.InvertColorsCommand.Execute(null);
                vm.CloseEffectsPanelCommand.Execute(null);
            }
        }

        private void OnBlackAndWhiteRequested(object? sender, EventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.BlackAndWhiteCommand.Execute(null);
                vm.CloseEffectsPanelCommand.Execute(null);
            }
        }

        private void OnPolaroidRequested(object? sender, EventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.PolaroidCommand.Execute(null);
                vm.CloseEffectsPanelCommand.Execute(null);
            }
        }

        private void OnEdgeDetectRequested(object? sender, EventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.EdgeDetectCommand.Execute(null);
                vm.CloseEffectsPanelCommand.Execute(null);
            }
        }

        private void OnEmbossRequested(object? sender, EventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.EmbossCommand.Execute(null);
                vm.CloseEffectsPanelCommand.Execute(null);
            }
        }

        private void OnMeanRemovalRequested(object? sender, EventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.MeanRemovalCommand.Execute(null);
                vm.CloseEffectsPanelCommand.Execute(null);
            }
        }

        private void OnSmoothRequested(object? sender, EventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SmoothCommand.Execute(null);
                vm.CloseEffectsPanelCommand.Execute(null);
            }
        }

        // --- XIP0039 Pain Point 3: Registry-driven dialog dispatch ---

        /// <summary>
        /// Single handler for all registry-backed effect dialogs.
        /// Adding a new dialog-based effect requires only an <see cref="EffectDialogRegistry"/>
        /// entry plus a menu item that calls <c>RaiseDialog("id")</c> — no new method here.
        /// </summary>
        private void OnEffectDialogRequested(object? sender, EffectDialogRequestedEventArgs e)
        {
            if (TryHandleEditorOperation(e.EffectId))
            {
                return;
            }

            if (TryHandleImmediateCatalogEffect(e.EffectId))
            {
                return;
            }

            if (!EffectDialogRegistry.TryCreate(e.EffectId, out var dialog) || dialog == null)
                return;

            if (dialog is IEffectDialog effectDialog)
            {
                ShowEffectDialog(dialog, effectDialog);
            }
        }

        private bool TryHandleImmediateCatalogEffect(string effectId)
        {
            if (DataContext is not MainViewModel vm || vm.PreviewImage == null)
            {
                return false;
            }

            if (!ImageEffectCatalog.TryGetDefinition(effectId, out EffectDefinition? definition) ||
                definition == null ||
                !definition.ApplyImmediately)
            {
                return false;
            }

            vm.StartEffectPreview();
            string statusMessage = $"Applied {definition.Name}";

            if (vm.ApplyEffect(
                definition.CreateConfiguredEffect(Array.Empty<EffectParameterState>()).Apply,
                statusMessage))
            {
                vm.ShowEffectAppliedNotification(statusMessage);
            }

            vm.CloseEffectsPanelCommand.Execute(null);
            return true;
        }

        private bool TryHandleEditorOperation(string effectId)
        {
            if (!EditorOperationCatalog.TryGetDefinition(effectId, out EditorOperationDefinition? operation) || operation == null)
            {
                return false;
            }

            if (operation.SchemaDefinition != null)
            {
                ShowEditorOperationDialog(operation);
                return true;
            }

            switch (operation.Kind)
            {
                case EditorOperationKind.Rotate90Clockwise:
                    OnRotate90CWRequested(this, EventArgs.Empty);
                    return true;
                case EditorOperationKind.Rotate90CounterClockwise:
                    OnRotate90CCWRequested(this, EventArgs.Empty);
                    return true;
                case EditorOperationKind.Rotate180:
                    OnRotate180Requested(this, EventArgs.Empty);
                    return true;
                case EditorOperationKind.FlipHorizontal:
                    OnFlipHorizontalRequested(this, EventArgs.Empty);
                    return true;
                case EditorOperationKind.FlipVertical:
                    OnFlipVerticalRequested(this, EventArgs.Empty);
                    return true;
                default:
                    return false;
            }
        }

        private void ShowEditorOperationDialog(EditorOperationDefinition operation)
        {
            if (operation.SchemaDefinition == null || DataContext is not MainViewModel vm)
            {
                return;
            }

            SchemaDrivenEffectDialog dialog = new(operation.SchemaDefinition);

            // Inject runtime defaults for operations that depend on current image dimensions
            if (vm.PreviewImage != null)
            {
                int imgW = (int)vm.ImageWidth;
                int imgH = (int)vm.ImageHeight;

                if (operation.Kind is EditorOperationKind.CropImage or EditorOperationKind.ResizeImage)
                {
                    foreach (EffectParameterState state in dialog.ParameterStates)
                    {
                        if (state is NumericParameterState n)
                        {
                            if (string.Equals(state.Key, "width", StringComparison.OrdinalIgnoreCase))
                                n.Value = imgW;
                            else if (string.Equals(state.Key, "height", StringComparison.OrdinalIgnoreCase))
                                n.Value = imgH;
                        }
                    }
                }
            }

            vm.StartEffectPreview();

            dialog.PreviewRequested += (s, e) => vm.PreviewEffect(e.EffectOperation);
            dialog.ApplyRequested += (s, e) =>
            {
                // Restore EditorCore source to pre-effect state. During preview, the View's
                // PropertyChanged sync path pushes the previewed image into EditorCore.SourceImage,
                // so direct EditorCore operations would otherwise apply on already-effected data.
                vm.RestorePreEffectSourceImage();

                switch (operation.Kind)
                {
                    case EditorOperationKind.AutoCropImage:
                        int tolerance = 0;
                        foreach (EffectParameterState state in dialog.ParameterStates)
                        {
                            if (string.Equals(state.Key, "tolerance", StringComparison.OrdinalIgnoreCase) &&
                                state is SliderParameterState slider)
                            {
                                tolerance = (int)Math.Round(slider.Value);
                                break;
                            }
                        }

                        vm.AutoCrop(tolerance);
                        break;
                    case EditorOperationKind.CropImage:
                        int x = 0, y = 0, w = 0, h = 0;
                        foreach (EffectParameterState state in dialog.ParameterStates)
                        {
                            if (state is NumericParameterState n)
                            {
                                switch (state.Key)
                                {
                                    case "x": x = (int)(n.Value ?? 0); break;
                                    case "y": y = (int)(n.Value ?? 0); break;
                                    case "width": w = (int)(n.Value ?? 0); break;
                                    case "height": h = (int)(n.Value ?? 0); break;
                                }
                            }
                        }

                        vm.CropImage(x, y, w, h);
                        break;
                    case EditorOperationKind.ResizeImage:
                        if (_editorCore.SourceImage != null &&
                            dialog.Definition.CreateConfiguredEffect(dialog.ParameterStates) is ResizeImageEffect resizeImageEffect)
                        {
                            SKSizeI targetSize = resizeImageEffect.GetTargetSize(_editorCore.SourceImage);

                            vm.ResizeImage(targetSize.Width, targetSize.Height);
                        }
                        break;
                    case EditorOperationKind.ResizeCanvas:
                        int top = 0, right = 0, bottom = 0, left = 0;
                        SKColor bgColor = SKColors.Transparent;
                        foreach (EffectParameterState state in dialog.ParameterStates)
                        {
                            if (state is NumericParameterState n)
                            {
                                switch (state.Key)
                                {
                                    case "top": top = (int)(n.Value ?? 0); break;
                                    case "right": right = (int)(n.Value ?? 0); break;
                                    case "bottom": bottom = (int)(n.Value ?? 0); break;
                                    case "left": left = (int)(n.Value ?? 0); break;
                                }
                            }
                            else if (state is ColorParameterState c &&
                                     string.Equals(state.Key, "background_color", StringComparison.OrdinalIgnoreCase))
                            {
                                bgColor = new SKColor(c.Value.R, c.Value.G, c.Value.B, c.Value.A);
                            }
                        }

                        vm.ResizeCanvas(top, right, bottom, left, bgColor);
                        break;
                    case EditorOperationKind.RotateCustomAngle:
                        float angle = 0;
                        bool autoResize = true;
                        foreach (EffectParameterState state in dialog.ParameterStates)
                        {
                            if (string.Equals(state.Key, "angle", StringComparison.OrdinalIgnoreCase) &&
                                state is SliderParameterState slider)
                            {
                                angle = (float)slider.Value;
                            }
                            else if (string.Equals(state.Key, "auto_resize", StringComparison.OrdinalIgnoreCase) &&
                                     state is CheckboxParameterState cb)
                            {
                                autoResize = cb.Value;
                            }
                        }

                        vm.RotateCustomAngle(angle, autoResize);
                        break;
                    default:
                        if (vm.ApplyEffect(e.EffectOperation, e.StatusMessage))
                        {
                            vm.ShowEffectAppliedNotification(e.StatusMessage);
                        }
                        break;
                }

                vm.EndEffectPreview();
                vm.CloseEffectsPanelCommand.Execute(null);
            };
            dialog.CancelRequested += (s, e) =>
            {
                vm.CancelEffectPreview();
                vm.CloseEffectsPanelCommand.Execute(null);
            };

            vm.EffectsPanelContent = dialog;
            vm.IsEffectsPanelOpen = true;
        }

        /// <summary>
        /// Wires preview/apply/cancel lifecycle for a dialog-based effect and opens the effects panel.
        /// </summary>
        private void ShowEffectDialog(UserControl dialog, IEffectDialog effectDialog)
        {
            var vm = DataContext as MainViewModel;
            if (vm == null) return;

            vm.StartEffectPreview();

            effectDialog.PreviewRequested += (s, e) => vm.PreviewEffect(e.EffectOperation);
            effectDialog.ApplyRequested += (s, e) =>
            {
                if (vm.ApplyEffect(e.EffectOperation, e.StatusMessage))
                {
                    vm.ShowEffectAppliedNotification(e.StatusMessage);
                }
                vm.CloseEffectsPanelCommand.Execute(null);
            };
            effectDialog.CancelRequested += (s, e) =>
            {
                vm.CancelEffectPreview();
                vm.CloseEffectsPanelCommand.Execute(null);
            };

            vm.EffectsPanelContent = dialog;
            vm.IsEffectsPanelOpen = true;
        }

        private void OnModalBackgroundPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            // Only close if clicking on the background, not the dialog content
            if (e.Source == sender && DataContext is MainViewModel vm)
            {
                vm.CancelEffectPreview();
                vm.CloseModalCommand.Execute(null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Validates that UI annotation state is synchronized with EditorCore state.
        /// ISSUE-001 mitigation: Detect annotation count mismatches in dual-state architecture.
        /// </summary>
        private void ValidateAnnotationSync()
        {
            var canvas = this.FindControl<Canvas>("AnnotationCanvas");
            if (canvas == null) return;

            int uiAnnotationCount = 0;
            foreach (var child in canvas.Children)
            {
                if (child is Control control && control.Tag is Annotation &&
                    control.Name != "CropOverlay" && control.Name != "CutOutOverlay")
                {
                    uiAnnotationCount++;
                }
            }

            int coreAnnotationCount = _editorCore.Annotations.Count;
        }
    }
}
