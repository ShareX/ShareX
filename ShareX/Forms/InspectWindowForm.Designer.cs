
namespace ShareX
{
    partial class InspectWindowForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InspectWindowForm));
            this.rtbInfo = new System.Windows.Forms.RichTextBox();
            this.pInfo = new System.Windows.Forms.Panel();
            this.btnInspectWindow = new System.Windows.Forms.Button();
            this.btnInspectControl = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.cmsWindowList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cbTopMost = new System.Windows.Forms.CheckBox();
            this.lblOpacity = new System.Windows.Forms.Label();
            this.nudOpacity = new System.Windows.Forms.NumericUpDown();
            this.lblOpacityTip = new System.Windows.Forms.Label();
            this.mbWindowList = new ShareX.HelpersLib.MenuButton();
            this.pInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOpacity)).BeginInit();
            this.SuspendLayout();
            // 
            // rtbInfo
            // 
            this.rtbInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbInfo.DetectUrls = false;
            resources.ApplyResources(this.rtbInfo, "rtbInfo");
            this.rtbInfo.Name = "rtbInfo";
            this.rtbInfo.ReadOnly = true;
            // 
            // pInfo
            // 
            resources.ApplyResources(this.pInfo, "pInfo");
            this.pInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pInfo.Controls.Add(this.rtbInfo);
            this.pInfo.Name = "pInfo";
            // 
            // btnInspectWindow
            // 
            resources.ApplyResources(this.btnInspectWindow, "btnInspectWindow");
            this.btnInspectWindow.Name = "btnInspectWindow";
            this.btnInspectWindow.UseVisualStyleBackColor = true;
            this.btnInspectWindow.Click += new System.EventHandler(this.btnInspectWindow_Click);
            // 
            // btnInspectControl
            // 
            resources.ApplyResources(this.btnInspectControl, "btnInspectControl");
            this.btnInspectControl.Name = "btnInspectControl";
            this.btnInspectControl.UseVisualStyleBackColor = true;
            this.btnInspectControl.Click += new System.EventHandler(this.btnInspectControl_Click);
            // 
            // btnRefresh
            // 
            resources.ApplyResources(this.btnRefresh, "btnRefresh");
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // cmsWindowList
            // 
            this.cmsWindowList.Name = "cmsWindowList";
            resources.ApplyResources(this.cmsWindowList, "cmsWindowList");
            // 
            // cbTopMost
            // 
            resources.ApplyResources(this.cbTopMost, "cbTopMost");
            this.cbTopMost.Name = "cbTopMost";
            this.cbTopMost.UseVisualStyleBackColor = true;
            this.cbTopMost.CheckedChanged += new System.EventHandler(this.cbTopMost_CheckedChanged);
            // 
            // lblOpacity
            // 
            resources.ApplyResources(this.lblOpacity, "lblOpacity");
            this.lblOpacity.Name = "lblOpacity";
            // 
            // nudOpacity
            // 
            this.nudOpacity.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nudOpacity, "nudOpacity");
            this.nudOpacity.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudOpacity.Name = "nudOpacity";
            this.nudOpacity.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudOpacity.ValueChanged += new System.EventHandler(this.nudOpacity_ValueChanged);
            // 
            // lblOpacityTip
            // 
            resources.ApplyResources(this.lblOpacityTip, "lblOpacityTip");
            this.lblOpacityTip.Name = "lblOpacityTip";
            // 
            // mbWindowList
            // 
            resources.ApplyResources(this.mbWindowList, "mbWindowList");
            this.mbWindowList.Menu = this.cmsWindowList;
            this.mbWindowList.Name = "mbWindowList";
            this.mbWindowList.UseVisualStyleBackColor = true;
            this.mbWindowList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mbWindowList_MouseDown);
            // 
            // InspectWindowForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblOpacityTip);
            this.Controls.Add(this.nudOpacity);
            this.Controls.Add(this.lblOpacity);
            this.Controls.Add(this.cbTopMost);
            this.Controls.Add(this.mbWindowList);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnInspectControl);
            this.Controls.Add(this.btnInspectWindow);
            this.Controls.Add(this.pInfo);
            this.Name = "InspectWindowForm";
            this.pInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudOpacity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbInfo;
        private System.Windows.Forms.Panel pInfo;
        private System.Windows.Forms.Button btnInspectWindow;
        private System.Windows.Forms.Button btnInspectControl;
        private System.Windows.Forms.Button btnRefresh;
        private HelpersLib.MenuButton mbWindowList;
        private System.Windows.Forms.ContextMenuStrip cmsWindowList;
        private System.Windows.Forms.CheckBox cbTopMost;
        private System.Windows.Forms.Label lblOpacity;
        private System.Windows.Forms.NumericUpDown nudOpacity;
        private System.Windows.Forms.Label lblOpacityTip;
    }
}