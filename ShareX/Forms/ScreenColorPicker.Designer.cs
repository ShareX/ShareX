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
            // btnPipette
            // 
            this.btnPipette.Image = global::ShareX.Properties.Resources.pipette;
            this.btnPipette.Location = new System.Drawing.Point(296, 274);
            this.btnPipette.Name = "btnPipette";
            this.btnPipette.Size = new System.Drawing.Size(32, 24);
            this.btnPipette.TabIndex = 56;
            this.btnPipette.UseVisualStyleBackColor = true;
            this.btnPipette.Click += new System.EventHandler(this.btnPipette_Click);
            // 
            // lblScreenColorPickerTip
            // 
            this.lblScreenColorPickerTip.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblScreenColorPickerTip.Location = new System.Drawing.Point(336, 270);
            this.lblScreenColorPickerTip.Name = "lblScreenColorPickerTip";
            this.lblScreenColorPickerTip.Size = new System.Drawing.Size(265, 34);
            this.lblScreenColorPickerTip.TabIndex = 55;
            this.lblScreenColorPickerTip.Text = "Press \"Ctrl\" button when this window active\r\nto stop screen color picker.";
            this.lblScreenColorPickerTip.Visible = false;
            // 
            // btnColorPicker
            // 
            this.btnColorPicker.Location = new System.Drawing.Point(152, 274);
            this.btnColorPicker.Name = "btnColorPicker";
            this.btnColorPicker.Size = new System.Drawing.Size(144, 24);
            this.btnColorPicker.TabIndex = 52;
            this.btnColorPicker.Text = "Start screen color picker";
            this.btnColorPicker.UseVisualStyleBackColor = true;
            this.btnColorPicker.Click += new System.EventHandler(this.btnColorPicker_Click);
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(80, 280);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(17, 13);
            this.lblY.TabIndex = 51;
            this.lblY.Text = "Y:";
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(8, 280);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(17, 13);
            this.lblX.TabIndex = 50;
            this.lblX.Text = "X:";
            // 
            // btnCopyAll
            // 
            this.btnCopyAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCopyAll.Location = new System.Drawing.Point(464, 232);
            this.btnCopyAll.Name = "btnCopyAll";
            this.btnCopyAll.Size = new System.Drawing.Size(64, 32);
            this.btnCopyAll.TabIndex = 57;
            this.btnCopyAll.Text = "Copy all";
            this.btnCopyAll.UseVisualStyleBackColor = true;
            this.btnCopyAll.Click += new System.EventHandler(this.btnCopyAll_Click);
            // 
            // txtX
            // 
            this.txtX.Location = new System.Drawing.Point(25, 276);
            this.txtX.Name = "txtX";
            this.txtX.ReadOnly = true;
            this.txtX.Size = new System.Drawing.Size(48, 20);
            this.txtX.TabIndex = 58;
            this.txtX.Text = "1680";
            this.txtX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtY
            // 
            this.txtY.Location = new System.Drawing.Point(97, 276);
            this.txtY.Name = "txtY";
            this.txtY.ReadOnly = true;
            this.txtY.Size = new System.Drawing.Size(48, 20);
            this.txtY.TabIndex = 59;
            this.txtY.Text = "1050";
            this.txtY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ScreenColorPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 305);
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
            this.Text = "ShareX - Screen color picker";
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