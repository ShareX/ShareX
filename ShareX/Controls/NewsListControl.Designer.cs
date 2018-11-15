namespace ShareX
{
    partial class NewsListControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvNews = new System.Windows.Forms.DataGridView();
            this.chIsUnread = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNews)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvNews
            // 
            this.dgvNews.AllowUserToAddRows = false;
            this.dgvNews.AllowUserToDeleteRows = false;
            this.dgvNews.AllowUserToResizeColumns = false;
            this.dgvNews.AllowUserToResizeRows = false;
            this.dgvNews.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvNews.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvNews.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvNews.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvNews.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNews.ColumnHeadersVisible = false;
            this.dgvNews.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chIsUnread,
            this.chDateTime,
            this.chText});
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle9.Padding = new System.Windows.Forms.Padding(5);
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvNews.DefaultCellStyle = dataGridViewCellStyle9;
            this.dgvNews.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvNews.Location = new System.Drawing.Point(0, 0);
            this.dgvNews.Name = "dgvNews";
            this.dgvNews.RowHeadersVisible = false;
            this.dgvNews.Size = new System.Drawing.Size(399, 363);
            this.dgvNews.TabIndex = 0;
            this.dgvNews.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvNews_CellMouseClick);
            this.dgvNews.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNews_CellMouseEnter);
            this.dgvNews.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNews_CellMouseLeave);
            // 
            // chIsUnread
            // 
            this.chIsUnread.HeaderText = "IsUnread";
            this.chIsUnread.Name = "chIsUnread";
            this.chIsUnread.ReadOnly = true;
            this.chIsUnread.Width = 5;
            // 
            // chDateTime
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.chDateTime.DefaultCellStyle = dataGridViewCellStyle7;
            this.chDateTime.HeaderText = "DateTime";
            this.chDateTime.Name = "chDateTime";
            this.chDateTime.ReadOnly = true;
            // 
            // chText
            // 
            this.chText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle8.Padding = new System.Windows.Forms.Padding(5);
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.chText.DefaultCellStyle = dataGridViewCellStyle8;
            this.chText.HeaderText = "Text";
            this.chText.Name = "chText";
            this.chText.ReadOnly = true;
            // 
            // NewsListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.dgvNews);
            this.Name = "NewsListControl";
            this.Size = new System.Drawing.Size(399, 363);
            ((System.ComponentModel.ISupportInitialize)(this.dgvNews)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvNews;
        private System.Windows.Forms.DataGridViewTextBoxColumn chIsUnread;
        private System.Windows.Forms.DataGridViewTextBoxColumn chDateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn chText;
    }
}
