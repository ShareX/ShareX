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
    public class AreaManager
    {
        public List<BaseShape> Shapes { get; private set; } = new List<BaseShape>();

        public BaseShape CurrentShape { get; private set; }

        public ShapeType CurrentShapeType { get; set; } = ShapeType.RegionRectangle;

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
            set
            {
                if (CurrentShape != null)
                {
                    CurrentShape.Rectangle = value;
                }
            }
        }

        public BaseShape[] Regions
        {
            get
            {
                return Shapes.OfType<BaseRegionShape>().ToArray();
            }
        }

        public BaseShape[] ValidRegions
        {
            get
            {
                return Regions.Where(x => IsAreaValid(x.Rectangle)).ToArray();
            }
        }

        public bool IsCurrentRegionValid
        {
            get
            {
                if (CurrentShape is BaseRegionShape)
                {
                    return IsAreaValid(CurrentRectangle);
                }

                return false;
            }
        }

        public Rectangle CurrentHoverRectangle { get; private set; }

        public bool IsCurrentHoverAreaValid
        {
            get
            {
                return !CurrentHoverRectangle.IsEmpty;
            }
        }

        public float RoundedRectangleRadius { get; set; } = 25;
        public int RoundedRectangleRadiusIncrement { get; set; } = 3;
        public TriangleAngle TriangleAngle { get; set; } = TriangleAngle.Top;

        public Point CurrentPosition { get; private set; }
        public Point PositionOnClick { get; private set; }

        public ResizeManager ResizeManager { get; private set; }
        public bool IsCreating { get; private set; }
        public bool IsMoving { get; private set; }

        public bool IsResizing
        {
            get
            {
                return ResizeManager.IsResizing;
            }
        }

        public bool IsProportionalResizing { get; private set; }
        public bool IsSnapResizing { get; private set; }

        public List<SimpleWindowInfo> Windows { get; set; }
        public bool WindowCaptureMode { get; set; }
        public bool IncludeControls { get; set; }
        public int MinimumSize { get; set; } = 3;

        private RectangleRegion surface;

        public AreaManager(RectangleRegion surface)
        {
            this.surface = surface;

            ResizeManager = new ResizeManager(surface, this);

            surface.MouseDown += surface_MouseDown;
            surface.MouseUp += surface_MouseUp;
            surface.KeyDown += surface_KeyDown;
            surface.KeyUp += surface_KeyUp;
        }

        private void surface_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Insert:
                    if (IsCreating)
                    {
                        EndRegionSelection();
                    }
                    else
                    {
                        if (ResizeManager.Visible)
                        {
                            DeselectArea();
                        }

                        if (CurrentShape == null || CurrentShape != AreaIntersect())
                        {
                            RegionSelection(InputManager.MousePosition);
                        }
                    }
                    break;
                case Keys.ShiftKey:
                    IsProportionalResizing = true;
                    break;
                case Keys.Menu:
                    IsSnapResizing = true;
                    break;
                case Keys.NumPad1:
                    CurrentShapeType = ShapeType.RegionRectangle;
                    break;
                case Keys.NumPad2:
                    CurrentShapeType = ShapeType.RegionRoundedRectangle;
                    break;
                case Keys.NumPad3:
                    CurrentShapeType = ShapeType.RegionEllipse;
                    break;
                case Keys.NumPad4:
                    CurrentShapeType = ShapeType.RegionTriangle;
                    break;
                case Keys.NumPad5:
                    CurrentShapeType = ShapeType.RegionDiamond;
                    break;
                case Keys.NumPad0:
                    CurrentShapeType = ShapeType.DrawingRectangle;
                    break;
                case Keys.Add:
                    switch (CurrentShapeType)
                    {
                        case ShapeType.RegionRoundedRectangle:
                            RoundedRectangleRadius += RoundedRectangleRadiusIncrement;
                            UpdateRoundedRectangle();
                            break;
                        case ShapeType.RegionTriangle:
                            if (TriangleAngle == TriangleAngle.Left)
                            {
                                TriangleAngle = TriangleAngle.Top;
                            }
                            else
                            {
                                TriangleAngle++;
                            }
                            UpdateTriangle();
                            break;
                    }
                    break;
                case Keys.Subtract:
                    switch (CurrentShapeType)
                    {
                        case ShapeType.RegionRoundedRectangle:
                            RoundedRectangleRadius = Math.Max(0, RoundedRectangleRadius - RoundedRectangleRadiusIncrement);
                            UpdateRoundedRectangle();
                            break;
                        case ShapeType.RegionTriangle:
                            if (TriangleAngle == TriangleAngle.Top)
                            {
                                TriangleAngle = TriangleAngle.Left;
                            }
                            else
                            {
                                TriangleAngle--;
                            }
                            UpdateTriangle();
                            break;
                    }
                    break;
            }
        }

        private void UpdateRoundedRectangle()
        {
            if (CurrentShape != null)
            {
                RoundedRectangleRegionShape roundedRectangleShape = CurrentShape as RoundedRectangleRegionShape;

                if (roundedRectangleShape != null)
                {
                    roundedRectangleShape.Radius = RoundedRectangleRadius;
                }
            }
        }

        private void UpdateTriangle()
        {
            if (CurrentShape != null)
            {
                TriangleRegionShape triangleShape = CurrentShape as TriangleRegionShape;

                if (triangleShape != null)
                {
                    triangleShape.Angle = TriangleAngle;
                }
            }
        }

        private void surface_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ShiftKey:
                    IsProportionalResizing = false;
                    break;
                case Keys.Menu:
                    IsSnapResizing = false;
                    break;
                case Keys.Delete:
                    RemoveCurrentArea();
                    if (IsCreating)
                    {
                        EndRegionSelection();
                    }
                    break;
            }
        }

        public void Update()
        {
            if (IsMoving)
            {
                Rectangle rect = CurrentRectangle;
                rect.X += InputManager.MouseVelocity.X;
                rect.Y += InputManager.MouseVelocity.Y;
                CurrentRectangle = rect;
            }

            if (IsCreating && !CurrentRectangle.IsEmpty)
            {
                CurrentPosition = InputManager.MousePosition0Based;

                Point newPosition = CurrentPosition;

                if (IsProportionalResizing)
                {
                    newPosition = CaptureHelpers.ProportionalPosition(PositionOnClick, CurrentPosition);
                }

                if (IsSnapResizing)
                {
                    newPosition = SnapPosition(PositionOnClick, newPosition);
                }

                CurrentRectangle = CaptureHelpers.CreateRectangle(PositionOnClick, newPosition);
            }

            CheckHover();

            ResizeManager.Update();
        }

        private Point SnapPosition(Point posOnClick, Point posCurrent)
        {
            Rectangle currentRect = CaptureHelpers.CreateRectangle(posOnClick, posCurrent);
            Point newPosition = posCurrent;

            foreach (SnapSize size in surface.Config.SnapSizes)
            {
                if (currentRect.Width.IsBetween(size.Width - surface.Config.SnapDistance, size.Width + surface.Config.SnapDistance) ||
                    currentRect.Height.IsBetween(size.Height - surface.Config.SnapDistance, size.Height + surface.Config.SnapDistance))
                {
                    newPosition = CaptureHelpers.CalculateNewPosition(posOnClick, posCurrent, size);
                    break;
                }
            }

            Rectangle newRect = CaptureHelpers.CreateRectangle(posOnClick, newPosition);

            if (surface.ScreenRectangle0Based.Contains(newRect))
            {
                return newPosition;
            }

            return posCurrent;
        }

        private void CheckHover()
        {
            CurrentHoverRectangle = Rectangle.Empty;

            if (!ResizeManager.IsCursorOnNode() && !IsCreating && !IsMoving && !IsResizing)
            {
                Rectangle hoverArea = GetAreaIntersectWithMouse();

                if (!hoverArea.IsEmpty)
                {
                    CurrentHoverRectangle = hoverArea;
                }
                else
                {
                    SimpleWindowInfo window = FindSelectedWindow();

                    if (window != null && !window.Rectangle.IsEmpty)
                    {
                        hoverArea = CaptureHelpers.ScreenToClient(window.Rectangle);
                        CurrentHoverRectangle = Rectangle.Intersect(surface.ScreenRectangle0Based, hoverArea);
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

        public WindowInfo FindSelectedWindowInfo(Point mousePosition)
        {
            if (Windows != null)
            {
                SimpleWindowInfo windowInfo = Windows.FirstOrDefault(x => x.IsWindow && x.Rectangle.Contains(mousePosition));

                if (windowInfo != null)
                {
                    return windowInfo.WindowInfo;
                }
            }

            return null;
        }

        private void surface_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!IsCreating)
                {
                    RegionSelection(e.Location);
                }
            }
        }

        private void surface_MouseUp(object sender, MouseEventArgs e)
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
                CancelRegionSelection();
                if (IsCreating)
                {
                    EndRegionSelection();
                }
            }
        }

        private void RegionSelection(Point location)
        {
            if (ResizeManager.IsCursorOnNode())
            {
                return;
            }

            BaseShape shape = AreaIntersect(InputManager.MousePosition0Based);

            PositionOnClick = InputManager.MousePosition0Based;

            if (shape != null) // Select area
            {
                IsMoving = true;
                CurrentShape = shape;
                SelectArea();
            }
            else if (!IsCreating) // Create new area
            {
                DeselectArea();

                Rectangle rect;

                if (surface.Config.IsFixedSize)
                {
                    IsMoving = true;
                    rect = new Rectangle(new Point(location.X - surface.Config.FixedSize.Width / 2, location.Y - surface.Config.FixedSize.Height / 2), surface.Config.FixedSize);
                }
                else
                {
                    IsCreating = true;
                    rect = new Rectangle(location, new Size(1, 1));
                }

                AddRegionShape(rect);
            }
        }

        private void EndRegionSelection()
        {
            IsCreating = false;
            IsMoving = false;

            if (!CurrentRectangle.IsEmpty)
            {
                if (!IsCurrentRegionValid)
                {
                    RemoveCurrentArea();
                    CheckHover();
                }
                else if (surface.Config.QuickCrop)
                {
                    surface.UpdateRegionPath();
                    surface.Close(SurfaceResult.Region);
                }
                else
                {
                    SelectArea();
                }
            }

            if (!CurrentHoverRectangle.IsEmpty)
            {
                AddRegionShape(CurrentHoverRectangle);

                if (surface.Config.QuickCrop)
                {
                    surface.UpdateRegionPath();
                    surface.Close(SurfaceResult.Region);
                }
                else
                {
                    SelectArea();
                }
            }
        }

        private void CancelRegionSelection()
        {
            BaseShape shape = AreaIntersect();

            if (shape != null)
            {
                Shapes.Remove(shape);
                DeselectArea();
            }
            else
            {
                surface.Close(SurfaceResult.Close);
            }
        }

        private void AddRegionShape(Rectangle rect)
        {
            BaseShape shape = CreateRegionShape(rect);
            Shapes.Add(shape);
            CurrentShape = shape;
        }

        public BaseShape CreateRegionShape(Rectangle rect)
        {
            BaseShape shape;

            switch (CurrentShapeType)
            {
                default:
                case ShapeType.RegionRectangle:
                    shape = new RectangleRegionShape();
                    break;
                case ShapeType.RegionRoundedRectangle:
                    shape = new RoundedRectangleRegionShape()
                    {
                        Radius = RoundedRectangleRadius
                    };
                    break;
                case ShapeType.RegionEllipse:
                    shape = new EllipseRegionShape();
                    break;
                case ShapeType.RegionTriangle:
                    shape = new TriangleRegionShape()
                    {
                        Angle = TriangleAngle
                    };
                    break;
                case ShapeType.RegionDiamond:
                    shape = new DiamondRegionShape();
                    break;
                case ShapeType.DrawingRectangle:
                    shape = new RectangleDrawingShape();
                    break;
                case ShapeType.DrawingRoundedRectangle:
                    shape = new RoundedRectangleDrawingShape();
                    break;
            }

            shape.Rectangle = rect;

            return shape;
        }

        private void SelectArea()
        {
            if (!CurrentRectangle.IsEmpty && !surface.Config.IsFixedSize)
            {
                ResizeManager.Show();
            }
        }

        private void DeselectArea()
        {
            CurrentShape = null;
            ResizeManager.Hide();
        }

        private void RemoveCurrentArea()
        {
            BaseShape shape = CurrentShape;

            if (shape != null)
            {
                Shapes.Remove(shape);
                DeselectArea();
            }
        }

        private bool IsAreaValid(Rectangle rect)
        {
            return !rect.IsEmpty && rect.Width >= MinimumSize && rect.Height >= MinimumSize;
        }

        public BaseShape AreaIntersect(Point mousePosition)
        {
            for (int i = Shapes.Count - 1; i >= 0; i--)
            {
                if (Shapes[i].Rectangle.Contains(mousePosition))
                {
                    return Shapes[i];
                }
            }

            return null;
        }

        public BaseShape AreaIntersect()
        {
            return AreaIntersect(InputManager.MousePosition0Based);
        }

        public Rectangle GetAreaIntersectWithMouse()
        {
            BaseShape shape = AreaIntersect();

            if (shape != null)
            {
                return shape.Rectangle;
            }

            return Rectangle.Empty;
        }

        public bool IsAreaIntersect()
        {
            return AreaIntersect() != null;
        }

        public Rectangle CombineAreas()
        {
            BaseShape[] areas = ValidRegions;

            if (areas.Length > 0)
            {
                Rectangle rect = areas[0].Rectangle;

                for (int i = 1; i < areas.Length; i++)
                {
                    rect = Rectangle.Union(rect, areas[i].Rectangle);
                }

                return rect;
            }

            return Rectangle.Empty;
        }
    }
}