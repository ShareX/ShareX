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
            this.hmHotkeys = new ShareX.HotkeyManagerControl();
            this.SuspendLayout();
            // 
            // hmHotkeys
            // 
            this.hmHotkeys.BackColor = System.Drawing.Color.White;
            this.hmHotkeys.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hmHotkeys.Location = new System.Drawing.Point(0, 0);
            this.hmHotkeys.Name = "hmHotkeys";
            this.hmHotkeys.Size = new System.Drawing.Size(534, 412);
            this.hmHotkeys.TabIndex = 0;
            // 
            // HotkeySettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(534, 412);
            this.Controls.Add(this.hmHotkeys);
            this.MinimumSize = new System.Drawing.Size(550, 200);
            this.Name = "HotkeySettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Hotkey settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HotkeySettingsForm_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private HotkeyManagerControl hmHotkeys;
    }
}