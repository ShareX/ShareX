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
            this.pbImage = new ShareX.HelpersLib.MyPictureBox();
            this.SuspendLayout();
            // 
            // pbImage
            // 
            this.pbImage.BackColor = System.Drawing.SystemColors.Window;
            this.pbImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbImage.Location = new System.Drawing.Point(0, 0);
            this.pbImage.Name = "pbImage";
            this.pbImage.PictureBoxBackColor = System.Drawing.SystemColors.Window;
            this.pbImage.Size = new System.Drawing.Size(800, 450);
            this.pbImage.TabIndex = 0;
            this.pbImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PinToScreenForm_MouseDown);
            this.pbImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PinToScreenForm_MouseUp);
            // 
            // PinToScreenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pbImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PinToScreenForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Pin to screen";
            this.TopMost = true;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PinToScreenForm_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PinToScreenForm_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private HelpersLib.MyPictureBox pbImage;
    }
}