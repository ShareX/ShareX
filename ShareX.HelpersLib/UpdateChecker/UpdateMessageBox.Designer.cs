namespace ShareX.HelpersLib
{
    partial class UpdateMessageBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateMessageBox));
            this.lblText = new ShareX.HelpersLib.BlackStyleLabel();
            this.btnNo = new ShareX.HelpersLib.BlackStyleButton();
            this.btnYes = new ShareX.HelpersLib.BlackStyleButton();
            this.cbDontShow = new ShareX.HelpersLib.BlackStyleCheckBox();
            this.SuspendLayout();
            // 
            // lblText
            // 
            this.lblText.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblText, "lblText");
            this.lblText.ForeColor = System.Drawing.Color.White;
            this.lblText.Name = "lblText";
            // 
            // btnNo
            // 
            resources.ApplyResources(this.btnNo, "btnNo");
            this.btnNo.ForeColor = System.Drawing.Color.White;
            this.btnNo.Name = "btnNo";
            this.btnNo.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnNo_MouseClick);
            // 
            // btnYes
            // 
            resources.ApplyResources(this.btnYes, "btnYes");
            this.btnYes.ForeColor = System.Drawing.Color.White;
            this.btnYes.Name = "btnYes";
            this.btnYes.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnYes_MouseClick);
            // 
            // cbDontShow
            // 
            resources.ApplyResources(this.cbDontShow, "cbDontShow");
            this.cbDontShow.ForeColor = System.Drawing.Color.White;
            this.cbDontShow.Name = "cbDontShow";
            this.cbDontShow.SpaceAfterCheckBox = 3;
            this.cbDontShow.CheckedChanged += new System.EventHandler(this.cbDontShow_CheckedChanged);
            // 
            // UpdateMessageBox
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.Controls.Add(this.cbDontShow);
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "UpdateMessageBox";
            this.Shown += new System.EventHandler(this.UpdateMessageBox_Shown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.UpdateMessageBox_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private BlackStyleButton btnYes;
        private BlackStyleButton btnNo;
        private BlackStyleLabel lblText;
        private BlackStyleCheckBox cbDontShow;
    }
}