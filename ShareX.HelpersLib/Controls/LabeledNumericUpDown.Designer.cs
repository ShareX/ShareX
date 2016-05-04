namespace ShareX.HelpersLib
{
    partial class LabeledNumericUpDown
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flpMain = new System.Windows.Forms.FlowLayoutPanel();
            this.lblText = new System.Windows.Forms.Label();
            this.nudValue = new System.Windows.Forms.NumericUpDown();
            this.flpMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudValue)).BeginInit();
            this.SuspendLayout();
            // 
            // flpMain
            // 
            this.flpMain.AutoSize = true;
            this.flpMain.Controls.Add(this.lblText);
            this.flpMain.Controls.Add(this.nudValue);
            this.flpMain.Location = new System.Drawing.Point(0, 0);
            this.flpMain.Name = "flpMain";
            this.flpMain.Size = new System.Drawing.Size(99, 26);
            this.flpMain.TabIndex = 0;
            this.flpMain.WrapContents = false;
            // 
            // lblText
            // 
            this.lblText.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(0, 6);
            this.lblText.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(35, 13);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "label1";
            // 
            // nudValue
            // 
            this.nudValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nudValue.Location = new System.Drawing.Point(41, 3);
            this.nudValue.Name = "nudValue";
            this.nudValue.Size = new System.Drawing.Size(55, 20);
            this.nudValue.TabIndex = 1;
            this.nudValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LabeledNumericUpDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.flpMain);
            this.Name = "LabeledNumericUpDown";
            this.Size = new System.Drawing.Size(100, 26);
            this.flpMain.ResumeLayout(false);
            this.flpMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpMain;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.NumericUpDown nudValue;
    }
}
