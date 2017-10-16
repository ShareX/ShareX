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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScreenColorPicker));
            this.btnPipette = new System.Windows.Forms.Button();
            this.lblScreenColorPickerTip = new System.Windows.Forms.Label();
            this.btnColorPicker = new System.Windows.Forms.Button();
            this.lblY = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.txtX = new System.Windows.Forms.TextBox();
            this.txtY = new System.Windows.Forms.TextBox();
            this.lblCursorPosition = new System.Windows.Forms.Label();
            this.btnCopy = new ShareX.HelpersLib.MenuButton();
            this.cmsCopy = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCopyAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyRGB = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyHex = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyCMYK = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyHSB = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyDecimal = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopyPosition = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCopy.SuspendLayout();
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
            this.btnPipette.Image = global::ShareX.Properties.Resources.pipette;
            resources.ApplyResources(this.btnPipette, "btnPipette");
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
            // lblCursorPosition
            // 
            resources.ApplyResources(this.lblCursorPosition, "lblCursorPosition");
            this.lblCursorPosition.Name = "lblCursorPosition";
            // 
            // btnCopy
            // 
            resources.ApplyResources(this.btnCopy, "btnCopy");
            this.btnCopy.Menu = this.cmsCopy;
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.UseVisualStyleBackColor = true;
            // 
            // cmsCopy
            // 
            this.cmsCopy.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCopyAll,
            this.tsmiCopyRGB,
            this.tsmiCopyHex,
            this.tsmiCopyCMYK,
            this.tsmiCopyHSB,
            this.tsmiCopyDecimal,
            this.tsmiCopyPosition});
            this.cmsCopy.Name = "cmsCopy";
            this.cmsCopy.ShowImageMargin = false;
            resources.ApplyResources(this.cmsCopy, "cmsCopy");
            // 
            // tsmiCopyAll
            // 
            this.tsmiCopyAll.Name = "tsmiCopyAll";
            resources.ApplyResources(this.tsmiCopyAll, "tsmiCopyAll");
            this.tsmiCopyAll.Click += new System.EventHandler(this.tsmiCopyAll_Click);
            // 
            // tsmiCopyRGB
            // 
            this.tsmiCopyRGB.Name = "tsmiCopyRGB";
            resources.ApplyResources(this.tsmiCopyRGB, "tsmiCopyRGB");
            this.tsmiCopyRGB.Click += new System.EventHandler(this.tsmiCopyRGB_Click);
            // 
            // tsmiCopyHex
            // 
            this.tsmiCopyHex.Name = "tsmiCopyHex";
            resources.ApplyResources(this.tsmiCopyHex, "tsmiCopyHex");
            this.tsmiCopyHex.Click += new System.EventHandler(this.tsmiCopyHex_Click);
            // 
            // tsmiCopyCMYK
            // 
            this.tsmiCopyCMYK.Name = "tsmiCopyCMYK";
            resources.ApplyResources(this.tsmiCopyCMYK, "tsmiCopyCMYK");
            this.tsmiCopyCMYK.Click += new System.EventHandler(this.tsmiCopyCMYK_Click);
            // 
            // tsmiCopyHSB
            // 
            this.tsmiCopyHSB.Name = "tsmiCopyHSB";
            resources.ApplyResources(this.tsmiCopyHSB, "tsmiCopyHSB");
            this.tsmiCopyHSB.Click += new System.EventHandler(this.tsmiCopyHSB_Click);
            // 
            // tsmiCopyDecimal
            // 
            this.tsmiCopyDecimal.Name = "tsmiCopyDecimal";
            resources.ApplyResources(this.tsmiCopyDecimal, "tsmiCopyDecimal");
            this.tsmiCopyDecimal.Click += new System.EventHandler(this.tsmiCopyDecimal_Click);
            // 
            // tsmiCopyPosition
            // 
            this.tsmiCopyPosition.Name = "tsmiCopyPosition";
            resources.ApplyResources(this.tsmiCopyPosition, "tsmiCopyPosition");
            this.tsmiCopyPosition.Click += new System.EventHandler(this.tsmiCopyPosition_Click);
            // 
            // ScreenColorPicker
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.lblCursorPosition);
            this.Controls.Add(this.txtY);
            this.Controls.Add(this.txtX);
            this.Controls.Add(this.btnPipette);
            this.Controls.Add(this.lblScreenColorPickerTip);
            this.Controls.Add(this.btnColorPicker);
            this.Controls.Add(this.lblY);
            this.Controls.Add(this.lblX);
            this.KeyPreview = true;
            this.Name = "ScreenColorPicker";
            this.TopMost = false;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ScreenColorPicker_KeyDown);
            this.Controls.SetChildIndex(this.lblX, 0);
            this.Controls.SetChildIndex(this.lblY, 0);
            this.Controls.SetChildIndex(this.btnColorPicker, 0);
            this.Controls.SetChildIndex(this.lblScreenColorPickerTip, 0);
            this.Controls.SetChildIndex(this.btnPipette, 0);
            this.Controls.SetChildIndex(this.txtX, 0);
            this.Controls.SetChildIndex(this.txtY, 0);
            this.Controls.SetChildIndex(this.lblCursorPosition, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.txtHex, 0);
            this.Controls.SetChildIndex(this.colorPicker, 0);
            this.Controls.SetChildIndex(this.btnCopy, 0);
            this.cmsCopy.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPipette;
        private System.Windows.Forms.Label lblScreenColorPickerTip;
        private System.Windows.Forms.Button btnColorPicker;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.TextBox txtX;
        private System.Windows.Forms.TextBox txtY;
        private System.Windows.Forms.Label lblCursorPosition;
        private HelpersLib.MenuButton btnCopy;
        private System.Windows.Forms.ContextMenuStrip cmsCopy;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyAll;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyRGB;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyHex;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyCMYK;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyPosition;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyHSB;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyDecimal;
    }
}