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
            this.btnFromFile = new System.Windows.Forms.Button();
            this.btnFromClipboard = new System.Windows.Forms.Button();
            this.btnFromScreen = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnFromFile
            // 
            this.btnFromFile.Image = global::ShareX.Properties.Resources.folder_open_image;
            this.btnFromFile.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFromFile.Location = new System.Drawing.Point(8, 88);
            this.btnFromFile.Name = "btnFromFile";
            this.btnFromFile.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnFromFile.Size = new System.Drawing.Size(296, 32);
            this.btnFromFile.TabIndex = 2;
            this.btnFromFile.Text = "Pin to screen from file...";
            this.btnFromFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFromFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFromFile.UseVisualStyleBackColor = true;
            this.btnFromFile.Click += new System.EventHandler(this.btnFromFile_Click);
            // 
            // btnFromClipboard
            // 
            this.btnFromClipboard.Image = global::ShareX.Properties.Resources.clipboard_paste_image;
            this.btnFromClipboard.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFromClipboard.Location = new System.Drawing.Point(8, 8);
            this.btnFromClipboard.Name = "btnFromClipboard";
            this.btnFromClipboard.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnFromClipboard.Size = new System.Drawing.Size(296, 32);
            this.btnFromClipboard.TabIndex = 0;
            this.btnFromClipboard.Text = "Pin to screen from clipboard";
            this.btnFromClipboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFromClipboard.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFromClipboard.UseVisualStyleBackColor = true;
            this.btnFromClipboard.Click += new System.EventHandler(this.btnFromClipboard_Click);
            // 
            // btnFromScreen
            // 
            this.btnFromScreen.Image = global::ShareX.Properties.Resources.monitor;
            this.btnFromScreen.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFromScreen.Location = new System.Drawing.Point(8, 48);
            this.btnFromScreen.Name = "btnFromScreen";
            this.btnFromScreen.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnFromScreen.Size = new System.Drawing.Size(296, 32);
            this.btnFromScreen.TabIndex = 1;
            this.btnFromScreen.Text = "Pin to screen from screen...";
            this.btnFromScreen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFromScreen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFromScreen.UseVisualStyleBackColor = true;
            this.btnFromScreen.Click += new System.EventHandler(this.btnFromScreen_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::ShareX.Properties.Resources.cross;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(8, 128);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnCancel.Size = new System.Drawing.Size(296, 32);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // PinToScreenStartupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(313, 169);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnFromScreen);
            this.Controls.Add(this.btnFromClipboard);
            this.Controls.Add(this.btnFromFile);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PinToScreenStartupForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Pin to screen";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnFromFile;
        private System.Windows.Forms.Button btnFromClipboard;
        private System.Windows.Forms.Button btnFromScreen;
        private System.Windows.Forms.Button btnCancel;
    }
}