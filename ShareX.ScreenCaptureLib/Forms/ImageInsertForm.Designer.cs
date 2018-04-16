namespace ShareX.ScreenCaptureLib
{
    partial class ImageInsertForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageInsertForm));
            this.btnCenter = new System.Windows.Forms.Button();
            this.btnCanvasExpandDown = new System.Windows.Forms.Button();
            this.btnCanvasExpandRight = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCenter
            // 
            resources.ApplyResources(this.btnCenter, "btnCenter");
            this.btnCenter.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.layout_center;
            this.btnCenter.Name = "btnCenter";
            this.btnCenter.UseVisualStyleBackColor = true;
            this.btnCenter.Click += new System.EventHandler(this.btnCenter_Click);
            // 
            // btnCanvasExpandDown
            // 
            resources.ApplyResources(this.btnCanvasExpandDown, "btnCanvasExpandDown");
            this.btnCanvasExpandDown.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.layout_split_vertical;
            this.btnCanvasExpandDown.Name = "btnCanvasExpandDown";
            this.btnCanvasExpandDown.UseVisualStyleBackColor = true;
            this.btnCanvasExpandDown.Click += new System.EventHandler(this.btnCanvasExpandDown_Click);
            // 
            // btnCanvasExpandRight
            // 
            resources.ApplyResources(this.btnCanvasExpandRight, "btnCanvasExpandRight");
            this.btnCanvasExpandRight.Image = global::ShareX.ScreenCaptureLib.Properties.Resources.layout_split;
            this.btnCanvasExpandRight.Name = "btnCanvasExpandRight";
            this.btnCanvasExpandRight.UseVisualStyleBackColor = true;
            this.btnCanvasExpandRight.Click += new System.EventHandler(this.btnCanvasExpandRight_Click);
            // 
            // ImageInsertForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCanvasExpandRight);
            this.Controls.Add(this.btnCanvasExpandDown);
            this.Controls.Add(this.btnCenter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImageInsertForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCenter;
        private System.Windows.Forms.Button btnCanvasExpandDown;
        private System.Windows.Forms.Button btnCanvasExpandRight;
    }
}