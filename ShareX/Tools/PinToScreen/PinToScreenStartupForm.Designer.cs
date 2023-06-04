namespace ShareX
{
    partial class PinToScreenStartupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PinToScreenStartupForm));
            this.btnFromFile = new System.Windows.Forms.Button();
            this.btnFromClipboard = new System.Windows.Forms.Button();
            this.btnFromScreen = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnFromFile
            // 
            this.btnFromFile.Image = global::ShareX.Properties.Resources.folder_open_image;
            resources.ApplyResources(this.btnFromFile, "btnFromFile");
            this.btnFromFile.Name = "btnFromFile";
            this.btnFromFile.UseVisualStyleBackColor = true;
            this.btnFromFile.Click += new System.EventHandler(this.btnFromFile_Click);
            // 
            // btnFromClipboard
            // 
            this.btnFromClipboard.Image = global::ShareX.Properties.Resources.clipboard_paste_image;
            resources.ApplyResources(this.btnFromClipboard, "btnFromClipboard");
            this.btnFromClipboard.Name = "btnFromClipboard";
            this.btnFromClipboard.UseVisualStyleBackColor = true;
            this.btnFromClipboard.Click += new System.EventHandler(this.btnFromClipboard_Click);
            // 
            // btnFromScreen
            // 
            this.btnFromScreen.Image = global::ShareX.Properties.Resources.monitor;
            resources.ApplyResources(this.btnFromScreen, "btnFromScreen");
            this.btnFromScreen.Name = "btnFromScreen";
            this.btnFromScreen.UseVisualStyleBackColor = true;
            this.btnFromScreen.Click += new System.EventHandler(this.btnFromScreen_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Image = global::ShareX.Properties.Resources.cross;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // PinToScreenStartupForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnFromScreen);
            this.Controls.Add(this.btnFromClipboard);
            this.Controls.Add(this.btnFromFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PinToScreenStartupForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnFromFile;
        private System.Windows.Forms.Button btnFromClipboard;
        private System.Windows.Forms.Button btnFromScreen;
        private System.Windows.Forms.Button btnCancel;
    }
}