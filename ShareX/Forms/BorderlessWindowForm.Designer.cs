
namespace ShareX
{
    partial class BorderlessWindowForm
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
            this.lblWindowTitle = new System.Windows.Forms.Label();
            this.txtWindowTitle = new System.Windows.Forms.TextBox();
            this.btnMakeWindowBorderless = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblWindowTitle
            // 
            this.lblWindowTitle.AutoSize = true;
            this.lblWindowTitle.Location = new System.Drawing.Point(13, 16);
            this.lblWindowTitle.Name = "lblWindowTitle";
            this.lblWindowTitle.Size = new System.Drawing.Size(68, 13);
            this.lblWindowTitle.TabIndex = 0;
            this.lblWindowTitle.Text = "Window title:";
            // 
            // txtWindowTitle
            // 
            this.txtWindowTitle.Location = new System.Drawing.Point(16, 32);
            this.txtWindowTitle.Name = "txtWindowTitle";
            this.txtWindowTitle.Size = new System.Drawing.Size(264, 20);
            this.txtWindowTitle.TabIndex = 1;
            this.txtWindowTitle.TextChanged += new System.EventHandler(this.txtWindowTitle_TextChanged);
            // 
            // btnMakeWindowBorderless
            // 
            this.btnMakeWindowBorderless.Enabled = false;
            this.btnMakeWindowBorderless.Location = new System.Drawing.Point(16, 56);
            this.btnMakeWindowBorderless.Name = "btnMakeWindowBorderless";
            this.btnMakeWindowBorderless.Size = new System.Drawing.Size(264, 32);
            this.btnMakeWindowBorderless.TabIndex = 2;
            this.btnMakeWindowBorderless.Text = "Make window borderless";
            this.btnMakeWindowBorderless.UseVisualStyleBackColor = true;
            this.btnMakeWindowBorderless.Click += new System.EventHandler(this.btnMakeWindowBorderless_Click);
            // 
            // BorderlessWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(297, 104);
            this.Controls.Add(this.btnMakeWindowBorderless);
            this.Controls.Add(this.txtWindowTitle);
            this.Controls.Add(this.lblWindowTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BorderlessWindowForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Borderless window";
            this.Shown += new System.EventHandler(this.BorderlessWindowForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWindowTitle;
        private System.Windows.Forms.TextBox txtWindowTitle;
        private System.Windows.Forms.Button btnMakeWindowBorderless;
    }
}