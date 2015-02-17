namespace ShareX.HelpersLib
{
    partial class AutomateForm
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
            this.btnRun = new System.Windows.Forms.Button();
            this.rtbInput = new System.Windows.Forms.RichTextBox();
            this.pInput = new System.Windows.Forms.Panel();
            this.cbFunctions = new System.Windows.Forms.ComboBox();
            this.lblFunctions = new System.Windows.Forms.Label();
            this.btnLoadExample = new System.Windows.Forms.Button();
            this.txtScriptName = new System.Windows.Forms.TextBox();
            this.btnSaveScript = new System.Windows.Forms.Button();
            this.lblScriptName = new System.Windows.Forms.Label();
            this.btnRemoveScript = new System.Windows.Forms.Button();
            this.btnAddMouseMove = new System.Windows.Forms.Button();
            this.lblLineDelay = new System.Windows.Forms.Label();
            this.nudLineDelay = new System.Windows.Forms.NumericUpDown();
            this.lblLineDelayMiliseconds = new System.Windows.Forms.Label();
            this.lvScripts = new ShareX.HelpersLib.MyListView();
            this.chScriptName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLineDelay)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRun
            // 
            this.btnRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnRun.Location = new System.Drawing.Point(9, 9);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(120, 63);
            this.btnRun.TabIndex = 1;
            this.btnRun.Text = "Start";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // rtbInput
            // 
            this.rtbInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbInput.Font = new System.Drawing.Font("Verdana", 12F);
            this.rtbInput.Location = new System.Drawing.Point(3, 3);
            this.rtbInput.Name = "rtbInput";
            this.rtbInput.Size = new System.Drawing.Size(496, 392);
            this.rtbInput.TabIndex = 0;
            this.rtbInput.Text = "";
            this.rtbInput.TextChanged += new System.EventHandler(this.rtbInput_TextChanged);
            // 
            // pInput
            // 
            this.pInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pInput.BackColor = System.Drawing.Color.White;
            this.pInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pInput.Controls.Add(this.rtbInput);
            this.pInput.Location = new System.Drawing.Point(8, 80);
            this.pInput.Name = "pInput";
            this.pInput.Padding = new System.Windows.Forms.Padding(3);
            this.pInput.Size = new System.Drawing.Size(504, 400);
            this.pInput.TabIndex = 0;
            // 
            // cbFunctions
            // 
            this.cbFunctions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFunctions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFunctions.FormattingEnabled = true;
            this.cbFunctions.Location = new System.Drawing.Point(344, 24);
            this.cbFunctions.Name = "cbFunctions";
            this.cbFunctions.Size = new System.Drawing.Size(168, 21);
            this.cbFunctions.TabIndex = 4;
            // 
            // lblFunctions
            // 
            this.lblFunctions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFunctions.AutoSize = true;
            this.lblFunctions.Location = new System.Drawing.Point(341, 8);
            this.lblFunctions.Name = "lblFunctions";
            this.lblFunctions.Size = new System.Drawing.Size(56, 13);
            this.lblFunctions.TabIndex = 3;
            this.lblFunctions.Text = "Functions:";
            // 
            // btnLoadExample
            // 
            this.btnLoadExample.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadExample.Location = new System.Drawing.Point(520, 96);
            this.btnLoadExample.Name = "btnLoadExample";
            this.btnLoadExample.Size = new System.Drawing.Size(152, 24);
            this.btnLoadExample.TabIndex = 2;
            this.btnLoadExample.Text = "Load example";
            this.btnLoadExample.UseVisualStyleBackColor = true;
            this.btnLoadExample.Click += new System.EventHandler(this.btnLoadExample_Click);
            // 
            // txtScriptName
            // 
            this.txtScriptName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScriptName.Location = new System.Drawing.Point(520, 24);
            this.txtScriptName.Name = "txtScriptName";
            this.txtScriptName.Size = new System.Drawing.Size(152, 20);
            this.txtScriptName.TabIndex = 6;
            // 
            // btnSaveScript
            // 
            this.btnSaveScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveScript.Location = new System.Drawing.Point(520, 48);
            this.btnSaveScript.Name = "btnSaveScript";
            this.btnSaveScript.Size = new System.Drawing.Size(152, 24);
            this.btnSaveScript.TabIndex = 7;
            this.btnSaveScript.Text = "Save";
            this.btnSaveScript.UseVisualStyleBackColor = true;
            this.btnSaveScript.Click += new System.EventHandler(this.btnSaveScript_Click);
            // 
            // lblScriptName
            // 
            this.lblScriptName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblScriptName.AutoSize = true;
            this.lblScriptName.Location = new System.Drawing.Point(517, 8);
            this.lblScriptName.Name = "lblScriptName";
            this.lblScriptName.Size = new System.Drawing.Size(66, 13);
            this.lblScriptName.TabIndex = 8;
            this.lblScriptName.Text = "Script name:";
            // 
            // btnRemoveScript
            // 
            this.btnRemoveScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveScript.Location = new System.Drawing.Point(520, 72);
            this.btnRemoveScript.Name = "btnRemoveScript";
            this.btnRemoveScript.Size = new System.Drawing.Size(152, 24);
            this.btnRemoveScript.TabIndex = 9;
            this.btnRemoveScript.Text = "Remove";
            this.btnRemoveScript.UseVisualStyleBackColor = true;
            this.btnRemoveScript.Click += new System.EventHandler(this.btnRemoveScript_Click);
            // 
            // btnAddMouseMove
            // 
            this.btnAddMouseMove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddMouseMove.Location = new System.Drawing.Point(344, 48);
            this.btnAddMouseMove.Name = "btnAddMouseMove";
            this.btnAddMouseMove.Size = new System.Drawing.Size(168, 23);
            this.btnAddMouseMove.TabIndex = 10;
            this.btnAddMouseMove.Text = "Add mouse move";
            this.btnAddMouseMove.UseVisualStyleBackColor = true;
            this.btnAddMouseMove.Click += new System.EventHandler(this.btnAddMouseMove_Click);
            // 
            // lblLineDelay
            // 
            this.lblLineDelay.AutoSize = true;
            this.lblLineDelay.Location = new System.Drawing.Point(133, 8);
            this.lblLineDelay.Name = "lblLineDelay";
            this.lblLineDelay.Size = new System.Drawing.Size(58, 13);
            this.lblLineDelay.TabIndex = 11;
            this.lblLineDelay.Text = "Line delay:";
            // 
            // nudLineDelay
            // 
            this.nudLineDelay.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudLineDelay.Location = new System.Drawing.Point(137, 25);
            this.nudLineDelay.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudLineDelay.Name = "nudLineDelay";
            this.nudLineDelay.Size = new System.Drawing.Size(87, 20);
            this.nudLineDelay.TabIndex = 12;
            this.nudLineDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudLineDelay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblLineDelayMiliseconds
            // 
            this.lblLineDelayMiliseconds.AutoSize = true;
            this.lblLineDelayMiliseconds.Location = new System.Drawing.Point(230, 29);
            this.lblLineDelayMiliseconds.Name = "lblLineDelayMiliseconds";
            this.lblLineDelayMiliseconds.Size = new System.Drawing.Size(20, 13);
            this.lblLineDelayMiliseconds.TabIndex = 13;
            this.lblLineDelayMiliseconds.Text = "ms";
            // 
            // lvScripts
            // 
            this.lvScripts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvScripts.AutoFillColumn = true;
            this.lvScripts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chScriptName});
            this.lvScripts.FullRowSelect = true;
            this.lvScripts.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvScripts.HideSelection = false;
            this.lvScripts.Location = new System.Drawing.Point(520, 128);
            this.lvScripts.MultiSelect = false;
            this.lvScripts.Name = "lvScripts";
            this.lvScripts.Size = new System.Drawing.Size(152, 352);
            this.lvScripts.TabIndex = 5;
            this.lvScripts.UseCompatibleStateImageBehavior = false;
            this.lvScripts.View = System.Windows.Forms.View.Details;
            this.lvScripts.SelectedIndexChanged += new System.EventHandler(this.lvScripts_SelectedIndexChanged);
            // 
            // chScriptName
            // 
            this.chScriptName.Text = "Name";
            this.chScriptName.Width = 130;
            // 
            // AutomateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 489);
            this.Controls.Add(this.lblLineDelayMiliseconds);
            this.Controls.Add(this.nudLineDelay);
            this.Controls.Add(this.lblLineDelay);
            this.Controls.Add(this.btnAddMouseMove);
            this.Controls.Add(this.btnRemoveScript);
            this.Controls.Add(this.lblScriptName);
            this.Controls.Add(this.btnSaveScript);
            this.Controls.Add(this.txtScriptName);
            this.Controls.Add(this.lvScripts);
            this.Controls.Add(this.btnLoadExample);
            this.Controls.Add(this.lblFunctions);
            this.Controls.Add(this.cbFunctions);
            this.Controls.Add(this.pInput);
            this.Controls.Add(this.btnRun);
            this.Name = "AutomateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - Automate";
            this.pInput.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudLineDelay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.RichTextBox rtbInput;
        private System.Windows.Forms.Panel pInput;
        private System.Windows.Forms.ComboBox cbFunctions;
        private System.Windows.Forms.Label lblFunctions;
        private System.Windows.Forms.Button btnLoadExample;
        private ShareX.HelpersLib.MyListView lvScripts;
        private System.Windows.Forms.TextBox txtScriptName;
        private System.Windows.Forms.Button btnSaveScript;
        private System.Windows.Forms.Label lblScriptName;
        private System.Windows.Forms.Button btnRemoveScript;
        private System.Windows.Forms.ColumnHeader chScriptName;
        private System.Windows.Forms.Button btnAddMouseMove;
        private System.Windows.Forms.Label lblLineDelay;
        private System.Windows.Forms.NumericUpDown nudLineDelay;
        private System.Windows.Forms.Label lblLineDelayMiliseconds;



    }
}

