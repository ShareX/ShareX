namespace ShareX
{
    partial class ScreenColorPicker
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
            if (colorTimer != null) colorTimer.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScreenColorPicker));
            this.btnPipette = new System.Windows.Forms.Button();
            this.lblScreenColorPickerTip = new System.Windows.Forms.Label();
            this.btnColorPicker = new System.Windows.Forms.Button();
            this.lblY = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.btnCopyAll = new System.Windows.Forms.Button();
            this.txtX = new System.Windows.Forms.TextBox();
            this.txtY = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // colorPicker
            // 
            resources.ApplyResources(this.colorPicker, "colorPicker");
            // 
            // txtHex
            // 
            resources.ApplyResources(this.txtHex, "txtHex");
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            // 
            // btnPipette
            // 
            resources.ApplyResources(this.btnPipette, "btnPipette");
            this.btnPipette.Image = global::ShareX.Properties.Resources.pipette;
            this.btnPipette.Name = "btnPipette";
            this.btnPipette.UseVisualStyleBackColor = true;
            this.btnPipette.Click += new System.EventHandler(this.btnPipette_Click);
            // 
            // lblScreenColorPickerTip
            // 
            resources.ApplyResources(this.lblScreenColorPickerTip, "lblScreenColorPickerTip");
            this.lblScreenColorPickerTip.Name = "lblScreenColorPickerTip";
            // 
            // btnColorPicker
            // 
            resources.ApplyResources(this.btnColorPicker, "btnColorPicker");
            this.btnColorPicker.Name = "btnColorPicker";
            this.btnColorPicker.UseVisualStyleBackColor = true;
            this.btnColorPicker.Click += new System.EventHandler(this.btnColorPicker_Click);
            // 
            // lblY
            // 
            resources.ApplyResources(this.lblY, "lblY");
            this.lblY.Name = "lblY";
            // 
            // lblX
            // 
            resources.ApplyResources(this.lblX, "lblX");
            this.lblX.Name = "lblX";
            // 
            // btnCopyAll
            // 
            resources.ApplyResources(this.btnCopyAll, "btnCopyAll");
            this.btnCopyAll.Name = "btnCopyAll";
            this.btnCopyAll.UseVisualStyleBackColor = true;
            this.btnCopyAll.Click += new System.EventHandler(this.btnCopyAll_Click);
            // 
            // txtX
            // 
            resources.ApplyResources(this.txtX, "txtX");
            this.txtX.Name = "txtX";
            this.txtX.ReadOnly = true;
            // 
            // txtY
            // 
            resources.ApplyResources(this.txtY, "txtY");
            this.txtY.Name = "txtY";
            this.txtY.ReadOnly = true;
            // 
            // ScreenColorPicker
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtY);
            this.Controls.Add(this.txtX);
            this.Controls.Add(this.btnCopyAll);
            this.Controls.Add(this.btnPipette);
            this.Controls.Add(this.lblScreenColorPickerTip);
            this.Controls.Add(this.btnColorPicker);
            this.Controls.Add(this.lblY);
            this.Controls.Add(this.lblX);
            this.KeyPreview = true;
            this.Name = "ScreenColorPicker";
            this.TopMost = false;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ScreenColorPicker_KeyDown);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.txtHex, 0);
            this.Controls.SetChildIndex(this.colorPicker, 0);
            this.Controls.SetChildIndex(this.lblX, 0);
            this.Controls.SetChildIndex(this.lblY, 0);
            this.Controls.SetChildIndex(this.btnColorPicker, 0);
            this.Controls.SetChildIndex(this.lblScreenColorPickerTip, 0);
            this.Controls.SetChildIndex(this.btnPipette, 0);
            this.Controls.SetChildIndex(this.btnCopyAll, 0);
            this.Controls.SetChildIndex(this.txtX, 0);
            this.Controls.SetChildIndex(this.txtY, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPipette;
        private System.Windows.Forms.Label lblScreenColorPickerTip;
        private System.Windows.Forms.Button btnColorPicker;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.Button btnCopyAll;
        private System.Windows.Forms.TextBox txtX;
        private System.Windows.Forms.TextBox txtY;
    }
}