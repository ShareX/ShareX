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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScrollingCaptureForm));
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
            this.gbAfterCapture = new System.Windows.Forms.GroupBox();
            this.cbAutoClose = new System.Windows.Forms.CheckBox();
            this.cbRemoveDuplicates = new System.Windows.Forms.CheckBox();
            this.cbAutoCombine = new System.Windows.Forms.CheckBox();
            this.chkAutoUpload = new System.Windows.Forms.CheckBox();
            this.gbWhileCapturing = new System.Windows.Forms.GroupBox();
            this.lblScrollMethod = new System.Windows.Forms.Label();
            this.cbScrollMethod = new System.Windows.Forms.ComboBox();
            this.cbAutoDetectScrollEnd = new System.Windows.Forms.CheckBox();
            this.gbBeforeCapture = new System.Windows.Forms.GroupBox();
            this.lblScrollTopMethodBeforeCapture = new System.Windows.Forms.Label();
            this.cbScrollTopMethodBeforeCapture = new System.Windows.Forms.ComboBox();
            this.lblStartDelay = new System.Windows.Forms.Label();
            this.cbStartSelectionAutomatically = new System.Windows.Forms.CheckBox();
            this.nudStartDelay = new System.Windows.Forms.NumericUpDown();
            this.cbStartCaptureAutomatically = new System.Windows.Forms.CheckBox();
            this.btnSelectRectangle = new System.Windows.Forms.Button();
            this.lblNote = new System.Windows.Forms.Label();
            this.lblSelectedRectangle = new System.Windows.Forms.Label();
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
            this.gbAfterCapture.SuspendLayout();
            this.gbWhileCapturing.SuspendLayout();
            this.gbBeforeCapture.SuspendLayout();
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
            resources.ApplyResources(this.btnSelectHandle, "btnSelectHandle");
            this.btnSelectHandle.Name = "btnSelectHandle";
            this.btnSelectHandle.UseVisualStyleBackColor = true;
            this.btnSelectHandle.Click += new System.EventHandler(this.btnSelectHandle_Click);
            // 
            // lblControlText
            // 
            resources.ApplyResources(this.lblControlText, "lblControlText");
            this.lblControlText.Name = "lblControlText";
            // 
            // btnCapture
            // 
            resources.ApplyResources(this.btnCapture, "btnCapture");
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.UseVisualStyleBackColor = true;
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // captureTimer
            // 
            this.captureTimer.Tick += new System.EventHandler(this.captureTimer_Tick);
            // 
            // nudScrollDelay
            // 
            resources.ApplyResources(this.nudScrollDelay, "nudScrollDelay");
            this.nudScrollDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudScrollDelay.Name = "nudScrollDelay";
            this.nudScrollDelay.ValueChanged += new System.EventHandler(this.nudScrollDelay_ValueChanged);
            // 
            // nudMaximumScrollCount
            // 
            resources.ApplyResources(this.nudMaximumScrollCount, "nudMaximumScrollCount");
            this.nudMaximumScrollCount.Name = "nudMaximumScrollCount";
            this.nudMaximumScrollCount.ValueChanged += new System.EventHandler(this.nudMaximumScrollCount_ValueChanged);
            // 
            // lblScrollDelay
            // 
            resources.ApplyResources(this.lblScrollDelay, "lblScrollDelay");
            this.lblScrollDelay.Name = "lblScrollDelay";
            // 
            // lblMaximumScrollCount
            // 
            resources.ApplyResources(this.lblMaximumScrollCount, "lblMaximumScrollCount");
            this.lblMaximumScrollCount.Name = "lblMaximumScrollCount";
            // 
            // tcScrollingCapture
            // 
            this.tcScrollingCapture.Controls.Add(this.tpCapture);
            this.tcScrollingCapture.Controls.Add(this.tpOutput);
            resources.ApplyResources(this.tcScrollingCapture, "tcScrollingCapture");
            this.tcScrollingCapture.Name = "tcScrollingCapture";
            this.tcScrollingCapture.SelectedIndex = 0;
            // 
            // tpCapture
            // 
            this.tpCapture.Controls.Add(this.gbAfterCapture);
            this.tpCapture.Controls.Add(this.gbWhileCapturing);
            this.tpCapture.Controls.Add(this.gbBeforeCapture);
            this.tpCapture.Controls.Add(this.lblNote);
            this.tpCapture.Controls.Add(this.lblSelectedRectangle);
            this.tpCapture.Controls.Add(this.lblControlText);
            this.tpCapture.Controls.Add(this.btnCapture);
            resources.ApplyResources(this.tpCapture, "tpCapture");
            this.tpCapture.Name = "tpCapture";
            this.tpCapture.UseVisualStyleBackColor = true;
            // 
            // gbAfterCapture
            // 
            resources.ApplyResources(this.gbAfterCapture, "gbAfterCapture");
            this.gbAfterCapture.Controls.Add(this.cbAutoClose);
            this.gbAfterCapture.Controls.Add(this.cbRemoveDuplicates);
            this.gbAfterCapture.Controls.Add(this.cbAutoCombine);
            this.gbAfterCapture.Controls.Add(this.chkAutoUpload);
            this.gbAfterCapture.Name = "gbAfterCapture";
            this.gbAfterCapture.TabStop = false;
            // 
            // cbAutoClose
            // 
            resources.ApplyResources(this.cbAutoClose, "cbAutoClose");
            this.cbAutoClose.Name = "cbAutoClose";
            this.cbAutoClose.UseVisualStyleBackColor = true;
            this.cbAutoClose.CheckedChanged += new System.EventHandler(this.cbAutoClose_CheckedChanged);
            // 
            // cbRemoveDuplicates
            // 
            resources.ApplyResources(this.cbRemoveDuplicates, "cbRemoveDuplicates");
            this.cbRemoveDuplicates.Name = "cbRemoveDuplicates";
            this.cbRemoveDuplicates.UseVisualStyleBackColor = true;
            this.cbRemoveDuplicates.CheckedChanged += new System.EventHandler(this.cbRemoveDuplicates_CheckedChanged);
            // 
            // cbAutoCombine
            // 
            resources.ApplyResources(this.cbAutoCombine, "cbAutoCombine");
            this.cbAutoCombine.Name = "cbAutoCombine";
            this.cbAutoCombine.UseVisualStyleBackColor = true;
            this.cbAutoCombine.CheckedChanged += new System.EventHandler(this.cbAutoCombine_CheckedChanged);
            // 
            // chkAutoUpload
            // 
            resources.ApplyResources(this.chkAutoUpload, "chkAutoUpload");
            this.chkAutoUpload.Name = "chkAutoUpload";
            this.chkAutoUpload.UseVisualStyleBackColor = true;
            this.chkAutoUpload.CheckedChanged += new System.EventHandler(this.chkAutoUpload_CheckedChanged);
            // 
            // gbWhileCapturing
            // 
            resources.ApplyResources(this.gbWhileCapturing, "gbWhileCapturing");
            this.gbWhileCapturing.Controls.Add(this.lblScrollDelay);
            this.gbWhileCapturing.Controls.Add(this.nudScrollDelay);
            this.gbWhileCapturing.Controls.Add(this.nudMaximumScrollCount);
            this.gbWhileCapturing.Controls.Add(this.lblMaximumScrollCount);
            this.gbWhileCapturing.Controls.Add(this.lblScrollMethod);
            this.gbWhileCapturing.Controls.Add(this.cbScrollMethod);
            this.gbWhileCapturing.Controls.Add(this.cbAutoDetectScrollEnd);
            this.gbWhileCapturing.Name = "gbWhileCapturing";
            this.gbWhileCapturing.TabStop = false;
            // 
            // lblScrollMethod
            // 
            resources.ApplyResources(this.lblScrollMethod, "lblScrollMethod");
            this.lblScrollMethod.Name = "lblScrollMethod";
            // 
            // cbScrollMethod
            // 
            this.cbScrollMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbScrollMethod.FormattingEnabled = true;
            resources.ApplyResources(this.cbScrollMethod, "cbScrollMethod");
            this.cbScrollMethod.Name = "cbScrollMethod";
            this.cbScrollMethod.SelectedIndexChanged += new System.EventHandler(this.cbScrollMethod_SelectedIndexChanged);
            // 
            // cbAutoDetectScrollEnd
            // 
            resources.ApplyResources(this.cbAutoDetectScrollEnd, "cbAutoDetectScrollEnd");
            this.cbAutoDetectScrollEnd.Name = "cbAutoDetectScrollEnd";
            this.cbAutoDetectScrollEnd.UseVisualStyleBackColor = true;
            this.cbAutoDetectScrollEnd.CheckedChanged += new System.EventHandler(this.cbAutoDetectScrollEnd_CheckedChanged);
            // 
            // gbBeforeCapture
            // 
            resources.ApplyResources(this.gbBeforeCapture, "gbBeforeCapture");
            this.gbBeforeCapture.Controls.Add(this.lblScrollTopMethodBeforeCapture);
            this.gbBeforeCapture.Controls.Add(this.cbScrollTopMethodBeforeCapture);
            this.gbBeforeCapture.Controls.Add(this.lblStartDelay);
            this.gbBeforeCapture.Controls.Add(this.cbStartSelectionAutomatically);
            this.gbBeforeCapture.Controls.Add(this.nudStartDelay);
            this.gbBeforeCapture.Controls.Add(this.cbStartCaptureAutomatically);
            this.gbBeforeCapture.Controls.Add(this.btnSelectHandle);
            this.gbBeforeCapture.Controls.Add(this.btnSelectRectangle);
            this.gbBeforeCapture.Name = "gbBeforeCapture";
            this.gbBeforeCapture.TabStop = false;
            // 
            // lblScrollTopMethodBeforeCapture
            // 
            resources.ApplyResources(this.lblScrollTopMethodBeforeCapture, "lblScrollTopMethodBeforeCapture");
            this.lblScrollTopMethodBeforeCapture.Name = "lblScrollTopMethodBeforeCapture";
            // 
            // cbScrollTopMethodBeforeCapture
            // 
            this.cbScrollTopMethodBeforeCapture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbScrollTopMethodBeforeCapture.FormattingEnabled = true;
            resources.ApplyResources(this.cbScrollTopMethodBeforeCapture, "cbScrollTopMethodBeforeCapture");
            this.cbScrollTopMethodBeforeCapture.Name = "cbScrollTopMethodBeforeCapture";
            this.cbScrollTopMethodBeforeCapture.SelectedIndexChanged += new System.EventHandler(this.cbScrollTopMethodBeforeCapture_SelectedIndexChanged);
            // 
            // lblStartDelay
            // 
            resources.ApplyResources(this.lblStartDelay, "lblStartDelay");
            this.lblStartDelay.Name = "lblStartDelay";
            // 
            // cbStartSelectionAutomatically
            // 
            resources.ApplyResources(this.cbStartSelectionAutomatically, "cbStartSelectionAutomatically");
            this.cbStartSelectionAutomatically.Name = "cbStartSelectionAutomatically";
            this.cbStartSelectionAutomatically.UseVisualStyleBackColor = true;
            this.cbStartSelectionAutomatically.CheckedChanged += new System.EventHandler(this.cbStartSelectionAutomatically_CheckedChanged);
            // 
            // nudStartDelay
            // 
            resources.ApplyResources(this.nudStartDelay, "nudStartDelay");
            this.nudStartDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudStartDelay.Name = "nudStartDelay";
            this.nudStartDelay.ValueChanged += new System.EventHandler(this.nudStartDelay_ValueChanged);
            // 
            // cbStartCaptureAutomatically
            // 
            resources.ApplyResources(this.cbStartCaptureAutomatically, "cbStartCaptureAutomatically");
            this.cbStartCaptureAutomatically.Name = "cbStartCaptureAutomatically";
            this.cbStartCaptureAutomatically.UseVisualStyleBackColor = true;
            this.cbStartCaptureAutomatically.CheckedChanged += new System.EventHandler(this.cbStartCaptureAutomatically_CheckedChanged);
            // 
            // btnSelectRectangle
            // 
            resources.ApplyResources(this.btnSelectRectangle, "btnSelectRectangle");
            this.btnSelectRectangle.Name = "btnSelectRectangle";
            this.btnSelectRectangle.UseVisualStyleBackColor = true;
            this.btnSelectRectangle.Click += new System.EventHandler(this.btnSelectRectangle_Click);
            // 
            // lblNote
            // 
            resources.ApplyResources(this.lblNote, "lblNote");
            this.lblNote.Name = "lblNote";
            // 
            // lblSelectedRectangle
            // 
            resources.ApplyResources(this.lblSelectedRectangle, "lblSelectedRectangle");
            this.lblSelectedRectangle.Name = "lblSelectedRectangle";
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
            resources.ApplyResources(this.tpOutput, "tpOutput");
            this.tpOutput.Name = "tpOutput";
            this.tpOutput.UseVisualStyleBackColor = true;
            // 
            // gbImages
            // 
            this.gbImages.Controls.Add(this.txtImagesCount);
            this.gbImages.Controls.Add(this.lblImageCount);
            this.gbImages.Controls.Add(this.nudIgnoreLast);
            this.gbImages.Controls.Add(this.lblIgnoreLast);
            resources.ApplyResources(this.gbImages, "gbImages");
            this.gbImages.Name = "gbImages";
            this.gbImages.TabStop = false;
            // 
            // txtImagesCount
            // 
            resources.ApplyResources(this.txtImagesCount, "txtImagesCount");
            this.txtImagesCount.Name = "txtImagesCount";
            this.txtImagesCount.ReadOnly = true;
            // 
            // lblImageCount
            // 
            resources.ApplyResources(this.lblImageCount, "lblImageCount");
            this.lblImageCount.Name = "lblImageCount";
            // 
            // nudIgnoreLast
            // 
            resources.ApplyResources(this.nudIgnoreLast, "nudIgnoreLast");
            this.nudIgnoreLast.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudIgnoreLast.Name = "nudIgnoreLast";
            this.nudIgnoreLast.ValueChanged += new System.EventHandler(this.nudIgnoreLast_ValueChanged);
            // 
            // lblIgnoreLast
            // 
            resources.ApplyResources(this.lblIgnoreLast, "lblIgnoreLast");
            this.lblIgnoreLast.Name = "lblIgnoreLast";
            // 
            // btnResetCombine
            // 
            resources.ApplyResources(this.btnResetCombine, "btnResetCombine");
            this.btnResetCombine.Name = "btnResetCombine";
            this.btnResetCombine.UseMnemonic = false;
            this.btnResetCombine.UseVisualStyleBackColor = true;
            this.btnResetCombine.Click += new System.EventHandler(this.btnResetCombine_Click);
            // 
            // btnGuessCombineAdjustments
            // 
            resources.ApplyResources(this.btnGuessCombineAdjustments, "btnGuessCombineAdjustments");
            this.btnGuessCombineAdjustments.Name = "btnGuessCombineAdjustments";
            this.btnGuessCombineAdjustments.UseMnemonic = false;
            this.btnGuessCombineAdjustments.UseVisualStyleBackColor = true;
            this.btnGuessCombineAdjustments.Click += new System.EventHandler(this.btnGuessCombineAdjustments_Click);
            // 
            // btnStartTask
            // 
            resources.ApplyResources(this.btnStartTask, "btnStartTask");
            this.btnStartTask.Name = "btnStartTask";
            this.btnStartTask.UseVisualStyleBackColor = true;
            this.btnStartTask.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // btnGuessEdges
            // 
            resources.ApplyResources(this.btnGuessEdges, "btnGuessEdges");
            this.btnGuessEdges.Name = "btnGuessEdges";
            this.btnGuessEdges.UseVisualStyleBackColor = true;
            this.btnGuessEdges.Click += new System.EventHandler(this.btnGuessEdges_Click);
            // 
            // gbCombineAdjustments
            // 
            this.gbCombineAdjustments.Controls.Add(this.lblCombineLastVertical);
            this.gbCombineAdjustments.Controls.Add(this.lblCombineVertical);
            this.gbCombineAdjustments.Controls.Add(this.nudCombineVertical);
            this.gbCombineAdjustments.Controls.Add(this.nudCombineLastVertical);
            resources.ApplyResources(this.gbCombineAdjustments, "gbCombineAdjustments");
            this.gbCombineAdjustments.Name = "gbCombineAdjustments";
            this.gbCombineAdjustments.TabStop = false;
            // 
            // lblCombineLastVertical
            // 
            resources.ApplyResources(this.lblCombineLastVertical, "lblCombineLastVertical");
            this.lblCombineLastVertical.Name = "lblCombineLastVertical";
            // 
            // lblCombineVertical
            // 
            resources.ApplyResources(this.lblCombineVertical, "lblCombineVertical");
            this.lblCombineVertical.Name = "lblCombineVertical";
            // 
            // nudCombineVertical
            // 
            resources.ApplyResources(this.nudCombineVertical, "nudCombineVertical");
            this.nudCombineVertical.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudCombineVertical.Name = "nudCombineVertical";
            this.nudCombineVertical.ValueChanged += new System.EventHandler(this.nudCombineVertical_ValueChanged);
            // 
            // nudCombineLastVertical
            // 
            resources.ApplyResources(this.nudCombineLastVertical, "nudCombineLastVertical");
            this.nudCombineLastVertical.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudCombineLastVertical.Name = "nudCombineLastVertical";
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
            resources.ApplyResources(this.gbTrimEdges, "gbTrimEdges");
            this.gbTrimEdges.Name = "gbTrimEdges";
            this.gbTrimEdges.TabStop = false;
            // 
            // lblTrimBottom
            // 
            resources.ApplyResources(this.lblTrimBottom, "lblTrimBottom");
            this.lblTrimBottom.Name = "lblTrimBottom";
            // 
            // lblTrimRight
            // 
            resources.ApplyResources(this.lblTrimRight, "lblTrimRight");
            this.lblTrimRight.Name = "lblTrimRight";
            // 
            // lblTrimTop
            // 
            resources.ApplyResources(this.lblTrimTop, "lblTrimTop");
            this.lblTrimTop.Name = "lblTrimTop";
            // 
            // lblTrimLeft
            // 
            resources.ApplyResources(this.lblTrimLeft, "lblTrimLeft");
            this.lblTrimLeft.Name = "lblTrimLeft";
            // 
            // nudTrimLeft
            // 
            resources.ApplyResources(this.nudTrimLeft, "nudTrimLeft");
            this.nudTrimLeft.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudTrimLeft.Name = "nudTrimLeft";
            this.nudTrimLeft.ValueChanged += new System.EventHandler(this.nudTrimLeft_ValueChanged);
            // 
            // nudTrimBottom
            // 
            resources.ApplyResources(this.nudTrimBottom, "nudTrimBottom");
            this.nudTrimBottom.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudTrimBottom.Name = "nudTrimBottom";
            this.nudTrimBottom.ValueChanged += new System.EventHandler(this.nudTrimBottom_ValueChanged);
            // 
            // nudTrimTop
            // 
            resources.ApplyResources(this.nudTrimTop, "nudTrimTop");
            this.nudTrimTop.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudTrimTop.Name = "nudTrimTop";
            this.nudTrimTop.ValueChanged += new System.EventHandler(this.nudTrimTop_ValueChanged);
            // 
            // nudTrimRight
            // 
            resources.ApplyResources(this.nudTrimRight, "nudTrimRight");
            this.nudTrimRight.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudTrimRight.Name = "nudTrimRight";
            this.nudTrimRight.ValueChanged += new System.EventHandler(this.nudTrimRight_ValueChanged);
            // 
            // pOutput
            // 
            resources.ApplyResources(this.pOutput, "pOutput");
            this.pOutput.Controls.Add(this.lblProcessing);
            this.pOutput.Controls.Add(this.pbOutput);
            this.pOutput.Name = "pOutput";
            // 
            // lblProcessing
            // 
            resources.ApplyResources(this.lblProcessing, "lblProcessing");
            this.lblProcessing.Name = "lblProcessing";
            // 
            // pbOutput
            // 
            resources.ApplyResources(this.pbOutput, "pbOutput");
            this.pbOutput.Name = "pbOutput";
            this.pbOutput.TabStop = false;
            // 
            // ScrollingCaptureForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tcScrollingCapture);
            this.Name = "ScrollingCaptureForm";
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaximumScrollCount)).EndInit();
            this.tcScrollingCapture.ResumeLayout(false);
            this.tpCapture.ResumeLayout(false);
            this.tpCapture.PerformLayout();
            this.gbAfterCapture.ResumeLayout(false);
            this.gbAfterCapture.PerformLayout();
            this.gbWhileCapturing.ResumeLayout(false);
            this.gbWhileCapturing.PerformLayout();
            this.gbBeforeCapture.ResumeLayout(false);
            this.gbBeforeCapture.PerformLayout();
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
        private Label lblNote;
        private CheckBox chkAutoUpload;
        private Label lblScrollTopMethodBeforeCapture;
        private ComboBox cbScrollTopMethodBeforeCapture;
        private GroupBox gbAfterCapture;
        private GroupBox gbWhileCapturing;
        private GroupBox gbBeforeCapture;
        private CheckBox cbAutoClose;
    }
}