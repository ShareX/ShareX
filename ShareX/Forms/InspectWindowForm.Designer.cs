
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
            this.btnInspectWindow = new System.Windows.Forms.Button();
            this.btnInspectControl = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.pInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbInfo
            // 
            this.rtbInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbInfo.DetectUrls = false;
            this.rtbInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbInfo.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbInfo.Location = new System.Drawing.Point(3, 3);
            this.rtbInfo.Name = "rtbInfo";
            this.rtbInfo.ReadOnly = true;
            this.rtbInfo.Size = new System.Drawing.Size(664, 480);
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
            this.pInfo.Location = new System.Drawing.Point(8, 40);
            this.pInfo.Name = "pInfo";
            this.pInfo.Padding = new System.Windows.Forms.Padding(3);
            this.pInfo.Size = new System.Drawing.Size(672, 488);
            this.pInfo.TabIndex = 3;
            // 
            // btnInspectWindow
            // 
            this.btnInspectWindow.Location = new System.Drawing.Point(8, 8);
            this.btnInspectWindow.Name = "btnInspectWindow";
            this.btnInspectWindow.Size = new System.Drawing.Size(144, 24);
            this.btnInspectWindow.TabIndex = 0;
            this.btnInspectWindow.Text = "Inspect window...";
            this.btnInspectWindow.UseVisualStyleBackColor = true;
            this.btnInspectWindow.Click += new System.EventHandler(this.btnInspectWindow_Click);
            // 
            // btnInspectControl
            // 
            this.btnInspectControl.Location = new System.Drawing.Point(160, 8);
            this.btnInspectControl.Name = "btnInspectControl";
            this.btnInspectControl.Size = new System.Drawing.Size(144, 24);
            this.btnInspectControl.TabIndex = 1;
            this.btnInspectControl.Text = "Inspect control...";
            this.btnInspectControl.UseVisualStyleBackColor = true;
            this.btnInspectControl.Click += new System.EventHandler(this.btnInspectControl_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(312, 8);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(144, 24);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // InspectWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(689, 536);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnInspectControl);
            this.Controls.Add(this.btnInspectWindow);
            this.Controls.Add(this.pInfo);
            this.Name = "InspectWindowForm";
            this.Text = "ShareX - Inspect window";
            this.pInfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbInfo;
        private System.Windows.Forms.Panel pInfo;
        private System.Windows.Forms.Button btnInspectWindow;
        private System.Windows.Forms.Button btnInspectControl;
        private System.Windows.Forms.Button btnRefresh;
    }
}