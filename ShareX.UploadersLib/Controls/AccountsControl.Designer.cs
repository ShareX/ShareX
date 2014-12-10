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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountsControl));
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
            resources.ApplyResources(this.pgSettings, "pgSettings");
            this.pgSettings.Name = "pgSettings";
            this.pgSettings.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgSettings.ToolbarVisible = false;
            this.pgSettings.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgSettings_PropertyValueChanged);
            // 
            // lbAccounts
            // 
            resources.ApplyResources(this.lbAccounts, "lbAccounts");
            this.lbAccounts.FormattingEnabled = true;
            this.lbAccounts.Name = "lbAccounts";
            this.lbAccounts.SelectedIndexChanged += new System.EventHandler(this.lbAccounts_SelectedIndexChanged);
            // 
            // btnAdd
            // 
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnTest
            // 
            resources.ApplyResources(this.btnTest, "btnTest");
            this.btnTest.Name = "btnTest";
            this.btnTest.UseVisualStyleBackColor = true;
            // 
            // btnRemove
            // 
            resources.ApplyResources(this.btnRemove, "btnRemove");
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // AccountsLayout
            // 
            resources.ApplyResources(this.AccountsLayout, "AccountsLayout");
            this.AccountsLayout.Controls.Add(this.pgSettings, 1, 0);
            this.AccountsLayout.Controls.Add(this.lbAccounts, -1, 0);
            this.AccountsLayout.Name = "AccountsLayout";
            // 
            // btnDuplicate
            // 
            resources.ApplyResources(this.btnDuplicate, "btnDuplicate");
            this.btnDuplicate.Name = "btnDuplicate";
            this.btnDuplicate.UseVisualStyleBackColor = true;
            // 
            // AccountsControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnDuplicate);
            this.Controls.Add(this.AccountsLayout);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnRemove);
            this.Name = "AccountsControl";
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