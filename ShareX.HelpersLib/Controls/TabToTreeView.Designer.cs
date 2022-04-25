namespace ShareX.HelpersLib
{
    partial class TabToTreeView
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
            this.tvMain = new System.Windows.Forms.TreeView();
            this.scMain = new System.Windows.Forms.SplitContainer();
            this.pSeparator = new System.Windows.Forms.Panel();
            this.tcMain = new ShareX.HelpersLib.TablessControl();
            this.pLeft = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            this.pLeft.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvMain
            // 
            this.tvMain.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tvMain.FullRowSelect = true;
            this.tvMain.HideSelection = false;
            this.tvMain.ItemHeight = 25;
            this.tvMain.Location = new System.Drawing.Point(8, 8);
            this.tvMain.Name = "tvMain";
            this.tvMain.ShowLines = false;
            this.tvMain.ShowPlusMinus = false;
            this.tvMain.Size = new System.Drawing.Size(221, 484);
            this.tvMain.TabIndex = 0;
            this.tvMain.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvMain_BeforeCollapse);
            this.tvMain.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvMain_AfterSelect);
            // 
            // scMain
            // 
            this.scMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scMain.IsSplitterFixed = true;
            this.scMain.Location = new System.Drawing.Point(0, 0);
            this.scMain.Margin = new System.Windows.Forms.Padding(0);
            this.scMain.Name = "scMain";
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.pSeparator);
            this.scMain.Panel1.Controls.Add(this.pLeft);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.tcMain);
            this.scMain.Size = new System.Drawing.Size(700, 500);
            this.scMain.SplitterDistance = 237;
            this.scMain.SplitterWidth = 3;
            this.scMain.TabIndex = 0;
            // 
            // pSeparator
            // 
            this.pSeparator.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pSeparator.Dock = System.Windows.Forms.DockStyle.Right;
            this.pSeparator.Location = new System.Drawing.Point(236, 0);
            this.pSeparator.MaximumSize = new System.Drawing.Size(1, 0);
            this.pSeparator.Name = "pSeparator";
            this.pSeparator.Size = new System.Drawing.Size(1, 500);
            this.pSeparator.TabIndex = 1;
            // 
            // tcMain
            // 
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 0);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(460, 500);
            this.tcMain.TabIndex = 0;
            this.tcMain.Visible = false;
            // 
            // pLeft
            // 
            this.pLeft.Controls.Add(this.tvMain);
            this.pLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pLeft.Location = new System.Drawing.Point(0, 0);
            this.pLeft.Name = "pLeft";
            this.pLeft.Padding = new System.Windows.Forms.Padding(8);
            this.pLeft.Size = new System.Drawing.Size(237, 500);
            this.pLeft.TabIndex = 2;
            // 
            // TabToTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.scMain);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "TabToTreeView";
            this.Size = new System.Drawing.Size(700, 500);
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
            this.pLeft.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvMain;
        private System.Windows.Forms.SplitContainer scMain;
        private TablessControl tcMain;
        private System.Windows.Forms.Panel pSeparator;
        private System.Windows.Forms.Panel pLeft;
    }
}
