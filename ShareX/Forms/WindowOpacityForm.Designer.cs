namespace ShareX.HelpersLib
{
    partial class WindowOpacityForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WindowOpacityForm));
            this.btnSelectWindow = new System.Windows.Forms.Button();
            this.lblSelectedItem = new System.Windows.Forms.Label();
            this.trkbOpacityValue = new System.Windows.Forms.TrackBar();
            this.btnRefreshOpacitySelectedElemet = new System.Windows.Forms.Button();
            this.btnSelectControl = new System.Windows.Forms.Button();
            this.tbSelectedItemData = new System.Windows.Forms.TextBox();
            this.lblOpacityValue = new System.Windows.Forms.Label();
            this.tbOpacityValue = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.trkbOpacityValue)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelectWindow
            // 
            resources.ApplyResources(this.btnSelectWindow, "btnSelectWindow");
            this.btnSelectWindow.Name = "btnSelectWindow";
            this.btnSelectWindow.UseVisualStyleBackColor = true;
            this.btnSelectWindow.Click += new System.EventHandler(this.btnSelectWindow_Click);
            // 
            // lblSelectedItem
            // 
            resources.ApplyResources(this.lblSelectedItem, "lblSelectedItem");
            this.lblSelectedItem.Name = "lblSelectedItem";
            // 
            // trkbOpacityValue
            // 
            resources.ApplyResources(this.trkbOpacityValue, "trkbOpacityValue");
            this.trkbOpacityValue.LargeChange = 10;
            this.trkbOpacityValue.Maximum = 100;
            this.trkbOpacityValue.Name = "trkbOpacityValue";
            this.trkbOpacityValue.SmallChange = 2;
            this.trkbOpacityValue.TickFrequency = 2;
            this.trkbOpacityValue.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trkbOpacityValue.Value = 100;
            this.trkbOpacityValue.ValueChanged += new System.EventHandler(this.trkbOpacityValue_ValueChanged);
            // 
            // btnRefreshOpacitySelectedElemet
            // 
            resources.ApplyResources(this.btnRefreshOpacitySelectedElemet, "btnRefreshOpacitySelectedElemet");
            this.btnRefreshOpacitySelectedElemet.Name = "btnRefreshOpacitySelectedElemet";
            this.btnRefreshOpacitySelectedElemet.UseVisualStyleBackColor = true;
            this.btnRefreshOpacitySelectedElemet.Click += new System.EventHandler(this.btnRefreshOpacitySelectedElemet_Click);
            // 
            // btnSelectControl
            // 
            resources.ApplyResources(this.btnSelectControl, "btnSelectControl");
            this.btnSelectControl.Name = "btnSelectControl";
            this.btnSelectControl.UseVisualStyleBackColor = true;
            this.btnSelectControl.Click += new System.EventHandler(this.btnSelectControl_Click);
            // 
            // tbSelectedItemData
            // 
            resources.ApplyResources(this.tbSelectedItemData, "tbSelectedItemData");
            this.tbSelectedItemData.Name = "tbSelectedItemData";
            this.tbSelectedItemData.ReadOnly = true;
            // 
            // lblOpacityValue
            // 
            resources.ApplyResources(this.lblOpacityValue, "lblOpacityValue");
            this.lblOpacityValue.Name = "lblOpacityValue";
            // 
            // tbOpacityValue
            // 
            resources.ApplyResources(this.tbOpacityValue, "tbOpacityValue");
            this.tbOpacityValue.Name = "tbOpacityValue";
            this.tbOpacityValue.ReadOnly = true;
            // 
            // WindowOpacityForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbOpacityValue);
            this.Controls.Add(this.lblOpacityValue);
            this.Controls.Add(this.tbSelectedItemData);
            this.Controls.Add(this.btnSelectControl);
            this.Controls.Add(this.btnRefreshOpacitySelectedElemet);
            this.Controls.Add(this.trkbOpacityValue);
            this.Controls.Add(this.lblSelectedItem);
            this.Controls.Add(this.btnSelectWindow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "WindowOpacityForm";
            ((System.ComponentModel.ISupportInitialize)(this.trkbOpacityValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectWindow;
        private System.Windows.Forms.Label lblSelectedItem;
        private System.Windows.Forms.TrackBar trkbOpacityValue;
        private System.Windows.Forms.Button btnRefreshOpacitySelectedElemet;
        private System.Windows.Forms.Button btnSelectControl;
        private System.Windows.Forms.TextBox tbSelectedItemData;
        private System.Windows.Forms.Label lblOpacityValue;
        private System.Windows.Forms.TextBox tbOpacityValue;
    }
}