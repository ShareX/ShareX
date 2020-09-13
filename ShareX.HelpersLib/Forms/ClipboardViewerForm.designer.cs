namespace ShareX.HelpersLib
{
    partial class ClipboardViewerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClipboardViewerForm));
            this.txtSelectedClipboardContent = new System.Windows.Forms.TextBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClearClipboard = new System.Windows.Forms.Button();
            this.lvClipboardContentList = new System.Windows.Forms.ListView();
            this.chFormat = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pbSelectedClipboardContent = new ShareX.HelpersLib.MyPictureBox();
            this.SuspendLayout();
            // 
            // txtSelectedClipboardContent
            // 
            resources.ApplyResources(this.txtSelectedClipboardContent, "txtSelectedClipboardContent");
            this.txtSelectedClipboardContent.Name = "txtSelectedClipboardContent";
            this.txtSelectedClipboardContent.ReadOnly = true;
            // 
            // btnRefresh
            // 
            resources.ApplyResources(this.btnRefresh, "btnRefresh");
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnClearClipboard
            // 
            resources.ApplyResources(this.btnClearClipboard, "btnClearClipboard");
            this.btnClearClipboard.Name = "btnClearClipboard";
            this.btnClearClipboard.UseVisualStyleBackColor = true;
            this.btnClearClipboard.Click += new System.EventHandler(this.btnClearClipboard_Click);
            // 
            // lvClipboardContentList
            // 
            resources.ApplyResources(this.lvClipboardContentList, "lvClipboardContentList");
            this.lvClipboardContentList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFormat});
            this.lvClipboardContentList.FullRowSelect = true;
            this.lvClipboardContentList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvClipboardContentList.HideSelection = false;
            this.lvClipboardContentList.MultiSelect = false;
            this.lvClipboardContentList.Name = "lvClipboardContentList";
            this.lvClipboardContentList.UseCompatibleStateImageBehavior = false;
            this.lvClipboardContentList.View = System.Windows.Forms.View.Details;
            this.lvClipboardContentList.SelectedIndexChanged += new System.EventHandler(this.lvClipboardContentList_SelectedIndexChanged);
            // 
            // chFormat
            // 
            resources.ApplyResources(this.chFormat, "chFormat");
            // 
            // pbSelectedClipboardContent
            // 
            resources.ApplyResources(this.pbSelectedClipboardContent, "pbSelectedClipboardContent");
            this.pbSelectedClipboardContent.BackColor = System.Drawing.SystemColors.Window;
            this.pbSelectedClipboardContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbSelectedClipboardContent.DrawCheckeredBackground = true;
            this.pbSelectedClipboardContent.FullscreenOnClick = true;
            this.pbSelectedClipboardContent.Name = "pbSelectedClipboardContent";
            this.pbSelectedClipboardContent.PictureBoxBackColor = System.Drawing.SystemColors.Window;
            this.pbSelectedClipboardContent.ShowImageSizeLabel = true;
            // 
            // ClipboardViewerForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pbSelectedClipboardContent);
            this.Controls.Add(this.lvClipboardContentList);
            this.Controls.Add(this.btnClearClipboard);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.txtSelectedClipboardContent);
            this.DoubleBuffered = true;
            this.Name = "ClipboardViewerForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtSelectedClipboardContent;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClearClipboard;
        private System.Windows.Forms.ListView lvClipboardContentList;
        private System.Windows.Forms.ColumnHeader chFormat;
        private MyPictureBox pbSelectedClipboardContent;
    }
}