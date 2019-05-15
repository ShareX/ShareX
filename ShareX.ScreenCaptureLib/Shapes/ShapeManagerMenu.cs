#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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

using ShareX.HelpersLib;
using ShareX.ScreenCaptureLib.Properties;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    internal partial class ShapeManager
    {
        public bool ToolbarCreated { get; private set; }
        public bool ToolbarCollapsed { get; private set; }

        internal TextAnimation MenuTextAnimation = new TextAnimation()
        {
            FadeInDuration = TimeSpan.FromMilliseconds(0),
            Duration = TimeSpan.FromMilliseconds(5000),
            FadeOutDuration = TimeSpan.FromMilliseconds(500)
        };

        private Form menuForm;
        private ToolStripEx tsMain;
        private ToolStripButton tsbSaveImage, tsbBorderColor, tsbFillColor, tsbHighlightColor;
        private ToolStripDropDownButton tsddbShapeOptions;
        private ToolStripMenuItem tsmiArrowHeadsBothSide, tsmiShadow, tsmiShadowColor, tsmiStepUseLetters, tsmiUndo, tsmiDelete, tsmiDeleteAll, tsmiMoveTop,
            tsmiMoveUp, tsmiMoveDown, tsmiMoveBottom, tsmiRegionCapture, tsmiQuickCrop, tsmiShowMagnifier, tsmiImageEditorBackgroundColor;
        private ToolStripLabeledNumericUpDown tslnudBorderSize, tslnudCornerRadius, tslnudCenterPoints, tslnudBlurRadius, tslnudPixelateSize, tslnudStepFontSize,
            tslnudMagnifierPixelCount, tslnudStartingStepValue, tslnudMagnifyStrength;
        private ToolStripLabel tslDragLeft, tslDragRight;
        private ToolStripLabeledComboBox tscbImageInterpolationMode, tscbCursorTypes;

        internal void CreateToolbar()
        {
            menuForm = new Form()
            {
                AutoScaleDimensions = new SizeF(6F, 13F),
                AutoScaleMode = AutoScaleMode.Font,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ClientSize = new Size(759, 509),
                FormBorderStyle = FormBorderStyle.None,
                Location = new Point(200, 200),
                ShowInTaskbar = false,
                StartPosition = FormStartPosition.Manual,
                Text = "ShareX - " + Resources.ShapeManager_CreateToolbar_AnnotateMenu,
                TopMost = Form.IsFullscreen
            };

            menuForm.Shown += MenuForm_Shown;
            menuForm.KeyDown += MenuForm_KeyDown;
            menuForm.KeyUp += MenuForm_KeyUp;
            menuForm.LocationChanged += MenuForm_LocationChanged;
            menuForm.GotFocus += MenuForm_GotFocus;
            menuForm.LostFocus += MenuForm_LostFocus;

            menuForm.SuspendLayout();

            tsMain = new ToolStripEx()
            {
                AutoSize = true,
                CanOverflow = false,
                ClickThrough = true,
                Dock = DockStyle.None,
                GripStyle = ToolStripGripStyle.Hidden,
                Location = new Point(0, 0),
                MinimumSize = new Size(10, 30),
                Padding = Form.IsEditorMode ? new Padding(5, 1, 3, 0) : new Padding(0, 1, 0, 0),
                Renderer = new ToolStripRoundedEdgeRenderer(),
                TabIndex = 0,
                ShowItemToolTips = false
            };

            tsMain.MouseLeave += TsMain_MouseLeave;

            tsMain.SuspendLayout();

            menuForm.Controls.Add(tsMain);

            if (Form.IsFullscreen)
            {
                tslDragLeft = new ToolStripLabel()
                {
                    DisplayStyle = ToolStripItemDisplayStyle.Image,
                    Image = Resources.ui_radio_button_uncheck,
                    Margin = new Padding(0, 0, 2, 0),
                    Padding = new Padding(2)
                };

                tsMain.Items.Add(tslDragLeft);
            }

            if (Form.IsEditorMode)
            {
                #region Editor mode

                ToolStripButton tsbCompleteEdit = new ToolStripButton();

                if (Form.Mode == RegionCaptureMode.Editor)
                {
                    tsbCompleteEdit.Text = Resources.ShapeManager_CreateToolbar_RunAfterCaptureTasks;
                }
                else
                {
                    tsbCompleteEdit.Text = Resources.ShapeManager_CreateToolbar_ApplyChangesContinueTaskEnter;
                }

                tsbCompleteEdit.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbCompleteEdit.Image = Resources.tick;
                tsbCompleteEdit.Click += (sender, e) => Form.CloseWindow(RegionResult.AnnotateRunAfterCaptureTasks);
                tsMain.Items.Add(tsbCompleteEdit);

                if (Form.Mode == RegionCaptureMode.TaskEditor)
                {
                    ToolStripButton tsbContinueTask = new ToolStripButton(Resources.ShapeManager_CreateToolbar_ContinueTaskSpaceOrRightClick);
                    tsbContinueTask.DisplayStyle = ToolStripItemDisplayStyle.Image;
                    tsbContinueTask.Image = Resources.control;
                    tsbContinueTask.Click += (sender, e) => Form.CloseWindow(RegionResult.AnnotateContinueTask);
                    tsMain.Items.Add(tsbContinueTask);

                    ToolStripButton tsbCancelTask = new ToolStripButton(Resources.ShapeManager_CreateToolbar_CancelTaskEsc);
                    tsbCancelTask.DisplayStyle = ToolStripItemDisplayStyle.Image;
                    tsbCancelTask.Image = Resources.cross;
                    tsbCancelTask.Click += (sender, e) => Form.CloseWindow(RegionResult.AnnotateCancelTask);
                    tsMain.Items.Add(tsbCancelTask);

                    tsMain.Items.Add(new ToolStripSeparator());
                }

                tsbSaveImage = new ToolStripButton(Resources.ShapeManager_CreateToolbar_SaveImage);
                tsbSaveImage.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbSaveImage.Image = Resources.disk_black;
                tsbSaveImage.Click += (sender, e) => Form.OnSaveImageRequested();
                tsMain.Items.Add(tsbSaveImage);

                ToolStripButton tsbSaveImageAs = new ToolStripButton(Resources.ShapeManager_CreateToolbar_SaveImageAs);
                tsbSaveImageAs.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbSaveImageAs.Image = Resources.disks_black;
                tsbSaveImageAs.Click += (sender, e) => Form.OnSaveImageAsRequested();
                tsMain.Items.Add(tsbSaveImageAs);

                ToolStripButton tsbCopyImage = new ToolStripButton(Resources.ShapeManager_CreateToolbar_CopyImageToClipboard);
                tsbCopyImage.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbCopyImage.Image = Resources.clipboard;
                tsbCopyImage.Click += (sender, e) => Form.OnCopyImageRequested();
                tsMain.Items.Add(tsbCopyImage);

                ToolStripButton tsbUploadImage = new ToolStripButton(Resources.ShapeManager_CreateToolbar_UploadImage);
                tsbUploadImage.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbUploadImage.Image = Resources.drive_globe;
                tsbUploadImage.Click += (sender, e) => Form.OnUploadImageRequested();
                tsMain.Items.Add(tsbUploadImage);

                ToolStripButton tsbPrintImage = new ToolStripButton(Resources.ShapeManager_CreateToolbar_PrintImage);
                tsbPrintImage.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbPrintImage.Image = Resources.printer;
                tsbPrintImage.Click += (sender, e) => Form.OnPrintImageRequested();
                tsMain.Items.Add(tsbPrintImage);

                tsMain.Items.Add(new ToolStripSeparator());

                #endregion Editor mode
            }
            else if (Helpers.IsTabletMode())
            {
                ToolStripButton tsbClose = new ToolStripButton("Close (Esc)");
                tsbClose.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbClose.Image = Resources.cross;
                tsbClose.Click += (sender, e) => Form.CloseWindow();
                tsMain.Items.Add(tsbClose);

                tsMain.Items.Add(new ToolStripSeparator());
            }

            #region Tools

            foreach (ShapeType shapeType in Helpers.GetEnums<ShapeType>())
            {
                if (Form.IsEditorMode)
                {
                    if (IsShapeTypeRegion(shapeType))
                    {
                        continue;
                    }
                }
                else if (shapeType == ShapeType.ToolSelect)
                {
                    tsMain.Items.Add(new ToolStripSeparator());
                }
                else if (shapeType == ShapeType.ToolCrop)
                {
                    continue;
                }

                ToolStripButton tsbShapeType = new ToolStripButton(shapeType.GetLocalizedDescription());
                tsbShapeType.DisplayStyle = ToolStripItemDisplayStyle.Image;

                Image img = null;

                switch (shapeType)
                {
                    case ShapeType.RegionRectangle:
                        img = Resources.layer_shape_region;
                        break;
                    case ShapeType.RegionEllipse:
                        img = Resources.layer_shape_ellipse_region;
                        break;
                    case ShapeType.RegionFreehand:
                        img = Resources.layer_shape_polygon;
                        break;
                    case ShapeType.DrawingRectangle:
                        img = Resources.layer_shape;
                        break;
                    case ShapeType.DrawingEllipse:
                        img = Resources.layer_shape_ellipse;
                        break;
                    case ShapeType.DrawingFreehand:
                        img = Resources.pencil;
                        break;
                    case ShapeType.DrawingLine:
                        img = Resources.layer_shape_line;
                        break;
                    case ShapeType.DrawingArrow:
                        img = Resources.layer_shape_arrow;
                        break;
                    case ShapeType.DrawingTextOutline:
                        img = Resources.edit_outline;
                        break;
                    case ShapeType.DrawingTextBackground:
                        img = Resources.edit_shade;
                        break;
                    case ShapeType.DrawingSpeechBalloon:
                        img = Resources.balloon_box_left;
                        break;
                    case ShapeType.DrawingStep:
                        img = Resources.counter_reset;
                        break;
                    case ShapeType.DrawingMagnify:
                        img = Resources.magnifier_zoom;
                        break;
                    case ShapeType.DrawingImage:
                        img = Resources.folder_open_image;
                        break;
                    case ShapeType.DrawingImageScreen:
                        img = Resources.monitor_image;
                        break;
                    case ShapeType.DrawingSticker:
                        if (MathHelpers.Random(1, 10) == 1)
                        {
                            img = Resources.smiley_cool;
                        }
                        else
                        {
                            img = Resources.smiley_yell;
                        }
                        break;
                    case ShapeType.DrawingCursor:
                        img = Resources.stamp_cursor;
                        break;
                    case ShapeType.EffectBlur:
                        img = Resources.layer_shade;
                        break;
                    case ShapeType.EffectPixelate:
                        img = Resources.grid;
                        break;
                    case ShapeType.EffectHighlight:
                        img = Resources.highlighter_text;
                        break;
                    case ShapeType.ToolCrop:
                        img = Resources.image_crop;
                        break;
                    case ShapeType.ToolSelect:
                        img = Resources.cursor;
                        break;
                }

                tsbShapeType.Image = img;
                tsbShapeType.Checked = shapeType == CurrentTool;
                tsbShapeType.Tag = shapeType;

                tsbShapeType.MouseDown += (sender, e) =>
                {
                    tsbShapeType.RadioCheck();
                    CurrentTool = shapeType;
                };

                tsMain.Items.Add(tsbShapeType);
            }

            #endregion Tools

            #region Shape options

            tsMain.Items.Add(new ToolStripSeparator());

            tsbBorderColor = new ToolStripButton(Resources.ShapeManager_CreateContextMenu_Border_color___);
            tsbBorderColor.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbBorderColor.Click += (sender, e) =>
            {
                Form.Pause();

                ShapeType shapeType = CurrentTool;

                Color borderColor;

                if (shapeType == ShapeType.DrawingTextBackground || shapeType == ShapeType.DrawingSpeechBalloon)
                {
                    borderColor = AnnotationOptions.TextBorderColor;
                }
                else if (shapeType == ShapeType.DrawingTextOutline)
                {
                    borderColor = AnnotationOptions.TextOutlineBorderColor;
                }
                else if (shapeType == ShapeType.DrawingStep)
                {
                    borderColor = AnnotationOptions.StepBorderColor;
                }
                else
                {
                    borderColor = AnnotationOptions.BorderColor;
                }

                if (PickColor(borderColor, out Color newColor))
                {
                    if (shapeType == ShapeType.DrawingTextBackground || shapeType == ShapeType.DrawingSpeechBalloon)
                    {
                        AnnotationOptions.TextBorderColor = newColor;
                    }
                    else if (shapeType == ShapeType.DrawingTextOutline)
                    {
                        AnnotationOptions.TextOutlineBorderColor = newColor;
                    }
                    else if (shapeType == ShapeType.DrawingStep)
                    {
                        AnnotationOptions.StepBorderColor = newColor;
                    }
                    else
                    {
                        AnnotationOptions.BorderColor = newColor;
                    }

                    UpdateMenu();
                    UpdateCurrentShape();
                }

                Form.Resume();
            };
            tsMain.Items.Add(tsbBorderColor);

            tsbFillColor = new ToolStripButton(Resources.ShapeManager_CreateContextMenu_Fill_color___);
            tsbFillColor.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbFillColor.Click += (sender, e) =>
            {
                Form.Pause();

                ShapeType shapeType = CurrentTool;

                Color fillColor;

                if (shapeType == ShapeType.DrawingTextBackground || shapeType == ShapeType.DrawingSpeechBalloon)
                {
                    fillColor = AnnotationOptions.TextFillColor;
                }
                else if (shapeType == ShapeType.DrawingStep)
                {
                    fillColor = AnnotationOptions.StepFillColor;
                }
                else
                {
                    fillColor = AnnotationOptions.FillColor;
                }

                if (PickColor(fillColor, out Color newColor))
                {
                    if (shapeType == ShapeType.DrawingTextBackground || shapeType == ShapeType.DrawingSpeechBalloon)
                    {
                        AnnotationOptions.TextFillColor = newColor;
                    }
                    else if (shapeType == ShapeType.DrawingStep)
                    {
                        AnnotationOptions.StepFillColor = newColor;
                    }
                    else
                    {
                        AnnotationOptions.FillColor = newColor;
                    }

                    UpdateMenu();
                    UpdateCurrentShape();
                }

                Form.Resume();
            };
            tsMain.Items.Add(tsbFillColor);

            tsbHighlightColor = new ToolStripButton(Resources.ShapeManager_CreateContextMenu_Highlight_color___);
            tsbHighlightColor.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbHighlightColor.Click += (sender, e) =>
            {
                Form.Pause();

                if (PickColor(AnnotationOptions.HighlightColor, out Color newColor))
                {
                    AnnotationOptions.HighlightColor = newColor;
                    UpdateMenu();
                    UpdateCurrentShape();
                }

                Form.Resume();
            };
            tsMain.Items.Add(tsbHighlightColor);

            tsddbShapeOptions = new ToolStripDropDownButton(Resources.ShapeManager_CreateToolbar_ShapeOptions);
            tsddbShapeOptions.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsddbShapeOptions.Image = Resources.layer__pencil;
            tsMain.Items.Add(tsddbShapeOptions);

            // TODO: Translate
            tslnudMagnifyStrength = new ToolStripLabeledNumericUpDown("Magnify strength:");
            tslnudMagnifyStrength.Content.Text2 = "%";
            tslnudMagnifyStrength.Content.Minimum = 100;
            tslnudMagnifyStrength.Content.Maximum = 1000;
            tslnudMagnifyStrength.Content.Increment = 100;
            tslnudMagnifyStrength.Content.ValueChanged = (sender, e) =>
            {
                AnnotationOptions.MagnifyStrength = (int)tslnudMagnifyStrength.Content.Value;
                UpdateCurrentShape();
            };
            tsddbShapeOptions.DropDownItems.Add(tslnudMagnifyStrength);

            tslnudBorderSize = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Border_size_);
            tslnudBorderSize.Content.Minimum = 0;
            tslnudBorderSize.Content.Maximum = 20;
            tslnudBorderSize.Content.ValueChanged = (sender, e) =>
            {
                ShapeType shapeType = CurrentTool;

                int borderSize = (int)tslnudBorderSize.Content.Value;

                if (shapeType == ShapeType.DrawingTextBackground || shapeType == ShapeType.DrawingSpeechBalloon)
                {
                    AnnotationOptions.TextBorderSize = borderSize;
                }
                else if (shapeType == ShapeType.DrawingTextOutline)
                {
                    AnnotationOptions.TextOutlineBorderSize = borderSize;
                }
                else if (shapeType == ShapeType.DrawingStep)
                {
                    AnnotationOptions.StepBorderSize = borderSize;
                }
                else
                {
                    AnnotationOptions.BorderSize = borderSize;
                }

                UpdateCurrentShape();
            };
            tsddbShapeOptions.DropDownItems.Add(tslnudBorderSize);

            tslnudCornerRadius = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Corner_radius_);
            tslnudCornerRadius.Content.Minimum = 0;
            tslnudCornerRadius.Content.Maximum = 150;
            tslnudCornerRadius.Content.ValueChanged = (sender, e) =>
            {
                ShapeType shapeType = CurrentTool;

                if (shapeType == ShapeType.RegionRectangle)
                {
                    AnnotationOptions.RegionCornerRadius = (int)tslnudCornerRadius.Content.Value;
                }
                else if (shapeType == ShapeType.DrawingRectangle || shapeType == ShapeType.DrawingTextBackground)
                {
                    AnnotationOptions.DrawingCornerRadius = (int)tslnudCornerRadius.Content.Value;
                }

                UpdateCurrentShape();
            };
            tsddbShapeOptions.DropDownItems.Add(tslnudCornerRadius);

            tscbImageInterpolationMode = new ToolStripLabeledComboBox(Resources.ShapeManager_CreateToolbar_InterpolationMode);
            tscbImageInterpolationMode.Content.AddRange(Helpers.GetLocalizedEnumDescriptions<ImageEditorInterpolationMode>());
            tscbImageInterpolationMode.Content.SelectedIndexChanged += (sender, e) =>
            {
                AnnotationOptions.ImageInterpolationMode = (ImageEditorInterpolationMode)tscbImageInterpolationMode.Content.SelectedIndex;
                tscbImageInterpolationMode.Invalidate();
                UpdateCurrentShape();
            };
            tsddbShapeOptions.DropDownItems.Add(tscbImageInterpolationMode);

            tscbCursorTypes = new ToolStripLabeledComboBox(Resources.ShapeManager_CursorType);
            CursorConverter cursorConverter = new CursorConverter();
            foreach (Cursor cursor in Helpers.CursorList)
            {
                string name = cursorConverter.ConvertToString(cursor);
                tscbCursorTypes.Content.Add(name);
            }
            tscbCursorTypes.Content.SelectedIndex = 3; // Cursors.Default
            tscbCursorTypes.Content.SelectedIndexChanged += (sender, e) => UpdateCurrentShape();
            tsddbShapeOptions.DropDownItems.Add(tscbCursorTypes);

            tslnudBlurRadius = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Blur_radius_);
            tslnudBlurRadius.Content.Minimum = 3;
            tslnudBlurRadius.Content.Maximum = 199;
            tslnudBlurRadius.Content.Increment = 2;
            tslnudBlurRadius.Content.ValueChanged = (sender, e) =>
            {
                AnnotationOptions.BlurRadius = (int)tslnudBlurRadius.Content.Value;
                UpdateCurrentShape();
            };
            tsddbShapeOptions.DropDownItems.Add(tslnudBlurRadius);

            tslnudPixelateSize = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Pixel_size_);
            tslnudPixelateSize.Content.Minimum = 2;
            tslnudPixelateSize.Content.Maximum = 10000;
            tslnudPixelateSize.Content.ValueChanged = (sender, e) =>
            {
                AnnotationOptions.PixelateSize = (int)tslnudPixelateSize.Content.Value;
                UpdateCurrentShape();
            };
            tsddbShapeOptions.DropDownItems.Add(tslnudPixelateSize);

            tslnudCenterPoints = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CenterPoints);
            tslnudCenterPoints.Content.Minimum = 0;
            tslnudCenterPoints.Content.Maximum = LineDrawingShape.MaximumCenterPointCount;
            tslnudCenterPoints.Content.ValueChanged = (sender, e) =>
            {
                AnnotationOptions.LineCenterPointCount = (int)tslnudCenterPoints.Content.Value;
                UpdateCurrentShape();
            };
            tsddbShapeOptions.DropDownItems.Add(tslnudCenterPoints);

            tsmiArrowHeadsBothSide = new ToolStripMenuItem(Resources.ShapeManager_ArrowsOnBothEnds);
            tsmiArrowHeadsBothSide.CheckOnClick = true;
            tsmiArrowHeadsBothSide.Click += (sender, e) =>
            {
                AnnotationOptions.ArrowHeadsBothSide = tsmiArrowHeadsBothSide.Checked;
                UpdateCurrentShape();
            };
            tsddbShapeOptions.DropDownItems.Add(tsmiArrowHeadsBothSide);

            tslnudStepFontSize = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateToolbar_FontSize);
            tslnudStepFontSize.Content.Minimum = 10;
            tslnudStepFontSize.Content.Maximum = 100;
            tslnudStepFontSize.Content.ValueChanged = (sender, e) =>
            {
                AnnotationOptions.StepFontSize = (int)tslnudStepFontSize.Content.Value;
                UpdateCurrentShape();
            };
            tsddbShapeOptions.DropDownItems.Add(tslnudStepFontSize);

            tslnudStartingStepValue = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateToolbar_StartingStepValue);
            tslnudStartingStepValue.Content.Minimum = 1;
            tslnudStartingStepValue.Content.Maximum = 10000;
            tslnudStartingStepValue.Content.ValueChanged = (sender, e) =>
            {
                StartingStepNumber = (int)tslnudStartingStepValue.Content.Value;
                UpdateCurrentShape();
            };
            tsddbShapeOptions.DropDownItems.Add(tslnudStartingStepValue);

            tsmiStepUseLetters = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_UseLetters);
            tsmiStepUseLetters.Checked = false;
            tsmiStepUseLetters.CheckOnClick = true;
            tsmiStepUseLetters.Click += (sender, e) =>
            {
                AnnotationOptions.StepUseLetters = tsmiStepUseLetters.Checked;
                UpdateCurrentShape();
            };
            tsddbShapeOptions.DropDownItems.Add(tsmiStepUseLetters);

            tsmiShadow = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_DropShadow);
            tsmiShadow.Checked = true;
            tsmiShadow.CheckOnClick = true;
            tsmiShadow.Click += (sender, e) =>
            {
                AnnotationOptions.Shadow = tsmiShadow.Checked;
                UpdateCurrentShape();
            };
            tsddbShapeOptions.DropDownItems.Add(tsmiShadow);

            tsmiShadowColor = new ToolStripMenuItem(Resources.DropShadowColor);
            tsmiShadowColor.Click += (sender, e) =>
            {
                Form.Pause();

                if (PickColor(AnnotationOptions.ShadowColor, out Color newColor))
                {
                    AnnotationOptions.ShadowColor = newColor;
                    UpdateMenu();
                    UpdateCurrentShape();
                }

                Form.Resume();
            };
            tsddbShapeOptions.DropDownItems.Add(tsmiShadowColor);

            // In dropdown menu if only last item is visible then menu opens at 0, 0 position on first open, so need to add dummy item to solve this weird bug...
            tsddbShapeOptions.DropDownItems.Add(new ToolStripSeparator() { Visible = false });

            #endregion Shape options

            #region Edit

            ToolStripDropDownButton tsddbEdit = new ToolStripDropDownButton(Resources.ShapeManager_CreateToolbar_Edit);
            tsddbEdit.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsddbEdit.Image = Resources.wrench_screwdriver;
            tsMain.Items.Add(tsddbEdit);

            tsmiUndo = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_Undo);
            tsmiUndo.Image = Resources.arrow_circle_225_left;
            tsmiUndo.ShortcutKeyDisplayString = "Ctrl+Z";
            tsmiUndo.Click += (sender, e) => UndoShape();
            tsddbEdit.DropDownItems.Add(tsmiUndo);

            ToolStripMenuItem tsmiPaste = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_PasteImageText);
            tsmiPaste.Image = Resources.clipboard;
            tsmiPaste.ShortcutKeyDisplayString = "Ctrl+V";
            tsmiPaste.Click += (sender, e) => PasteFromClipboard(false);
            tsddbEdit.DropDownItems.Add(tsmiPaste);

            tsddbEdit.DropDownItems.Add(new ToolStripSeparator());

            tsmiDelete = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_Delete);
            tsmiDelete.Image = Resources.layer__minus;
            tsmiDelete.ShortcutKeyDisplayString = "Del";
            tsmiDelete.Click += (sender, e) => DeleteCurrentShape();
            tsddbEdit.DropDownItems.Add(tsmiDelete);

            tsmiDeleteAll = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_DeleteAll);
            tsmiDeleteAll.Image = Resources.eraser;
            tsmiDeleteAll.ShortcutKeyDisplayString = "Shift+Del";
            tsmiDeleteAll.Click += (sender, e) => DeleteAllShapes();
            tsddbEdit.DropDownItems.Add(tsmiDeleteAll);

            tsddbEdit.DropDownItems.Add(new ToolStripSeparator());

            tsmiMoveTop = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_BringToFront);
            tsmiMoveTop.Image = Resources.layers_stack_arrange;
            tsmiMoveTop.ShortcutKeyDisplayString = "Home";
            tsmiMoveTop.Click += (sender, e) => MoveCurrentShapeTop();
            tsddbEdit.DropDownItems.Add(tsmiMoveTop);

            tsmiMoveUp = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_BringForward);
            tsmiMoveUp.Image = Resources.layers_arrange;
            tsmiMoveUp.ShortcutKeyDisplayString = "Page up";
            tsmiMoveUp.Click += (sender, e) => MoveCurrentShapeUp();
            tsddbEdit.DropDownItems.Add(tsmiMoveUp);

            tsmiMoveDown = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_SendBackward);
            tsmiMoveDown.Image = Resources.layers_arrange_back;
            tsmiMoveDown.ShortcutKeyDisplayString = "Page down";
            tsmiMoveDown.Click += (sender, e) => MoveCurrentShapeDown();
            tsddbEdit.DropDownItems.Add(tsmiMoveDown);

            tsmiMoveBottom = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_SendToBack);
            tsmiMoveBottom.Image = Resources.layers_stack_arrange_back;
            tsmiMoveBottom.ShortcutKeyDisplayString = "End";
            tsmiMoveBottom.Click += (sender, e) => MoveCurrentShapeBottom();
            tsddbEdit.DropDownItems.Add(tsmiMoveBottom);

            #endregion Edit

            if (Form.IsEditorMode)
            {
                #region Image

                ToolStripDropDownButton tsddbImage = new ToolStripDropDownButton(Resources.ShapeManager_CreateToolbar_Image);
                tsddbImage.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsddbImage.Image = Resources.image__pencil;
                tsMain.Items.Add(tsddbImage);

                ToolStripMenuItem tsmiNewImage = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_NewImage);
                tsmiNewImage.Image = Resources.image_empty;
                tsmiNewImage.Click += (sender, e) => NewImage();
                tsddbImage.DropDownItems.Add(tsmiNewImage);

                ToolStripMenuItem tsmiOpenImage = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_OpenImageFile);
                tsmiOpenImage.Image = Resources.folder_open_image;
                tsmiOpenImage.Click += (sender, e) => OpenImageFile();
                tsddbImage.DropDownItems.Add(tsmiOpenImage);

                ToolStripMenuItem tsmiInsertImageFile = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_InsertImageFile);
                tsmiInsertImageFile.Image = Resources.image__plus;
                tsmiInsertImageFile.Click += (sender, e) => InsertImageFile();
                tsddbImage.DropDownItems.Add(tsmiInsertImageFile);

                ToolStripMenuItem tsmiInsertImageFromScreen = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_InsertImageFromScreen);
                tsmiInsertImageFromScreen.Image = Resources.camera;
                tsmiInsertImageFromScreen.Click += (sender, e) => InsertImageFromScreen();
                tsddbImage.DropDownItems.Add(tsmiInsertImageFromScreen);

                tsddbImage.DropDownItems.Add(new ToolStripSeparator());

                ToolStripMenuItem tsmiImageSize = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_ImageSize);
                tsmiImageSize.Image = Resources.image_select;
                tsmiImageSize.Click += (sender, e) => ChangeImageSize();
                tsddbImage.DropDownItems.Add(tsmiImageSize);

                ToolStripMenuItem tsmiCanvasSize = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_CanvasSize);
                tsmiCanvasSize.Image = Resources.image_resize;
                tsmiCanvasSize.Click += (sender, e) => ChangeCanvasSize();
                tsddbImage.DropDownItems.Add(tsmiCanvasSize);

                ToolStripMenuItem tsmiCropImage = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_CropImage);
                tsmiCropImage.Image = Resources.image_crop;
                tsmiCropImage.Click += (sender, e) => AddCropTool();
                tsddbImage.DropDownItems.Add(tsmiCropImage);

                ToolStripMenuItem tsmiAutoCropImage = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_AutoCropImage);
                tsmiAutoCropImage.Image = Resources.image_resize_actual;
                tsmiAutoCropImage.Click += (sender, e) => AutoCropImage();
                tsddbImage.DropDownItems.Add(tsmiAutoCropImage);

                tsddbImage.DropDownItems.Add(new ToolStripSeparator());

                ToolStripMenuItem tsmiRotate90Clockwise = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_Rotate90Clockwise);
                tsmiRotate90Clockwise.Image = Resources.arrow_circle;
                tsmiRotate90Clockwise.Click += (sender, e) => RotateImage(RotateFlipType.Rotate90FlipNone);
                tsddbImage.DropDownItems.Add(tsmiRotate90Clockwise);

                ToolStripMenuItem tsmiRotate90CounterClockwise = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_Rotate90CounterClockwise);
                tsmiRotate90CounterClockwise.Image = Resources.arrow_circle_135_left;
                tsmiRotate90CounterClockwise.Click += (sender, e) => RotateImage(RotateFlipType.Rotate270FlipNone);
                tsddbImage.DropDownItems.Add(tsmiRotate90CounterClockwise);

                ToolStripMenuItem tsmiRotate180 = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_Rotate180);
                tsmiRotate180.Image = Resources.arrow_circle_double;
                tsmiRotate180.Click += (sender, e) => RotateImage(RotateFlipType.Rotate180FlipNone);
                tsddbImage.DropDownItems.Add(tsmiRotate180);

                tsddbImage.DropDownItems.Add(new ToolStripSeparator());

                ToolStripMenuItem tsmiFlipHorizontal = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_FlipHorizontal);
                tsmiFlipHorizontal.Image = Resources.layer_flip;
                tsmiFlipHorizontal.Click += (sender, e) => RotateImage(RotateFlipType.RotateNoneFlipX);
                tsddbImage.DropDownItems.Add(tsmiFlipHorizontal);

                ToolStripMenuItem tsmiFlipVertical = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_FlipVertical);
                tsmiFlipVertical.Image = Resources.layer_flip_vertical;
                tsmiFlipVertical.Click += (sender, e) => RotateImage(RotateFlipType.RotateNoneFlipY);
                tsddbImage.DropDownItems.Add(tsmiFlipVertical);

                tsddbImage.DropDownItems.Add(new ToolStripSeparator());

                ToolStripMenuItem tsmiAddImageEffects = new ToolStripMenuItem(Resources.ImageEffects);
                tsmiAddImageEffects.Image = Resources.image_saturation;
                tsmiAddImageEffects.Click += (sender, e) => AddImageEffects();
                tsddbImage.DropDownItems.Add(tsmiAddImageEffects);

                #endregion Image
            }

            tsMain.Items.Add(new ToolStripSeparator());

            if (!Form.IsEditorMode)
            {
                #region Capture

                ToolStripDropDownButton tsddbCapture = new ToolStripDropDownButton(Resources.ShapeManager_CreateContextMenu_Capture);
                tsddbCapture.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsddbCapture.Image = Resources.camera;
                tsMain.Items.Add(tsddbCapture);

                tsmiRegionCapture = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_CaptureRegions);
                tsmiRegionCapture.Image = Resources.layer;
                tsmiRegionCapture.ShortcutKeyDisplayString = "Enter";
                tsmiRegionCapture.Click += (sender, e) =>
                {
                    Form.UpdateRegionPath();
                    Form.CloseWindow(RegionResult.Region);
                };
                tsddbCapture.DropDownItems.Add(tsmiRegionCapture);

                if (RegionCaptureForm.LastRegionFillPath != null)
                {
                    ToolStripMenuItem tsmiLastRegionCapture = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_LastRegion);
                    tsmiLastRegionCapture.Image = Resources.layers;
                    tsmiLastRegionCapture.Click += (sender, e) => Form.CloseWindow(RegionResult.LastRegion);
                    tsddbCapture.DropDownItems.Add(tsmiLastRegionCapture);
                }

                ToolStripMenuItem tsmiFullscreenCapture = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Capture_fullscreen);
                tsmiFullscreenCapture.Image = Resources.layer_fullscreen;
                tsmiFullscreenCapture.ShortcutKeyDisplayString = "Space";
                tsmiFullscreenCapture.Click += (sender, e) => Form.CloseWindow(RegionResult.Fullscreen);
                tsddbCapture.DropDownItems.Add(tsmiFullscreenCapture);

                ToolStripMenuItem tsmiActiveMonitorCapture = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Capture_active_monitor);
                tsmiActiveMonitorCapture.Image = Resources.monitor;
                tsmiActiveMonitorCapture.ShortcutKeyDisplayString = "~";
                tsmiActiveMonitorCapture.Click += (sender, e) => Form.CloseWindow(RegionResult.ActiveMonitor);
                tsddbCapture.DropDownItems.Add(tsmiActiveMonitorCapture);

                ToolStripMenuItem tsmiMonitorCapture = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Capture_monitor);
                tsmiMonitorCapture.HideImageMargin();
                tsmiMonitorCapture.Image = Resources.monitor_window;
                tsddbCapture.DropDownItems.Add(tsmiMonitorCapture);

                Screen[] screens = Screen.AllScreens;

                for (int i = 0; i < screens.Length; i++)
                {
                    Screen screen = screens[i];
                    ToolStripMenuItem tsmi = new ToolStripMenuItem($"{screen.Bounds.Width}x{screen.Bounds.Height}");
                    tsmi.ShortcutKeyDisplayString = (i + 1).ToString();
                    int index = i;
                    tsmi.Click += (sender, e) =>
                    {
                        Form.MonitorIndex = index;
                        Form.CloseWindow(RegionResult.Monitor);
                    };
                    tsmiMonitorCapture.DropDownItems.Add(tsmi);
                }

                #endregion Capture
            }

            #region Options

            ToolStripDropDownButton tsddbOptions = new ToolStripDropDownButton(Resources.ShapeManager_CreateContextMenu_Options);
            tsddbOptions.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsddbOptions.Image = Resources.gear;
            tsMain.Items.Add(tsddbOptions);

            if (Form.IsEditorMode)
            {
                ToolStripLabeledComboBox tscbImageEditorStartMode = new ToolStripLabeledComboBox(Resources.ShapeManager_CreateToolbar_EditorStartMode);
                tscbImageEditorStartMode.Content.AddRange(Helpers.GetLocalizedEnumDescriptions<ImageEditorStartMode>());
                tscbImageEditorStartMode.Content.SelectedIndex = (int)Options.ImageEditorStartMode;
                tscbImageEditorStartMode.Content.SelectedIndexChanged +=
                    (sender, e) => Options.ImageEditorStartMode = (ImageEditorStartMode)tscbImageEditorStartMode.Content.SelectedIndex;
                tsddbOptions.DropDownItems.Add(tscbImageEditorStartMode);

                ToolStripMenuItem tsmiAutoCloseEditorOnTask = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_AutoCloseEditorOnTask);
                tsmiAutoCloseEditorOnTask.Checked = Options.AutoCloseEditorOnTask;
                tsmiAutoCloseEditorOnTask.CheckOnClick = true;
                tsmiAutoCloseEditorOnTask.Click += (sender, e) => Options.AutoCloseEditorOnTask = tsmiAutoCloseEditorOnTask.Checked;
                tsddbOptions.DropDownItems.Add(tsmiAutoCloseEditorOnTask);

                tsmiImageEditorBackgroundColor = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_EditorBackgroundColor);
                tsmiImageEditorBackgroundColor.Click += (sender, e) =>
                {
                    Form.Pause();

                    if (PickColor(Options.ImageEditorBackgroundColor, out Color newColor))
                    {
                        Options.ImageEditorBackgroundColor = newColor;
                        UpdateMenu();
                    }

                    Form.Resume();
                };
                tsddbOptions.DropDownItems.Add(tsmiImageEditorBackgroundColor);

                tsddbOptions.DropDownItems.Add(new ToolStripSeparator());
            }

            if (!Form.IsEditorMode)
            {
                tsmiQuickCrop = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Multi_region_mode);
                tsmiQuickCrop.Checked = !Options.QuickCrop;
                tsmiQuickCrop.CheckOnClick = true;
                tsmiQuickCrop.ShortcutKeyDisplayString = "Q";
                tsmiQuickCrop.Click += (sender, e) => Options.QuickCrop = !tsmiQuickCrop.Checked;
                tsddbOptions.DropDownItems.Add(tsmiQuickCrop);
            }

            ToolStripMenuItem tsmiShowInfo = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_position_and_size_info);
            tsmiShowInfo.Checked = Options.ShowInfo;
            tsmiShowInfo.CheckOnClick = true;
            tsmiShowInfo.Click += (sender, e) => Options.ShowInfo = tsmiShowInfo.Checked;
            tsddbOptions.DropDownItems.Add(tsmiShowInfo);

            tsmiShowMagnifier = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_magnifier);
            tsmiShowMagnifier.Checked = Options.ShowMagnifier;
            tsmiShowMagnifier.CheckOnClick = true;
            tsmiShowMagnifier.Click += (sender, e) => Options.ShowMagnifier = tsmiShowMagnifier.Checked;
            tsddbOptions.DropDownItems.Add(tsmiShowMagnifier);

            ToolStripMenuItem tsmiUseSquareMagnifier = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Square_shape_magnifier);
            tsmiUseSquareMagnifier.Checked = Options.UseSquareMagnifier;
            tsmiUseSquareMagnifier.CheckOnClick = true;
            tsmiUseSquareMagnifier.Click += (sender, e) => Options.UseSquareMagnifier = tsmiUseSquareMagnifier.Checked;
            tsddbOptions.DropDownItems.Add(tsmiUseSquareMagnifier);

            tslnudMagnifierPixelCount = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Magnifier_pixel_count_);
            tslnudMagnifierPixelCount.Content.Minimum = RegionCaptureOptions.MagnifierPixelCountMinimum;
            tslnudMagnifierPixelCount.Content.Maximum = RegionCaptureOptions.MagnifierPixelCountMaximum;
            tslnudMagnifierPixelCount.Content.Increment = 2;
            tslnudMagnifierPixelCount.Content.Value = Options.MagnifierPixelCount;
            tslnudMagnifierPixelCount.Content.ValueChanged = (sender, e) => Options.MagnifierPixelCount = (int)tslnudMagnifierPixelCount.Content.Value;
            tsddbOptions.DropDownItems.Add(tslnudMagnifierPixelCount);

            ToolStripLabeledNumericUpDown tslnudMagnifierPixelSize = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Magnifier_pixel_size_);
            tslnudMagnifierPixelSize.Content.Minimum = RegionCaptureOptions.MagnifierPixelSizeMinimum;
            tslnudMagnifierPixelSize.Content.Maximum = RegionCaptureOptions.MagnifierPixelSizeMaximum;
            tslnudMagnifierPixelSize.Content.Value = Options.MagnifierPixelSize;
            tslnudMagnifierPixelSize.Content.ValueChanged = (sender, e) => Options.MagnifierPixelSize = (int)tslnudMagnifierPixelSize.Content.Value;
            tsddbOptions.DropDownItems.Add(tslnudMagnifierPixelSize);

            ToolStripMenuItem tsmiShowCrosshair = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_screen_wide_crosshair);
            tsmiShowCrosshair.Checked = Options.ShowCrosshair;
            tsmiShowCrosshair.CheckOnClick = true;
            tsmiShowCrosshair.Click += (sender, e) => Options.ShowCrosshair = tsmiShowCrosshair.Checked;
            tsddbOptions.DropDownItems.Add(tsmiShowCrosshair);

            ToolStripMenuItem tsmiUseLightResizeNodes = new ToolStripMenuItem(Resources.LightResizeNodes);
            tsmiUseLightResizeNodes.Checked = Options.UseLightResizeNodes;
            tsmiUseLightResizeNodes.CheckOnClick = true;
            tsmiUseLightResizeNodes.Click += (sender, e) => Options.UseLightResizeNodes = tsmiUseLightResizeNodes.Checked;
            tsddbOptions.DropDownItems.Add(tsmiUseLightResizeNodes);

            ToolStripMenuItem tsmiEnableAnimations = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_EnableAnimations);
            tsmiEnableAnimations.Checked = Options.EnableAnimations;
            tsmiEnableAnimations.CheckOnClick = true;
            tsmiEnableAnimations.Click += (sender, e) => Options.EnableAnimations = tsmiEnableAnimations.Checked;
            tsddbOptions.DropDownItems.Add(tsmiEnableAnimations);

            if (!Form.IsEditorMode)
            {
                ToolStripMenuItem tsmiFixedSize = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Fixed_size_region_mode);
                tsmiFixedSize.Checked = Options.IsFixedSize;
                tsmiFixedSize.CheckOnClick = true;
                tsmiFixedSize.Click += (sender, e) => Options.IsFixedSize = tsmiFixedSize.Checked;
                tsddbOptions.DropDownItems.Add(tsmiFixedSize);

                ToolStripDoubleLabeledNumericUpDown tslnudFixedSize = new ToolStripDoubleLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Width_,
                    Resources.ShapeManager_CreateContextMenu_Height_);
                tslnudFixedSize.Content.Minimum = 10;
                tslnudFixedSize.Content.Maximum = 10000;
                tslnudFixedSize.Content.Increment = 10;
                tslnudFixedSize.Content.Value = Options.FixedSize.Width;
                tslnudFixedSize.Content.Value2 = Options.FixedSize.Height;
                tslnudFixedSize.Content.ValueChanged = (sender, e) => Options.FixedSize = new Size((int)tslnudFixedSize.Content.Value, (int)tslnudFixedSize.Content.Value2);
                tsddbOptions.DropDownItems.Add(tslnudFixedSize);
            }

            ToolStripMenuItem tsmiShowFPS = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_FPS);
            tsmiShowFPS.Checked = Options.ShowFPS;
            tsmiShowFPS.CheckOnClick = true;
            tsmiShowFPS.Click += (sender, e) =>
            {
                Options.ShowFPS = tsmiShowFPS.Checked;
                Form.UpdateTitle();
            };
            tsddbOptions.DropDownItems.Add(tsmiShowFPS);

            ToolStripMenuItem tsmiSwitchToDrawingToolAfterSelection = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_SwitchToDrawingToolAfterSelection);
            tsmiSwitchToDrawingToolAfterSelection.Checked = Options.SwitchToDrawingToolAfterSelection;
            tsmiSwitchToDrawingToolAfterSelection.CheckOnClick = true;
            tsmiSwitchToDrawingToolAfterSelection.Click += (sender, e) =>
            {
                Options.SwitchToDrawingToolAfterSelection = tsmiSwitchToDrawingToolAfterSelection.Checked;
            };
            tsddbOptions.DropDownItems.Add(tsmiSwitchToDrawingToolAfterSelection);

            ToolStripMenuItem tsmiSwitchToSelectionToolAfterDrawing = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_SwitchToSelectionToolAfterDrawing);
            tsmiSwitchToSelectionToolAfterDrawing.Checked = Options.SwitchToSelectionToolAfterDrawing;
            tsmiSwitchToSelectionToolAfterDrawing.CheckOnClick = true;
            tsmiSwitchToSelectionToolAfterDrawing.Click += (sender, e) =>
            {
                Options.SwitchToSelectionToolAfterDrawing = tsmiSwitchToSelectionToolAfterDrawing.Checked;
            };
            tsddbOptions.DropDownItems.Add(tsmiSwitchToSelectionToolAfterDrawing);

            if (!Form.IsEditorMode)
            {
                ToolStripMenuItem tsmiRememberMenuState = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_RememberMenuState);
                tsmiRememberMenuState.Checked = Options.RememberMenuState;
                tsmiRememberMenuState.CheckOnClick = true;
                tsmiRememberMenuState.Click += (sender, e) => Options.RememberMenuState = tsmiRememberMenuState.Checked;
                tsddbOptions.DropDownItems.Add(tsmiRememberMenuState);
            }

            tsddbOptions.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem tsmiKeybinds = new ToolStripMenuItem(Resources.OpenKeybindsPage);
            tsmiKeybinds.Click += (sender, e) =>
            {
                if (Form.IsFullscreen)
                {
                    if (MessageBox.Show(Form, Resources.ThisWindowWillCloseBeforeOpeningKeybindsPageWantContinue,
                        "ShareX", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Form.CloseWindow();
                    }
                    else
                    {
                        return;
                    }
                }

                URLHelpers.OpenURL("https://getsharex.com/docs/region-capture");
            };
            tsddbOptions.DropDownItems.Add(tsmiKeybinds);

            #endregion Options

            if (Form.IsFullscreen)
            {
                tslDragRight = new ToolStripLabel()
                {
                    Alignment = ToolStripItemAlignment.Right,
                    DisplayStyle = ToolStripItemDisplayStyle.Image,
                    Image = Resources.ui_radio_button_uncheck,
                    Margin = new Padding(0, 0, 2, 0),
                    Padding = new Padding(2)
                };

                tsMain.Items.Add(tslDragRight);

                tslDragLeft.MouseDown += TslDrag_MouseDown;
                tslDragRight.MouseDown += TslDrag_MouseDown;
                tslDragLeft.MouseEnter += TslDrag_MouseEnter;
                tslDragRight.MouseEnter += TslDrag_MouseEnter;
                tslDragLeft.MouseLeave += TslDrag_MouseLeave;
                tslDragRight.MouseLeave += TslDrag_MouseLeave;
            }

            foreach (ToolStripItem tsi in tsMain.Items.OfType<ToolStripItem>())
            {
                if (!string.IsNullOrEmpty(tsi.Text))
                {
                    tsi.MouseEnter += (sender, e) =>
                    {
                        Point pos = Form.PointToClient(menuForm.PointToScreen(tsi.Bounds.Location));
                        pos.Y += tsi.Height + 8;

                        MenuTextAnimation.Text = tsi.Text;
                        MenuTextAnimation.Position = pos;
                        MenuTextAnimation.Start();
                    };

                    tsi.MouseLeave += TsMain_MouseLeave;
                }
            }

            tsMain.ResumeLayout(false);
            tsMain.PerformLayout();
            menuForm.ResumeLayout(false);

            menuForm.Show(Form);

            UpdateMenu();

            CurrentShapeChanged += shape => UpdateMenu();
            CurrentShapeTypeChanged += shapeType => UpdateMenu();
            ShapeCreated += shape => UpdateMenu();

            ConfigureMenuState();

            Form.Activate();

            ToolbarCreated = true;
        }

        private void MenuForm_Shown(object sender, EventArgs e)
        {
            Form.ToolbarHeight = menuForm.Height;
            Form.CenterCanvas();
        }

        private void MenuForm_KeyDown(object sender, KeyEventArgs e)
        {
            form_KeyDown(sender, e);
            Form.RegionCaptureForm_KeyDown(sender, e);

            e.Handled = true;
        }

        private void MenuForm_KeyUp(object sender, KeyEventArgs e)
        {
            form_KeyUp(sender, e);

            e.Handled = true;
        }

        private void MenuForm_LocationChanged(object sender, EventArgs e)
        {
            CheckMenuPosition();
        }

        private void MenuForm_GotFocus(object sender, EventArgs e)
        {
            Form.Resume();
        }

        private void MenuForm_LostFocus(object sender, EventArgs e)
        {
            Form.Pause();
        }

        private void TsMain_MouseLeave(object sender, EventArgs e)
        {
            MenuTextAnimation.Stop();
        }

        private void TslDrag_MouseEnter(object sender, EventArgs e)
        {
            menuForm.Cursor = Cursors.SizeAll;
        }

        private void TslDrag_MouseLeave(object sender, EventArgs e)
        {
            menuForm.Cursor = Cursors.Default;
        }

        private void TslDrag_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.DefWindowProc(menuForm.Handle, (uint)WindowsMessages.SYSCOMMAND, (UIntPtr)NativeConstants.MOUSE_MOVE, IntPtr.Zero);
            }
            else if (e.Button == MouseButtons.Right)
            {
                SetMenuCollapsed(!ToolbarCollapsed);
                CheckMenuPosition();
            }
        }

        private void ConfigureMenuState()
        {
            if (!Form.IsEditorMode && Options.RememberMenuState)
            {
                SetMenuCollapsed(Options.MenuCollapsed);
            }

            UpdateMenuPosition();
        }

        internal void UpdateMenuPosition()
        {
            Rectangle rectScreen;

            if (Form.IsFullscreen)
            {
                rectScreen = CaptureHelpers.GetActiveScreenBounds();
                rectScreen.Y += 20;
            }
            else
            {
                rectScreen = Form.RectangleToScreen(Form.ClientArea);
            }

            if (!Form.IsEditorMode && Options.RememberMenuState && rectScreen.Contains(Options.MenuPosition))
            {
                menuForm.Location = Options.MenuPosition;
            }
            else if (tsMain.Width < rectScreen.Width)
            {
                menuForm.Location = new Point(rectScreen.X + (rectScreen.Width / 2) - (tsMain.Width / 2), rectScreen.Y);
            }
            else
            {
                menuForm.Location = rectScreen.Location;
            }
        }

        private void CheckMenuPosition()
        {
            Rectangle rectMenu = menuForm.Bounds;
            Rectangle rectScreen = CaptureHelpers.GetScreenBounds();
            Point pos = rectMenu.Location;

            if (rectMenu.Width < rectScreen.Width)
            {
                if (rectMenu.X < rectScreen.X)
                {
                    pos.X = rectScreen.X;
                }
                else if (rectMenu.Right > rectScreen.Right)
                {
                    pos.X = rectScreen.Right - rectMenu.Width;
                }
            }

            if (rectMenu.Height < rectScreen.Height)
            {
                if (rectMenu.Y < rectScreen.Y)
                {
                    pos.Y = rectScreen.Y;
                }
                else if (rectMenu.Bottom > rectScreen.Bottom)
                {
                    pos.Y = rectScreen.Bottom - rectMenu.Height;
                }
            }

            if (pos != rectMenu.Location)
            {
                menuForm.Location = pos;
            }

            if (!Form.IsEditorMode && Options.RememberMenuState)
            {
                Options.MenuPosition = pos;
            }
        }

        private void SetMenuCollapsed(bool isCollapsed)
        {
            if (ToolbarCollapsed == isCollapsed)
            {
                return;
            }

            ToolbarCollapsed = isCollapsed;

            if (ToolbarCollapsed)
            {
                foreach (ToolStripItem tsi in tsMain.Items.OfType<ToolStripItem>())
                {
                    if (tsi == tslDragLeft)
                    {
                        continue;
                    }

                    tsi.Visible = false;
                }
            }
            else
            {
                foreach (ToolStripItem tsi in tsMain.Items.OfType<ToolStripItem>())
                {
                    tsi.Visible = true;
                }

                UpdateMenu();
            }

            if (!Form.IsEditorMode && Options.RememberMenuState)
            {
                Options.MenuCollapsed = ToolbarCollapsed;
            }
        }

        private void UpdateMenu()
        {
            if (Form.IsClosing || menuForm == null || menuForm.IsDisposed) return;

            ShapeType shapeType = CurrentTool;

            foreach (ToolStripButton tsb in tsMain.Items.OfType<ToolStripButton>().Where(x => x.Tag is ShapeType))
            {
                if ((ShapeType)tsb.Tag == shapeType)
                {
                    tsb.RadioCheck();
                    break;
                }
            }

            // use menu of current shape in case of an active select tool.
            if (CurrentShape != null && shapeType == ShapeType.ToolSelect)
            {
                shapeType = CurrentShape.ShapeType;
            }

            Color borderColor;

            if (shapeType == ShapeType.DrawingTextBackground || shapeType == ShapeType.DrawingSpeechBalloon)
            {
                borderColor = AnnotationOptions.TextBorderColor;
            }
            else if (shapeType == ShapeType.DrawingTextOutline)
            {
                borderColor = AnnotationOptions.TextOutlineBorderColor;
            }
            else if (shapeType == ShapeType.DrawingStep)
            {
                borderColor = AnnotationOptions.StepBorderColor;
            }
            else
            {
                borderColor = AnnotationOptions.BorderColor;
            }

            if (tsbBorderColor.Image != null) tsbBorderColor.Image.Dispose();
            tsbBorderColor.Image = ImageHelpers.CreateColorPickerIcon(borderColor, new Rectangle(0, 0, 16, 16), 8);

            int borderSize;

            if (shapeType == ShapeType.DrawingTextBackground || shapeType == ShapeType.DrawingSpeechBalloon)
            {
                borderSize = AnnotationOptions.TextBorderSize;
            }
            else if (shapeType == ShapeType.DrawingTextOutline)
            {
                borderSize = AnnotationOptions.TextOutlineBorderSize;
            }
            else if (shapeType == ShapeType.DrawingStep)
            {
                borderSize = AnnotationOptions.StepBorderSize;
            }
            else
            {
                borderSize = AnnotationOptions.BorderSize;
            }

            tslnudBorderSize.Content.Value = borderSize;

            Color fillColor;

            if (shapeType == ShapeType.DrawingTextBackground || shapeType == ShapeType.DrawingSpeechBalloon)
            {
                fillColor = AnnotationOptions.TextFillColor;
            }
            else if (shapeType == ShapeType.DrawingStep)
            {
                fillColor = AnnotationOptions.StepFillColor;
            }
            else
            {
                fillColor = AnnotationOptions.FillColor;
            }

            if (tsbFillColor.Image != null) tsbFillColor.Image.Dispose();
            tsbFillColor.Image = ImageHelpers.CreateColorPickerIcon(fillColor, new Rectangle(0, 0, 16, 16));

            int cornerRadius = 0;

            if (shapeType == ShapeType.RegionRectangle)
            {
                cornerRadius = AnnotationOptions.RegionCornerRadius;
            }
            else if (shapeType == ShapeType.DrawingRectangle || shapeType == ShapeType.DrawingTextBackground)
            {
                cornerRadius = AnnotationOptions.DrawingCornerRadius;
            }

            tslnudCornerRadius.Content.Value = cornerRadius;

            tscbImageInterpolationMode.Content.SelectedIndex = (int)AnnotationOptions.ImageInterpolationMode;

            tslnudBlurRadius.Content.Value = AnnotationOptions.BlurRadius;

            tslnudPixelateSize.Content.Value = AnnotationOptions.PixelateSize;

            if (tsbHighlightColor.Image != null) tsbHighlightColor.Image.Dispose();
            tsbHighlightColor.Image = ImageHelpers.CreateColorPickerIcon(AnnotationOptions.HighlightColor, new Rectangle(0, 0, 16, 16));

            tslnudStepFontSize.Content.Value = AnnotationOptions.StepFontSize;
            tslnudStartingStepValue.Content.Value = StartingStepNumber;
            tsmiStepUseLetters.Checked = AnnotationOptions.StepUseLetters;

            tslnudMagnifyStrength.Content.Value = AnnotationOptions.MagnifyStrength;

            tsmiShadow.Checked = AnnotationOptions.Shadow;

            if (tsmiShadowColor.Image != null) tsmiShadowColor.Image.Dispose();
            tsmiShadowColor.Image = ImageHelpers.CreateColorPickerIcon(AnnotationOptions.ShadowColor, new Rectangle(0, 0, 16, 16));

            tslnudCenterPoints.Content.Value = AnnotationOptions.LineCenterPointCount;

            tsmiArrowHeadsBothSide.Checked = AnnotationOptions.ArrowHeadsBothSide;

            switch (shapeType)
            {
                default:
                    tsddbShapeOptions.Visible = false;
                    break;
                case ShapeType.RegionRectangle:
                case ShapeType.DrawingRectangle:
                case ShapeType.DrawingEllipse:
                case ShapeType.DrawingFreehand:
                case ShapeType.DrawingLine:
                case ShapeType.DrawingArrow:
                case ShapeType.DrawingTextOutline:
                case ShapeType.DrawingTextBackground:
                case ShapeType.DrawingSpeechBalloon:
                case ShapeType.DrawingStep:
                case ShapeType.DrawingMagnify:
                case ShapeType.DrawingImage:
                case ShapeType.DrawingImageScreen:
                case ShapeType.DrawingCursor:
                case ShapeType.EffectBlur:
                case ShapeType.EffectPixelate:
                    tsddbShapeOptions.Visible = true;
                    break;
            }

            tsmiUndo.Enabled = tsmiDeleteAll.Enabled = Shapes.Count > 0;
            tsmiDelete.Enabled = tsmiMoveTop.Enabled = tsmiMoveUp.Enabled = tsmiMoveDown.Enabled = tsmiMoveBottom.Enabled = CurrentShape != null;

            switch (shapeType)
            {
                default:
                    tsbBorderColor.Visible = false;
                    tslnudBorderSize.Visible = false;
                    tsmiShadow.Visible = false;
                    tsmiShadowColor.Visible = false;
                    break;
                case ShapeType.DrawingRectangle:
                case ShapeType.DrawingEllipse:
                case ShapeType.DrawingFreehand:
                case ShapeType.DrawingLine:
                case ShapeType.DrawingArrow:
                case ShapeType.DrawingTextOutline:
                case ShapeType.DrawingTextBackground:
                case ShapeType.DrawingSpeechBalloon:
                case ShapeType.DrawingStep:
                case ShapeType.DrawingMagnify:
                    tsbBorderColor.Visible = true;
                    tslnudBorderSize.Visible = true;
                    tsmiShadow.Visible = true;
                    tsmiShadowColor.Visible = true;
                    break;
            }

            switch (shapeType)
            {
                default:
                    tsbFillColor.Visible = false;
                    break;
                case ShapeType.DrawingRectangle:
                case ShapeType.DrawingEllipse:
                case ShapeType.DrawingTextBackground:
                case ShapeType.DrawingSpeechBalloon:
                case ShapeType.DrawingStep:
                case ShapeType.DrawingMagnify:
                    tsbFillColor.Visible = true;
                    break;
            }

            switch (shapeType)
            {
                default:
                    tslnudCornerRadius.Visible = false;
                    break;
                case ShapeType.RegionRectangle:
                case ShapeType.DrawingRectangle:
                case ShapeType.DrawingTextBackground:
                    tslnudCornerRadius.Visible = true;
                    break;
            }

            tslnudCenterPoints.Visible = shapeType == ShapeType.DrawingLine || shapeType == ShapeType.DrawingArrow;
            tsmiArrowHeadsBothSide.Visible = shapeType == ShapeType.DrawingArrow;
            tscbImageInterpolationMode.Visible = shapeType == ShapeType.DrawingImage || shapeType == ShapeType.DrawingImageScreen || shapeType == ShapeType.DrawingMagnify;
            tslnudStartingStepValue.Visible = shapeType == ShapeType.DrawingStep;
            tslnudStepFontSize.Visible = tsmiStepUseLetters.Visible = shapeType == ShapeType.DrawingStep;
            tslnudMagnifyStrength.Visible = shapeType == ShapeType.DrawingMagnify;
            tscbCursorTypes.Visible = shapeType == ShapeType.DrawingCursor;
            tslnudBlurRadius.Visible = shapeType == ShapeType.EffectBlur;
            tslnudPixelateSize.Visible = shapeType == ShapeType.EffectPixelate;
            tsbHighlightColor.Visible = shapeType == ShapeType.EffectHighlight;

            if (tsmiRegionCapture != null)
            {
                tsmiRegionCapture.Visible = !Options.QuickCrop && ValidRegions.Length > 0;
            }

            if (tsmiImageEditorBackgroundColor != null)
            {
                if (tsmiImageEditorBackgroundColor.Image != null) tsmiImageEditorBackgroundColor.Image.Dispose();
                tsmiImageEditorBackgroundColor.Image = ImageHelpers.CreateColorPickerIcon(Options.ImageEditorBackgroundColor, new Rectangle(0, 0, 16, 16));
            }
        }

        internal Cursor GetSelectedCursor()
        {
            if (tscbCursorTypes.Content.SelectedIndex > -1)
            {
                return Helpers.CursorList[tscbCursorTypes.Content.SelectedIndex];
            }

            return Cursors.Default;
        }
    }
}