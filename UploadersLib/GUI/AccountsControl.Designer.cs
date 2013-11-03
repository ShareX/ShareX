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
            this.SettingsGrid = new System.Windows.Forms.PropertyGrid();
            this.AccountsList = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.AccountsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.btnClone = new System.Windows.Forms.Button();
            this.AccountsLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // SettingsGrid
            // 
            this.SettingsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SettingsGrid.Location = new System.Drawing.Point(258, 3);
            this.SettingsGrid.Name = "SettingsGrid";
            this.SettingsGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.SettingsGrid.Size = new System.Drawing.Size(481, 345);
            this.SettingsGrid.TabIndex = 1;
            this.SettingsGrid.ToolbarVisible = false;
            this.SettingsGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.SettingsGrid_PropertyValueChanged);
            // 
            // AccountsList
            // 
            this.AccountsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AccountsList.FormattingEnabled = true;
            this.AccountsList.IntegralHeight = false;
            this.AccountsList.Location = new System.Drawing.Point(3, 3);
            this.AccountsList.Name = "AccountsList";
            this.AccountsList.Size = new System.Drawing.Size(249, 345);
            this.AccountsList.TabIndex = 0;
            this.AccountsList.SelectedIndexChanged += new System.EventHandler(this.AccountsList_SelectedIndexChanged);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.Transparent;
            this.btnAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAdd.Location = new System.Drawing.Point(8, 8);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(64, 24);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            // 
            // btnTest
            // 
            this.btnTest.BackColor = System.Drawing.Color.Transparent;
            this.btnTest.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnTest.Location = new System.Drawing.Point(152, 8);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(64, 24);
            this.btnTest.TabIndex = 2;
            this.btnTest.Text = "Test...";
            this.btnTest.UseVisualStyleBackColor = false;
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
            this.AccountsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.4086F));
            this.AccountsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65.5914F));
            this.AccountsLayout.Controls.Add(this.SettingsGrid, 1, 0);
            this.AccountsLayout.Controls.Add(this.AccountsList, -1, 0);
            this.AccountsLayout.Location = new System.Drawing.Point(8, 40);
            this.AccountsLayout.Name = "AccountsLayout";
            this.AccountsLayout.RowCount = 1;
            this.AccountsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.AccountsLayout.Size = new System.Drawing.Size(742, 351);
            this.AccountsLayout.TabIndex = 4;
            // 
            // btnClone
            // 
            this.btnClone.Location = new System.Drawing.Point(224, 8);
            this.btnClone.Name = "btnClone";
            this.btnClone.Size = new System.Drawing.Size(72, 24);
            this.btnClone.TabIndex = 3;
            this.btnClone.Text = "&Clone";
            this.btnClone.UseVisualStyleBackColor = true;
            this.btnClone.Visible = false;
            // 
            // AccountsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnClone);
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
        public System.Windows.Forms.PropertyGrid SettingsGrid;
        public System.Windows.Forms.ListBox AccountsList;
        public System.Windows.Forms.Button btnClone;
    }
}