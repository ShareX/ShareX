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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HotkeySelectionControl));
            this.lblHotkeyStatus = new System.Windows.Forms.Label();
            this.lblHotkeyDescription = new ShareX.HelpersLib.LabelNoCopy();
            this.btnHotkey = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblHotkeyStatus
            // 
            resources.ApplyResources(this.lblHotkeyStatus, "lblHotkeyStatus");
            this.lblHotkeyStatus.BackColor = System.Drawing.Color.IndianRed;
            this.lblHotkeyStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblHotkeyStatus.Name = "lblHotkeyStatus";
            // 
            // lblHotkeyDescription
            // 
            resources.ApplyResources(this.lblHotkeyDescription, "lblHotkeyDescription");
            this.lblHotkeyDescription.BackColor = System.Drawing.Color.White;
            this.lblHotkeyDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblHotkeyDescription.Name = "lblHotkeyDescription";
            this.lblHotkeyDescription.UseMnemonic = false;
            this.lblHotkeyDescription.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblHotkeyDescription_MouseClick);
            this.lblHotkeyDescription.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lblHotkeyDescription_MouseDoubleClick);
            this.lblHotkeyDescription.MouseEnter += new System.EventHandler(this.lblHotkeyDescription_MouseEnter);
            this.lblHotkeyDescription.MouseLeave += new System.EventHandler(this.lblHotkeyDescription_MouseLeave);
            // 
            // btnHotkey
            // 
            resources.ApplyResources(this.btnHotkey, "btnHotkey");
            this.btnHotkey.Name = "btnHotkey";
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
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnHotkey);
            this.Controls.Add(this.lblHotkeyStatus);
            this.Controls.Add(this.lblHotkeyDescription);
            this.Name = "HotkeySelectionControl";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblHotkeyStatus;
        private ShareX.HelpersLib.LabelNoCopy lblHotkeyDescription;
        private System.Windows.Forms.Button btnHotkey;
    }
}
