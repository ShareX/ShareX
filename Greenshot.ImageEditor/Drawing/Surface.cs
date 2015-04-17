/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2014 Thomas Braun, Jens Klingen, Robin Krom
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

using Greenshot.Core;
using Greenshot.Drawing.Fields;
using Greenshot.Helpers;
using Greenshot.IniFile;
using Greenshot.Memento;
using Greenshot.Plugin;
using Greenshot.Plugin.Drawing;
using GreenshotPlugin;
using GreenshotPlugin.Controls;
using GreenshotPlugin.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace Greenshot.Drawing
{
    /// <summary>
    /// Description of Surface.
    /// </summary>
    public class Surface : Control, ISurface
    {
        public static int Count = 0;
        private static CoreConfiguration conf = IniConfig.GetIniSection<CoreConfiguration>();

        // Property to identify the Surface ID
        private Guid _uniqueId = Guid.NewGuid();

        /// <summary>
        /// The GUID of the surface
        /// </summary>
        public Guid ID
        {
            get
            {
                return _uniqueId;
            }
            set
            {
                _uniqueId = value;
            }
        }

        /// <summary>
        /// Event handlers (do not serialize!)
        /// </summary>
        [NonSerialized]
        private SurfaceElementEventHandler _movingElementChanged;
        public event SurfaceElementEventHandler MovingElementChanged
        {
            add
            {
                _movingElementChanged += value;
            }
            remove
            {
                _movingElementChanged -= value;
            }
        }
        [NonSerialized]
        private SurfaceDrawingModeEventHandler _drawingModeChanged;
        public event SurfaceDrawingModeEventHandler DrawingModeChanged
        {
            add
            {
                _drawingModeChanged += value;
            }
            remove
            {
                _drawingModeChanged -= value;
            }
        }
        [NonSerialized]
        private SurfaceSizeChangeEventHandler _surfaceSizeChanged;
        public event SurfaceSizeChangeEventHandler SurfaceSizeChanged
        {
            add
            {
                _surfaceSizeChanged += value;
            }
            remove
            {
                _surfaceSizeChanged -= value;
            }
        }
        [NonSerialized]
        private SurfaceMessageEventHandler _surfaceMessage;
        public event SurfaceMessageEventHandler SurfaceMessage
        {
            add
            {
                _surfaceMessage += value;
            }
            remove
            {
                _surfaceMessage -= value;
            }
        }

        /// <summary>
        /// inUndoRedo makes sure we don't undo/redo while in a undo/redo action
        /// </summary>
        [NonSerialized]
        private bool _inUndoRedo;

        /// <summary>
        /// Make only one surfacemove cycle undoable, see SurfaceMouseMove
        /// </summary>
        [NonSerialized]
        private bool _isSurfaceMoveMadeUndoable;

        /// <summary>
        /// Undo/Redo stacks, should not be serialized as the file would be way to big
        /// </summary>
        [NonSerialized]
        private readonly Stack<IMemento> _undoStack = new Stack<IMemento>();
        [NonSerialized]
        private readonly Stack<IMemento> _redoStack = new Stack<IMemento>();

        /// <summary>
        /// Last save location, do not serialize!
        /// </summary>
        [NonSerialized]
        private string _lastSaveFullPath;

        /// <summary>
        /// current drawing mode, do not serialize!
        /// </summary>
        [NonSerialized]
        private DrawingModes _drawingMode = DrawingModes.None;

        /// <summary>
        /// the keyslocked flag helps with focus issues
        /// </summary>
        [NonSerialized]
        private bool _keysLocked;

        /// <summary>
        /// Location of the mouse-down (it "starts" here), do not serialize
        /// </summary>
        [NonSerialized]
        private Point _mouseStart = Point.Empty;

        /// <summary>
        /// are we in a mouse down, do not serialize
        /// </summary>
        [NonSerialized]
        private bool _mouseDown;

        /// <summary>
        /// The selected element for the mouse down, do not serialize
        /// </summary>
        [NonSerialized]
        private IDrawableContainer _mouseDownElement;

        /// <summary>
        /// all selected elements, do not serialize
        /// </summary>
        [NonSerialized]
        private DrawableContainerList selectedElements;

        /// <summary>
        /// the element we are drawing with, do not serialize
        /// </summary>
        [NonSerialized]
        private IDrawableContainer _drawingElement;

        /// <summary>
        /// the element we want to draw with (not yet drawn), do not serialize
        /// </summary>
        [NonSerialized]
        private IDrawableContainer _undrawnElement;

        /// <summary>
        /// the cropcontainer, when cropping this is set, do not serialize
        /// </summary>
        [NonSerialized]
        private IDrawableContainer _cropContainer;

        /// <summary>
        /// the brush which is used for transparent backgrounds, set by the editor, do not serialize
        /// </summary>
        [NonSerialized]
        private Brush _transparencyBackgroundBrush;

        /// <summary>
        /// The buffer is only for drawing on it when using filters (to supply access)
        /// This saves a lot of "create new bitmap" commands
        /// Should not be serialized, as it's generated.
        /// The actual bitmap is in the paintbox...
        /// TODO: Check if this buffer is still needed!
        /// </summary>
        [NonSerialized]
        private Bitmap _buffer;

        /// <summary>
        /// all stepLabels for the surface, needed with serialization
        /// </summary>
        private List<StepLabelContainer> _stepLabels = new List<StepLabelContainer>();

        public void AddStepLabel(StepLabelContainer stepLabel)
        {
            _stepLabels.Add(stepLabel);
        }

        public void RemoveStepLabel(StepLabelContainer stepLabel)
        {
            _stepLabels.Remove(stepLabel);
        }

        /// <summary>
        /// Count all the VISIBLE steplabels in the surface, up to the supplied one
        /// </summary>
        /// <param name="stopAtContainer">can be null, if not the counting stops here</param>
        /// <returns>number of steplabels before the supplied container</returns>
        public int CountStepLabels(IDrawableContainer stopAtContainer)
        {
            int number = 1;
            foreach (var possibleThis in _stepLabels)
            {
                if (possibleThis == stopAtContainer)
                {
                    break;
                }
                if (IsOnSurface(possibleThis))
                {
                    number++;
                }
            }
            return number;
        }

        /// <summary>
        /// all elements on the surface, needed with serialization
        /// </summary>
        private readonly DrawableContainerList _elements;

        /// <summary>
        /// all elements on the surface, needed with serialization
        /// </summary>
        private FieldAggregator _fieldAggregator = new FieldAggregator();

        /// <summary>
        /// the cursor container, needed with serialization as we need a direct acces to it.
        /// </summary>
        private IDrawableContainer _cursorContainer;

        /// <summary>
        /// the capture details, needed with serialization
        /// </summary>
        private ICaptureDetails _captureDetails;

        /// <summary>
        /// the modified flag specifies if the surface has had modifications after the last export.
        /// Initial state is modified, as "it's not saved"
        /// After serialization this should actually be "false" (the surface came from a stream)
        /// For now we just serialize it...
        /// </summary>
        private bool _modified = true;

        /// <summary>
        /// The image is the actual captured image, needed with serialization
        /// </summary>
        private Image _image;
        public Image Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                Size = _image.Size;
            }
        }

        /// <summary>
        /// The field aggregator is that which is used to have access to all the fields inside the currently selected elements.
        /// e.g. used to decided if and which line thickness is shown when multiple elements are selected.
        /// </summary>
        public FieldAggregator FieldAggregator
        {
            get
            {
                return _fieldAggregator;
            }
            set
            {
                _fieldAggregator = value;
            }
        }

        /// <summary>
        /// The cursor container has it's own accessor so we can find and remove this (when needed)
        /// </summary>
        public IDrawableContainer CursorContainer
        {
            get
            {
                return _cursorContainer;
            }
        }

        /// <summary>
        /// A simple getter to ask if this surface has a cursor
        /// </summary>
        public bool HasCursor
        {
            get
            {
                return _cursorContainer != null;
            }
        }

        /// <summary>
        /// A simple helper method to remove the cursor from the surface
        /// </summary>
        public void RemoveCursor()
        {
            RemoveElement(_cursorContainer, true);
            _cursorContainer = null;
        }

        /// <summary>
        /// The brush which is used to draw the transparent background
        /// </summary>
        public Brush TransparencyBackgroundBrush
        {
            get
            {
                return _transparencyBackgroundBrush;
            }
            set
            {
                _transparencyBackgroundBrush = value;
            }
        }

        /// <summary>
        /// Are the keys on this surface locked?
        /// </summary>
        public bool KeysLocked
        {
            get
            {
                return _keysLocked;
            }
            set
            {
                _keysLocked = value;
            }
        }

        /// <summary>
        /// Is this surface modified? This is only true if the surface has not been exported.
        /// </summary>
        public bool Modified
        {
            get
            {
                return _modified;
            }
            set
            {
                _modified = value;
            }
        }

        /// <summary>
        /// The DrawingMode property specifies the mode for drawing, more or less the element type.
        /// </summary>
        public DrawingModes DrawingMode
        {
            get
            {
                return _drawingMode;
            }
            set
            {
                _drawingMode = value;
                if (_drawingModeChanged != null)
                {
                    SurfaceDrawingModeEventArgs eventArgs = new SurfaceDrawingModeEventArgs();
                    eventArgs.DrawingMode = _drawingMode;
                    _drawingModeChanged.Invoke(this, eventArgs);
                }
                DeselectAllElements();
                CreateUndrawnElement();
            }
        }

        /// <summary>
        /// Property for accessing the last save "full" path
        /// </summary>
        public string LastSaveFullPath
        {
            get
            {
                return _lastSaveFullPath;
            }
            set
            {
                _lastSaveFullPath = value;
            }
        }

        /// <summary>
        /// Property for accessing the URL to which the surface was recently uploaded
        /// </summary>
        public string UploadURL
        {
            get;
            set;
        }

        /// <summary>
        /// Property for accessing the capture details
        /// </summary>
        public ICaptureDetails CaptureDetails
        {
            get
            {
                return _captureDetails;
            }
            set
            {
                _captureDetails = value;
            }
        }

        /// <summary>
        /// Base Surface constructor
        /// </summary>
        public Surface()
            : base()
        {
            Count++;
            _elements = new DrawableContainerList(_uniqueId);
            selectedElements = new DrawableContainerList(_uniqueId);
            LOG.Debug("Creating surface!");
            MouseDown += SurfaceMouseDown;
            MouseUp += SurfaceMouseUp;
            MouseMove += SurfaceMouseMove;
            MouseDoubleClick += SurfaceDoubleClick;
            Paint += SurfacePaint;
            AllowDrop = true;
            DragDrop += OnDragDrop;
            DragEnter += OnDragEnter;
            // bind selected & elements to this, otherwise they can't inform of modifications
            selectedElements.Parent = this;
            _elements.Parent = this;
            // Make sure we are visible
            Visible = true;
            TabStop = false;
            // Enable double buffering
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.ContainerControl | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
        }

        /// <summary>
        /// Private method, the current image is disposed the new one will stay.
        /// </summary>
        /// <param name="newImage">The new image</param>
        /// <param name="dispose">true if the old image needs to be disposed, when using undo this should not be true!!</param>
        private void SetImage(Image newImage, bool dispose)
        {
            // Dispose
            if (_image != null && dispose)
            {
                _image.Dispose();
            }

            // Set new values
            Image = newImage;
            Size = newImage.Size;

            _modified = true;
        }

        /// <summary>
        /// Surface constructor with an image
        /// </summary>
        /// <param name="newImage"></param>
        public Surface(Image newImage)
            : this()
        {
            LOG.DebugFormat("Got image with dimensions {0} and format {1}", newImage.Size, newImage.PixelFormat);
            SetImage(newImage, true);
        }

        /// <summary>
        /// Surface contructor with a capture
        /// </summary>
        /// <param name="capture"></param>
        public Surface(ICapture capture)
            : this(capture.Image)
        {
            // check if cursor is captured, and visible
            if (capture.Cursor != null && capture.CursorVisible)
            {
                Rectangle cursorRect = new Rectangle(capture.CursorLocation, capture.Cursor.Size);
                Rectangle captureRect = new Rectangle(Point.Empty, capture.Image.Size);
                // check if cursor is on the capture, otherwise we leave it out.
                if (cursorRect.IntersectsWith(captureRect))
                {
                    _cursorContainer = AddIconContainer(capture.Cursor, capture.CursorLocation.X, capture.CursorLocation.Y);
                    SelectElement(_cursorContainer);
                }
            }
            // Make sure the image is NOT disposed, we took the reference directly into ourselves
            ((Capture)capture).NullImage();

            _captureDetails = capture.CaptureDetails;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Count--;
                LOG.Debug("Disposing surface!");
                if (_buffer != null)
                {
                    _buffer.Dispose();
                    _buffer = null;
                }
                if (_transparencyBackgroundBrush != null)
                {
                    _transparencyBackgroundBrush.Dispose();
                    _transparencyBackgroundBrush = null;
                }

                // Cleanup undo/redo stacks
                while (_undoStack != null && _undoStack.Count > 0)
                {
                    _undoStack.Pop().Dispose();
                }
                while (_redoStack != null && _redoStack.Count > 0)
                {
                    _redoStack.Pop().Dispose();
                }
                foreach (IDrawableContainer container in _elements)
                {
                    container.Dispose();
                }
                if (_undrawnElement != null)
                {
                    _undrawnElement.Dispose();
                    _undrawnElement = null;
                }
                if (_cropContainer != null)
                {
                    _cropContainer.Dispose();
                    _cropContainer = null;
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Undo the last action
        /// </summary>
        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                _inUndoRedo = true;
                IMemento top = _undoStack.Pop();
                _redoStack.Push(top.Restore());
                _inUndoRedo = false;
            }
        }

        /// <summary>
        /// Undo an undo (=redo)
        /// </summary>
        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                _inUndoRedo = true;
                IMemento top = _redoStack.Pop();
                _undoStack.Push(top.Restore());
                _inUndoRedo = false;
            }
        }

        /// <summary>
        /// Returns if the surface can do a undo
        /// </summary>
        public bool CanUndo
        {
            get
            {
                return _undoStack.Count > 0;
            }
        }

        /// <summary>
        /// Returns if the surface can do a redo
        /// </summary>
        public bool CanRedo
        {
            get
            {
                return _redoStack.Count > 0;
            }
        }

        /// <summary>
        /// Make an action undo-able
        /// </summary>
        /// <param name="memento">The memento implementing the undo</param>
        /// <param name="allowMerge">Allow changes to be merged</param>
        public void MakeUndoable(IMemento memento, bool allowMerge)
        {
            if (_inUndoRedo)
            {
                throw new InvalidOperationException("Invoking do within an undo/redo action.");
            }
            if (memento != null)
            {
                bool allowPush = true;
                if (_undoStack.Count > 0 && allowMerge)
                {
                    // Check if merge is possible
                    allowPush = !_undoStack.Peek().Merge(memento);
                }
                if (allowPush)
                {
                    // Clear the redo-stack and dispose
                    while (_redoStack.Count > 0)
                    {
                        _redoStack.Pop().Dispose();
                    }
                    _undoStack.Push(memento);
                }
            }
        }

        /// <summary>
        /// This saves the elements of this surface to a stream.
        /// Is used to save a template of the complete surface
        /// </summary>
        /// <param name="streamWrite"></param>
        /// <returns></returns>
        public long SaveElementsToStream(Stream streamWrite)
        {
            long bytesWritten = 0;
            try
            {
                long lengtBefore = streamWrite.Length;
                BinaryFormatter binaryWrite = new BinaryFormatter();
                binaryWrite.Serialize(streamWrite, _elements);
                bytesWritten = streamWrite.Length - lengtBefore;
            }
            catch (Exception e)
            {
                LOG.Error("Error serializing elements to stream.", e);
            }
            return bytesWritten;
        }

        /// <summary>
        /// This loads elements from a stream, among others this is used to load a surface.
        /// </summary>
        /// <param name="streamRead"></param>
        public void LoadElementsFromStream(Stream streamRead)
        {
            try
            {
                BinaryFormatter binaryRead = new BinaryFormatter();
                DrawableContainerList loadedElements = (DrawableContainerList)binaryRead.Deserialize(streamRead);
                loadedElements.Parent = this;
                // Make sure the steplabels are sorted accoring to their number
                _stepLabels.Sort(delegate(StepLabelContainer p1, StepLabelContainer p2)
                {
                    return p1.Number.CompareTo(p2.Number);
                });
                DeselectAllElements();
                AddElements(loadedElements);
                SelectElements(loadedElements);
                FieldAggregator.BindElements(loadedElements);
            }
            catch (Exception e)
            {
                LOG.Error("Error serializing elements from stream.", e);
            }
        }

        /// <summary>
        /// This is called from the DrawingMode setter, which is not very correct...
        /// But here an element is created which is not yet draw, thus "undrawnElement".
        /// The element is than used while drawing on the surface.
        /// </summary>
        private void CreateUndrawnElement()
        {
            if (_undrawnElement != null)
            {
                FieldAggregator.UnbindElement(_undrawnElement);
            }
            switch (DrawingMode)
            {
                case DrawingModes.Rect:
                    _undrawnElement = new RectangleContainer(this);
                    break;
                case DrawingModes.Ellipse:
                    _undrawnElement = new EllipseContainer(this);
                    break;
                case DrawingModes.Text:
                    _undrawnElement = new TextContainer(this);
                    break;
                case DrawingModes.SpeechBubble:
                    _undrawnElement = new SpeechbubbleContainer(this);
                    break;
                case DrawingModes.StepLabel:
                    _undrawnElement = new StepLabelContainer(this);
                    break;
                case DrawingModes.Line:
                    _undrawnElement = new LineContainer(this);
                    break;
                case DrawingModes.Arrow:
                    _undrawnElement = new ArrowContainer(this);
                    break;
                case DrawingModes.Highlight:
                    _undrawnElement = new HighlightContainer(this);
                    break;
                case DrawingModes.Obfuscate:
                    _undrawnElement = new ObfuscateContainer(this);
                    break;
                case DrawingModes.Crop:
                    _cropContainer = new CropContainer(this);
                    _undrawnElement = _cropContainer;
                    break;
                case DrawingModes.Bitmap:
                    _undrawnElement = new ImageContainer(this);
                    break;
                case DrawingModes.Path:
                    _undrawnElement = new FreehandContainer(this);
                    break;
                case DrawingModes.None:
                    _undrawnElement = null;
                    break;
            }
            if (_undrawnElement != null)
            {
                FieldAggregator.BindElement(_undrawnElement);
            }
        }

        #region Plugin interface implementations

        public IImageContainer AddImageContainer(Image image, int x, int y)
        {
            ImageContainer bitmapContainer = new ImageContainer(this);
            bitmapContainer.Image = image;
            bitmapContainer.Left = x;
            bitmapContainer.Top = y;
            AddElement(bitmapContainer);
            return bitmapContainer;
        }

        public IImageContainer AddImageContainer(string filename, int x, int y)
        {
            ImageContainer bitmapContainer = new ImageContainer(this);
            bitmapContainer.Load(filename);
            bitmapContainer.Left = x;
            bitmapContainer.Top = y;
            AddElement(bitmapContainer);
            return bitmapContainer;
        }

        public IIconContainer AddIconContainer(Icon icon, int x, int y)
        {
            IconContainer iconContainer = new IconContainer(this);
            iconContainer.Icon = icon;
            iconContainer.Left = x;
            iconContainer.Top = y;
            AddElement(iconContainer);
            return iconContainer;
        }

        public IIconContainer AddIconContainer(string filename, int x, int y)
        {
            IconContainer iconContainer = new IconContainer(this);
            iconContainer.Load(filename);
            iconContainer.Left = x;
            iconContainer.Top = y;
            AddElement(iconContainer);
            return iconContainer;
        }

        public ICursorContainer AddCursorContainer(Cursor cursor, int x, int y)
        {
            CursorContainer cursorContainer = new CursorContainer(this);
            cursorContainer.Cursor = cursor;
            cursorContainer.Left = x;
            cursorContainer.Top = y;
            AddElement(cursorContainer);
            return cursorContainer;
        }

        public ICursorContainer AddCursorContainer(string filename, int x, int y)
        {
            CursorContainer cursorContainer = new CursorContainer(this);
            cursorContainer.Load(filename);
            cursorContainer.Left = x;
            cursorContainer.Top = y;
            AddElement(cursorContainer);
            return cursorContainer;
        }

        public ITextContainer AddTextContainer(string text, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, FontFamily family, float size, bool italic, bool bold, bool shadow, int borderSize, Color color, Color fillColor)
        {
            TextContainer textContainer = new TextContainer(this);
            textContainer.Text = text;
            textContainer.SetFieldValue(FieldType.FONT_FAMILY, family.Name);
            textContainer.SetFieldValue(FieldType.FONT_BOLD, bold);
            textContainer.SetFieldValue(FieldType.FONT_ITALIC, italic);
            textContainer.SetFieldValue(FieldType.FONT_SIZE, size);
            textContainer.SetFieldValue(FieldType.FILL_COLOR, fillColor);
            textContainer.SetFieldValue(FieldType.LINE_COLOR, color);
            textContainer.SetFieldValue(FieldType.LINE_THICKNESS, borderSize);
            textContainer.SetFieldValue(FieldType.SHADOW, shadow);
            // Make sure the Text fits
            textContainer.FitToText();
            // Align to Surface
            textContainer.AlignToParent(horizontalAlignment, verticalAlignment);

            //AggregatedProperties.UpdateElement(textContainer);
            AddElement(textContainer);
            return textContainer;
        }

        #endregion Plugin interface implementations

        #region DragDrop

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            if (LOG.IsDebugEnabled)
            {
                LOG.Debug("DragEnter got following formats: ");
                foreach (string format in ClipboardHelper.GetFormats(e.Data))
                {
                    LOG.Debug(format);
                }
            }
            if ((e.AllowedEffect & DragDropEffects.Copy) != DragDropEffects.Copy)
            {
                e.Effect = DragDropEffects.None;
            }
            else
            {
                if (ClipboardHelper.ContainsImage(e.Data) || ClipboardHelper.ContainsFormat(e.Data, "DragImageBits"))
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
        }

        /// <summary>
        /// Handle the drag/drop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDragDrop(object sender, DragEventArgs e)
        {
            Point mouse = PointToClient(new Point(e.X, e.Y));

            foreach (Image image in ClipboardHelper.GetImages(e.Data))
            {
                AddImageContainer(image, mouse.X, mouse.Y);
                mouse.Offset(10, 10);
                image.Dispose();
            }
        }

        #endregion DragDrop

        /// <summary>
        /// Auto crop the image
        /// </summary>
        /// <returns>true if cropped</returns>
        public bool AutoCrop()
        {
            Rectangle cropRectangle;
            using (Image tmpImage = GetImageForExport())
            {
                cropRectangle = ImageHelper.FindAutoCropRectangle(tmpImage, conf.AutoCropDifference);
            }
            if (!IsCropPossible(ref cropRectangle))
            {
                return false;
            }
            DeselectAllElements();
            // Maybe a bit obscure, but the following line creates a drop container
            // It's available as "undrawnElement"
            DrawingMode = DrawingModes.Crop;
            _undrawnElement.Left = cropRectangle.X;
            _undrawnElement.Top = cropRectangle.Y;
            _undrawnElement.Width = cropRectangle.Width;
            _undrawnElement.Height = cropRectangle.Height;
            _undrawnElement.Status = EditStatus.UNDRAWN;
            AddElement(_undrawnElement);
            SelectElement(_undrawnElement);
            _drawingElement = null;
            _undrawnElement = null;
            return true;
        }

        /// <summary>
        /// A simple clear
        /// </summary>
        /// <param name="newColor">The color for the background</param>
        public void Clear(Color newColor)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = ImageHelper.CreateEmptyLike(Image, Color.Empty);
            if (newBitmap != null)
            {
                // Make undoable
                MakeUndoable(new SurfaceBackgroundChangeMemento(this, null), false);
                SetImage(newBitmap, false);
                Invalidate();
            }
        }

        /// <summary>
        /// Apply a bitmap effect to the surface
        /// </summary>
        /// <param name="effect"></param>
        public void ApplyBitmapEffect(IEffect effect)
        {
            BackgroundForm backgroundForm = new BackgroundForm("Effect", "Please wait");
            backgroundForm.Show();
            Application.DoEvents();
            try
            {
                Rectangle imageRectangle = new Rectangle(Point.Empty, Image.Size);
                Matrix matrix = new Matrix();
                Image newImage = ImageHelper.ApplyEffect(Image, effect, matrix);
                if (newImage != null)
                {
                    // Make sure the elements move according to the offset the effect made the bitmap move
                    _elements.Transform(matrix);
                    // Make undoable
                    MakeUndoable(new SurfaceBackgroundChangeMemento(this, matrix), false);
                    SetImage(newImage, false);
                    Invalidate();
                    if (_surfaceSizeChanged != null && !imageRectangle.Equals(new Rectangle(Point.Empty, newImage.Size)))
                    {
                        _surfaceSizeChanged(this, null);
                    }
                }
                else
                {
                    // clean up matrix, as it hasn't been used in the undo stack.
                    matrix.Dispose();
                }
            }
            finally
            {
                // Always close the background form
                backgroundForm.CloseDialog();
            }
        }

        /// <summary>
        /// check if a crop is possible
        /// </summary>
        /// <param name="cropRectangle"></param>
        /// <returns>true if this is possible</returns>
        public bool IsCropPossible(ref Rectangle cropRectangle)
        {
            cropRectangle = GuiRectangle.GetGuiRectangle(cropRectangle.Left, cropRectangle.Top, cropRectangle.Width, cropRectangle.Height);
            if (cropRectangle.Left < 0)
            {
                cropRectangle = new Rectangle(0, cropRectangle.Top, cropRectangle.Width + cropRectangle.Left, cropRectangle.Height);
            }
            if (cropRectangle.Top < 0)
            {
                cropRectangle = new Rectangle(cropRectangle.Left, 0, cropRectangle.Width, cropRectangle.Height + cropRectangle.Top);
            }
            if (cropRectangle.Left + cropRectangle.Width > Width)
            {
                cropRectangle = new Rectangle(cropRectangle.Left, cropRectangle.Top, Width - cropRectangle.Left, cropRectangle.Height);
            }
            if (cropRectangle.Top + cropRectangle.Height > Height)
            {
                cropRectangle = new Rectangle(cropRectangle.Left, cropRectangle.Top, cropRectangle.Width, Height - cropRectangle.Top);
            }
            if (cropRectangle.Height > 0 && cropRectangle.Width > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Use to send any registered SurfaceMessageEventHandler a message, e.g. used for the notification area
        /// </summary>
        /// <param name="source">Who send</param>
        /// <param name="messageType">Type of message</param>
        /// <param name="message">Message itself</param>
        public void SendMessageEvent(object source, SurfaceMessageTyp messageType, string message)
        {
            if (_surfaceMessage != null)
            {
                SurfaceMessageEventArgs eventArgs = new SurfaceMessageEventArgs();
                eventArgs.Message = message;
                eventArgs.MessageType = messageType;
                eventArgs.Surface = this;
                _surfaceMessage(source, eventArgs);
            }
        }

        /// <summary>
        /// Crop the surface
        /// </summary>
        /// <param name="cropRectangle"></param>
        /// <returns></returns>
        public bool ApplyCrop(Rectangle cropRectangle)
        {
            if (IsCropPossible(ref cropRectangle))
            {
                Rectangle imageRectangle = new Rectangle(Point.Empty, Image.Size);
                Bitmap tmpImage;
                // Make sure we have information, this this fails
                try
                {
                    tmpImage = ImageHelper.CloneArea(Image, cropRectangle, PixelFormat.DontCare);
                }
                catch (Exception ex)
                {
                    ex.Data.Add("CropRectangle", cropRectangle);
                    ex.Data.Add("Width", Image.Width);
                    ex.Data.Add("Height", Image.Height);
                    ex.Data.Add("Pixelformat", Image.PixelFormat);
                    throw;
                }

                Matrix matrix = new Matrix();
                matrix.Translate(-cropRectangle.Left, -cropRectangle.Top, MatrixOrder.Append);
                // Make undoable
                MakeUndoable(new SurfaceBackgroundChangeMemento(this, matrix), false);

                // Do not dispose otherwise we can't undo the image!
                SetImage(tmpImage, false);
                _elements.Transform(matrix);
                if (_surfaceSizeChanged != null && !imageRectangle.Equals(new Rectangle(Point.Empty, tmpImage.Size)))
                {
                    _surfaceSizeChanged(this, null);
                }
                Invalidate();
                return true;
            }
            return false;
        }

        /// <summary>
        /// The background here is the captured image.
        /// This is called from the SurfaceBackgroundChangeMemento.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="matrix"></param>
        public void UndoBackgroundChange(Image previous, Matrix matrix)
        {
            SetImage(previous, false);
            if (matrix != null)
            {
                _elements.Transform(matrix);
            }
            if (_surfaceSizeChanged != null)
            {
                _surfaceSizeChanged(this, null);
            }
            Invalidate();
        }

        /// <summary>
        /// This event handler is called when someone presses the mouse on a surface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SurfaceMouseDown(object sender, MouseEventArgs e)
        {
            _mouseStart = e.Location;

            // check contextmenu
            if (e.Button == MouseButtons.Right)
            {
                DrawableContainerList selectedList = null;
                if (selectedElements != null && selectedElements.Count > 0)
                {
                    selectedList = selectedElements;
                }
                else
                {
                    // Single element
                    IDrawableContainer rightClickedContainer = _elements.ClickableElementAt(_mouseStart.X, _mouseStart.Y);
                    if (rightClickedContainer != null)
                    {
                        selectedList = new DrawableContainerList(ID);
                        selectedList.Add(rightClickedContainer);
                    }
                }
                if (selectedList != null && selectedList.Count > 0)
                {
                    selectedList.ShowContextMenu(e, this);
                }
                return;
            }

            _mouseDown = true;
            _isSurfaceMoveMadeUndoable = false;

            if (_cropContainer != null && ((_undrawnElement == null) || (_undrawnElement != null && DrawingMode != DrawingModes.Crop)))
            {
                RemoveElement(_cropContainer, false);
                _cropContainer = null;
                _drawingElement = null;
            }

            if (_drawingElement == null && DrawingMode != DrawingModes.None)
            {
                if (_undrawnElement == null)
                {
                    DeselectAllElements();
                    if (_undrawnElement == null)
                    {
                        CreateUndrawnElement();
                    }
                }
                _drawingElement = _undrawnElement;
                // if a new element has been drawn, set location and register it
                if (_drawingElement != null)
                {
                    _drawingElement.Status = _undrawnElement.DefaultEditMode;
                    _drawingElement.PropertyChanged += ElementPropertyChanged;
                    if (!_drawingElement.HandleMouseDown(_mouseStart.X, _mouseStart.Y))
                    {
                        _drawingElement.Left = _mouseStart.X;
                        _drawingElement.Top = _mouseStart.Y;
                    }
                    AddElement(_drawingElement);
                    _drawingElement.Selected = true;
                }
                _undrawnElement = null;
            }
            else
            {
                // check whether an existing element was clicked
                // we save mouse down element separately from selectedElements (checked on mouse up),
                // since it could be moved around before it is actually selected
                _mouseDownElement = _elements.ClickableElementAt(_mouseStart.X, _mouseStart.Y);

                if (_mouseDownElement != null)
                {
                    _mouseDownElement.Status = EditStatus.MOVING;
                }
            }
        }

        /// <summary>
        /// This event handle is called when the mouse button is unpressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SurfaceMouseUp(object sender, MouseEventArgs e)
        {
            Point currentMouse = new Point(e.X, e.Y);

            _elements.Status = EditStatus.IDLE;
            if (_mouseDownElement != null)
            {
                _mouseDownElement.Status = EditStatus.IDLE;
            }
            _mouseDown = false;
            _mouseDownElement = null;
            if (DrawingMode == DrawingModes.None)
            {
                // check whether an existing element was clicked
                IDrawableContainer element = _elements.ClickableElementAt(currentMouse.X, currentMouse.Y);
                bool shiftModifier = (ModifierKeys & Keys.Shift) == Keys.Shift;
                if (element != null)
                {
                    element.Invalidate();
                    bool alreadySelected = selectedElements.Contains(element);
                    if (shiftModifier)
                    {
                        if (alreadySelected)
                        {
                            DeselectElement(element);
                        }
                        else
                        {
                            SelectElement(element);
                        }
                    }
                    else
                    {
                        if (!alreadySelected)
                        {
                            DeselectAllElements();
                            SelectElement(element);
                        }
                    }
                }
                else if (!shiftModifier)
                {
                    DeselectAllElements();
                }
            }

            if (selectedElements.Count > 0)
            {
                selectedElements.ShowGrippers();
                selectedElements.Selected = true;
            }

            if (_drawingElement != null)
            {
                if (!_drawingElement.InitContent())
                {
                    _elements.Remove(_drawingElement);
                    _drawingElement.Invalidate();
                }
                else
                {
                    _drawingElement.HandleMouseUp(currentMouse.X, currentMouse.Y);
                    _drawingElement.Invalidate();
                    if (Math.Abs(_drawingElement.Width) < 5 && Math.Abs(_drawingElement.Height) < 5)
                    {
                        _drawingElement.Width = 25;
                        _drawingElement.Height = 25;
                    }
                    SelectElement(_drawingElement);
                    _drawingElement.Selected = true;
                }
                _drawingElement = null;
            }
        }

        /// <summary>
        /// This event handler is called when the mouse moves over the surface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SurfaceMouseMove(object sender, MouseEventArgs e)
        {
            Point currentMouse = e.Location;

            if (DrawingMode != DrawingModes.None)
            {
                Cursor = Cursors.Cross;
            }
            else
            {
                Cursor = Cursors.Default;
            }

            if (_mouseDown)
            {
                if (_mouseDownElement != null)
                { // an element is currently dragged
                    _mouseDownElement.Invalidate();
                    selectedElements.HideGrippers();
                    // Move the element
                    if (_mouseDownElement.Selected)
                    {
                        if (!_isSurfaceMoveMadeUndoable)
                        {
                            // Only allow one undoable per mouse-down/move/up "cycle"
                            _isSurfaceMoveMadeUndoable = true;
                            selectedElements.MakeBoundsChangeUndoable(false);
                        }
                        // dragged element has been selected before -> move all
                        selectedElements.MoveBy(currentMouse.X - _mouseStart.X, currentMouse.Y - _mouseStart.Y);
                    }
                    else
                    {
                        if (!_isSurfaceMoveMadeUndoable)
                        {
                            // Only allow one undoable per mouse-down/move/up "cycle"
                            _isSurfaceMoveMadeUndoable = true;
                            _mouseDownElement.MakeBoundsChangeUndoable(false);
                        }
                        // dragged element is not among selected elements -> just move dragged one
                        _mouseDownElement.MoveBy(currentMouse.X - _mouseStart.X, currentMouse.Y - _mouseStart.Y);
                    }
                    _mouseStart = currentMouse;
                    _mouseDownElement.Invalidate();
                    _modified = true;
                }
                else if (_drawingElement != null)
                {
                    _drawingElement.HandleMouseMove(currentMouse.X, currentMouse.Y);
                    _modified = true;
                }
            }
        }

        /// <summary>
        /// This event handler is called when the surface is double clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SurfaceDoubleClick(object sender, MouseEventArgs e)
        {
            selectedElements.OnDoubleClick();
            selectedElements.Invalidate();
        }

        /// <summary>
        /// Privately used to get the rendered image with all the elements on it.
        /// </summary>
        /// <param name="renderMode"></param>
        /// <returns></returns>
        private Image GetImage(RenderMode renderMode)
        {
            // Generate a copy of the original image with a dpi equal to the default...
            Bitmap clone = ImageHelper.Clone(_image, PixelFormat.DontCare);
            // otherwise we would have a problem drawing the image to the surface... :(
            using (Graphics graphics = Graphics.FromImage(clone))
            {
                // Do not set the following, the containers need to decide themselves
                //graphics.SmoothingMode = SmoothingMode.HighQuality;
                //graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                //graphics.CompositingQuality = CompositingQuality.HighQuality;
                //graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                _elements.Draw(graphics, clone, renderMode, new Rectangle(Point.Empty, clone.Size));
            }
            return clone;
        }

        /// <summary>
        /// This returns the image "result" of this surface, with all the elements rendered on it.
        /// </summary>
        /// <returns></returns>
        public Image GetImageForExport()
        {
            return GetImage(RenderMode.EXPORT);
        }

        /// <summary>
        /// This is the event handler for the Paint Event, try to draw as little as possible!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SurfacePaint(object sender, PaintEventArgs e)
        {
            Graphics targetGraphics = e.Graphics;
            Rectangle clipRectangle = e.ClipRectangle;
            if (Rectangle.Empty.Equals(clipRectangle))
            {
                LOG.Debug("Empty cliprectangle??");
                return;
            }

            if (_elements.HasIntersectingFilters(clipRectangle))
            {
                if (_buffer != null)
                {
                    if (_buffer.Width != Image.Width || _buffer.Height != Image.Height || _buffer.PixelFormat != Image.PixelFormat)
                    {
                        _buffer.Dispose();
                        _buffer = null;
                    }
                }
                if (_buffer == null)
                {
                    _buffer = ImageHelper.CreateEmpty(Image.Width, Image.Height, Image.PixelFormat, Color.Empty, Image.HorizontalResolution, Image.VerticalResolution);
                    LOG.DebugFormat("Created buffer with size: {0}x{1}", Image.Width, Image.Height);
                }
                // Elements might need the bitmap, so we copy the part we need
                using (Graphics graphics = Graphics.FromImage(_buffer))
                {
                    // do not set the following, the containers need to decide this themselves!
                    //graphics.SmoothingMode = SmoothingMode.HighQuality;
                    //graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    //graphics.CompositingQuality = CompositingQuality.HighQuality;
                    //graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    DrawBackground(graphics, clipRectangle);
                    graphics.DrawImage(Image, clipRectangle, clipRectangle, GraphicsUnit.Pixel);
                    graphics.SetClip(targetGraphics);
                    _elements.Draw(graphics, _buffer, RenderMode.EDIT, clipRectangle);
                }
                targetGraphics.DrawImage(_buffer, clipRectangle, clipRectangle, GraphicsUnit.Pixel);
            }
            else
            {
                DrawBackground(targetGraphics, clipRectangle);
                targetGraphics.DrawImage(Image, clipRectangle, clipRectangle, GraphicsUnit.Pixel);
                _elements.Draw(targetGraphics, null, RenderMode.EDIT, clipRectangle);
            }
        }

        private void DrawBackground(Graphics targetGraphics, Rectangle clipRectangle)
        {
            // check if we need to draw the checkerboard
            if (Image.IsAlphaPixelFormat(Image.PixelFormat) && _transparencyBackgroundBrush != null)
            {
                targetGraphics.FillRectangle(_transparencyBackgroundBrush, clipRectangle);
            }
            else
            {
                targetGraphics.Clear(BackColor);
            }
        }

        /// <summary>
        /// Draw a checkboard when capturing with transparency
        /// </summary>
        /// <param name="e">PaintEventArgs</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        /// <summary>
        /// Wrapper for makeUndoable flag which was introduced later, will call AddElement with makeundoable set to true
        /// </summary>
        /// <param name="element">the new element</param>
        public void AddElement(IDrawableContainer element)
        {
            AddElement(element, true);
        }

        /// <summary>
        /// Add a new element to the surface
        /// </summary>
        /// <param name="element">the new element</param>
        /// <param name="makeUndoable">true if the adding should be undoable</param>
        public void AddElement(IDrawableContainer element, bool makeUndoable)
        {
            _elements.Add(element);
            DrawableContainer container = element as DrawableContainer;
            if (container != null)
            {
                container.FieldChanged += element_FieldChanged;
            }
            element.PropertyChanged += ElementPropertyChanged;
            if (element.Status == EditStatus.UNDRAWN)
            {
                element.Status = EditStatus.IDLE;
            }
            element.Invalidate();
            if (makeUndoable)
            {
                MakeUndoable(new AddElementMemento(this, element), false);
            }
            _modified = true;
        }

        /// <summary>
        /// Remove an element of the elements list
        /// </summary>
        /// <param name="elementToRemove">Element to remove</param>
        /// <param name="makeUndoable">flag specifying if the remove needs to be undoable</param>
        public void RemoveElement(IDrawableContainer elementToRemove, bool makeUndoable)
        {
            DeselectElement(elementToRemove);
            _elements.Remove(elementToRemove);
            DrawableContainer element = elementToRemove as DrawableContainer;
            if (element != null)
            {
                element.FieldChanged -= element_FieldChanged;
            }
            elementToRemove.PropertyChanged -= ElementPropertyChanged;
            // Do not dispose, the memento should!! element.Dispose();
            Invalidate();
            if (makeUndoable)
            {
                MakeUndoable(new DeleteElementMemento(this, elementToRemove), false);
            }
            _modified = true;
        }

        /// <summary>
        /// Add the supplied elements to the surface
        /// </summary>
        /// <param name="elementsToAdd"></param>
        public void AddElements(DrawableContainerList elementsToAdd)
        {
            foreach (IDrawableContainer element in elementsToAdd)
            {
                AddElement(element, true);
            }
        }

        /// <summary>
        /// Returns if this surface has selected elements
        /// </summary>
        /// <returns></returns>
        public bool HasSelectedElements
        {
            get
            {
                return (selectedElements != null && selectedElements.Count > 0);
            }
        }

        /// <summary>
        /// Remove all the selected elements
        /// </summary>
        public void RemoveSelectedElements()
        {
            if (HasSelectedElements)
            {
                // As RemoveElement will remove the element from the selectedElements list we need to copy the element
                // to another list.
                List<DrawableContainer> elementsToRemove = new List<DrawableContainer>();
                foreach (DrawableContainer element in selectedElements)
                {
                    // Collect to remove later
                    elementsToRemove.Add(element);
                }
                // Remove now
                foreach (DrawableContainer element in elementsToRemove)
                {
                    RemoveElement(element, true);
                }
                selectedElements.Clear();
                if (_movingElementChanged != null)
                {
                    SurfaceElementEventArgs eventArgs = new SurfaceElementEventArgs();
                    eventArgs.Elements = selectedElements;
                    _movingElementChanged(this, eventArgs);
                }
            }
        }

        /// <summary>
        /// Cut the selected elements from the surface to the clipboard
        /// </summary>
        public void CutSelectedElements()
        {
            if (HasSelectedElements)
            {
                ClipboardHelper.SetClipboardData(typeof(DrawableContainerList), selectedElements);
                RemoveSelectedElements();
            }
        }

        /// <summary>
        /// Copy the selected elements to the clipboard
        /// </summary>
        public void CopySelectedElements()
        {
            if (HasSelectedElements)
            {
                ClipboardHelper.SetClipboardData(typeof(DrawableContainerList), selectedElements);
            }
        }

        /// <summary>
        /// This method is called to confirm/cancel "confirmable" elements, like the crop-container.
        /// Called when pressing enter or using the "check" in the editor.
        /// </summary>
        /// <param name="confirm"></param>
        public void ConfirmSelectedConfirmableElements(bool confirm)
        {
            // create new collection so that we can iterate safely (selectedElements might change due with confirm/cancel)
            List<IDrawableContainer> selectedDCs = new List<IDrawableContainer>(selectedElements);
            foreach (IDrawableContainer dc in selectedDCs)
            {
                if (dc.Equals(_cropContainer))
                {
                    DrawingMode = DrawingModes.None;
                    // No undo memento for the cropcontainer itself, only for the effect
                    RemoveElement(_cropContainer, false);
                    if (confirm)
                    {
                        ApplyCrop(_cropContainer.Bounds);
                    }
                    _cropContainer.Dispose();
                    _cropContainer = null;
                }
            }
        }

        /// <summary>
        /// Paste all the elements that are on the clipboard
        /// </summary>
        public void PasteElementFromClipboard()
        {
            IDataObject clipboard = ClipboardHelper.GetDataObject();

            List<string> formats = ClipboardHelper.GetFormats(clipboard);
            if (formats == null || formats.Count == 0)
            {
                return;
            }
            if (LOG.IsDebugEnabled)
            {
                LOG.Debug("List of clipboard formats available for pasting:");
                foreach (string format in formats)
                {
                    LOG.Debug("\tgot format: " + format);
                }
            }

            if (formats.Contains(typeof(DrawableContainerList).FullName))
            {
                DrawableContainerList dcs = (DrawableContainerList)ClipboardHelper.GetFromDataObject(clipboard, typeof(DrawableContainerList));
                if (dcs != null)
                {
                    // Make element(s) only move 10,10 if the surface is the same
                    Point moveOffset;
                    bool isSameSurface = (dcs.ParentID == _uniqueId);
                    dcs.Parent = this;
                    if (isSameSurface)
                    {
                        moveOffset = new Point(10, 10);
                    }
                    else
                    {
                        moveOffset = Point.Empty;
                    }
                    // Here a fix for bug #1475, first calculate the bounds of the complete DrawableContainerList
                    Rectangle drawableContainerListBounds = Rectangle.Empty;
                    foreach (IDrawableContainer element in dcs)
                    {
                        if (drawableContainerListBounds == Rectangle.Empty)
                        {
                            drawableContainerListBounds = element.DrawingBounds;
                        }
                        else
                        {
                            drawableContainerListBounds = Rectangle.Union(drawableContainerListBounds, element.DrawingBounds);
                        }
                    }
                    // And find a location inside the target surface to paste to
                    bool containersCanFit = drawableContainerListBounds.Width < Bounds.Width && drawableContainerListBounds.Height < Bounds.Height;
                    if (!containersCanFit)
                    {
                        Point containersLocation = drawableContainerListBounds.Location;
                        containersLocation.Offset(moveOffset);
                        if (!Bounds.Contains(containersLocation))
                        {
                            // Easy fix for same surface
                            if (isSameSurface)
                            {
                                moveOffset = new Point(-10, -10);
                            }
                            else
                            {
                                // For different surface, which is most likely smaller, we move to "10,10"
                                moveOffset = new Point(-drawableContainerListBounds.Location.X + 10, -drawableContainerListBounds.Location.Y + 10);
                            }
                        }
                    }
                    else
                    {
                        Rectangle moveContainerListBounds = drawableContainerListBounds;
                        moveContainerListBounds.Offset(moveOffset);
                        // check if the element is inside
                        if (!Bounds.Contains(moveContainerListBounds))
                        {
                            // Easy fix for same surface
                            if (isSameSurface)
                            {
                                moveOffset = new Point(-10, -10);
                            }
                            else
                            {
                                // For different surface, which is most likely smaller
                                int offsetX = 0;
                                int offsetY = 0;
                                if (drawableContainerListBounds.Right > Bounds.Right)
                                {
                                    offsetX = Bounds.Right - drawableContainerListBounds.Right;
                                    // Correction for the correction
                                    if (drawableContainerListBounds.Left + offsetX < 0)
                                    {
                                        offsetX += Math.Abs(drawableContainerListBounds.Left + offsetX);
                                    }
                                }
                                if (drawableContainerListBounds.Bottom > Bounds.Bottom)
                                {
                                    offsetY = Bounds.Bottom - drawableContainerListBounds.Bottom;
                                    // Correction for the correction
                                    if (drawableContainerListBounds.Top + offsetY < 0)
                                    {
                                        offsetY += Math.Abs(drawableContainerListBounds.Top + offsetY);
                                    }
                                }
                                moveOffset = new Point(offsetX, offsetY);
                            }
                        }
                    }
                    dcs.MoveBy(moveOffset.X, moveOffset.Y);
                    AddElements(dcs);
                    FieldAggregator.BindElements(dcs);
                    DeselectAllElements();
                    SelectElements(dcs);
                }
            }
            else if (ClipboardHelper.ContainsImage(clipboard))
            {
                int x = 10;
                int y = 10;
                foreach (Image clipboardImage in ClipboardHelper.GetImages(clipboard))
                {
                    if (clipboardImage != null)
                    {
                        DeselectAllElements();
                        IImageContainer container = AddImageContainer(clipboardImage as Bitmap, x, y);
                        SelectElement(container);
                        clipboardImage.Dispose();
                        x += 10;
                        y += 10;
                    }
                }
            }
            else if (ClipboardHelper.ContainsText(clipboard))
            {
                string text = ClipboardHelper.GetText(clipboard);
                if (text != null)
                {
                    DeselectAllElements();
                    ITextContainer textContainer = AddTextContainer(text, HorizontalAlignment.Center, VerticalAlignment.CENTER,
                        FontFamily.GenericSansSerif, 12f, false, false, false, 2, Color.Black, Color.Transparent);
                    SelectElement(textContainer);
                }
            }
        }

        /// <summary>
        /// Duplicate all the selecteded elements
        /// </summary>
        public void DuplicateSelectedElements()
        {
            LOG.DebugFormat("Duplicating {0} selected elements", selectedElements.Count);
            DrawableContainerList dcs = selectedElements.Clone();
            dcs.Parent = this;
            dcs.MoveBy(10, 10);
            AddElements(dcs);
            DeselectAllElements();
            SelectElements(dcs);
        }

        /// <summary>
        /// Deselect the specified element
        /// </summary>
        /// <param name="container"></param>
        public void DeselectElement(IDrawableContainer container)
        {
            container.HideGrippers();
            container.Selected = false;
            selectedElements.Remove(container);
            FieldAggregator.UnbindElement(container);
            if (_movingElementChanged != null)
            {
                SurfaceElementEventArgs eventArgs = new SurfaceElementEventArgs();
                eventArgs.Elements = selectedElements;
                _movingElementChanged(this, eventArgs);
            }
        }

        /// <summary>
        /// Deselect all the selected elements
        /// </summary>
        public void DeselectAllElements()
        {
            if (HasSelectedElements)
            {
                while (selectedElements.Count > 0)
                {
                    IDrawableContainer element = selectedElements[0];
                    element.Invalidate();
                    element.HideGrippers();
                    element.Selected = false;
                    selectedElements.Remove(element);
                    FieldAggregator.UnbindElement(element);
                }
                if (_movingElementChanged != null)
                {
                    SurfaceElementEventArgs eventArgs = new SurfaceElementEventArgs();
                    eventArgs.Elements = selectedElements;
                    _movingElementChanged(this, eventArgs);
                }
            }
        }

        /// <summary>
        /// Select the supplied element
        /// </summary>
        /// <param name="container"></param>
        public void SelectElement(IDrawableContainer container)
        {
            if (!selectedElements.Contains(container))
            {
                selectedElements.Add(container);
                container.ShowGrippers();
                container.Selected = true;
                FieldAggregator.BindElement(container);
                if (_movingElementChanged != null)
                {
                    SurfaceElementEventArgs eventArgs = new SurfaceElementEventArgs();
                    eventArgs.Elements = selectedElements;
                    _movingElementChanged(this, eventArgs);
                }
                container.Invalidate();
            }
        }

        /// <summary>
        /// Select all elements, this is called when Ctrl+A is pressed
        /// </summary>
        public void SelectAllElements()
        {
            SelectElements(_elements);
        }

        /// <summary>
        /// Select the supplied elements
        /// </summary>
        /// <param name="elements"></param>
        public void SelectElements(DrawableContainerList elements)
        {
            foreach (DrawableContainer element in elements)
            {
                SelectElement(element);
            }
        }

        /// <summary>
        /// Process key presses on the surface, this is called from the editor (and NOT an override from the Control)
        /// </summary>
        /// <param name="k">Keys</param>
        /// <returns>false if no keys were processed</returns>
        public bool ProcessCmdKey(Keys k)
        {
            if (selectedElements.Count > 0)
            {
                bool shiftModifier = (ModifierKeys & Keys.Shift) == Keys.Shift;
                int px = shiftModifier ? 10 : 1;
                Point moveBy = Point.Empty;

                switch (k)
                {
                    case Keys.Left:
                    case Keys.Left | Keys.Shift:
                        moveBy = new Point(-px, 0);
                        break;
                    case Keys.Up:
                    case Keys.Up | Keys.Shift:
                        moveBy = new Point(0, -px);
                        break;
                    case Keys.Right:
                    case Keys.Right | Keys.Shift:
                        moveBy = new Point(px, 0);
                        break;
                    case Keys.Down:
                    case Keys.Down | Keys.Shift:
                        moveBy = new Point(0, px);
                        break;
                    case Keys.PageUp:
                        PullElementsUp();
                        break;
                    case Keys.PageDown:
                        PushElementsDown();
                        break;
                    case Keys.Home:
                        PullElementsToTop();
                        break;
                    case Keys.End:
                        PushElementsToBottom();
                        break;
                    case Keys.Enter:
                        ConfirmSelectedConfirmableElements(true);
                        break;
                    case Keys.Escape:
                        ConfirmSelectedConfirmableElements(false);
                        break;
                    /*case Keys.Delete:
                        RemoveSelectedElements();
                        break;*/
                    default:
                        return false;
                }
                if (!Point.Empty.Equals(moveBy))
                {
                    selectedElements.MakeBoundsChangeUndoable(true);
                    selectedElements.MoveBy(moveBy.X, moveBy.Y);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Property for accessing the elements on the surface
        /// </summary>
        public DrawableContainerList Elements
        {
            get
            {
                return _elements;
            }
        }

        /// <summary>
        /// pulls selected elements up one level in hierarchy
        /// </summary>
        public void PullElementsUp()
        {
            _elements.PullElementsUp(selectedElements);
            _elements.Invalidate();
        }

        /// <summary>
        /// pushes selected elements up to top in hierarchy
        /// </summary>
        public void PullElementsToTop()
        {
            _elements.PullElementsToTop(selectedElements);
            _elements.Invalidate();
        }

        /// <summary>
        /// pushes selected elements down one level in hierarchy
        /// </summary>
        public void PushElementsDown()
        {
            _elements.PushElementsDown(selectedElements);
            _elements.Invalidate();
        }

        /// <summary>
        /// pushes selected elements down to bottom in hierarchy
        /// </summary>
        public void PushElementsToBottom()
        {
            _elements.PushElementsToBottom(selectedElements);
            _elements.Invalidate();
        }

        /// <summary>
        /// indicates whether the selected elements could be pulled up in hierarchy
        /// </summary>
        /// <returns>true if selected elements could be pulled up, false otherwise</returns>
        public bool CanPullSelectionUp()
        {
            return _elements.CanPullUp(selectedElements);
        }

        /// <summary>
        /// indicates whether the selected elements could be pushed down in hierarchy
        /// </summary>
        /// <returns>true if selected elements could be pushed down, false otherwise</returns>
        public bool CanPushSelectionDown()
        {
            return _elements.CanPushDown(selectedElements);
        }

        public void ElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Invalidate();
        }

        public void element_FieldChanged(object sender, FieldChangedEventArgs e)
        {
            selectedElements.HandleFieldChangedEvent(sender, e);
        }

        public bool IsOnSurface(IDrawableContainer container)
        {
            return _elements.Contains(container);
        }
    }
}