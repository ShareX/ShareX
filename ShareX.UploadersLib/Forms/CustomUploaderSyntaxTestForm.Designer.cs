namespace ShareX.UploadersLib
{
    partial class CustomUploaderSyntaxTestForm
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
            this.txtResponseText = new System.Windows.Forms.TextBox();
            this.lblResponseText = new System.Windows.Forms.Label();
            this.lblURLSyntax = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.rtbURLSyntax = new System.Windows.Forms.RichTextBox();
            this.pURLSyntax = new System.Windows.Forms.Panel();
            this.pURLSyntax.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtResponseText
            // 
            this.txtResponseText.Location = new System.Drawing.Point(16, 32);
            this.txtResponseText.Multiline = true;
            this.txtResponseText.Name = "txtResponseText";
            this.txtResponseText.Size = new System.Drawing.Size(568, 168);
            this.txtResponseText.TabIndex = 5;
            // 
            // lblResponseText
            // 
            this.lblResponseText.AutoSize = true;
            this.lblResponseText.Location = new System.Drawing.Point(13, 16);
            this.lblResponseText.Name = "lblResponseText";
            this.lblResponseText.Size = new System.Drawing.Size(78, 13);
            this.lblResponseText.TabIndex = 4;
            this.lblResponseText.Text = "Response text:";
            // 
            // lblURLSyntax
            // 
            this.lblURLSyntax.AutoSize = true;
            this.lblURLSyntax.Location = new System.Drawing.Point(13, 208);
            this.lblURLSyntax.Name = "lblURLSyntax";
            this.lblURLSyntax.Size = new System.Drawing.Size(65, 13);
            this.lblURLSyntax.TabIndex = 0;
            this.lblURLSyntax.Text = "URL syntax:";
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(13, 256);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(40, 13);
            this.lblResult.TabIndex = 2;
            this.lblResult.Text = "Result:";
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(16, 272);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.Size = new System.Drawing.Size(568, 56);
            this.txtResult.TabIndex = 3;
            // 
            // rtbURLSyntax
            // 
            this.rtbURLSyntax.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbURLSyntax.DetectUrls = false;
            this.rtbURLSyntax.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbURLSyntax.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbURLSyntax.Location = new System.Drawing.Point(2, 2);
            this.rtbURLSyntax.Multiline = false;
            this.rtbURLSyntax.Name = "rtbURLSyntax";
            this.rtbURLSyntax.Size = new System.Drawing.Size(562, 14);
            this.rtbURLSyntax.TabIndex = 0;
            this.rtbURLSyntax.Text = "";
            this.rtbURLSyntax.TextChanged += new System.EventHandler(this.rtbURLSyntax_TextChanged);
            // 
            // pURLSyntax
            // 
            this.pURLSyntax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pURLSyntax.Controls.Add(this.rtbURLSyntax);
            this.pURLSyntax.Location = new System.Drawing.Point(16, 224);
            this.pURLSyntax.Name = "pURLSyntax";
            this.pURLSyntax.Padding = new System.Windows.Forms.Padding(2);
            this.pURLSyntax.Size = new System.Drawing.Size(568, 20);
            this.pURLSyntax.TabIndex = 1;
            // 
            // CustomUploaderSyntaxTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 344);
            this.Controls.Add(this.pURLSyntax);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.lblURLSyntax);
            this.Controls.Add(this.lblResponseText);
            this.Controls.Add(this.txtResponseText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "CustomUploaderSyntaxTestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Custom uploader syntax test";
            this.pURLSyntax.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtResponseText;
        private System.Windows.Forms.Label lblResponseText;
        private System.Windows.Forms.Label lblURLSyntax;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.RichTextBox rtbURLSyntax;
        private System.Windows.Forms.Panel pURLSyntax;
    }
}