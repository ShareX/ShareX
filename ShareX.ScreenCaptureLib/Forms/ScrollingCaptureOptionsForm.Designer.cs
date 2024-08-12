namespace ShareX.ScreenCaptureLib
{
    partial class ScrollingCaptureOptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScrollingCaptureOptionsForm));
            this.lblStartDelay = new System.Windows.Forms.Label();
            this.nudStartDelay = new System.Windows.Forms.NumericUpDown();
            this.lblScrollDelay = new System.Windows.Forms.Label();
            this.nudScrollDelay = new System.Windows.Forms.NumericUpDown();
            this.cbAutoScrollTop = new System.Windows.Forms.CheckBox();
            this.lblScrollAmount = new System.Windows.Forms.Label();
            this.nudScrollAmount = new System.Windows.Forms.NumericUpDown();
            this.cbAutoUpload = new System.Windows.Forms.CheckBox();
            this.lblStartDelayHint = new System.Windows.Forms.Label();
            this.lblScrollDelayHint = new System.Windows.Forms.Label();
            this.lblScrollAmountHint = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbShowRegion = new System.Windows.Forms.CheckBox();
            this.cbAutoIgnoreBottomEdge = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollAmount)).BeginInit();
            this.SuspendLayout();
            // 
            // lblStartDelay
            // 
            resources.ApplyResources(this.lblStartDelay, "lblStartDelay");
            this.lblStartDelay.Name = "lblStartDelay";
            // 
            // nudStartDelay
            // 
            this.nudStartDelay.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nudStartDelay, "nudStartDelay");
            this.nudStartDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudStartDelay.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudStartDelay.Name = "nudStartDelay";
            this.nudStartDelay.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // lblScrollDelay
            // 
            resources.ApplyResources(this.lblScrollDelay, "lblScrollDelay");
            this.lblScrollDelay.Name = "lblScrollDelay";
            // 
            // nudScrollDelay
            // 
            this.nudScrollDelay.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nudScrollDelay, "nudScrollDelay");
            this.nudScrollDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudScrollDelay.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudScrollDelay.Name = "nudScrollDelay";
            this.nudScrollDelay.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // cbAutoScrollTop
            // 
            resources.ApplyResources(this.cbAutoScrollTop, "cbAutoScrollTop");
            this.cbAutoScrollTop.Name = "cbAutoScrollTop";
            this.cbAutoScrollTop.UseVisualStyleBackColor = true;
            // 
            // lblScrollAmount
            // 
            resources.ApplyResources(this.lblScrollAmount, "lblScrollAmount");
            this.lblScrollAmount.Name = "lblScrollAmount";
            // 
            // nudScrollAmount
            // 
            resources.ApplyResources(this.nudScrollAmount, "nudScrollAmount");
            this.nudScrollAmount.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudScrollAmount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudScrollAmount.Name = "nudScrollAmount";
            this.nudScrollAmount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cbAutoUpload
            // 
            resources.ApplyResources(this.cbAutoUpload, "cbAutoUpload");
            this.cbAutoUpload.Name = "cbAutoUpload";
            this.cbAutoUpload.UseVisualStyleBackColor = true;
            // 
            // lblStartDelayHint
            // 
            resources.ApplyResources(this.lblStartDelayHint, "lblStartDelayHint");
            this.lblStartDelayHint.Name = "lblStartDelayHint";
            // 
            // lblScrollDelayHint
            // 
            resources.ApplyResources(this.lblScrollDelayHint, "lblScrollDelayHint");
            this.lblScrollDelayHint.Name = "lblScrollDelayHint";
            // 
            // lblScrollAmountHint
            // 
            resources.ApplyResources(this.lblScrollAmountHint, "lblScrollAmountHint");
            this.lblScrollAmountHint.Name = "lblScrollAmountHint";
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
            // cbShowRegion
            // 
            resources.ApplyResources(this.cbShowRegion, "cbShowRegion");
            this.cbShowRegion.Name = "cbShowRegion";
            this.cbShowRegion.UseVisualStyleBackColor = true;
            // 
            // cbAutoIgnoreBottomEdge
            // 
            resources.ApplyResources(this.cbAutoIgnoreBottomEdge, "cbAutoIgnoreBottomEdge");
            this.cbAutoIgnoreBottomEdge.Name = "cbAutoIgnoreBottomEdge";
            this.cbAutoIgnoreBottomEdge.UseVisualStyleBackColor = true;
            // 
            // ScrollingCaptureOptionsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.cbAutoIgnoreBottomEdge);
            this.Controls.Add(this.cbShowRegion);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblScrollAmountHint);
            this.Controls.Add(this.lblScrollDelayHint);
            this.Controls.Add(this.lblStartDelayHint);
            this.Controls.Add(this.cbAutoUpload);
            this.Controls.Add(this.nudScrollAmount);
            this.Controls.Add(this.lblScrollAmount);
            this.Controls.Add(this.cbAutoScrollTop);
            this.Controls.Add(this.nudScrollDelay);
            this.Controls.Add(this.lblScrollDelay);
            this.Controls.Add(this.nudStartDelay);
            this.Controls.Add(this.lblStartDelay);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ScrollingCaptureOptionsForm";
            ((System.ComponentModel.ISupportInitialize)(this.nudStartDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollAmount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStartDelay;
        private System.Windows.Forms.NumericUpDown nudStartDelay;
        private System.Windows.Forms.Label lblScrollDelay;
        private System.Windows.Forms.NumericUpDown nudScrollDelay;
        private System.Windows.Forms.CheckBox cbAutoScrollTop;
        private System.Windows.Forms.Label lblScrollAmount;
        private System.Windows.Forms.NumericUpDown nudScrollAmount;
        private System.Windows.Forms.CheckBox cbAutoUpload;
        private System.Windows.Forms.Label lblStartDelayHint;
        private System.Windows.Forms.Label lblScrollDelayHint;
        private System.Windows.Forms.Label lblScrollAmountHint;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox cbShowRegion;
        private System.Windows.Forms.CheckBox cbAutoIgnoreBottomEdge;
    }
}