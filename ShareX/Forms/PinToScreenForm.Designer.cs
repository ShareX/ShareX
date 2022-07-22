namespace ShareX
{
    partial class PinToScreenForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PinToScreenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PinToScreenForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ShareX - Pin to screen";
            this.TopMost = true;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PinToScreenForm_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PinToScreenForm_MouseUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.PinToScreenForm_MouseWheel);
            this.ResumeLayout(false);

        }

        #endregion
    }
}