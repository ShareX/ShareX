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
            this.olvMain = new HistoryLib.CustomControls.ObjectListView();
            this.SuspendLayout();
            //
            // olvMain
            //
            this.olvMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvMain.FullRowSelect = true;
            this.olvMain.GridLines = true;
            this.olvMain.HideSelection = false;
            this.olvMain.Location = new System.Drawing.Point(0, 0);
            this.olvMain.MultiSelect = false;
            this.olvMain.Name = "olvMain";
            this.olvMain.SetObjectType = HistoryLib.CustomControls.ObjectListView.ObjectType.Properties;
            this.olvMain.Size = new System.Drawing.Size(454, 411);
            this.olvMain.TabIndex = 0;
            this.olvMain.UseCompatibleStateImageBehavior = false;
            this.olvMain.View = System.Windows.Forms.View.Details;
            //
            // HistoryItemInfoForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 411);
            this.Controls.Add(this.olvMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.Name = "HistoryItemInfoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - History item info";
            this.ResumeLayout(false);
        }

        #endregion Windows Form Designer generated code

        private CustomControls.ObjectListView olvMain;
    }
}