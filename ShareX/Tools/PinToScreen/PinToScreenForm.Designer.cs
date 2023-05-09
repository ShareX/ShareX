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
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ShareX - Pin to screen";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PinToScreenForm_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PinToScreenForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PinToScreenForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PinToScreenForm_MouseUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.PinToScreenForm_MouseWheel);
            this.ResumeLayout(false);

        }

        #endregion
    }
}