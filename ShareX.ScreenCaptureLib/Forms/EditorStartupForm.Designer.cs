namespace ShareX.ScreenCaptureLib
{
    partial class EditorStartupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorStartupForm));
            this.btnOpenImageFile = new System.Windows.Forms.Button();
            this.btnCreateNewImage = new System.Windows.Forms.Button();
            this.btnLoadImageFromClipboard = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnLoadImageFromURL = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOpenImageFile
            // 
            resources.ApplyResources(this.btnOpenImageFile, "btnOpenImageFile");
            this.btnOpenImageFile.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.folder_open_image;
            this.btnOpenImageFile.Name = "btnOpenImageFile";
            this.btnOpenImageFile.UseVisualStyleBackColor = true;
            this.btnOpenImageFile.Click += new System.EventHandler(this.btnOpenImageFile_Click);
            // 
            // btnCreateNewImage
            // 
            resources.ApplyResources(this.btnCreateNewImage, "btnCreateNewImage");
            this.btnCreateNewImage.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.image_empty;
            this.btnCreateNewImage.Name = "btnCreateNewImage";
            this.btnCreateNewImage.UseVisualStyleBackColor = true;
            this.btnCreateNewImage.Click += new System.EventHandler(this.btnCreateNewImage_Click);
            // 
            // btnLoadImageFromClipboard
            // 
            resources.ApplyResources(this.btnLoadImageFromClipboard, "btnLoadImageFromClipboard");
            this.btnLoadImageFromClipboard.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.clipboard;
            this.btnLoadImageFromClipboard.Name = "btnLoadImageFromClipboard";
            this.btnLoadImageFromClipboard.UseVisualStyleBackColor = true;
            this.btnLoadImageFromClipboard.Click += new System.EventHandler(this.btnLoadImageFromClipboard_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.cross;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnLoadImageFromURL
            // 
            resources.ApplyResources(this.btnLoadImageFromURL, "btnLoadImageFromURL");
            this.btnLoadImageFromURL.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.drive_globe;
            this.btnLoadImageFromURL.Name = "btnLoadImageFromURL";
            this.btnLoadImageFromURL.UseVisualStyleBackColor = true;
            this.btnLoadImageFromURL.Click += new System.EventHandler(this.btnLoadImageFromURL_Click);
            // 
            // EditorStartupForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.btnLoadImageFromURL);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnLoadImageFromClipboard);
            this.Controls.Add(this.btnCreateNewImage);
            this.Controls.Add(this.btnOpenImageFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditorStartupForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOpenImageFile;
        private System.Windows.Forms.Button btnCreateNewImage;
        private System.Windows.Forms.Button btnLoadImageFromClipboard;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnLoadImageFromURL;
    }
}