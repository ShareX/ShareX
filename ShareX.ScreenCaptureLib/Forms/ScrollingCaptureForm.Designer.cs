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
            this.components = new Container();
            this.btnSelectHandle = new Button();
            this.lblControlText = new Label();
            this.btnCapture = new Button();
            this.captureTimer = new Timer(this.components);
            this.nudScrollDelay = new NumericUpDown();
            this.nudMaximumScrollCount = new NumericUpDown();
            this.lblScrollDelay = new Label();
            this.lblMaximumScrollCount = new Label();
            this.tcScrollingCapture = new TabControl();
            this.tpCapture = new TabPage();
            this.tpOutput = new TabPage();
            this.pbOutput = new PictureBox();
            this.pOutput = new Panel();
            ((ISupportInitialize)(this.nudScrollDelay)).BeginInit();
            ((ISupportInitialize)(this.nudMaximumScrollCount)).BeginInit();
            this.tcScrollingCapture.SuspendLayout();
            this.tpCapture.SuspendLayout();
            this.tpOutput.SuspendLayout();
            ((ISupportInitialize)(this.pbOutput)).BeginInit();
            this.pOutput.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSelectHandle
            // 
            this.btnSelectHandle.Location = new Point(12, 10);
            this.btnSelectHandle.Name = "btnSelectHandle";
            this.btnSelectHandle.Size = new Size(304, 23);
            this.btnSelectHandle.TabIndex = 0;
            this.btnSelectHandle.Text = "Select window or control to scroll";
            this.btnSelectHandle.UseVisualStyleBackColor = true;
            this.btnSelectHandle.Click += new EventHandler(this.btnSelectHandle_Click);
            // 
            // lblControlText
            // 
            this.lblControlText.Location = new Point(324, 15);
            this.lblControlText.Name = "lblControlText";
            this.lblControlText.Size = new Size(240, 19);
            this.lblControlText.TabIndex = 1;
            this.lblControlText.Text = "Text";
            // 
            // btnCapture
            // 
            this.btnCapture.Enabled = false;
            this.btnCapture.Location = new Point(12, 106);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new Size(152, 23);
            this.btnCapture.TabIndex = 2;
            this.btnCapture.Text = "Start capture";
            this.btnCapture.UseVisualStyleBackColor = true;
            this.btnCapture.Click += new EventHandler(this.btnCapture_Click);
            // 
            // captureTimer
            // 
            this.captureTimer.Tick += new EventHandler(this.captureTimer_Tick);
            // 
            // nudScrollDelay
            // 
            this.nudScrollDelay.Location = new Point(140, 46);
            this.nudScrollDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudScrollDelay.Name = "nudScrollDelay";
            this.nudScrollDelay.Size = new Size(80, 20);
            this.nudScrollDelay.TabIndex = 3;
            this.nudScrollDelay.TextAlign = HorizontalAlignment.Center;
            this.nudScrollDelay.ValueChanged += new EventHandler(this.nudScrollDelay_ValueChanged);
            // 
            // nudMaximumScrollCount
            // 
            this.nudMaximumScrollCount.Location = new Point(140, 70);
            this.nudMaximumScrollCount.Name = "nudMaximumScrollCount";
            this.nudMaximumScrollCount.Size = new Size(80, 20);
            this.nudMaximumScrollCount.TabIndex = 4;
            this.nudMaximumScrollCount.TextAlign = HorizontalAlignment.Center;
            this.nudMaximumScrollCount.ValueChanged += new EventHandler(this.nudMaximumScrollCount_ValueChanged);
            // 
            // lblScrollDelay
            // 
            this.lblScrollDelay.AutoSize = true;
            this.lblScrollDelay.Location = new Point(12, 50);
            this.lblScrollDelay.Name = "lblScrollDelay";
            this.lblScrollDelay.Size = new Size(64, 13);
            this.lblScrollDelay.TabIndex = 5;
            this.lblScrollDelay.Text = "Scroll delay:";
            // 
            // lblMaximumScrollCount
            // 
            this.lblMaximumScrollCount.AutoSize = true;
            this.lblMaximumScrollCount.Location = new Point(12, 74);
            this.lblMaximumScrollCount.Name = "lblMaximumScrollCount";
            this.lblMaximumScrollCount.Size = new Size(111, 13);
            this.lblMaximumScrollCount.TabIndex = 6;
            this.lblMaximumScrollCount.Text = "Maximum scroll count:";
            // 
            // tcScrollingCapture
            // 
            this.tcScrollingCapture.Controls.Add(this.tpCapture);
            this.tcScrollingCapture.Controls.Add(this.tpOutput);
            this.tcScrollingCapture.Dock = DockStyle.Fill;
            this.tcScrollingCapture.Location = new Point(0, 0);
            this.tcScrollingCapture.Name = "tcScrollingCapture";
            this.tcScrollingCapture.SelectedIndex = 0;
            this.tcScrollingCapture.Size = new Size(567, 427);
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
            this.tpCapture.Location = new Point(4, 22);
            this.tpCapture.Name = "tpCapture";
            this.tpCapture.Padding = new Padding(3);
            this.tpCapture.Size = new Size(559, 401);
            this.tpCapture.TabIndex = 0;
            this.tpCapture.Text = "Capture";
            this.tpCapture.UseVisualStyleBackColor = true;
            // 
            // tpOutput
            // 
            this.tpOutput.Controls.Add(this.pOutput);
            this.tpOutput.Location = new Point(4, 22);
            this.tpOutput.Name = "tpOutput";
            this.tpOutput.Padding = new Padding(3);
            this.tpOutput.Size = new Size(559, 401);
            this.tpOutput.TabIndex = 1;
            this.tpOutput.Text = "Output";
            this.tpOutput.UseVisualStyleBackColor = true;
            // 
            // pbOutput
            // 
            this.pbOutput.Location = new Point(0, 0);
            this.pbOutput.Name = "pbOutput";
            this.pbOutput.Size = new Size(100, 100);
            this.pbOutput.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pbOutput.TabIndex = 0;
            this.pbOutput.TabStop = false;
            // 
            // pOutput
            // 
            this.pOutput.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.pOutput.AutoScroll = true;
            this.pOutput.Controls.Add(this.pbOutput);
            this.pOutput.Location = new Point(8, 8);
            this.pOutput.Name = "pOutput";
            this.pOutput.Size = new Size(544, 384);
            this.pOutput.TabIndex = 1;
            // 
            // ScrollingCaptureForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(567, 427);
            this.Controls.Add(this.tcScrollingCapture);
            this.Name = "ScrollingCaptureForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "ShareX - Scrolling capture";
            ((ISupportInitialize)(this.nudScrollDelay)).EndInit();
            ((ISupportInitialize)(this.nudMaximumScrollCount)).EndInit();
            this.tcScrollingCapture.ResumeLayout(false);
            this.tpCapture.ResumeLayout(false);
            this.tpCapture.PerformLayout();
            this.tpOutput.ResumeLayout(false);
            ((ISupportInitialize)(this.pbOutput)).EndInit();
            this.pOutput.ResumeLayout(false);
            this.pOutput.PerformLayout();
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
    }
}