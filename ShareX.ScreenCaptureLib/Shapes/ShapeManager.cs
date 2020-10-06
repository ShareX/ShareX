#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
using ShareX.ImageEffectsLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    internal partial class ShapeManager : IDisposable
    {
        public List<BaseShape> Shapes { get; private set; } = new List<BaseShape>();

        private BaseShape currentShape;

        public BaseShape CurrentShape
        {
            get
            {
                return currentShape;
            }
            private set
            {
                currentShape = value;

                if (currentShape != null)
                {
                    currentShape.OnConfigSave();
                }

                OnCurrentShapeChanged(currentShape);
            }
        }

        private ShapeType currentTool;

        public ShapeType CurrentTool
        {
            get
            {
                return currentTool;
            }
            set
            {
                if (currentTool == value) return;

                currentTool = value;

                if (Form.IsAnnotationMode)
                {
                    if (IsCurrentShapeTypeRegion)
                    {
                        Options.LastRegionTool = CurrentTool;
                    }
                    else if (Form.IsEditorMode)
                    {
                        Options.LastEditorTool = CurrentTool;
                    }
                    else
                    {
                        Options.LastAnnotationTool = CurrentTool;
                    }

                    ClearTools();
                }

                if (CurrentShape != null)
                {
                    // do not keep selection if select tool does not handle it
                    if (currentTool == ShapeType.ToolSelect)
                    {
                        if (!CurrentShape.IsHandledBySelectTool)
                        {
                            DeselectCurrentShape();
                        }
                    }
                    // do not keep selection if we switch away from a tool and the selected shape does not match the new type
                    else if (CurrentShape.ShapeType != currentTool)
                    {
                        DeselectCurrentShape();
                    }
                }

                OnCurrentShapeTypeChanged(currentTool);
            }
        }

        public ShapeType CurrentShapeTool
        {
            get
            {
                ShapeType tool = CurrentTool;

                if (tool == ShapeType.ToolSelect)
                {
                    BaseShape shape = CurrentShape;

                    if (shape != null)
                    {
                        tool = shape.ShapeType;
                    }
                }

                return tool;
            }
        }

        public Rectangle CurrentRectangle
        {
            get
            {
                if (CurrentShape != null)
                {
                    return CurrentShape.Rectangle;
                }

                return Rectangle.Empty;
            }
        }

        public bool IsCurrentShapeValid => CurrentShape != null && CurrentShape.IsValidShape;

        public BaseShape[] Regions => Shapes.OfType<BaseRegionShape>().ToArray();

        public BaseShape[] ValidRegions => Regions.Where(x => x.IsValidShape).ToArray();

        public BaseDrawingShape[] DrawingShapes => Shapes.OfType<BaseDrawingShape>().ToArray();

        public BaseEffectShape[] EffectShapes => Shapes.OfType<BaseEffectShape>().ToArray();

        public BaseTool[] ToolShapes => Shapes.OfType<BaseTool>().ToArray();

        private BaseShape currentHoverShape;

        public BaseShape CurrentHoverShape
        {
            get
            {
                return currentHoverShape;
            }
            private set
            {
                if (currentHoverShape != null)
                {
                    if (PreviousHoverRectangle == Rectangle.Empty || PreviousHoverRectangle != currentHoverShape.Rectangle)
                    {
                        PreviousHoverRectangle = currentHoverShape.Rectangle;
                    }
                }
                else
                {
                    PreviousHoverRectangle = Rectangle.Empty;
                }

                currentHoverShape = value;
            }
        }

        public Rectangle PreviousHoverRectangle { get; private set; }

        public bool IsCurrentHoverShapeValid => CurrentHoverShape != null && CurrentHoverShape.IsValidShape;

        public bool IsCurrentShapeTypeRegion => IsShapeTypeRegion(CurrentTool);
        public int StartingStepNumber { get; set; } = 1;

        public bool IsCreating { get; set; }

        private bool isMoving;

        public bool IsMoving
        {
            get
            {
                return isMoving;
            }
            set
            {
                if (isMoving != value)
                {
                    isMoving = value;

                    if (isMoving)
                    {
                        Form.SetHandCursor(true);
                    }
                    else
                    {
                        Form.SetDefaultCursor();
                    }
                }
            }
        }

        private bool isPanning;

        public bool IsPanning
        {
            get
            {
                return isPanning;
            }
            set
            {
                if (isPanning != value)
                {
                    isPanning = value;

                    if (isPanning)
                    {
                        Form.SetHandCursor(true);
                    }
                    else
                    {
                        Form.SetDefaultCursor();
                    }
                }
            }
        }

        public bool IsResizing { get; set; }
        // Is holding Ctrl?
        public bool IsCtrlModifier { get; private set; }
        public bool IsCornerMoving { get; private set; }
        // Is holding Shift?
        public bool IsProportionalResizing { get; private set; }
        // Is holding Alt?
        public bool IsSnapResizing { get; private set; }
        public bool IsRenderingOutput { get; private set; }
        public Point RenderOffset { get; private set; }
        public bool IsModified { get; internal set; }

        public InputManager InputManager { get; private set; } = new InputManager();
        public List<SimpleWindowInfo> Windows { get; set; }
        public bool WindowCaptureMode { get; set; }
        public bool IncludeControls { get; set; }

        public RegionCaptureOptions Options { get; private set; }

        public AnnotationOptions AnnotationOptions => Options.AnnotationOptions;

        internal List<ImageEditorControl> DrawableObjects { get; private set; }
        internal ResizeNode[] ResizeNodes { get; private set; }

        private bool nodesVisible;

        public bool NodesVisible
        {
            get
            {
                return nodesVisible;
            }
            set
            {
                nodesVisible = value;

                if (!nodesVisible)
                {
                    foreach (ResizeNode node in ResizeNodes)
                    {
                        node.Visible = false;
                    }
                }
                else
                {
                    BaseShape shape = CurrentShape;

                    if (shape != null)
                    {
                        shape.OnNodePositionUpdate();
                        shape.OnNodeVisible();
                    }
                }
            }
        }

        public bool IsCursorOnObject => DrawableObjects.Any(x => x.HandleMouseInput && x.IsCursorHover);

        public event Action<BaseShape> CurrentShapeChanged;
        public event Action<ShapeType> CurrentShapeTypeChanged;
        public event Action<BaseShape> ShapeCreated;

        internal RegionCaptureForm Form { get; private set; }

        private bool isLeftPressed, isRightPressed, isUpPressed, isDownPressed;
        private ScrollbarManager scrollbarManager;

        public ShapeManager(RegionCaptureForm form)
        {
            Form = form;
            Options = form.Options;

            DrawableObjects = new List<ImageEditorControl>();
            ResizeNodes = new ResizeNode[9];

            for (int i = 0; i < ResizeNodes.Length; i++)
            {
                ResizeNode node = new ResizeNode();
                node.SetCustomNode(form.CustomNodeImage);
                DrawableObjects.Add(node);
                ResizeNodes[i] = node;
            }

            ResizeNodes[(int)NodePosition.BottomRight].Order = 10;

            form.Shown += form_Shown;
            form.LostFocus += form_LostFocus;
            form.MouseDown += form_MouseDown;
            form.MouseUp += form_MouseUp;
            form.MouseDoubleClick += form_MouseDoubleClick;
            form.MouseWheel += form_MouseWheel;
            form.KeyDown += form_KeyDown;
            form.KeyUp += form_KeyUp;

            CurrentShape = null;

            if (form.Mode == RegionCaptureMode.Annotation)
            {
                CurrentTool = Options.LastRegionTool;
            }
            else if (form.IsEditorMode)
            {
                CurrentTool = Options.LastEditorTool;
            }
            else
            {
                CurrentTool = ShapeType.RegionRectangle;
            }

            if (form.IsEditorMode)
            {
                scrollbarManager = new ScrollbarManager(form, this);
            }

            foreach (ImageEditorControl control in DrawableObjects)
            {
                control.MouseDown += (sender, e) => Form.SetHandCursor(true);
                control.MouseUp += (sender, e) =>
                {
                    if (control.IsCursorHover)
                    {
                        Form.SetHandCursor(false);
                    }
                    else
                    {
                        Form.SetDefaultCursor();
                    }
                };
                control.MouseEnter += () => Form.SetHandCursor(false);
                control.MouseLeave += () => Form.SetDefaultCursor();
            }
        }

        private void form_Shown(object sender, EventArgs e)
        {
            if (Form.IsAnnotationMode)
            {
                CreateToolbar();
            }
        }

        private void form_LostFocus(object sender, EventArgs e)
        {
            ResetModifiers();
        }

        private void form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!IsCreating)
                {
                    StartRegionSelection();
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                if (Form.IsEditorMode)
                {
                    StartPanning();
                }
            }
        }

        private void form_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsMoving || IsCreating)
                {
                    EndRegionSelection();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (IsCreating)
                {
                    DeleteCurrentShape();
                    EndRegionSelection();
                }
                else if (Form.IsAnnotationMode)
                {
                    RunAction(Options.RegionCaptureActionRightClick);
                }
                else if (IsShapeIntersect())
                {
                    DeleteIntersectShape();
                }
                else
                {
                    Form.CloseWindow();
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                if (Form.IsEditorMode)
                {
                    EndPanning();
                }
                else
                {
                    RunAction(Options.RegionCaptureActionMiddleClick);
                }
            }
            else if (e.Button == MouseButtons.XButton1)
            {
                RunAction(Options.RegionCaptureActionX1Click);
            }
            else if (e.Button == MouseButtons.XButton2)
            {
                RunAction(Options.RegionCaptureActionX2Click);
            }
        }

        private void form_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsCurrentShapeTypeRegion && ValidRegions.Length > 0)
                {
                    Form.UpdateRegionPath();
                    Form.CloseWindow(RegionResult.Region);
                }
                else if (CurrentShape != null && !IsCreating)
                {
                    CurrentShape.OnDoubleClicked();
                }
            }
        }

        private void form_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (Options.ShowMagnifier)
                {
                    Options.MagnifierPixelCount = Math.Min(Options.MagnifierPixelCount + 2, RegionCaptureOptions.MagnifierPixelCountMaximum);
                }
                else
                {
                    Options.ShowMagnifier = true;
                }
            }
            else if (e.Delta < 0)
            {
                int magnifierPixelCount = Options.MagnifierPixelCount - 2;
                if (magnifierPixelCount < RegionCaptureOptions.MagnifierPixelCountMinimum)
                {
                    magnifierPixelCount = RegionCaptureOptions.MagnifierPixelCountMinimum;
                    Options.ShowMagnifier = false;
                }
                Options.MagnifierPixelCount = magnifierPixelCount;
            }

            if (Form.IsAnnotationMode)
            {
                tsmiShowMagnifier.Checked = Options.ShowMagnifier;
                tslnudMagnifierPixelCount.Content.Value = Options.MagnifierPixelCount;
            }
        }

        public bool HandleEscape()
        {
            // the escape key handling has 3 stages:
            // 1. initiate exit if region selection is active
            // 2. if a shape is selected, unselect it
            // 3. switch to the select tool if a any other tool is active

            switch (CurrentTool)
            {
                case ShapeType.RegionRectangle:
                case ShapeType.RegionEllipse:
                case ShapeType.RegionFreehand:
                    return false;
            }

            if (CurrentShape != null)
            {
                ClearTools();
                DeselectCurrentShape();
            }

            if (CurrentTool != ShapeType.ToolSelect)
            {
                CurrentTool = ShapeType.ToolSelect;
                return true;
            }

            return false;
        }

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    if (!IsCtrlModifier && !IsCornerMoving)
                    {
                        if (IsCreating || IsResizing)
                        {
                            IsCornerMoving = true;
                        }
                        else
                        {
                            IsCtrlModifier = true;
                        }
                    }
                    break;
                case Keys.ShiftKey:
                    IsProportionalResizing = true;
                    break;
                case Keys.Menu:
                    IsSnapResizing = true;
                    break;
                case Keys.Left:
                    isLeftPressed = true;
                    break;
                case Keys.Right:
                    isRightPressed = true;
                    break;
                case Keys.Up:
                    isUpPressed = true;
                    break;
                case Keys.Down:
                    isDownPressed = true;
                    break;
            }

            switch (e.KeyData)
            {
                case Keys.Insert:
                    if (IsCreating)
                    {
                        EndRegionSelection();
                    }
                    else
                    {
                        StartRegionSelection();
                    }
                    break;
                case Keys.Delete:
                    DeleteCurrentShape();

                    if (IsCreating)
                    {
                        EndRegionSelection();
                    }
                    break;
                case Keys.Shift | Keys.Delete:
                    DeleteAllShapes();
                    break;
            }

            if (!IsCreating)
            {
                if (Form.Mode == RegionCaptureMode.Annotation)
                {
                    switch (e.KeyData)
                    {
                        case Keys.Tab:
                            SwapShapeType();
                            break;
                        case Keys.NumPad0:
                            CurrentTool = ShapeType.RegionRectangle;
                            break;
                    }
                }

                if (Form.IsAnnotationMode)
                {
                    switch (e.KeyData)
                    {
                        case Keys.R:
                        case Keys.NumPad1:
                            CurrentTool = ShapeType.DrawingRectangle;
                            break;
                        case Keys.E:
                        case Keys.NumPad2:
                            CurrentTool = ShapeType.DrawingEllipse;
                            break;
                        case Keys.F:
                        case Keys.NumPad3:
                            CurrentTool = ShapeType.DrawingFreehand;
                            break;
                        case Keys.L:
                        case Keys.NumPad4:
                            CurrentTool = ShapeType.DrawingLine;
                            break;
                        case Keys.A:
                        case Keys.NumPad5:
                            CurrentTool = ShapeType.DrawingArrow;
                            break;
                        case Keys.O:
                        case Keys.NumPad6:
                            CurrentTool = ShapeType.DrawingTextOutline;
                            break;
                        case Keys.T:
                            CurrentTool = ShapeType.DrawingTextBackground;
                            break;
                        case Keys.S:
                            CurrentTool = ShapeType.DrawingSpeechBalloon;
                            break;
                        case Keys.I:
                        case Keys.NumPad7:
                            CurrentTool = ShapeType.DrawingStep;
                            break;
                        case Keys.B:
                        case Keys.NumPad8:
                            CurrentTool = ShapeType.EffectBlur;
                            break;
                        case Keys.P:
                        case Keys.NumPad9:
                            CurrentTool = ShapeType.EffectPixelate;
                            break;
                        case Keys.H:
                            CurrentTool = ShapeType.EffectHighlight;
                            break;
                        case Keys.Control | Keys.D:
                            DuplicateCurrrentShape(true);
                            break;
                        case Keys.Control | Keys.V:
                            PasteFromClipboard(true);
                            break;
                        case Keys.Control | Keys.Z:
                            UndoShape();
                            break;
                        case Keys.Home:
                            MoveCurrentShapeTop();
                            break;
                        case Keys.End:
                            MoveCurrentShapeBottom();
                            break;
                        case Keys.PageUp:
                            MoveCurrentShapeUp();
                            break;
                        case Keys.PageDown:
                            MoveCurrentShapeDown();
                            break;
                        case Keys.M:
                            CurrentTool = ShapeType.ToolSelect;
                            break;
                    }
                }

                if (Form.IsEditorMode)
                {
                    switch (e.KeyData)
                    {
                        case Keys.C:
                            CurrentTool = ShapeType.ToolCrop;
                            break;
                        case Keys.Control | Keys.S:
                            Form.OnSaveImageRequested();
                            break;
                        case Keys.Control | Keys.Shift | Keys.S:
                            Form.OnSaveImageAsRequested();
                            break;
                        case Keys.Control | Keys.Shift | Keys.C:
                            Form.OnCopyImageRequested();
                            break;
                        case Keys.Control | Keys.U:
                            Form.OnUploadImageRequested();
                            break;
                        case Keys.Control | Keys.P:
                            Form.OnPrintImageRequested();
                            break;
                    }
                }
            }

            int speed;

            if (e.Shift)
            {
                speed = RegionCaptureOptions.MoveSpeedMaximum;
            }
            else
            {
                speed = RegionCaptureOptions.MoveSpeedMinimum;
            }

            int x = 0;

            if (isLeftPressed)
            {
                x -= speed;
            }

            if (isRightPressed)
            {
                x += speed;
            }

            int y = 0;

            if (isUpPressed)
            {
                y -= speed;
            }

            if (isDownPressed)
            {
                y += speed;
            }

            if (x != 0 || y != 0)
            {
                BaseShape shape = CurrentShape;

                if (shape == null || IsCreating)
                {
                    Cursor.Position = Cursor.Position.Add(x, y);
                }
                else
                {
                    if (e.Control)
                    {
                        shape.OnMoving();
                        shape.Move(x, y);
                    }
                    else
                    {
                        shape.OnResizing();
                        shape.Resize(x, y, !e.Alt);
                    }
                }
            }
        }

        private void form_KeyUp(object sender, KeyEventArgs e)
        {
            bool wasMoving = isLeftPressed || isRightPressed || isUpPressed || isDownPressed;

            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    IsCtrlModifier = false;
                    IsCornerMoving = false;
                    break;
                case Keys.ShiftKey:
                    IsProportionalResizing = false;
                    break;
                case Keys.Menu:
                    IsSnapResizing = false;
                    break;
                case Keys.Left:
                    isLeftPressed = false;
                    break;
                case Keys.Right:
                    isRightPressed = false;
                    break;
                case Keys.Up:
                    isUpPressed = false;
                    break;
                case Keys.Down:
                    isDownPressed = false;
                    break;
            }

            if (!IsCreating && !IsMoving && wasMoving)
            {
                bool isMoving = isLeftPressed || isRightPressed || isUpPressed || isDownPressed;

                if (!isMoving)
                {
                    ShapeMoved();
                }
            }
        }

        private void ShapeMoved()
        {
            if (!IsCreating)
            {
                BaseShape shape = CurrentShape;

                if (shape != null)
                {
                    shape.OnMoved();
                }
            }
        }

        private void RunAction(RegionCaptureAction action)
        {
            switch (action)
            {
                case RegionCaptureAction.CancelCapture:
                    if (Form.IsEditorMode)
                    {
                        if (Form.ShowExitConfirmation())
                        {
                            Form.CloseWindow(RegionResult.AnnotateContinueTask);
                        }
                    }
                    else
                    {
                        Form.CloseWindow();
                    }
                    break;
                case RegionCaptureAction.RemoveShapeCancelCapture:
                    if (IsShapeIntersect())
                    {
                        DeleteIntersectShape();
                    }
                    else if (Form.IsEditorMode)
                    {
                        if (Form.ShowExitConfirmation())
                        {
                            Form.CloseWindow(RegionResult.AnnotateContinueTask);
                        }
                    }
                    else
                    {
                        Form.CloseWindow();
                    }
                    break;
                case RegionCaptureAction.RemoveShape:
                    DeleteIntersectShape();
                    break;
                case RegionCaptureAction.SwapToolType:
                    SwapShapeType();
                    break;
                case RegionCaptureAction.CaptureFullscreen:
                    Form.CloseWindow(RegionResult.Fullscreen);
                    break;
                case RegionCaptureAction.CaptureActiveMonitor:
                    Form.CloseWindow(RegionResult.ActiveMonitor);
                    break;
            }
        }

        public void Update()
        {
            OrderStepShapes();

            BaseShape shape = CurrentShape;

            if (shape != null)
            {
                shape.OnUpdate();
            }

            UpdateCurrentHoverShape();

            UpdateNodes();

            if (scrollbarManager != null)
            {
                scrollbarManager.Update();
            }
        }

        public void StartRegionSelection()
        {
            if (IsCursorOnObject)
            {
                return;
            }

            InputManager.Update(Form); // If it's a touch event we don't have the correct point yet, so refresh it now

            BaseShape shape = GetIntersectShape();

            if (shape != null && shape.IsSelectable) // Select shape
            {
                DeselectCurrentShape();
                IsMoving = true;
                shape.OnMoving();
                CurrentShape = shape;
                SelectCurrentShape();
            }
            else if (shape == null && CurrentTool == ShapeType.ToolSelect)
            {
                ClearTools();
                DeselectCurrentShape();
            }
            else if (!IsCreating && CurrentTool != ShapeType.ToolSelect) // Create new shape
            {
                ClearTools();
                DeselectCurrentShape();

                shape = AddShape();
                shape.OnCreating();
            }
        }

        public void EndRegionSelection()
        {
            bool wasCreating = IsCreating;
            bool wasMoving = IsMoving;

            IsCreating = false;
            IsMoving = false;

            BaseShape shape = CurrentShape;

            if (shape != null)
            {
                if (!shape.IsValidShape)
                {
                    shape.Rectangle = Rectangle.Empty;

                    UpdateCurrentHoverShape();

                    if (IsCurrentHoverShapeValid)
                    {
                        shape.Rectangle = CurrentHoverShape.Rectangle;
                    }
                    else
                    {
                        DeleteCurrentShape();
                        shape = null;
                    }
                }

                if (shape != null)
                {
                    if (Options.QuickCrop && IsCurrentShapeTypeRegion)
                    {
                        Form.UpdateRegionPath();
                        Form.CloseWindow(RegionResult.Region);
                    }
                    else
                    {
                        if (wasCreating)
                        {
                            shape.OnCreated();

                            OnShapeCreated(shape);

                            SelectCurrentShape();

                            if (Options.SwitchToSelectionToolAfterDrawing && shape.IsHandledBySelectTool)
                            {
                                CurrentTool = ShapeType.ToolSelect;
                            }
                        }
                        else if (wasMoving)
                        {
                            shape.OnMoved();
                            SelectCurrentShape();
                        }
                    }
                }
            }
        }

        private void StartPanning()
        {
            IsPanning = true;
            Form.PanningStrech = new Point(0, 0);
            Options.ShowEditorPanTip = false;
        }

        private void EndPanning()
        {
            IsPanning = false;
        }

        internal void UpdateObjects()
        {
            ImageEditorControl[] objects = DrawableObjects.OrderByDescending(x => x.Order).ToArray();

            Point position = InputManager.ClientMousePosition;

            if (objects.All(x => !x.IsDragging))
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    ImageEditorControl obj = objects[i];

                    if (obj.Visible)
                    {
                        obj.IsCursorHover = obj.Rectangle.Contains(position);

                        if (obj.IsCursorHover)
                        {
                            if (InputManager.IsMousePressed(MouseButtons.Left))
                            {
                                obj.OnMouseDown(position);
                            }

                            for (int j = i + 1; j < objects.Length; j++)
                            {
                                objects[j].IsCursorHover = false;
                            }

                            break;
                        }
                    }
                    else
                    {
                        obj.IsCursorHover = false;
                    }
                }
            }
            else
            {
                if (InputManager.IsMouseReleased(MouseButtons.Left))
                {
                    foreach (ImageEditorControl obj in objects)
                    {
                        if (obj.IsDragging)
                        {
                            obj.OnMouseUp(position);
                        }
                    }
                }
            }
        }

        internal void DrawObjects(Graphics g)
        {
            foreach (ImageEditorControl obj in DrawableObjects)
            {
                if (obj.Visible)
                {
                    obj.OnDraw(g);
                }
            }
        }

        private BaseShape AddShape()
        {
            BaseShape shape = CreateShape();
            AddShape(shape);
            return shape;
        }

        private void AddShape(BaseShape shape)
        {
            Shapes.Add(shape);
            CurrentShape = shape;

            if (shape.ShapeCategory == ShapeCategory.Drawing || shape.ShapeCategory == ShapeCategory.Effect)
            {
                IsModified = true;
            }
        }

        private BaseShape CreateShape()
        {
            return CreateShape(CurrentTool);
        }

        private BaseShape CreateShape(ShapeType shapeType)
        {
            BaseShape shape;

            switch (shapeType)
            {
                default:
                case ShapeType.RegionRectangle:
                    shape = new RectangleRegionShape();
                    break;
                case ShapeType.RegionEllipse:
                    shape = new EllipseRegionShape();
                    break;
                case ShapeType.RegionFreehand:
                    shape = new FreehandRegionShape();
                    break;
                case ShapeType.DrawingRectangle:
                    shape = new RectangleDrawingShape();
                    break;
                case ShapeType.DrawingEllipse:
                    shape = new EllipseDrawingShape();
                    break;
                case ShapeType.DrawingFreehand:
                    shape = new FreehandDrawingShape();
                    break;
                case ShapeType.DrawingLine:
                    shape = new LineDrawingShape();
                    break;
                case ShapeType.DrawingArrow:
                    shape = new ArrowDrawingShape();
                    break;
                case ShapeType.DrawingTextOutline:
                    shape = new TextOutlineDrawingShape();
                    break;
                case ShapeType.DrawingTextBackground:
                    shape = new TextDrawingShape();
                    break;
                case ShapeType.DrawingSpeechBalloon:
                    shape = new SpeechBalloonDrawingShape();
                    break;
                case ShapeType.DrawingStep:
                    shape = new StepDrawingShape();
                    break;
                case ShapeType.DrawingMagnify:
                    shape = new MagnifyDrawingShape();
                    break;
                case ShapeType.DrawingImage:
                    shape = new ImageFileDrawingShape();
                    break;
                case ShapeType.DrawingImageScreen:
                    shape = new ImageScreenDrawingShape();
                    break;
                case ShapeType.DrawingSticker:
                    shape = new StickerDrawingShape();
                    break;
                case ShapeType.DrawingCursor:
                    shape = new CursorDrawingShape();
                    break;
                case ShapeType.DrawingSmartEraser:
                    shape = new SmartEraserDrawingShape();
                    break;
                case ShapeType.EffectBlur:
                    shape = new BlurEffectShape();
                    break;
                case ShapeType.EffectPixelate:
                    shape = new PixelateEffectShape();
                    break;
                case ShapeType.EffectHighlight:
                    shape = new HighlightEffectShape();
                    break;
                case ShapeType.ToolCrop:
                    shape = new CropTool();
                    break;
            }

            shape.Manager = this;

            shape.OnConfigLoad();

            return shape;
        }

        private void UpdateCurrentShape()
        {
            BaseShape shape = CurrentShape;

            if (shape != null)
            {
                shape.OnConfigLoad();
                shape.OnMoved();
            }

            Form.Resume();
        }

        private void SwapShapeType()
        {
            if (Form.Mode == RegionCaptureMode.Annotation)
            {
                if (IsCurrentShapeTypeRegion)
                {
                    CurrentTool = Options.LastAnnotationTool;
                }
                else
                {
                    CurrentTool = Options.LastRegionTool;
                }
            }
        }

        public Point SnapPosition(Point posOnClick, Point posCurrent)
        {
            Size currentSize = CaptureHelpers.CreateRectangle(posOnClick, posCurrent).Size;
            Vector2 vector = new Vector2(currentSize.Width, currentSize.Height);

            SnapSize snapSize = (from size in Options.SnapSizes
                                 let distance = MathHelpers.Distance(vector, new Vector2(size.Width, size.Height))
                                 where distance > 0 && distance < RegionCaptureOptions.SnapDistance
                                 orderby distance
                                 select size).FirstOrDefault();

            if (snapSize != null)
            {
                Point posNew = CaptureHelpers.CalculateNewPosition(posOnClick, posCurrent, snapSize);

                Rectangle newRect = CaptureHelpers.CreateRectangle(posOnClick, posNew);

                if (Form.ClientArea.Contains(newRect))
                {
                    return posNew;
                }
            }

            return posCurrent;
        }

        private void UpdateCurrentHoverShape()
        {
            CurrentHoverShape = CheckHover();
        }

        private BaseShape CheckHover()
        {
            if (!IsCursorOnObject && !IsCreating && !IsMoving && !IsResizing)
            {
                BaseShape shape = GetIntersectShape();

                if (shape != null && shape.IsValidShape)
                {
                    return shape;
                }
                else
                {
                    switch (CurrentTool)
                    {
                        case ShapeType.RegionFreehand:
                        case ShapeType.DrawingFreehand:
                        case ShapeType.DrawingLine:
                        case ShapeType.DrawingArrow:
                        case ShapeType.DrawingTextOutline:
                        case ShapeType.DrawingTextBackground:
                        case ShapeType.DrawingSpeechBalloon:
                        case ShapeType.DrawingStep:
                        case ShapeType.DrawingImage:
                        case ShapeType.DrawingSticker:
                        case ShapeType.DrawingCursor:
                        case ShapeType.ToolSelect:
                            return null;
                    }

                    if (Options.IsFixedSize && IsCurrentShapeTypeRegion)
                    {
                        Point location = InputManager.ClientMousePosition;

                        BaseShape rectangleRegionShape = CreateShape(ShapeType.RegionRectangle);
                        rectangleRegionShape.Rectangle = new Rectangle(new Point(location.X - (Options.FixedSize.Width / 2),
                            location.Y - (Options.FixedSize.Height / 2)), Options.FixedSize);
                        return rectangleRegionShape;
                    }
                    else
                    {
                        SimpleWindowInfo window = FindSelectedWindow();

                        if (window != null && !window.Rectangle.IsEmpty)
                        {
                            Rectangle hoverArea = CaptureHelpers.ScreenToClient(window.Rectangle);

                            BaseShape rectangleRegionShape = CreateShape(ShapeType.RegionRectangle);
                            rectangleRegionShape.Rectangle = Rectangle.Intersect(Form.ClientArea, hoverArea);
                            return rectangleRegionShape;
                        }
                    }
                }
            }

            return null;
        }

        public SimpleWindowInfo FindSelectedWindow()
        {
            if (Windows != null)
            {
                return Windows.FirstOrDefault(x => x.Rectangle.Contains(InputManager.MousePosition));
            }

            return null;
        }

        public WindowInfo FindSelectedWindowInfo(Point position)
        {
            if (Windows != null)
            {
                SimpleWindowInfo windowInfo = Windows.FirstOrDefault(x => x.IsWindow && x.Rectangle.Contains(position));

                if (windowInfo != null)
                {
                    return windowInfo.WindowInfo;
                }
            }

            return null;
        }

        public Bitmap RenderOutputImage(Bitmap bmp)
        {
            return RenderOutputImage(bmp, Point.Empty);
        }

        public Bitmap RenderOutputImage(Bitmap bmp, Point offset)
        {
            Bitmap bmpOutput = (Bitmap)bmp.Clone();

            if (DrawingShapes.Length > 0 || EffectShapes.Length > 0)
            {
                IsRenderingOutput = true;
                RenderOffset = offset;

                MoveAll(-offset.X, -offset.Y);

                using (Graphics g = Graphics.FromImage(bmpOutput))
                {
                    foreach (BaseEffectShape shape in EffectShapes)
                    {
                        if (shape != null)
                        {
                            shape.OnDrawFinal(g, bmpOutput);
                        }
                    }

                    foreach (BaseDrawingShape shape in DrawingShapes)
                    {
                        if (shape != null)
                        {
                            shape.OnDraw(g);
                        }
                    }
                }

                MoveAll(offset);

                RenderOffset = Point.Empty;
                IsRenderingOutput = false;
            }

            return bmpOutput;
        }

        private void SelectShape(BaseShape shape)
        {
            if (shape != null)
            {
                shape.ShowNodes();

                if (Options.SwitchToDrawingToolAfterSelection)
                {
                    CurrentTool = shape.ShapeType;
                }
            }
        }

        private void SelectCurrentShape()
        {
            SelectShape(CurrentShape);
        }

        private void SelectIntersectShape()
        {
            BaseShape shape = GetIntersectShape();

            if (shape != null)
            {
                CurrentShape = shape;
                SelectShape(shape);
            }
        }

        private void DeselectShape(BaseShape shape)
        {
            if (shape == CurrentShape)
            {
                CurrentShape = null;
                NodesVisible = false;
            }
        }

        private void DeselectCurrentShape()
        {
            DeselectShape(CurrentShape);
        }

        public void DeleteShape(BaseShape shape)
        {
            if (shape != null)
            {
                shape.Dispose();
                Shapes.Remove(shape);
                DeselectShape(shape);

                if (shape.ShapeCategory == ShapeCategory.Drawing || shape.ShapeCategory == ShapeCategory.Effect)
                {
                    IsModified = true;
                }

                UpdateMenu();
            }
        }

        private void DeleteCurrentShape()
        {
            DeleteShape(CurrentShape);
        }

        private void DeleteIntersectShape()
        {
            DeleteShape(GetIntersectShape());
        }

        private void DeleteAllShapes()
        {
            if (Shapes.Count > 0)
            {
                foreach (BaseShape shape in Shapes)
                {
                    shape.Dispose();
                }

                Shapes.Clear();
                DeselectCurrentShape();
                IsModified = true;
            }
        }

        private void ResetModifiers()
        {
            IsCtrlModifier = IsCornerMoving = IsProportionalResizing = IsSnapResizing = false;
        }

        private void ClearTools()
        {
            foreach (BaseTool tool in ToolShapes)
            {
                tool.Dispose();
                Shapes.Remove(tool);
            }
        }

        public BaseShape GetIntersectShape()
        {
            return GetIntersectShape(InputManager.ClientMousePosition);
        }

        public BaseShape GetIntersectShape(Point position)
        {
            if (!IsCtrlModifier)
            {
                for (int i = Shapes.Count - 1; i >= 0; i--)
                {
                    BaseShape shape = Shapes[i];

                    if (shape.IsSelectable && shape.Intersects(position))
                    {
                        return shape;
                    }
                }
            }

            return null;
        }

        public bool IsShapeIntersect()
        {
            return GetIntersectShape() != null;
        }

        public void UndoShape()
        {
            if (Shapes.Count > 0)
            {
                DeleteShape(Shapes[Shapes.Count - 1]);
            }
        }

        public void MoveShapeBottom(BaseShape shape)
        {
            if (shape != null)
            {
                for (int i = 0; i < Shapes.Count; i++)
                {
                    if (Shapes[i] == shape)
                    {
                        Shapes.Move(i, 0);
                        return;
                    }
                }
            }
        }

        public void MoveCurrentShapeBottom()
        {
            MoveShapeBottom(CurrentShape);
        }

        public void MoveShapeTop(BaseShape shape)
        {
            if (shape != null)
            {
                for (int i = 0; i < Shapes.Count; i++)
                {
                    if (Shapes[i] == shape)
                    {
                        Shapes.Move(i, Shapes.Count - 1);
                        return;
                    }
                }
            }
        }

        public void MoveCurrentShapeTop()
        {
            MoveShapeTop(CurrentShape);
        }

        public void MoveShapeDown(BaseShape shape)
        {
            if (shape != null)
            {
                for (int i = 1; i < Shapes.Count; i++)
                {
                    if (Shapes[i] == shape)
                    {
                        Shapes.Move(i, --i);
                        return;
                    }
                }
            }
        }

        public void MoveCurrentShapeDown()
        {
            MoveShapeDown(CurrentShape);
        }

        public void MoveShapeUp(BaseShape shape)
        {
            if (shape != null)
            {
                for (int i = 0; i < Shapes.Count - 1; i++)
                {
                    if (Shapes[i] == shape)
                    {
                        Shapes.Move(i, ++i);
                        return;
                    }
                }
            }
        }

        public void MoveCurrentShapeUp()
        {
            MoveShapeUp(CurrentShape);
        }

        public void MoveAll(int x, int y)
        {
            if (x != 0 || y != 0)
            {
                foreach (BaseShape shape in Shapes)
                {
                    shape.Move(x, y);
                }
            }
        }

        public void MoveAll(Point offset)
        {
            MoveAll(offset.X, offset.Y);
        }

        public void RemoveOutsideShapes()
        {
            foreach (BaseShape shape in Shapes.ToArray())
            {
                if (!Form.CanvasRectangle.IntersectsWith(shape.Rectangle))
                {
                    shape.Remove();
                }
            }
        }

        private bool IsShapeTypeRegion(ShapeType shapeType)
        {
            switch (shapeType)
            {
                case ShapeType.RegionRectangle:
                case ShapeType.RegionEllipse:
                case ShapeType.RegionFreehand:
                    return true;
            }

            return false;
        }

        private void UpdateNodes()
        {
            BaseShape shape = CurrentShape;

            if (shape != null && NodesVisible)
            {
                if (InputManager.IsMouseDown(MouseButtons.Left))
                {
                    shape.OnNodeUpdate();
                }
                else if (IsResizing)
                {
                    IsResizing = false;

                    shape.OnResized();
                }

                shape.OnNodePositionUpdate();
            }
        }

        public void OrderStepShapes()
        {
            int i = StartingStepNumber;

            foreach (StepDrawingShape shape in Shapes.OfType<StepDrawingShape>())
            {
                shape.Number = i++;
            }
        }

        private void DuplicateCurrrentShape(bool insertMousePosition)
        {
            BaseShape shape = CurrentShape;

            if (shape != null && shape.IsHandledBySelectTool)
            {
                BaseShape shapeCopy = shape.Duplicate();
                if (shapeCopy != null)
                {
                    if (insertMousePosition)
                    {
                        shapeCopy.MoveAbsolute(InputManager.ClientMousePosition, true);
                    }
                    else
                    {
                        shapeCopy.Move(10, 10);
                    }

                    shapeCopy.OnMoved();
                    AddShape(shapeCopy);
                    SelectCurrentShape();
                }
            }
        }

        private void PasteFromClipboard(bool insertMousePosition)
        {
            if (Clipboard.ContainsImage())
            {
                Bitmap bmp = ClipboardHelpers.GetImage();
                InsertImage(bmp);
            }
            else if (Clipboard.ContainsFileDropList())
            {
                string[] files = ClipboardHelpers.GetFileDropList();

                if (files != null)
                {
                    string imageFilePath = files.FirstOrDefault(x => Helpers.IsImageFile(x));

                    if (!string.IsNullOrEmpty(imageFilePath))
                    {
                        Bitmap bmp = ImageHelpers.LoadImage(imageFilePath);
                        InsertImage(bmp);
                    }
                }
            }
            else if (Clipboard.ContainsText())
            {
                string text = ClipboardHelpers.GetText();

                if (!string.IsNullOrEmpty(text))
                {
                    Point pos;

                    if (insertMousePosition)
                    {
                        pos = InputManager.ClientMousePosition;
                    }
                    else
                    {
                        pos = Form.ClientArea.Center();
                    }

                    CurrentTool = ShapeType.DrawingTextBackground;
                    TextDrawingShape shape = (TextDrawingShape)CreateShape(ShapeType.DrawingTextBackground);
                    shape.Rectangle = new Rectangle(pos.X, pos.Y, 1, 1);
                    shape.Text = text.Trim();
                    shape.OnCreated();
                    AddShape(shape);
                    SelectCurrentShape();
                }
            }
        }

        public void AddCursor(IntPtr cursorHandle, Point position)
        {
            CursorDrawingShape shape = (CursorDrawingShape)CreateShape(ShapeType.DrawingCursor);
            shape.UpdateCursor(cursorHandle, position);
            Shapes.Add(shape);
        }

        public void DrawRegionArea(Graphics g, Rectangle rect, bool isAnimated, bool showAreaInfo = false)
        {
            Form.DrawRegionArea(g, rect, isAnimated);

            if (showAreaInfo)
            {
                string areaText = Form.GetAreaText(rect);
                Form.DrawAreaText(g, areaText, rect);
            }
        }

        public void UpdateCanvas(Bitmap canvas, bool centerCanvas = true)
        {
            Form.InitBackground(canvas, centerCanvas);

            foreach (BaseEffectShape effect in EffectShapes)
            {
                effect.OnMoved();
            }

            IsModified = true;
        }

        public void CropArea(Rectangle rect)
        {
            Bitmap bmp = CropImage(rect, true);

            if (bmp != null)
            {
                MoveAll(Form.CanvasRectangle.X - rect.X, Form.CanvasRectangle.Y - rect.Y);
                UpdateCanvas(bmp);
            }
        }

        public Bitmap CropImage(Rectangle rect, bool onlyIfSizeDifferent = false)
        {
            rect = CaptureHelpers.ScreenToClient(rect);
            Point offset = CaptureHelpers.ScreenToClient(Form.CanvasRectangle.Location);
            rect.X -= offset.X;
            rect.Y -= offset.Y;
            rect.Intersect(new Rectangle(0, 0, Form.Canvas.Width, Form.Canvas.Height));

            if (rect.IsValid() && (!onlyIfSizeDifferent || rect.Size != Form.Canvas.Size))
            {
                return ImageHelpers.CropBitmap(Form.Canvas, rect);
            }

            return null;
        }

        public Color GetColor(Bitmap bmp, Point pos)
        {
            if (bmp != null)
            {
                Point position = CaptureHelpers.ScreenToClient(pos);
                Point offset = CaptureHelpers.ScreenToClient(Form.CanvasRectangle.Location);
                position.X -= offset.X;
                position.Y -= offset.Y;

                if (position.X.IsBetween(0, bmp.Width - 1) && position.Y.IsBetween(0, bmp.Height - 1))
                {
                    return bmp.GetPixel(position.X, position.Y);
                }
            }

            return Color.Empty;
        }

        public Color GetCurrentColor(Bitmap bmp)
        {
            return GetColor(bmp, InputManager.ClientMousePosition);
        }

        public Color GetCurrentColor()
        {
            return GetCurrentColor(Form.Canvas);
        }

        public void NewImage()
        {
            Form.Pause();

            Bitmap bmp = NewImageForm.CreateNewImage(Options, Form);

            Form.Resume();

            if (bmp != null)
            {
                Form.ImageFilePath = "";
                DeleteAllShapes();
                UpdateMenu();
                UpdateCanvas(bmp);
            }
        }

        private void OpenImageFile()
        {
            Form.Pause();

            string filePath = ImageHelpers.OpenImageFileDialog(Form);

            Form.Resume();

            LoadImageFile(filePath);
        }

        private void LoadImageFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                Bitmap bmp = ImageHelpers.LoadImage(filePath);

                if (bmp != null)
                {
                    Form.ImageFilePath = filePath;
                    DeleteAllShapes();
                    UpdateMenu();
                    UpdateCanvas(bmp);
                }
            }
        }

        private void InsertImageFile()
        {
            Form.Pause();

            string filePath = ImageHelpers.OpenImageFileDialog(Form);

            Form.Resume();

            if (!string.IsNullOrEmpty(filePath))
            {
                Bitmap bmp = ImageHelpers.LoadImage(filePath);
                InsertImage(bmp);
            }
        }

        private void InsertImageFromScreen()
        {
            Bitmap bmp;

            try
            {
                Form.Pause();
                Form.Hide();
                menuForm.Hide();
                Thread.Sleep(250);

                bmp = RegionCaptureTasks.GetRegionImage(Options);
            }
            finally
            {
                Form.Show();
                menuForm.Show();
                Form.Resume();
            }

            InsertImage(bmp);
        }

        private void InsertImage(Image img)
        {
            if (img != null)
            {
                Point pos;
                bool centerImage;

                using (ImageInsertForm imageInsertForm = new ImageInsertForm())
                {
                    imageInsertForm.ShowDialog(Form);

                    switch (imageInsertForm.ImageInsertMethod)
                    {
                        default:
                        case ImageInsertMethod.Center:
                            pos = Form.ClientArea.Center();
                            centerImage = true;
                            break;
                        case ImageInsertMethod.CanvasExpandDown:
                            pos = new Point(Form.CanvasRectangle.X, Form.CanvasRectangle.Bottom);
                            centerImage = false;
                            ChangeCanvasSize(new Padding(0, 0, Math.Max(0, img.Width - Form.CanvasRectangle.Width), img.Height), Options.EditorCanvasColor);
                            break;
                        case ImageInsertMethod.CanvasExpandRight:
                            pos = new Point(Form.CanvasRectangle.Right, Form.CanvasRectangle.Y);
                            centerImage = false;
                            ChangeCanvasSize(new Padding(0, 0, img.Width, Math.Max(0, img.Height - Form.CanvasRectangle.Height)), Options.EditorCanvasColor);
                            break;
                    }
                }

                CurrentTool = ShapeType.DrawingImage;
                ImageDrawingShape shape = (ImageDrawingShape)CreateShape(ShapeType.DrawingImage);
                shape.Rectangle = new Rectangle(pos.X, pos.Y, 1, 1);
                shape.SetImage(img, centerImage);
                shape.OnCreated();
                AddShape(shape);
                SelectCurrentShape();
            }
        }

        private void ChangeImageSize()
        {
            Form.Pause();

            Size oldSize = Form.Canvas.Size;

            using (ImageSizeForm imageSizeForm = new ImageSizeForm(oldSize, Options.ImageEditorResizeInterpolationMode))
            {
                if (imageSizeForm.ShowDialog(Form) == DialogResult.OK)
                {
                    Size size = imageSizeForm.ImageSize;
                    Options.ImageEditorResizeInterpolationMode = imageSizeForm.InterpolationMode;

                    if (size != oldSize)
                    {
                        InterpolationMode interpolationMode = ImageHelpers.GetInterpolationMode(Options.ImageEditorResizeInterpolationMode);
                        Bitmap bmp = ImageHelpers.ResizeImage(Form.Canvas, size, interpolationMode);

                        if (bmp != null)
                        {
                            UpdateCanvas(bmp);
                        }
                    }
                }
            }

            Form.Resume();
        }

        private void ChangeCanvasSize()
        {
            Form.Pause();

            using (CanvasSizeForm canvasSizeForm = new CanvasSizeForm())
            {
                if (canvasSizeForm.ShowDialog(Form) == DialogResult.OK)
                {
                    Padding canvas = canvasSizeForm.Canvas;
                    Bitmap bmp = ImageHelpers.AddCanvas(Form.Canvas, canvas, Options.EditorCanvasColor);

                    if (bmp != null)
                    {
                        MoveAll(canvas.Left, canvas.Top);
                        UpdateCanvas(bmp);
                    }
                }
            }

            Form.Resume();
        }

        public void AutoResizeCanvas()
        {
            Rectangle canvas = Form.CanvasRectangle;
            Rectangle combinedImageRectangle = Shapes.OfType<ImageDrawingShape>().Select(x => x.Rectangle).Combine();

            if (!canvas.Contains(combinedImageRectangle))
            {
                Padding margin = new Padding(Math.Max(0, canvas.X - combinedImageRectangle.X), Math.Max(0, canvas.Y - combinedImageRectangle.Y),
                    Math.Max(0, combinedImageRectangle.Right - canvas.Right), Math.Max(0, combinedImageRectangle.Bottom - canvas.Bottom));
                ChangeCanvasSize(margin, Options.EditorCanvasColor);
            }
        }

        private void ChangeCanvasSize(Padding margin, Color canvasColor)
        {
            Bitmap bmp = ImageHelpers.AddCanvas(Form.Canvas, margin, canvasColor);

            if (bmp != null)
            {
                Form.CanvasRectangle = Form.CanvasRectangle.LocationOffset(-margin.Left, -margin.Top);
                UpdateCanvas(bmp, false);
            }
        }

        private void AddCropTool()
        {
            CurrentTool = ShapeType.ToolCrop;
            CropTool tool = (CropTool)CreateShape(ShapeType.ToolCrop);
            tool.Rectangle = Form.CanvasRectangle;
            tool.OnCreated();
            AddShape(tool);
            SelectCurrentShape();
        }

        private void AutoCropImage()
        {
            Rectangle source = new Rectangle(0, 0, Form.Canvas.Width, Form.Canvas.Height);
            Rectangle crop;

            using (Bitmap resultImage = Form.GetResultImage())
            {
                crop = ImageHelpers.FindAutoCropRectangle(resultImage);
            }

            if (source != crop && crop.X >= 0 && crop.Y >= 0 && crop.Width > 0 && crop.Height > 0)
            {
                CurrentTool = ShapeType.ToolCrop;
                CropTool tool = (CropTool)CreateShape(ShapeType.ToolCrop);
                tool.Rectangle = crop.LocationOffset(Form.CanvasRectangle.Location);
                tool.OnCreated();
                AddShape(tool);
                SelectCurrentShape();
            }
        }

        private void RotateImage(RotateFlipType type)
        {
            Bitmap bmp = (Bitmap)Form.Canvas.Clone();
            bmp.RotateFlip(type);
            UpdateCanvas(bmp);
        }

        private void AddImageEffects()
        {
            Form.Pause();

            using (ImageEffectsForm imageEffectsForm = new ImageEffectsForm(Form.Canvas, Options.ImageEffectPresets, Options.SelectedImageEffectPreset))
            {
                imageEffectsForm.EditorMode();

                bool applyEffect = imageEffectsForm.ShowDialog(Form) == DialogResult.OK;

                Options.SelectedImageEffectPreset = imageEffectsForm.SelectedPresetIndex;

                if (applyEffect)
                {
                    ImageEffectPreset preset = imageEffectsForm.Presets.ReturnIfValidIndex(Options.SelectedImageEffectPreset);

                    if (preset != null)
                    {
                        Bitmap bmp = preset.ApplyEffects(Form.Canvas);

                        if (bmp != null)
                        {
                            UpdateCanvas(bmp);
                        }
                    }
                }
            }

            Form.Resume();
        }

        private bool PickColor(Color currentColor, out Color newColor)
        {
            Func<PointInfo> openScreenColorPicker = null;

            if (Form.IsFullscreen)
            {
                openScreenColorPicker = () =>
                {
                    using (Bitmap canvas = Form.Canvas.CloneSafe())
                    {
                        return RegionCaptureTasks.GetPointInfo(Options, canvas);
                    }
                };
            }
            else
            {
                openScreenColorPicker = () => RegionCaptureTasks.GetPointInfo(Options);
            }

            return ColorPickerForm.PickColor(currentColor, out newColor, Form, openScreenColorPicker);
        }

        private void OnCurrentShapeChanged(BaseShape shape)
        {
            if (CurrentShapeChanged != null)
            {
                CurrentShapeChanged(shape);
            }
        }

        private void OnCurrentShapeTypeChanged(ShapeType shapeType)
        {
            if (CurrentShapeTypeChanged != null)
            {
                CurrentShapeTypeChanged(shapeType);
            }
        }

        private void OnShapeCreated(BaseShape shape)
        {
            if (ShapeCreated != null)
            {
                ShapeCreated(shape);
            }
        }

        public void Dispose()
        {
            DeleteAllShapes();
        }
    }
}