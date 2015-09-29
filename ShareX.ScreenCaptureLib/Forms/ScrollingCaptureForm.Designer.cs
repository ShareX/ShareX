using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ShareX.HelpersLib;

namespace ShareX.ScreenCaptureLib
{
    partial class ScrollingCaptureForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnSelectHandle = new System.Windows.Forms.Button();
            this.lblControlText = new System.Windows.Forms.Label();
            this.btnCapture = new System.Windows.Forms.Button();
            this.captureTimer = new System.Windows.Forms.Timer(this.components);
            this.nudScrollDelay = new System.Windows.Forms.NumericUpDown();
            this.nudMaximumScrollCount = new System.Windows.Forms.NumericUpDown();
            this.lblScrollDelay = new System.Windows.Forms.Label();
            this.lblMaximumScrollCount = new System.Windows.Forms.Label();
            this.tcScrollingCapture = new System.Windows.Forms.TabControl();
            this.tpCapture = new System.Windows.Forms.TabPage();
            this.cbStartSelectionAutomatically = new System.Windows.Forms.CheckBox();
            this.cbAutoCombine = new System.Windows.Forms.CheckBox();
            this.lblSelectedRectangle = new System.Windows.Forms.Label();
            this.btnSelectRectangle = new System.Windows.Forms.Button();
            this.lblStartDelay = new System.Windows.Forms.Label();
            this.nudStartDelay = new System.Windows.Forms.NumericUpDown();
            this.cbScrollTopBeforeCapture = new System.Windows.Forms.CheckBox();
            this.cbStartCaptureAutomatically = new System.Windows.Forms.CheckBox();
            this.cbRemoveDuplicates = new System.Windows.Forms.CheckBox();
            this.cbAutoDetectScrollEnd = new System.Windows.Forms.CheckBox();
            this.lblScrollMethod = new System.Windows.Forms.Label();
            this.cbScrollMethod = new System.Windows.Forms.ComboBox();
            this.tpOutput = new System.Windows.Forms.TabPage();
            this.gbImages = new System.Windows.Forms.GroupBox();
            this.txtImagesCount = new System.Windows.Forms.TextBox();
            this.lblImageCount = new System.Windows.Forms.Label();
            this.nudIgnoreLast = new System.Windows.Forms.NumericUpDown();
            this.lblIgnoreLast = new System.Windows.Forms.Label();
            this.btnResetCombine = new System.Windows.Forms.Button();
            this.btnGuessCombineAdjustments = new System.Windows.Forms.Button();
            this.btnStartTask = new System.Windows.Forms.Button();
            this.btnGuessEdges = new System.Windows.Forms.Button();
            this.gbCombineAdjustments = new System.Windows.Forms.GroupBox();
            this.lblCombineLastVertical = new System.Windows.Forms.Label();
            this.lblCombineVertical = new System.Windows.Forms.Label();
            this.nudCombineVertical = new System.Windows.Forms.NumericUpDown();
            this.nudCombineLastVertical = new System.Windows.Forms.NumericUpDown();
            this.gbTrimEdges = new System.Windows.Forms.GroupBox();
            this.lblTrimBottom = new System.Windows.Forms.Label();
            this.lblTrimRight = new System.Windows.Forms.Label();
            this.lblTrimTop = new System.Windows.Forms.Label();
            this.lblTrimLeft = new System.Windows.Forms.Label();
            this.nudTrimLeft = new System.Windows.Forms.NumericUpDown();
            this.nudTrimBottom = new System.Windows.Forms.NumericUpDown();
            this.nudTrimTop = new System.Windows.Forms.NumericUpDown();
            this.nudTrimRight = new System.Windows.Forms.NumericUpDown();
            this.pOutput = new System.Windows.Forms.Panel();
            this.lblProcessing = new System.Windows.Forms.Label();
            this.pbOutput = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaximumScrollCount)).BeginInit();
            this.tcScrollingCapture.SuspendLayout();
            this.tpCapture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartDelay)).BeginInit();
            this.tpOutput.SuspendLayout();
            this.gbImages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudIgnoreLast)).BeginInit();
            this.gbCombineAdjustments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCombineVertical)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCombineLastVertical)).BeginInit();
            this.gbTrimEdges.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTrimLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTrimBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTrimTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTrimRight)).BeginInit();
            this.pOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbOutput)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelectHandle
            // 
            this.btnSelectHandle.Location = new System.Drawing.Point(16, 16);
            this.btnSelectHandle.Name = "btnSelectHandle";
            this.btnSelectHandle.Size = new System.Drawing.Size(344, 23);
            this.btnSelectHandle.TabIndex = 0;
            this.btnSelectHandle.Text = "Select window or control to scroll...";
            this.btnSelectHandle.UseVisualStyleBackColor = true;
            this.btnSelectHandle.Click += new System.EventHandler(this.btnSelectHandle_Click);
            // 
            // lblControlText
            // 
            this.lblControlText.AutoSize = true;
            this.lblControlText.Location = new System.Drawing.Point(368, 21);
            this.lblControlText.Name = "lblControlText";
            this.lblControlText.Size = new System.Drawing.Size(0, 13);
            this.lblControlText.TabIndex = 1;
            // 
            // btnCapture
            // 
            this.btnCapture.Enabled = false;
            this.btnCapture.Location = new System.Drawing.Point(16, 312);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(344, 32);
            this.btnCapture.TabIndex = 18;
            this.btnCapture.Text = "Start capture";
            this.btnCapture.UseVisualStyleBackColor = true;
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // captureTimer
            // 
            this.captureTimer.Tick += new System.EventHandler(this.captureTimer_Tick);
            // 
            // nudScrollDelay
            // 
            this.nudScrollDelay.Location = new System.Drawing.Point(160, 116);
            this.nudScrollDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudScrollDelay.Name = "nudScrollDelay";
            this.nudScrollDelay.Size = new System.Drawing.Size(80, 20);
            this.nudScrollDelay.TabIndex = 9;
            this.nudScrollDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudScrollDelay.ValueChanged += new System.EventHandler(this.nudScrollDelay_ValueChanged);
            // 
            // nudMaximumScrollCount
            // 
            this.nudMaximumScrollCount.Location = new System.Drawing.Point(160, 140);
            this.nudMaximumScrollCount.Name = "nudMaximumScrollCount";
            this.nudMaximumScrollCount.Size = new System.Drawing.Size(80, 20);
            this.nudMaximumScrollCount.TabIndex = 11;
            this.nudMaximumScrollCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudMaximumScrollCount.ValueChanged += new System.EventHandler(this.nudMaximumScrollCount_ValueChanged);
            // 
            // lblScrollDelay
            // 
            this.lblScrollDelay.AutoSize = true;
            this.lblScrollDelay.Location = new System.Drawing.Point(13, 120);
            this.lblScrollDelay.Name = "lblScrollDelay";
            this.lblScrollDelay.Size = new System.Drawing.Size(64, 13);
            this.lblScrollDelay.TabIndex = 8;
            this.lblScrollDelay.Text = "Scroll delay:";
            // 
            // lblMaximumScrollCount
            // 
            this.lblMaximumScrollCount.AutoSize = true;
            this.lblMaximumScrollCount.Location = new System.Drawing.Point(13, 144);
            this.lblMaximumScrollCount.Name = "lblMaximumScrollCount";
            this.lblMaximumScrollCount.Size = new System.Drawing.Size(111, 13);
            this.lblMaximumScrollCount.TabIndex = 10;
            this.lblMaximumScrollCount.Text = "Maximum scroll count:";
            // 
            // tcScrollingCapture
            // 
            this.tcScrollingCapture.Controls.Add(this.tpCapture);
            this.tcScrollingCapture.Controls.Add(this.tpOutput);
            this.tcScrollingCapture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcScrollingCapture.Location = new System.Drawing.Point(0, 0);
            this.tcScrollingCapture.Name = "tcScrollingCapture";
            this.tcScrollingCapture.SelectedIndex = 0;
            this.tcScrollingCapture.Size = new System.Drawing.Size(984, 661);
            this.tcScrollingCapture.TabIndex = 0;
            // 
            // tpCapture
            // 
            this.tpCapture.Controls.Add(this.cbStartSelectionAutomatically);
            this.tpCapture.Controls.Add(this.cbAutoCombine);
            this.tpCapture.Controls.Add(this.lblSelectedRectangle);
            this.tpCapture.Controls.Add(this.btnSelectRectangle);
            this.tpCapture.Controls.Add(this.lblStartDelay);
            this.tpCapture.Controls.Add(this.nudStartDelay);
            this.tpCapture.Controls.Add(this.cbScrollTopBeforeCapture);
            this.tpCapture.Controls.Add(this.cbStartCaptureAutomatically);
            this.tpCapture.Controls.Add(this.cbRemoveDuplicates);
            this.tpCapture.Controls.Add(this.cbAutoDetectScrollEnd);
            this.tpCapture.Controls.Add(this.lblScrollMethod);
            this.tpCapture.Controls.Add(this.cbScrollMethod);
            this.tpCapture.Controls.Add(this.btnSelectHandle);
            this.tpCapture.Controls.Add(this.lblMaximumScrollCount);
            this.tpCapture.Controls.Add(this.lblControlText);
            this.tpCapture.Controls.Add(this.lblScrollDelay);
            this.tpCapture.Controls.Add(this.btnCapture);
            this.tpCapture.Controls.Add(this.nudMaximumScrollCount);
            this.tpCapture.Controls.Add(this.nudScrollDelay);
            this.tpCapture.Location = new System.Drawing.Point(4, 22);
            this.tpCapture.Name = "tpCapture";
            this.tpCapture.Padding = new System.Windows.Forms.Padding(3);
            this.tpCapture.Size = new System.Drawing.Size(976, 635);
            this.tpCapture.TabIndex = 0;
            this.tpCapture.Text = "Capture";
            this.tpCapture.UseVisualStyleBackColor = true;
            // 
            // cbStartSelectionAutomatically
            // 
            this.cbStartSelectionAutomatically.AutoSize = true;
            this.cbStartSelectionAutomatically.Location = new System.Drawing.Point(16, 168);
            this.cbStartSelectionAutomatically.Name = "cbStartSelectionAutomatically";
            this.cbStartSelectionAutomatically.Size = new System.Drawing.Size(288, 17);
            this.cbStartSelectionAutomatically.TabIndex = 12;
            this.cbStartSelectionAutomatically.Text = "Automatically start selection before opening this window";
            this.cbStartSelectionAutomatically.UseVisualStyleBackColor = true;
            this.cbStartSelectionAutomatically.CheckedChanged += new System.EventHandler(this.cbStartSelectionAutomatically_CheckedChanged);
            // 
            // cbAutoCombine
            // 
            this.cbAutoCombine.AutoSize = true;
            this.cbAutoCombine.Location = new System.Drawing.Point(16, 288);
            this.cbAutoCombine.Name = "cbAutoCombine";
            this.cbAutoCombine.Size = new System.Drawing.Size(280, 17);
            this.cbAutoCombine.TabIndex = 17;
            this.cbAutoCombine.Text = "Automatically guess offsets and combine after capture";
            this.cbAutoCombine.UseVisualStyleBackColor = true;
            this.cbAutoCombine.CheckedChanged += new System.EventHandler(this.cbAutoCombine_CheckedChanged);
            // 
            // lblSelectedRectangle
            // 
            this.lblSelectedRectangle.AutoSize = true;
            this.lblSelectedRectangle.Location = new System.Drawing.Point(368, 45);
            this.lblSelectedRectangle.Name = "lblSelectedRectangle";
            this.lblSelectedRectangle.Size = new System.Drawing.Size(0, 13);
            this.lblSelectedRectangle.TabIndex = 3;
            // 
            // btnSelectRectangle
            // 
            this.btnSelectRectangle.Enabled = false;
            this.btnSelectRectangle.Location = new System.Drawing.Point(16, 40);
            this.btnSelectRectangle.Name = "btnSelectRectangle";
            this.btnSelectRectangle.Size = new System.Drawing.Size(344, 23);
            this.btnSelectRectangle.TabIndex = 2;
            this.btnSelectRectangle.Text = "(Optional) Select custom region in window...";
            this.btnSelectRectangle.UseVisualStyleBackColor = true;
            this.btnSelectRectangle.Click += new System.EventHandler(this.btnSelectRectangle_Click);
            // 
            // lblStartDelay
            // 
            this.lblStartDelay.AutoSize = true;
            this.lblStartDelay.Location = new System.Drawing.Point(13, 96);
            this.lblStartDelay.Name = "lblStartDelay";
            this.lblStartDelay.Size = new System.Drawing.Size(60, 13);
            this.lblStartDelay.TabIndex = 6;
            this.lblStartDelay.Text = "Start delay:";
            // 
            // nudStartDelay
            // 
            this.nudStartDelay.Location = new System.Drawing.Point(160, 92);
            this.nudStartDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudStartDelay.Name = "nudStartDelay";
            this.nudStartDelay.Size = new System.Drawing.Size(80, 20);
            this.nudStartDelay.TabIndex = 7;
            this.nudStartDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudStartDelay.ValueChanged += new System.EventHandler(this.nudStartDelay_ValueChanged);
            // 
            // cbScrollTopBeforeCapture
            // 
            this.cbScrollTopBeforeCapture.AutoSize = true;
            this.cbScrollTopBeforeCapture.Location = new System.Drawing.Point(16, 216);
            this.cbScrollTopBeforeCapture.Name = "cbScrollTopBeforeCapture";
            this.cbScrollTopBeforeCapture.Size = new System.Drawing.Size(223, 17);
            this.cbScrollTopBeforeCapture.TabIndex = 14;
            this.cbScrollTopBeforeCapture.Text = "Attempt scrolling to the top before capture";
            this.cbScrollTopBeforeCapture.UseVisualStyleBackColor = true;
            this.cbScrollTopBeforeCapture.CheckedChanged += new System.EventHandler(this.cbScrollTopBeforeCapture_CheckedChanged);
            // 
            // cbStartCaptureAutomatically
            // 
            this.cbStartCaptureAutomatically.AutoSize = true;
            this.cbStartCaptureAutomatically.Location = new System.Drawing.Point(16, 192);
            this.cbStartCaptureAutomatically.Name = "cbStartCaptureAutomatically";
            this.cbStartCaptureAutomatically.Size = new System.Drawing.Size(213, 17);
            this.cbStartCaptureAutomatically.TabIndex = 13;
            this.cbStartCaptureAutomatically.Text = "Start capture immediately after selection";
            this.cbStartCaptureAutomatically.UseVisualStyleBackColor = true;
            this.cbStartCaptureAutomatically.CheckedChanged += new System.EventHandler(this.cbStartCaptureAutomatically_CheckedChanged);
            // 
            // cbRemoveDuplicates
            // 
            this.cbRemoveDuplicates.AutoSize = true;
            this.cbRemoveDuplicates.Location = new System.Drawing.Point(16, 264);
            this.cbRemoveDuplicates.Name = "cbRemoveDuplicates";
            this.cbRemoveDuplicates.Size = new System.Drawing.Size(148, 17);
            this.cbRemoveDuplicates.TabIndex = 16;
            this.cbRemoveDuplicates.Text = "Remove duplicate images";
            this.cbRemoveDuplicates.UseVisualStyleBackColor = true;
            this.cbRemoveDuplicates.CheckedChanged += new System.EventHandler(this.cbRemoveDuplicates_CheckedChanged);
            // 
            // cbAutoDetectScrollEnd
            // 
            this.cbAutoDetectScrollEnd.AutoSize = true;
            this.cbAutoDetectScrollEnd.Location = new System.Drawing.Point(16, 240);
            this.cbAutoDetectScrollEnd.Name = "cbAutoDetectScrollEnd";
            this.cbAutoDetectScrollEnd.Size = new System.Drawing.Size(169, 17);
            this.cbAutoDetectScrollEnd.TabIndex = 15;
            this.cbAutoDetectScrollEnd.Text = "Automatically detect scroll end";
            this.cbAutoDetectScrollEnd.UseVisualStyleBackColor = true;
            this.cbAutoDetectScrollEnd.CheckedChanged += new System.EventHandler(this.cbAutoDetectScrollEnd_CheckedChanged);
            // 
            // lblScrollMethod
            // 
            this.lblScrollMethod.AutoSize = true;
            this.lblScrollMethod.Location = new System.Drawing.Point(13, 72);
            this.lblScrollMethod.Name = "lblScrollMethod";
            this.lblScrollMethod.Size = new System.Drawing.Size(74, 13);
            this.lblScrollMethod.TabIndex = 4;
            this.lblScrollMethod.Text = "Scroll method:";
            // 
            // cbScrollMethod
            // 
            this.cbScrollMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbScrollMethod.FormattingEnabled = true;
            this.cbScrollMethod.Location = new System.Drawing.Point(160, 68);
            this.cbScrollMethod.Name = "cbScrollMethod";
            this.cbScrollMethod.Size = new System.Drawing.Size(312, 21);
            this.cbScrollMethod.TabIndex = 5;
            this.cbScrollMethod.SelectedIndexChanged += new System.EventHandler(this.cbScrollMethod_SelectedIndexChanged);
            // 
            // tpOutput
            // 
            this.tpOutput.Controls.Add(this.gbImages);
            this.tpOutput.Controls.Add(this.btnResetCombine);
            this.tpOutput.Controls.Add(this.btnGuessCombineAdjustments);
            this.tpOutput.Controls.Add(this.btnStartTask);
            this.tpOutput.Controls.Add(this.btnGuessEdges);
            this.tpOutput.Controls.Add(this.gbCombineAdjustments);
            this.tpOutput.Controls.Add(this.gbTrimEdges);
            this.tpOutput.Controls.Add(this.pOutput);
            this.tpOutput.Location = new System.Drawing.Point(4, 22);
            this.tpOutput.Name = "tpOutput";
            this.tpOutput.Padding = new System.Windows.Forms.Padding(3);
            this.tpOutput.Size = new System.Drawing.Size(976, 635);
            this.tpOutput.TabIndex = 1;
            this.tpOutput.Text = "Output";
            this.tpOutput.UseVisualStyleBackColor = true;
            // 
            // gbImages
            // 
            this.gbImages.Controls.Add(this.txtImagesCount);
            this.gbImages.Controls.Add(this.lblImageCount);
            this.gbImages.Controls.Add(this.nudIgnoreLast);
            this.gbImages.Controls.Add(this.lblIgnoreLast);
            this.gbImages.Location = new System.Drawing.Point(8, 8);
            this.gbImages.Name = "gbImages";
            this.gbImages.Size = new System.Drawing.Size(184, 120);
            this.gbImages.TabIndex = 10;
            this.gbImages.TabStop = false;
            this.gbImages.Text = "Images";
            // 
            // txtImagesCount
            // 
            this.txtImagesCount.Location = new System.Drawing.Point(120, 16);
            this.txtImagesCount.Name = "txtImagesCount";
            this.txtImagesCount.ReadOnly = true;
            this.txtImagesCount.Size = new System.Drawing.Size(56, 20);
            this.txtImagesCount.TabIndex = 10;
            this.txtImagesCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblImageCount
            // 
            this.lblImageCount.AutoSize = true;
            this.lblImageCount.Location = new System.Drawing.Point(8, 20);
            this.lblImageCount.Name = "lblImageCount";
            this.lblImageCount.Size = new System.Drawing.Size(69, 13);
            this.lblImageCount.TabIndex = 5;
            this.lblImageCount.Text = "Image count:";
            // 
            // nudIgnoreLast
            // 
            this.nudIgnoreLast.Location = new System.Drawing.Point(120, 40);
            this.nudIgnoreLast.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudIgnoreLast.Name = "nudIgnoreLast";
            this.nudIgnoreLast.Size = new System.Drawing.Size(56, 20);
            this.nudIgnoreLast.TabIndex = 9;
            this.nudIgnoreLast.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudIgnoreLast.ValueChanged += new System.EventHandler(this.nudIgnoreLast_ValueChanged);
            // 
            // lblIgnoreLast
            // 
            this.lblIgnoreLast.AutoSize = true;
            this.lblIgnoreLast.Location = new System.Drawing.Point(8, 44);
            this.lblIgnoreLast.Name = "lblIgnoreLast";
            this.lblIgnoreLast.Size = new System.Drawing.Size(69, 13);
            this.lblIgnoreLast.TabIndex = 8;
            this.lblIgnoreLast.Text = "Remove last:";
            // 
            // btnResetCombine
            // 
            this.btnResetCombine.Enabled = false;
            this.btnResetCombine.Location = new System.Drawing.Point(560, 96);
            this.btnResetCombine.Name = "btnResetCombine";
            this.btnResetCombine.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnResetCombine.Size = new System.Drawing.Size(376, 23);
            this.btnResetCombine.TabIndex = 3;
            this.btnResetCombine.Text = "Reset output options";
            this.btnResetCombine.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnResetCombine.UseMnemonic = false;
            this.btnResetCombine.UseVisualStyleBackColor = true;
            this.btnResetCombine.Click += new System.EventHandler(this.btnResetCombine_Click);
            // 
            // btnGuessCombineAdjustments
            // 
            this.btnGuessCombineAdjustments.Enabled = false;
            this.btnGuessCombineAdjustments.Location = new System.Drawing.Point(560, 40);
            this.btnGuessCombineAdjustments.Name = "btnGuessCombineAdjustments";
            this.btnGuessCombineAdjustments.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnGuessCombineAdjustments.Size = new System.Drawing.Size(376, 23);
            this.btnGuessCombineAdjustments.TabIndex = 4;
            this.btnGuessCombineAdjustments.Text = "Guess combine adjustments & combine";
            this.btnGuessCombineAdjustments.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGuessCombineAdjustments.UseMnemonic = false;
            this.btnGuessCombineAdjustments.UseVisualStyleBackColor = true;
            this.btnGuessCombineAdjustments.Click += new System.EventHandler(this.btnGuessCombineAdjustments_Click);
            // 
            // btnStartTask
            // 
            this.btnStartTask.Enabled = false;
            this.btnStartTask.Location = new System.Drawing.Point(560, 64);
            this.btnStartTask.Name = "btnStartTask";
            this.btnStartTask.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnStartTask.Size = new System.Drawing.Size(376, 23);
            this.btnStartTask.TabIndex = 6;
            this.btnStartTask.Text = "Upload/save depending on after capture settings";
            this.btnStartTask.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStartTask.UseVisualStyleBackColor = true;
            this.btnStartTask.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // btnGuessEdges
            // 
            this.btnGuessEdges.Enabled = false;
            this.btnGuessEdges.Location = new System.Drawing.Point(560, 16);
            this.btnGuessEdges.Name = "btnGuessEdges";
            this.btnGuessEdges.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnGuessEdges.Size = new System.Drawing.Size(376, 23);
            this.btnGuessEdges.TabIndex = 2;
            this.btnGuessEdges.Text = "Guess edge values to trim";
            this.btnGuessEdges.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGuessEdges.UseVisualStyleBackColor = true;
            this.btnGuessEdges.Click += new System.EventHandler(this.btnGuessEdges_Click);
            // 
            // gbCombineAdjustments
            // 
            this.gbCombineAdjustments.Controls.Add(this.lblCombineLastVertical);
            this.gbCombineAdjustments.Controls.Add(this.lblCombineVertical);
            this.gbCombineAdjustments.Controls.Add(this.nudCombineVertical);
            this.gbCombineAdjustments.Controls.Add(this.nudCombineLastVertical);
            this.gbCombineAdjustments.Location = new System.Drawing.Point(368, 8);
            this.gbCombineAdjustments.Name = "gbCombineAdjustments";
            this.gbCombineAdjustments.Size = new System.Drawing.Size(184, 120);
            this.gbCombineAdjustments.TabIndex = 1;
            this.gbCombineAdjustments.TabStop = false;
            this.gbCombineAdjustments.Text = "Combine adjustments";
            // 
            // lblCombineLastVertical
            // 
            this.lblCombineLastVertical.AutoSize = true;
            this.lblCombineLastVertical.Location = new System.Drawing.Point(8, 44);
            this.lblCombineLastVertical.Name = "lblCombineLastVertical";
            this.lblCombineLastVertical.Size = new System.Drawing.Size(67, 13);
            this.lblCombineLastVertical.TabIndex = 2;
            this.lblCombineLastVertical.Text = "Last vertical:";
            // 
            // lblCombineVertical
            // 
            this.lblCombineVertical.AutoSize = true;
            this.lblCombineVertical.Location = new System.Drawing.Point(8, 20);
            this.lblCombineVertical.Name = "lblCombineVertical";
            this.lblCombineVertical.Size = new System.Drawing.Size(45, 13);
            this.lblCombineVertical.TabIndex = 0;
            this.lblCombineVertical.Text = "Vertical:";
            // 
            // nudCombineVertical
            // 
            this.nudCombineVertical.Location = new System.Drawing.Point(120, 16);
            this.nudCombineVertical.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudCombineVertical.Name = "nudCombineVertical";
            this.nudCombineVertical.Size = new System.Drawing.Size(56, 20);
            this.nudCombineVertical.TabIndex = 1;
            this.nudCombineVertical.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudCombineVertical.ValueChanged += new System.EventHandler(this.nudCombineVertical_ValueChanged);
            // 
            // nudCombineLastVertical
            // 
            this.nudCombineLastVertical.Location = new System.Drawing.Point(120, 40);
            this.nudCombineLastVertical.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudCombineLastVertical.Name = "nudCombineLastVertical";
            this.nudCombineLastVertical.Size = new System.Drawing.Size(56, 20);
            this.nudCombineLastVertical.TabIndex = 3;
            this.nudCombineLastVertical.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudCombineLastVertical.ValueChanged += new System.EventHandler(this.nudCombineLastVertical_ValueChanged);
            // 
            // gbTrimEdges
            // 
            this.gbTrimEdges.Controls.Add(this.lblTrimBottom);
            this.gbTrimEdges.Controls.Add(this.lblTrimRight);
            this.gbTrimEdges.Controls.Add(this.lblTrimTop);
            this.gbTrimEdges.Controls.Add(this.lblTrimLeft);
            this.gbTrimEdges.Controls.Add(this.nudTrimLeft);
            this.gbTrimEdges.Controls.Add(this.nudTrimBottom);
            this.gbTrimEdges.Controls.Add(this.nudTrimTop);
            this.gbTrimEdges.Controls.Add(this.nudTrimRight);
            this.gbTrimEdges.Location = new System.Drawing.Point(200, 8);
            this.gbTrimEdges.Name = "gbTrimEdges";
            this.gbTrimEdges.Size = new System.Drawing.Size(160, 120);
            this.gbTrimEdges.TabIndex = 0;
            this.gbTrimEdges.TabStop = false;
            this.gbTrimEdges.Text = "Trim edges";
            // 
            // lblTrimBottom
            // 
            this.lblTrimBottom.AutoSize = true;
            this.lblTrimBottom.Location = new System.Drawing.Point(8, 92);
            this.lblTrimBottom.Name = "lblTrimBottom";
            this.lblTrimBottom.Size = new System.Drawing.Size(43, 13);
            this.lblTrimBottom.TabIndex = 6;
            this.lblTrimBottom.Text = "Bottom:";
            // 
            // lblTrimRight
            // 
            this.lblTrimRight.AutoSize = true;
            this.lblTrimRight.Location = new System.Drawing.Point(8, 68);
            this.lblTrimRight.Name = "lblTrimRight";
            this.lblTrimRight.Size = new System.Drawing.Size(35, 13);
            this.lblTrimRight.TabIndex = 4;
            this.lblTrimRight.Text = "Right:";
            // 
            // lblTrimTop
            // 
            this.lblTrimTop.AutoSize = true;
            this.lblTrimTop.Location = new System.Drawing.Point(8, 44);
            this.lblTrimTop.Name = "lblTrimTop";
            this.lblTrimTop.Size = new System.Drawing.Size(29, 13);
            this.lblTrimTop.TabIndex = 2;
            this.lblTrimTop.Text = "Top:";
            // 
            // lblTrimLeft
            // 
            this.lblTrimLeft.AutoSize = true;
            this.lblTrimLeft.Location = new System.Drawing.Point(8, 20);
            this.lblTrimLeft.Name = "lblTrimLeft";
            this.lblTrimLeft.Size = new System.Drawing.Size(28, 13);
            this.lblTrimLeft.TabIndex = 0;
            this.lblTrimLeft.Text = "Left:";
            // 
            // nudTrimLeft
            // 
            this.nudTrimLeft.Location = new System.Drawing.Point(96, 16);
            this.nudTrimLeft.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudTrimLeft.Name = "nudTrimLeft";
            this.nudTrimLeft.Size = new System.Drawing.Size(56, 20);
            this.nudTrimLeft.TabIndex = 1;
            this.nudTrimLeft.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudTrimLeft.ValueChanged += new System.EventHandler(this.nudTrimLeft_ValueChanged);
            // 
            // nudTrimBottom
            // 
            this.nudTrimBottom.Location = new System.Drawing.Point(96, 88);
            this.nudTrimBottom.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudTrimBottom.Name = "nudTrimBottom";
            this.nudTrimBottom.Size = new System.Drawing.Size(56, 20);
            this.nudTrimBottom.TabIndex = 7;
            this.nudTrimBottom.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudTrimBottom.ValueChanged += new System.EventHandler(this.nudTrimBottom_ValueChanged);
            // 
            // nudTrimTop
            // 
            this.nudTrimTop.Location = new System.Drawing.Point(96, 40);
            this.nudTrimTop.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudTrimTop.Name = "nudTrimTop";
            this.nudTrimTop.Size = new System.Drawing.Size(56, 20);
            this.nudTrimTop.TabIndex = 3;
            this.nudTrimTop.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudTrimTop.ValueChanged += new System.EventHandler(this.nudTrimTop_ValueChanged);
            // 
            // nudTrimRight
            // 
            this.nudTrimRight.Location = new System.Drawing.Point(96, 64);
            this.nudTrimRight.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudTrimRight.Name = "nudTrimRight";
            this.nudTrimRight.Size = new System.Drawing.Size(56, 20);
            this.nudTrimRight.TabIndex = 5;
            this.nudTrimRight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudTrimRight.ValueChanged += new System.EventHandler(this.nudTrimRight_ValueChanged);
            // 
            // pOutput
            // 
            this.pOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pOutput.AutoScroll = true;
            this.pOutput.Controls.Add(this.lblProcessing);
            this.pOutput.Controls.Add(this.pbOutput);
            this.pOutput.Location = new System.Drawing.Point(8, 136);
            this.pOutput.Name = "pOutput";
            this.pOutput.Size = new System.Drawing.Size(961, 490);
            this.pOutput.TabIndex = 7;
            // 
            // lblProcessing
            // 
            this.lblProcessing.AutoSize = true;
            this.lblProcessing.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProcessing.Location = new System.Drawing.Point(8, 8);
            this.lblProcessing.Name = "lblProcessing";
            this.lblProcessing.Size = new System.Drawing.Size(137, 25);
            this.lblProcessing.TabIndex = 0;
            this.lblProcessing.Text = "Processing...";
            this.lblProcessing.Visible = false;
            // 
            // pbOutput
            // 
            this.pbOutput.Location = new System.Drawing.Point(0, 0);
            this.pbOutput.Name = "pbOutput";
            this.pbOutput.Size = new System.Drawing.Size(10, 10);
            this.pbOutput.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbOutput.TabIndex = 0;
            this.pbOutput.TabStop = false;
            // 
            // ScrollingCaptureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.tcScrollingCapture);
            this.Name = "ScrollingCaptureForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Scrolling capture";
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaximumScrollCount)).EndInit();
            this.tcScrollingCapture.ResumeLayout(false);
            this.tpCapture.ResumeLayout(false);
            this.tpCapture.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartDelay)).EndInit();
            this.tpOutput.ResumeLayout(false);
            this.gbImages.ResumeLayout(false);
            this.gbImages.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudIgnoreLast)).EndInit();
            this.gbCombineAdjustments.ResumeLayout(false);
            this.gbCombineAdjustments.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCombineVertical)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCombineLastVertical)).EndInit();
            this.gbTrimEdges.ResumeLayout(false);
            this.gbTrimEdges.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTrimLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTrimBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTrimTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTrimRight)).EndInit();
            this.pOutput.ResumeLayout(false);
            this.pOutput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbOutput)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Button btnSelectHandle;
        private Label lblControlText;
        private Button btnCapture;
        private Timer captureTimer;
        private NumericUpDown nudScrollDelay;
        private NumericUpDown nudMaximumScrollCount;
        private Label lblScrollDelay;
        private Label lblMaximumScrollCount;
        private TabControl tcScrollingCapture;
        private TabPage tpCapture;
        private TabPage tpOutput;
        private Panel pOutput;
        private PictureBox pbOutput;
        private GroupBox gbCombineAdjustments;
        private Label lblCombineVertical;
        private NumericUpDown nudCombineVertical;
        private NumericUpDown nudCombineLastVertical;
        private GroupBox gbTrimEdges;
        private Label lblTrimBottom;
        private Label lblTrimRight;
        private Label lblTrimTop;
        private Label lblTrimLeft;
        private NumericUpDown nudTrimLeft;
        private NumericUpDown nudTrimBottom;
        private NumericUpDown nudTrimTop;
        private NumericUpDown nudTrimRight;
        private Label lblCombineLastVertical;
        private Button btnGuessEdges;
        private Button btnStartTask;
        private Button btnGuessCombineAdjustments;
        private Button btnResetCombine;
        private Label lblScrollMethod;
        private ComboBox cbScrollMethod;
        private CheckBox cbAutoDetectScrollEnd;
        private CheckBox cbRemoveDuplicates;
        private CheckBox cbStartCaptureAutomatically;
        private CheckBox cbScrollTopBeforeCapture;
        private Label lblProcessing;
        private Label lblImageCount;
        private Label lblStartDelay;
        private NumericUpDown nudStartDelay;
        private Button btnSelectRectangle;
        private Label lblSelectedRectangle;
        private CheckBox cbAutoCombine;
        private CheckBox cbStartSelectionAutomatically;
        private NumericUpDown nudIgnoreLast;
        private Label lblIgnoreLast;
        private GroupBox gbImages;
        private TextBox txtImagesCount;
    }
}