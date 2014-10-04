namespace HistoryLib
{
    partial class HistoryItemInfoForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryItemInfoForm));
            this.olvMain = new HistoryLib.CustomControls.ObjectListView();
            this.SuspendLayout();
            // 
            // olvMain
            // 
            resources.ApplyResources(this.olvMain, "olvMain");
            this.olvMain.FullRowSelect = true;
            this.olvMain.GridLines = true;
            this.olvMain.HideSelection = false;
            this.olvMain.MultiSelect = false;
            this.olvMain.Name = "olvMain";
            this.olvMain.SetObjectType = HistoryLib.CustomControls.ObjectListView.ObjectType.Properties;
            this.olvMain.UseCompatibleStateImageBehavior = false;
            this.olvMain.View = System.Windows.Forms.View.Details;
            // 
            // HistoryItemInfoForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.olvMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.Name = "HistoryItemInfoForm";
            this.ResumeLayout(false);

        }

        #endregion Windows Form Designer generated code

        private CustomControls.ObjectListView olvMain;
    }
}