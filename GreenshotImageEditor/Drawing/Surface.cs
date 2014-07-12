/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2013  Thomas Braun, Jens Klingen, Robin Krom
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

        /// <summary>
        /// Event handlers (do not serialize!)
        /// </summary>
        [NonSerialized]
        private SurfaceElementEventHandler movingElementChanged;
        public event SurfaceElementEventHandler MovingElementChanged
        {
            add
            {
                movingElementChanged += value;
            }
            remove
            {
                movingElementChanged -= value;
            }
        }
        [NonSerialized]
        private SurfaceDrawingModeEventHandler drawingModeChanged;
        public event SurfaceDrawingModeEventHandler DrawingModeChanged
        {
            add
            {
                drawingModeChanged += value;
            }
            remove
            {
                drawingModeChanged -= value;
            }
        }
        [NonSerialized]
        private SurfaceSizeChangeEventHandler surfaceSizeChanged;
        public event SurfaceSizeChangeEventHandler SurfaceSizeChanged
        {
            add
            {
                surfaceSizeChanged += value;
            }
            remove
            {
                surfaceSizeChanged -= value;
            }
        }
        [NonSerialized]
        private SurfaceMessageEventHandler surfaceMessage;
        public event SurfaceMessageEventHandler SurfaceMessage
        {
            add
            {
                surfaceMessage += value;
            }
            remove
            {
                surfaceMessage -= value;
            }
        }

        /// <summary>
        /// inUndoRedo makes sure we don't undo/redo while in a undo/redo action
        /// </summary>
        [NonSerialized]
        private bool inUndoRedo = false;

        /// <summary>
        /// Make only one surfacemove cycle undoable, see SurfaceMouseMove
        /// </summary>
        [NonSerialized]
        private bool isSurfaceMoveMadeUndoable = false;

        /// <summary>
        /// Undo/Redo stacks, should not be serialized as the file would be way to big
        /// </summary>
        [NonSerialized]
        private Stack<IMemento> undoStack = new Stack<IMemento>();
        [NonSerialized]
        private Stack<IMemento> redoStack = new Stack<IMemento>();

        /// <summary>
        /// Last save location, do not serialize!
        /// </summary>
        [NonSerialized]
        private string lastSaveFullPath = null;

        /// <summary>
        /// current drawing mode, do not serialize!
        /// </summary>
        [NonSerialized]
        private DrawingModes drawingMode = DrawingModes.None;

        /// <summary>
        /// the keyslocked flag helps with focus issues
        /// </summary>
        [NonSerialized]
        private bool keysLocked = false;

        /// <summary>
        /// Location of the mouse-down (it "starts" here), do not serialize
        /// </summary>
        [NonSerialized]
        private Point mouseStart = Point.Empty;

        /// <summary>
        /// are we in a mouse down, do not serialize
        /// </summary>
        [NonSerialized]
        private bool mouseDown = false;

        /// <summary>
        /// are we dragging, do not serialize
        /// </summary>
        [NonSerialized]
        private bool draggingInProgress = false;

        /// <summary>
        /// The selected element for the mouse down, do not serialize
        /// </summary>
        [NonSerialized]
        private IDrawableContainer mouseDownElement = null;

        /// <summary>
        /// all selected elements, do not serialize
        /// </summary>
        [NonSerialized]
        private DrawableContainerList selectedElements = new DrawableContainerList();

        /// <summary>
        /// the element we are drawing with, do not serialize
        /// </summary>
        [NonSerialized]
        private IDrawableContainer drawingElement = null;

        /// <summary>
        /// the element we want to draw with (not yet drawn), do not serialize
        /// </summary>
        [NonSerialized]
        private IDrawableContainer undrawnElement = null;

        /// <summary>
        /// the cropcontainer, when cropping this is set, do not serialize
        /// </summary>
        [NonSerialized]
        private IDrawableContainer cropContainer = null;

        /// <summary>
        /// the brush which is used for transparent backgrounds, set by the editor, do not serialize
        /// </summary>
        [NonSerialized]
        private Brush transparencyBackgroundBrush;

        /// <summary>
        /// The buffer is only for drawing on it when using filters (to supply access)
        /// This saves a lot of "create new bitmap" commands
        /// Should not be serialized, as it's generated.
        /// The actual bitmap is in the paintbox...
        /// </summary>
        [NonSerialized]
        private Bitmap buffer = null;

        /// <summary>
        /// all elements on the surface, needed with serialization
        /// </summary>
        private DrawableContainerList elements = new DrawableContainerList();

        /// <summary>
        /// all elements on the surface, needed with serialization
        /// </summary>
        private FieldAggregator fieldAggregator = new FieldAggregator();

        /// <summary>
        /// the cursor container, needed with serialization as we need a direct acces to it.
        /// </summary>
        private IDrawableContainer cursorContainer = null;

        /// <summary>
        /// the capture details, needed with serialization
        /// </summary>
        private ICaptureDetails captureDetails = null;

        /// <summary>
        /// the modified flag specifies if the surface has had modifications after the last export.
        /// Initial state is modified, as "it's not saved"
        /// After serialization this should actually be "false" (the surface came from a stream)
        /// For now we just serialize it...
        /// </summary>
        private bool modified = true;

        /// <summary>
        /// The image is the actual captured image, needed with serialization
        /// </summary>
        private Image image = null;
        public Image Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                Size = image.Size;
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
                return fieldAggregator;
            }
            set
            {
                fieldAggregator = value;
            }
        }

        /// <summary>
        /// The cursor container has it's own accessor so we can find and remove this (when needed)
        /// </summary>
        public IDrawableContainer CursorContainer
        {
            get
            {
                return cursorContainer;
            }
        }

        /// <summary>
        /// A simple getter to ask if this surface has a cursor
        /// </summary>
        public bool HasCursor
        {
            get
            {
                return cursorContainer != null;
            }
        }

        /// <summary>
        /// A simple helper method to remove the cursor from the surface
        /// </summary>
        public void RemoveCursor()
        {
            RemoveElement(cursorContainer, true);
            cursorContainer = null;
        }

        /// <summary>
        /// The brush which is used to draw the transparent background
        /// </summary>
        public Brush TransparencyBackgroundBrush
        {
            get
            {
                return transparencyBackgroundBrush;
            }
            set
            {
                transparencyBackgroundBrush = value;
            }
        }

        /// <summary>
        /// Are the keys on this surface locked?
        /// </summary>
        public bool KeysLocked
        {
            get
            {
                return keysLocked;
            }
            set
            {
                keysLocked = value;
            }
        }

        /// <summary>
        /// Is this surface modified? This is only true if the surface has not been exported.
        /// </summary>
        public bool Modified
        {
            get
            {
                return modified;
            }
            set
            {
                modified = value;
            }
        }

        /// <summary>
        /// The DrawingMode property specifies the mode for drawing, more or less the element type.
        /// </summary>
        public DrawingModes DrawingMode
        {
            get
            {
                return drawingMode;
            }
            set
            {
                drawingMode = value;
                if (drawingModeChanged != null)
                {
                    SurfaceDrawingModeEventArgs eventArgs = new SurfaceDrawingModeEventArgs();
                    eventArgs.DrawingMode = drawingMode;
                    drawingModeChanged.Invoke(this, eventArgs);
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
                return lastSaveFullPath;
            }
            set
            {
                lastSaveFullPath = value;
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
                return captureDetails;
            }
            set
            {
                captureDetails = value;
            }
        }

        /// <summary>
        /// Base Surface constructor
        /// </summary>
        public Surface()
            : base()
        {
            Count++;
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
            elements.Parent = this;
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
        /// <param name="image">The new image</param>
        /// <param name="dispose">true if the old image needs to be disposed, when using undo this should not be true!!</param>
        private void SetImage(Image newImage, bool dispose)
        {
            // Dispose
            if (image != null && dispose)
            {
                image.Dispose();
            }

            // Set new values
            Image = newImage;
            Size = newImage.Size;

            modified = true;
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
                    cursorContainer = AddIconContainer(capture.Cursor, capture.CursorLocation.X, capture.CursorLocation.Y);
                    SelectElement(cursorContainer);
                }
            }
            // Make sure the image is NOT disposed, we took the reference directly into ourselves
            ((Capture)capture).NullImage();

            captureDetails = capture.CaptureDetails;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Count--;
                LOG.Debug("Disposing surface!");
                if (buffer != null)
                {
                    buffer.Dispose();
                    buffer = null;
                }
                if (transparencyBackgroundBrush != null)
                {
                    transparencyBackgroundBrush.Dispose();
                    transparencyBackgroundBrush = null;
                }

                // Cleanup undo/redo stacks
                while (undoStack != null && undoStack.Count > 0)
                {
                    undoStack.Pop().Dispose();
                }
                while (redoStack != null && redoStack.Count > 0)
                {
                    redoStack.Pop().Dispose();
                }
                foreach (IDrawableContainer container in elements)
                {
                    container.Dispose();
                }
                if (undrawnElement != null)
                {
                    undrawnElement.Dispose();
                    undrawnElement = null;
                }
                if (cropContainer != null)
                {
                    cropContainer.Dispose();
                    cropContainer = null;
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Undo the last action
        /// </summary>
        public void Undo()
        {
            if (undoStack.Count > 0)
            {
                inUndoRedo = true;
                IMemento top = undoStack.Pop();
                redoStack.Push(top.Restore());
                inUndoRedo = false;
            }
        }

        /// <summary>
        /// Undo an undo (=redo)
        /// </summary>
        public void Redo()
        {
            if (redoStack.Count > 0)
            {
                inUndoRedo = true;
                IMemento top = redoStack.Pop();
                undoStack.Push(top.Restore());
                inUndoRedo = false;
            }
        }

        /// <summary>
        /// Returns if the surface can do a undo
        /// </summary>
        public bool CanUndo
        {
            get
            {
                return undoStack.Count > 0;
            }
        }

        /// <summary>
        /// Returns if the surface can do a redo
        /// </summary>
        public bool CanRedo
        {
            get
            {
                return redoStack.Count > 0;
            }
        }

        /// <summary>
        /// Get the language key for the undo action
        /// </summary>
        public LangKey UndoActionLanguageKey
        {
            get
            {
                if (CanUndo)
                {
                    return undoStack.Peek().ActionLanguageKey;
                }
                else
                {
                    return LangKey.none;
                }
            }
        }

        /// <summary>
        /// Get the language key for redo action
        /// </summary>
        public LangKey RedoActionLanguageKey
        {
            get
            {
                if (CanRedo)
                {
                    return redoStack.Peek().ActionLanguageKey;
                }
                else
                {
                    return LangKey.none;
                }
            }
        }

        /// <summary>
        /// Make an action undo-able
        /// </summary>
        /// <param name="memento">The memento implementing the undo</param>
        public void MakeUndoable(IMemento memento, bool allowMerge)
        {
            if (inUndoRedo)
            {
                throw new InvalidOperationException("Invoking do within an undo/redo action.");
            }
            if (memento != null)
            {
                bool allowPush = true;
                if (undoStack.Count > 0 && allowMerge)
                {
                    // Check if merge is possible
                    allowPush = !undoStack.Peek().Merge(memento);
                }
                if (allowPush)
                {
                    // Clear the redo-stack and dispose
                    while (redoStack.Count > 0)
                    {
                        redoStack.Pop().Dispose();
                    }
                    undoStack.Push(memento);
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
                binaryWrite.Serialize(streamWrite, elements);
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
                if (loadedElements != null)
                {
                    loadedElements.Parent = this;
                    DeselectAllElements();
                    AddElements(loadedElements);
                    SelectElements(loadedElements);
                    FieldAggregator.BindElements(loadedElements);
                }
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
            if (undrawnElement != null)
            {
                FieldAggregator.UnbindElement(undrawnElement);
            }
            switch (DrawingMode)
            {
                case DrawingModes.Rect:
                    undrawnElement = new RectangleContainer(this);
                    break;
                case DrawingModes.Ellipse:
                    undrawnElement = new EllipseContainer(this);
                    break;
                case DrawingModes.Text:
                    undrawnElement = new TextContainer(this);
                    break;
                case DrawingModes.Line:
                    undrawnElement = new LineContainer(this);
                    break;
                case DrawingModes.Arrow:
                    undrawnElement = new ArrowContainer(this);
                    break;
                case DrawingModes.Highlight:
                    undrawnElement = new HighlightContainer(this);
                    break;
                case DrawingModes.Obfuscate:
                    undrawnElement = new ObfuscateContainer(this);
                    break;
                case DrawingModes.Crop:
                    cropContainer = new CropContainer(this);
                    undrawnElement = cropContainer;
                    break;
                case DrawingModes.Bitmap:
                    undrawnElement = new ImageContainer(this);
                    break;
                case DrawingModes.Path:
                    undrawnElement = new FreehandContainer(this);
                    break;
                case DrawingModes.None:
                    undrawnElement = null;
                    break;
            }
            if (undrawnElement != null)
            {
                FieldAggregator.BindElement(undrawnElement);
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
            if (draggingInProgress || (e.AllowedEffect & DragDropEffects.Copy) != DragDropEffects.Copy)
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
            List<string> filenames = ClipboardHelper.GetImageFilenames(e.Data);
            Point mouse = PointToClient(new Point(e.X, e.Y));

            foreach (Image image in ClipboardHelper.GetImages(e.Data))
            {
                AddImageContainer(image, mouse.X, mouse.Y);
                mouse.Offset(10, 10);
                image.Dispose();
            }
        }

        //		private void QueryContinueDragDrop(object sender, QueryContinueDragEventArgs e) {
        //			LOG.Debug("QueryContinueDrag: " + e.Action);
        //			if (e.EscapePressed) {
        //				e.Action = DragAction.Cancel;
        //			}
        //		}
        //
        //		private void GiveFeedbackDragDrop(object sender, GiveFeedbackEventArgs e) {
        //			e.UseDefaultCursors = true;
        //		}

        #endregion DragDrop

        /// <summary>
        /// Auto crop the image
        /// </summary>
        /// <returns>true if cropped</returns>
        public bool AutoCrop()
        {
            Rectangle cropRectangle = ImageHelper.FindAutoCropRectangle(Image, conf.AutoCropDifference);
            if (isCropPossible(ref cropRectangle))
            {
                DeselectAllElements();
                // Maybe a bit obscure, but the following line creates a drop container
                // It's available as "undrawnElement"
                DrawingMode = DrawingModes.Crop;
                undrawnElement.Left = cropRectangle.X;
                undrawnElement.Top = cropRectangle.Y;
                undrawnElement.Width = cropRectangle.Width;
                undrawnElement.Height = cropRectangle.Height;
                undrawnElement.Status = EditStatus.UNDRAWN;
                AddElement(undrawnElement);
                SelectElement(undrawnElement);
                drawingElement = null;
                undrawnElement = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// A simple clear
        /// </summary>
        /// <param name="newColor">The color for the background</param>
        public void Clear(Color newColor)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = ImageHelper.CreateEmptyLike((Bitmap)Image, Color.Empty);
            if (newBitmap != null)
            {
                // Make undoable
                MakeUndoable(new SurfaceBackgroundChangeMemento(this, Point.Empty), false);
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
                Point offset;
                Image newImage = ImageHelper.ApplyEffect(Image, effect, out offset);
                if (newImage != null)
                {
                    // Make sure the elements move according to the offset the effect made the bitmap move
                    elements.MoveBy(offset.X, offset.Y);
                    // Make undoable
                    MakeUndoable(new SurfaceBackgroundChangeMemento(this, offset), false);
                    SetImage(newImage, false);
                    Invalidate();
                    if (surfaceSizeChanged != null && !imageRectangle.Equals(new Rectangle(Point.Empty, newImage.Size)))
                    {
                        surfaceSizeChanged(this, null);
                    }
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
        public bool isCropPossible(ref Rectangle cropRectangle)
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
            if (surfaceMessage != null)
            {
                SurfaceMessageEventArgs eventArgs = new SurfaceMessageEventArgs();
                eventArgs.Message = message;
                eventArgs.MessageType = messageType;
                eventArgs.Surface = this;
                surfaceMessage(source, eventArgs);
            }
        }

        /// <summary>
        /// Crop the surface
        /// </summary>
        /// <param name="cropRectangle"></param>
        /// <returns></returns>
        public bool ApplyCrop(Rectangle cropRectangle)
        {
            if (isCropPossible(ref cropRectangle))
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

                Point offset = new Point(-cropRectangle.Left, -cropRectangle.Top);
                // Make undoable
                MakeUndoable(new SurfaceBackgroundChangeMemento(this, offset), false);

                // Do not dispose otherwise we can't undo the image!
                SetImage(tmpImage, false);
                elements.MoveBy(offset.X, offset.Y);
                if (surfaceSizeChanged != null && !imageRectangle.Equals(new Rectangle(Point.Empty, tmpImage.Size)))
                {
                    surfaceSizeChanged(this, null);
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
        /// <param name="offset"></param>
        public void UndoBackgroundChange(Image previous, Point offset)
        {
            SetImage(previous, false);
            elements.MoveBy(offset.X, offset.Y);
            if (surfaceSizeChanged != null)
            {
                surfaceSizeChanged(this, null);
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
            mouseStart = e.Location;

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
                    IDrawableContainer rightClickedContainer = elements.ClickableElementAt(mouseStart.X, mouseStart.Y);
                    if (rightClickedContainer != null)
                    {
                        selectedList = new DrawableContainerList();
                        selectedList.Add(rightClickedContainer);
                    }
                }
                if (selectedList != null && selectedList.Count > 0)
                {
                    selectedList.ShowContextMenu(e, this);
                }
                return;
            }

            mouseDown = true;
            isSurfaceMoveMadeUndoable = false;

            if (cropContainer != null && ((undrawnElement == null) || (undrawnElement != null && DrawingMode != DrawingModes.Crop)))
            {
                RemoveElement(cropContainer, false);
                cropContainer = null;
                drawingElement = null;
            }

            if (drawingElement == null && DrawingMode != DrawingModes.None)
            {
                if (undrawnElement == null)
                {
                    DeselectAllElements();
                    if (undrawnElement == null)
                    {
                        CreateUndrawnElement();
                    }
                }
                drawingElement = undrawnElement;
                drawingElement.Status = EditStatus.DRAWING;
                undrawnElement = null;
                // if a new element has been drawn, set location and register it
                if (drawingElement != null)
                {
                    drawingElement.PropertyChanged += ElementPropertyChanged;
                    if (!drawingElement.HandleMouseDown(mouseStart.X, mouseStart.Y))
                    {
                        drawingElement.Left = mouseStart.X;
                        drawingElement.Top = mouseStart.Y;
                    }
                    AddElement(drawingElement);
                    drawingElement.Selected = true;
                }
            }
            else
            {
                // check whether an existing element was clicked
                // we save mouse down element separately from selectedElements (checked on mouse up),
                // since it could be moved around before it is actually selected
                mouseDownElement = elements.ClickableElementAt(mouseStart.X, mouseStart.Y);

                if (mouseDownElement != null)
                {
                    mouseDownElement.Status = EditStatus.MOVING;
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

            elements.Status = EditStatus.IDLE;
            if (mouseDownElement != null)
            {
                mouseDownElement.Status = EditStatus.IDLE;
            }
            mouseDown = false;
            mouseDownElement = null;
            if (DrawingMode == DrawingModes.None)
            {
                // check whether an existing element was clicked
                IDrawableContainer element = elements.ClickableElementAt(currentMouse.X, currentMouse.Y);
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

            if (drawingElement != null)
            {
                if (!drawingElement.InitContent())
                {
                    elements.Remove(drawingElement);
                    drawingElement.Invalidate();
                }
                else
                {
                    drawingElement.HandleMouseUp(currentMouse.X, currentMouse.Y);
                    drawingElement.Invalidate();
                    if (Math.Abs(drawingElement.Width) < 5 && Math.Abs(drawingElement.Height) < 5)
                    {
                        drawingElement.Width = 25;
                        drawingElement.Height = 25;
                    }
                    SelectElement(drawingElement);
                    drawingElement.Selected = true;
                }
                drawingElement = null;
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

            if (mouseDown)
            {
                if (mouseDownElement != null)
                { // an element is currently dragged
                    mouseDownElement.Invalidate();
                    selectedElements.HideGrippers();
                    // Move the element
                    if (mouseDownElement.Selected)
                    {
                        if (!isSurfaceMoveMadeUndoable)
                        {
                            // Only allow one undoable per mouse-down/move/up "cycle"
                            isSurfaceMoveMadeUndoable = true;
                            selectedElements.MakeBoundsChangeUndoable(false);
                        }
                        // dragged element has been selected before -> move all
                        selectedElements.MoveBy(currentMouse.X - mouseStart.X, currentMouse.Y - mouseStart.Y);
                    }
                    else
                    {
                        if (!isSurfaceMoveMadeUndoable)
                        {
                            // Only allow one undoable per mouse-down/move/up "cycle"
                            isSurfaceMoveMadeUndoable = true;
                            mouseDownElement.MakeBoundsChangeUndoable(false);
                        }
                        // dragged element is not among selected elements -> just move dragged one
                        mouseDownElement.MoveBy(currentMouse.X - mouseStart.X, currentMouse.Y - mouseStart.Y);
                    }
                    mouseStart = currentMouse;
                    mouseDownElement.Invalidate();
                    modified = true;
                }
                else if (drawingElement != null)
                {
                    drawingElement.HandleMouseMove(currentMouse.X, currentMouse.Y);
                    modified = true;
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
            Bitmap clone = ImageHelper.Clone(image, PixelFormat.DontCare);
            // otherwise we would have a problem drawing the image to the surface... :(
            using (Graphics graphics = Graphics.FromImage(clone))
            {
                // Do not set the following, the containers need to decide themselves
                //graphics.SmoothingMode = SmoothingMode.HighQuality;
                //graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                //graphics.CompositingQuality = CompositingQuality.HighQuality;
                //graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                elements.Draw(graphics, clone, renderMode, new Rectangle(Point.Empty, clone.Size));
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

            if (elements.hasIntersectingFilters(clipRectangle))
            {
                if (buffer != null)
                {
                    if (buffer.Width != Image.Width || buffer.Height != Image.Height || buffer.PixelFormat != Image.PixelFormat)
                    {
                        buffer.Dispose();
                        buffer = null;
                    }
                }
                if (buffer == null)
                {
                    buffer = ImageHelper.CreateEmpty(Image.Width, Image.Height, Image.PixelFormat, Color.Empty, Image.HorizontalResolution, Image.VerticalResolution);
                    LOG.DebugFormat("Created buffer with size: {0}x{1}", Image.Width, Image.Height);
                }
                // Elements might need the bitmap, so we copy the part we need
                using (Graphics graphics = Graphics.FromImage(buffer))
                {
                    // do not set the following, the containers need to decide this themselves!
                    //graphics.SmoothingMode = SmoothingMode.HighQuality;
                    //graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    //graphics.CompositingQuality = CompositingQuality.HighQuality;
                    //graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(Image, clipRectangle, clipRectangle, GraphicsUnit.Pixel);
                    graphics.SetClip(targetGraphics);
                    elements.Draw(graphics, buffer, RenderMode.EDIT, clipRectangle);
                }
                targetGraphics.DrawImage(buffer, clipRectangle, clipRectangle, GraphicsUnit.Pixel);
            }
            else
            {
                targetGraphics.DrawImage(Image, clipRectangle, clipRectangle, GraphicsUnit.Pixel);
                elements.Draw(targetGraphics, null, RenderMode.EDIT, clipRectangle);
            }
        }

        /// <summary>
        /// Draw a checkboard when capturing with transparency
        /// </summary>
        /// <param name="e">PaintEventArgs</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // check if we need to draw the checkerboard
            if (Image.IsAlphaPixelFormat(Image.PixelFormat) && transparencyBackgroundBrush != null)
            {
                Graphics targetGraphics = e.Graphics;
                Rectangle clipRectangle = e.ClipRectangle;
                targetGraphics.FillRectangle(transparencyBackgroundBrush, clipRectangle);
            }
            else
            {
                Graphics targetGraphics = e.Graphics;
                targetGraphics.Clear(BackColor);
                //base.OnPaintBackground(e);
            }
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
            elements.Add(element);
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
            modified = true;
        }

        /// <summary>
        /// Remove an element of the elements list
        /// </summary>
        /// <param name="elementToRemove">Element to remove</param>
        /// <param name="makeUndoable">flag specifying if the remove needs to be undoable</param>
        public void RemoveElement(IDrawableContainer elementToRemove, bool makeUndoable)
        {
            DeselectElement(elementToRemove);
            elements.Remove(elementToRemove);
            DrawableContainer element = elementToRemove as DrawableContainer;
            if (element != null)
            {
                element.FieldChanged -= element_FieldChanged;
            }
            elementToRemove.PropertyChanged -= ElementPropertyChanged;
            // Do not dispose, the memento should!! element.Dispose();
            elementToRemove.Invalidate();
            if (makeUndoable)
            {
                MakeUndoable(new DeleteElementMemento(this, elementToRemove), false);
            }
            modified = true;
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
                if (movingElementChanged != null)
                {
                    SurfaceElementEventArgs eventArgs = new SurfaceElementEventArgs();
                    eventArgs.Elements = selectedElements;
                    movingElementChanged(this, eventArgs);
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
                if (dc.Equals(cropContainer))
                {
                    DrawingMode = DrawingModes.None;
                    // No undo memento for the cropcontainer itself, only for the effect
                    RemoveElement(cropContainer, false);
                    if (confirm)
                    {
                        ApplyCrop(cropContainer.Bounds);
                    }
                    cropContainer.Dispose();
                    cropContainer = null;
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
                    dcs.Parent = this;
                    dcs.MoveBy(10, 10);
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
            if (movingElementChanged != null)
            {
                SurfaceElementEventArgs eventArgs = new SurfaceElementEventArgs();
                eventArgs.Elements = selectedElements;
                movingElementChanged(this, eventArgs);
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
                if (movingElementChanged != null)
                {
                    SurfaceElementEventArgs eventArgs = new SurfaceElementEventArgs();
                    eventArgs.Elements = selectedElements;
                    movingElementChanged(this, eventArgs);
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
                if (movingElementChanged != null)
                {
                    SurfaceElementEventArgs eventArgs = new SurfaceElementEventArgs();
                    eventArgs.Elements = selectedElements;
                    movingElementChanged(this, eventArgs);
                }
                container.Invalidate();
            }
        }

        /// <summary>
        /// Select all elements, this is called when Ctrl+A is pressed
        /// </summary>
        public void SelectAllElements()
        {
            SelectElements(elements);
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
                return elements;
            }
        }

        /// <summary>
        /// pulls selected elements up one level in hierarchy
        /// </summary>
        public void PullElementsUp()
        {
            elements.PullElementsUp(selectedElements);
            elements.Invalidate();
        }

        /// <summary>
        /// pushes selected elements up to top in hierarchy
        /// </summary>
        public void PullElementsToTop()
        {
            elements.PullElementsToTop(selectedElements);
            elements.Invalidate();
        }

        /// <summary>
        /// pushes selected elements down one level in hierarchy
        /// </summary>
        public void PushElementsDown()
        {
            elements.PushElementsDown(selectedElements);
            elements.Invalidate();
        }

        /// <summary>
        /// pushes selected elements down to bottom in hierarchy
        /// </summary>
        public void PushElementsToBottom()
        {
            elements.PushElementsToBottom(selectedElements);
            elements.Invalidate();
        }

        /// <summary>
        /// indicates whether the selected elements could be pulled up in hierarchy
        /// </summary>
        /// <returns>true if selected elements could be pulled up, false otherwise</returns>
        public bool CanPullSelectionUp()
        {
            return elements.CanPullUp(selectedElements);
        }

        /// <summary>
        /// indicates whether the selected elements could be pushed down in hierarchy
        /// </summary>
        /// <returns>true if selected elements could be pushed down, false otherwise</returns>
        public bool CanPushSelectionDown()
        {
            return elements.CanPushDown(selectedElements);
        }

        public void ElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Invalidate();
        }

        public void element_FieldChanged(object sender, FieldChangedEventArgs e)
        {
            selectedElements.HandleFieldChangedEvent(sender, e);
        }
    }
}