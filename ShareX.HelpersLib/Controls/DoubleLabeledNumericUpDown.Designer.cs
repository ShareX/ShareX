namespace ShareX.HelpersLib
{
    partial class DoubleLabeledNumericUpDown
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
            this.lblText2 = new System.Windows.Forms.Label();
            this.nudValue2 = new System.Windows.Forms.NumericUpDown();
            this.flpMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudValue2)).BeginInit();
            this.SuspendLayout();
            // 
            // flpMain
            // 
            this.flpMain.AutoSize = true;
            this.flpMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpMain.Controls.Add(this.lblText);
            this.flpMain.Controls.Add(this.nudValue);
            this.flpMain.Controls.Add(this.lblText2);
            this.flpMain.Controls.Add(this.nudValue2);
            this.flpMain.Location = new System.Drawing.Point(0, 0);
            this.flpMain.Margin = new System.Windows.Forms.Padding(0);
            this.flpMain.Name = "flpMain";
            this.flpMain.Size = new System.Drawing.Size(191, 20);
            this.flpMain.TabIndex = 0;
            this.flpMain.WrapContents = false;
            // 
            // lblText
            // 
            this.lblText.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(0, 3);
            this.lblText.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(35, 13);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "label1";
            // 
            // nudValue
            // 
            this.nudValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nudValue.Location = new System.Drawing.Point(38, 0);
            this.nudValue.Margin = new System.Windows.Forms.Padding(0);
            this.nudValue.Name = "nudValue";
            this.nudValue.Size = new System.Drawing.Size(55, 20);
            this.nudValue.TabIndex = 1;
            this.nudValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblText2
            // 
            this.lblText2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblText2.AutoSize = true;
            this.lblText2.Location = new System.Drawing.Point(98, 3);
            this.lblText2.Margin = new System.Windows.Forms.Padding(5, 0, 3, 0);
            this.lblText2.Name = "lblText2";
            this.lblText2.Size = new System.Drawing.Size(35, 13);
            this.lblText2.TabIndex = 2;
            this.lblText2.Text = "label2";
            // 
            // nudValue2
            // 
            this.nudValue2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nudValue2.Location = new System.Drawing.Point(136, 0);
            this.nudValue2.Margin = new System.Windows.Forms.Padding(0);
            this.nudValue2.Name = "nudValue2";
            this.nudValue2.Size = new System.Drawing.Size(55, 20);
            this.nudValue2.TabIndex = 3;
            this.nudValue2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // DoubleLabeledNumericUpDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.flpMain);
            this.Name = "DoubleLabeledNumericUpDown";
            this.Size = new System.Drawing.Size(195, 23);
            this.flpMain.ResumeLayout(false);
            this.flpMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudValue2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpMain;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.NumericUpDown nudValue;
        private System.Windows.Forms.Label lblText2;
        private System.Windows.Forms.NumericUpDown nudValue2;
    }
}
