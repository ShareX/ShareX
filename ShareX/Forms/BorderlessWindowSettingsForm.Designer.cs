
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BorderlessWindowSettingsForm));
            this.cbRememberWindowTitle = new System.Windows.Forms.CheckBox();
            this.cbAutoCloseWindow = new System.Windows.Forms.CheckBox();
            this.cbExcludeTaskbarArea = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbRememberWindowTitle
            // 
            resources.ApplyResources(this.cbRememberWindowTitle, "cbRememberWindowTitle");
            this.cbRememberWindowTitle.Name = "cbRememberWindowTitle";
            this.cbRememberWindowTitle.UseVisualStyleBackColor = true;
            this.cbRememberWindowTitle.CheckedChanged += new System.EventHandler(this.cbRememberWindowTitle_CheckedChanged);
            // 
            // cbAutoCloseWindow
            // 
            resources.ApplyResources(this.cbAutoCloseWindow, "cbAutoCloseWindow");
            this.cbAutoCloseWindow.Name = "cbAutoCloseWindow";
            this.cbAutoCloseWindow.UseVisualStyleBackColor = true;
            this.cbAutoCloseWindow.CheckedChanged += new System.EventHandler(this.cbAutoCloseWindow_CheckedChanged);
            // 
            // cbExcludeTaskbarArea
            // 
            resources.ApplyResources(this.cbExcludeTaskbarArea, "cbExcludeTaskbarArea");
            this.cbExcludeTaskbarArea.Name = "cbExcludeTaskbarArea";
            this.cbExcludeTaskbarArea.UseVisualStyleBackColor = true;
            this.cbExcludeTaskbarArea.CheckedChanged += new System.EventHandler(this.cbExcludeTaskbarArea_CheckedChanged);
            // 
            // BorderlessWindowSettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbExcludeTaskbarArea);
            this.Controls.Add(this.cbAutoCloseWindow);
            this.Controls.Add(this.cbRememberWindowTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BorderlessWindowSettingsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbRememberWindowTitle;
        private System.Windows.Forms.CheckBox cbAutoCloseWindow;
        private System.Windows.Forms.CheckBox cbExcludeTaskbarArea;
    }
}