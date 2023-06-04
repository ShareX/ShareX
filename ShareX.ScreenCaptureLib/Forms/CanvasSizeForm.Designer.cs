namespace ShareX.ScreenCaptureLib
{
    partial class CanvasSizeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CanvasSizeForm));
            this.nudLeft = new System.Windows.Forms.NumericUpDown();
            this.lblLeft = new System.Windows.Forms.Label();
            this.lblRight = new System.Windows.Forms.Label();
            this.nudRight = new System.Windows.Forms.NumericUpDown();
            this.lblTop = new System.Windows.Forms.Label();
            this.nudTop = new System.Windows.Forms.NumericUpDown();
            this.lblBottom = new System.Windows.Forms.Label();
            this.nudBottom = new System.Windows.Forms.NumericUpDown();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbtnCanvasColor = new ShareX.HelpersLib.ColorButton();
            ((System.ComponentModel.ISupportInitialize)(this.nudLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBottom)).BeginInit();
            this.SuspendLayout();
            // 
            // nudLeft
            // 
            resources.ApplyResources(this.nudLeft, "nudLeft");
            this.nudLeft.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudLeft.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.nudLeft.Name = "nudLeft";
            // 
            // lblLeft
            // 
            resources.ApplyResources(this.lblLeft, "lblLeft");
            this.lblLeft.Name = "lblLeft";
            // 
            // lblRight
            // 
            resources.ApplyResources(this.lblRight, "lblRight");
            this.lblRight.Name = "lblRight";
            // 
            // nudRight
            // 
            resources.ApplyResources(this.nudRight, "nudRight");
            this.nudRight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudRight.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.nudRight.Name = "nudRight";
            // 
            // lblTop
            // 
            resources.ApplyResources(this.lblTop, "lblTop");
            this.lblTop.Name = "lblTop";
            // 
            // nudTop
            // 
            resources.ApplyResources(this.nudTop, "nudTop");
            this.nudTop.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudTop.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.nudTop.Name = "nudTop";
            // 
            // lblBottom
            // 
            resources.ApplyResources(this.lblBottom, "lblBottom");
            this.lblBottom.Name = "lblBottom";
            // 
            // nudBottom
            // 
            resources.ApplyResources(this.nudBottom, "nudBottom");
            this.nudBottom.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudBottom.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.nudBottom.Name = "nudBottom";
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cbtnCanvasColor
            // 
            this.cbtnCanvasColor.Color = System.Drawing.Color.Transparent;
            this.cbtnCanvasColor.ColorPickerOptions = null;
            resources.ApplyResources(this.cbtnCanvasColor, "cbtnCanvasColor");
            this.cbtnCanvasColor.Name = "cbtnCanvasColor";
            this.cbtnCanvasColor.UseVisualStyleBackColor = true;
            // 
            // CanvasSizeForm
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.cbtnCanvasColor);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblBottom);
            this.Controls.Add(this.nudBottom);
            this.Controls.Add(this.lblTop);
            this.Controls.Add(this.nudTop);
            this.Controls.Add(this.lblRight);
            this.Controls.Add(this.nudRight);
            this.Controls.Add(this.lblLeft);
            this.Controls.Add(this.nudLeft);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CanvasSizeForm";
            this.Shown += new System.EventHandler(this.CanvasSizeForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.nudLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBottom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nudLeft;
        private System.Windows.Forms.Label lblLeft;
        private System.Windows.Forms.Label lblRight;
        private System.Windows.Forms.NumericUpDown nudRight;
        private System.Windows.Forms.Label lblTop;
        private System.Windows.Forms.NumericUpDown nudTop;
        private System.Windows.Forms.Label lblBottom;
        private System.Windows.Forms.NumericUpDown nudBottom;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private HelpersLib.ColorButton cbtnCanvasColor;
    }
}