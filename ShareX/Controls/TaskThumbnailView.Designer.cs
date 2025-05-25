namespace ShareX
{
    partial class TaskThumbnailView
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
            this.flpMain = new System.Windows.Forms.FlowLayoutPanel();
            this.pMain = new System.Windows.Forms.Panel();
            this.sbMain = new ShareX.HelpersLib.CustomVScrollBar();
            this.pMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // flpMain
            // 
            this.flpMain.AutoSize = true;
            this.flpMain.Location = new System.Drawing.Point(0, 0);
            this.flpMain.Name = "flpMain";
            this.flpMain.Padding = new System.Windows.Forms.Padding(5, 3, 5, 5);
            this.flpMain.Size = new System.Drawing.Size(128, 128);
            this.flpMain.TabIndex = 0;
            this.flpMain.SizeChanged += new System.EventHandler(this.flpMain_SizeChanged);
            this.flpMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Panel_MouseDown);
            this.flpMain.MouseEnter += new System.EventHandler(this.Panel_MouseEnter);
            this.flpMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Panel_MouseUp);
            // 
            // pMain
            // 
            this.pMain.Controls.Add(this.flpMain);
            this.pMain.Controls.Add(this.sbMain);
            this.pMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pMain.Location = new System.Drawing.Point(0, 0);
            this.pMain.Name = "pMain";
            this.pMain.Size = new System.Drawing.Size(242, 228);
            this.pMain.TabIndex = 1;
            this.pMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Panel_MouseDown);
            this.pMain.MouseEnter += new System.EventHandler(this.Panel_MouseEnter);
            this.pMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Panel_MouseUp);
            this.pMain.Resize += new System.EventHandler(this.pMain_Resize);
            // 
            // sbMain
            // 
            this.sbMain.Dock = System.Windows.Forms.DockStyle.Right;
            this.sbMain.LargeScrollStep = 100;
            this.sbMain.Location = new System.Drawing.Point(224, 0);
            this.sbMain.Maximum = 0;
            this.sbMain.Minimum = 0;
            this.sbMain.Name = "sbMain";
            this.sbMain.PageSize = 0;
            this.sbMain.Size = new System.Drawing.Size(18, 228);
            this.sbMain.SmallScrollStep = 20;
            this.sbMain.TabIndex = 2;
            this.sbMain.Text = "customVScrollBar1";
            this.sbMain.Value = 0;
            this.sbMain.ValueChanged += new System.EventHandler(this.sbMain_ValueChanged);
            // 
            // TaskThumbnailView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(47)))), ((int)(((byte)(56)))));
            this.Controls.Add(this.pMain);
            this.Name = "TaskThumbnailView";
            this.Size = new System.Drawing.Size(242, 228);
            this.SizeChanged += new System.EventHandler(this.TaskThumbnailView_SizeChanged);
            this.VisibleChanged += new System.EventHandler(this.TaskThumbnailView_VisibleChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Panel_MouseDown);
            this.MouseEnter += new System.EventHandler(this.Panel_MouseEnter);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Panel_MouseUp);
            this.pMain.ResumeLayout(false);
            this.pMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpMain;
        private System.Windows.Forms.Panel pMain;
        private HelpersLib.CustomVScrollBar sbMain;
    }
}
