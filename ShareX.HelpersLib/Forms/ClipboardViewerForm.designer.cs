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
            this.txtSelectedClipboardContent = new System.Windows.Forms.TextBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClearClipboard = new System.Windows.Forms.Button();
            this.lvClipboardContentList = new System.Windows.Forms.ListView();
            this.chFormat = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pbSelectedClipboardContent = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbSelectedClipboardContent)).BeginInit();
            this.SuspendLayout();
            // 
            // txtSelectedClipboardContent
            // 
            this.txtSelectedClipboardContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSelectedClipboardContent.Location = new System.Drawing.Point(264, 8);
            this.txtSelectedClipboardContent.Multiline = true;
            this.txtSelectedClipboardContent.Name = "txtSelectedClipboardContent";
            this.txtSelectedClipboardContent.ReadOnly = true;
            this.txtSelectedClipboardContent.Size = new System.Drawing.Size(512, 544);
            this.txtSelectedClipboardContent.TabIndex = 3;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(8, 8);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 32);
            this.btnRefresh.TabIndex = 0;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnClearClipboard
            // 
            this.btnClearClipboard.Location = new System.Drawing.Point(136, 8);
            this.btnClearClipboard.Name = "btnClearClipboard";
            this.btnClearClipboard.Size = new System.Drawing.Size(120, 32);
            this.btnClearClipboard.TabIndex = 1;
            this.btnClearClipboard.Text = "Clear clipboard";
            this.btnClearClipboard.UseVisualStyleBackColor = true;
            this.btnClearClipboard.Click += new System.EventHandler(this.btnClearClipboard_Click);
            // 
            // lvClipboardContentList
            // 
            this.lvClipboardContentList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lvClipboardContentList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFormat});
            this.lvClipboardContentList.FullRowSelect = true;
            this.lvClipboardContentList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvClipboardContentList.HideSelection = false;
            this.lvClipboardContentList.Location = new System.Drawing.Point(8, 48);
            this.lvClipboardContentList.MultiSelect = false;
            this.lvClipboardContentList.Name = "lvClipboardContentList";
            this.lvClipboardContentList.Size = new System.Drawing.Size(248, 504);
            this.lvClipboardContentList.TabIndex = 2;
            this.lvClipboardContentList.UseCompatibleStateImageBehavior = false;
            this.lvClipboardContentList.View = System.Windows.Forms.View.Details;
            this.lvClipboardContentList.SelectedIndexChanged += new System.EventHandler(this.lvClipboardContentList_SelectedIndexChanged);
            // 
            // chFormat
            // 
            this.chFormat.Text = "Format";
            this.chFormat.Width = 244;
            // 
            // pbSelectedClipboardContent
            // 
            this.pbSelectedClipboardContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbSelectedClipboardContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbSelectedClipboardContent.Location = new System.Drawing.Point(264, 8);
            this.pbSelectedClipboardContent.Name = "pbSelectedClipboardContent";
            this.pbSelectedClipboardContent.Size = new System.Drawing.Size(512, 544);
            this.pbSelectedClipboardContent.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbSelectedClipboardContent.TabIndex = 4;
            this.pbSelectedClipboardContent.TabStop = false;
            // 
            // ClipboardViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.pbSelectedClipboardContent);
            this.Controls.Add(this.lvClipboardContentList);
            this.Controls.Add(this.btnClearClipboard);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.txtSelectedClipboardContent);
            this.DoubleBuffered = true;
            this.Name = "ClipboardViewerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Clipboard viewer";
            ((System.ComponentModel.ISupportInitialize)(this.pbSelectedClipboardContent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtSelectedClipboardContent;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClearClipboard;
        private System.Windows.Forms.ListView lvClipboardContentList;
        private System.Windows.Forms.ColumnHeader chFormat;
        private System.Windows.Forms.PictureBox pbSelectedClipboardContent;
    }
}