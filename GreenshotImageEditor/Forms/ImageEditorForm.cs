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
using Greenshot.Helpers;
using Greenshot.IniFile;
using Greenshot.Plugin;
using GreenshotPlugin.Controls;
using GreenshotPlugin.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public event Action<Image> ClipboardCopyRequested;
        public event Action<Image> ImageUploadRequested;
        public event Func<Image, string, string> ImageSaveAsRequested;
        public event Action<Image, string> ImageSaveRequested;

        private static EditorConfiguration editorConfiguration = IniConfig.GetIniSection<EditorConfiguration>();
        private static List<string> ignoreDestinations = new List<string> { };
        private static List<IImageEditor> editorList = new List<IImageEditor>();

        private Surface surface;
        private GreenshotToolStripButton[] toolbarButtons;

        private static string[] SUPPORTED_CLIPBOARD_FORMATS = { typeof(string).FullName, "Text", typeof(DrawableContainerList).FullName };

        private bool originalBoldCheckState = false;
        private bool originalItalicCheckState = false;

        // whether part of the editor controls are disabled depending on selected item(s)
        private bool controlsDisabledDueToConfirmable = false;

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
                return editorList;
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
                isTaskWork = btnSaveClose.Visible = btnClose.Visible = btnCancelTasks.Visible = tssTaskButtons.Visible = value;
            }
        }

        public ImageEditorForm(ISurface iSurface, bool outputMade)
        {
            editorList.Add(this);

            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            ManualLanguageApply = true;
            InitializeComponent();

            Shown += delegate
            {
                // Make sure the editor is placed on the same location as the last editor was on close
                WindowDetails thisForm = new WindowDetails(Handle);
                thisForm.WindowPlacement = editorConfiguration.GetEditorPlacement();
            };

            // init surface
            Surface = iSurface;
            // Intial "saved" flag for asking if the image needs to be save
            surface.Modified = !outputMade;

            updateUI();

            // Workaround: As the cursor is (mostly) selected on the surface a funny artifact is visible, this fixes it.
            //hideToolstripItems();
        }

        /// <summary>
        /// Remove the current surface
        /// </summary>
        private void RemoveSurface()
        {
            if (surface != null)
            {
                panel1.Controls.Remove(surface as Control);
                surface.Dispose();
                surface = null;
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
            surface = newSurface as Surface;
            panel1.Controls.Add(surface as Surface);
            Image backgroundForTransparency = GreenshotResources.getImage("Checkerboard.Image");
            surface.TransparencyBackgroundBrush = new TextureBrush(backgroundForTransparency, WrapMode.Tile);

            surface.MovingElementChanged += delegate
            {
                refreshEditorControls();
            };
            surface.DrawingModeChanged += surface_DrawingModeChanged;
            surface.SurfaceSizeChanged += SurfaceSizeChanged;
            surface.SurfaceMessage += SurfaceMessageReceived;
            surface.FieldAggregator.FieldChanged += FieldAggregatorFieldChanged;
            SurfaceSizeChanged(Surface, null);

            bindFieldControls();
            surface.DrawingMode = DrawingModes.Rect;
            refreshEditorControls();
            // Fix title
            if (surface != null && surface.CaptureDetails != null && surface.CaptureDetails.Title != null)
            {
                SetTitle(surface.CaptureDetails.Title);
            }
            WindowDetails.ToForeground(Handle);
        }

        private void updateUI()
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

            toolbarButtons = new GreenshotToolStripButton[] { btnCursor, btnRect, btnEllipse, btnText, btnLine, btnArrow, btnFreehand, btnHighlight, btnObfuscate, btnCrop };
            //toolbarDropDownButtons = new ToolStripDropDownButton[]{btnBlur, btnPixeliate, btnTextHighlighter, btnAreaHighlighter, btnMagnifier};

            // Workaround: for the MouseWheel event which doesn't get to the panel
            MouseWheel += PanelMouseWheel;
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

        /// <summary>
        /// This is the SufraceMessageEvent receiver which display a message in the status bar if the
        /// surface is exported. It also updates the title to represent the filename, if there is one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void SurfaceMessageReceived(object sender, SurfaceMessageEventArgs eventArgs)
        {
            string dateTime = DateTime.Now.ToLongTimeString();

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

        /// <summary>
        /// This is called when the size of the surface chances, used for resizing and displaying the size information
        /// </summary>
        /// <param name="source"></param>
        private void SurfaceSizeChanged(object sender, EventArgs e)
        {
            if (editorConfiguration.MatchSizeToCapture)
            {
                // Set editor's initial size to the size of the surface plus the size of the chrome
                Size imageSize = Surface.Image.Size;
                Size currentFormSize = Size;
                Size currentImageClientSize = panel1.ClientSize;
                int minimumFormWidth = 650;
                int minimumFormHeight = 530;
                int newWidth = Math.Max(minimumFormWidth, (currentFormSize.Width - currentImageClientSize.Width) + imageSize.Width);
                int newHeight = Math.Max(minimumFormHeight, (currentFormSize.Height - currentImageClientSize.Height) + imageSize.Height);
                Size = new Size(newWidth, newHeight);
            }
            UpdateTitle();
            ImageEditorFormResize(sender, new EventArgs());
        }

        private void ReloadConfiguration(object source, FileSystemEventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                // Even update language when needed
                ApplyLanguage();

                // Fix title
                if (surface != null && surface.CaptureDetails != null && surface.CaptureDetails.Title != null)
                {
                    SetTitle(surface.CaptureDetails.Title);
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
                return surface;
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
            surface.LastSaveFullPath = fullpath;

            if (fullpath == null)
            {
                return;
            }
            //updateStatusLabel(string.Format("Image saved to {0}.", fullpath), fileSavedStatusContextMenu);
            SetTitle(Path.GetFileName(fullpath));
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
            return surface.GetImageForExport();
        }

        public ICaptureDetails CaptureDetails
        {
            get { return surface.CaptureDetails; }
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
            surface.DrawingMode = DrawingModes.Ellipse;
            refreshFieldControls();
        }

        private void BtnCursorClick(object sender, EventArgs e)
        {
            surface.DrawingMode = DrawingModes.None;
            refreshFieldControls();
        }

        private void BtnRectClick(object sender, EventArgs e)
        {
            surface.DrawingMode = DrawingModes.Rect;
            refreshFieldControls();
        }

        private void BtnTextClick(object sender, EventArgs e)
        {
            surface.DrawingMode = DrawingModes.Text;
            refreshFieldControls();
        }

        private void BtnLineClick(object sender, EventArgs e)
        {
            surface.DrawingMode = DrawingModes.Line;
            refreshFieldControls();
        }

        private void BtnArrowClick(object sender, EventArgs e)
        {
            surface.DrawingMode = DrawingModes.Arrow;
            refreshFieldControls();
        }

        private void BtnCropClick(object sender, EventArgs e)
        {
            surface.DrawingMode = DrawingModes.Crop;
            refreshFieldControls();
        }

        private void BtnHighlightClick(object sender, EventArgs e)
        {
            surface.DrawingMode = DrawingModes.Highlight;
            refreshFieldControls();
        }

        private void BtnObfuscateClick(object sender, EventArgs e)
        {
            surface.DrawingMode = DrawingModes.Obfuscate;
            refreshFieldControls();
        }

        private void BtnFreehandClick(object sender, EventArgs e)
        {
            surface.DrawingMode = DrawingModes.Path;
            refreshFieldControls();
        }

        private void SetButtonChecked(ToolStripButton btn)
        {
            UncheckAllToolButtons();
            btn.Checked = true;
        }

        private void UncheckAllToolButtons()
        {
            if (toolbarButtons != null)
            {
                foreach (ToolStripButton butt in toolbarButtons)
                {
                    butt.Checked = false;
                }
            }
        }

        private void AddRectangleToolStripMenuItemClick(object sender, EventArgs e)
        {
            BtnRectClick(sender, e);
        }

        private void DrawFreehandToolStripMenuItemClick(object sender, EventArgs e)
        {
            BtnFreehandClick(sender, e);
        }

        private void AddEllipseToolStripMenuItemClick(object sender, EventArgs e)
        {
            BtnEllipseClick(sender, e);
        }

        private void AddTextBoxToolStripMenuItemClick(object sender, EventArgs e)
        {
            BtnTextClick(sender, e);
        }

        private void DrawLineToolStripMenuItemClick(object sender, EventArgs e)
        {
            BtnLineClick(sender, e);
        }

        private void DrawArrowToolStripMenuItemClick(object sender, EventArgs e)
        {
            BtnArrowClick(sender, e);
        }

        private void DrawHighlightToolStripMenuItemClick(object sender, EventArgs e)
        {
            BtnHighlightClick(sender, e);
        }

        private void BlurToolStripMenuItemClick(object sender, EventArgs e)
        {
            BtnObfuscateClick(sender, e);
        }

        private void RemoveObjectToolStripMenuItemClick(object sender, EventArgs e)
        {
            surface.RemoveSelectedElements();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            surface.SelectAllElements();
            surface.RemoveSelectedElements();
        }

        private void BtnDeleteClick(object sender, EventArgs e)
        {
            RemoveObjectToolStripMenuItemClick(sender, e);
        }

        #endregion drawing options

        #region copy&paste options

        private void CutToolStripMenuItemClick(object sender, EventArgs e)
        {
            surface.CutSelectedElements();
            updateClipboardSurfaceDependencies();
        }

        private void BtnCutClick(object sender, EventArgs e)
        {
            CutToolStripMenuItemClick(sender, e);
        }

        private void CopyToolStripMenuItemClick(object sender, EventArgs e)
        {
            surface.CopySelectedElements();
            updateClipboardSurfaceDependencies();
        }

        private void BtnCopyClick(object sender, EventArgs e)
        {
            CopyToolStripMenuItemClick(sender, e);
        }

        private void PasteToolStripMenuItemClick(object sender, EventArgs e)
        {
            surface.PasteElementFromClipboard();
            updateClipboardSurfaceDependencies();
        }

        private void BtnPasteClick(object sender, EventArgs e)
        {
            PasteToolStripMenuItemClick(sender, e);
        }

        private void UndoToolStripMenuItemClick(object sender, EventArgs e)
        {
            surface.Undo();
            updateUndoRedoSurfaceDependencies();
        }

        private void BtnUndoClick(object sender, EventArgs e)
        {
            UndoToolStripMenuItemClick(sender, e);
        }

        private void RedoToolStripMenuItemClick(object sender, EventArgs e)
        {
            surface.Redo();
            updateUndoRedoSurfaceDependencies();
        }

        private void BtnRedoClick(object sender, EventArgs e)
        {
            RedoToolStripMenuItemClick(sender, e);
        }

        private void DuplicateToolStripMenuItemClick(object sender, EventArgs e)
        {
            surface.DuplicateSelectedElements();
            updateClipboardSurfaceDependencies();
        }

        #endregion copy&paste options

        #region element properties

        private void UpOneLevelToolStripMenuItemClick(object sender, EventArgs e)
        {
            surface.PullElementsUp();
        }

        private void DownOneLevelToolStripMenuItemClick(object sender, EventArgs e)
        {
            surface.PushElementsDown();
        }

        private void UpToTopToolStripMenuItemClick(object sender, EventArgs e)
        {
            surface.PullElementsToTop();
        }

        private void DownToBottomToolStripMenuItemClick(object sender, EventArgs e)
        {
            surface.PushElementsToBottom();
        }

        #endregion element properties

        #region help

        private void HelpToolStripMenuItem1Click(object sender, EventArgs e)
        {
        }

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void PreferencesToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void BtnSettingsClick(object sender, EventArgs e)
        {
            PreferencesToolStripMenuItemClick(sender, e);
        }

        private void BtnHelpClick(object sender, EventArgs e)
        {
            HelpToolStripMenuItem1Click(sender, e);
        }

        #endregion help

        #region image editor event handlers

        private void ImageEditorFormActivated(object sender, EventArgs e)
        {
            updateClipboardSurfaceDependencies();
            updateUndoRedoSurfaceDependencies();

            btnSave.Enabled = File.Exists(surface.LastSaveFullPath);
        }

        private void ImageEditorFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!forceClose && surface.Modified)
            {
                if (!editorConfiguration.SuppressSaveDialogAtClose)
                {
                    // Make sure the editor is visible
                    WindowDetails.ToForeground(Handle);

                    MessageBoxButtons buttons = MessageBoxButtons.YesNoCancel;
                    // Dissallow "CANCEL" if the application needs to shutdown
                    if (e.CloseReason == CloseReason.ApplicationExitCall || e.CloseReason == CloseReason.WindowsShutDown || e.CloseReason == CloseReason.TaskManagerClosing)
                    {
                        buttons = MessageBoxButtons.YesNo;
                    }

                    DialogResult result = MessageBox.Show("Do you want the save the screenshot?", "Save image?", buttons, MessageBoxIcon.Question);

                    if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }

                    if (result == DialogResult.Yes)
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
            editorConfiguration.SetEditorPlacement(new WindowDetails(Handle).WindowPlacement);
            IniConfig.Save();

            // remove from the editor list
            editorList.Remove(this);
        }

        private void ImageEditorFormKeyDown(object sender, KeyEventArgs e)
        {
            // LOG.Debug("Got key event "+e.KeyCode + ", " + e.Modifiers);
            // avoid conflict with other shortcuts and
            // make sure there's no selected element claiming input focus
            if (e.Modifiers.Equals(Keys.None) && !surface.KeysLocked)
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
            if (!surface.KeysLocked)
            {
                return base.ProcessKeyPreview(ref msg);
            }
            return false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keys)
        {
            // disable default key handling if surface has requested a lock
            if (!surface.KeysLocked)
            {
                if (!surface.ProcessCmdKey(keys))
                {
                    return base.ProcessCmdKey(ref msg, keys);
                }
            }
            return false;
        }

        #endregion key handling

        #region helpers

        private void updateUndoRedoSurfaceDependencies()
        {
            if (surface == null)
            {
                return;
            }
            bool canUndo = surface.CanUndo;
            btnUndo.Enabled = canUndo;
            undoToolStripMenuItem.Enabled = canUndo;
            string undoAction = "";
            if (canUndo)
            {
                if (surface.UndoActionLanguageKey != LangKey.none)
                {
                    undoAction = Language.GetString(surface.UndoActionLanguageKey);
                }
            }
            string undoText = string.Format("Undo {0}", undoAction);
            btnUndo.Text = undoText;
            undoToolStripMenuItem.Text = undoText;

            bool canRedo = surface.CanRedo;
            btnRedo.Enabled = canRedo;
            redoToolStripMenuItem.Enabled = canRedo;
            string redoAction = "";
            if (canRedo)
            {
                if (surface.RedoActionLanguageKey != LangKey.none)
                {
                    redoAction = Language.GetString(surface.RedoActionLanguageKey);
                }
            }
            string redoText = string.Format("Redo {0}", redoAction);
            btnRedo.Text = redoText;
            redoToolStripMenuItem.Text = redoText;
        }

        private void updateClipboardSurfaceDependencies()
        {
            if (surface == null)
            {
                return;
            }
            // check dependencies for the Surface
            bool hasItems = surface.HasSelectedElements;
            bool actionAllowedForSelection = hasItems && !controlsDisabledDueToConfirmable;

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
            bool hasClipboard = ClipboardHelper.ContainsFormat(SUPPORTED_CLIPBOARD_FORMATS) || ClipboardHelper.ContainsImage();
            btnPaste.Enabled = hasClipboard && !controlsDisabledDueToConfirmable;
            pasteToolStripMenuItem.Enabled = hasClipboard && !controlsDisabledDueToConfirmable;
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

        private void clearStatusLabel()
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

        private void CopyPathMenuItemClick(object sender, EventArgs e)
        {
            ClipboardHelper.SetClipboardData(surface.LastSaveFullPath);
        }

        private void OpenDirectoryMenuItemClick(object sender, EventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo("explorer");
            psi.Arguments = Path.GetDirectoryName(surface.LastSaveFullPath);
            psi.UseShellExecute = false;
            Process p = new Process();
            p.StartInfo = psi;
            p.Start();
        }

        #endregion status label handling

        private void bindFieldControls()
        {
            new BidirectionalBinding(btnFillColor, "SelectedColor", surface.FieldAggregator.GetField(FieldType.FILL_COLOR), "Value", NotNullValidator.GetInstance());
            new BidirectionalBinding(btnLineColor, "SelectedColor", surface.FieldAggregator.GetField(FieldType.LINE_COLOR), "Value", NotNullValidator.GetInstance());
            new BidirectionalBinding(lineThicknessUpDown, "Value", surface.FieldAggregator.GetField(FieldType.LINE_THICKNESS), "Value", DecimalIntConverter.GetInstance(), NotNullValidator.GetInstance());
            new BidirectionalBinding(blurRadiusUpDown, "Value", surface.FieldAggregator.GetField(FieldType.BLUR_RADIUS), "Value", DecimalIntConverter.GetInstance(), NotNullValidator.GetInstance());
            new BidirectionalBinding(magnificationFactorUpDown, "Value", surface.FieldAggregator.GetField(FieldType.MAGNIFICATION_FACTOR), "Value", DecimalIntConverter.GetInstance(), NotNullValidator.GetInstance());
            new BidirectionalBinding(pixelSizeUpDown, "Value", surface.FieldAggregator.GetField(FieldType.PIXEL_SIZE), "Value", DecimalIntConverter.GetInstance(), NotNullValidator.GetInstance());
            new BidirectionalBinding(brightnessUpDown, "Value", surface.FieldAggregator.GetField(FieldType.BRIGHTNESS), "Value", DecimalDoublePercentageConverter.GetInstance(), NotNullValidator.GetInstance());
            new BidirectionalBinding(fontFamilyComboBox, "Text", surface.FieldAggregator.GetField(FieldType.FONT_FAMILY), "Value", NotNullValidator.GetInstance());
            new BidirectionalBinding(fontSizeUpDown, "Value", surface.FieldAggregator.GetField(FieldType.FONT_SIZE), "Value", DecimalFloatConverter.GetInstance(), NotNullValidator.GetInstance());
            new BidirectionalBinding(fontBoldButton, "Checked", surface.FieldAggregator.GetField(FieldType.FONT_BOLD), "Value", NotNullValidator.GetInstance());
            new BidirectionalBinding(fontItalicButton, "Checked", surface.FieldAggregator.GetField(FieldType.FONT_ITALIC), "Value", NotNullValidator.GetInstance());
            new BidirectionalBinding(textHorizontalAlignmentButton, "SelectedTag", surface.FieldAggregator.GetField(FieldType.TEXT_HORIZONTAL_ALIGNMENT), "Value", HorizontalAlignmentConverter.GetInstance(), NotNullValidator.GetInstance());
            new BidirectionalBinding(textVerticalAlignmentButton, "SelectedTag", surface.FieldAggregator.GetField(FieldType.TEXT_VERTICAL_ALIGNMENT), "Value", VerticalAlignmentConverter.GetInstance(), NotNullValidator.GetInstance());
            new BidirectionalBinding(shadowButton, "Checked", surface.FieldAggregator.GetField(FieldType.SHADOW), "Value", NotNullValidator.GetInstance());
            new BidirectionalBinding(previewQualityUpDown, "Value", surface.FieldAggregator.GetField(FieldType.PREVIEW_QUALITY), "Value", DecimalDoublePercentageConverter.GetInstance(), NotNullValidator.GetInstance());
            new BidirectionalBinding(obfuscateModeButton, "SelectedTag", surface.FieldAggregator.GetField(FieldType.PREPARED_FILTER_OBFUSCATE), "Value");
            new BidirectionalBinding(highlightModeButton, "SelectedTag", surface.FieldAggregator.GetField(FieldType.PREPARED_FILTER_HIGHLIGHT), "Value");
        }

        /// <summary>
        /// shows/hides field controls (2nd toolbar on top) depending on fields of selected elements
        /// </summary>
        private void refreshFieldControls()
        {
            propertiesToolStrip.SuspendLayout();

            if (surface.HasSelectedElements || surface.DrawingMode != DrawingModes.None)
            {
                toolStripSeparator5.Visible = true;
                FieldAggregator props = surface.FieldAggregator;
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
                hideToolstripItems();
            }

            propertiesToolStrip.ResumeLayout();
        }

        private void hideToolstripItems()
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
        private void refreshEditorControls()
        {
            FieldAggregator props = surface.FieldAggregator;
            // if a confirmable element is selected, we must disable most of the controls
            // since we demand confirmation or cancel for confirmable element
            if (props.HasFieldValue(FieldType.FLAGS) && ((FieldType.Flag)props.GetFieldValue(FieldType.FLAGS) & FieldType.Flag.CONFIRMABLE) == FieldType.Flag.CONFIRMABLE)
            {
                // disable most controls
                if (!controlsDisabledDueToConfirmable)
                {
                    ToolStripItemEndisabler.Disable(menuStrip1);
                    //ToolStripItemEndisabler.Disable(propertiesToolStrip);
                    ToolStripItemEndisabler.Disable(toolStrip2);
                    ToolStripItemEndisabler.Enable(closeToolStripMenuItem);
                    controlsDisabledDueToConfirmable = true;
                }
            }
            else if (controlsDisabledDueToConfirmable)
            {
                // re-enable disabled controls, confirmable element has either been confirmed or cancelled
                ToolStripItemEndisabler.Enable(menuStrip1);
                //ToolStripItemEndisabler.Enable(propertiesToolStrip);
                ToolStripItemEndisabler.Enable(toolStrip2);
                controlsDisabledDueToConfirmable = false;
            }

            // en/disable controls depending on whether an element is selected at all
            updateClipboardSurfaceDependencies();
            updateUndoRedoSurfaceDependencies();

            // en/disablearrage controls depending on hierarchy of selected elements
            bool actionAllowedForSelection = surface.HasSelectedElements && !controlsDisabledDueToConfirmable;
            bool push = actionAllowedForSelection && surface.CanPushSelectionDown();
            bool pull = actionAllowedForSelection && surface.CanPullSelectionUp();
            arrangeToolStripMenuItem.Enabled = (push || pull);
            if (arrangeToolStripMenuItem.Enabled)
            {
                upToTopToolStripMenuItem.Enabled = pull;
                upOneLevelToolStripMenuItem.Enabled = pull;
                downToBottomToolStripMenuItem.Enabled = push;
                downOneLevelToolStripMenuItem.Enabled = push;
            }

            // finally show/hide field controls depending on the fields of selected elements
            refreshFieldControls();
        }

        private void ArrowHeadsToolStripMenuItemClick(object sender, EventArgs e)
        {
            surface.FieldAggregator.GetField(FieldType.ARROWHEADS).Value = (ArrowContainer.ArrowHeadCombination)((ToolStripMenuItem)sender).Tag;
        }

        private void EditToolStripMenuItemClick(object sender, EventArgs e)
        {
            updateClipboardSurfaceDependencies();
            updateUndoRedoSurfaceDependencies();
        }

        private void FontPropertyChanged(object sender, EventArgs e)
        {
            // in case we forced another FontStyle before, reset it first.
            if (originalBoldCheckState != fontBoldButton.Checked) fontBoldButton.Checked = originalBoldCheckState;
            if (originalItalicCheckState != fontItalicButton.Checked) fontItalicButton.Checked = originalItalicCheckState;

            FontFamily fam = fontFamilyComboBox.FontFamily;

            bool boldAvailable = fam.IsStyleAvailable(FontStyle.Bold);
            if (!boldAvailable)
            {
                originalBoldCheckState = fontBoldButton.Checked;
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
                refreshFieldControls();
            }
        }

        private void FontBoldButtonClick(object sender, EventArgs e)
        {
            originalBoldCheckState = fontBoldButton.Checked;
        }

        private void FontItalicButtonClick(object sender, EventArgs e)
        {
            originalItalicCheckState = fontItalicButton.Checked;
        }

        private void ToolBarFocusableElementGotFocus(object sender, EventArgs e)
        {
            surface.KeysLocked = true;
        }

        private void ToolBarFocusableElementLostFocus(object sender, EventArgs e)
        {
            surface.KeysLocked = false;
        }

        private void SaveElementsToolStripMenuItemClick(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Greenshot templates (*.gst)|*.gst";
            saveFileDialog.FileName = FilenameHelper.GetFilenameWithoutExtensionFromPattern(coreConfiguration.OutputFileFilenamePattern, surface.CaptureDetails);
            DialogResult dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult.Equals(DialogResult.OK))
            {
                using (Stream streamWrite = File.OpenWrite(saveFileDialog.FileName))
                {
                    surface.SaveElementsToStream(streamWrite);
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
                    surface.LoadElementsFromStream(streamRead);
                }
                surface.Refresh();
            }
        }

        private void DestinationToolStripMenuItemClick(object sender, EventArgs e)
        {
            IDestination clickedDestination = null;
            if (sender is Control)
            {
                Control clickedControl = sender as Control;
                if (clickedControl.ContextMenuStrip != null)
                {
                    clickedControl.ContextMenuStrip.Show(Cursor.Position);
                    return;
                }
                clickedDestination = (IDestination)clickedControl.Tag;
            }
            else if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem clickedMenuItem = sender as ToolStripMenuItem;
                clickedDestination = (IDestination)clickedMenuItem.Tag;
            }
            if (clickedDestination != null)
            {
                ExportInformation exportInformation = clickedDestination.ExportCapture(true, surface, surface.CaptureDetails);
                if (exportInformation != null && exportInformation.ExportMade)
                {
                    surface.Modified = false;
                }
            }
        }

        protected void FilterPresetDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            refreshFieldControls();
            Invalidate(true);
        }

        private void SelectAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            surface.SelectAllElements();
        }

        private void BtnConfirmClick(object sender, EventArgs e)
        {
            surface.ConfirmSelectedConfirmableElements(true);
            refreshFieldControls();
        }

        private void BtnCancelClick(object sender, EventArgs e)
        {
            surface.ConfirmSelectedConfirmableElements(false);
            refreshFieldControls();
        }

        private void AutoCropToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (surface.AutoCrop())
            {
                refreshFieldControls();
            }
        }

        private void AddBorderToolStripMenuItemClick(object sender, EventArgs e)
        {
            surface.ApplyBitmapEffect(new BorderEffect());
            updateUndoRedoSurfaceDependencies();
        }

        /// <summary>
        /// This is used when the dropshadow button is used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddDropshadowToolStripMenuItemClick(object sender, EventArgs e)
        {
            DropShadowEffect dropShadowEffect = new DropShadowEffect();
            //DialogResult result = new DropShadowSettingsForm(dropShadowEffect).ShowDialog(this);
            //if (result == DialogResult.OK) {
            surface.ApplyBitmapEffect(dropShadowEffect);
            updateUndoRedoSurfaceDependencies();
            //}
        }

        /// <summary>
        /// Currently unused
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeToolStripMenuItemClick(object sender, EventArgs e)
        {
            ResizeEffect resizeEffect = new ResizeEffect(surface.Image.Width, surface.Image.Height, true);
            // DialogResult result = new ResizeSettingsForm(resizeEffect).ShowDialog(this);
            // if (result == DialogResult.OK) {
            surface.ApplyBitmapEffect(resizeEffect);
            updateUndoRedoSurfaceDependencies();
            //}
        }

        /// <summary>
        /// Call the torn edge effect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TornEdgesToolStripMenuItemClick(object sender, EventArgs e)
        {
            TornEdgeEffect tornEdgeEffect = new TornEdgeEffect();
            //DialogResult result = new TornEdgeSettingsForm(tornEdgeEffect).ShowDialog(this);
            //if (result == DialogResult.OK) {
            surface.ApplyBitmapEffect(tornEdgeEffect);
            updateUndoRedoSurfaceDependencies();
            //}
        }

        private void GrayscaleToolStripMenuItemClick(object sender, EventArgs e)
        {
            surface.ApplyBitmapEffect(new GrayscaleEffect());
            updateUndoRedoSurfaceDependencies();
        }

        private void ClearToolStripMenuItemClick(object sender, EventArgs e)
        {
            surface.Clear(Color.Transparent);
            updateUndoRedoSurfaceDependencies();
        }

        private void RotateCwToolstripButtonClick(object sender, EventArgs e)
        {
            surface.ApplyBitmapEffect(new RotateEffect(90));
            updateUndoRedoSurfaceDependencies();
        }

        private void RotateCcwToolstripButtonClick(object sender, EventArgs e)
        {
            surface.ApplyBitmapEffect(new RotateEffect(270));
            updateUndoRedoSurfaceDependencies();
        }

        private void InvertToolStripMenuItemClick(object sender, EventArgs e)
        {
            surface.ApplyBitmapEffect(new InvertEffect());
            updateUndoRedoSurfaceDependencies();
        }

        private void ImageEditorFormResize(object sender, EventArgs e)
        {
            if (Surface == null)
            {
                return;
            }
            Size imageSize = Surface.Image.Size;
            Size currentClientSize = panel1.ClientSize;
            var canvas = Surface as Control;
            Panel panel = (Panel)canvas.Parent;
            int offsetX = -panel.HorizontalScroll.Value;
            int offsetY = -panel.VerticalScroll.Value;
            if (canvas != null)
            {
                if (currentClientSize.Width > imageSize.Width)
                {
                    canvas.Left = offsetX + ((currentClientSize.Width - imageSize.Width) / 2);
                }
                else
                {
                    canvas.Left = offsetX + 0;
                }
            }
            if (canvas != null)
            {
                if (currentClientSize.Height > imageSize.Height)
                {
                    canvas.Top = offsetY + ((currentClientSize.Height - imageSize.Height) / 2);
                }
                else
                {
                    canvas.Top = offsetY + 0;
                }
            }
        }

        public void OnClipboardCopyRequested()
        {
            if (ClipboardCopyRequested != null)
            {
                using (Image img = surface.GetImageForExport())
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
                Image img = surface.GetImageForExport();
                ImageUploadRequested(img);
            }
        }

        public void OnImageSaveRequested()
        {
            if (ImageSaveRequested != null && File.Exists(surface.LastSaveFullPath))
            {
                using (Image img = surface.GetImageForExport())
                {
                    ImageSaveRequested(img, surface.LastSaveFullPath);
                }
            }
        }

        public void OnImageSaveAsRequested()
        {
            if (ImageSaveAsRequested != null)
            {
                using (Image img = surface.GetImageForExport())
                {
                    string newFilePath = ImageSaveAsRequested(img, surface.LastSaveFullPath);
                    if (!string.IsNullOrEmpty(newFilePath))
                    {
                        SetImagePath(newFilePath);
                    }
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
    }
}