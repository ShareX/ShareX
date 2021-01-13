
namespace ShareX
{
    partial class InspectWindowForm
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
            this.rtbInfo = new System.Windows.Forms.RichTextBox();
            this.pInfo = new System.Windows.Forms.Panel();
            this.pInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbInfo
            // 
            this.rtbInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbInfo.DetectUrls = false;
            this.rtbInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbInfo.Location = new System.Drawing.Point(2, 2);
            this.rtbInfo.Name = "rtbInfo";
            this.rtbInfo.Size = new System.Drawing.Size(666, 514);
            this.rtbInfo.TabIndex = 0;
            this.rtbInfo.Text = "";
            // 
            // pInfo
            // 
            this.pInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pInfo.Controls.Add(this.rtbInfo);
            this.pInfo.Location = new System.Drawing.Point(8, 8);
            this.pInfo.Name = "pInfo";
            this.pInfo.Padding = new System.Windows.Forms.Padding(2);
            this.pInfo.Size = new System.Drawing.Size(672, 520);
            this.pInfo.TabIndex = 1;
            // 
            // InspectWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(689, 536);
            this.Controls.Add(this.pInfo);
            this.Name = "InspectWindowForm";
            this.Text = "ShareX - Inspect window";
            this.pInfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbInfo;
        private System.Windows.Forms.Panel pInfo;
    }
}