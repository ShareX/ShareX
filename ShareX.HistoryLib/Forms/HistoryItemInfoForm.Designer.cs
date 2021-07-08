namespace ShareX.HistoryLib
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
            this.pgHistoryItem = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // pgHistoryItem
            // 
            resources.ApplyResources(this.pgHistoryItem, "pgHistoryItem");
            this.pgHistoryItem.Name = "pgHistoryItem";
            this.pgHistoryItem.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgHistoryItem.ToolbarVisible = false;
            // 
            // HistoryItemInfoForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.pgHistoryItem);
            this.Name = "HistoryItemInfoForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.ResumeLayout(false);

        }

        #endregion Windows Form Designer generated code

        private System.Windows.Forms.PropertyGrid pgHistoryItem;
    }
}