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
using Greenshot.Drawing;
using Greenshot.Drawing.Fields;
using Greenshot.Drawing.Fields.Binding;
using Greenshot.Forms;
using Greenshot.Helpers;
using Greenshot.IniFile;
using Greenshot.Plugin;
using Greenshot.Properties;
using GreenshotPlugin;
using GreenshotPlugin.Controls;
using GreenshotPlugin.Core;
using GreenshotPlugin.UnmanagedHelpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Greenshot
{
    /// <summary>
    /// Description of ImageEditorForm.
    /// </summary>
    public partial class ImageEditorForm : BaseForm, IImageEditor
    {
        public event Action<Image, string> ImageSaveRequested;
        public event Func<Image, string, string> ImageSaveAsRequested;
        public event Action<Image> ClipboardCopyRequested;
        public event Action<Image> ImageUploadRequested;
        public event Action<Image> PrintImageRequested;

        private static readonly EditorConfiguration EditorConfiguration = IniConfig.GetIniSection<EditorConfiguration>();
        private static readonly List<string> IgnoreDestinations = new List<string> { };
        private static readonly List<IImageEditor> EditorList = new List<IImageEditor>();

        private Surface _surface;
        private GreenshotToolStripButton[] _toolbarButtons;

        private static readonly string[] SupportedClipboardFormats = { typeof(string).FullName, "Text", typeof(DrawableContainerList).FullName };

        private bool _originalBoldCheckState;
        private bool _originalItalicCheckState;

        // whether part of the editor controls are disabled depending on selected item(s)
        private bool _controlsDisabledDueToConfirmable;

        private bool forceClose = false;
        private string titlePath = null;

        /// <summary>
        /// An Implementation for the IImageEditor, this way Plugins have access to the HWND handles wich can be used with Win32 API calls.
        /// </summary>
        public IWin32Window WindowHandle
        {
            get { return this; }
        }

        public static List<IImageEditor> Editors
        {
            get
            {
                try
                {
                    EditorList.Sort(delegate (IImageEditor e1, IImageEditor e2)
                    {
                        return String.Compare(e1.Surface.CaptureDetails.Title, e2.Surface.CaptureDetails.Title, StringComparison.Ordinal);
                    });
                }
                catch (Exception ex)
                {
                    LOG.Warn("Sorting of editors failed.", ex);
                }
                return EditorList;
            }
        }

        private bool isTaskWork;

        public bool IsTaskWork
        {
            get
            {
                return isTaskWork;
            }
            set
            {
                isTaskWork = tsbSaveClose.Visible = tsbClose.Visible = tsbCancelTasks.Visible = tssTaskButtons.Visible = value;
            }
        }

        public ImageEditorForm(ISurface iSurface, bool outputMade)
        {
            EditorList.Add(this);

            InitializeComponent();

            if (EditorConfiguration.MatchSizeToCapture)
            {
                RECT lastPosition = EditorConfiguration.GetEditorPlacement().NormalPosition;

                WindowPlacement wp = new WindowDetails(Handle).WindowPlacement;
                wp.NormalPosition.Top = lastPosition.Top;
                wp.NormalPosition.Left = lastPosition.Left;
                // don't actually show window now (it is done later)
                wp.ShowCmd = ShowWindowCommand.Hide;

                this.StartPosition = FormStartPosition.Manual;

                WindowDetails thisForm = new WindowDetails(Handle)
                {
                    WindowPlacement = wp
                };

                // Once image is loaded into window, size and position window
                Load += delegate
                {
                    Rectangle workingArea = Screen.FromControl(this).WorkingArea;
                    WindowPlacement windowPlacement = new WindowDetails(Handle).WindowPlacement;

                    if (EditorConfiguration.MaximizeWhenLargeImage)
                    {
                        if ((windowPlacement.NormalPosition.Width > workingArea.Width) || (windowPlacement.NormalPosition.Height > workingArea.Height))
                        {
                            windowPlacement.ShowCmd = ShowWindowCommand.Maximize;
                        }
                    }

                    if (windowPlacement.NormalPosition.Right > workingArea.Right)
                    {
                        int toMoveLeft = windowPlacement.NormalPosition.Right - workingArea.Right;
                        if (windowPlacement.NormalPosition.Left - toMoveLeft < 0)
                            toMoveLeft = windowPlacement.NormalPosition.Left;

                        windowPlacement.NormalPosition.Left -= toMoveLeft;
                        windowPlacement.NormalPosition.Right -= toMoveLeft;
                    }
                    if (windowPlacement.NormalPosition.Bottom > workingArea.Bottom)
                    {
                        int toMoveUp = windowPlacement.NormalPosition.Bottom - workingArea.Bottom;
                        if (windowPlacement.NormalPosition.Top - toMoveUp < 0)
                            toMoveUp = windowPlacement.NormalPosition.Top;

                        windowPlacement.NormalPosition.Top -= toMoveUp;
                        windowPlacement.NormalPosition.Bottom -= toMoveUp;
                    }
                    WindowDetails thisForm1 = new WindowDetails(Handle) { WindowPlacement = windowPlacement };
                };
            }
            else
            {
                Load += delegate
                {
                    //Make sure the editor is placed on the same location as the last editor was on close
                    WindowDetails thisForm = new WindowDetails(Handle)
                    {
                        WindowPlacement = EditorConfiguration.GetEditorPlacement()
                    };
                };
            }

            // init surface
            Surface = iSurface;
            // Intial "saved" flag for asking if the image needs to be save
            _surface.Modified = !outputMade;

            UpdateUi();

            // Workaround: As the cursor is (mostly) selected on the surface a funny artifact is visible, this fixes it.
            //HideToolstripItems();
        }

        /// <summary>
        /// Remove the current surface
        /// </summary>
        private void RemoveSurface()
        {
            if (_surface != null)
            {
                panel1.Controls.Remove(_surface);
                _surface.Dispose();
                _surface = null;
            }
        }

        /// <summary>
        /// Change the surface
        /// </summary>
        /// <param name="newSurface"></param>
        private void SetSurface(ISurface newSurface)
        {
            if (Surface != null && Surface.Modified)
            {
                throw new ApplicationException("Surface modified");
            }

            RemoveSurface();

            panel1.Height = 10;
            panel1.Width = 10;
            _surface = newSurface as Surface;
            panel1.Controls.Add(_surface);
            Image backgroundForTransparency = GreenshotResources.getImage("Checkerboard.Image");
            if (_surface != null)
            {
                _surface.TransparencyBackgroundBrush = new TextureBrush(backgroundForTransparency, WrapMode.Tile);

                _surface.MovingElementChanged += delegate
                {
                    RefreshEditorControls();
                };
                _surface.DrawingModeChanged += surface_DrawingModeChanged;
                _surface.SurfaceSizeChanged += SurfaceSizeChanged;
                _surface.SurfaceMessage += SurfaceMessageReceived;
                _surface.FieldAggregator.FieldChanged += FieldAggregatorFieldChanged;
                SurfaceSizeChanged(Surface, null);

                BindFieldControls();
                _surface.DrawingMode = EditorConfiguration.RememberLastDrawingMode ? EditorConfiguration.LastDrawingMode : EditorConfiguration.DefaultDrawingMode;
                RefreshEditorControls();
                // Fix title
                if (_surface != null && _surface.CaptureDetails != null && _surface.CaptureDetails.Title != null)
                {
                    Text = _surface.CaptureDetails.Title;
                }
            }
            WindowDetails.ToForeground(Handle);
        }

        private void UpdateUi()
        {
            Icon = GreenshotResources.getGreenshotIcon();

            // Make sure Double-buffer is enabled
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            // resizing the panel is futile, since it is docked. however, it seems
            // to fix the bug (?) with the vscrollbar not being able to shrink to
            // a smaller size than the initial panel size (as set by the forms designer)
            panel1.Height = 10;

            fontFamilyComboBox.PropertyChanged += FontPropertyChanged;

            obfuscateModeButton.DropDownItemClicked += FilterPresetDropDownItemClicked;
            highlightModeButton.DropDownItemClicked += FilterPresetDropDownItemClicked;

            _toolbarButtons = new[] { btnCursor, btnRect, btnEllipse, btnText, btnLine, btnArrow, btnFreehand,
                    btnHighlight, btnObfuscate, btnCrop, btnStepLabel, btnSpeechBubble };
            //toolbarDropDownButtons = new ToolStripDropDownButton[]{btnBlur, btnPixeliate, btnTextHighlighter, btnAreaHighlighter, btnMagnifier};

            // Workaround: for the MouseWheel event which doesn't get to the panel
            MouseWheel += PanelMouseWheel;
        }

        /// <summary>
        /// Workaround for having a border around the dropdown
        /// See: http://stackoverflow.com/questions/9560812/change-border-of-toolstripcombobox-with-flat-style
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PropertiesToolStrip_Paint(object sender, PaintEventArgs e)
        {
            using (Pen cbBorderPen = new Pen(SystemColors.ActiveBorder))
            {
                // Loop over all items in the propertiesToolStrip
                foreach (ToolStripItem item in propertiesToolStrip.Items)
                {
                    ToolStripComboBox cb = item as ToolStripComboBox;
                    // Only ToolStripComboBox that are visible
                    if (cb == null || !cb.Visible)
                    {
                        continue;
                    }
                    // Calculate the rectangle
                    if (cb.ComboBox != null)
                    {
                        Rectangle r = new Rectangle(cb.ComboBox.Location.X - 1, cb.ComboBox.Location.Y - 1, cb.ComboBox.Size.Width + 1, cb.ComboBox.Size.Height + 1);

                        // Draw the rectangle
                        e.Graphics.DrawRectangle(cbBorderPen, r);
                    }
                }
            }
        }

        /// <summary>
        /// According to some information I found, the clear doesn't work correctly when the shortcutkeys are set?
        /// This helper method takes care of this.
        /// </summary>
        /// <param name="items"></param>
        private void ClearItems(ToolStripItemCollection items)
        {
            foreach (var item in items)
            {
                ToolStripMenuItem menuItem = item as ToolStripMenuItem;
                if (menuItem != null && menuItem.ShortcutKeys != Keys.None)
                {
                    menuItem.ShortcutKeys = Keys.None;
                }
            }
            items.Clear();
        }

        private delegate void SurfaceMessageReceivedThreadSafeDelegate(object sender, SurfaceMessageEventArgs eventArgs);

        /// <summary>
        /// This is the SufraceMessageEvent receiver which display a message in the status bar if the
        /// surface is exported. It also updates the title to represent the filename, if there is one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void SurfaceMessageReceived(object sender, SurfaceMessageEventArgs eventArgs)
        {
            if (InvokeRequired)
            {
                Invoke(new SurfaceMessageReceivedThreadSafeDelegate(SurfaceMessageReceived), new object[] { sender, eventArgs });
            }
            else
            {
                string dateTime = DateTime.Now.ToLongTimeString();
                // Fix that we only open files, like in the tooltip
                switch (eventArgs.MessageType)
                {
                    case SurfaceMessageTyp.FileSaved:
                        // Put the event message on the status label and attach the context menu
                        //updateStatusLabel(dateTime + " - " + eventArgs.Message, fileSavedStatusContextMenu);
                        // Change title
                        SetTitle(eventArgs.Surface.LastSaveFullPath);
                        break;
                    case SurfaceMessageTyp.Error:
                    case SurfaceMessageTyp.Info:
                    case SurfaceMessageTyp.UploadedUri:
                    default:
                        // Put the event message on the status label
                        //updateStatusLabel(dateTime + " - " + eventArgs.Message);
                        break;
                }
            }
        }

        /// <summary>
        /// This is called when the size of the surface chances, used for resizing and displaying the size information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SurfaceSizeChanged(object sender, EventArgs e)
        {
            if (EditorConfiguration.MatchSizeToCapture)
            {
                // Set editor's initial size to the size of the surface plus the size of the chrome
                Size imageSize = Surface.Image.Size;
                Size currentFormSize = Size;
                Size currentImageClientSize = panel1.ClientSize;
                // Scale minimum size based on icons over default 16 pixels
                int minimumFormWidth = 650 + 24 * Math.Max(coreConfiguration.IconSize.Width - 16, 0);
                int minimumFormHeight = 530 + 17 * Math.Max(coreConfiguration.IconSize.Height - 16, 0);
                int newWidth = Math.Max(minimumFormWidth, currentFormSize.Width - currentImageClientSize.Width + imageSize.Width);
                int newHeight = Math.Max(minimumFormHeight, currentFormSize.Height - currentImageClientSize.Height + imageSize.Height);
                Size = new Size(newWidth, newHeight);
            }
            UpdateTitle();
            ImageEditorFormResize(sender, new EventArgs());
        }

        private void ReloadConfiguration(object source, FileSystemEventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                // Fix title
                if (_surface != null && _surface.CaptureDetails != null && _surface.CaptureDetails.Title != null)
                {
                    SetTitle(_surface.CaptureDetails.Title);
                }
            });
        }

        private void SetTitle(string filePath = null)
        {
            titlePath = filePath;
            string title = "Greenshot image editor";
            title += " - " + Surface.Image.Width + "x" + Surface.Image.Height;
            if (!string.IsNullOrEmpty(titlePath))
            {
                title += " - " + filePath;
            }
            Text = title;
        }

        private void UpdateTitle()
        {
            SetTitle(titlePath);
        }

        public ISurface Surface
        {
            get
            {
                return _surface;
            }
            set
            {
                SetSurface(value);
            }
        }

        public void SetImagePath(string fullpath)
        {
            // Check if the editor supports the format
            if (fullpath != null && (fullpath.EndsWith(".ico") || fullpath.EndsWith(".wmf")))
            {
                fullpath = null;
            }
            _surface.LastSaveFullPath = fullpath;

            if (fullpath == null)
            {
                return;
            }
            //updateStatusLabel(string.Format("Image saved to {0}.", fullpath), fileSavedStatusContextMenu);
            SetTitle(Path.GetFileName(fullpath));

            tsbSaveImage.Enabled = tsmiSaveImage.Enabled = File.Exists(_surface.LastSaveFullPath);
        }

        private void surface_DrawingModeChanged(object source, SurfaceDrawingModeEventArgs eventArgs)
        {
            switch (eventArgs.DrawingMode)
            {
                case DrawingModes.None:
                    SetButtonChecked(btnCursor);
                    break;
                case DrawingModes.Ellipse:
                    SetButtonChecked(btnEllipse);
                    break;
                case DrawingModes.Rect:
                    SetButtonChecked(btnRect);
                    break;
                case DrawingModes.Text:
                    SetButtonChecked(btnText);
                    break;
                case DrawingModes.SpeechBubble:
                    SetButtonChecked(btnSpeechBubble);
                    break;
                case DrawingModes.StepLabel:
                    SetButtonChecked(btnStepLabel);
                    break;
                case DrawingModes.Line:
                    SetButtonChecked(btnLine);
                    break;
                case DrawingModes.Arrow:
                    SetButtonChecked(btnArrow);
                    break;
                case DrawingModes.Crop:
                    SetButtonChecked(btnCrop);
                    break;
                case DrawingModes.Highlight:
                    SetButtonChecked(btnHighlight);
                    break;
                case DrawingModes.Obfuscate:
                    SetButtonChecked(btnObfuscate);
                    break;
                case DrawingModes.Path:
                    SetButtonChecked(btnFreehand);
                    break;
            }
        }

        #region plugin interfaces

        /**
		 * Interfaces for plugins, see GreenshotInterface for more details!
		 */

        public Image GetImageForExport()
        {
            return _surface.GetImageForExport();
        }

        public ICaptureDetails CaptureDetails
        {
            get { return _surface.CaptureDetails; }
        }

        public ToolStripMenuItem GetPluginMenuItem()
        {
            return null;
        }

        public ToolStripMenuItem GetFileMenuItem()
        {
            return null;
        }

        #endregion plugin interfaces

        #region drawing options

        private void BtnEllipseClick(object sender, EventArgs e)
        {
            _surface.DrawingMode = DrawingModes.Ellipse;
            RefreshFieldControls();
        }

        private void BtnCursorClick(object sender, EventArgs e)
        {
            _surface.DrawingMode = DrawingModes.None;
            RefreshFieldControls();
        }

        private void BtnRectClick(object sender, EventArgs e)
        {
            _surface.DrawingMode = DrawingModes.Rect;
            RefreshFieldControls();
        }

        private void BtnTextClick(object sender, EventArgs e)
        {
            _surface.DrawingMode = DrawingModes.Text;
            RefreshFieldControls();
        }

        private void BtnSpeechBubbleClick(object sender, EventArgs e)
        {
            _surface.DrawingMode = DrawingModes.SpeechBubble;
            RefreshFieldControls();
        }

        private void BtnStepLabelClick(object sender, EventArgs e)
        {
            _surface.DrawingMode = DrawingModes.StepLabel;
            RefreshFieldControls();
        }

        private void BtnLineClick(object sender, EventArgs e)
        {
            _surface.DrawingMode = DrawingModes.Line;
            RefreshFieldControls();
        }

        private void BtnArrowClick(object sender, EventArgs e)
        {
            _surface.DrawingMode = DrawingModes.Arrow;
            RefreshFieldControls();
        }

        private void BtnCropClick(object sender, EventArgs e)
        {
            _surface.DrawingMode = DrawingModes.Crop;
            RefreshFieldControls();
        }

        private void BtnHighlightClick(object sender, EventArgs e)
        {
            _surface.DrawingMode = DrawingModes.Highlight;
            RefreshFieldControls();
        }

        private void BtnObfuscateClick(object sender, EventArgs e)
        {
            _surface.DrawingMode = DrawingModes.Obfuscate;
            RefreshFieldControls();
        }

        private void BtnFreehandClick(object sender, EventArgs e)
        {
            _surface.DrawingMode = DrawingModes.Path;
            RefreshFieldControls();
        }

        private void SetButtonChecked(ToolStripButton btn)
        {
            UncheckAllToolButtons();
            btn.Checked = true;
        }

        private void UncheckAllToolButtons()
        {
            if (_toolbarButtons != null)
            {
                foreach (GreenshotToolStripButton butt in _toolbarButtons)
                {
                    butt.Checked = false;
                }
            }
        }

        private void RemoveObjectToolStripMenuItemClick(object sender, EventArgs e)
        {
            _surface.RemoveSelectedElements();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _surface.SelectAllElements();
            _surface.RemoveSelectedElements();
        }

        #endregion drawing options

        #region copy&paste options

        private void CutToolStripMenuItemClick(object sender, EventArgs e)
        {
            _surface.CutSelectedElements();
            UpdateClipboardSurfaceDependencies();
        }

        private void CopyToolStripMenuItemClick(object sender, EventArgs e)
        {
            _surface.CopySelectedElements();
            UpdateClipboardSurfaceDependencies();
        }

        private void PasteToolStripMenuItemClick(object sender, EventArgs e)
        {
            _surface.PasteElementFromClipboard();
            UpdateClipboardSurfaceDependencies();
        }

        private void UndoToolStripMenuItemClick(object sender, EventArgs e)
        {
            _surface.Undo();
            UpdateUndoRedoSurfaceDependencies();
        }

        private void RedoToolStripMenuItemClick(object sender, EventArgs e)
        {
            _surface.Redo();
            UpdateUndoRedoSurfaceDependencies();
        }

        private void DuplicateToolStripMenuItemClick(object sender, EventArgs e)
        {
            _surface.DuplicateSelectedElements();
            UpdateClipboardSurfaceDependencies();
        }

        #endregion copy&paste options

        #region element properties

        private void UpOneLevelToolStripMenuItemClick(object sender, EventArgs e)
        {
            _surface.PullElementsUp();
        }

        private void DownOneLevelToolStripMenuItemClick(object sender, EventArgs e)
        {
            _surface.PushElementsDown();
        }

        private void UpToTopToolStripMenuItemClick(object sender, EventArgs e)
        {
            _surface.PullElementsToTop();
        }

        private void DownToBottomToolStripMenuItemClick(object sender, EventArgs e)
        {
            _surface.PushElementsToBottom();
        }

        #endregion element properties

        #region image editor event handlers

        private void ImageEditorFormActivated(object sender, EventArgs e)
        {
            UpdateClipboardSurfaceDependencies();
            UpdateUndoRedoSurfaceDependencies();

            tsbSaveImage.Enabled = tsmiSaveImage.Enabled = _surface != null ? File.Exists(_surface.LastSaveFullPath) : false;
        }

        private void ImageEditorFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!forceClose && _surface.Modified)
            {
                if (!EditorConfiguration.SuppressSaveDialogAtClose)
                {
                    // Make sure the editor is visible
                    WindowDetails.ToForeground(Handle);

                    MessageBoxButtons buttons = MessageBoxButtons.YesNoCancel;
                    // Dissallow "CANCEL" if the application needs to shutdown
                    if (e.CloseReason == CloseReason.ApplicationExitCall || e.CloseReason == CloseReason.WindowsShutDown || e.CloseReason == CloseReason.TaskManagerClosing)
                    {
                        buttons = MessageBoxButtons.YesNo;
                    }

                    DialogResult result = MessageBox.Show("Do you want to save the screenshot?", "Save confirmation", buttons, MessageBoxIcon.Question);

                    if (result.Equals(DialogResult.Cancel))
                    {
                        e.Cancel = true;
                        return;
                    }

                    if (result.Equals(DialogResult.Yes))
                    {
                        DialogResult = DialogResult.OK;
                        OnImageSaveRequested();
                    }
                }
                else
                {
                    DialogResult = DialogResult.OK;
                }
            }

            // persist our geometry string.
            EditorConfiguration.SetEditorPlacement(new WindowDetails(Handle).WindowPlacement);
            // save last used drawing mode
            EditorConfiguration.LastDrawingMode = _surface.DrawingMode;
            IniConfig.Save();

            // remove from the editor list
            EditorList.Remove(this);
        }

        private void ImageEditorFormKeyDown(object sender, KeyEventArgs e)
        {
            // LOG.Debug("Got key event "+e.KeyCode + ", " + e.Modifiers);
            // avoid conflict with other shortcuts and
            // make sure there's no selected element claiming input focus
            if (e.Modifiers.Equals(Keys.None) && !_surface.KeysLocked)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        BtnCursorClick(sender, e);
                        break;
                    case Keys.R:
                        BtnRectClick(sender, e);
                        break;
                    case Keys.E:
                        BtnEllipseClick(sender, e);
                        break;
                    case Keys.L:
                        BtnLineClick(sender, e);
                        break;
                    case Keys.F:
                        BtnFreehandClick(sender, e);
                        break;
                    case Keys.A:
                        BtnArrowClick(sender, e);
                        break;
                    case Keys.T:
                        BtnTextClick(sender, e);
                        break;
                    case Keys.S:
                        BtnSpeechBubbleClick(sender, e);
                        break;
                    case Keys.I:
                        BtnStepLabelClick(sender, e);
                        break;
                    case Keys.H:
                        BtnHighlightClick(sender, e);
                        break;
                    case Keys.O:
                        BtnObfuscateClick(sender, e);
                        break;
                    case Keys.C:
                        BtnCropClick(sender, e);
                        break;
                }
            }
            else if (e.Modifiers.Equals(Keys.Control))
            {
                switch (e.KeyCode)
                {
                    case Keys.Z:
                        UndoToolStripMenuItemClick(sender, e);
                        break;
                    case Keys.Y:
                        RedoToolStripMenuItemClick(sender, e);
                        break;
                    case Keys.Q:	// Dropshadow Ctrl + Q
                        AddDropshadowToolStripMenuItemClick(sender, e);
                        break;
                    case Keys.B:	// Border Ctrl + B
                        AddBorderToolStripMenuItemClick(sender, e);
                        break;
                    case Keys.T:	// Torn edge Ctrl + T
                        TornEdgesToolStripMenuItemClick(sender, e);
                        break;
                    case Keys.I:	// Invert Ctrl + I
                        InvertToolStripMenuItemClick(sender, e);
                        break;
                    case Keys.G:	// Grayscale Ctrl + G
                        GrayscaleToolStripMenuItemClick(sender, e);
                        break;
                    case Keys.Delete:	// Grayscale Ctrl + Delete
                        ClearToolStripMenuItemClick(sender, e);
                        break;
                    case Keys.Oemcomma:	// Rotate CCW Ctrl + ,
                        RotateCcwToolstripButtonClick(sender, e);
                        break;
                    case Keys.OemPeriod:	// Rotate CW Ctrl + .
                        RotateCwToolstripButtonClick(sender, e);
                        break;
                }
            }
            else if (e.Modifiers.Equals(Keys.Alt))
            {
                switch (e.KeyCode)
                {
                    case Keys.S:
                        if (IsTaskWork)
                        {
                            btnSaveClose_Click(sender, e);
                        }
                        break;
                    case Keys.W:
                        if (IsTaskWork)
                        {
                            btnClose_Click(sender, e);
                        }
                        break;
                    case Keys.C:
                        if (IsTaskWork)
                        {
                            btnCancelTasks_Click(sender, e);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// This is a "work-around" for the MouseWheel event which doesn't get to the panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelMouseWheel(object sender, MouseEventArgs e)
        {
            panel1.Focus();
        }

        #endregion image editor event handlers

        #region key handling

        protected override bool ProcessKeyPreview(ref Message msg)
        {
            // disable default key handling if surface has requested a lock
            if (!_surface.KeysLocked)
            {
                return base.ProcessKeyPreview(ref msg);
            }
            return false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keys)
        {
            // disable default key handling if surface has requested a lock
            if (!_surface.KeysLocked)
            {
                if (!_surface.ProcessCmdKey(keys))
                {
                    return base.ProcessCmdKey(ref msg, keys);
                }
            }
            return false;
        }

        #endregion key handling

        #region helpers

        private void UpdateUndoRedoSurfaceDependencies()
        {
            if (_surface == null)
            {
                return;
            }
            bool canUndo = _surface.CanUndo;
            btnUndo.Enabled = canUndo;
            undoToolStripMenuItem.Enabled = canUndo;
            string undoText = "Undo";
            btnUndo.Text = undoText;
            undoToolStripMenuItem.Text = undoText;

            bool canRedo = _surface.CanRedo;
            btnRedo.Enabled = canRedo;
            redoToolStripMenuItem.Enabled = canRedo;
            string redoText = "Redo";
            btnRedo.Text = redoText;
            redoToolStripMenuItem.Text = redoText;
        }

        private void UpdateClipboardSurfaceDependencies()
        {
            if (_surface == null)
            {
                return;
            }
            // check dependencies for the Surface
            bool hasItems = _surface.HasSelectedElements;
            bool actionAllowedForSelection = hasItems && !_controlsDisabledDueToConfirmable;

            // buttons
            btnCut.Enabled = actionAllowedForSelection;
            btnCopy.Enabled = actionAllowedForSelection;
            btnDelete.Enabled = actionAllowedForSelection;

            // menus
            removeObjectToolStripMenuItem.Enabled = actionAllowedForSelection;
            copyToolStripMenuItem.Enabled = actionAllowedForSelection;
            cutToolStripMenuItem.Enabled = actionAllowedForSelection;
            duplicateToolStripMenuItem.Enabled = actionAllowedForSelection;

            // check dependencies for the Clipboard
            bool hasClipboard = ClipboardHelper.ContainsFormat(SupportedClipboardFormats) || ClipboardHelper.ContainsImage();
            btnPaste.Enabled = hasClipboard && !_controlsDisabledDueToConfirmable;
            pasteToolStripMenuItem.Enabled = hasClipboard && !_controlsDisabledDueToConfirmable;
        }

        #endregion helpers

        #region status label handling

        /*private void updateStatusLabel(string text, ContextMenuStrip contextMenu)
        {
            //statusLabel.Text = text;
            statusStrip1.ContextMenuStrip = contextMenu;
        }

        private void updateStatusLabel(string text)
        {
            updateStatusLabel(text, null);
        }

        private void ClearStatusLabel()
        {
            updateStatusLabel(null, null);
        }

        private void StatusLabelClicked(object sender, MouseEventArgs e)
        {
            ToolStrip ss = (StatusStrip)((ToolStripStatusLabel)sender).Owner;
            if (ss.ContextMenuStrip != null)
            {
                ss.ContextMenuStrip.Show(ss, e.X, e.Y);
            }
        }*/

        #endregion status label handling

        private void BindFieldControls()
        {
            new BidirectionalBinding(btnFillColor, "SelectedColor", _surface.FieldAggregator.GetField(FieldType.FILL_COLOR), "Value", NotNullValidator.GetInstance());
            new BidirectionalBinding(btnLineColor, "SelectedColor", _surface.FieldAggregator.GetField(FieldType.LINE_COLOR), "Value", NotNullValidator.GetInstance());
            new BidirectionalBinding(lineThicknessUpDown, "Value", _surface.FieldAggregator.GetField(FieldType.LINE_THICKNESS), "Value", DecimalIntConverter.GetInstance(), NotNullValidator.GetInstance());
            new BidirectionalBinding(blurRadiusUpDown, "Value", _surface.FieldAggregator.GetField(FieldType.BLUR_RADIUS), "Value", DecimalIntConverter.GetInstance(), NotNullValidator.GetInstance());
            new BidirectionalBinding(magnificationFactorUpDown, "Value", _surface.FieldAggregator.GetField(FieldType.MAGNIFICATION_FACTOR), "Value", DecimalIntConverter.GetInstance(), NotNullValidator.GetInstance());
            new BidirectionalBinding(pixelSizeUpDown, "Value", _surface.FieldAggregator.GetField(FieldType.PIXEL_SIZE), "Value", DecimalIntConverter.GetInstance(), NotNullValidator.GetInstance());
            new BidirectionalBinding(brightnessUpDown, "Value", _surface.FieldAggregator.GetField(FieldType.BRIGHTNESS), "Value", DecimalDoublePercentageConverter.GetInstance(), NotNullValidator.GetInstance());
            new BidirectionalBinding(fontFamilyComboBox, "Text", _surface.FieldAggregator.GetField(FieldType.FONT_FAMILY), "Value", NotNullValidator.GetInstance());
            new BidirectionalBinding(fontSizeUpDown, "Value", _surface.FieldAggregator.GetField(FieldType.FONT_SIZE), "Value", DecimalFloatConverter.GetInstance(), NotNullValidator.GetInstance());
            new BidirectionalBinding(fontBoldButton, "Checked", _surface.FieldAggregator.GetField(FieldType.FONT_BOLD), "Value", NotNullValidator.GetInstance());
            new BidirectionalBinding(fontItalicButton, "Checked", _surface.FieldAggregator.GetField(FieldType.FONT_ITALIC), "Value", NotNullValidator.GetInstance());
            new BidirectionalBinding(textHorizontalAlignmentButton, "SelectedTag", _surface.FieldAggregator.GetField(FieldType.TEXT_HORIZONTAL_ALIGNMENT), "Value", NotNullValidator.GetInstance());
            new BidirectionalBinding(textVerticalAlignmentButton, "SelectedTag", _surface.FieldAggregator.GetField(FieldType.TEXT_VERTICAL_ALIGNMENT), "Value", NotNullValidator.GetInstance());
            new BidirectionalBinding(shadowButton, "Checked", _surface.FieldAggregator.GetField(FieldType.SHADOW), "Value", NotNullValidator.GetInstance());
            new BidirectionalBinding(previewQualityUpDown, "Value", _surface.FieldAggregator.GetField(FieldType.PREVIEW_QUALITY), "Value", DecimalDoublePercentageConverter.GetInstance(), NotNullValidator.GetInstance());
            new BidirectionalBinding(obfuscateModeButton, "SelectedTag", _surface.FieldAggregator.GetField(FieldType.PREPARED_FILTER_OBFUSCATE), "Value");
            new BidirectionalBinding(highlightModeButton, "SelectedTag", _surface.FieldAggregator.GetField(FieldType.PREPARED_FILTER_HIGHLIGHT), "Value");
        }

        /// <summary>
        /// shows/hides field controls (2nd toolbar on top) depending on fields of selected elements
        /// </summary>
        private void RefreshFieldControls()
        {
            propertiesToolStrip.SuspendLayout();

            if (_surface.HasSelectedElements || _surface.DrawingMode != DrawingModes.None)
            {
                toolStripSeparator5.Visible = true;
                FieldAggregator props = _surface.FieldAggregator;
                btnFillColor.Visible = props.HasFieldValue(FieldType.FILL_COLOR);
                btnLineColor.Visible = props.HasFieldValue(FieldType.LINE_COLOR);
                lineThicknessLabel.Visible = lineThicknessUpDown.Visible = props.HasFieldValue(FieldType.LINE_THICKNESS);
                blurRadiusLabel.Visible = blurRadiusUpDown.Visible = props.HasFieldValue(FieldType.BLUR_RADIUS);
                previewQualityLabel.Visible = previewQualityUpDown.Visible = props.HasFieldValue(FieldType.PREVIEW_QUALITY);
                magnificationFactorLabel.Visible = magnificationFactorUpDown.Visible = props.HasFieldValue(FieldType.MAGNIFICATION_FACTOR);
                pixelSizeLabel.Visible = pixelSizeUpDown.Visible = props.HasFieldValue(FieldType.PIXEL_SIZE);
                brightnessLabel.Visible = brightnessUpDown.Visible = props.HasFieldValue(FieldType.BRIGHTNESS);
                arrowHeadsLabel.Visible = arrowHeadsDropDownButton.Visible = props.HasFieldValue(FieldType.ARROWHEADS);
                fontFamilyComboBox.Visible = props.HasFieldValue(FieldType.FONT_FAMILY);
                fontSizeLabel.Visible = fontSizeUpDown.Visible = props.HasFieldValue(FieldType.FONT_SIZE);
                fontBoldButton.Visible = props.HasFieldValue(FieldType.FONT_BOLD);
                fontItalicButton.Visible = props.HasFieldValue(FieldType.FONT_ITALIC);
                textHorizontalAlignmentButton.Visible = props.HasFieldValue(FieldType.TEXT_HORIZONTAL_ALIGNMENT);
                textVerticalAlignmentButton.Visible = props.HasFieldValue(FieldType.TEXT_VERTICAL_ALIGNMENT);
                shadowButton.Visible = props.HasFieldValue(FieldType.SHADOW);
                btnConfirm.Visible = btnCancel.Visible = props.HasFieldValue(FieldType.FLAGS)
                    && ((FieldType.Flag)props.GetFieldValue(FieldType.FLAGS) & FieldType.Flag.CONFIRMABLE) == FieldType.Flag.CONFIRMABLE;
                obfuscateModeButton.Visible = props.HasFieldValue(FieldType.PREPARED_FILTER_OBFUSCATE);
                highlightModeButton.Visible = props.HasFieldValue(FieldType.PREPARED_FILTER_HIGHLIGHT);
            }
            else
            {
                HideToolstripItems();
            }

            propertiesToolStrip.ResumeLayout();
        }

        private void HideToolstripItems()
        {
            btnFillColor.Visible = btnLineColor.Visible = lineThicknessLabel.Visible = lineThicknessUpDown.Visible = blurRadiusLabel.Visible =
                blurRadiusUpDown.Visible = previewQualityLabel.Visible = previewQualityUpDown.Visible = magnificationFactorLabel.Visible =
                magnificationFactorUpDown.Visible = pixelSizeLabel.Visible = pixelSizeUpDown.Visible = brightnessLabel.Visible = brightnessUpDown.Visible =
                arrowHeadsLabel.Visible = arrowHeadsDropDownButton.Visible = fontFamilyComboBox.Visible = fontSizeLabel.Visible = fontSizeUpDown.Visible =
                fontBoldButton.Visible = fontItalicButton.Visible = textHorizontalAlignmentButton.Visible = textVerticalAlignmentButton.Visible =
                shadowButton.Visible = btnConfirm.Visible = btnCancel.Visible = obfuscateModeButton.Visible = highlightModeButton.Visible =
                toolStripSeparator5.Visible = false;
        }

        /// <summary>
        /// refreshes all editor controls depending on selected elements and their fields
        /// </summary>
        private void RefreshEditorControls()
        {
            int stepLabels = _surface.CountStepLabels(null);
            Image icon;
            if (stepLabels <= 20)
            {
                icon = (Image)Resources.ResourceManager.GetObject(string.Format("notification_counter_{0:00}", stepLabels));
            }
            else
            {
                icon = (Image)Resources.ResourceManager.GetObject("notification_counter_20_plus");
            }
            btnStepLabel.Image = icon;

            FieldAggregator props = _surface.FieldAggregator;
            // if a confirmable element is selected, we must disable most of the controls
            // since we demand confirmation or cancel for confirmable element
            if (props.HasFieldValue(FieldType.FLAGS) && ((FieldType.Flag)props.GetFieldValue(FieldType.FLAGS) & FieldType.Flag.CONFIRMABLE) == FieldType.Flag.CONFIRMABLE
                && _surface.HasSelectedElements) // if nothing is selected, there is nothing to cancel, so don't disable controls
            {
                // disable most controls
                if (!_controlsDisabledDueToConfirmable)
                {
                    ToolStripItemEndisabler.Disable(menuStrip1);
                    //ToolStripItemEndisabler.Disable(propertiesToolStrip);
                    ToolStripItemEndisabler.Disable(toolsToolStrip);
                    ToolStripItemEndisabler.Enable(closeToolStripMenuItem);
                    _controlsDisabledDueToConfirmable = true;
                }
            }
            else if (_controlsDisabledDueToConfirmable)
            {
                // re-enable disabled controls, confirmable element has either been confirmed or cancelled
                ToolStripItemEndisabler.Enable(menuStrip1);
                //ToolStripItemEndisabler.Enable(propertiesToolStrip);
                ToolStripItemEndisabler.Enable(toolsToolStrip);
                _controlsDisabledDueToConfirmable = false;
            }

            // en/disable controls depending on whether an element is selected at all
            UpdateClipboardSurfaceDependencies();
            UpdateUndoRedoSurfaceDependencies();

            // en/disablearrage controls depending on hierarchy of selected elements
            bool actionAllowedForSelection = _surface.HasSelectedElements && !_controlsDisabledDueToConfirmable;
            bool push = actionAllowedForSelection && _surface.CanPushSelectionDown();
            bool pull = actionAllowedForSelection && _surface.CanPullSelectionUp();
            arrangeToolStripMenuItem.Enabled = push || pull;
            if (arrangeToolStripMenuItem.Enabled)
            {
                upToTopToolStripMenuItem.Enabled = pull;
                upOneLevelToolStripMenuItem.Enabled = pull;
                downToBottomToolStripMenuItem.Enabled = push;
                downOneLevelToolStripMenuItem.Enabled = push;
            }

            // finally show/hide field controls depending on the fields of selected elements
            RefreshFieldControls();
        }

        private void ArrowHeadsToolStripMenuItemClick(object sender, EventArgs e)
        {
            _surface.FieldAggregator.GetField(FieldType.ARROWHEADS).Value = (ArrowContainer.ArrowHeadCombination)((ToolStripMenuItem)sender).Tag;
        }

        private void FontPropertyChanged(object sender, EventArgs e)
        {
            // in case we forced another FontStyle before, reset it first.
            if (fontBoldButton != null && _originalBoldCheckState != fontBoldButton.Checked)
            {
                fontBoldButton.Checked = _originalBoldCheckState;
            }
            if (fontItalicButton != null && _originalItalicCheckState != fontItalicButton.Checked)
            {
                fontItalicButton.Checked = _originalItalicCheckState;
            }

            FontFamily fam = fontFamilyComboBox.FontFamily;

            bool boldAvailable = fam.IsStyleAvailable(FontStyle.Bold);
            if (!boldAvailable)
            {
                _originalBoldCheckState = fontBoldButton.Checked;
                fontBoldButton.Checked = false;
            }
            fontBoldButton.Enabled = boldAvailable;

            bool italicAvailable = fam.IsStyleAvailable(FontStyle.Italic);
            if (!italicAvailable) fontItalicButton.Checked = false;
            fontItalicButton.Enabled = italicAvailable;

            bool regularAvailable = fam.IsStyleAvailable(FontStyle.Regular);
            if (!regularAvailable)
            {
                if (boldAvailable)
                {
                    fontBoldButton.Checked = true;
                }
                else if (italicAvailable)
                {
                    fontItalicButton.Checked = true;
                }
            }
        }

        private void FieldAggregatorFieldChanged(object sender, FieldChangedEventArgs e)
        {
            // in addition to selection, deselection of elements, we need to
            // refresh toolbar if prepared filter mode is changed
            if (e.Field.FieldType == FieldType.PREPARED_FILTER_HIGHLIGHT)
            {
                RefreshFieldControls();
            }
        }

        private void FontBoldButtonClick(object sender, EventArgs e)
        {
            _originalBoldCheckState = fontBoldButton.Checked;
        }

        private void FontItalicButtonClick(object sender, EventArgs e)
        {
            _originalItalicCheckState = fontItalicButton.Checked;
        }

        private void ToolBarFocusableElementGotFocus(object sender, EventArgs e)
        {
            _surface.KeysLocked = true;
        }

        private void ToolBarFocusableElementLostFocus(object sender, EventArgs e)
        {
            _surface.KeysLocked = false;
        }

        private void SaveElementsToolStripMenuItemClick(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Greenshot templates (*.gst)|*.gst";
            saveFileDialog.FileName = FilenameHelper.GetFilenameWithoutExtensionFromPattern(coreConfiguration.OutputFileFilenamePattern, _surface.CaptureDetails);
            DialogResult dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult.Equals(DialogResult.OK))
            {
                using (Stream streamWrite = File.OpenWrite(saveFileDialog.FileName))
                {
                    _surface.SaveElementsToStream(streamWrite);
                }
            }
        }

        private void LoadElementsToolStripMenuItemClick(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Greenshot templates (*.gst)|*.gst";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (Stream streamRead = File.OpenRead(openFileDialog.FileName))
                {
                    _surface.LoadElementsFromStream(streamRead);
                }
                _surface.Refresh();
            }
        }

        protected void FilterPresetDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            RefreshFieldControls();
            Invalidate(true);
        }

        private void SelectAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            _surface.SelectAllElements();
        }

        private void BtnConfirmClick(object sender, EventArgs e)
        {
            _surface.ConfirmSelectedConfirmableElements(true);
            RefreshFieldControls();
        }

        private void BtnCancelClick(object sender, EventArgs e)
        {
            _surface.ConfirmSelectedConfirmableElements(false);
            RefreshFieldControls();
        }

        private void AutoCropToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_surface.AutoCrop())
            {
                RefreshFieldControls();
            }
        }

        private void AddBorderToolStripMenuItemClick(object sender, EventArgs e)
        {
            _surface.ApplyBitmapEffect(new BorderEffect());
            UpdateUndoRedoSurfaceDependencies();
        }

        /// <summary>
        /// This is used when the dropshadow button is used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddDropshadowToolStripMenuItemClick(object sender, EventArgs e)
        {
            DropShadowEffect dropShadowEffect = EditorConfiguration.DropShadowEffectSettings;
            // Use the dropshadow settings form to make it possible to change the default values
            DialogResult result = new DropShadowSettingsForm(dropShadowEffect).ShowDialog(this);
            if (result == DialogResult.OK)
            {
                _surface.ApplyBitmapEffect(dropShadowEffect);
                UpdateUndoRedoSurfaceDependencies();
            }
        }

        /// <summary>
        /// Open the resize settings from, and resize if ok was pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnResizeClick(object sender, EventArgs e)
        {
            ResizeEffect resizeEffect = new ResizeEffect(_surface.Image.Width, _surface.Image.Height, true);
            // Use the Resize SettingsForm to make it possible to change the default values
            DialogResult result = new ResizeSettingsForm(resizeEffect).ShowDialog(this);
            if (result == DialogResult.OK)
            {
                _surface.ApplyBitmapEffect(resizeEffect);
                UpdateUndoRedoSurfaceDependencies();
            }
        }

        /// <summary>
        /// Currently unused
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeToolStripMenuItemClick(object sender, EventArgs e)
        {
            ResizeEffect resizeEffect = new ResizeEffect(_surface.Image.Width, _surface.Image.Height, true);
            // DialogResult result = new ResizeSettingsForm(resizeEffect).ShowDialog(this);
            // if (result == DialogResult.OK) {
            _surface.ApplyBitmapEffect(resizeEffect);
            UpdateUndoRedoSurfaceDependencies();
            //}
        }

        /// <summary>
        /// Call the torn edge effect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TornEdgesToolStripMenuItemClick(object sender, EventArgs e)
        {
            TornEdgeEffect tornEdgeEffect = EditorConfiguration.TornEdgeEffectSettings;
            // Use the dropshadow settings form to make it possible to change the default values
            DialogResult result = new TornEdgeSettingsForm(tornEdgeEffect).ShowDialog(this);
            if (result == DialogResult.OK)
            {
                _surface.ApplyBitmapEffect(tornEdgeEffect);
                UpdateUndoRedoSurfaceDependencies();
            }
        }

        private void GrayscaleToolStripMenuItemClick(object sender, EventArgs e)
        {
            _surface.ApplyBitmapEffect(new GrayscaleEffect());
            UpdateUndoRedoSurfaceDependencies();
        }

        private void ClearToolStripMenuItemClick(object sender, EventArgs e)
        {
            _surface.Clear(Color.Transparent);
            UpdateUndoRedoSurfaceDependencies();
        }

        private void RotateCwToolstripButtonClick(object sender, EventArgs e)
        {
            _surface.ApplyBitmapEffect(new RotateEffect(90));
            UpdateUndoRedoSurfaceDependencies();
        }

        private void RotateCcwToolstripButtonClick(object sender, EventArgs e)
        {
            _surface.ApplyBitmapEffect(new RotateEffect(270));
            UpdateUndoRedoSurfaceDependencies();
        }

        private void InvertToolStripMenuItemClick(object sender, EventArgs e)
        {
            _surface.ApplyBitmapEffect(new InvertEffect());
            UpdateUndoRedoSurfaceDependencies();
        }

        private void ImageEditorFormResize(object sender, EventArgs e)
        {
            if (Surface == null || Surface.Image == null || panel1 == null)
            {
                return;
            }
            Size imageSize = Surface.Image.Size;
            Size currentClientSize = panel1.ClientSize;
            var canvas = Surface as Control;
            if (canvas == null)
            {
                return;
            }
            Panel panel = (Panel)canvas.Parent;
            if (panel == null)
            {
                return;
            }
            int offsetX = -panel.HorizontalScroll.Value;
            int offsetY = -panel.VerticalScroll.Value;
            if (currentClientSize.Width > imageSize.Width)
            {
                canvas.Left = offsetX + ((currentClientSize.Width - imageSize.Width) / 2);
            }
            else
            {
                canvas.Left = offsetX + 0;
            }
            if (currentClientSize.Height > imageSize.Height)
            {
                canvas.Top = offsetY + ((currentClientSize.Height - imageSize.Height) / 2);
            }
            else
            {
                canvas.Top = offsetY + 0;
            }
        }

        public void OnImageSaveRequested()
        {
            if (ImageSaveRequested != null && File.Exists(_surface.LastSaveFullPath))
            {
                using (Image img = _surface.GetImageForExport())
                {
                    ImageSaveRequested(img, _surface.LastSaveFullPath);
                    _surface.Modified = false;
                }
            }
        }

        public void OnImageSaveAsRequested()
        {
            if (ImageSaveAsRequested != null)
            {
                using (Image img = _surface.GetImageForExport())
                {
                    string newFilePath = ImageSaveAsRequested(img, _surface.LastSaveFullPath);
                    _surface.Modified = false;
                    if (!string.IsNullOrEmpty(newFilePath))
                    {
                        SetImagePath(newFilePath);
                    }
                }
            }
        }

        public void OnClipboardCopyRequested()
        {
            if (ClipboardCopyRequested != null)
            {
                using (Image img = _surface.GetImageForExport())
                {
                    ClipboardCopyRequested(img);
                }
            }
        }

        public void OnImageUploadRequested()
        {
            if (ImageUploadRequested != null)
            {
                // Image will be disposed in upload task
                Image img = _surface.GetImageForExport();
                ImageUploadRequested(img);
            }
        }

        public void OnPrintImageRequested()
        {
            if (PrintImageRequested != null)
            {
                using (Image img = _surface.GetImageForExport())
                {
                    PrintImageRequested(img);
                }
            }
        }

        private void btnSaveClose_Click(object sender, EventArgs e)
        {
            OnImageSaveRequested();
            DialogResult = DialogResult.OK;
            forceClose = true;
            Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            forceClose = true;
            Close();
        }

        private void btnCancelTasks_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            forceClose = true;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            OnImageSaveRequested();
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            OnImageSaveAsRequested();
        }

        private void btnClipboardCopy_Click(object sender, EventArgs e)
        {
            OnClipboardCopyRequested();
        }

        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            OnImageUploadRequested();
        }

        private void tsbPrintImage_Click(object sender, EventArgs e)
        {
            OnPrintImageRequested();
        }

        private void menuStrip1_Click(object sender, EventArgs e)
        {
            UpdateClipboardSurfaceDependencies();
            UpdateUndoRedoSurfaceDependencies();
        }

        private void tsmiSettings_Click(object sender, EventArgs e)
        {
            using (EditorSettingsForm settingsForm = new EditorSettingsForm())
            {
                settingsForm.ShowDialog();
            }
        }

        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            using (AboutForm aboutForm = new AboutForm())
            {
                aboutForm.ShowDialog();
            }
        }
    }
}