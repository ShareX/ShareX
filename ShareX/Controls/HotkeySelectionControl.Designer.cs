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
            this.lblHotkeyStatus = new System.Windows.Forms.Label();
            this.lblHotkeyDescription = new System.Windows.Forms.Label();
            this.btnHotkey = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblHotkeyStatus
            // 
            this.lblHotkeyStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHotkeyStatus.BackColor = System.Drawing.Color.IndianRed;
            this.lblHotkeyStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblHotkeyStatus.Location = new System.Drawing.Point(456, 1);
            this.lblHotkeyStatus.Name = "lblHotkeyStatus";
            this.lblHotkeyStatus.Size = new System.Drawing.Size(24, 21);
            this.lblHotkeyStatus.TabIndex = 2;
            // 
            // lblHotkeyDescription
            // 
            this.lblHotkeyDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHotkeyDescription.BackColor = System.Drawing.Color.White;
            this.lblHotkeyDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblHotkeyDescription.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblHotkeyDescription.Location = new System.Drawing.Point(0, 1);
            this.lblHotkeyDescription.Name = "lblHotkeyDescription";
            this.lblHotkeyDescription.Size = new System.Drawing.Size(254, 21);
            this.lblHotkeyDescription.TabIndex = 0;
            this.lblHotkeyDescription.Text = "Description";
            this.lblHotkeyDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblHotkeyDescription.UseMnemonic = false;
            this.lblHotkeyDescription.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblHotkeyDescription_MouseClick);
            this.lblHotkeyDescription.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lblHotkeyDescription_MouseDoubleClick);
            this.lblHotkeyDescription.MouseEnter += new System.EventHandler(this.lblHotkeyDescription_MouseEnter);
            this.lblHotkeyDescription.MouseLeave += new System.EventHandler(this.lblHotkeyDescription_MouseLeave);
            // 
            // btnHotkey
            // 
            this.btnHotkey.AccessibleName = "";
            this.btnHotkey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHotkey.Location = new System.Drawing.Point(259, 0);
            this.btnHotkey.Name = "btnHotkey";
            this.btnHotkey.Size = new System.Drawing.Size(192, 23);
            this.btnHotkey.TabIndex = 1;
            this.btnHotkey.Text = "Hotkey";
            this.btnHotkey.UseVisualStyleBackColor = true;
            this.btnHotkey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnHotkey_KeyDown);
            this.btnHotkey.KeyUp += new System.Windows.Forms.KeyEventHandler(this.btnHotkey_KeyUp);
            this.btnHotkey.Leave += new System.EventHandler(this.btnHotkey_Leave);
            this.btnHotkey.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnHotkey_MouseClick);
            this.btnHotkey.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.btnHotkey_PreviewKeyDown);
            // 
            // HotkeySelectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnHotkey);
            this.Controls.Add(this.lblHotkeyStatus);
            this.Controls.Add(this.lblHotkeyDescription);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "HotkeySelectionControl";
            this.Size = new System.Drawing.Size(480, 23);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblHotkeyStatus;
        private System.Windows.Forms.Label lblHotkeyDescription;
        private System.Windows.Forms.Button btnHotkey;
    }
}
