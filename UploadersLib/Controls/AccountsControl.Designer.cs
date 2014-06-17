namespace UploadersLib
{
    public partial class AccountsControl
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
            this.pgSettings = new System.Windows.Forms.PropertyGrid();
            this.lbAccounts = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.AccountsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.btnDuplicate = new System.Windows.Forms.Button();
            this.AccountsLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // pgSettings
            // 
            this.pgSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgSettings.Location = new System.Drawing.Point(200, 0);
            this.pgSettings.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.pgSettings.Name = "pgSettings";
            this.pgSettings.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgSettings.Size = new System.Drawing.Size(544, 352);
            this.pgSettings.TabIndex = 1;
            this.pgSettings.ToolbarVisible = false;
            this.pgSettings.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgSettings_PropertyValueChanged);
            // 
            // lbAccounts
            // 
            this.lbAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbAccounts.FormattingEnabled = true;
            this.lbAccounts.IntegralHeight = false;
            this.lbAccounts.Location = new System.Drawing.Point(0, 0);
            this.lbAccounts.Margin = new System.Windows.Forms.Padding(0);
            this.lbAccounts.Name = "lbAccounts";
            this.lbAccounts.Size = new System.Drawing.Size(197, 352);
            this.lbAccounts.TabIndex = 0;
            this.lbAccounts.SelectedIndexChanged += new System.EventHandler(this.lbAccounts_SelectedIndexChanged);
            // 
            // btnAdd
            // 
            this.btnAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAdd.Location = new System.Drawing.Point(8, 8);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(64, 24);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnTest
            // 
            this.btnTest.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnTest.Location = new System.Drawing.Point(224, 8);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(64, 24);
            this.btnTest.TabIndex = 2;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            // 
            // btnRemove
            // 
            this.btnRemove.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRemove.Location = new System.Drawing.Point(80, 8);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(64, 24);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // AccountsLayout
            // 
            this.AccountsLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AccountsLayout.ColumnCount = 2;
            this.AccountsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.54987F));
            this.AccountsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.45013F));
            this.AccountsLayout.Controls.Add(this.pgSettings, 1, 0);
            this.AccountsLayout.Controls.Add(this.lbAccounts, -1, 0);
            this.AccountsLayout.Location = new System.Drawing.Point(8, 40);
            this.AccountsLayout.Name = "AccountsLayout";
            this.AccountsLayout.RowCount = 1;
            this.AccountsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.AccountsLayout.Size = new System.Drawing.Size(744, 352);
            this.AccountsLayout.TabIndex = 4;
            // 
            // btnDuplicate
            // 
            this.btnDuplicate.Location = new System.Drawing.Point(152, 8);
            this.btnDuplicate.Name = "btnDuplicate";
            this.btnDuplicate.Size = new System.Drawing.Size(64, 24);
            this.btnDuplicate.TabIndex = 3;
            this.btnDuplicate.Text = "Duplicate";
            this.btnDuplicate.UseVisualStyleBackColor = true;
            // 
            // AccountsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnDuplicate);
            this.Controls.Add(this.AccountsLayout);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnRemove);
            this.Name = "AccountsControl";
            this.Size = new System.Drawing.Size(760, 400);
            this.AccountsLayout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion Component Designer generated code

        public System.Windows.Forms.Button btnAdd;
        public System.Windows.Forms.Button btnTest;
        public System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.TableLayoutPanel AccountsLayout;
        public System.Windows.Forms.PropertyGrid pgSettings;
        public System.Windows.Forms.ListBox lbAccounts;
        public System.Windows.Forms.Button btnDuplicate;
    }
}