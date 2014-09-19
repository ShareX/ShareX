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

namespace Greenshot {
	partial class ImageEditorForm {
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageEditorForm));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new GreenshotPlugin.Controls.NonJumpingPanel();
            this.toolStrip2 = new Greenshot.Controls.ToolStripEx();
            this.btnCursor = new GreenshotPlugin.Controls.GreenshotToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRect = new GreenshotPlugin.Controls.GreenshotToolStripButton();
            this.btnEllipse = new GreenshotPlugin.Controls.GreenshotToolStripButton();
            this.btnLine = new GreenshotPlugin.Controls.GreenshotToolStripButton();
            this.btnArrow = new GreenshotPlugin.Controls.GreenshotToolStripButton();
            this.btnFreehand = new GreenshotPlugin.Controls.GreenshotToolStripButton();
            this.btnText = new GreenshotPlugin.Controls.GreenshotToolStripButton();
            this.btnSpeechBubble = new GreenshotPlugin.Controls.GreenshotToolStripButton();
            this.btnStepLabel = new GreenshotPlugin.Controls.GreenshotToolStripButton();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.btnHighlight = new GreenshotPlugin.Controls.GreenshotToolStripButton();
            this.btnObfuscate = new GreenshotPlugin.Controls.GreenshotToolStripButton();
            this.toolStripSplitButton1 = new GreenshotPlugin.Controls.GreenshotToolStripDropDownButton();
            this.addBorderToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.addDropshadowToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.tornEdgesToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.grayscaleToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.invertToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCrop = new GreenshotPlugin.Controls.GreenshotToolStripButton();
            this.rotateCwToolstripButton = new GreenshotPlugin.Controls.GreenshotToolStripButton();
            this.rotateCcwToolstripButton = new GreenshotPlugin.Controls.GreenshotToolStripButton();
            this.btnResize = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1 = new Greenshot.Controls.MenuStripEx();
            this.editToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.copyToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.pasteToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.removeObjectToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.duplicateToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.selectAllToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arrangeToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.upToTopToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.upOneLevelToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.downOneLevelToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.downToBottomToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.autoCropToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.propertiesToolStrip = new Greenshot.Controls.ToolStripEx();
            this.btnSaveClose = new System.Windows.Forms.ToolStripButton();
            this.btnClose = new System.Windows.Forms.ToolStripButton();
            this.btnCancelTasks = new System.Windows.Forms.ToolStripButton();
            this.tssTaskButtons = new System.Windows.Forms.ToolStripSeparator();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnSaveAs = new System.Windows.Forms.ToolStripButton();
            this.btnClipboardCopy = new System.Windows.Forms.ToolStripButton();
            this.btnUploadImage = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnUndo = new System.Windows.Forms.ToolStripButton();
            this.btnRedo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCut = new GreenshotPlugin.Controls.GreenshotToolStripButton();
            this.btnCopy = new GreenshotPlugin.Controls.GreenshotToolStripButton();
            this.btnPaste = new GreenshotPlugin.Controls.GreenshotToolStripButton();
            this.btnDelete = new GreenshotPlugin.Controls.GreenshotToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.obfuscateModeButton = new Greenshot.Controls.BindableToolStripDropDownButton();
            this.pixelizeToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.blurToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.highlightModeButton = new Greenshot.Controls.BindableToolStripDropDownButton();
            this.textHighlightMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.areaHighlightMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.grayscaleHighlightMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.magnifyMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.btnFillColor = new Greenshot.Controls.ToolStripColorButton();
            this.btnLineColor = new Greenshot.Controls.ToolStripColorButton();
            this.lineThicknessLabel = new GreenshotPlugin.Controls.GreenshotToolStripLabel();
            this.lineThicknessUpDown = new Greenshot.Controls.ToolStripNumericUpDown();
            this.fontFamilyComboBox = new Greenshot.Controls.FontFamilyComboBox();
            this.fontSizeLabel = new GreenshotPlugin.Controls.GreenshotToolStripLabel();
            this.fontSizeUpDown = new Greenshot.Controls.ToolStripNumericUpDown();
            this.fontBoldButton = new Greenshot.Controls.BindableToolStripButton();
            this.fontItalicButton = new Greenshot.Controls.BindableToolStripButton();
            this.textHorizontalAlignmentButton = new Greenshot.Controls.BindableToolStripDropDownButton();
            this.alignLeftToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.alignCenterToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.alignRightToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.textVerticalAlignmentButton = new Greenshot.Controls.BindableToolStripDropDownButton();
            this.alignTopToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.alignMiddleToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.alignBottomToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.blurRadiusLabel = new GreenshotPlugin.Controls.GreenshotToolStripLabel();
            this.blurRadiusUpDown = new Greenshot.Controls.ToolStripNumericUpDown();
            this.brightnessLabel = new GreenshotPlugin.Controls.GreenshotToolStripLabel();
            this.brightnessUpDown = new Greenshot.Controls.ToolStripNumericUpDown();
            this.previewQualityLabel = new GreenshotPlugin.Controls.GreenshotToolStripLabel();
            this.previewQualityUpDown = new Greenshot.Controls.ToolStripNumericUpDown();
            this.magnificationFactorLabel = new GreenshotPlugin.Controls.GreenshotToolStripLabel();
            this.magnificationFactorUpDown = new Greenshot.Controls.ToolStripNumericUpDown();
            this.pixelSizeLabel = new GreenshotPlugin.Controls.GreenshotToolStripLabel();
            this.pixelSizeUpDown = new Greenshot.Controls.ToolStripNumericUpDown();
            this.arrowHeadsLabel = new GreenshotPlugin.Controls.GreenshotToolStripLabel();
            this.arrowHeadsDropDownButton = new GreenshotPlugin.Controls.GreenshotToolStripDropDownButton();
            this.arrowHeadStartMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.arrowHeadEndMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.arrowHeadBothMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.arrowHeadNoneMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.shadowButton = new Greenshot.Controls.BindableToolStripButton();
            this.btnConfirm = new Greenshot.Controls.BindableToolStripButton();
            this.btnCancel = new Greenshot.Controls.BindableToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new GreenshotPlugin.Controls.GreenshotToolStripMenuItem();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.LeftToolStripPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.propertiesToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.AutoScroll = true;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tableLayoutPanel1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(756, 434);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // toolStripContainer1.LeftToolStripPanel
            // 
            this.toolStripContainer1.LeftToolStripPanel.Controls.Add(this.toolStrip2);
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(785, 485);
            this.toolStripContainer1.TabIndex = 2;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.propertiesToolStrip);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 434F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(756, 434);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(750, 428);
            this.panel1.TabIndex = 2;
            // 
            // toolStrip2
            // 
            this.toolStrip2.ClickThrough = true;
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnCursor,
            this.toolStripSeparator1,
            this.btnRect,
            this.btnEllipse,
            this.btnLine,
            this.btnArrow,
            this.btnFreehand,
            this.btnText,
            this.btnSpeechBubble,
            this.btnStepLabel,
            this.toolStripSeparator14,
            this.btnHighlight,
            this.btnObfuscate,
            this.toolStripSplitButton1,
            this.toolStripSeparator13,
            this.btnCrop,
            this.rotateCwToolstripButton,
            this.rotateCcwToolstripButton,
            this.btnResize});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.toolStrip2.Size = new System.Drawing.Size(29, 434);
            this.toolStrip2.Stretch = true;
            this.toolStrip2.TabIndex = 0;
            // 
            // btnCursor
            // 
            this.btnCursor.CheckOnClick = true;
            this.btnCursor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCursor.Image = ((System.Drawing.Image)(resources.GetObject("btnCursor.Image")));
            this.btnCursor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCursor.LanguageKey = "editor_cursortool";
            this.btnCursor.Name = "btnCursor";
            this.btnCursor.Size = new System.Drawing.Size(22, 20);
            this.btnCursor.Text = "Selection Tool (ESC)";
            this.btnCursor.Click += new System.EventHandler(this.BtnCursorClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(30, 6);
            // 
            // btnRect
            // 
            this.btnRect.Checked = true;
            this.btnRect.CheckOnClick = true;
            this.btnRect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnRect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRect.Image = ((System.Drawing.Image)(resources.GetObject("btnRect.Image")));
            this.btnRect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRect.LanguageKey = "editor_drawrectangle";
            this.btnRect.Name = "btnRect";
            this.btnRect.Size = new System.Drawing.Size(30, 20);
            this.btnRect.Text = "Draw rectangle (R)";
            this.btnRect.Click += new System.EventHandler(this.BtnRectClick);
            // 
            // btnEllipse
            // 
            this.btnEllipse.CheckOnClick = true;
            this.btnEllipse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEllipse.Image = ((System.Drawing.Image)(resources.GetObject("btnEllipse.Image")));
            this.btnEllipse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEllipse.LanguageKey = "editor_drawellipse";
            this.btnEllipse.Name = "btnEllipse";
            this.btnEllipse.Size = new System.Drawing.Size(30, 20);
            this.btnEllipse.Text = "Draw ellipse (E)";
            this.btnEllipse.Click += new System.EventHandler(this.BtnEllipseClick);
            // 
            // btnLine
            // 
            this.btnLine.CheckOnClick = true;
            this.btnLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLine.Image = ((System.Drawing.Image)(resources.GetObject("btnLine.Image")));
            this.btnLine.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLine.LanguageKey = "editor_drawline";
            this.btnLine.Name = "btnLine";
            this.btnLine.Size = new System.Drawing.Size(30, 20);
            this.btnLine.Text = "Draw line (L)";
            this.btnLine.Click += new System.EventHandler(this.BtnLineClick);
            // 
            // btnArrow
            // 
            this.btnArrow.CheckOnClick = true;
            this.btnArrow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnArrow.Image = ((System.Drawing.Image)(resources.GetObject("btnArrow.Image")));
            this.btnArrow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnArrow.LanguageKey = "editor_drawarrow";
            this.btnArrow.Name = "btnArrow";
            this.btnArrow.Size = new System.Drawing.Size(30, 20);
            this.btnArrow.Text = "Draw arrow (A)";
            this.btnArrow.Click += new System.EventHandler(this.BtnArrowClick);
            // 
            // btnFreehand
            // 
            this.btnFreehand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFreehand.Image = ((System.Drawing.Image)(resources.GetObject("btnFreehand.Image")));
            this.btnFreehand.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFreehand.LanguageKey = "editor_drawfreehand";
            this.btnFreehand.Name = "btnFreehand";
            this.btnFreehand.Size = new System.Drawing.Size(30, 20);
            this.btnFreehand.Text = "Draw freehand (F)";
            this.btnFreehand.Click += new System.EventHandler(this.BtnFreehandClick);
            // 
            // btnText
            // 
            this.btnText.CheckOnClick = true;
            this.btnText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnText.Image = ((System.Drawing.Image)(resources.GetObject("btnText.Image")));
            this.btnText.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnText.LanguageKey = "editor_drawtextbox";
            this.btnText.Name = "btnText";
            this.btnText.Size = new System.Drawing.Size(30, 20);
            this.btnText.Text = "Add textbox (T)";
            this.btnText.Click += new System.EventHandler(this.BtnTextClick);
            // 
            // btnSpeechBubble
            // 
            this.btnSpeechBubble.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSpeechBubble.Image = ((System.Drawing.Image)(resources.GetObject("btnSpeechBubble.Image")));
            this.btnSpeechBubble.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSpeechBubble.Name = "btnSpeechBubble";
            this.btnSpeechBubble.Size = new System.Drawing.Size(30, 20);
            this.btnSpeechBubble.Text = "Add speech bubble";
            this.btnSpeechBubble.Click += new System.EventHandler(this.btnSpeechBubble_Click);
            // 
            // btnStepLabel
            // 
            this.btnStepLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStepLabel.Image = ((System.Drawing.Image)(resources.GetObject("btnStepLabel.Image")));
            this.btnStepLabel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStepLabel.Name = "btnStepLabel";
            this.btnStepLabel.Size = new System.Drawing.Size(30, 20);
            this.btnStepLabel.Text = "Add step label";
            this.btnStepLabel.Click += new System.EventHandler(this.btnStepLabel_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(30, 6);
            // 
            // btnHighlight
            // 
            this.btnHighlight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnHighlight.Image = ((System.Drawing.Image)(resources.GetObject("btnHighlight.Image")));
            this.btnHighlight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnHighlight.LanguageKey = "editor_drawhighlighter";
            this.btnHighlight.Name = "btnHighlight";
            this.btnHighlight.Size = new System.Drawing.Size(30, 20);
            this.btnHighlight.Text = "Highlight (H)";
            this.btnHighlight.Click += new System.EventHandler(this.BtnHighlightClick);
            // 
            // btnObfuscate
            // 
            this.btnObfuscate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnObfuscate.Image = ((System.Drawing.Image)(resources.GetObject("btnObfuscate.Image")));
            this.btnObfuscate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnObfuscate.LanguageKey = "editor_obfuscate";
            this.btnObfuscate.Name = "btnObfuscate";
            this.btnObfuscate.Size = new System.Drawing.Size(30, 20);
            this.btnObfuscate.Text = "Obfuscate (O)";
            this.btnObfuscate.Click += new System.EventHandler(this.BtnObfuscateClick);
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addBorderToolStripMenuItem,
            this.addDropshadowToolStripMenuItem,
            this.tornEdgesToolStripMenuItem,
            this.grayscaleToolStripMenuItem,
            this.invertToolStripMenuItem});
            this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.LanguageKey = "editor_effects";
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.ShowDropDownArrow = false;
            this.toolStripSplitButton1.Size = new System.Drawing.Size(30, 20);
            this.toolStripSplitButton1.Text = "Effects";
            // 
            // addBorderToolStripMenuItem
            // 
            this.addBorderToolStripMenuItem.LanguageKey = "editor_border";
            this.addBorderToolStripMenuItem.Name = "addBorderToolStripMenuItem";
            this.addBorderToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.addBorderToolStripMenuItem.Text = "Border";
            this.addBorderToolStripMenuItem.Click += new System.EventHandler(this.AddBorderToolStripMenuItemClick);
            // 
            // addDropshadowToolStripMenuItem
            // 
            this.addDropshadowToolStripMenuItem.LanguageKey = "editor_image_shadow";
            this.addDropshadowToolStripMenuItem.Name = "addDropshadowToolStripMenuItem";
            this.addDropshadowToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.addDropshadowToolStripMenuItem.Text = "Drop shadow";
            this.addDropshadowToolStripMenuItem.Click += new System.EventHandler(this.AddDropshadowToolStripMenuItemClick);
            // 
            // tornEdgesToolStripMenuItem
            // 
            this.tornEdgesToolStripMenuItem.LanguageKey = "editor_torn_edge";
            this.tornEdgesToolStripMenuItem.Name = "tornEdgesToolStripMenuItem";
            this.tornEdgesToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.tornEdgesToolStripMenuItem.Text = "Torn edge";
            this.tornEdgesToolStripMenuItem.Click += new System.EventHandler(this.TornEdgesToolStripMenuItemClick);
            // 
            // grayscaleToolStripMenuItem
            // 
            this.grayscaleToolStripMenuItem.LanguageKey = "editor_grayscale";
            this.grayscaleToolStripMenuItem.Name = "grayscaleToolStripMenuItem";
            this.grayscaleToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.grayscaleToolStripMenuItem.Text = "Grayscale";
            this.grayscaleToolStripMenuItem.Click += new System.EventHandler(this.GrayscaleToolStripMenuItemClick);
            // 
            // invertToolStripMenuItem
            // 
            this.invertToolStripMenuItem.LanguageKey = "editor_invert";
            this.invertToolStripMenuItem.Name = "invertToolStripMenuItem";
            this.invertToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.invertToolStripMenuItem.Text = "Invert";
            this.invertToolStripMenuItem.Click += new System.EventHandler(this.InvertToolStripMenuItemClick);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(30, 6);
            // 
            // btnCrop
            // 
            this.btnCrop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCrop.Image = ((System.Drawing.Image)(resources.GetObject("btnCrop.Image")));
            this.btnCrop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCrop.LanguageKey = "editor_crop";
            this.btnCrop.Name = "btnCrop";
            this.btnCrop.Size = new System.Drawing.Size(30, 20);
            this.btnCrop.Text = "Crop (C)";
            this.btnCrop.Click += new System.EventHandler(this.BtnCropClick);
            // 
            // rotateCwToolstripButton
            // 
            this.rotateCwToolstripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rotateCwToolstripButton.Image = ((System.Drawing.Image)(resources.GetObject("rotateCwToolstripButton.Image")));
            this.rotateCwToolstripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rotateCwToolstripButton.LanguageKey = "editor_rotatecw";
            this.rotateCwToolstripButton.Name = "rotateCwToolstripButton";
            this.rotateCwToolstripButton.Size = new System.Drawing.Size(30, 20);
            this.rotateCwToolstripButton.Text = "Rotate clockwise (Control + .)";
            this.rotateCwToolstripButton.Click += new System.EventHandler(this.RotateCwToolstripButtonClick);
            // 
            // rotateCcwToolstripButton
            // 
            this.rotateCcwToolstripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rotateCcwToolstripButton.Image = ((System.Drawing.Image)(resources.GetObject("rotateCcwToolstripButton.Image")));
            this.rotateCcwToolstripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rotateCcwToolstripButton.LanguageKey = "editor_rotateccw";
            this.rotateCcwToolstripButton.Name = "rotateCcwToolstripButton";
            this.rotateCcwToolstripButton.Size = new System.Drawing.Size(30, 20);
            this.rotateCcwToolstripButton.Text = "Rotate counter clockwise (Control + ,)";
            this.rotateCcwToolstripButton.Click += new System.EventHandler(this.RotateCcwToolstripButtonClick);
            // 
            // btnResize
            // 
            this.btnResize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnResize.Image = ((System.Drawing.Image)(resources.GetObject("btnResize.Image")));
            this.btnResize.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnResize.Name = "btnResize";
            this.btnResize.Size = new System.Drawing.Size(30, 20);
            this.btnResize.Text = "Resize";
            this.btnResize.Click += new System.EventHandler(this.btnResize_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ClickThrough = true;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(785, 24);
            this.menuStrip1.TabIndex = 1;
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator15,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.removeObjectToolStripMenuItem,
            this.toolStripSeparator4,
            this.duplicateToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.arrangeToolStripMenuItem,
            this.toolStripSeparator12,
            this.autoCropToolStripMenuItem});
            this.editToolStripMenuItem.LanguageKey = "editor_edit";
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.EditToolStripMenuItemClick);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Enabled = false;
            this.undoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("undoToolStripMenuItem.Image")));
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.UndoToolStripMenuItemClick);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Enabled = false;
            this.redoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("redoToolStripMenuItem.Image")));
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.RedoToolStripMenuItemClick);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(255, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Enabled = false;
            this.cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripMenuItem.Image")));
            this.cutToolStripMenuItem.LanguageKey = "editor_cuttoclipboard";
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.CutToolStripMenuItemClick);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Enabled = false;
            this.copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem.Image")));
            this.copyToolStripMenuItem.LanguageKey = "editor_copytoclipboard";
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItemClick);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Enabled = false;
            this.pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripMenuItem.Image")));
            this.pasteToolStripMenuItem.LanguageKey = "editor_pastefromclipboard";
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.PasteToolStripMenuItemClick);
            // 
            // removeObjectToolStripMenuItem
            // 
            this.removeObjectToolStripMenuItem.Enabled = false;
            this.removeObjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeObjectToolStripMenuItem.Image")));
            this.removeObjectToolStripMenuItem.LanguageKey = "editor_deleteelement";
            this.removeObjectToolStripMenuItem.Name = "removeObjectToolStripMenuItem";
            this.removeObjectToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.removeObjectToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.removeObjectToolStripMenuItem.Text = "Delete";
            this.removeObjectToolStripMenuItem.Click += new System.EventHandler(this.RemoveObjectToolStripMenuItemClick);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(255, 6);
            // 
            // duplicateToolStripMenuItem
            // 
            this.duplicateToolStripMenuItem.Enabled = false;
            this.duplicateToolStripMenuItem.LanguageKey = "editor_duplicate";
            this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
            this.duplicateToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.duplicateToolStripMenuItem.Text = "Duplicate selected element";
            this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.DuplicateToolStripMenuItemClick);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.LanguageKey = "editor_selectall";
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.selectAllToolStripMenuItem.Text = "Select all";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.SelectAllToolStripMenuItemClick);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.clearToolStripMenuItem.Text = "Delete all";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // arrangeToolStripMenuItem
            // 
            this.arrangeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.upToTopToolStripMenuItem,
            this.upOneLevelToolStripMenuItem,
            this.downOneLevelToolStripMenuItem,
            this.downToBottomToolStripMenuItem});
            this.arrangeToolStripMenuItem.Enabled = false;
            this.arrangeToolStripMenuItem.LanguageKey = "editor_arrange";
            this.arrangeToolStripMenuItem.Name = "arrangeToolStripMenuItem";
            this.arrangeToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.arrangeToolStripMenuItem.Text = "Arrange";
            // 
            // upToTopToolStripMenuItem
            // 
            this.upToTopToolStripMenuItem.Enabled = false;
            this.upToTopToolStripMenuItem.LanguageKey = "editor_uptotop";
            this.upToTopToolStripMenuItem.Name = "upToTopToolStripMenuItem";
            this.upToTopToolStripMenuItem.ShortcutKeyDisplayString = "Home";
            this.upToTopToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.upToTopToolStripMenuItem.Text = "Up to top";
            this.upToTopToolStripMenuItem.Click += new System.EventHandler(this.UpToTopToolStripMenuItemClick);
            // 
            // upOneLevelToolStripMenuItem
            // 
            this.upOneLevelToolStripMenuItem.Enabled = false;
            this.upOneLevelToolStripMenuItem.LanguageKey = "editor_uponelevel";
            this.upOneLevelToolStripMenuItem.Name = "upOneLevelToolStripMenuItem";
            this.upOneLevelToolStripMenuItem.ShortcutKeyDisplayString = "PgUp";
            this.upOneLevelToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.upOneLevelToolStripMenuItem.Text = "Up one level";
            this.upOneLevelToolStripMenuItem.Click += new System.EventHandler(this.UpOneLevelToolStripMenuItemClick);
            // 
            // downOneLevelToolStripMenuItem
            // 
            this.downOneLevelToolStripMenuItem.Enabled = false;
            this.downOneLevelToolStripMenuItem.LanguageKey = "editor_downonelevel";
            this.downOneLevelToolStripMenuItem.Name = "downOneLevelToolStripMenuItem";
            this.downOneLevelToolStripMenuItem.ShortcutKeyDisplayString = "PgDn";
            this.downOneLevelToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.downOneLevelToolStripMenuItem.Text = "Down one level";
            this.downOneLevelToolStripMenuItem.Click += new System.EventHandler(this.DownOneLevelToolStripMenuItemClick);
            // 
            // downToBottomToolStripMenuItem
            // 
            this.downToBottomToolStripMenuItem.Enabled = false;
            this.downToBottomToolStripMenuItem.LanguageKey = "editor_downtobottom";
            this.downToBottomToolStripMenuItem.Name = "downToBottomToolStripMenuItem";
            this.downToBottomToolStripMenuItem.ShortcutKeyDisplayString = "End";
            this.downToBottomToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.downToBottomToolStripMenuItem.Text = "Down to bottom";
            this.downToBottomToolStripMenuItem.Click += new System.EventHandler(this.DownToBottomToolStripMenuItemClick);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(255, 6);
            // 
            // autoCropToolStripMenuItem
            // 
            this.autoCropToolStripMenuItem.LanguageKey = "editor_autocrop";
            this.autoCropToolStripMenuItem.Name = "autoCropToolStripMenuItem";
            this.autoCropToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.autoCropToolStripMenuItem.Text = "Auto crop";
            this.autoCropToolStripMenuItem.Click += new System.EventHandler(this.AutoCropToolStripMenuItemClick);
            // 
            // propertiesToolStrip
            // 
            this.propertiesToolStrip.ClickThrough = true;
            this.propertiesToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.propertiesToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.propertiesToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSaveClose,
            this.btnClose,
            this.btnCancelTasks,
            this.tssTaskButtons,
            this.btnSave,
            this.btnSaveAs,
            this.btnClipboardCopy,
            this.btnUploadImage,
            this.toolStripSeparator2,
            this.btnUndo,
            this.btnRedo,
            this.toolStripSeparator3,
            this.btnCut,
            this.btnCopy,
            this.btnPaste,
            this.btnDelete,
            this.toolStripSeparator5,
            this.obfuscateModeButton,
            this.highlightModeButton,
            this.btnFillColor,
            this.btnLineColor,
            this.lineThicknessLabel,
            this.lineThicknessUpDown,
            this.fontFamilyComboBox,
            this.fontSizeLabel,
            this.fontSizeUpDown,
            this.fontBoldButton,
            this.fontItalicButton,
            this.textHorizontalAlignmentButton,
            this.textVerticalAlignmentButton,
            this.blurRadiusLabel,
            this.blurRadiusUpDown,
            this.brightnessLabel,
            this.brightnessUpDown,
            this.previewQualityLabel,
            this.previewQualityUpDown,
            this.magnificationFactorLabel,
            this.magnificationFactorUpDown,
            this.pixelSizeLabel,
            this.pixelSizeUpDown,
            this.arrowHeadsLabel,
            this.arrowHeadsDropDownButton,
            this.shadowButton,
            this.btnConfirm,
            this.btnCancel});
            this.propertiesToolStrip.Location = new System.Drawing.Point(0, 24);
            this.propertiesToolStrip.MinimumSize = new System.Drawing.Size(0, 27);
            this.propertiesToolStrip.Name = "propertiesToolStrip";
            this.propertiesToolStrip.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
            this.propertiesToolStrip.Size = new System.Drawing.Size(785, 27);
            this.propertiesToolStrip.Stretch = true;
            this.propertiesToolStrip.TabIndex = 0;
            // 
            // btnSaveClose
            // 
            this.btnSaveClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSaveClose.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveClose.Image")));
            this.btnSaveClose.Name = "btnSaveClose";
            this.btnSaveClose.Size = new System.Drawing.Size(23, 24);
            this.btnSaveClose.Text = "Save and close";
            this.btnSaveClose.Click += new System.EventHandler(this.btnSaveClose_Click);
            // 
            // btnClose
            // 
            this.btnClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(23, 24);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCancelTasks
            // 
            this.btnCancelTasks.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCancelTasks.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelTasks.Image")));
            this.btnCancelTasks.Name = "btnCancelTasks";
            this.btnCancelTasks.Size = new System.Drawing.Size(23, 24);
            this.btnCancelTasks.Text = "Close and cancel tasks";
            this.btnCancelTasks.Click += new System.EventHandler(this.btnCancelTasks_Click);
            // 
            // tssTaskButtons
            // 
            this.tssTaskButtons.Name = "tssTaskButtons";
            this.tssTaskButtons.Size = new System.Drawing.Size(6, 27);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 24);
            this.btnSave.Text = "Save image";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveAs.Image")));
            this.btnSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(23, 24);
            this.btnSaveAs.Text = "Save image as...";
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // btnClipboardCopy
            // 
            this.btnClipboardCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClipboardCopy.Image = ((System.Drawing.Image)(resources.GetObject("btnClipboardCopy.Image")));
            this.btnClipboardCopy.Name = "btnClipboardCopy";
            this.btnClipboardCopy.Size = new System.Drawing.Size(23, 24);
            this.btnClipboardCopy.Text = "Copy to clipboard";
            this.btnClipboardCopy.Click += new System.EventHandler(this.btnClipboardCopy_Click);
            // 
            // btnUploadImage
            // 
            this.btnUploadImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUploadImage.Image = ((System.Drawing.Image)(resources.GetObject("btnUploadImage.Image")));
            this.btnUploadImage.Name = "btnUploadImage";
            this.btnUploadImage.Size = new System.Drawing.Size(23, 24);
            this.btnUploadImage.Text = "Upload image";
            this.btnUploadImage.Click += new System.EventHandler(this.btnUploadImage_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // btnUndo
            // 
            this.btnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUndo.Enabled = false;
            this.btnUndo.Image = ((System.Drawing.Image)(resources.GetObject("btnUndo.Image")));
            this.btnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(23, 24);
            this.btnUndo.Click += new System.EventHandler(this.BtnUndoClick);
            // 
            // btnRedo
            // 
            this.btnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRedo.Enabled = false;
            this.btnRedo.Image = ((System.Drawing.Image)(resources.GetObject("btnRedo.Image")));
            this.btnRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Size = new System.Drawing.Size(23, 24);
            this.btnRedo.Click += new System.EventHandler(this.BtnRedoClick);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // btnCut
            // 
            this.btnCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCut.Enabled = false;
            this.btnCut.Image = ((System.Drawing.Image)(resources.GetObject("btnCut.Image")));
            this.btnCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCut.LanguageKey = "editor_cuttoclipboard";
            this.btnCut.Name = "btnCut";
            this.btnCut.Size = new System.Drawing.Size(23, 24);
            this.btnCut.Text = "Cut";
            this.btnCut.Click += new System.EventHandler(this.BtnCutClick);
            // 
            // btnCopy
            // 
            this.btnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCopy.Enabled = false;
            this.btnCopy.Image = ((System.Drawing.Image)(resources.GetObject("btnCopy.Image")));
            this.btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCopy.LanguageKey = "editor_copytoclipboard";
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(23, 24);
            this.btnCopy.Text = "Copy";
            this.btnCopy.Click += new System.EventHandler(this.BtnCopyClick);
            // 
            // btnPaste
            // 
            this.btnPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPaste.Enabled = false;
            this.btnPaste.Image = ((System.Drawing.Image)(resources.GetObject("btnPaste.Image")));
            this.btnPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPaste.LanguageKey = "editor_pastefromclipboard";
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(23, 24);
            this.btnPaste.Text = "Paste";
            this.btnPaste.Click += new System.EventHandler(this.BtnPasteClick);
            // 
            // btnDelete
            // 
            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDelete.Enabled = false;
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.LanguageKey = "editor_deleteelement";
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(23, 24);
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.BtnDeleteClick);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 27);
            // 
            // obfuscateModeButton
            // 
            this.obfuscateModeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.obfuscateModeButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pixelizeToolStripMenuItem,
            this.blurToolStripMenuItem});
            this.obfuscateModeButton.Image = ((System.Drawing.Image)(resources.GetObject("obfuscateModeButton.Image")));
            this.obfuscateModeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.obfuscateModeButton.LanguageKey = "editor_obfuscate_mode";
            this.obfuscateModeButton.Name = "obfuscateModeButton";
            this.obfuscateModeButton.SelectedTag = Greenshot.Drawing.FilterContainer.PreparedFilter.BLUR;
            this.obfuscateModeButton.Size = new System.Drawing.Size(29, 24);
            this.obfuscateModeButton.Tag = Greenshot.Drawing.FilterContainer.PreparedFilter.BLUR;
            this.obfuscateModeButton.Text = "Obfuscation mode";
            // 
            // pixelizeToolStripMenuItem
            // 
            this.pixelizeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pixelizeToolStripMenuItem.Image")));
            this.pixelizeToolStripMenuItem.LanguageKey = "editor_obfuscate_pixelize";
            this.pixelizeToolStripMenuItem.Name = "pixelizeToolStripMenuItem";
            this.pixelizeToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.pixelizeToolStripMenuItem.Tag = Greenshot.Drawing.FilterContainer.PreparedFilter.PIXELIZE;
            this.pixelizeToolStripMenuItem.Text = "Pixelize";
            // 
            // blurToolStripMenuItem
            // 
            this.blurToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("blurToolStripMenuItem.Image")));
            this.blurToolStripMenuItem.LanguageKey = "editor_obfuscate_blur";
            this.blurToolStripMenuItem.Name = "blurToolStripMenuItem";
            this.blurToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.blurToolStripMenuItem.Tag = Greenshot.Drawing.FilterContainer.PreparedFilter.BLUR;
            this.blurToolStripMenuItem.Text = "Blur";
            // 
            // highlightModeButton
            // 
            this.highlightModeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.highlightModeButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textHighlightMenuItem,
            this.areaHighlightMenuItem,
            this.grayscaleHighlightMenuItem,
            this.magnifyMenuItem});
            this.highlightModeButton.Image = ((System.Drawing.Image)(resources.GetObject("highlightModeButton.Image")));
            this.highlightModeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.highlightModeButton.LanguageKey = "editor_highlight_mode";
            this.highlightModeButton.Name = "highlightModeButton";
            this.highlightModeButton.SelectedTag = Greenshot.Drawing.FilterContainer.PreparedFilter.TEXT_HIGHTLIGHT;
            this.highlightModeButton.Size = new System.Drawing.Size(29, 24);
            this.highlightModeButton.Tag = Greenshot.Drawing.FilterContainer.PreparedFilter.TEXT_HIGHTLIGHT;
            this.highlightModeButton.Text = "Highlight mode";
            // 
            // textHighlightMenuItem
            // 
            this.textHighlightMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("textHighlightMenuItem.Image")));
            this.textHighlightMenuItem.LanguageKey = "editor_highlight_text";
            this.textHighlightMenuItem.Name = "textHighlightMenuItem";
            this.textHighlightMenuItem.Size = new System.Drawing.Size(149, 22);
            this.textHighlightMenuItem.Tag = Greenshot.Drawing.FilterContainer.PreparedFilter.TEXT_HIGHTLIGHT;
            this.textHighlightMenuItem.Text = "Highlight text";
            // 
            // areaHighlightMenuItem
            // 
            this.areaHighlightMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("areaHighlightMenuItem.Image")));
            this.areaHighlightMenuItem.LanguageKey = "editor_highlight_area";
            this.areaHighlightMenuItem.Name = "areaHighlightMenuItem";
            this.areaHighlightMenuItem.Size = new System.Drawing.Size(149, 22);
            this.areaHighlightMenuItem.Tag = Greenshot.Drawing.FilterContainer.PreparedFilter.AREA_HIGHLIGHT;
            this.areaHighlightMenuItem.Text = "Highlight area";
            // 
            // grayscaleHighlightMenuItem
            // 
            this.grayscaleHighlightMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("grayscaleHighlightMenuItem.Image")));
            this.grayscaleHighlightMenuItem.LanguageKey = "editor_highlight_grayscale";
            this.grayscaleHighlightMenuItem.Name = "grayscaleHighlightMenuItem";
            this.grayscaleHighlightMenuItem.Size = new System.Drawing.Size(149, 22);
            this.grayscaleHighlightMenuItem.Tag = Greenshot.Drawing.FilterContainer.PreparedFilter.GRAYSCALE;
            this.grayscaleHighlightMenuItem.Text = "Grayscale";
            // 
            // magnifyMenuItem
            // 
            this.magnifyMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("magnifyMenuItem.Image")));
            this.magnifyMenuItem.LanguageKey = "editor_highlight_magnify";
            this.magnifyMenuItem.Name = "magnifyMenuItem";
            this.magnifyMenuItem.Size = new System.Drawing.Size(149, 22);
            this.magnifyMenuItem.Tag = Greenshot.Drawing.FilterContainer.PreparedFilter.MAGNIFICATION;
            this.magnifyMenuItem.Text = "Magnify";
            // 
            // btnFillColor
            // 
            this.btnFillColor.AutoSize = false;
            this.btnFillColor.BackColor = System.Drawing.Color.Transparent;
            this.btnFillColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFillColor.Image = ((System.Drawing.Image)(resources.GetObject("btnFillColor.Image")));
            this.btnFillColor.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnFillColor.LanguageKey = "editor_backcolor";
            this.btnFillColor.Margin = new System.Windows.Forms.Padding(0);
            this.btnFillColor.Name = "btnFillColor";
            this.btnFillColor.SelectedColor = System.Drawing.Color.Transparent;
            this.btnFillColor.Size = new System.Drawing.Size(23, 24);
            this.btnFillColor.Text = "Fill color";
            // 
            // btnLineColor
            // 
            this.btnLineColor.AutoSize = false;
            this.btnLineColor.BackColor = System.Drawing.Color.Transparent;
            this.btnLineColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLineColor.Image = ((System.Drawing.Image)(resources.GetObject("btnLineColor.Image")));
            this.btnLineColor.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnLineColor.LanguageKey = "editor_forecolor";
            this.btnLineColor.Name = "btnLineColor";
            this.btnLineColor.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(222)))), ((int)(((byte)(250)))));
            this.btnLineColor.Size = new System.Drawing.Size(23, 24);
            this.btnLineColor.Text = "Line color";
            // 
            // lineThicknessLabel
            // 
            this.lineThicknessLabel.LanguageKey = "editor_thickness";
            this.lineThicknessLabel.Name = "lineThicknessLabel";
            this.lineThicknessLabel.Size = new System.Drawing.Size(81, 24);
            this.lineThicknessLabel.Text = "Line thickness";
            // 
            // lineThicknessUpDown
            // 
            this.lineThicknessUpDown.DecimalPlaces = 0;
            this.lineThicknessUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.lineThicknessUpDown.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.lineThicknessUpDown.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.lineThicknessUpDown.Name = "lineThicknessUpDown";
            this.lineThicknessUpDown.Size = new System.Drawing.Size(41, 24);
            this.lineThicknessUpDown.Text = "0";
            this.lineThicknessUpDown.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.lineThicknessUpDown.GotFocus += new System.EventHandler(this.ToolBarFocusableElementGotFocus);
            this.lineThicknessUpDown.LostFocus += new System.EventHandler(this.ToolBarFocusableElementLostFocus);
            // 
            // fontFamilyComboBox
            // 
            this.fontFamilyComboBox.AutoSize = false;
            this.fontFamilyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fontFamilyComboBox.MaxDropDownItems = 20;
            this.fontFamilyComboBox.Name = "fontFamilyComboBox";
            this.fontFamilyComboBox.Size = new System.Drawing.Size(200, 23);
            this.fontFamilyComboBox.Text = "Aharoni";
            this.fontFamilyComboBox.GotFocus += new System.EventHandler(this.ToolBarFocusableElementGotFocus);
            this.fontFamilyComboBox.LostFocus += new System.EventHandler(this.ToolBarFocusableElementLostFocus);
            // 
            // fontSizeLabel
            // 
            this.fontSizeLabel.LanguageKey = "editor_fontsize";
            this.fontSizeLabel.Name = "fontSizeLabel";
            this.fontSizeLabel.Size = new System.Drawing.Size(27, 15);
            this.fontSizeLabel.Text = "Size";
            // 
            // fontSizeUpDown
            // 
            this.fontSizeUpDown.DecimalPlaces = 0;
            this.fontSizeUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.fontSizeUpDown.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.fontSizeUpDown.Minimum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.fontSizeUpDown.Name = "fontSizeUpDown";
            this.fontSizeUpDown.Size = new System.Drawing.Size(41, 23);
            this.fontSizeUpDown.Text = "12";
            this.fontSizeUpDown.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.fontSizeUpDown.GotFocus += new System.EventHandler(this.ToolBarFocusableElementGotFocus);
            this.fontSizeUpDown.LostFocus += new System.EventHandler(this.ToolBarFocusableElementLostFocus);
            // 
            // fontBoldButton
            // 
            this.fontBoldButton.CheckOnClick = true;
            this.fontBoldButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.fontBoldButton.Image = ((System.Drawing.Image)(resources.GetObject("fontBoldButton.Image")));
            this.fontBoldButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fontBoldButton.LanguageKey = "editor_bold";
            this.fontBoldButton.Name = "fontBoldButton";
            this.fontBoldButton.Size = new System.Drawing.Size(23, 20);
            this.fontBoldButton.Text = "Bold";
            this.fontBoldButton.Click += new System.EventHandler(this.FontBoldButtonClick);
            // 
            // fontItalicButton
            // 
            this.fontItalicButton.CheckOnClick = true;
            this.fontItalicButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.fontItalicButton.Image = ((System.Drawing.Image)(resources.GetObject("fontItalicButton.Image")));
            this.fontItalicButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fontItalicButton.LanguageKey = "editor_italic";
            this.fontItalicButton.Name = "fontItalicButton";
            this.fontItalicButton.Size = new System.Drawing.Size(23, 20);
            this.fontItalicButton.Text = "Italic";
            this.fontItalicButton.Click += new System.EventHandler(this.FontItalicButtonClick);
            // 
            // textHorizontalAlignmentButton
            // 
            this.textHorizontalAlignmentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.textHorizontalAlignmentButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alignLeftToolStripMenuItem,
            this.alignCenterToolStripMenuItem,
            this.alignRightToolStripMenuItem});
            this.textHorizontalAlignmentButton.Image = ((System.Drawing.Image)(resources.GetObject("textHorizontalAlignmentButton.Image")));
            this.textHorizontalAlignmentButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.textHorizontalAlignmentButton.LanguageKey = "editor_align_horizontal";
            this.textHorizontalAlignmentButton.Name = "textHorizontalAlignmentButton";
            this.textHorizontalAlignmentButton.SelectedTag = System.Windows.Forms.HorizontalAlignment.Center;
            this.textHorizontalAlignmentButton.Size = new System.Drawing.Size(29, 20);
            this.textHorizontalAlignmentButton.Tag = System.Windows.Forms.HorizontalAlignment.Center;
            this.textHorizontalAlignmentButton.Text = "Horizontal alignment";
            // 
            // alignLeftToolStripMenuItem
            // 
            this.alignLeftToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alignLeftToolStripMenuItem.Image")));
            this.alignLeftToolStripMenuItem.LanguageKey = "editor_align_left";
            this.alignLeftToolStripMenuItem.Name = "alignLeftToolStripMenuItem";
            this.alignLeftToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.alignLeftToolStripMenuItem.Tag = System.Windows.Forms.HorizontalAlignment.Left;
            this.alignLeftToolStripMenuItem.Text = "Left";
            // 
            // alignCenterToolStripMenuItem
            // 
            this.alignCenterToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alignCenterToolStripMenuItem.Image")));
            this.alignCenterToolStripMenuItem.LanguageKey = "editor_align_center";
            this.alignCenterToolStripMenuItem.Name = "alignCenterToolStripMenuItem";
            this.alignCenterToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.alignCenterToolStripMenuItem.Tag = System.Windows.Forms.HorizontalAlignment.Center;
            this.alignCenterToolStripMenuItem.Text = "Center";
            // 
            // alignRightToolStripMenuItem
            // 
            this.alignRightToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alignRightToolStripMenuItem.Image")));
            this.alignRightToolStripMenuItem.LanguageKey = "editor_align_right";
            this.alignRightToolStripMenuItem.Name = "alignRightToolStripMenuItem";
            this.alignRightToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.alignRightToolStripMenuItem.Tag = System.Windows.Forms.HorizontalAlignment.Right;
            this.alignRightToolStripMenuItem.Text = "Right";
            // 
            // textVerticalAlignmentButton
            // 
            this.textVerticalAlignmentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.textVerticalAlignmentButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alignTopToolStripMenuItem,
            this.alignMiddleToolStripMenuItem,
            this.alignBottomToolStripMenuItem});
            this.textVerticalAlignmentButton.Image = ((System.Drawing.Image)(resources.GetObject("textVerticalAlignmentButton.Image")));
            this.textVerticalAlignmentButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.textVerticalAlignmentButton.LanguageKey = "editor_align_vertical";
            this.textVerticalAlignmentButton.Name = "textVerticalAlignmentButton";
            this.textVerticalAlignmentButton.SelectedTag = Greenshot.Plugin.VerticalAlignment.CENTER;
            this.textVerticalAlignmentButton.Size = new System.Drawing.Size(29, 20);
            this.textVerticalAlignmentButton.Tag = Greenshot.Plugin.VerticalAlignment.CENTER;
            this.textVerticalAlignmentButton.Text = "Vertical alignment";
            // 
            // alignTopToolStripMenuItem
            // 
            this.alignTopToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alignTopToolStripMenuItem.Image")));
            this.alignTopToolStripMenuItem.LanguageKey = "editor_align_top";
            this.alignTopToolStripMenuItem.Name = "alignTopToolStripMenuItem";
            this.alignTopToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.alignTopToolStripMenuItem.Tag = Greenshot.Plugin.VerticalAlignment.TOP;
            this.alignTopToolStripMenuItem.Text = "Top";
            // 
            // alignMiddleToolStripMenuItem
            // 
            this.alignMiddleToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alignMiddleToolStripMenuItem.Image")));
            this.alignMiddleToolStripMenuItem.LanguageKey = "editor_align_middle";
            this.alignMiddleToolStripMenuItem.Name = "alignMiddleToolStripMenuItem";
            this.alignMiddleToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.alignMiddleToolStripMenuItem.Tag = Greenshot.Plugin.VerticalAlignment.CENTER;
            this.alignMiddleToolStripMenuItem.Text = "Middle";
            // 
            // alignBottomToolStripMenuItem
            // 
            this.alignBottomToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alignBottomToolStripMenuItem.Image")));
            this.alignBottomToolStripMenuItem.LanguageKey = "editor_align_bottom";
            this.alignBottomToolStripMenuItem.Name = "alignBottomToolStripMenuItem";
            this.alignBottomToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.alignBottomToolStripMenuItem.Tag = Greenshot.Plugin.VerticalAlignment.BOTTOM;
            this.alignBottomToolStripMenuItem.Text = "Bottom";
            // 
            // blurRadiusLabel
            // 
            this.blurRadiusLabel.LanguageKey = "editor_blur_radius";
            this.blurRadiusLabel.Name = "blurRadiusLabel";
            this.blurRadiusLabel.Size = new System.Drawing.Size(63, 15);
            this.blurRadiusLabel.Text = "Blur radius";
            // 
            // blurRadiusUpDown
            // 
            this.blurRadiusUpDown.DecimalPlaces = 0;
            this.blurRadiusUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.blurRadiusUpDown.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.blurRadiusUpDown.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.blurRadiusUpDown.Name = "blurRadiusUpDown";
            this.blurRadiusUpDown.Size = new System.Drawing.Size(41, 23);
            this.blurRadiusUpDown.Text = "1";
            this.blurRadiusUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.blurRadiusUpDown.GotFocus += new System.EventHandler(this.ToolBarFocusableElementGotFocus);
            this.blurRadiusUpDown.LostFocus += new System.EventHandler(this.ToolBarFocusableElementLostFocus);
            // 
            // brightnessLabel
            // 
            this.brightnessLabel.LanguageKey = "editor_brightness";
            this.brightnessLabel.Name = "brightnessLabel";
            this.brightnessLabel.Size = new System.Drawing.Size(62, 15);
            this.brightnessLabel.Text = "Brightness";
            // 
            // brightnessUpDown
            // 
            this.brightnessUpDown.DecimalPlaces = 0;
            this.brightnessUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.brightnessUpDown.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.brightnessUpDown.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.brightnessUpDown.Name = "brightnessUpDown";
            this.brightnessUpDown.Size = new System.Drawing.Size(41, 23);
            this.brightnessUpDown.Text = "100";
            this.brightnessUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.brightnessUpDown.GotFocus += new System.EventHandler(this.ToolBarFocusableElementGotFocus);
            this.brightnessUpDown.LostFocus += new System.EventHandler(this.ToolBarFocusableElementLostFocus);
            // 
            // previewQualityLabel
            // 
            this.previewQualityLabel.LanguageKey = "editor_preview_quality";
            this.previewQualityLabel.Name = "previewQualityLabel";
            this.previewQualityLabel.Size = new System.Drawing.Size(87, 15);
            this.previewQualityLabel.Text = "Preview quality";
            // 
            // previewQualityUpDown
            // 
            this.previewQualityUpDown.DecimalPlaces = 0;
            this.previewQualityUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.previewQualityUpDown.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.previewQualityUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.previewQualityUpDown.Name = "previewQualityUpDown";
            this.previewQualityUpDown.Size = new System.Drawing.Size(41, 23);
            this.previewQualityUpDown.Text = "50";
            this.previewQualityUpDown.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.previewQualityUpDown.GotFocus += new System.EventHandler(this.ToolBarFocusableElementGotFocus);
            this.previewQualityUpDown.LostFocus += new System.EventHandler(this.ToolBarFocusableElementLostFocus);
            // 
            // magnificationFactorLabel
            // 
            this.magnificationFactorLabel.LanguageKey = "editor_magnification_factor";
            this.magnificationFactorLabel.Name = "magnificationFactorLabel";
            this.magnificationFactorLabel.Size = new System.Drawing.Size(115, 15);
            this.magnificationFactorLabel.Tag = Greenshot.Drawing.FilterContainer.PreparedFilter.MAGNIFICATION;
            this.magnificationFactorLabel.Text = "Magnification factor";
            // 
            // magnificationFactorUpDown
            // 
            this.magnificationFactorUpDown.DecimalPlaces = 0;
            this.magnificationFactorUpDown.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.magnificationFactorUpDown.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.magnificationFactorUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.magnificationFactorUpDown.Name = "magnificationFactorUpDown";
            this.magnificationFactorUpDown.Size = new System.Drawing.Size(29, 23);
            this.magnificationFactorUpDown.Text = "2";
            this.magnificationFactorUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.magnificationFactorUpDown.GotFocus += new System.EventHandler(this.ToolBarFocusableElementGotFocus);
            this.magnificationFactorUpDown.LostFocus += new System.EventHandler(this.ToolBarFocusableElementLostFocus);
            // 
            // pixelSizeLabel
            // 
            this.pixelSizeLabel.LanguageKey = "editor_pixel_size";
            this.pixelSizeLabel.Name = "pixelSizeLabel";
            this.pixelSizeLabel.Size = new System.Drawing.Size(53, 15);
            this.pixelSizeLabel.Text = "Pixel size";
            // 
            // pixelSizeUpDown
            // 
            this.pixelSizeUpDown.DecimalPlaces = 0;
            this.pixelSizeUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.pixelSizeUpDown.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.pixelSizeUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.pixelSizeUpDown.Name = "pixelSizeUpDown";
            this.pixelSizeUpDown.Size = new System.Drawing.Size(41, 23);
            this.pixelSizeUpDown.Text = "5";
            this.pixelSizeUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.pixelSizeUpDown.GotFocus += new System.EventHandler(this.ToolBarFocusableElementGotFocus);
            this.pixelSizeUpDown.LostFocus += new System.EventHandler(this.ToolBarFocusableElementLostFocus);
            // 
            // arrowHeadsLabel
            // 
            this.arrowHeadsLabel.LanguageKey = "editor_pixel_size";
            this.arrowHeadsLabel.Name = "arrowHeadsLabel";
            this.arrowHeadsLabel.Size = new System.Drawing.Size(73, 15);
            this.arrowHeadsLabel.Text = "Arrow heads";
            // 
            // arrowHeadsDropDownButton
            // 
            this.arrowHeadsDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.arrowHeadsDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.arrowHeadStartMenuItem,
            this.arrowHeadEndMenuItem,
            this.arrowHeadBothMenuItem,
            this.arrowHeadNoneMenuItem});
            this.arrowHeadsDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("arrowHeadsDropDownButton.Image")));
            this.arrowHeadsDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.arrowHeadsDropDownButton.LanguageKey = "editor_arrowheads";
            this.arrowHeadsDropDownButton.Name = "arrowHeadsDropDownButton";
            this.arrowHeadsDropDownButton.Size = new System.Drawing.Size(29, 20);
            this.arrowHeadsDropDownButton.Text = "Arrow heads";
            // 
            // arrowHeadStartMenuItem
            // 
            this.arrowHeadStartMenuItem.LanguageKey = "editor_arrowheads_start";
            this.arrowHeadStartMenuItem.Name = "arrowHeadStartMenuItem";
            this.arrowHeadStartMenuItem.Size = new System.Drawing.Size(129, 22);
            this.arrowHeadStartMenuItem.Tag = Greenshot.Drawing.ArrowContainer.ArrowHeadCombination.START_POINT;
            this.arrowHeadStartMenuItem.Text = "Start point";
            this.arrowHeadStartMenuItem.Click += new System.EventHandler(this.ArrowHeadsToolStripMenuItemClick);
            // 
            // arrowHeadEndMenuItem
            // 
            this.arrowHeadEndMenuItem.LanguageKey = "editor_arrowheads_end";
            this.arrowHeadEndMenuItem.Name = "arrowHeadEndMenuItem";
            this.arrowHeadEndMenuItem.Size = new System.Drawing.Size(129, 22);
            this.arrowHeadEndMenuItem.Tag = Greenshot.Drawing.ArrowContainer.ArrowHeadCombination.END_POINT;
            this.arrowHeadEndMenuItem.Text = "End point";
            this.arrowHeadEndMenuItem.Click += new System.EventHandler(this.ArrowHeadsToolStripMenuItemClick);
            // 
            // arrowHeadBothMenuItem
            // 
            this.arrowHeadBothMenuItem.LanguageKey = "editor_arrowheads_both";
            this.arrowHeadBothMenuItem.Name = "arrowHeadBothMenuItem";
            this.arrowHeadBothMenuItem.Size = new System.Drawing.Size(129, 22);
            this.arrowHeadBothMenuItem.Tag = Greenshot.Drawing.ArrowContainer.ArrowHeadCombination.BOTH;
            this.arrowHeadBothMenuItem.Text = "Both";
            this.arrowHeadBothMenuItem.Click += new System.EventHandler(this.ArrowHeadsToolStripMenuItemClick);
            // 
            // arrowHeadNoneMenuItem
            // 
            this.arrowHeadNoneMenuItem.LanguageKey = "editor_arrowheads_none";
            this.arrowHeadNoneMenuItem.Name = "arrowHeadNoneMenuItem";
            this.arrowHeadNoneMenuItem.Size = new System.Drawing.Size(129, 22);
            this.arrowHeadNoneMenuItem.Tag = Greenshot.Drawing.ArrowContainer.ArrowHeadCombination.NONE;
            this.arrowHeadNoneMenuItem.Text = "None";
            this.arrowHeadNoneMenuItem.Click += new System.EventHandler(this.ArrowHeadsToolStripMenuItemClick);
            // 
            // shadowButton
            // 
            this.shadowButton.CheckOnClick = true;
            this.shadowButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.shadowButton.Image = ((System.Drawing.Image)(resources.GetObject("shadowButton.Image")));
            this.shadowButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.shadowButton.LanguageKey = "editor_shadow";
            this.shadowButton.Name = "shadowButton";
            this.shadowButton.Size = new System.Drawing.Size(23, 20);
            this.shadowButton.Text = "Drop shadow";
            // 
            // btnConfirm
            // 
            this.btnConfirm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnConfirm.Image = ((System.Drawing.Image)(resources.GetObject("btnConfirm.Image")));
            this.btnConfirm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConfirm.LanguageKey = "editor_confirm";
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(23, 20);
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.Click += new System.EventHandler(this.BtnConfirmClick);
            // 
            // btnCancel
            // 
            this.btnCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCancel.LanguageKey = "editor_cancel";
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(23, 20);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.BtnCancelClick);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(304, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("closeToolStripMenuItem.Image")));
            this.closeToolStripMenuItem.LanguageKey = "editor_close";
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(307, 22);
            this.closeToolStripMenuItem.Text = "Close";
            // 
            // ImageEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(785, 485);
            this.Controls.Add(this.toolStripContainer1);
            this.KeyPreview = true;
            this.LanguageKey = "editor_title";
            this.MinimumSize = new System.Drawing.Size(100, 100);
            this.Name = "ImageEditorForm";
            this.Text = "Greenshot image editor";
            this.Activated += new System.EventHandler(this.ImageEditorFormActivated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImageEditorFormFormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ImageEditorFormKeyDown);
            this.Resize += new System.EventHandler(this.ImageEditorFormResize);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.LeftToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.LeftToolStripPanel.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.propertiesToolStrip.ResumeLayout(false);
            this.propertiesToolStrip.PerformLayout();
            this.ResumeLayout(false);

		}
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem alignRightToolStripMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem alignCenterToolStripMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem alignLeftToolStripMenuItem;
		private Greenshot.Controls.BindableToolStripDropDownButton textHorizontalAlignmentButton;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem alignMiddleToolStripMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem alignBottomToolStripMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem alignTopToolStripMenuItem;
		private Greenshot.Controls.BindableToolStripDropDownButton textVerticalAlignmentButton;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem invertToolStripMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem grayscaleToolStripMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripButton rotateCcwToolstripButton;
		private GreenshotPlugin.Controls.GreenshotToolStripButton rotateCwToolstripButton;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem addBorderToolStripMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem tornEdgesToolStripMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem addDropshadowToolStripMenuItem;
        private GreenshotPlugin.Controls.GreenshotToolStripDropDownButton toolStripSplitButton1;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem grayscaleHighlightMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem areaHighlightMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem textHighlightMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem magnifyMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem arrowHeadStartMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem arrowHeadEndMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem arrowHeadBothMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem arrowHeadNoneMenuItem;
		private Greenshot.Controls.BindableToolStripButton btnCancel;
		private Greenshot.Controls.BindableToolStripButton btnConfirm;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem selectAllToolStripMenuItem;
		private Greenshot.Controls.BindableToolStripDropDownButton highlightModeButton;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem pixelizeToolStripMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem blurToolStripMenuItem;
		private Greenshot.Controls.BindableToolStripDropDownButton obfuscateModeButton;
        private GreenshotPlugin.Controls.GreenshotToolStripButton btnHighlight;
		private Greenshot.Controls.FontFamilyComboBox fontFamilyComboBox;
		private Greenshot.Controls.BindableToolStripButton shadowButton;
		private Greenshot.Controls.BindableToolStripButton fontItalicButton;
		private Greenshot.Controls.BindableToolStripButton fontBoldButton;
		private Greenshot.Controls.ToolStripNumericUpDown fontSizeUpDown;
		private GreenshotPlugin.Controls.GreenshotToolStripLabel fontSizeLabel;
		private Greenshot.Controls.ToolStripNumericUpDown brightnessUpDown;
        private GreenshotPlugin.Controls.GreenshotToolStripLabel brightnessLabel;
		private GreenshotPlugin.Controls.GreenshotToolStripDropDownButton arrowHeadsDropDownButton;
		private GreenshotPlugin.Controls.GreenshotToolStripLabel arrowHeadsLabel;
		private Greenshot.Controls.ToolStripNumericUpDown pixelSizeUpDown;
		private GreenshotPlugin.Controls.GreenshotToolStripLabel pixelSizeLabel;
		private Greenshot.Controls.ToolStripNumericUpDown magnificationFactorUpDown;
		private GreenshotPlugin.Controls.GreenshotToolStripLabel magnificationFactorLabel;
		private Greenshot.Controls.ToolStripNumericUpDown previewQualityUpDown;
		private GreenshotPlugin.Controls.GreenshotToolStripLabel previewQualityLabel;
		private Greenshot.Controls.ToolStripNumericUpDown blurRadiusUpDown;
        private GreenshotPlugin.Controls.GreenshotToolStripLabel blurRadiusLabel;
		private GreenshotPlugin.Controls.GreenshotToolStripLabel lineThicknessLabel;
		private Greenshot.Controls.ToolStripNumericUpDown lineThicknessUpDown;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
		private GreenshotPlugin.Controls.GreenshotToolStripButton btnFreehand;
		private GreenshotPlugin.Controls.GreenshotToolStripButton btnObfuscate;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private GreenshotPlugin.Controls.GreenshotToolStripButton btnCrop;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem downToBottomToolStripMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem upToTopToolStripMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem downOneLevelToolStripMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem upOneLevelToolStripMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem arrangeToolStripMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripButton btnCursor;
		private Greenshot.Controls.ToolStripEx toolStrip2;
        private GreenshotPlugin.Controls.GreenshotToolStripButton btnArrow;
        private GreenshotPlugin.Controls.GreenshotToolStripButton btnText;
        private GreenshotPlugin.Controls.GreenshotToolStripButton btnLine;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem duplicateToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private GreenshotPlugin.Controls.GreenshotToolStripMenuItem removeObjectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem pasteToolStripMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem copyToolStripMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem cutToolStripMenuItem;
		private GreenshotPlugin.Controls.GreenshotToolStripMenuItem editToolStripMenuItem;
        private Greenshot.Controls.MenuStripEx menuStrip1;
		private GreenshotPlugin.Controls.GreenshotToolStripButton btnCut;
		private GreenshotPlugin.Controls.GreenshotToolStripButton btnCopy;
		private GreenshotPlugin.Controls.GreenshotToolStripButton btnPaste;
		private System.Windows.Forms.ToolStripButton btnUndo;
        private System.Windows.Forms.ToolStripButton btnRedo;
		private GreenshotPlugin.Controls.GreenshotToolStripButton btnDelete;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private GreenshotPlugin.Controls.GreenshotToolStripButton btnEllipse;
		private GreenshotPlugin.Controls.GreenshotToolStripButton btnRect;
		private System.Windows.Forms.ToolStripContainer toolStripContainer1;
		private Greenshot.Controls.ToolStripEx propertiesToolStrip;
		private GreenshotPlugin.Controls.NonJumpingPanel panel1;
		private Greenshot.Controls.ToolStripColorButton btnFillColor;
		private Greenshot.Controls.ToolStripColorButton btnLineColor;
        private GreenshotPlugin.Controls.GreenshotToolStripMenuItem autoCropToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnSaveClose;
        private System.Windows.Forms.ToolStripButton btnClose;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator tssTaskButtons;
        private System.Windows.Forms.ToolStripButton btnClipboardCopy;
        private System.Windows.Forms.ToolStripButton btnUploadImage;
        private System.Windows.Forms.ToolStripButton btnSaveAs;
        private System.Windows.Forms.ToolStripButton btnCancelTasks;
        private System.Windows.Forms.ToolStripButton btnSave;
        private GreenshotPlugin.Controls.GreenshotToolStripButton btnSpeechBubble;
        private GreenshotPlugin.Controls.GreenshotToolStripButton btnStepLabel;
        private System.Windows.Forms.ToolStripButton btnResize;
	}
}
