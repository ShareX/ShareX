namespace ShareX
{
    partial class ChromeForm
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
            this.btnRegister = new System.Windows.Forms.Button();
            this.btnUnregister = new System.Windows.Forms.Button();
            this.lbl1 = new System.Windows.Forms.Label();
            this.lbl2 = new System.Windows.Forms.Label();
            this.btnInstallExtension = new System.Windows.Forms.Button();
            this.lbl3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnRegister
            // 
            this.btnRegister.Image = global::ShareX.Properties.Resources.tick_button;
            this.btnRegister.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRegister.Location = new System.Drawing.Point(8, 32);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.btnRegister.Size = new System.Drawing.Size(192, 32);
            this.btnRegister.TabIndex = 0;
            this.btnRegister.Text = "Enable Chrome support";
            this.btnRegister.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRegister.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // btnUnregister
            // 
            this.btnUnregister.Image = global::ShareX.Properties.Resources.cross_button;
            this.btnUnregister.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUnregister.Location = new System.Drawing.Point(208, 32);
            this.btnUnregister.Name = "btnUnregister";
            this.btnUnregister.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.btnUnregister.Size = new System.Drawing.Size(192, 32);
            this.btnUnregister.TabIndex = 1;
            this.btnUnregister.Text = "Disable Chrome support";
            this.btnUnregister.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUnregister.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUnregister.UseVisualStyleBackColor = true;
            this.btnUnregister.Click += new System.EventHandler(this.btnUnregister_Click);
            // 
            // lbl1
            // 
            this.lbl1.Location = new System.Drawing.Point(8, 8);
            this.lbl1.Name = "lbl1";
            this.lbl1.Size = new System.Drawing.Size(392, 24);
            this.lbl1.TabIndex = 2;
            this.lbl1.Text = "1. Enable Chrome support otherwise extension can\'t interact with ShareX.";
            // 
            // lbl2
            // 
            this.lbl2.Location = new System.Drawing.Point(8, 80);
            this.lbl2.Name = "lbl2";
            this.lbl2.Size = new System.Drawing.Size(392, 24);
            this.lbl2.TabIndex = 3;
            this.lbl2.Text = "2. Install ShareX Chrome extension from Chrome web store.";
            // 
            // btnInstallExtension
            // 
            this.btnInstallExtension.Image = global::ShareX.Properties.Resources.arrow_270;
            this.btnInstallExtension.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnInstallExtension.Location = new System.Drawing.Point(8, 104);
            this.btnInstallExtension.Name = "btnInstallExtension";
            this.btnInstallExtension.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.btnInstallExtension.Size = new System.Drawing.Size(392, 32);
            this.btnInstallExtension.TabIndex = 4;
            this.btnInstallExtension.Text = "Install ShareX Chrome extension";
            this.btnInstallExtension.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnInstallExtension.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnInstallExtension.UseVisualStyleBackColor = true;
            this.btnInstallExtension.Click += new System.EventHandler(this.btnInstallExtension_Click);
            // 
            // lbl3
            // 
            this.lbl3.Location = new System.Drawing.Point(8, 152);
            this.lbl3.Name = "lbl3";
            this.lbl3.Size = new System.Drawing.Size(392, 40);
            this.lbl3.TabIndex = 5;
            this.lbl3.Text = "3. In Chrome right click on image or selected text and you will see \"Upload with " +
    "ShareX\" button in context menu.";
            // 
            // ChromeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 195);
            this.Controls.Add(this.lbl3);
            this.Controls.Add(this.btnInstallExtension);
            this.Controls.Add(this.lbl2);
            this.Controls.Add(this.lbl1);
            this.Controls.Add(this.btnUnregister);
            this.Controls.Add(this.btnRegister);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ChromeForm";
            this.Text = "ShareX - Chrome support";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Button btnUnregister;
        private System.Windows.Forms.Label lbl1;
        private System.Windows.Forms.Label lbl2;
        private System.Windows.Forms.Button btnInstallExtension;
        private System.Windows.Forms.Label lbl3;
    }
}