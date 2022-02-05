namespace ShareX
{
    partial class TimedRecordingForm
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
            this.lengthTxt = new System.Windows.Forms.TextBox();
            this.lblRecordingLength = new System.Windows.Forms.Label();
            this.startRecordingBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.secondsRadioBtn = new System.Windows.Forms.RadioButton();
            this.minutesRadioBtn = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // lengthTxt
            // 
            this.lengthTxt.Location = new System.Drawing.Point(12, 28);
            this.lengthTxt.Name = "lengthTxt";
            this.lengthTxt.Size = new System.Drawing.Size(203, 20);
            this.lengthTxt.TabIndex = 5;
            // 
            // lblRecordingLength
            // 
            this.lblRecordingLength.AutoSize = true;
            this.lblRecordingLength.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblRecordingLength.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblRecordingLength.Location = new System.Drawing.Point(12, 12);
            this.lblRecordingLength.Name = "lblRecordingLength";
            this.lblRecordingLength.Size = new System.Drawing.Size(92, 13);
            this.lblRecordingLength.TabIndex = 6;
            this.lblRecordingLength.Text = "Recording Length";
            // 
            // startRecordingBtn
            // 
            this.startRecordingBtn.Location = new System.Drawing.Point(12, 101);
            this.startRecordingBtn.Name = "startRecordingBtn";
            this.startRecordingBtn.Size = new System.Drawing.Size(122, 23);
            this.startRecordingBtn.TabIndex = 7;
            this.startRecordingBtn.Text = "Start Recording";
            this.startRecordingBtn.UseVisualStyleBackColor = true;
            this.startRecordingBtn.Click += new System.EventHandler(this.startRecordingBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(140, 101);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 8;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // secondsRadioBtn
            // 
            this.secondsRadioBtn.AutoSize = true;
            this.secondsRadioBtn.Checked = true;
            this.secondsRadioBtn.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.secondsRadioBtn.Location = new System.Drawing.Point(80, 54);
            this.secondsRadioBtn.Name = "secondsRadioBtn";
            this.secondsRadioBtn.Size = new System.Drawing.Size(67, 17);
            this.secondsRadioBtn.TabIndex = 11;
            this.secondsRadioBtn.TabStop = true;
            this.secondsRadioBtn.Text = "Seconds";
            this.secondsRadioBtn.UseVisualStyleBackColor = true;
            // 
            // minutesRadioBtn
            // 
            this.minutesRadioBtn.AutoSize = true;
            this.minutesRadioBtn.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.minutesRadioBtn.Location = new System.Drawing.Point(153, 54);
            this.minutesRadioBtn.Name = "minutesRadioBtn";
            this.minutesRadioBtn.Size = new System.Drawing.Size(62, 17);
            this.minutesRadioBtn.TabIndex = 12;
            this.minutesRadioBtn.TabStop = true;
            this.minutesRadioBtn.Text = "Minutes";
            this.minutesRadioBtn.UseVisualStyleBackColor = true;
            // 
            // TimedRecordingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(47)))), ((int)(((byte)(56)))));
            this.ClientSize = new System.Drawing.Size(250, 136);
            this.Controls.Add(this.minutesRadioBtn);
            this.Controls.Add(this.secondsRadioBtn);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.startRecordingBtn);
            this.Controls.Add(this.lblRecordingLength);
            this.Controls.Add(this.lengthTxt);
            this.Name = "TimedRecordingForm";
            this.ShowIcon = false;
            this.Text = "ShareX - Recording Timer";
            this.Load += new System.EventHandler(this.TimedRecordingForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox lengthTxt;
        private System.Windows.Forms.Label lblRecordingLength;
        private System.Windows.Forms.Button startRecordingBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.RadioButton secondsRadioBtn;
        private System.Windows.Forms.RadioButton minutesRadioBtn;
    }
}