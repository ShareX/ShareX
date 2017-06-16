namespace ShareX
{
    partial class ActionsToolbarEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActionsToolbarEditForm));
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
            resources.ApplyResources(this.btnRemove, "btnRemove");
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.Menu = this.cmsAction;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // cmsAction
            // 
            this.cmsAction.Name = "cmsAction";
            resources.ApplyResources(this.cmsAction, "cmsAction");
            // 
            // lvActions
            // 
            this.lvActions.AllowDrop = true;
            this.lvActions.AllowItemDrag = true;
            resources.ApplyResources(this.lvActions, "lvActions");
            this.lvActions.AutoFillColumn = true;
            this.lvActions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chAction});
            this.lvActions.FullRowSelect = true;
            this.lvActions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvActions.HideSelection = false;
            this.lvActions.MultiSelect = false;
            this.lvActions.Name = "lvActions";
            this.lvActions.SmallImageList = this.ilMain;
            this.lvActions.UseCompatibleStateImageBehavior = false;
            this.lvActions.View = System.Windows.Forms.View.Details;
            this.lvActions.ItemMoved += new ShareX.HelpersLib.MyListView.ListViewItemMovedEventHandler(this.lvActions_ItemMoved);
            // 
            // chAction
            // 
            resources.ApplyResources(this.chAction, "chAction");
            // 
            // ilMain
            // 
            this.ilMain.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            resources.ApplyResources(this.ilMain, "ilMain");
            this.ilMain.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ActionsToolbarEditForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.lvActions);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnRemove);
            this.Name = "ActionsToolbarEditForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
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