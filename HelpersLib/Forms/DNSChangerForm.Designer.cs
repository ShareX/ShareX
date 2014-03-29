namespace HelpersLib
{
    partial class DNSChangerForm
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
            this.cbAdapters = new System.Windows.Forms.ComboBox();
            this.lblAdapters = new System.Windows.Forms.Label();
            this.txtPreferredDNS = new System.Windows.Forms.TextBox();
            this.lblPreferredDNS = new System.Windows.Forms.Label();
            this.lblAlternateDNS = new System.Windows.Forms.Label();
            this.txtAlternateDNS = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbDNSType = new System.Windows.Forms.ComboBox();
            this.lblDNS = new System.Windows.Forms.Label();
            this.cbAutomatic = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbAdapters
            // 
            this.cbAdapters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAdapters.FormattingEnabled = true;
            this.cbAdapters.Location = new System.Drawing.Point(136, 12);
            this.cbAdapters.Name = "cbAdapters";
            this.cbAdapters.Size = new System.Drawing.Size(256, 21);
            this.cbAdapters.TabIndex = 0;
            this.cbAdapters.SelectedIndexChanged += new System.EventHandler(this.cbAdapters_SelectedIndexChanged);
            // 
            // lblAdapters
            // 
            this.lblAdapters.AutoSize = true;
            this.lblAdapters.Location = new System.Drawing.Point(16, 16);
            this.lblAdapters.Name = "lblAdapters";
            this.lblAdapters.Size = new System.Drawing.Size(94, 13);
            this.lblAdapters.TabIndex = 1;
            this.lblAdapters.Text = "Network adapters:";
            // 
            // txtPreferredDNS
            // 
            this.txtPreferredDNS.Location = new System.Drawing.Point(136, 84);
            this.txtPreferredDNS.Name = "txtPreferredDNS";
            this.txtPreferredDNS.Size = new System.Drawing.Size(256, 20);
            this.txtPreferredDNS.TabIndex = 3;
            this.txtPreferredDNS.TextChanged += new System.EventHandler(this.txtPreferredDNS_TextChanged);
            // 
            // lblPreferredDNS
            // 
            this.lblPreferredDNS.AutoSize = true;
            this.lblPreferredDNS.Location = new System.Drawing.Point(16, 88);
            this.lblPreferredDNS.Name = "lblPreferredDNS";
            this.lblPreferredDNS.Size = new System.Drawing.Size(111, 13);
            this.lblPreferredDNS.TabIndex = 3;
            this.lblPreferredDNS.Text = "Preferred DNS server:";
            // 
            // lblAlternateDNS
            // 
            this.lblAlternateDNS.AutoSize = true;
            this.lblAlternateDNS.Location = new System.Drawing.Point(16, 112);
            this.lblAlternateDNS.Name = "lblAlternateDNS";
            this.lblAlternateDNS.Size = new System.Drawing.Size(110, 13);
            this.lblAlternateDNS.TabIndex = 4;
            this.lblAlternateDNS.Text = "Alternate DNS server:";
            // 
            // txtAlternateDNS
            // 
            this.txtAlternateDNS.Location = new System.Drawing.Point(136, 108);
            this.txtAlternateDNS.Name = "txtAlternateDNS";
            this.txtAlternateDNS.Size = new System.Drawing.Size(256, 20);
            this.txtAlternateDNS.TabIndex = 4;
            this.txtAlternateDNS.TextChanged += new System.EventHandler(this.txtAlternateDNS_TextChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(224, 136);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 24);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Apply";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(312, 136);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cbDNSType
            // 
            this.cbDNSType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDNSType.FormattingEnabled = true;
            this.cbDNSType.Location = new System.Drawing.Point(136, 60);
            this.cbDNSType.Name = "cbDNSType";
            this.cbDNSType.Size = new System.Drawing.Size(256, 21);
            this.cbDNSType.TabIndex = 2;
            this.cbDNSType.SelectedIndexChanged += new System.EventHandler(this.cbDNSType_SelectedIndexChanged);
            // 
            // lblDNS
            // 
            this.lblDNS.AutoSize = true;
            this.lblDNS.Location = new System.Drawing.Point(16, 64);
            this.lblDNS.Name = "lblDNS";
            this.lblDNS.Size = new System.Drawing.Size(70, 13);
            this.lblDNS.TabIndex = 9;
            this.lblDNS.Text = "DNS servers:";
            // 
            // cbAutomatic
            // 
            this.cbAutomatic.AutoSize = true;
            this.cbAutomatic.Location = new System.Drawing.Point(19, 40);
            this.cbAutomatic.Name = "cbAutomatic";
            this.cbAutomatic.Size = new System.Drawing.Size(219, 17);
            this.cbAutomatic.TabIndex = 1;
            this.cbAutomatic.Text = "Obtain DNS server address automatically";
            this.cbAutomatic.UseVisualStyleBackColor = true;
            this.cbAutomatic.CheckedChanged += new System.EventHandler(this.cbAutomatic_CheckedChanged);
            // 
            // DNSChangerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 170);
            this.Controls.Add(this.cbAutomatic);
            this.Controls.Add(this.lblDNS);
            this.Controls.Add(this.cbDNSType);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtAlternateDNS);
            this.Controls.Add(this.lblAlternateDNS);
            this.Controls.Add(this.lblPreferredDNS);
            this.Controls.Add(this.txtPreferredDNS);
            this.Controls.Add(this.lblAdapters);
            this.Controls.Add(this.cbAdapters);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "DNSChangerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - DNS changer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbAdapters;
        private System.Windows.Forms.Label lblAdapters;
        private System.Windows.Forms.TextBox txtPreferredDNS;
        private System.Windows.Forms.Label lblPreferredDNS;
        private System.Windows.Forms.Label lblAlternateDNS;
        private System.Windows.Forms.TextBox txtAlternateDNS;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cbDNSType;
        private System.Windows.Forms.Label lblDNS;
        private System.Windows.Forms.CheckBox cbAutomatic;
    }
}