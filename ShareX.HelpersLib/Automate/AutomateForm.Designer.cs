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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutomateForm));
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
            this.lblKeys = new System.Windows.Forms.Label();
            this.cbKeys = new System.Windows.Forms.ComboBox();
            this.lvScripts = new ShareX.HelpersLib.MyListView();
            this.chScriptName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLineDelay)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRun
            // 
            resources.ApplyResources(this.btnRun, "btnRun");
            this.btnRun.Name = "btnRun";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // rtbInput
            // 
            this.rtbInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.rtbInput, "rtbInput");
            this.rtbInput.Name = "rtbInput";
            this.rtbInput.TextChanged += new System.EventHandler(this.rtbInput_TextChanged);
            // 
            // pInput
            // 
            resources.ApplyResources(this.pInput, "pInput");
            this.pInput.BackColor = System.Drawing.Color.White;
            this.pInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pInput.Controls.Add(this.rtbInput);
            this.pInput.Name = "pInput";
            // 
            // cbFunctions
            // 
            resources.ApplyResources(this.cbFunctions, "cbFunctions");
            this.cbFunctions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFunctions.FormattingEnabled = true;
            this.cbFunctions.Name = "cbFunctions";
            this.cbFunctions.SelectionChangeCommitted += new System.EventHandler(this.cbFunctions_SelectionChangeCommitted);
            // 
            // lblFunctions
            // 
            resources.ApplyResources(this.lblFunctions, "lblFunctions");
            this.lblFunctions.Name = "lblFunctions";
            // 
            // btnLoadExample
            // 
            resources.ApplyResources(this.btnLoadExample, "btnLoadExample");
            this.btnLoadExample.Name = "btnLoadExample";
            this.btnLoadExample.UseVisualStyleBackColor = true;
            this.btnLoadExample.Click += new System.EventHandler(this.btnLoadExample_Click);
            // 
            // txtScriptName
            // 
            resources.ApplyResources(this.txtScriptName, "txtScriptName");
            this.txtScriptName.Name = "txtScriptName";
            // 
            // btnSaveScript
            // 
            resources.ApplyResources(this.btnSaveScript, "btnSaveScript");
            this.btnSaveScript.Name = "btnSaveScript";
            this.btnSaveScript.UseVisualStyleBackColor = true;
            this.btnSaveScript.Click += new System.EventHandler(this.btnSaveScript_Click);
            // 
            // lblScriptName
            // 
            resources.ApplyResources(this.lblScriptName, "lblScriptName");
            this.lblScriptName.Name = "lblScriptName";
            // 
            // btnRemoveScript
            // 
            resources.ApplyResources(this.btnRemoveScript, "btnRemoveScript");
            this.btnRemoveScript.Name = "btnRemoveScript";
            this.btnRemoveScript.UseVisualStyleBackColor = true;
            this.btnRemoveScript.Click += new System.EventHandler(this.btnRemoveScript_Click);
            // 
            // btnAddMouseMove
            // 
            resources.ApplyResources(this.btnAddMouseMove, "btnAddMouseMove");
            this.btnAddMouseMove.Name = "btnAddMouseMove";
            this.btnAddMouseMove.UseVisualStyleBackColor = true;
            this.btnAddMouseMove.Click += new System.EventHandler(this.btnAddMouseMove_Click);
            // 
            // lblLineDelay
            // 
            resources.ApplyResources(this.lblLineDelay, "lblLineDelay");
            this.lblLineDelay.Name = "lblLineDelay";
            // 
            // nudLineDelay
            // 
            this.nudLineDelay.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nudLineDelay, "nudLineDelay");
            this.nudLineDelay.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudLineDelay.Name = "nudLineDelay";
            this.nudLineDelay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblLineDelayMiliseconds
            // 
            resources.ApplyResources(this.lblLineDelayMiliseconds, "lblLineDelayMiliseconds");
            this.lblLineDelayMiliseconds.Name = "lblLineDelayMiliseconds";
            // 
            // lblKeys
            // 
            resources.ApplyResources(this.lblKeys, "lblKeys");
            this.lblKeys.Name = "lblKeys";
            // 
            // cbKeys
            // 
            resources.ApplyResources(this.cbKeys, "cbKeys");
            this.cbKeys.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbKeys.FormattingEnabled = true;
            this.cbKeys.Name = "cbKeys";
            this.cbKeys.SelectionChangeCommitted += new System.EventHandler(this.cbKeys_SelectionChangeCommitted);
            // 
            // lvScripts
            // 
            resources.ApplyResources(this.lvScripts, "lvScripts");
            this.lvScripts.AutoFillColumn = true;
            this.lvScripts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chScriptName});
            this.lvScripts.FullRowSelect = true;
            this.lvScripts.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvScripts.HideSelection = false;
            this.lvScripts.MultiSelect = false;
            this.lvScripts.Name = "lvScripts";
            this.lvScripts.UseCompatibleStateImageBehavior = false;
            this.lvScripts.View = System.Windows.Forms.View.Details;
            this.lvScripts.SelectedIndexChanged += new System.EventHandler(this.lvScripts_SelectedIndexChanged);
            // 
            // chScriptName
            // 
            resources.ApplyResources(this.chScriptName, "chScriptName");
            // 
            // AutomateForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblKeys);
            this.Controls.Add(this.cbKeys);
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
        private System.Windows.Forms.Label lblKeys;
        private System.Windows.Forms.ComboBox cbKeys;



    }
}

