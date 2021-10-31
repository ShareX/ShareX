
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BorderlessWindowForm));
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
            resources.ApplyResources(this.lblWindowTitle, "lblWindowTitle");
            this.lblWindowTitle.Name = "lblWindowTitle";
            // 
            // txtWindowTitle
            // 
            resources.ApplyResources(this.txtWindowTitle, "txtWindowTitle");
            this.txtWindowTitle.Name = "txtWindowTitle";
            this.txtWindowTitle.TextChanged += new System.EventHandler(this.txtWindowTitle_TextChanged);
            // 
            // btnMakeWindowBorderless
            // 
            resources.ApplyResources(this.btnMakeWindowBorderless, "btnMakeWindowBorderless");
            this.btnMakeWindowBorderless.Name = "btnMakeWindowBorderless";
            this.btnMakeWindowBorderless.UseVisualStyleBackColor = true;
            this.btnMakeWindowBorderless.Click += new System.EventHandler(this.btnMakeWindowBorderless_Click);
            // 
            // btnSettings
            // 
            resources.ApplyResources(this.btnSettings, "btnSettings");
            this.btnSettings.Image = global::ShareX.Properties.Resources.gear;
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // cmsWindowList
            // 
            this.cmsWindowList.Name = "cmsWindowList";
            resources.ApplyResources(this.cmsWindowList, "cmsWindowList");
            // 
            // mbWindowList
            // 
            resources.ApplyResources(this.mbWindowList, "mbWindowList");
            this.mbWindowList.Menu = this.cmsWindowList;
            this.mbWindowList.Name = "mbWindowList";
            this.mbWindowList.UseVisualStyleBackColor = true;
            this.mbWindowList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mbWindowList_MouseDown);
            // 
            // BorderlessWindowForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.mbWindowList);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnMakeWindowBorderless);
            this.Controls.Add(this.txtWindowTitle);
            this.Controls.Add(this.lblWindowTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BorderlessWindowForm";
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