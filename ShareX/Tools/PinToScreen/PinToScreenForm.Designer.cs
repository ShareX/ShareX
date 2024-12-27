using ShareX.HelpersLib.Controls;

namespace ShareX
{
    partial class PinToScreenForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PinToScreenForm));
            tsMain = new ToolStripEx();
            tsbCopy = new System.Windows.Forms.ToolStripButton();
            tslScale = new System.Windows.Forms.ToolStripLabel();
            tsbOptions = new System.Windows.Forms.ToolStripButton();
            tsbClose = new System.Windows.Forms.ToolStripButton();
            tsbSave = new System.Windows.Forms.ToolStripButton();
            tsMain.SuspendLayout();
            SuspendLayout();
            // 
            // tsMain
            // 
            tsMain.ClickThrough = true;
            resources.ApplyResources(tsMain, "tsMain");
            tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tsbCopy, tsbSave, tslScale, tsbOptions, tsbClose });
            tsMain.Name = "tsMain";
            tsMain.MouseLeave += TsMain_MouseLeave;
            // 
            // tsbCopy
            // 
            tsbCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbCopy.Image = Properties.Resources.document_copy;
            resources.ApplyResources(tsbCopy, "tsbCopy");
            tsbCopy.Name = "tsbCopy";
            tsbCopy.Padding = new System.Windows.Forms.Padding(4);
            tsbCopy.Click += TsbCopy_Click;
            // 
            // tslScale
            // 
            resources.ApplyResources(tslScale, "tslScale");
            tslScale.Name = "tslScale";
            tslScale.Padding = new System.Windows.Forms.Padding(4);
            tslScale.Click += TslScale_Click;
            // 
            // tsbOptions
            // 
            tsbOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbOptions.Image = Properties.Resources.gear;
            resources.ApplyResources(tsbOptions, "tsbOptions");
            tsbOptions.Name = "tsbOptions";
            tsbOptions.Padding = new System.Windows.Forms.Padding(4);
            tsbOptions.Click += tsbOptions_Click;
            // 
            // tsbClose
            // 
            tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbClose.Image = Properties.Resources.cross_button;
            resources.ApplyResources(tsbClose, "tsbClose");
            tsbClose.Name = "tsbClose";
            tsbClose.Padding = new System.Windows.Forms.Padding(4);
            tsbClose.Click += tsbClose_Click;
            // 
            // tsbSave
            // 
            tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbSave.Image = Properties.Resources.disk;
            resources.ApplyResources(tsbSave, "tsbSave");
            tsbSave.Name = "tsbSave";
            tsbSave.Click += TsbSave_Click;
            // 
            // PinToScreenForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(tsMain);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Name = "PinToScreenForm";
            ShowInTaskbar = false;
            KeyUp += PinToScreenForm_KeyUp;
            MouseDown += PinToScreenForm_MouseDown;
            MouseEnter += PinToScreenForm_MouseEnter;
            MouseLeave += PinToScreenForm_MouseLeave;
            MouseMove += PinToScreenForm_MouseMove;
            MouseUp += PinToScreenForm_MouseUp;
            MouseWheel += PinToScreenForm_MouseWheel;
            tsMain.ResumeLayout(false);
            tsMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private ToolStripEx tsMain;
        private System.Windows.Forms.ToolStripButton tsbOptions;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripLabel tslScale;
        private System.Windows.Forms.ToolStripButton tsbCopy;
        private System.Windows.Forms.ToolStripButton tsbSave;
    }
}