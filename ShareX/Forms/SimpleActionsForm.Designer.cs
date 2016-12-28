namespace ShareX
{
    partial class SimpleActionsForm
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
            this.tsMain = new ShareX.HelpersLib.ToolStripEx();
            this.tslTitle = new System.Windows.Forms.ToolStripLabel();
            this.tsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsMain
            // 
            this.tsMain.CanOverflow = false;
            this.tsMain.ClickThrough = true;
            this.tsMain.Dock = System.Windows.Forms.DockStyle.None;
            this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslTitle});
            this.tsMain.Location = new System.Drawing.Point(0, 0);
            this.tsMain.MinimumSize = new System.Drawing.Size(10, 30);
            this.tsMain.Name = "tsMain";
            this.tsMain.Padding = new System.Windows.Forms.Padding(2);
            this.tsMain.Size = new System.Drawing.Size(86, 30);
            this.tsMain.TabIndex = 0;
            this.tsMain.Text = "toolStripEx1";
            // 
            // tslTitle
            // 
            this.tslTitle.Margin = new System.Windows.Forms.Padding(3, 1, 3, 2);
            this.tslTitle.Name = "tslTitle";
            this.tslTitle.Size = new System.Drawing.Size(43, 23);
            this.tslTitle.Text = "ShareX";
            this.tslTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tslTitle_MouseDown);
            this.tslTitle.MouseEnter += new System.EventHandler(this.tslTitle_MouseEnter);
            this.tslTitle.MouseLeave += new System.EventHandler(this.tslTitle_MouseLeave);
            this.tslTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tslTitle_MouseUp);
            // 
            // ToolMenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.tsMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ToolMenuForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX";
            this.TopMost = true;
            this.tsMain.ResumeLayout(false);
            this.tsMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HelpersLib.ToolStripEx tsMain;
        private System.Windows.Forms.ToolStripLabel tslTitle;
    }
}