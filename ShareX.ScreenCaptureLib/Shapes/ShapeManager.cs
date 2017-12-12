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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
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
            private set
            {
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

                DeselectCurrentShape();

                OnCurrentShapeTypeChanged(currentTool);
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

        public bool IsCreating { get; set; }
        public bool IsMoving { get; set; }
        public bool IsPanning { get; set; }
        public bool IsResizing { get; set; }
        public bool IsCornerMoving { get; private set; }
        public bool IsProportionalResizing { get; private set; }
        public bool IsSnapResizing { get; private set; }
        public bool IsRenderingOutput { get; private set; }

        private bool isAnnotated;

        public bool IsAnnotated => isAnnotated || DrawingShapes.Where(x => x.ShapeType != ShapeType.DrawingCursor).Count() > 0 || EffectShapes.Length > 0;

        public InputManager InputManager { get; private set; } = new InputManager();
        public List<SimpleWindowInfo> Windows { get; set; }
        public bool WindowCaptureMode { get; set; }
        public bool IncludeControls { get; set; }

        public RegionCaptureOptions Options { get; private set; }

        public AnnotationOptions AnnotationOptions => Options.AnnotationOptions;

        internal List<DrawableObject> DrawableObjects { get; private set; }
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

        public ShapeManager(RegionCaptureForm form)
        {
            Form = form;
            Options = form.Options;

            DrawableObjects = new List<DrawableObject>();
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
            IsCornerMoving = IsProportionalResizing = IsSnapResizing = false;
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
                    Form.Close();
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
                    Form.Close(RegionResult.Region);
                }
                else if (CurrentShape != null && !IsCreating)
                {
                    CurrentShape.OnDoubleClicked();
                }
            }
        }

        private void form_MouseWheel(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys.HasFlag(Keys.Control) && Form.Mode == RegionCaptureMode.Annotation)
            {
                if (e.Delta > 0)
                {
                    CurrentTool = CurrentTool.Previous<ShapeType>();
                }
                else if (e.Delta < 0)
                {
                    CurrentTool = CurrentTool.Next<ShapeType>();
                }
            }
            else
            {
                if (e.Delta > 0)
                {
                    Options.MagnifierPixelCount = Math.Min(Options.MagnifierPixelCount + 2, RegionCaptureOptions.MagnifierPixelCountMaximum);
                }
                else if (e.Delta < 0)
                {
                    Options.MagnifierPixelCount = Math.Max(Options.MagnifierPixelCount - 2, RegionCaptureOptions.MagnifierPixelCountMinimum);
                }
            }
        }

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    IsCornerMoving = true;
                    break;
                case Keys.ShiftKey:
                    IsProportionalResizing = true;
                    break;
                case Keys.Menu:
                    IsSnapResizing = true;
                    break;
                case Keys.Left:
                case Keys.A:
                    isLeftPressed = true;
                    break;
                case Keys.Right:
                case Keys.D:
                    isRightPressed = true;
                    break;
                case Keys.Up:
                case Keys.W:
                    isUpPressed = true;
                    break;
                case Keys.Down:
                case Keys.S:
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
                case Keys.F1:
                    Options.ShowHotkeys = !Options.ShowHotkeys;
                    if (tsmiTips != null)
                    {
                        tsmiTips.Checked = Options.ShowHotkeys;
                    }
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
                        case Keys.NumPad1:
                            CurrentTool = ShapeType.DrawingRectangle;
                            break;
                        case Keys.NumPad2:
                            CurrentTool = ShapeType.DrawingEllipse;
                            break;
                        case Keys.NumPad3:
                            CurrentTool = ShapeType.DrawingFreehand;
                            break;
                        case Keys.NumPad4:
                            CurrentTool = ShapeType.DrawingLine;
                            break;
                        case Keys.NumPad5:
                            CurrentTool = ShapeType.DrawingArrow;
                            break;
                        case Keys.NumPad6:
                            CurrentTool = ShapeType.DrawingTextOutline;
                            break;
                        case Keys.NumPad7:
                            CurrentTool = ShapeType.DrawingStep;
                            break;
                        case Keys.NumPad8:
                            CurrentTool = ShapeType.EffectBlur;
                            break;
                        case Keys.NumPad9:
                            CurrentTool = ShapeType.EffectPixelate;
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
                        case Keys.Q:
                            Options.QuickCrop = !Options.QuickCrop;
                            tsmiQuickCrop.Checked = !Options.QuickCrop;
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
                    IsCornerMoving = false;
                    break;
                case Keys.ShiftKey:
                    IsProportionalResizing = false;
                    break;
                case Keys.Menu:
                    IsSnapResizing = false;
                    break;
                case Keys.Left:
                case Keys.A:
                    isLeftPressed = false;
                    break;
                case Keys.Right:
                case Keys.D:
                    isRightPressed = false;
                    break;
                case Keys.Up:
                case Keys.W:
                    isUpPressed = false;
                    break;
                case Keys.Down:
                case Keys.S:
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
                    if (Form.Mode == RegionCaptureMode.TaskEditor)
                    {
                        Form.Close(RegionResult.AnnotateContinueTask);
                    }
                    else
                    {
                        Form.Close();
                    }
                    break;
                case RegionCaptureAction.RemoveShapeCancelCapture:
                    if (IsShapeIntersect())
                    {
                        DeleteIntersectShape();
                    }
                    else if (Form.Mode == RegionCaptureMode.TaskEditor)
                    {
                        Form.Close(RegionResult.AnnotateContinueTask);
                    }
                    else
                    {
                        Form.Close();
                    }
                    break;
                case RegionCaptureAction.RemoveShape:
                    DeleteIntersectShape();
                    break;
                case RegionCaptureAction.SwapToolType:
                    SwapShapeType();
                    break;
                case RegionCaptureAction.CaptureFullscreen:
                    Form.Close(RegionResult.Fullscreen);
                    break;
                case RegionCaptureAction.CaptureActiveMonitor:
                    Form.Close(RegionResult.ActiveMonitor);
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
        }

        private void StartRegionSelection()
        {
            if (IsCursorOnObject)
            {
                return;
            }

            InputManager.Update(Form); // If it's a touch event we don't have the correct point yet, so refresh it now

            BaseShape shape = GetIntersectShape();

            if (shape != null && shape.ShapeType == CurrentTool) // Select shape
            {
                IsMoving = true;
                shape.OnMoving();
                Form.Cursor = Cursors.SizeAll;
                CurrentShape = shape;
                SelectCurrentShape();
            }
            else if (!IsCreating) // Create new shape
            {
                ClearTools();
                DeselectCurrentShape();

                shape = AddShape();
                shape.OnCreating();
            }
        }

        private void EndRegionSelection()
        {
            bool wasCreating = IsCreating;
            bool wasMoving = IsMoving;

            IsCreating = false;
            IsMoving = false;
            Form.SetDefaultCursor();

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
                        Form.Close(RegionResult.Region);
                    }
                    else
                    {
                        if (wasCreating)
                        {
                            shape.OnCreated();

                            OnShapeCreated(shape);
                        }
                        else if (wasMoving)
                        {
                            shape.OnMoved();
                        }

                        SelectCurrentShape();
                    }
                }
            }
        }

        private void StartPanning()
        {
            IsPanning = true;
            Form.PanningStrech = new Point(0, 0);
            Form.Cursor = Cursors.SizeAll;

            Options.ShowEditorPanTip = false;
        }

        private void EndPanning()
        {
            IsPanning = false;
            Form.SetDefaultCursor();
        }

        internal void UpdateObjects()
        {
            DrawableObject[] objects = DrawableObjects.OrderByDescending(x => x.Order).ToArray();

            Point position = InputManager.ClientMousePosition;

            if (objects.All(x => !x.IsDragging))
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    DrawableObject obj = objects[i];

                    if (obj.Visible)
                    {
                        obj.IsCursorHover = obj.Rectangle.Contains(position);

                        if (obj.IsCursorHover)
                        {
                            if (InputManager.IsMousePressed(MouseButtons.Left))
                            {
                                obj.OnMousePressed(position);
                            }

                            for (int j = i + 1; j < objects.Length; j++)
                            {
                                objects[j].IsCursorHover = false;
                            }

                            break;
                        }
                    }
                }
            }
            else
            {
                if (InputManager.IsMouseReleased(MouseButtons.Left))
                {
                    foreach (DrawableObject obj in objects)
                    {
                        if (obj.IsDragging)
                        {
                            obj.OnMouseReleased(position);
                        }
                    }
                }
            }
        }

        internal void DrawObjects(Graphics g)
        {
            foreach (DrawableObject obj in DrawableObjects)
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
                case ShapeType.DrawingImage:
                    shape = new ImageDrawingShape();
                    break;
                case ShapeType.DrawingImageScreen:
                    shape = new ImageScreenDrawingShape();
                    break;
                case ShapeType.DrawingCursor:
                    shape = new CursorDrawingShape();
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
                        case ShapeType.DrawingCursor:
                            return null;
                    }

                    if (Options.IsFixedSize && IsCurrentShapeTypeRegion)
                    {
                        Point location = InputManager.ClientMousePosition;

                        return new RectangleRegionShape()
                        {
                            Rectangle = new Rectangle(new Point(location.X - Options.FixedSize.Width / 2, location.Y - Options.FixedSize.Height / 2), Options.FixedSize)
                        };
                    }
                    else
                    {
                        SimpleWindowInfo window = FindSelectedWindow();

                        if (window != null && !window.Rectangle.IsEmpty)
                        {
                            Rectangle hoverArea = CaptureHelpers.ScreenToClient(window.Rectangle);

                            return new RectangleRegionShape()
                            {
                                Rectangle = Rectangle.Intersect(Form.ClientArea, hoverArea)
                            };
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

        public Image RenderOutputImage(Image img)
        {
            Bitmap bmp = new Bitmap(img);

            if (DrawingShapes.Length > 0 || EffectShapes.Length > 0)
            {
                IsRenderingOutput = true;

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    foreach (BaseEffectShape shape in EffectShapes)
                    {
                        if (shape != null)
                        {
                            shape.OnDrawFinal(g, bmp);
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

                IsRenderingOutput = false;
            }

            return bmp;
        }

        private void SelectShape(BaseShape shape)
        {
            if (shape != null)
            {
                shape.ShowNodes();
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
            foreach (BaseShape shape in Shapes)
            {
                shape.Dispose();
            }

            Shapes.Clear();
            DeselectCurrentShape();
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
            for (int i = Shapes.Count - 1; i >= 0; i--)
            {
                BaseShape shape = Shapes[i];

                if (shape.ShapeType == CurrentTool && shape.Intersects(position))
                {
                    return shape;
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
            foreach (BaseShape shape in Shapes)
            {
                shape.Move(x, y);
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
            int i = 1;

            foreach (StepDrawingShape shape in Shapes.OfType<StepDrawingShape>())
            {
                shape.Number = i++;
            }
        }

        private void PasteFromClipboard(bool insertMousePosition)
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

            if (Clipboard.ContainsImage())
            {
                Image img = ClipboardHelpers.GetImage();

                if (img != null)
                {
                    CurrentTool = ShapeType.DrawingImage;
                    ImageDrawingShape shape = (ImageDrawingShape)CreateShape(ShapeType.DrawingImage);
                    shape.Rectangle = new Rectangle(pos.X, pos.Y, 1, 1);
                    shape.SetImage(img, true);
                    shape.OnCreated();
                    AddShape(shape);
                    SelectCurrentShape();
                }
            }
            else if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();

                if (!string.IsNullOrEmpty(text))
                {
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

        public void DrawRegionArea(Graphics g, Rectangle rect, bool isAnimated)
        {
            Form.DrawRegionArea(g, rect, isAnimated);
        }

        private void UpdateCanvas(Image img)
        {
            Form.InitBackground(img);

            foreach (BaseEffectShape effect in EffectShapes)
            {
                effect.OnMoved();
            }

            isAnnotated = true;
        }

        public void CropArea(Rectangle rect)
        {
            Image img = CropImage(rect, true);

            if (img != null)
            {
                MoveAll(Form.CanvasRectangle.X - rect.X, Form.CanvasRectangle.Y - rect.Y);
                UpdateCanvas(img);
            }
        }

        public Image CropImage(Rectangle rect, bool onlyIfSizeDifferent = false)
        {
            rect = CaptureHelpers.ScreenToClient(rect);

            Point offset = CaptureHelpers.ScreenToClient(Form.CanvasRectangle.Location);

            rect.X -= offset.X;
            rect.Y -= offset.Y;

            rect.Intersect(new Rectangle(0, 0, Form.Canvas.Width, Form.Canvas.Height));

            if (rect.IsValid() && (!onlyIfSizeDifferent || rect.Size != Form.Canvas.Size))
            {
                return ImageHelpers.CropBitmap((Bitmap)Form.Canvas, rect);
            }

            return null;
        }

        private void ChangeImageSize()
        {
            Size oldSize = Form.Canvas.Size;

            using (ImageSizeForm imageSizeForm = new ImageSizeForm(oldSize, Options.ImageEditorInterpolationMode))
            {
                if (imageSizeForm.ShowDialog(Form) == DialogResult.OK)
                {
                    Size size = imageSizeForm.ImageSize;
                    Options.ImageEditorInterpolationMode = imageSizeForm.InterpolationMode;

                    if (size != oldSize)
                    {
                        InterpolationMode interpolationMode = GetInterpolationMode(Options.ImageEditorInterpolationMode);
                        Image img = ImageHelpers.ResizeImage(Form.Canvas, size, interpolationMode);

                        if (img != null)
                        {
                            UpdateCanvas(img);
                        }
                    }
                }
            }
        }

        private InterpolationMode GetInterpolationMode(ImageEditorInterpolationMode interpolationMode)
        {
            switch (interpolationMode)
            {
                default:
                case ImageEditorInterpolationMode.HighQualityBicubic:
                    return InterpolationMode.HighQualityBicubic;
                case ImageEditorInterpolationMode.Bicubic:
                    return InterpolationMode.Bicubic;
                case ImageEditorInterpolationMode.HighQualityBilinear:
                    return InterpolationMode.HighQualityBilinear;
                case ImageEditorInterpolationMode.Bilinear:
                    return InterpolationMode.Bilinear;
                case ImageEditorInterpolationMode.NearestNeighbor:
                    return InterpolationMode.NearestNeighbor;
            }
        }

        private void ChangeCanvasSize()
        {
            using (CanvasSizeForm canvasSizeForm = new CanvasSizeForm())
            {
                if (canvasSizeForm.ShowDialog(Form) == DialogResult.OK)
                {
                    Padding canvas = canvasSizeForm.Canvas;
                    Image img = ImageHelpers.AddCanvas(Form.Canvas, canvas);

                    if (img != null)
                    {
                        MoveAll(canvas.Left, canvas.Top);
                        UpdateCanvas(img);
                    }
                }
            }
        }

        private void AutoCropImage()
        {
            Bitmap bmp = ImageHelpers.AutoCropImage((Bitmap)Form.Canvas);

            if (bmp != null)
            {
                UpdateCanvas(bmp);
            }
        }

        private void RotateImage(RotateFlipType type)
        {
            Image img = (Image)Form.Canvas.Clone();
            img.RotateFlip(type);
            UpdateCanvas(img);
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