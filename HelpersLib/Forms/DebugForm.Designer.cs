namespace HelpersLib
{
    partial class DebugForm
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
            this.btnLoadedAssemblies = new System.Windows.Forms.Button();
            this.btnCopyAll = new System.Windows.Forms.Button();
            this.rtbDebug = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnLoadedAssemblies
            // 
            this.btnLoadedAssemblies.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLoadedAssemblies.Location = new System.Drawing.Point(96, 511);
            this.btnLoadedAssemblies.Name = "btnLoadedAssemblies";
            this.btnLoadedAssemblies.Size = new System.Drawing.Size(120, 23);
            this.btnLoadedAssemblies.TabIndex = 1;
            this.btnLoadedAssemblies.Text = "Loaded assemblies";
            this.btnLoadedAssemblies.UseVisualStyleBackColor = true;
            this.btnLoadedAssemblies.Click += new System.EventHandler(this.btnLoadedAssemblies_Click);
            // 
            // btnCopyAll
            // 
            this.btnCopyAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopyAll.Location = new System.Drawing.Point(8, 511);
            this.btnCopyAll.Name = "btnCopyAll";
            this.btnCopyAll.Size = new System.Drawing.Size(80, 23);
            this.btnCopyAll.TabIndex = 0;
            this.btnCopyAll.Text = "Copy all";
            this.btnCopyAll.UseVisualStyleBackColor = true;
            this.btnCopyAll.Click += new System.EventHandler(this.btnCopyAll_Click);
            // 
            // rtbDebug
            // 
            this.rtbDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbDebug.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbDebug.Location = new System.Drawing.Point(8, 8);
            this.rtbDebug.Name = "rtbDebug";
            this.rtbDebug.ReadOnly = true;
            this.rtbDebug.Size = new System.Drawing.Size(744, 496);
            this.rtbDebug.TabIndex = 3;
            this.rtbDebug.Text = "";
            this.rtbDebug.WordWrap = false;
            this.rtbDebug.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbDebug_LinkClicked);
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 541);
            this.Controls.Add(this.rtbDebug);
            this.Controls.Add(this.btnCopyAll);
            this.Controls.Add(this.btnLoadedAssemblies);
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "DebugForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Debug log";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DebugForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLoadedAssemblies;
        private System.Windows.Forms.Button btnCopyAll;
        private System.Windows.Forms.RichTextBox rtbDebug;
    }
}