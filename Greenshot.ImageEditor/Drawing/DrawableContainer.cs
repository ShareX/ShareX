/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015 Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using Greenshot.Configuration;
using Greenshot.Drawing.Fields;
using Greenshot.Drawing.Filters;
using Greenshot.Helpers;
using Greenshot.IniFile;
using Greenshot.Memento;
using Greenshot.Plugin;
using Greenshot.Plugin.Drawing;
using GreenshotPlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Greenshot.Drawing
{
    /// <summary>
    /// represents a rectangle, ellipse, label or whatever. Can contain filters, too.
    /// serializable for clipboard support
    /// Subclasses should fulfill INotifyPropertyChanged contract, i.e. call
    /// OnPropertyChanged whenever a public property has been changed.
    /// </summary>
    [Serializable]
    public abstract class DrawableContainer : AbstractFieldHolderWithChildren, IDrawableContainer
    {
        protected static readonly EditorConfiguration EditorConfig = IniConfig.GetIniSection<EditorConfiguration>();

        protected static readonly Color DefaultLineColor = Color.FromArgb(0, 150, 255);

        private bool isMadeUndoable;
        private const int M11 = 0;
        private const int M12 = 1;
        private const int M21 = 2;
        private const int M22 = 3;

        protected EditStatus _defaultEditMode = EditStatus.DRAWING;
        public EditStatus DefaultEditMode
        {
            get
            {
                return _defaultEditMode;
            }
        }

        /// <summary>
        /// The public accessible Dispose
        /// Will call the GarbageCollector to SuppressFinalize, preventing being cleaned twice
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            if (_grippers != null)
            {
                for (int i = 0; i < _grippers.Length; i++)
                {
                    if (_grippers[i] == null)
                    {
                        continue;
                    }
                    _grippers[i].Dispose();
                    _grippers[i] = null;
                }
                _grippers = null;
            }

            FieldAggregator aggProps = _parent.FieldAggregator;
            aggProps.UnbindElement(this);
        }

        ~DrawableContainer()
        {
            Dispose(false);
        }

        [NonSerialized]
        private PropertyChangedEventHandler _propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }

        public List<IFilter> Filters
        {
            get
            {
                List<IFilter> ret = new List<IFilter>();
                foreach (IFieldHolder c in Children)
                {
                    if (c is IFilter)
                    {
                        ret.Add(c as IFilter);
                    }
                }
                return ret;
            }
        }

        [NonSerialized]
        internal Surface _parent;
        public ISurface Parent
        {
            get { return _parent; }
            set { SwitchParent((Surface)value); }
        }
        [NonSerialized]
        protected Gripper[] _grippers;
        private bool layoutSuspended;

        [NonSerialized]
        private Gripper _targetGripper;

        public Gripper TargetGripper
        {
            get
            {
                return _targetGripper;
            }
        }

        [NonSerialized]
        private bool _selected;
        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                OnPropertyChanged("Selected");
            }
        }

        [NonSerialized]
        private EditStatus _status = EditStatus.UNDRAWN;
        public EditStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        private int left;
        public int Left
        {
            get
            {
                return left;
            }
            set
            {
                if (value == left)
                {
                    return;
                }
                left = value;
                DoLayout();
            }
        }

        private int top;
        public int Top
        {
            get
            {
                return top;
            }
            set
            {
                if (value == top)
                {
                    return;
                }
                top = value;
                DoLayout();
            }
        }

        private int width;
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                if (value == width)
                {
                    return;
                }
                width = value;
                DoLayout();
            }
        }

        private int height;
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                if (value == height)
                {
                    return;
                }
                height = value;
                DoLayout();
            }
        }

        public Point Location
        {
            get
            {
                return new Point(left, top);
            }
            set
            {
                left = value.X;
                top = value.Y;
            }
        }

        public Size Size
        {
            get
            {
                return new Size(width, height);
            }
            set
            {
                width = value.Width;
                height = value.Height;
            }
        }

        [NonSerialized]
        // will store current bounds of this DrawableContainer before starting a resize
        protected Rectangle _boundsBeforeResize = Rectangle.Empty;

        [NonSerialized]
        // "workbench" rectangle - used for calculating bounds during resizing (to be applied to this DrawableContainer afterwards)
        protected RectangleF _boundsAfterResize = RectangleF.Empty;

        public Rectangle Bounds
        {
            get
            {
                return GuiRectangle.GetGuiRectangle(Left, Top, Width, Height);
            }
            set
            {
                Left = Round(value.Left);
                Top = Round(value.Top);
                Width = Round(value.Width);
                Height = Round(value.Height);
            }
        }

        public virtual void ApplyBounds(RectangleF newBounds)
        {
            Left = Round(newBounds.Left);
            Top = Round(newBounds.Top);
            Width = Round(newBounds.Width);
            Height = Round(newBounds.Height);
        }

        public DrawableContainer(Surface parent)
        {
            InitializeFields();
            _parent = parent;
            InitControls();
        }

        public void Add(IFilter filter)
        {
            AddChild(filter);
        }

        public void Remove(IFilter filter)
        {
            RemoveChild(filter);
        }

        private static int Round(float f)
        {
            if (float.IsPositiveInfinity(f) || f > int.MaxValue / 2) return int.MaxValue / 2;
            if (float.IsNegativeInfinity(f) || f < int.MinValue / 2) return int.MinValue / 2;
            return (int)Math.Round(f);
        }

        private bool accountForShadowChange;
        public virtual Rectangle DrawingBounds
        {
            get
            {
                foreach (IFilter filter in Filters)
                {
                    if (filter.Invert)
                    {
                        return new Rectangle(Point.Empty, _parent.Image.Size);
                    }
                }
                // Take a base safetymargin
                int lineThickness = 5;
                if (HasField(FieldType.LINE_THICKNESS))
                {
                    lineThickness += GetFieldValueAsInt(FieldType.LINE_THICKNESS);
                }
                int offset = lineThickness / 2;

                int shadow = 0;
                if (accountForShadowChange || (HasField(FieldType.SHADOW) && GetFieldValueAsBool(FieldType.SHADOW)))
                {
                    accountForShadowChange = false;
                    shadow += 10;
                }
                return new Rectangle(Bounds.Left - offset, Bounds.Top - offset, Bounds.Width + lineThickness + shadow, Bounds.Height + lineThickness + shadow);
            }
        }

        public virtual void Invalidate()
        {
            if (Status != EditStatus.UNDRAWN)
            {
                _parent.Invalidate(DrawingBounds);
            }
        }

        public void AlignToParent(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            int lineThickness = GetFieldValueAsInt(FieldType.LINE_THICKNESS);
            if (horizontalAlignment == HorizontalAlignment.Left)
            {
                Left = lineThickness / 2;
            }
            if (horizontalAlignment == HorizontalAlignment.Right)
            {
                Left = _parent.Width - Width - lineThickness / 2;
            }
            if (horizontalAlignment == HorizontalAlignment.Center)
            {
                Left = (_parent.Width / 2) - (Width / 2) - lineThickness / 2;
            }

            if (verticalAlignment == VerticalAlignment.TOP)
            {
                Top = lineThickness / 2;
            }
            if (verticalAlignment == VerticalAlignment.BOTTOM)
            {
                Top = _parent.Height - Height - lineThickness / 2;
            }
            if (verticalAlignment == VerticalAlignment.CENTER)
            {
                Top = (_parent.Height / 2) - (Height / 2) - lineThickness / 2;
            }
        }

        public virtual bool InitContent()
        {
            return true;
        }

        public virtual void OnDoubleClick()
        {
        }

        private void InitControls()
        {
            InitGrippers();

            DoLayout();
        }

        /// <summary>
        /// Move the TargetGripper around, confined to the surface to solve BUG-1682
        /// </summary>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        protected virtual void TargetGripperMove(int newX, int newY)
        {
            Point newGripperLocation = new Point(newX, newY);
            Rectangle surfaceBounds = new Rectangle(0, 0, _parent.Width, _parent.Height);
            // Check if gripper inside the parent (surface), if not we need to move it inside
            // This was made for BUG-1682
            if (!surfaceBounds.Contains(newGripperLocation))
            {
                if (newGripperLocation.X > surfaceBounds.Right)
                {
                    newGripperLocation.X = surfaceBounds.Right - 5;
                }
                if (newGripperLocation.X < surfaceBounds.Left)
                {
                    newGripperLocation.X = surfaceBounds.Left;
                }
                if (newGripperLocation.Y > surfaceBounds.Bottom)
                {
                    newGripperLocation.Y = surfaceBounds.Bottom - 5;
                }
                if (newGripperLocation.Y < surfaceBounds.Top)
                {
                    newGripperLocation.Y = surfaceBounds.Top;
                }
            }

            _targetGripper.Left = newGripperLocation.X;
            _targetGripper.Top = newGripperLocation.Y;
        }

        /// <summary>
        /// Initialize a target gripper
        /// </summary>
        protected void InitTargetGripper(Color gripperColor, Point location)
        {
            _targetGripper = new Gripper
            {
                Cursor = Cursors.SizeAll,
                BackColor = gripperColor,
                Visible = false,
                Parent = _parent,
                Location = location
            };
            _targetGripper.MouseDown += GripperMouseDown;
            _targetGripper.MouseUp += GripperMouseUp;
            _targetGripper.MouseMove += GripperMouseMove;
            if (_parent != null)
            {
                _parent.Controls.Add(_targetGripper); // otherwise we'll attach them in switchParent
            }
        }

        protected void InitGrippers()
        {
            _grippers = new Gripper[8];
            for (int i = 0; i < _grippers.Length; i++)
            {
                _grippers[i] = new Gripper();
                _grippers[i].Position = i;
                _grippers[i].MouseDown += GripperMouseDown;
                _grippers[i].MouseUp += GripperMouseUp;
                _grippers[i].MouseMove += GripperMouseMove;
                _grippers[i].Visible = false;
                _grippers[i].Parent = _parent;
            }
            _grippers[Gripper.POSITION_TOP_CENTER].Cursor = Cursors.SizeNS;
            _grippers[Gripper.POSITION_MIDDLE_RIGHT].Cursor = Cursors.SizeWE;
            _grippers[Gripper.POSITION_BOTTOM_CENTER].Cursor = Cursors.SizeNS;
            _grippers[Gripper.POSITION_MIDDLE_LEFT].Cursor = Cursors.SizeWE;
            if (_parent != null)
            {
                _parent.Controls.AddRange(_grippers); // otherwise we'll attach them in switchParent
            }
        }

        public void SuspendLayout()
        {
            layoutSuspended = true;
        }

        public void ResumeLayout()
        {
            layoutSuspended = false;
            DoLayout();
        }

        protected virtual void DoLayout()
        {
            if (_grippers == null)
            {
                return;
            }
            if (!layoutSuspended)
            {
                int[] xChoords = { Left - 2, Left + Width / 2 - 2, Left + Width - 2 };
                int[] yChoords = { Top - 2, Top + Height / 2 - 2, Top + Height - 2 };

                _grippers[Gripper.POSITION_TOP_LEFT].Left = xChoords[0]; _grippers[Gripper.POSITION_TOP_LEFT].Top = yChoords[0];
                _grippers[Gripper.POSITION_TOP_CENTER].Left = xChoords[1]; _grippers[Gripper.POSITION_TOP_CENTER].Top = yChoords[0];
                _grippers[Gripper.POSITION_TOP_RIGHT].Left = xChoords[2]; _grippers[Gripper.POSITION_TOP_RIGHT].Top = yChoords[0];
                _grippers[Gripper.POSITION_MIDDLE_RIGHT].Left = xChoords[2]; _grippers[Gripper.POSITION_MIDDLE_RIGHT].Top = yChoords[1];
                _grippers[Gripper.POSITION_BOTTOM_RIGHT].Left = xChoords[2]; _grippers[Gripper.POSITION_BOTTOM_RIGHT].Top = yChoords[2];
                _grippers[Gripper.POSITION_BOTTOM_CENTER].Left = xChoords[1]; _grippers[Gripper.POSITION_BOTTOM_CENTER].Top = yChoords[2];
                _grippers[Gripper.POSITION_BOTTOM_LEFT].Left = xChoords[0]; _grippers[Gripper.POSITION_BOTTOM_LEFT].Top = yChoords[2];
                _grippers[Gripper.POSITION_MIDDLE_LEFT].Left = xChoords[0]; _grippers[Gripper.POSITION_MIDDLE_LEFT].Top = yChoords[1];

                if ((_grippers[Gripper.POSITION_TOP_LEFT].Left < _grippers[Gripper.POSITION_BOTTOM_RIGHT].Left && _grippers[Gripper.POSITION_TOP_LEFT].Top < _grippers[Gripper.POSITION_BOTTOM_RIGHT].Top) ||
                    _grippers[Gripper.POSITION_TOP_LEFT].Left > _grippers[Gripper.POSITION_BOTTOM_RIGHT].Left && _grippers[Gripper.POSITION_TOP_LEFT].Top > _grippers[Gripper.POSITION_BOTTOM_RIGHT].Top)
                {
                    _grippers[Gripper.POSITION_TOP_LEFT].Cursor = Cursors.SizeNWSE;
                    _grippers[Gripper.POSITION_TOP_RIGHT].Cursor = Cursors.SizeNESW;
                    _grippers[Gripper.POSITION_BOTTOM_RIGHT].Cursor = Cursors.SizeNWSE;
                    _grippers[Gripper.POSITION_BOTTOM_LEFT].Cursor = Cursors.SizeNESW;
                }
                else if ((_grippers[Gripper.POSITION_TOP_LEFT].Left > _grippers[Gripper.POSITION_BOTTOM_RIGHT].Left && _grippers[Gripper.POSITION_TOP_LEFT].Top < _grippers[Gripper.POSITION_BOTTOM_RIGHT].Top) ||
                  _grippers[Gripper.POSITION_TOP_LEFT].Left < _grippers[Gripper.POSITION_BOTTOM_RIGHT].Left && _grippers[Gripper.POSITION_TOP_LEFT].Top > _grippers[Gripper.POSITION_BOTTOM_RIGHT].Top)
                {
                    _grippers[Gripper.POSITION_TOP_LEFT].Cursor = Cursors.SizeNESW;
                    _grippers[Gripper.POSITION_TOP_RIGHT].Cursor = Cursors.SizeNWSE;
                    _grippers[Gripper.POSITION_BOTTOM_RIGHT].Cursor = Cursors.SizeNESW;
                    _grippers[Gripper.POSITION_BOTTOM_LEFT].Cursor = Cursors.SizeNWSE;
                }
                else if (_grippers[Gripper.POSITION_TOP_LEFT].Left == _grippers[Gripper.POSITION_BOTTOM_RIGHT].Left)
                {
                    _grippers[Gripper.POSITION_TOP_LEFT].Cursor = Cursors.SizeNS;
                    _grippers[Gripper.POSITION_BOTTOM_RIGHT].Cursor = Cursors.SizeNS;
                }
                else if (_grippers[Gripper.POSITION_TOP_LEFT].Top == _grippers[Gripper.POSITION_BOTTOM_RIGHT].Top)
                {
                    _grippers[Gripper.POSITION_TOP_LEFT].Cursor = Cursors.SizeWE;
                    _grippers[Gripper.POSITION_BOTTOM_RIGHT].Cursor = Cursors.SizeWE;
                }
            }
        }

        private void GripperMouseDown(object sender, MouseEventArgs e)
        {
            Gripper originatingGripper = (Gripper)sender;
            if (originatingGripper != _targetGripper)
            {
                Status = EditStatus.RESIZING;
                _boundsBeforeResize = new Rectangle(left, top, width, height);
                _boundsAfterResize = new RectangleF(_boundsBeforeResize.Left, _boundsBeforeResize.Top, _boundsBeforeResize.Width, _boundsBeforeResize.Height);
            }
            else
            {
                Status = EditStatus.MOVING;
            }
            isMadeUndoable = false;
        }

        private void GripperMouseUp(object sender, MouseEventArgs e)
        {
            Gripper originatingGripper = (Gripper)sender;
            if (originatingGripper != _targetGripper)
            {
                _boundsBeforeResize = Rectangle.Empty;
                _boundsAfterResize = RectangleF.Empty;
                isMadeUndoable = false;
            }
            Status = EditStatus.IDLE;
            Invalidate();
        }

        private void GripperMouseMove(object sender, MouseEventArgs e)
        {
            Invalidate();
            Gripper originatingGripper = (Gripper)sender;
            int absX = originatingGripper.Left + e.X;
            int absY = originatingGripper.Top + e.Y;
            if (originatingGripper == _targetGripper && Status.Equals(EditStatus.MOVING))
            {
                TargetGripperMove(absX, absY);
            }
            else if (Status.Equals(EditStatus.RESIZING))
            {
                // check if we already made this undoable
                if (!isMadeUndoable)
                {
                    // don't allow another undo until we are finished with this move
                    isMadeUndoable = true;
                    // Make undo-able
                    MakeBoundsChangeUndoable(false);
                }

                SuspendLayout();

                // reset "workbench" rectangle to current bounds
                _boundsAfterResize.X = _boundsBeforeResize.X;
                _boundsAfterResize.Y = _boundsBeforeResize.Y;
                _boundsAfterResize.Width = _boundsBeforeResize.Width;
                _boundsAfterResize.Height = _boundsBeforeResize.Height;

                // calculate scaled rectangle
                ScaleHelper.Scale(ref _boundsAfterResize, originatingGripper.Position, new PointF(absX, absY), ScaleHelper.GetScaleOptions());

                // apply scaled bounds to this DrawableContainer
                ApplyBounds(_boundsAfterResize);

                ResumeLayout();
            }
            Invalidate();
        }

        public bool hasFilters
        {
            get
            {
                return Filters.Count > 0;
            }
        }

        public abstract void Draw(Graphics graphics, RenderMode renderMode);

        public virtual void DrawContent(Graphics graphics, Bitmap bmp, RenderMode renderMode, Rectangle clipRectangle)
        {
            if (Children.Count > 0)
            {
                if (Status != EditStatus.IDLE)
                {
                    DrawSelectionBorder(graphics, Bounds);
                }
                else
                {
                    if (clipRectangle.Width != 0 && clipRectangle.Height != 0)
                    {
                        foreach (IFilter filter in Filters)
                        {
                            if (filter.Invert)
                            {
                                filter.Apply(graphics, bmp, Bounds, renderMode);
                            }
                            else
                            {
                                Rectangle drawingRect = new Rectangle(Bounds.Location, Bounds.Size);
                                drawingRect.Intersect(clipRectangle);
                                if (filter is MagnifierFilter)
                                {
                                    // quick&dirty bugfix, because MagnifierFilter behaves differently when drawn only partially
                                    // what we should actually do to resolve this is add a better magnifier which is not that special
                                    filter.Apply(graphics, bmp, Bounds, renderMode);
                                }
                                else
                                {
                                    filter.Apply(graphics, bmp, drawingRect, renderMode);
                                }
                            }
                        }
                    }
                }
            }
            Draw(graphics, renderMode);
        }

        public virtual bool Contains(int x, int y)
        {
            return Bounds.Contains(x, y);
        }

        public virtual bool ClickableAt(int x, int y)
        {
            Rectangle r = GuiRectangle.GetGuiRectangle(Left, Top, Width, Height);
            r.Inflate(5, 5);
            return r.Contains(x, y);
        }

        protected void DrawSelectionBorder(Graphics g, Rectangle rect)
        {
            using (Pen pen = new Pen(Color.MediumSeaGreen))
            {
                pen.DashPattern = new float[] { 1, 2 };
                pen.Width = 1;
                g.DrawRectangle(pen, rect);
            }
        }

        public virtual void ShowGrippers()
        {
            if (_grippers != null)
            {
                for (int i = 0; i < _grippers.Length; i++)
                {
                    if (_grippers[i].Enabled)
                    {
                        _grippers[i].Show();
                    }
                    else
                    {
                        _grippers[i].Hide();
                    }
                }
            }
            if (_targetGripper != null)
            {
                if (_targetGripper.Enabled)
                {
                    _targetGripper.Show();
                }
                else
                {
                    _targetGripper.Hide();
                }
            }
            ResumeLayout();
        }

        public virtual void HideGrippers()
        {
            SuspendLayout();
            if (_grippers != null)
            {
                for (int i = 0; i < _grippers.Length; i++)
                {
                    _grippers[i].Hide();
                }
            }
            if (_targetGripper != null)
            {
                _targetGripper.Hide();
            }
        }

        public void ResizeTo(int width, int height, int anchorPosition)
        {
            SuspendLayout();
            Width = width;
            Height = height;
            ResumeLayout();
        }

        /// <summary>
        /// Make a following bounds change on this drawablecontainer undoable!
        /// </summary>
        /// <param name="allowMerge">true means allow the moves to be merged</param>
        public void MakeBoundsChangeUndoable(bool allowMerge)
        {
            _parent.MakeUndoable(new DrawableContainerBoundsChangeMemento(this), allowMerge);
        }

        public void MoveBy(int dx, int dy)
        {
            SuspendLayout();
            Left += dx;
            Top += dy;
            ResumeLayout();
        }

        /// <summary>
        /// A handler for the MouseDown, used if you don't want the surface to handle this for you
        /// </summary>
        /// <param name="x">current mouse x</param>
        /// <param name="y">current mouse y</param>
        /// <returns>true if the event is handled, false if the surface needs to handle it</returns>
        public virtual bool HandleMouseDown(int x, int y)
        {
            Left = _boundsBeforeResize.X = x;
            Top = _boundsBeforeResize.Y = y;
            return true;
        }

        /// <summary>
        /// A handler for the MouseMove, used if you don't want the surface to handle this for you
        /// </summary>
        /// <param name="x">current mouse x</param>
        /// <param name="y">current mouse y</param>
        /// <returns>true if the event is handled, false if the surface needs to handle it</returns>
        public virtual bool HandleMouseMove(int x, int y)
        {
            Invalidate();
            SuspendLayout();

            // reset "workrbench" rectangle to current bounds
            _boundsAfterResize.X = _boundsBeforeResize.Left;
            _boundsAfterResize.Y = _boundsBeforeResize.Top;
            _boundsAfterResize.Width = x - _boundsAfterResize.Left;
            _boundsAfterResize.Height = y - _boundsAfterResize.Top;

            ScaleHelper.Scale(_boundsBeforeResize, x, y, ref _boundsAfterResize, GetAngleRoundProcessor());

            // apply scaled bounds to this DrawableContainer
            ApplyBounds(_boundsAfterResize);

            ResumeLayout();
            Invalidate();
            return true;
        }

        /// <summary>
        /// A handler for the MouseUp
        /// </summary>
        /// <param name="x">current mouse x</param>
        /// <param name="y">current mouse y</param>
        public virtual void HandleMouseUp(int x, int y)
        {
        }

        protected virtual void SwitchParent(Surface newParent)
        {
            // Target gripper
            if (_parent != null && _targetGripper != null)
            {
                _parent.Controls.Remove(_targetGripper);
            }
            // Normal grippers
            if (_parent != null && _grippers != null)
            {
                for (int i = 0; i < _grippers.Length; i++)
                {
                    _parent.Controls.Remove(_grippers[i]);
                }
            }
            else if (_grippers == null)
            {
                InitControls();
            }
            _parent = newParent;
            // Target gripper
            if (_parent != null && _targetGripper != null)
            {
                _parent.Controls.Add(_targetGripper);
            }
            // Normal grippers
            if (_parent != null && _grippers != null)
            {
                _parent.Controls.AddRange(_grippers);
            }

            foreach (IFilter filter in Filters)
            {
                filter.Parent = this;
            }
        }

        // drawablecontainers are regarded equal if they are of the same type and their bounds are equal. this should be sufficient.
        public override bool Equals(object obj)
        {
            bool ret = false;
            if (obj != null && GetType() == obj.GetType())
            {
                DrawableContainer other = obj as DrawableContainer;
                if (other != null && left == other.left && top == other.top && width == other.width && height == other.height)
                {
                    ret = true;
                }
            }
            return ret;
        }

        public override int GetHashCode()
        {
            return left.GetHashCode() ^ top.GetHashCode() ^ width.GetHashCode() ^ height.GetHashCode() ^ GetFields().GetHashCode();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                Invalidate();
            }
        }

        /// <summary>
        /// This method will be called before a field is changes.
        /// Using this makes it possible to invalidate the object as is before changing.
        /// </summary>
        /// <param name="fieldToBeChanged">The field to be changed</param>
        /// <param name="newValue">The new value</param>
        public virtual void BeforeFieldChange(Field fieldToBeChanged, object newValue)
        {
            _parent.MakeUndoable(new ChangeFieldHolderMemento(this, fieldToBeChanged), true);
            Invalidate();
        }

        /// <summary>
        /// Handle the field changed event, this should invalidate the correct bounds (e.g. when shadow comes or goes more pixels!)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleFieldChanged(object sender, FieldChangedEventArgs e)
        {
            LOG.DebugFormat("Field {0} changed", e.Field.FieldType);
            if (e.Field.FieldType == FieldType.SHADOW)
            {
                accountForShadowChange = true;
            }
            Invalidate();
        }

        /// <summary>
        /// Retrieve the Y scale from the matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static float CalculateScaleY(Matrix matrix)
        {
            return matrix.Elements[M22];
        }

        /// <summary>
        /// Retrieve the X scale from the matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static float CalculateScaleX(Matrix matrix)
        {
            return matrix.Elements[M11];
        }

        /// <summary>
        /// Retrieve the rotation angle from the matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static int CalculateAngle(Matrix matrix)
        {
            const int M11 = 0;
            const int M21 = 2;
            var radians = Math.Atan2(matrix.Elements[M21], matrix.Elements[M11]);
            return (int)-Math.Round(radians * 180 / Math.PI);
        }

        /// <summary>
        /// This method is called on a DrawableContainers when:
        /// 1) The capture on the surface is modified in such a way, that the elements would not be placed correctly.
        /// 2) Currently not implemented: an element needs to be moved, scaled or rotated.
        /// This basis implementation makes sure the coordinates of the element, including the TargetGripper, is correctly rotated/scaled/translated.
        /// But this implementation doesn't take care of any changes to the content!!
        /// </summary>
        /// <param name="matrix"></param>
        public virtual void Transform(Matrix matrix)
        {
            if (matrix == null)
            {
                return;
            }
            SuspendLayout();
            Point topLeft = new Point(Left, Top);
            Point bottomRight = new Point(Left + Width, Top + Height);
            Point[] points;
            if (TargetGripper != null)
            {
                points = new[] { topLeft, bottomRight, TargetGripper.Location };
            }
            else
            {
                points = new[] { topLeft, bottomRight };
            }
            matrix.TransformPoints(points);

            Left = points[0].X;
            Top = points[0].Y;
            Width = points[1].X - points[0].X;
            Height = points[1].Y - points[0].Y;
            if (TargetGripper != null)
            {
                TargetGripper.Location = points[points.Length - 1];
            }
            ResumeLayout();
        }

        protected virtual ScaleHelper.IDoubleProcessor GetAngleRoundProcessor()
        {
            return ScaleHelper.ShapeAngleRoundBehavior.Instance;
        }

        public virtual bool HasContextMenu
        {
            get
            {
                return true;
            }
        }

        public virtual bool HasDefaultSize
        {
            get
            {
                return false;
            }
        }

        public virtual Size DefaultSize
        {
            get
            {
                throw new NotSupportedException("Object doesn't have a default size");
            }
        }

        /// <summary>
        /// Allows to override the initializing of the fields, so we can actually have our own defaults
        /// </summary>
        protected virtual void InitializeFields()
        {
        }
    }
}