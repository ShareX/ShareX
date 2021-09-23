
namespace ShareX
{
    partial class BorderlessWindowSettingsForm
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
            this.cbRememberWindowTitle = new System.Windows.Forms.CheckBox();
            this.cbAutoCloseWindow = new System.Windows.Forms.CheckBox();
            this.cbExcludeTaskbarArea = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbRememberWindowTitle
            // 
            this.cbRememberWindowTitle.AutoSize = true;
            this.cbRememberWindowTitle.Location = new System.Drawing.Point(16, 16);
            this.cbRememberWindowTitle.Name = "cbRememberWindowTitle";
            this.cbRememberWindowTitle.Size = new System.Drawing.Size(135, 17);
            this.cbRememberWindowTitle.TabIndex = 0;
            this.cbRememberWindowTitle.Text = "Remember window title";
            this.cbRememberWindowTitle.UseVisualStyleBackColor = true;
            this.cbRememberWindowTitle.CheckedChanged += new System.EventHandler(this.cbRememberWindowTitle_CheckedChanged);
            // 
            // cbAutoCloseWindow
            // 
            this.cbAutoCloseWindow.AutoSize = true;
            this.cbAutoCloseWindow.Location = new System.Drawing.Point(16, 40);
            this.cbAutoCloseWindow.Name = "cbAutoCloseWindow";
            this.cbAutoCloseWindow.Size = new System.Drawing.Size(155, 17);
            this.cbAutoCloseWindow.TabIndex = 1;
            this.cbAutoCloseWindow.Text = "Automatically close window";
            this.cbAutoCloseWindow.UseVisualStyleBackColor = true;
            this.cbAutoCloseWindow.CheckedChanged += new System.EventHandler(this.cbAutoCloseWindow_CheckedChanged);
            // 
            // cbExcludeTaskbarArea
            // 
            this.cbExcludeTaskbarArea.AutoSize = true;
            this.cbExcludeTaskbarArea.Location = new System.Drawing.Point(16, 64);
            this.cbExcludeTaskbarArea.Name = "cbExcludeTaskbarArea";
            this.cbExcludeTaskbarArea.Size = new System.Drawing.Size(126, 17);
            this.cbExcludeTaskbarArea.TabIndex = 2;
            this.cbExcludeTaskbarArea.Text = "Exclude taskbar area";
            this.cbExcludeTaskbarArea.UseVisualStyleBackColor = true;
            this.cbExcludeTaskbarArea.CheckedChanged += new System.EventHandler(this.cbExcludeTaskbarArea_CheckedChanged);
            // 
            // BorderlessWindowSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 148);
            this.Controls.Add(this.cbExcludeTaskbarArea);
            this.Controls.Add(this.cbAutoCloseWindow);
            this.Controls.Add(this.cbRememberWindowTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BorderlessWindowSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Borderless window settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbRememberWindowTitle;
        private System.Windows.Forms.CheckBox cbAutoCloseWindow;
        private System.Windows.Forms.CheckBox cbExcludeTaskbarArea;
    }
}