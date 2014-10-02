namespace ShareX
{
    partial class HotkeySettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HotkeySettingsForm));
            this.hmHotkeys = new ShareX.HotkeyManagerControl();
            this.SuspendLayout();
            // 
            // hmHotkeys
            // 
            resources.ApplyResources(this.hmHotkeys, "hmHotkeys");
            this.hmHotkeys.BackColor = System.Drawing.Color.White;
            this.hmHotkeys.Name = "hmHotkeys";
            // 
            // HotkeySettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.hmHotkeys);
            this.Name = "HotkeySettingsForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HotkeySettingsForm_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private HotkeyManagerControl hmHotkeys;
    }
}