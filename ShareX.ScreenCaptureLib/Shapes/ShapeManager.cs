#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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

        private ShapeType currentShapeType;

        public ShapeType CurrentShapeType
        {
            get
            {
                return currentShapeType;
            }
            private set
            {
                currentShapeType = value;

                if (form.IsAnnotationMode)
                {
                    if (IsCurrentShapeTypeRegion)
                    {
                        Config.LastRegionTool = CurrentShapeType;
                    }
                    else
                    {
                        Config.LastAnnotationTool = CurrentShapeType;
                    }
                }

                DeselectCurrentShape();

                OnCurrentShapeTypeChanged(currentShapeType);
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

        public BaseShape CurrentHoverShape { get; private set; }

        public bool IsCurrentHoverShapeValid => CurrentHoverShape != null && CurrentHoverShape.IsValidShape;

        public bool IsCurrentShapeTypeRegion
        {
            get
            {
                return IsShapeTypeRegion(CurrentShapeType);
            }
        }

        public bool IsCreating { get; set; }
        public bool IsMoving { get; set; }
        public bool IsResizing { get; set; }
        public bool IsCornerMoving { get; private set; }
        public bool IsProportionalResizing { get; private set; }
        public bool IsSnapResizing { get; private set; }

        public List<SimpleWindowInfo> Windows { get; set; }
        public bool WindowCaptureMode { get; set; }
        public bool IncludeControls { get; set; }

        public RegionCaptureOptions Config { get; private set; }

        public AnnotationOptions AnnotationOptions => Config.AnnotationOptions;

        public List<ResizeNode> ResizeNodes { get; private set; }

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

        public bool IsCursorOnNode => NodesVisible && ResizeNodes.Any(node => node.IsCursorHover);

        public event Action<BaseShape> CurrentShapeChanged;
        public event Action<ShapeType> CurrentShapeTypeChanged;
        public event Action<BaseShape> ShapeCreated;

        private RegionCaptureForm form;
        private bool isLeftPressed, isRightPressed, isUpPressed, isDownPressed;

        public ShapeManager(RegionCaptureForm form)
        {
            this.form = form;
            Config = form.Config;

            ResizeNodes = new List<ResizeNode>();

            for (int i = 0; i < 9; i++)
            {
                ResizeNode node = new ResizeNode();
                form.DrawableObjects.Add(node);
                ResizeNodes.Add(node);
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
                CurrentShapeType = Config.LastRegionTool;
            }
            else if (form.Mode == RegionCaptureMode.Editor)
            {
                CurrentShapeType = Config.LastAnnotationTool;
            }
            else
            {
                CurrentShapeType = ShapeType.RegionRectangle;
            }
        }

        private void form_Shown(object sender, EventArgs e)
        {
            if (form.IsAnnotationMode)
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
                else if (form.IsAnnotationMode)
                {
                    RunAction(Config.RegionCaptureActionRightClick);
                }
                else if (IsShapeIntersect())
                {
                    DeleteIntersectShape();
                }
                else
                {
                    form.Close();
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                RunAction(Config.RegionCaptureActionMiddleClick);
            }
            else if (e.Button == MouseButtons.XButton1)
            {
                RunAction(Config.RegionCaptureActionX1Click);
            }
            else if (e.Button == MouseButtons.XButton2)
            {
                RunAction(Config.RegionCaptureActionX2Click);
            }
        }

        private void form_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsCurrentShapeTypeRegion && ValidRegions.Length > 0)
                {
                    form.UpdateRegionPath();
                    form.Close(RegionResult.Region);
                }
                else if (CurrentShape != null && !IsCreating)
                {
                    CurrentShape.OnDoubleClicked();
                }
            }
        }

        private void form_MouseWheel(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys.HasFlag(Keys.Control) && form.Mode == RegionCaptureMode.Annotation)
            {
                if (e.Delta > 0)
                {
                    CurrentShapeType = CurrentShapeType.Previous<ShapeType>();
                }
                else if (e.Delta < 0)
                {
                    CurrentShapeType = CurrentShapeType.Next<ShapeType>();
                }
            }
            else
            {
                if (e.Delta > 0)
                {
                    Config.MagnifierPixelCount = Math.Min(Config.MagnifierPixelCount + 2, RegionCaptureOptions.MagnifierPixelCountMaximum);
                }
                else if (e.Delta < 0)
                {
                    Config.MagnifierPixelCount = Math.Max(Config.MagnifierPixelCount - 2, RegionCaptureOptions.MagnifierPixelCountMinimum);
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
                case Keys.Control | Keys.Z:
                    UndoShape();
                    break;
            }

            if (!IsCreating)
            {
                if (form.Mode == RegionCaptureMode.Annotation)
                {
                    switch (e.KeyData)
                    {
                        case Keys.Tab:
                            SwapShapeType();
                            break;
                        case Keys.NumPad0:
                            CurrentShapeType = ShapeType.RegionRectangle;
                            break;
                    }
                }

                if (form.IsAnnotationMode)
                {
                    switch (e.KeyData)
                    {
                        case Keys.NumPad1:
                            CurrentShapeType = ShapeType.DrawingRectangle;
                            break;
                        case Keys.NumPad2:
                            CurrentShapeType = ShapeType.DrawingEllipse;
                            break;
                        case Keys.NumPad3:
                            CurrentShapeType = ShapeType.DrawingFreehand;
                            break;
                        case Keys.NumPad4:
                            CurrentShapeType = ShapeType.DrawingLine;
                            break;
                        case Keys.NumPad5:
                            CurrentShapeType = ShapeType.DrawingArrow;
                            break;
                        case Keys.NumPad6:
                            CurrentShapeType = ShapeType.DrawingText;
                            break;
                        case Keys.NumPad7:
                            CurrentShapeType = ShapeType.DrawingStep;
                            break;
                        case Keys.NumPad8:
                            CurrentShapeType = ShapeType.EffectBlur;
                            break;
                        case Keys.NumPad9:
                            CurrentShapeType = ShapeType.EffectPixelate;
                            break;
                        case Keys.Control | Keys.V:
                            PasteFromClipboard();
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
                        shape.Move(x, y);
                    }
                    else
                    {
                        shape.Resize(x, y, !e.Alt);
                    }
                }
            }
        }

        private void form_KeyUp(object sender, KeyEventArgs e)
        {
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

            switch (e.KeyData)
            {
                case Keys.Delete:
                    DeleteCurrentShape();

                    if (IsCreating)
                    {
                        EndRegionSelection();
                    }
                    break;
            }

            if (form.IsAnnotationMode)
            {
                switch (e.KeyData)
                {
                    case Keys.Q:
                        Config.QuickCrop = !Config.QuickCrop;
                        tsmiQuickCrop.Checked = !Config.QuickCrop;
                        break;
                }
            }
        }

        private void RunAction(RegionCaptureAction action)
        {
            switch (action)
            {
                case RegionCaptureAction.CancelCapture:
                    form.Close();
                    break;
                case RegionCaptureAction.RemoveShapeCancelCapture:
                    if (IsShapeIntersect())
                    {
                        DeleteIntersectShape();
                    }
                    else
                    {
                        form.Close();
                    }
                    break;
                case RegionCaptureAction.RemoveShape:
                    DeleteIntersectShape();
                    break;
                case RegionCaptureAction.SwapToolType:
                    SwapShapeType();
                    break;
                case RegionCaptureAction.CaptureFullscreen:
                    form.Close(RegionResult.Fullscreen);
                    break;
                case RegionCaptureAction.CaptureActiveMonitor:
                    form.Close(RegionResult.ActiveMonitor);
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

            CheckHover();

            UpdateNodes();
        }

        private void StartRegionSelection()
        {
            if (IsCursorOnNode)
            {
                return;
            }

            BaseShape shape = GetIntersectShape();

            if (shape != null && shape.ShapeType == CurrentShapeType) // Select shape
            {
                IsMoving = true;
                CurrentShape = shape;
                SelectCurrentShape();
            }
            else if (!IsCreating) // Create new shape
            {
                DeselectCurrentShape();

                shape = AddShape();
                shape.OnCreating();
            }
        }

        private void EndRegionSelection()
        {
            bool wasCreating = IsCreating;

            IsCreating = false;
            IsMoving = false;

            BaseShape shape = CurrentShape;

            if (shape != null)
            {
                if (!shape.IsValidShape)
                {
                    shape.Rectangle = Rectangle.Empty;

                    CheckHover();

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
                    if (Config.QuickCrop && IsCurrentShapeTypeRegion)
                    {
                        form.UpdateRegionPath();
                        form.Close(RegionResult.Region);
                    }
                    else
                    {
                        if (wasCreating)
                        {
                            shape.OnCreated();

                            OnShapeCreated(shape);
                        }

                        SelectCurrentShape();
                    }
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
            return CreateShape(CurrentShapeType);
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
                case ShapeType.DrawingText:
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
                case ShapeType.EffectBlur:
                    shape = new BlurEffectShape();
                    break;
                case ShapeType.EffectPixelate:
                    shape = new PixelateEffectShape();
                    break;
                case ShapeType.EffectHighlight:
                    shape = new HighlightEffectShape();
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
            }
        }

        private void SwapShapeType()
        {
            if (form.Mode == RegionCaptureMode.Annotation)
            {
                if (IsCurrentShapeTypeRegion)
                {
                    CurrentShapeType = Config.LastAnnotationTool;
                }
                else
                {
                    CurrentShapeType = Config.LastRegionTool;
                }
            }
        }

        public Point SnapPosition(Point posOnClick, Point posCurrent)
        {
            Size currentSize = CaptureHelpers.CreateRectangle(posOnClick, posCurrent).Size;
            Vector2 vector = new Vector2(currentSize.Width, currentSize.Height);

            SnapSize snapSize = (from size in Config.SnapSizes
                                 let distance = MathHelpers.Distance(vector, new Vector2(size.Width, size.Height))
                                 where distance > 0 && distance < RegionCaptureOptions.SnapDistance
                                 orderby distance
                                 select size).FirstOrDefault();

            if (snapSize != null)
            {
                Point posNew = CaptureHelpers.CalculateNewPosition(posOnClick, posCurrent, snapSize);

                Rectangle newRect = CaptureHelpers.CreateRectangle(posOnClick, posNew);

                if (form.ScreenRectangle0Based.Contains(newRect))
                {
                    return posNew;
                }
            }

            return posCurrent;
        }

        private void CheckHover()
        {
            CurrentHoverShape = null;

            if (!IsCursorOnNode && !IsCreating && !IsMoving && !IsResizing)
            {
                BaseShape shape = GetIntersectShape();

                if (shape != null && shape.IsValidShape)
                {
                    CurrentHoverShape = shape;
                }
                else
                {
                    switch (CurrentShapeType)
                    {
                        case ShapeType.RegionFreehand:
                        case ShapeType.DrawingFreehand:
                        case ShapeType.DrawingLine:
                        case ShapeType.DrawingArrow:
                        case ShapeType.DrawingText:
                        case ShapeType.DrawingSpeechBalloon:
                        case ShapeType.DrawingStep:
                        case ShapeType.DrawingImage:
                            return;
                    }

                    if (Config.IsFixedSize && IsCurrentShapeTypeRegion)
                    {
                        Point location = InputManager.MousePosition0Based;

                        CurrentHoverShape = new RectangleRegionShape()
                        {
                            Rectangle = new Rectangle(new Point(location.X - Config.FixedSize.Width / 2, location.Y - Config.FixedSize.Height / 2), Config.FixedSize)
                        };
                    }
                    else
                    {
                        SimpleWindowInfo window = FindSelectedWindow();

                        if (window != null && !window.Rectangle.IsEmpty)
                        {
                            Rectangle hoverArea = CaptureHelpers.ScreenToClient(window.Rectangle);

                            CurrentHoverShape = new RectangleRegionShape()
                            {
                                Rectangle = Rectangle.Intersect(form.ScreenRectangle0Based, hoverArea)
                            };
                        }
                    }
                }
            }
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

        public BaseShape GetIntersectShape()
        {
            return GetIntersectShape(InputManager.MousePosition0Based);
        }

        public BaseShape GetIntersectShape(Point position)
        {
            for (int i = Shapes.Count - 1; i >= 0; i--)
            {
                BaseShape shape = Shapes[i];

                if (shape.ShapeType == CurrentShapeType && shape.Intersects(position))
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
                else
                {
                    IsResizing = false;
                }

                shape.OnNodePositionUpdate();
            }
        }

        public void PauseForm()
        {
            form.Pause();
        }

        public void ResumeForm()
        {
            form.Resume();
        }

        public void OrderStepShapes()
        {
            int i = 1;

            foreach (StepDrawingShape shape in Shapes.OfType<StepDrawingShape>())
            {
                shape.Number = i++;
            }
        }

        private void PasteFromClipboard()
        {
            if (Clipboard.ContainsImage())
            {
                Image img = ClipboardHelpers.GetImage();

                if (img != null)
                {
                    CurrentShapeType = ShapeType.DrawingImage;
                    ImageDrawingShape shape = (ImageDrawingShape)CreateShape(ShapeType.DrawingImage);
                    shape.StartPosition = shape.EndPosition = InputManager.MousePosition0Based;
                    shape.SetImage(img, true);
                    AddShape(shape);
                    SelectCurrentShape();
                }
            }
            else if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();

                if (!string.IsNullOrEmpty(text))
                {
                    CurrentShapeType = ShapeType.DrawingText;
                    TextDrawingShape shape = (TextDrawingShape)CreateShape(ShapeType.DrawingText);
                    shape.StartPosition = shape.EndPosition = InputManager.MousePosition0Based;
                    shape.Text = text.Trim();
                    shape.AutoSize(true);
                    AddShape(shape);
                    SelectCurrentShape();
                }
            }
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