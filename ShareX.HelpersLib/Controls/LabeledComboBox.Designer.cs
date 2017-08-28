namespace ShareX.HelpersLib
{
    partial class LabeledComboBox
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
            this.cbList = new System.Windows.Forms.ComboBox();
            this.flpMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // flpMain
            // 
            this.flpMain.AutoSize = true;
            this.flpMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpMain.Controls.Add(this.lblText);
            this.flpMain.Controls.Add(this.cbList);
            this.flpMain.Location = new System.Drawing.Point(0, 0);
            this.flpMain.Margin = new System.Windows.Forms.Padding(0);
            this.flpMain.Name = "flpMain";
            this.flpMain.Size = new System.Drawing.Size(147, 27);
            this.flpMain.TabIndex = 0;
            this.flpMain.WrapContents = false;
            // 
            // lblText
            // 
            this.lblText.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(0, 7);
            this.lblText.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(35, 13);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "label1";
            // 
            // cbList
            // 
            this.cbList.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cbList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbList.FormattingEnabled = true;
            this.cbList.Location = new System.Drawing.Point(41, 3);
            this.cbList.Name = "cbList";
            this.cbList.Size = new System.Drawing.Size(103, 21);
            this.cbList.TabIndex = 1;
            // 
            // LabeledComboBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.flpMain);
            this.Name = "LabeledComboBox";
            this.Size = new System.Drawing.Size(147, 27);
            this.flpMain.ResumeLayout(false);
            this.flpMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpMain;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.ComboBox cbList;
    }
}
