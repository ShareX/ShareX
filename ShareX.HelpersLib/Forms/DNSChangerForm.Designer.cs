namespace ShareX.HelpersLib
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DNSChangerForm));
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
            this.btnPingPrimary = new System.Windows.Forms.Button();
            this.btnPingSecondary = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbAdapters
            // 
            this.cbAdapters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAdapters.FormattingEnabled = true;
            resources.ApplyResources(this.cbAdapters, "cbAdapters");
            this.cbAdapters.Name = "cbAdapters";
            this.cbAdapters.SelectedIndexChanged += new System.EventHandler(this.cbAdapters_SelectedIndexChanged);
            // 
            // lblAdapters
            // 
            resources.ApplyResources(this.lblAdapters, "lblAdapters");
            this.lblAdapters.Name = "lblAdapters";
            // 
            // txtPreferredDNS
            // 
            resources.ApplyResources(this.txtPreferredDNS, "txtPreferredDNS");
            this.txtPreferredDNS.Name = "txtPreferredDNS";
            this.txtPreferredDNS.TextChanged += new System.EventHandler(this.txtPreferredDNS_TextChanged);
            // 
            // lblPreferredDNS
            // 
            resources.ApplyResources(this.lblPreferredDNS, "lblPreferredDNS");
            this.lblPreferredDNS.Name = "lblPreferredDNS";
            // 
            // lblAlternateDNS
            // 
            resources.ApplyResources(this.lblAlternateDNS, "lblAlternateDNS");
            this.lblAlternateDNS.Name = "lblAlternateDNS";
            // 
            // txtAlternateDNS
            // 
            resources.ApplyResources(this.txtAlternateDNS, "txtAlternateDNS");
            this.txtAlternateDNS.Name = "txtAlternateDNS";
            this.txtAlternateDNS.TextChanged += new System.EventHandler(this.txtAlternateDNS_TextChanged);
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cbDNSType
            // 
            this.cbDNSType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDNSType.FormattingEnabled = true;
            resources.ApplyResources(this.cbDNSType, "cbDNSType");
            this.cbDNSType.Name = "cbDNSType";
            this.cbDNSType.SelectedIndexChanged += new System.EventHandler(this.cbDNSType_SelectedIndexChanged);
            // 
            // lblDNS
            // 
            resources.ApplyResources(this.lblDNS, "lblDNS");
            this.lblDNS.Name = "lblDNS";
            // 
            // cbAutomatic
            // 
            resources.ApplyResources(this.cbAutomatic, "cbAutomatic");
            this.cbAutomatic.Name = "cbAutomatic";
            this.cbAutomatic.UseVisualStyleBackColor = true;
            this.cbAutomatic.CheckedChanged += new System.EventHandler(this.cbAutomatic_CheckedChanged);
            // 
            // btnPingPrimary
            // 
            resources.ApplyResources(this.btnPingPrimary, "btnPingPrimary");
            this.btnPingPrimary.Name = "btnPingPrimary";
            this.btnPingPrimary.UseVisualStyleBackColor = true;
            this.btnPingPrimary.Click += new System.EventHandler(this.btnPingPrimary_Click);
            // 
            // btnPingSecondary
            // 
            resources.ApplyResources(this.btnPingSecondary, "btnPingSecondary");
            this.btnPingSecondary.Name = "btnPingSecondary";
            this.btnPingSecondary.UseVisualStyleBackColor = true;
            this.btnPingSecondary.Click += new System.EventHandler(this.btnPingSecondary_Click);
            // 
            // DNSChangerForm
            // 
            this.AcceptButton = this.btnSave;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.btnPingSecondary);
            this.Controls.Add(this.btnPingPrimary);
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
        private System.Windows.Forms.Button btnPingPrimary;
        private System.Windows.Forms.Button btnPingSecondary;
    }
}