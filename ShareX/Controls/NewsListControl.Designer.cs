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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
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
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(5);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvNews.DefaultCellStyle = dataGridViewCellStyle3;
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
            this.chDateTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.chDateTime.DefaultCellStyle = dataGridViewCellStyle1;
            this.chDateTime.HeaderText = "DateTime";
            this.chDateTime.Name = "chDateTime";
            this.chDateTime.ReadOnly = true;
            this.chDateTime.Width = 5;
            // 
            // chText
            // 
            this.chText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(5);
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.chText.DefaultCellStyle = dataGridViewCellStyle2;
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
