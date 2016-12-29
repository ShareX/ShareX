namespace ShareX
{
    partial class SimpleActionsEditForm
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
            this.components = new System.ComponentModel.Container();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new ShareX.HelpersLib.MenuButton();
            this.cmsAction = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lvActions = new ShareX.HelpersLib.MyListView();
            this.chAction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ilMain = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(128, 8);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(112, 23);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(8, 8);
            this.btnAdd.Menu = this.cmsAction;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(115, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // cmsAction
            // 
            this.cmsAction.Name = "cmsAction";
            this.cmsAction.Size = new System.Drawing.Size(61, 4);
            // 
            // lvActions
            // 
            this.lvActions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvActions.AutoFillColumn = true;
            this.lvActions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chAction});
            this.lvActions.FullRowSelect = true;
            this.lvActions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvActions.HideSelection = false;
            this.lvActions.Location = new System.Drawing.Point(8, 40);
            this.lvActions.MultiSelect = false;
            this.lvActions.Name = "lvActions";
            this.lvActions.Size = new System.Drawing.Size(448, 368);
            this.lvActions.SmallImageList = this.ilMain;
            this.lvActions.TabIndex = 3;
            this.lvActions.UseCompatibleStateImageBehavior = false;
            this.lvActions.View = System.Windows.Forms.View.Details;
            // 
            // chAction
            // 
            this.chAction.Width = 435;
            // 
            // ilMain
            // 
            this.ilMain.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilMain.ImageSize = new System.Drawing.Size(16, 16);
            this.ilMain.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // SimpleActionsEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(465, 417);
            this.Controls.Add(this.lvActions);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnRemove);
            this.Name = "SimpleActionsEditForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Simple actions edit";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnRemove;
        private HelpersLib.MenuButton btnAdd;
        private System.Windows.Forms.ContextMenuStrip cmsAction;
        private HelpersLib.MyListView lvActions;
        private System.Windows.Forms.ColumnHeader chAction;
        private System.Windows.Forms.ImageList ilMain;
    }
}