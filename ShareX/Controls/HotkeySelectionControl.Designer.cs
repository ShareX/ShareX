namespace ShareX
{
    partial class HotkeySelectionControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HotkeySelectionControl));
            this.btnEdit = new System.Windows.Forms.Button();
            this.cmsTask = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnTask = new ShareX.HelpersLib.MenuButton();
            this.btnHotkey = new ShareX.HelpersLib.ColorButton();
            this.SuspendLayout();
            // 
            // btnEdit
            // 
            resources.ApplyResources(this.btnEdit, "btnEdit");
            this.btnEdit.Image = global::ShareX.Properties.Resources.gear;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // cmsTask
            // 
            this.cmsTask.Name = "cmsTask";
            resources.ApplyResources(this.cmsTask, "cmsTask");
            // 
            // btnTask
            // 
            resources.ApplyResources(this.btnTask, "btnTask");
            this.btnTask.Image = global::ShareX.Properties.Resources.gear;
            this.btnTask.Menu = this.cmsTask;
            this.btnTask.Name = "btnTask";
            this.btnTask.UseMnemonic = false;
            this.btnTask.UseVisualStyleBackColor = true;
            // 
            // btnHotkey
            // 
            resources.ApplyResources(this.btnHotkey, "btnHotkey");
            this.btnHotkey.Color = System.Drawing.Color.Empty;
            this.btnHotkey.ColorPickerOptions = null;
            this.btnHotkey.ManualButtonClick = true;
            this.btnHotkey.Name = "btnHotkey";
            this.btnHotkey.Offset = 0;
            this.btnHotkey.UseVisualStyleBackColor = true;
            this.btnHotkey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnHotkey_KeyDown);
            this.btnHotkey.KeyUp += new System.Windows.Forms.KeyEventHandler(this.btnHotkey_KeyUp);
            this.btnHotkey.Leave += new System.EventHandler(this.btnHotkey_Leave);
            this.btnHotkey.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnHotkey_MouseClick);
            this.btnHotkey.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.btnHotkey_PreviewKeyDown);
            // 
            // HotkeySelectionControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.btnTask);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnHotkey);
            this.Name = "HotkeySelectionControl";
            this.ResumeLayout(false);

        }

        #endregion
        private HelpersLib.ColorButton btnHotkey;
        private System.Windows.Forms.Button btnEdit;
        private HelpersLib.MenuButton btnTask;
        private System.Windows.Forms.ContextMenuStrip cmsTask;
    }
}
