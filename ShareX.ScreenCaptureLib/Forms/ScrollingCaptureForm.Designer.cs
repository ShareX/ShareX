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
            this.tpOutput = new System.Windows.Forms.TabPage();
            this.btnProcess = new System.Windows.Forms.Button();
            this.btnGuessEdges = new System.Windows.Forms.Button();
            this.btnCombine = new System.Windows.Forms.Button();
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
            this.pbOutput = new System.Windows.Forms.PictureBox();
            this.btnGuessCombineAdjustments = new System.Windows.Forms.Button();
            this.btnResetCombine = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaximumScrollCount)).BeginInit();
            this.tcScrollingCapture.SuspendLayout();
            this.tpCapture.SuspendLayout();
            this.tpOutput.SuspendLayout();
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
            this.btnSelectHandle.Location = new System.Drawing.Point(12, 10);
            this.btnSelectHandle.Name = "btnSelectHandle";
            this.btnSelectHandle.Size = new System.Drawing.Size(304, 23);
            this.btnSelectHandle.TabIndex = 0;
            this.btnSelectHandle.Text = "Select window or control to scroll";
            this.btnSelectHandle.UseVisualStyleBackColor = true;
            this.btnSelectHandle.Click += new System.EventHandler(this.btnSelectHandle_Click);
            // 
            // lblControlText
            // 
            this.lblControlText.Location = new System.Drawing.Point(324, 15);
            this.lblControlText.Name = "lblControlText";
            this.lblControlText.Size = new System.Drawing.Size(240, 19);
            this.lblControlText.TabIndex = 1;
            this.lblControlText.Text = "Text";
            // 
            // btnCapture
            // 
            this.btnCapture.Enabled = false;
            this.btnCapture.Location = new System.Drawing.Point(12, 106);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(152, 23);
            this.btnCapture.TabIndex = 2;
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
            this.nudScrollDelay.Location = new System.Drawing.Point(140, 46);
            this.nudScrollDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudScrollDelay.Name = "nudScrollDelay";
            this.nudScrollDelay.Size = new System.Drawing.Size(80, 20);
            this.nudScrollDelay.TabIndex = 3;
            this.nudScrollDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudScrollDelay.ValueChanged += new System.EventHandler(this.nudScrollDelay_ValueChanged);
            // 
            // nudMaximumScrollCount
            // 
            this.nudMaximumScrollCount.Location = new System.Drawing.Point(140, 70);
            this.nudMaximumScrollCount.Name = "nudMaximumScrollCount";
            this.nudMaximumScrollCount.Size = new System.Drawing.Size(80, 20);
            this.nudMaximumScrollCount.TabIndex = 4;
            this.nudMaximumScrollCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudMaximumScrollCount.ValueChanged += new System.EventHandler(this.nudMaximumScrollCount_ValueChanged);
            // 
            // lblScrollDelay
            // 
            this.lblScrollDelay.AutoSize = true;
            this.lblScrollDelay.Location = new System.Drawing.Point(12, 50);
            this.lblScrollDelay.Name = "lblScrollDelay";
            this.lblScrollDelay.Size = new System.Drawing.Size(64, 13);
            this.lblScrollDelay.TabIndex = 5;
            this.lblScrollDelay.Text = "Scroll delay:";
            // 
            // lblMaximumScrollCount
            // 
            this.lblMaximumScrollCount.AutoSize = true;
            this.lblMaximumScrollCount.Location = new System.Drawing.Point(12, 74);
            this.lblMaximumScrollCount.Name = "lblMaximumScrollCount";
            this.lblMaximumScrollCount.Size = new System.Drawing.Size(111, 13);
            this.lblMaximumScrollCount.TabIndex = 6;
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
            this.tcScrollingCapture.Size = new System.Drawing.Size(935, 689);
            this.tcScrollingCapture.TabIndex = 7;
            // 
            // tpCapture
            // 
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
            this.tpCapture.Size = new System.Drawing.Size(927, 663);
            this.tpCapture.TabIndex = 0;
            this.tpCapture.Text = "Capture";
            this.tpCapture.UseVisualStyleBackColor = true;
            // 
            // tpOutput
            // 
            this.tpOutput.Controls.Add(this.btnResetCombine);
            this.tpOutput.Controls.Add(this.btnGuessCombineAdjustments);
            this.tpOutput.Controls.Add(this.btnProcess);
            this.tpOutput.Controls.Add(this.btnGuessEdges);
            this.tpOutput.Controls.Add(this.btnCombine);
            this.tpOutput.Controls.Add(this.gbCombineAdjustments);
            this.tpOutput.Controls.Add(this.gbTrimEdges);
            this.tpOutput.Controls.Add(this.pOutput);
            this.tpOutput.Location = new System.Drawing.Point(4, 22);
            this.tpOutput.Name = "tpOutput";
            this.tpOutput.Padding = new System.Windows.Forms.Padding(3);
            this.tpOutput.Size = new System.Drawing.Size(927, 663);
            this.tpOutput.TabIndex = 1;
            this.tpOutput.Text = "Output";
            this.tpOutput.UseVisualStyleBackColor = true;
            // 
            // btnProcess
            // 
            this.btnProcess.Location = new System.Drawing.Point(312, 88);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(376, 23);
            this.btnProcess.TabIndex = 9;
            this.btnProcess.Text = "4. Upload/save depending on after capture settings";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // btnGuessEdges
            // 
            this.btnGuessEdges.Enabled = false;
            this.btnGuessEdges.Location = new System.Drawing.Point(312, 16);
            this.btnGuessEdges.Name = "btnGuessEdges";
            this.btnGuessEdges.Size = new System.Drawing.Size(376, 23);
            this.btnGuessEdges.TabIndex = 8;
            this.btnGuessEdges.Text = "1. Guess edge values to trim";
            this.btnGuessEdges.UseVisualStyleBackColor = true;
            this.btnGuessEdges.Click += new System.EventHandler(this.btnGuessEdges_Click);
            // 
            // btnCombine
            // 
            this.btnCombine.Enabled = false;
            this.btnCombine.Location = new System.Drawing.Point(312, 64);
            this.btnCombine.Name = "btnCombine";
            this.btnCombine.Size = new System.Drawing.Size(376, 23);
            this.btnCombine.TabIndex = 8;
            this.btnCombine.Text = "3. Combine images using these settings";
            this.btnCombine.UseVisualStyleBackColor = true;
            this.btnCombine.Click += new System.EventHandler(this.btnCombine_Click);
            // 
            // gbCombineAdjustments
            // 
            this.gbCombineAdjustments.Controls.Add(this.lblCombineLastVertical);
            this.gbCombineAdjustments.Controls.Add(this.lblCombineVertical);
            this.gbCombineAdjustments.Controls.Add(this.nudCombineVertical);
            this.gbCombineAdjustments.Controls.Add(this.nudCombineLastVertical);
            this.gbCombineAdjustments.Location = new System.Drawing.Point(144, 8);
            this.gbCombineAdjustments.Name = "gbCombineAdjustments";
            this.gbCombineAdjustments.Size = new System.Drawing.Size(152, 120);
            this.gbCombineAdjustments.TabIndex = 7;
            this.gbCombineAdjustments.TabStop = false;
            this.gbCombineAdjustments.Text = "Combine adjustments";
            // 
            // lblCombineLastVertical
            // 
            this.lblCombineLastVertical.AutoSize = true;
            this.lblCombineLastVertical.Location = new System.Drawing.Point(8, 44);
            this.lblCombineLastVertical.Name = "lblCombineLastVertical";
            this.lblCombineLastVertical.Size = new System.Drawing.Size(67, 13);
            this.lblCombineLastVertical.TabIndex = 16;
            this.lblCombineLastVertical.Text = "Last vertical:";
            // 
            // lblCombineVertical
            // 
            this.lblCombineVertical.AutoSize = true;
            this.lblCombineVertical.Location = new System.Drawing.Point(8, 20);
            this.lblCombineVertical.Name = "lblCombineVertical";
            this.lblCombineVertical.Size = new System.Drawing.Size(45, 13);
            this.lblCombineVertical.TabIndex = 15;
            this.lblCombineVertical.Text = "Vertical:";
            // 
            // nudCombineVertical
            // 
            this.nudCombineVertical.Location = new System.Drawing.Point(88, 16);
            this.nudCombineVertical.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudCombineVertical.Name = "nudCombineVertical";
            this.nudCombineVertical.Size = new System.Drawing.Size(56, 20);
            this.nudCombineVertical.TabIndex = 8;
            this.nudCombineVertical.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudCombineVertical.ValueChanged += new System.EventHandler(this.nudCombineVertical_ValueChanged);
            // 
            // nudCombineLastVertical
            // 
            this.nudCombineLastVertical.Location = new System.Drawing.Point(88, 40);
            this.nudCombineLastVertical.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudCombineLastVertical.Name = "nudCombineLastVertical";
            this.nudCombineLastVertical.Size = new System.Drawing.Size(56, 20);
            this.nudCombineLastVertical.TabIndex = 9;
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
            this.gbTrimEdges.Location = new System.Drawing.Point(8, 8);
            this.gbTrimEdges.Name = "gbTrimEdges";
            this.gbTrimEdges.Size = new System.Drawing.Size(128, 120);
            this.gbTrimEdges.TabIndex = 6;
            this.gbTrimEdges.TabStop = false;
            this.gbTrimEdges.Text = "Trim edges";
            // 
            // lblTrimBottom
            // 
            this.lblTrimBottom.AutoSize = true;
            this.lblTrimBottom.Location = new System.Drawing.Point(8, 92);
            this.lblTrimBottom.Name = "lblTrimBottom";
            this.lblTrimBottom.Size = new System.Drawing.Size(43, 13);
            this.lblTrimBottom.TabIndex = 7;
            this.lblTrimBottom.Text = "Bottom:";
            // 
            // lblTrimRight
            // 
            this.lblTrimRight.AutoSize = true;
            this.lblTrimRight.Location = new System.Drawing.Point(8, 68);
            this.lblTrimRight.Name = "lblTrimRight";
            this.lblTrimRight.Size = new System.Drawing.Size(35, 13);
            this.lblTrimRight.TabIndex = 7;
            this.lblTrimRight.Text = "Right:";
            // 
            // lblTrimTop
            // 
            this.lblTrimTop.AutoSize = true;
            this.lblTrimTop.Location = new System.Drawing.Point(8, 44);
            this.lblTrimTop.Name = "lblTrimTop";
            this.lblTrimTop.Size = new System.Drawing.Size(29, 13);
            this.lblTrimTop.TabIndex = 7;
            this.lblTrimTop.Text = "Top:";
            // 
            // lblTrimLeft
            // 
            this.lblTrimLeft.AutoSize = true;
            this.lblTrimLeft.Location = new System.Drawing.Point(8, 20);
            this.lblTrimLeft.Name = "lblTrimLeft";
            this.lblTrimLeft.Size = new System.Drawing.Size(28, 13);
            this.lblTrimLeft.TabIndex = 7;
            this.lblTrimLeft.Text = "Left:";
            // 
            // nudTrimLeft
            // 
            this.nudTrimLeft.Location = new System.Drawing.Point(64, 16);
            this.nudTrimLeft.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudTrimLeft.Name = "nudTrimLeft";
            this.nudTrimLeft.Size = new System.Drawing.Size(56, 20);
            this.nudTrimLeft.TabIndex = 2;
            this.nudTrimLeft.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudTrimLeft.ValueChanged += new System.EventHandler(this.nudTrimLeft_ValueChanged);
            // 
            // nudTrimBottom
            // 
            this.nudTrimBottom.Location = new System.Drawing.Point(64, 88);
            this.nudTrimBottom.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudTrimBottom.Name = "nudTrimBottom";
            this.nudTrimBottom.Size = new System.Drawing.Size(56, 20);
            this.nudTrimBottom.TabIndex = 5;
            this.nudTrimBottom.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudTrimBottom.ValueChanged += new System.EventHandler(this.nudTrimBottom_ValueChanged);
            // 
            // nudTrimTop
            // 
            this.nudTrimTop.Location = new System.Drawing.Point(64, 40);
            this.nudTrimTop.Maximum = new decimal(new int[] {
            1000,
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
            this.nudTrimRight.Location = new System.Drawing.Point(64, 64);
            this.nudTrimRight.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudTrimRight.Name = "nudTrimRight";
            this.nudTrimRight.Size = new System.Drawing.Size(56, 20);
            this.nudTrimRight.TabIndex = 4;
            this.nudTrimRight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudTrimRight.ValueChanged += new System.EventHandler(this.nudTrimRight_ValueChanged);
            // 
            // pOutput
            // 
            this.pOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pOutput.AutoScroll = true;
            this.pOutput.Controls.Add(this.pbOutput);
            this.pOutput.Location = new System.Drawing.Point(8, 136);
            this.pOutput.Name = "pOutput";
            this.pOutput.Size = new System.Drawing.Size(912, 518);
            this.pOutput.TabIndex = 1;
            // 
            // pbOutput
            // 
            this.pbOutput.Location = new System.Drawing.Point(0, 0);
            this.pbOutput.Name = "pbOutput";
            this.pbOutput.Size = new System.Drawing.Size(100, 100);
            this.pbOutput.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbOutput.TabIndex = 0;
            this.pbOutput.TabStop = false;
            // 
            // btnGuessCombineAdjustments
            // 
            this.btnGuessCombineAdjustments.Enabled = false;
            this.btnGuessCombineAdjustments.Location = new System.Drawing.Point(312, 40);
            this.btnGuessCombineAdjustments.Name = "btnGuessCombineAdjustments";
            this.btnGuessCombineAdjustments.Size = new System.Drawing.Size(376, 23);
            this.btnGuessCombineAdjustments.TabIndex = 10;
            this.btnGuessCombineAdjustments.Text = "2. Guess combine adjustments";
            this.btnGuessCombineAdjustments.UseVisualStyleBackColor = true;
            this.btnGuessCombineAdjustments.Click += new System.EventHandler(this.btnGuessCombineAdjustments_Click);
            // 
            // btnResetCombine
            // 
            this.btnResetCombine.Location = new System.Drawing.Point(696, 16);
            this.btnResetCombine.Name = "btnResetCombine";
            this.btnResetCombine.Size = new System.Drawing.Size(75, 23);
            this.btnResetCombine.TabIndex = 11;
            this.btnResetCombine.Text = "Reset";
            this.btnResetCombine.UseVisualStyleBackColor = true;
            this.btnResetCombine.Click += new System.EventHandler(this.btnResetCombine_Click);
            // 
            // ScrollingCaptureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(935, 689);
            this.Controls.Add(this.tcScrollingCapture);
            this.Name = "ScrollingCaptureForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Scrolling capture";
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaximumScrollCount)).EndInit();
            this.tcScrollingCapture.ResumeLayout(false);
            this.tpCapture.ResumeLayout(false);
            this.tpCapture.PerformLayout();
            this.tpOutput.ResumeLayout(false);
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
        private Button btnCombine;
        private Label lblCombineLastVertical;
        private Button btnGuessEdges;
        private Button btnProcess;
        private Button btnGuessCombineAdjustments;
        private Button btnResetCombine;
    }
}