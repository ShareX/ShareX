#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    internal partial class ShapeManager
    {
        public bool ToolbarCreated { get; private set; }
        public bool IsMenuCollapsed { get; private set; }

        internal TextAnimation MenuTextAnimation = new TextAnimation()
        {
            FadeInDuration = TimeSpan.FromMilliseconds(0),
            Duration = TimeSpan.FromMilliseconds(5000),
            FadeOutDuration = TimeSpan.FromMilliseconds(500)
        };

        private Form menuForm;
        private ToolStripEx tsMain;
        private ToolStripButton tsbBorderColor, tsbFillColor, tsbHighlightColor;
        private ToolStripDropDownButton tsddbShapeOptions;
        private ToolStripMenuItem tsmiArrowHeadsBothSide, tsmiShadow, tsmiUndo, tsmiDelete, tsmiDeleteAll, tsmiMoveTop, tsmiMoveUp, tsmiMoveDown, tsmiMoveBottom, tsmiRegionCapture, tsmiQuickCrop, tsmiTips;
        private ToolStripLabeledNumericUpDown tslnudBorderSize, tslnudCornerRadius, tslnudCenterPoints, tslnudBlurRadius, tslnudPixelateSize;
        private ToolStripLabel tslDragLeft;
        private ToolStripLabeledComboBox tscbCursorTypes;

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
                Text = "ShareX - Annotate menu",
                TopMost = !form.IsEditorMode
            };

            menuForm.Shown += MenuForm_Shown;
            menuForm.KeyDown += MenuForm_KeyDown;
            menuForm.KeyUp += MenuForm_KeyUp;
            menuForm.LocationChanged += MenuForm_LocationChanged;

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
                Padding = new Padding(0, 1, 0, 0),
                Renderer = new CustomToolStripProfessionalRenderer(),
                TabIndex = 0,
                ShowItemToolTips = false
            };

            tsMain.MouseLeave += TsMain_MouseLeave;

            tsMain.SuspendLayout();

            // https://www.medo64.com/2014/01/scaling-toolstrip-with-dpi/
            using (Graphics g = menuForm.CreateGraphics())
            {
                double scale = Math.Max(g.DpiX, g.DpiY) / 96.0;
                double newScale = ((int)Math.Floor(scale * 100) / 25 * 25) / 100.0;
                if (newScale > 1)
                {
                    int newWidth = (int)(tsMain.ImageScalingSize.Width * newScale);
                    int newHeight = (int)(tsMain.ImageScalingSize.Height * newScale);
                    tsMain.ImageScalingSize = new Size(newWidth, newHeight);
                }
            }

            menuForm.Controls.Add(tsMain);

            tslDragLeft = new ToolStripLabel()
            {
                DisplayStyle = ToolStripItemDisplayStyle.Image,
                Image = Resources.ui_radio_button_uncheck,
                Margin = new Padding(2, 0, 2, 0),
                Padding = new Padding(2)
            };

            tsMain.Items.Add(tslDragLeft);

            if (form.IsEditorMode)
            {
                #region Editor mode

                ToolStripButton tsbCompleteEdit = new ToolStripButton();

                if (form.Mode == RegionCaptureMode.Editor)
                {
                    tsbCompleteEdit.Text = Resources.ShapeManager_CreateToolbar_RunAfterCaptureTasks;
                }
                else
                {
                    tsbCompleteEdit.Text = Resources.ShapeManager_CreateToolbar_ApplyChangesContinueTaskEnter;
                }

                tsbCompleteEdit.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbCompleteEdit.Image = Resources.tick;
                tsbCompleteEdit.MouseDown += (sender, e) => form.Close(RegionResult.AnnotateRunAfterCaptureTasks);
                tsMain.Items.Add(tsbCompleteEdit);

                if (form.Mode == RegionCaptureMode.TaskEditor)
                {
                    ToolStripButton tsbClose = new ToolStripButton(Resources.ShapeManager_CreateToolbar_ContinueTaskSpaceOrRightClick);
                    tsbClose.DisplayStyle = ToolStripItemDisplayStyle.Image;
                    tsbClose.Image = Resources.control;
                    tsbClose.MouseDown += (sender, e) => form.Close(RegionResult.AnnotateContinueTask);
                    tsMain.Items.Add(tsbClose);

                    ToolStripButton tsbCloseCancel = new ToolStripButton(Resources.ShapeManager_CreateToolbar_CancelTaskEsc);
                    tsbCloseCancel.DisplayStyle = ToolStripItemDisplayStyle.Image;
                    tsbCloseCancel.Image = Resources.cross;
                    tsbCloseCancel.MouseDown += (sender, e) => form.Close(RegionResult.AnnotateCancelTask);
                    tsMain.Items.Add(tsbCloseCancel);
                }

                if (form.Mode == RegionCaptureMode.TaskEditor)
                {
                    tsMain.Items.Add(new ToolStripSeparator());
                }

                ToolStripButton tsbSaveImage = new ToolStripButton(Resources.ShapeManager_CreateToolbar_SaveImage);
                tsbSaveImage.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbSaveImage.Enabled = File.Exists(form.ImageFilePath);
                tsbSaveImage.Image = Resources.disk_black;
                tsbSaveImage.MouseDown += (sender, e) => form.Close(RegionResult.AnnotateSaveImage);
                tsMain.Items.Add(tsbSaveImage);

                ToolStripButton tsbSaveImageAs = new ToolStripButton(Resources.ShapeManager_CreateToolbar_SaveImageAs);
                tsbSaveImageAs.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbSaveImageAs.Image = Resources.disks_black;
                tsbSaveImageAs.MouseDown += (sender, e) => form.Close(RegionResult.AnnotateSaveImageAs);
                tsMain.Items.Add(tsbSaveImageAs);

                ToolStripButton tsbCopyImage = new ToolStripButton(Resources.ShapeManager_CreateToolbar_CopyImageToClipboard);
                tsbCopyImage.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbCopyImage.Image = Resources.clipboard;
                tsbCopyImage.MouseDown += (sender, e) => form.Close(RegionResult.AnnotateCopyImage);
                tsMain.Items.Add(tsbCopyImage);

                ToolStripButton tsbUploadImage = new ToolStripButton(Resources.ShapeManager_CreateToolbar_UploadImage);
                tsbUploadImage.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbUploadImage.Image = Resources.drive_globe;
                tsbUploadImage.MouseDown += (sender, e) => form.Close(RegionResult.AnnotateUploadImage);
                tsMain.Items.Add(tsbUploadImage);

                ToolStripButton tsbPrintImage = new ToolStripButton(Resources.ShapeManager_CreateToolbar_PrintImage);
                tsbPrintImage.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsbPrintImage.Image = Resources.printer;
                tsbPrintImage.MouseDown += (sender, e) => form.Close(RegionResult.AnnotatePrintImage);
                tsMain.Items.Add(tsbPrintImage);

                tsMain.Items.Add(new ToolStripSeparator());

                #endregion Editor mode
            }

            #region Tools

            foreach (ShapeType shapeType in Helpers.GetEnums<ShapeType>())
            {
                if (form.IsEditorMode)
                {
                    if (IsShapeTypeRegion(shapeType))
                    {
                        continue;
                    }
                }
                else if (shapeType == ShapeType.DrawingRectangle)
                {
                    tsMain.Items.Add(new ToolStripSeparator());
                }
                else if (shapeType == ShapeType.DrawingCrop)
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
                    case ShapeType.DrawingImage:
                        img = Resources.folder_open_image;
                        break;
                    case ShapeType.DrawingImageScreen:
                        img = Resources.monitor_image;
                        break;
                    case ShapeType.DrawingCursor:
                        img = Resources.cursor;
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
                    case ShapeType.DrawingCrop:
                        img = Resources.image_crop;
                        break;
                }

                tsbShapeType.Image = img;
                tsbShapeType.Checked = shapeType == CurrentShapeType;
                tsbShapeType.Tag = shapeType;

                tsbShapeType.MouseDown += (sender, e) =>
                {
                    tsbShapeType.RadioCheck();
                    CurrentShapeType = shapeType;
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
                PauseForm();

                ShapeType shapeType = CurrentShapeType;

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

                if (ColorPickerForm.PickColor(borderColor, out Color newColor))
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

                ResumeForm();
            };
            tsMain.Items.Add(tsbBorderColor);

            tsbFillColor = new ToolStripButton(Resources.ShapeManager_CreateContextMenu_Fill_color___);
            tsbFillColor.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbFillColor.Click += (sender, e) =>
            {
                PauseForm();

                ShapeType shapeType = CurrentShapeType;

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

                if (ColorPickerForm.PickColor(fillColor, out Color newColor))
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

                ResumeForm();
            };
            tsMain.Items.Add(tsbFillColor);

            tsbHighlightColor = new ToolStripButton(Resources.ShapeManager_CreateContextMenu_Highlight_color___);
            tsbHighlightColor.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbHighlightColor.Click += (sender, e) =>
            {
                PauseForm();

                if (ColorPickerForm.PickColor(AnnotationOptions.HighlightColor, out Color newColor))
                {
                    AnnotationOptions.HighlightColor = newColor;
                    UpdateMenu();
                    UpdateCurrentShape();
                }

                ResumeForm();
            };
            tsMain.Items.Add(tsbHighlightColor);

            tsddbShapeOptions = new ToolStripDropDownButton(Resources.ShapeManager_CreateToolbar_ShapeOptions);
            tsddbShapeOptions.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsddbShapeOptions.Image = Resources.layer__pencil;
            tsMain.Items.Add(tsddbShapeOptions);

            tslnudBorderSize = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Border_size_);
            tslnudBorderSize.Content.Minimum = 0;
            tslnudBorderSize.Content.Maximum = 20;
            tslnudBorderSize.Content.ValueChanged = (sender, e) =>
            {
                ShapeType shapeType = CurrentShapeType;

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
                ShapeType shapeType = CurrentShapeType;

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

            tscbCursorTypes = new ToolStripLabeledComboBox("Cursor type:");
            CursorConverter cursorConverter = new CursorConverter();
            foreach (Cursor cursor in Helpers.CursorList)
            {
                string name = cursorConverter.ConvertToString(cursor);
                tscbCursorTypes.Content.Add(name);
            }
            tscbCursorTypes.Content.SelectedIndex = 3; // Cursors.Default
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

            tslnudCenterPoints = new ToolStripLabeledNumericUpDown("Center points:");
            tslnudCenterPoints.Content.Minimum = 0;
            tslnudCenterPoints.Content.Maximum = LineDrawingShape.MaximumCenterPointCount;
            tslnudCenterPoints.Content.ValueChanged = (sender, e) =>
            {
                AnnotationOptions.LineCenterPointCount = (int)tslnudCenterPoints.Content.Value;
                UpdateCurrentShape();
            };
            tsddbShapeOptions.DropDownItems.Add(tslnudCenterPoints);

            tsmiArrowHeadsBothSide = new ToolStripMenuItem("Arrows on both ends");
            tsmiArrowHeadsBothSide.CheckOnClick = true;
            tsmiArrowHeadsBothSide.Click += (sender, e) =>
            {
                AnnotationOptions.ArrowHeadsBothSide = tsmiArrowHeadsBothSide.Checked;
                UpdateCurrentShape();
            };
            tsddbShapeOptions.DropDownItems.Add(tsmiArrowHeadsBothSide);

            tsmiShadow = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_DropShadow);
            tsmiShadow.Checked = true;
            tsmiShadow.CheckOnClick = true;
            tsmiShadow.Click += (sender, e) =>
            {
                AnnotationOptions.Shadow = tsmiShadow.Checked;
                UpdateCurrentShape();
            };
            tsddbShapeOptions.DropDownItems.Add(tsmiShadow);

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
            tsmiUndo.MouseDown += (sender, e) => UndoShape();
            tsddbEdit.DropDownItems.Add(tsmiUndo);

            tsddbEdit.DropDownItems.Add(new ToolStripSeparator());

            tsmiDelete = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_Delete);
            tsmiDelete.Image = Resources.layer__minus;
            tsmiDelete.ShortcutKeyDisplayString = "Del";
            tsmiDelete.MouseDown += (sender, e) => DeleteCurrentShape();
            tsddbEdit.DropDownItems.Add(tsmiDelete);

            tsmiDeleteAll = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_DeleteAll);
            tsmiDeleteAll.Image = Resources.eraser;
            tsmiDeleteAll.ShortcutKeyDisplayString = "Shift+Del";
            tsmiDeleteAll.MouseDown += (sender, e) => DeleteAllShapes();
            tsddbEdit.DropDownItems.Add(tsmiDeleteAll);

            tsddbEdit.DropDownItems.Add(new ToolStripSeparator());

            tsmiMoveTop = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_BringToFront);
            tsmiMoveTop.Image = Resources.layers_stack_arrange;
            tsmiMoveTop.ShortcutKeyDisplayString = "Home";
            tsmiMoveTop.MouseDown += (sender, e) => MoveCurrentShapeTop();
            tsddbEdit.DropDownItems.Add(tsmiMoveTop);

            tsmiMoveUp = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_BringForward);
            tsmiMoveUp.Image = Resources.layers_arrange;
            tsmiMoveUp.ShortcutKeyDisplayString = "Page up";
            tsmiMoveUp.MouseDown += (sender, e) => MoveCurrentShapeUp();
            tsddbEdit.DropDownItems.Add(tsmiMoveUp);

            tsmiMoveDown = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_SendBackward);
            tsmiMoveDown.Image = Resources.layers_arrange_back;
            tsmiMoveDown.ShortcutKeyDisplayString = "Page down";
            tsmiMoveDown.MouseDown += (sender, e) => MoveCurrentShapeDown();
            tsddbEdit.DropDownItems.Add(tsmiMoveDown);

            tsmiMoveBottom = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_SendToBack);
            tsmiMoveBottom.Image = Resources.layers_stack_arrange_back;
            tsmiMoveBottom.ShortcutKeyDisplayString = "End";
            tsmiMoveBottom.MouseDown += (sender, e) => MoveCurrentShapeBottom();
            tsddbEdit.DropDownItems.Add(tsmiMoveBottom);

            #endregion Edit

            if (form.IsEditorMode)
            {
                #region Image

                ToolStripDropDownButton tsddbImage = new ToolStripDropDownButton("Image");
                tsddbImage.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsddbImage.Image = Resources.image__pencil;
                tsMain.Items.Add(tsddbImage);

                ToolStripMenuItem tsmiImageSize = new ToolStripMenuItem("Image size...");
                tsmiImageSize.Image = Resources.image_resize;
                tsmiImageSize.MouseDown += (sender, e) => ChangeImageSize();
                tsddbImage.DropDownItems.Add(tsmiImageSize);

                ToolStripMenuItem tsmiCanvasSize = new ToolStripMenuItem("Canvas size...");
                tsmiCanvasSize.Image = Resources.image_resize_actual;
                tsmiCanvasSize.MouseDown += (sender, e) => ChangeCanvasSize();
                tsddbImage.DropDownItems.Add(tsmiCanvasSize);

                tsddbImage.DropDownItems.Add(new ToolStripSeparator());

                ToolStripMenuItem tsmiRotate90Clockwise = new ToolStripMenuItem("Rotate 90° clockwise");
                tsmiRotate90Clockwise.Image = Resources.arrow_circle;
                tsmiRotate90Clockwise.MouseDown += (sender, e) => RotateImage(RotateFlipType.Rotate90FlipNone);
                tsddbImage.DropDownItems.Add(tsmiRotate90Clockwise);

                ToolStripMenuItem tsmiRotate90CounterClockwise = new ToolStripMenuItem("Rotate 90° counter clockwise");
                tsmiRotate90CounterClockwise.Image = Resources.arrow_circle_135_left;
                tsmiRotate90CounterClockwise.MouseDown += (sender, e) => RotateImage(RotateFlipType.Rotate270FlipNone);
                tsddbImage.DropDownItems.Add(tsmiRotate90CounterClockwise);

                ToolStripMenuItem tsmiRotate180 = new ToolStripMenuItem("Rotate 180°");
                tsmiRotate180.Image = Resources.arrow_circle_double;
                tsmiRotate180.MouseDown += (sender, e) => RotateImage(RotateFlipType.Rotate180FlipNone);
                tsddbImage.DropDownItems.Add(tsmiRotate180);

                tsddbImage.DropDownItems.Add(new ToolStripSeparator());

                ToolStripMenuItem tsmiFlipHorizontal = new ToolStripMenuItem("Flip horizontal");
                tsmiFlipHorizontal.Image = Resources.layer_flip;
                tsmiFlipHorizontal.MouseDown += (sender, e) => RotateImage(RotateFlipType.RotateNoneFlipX);
                tsddbImage.DropDownItems.Add(tsmiFlipHorizontal);

                ToolStripMenuItem tsmiFlipVertical = new ToolStripMenuItem("Flip vertical");
                tsmiFlipVertical.Image = Resources.layer_flip_vertical;
                tsmiFlipVertical.MouseDown += (sender, e) => RotateImage(RotateFlipType.RotateNoneFlipY);
                tsddbImage.DropDownItems.Add(tsmiFlipVertical);

                #endregion
            }

            tsMain.Items.Add(new ToolStripSeparator());

            #region Capture

            ToolStripDropDownButton tsddbCapture = new ToolStripDropDownButton(Resources.ShapeManager_CreateContextMenu_Capture);
            tsddbCapture.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsddbCapture.Image = Resources.camera;
            tsMain.Items.Add(tsddbCapture);

            tsmiRegionCapture = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_CaptureRegions);
            tsmiRegionCapture.Image = Resources.layer;
            tsmiRegionCapture.ShortcutKeyDisplayString = "Enter";
            tsmiRegionCapture.MouseDown += (sender, e) =>
            {
                form.UpdateRegionPath();
                form.Close(RegionResult.Region);
            };
            tsddbCapture.DropDownItems.Add(tsmiRegionCapture);

            if (RegionCaptureForm.LastRegionFillPath != null)
            {
                ToolStripMenuItem tsmiLastRegionCapture = new ToolStripMenuItem(Resources.ShapeManager_CreateToolbar_LastRegion);
                tsmiLastRegionCapture.Image = Resources.layers;
                tsmiLastRegionCapture.MouseDown += (sender, e) => form.Close(RegionResult.LastRegion);
                tsddbCapture.DropDownItems.Add(tsmiLastRegionCapture);
            }

            ToolStripMenuItem tsmiFullscreenCapture = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Capture_fullscreen);
            tsmiFullscreenCapture.Image = Resources.layer_fullscreen;
            tsmiFullscreenCapture.ShortcutKeyDisplayString = "Space";
            tsmiFullscreenCapture.MouseDown += (sender, e) => form.Close(RegionResult.Fullscreen);
            tsddbCapture.DropDownItems.Add(tsmiFullscreenCapture);

            ToolStripMenuItem tsmiActiveMonitorCapture = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Capture_active_monitor);
            tsmiActiveMonitorCapture.Image = Resources.monitor;
            tsmiActiveMonitorCapture.ShortcutKeyDisplayString = "~";
            tsmiActiveMonitorCapture.MouseDown += (sender, e) => form.Close(RegionResult.ActiveMonitor);
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
                tsmi.MouseDown += (sender, e) =>
                {
                    form.MonitorIndex = index;
                    form.Close(RegionResult.Monitor);
                };
                tsmiMonitorCapture.DropDownItems.Add(tsmi);
            }

            #endregion Capture

            #region Options

            ToolStripDropDownButton tsddbOptions = new ToolStripDropDownButton(Resources.ShapeManager_CreateContextMenu_Options);
            tsddbOptions.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsddbOptions.Image = Resources.gear;
            tsMain.Items.Add(tsddbOptions);

            if (!form.IsEditorMode)
            {
                tsmiQuickCrop = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Multi_region_mode);
                tsmiQuickCrop.Checked = !Config.QuickCrop;
                tsmiQuickCrop.CheckOnClick = true;
                tsmiQuickCrop.ShortcutKeyDisplayString = "Q";
                tsmiQuickCrop.Click += (sender, e) => Config.QuickCrop = !tsmiQuickCrop.Checked;
                tsddbOptions.DropDownItems.Add(tsmiQuickCrop);

                tsmiTips = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_tips);
                tsmiTips.Checked = Config.ShowHotkeys;
                tsmiTips.CheckOnClick = true;
                tsmiTips.ShortcutKeyDisplayString = "F1";
                tsmiTips.Click += (sender, e) => Config.ShowHotkeys = tsmiTips.Checked;
                tsddbOptions.DropDownItems.Add(tsmiTips);
            }

            ToolStripMenuItem tsmiShowInfo = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_position_and_size_info);
            tsmiShowInfo.Checked = Config.ShowInfo;
            tsmiShowInfo.CheckOnClick = true;
            tsmiShowInfo.Click += (sender, e) => Config.ShowInfo = tsmiShowInfo.Checked;
            tsddbOptions.DropDownItems.Add(tsmiShowInfo);

            ToolStripMenuItem tsmiShowMagnifier = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_magnifier);
            tsmiShowMagnifier.Checked = Config.ShowMagnifier;
            tsmiShowMagnifier.CheckOnClick = true;
            tsmiShowMagnifier.Click += (sender, e) => Config.ShowMagnifier = tsmiShowMagnifier.Checked;
            tsddbOptions.DropDownItems.Add(tsmiShowMagnifier);

            ToolStripMenuItem tsmiUseSquareMagnifier = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Square_shape_magnifier);
            tsmiUseSquareMagnifier.Checked = Config.UseSquareMagnifier;
            tsmiUseSquareMagnifier.CheckOnClick = true;
            tsmiUseSquareMagnifier.Click += (sender, e) => Config.UseSquareMagnifier = tsmiUseSquareMagnifier.Checked;
            tsddbOptions.DropDownItems.Add(tsmiUseSquareMagnifier);

            ToolStripLabeledNumericUpDown tslnudMagnifierPixelCount = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Magnifier_pixel_count_);
            tslnudMagnifierPixelCount.Content.Minimum = RegionCaptureOptions.MagnifierPixelCountMinimum;
            tslnudMagnifierPixelCount.Content.Maximum = RegionCaptureOptions.MagnifierPixelCountMaximum;
            tslnudMagnifierPixelCount.Content.Increment = 2;
            tslnudMagnifierPixelCount.Content.Value = Config.MagnifierPixelCount;
            tslnudMagnifierPixelCount.Content.ValueChanged = (sender, e) => Config.MagnifierPixelCount = (int)tslnudMagnifierPixelCount.Content.Value;
            tsddbOptions.DropDownItems.Add(tslnudMagnifierPixelCount);

            ToolStripLabeledNumericUpDown tslnudMagnifierPixelSize = new ToolStripLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Magnifier_pixel_size_);
            tslnudMagnifierPixelSize.Content.Minimum = RegionCaptureOptions.MagnifierPixelSizeMinimum;
            tslnudMagnifierPixelSize.Content.Maximum = RegionCaptureOptions.MagnifierPixelSizeMaximum;
            tslnudMagnifierPixelSize.Content.Value = Config.MagnifierPixelSize;
            tslnudMagnifierPixelSize.Content.ValueChanged = (sender, e) => Config.MagnifierPixelSize = (int)tslnudMagnifierPixelSize.Content.Value;
            tsddbOptions.DropDownItems.Add(tslnudMagnifierPixelSize);

            ToolStripMenuItem tsmiShowCrosshair = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_screen_wide_crosshair);
            tsmiShowCrosshair.Checked = Config.ShowCrosshair;
            tsmiShowCrosshair.CheckOnClick = true;
            tsmiShowCrosshair.Click += (sender, e) => Config.ShowCrosshair = tsmiShowCrosshair.Checked;
            tsddbOptions.DropDownItems.Add(tsmiShowCrosshair);

            ToolStripMenuItem tsmiEnableAnimations = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_EnableAnimations);
            tsmiEnableAnimations.Checked = Config.EnableAnimations;
            tsmiEnableAnimations.CheckOnClick = true;
            tsmiEnableAnimations.Click += (sender, e) => Config.EnableAnimations = tsmiEnableAnimations.Checked;
            tsddbOptions.DropDownItems.Add(tsmiEnableAnimations);

            if (!form.IsEditorMode)
            {
                ToolStripMenuItem tsmiFixedSize = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Fixed_size_region_mode);
                tsmiFixedSize.Checked = Config.IsFixedSize;
                tsmiFixedSize.CheckOnClick = true;
                tsmiFixedSize.Click += (sender, e) => Config.IsFixedSize = tsmiFixedSize.Checked;
                tsddbOptions.DropDownItems.Add(tsmiFixedSize);

                ToolStripDoubleLabeledNumericUpDown tslnudFixedSize = new ToolStripDoubleLabeledNumericUpDown(Resources.ShapeManager_CreateContextMenu_Width_,
                    Resources.ShapeManager_CreateContextMenu_Height_);
                tslnudFixedSize.Content.Minimum = 10;
                tslnudFixedSize.Content.Maximum = 10000;
                tslnudFixedSize.Content.Increment = 10;
                tslnudFixedSize.Content.Value = Config.FixedSize.Width;
                tslnudFixedSize.Content.Value2 = Config.FixedSize.Height;
                tslnudFixedSize.Content.ValueChanged = (sender, e) => Config.FixedSize = new Size((int)tslnudFixedSize.Content.Value, (int)tslnudFixedSize.Content.Value2);
                tsddbOptions.DropDownItems.Add(tslnudFixedSize);
            }

            ToolStripMenuItem tsmiShowFPS = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_Show_FPS);
            tsmiShowFPS.Checked = Config.ShowFPS;
            tsmiShowFPS.CheckOnClick = true;
            tsmiShowFPS.Click += (sender, e) => Config.ShowFPS = tsmiShowFPS.Checked;
            tsddbOptions.DropDownItems.Add(tsmiShowFPS);

            ToolStripMenuItem tsmiRememberMenuState = new ToolStripMenuItem(Resources.ShapeManager_CreateContextMenu_RememberMenuState);
            tsmiRememberMenuState.Checked = Config.RememberMenuState;
            tsmiRememberMenuState.CheckOnClick = true;
            tsmiRememberMenuState.Click += (sender, e) => Config.RememberMenuState = tsmiRememberMenuState.Checked;
            tsddbOptions.DropDownItems.Add(tsmiRememberMenuState);

            #endregion Options

            ToolStripLabel tslDragRight = new ToolStripLabel()
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

            foreach (ToolStripItem tsi in tsMain.Items.OfType<ToolStripItem>())
            {
                if (!string.IsNullOrEmpty(tsi.Text))
                {
                    tsi.MouseEnter += (sender, e) =>
                    {
                        Point pos = form.PointToClient(menuForm.PointToScreen(tsi.Bounds.Location));
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

            menuForm.Show(form);

            UpdateMenu();

            CurrentShapeChanged += shape => UpdateMenu();
            CurrentShapeTypeChanged += shapeType => UpdateMenu();
            ShapeCreated += shape => UpdateMenu();

            ConfigureMenuState();

            form.Activate();

            ToolbarCreated = true;
        }

        private void MenuForm_Shown(object sender, EventArgs e)
        {
            form.toolbarAnimationRectangle = form.RectangleToClient(menuForm.Bounds);

            form.toolbarAnimation = new OpacityAnimation()
            {
                FadeInDuration = TimeSpan.FromMilliseconds(500),
                Duration = TimeSpan.FromMilliseconds(500),
                FadeOutDuration = TimeSpan.FromMilliseconds(500)
            };

            form.toolbarAnimation.Start();
        }

        private void MenuForm_KeyDown(object sender, KeyEventArgs e)
        {
            form_KeyDown(sender, e);
            form.RegionCaptureForm_KeyDown(sender, e);

            e.Handled = true;
        }

        private void MenuForm_KeyUp(object sender, KeyEventArgs e)
        {
            form_KeyUp(sender, e);
            form.RegionCaptureForm_KeyUp(sender, e);

            e.Handled = true;
        }

        private void MenuForm_LocationChanged(object sender, EventArgs e)
        {
            CheckMenuPosition();
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
                SetMenuCollapsed(!IsMenuCollapsed);
                CheckMenuPosition();
            }
        }

        private void ConfigureMenuState()
        {
            if (Config.RememberMenuState)
            {
                SetMenuCollapsed(Config.MenuCollapsed);
            }

            UpdateMenuPosition();
        }

        internal void UpdateMenuPosition()
        {
            Rectangle rectScreen = form.RectangleToScreen(form.ScreenRectangle0Based);

            if (Config.RememberMenuState && rectScreen.Contains(Config.MenuPosition))
            {
                menuForm.Location = Config.MenuPosition;
            }
            else
            {
                //Rectangle rectActiveScreen = CaptureHelpers.GetActiveScreenBounds();

                if (tsMain.Width < rectScreen.Width)
                {
                    menuForm.Location = new Point(rectScreen.X + rectScreen.Width / 2 - tsMain.Width / 2, rectScreen.Y);
                }
                else
                {
                    menuForm.Location = rectScreen.Location;
                }
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

            if (Config.RememberMenuState)
            {
                Config.MenuPosition = pos;
            }
        }

        private void SetMenuCollapsed(bool isCollapsed)
        {
            if (IsMenuCollapsed == isCollapsed)
            {
                return;
            }

            IsMenuCollapsed = isCollapsed;

            if (IsMenuCollapsed)
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

            if (Config.RememberMenuState)
            {
                Config.MenuCollapsed = IsMenuCollapsed;
            }
        }

        private void UpdateMenu()
        {
            if (menuForm == null) return;

            ShapeType shapeType = CurrentShapeType;

            foreach (ToolStripButton tsb in tsMain.Items.OfType<ToolStripButton>().Where(x => x.Tag is ShapeType))
            {
                if ((ShapeType)tsb.Tag == shapeType)
                {
                    tsb.RadioCheck();
                    break;
                }
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

            tslnudBlurRadius.Content.Value = AnnotationOptions.BlurRadius;

            tslnudPixelateSize.Content.Value = AnnotationOptions.PixelateSize;

            if (tsbHighlightColor.Image != null) tsbHighlightColor.Image.Dispose();
            tsbHighlightColor.Image = ImageHelpers.CreateColorPickerIcon(AnnotationOptions.HighlightColor, new Rectangle(0, 0, 16, 16));

            tsmiShadow.Checked = AnnotationOptions.Shadow;

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
                    tsbBorderColor.Visible = true;
                    tslnudBorderSize.Visible = true;
                    tsmiShadow.Visible = true;
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
            tscbCursorTypes.Visible = shapeType == ShapeType.DrawingCursor;
            tslnudBlurRadius.Visible = shapeType == ShapeType.EffectBlur;
            tslnudPixelateSize.Visible = shapeType == ShapeType.EffectPixelate;
            tsbHighlightColor.Visible = shapeType == ShapeType.EffectHighlight;

            if (tsmiRegionCapture != null)
            {
                tsmiRegionCapture.Visible = !Config.QuickCrop && ValidRegions.Length > 0;
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