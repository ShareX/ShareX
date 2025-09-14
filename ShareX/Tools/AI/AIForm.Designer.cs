namespace ShareX
{
    partial class AIForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AIForm));
            lblInput = new System.Windows.Forms.Label();
            lblImage = new System.Windows.Forms.Label();
            txtImage = new System.Windows.Forms.TextBox();
            btnAnalyze = new System.Windows.Forms.Button();
            lblResult = new System.Windows.Forms.Label();
            txtResult = new System.Windows.Forms.TextBox();
            btnImageBrowse = new System.Windows.Forms.Button();
            pbImage = new ShareX.HelpersLib.MyPictureBox();
            cbInput = new System.Windows.Forms.ComboBox();
            lblTimer = new System.Windows.Forms.Label();
            btnCapture = new System.Windows.Forms.Button();
            btnResultCopy = new System.Windows.Forms.Button();
            btnOptions = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // lblInput
            // 
            resources.ApplyResources(lblInput, "lblInput");
            lblInput.Name = "lblInput";
            // 
            // lblImage
            // 
            resources.ApplyResources(lblImage, "lblImage");
            lblImage.Name = "lblImage";
            // 
            // txtImage
            // 
            resources.ApplyResources(txtImage, "txtImage");
            txtImage.Name = "txtImage";
            txtImage.TextChanged += txtImage_TextChanged;
            // 
            // btnAnalyze
            // 
            resources.ApplyResources(btnAnalyze, "btnAnalyze");
            btnAnalyze.Name = "btnAnalyze";
            btnAnalyze.UseVisualStyleBackColor = true;
            btnAnalyze.Click += btnAnalyze_Click;
            // 
            // lblResult
            // 
            resources.ApplyResources(lblResult, "lblResult");
            lblResult.Name = "lblResult";
            // 
            // txtResult
            // 
            resources.ApplyResources(txtResult, "txtResult");
            txtResult.Name = "txtResult";
            txtResult.ReadOnly = true;
            // 
            // btnImageBrowse
            // 
            btnImageBrowse.Image = Properties.Resources.folder_open_image;
            resources.ApplyResources(btnImageBrowse, "btnImageBrowse");
            btnImageBrowse.Name = "btnImageBrowse";
            btnImageBrowse.UseVisualStyleBackColor = true;
            btnImageBrowse.Click += btnImageBrowse_Click;
            // 
            // pbImage
            // 
            pbImage.BackColor = System.Drawing.SystemColors.Window;
            pbImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pbImage.DrawCheckeredBackground = true;
            pbImage.FullscreenOnClick = true;
            resources.ApplyResources(pbImage, "pbImage");
            pbImage.Name = "pbImage";
            pbImage.PictureBoxBackColor = System.Drawing.SystemColors.Window;
            // 
            // cbInput
            // 
            cbInput.FormattingEnabled = true;
            cbInput.Items.AddRange(new object[] { resources.GetString("cbInput.Items"), resources.GetString("cbInput.Items1"), resources.GetString("cbInput.Items2"), resources.GetString("cbInput.Items3") });
            resources.ApplyResources(cbInput, "cbInput");
            cbInput.Name = "cbInput";
            cbInput.TextChanged += cbInput_TextChanged;
            // 
            // lblTimer
            // 
            resources.ApplyResources(lblTimer, "lblTimer");
            lblTimer.Name = "lblTimer";
            // 
            // btnCapture
            // 
            btnCapture.Image = Properties.Resources.camera;
            resources.ApplyResources(btnCapture, "btnCapture");
            btnCapture.Name = "btnCapture";
            btnCapture.UseVisualStyleBackColor = true;
            btnCapture.Click += btnCapture_Click;
            // 
            // btnResultCopy
            // 
            resources.ApplyResources(btnResultCopy, "btnResultCopy");
            btnResultCopy.Image = Properties.Resources.document_copy;
            btnResultCopy.Name = "btnResultCopy";
            btnResultCopy.UseVisualStyleBackColor = true;
            btnResultCopy.Click += btnResultCopy_Click;
            // 
            // btnOptions
            // 
            resources.ApplyResources(btnOptions, "btnOptions");
            btnOptions.Name = "btnOptions";
            btnOptions.UseVisualStyleBackColor = true;
            btnOptions.Click += btnOptions_Click;
            // 
            // AIForm
            // 
            AllowDrop = true;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(btnOptions);
            Controls.Add(btnResultCopy);
            Controls.Add(btnCapture);
            Controls.Add(txtResult);
            Controls.Add(lblTimer);
            Controls.Add(cbInput);
            Controls.Add(pbImage);
            Controls.Add(btnImageBrowse);
            Controls.Add(lblResult);
            Controls.Add(btnAnalyze);
            Controls.Add(txtImage);
            Controls.Add(lblImage);
            Controls.Add(lblInput);
            Name = "AIForm";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Load += AIForm_Load;
            Shown += AIForm_Shown;
            DragDrop += AIForm_DragDrop;
            DragEnter += AIForm_DragEnter;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.Label lblImage;
        private System.Windows.Forms.TextBox txtImage;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Button btnImageBrowse;
        private HelpersLib.MyPictureBox pbImage;
        private System.Windows.Forms.ComboBox cbInput;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Button btnCapture;
        private System.Windows.Forms.Button btnResultCopy;
        private System.Windows.Forms.Button btnOptions;
    }
}