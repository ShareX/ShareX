
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
            this.components = new System.ComponentModel.Container();
            this.lblWindowTitle = new System.Windows.Forms.Label();
            this.txtWindowTitle = new System.Windows.Forms.TextBox();
            this.btnMakeWindowBorderless = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.cmsWindowList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mbWindowList = new ShareX.HelpersLib.MenuButton();
            this.SuspendLayout();
            // 
            // lblWindowTitle
            // 
            this.lblWindowTitle.AutoSize = true;
            this.lblWindowTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWindowTitle.Location = new System.Drawing.Point(13, 56);
            this.lblWindowTitle.Name = "lblWindowTitle";
            this.lblWindowTitle.Size = new System.Drawing.Size(82, 16);
            this.lblWindowTitle.TabIndex = 1;
            this.lblWindowTitle.Text = "Window title:";
            // 
            // txtWindowTitle
            // 
            this.txtWindowTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWindowTitle.Location = new System.Drawing.Point(16, 80);
            this.txtWindowTitle.Name = "txtWindowTitle";
            this.txtWindowTitle.Size = new System.Drawing.Size(296, 22);
            this.txtWindowTitle.TabIndex = 2;
            this.txtWindowTitle.TextChanged += new System.EventHandler(this.txtWindowTitle_TextChanged);
            // 
            // btnMakeWindowBorderless
            // 
            this.btnMakeWindowBorderless.Enabled = false;
            this.btnMakeWindowBorderless.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMakeWindowBorderless.Location = new System.Drawing.Point(16, 112);
            this.btnMakeWindowBorderless.Name = "btnMakeWindowBorderless";
            this.btnMakeWindowBorderless.Size = new System.Drawing.Size(256, 32);
            this.btnMakeWindowBorderless.TabIndex = 3;
            this.btnMakeWindowBorderless.Text = "Make window borderless";
            this.btnMakeWindowBorderless.UseVisualStyleBackColor = true;
            this.btnMakeWindowBorderless.Click += new System.EventHandler(this.btnMakeWindowBorderless_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettings.Image = global::ShareX.Properties.Resources.gear;
            this.btnSettings.Location = new System.Drawing.Point(280, 112);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(32, 32);
            this.btnSettings.TabIndex = 4;
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // cmsWindowList
            // 
            this.cmsWindowList.Name = "cmsWindowList";
            this.cmsWindowList.Size = new System.Drawing.Size(61, 4);
            // 
            // mbWindowList
            // 
            this.mbWindowList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbWindowList.Location = new System.Drawing.Point(16, 16);
            this.mbWindowList.Menu = this.cmsWindowList;
            this.mbWindowList.Name = "mbWindowList";
            this.mbWindowList.Size = new System.Drawing.Size(296, 32);
            this.mbWindowList.TabIndex = 0;
            this.mbWindowList.Text = "Select a window";
            this.mbWindowList.UseVisualStyleBackColor = true;
            this.mbWindowList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mbWindowList_MouseDown);
            // 
            // BorderlessWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(328, 161);
            this.Controls.Add(this.mbWindowList);
            this.Controls.Add(this.btnSettings);
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
        private System.Windows.Forms.Button btnSettings;
        private HelpersLib.MenuButton mbWindowList;
        private System.Windows.Forms.ContextMenuStrip cmsWindowList;
    }
}