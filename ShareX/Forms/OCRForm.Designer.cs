namespace ShareX
{
    partial class OCRForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OCRForm));
            this.lblLanguage = new System.Windows.Forms.Label();
            this.cbLanguages = new System.Windows.Forms.ComboBox();
            this.lblResult = new System.Windows.Forms.Label();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.lblScaleFactor = new System.Windows.Forms.Label();
            this.nudScaleFactor = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudScaleFactor)).BeginInit();
            this.SuspendLayout();
            // 
            // lblLanguage
            // 
            resources.ApplyResources(this.lblLanguage, "lblLanguage");
            this.lblLanguage.Name = "lblLanguage";
            // 
            // cbLanguages
            // 
            this.cbLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cbLanguages, "cbLanguages");
            this.cbLanguages.FormattingEnabled = true;
            this.cbLanguages.Name = "cbLanguages";
            this.cbLanguages.SelectedIndexChanged += new System.EventHandler(this.cbLanguages_SelectedIndexChanged);
            // 
            // lblResult
            // 
            resources.ApplyResources(this.lblResult, "lblResult");
            this.lblResult.Name = "lblResult";
            // 
            // txtResult
            // 
            resources.ApplyResources(this.txtResult, "txtResult");
            this.txtResult.Name = "txtResult";
            // 
            // lblScaleFactor
            // 
            resources.ApplyResources(this.lblScaleFactor, "lblScaleFactor");
            this.lblScaleFactor.Name = "lblScaleFactor";
            // 
            // nudScaleFactor
            // 
            this.nudScaleFactor.DecimalPlaces = 1;
            resources.ApplyResources(this.nudScaleFactor, "nudScaleFactor");
            this.nudScaleFactor.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.nudScaleFactor.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudScaleFactor.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudScaleFactor.Name = "nudScaleFactor";
            this.nudScaleFactor.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudScaleFactor.ValueChanged += new System.EventHandler(this.nudScaleFactor_ValueChanged);
            // 
            // OCRForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.nudScaleFactor);
            this.Controls.Add(this.lblScaleFactor);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.cbLanguages);
            this.Controls.Add(this.lblLanguage);
            this.Name = "OCRForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Shown += new System.EventHandler(this.OCRForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.nudScaleFactor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.ComboBox cbLanguages;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Label lblScaleFactor;
        private System.Windows.Forms.NumericUpDown nudScaleFactor;
    }
}