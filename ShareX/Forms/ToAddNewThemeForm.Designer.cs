
namespace ShareX.Forms
{
    partial class ToAddNewThemeForm
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
            this.pgTheme = new System.Windows.Forms.PropertyGrid();
            this.btnThemeAdd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pgTheme
            // 
            this.pgTheme.HelpVisible = false;
            this.pgTheme.Location = new System.Drawing.Point(9, 12);
            this.pgTheme.Name = "pgTheme";
            this.pgTheme.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgTheme.Size = new System.Drawing.Size(472, 304);
            this.pgTheme.TabIndex = 7;
            this.pgTheme.ToolbarVisible = false;
            this.pgTheme.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgTheme_PropertyValueChanged);
          
            // 
            // btnThemeAdd
            // 
            this.btnThemeAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnThemeAdd.Location = new System.Drawing.Point(198, 322);
            this.btnThemeAdd.Name = "btnThemeAdd";
            this.btnThemeAdd.Size = new System.Drawing.Size(88, 24);
            this.btnThemeAdd.TabIndex = 8;
            this.btnThemeAdd.Text = "Add";
            this.btnThemeAdd.UseVisualStyleBackColor = true;
            this.btnThemeAdd.Click += new System.EventHandler(this.btnThemeAdd_Click);
            // 
            // ToAddNewThemeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 350);
            this.Controls.Add(this.btnThemeAdd);
            this.Controls.Add(this.pgTheme);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ToAddNewThemeForm";
            this.Text = "Add New Theme";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ToAddNewThemeForm_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid pgTheme;
        private System.Windows.Forms.Button btnThemeAdd;
    }
}