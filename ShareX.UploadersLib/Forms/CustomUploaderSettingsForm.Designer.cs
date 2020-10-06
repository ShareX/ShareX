namespace ShareX.UploadersLib
{
    partial class CustomUploaderSettingsForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomUploaderSettingsForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tscResponseText = new System.Windows.Forms.ToolStripContainer();
            this.pResponseText = new System.Windows.Forms.Panel();
            this.rtbResponseText = new System.Windows.Forms.RichTextBox();
            this.tsResponseText = new System.Windows.Forms.ToolStrip();
            this.tsbResponseTextJSONFormat = new System.Windows.Forms.ToolStripButton();
            this.tsbResponseTextXMLFormat = new System.Windows.Forms.ToolStripButton();
            this.tsbResponseTextCopy = new System.Windows.Forms.ToolStripButton();
            this.cbImageUploader = new System.Windows.Forms.ComboBox();
            this.btnTextUploaderTest = new System.Windows.Forms.Button();
            this.lblURLShortener = new System.Windows.Forms.Label();
            this.cbTextUploader = new System.Windows.Forms.ComboBox();
            this.btnURLShortenerTest = new System.Windows.Forms.Button();
            this.lblTextUploader = new System.Windows.Forms.Label();
            this.gbCustomUploaders = new System.Windows.Forms.GroupBox();
            this.mbHelp = new ShareX.HelpersLib.MenuButton();
            this.cmsHelp = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCustomUploaderGuide = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCustomUploaders = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUpdateFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnDuplicate = new System.Windows.Forms.Button();
            this.btnClearUploaders = new System.Windows.Forms.Button();
            this.eiCustomUploaders = new ShareX.HelpersLib.ExportImportControl();
            this.lbCustomUploaderList = new System.Windows.Forms.ListBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.cbURLShortener = new System.Windows.Forms.ComboBox();
            this.cbFileUploader = new System.Windows.Forms.ComboBox();
            this.btnImageUploaderTest = new System.Windows.Forms.Button();
            this.lblFileUploader = new System.Windows.Forms.Label();
            this.btnFileUploaderTest = new System.Windows.Forms.Button();
            this.lblImageUploader = new System.Windows.Forms.Label();
            this.tcCustomUploader = new System.Windows.Forms.TabControl();
            this.tpRequest = new System.Windows.Forms.TabPage();
            this.dgvHeaders = new System.Windows.Forms.DataGridView();
            this.cHeadersName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cHeadersValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvParameters = new System.Windows.Forms.DataGridView();
            this.cParametersName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cParametersValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblHeaders = new System.Windows.Forms.Label();
            this.lblParameters = new System.Windows.Forms.Label();
            this.pRequestURL = new System.Windows.Forms.Panel();
            this.rtbRequestURL = new System.Windows.Forms.RichTextBox();
            this.cbRequestMethod = new System.Windows.Forms.ComboBox();
            this.lblRequestURL = new System.Windows.Forms.Label();
            this.cbBody = new System.Windows.Forms.ComboBox();
            this.lblRequestMethod = new System.Windows.Forms.Label();
            this.lblBody = new System.Windows.Forms.Label();
            this.pBodyArguments = new System.Windows.Forms.Panel();
            this.dgvArguments = new System.Windows.Forms.DataGridView();
            this.cArgumentsName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cArgumentsValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblFileFormName = new System.Windows.Forms.Label();
            this.txtFileFormName = new System.Windows.Forms.TextBox();
            this.pBodyData = new System.Windows.Forms.Panel();
            this.btnDataBeautify = new System.Windows.Forms.Button();
            this.btnDataMinify = new System.Windows.Forms.Button();
            this.pData = new System.Windows.Forms.Panel();
            this.rtbData = new System.Windows.Forms.RichTextBox();
            this.tpResponse = new System.Windows.Forms.TabPage();
            this.pResultErrorMessage = new System.Windows.Forms.Panel();
            this.rtbResultErrorMessage = new System.Windows.Forms.RichTextBox();
            this.lblResultErrorMessage = new System.Windows.Forms.Label();
            this.lblParseResponse = new System.Windows.Forms.Label();
            this.pResultDeletionURL = new System.Windows.Forms.Panel();
            this.rtbResultDeletionURL = new System.Windows.Forms.RichTextBox();
            this.lblResultDeletionURL = new System.Windows.Forms.Label();
            this.pResultThumbnailURL = new System.Windows.Forms.Panel();
            this.rtbResultThumbnailURL = new System.Windows.Forms.RichTextBox();
            this.pResultURL = new System.Windows.Forms.Panel();
            this.rtbResultURL = new System.Windows.Forms.RichTextBox();
            this.lblResultThumbnailURL = new System.Windows.Forms.Label();
            this.lblResultURL = new System.Windows.Forms.Label();
            this.tcResponseParse = new System.Windows.Forms.TabControl();
            this.tpJsonParse = new System.Windows.Forms.TabPage();
            this.btnJsonAddSyntax = new System.Windows.Forms.Button();
            this.btnJsonPathHelp = new System.Windows.Forms.Button();
            this.lblJsonPathExample = new System.Windows.Forms.Label();
            this.lblJsonPath = new System.Windows.Forms.Label();
            this.txtJsonPath = new System.Windows.Forms.TextBox();
            this.tpXmlParse = new System.Windows.Forms.TabPage();
            this.btnXmlAddSyntax = new System.Windows.Forms.Button();
            this.btnXPathHelp = new System.Windows.Forms.Button();
            this.lblXPathExample = new System.Windows.Forms.Label();
            this.lblXPath = new System.Windows.Forms.Label();
            this.txtXPath = new System.Windows.Forms.TextBox();
            this.tpRegexParse = new System.Windows.Forms.TabPage();
            this.lblRegex = new System.Windows.Forms.Label();
            this.dgvRegex = new System.Windows.Forms.DataGridView();
            this.cRegex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnRegexAddSyntax = new System.Windows.Forms.Button();
            this.tpTest = new System.Windows.Forms.TabPage();
            this.tcTest = new System.Windows.Forms.TabControl();
            this.tpResult = new System.Windows.Forms.TabPage();
            this.pResult = new System.Windows.Forms.Panel();
            this.rtbResult = new System.Windows.Forms.RichTextBox();
            this.tpResponseInfo = new System.Windows.Forms.TabPage();
            this.pResponseInfo = new System.Windows.Forms.Panel();
            this.rtbResponseInfo = new System.Windows.Forms.RichTextBox();
            this.tpResponseText = new System.Windows.Forms.TabPage();
            this.lblDestinationType = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.mbDestinationType = new ShareX.HelpersLib.MenuButton();
            this.cmsDestinationType = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblURLSharingService = new System.Windows.Forms.Label();
            this.cbURLSharingService = new System.Windows.Forms.ComboBox();
            this.btnURLSharingServiceTest = new System.Windows.Forms.Button();
            this.ttHelpTip = new System.Windows.Forms.ToolTip(this.components);
            this.tscResponseText.ContentPanel.SuspendLayout();
            this.tscResponseText.TopToolStripPanel.SuspendLayout();
            this.tscResponseText.SuspendLayout();
            this.pResponseText.SuspendLayout();
            this.tsResponseText.SuspendLayout();
            this.gbCustomUploaders.SuspendLayout();
            this.cmsHelp.SuspendLayout();
            this.tcCustomUploader.SuspendLayout();
            this.tpRequest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHeaders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParameters)).BeginInit();
            this.pRequestURL.SuspendLayout();
            this.pBodyArguments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvArguments)).BeginInit();
            this.pBodyData.SuspendLayout();
            this.pData.SuspendLayout();
            this.tpResponse.SuspendLayout();
            this.pResultErrorMessage.SuspendLayout();
            this.pResultDeletionURL.SuspendLayout();
            this.pResultThumbnailURL.SuspendLayout();
            this.pResultURL.SuspendLayout();
            this.tcResponseParse.SuspendLayout();
            this.tpJsonParse.SuspendLayout();
            this.tpXmlParse.SuspendLayout();
            this.tpRegexParse.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRegex)).BeginInit();
            this.tpTest.SuspendLayout();
            this.tcTest.SuspendLayout();
            this.tpResult.SuspendLayout();
            this.pResult.SuspendLayout();
            this.tpResponseInfo.SuspendLayout();
            this.pResponseInfo.SuspendLayout();
            this.tpResponseText.SuspendLayout();
            this.SuspendLayout();
            // 
            // tscResponseText
            // 
            // 
            // tscResponseText.ContentPanel
            // 
            this.tscResponseText.ContentPanel.Controls.Add(this.pResponseText);
            resources.ApplyResources(this.tscResponseText.ContentPanel, "tscResponseText.ContentPanel");
            resources.ApplyResources(this.tscResponseText, "tscResponseText");
            this.tscResponseText.Name = "tscResponseText";
            // 
            // tscResponseText.TopToolStripPanel
            // 
            this.tscResponseText.TopToolStripPanel.Controls.Add(this.tsResponseText);
            this.tscResponseText.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // pResponseText
            // 
            this.pResponseText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pResponseText.Controls.Add(this.rtbResponseText);
            resources.ApplyResources(this.pResponseText, "pResponseText");
            this.pResponseText.Name = "pResponseText";
            // 
            // rtbResponseText
            // 
            this.rtbResponseText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.rtbResponseText, "rtbResponseText");
            this.rtbResponseText.Name = "rtbResponseText";
            this.rtbResponseText.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.txtCustomUploaderLog_LinkClicked);
            // 
            // tsResponseText
            // 
            resources.ApplyResources(this.tsResponseText, "tsResponseText");
            this.tsResponseText.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsResponseText.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbResponseTextJSONFormat,
            this.tsbResponseTextXMLFormat,
            this.tsbResponseTextCopy});
            this.tsResponseText.Name = "tsResponseText";
            this.tsResponseText.ShowItemToolTips = false;
            // 
            // tsbResponseTextJSONFormat
            // 
            this.tsbResponseTextJSONFormat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.tsbResponseTextJSONFormat, "tsbResponseTextJSONFormat");
            this.tsbResponseTextJSONFormat.Name = "tsbResponseTextJSONFormat";
            this.tsbResponseTextJSONFormat.Click += new System.EventHandler(this.tsbCustomUploaderJSONFormat_Click);
            // 
            // tsbResponseTextXMLFormat
            // 
            this.tsbResponseTextXMLFormat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.tsbResponseTextXMLFormat, "tsbResponseTextXMLFormat");
            this.tsbResponseTextXMLFormat.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.tsbResponseTextXMLFormat.Name = "tsbResponseTextXMLFormat";
            this.tsbResponseTextXMLFormat.Click += new System.EventHandler(this.tsbCustomUploaderXMLFormat_Click);
            // 
            // tsbResponseTextCopy
            // 
            this.tsbResponseTextCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.tsbResponseTextCopy, "tsbResponseTextCopy");
            this.tsbResponseTextCopy.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.tsbResponseTextCopy.Name = "tsbResponseTextCopy";
            this.tsbResponseTextCopy.Click += new System.EventHandler(this.tsbCustomUploaderCopyResponseText_Click);
            // 
            // cbImageUploader
            // 
            this.cbImageUploader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbImageUploader.FormattingEnabled = true;
            resources.ApplyResources(this.cbImageUploader, "cbImageUploader");
            this.cbImageUploader.Name = "cbImageUploader";
            this.cbImageUploader.SelectedIndexChanged += new System.EventHandler(this.cbCustomUploaderImageUploader_SelectedIndexChanged);
            // 
            // btnTextUploaderTest
            // 
            resources.ApplyResources(this.btnTextUploaderTest, "btnTextUploaderTest");
            this.btnTextUploaderTest.Name = "btnTextUploaderTest";
            this.btnTextUploaderTest.UseVisualStyleBackColor = true;
            this.btnTextUploaderTest.Click += new System.EventHandler(this.btnCustomUploaderTextUploaderTest_Click);
            // 
            // lblURLShortener
            // 
            resources.ApplyResources(this.lblURLShortener, "lblURLShortener");
            this.lblURLShortener.Name = "lblURLShortener";
            // 
            // cbTextUploader
            // 
            this.cbTextUploader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTextUploader.FormattingEnabled = true;
            resources.ApplyResources(this.cbTextUploader, "cbTextUploader");
            this.cbTextUploader.Name = "cbTextUploader";
            this.cbTextUploader.SelectedIndexChanged += new System.EventHandler(this.cbCustomUploaderTextUploader_SelectedIndexChanged);
            // 
            // btnURLShortenerTest
            // 
            resources.ApplyResources(this.btnURLShortenerTest, "btnURLShortenerTest");
            this.btnURLShortenerTest.Name = "btnURLShortenerTest";
            this.btnURLShortenerTest.UseVisualStyleBackColor = true;
            this.btnURLShortenerTest.Click += new System.EventHandler(this.btnCustomUploaderURLShortenerTest_Click);
            // 
            // lblTextUploader
            // 
            resources.ApplyResources(this.lblTextUploader, "lblTextUploader");
            this.lblTextUploader.Name = "lblTextUploader";
            // 
            // gbCustomUploaders
            // 
            this.gbCustomUploaders.Controls.Add(this.mbHelp);
            this.gbCustomUploaders.Controls.Add(this.btnNew);
            this.gbCustomUploaders.Controls.Add(this.btnDuplicate);
            this.gbCustomUploaders.Controls.Add(this.btnClearUploaders);
            this.gbCustomUploaders.Controls.Add(this.eiCustomUploaders);
            this.gbCustomUploaders.Controls.Add(this.lbCustomUploaderList);
            this.gbCustomUploaders.Controls.Add(this.btnRemove);
            resources.ApplyResources(this.gbCustomUploaders, "gbCustomUploaders");
            this.gbCustomUploaders.Name = "gbCustomUploaders";
            this.gbCustomUploaders.TabStop = false;
            // 
            // mbHelp
            // 
            resources.ApplyResources(this.mbHelp, "mbHelp");
            this.mbHelp.Menu = this.cmsHelp;
            this.mbHelp.Name = "mbHelp";
            this.mbHelp.UseVisualStyleBackColor = true;
            // 
            // cmsHelp
            // 
            this.cmsHelp.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCustomUploaderGuide,
            this.tsmiCustomUploaders,
            this.tsmiExportAll,
            this.tsmiUpdateFolder});
            this.cmsHelp.Name = "cmsCustomUploaderHelp";
            this.cmsHelp.ShowImageMargin = false;
            resources.ApplyResources(this.cmsHelp, "cmsHelp");
            // 
            // tsmiCustomUploaderGuide
            // 
            this.tsmiCustomUploaderGuide.Name = "tsmiCustomUploaderGuide";
            resources.ApplyResources(this.tsmiCustomUploaderGuide, "tsmiCustomUploaderGuide");
            this.tsmiCustomUploaderGuide.Click += new System.EventHandler(this.tsmiCustomUploaderGuide_Click);
            // 
            // tsmiCustomUploaders
            // 
            this.tsmiCustomUploaders.Name = "tsmiCustomUploaders";
            resources.ApplyResources(this.tsmiCustomUploaders, "tsmiCustomUploaders");
            this.tsmiCustomUploaders.Click += new System.EventHandler(this.tsmiCustomUploaderExamples_Click);
            // 
            // tsmiExportAll
            // 
            this.tsmiExportAll.Name = "tsmiExportAll";
            resources.ApplyResources(this.tsmiExportAll, "tsmiExportAll");
            this.tsmiExportAll.Click += new System.EventHandler(this.tsmiCustomUploaderExportAll_Click);
            // 
            // tsmiUpdateFolder
            // 
            this.tsmiUpdateFolder.Name = "tsmiUpdateFolder";
            resources.ApplyResources(this.tsmiUpdateFolder, "tsmiUpdateFolder");
            this.tsmiUpdateFolder.Click += new System.EventHandler(this.tsmiUpdateFolder_Click);
            // 
            // btnNew
            // 
            resources.ApplyResources(this.btnNew, "btnNew");
            this.btnNew.Name = "btnNew";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnCustomUploaderNew_Click);
            // 
            // btnDuplicate
            // 
            resources.ApplyResources(this.btnDuplicate, "btnDuplicate");
            this.btnDuplicate.Name = "btnDuplicate";
            this.btnDuplicate.UseVisualStyleBackColor = true;
            this.btnDuplicate.Click += new System.EventHandler(this.btnCustomUploaderDuplicate_Click);
            // 
            // btnClearUploaders
            // 
            resources.ApplyResources(this.btnClearUploaders, "btnClearUploaders");
            this.btnClearUploaders.Name = "btnClearUploaders";
            this.btnClearUploaders.UseVisualStyleBackColor = true;
            this.btnClearUploaders.Click += new System.EventHandler(this.btnCustomUploaderClearUploaders_Click);
            // 
            // eiCustomUploaders
            // 
            this.eiCustomUploaders.CustomFilter = "ShareX custom uploader (*.sxcu)|*.sxcu";
            this.eiCustomUploaders.DefaultFileName = null;
            this.eiCustomUploaders.ExportIgnoreDefaultValue = true;
            this.eiCustomUploaders.ExportIgnoreNull = true;
            resources.ApplyResources(this.eiCustomUploaders, "eiCustomUploaders");
            this.eiCustomUploaders.Name = "eiCustomUploaders";
            this.eiCustomUploaders.ObjectType = null;
            this.eiCustomUploaders.SerializationBinder = null;
            this.eiCustomUploaders.ExportRequested += new ShareX.HelpersLib.ExportImportControl.ExportEventHandler(this.eiCustomUploaders_ExportRequested);
            this.eiCustomUploaders.ImportRequested += new ShareX.HelpersLib.ExportImportControl.ImportEventHandler(this.eiCustomUploaders_ImportRequested);
            this.eiCustomUploaders.ImportCompleted += new System.Action(this.eiCustomUploaders_ImportCompleted);
            // 
            // lbCustomUploaderList
            // 
            this.lbCustomUploaderList.FormattingEnabled = true;
            resources.ApplyResources(this.lbCustomUploaderList, "lbCustomUploaderList");
            this.lbCustomUploaderList.Name = "lbCustomUploaderList";
            this.lbCustomUploaderList.SelectedIndexChanged += new System.EventHandler(this.lbCustomUploaderList_SelectedIndexChanged);
            // 
            // btnRemove
            // 
            resources.ApplyResources(this.btnRemove, "btnRemove");
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnCustomUploaderRemove_Click);
            // 
            // cbURLShortener
            // 
            this.cbURLShortener.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbURLShortener.FormattingEnabled = true;
            resources.ApplyResources(this.cbURLShortener, "cbURLShortener");
            this.cbURLShortener.Name = "cbURLShortener";
            this.cbURLShortener.SelectedIndexChanged += new System.EventHandler(this.cbCustomUploaderURLShortener_SelectedIndexChanged);
            // 
            // cbFileUploader
            // 
            this.cbFileUploader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFileUploader.FormattingEnabled = true;
            resources.ApplyResources(this.cbFileUploader, "cbFileUploader");
            this.cbFileUploader.Name = "cbFileUploader";
            this.cbFileUploader.SelectedIndexChanged += new System.EventHandler(this.cbCustomUploaderFileUploader_SelectedIndexChanged);
            // 
            // btnImageUploaderTest
            // 
            resources.ApplyResources(this.btnImageUploaderTest, "btnImageUploaderTest");
            this.btnImageUploaderTest.Name = "btnImageUploaderTest";
            this.btnImageUploaderTest.UseVisualStyleBackColor = true;
            this.btnImageUploaderTest.Click += new System.EventHandler(this.btnCustomUploaderImageUploaderTest_Click);
            // 
            // lblFileUploader
            // 
            resources.ApplyResources(this.lblFileUploader, "lblFileUploader");
            this.lblFileUploader.Name = "lblFileUploader";
            // 
            // btnFileUploaderTest
            // 
            resources.ApplyResources(this.btnFileUploaderTest, "btnFileUploaderTest");
            this.btnFileUploaderTest.Name = "btnFileUploaderTest";
            this.btnFileUploaderTest.UseVisualStyleBackColor = true;
            this.btnFileUploaderTest.Click += new System.EventHandler(this.btnCustomUploaderFileUploaderTest_Click);
            // 
            // lblImageUploader
            // 
            resources.ApplyResources(this.lblImageUploader, "lblImageUploader");
            this.lblImageUploader.Name = "lblImageUploader";
            // 
            // tcCustomUploader
            // 
            resources.ApplyResources(this.tcCustomUploader, "tcCustomUploader");
            this.tcCustomUploader.Controls.Add(this.tpRequest);
            this.tcCustomUploader.Controls.Add(this.tpResponse);
            this.tcCustomUploader.Controls.Add(this.tpTest);
            this.tcCustomUploader.Name = "tcCustomUploader";
            this.tcCustomUploader.SelectedIndex = 0;
            // 
            // tpRequest
            // 
            this.tpRequest.Controls.Add(this.dgvHeaders);
            this.tpRequest.Controls.Add(this.dgvParameters);
            this.tpRequest.Controls.Add(this.lblHeaders);
            this.tpRequest.Controls.Add(this.lblParameters);
            this.tpRequest.Controls.Add(this.pRequestURL);
            this.tpRequest.Controls.Add(this.cbRequestMethod);
            this.tpRequest.Controls.Add(this.lblRequestURL);
            this.tpRequest.Controls.Add(this.cbBody);
            this.tpRequest.Controls.Add(this.lblRequestMethod);
            this.tpRequest.Controls.Add(this.lblBody);
            this.tpRequest.Controls.Add(this.pBodyArguments);
            this.tpRequest.Controls.Add(this.pBodyData);
            resources.ApplyResources(this.tpRequest, "tpRequest");
            this.tpRequest.Name = "tpRequest";
            this.tpRequest.UseVisualStyleBackColor = true;
            // 
            // dgvHeaders
            // 
            this.dgvHeaders.AllowUserToResizeRows = false;
            this.dgvHeaders.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvHeaders.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle17.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle17.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle17.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle17.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvHeaders.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle17;
            this.dgvHeaders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHeaders.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cHeadersName,
            this.cHeadersValue});
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle18.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle18.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle18.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            dataGridViewCellStyle18.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle18.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvHeaders.DefaultCellStyle = dataGridViewCellStyle18;
            this.dgvHeaders.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvHeaders.GridColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.dgvHeaders, "dgvHeaders");
            this.dgvHeaders.MultiSelect = false;
            this.dgvHeaders.Name = "dgvHeaders";
            this.dgvHeaders.RowHeadersVisible = false;
            this.dgvHeaders.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHeaders_CellValueChanged);
            this.dgvHeaders.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgv_EditingControlShowing);
            // 
            // cHeadersName
            // 
            resources.ApplyResources(this.cHeadersName, "cHeadersName");
            this.cHeadersName.Name = "cHeadersName";
            this.cHeadersName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cHeadersValue
            // 
            this.cHeadersValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.cHeadersValue, "cHeadersValue");
            this.cHeadersValue.Name = "cHeadersValue";
            this.cHeadersValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvParameters
            // 
            this.dgvParameters.AllowUserToResizeRows = false;
            this.dgvParameters.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvParameters.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle19.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle19.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle19.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle19.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle19.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle19.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle19.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvParameters.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle19;
            this.dgvParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvParameters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cParametersName,
            this.cParametersValue});
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle20.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle20.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle20.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            dataGridViewCellStyle20.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle20.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle20.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvParameters.DefaultCellStyle = dataGridViewCellStyle20;
            this.dgvParameters.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvParameters.GridColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.dgvParameters, "dgvParameters");
            this.dgvParameters.MultiSelect = false;
            this.dgvParameters.Name = "dgvParameters";
            this.dgvParameters.RowHeadersVisible = false;
            this.dgvParameters.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvParameters_CellValueChanged);
            this.dgvParameters.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgv_EditingControlShowing);
            // 
            // cParametersName
            // 
            resources.ApplyResources(this.cParametersName, "cParametersName");
            this.cParametersName.Name = "cParametersName";
            this.cParametersName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cParametersValue
            // 
            this.cParametersValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.cParametersValue, "cParametersValue");
            this.cParametersValue.Name = "cParametersValue";
            this.cParametersValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // lblHeaders
            // 
            resources.ApplyResources(this.lblHeaders, "lblHeaders");
            this.lblHeaders.Name = "lblHeaders";
            // 
            // lblParameters
            // 
            resources.ApplyResources(this.lblParameters, "lblParameters");
            this.lblParameters.Name = "lblParameters";
            // 
            // pRequestURL
            // 
            this.pRequestURL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pRequestURL.Controls.Add(this.rtbRequestURL);
            resources.ApplyResources(this.pRequestURL, "pRequestURL");
            this.pRequestURL.Name = "pRequestURL";
            // 
            // rtbRequestURL
            // 
            this.rtbRequestURL.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbRequestURL.DetectUrls = false;
            resources.ApplyResources(this.rtbRequestURL, "rtbRequestURL");
            this.rtbRequestURL.Name = "rtbRequestURL";
            this.rtbRequestURL.TextChanged += new System.EventHandler(this.rtbCustomUploaderRequestURL_TextChanged);
            // 
            // cbRequestMethod
            // 
            this.cbRequestMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRequestMethod.FormattingEnabled = true;
            resources.ApplyResources(this.cbRequestMethod, "cbRequestMethod");
            this.cbRequestMethod.Name = "cbRequestMethod";
            this.cbRequestMethod.SelectedIndexChanged += new System.EventHandler(this.cbCustomUploaderRequestType_SelectedIndexChanged);
            // 
            // lblRequestURL
            // 
            resources.ApplyResources(this.lblRequestURL, "lblRequestURL");
            this.lblRequestURL.Name = "lblRequestURL";
            // 
            // cbBody
            // 
            this.cbBody.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBody.DropDownWidth = 280;
            this.cbBody.FormattingEnabled = true;
            resources.ApplyResources(this.cbBody, "cbBody");
            this.cbBody.Name = "cbBody";
            this.cbBody.SelectedIndexChanged += new System.EventHandler(this.cbCustomUploaderRequestFormat_SelectedIndexChanged);
            // 
            // lblRequestMethod
            // 
            resources.ApplyResources(this.lblRequestMethod, "lblRequestMethod");
            this.lblRequestMethod.Name = "lblRequestMethod";
            // 
            // lblBody
            // 
            resources.ApplyResources(this.lblBody, "lblBody");
            this.lblBody.Name = "lblBody";
            // 
            // pBodyArguments
            // 
            this.pBodyArguments.Controls.Add(this.dgvArguments);
            this.pBodyArguments.Controls.Add(this.lblFileFormName);
            this.pBodyArguments.Controls.Add(this.txtFileFormName);
            resources.ApplyResources(this.pBodyArguments, "pBodyArguments");
            this.pBodyArguments.Name = "pBodyArguments";
            // 
            // dgvArguments
            // 
            this.dgvArguments.AllowUserToResizeRows = false;
            this.dgvArguments.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvArguments.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle21.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle21.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle21.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle21.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle21.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle21.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle21.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvArguments.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle21;
            this.dgvArguments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvArguments.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cArgumentsName,
            this.cArgumentsValue});
            dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle22.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle22.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle22.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            dataGridViewCellStyle22.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle22.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle22.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvArguments.DefaultCellStyle = dataGridViewCellStyle22;
            this.dgvArguments.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvArguments.GridColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.dgvArguments, "dgvArguments");
            this.dgvArguments.MultiSelect = false;
            this.dgvArguments.Name = "dgvArguments";
            this.dgvArguments.RowHeadersVisible = false;
            this.dgvArguments.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvArguments_CellValueChanged);
            this.dgvArguments.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgv_EditingControlShowing);
            // 
            // cArgumentsName
            // 
            resources.ApplyResources(this.cArgumentsName, "cArgumentsName");
            this.cArgumentsName.Name = "cArgumentsName";
            this.cArgumentsName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cArgumentsValue
            // 
            this.cArgumentsValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.cArgumentsValue, "cArgumentsValue");
            this.cArgumentsValue.Name = "cArgumentsValue";
            this.cArgumentsValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // lblFileFormName
            // 
            resources.ApplyResources(this.lblFileFormName, "lblFileFormName");
            this.lblFileFormName.Name = "lblFileFormName";
            // 
            // txtFileFormName
            // 
            resources.ApplyResources(this.txtFileFormName, "txtFileFormName");
            this.txtFileFormName.Name = "txtFileFormName";
            this.txtFileFormName.TextChanged += new System.EventHandler(this.txtCustomUploaderFileForm_TextChanged);
            // 
            // pBodyData
            // 
            this.pBodyData.Controls.Add(this.btnDataBeautify);
            this.pBodyData.Controls.Add(this.btnDataMinify);
            this.pBodyData.Controls.Add(this.pData);
            resources.ApplyResources(this.pBodyData, "pBodyData");
            this.pBodyData.Name = "pBodyData";
            // 
            // btnDataBeautify
            // 
            resources.ApplyResources(this.btnDataBeautify, "btnDataBeautify");
            this.btnDataBeautify.Name = "btnDataBeautify";
            this.btnDataBeautify.UseVisualStyleBackColor = true;
            this.btnDataBeautify.Click += new System.EventHandler(this.btnCustomUploaderDataBeautify_Click);
            // 
            // btnDataMinify
            // 
            resources.ApplyResources(this.btnDataMinify, "btnDataMinify");
            this.btnDataMinify.Name = "btnDataMinify";
            this.btnDataMinify.UseVisualStyleBackColor = true;
            this.btnDataMinify.Click += new System.EventHandler(this.btnCustomUploaderDataMinify_Click);
            // 
            // pData
            // 
            this.pData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pData.Controls.Add(this.rtbData);
            resources.ApplyResources(this.pData, "pData");
            this.pData.Name = "pData";
            // 
            // rtbData
            // 
            this.rtbData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbData.DetectUrls = false;
            resources.ApplyResources(this.rtbData, "rtbData");
            this.rtbData.Name = "rtbData";
            this.rtbData.TextChanged += new System.EventHandler(this.rtbCustomUploaderData_TextChanged);
            // 
            // tpResponse
            // 
            this.tpResponse.Controls.Add(this.pResultErrorMessage);
            this.tpResponse.Controls.Add(this.lblResultErrorMessage);
            this.tpResponse.Controls.Add(this.lblParseResponse);
            this.tpResponse.Controls.Add(this.pResultDeletionURL);
            this.tpResponse.Controls.Add(this.lblResultDeletionURL);
            this.tpResponse.Controls.Add(this.pResultThumbnailURL);
            this.tpResponse.Controls.Add(this.pResultURL);
            this.tpResponse.Controls.Add(this.lblResultThumbnailURL);
            this.tpResponse.Controls.Add(this.lblResultURL);
            this.tpResponse.Controls.Add(this.tcResponseParse);
            resources.ApplyResources(this.tpResponse, "tpResponse");
            this.tpResponse.Name = "tpResponse";
            this.tpResponse.UseVisualStyleBackColor = true;
            // 
            // pResultErrorMessage
            // 
            this.pResultErrorMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pResultErrorMessage.Controls.Add(this.rtbResultErrorMessage);
            resources.ApplyResources(this.pResultErrorMessage, "pResultErrorMessage");
            this.pResultErrorMessage.Name = "pResultErrorMessage";
            // 
            // rtbResultErrorMessage
            // 
            this.rtbResultErrorMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbResultErrorMessage.DetectUrls = false;
            resources.ApplyResources(this.rtbResultErrorMessage, "rtbResultErrorMessage");
            this.rtbResultErrorMessage.Name = "rtbResultErrorMessage";
            this.rtbResultErrorMessage.TextChanged += new System.EventHandler(this.rtbResultErrorMessage_TextChanged);
            this.rtbResultErrorMessage.Enter += new System.EventHandler(this.rtbResultErrorMessage_Enter);
            // 
            // lblResultErrorMessage
            // 
            resources.ApplyResources(this.lblResultErrorMessage, "lblResultErrorMessage");
            this.lblResultErrorMessage.Name = "lblResultErrorMessage";
            // 
            // lblParseResponse
            // 
            resources.ApplyResources(this.lblParseResponse, "lblParseResponse");
            this.lblParseResponse.Name = "lblParseResponse";
            // 
            // pResultDeletionURL
            // 
            this.pResultDeletionURL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pResultDeletionURL.Controls.Add(this.rtbResultDeletionURL);
            resources.ApplyResources(this.pResultDeletionURL, "pResultDeletionURL");
            this.pResultDeletionURL.Name = "pResultDeletionURL";
            // 
            // rtbResultDeletionURL
            // 
            this.rtbResultDeletionURL.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbResultDeletionURL.DetectUrls = false;
            resources.ApplyResources(this.rtbResultDeletionURL, "rtbResultDeletionURL");
            this.rtbResultDeletionURL.Name = "rtbResultDeletionURL";
            this.rtbResultDeletionURL.TextChanged += new System.EventHandler(this.rtbCustomUploaderDeletionURL_TextChanged);
            this.rtbResultDeletionURL.Enter += new System.EventHandler(this.rtbCustomUploaderDeletionURL_Enter);
            // 
            // lblResultDeletionURL
            // 
            resources.ApplyResources(this.lblResultDeletionURL, "lblResultDeletionURL");
            this.lblResultDeletionURL.Name = "lblResultDeletionURL";
            // 
            // pResultThumbnailURL
            // 
            this.pResultThumbnailURL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pResultThumbnailURL.Controls.Add(this.rtbResultThumbnailURL);
            resources.ApplyResources(this.pResultThumbnailURL, "pResultThumbnailURL");
            this.pResultThumbnailURL.Name = "pResultThumbnailURL";
            // 
            // rtbResultThumbnailURL
            // 
            this.rtbResultThumbnailURL.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbResultThumbnailURL.DetectUrls = false;
            resources.ApplyResources(this.rtbResultThumbnailURL, "rtbResultThumbnailURL");
            this.rtbResultThumbnailURL.Name = "rtbResultThumbnailURL";
            this.rtbResultThumbnailURL.TextChanged += new System.EventHandler(this.rtbCustomUploaderThumbnailURL_TextChanged);
            this.rtbResultThumbnailURL.Enter += new System.EventHandler(this.rtbCustomUploaderThumbnailURL_Enter);
            // 
            // pResultURL
            // 
            this.pResultURL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pResultURL.Controls.Add(this.rtbResultURL);
            resources.ApplyResources(this.pResultURL, "pResultURL");
            this.pResultURL.Name = "pResultURL";
            // 
            // rtbResultURL
            // 
            this.rtbResultURL.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbResultURL.DetectUrls = false;
            resources.ApplyResources(this.rtbResultURL, "rtbResultURL");
            this.rtbResultURL.Name = "rtbResultURL";
            this.rtbResultURL.TextChanged += new System.EventHandler(this.rtbCustomUploaderURL_TextChanged);
            this.rtbResultURL.Enter += new System.EventHandler(this.rtbCustomUploaderURL_Enter);
            // 
            // lblResultThumbnailURL
            // 
            resources.ApplyResources(this.lblResultThumbnailURL, "lblResultThumbnailURL");
            this.lblResultThumbnailURL.Name = "lblResultThumbnailURL";
            // 
            // lblResultURL
            // 
            resources.ApplyResources(this.lblResultURL, "lblResultURL");
            this.lblResultURL.Name = "lblResultURL";
            // 
            // tcResponseParse
            // 
            this.tcResponseParse.Controls.Add(this.tpJsonParse);
            this.tcResponseParse.Controls.Add(this.tpXmlParse);
            this.tcResponseParse.Controls.Add(this.tpRegexParse);
            resources.ApplyResources(this.tcResponseParse, "tcResponseParse");
            this.tcResponseParse.Name = "tcResponseParse";
            this.tcResponseParse.SelectedIndex = 0;
            // 
            // tpJsonParse
            // 
            this.tpJsonParse.Controls.Add(this.btnJsonAddSyntax);
            this.tpJsonParse.Controls.Add(this.btnJsonPathHelp);
            this.tpJsonParse.Controls.Add(this.lblJsonPathExample);
            this.tpJsonParse.Controls.Add(this.lblJsonPath);
            this.tpJsonParse.Controls.Add(this.txtJsonPath);
            resources.ApplyResources(this.tpJsonParse, "tpJsonParse");
            this.tpJsonParse.Name = "tpJsonParse";
            this.tpJsonParse.UseVisualStyleBackColor = true;
            // 
            // btnJsonAddSyntax
            // 
            resources.ApplyResources(this.btnJsonAddSyntax, "btnJsonAddSyntax");
            this.btnJsonAddSyntax.Name = "btnJsonAddSyntax";
            this.btnJsonAddSyntax.UseVisualStyleBackColor = true;
            this.btnJsonAddSyntax.Click += new System.EventHandler(this.btnCustomUploaderJsonAddSyntax_Click);
            // 
            // btnJsonPathHelp
            // 
            resources.ApplyResources(this.btnJsonPathHelp, "btnJsonPathHelp");
            this.btnJsonPathHelp.Name = "btnJsonPathHelp";
            this.btnJsonPathHelp.UseVisualStyleBackColor = true;
            this.btnJsonPathHelp.Click += new System.EventHandler(this.btnCustomUploadJsonPathHelp_Click);
            // 
            // lblJsonPathExample
            // 
            resources.ApplyResources(this.lblJsonPathExample, "lblJsonPathExample");
            this.lblJsonPathExample.Name = "lblJsonPathExample";
            // 
            // lblJsonPath
            // 
            resources.ApplyResources(this.lblJsonPath, "lblJsonPath");
            this.lblJsonPath.Name = "lblJsonPath";
            // 
            // txtJsonPath
            // 
            resources.ApplyResources(this.txtJsonPath, "txtJsonPath");
            this.txtJsonPath.Name = "txtJsonPath";
            this.txtJsonPath.TextChanged += new System.EventHandler(this.txtCustomUploaderJsonPath_TextChanged);
            // 
            // tpXmlParse
            // 
            this.tpXmlParse.Controls.Add(this.btnXmlAddSyntax);
            this.tpXmlParse.Controls.Add(this.btnXPathHelp);
            this.tpXmlParse.Controls.Add(this.lblXPathExample);
            this.tpXmlParse.Controls.Add(this.lblXPath);
            this.tpXmlParse.Controls.Add(this.txtXPath);
            resources.ApplyResources(this.tpXmlParse, "tpXmlParse");
            this.tpXmlParse.Name = "tpXmlParse";
            this.tpXmlParse.UseVisualStyleBackColor = true;
            // 
            // btnXmlAddSyntax
            // 
            resources.ApplyResources(this.btnXmlAddSyntax, "btnXmlAddSyntax");
            this.btnXmlAddSyntax.Name = "btnXmlAddSyntax";
            this.btnXmlAddSyntax.UseVisualStyleBackColor = true;
            this.btnXmlAddSyntax.Click += new System.EventHandler(this.btnCustomUploaderXmlSyntaxAdd_Click);
            // 
            // btnXPathHelp
            // 
            resources.ApplyResources(this.btnXPathHelp, "btnXPathHelp");
            this.btnXPathHelp.Name = "btnXPathHelp";
            this.btnXPathHelp.UseVisualStyleBackColor = true;
            this.btnXPathHelp.Click += new System.EventHandler(this.btnCustomUploaderXPathHelp_Click);
            // 
            // lblXPathExample
            // 
            resources.ApplyResources(this.lblXPathExample, "lblXPathExample");
            this.lblXPathExample.Name = "lblXPathExample";
            // 
            // lblXPath
            // 
            resources.ApplyResources(this.lblXPath, "lblXPath");
            this.lblXPath.Name = "lblXPath";
            // 
            // txtXPath
            // 
            resources.ApplyResources(this.txtXPath, "txtXPath");
            this.txtXPath.Name = "txtXPath";
            this.txtXPath.TextChanged += new System.EventHandler(this.txtCustomUploaderXPath_TextChanged);
            // 
            // tpRegexParse
            // 
            this.tpRegexParse.Controls.Add(this.lblRegex);
            this.tpRegexParse.Controls.Add(this.dgvRegex);
            this.tpRegexParse.Controls.Add(this.btnRegexAddSyntax);
            resources.ApplyResources(this.tpRegexParse, "tpRegexParse");
            this.tpRegexParse.Name = "tpRegexParse";
            this.tpRegexParse.UseVisualStyleBackColor = true;
            // 
            // lblRegex
            // 
            resources.ApplyResources(this.lblRegex, "lblRegex");
            this.lblRegex.Name = "lblRegex";
            // 
            // dgvRegex
            // 
            this.dgvRegex.AllowUserToResizeRows = false;
            this.dgvRegex.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvRegex.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle23.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle23.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle23.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle23.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle23.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle23.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle23.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRegex.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle23;
            this.dgvRegex.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRegex.ColumnHeadersVisible = false;
            this.dgvRegex.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cRegex});
            dataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle24.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle24.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle24.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            dataGridViewCellStyle24.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle24.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle24.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvRegex.DefaultCellStyle = dataGridViewCellStyle24;
            this.dgvRegex.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvRegex.GridColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.dgvRegex, "dgvRegex");
            this.dgvRegex.MultiSelect = false;
            this.dgvRegex.Name = "dgvRegex";
            this.dgvRegex.RowHeadersVisible = false;
            this.dgvRegex.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRegex_CellValueChanged);
            this.dgvRegex.SelectionChanged += new System.EventHandler(this.dgvRegex_SelectionChanged);
            // 
            // cRegex
            // 
            this.cRegex.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.cRegex, "cRegex");
            this.cRegex.Name = "cRegex";
            this.cRegex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btnRegexAddSyntax
            // 
            resources.ApplyResources(this.btnRegexAddSyntax, "btnRegexAddSyntax");
            this.btnRegexAddSyntax.Name = "btnRegexAddSyntax";
            this.btnRegexAddSyntax.UseVisualStyleBackColor = true;
            this.btnRegexAddSyntax.Click += new System.EventHandler(this.btnCustomUploaderRegexAddSyntax_Click);
            // 
            // tpTest
            // 
            this.tpTest.Controls.Add(this.tcTest);
            resources.ApplyResources(this.tpTest, "tpTest");
            this.tpTest.Name = "tpTest";
            this.tpTest.UseVisualStyleBackColor = true;
            // 
            // tcTest
            // 
            this.tcTest.Controls.Add(this.tpResult);
            this.tcTest.Controls.Add(this.tpResponseInfo);
            this.tcTest.Controls.Add(this.tpResponseText);
            resources.ApplyResources(this.tcTest, "tcTest");
            this.tcTest.Name = "tcTest";
            this.tcTest.SelectedIndex = 0;
            // 
            // tpResult
            // 
            this.tpResult.Controls.Add(this.pResult);
            resources.ApplyResources(this.tpResult, "tpResult");
            this.tpResult.Name = "tpResult";
            this.tpResult.UseVisualStyleBackColor = true;
            // 
            // pResult
            // 
            this.pResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pResult.Controls.Add(this.rtbResult);
            resources.ApplyResources(this.pResult, "pResult");
            this.pResult.Name = "pResult";
            // 
            // rtbResult
            // 
            this.rtbResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.rtbResult, "rtbResult");
            this.rtbResult.Name = "rtbResult";
            this.rtbResult.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.txtCustomUploaderLog_LinkClicked);
            // 
            // tpResponseInfo
            // 
            this.tpResponseInfo.Controls.Add(this.pResponseInfo);
            resources.ApplyResources(this.tpResponseInfo, "tpResponseInfo");
            this.tpResponseInfo.Name = "tpResponseInfo";
            this.tpResponseInfo.UseVisualStyleBackColor = true;
            // 
            // pResponseInfo
            // 
            this.pResponseInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pResponseInfo.Controls.Add(this.rtbResponseInfo);
            resources.ApplyResources(this.pResponseInfo, "pResponseInfo");
            this.pResponseInfo.Name = "pResponseInfo";
            // 
            // rtbResponseInfo
            // 
            this.rtbResponseInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.rtbResponseInfo, "rtbResponseInfo");
            this.rtbResponseInfo.Name = "rtbResponseInfo";
            this.rtbResponseInfo.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.txtCustomUploaderLog_LinkClicked);
            // 
            // tpResponseText
            // 
            this.tpResponseText.Controls.Add(this.tscResponseText);
            resources.ApplyResources(this.tpResponseText, "tpResponseText");
            this.tpResponseText.Name = "tpResponseText";
            this.tpResponseText.UseVisualStyleBackColor = true;
            // 
            // lblDestinationType
            // 
            resources.ApplyResources(this.lblDestinationType, "lblDestinationType");
            this.lblDestinationType.Name = "lblDestinationType";
            // 
            // lblName
            // 
            resources.ApplyResources(this.lblName, "lblName");
            this.lblName.Name = "lblName";
            // 
            // mbDestinationType
            // 
            resources.ApplyResources(this.mbDestinationType, "mbDestinationType");
            this.mbDestinationType.Menu = this.cmsDestinationType;
            this.mbDestinationType.Name = "mbDestinationType";
            this.ttHelpTip.SetToolTip(this.mbDestinationType, resources.GetString("mbDestinationType.ToolTip"));
            this.mbDestinationType.UseVisualStyleBackColor = true;
            // 
            // cmsDestinationType
            // 
            this.cmsDestinationType.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.cmsDestinationType.Name = "cmsCustomUploaderDestinationType";
            this.cmsDestinationType.ShowCheckMargin = true;
            this.cmsDestinationType.ShowImageMargin = false;
            resources.ApplyResources(this.cmsDestinationType, "cmsDestinationType");
            // 
            // txtName
            // 
            resources.ApplyResources(this.txtName, "txtName");
            this.txtName.Name = "txtName";
            this.txtName.TextChanged += new System.EventHandler(this.txtCustomUploaderName_TextChanged);
            // 
            // lblURLSharingService
            // 
            resources.ApplyResources(this.lblURLSharingService, "lblURLSharingService");
            this.lblURLSharingService.Name = "lblURLSharingService";
            // 
            // cbURLSharingService
            // 
            this.cbURLSharingService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbURLSharingService.FormattingEnabled = true;
            resources.ApplyResources(this.cbURLSharingService, "cbURLSharingService");
            this.cbURLSharingService.Name = "cbURLSharingService";
            this.cbURLSharingService.SelectedIndexChanged += new System.EventHandler(this.cbCustomUploaderURLSharingService_SelectedIndexChanged);
            // 
            // btnURLSharingServiceTest
            // 
            resources.ApplyResources(this.btnURLSharingServiceTest, "btnURLSharingServiceTest");
            this.btnURLSharingServiceTest.Name = "btnURLSharingServiceTest";
            this.btnURLSharingServiceTest.UseVisualStyleBackColor = true;
            this.btnURLSharingServiceTest.Click += new System.EventHandler(this.btnCustomUploaderURLSharingServiceTest_Click);
            // 
            // ttHelpTip
            // 
            this.ttHelpTip.AutomaticDelay = 0;
            this.ttHelpTip.AutoPopDelay = 30000;
            this.ttHelpTip.BackColor = System.Drawing.SystemColors.Window;
            this.ttHelpTip.InitialDelay = 500;
            this.ttHelpTip.IsBalloon = true;
            this.ttHelpTip.ReshowDelay = 100;
            this.ttHelpTip.UseAnimation = false;
            this.ttHelpTip.UseFading = false;
            // 
            // CustomUploaderSettingsForm
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tcCustomUploader);
            this.Controls.Add(this.lblDestinationType);
            this.Controls.Add(this.mbDestinationType);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.btnURLSharingServiceTest);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.cbURLSharingService);
            this.Controls.Add(this.gbCustomUploaders);
            this.Controls.Add(this.lblURLSharingService);
            this.Controls.Add(this.cbImageUploader);
            this.Controls.Add(this.btnTextUploaderTest);
            this.Controls.Add(this.lblImageUploader);
            this.Controls.Add(this.lblURLShortener);
            this.Controls.Add(this.btnFileUploaderTest);
            this.Controls.Add(this.cbTextUploader);
            this.Controls.Add(this.lblFileUploader);
            this.Controls.Add(this.btnURLShortenerTest);
            this.Controls.Add(this.btnImageUploaderTest);
            this.Controls.Add(this.lblTextUploader);
            this.Controls.Add(this.cbFileUploader);
            this.Controls.Add(this.cbURLShortener);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CustomUploaderSettingsForm";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.CustomUploaderSettingsForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.CustomUploaderSettingsForm_DragEnter);
            this.tscResponseText.ContentPanel.ResumeLayout(false);
            this.tscResponseText.TopToolStripPanel.ResumeLayout(false);
            this.tscResponseText.TopToolStripPanel.PerformLayout();
            this.tscResponseText.ResumeLayout(false);
            this.tscResponseText.PerformLayout();
            this.pResponseText.ResumeLayout(false);
            this.tsResponseText.ResumeLayout(false);
            this.tsResponseText.PerformLayout();
            this.gbCustomUploaders.ResumeLayout(false);
            this.cmsHelp.ResumeLayout(false);
            this.tcCustomUploader.ResumeLayout(false);
            this.tpRequest.ResumeLayout(false);
            this.tpRequest.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHeaders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParameters)).EndInit();
            this.pRequestURL.ResumeLayout(false);
            this.pBodyArguments.ResumeLayout(false);
            this.pBodyArguments.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvArguments)).EndInit();
            this.pBodyData.ResumeLayout(false);
            this.pData.ResumeLayout(false);
            this.tpResponse.ResumeLayout(false);
            this.tpResponse.PerformLayout();
            this.pResultErrorMessage.ResumeLayout(false);
            this.pResultDeletionURL.ResumeLayout(false);
            this.pResultThumbnailURL.ResumeLayout(false);
            this.pResultURL.ResumeLayout(false);
            this.tcResponseParse.ResumeLayout(false);
            this.tpJsonParse.ResumeLayout(false);
            this.tpJsonParse.PerformLayout();
            this.tpXmlParse.ResumeLayout(false);
            this.tpXmlParse.PerformLayout();
            this.tpRegexParse.ResumeLayout(false);
            this.tpRegexParse.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRegex)).EndInit();
            this.tpTest.ResumeLayout(false);
            this.tcTest.ResumeLayout(false);
            this.tpResult.ResumeLayout(false);
            this.pResult.ResumeLayout(false);
            this.tpResponseInfo.ResumeLayout(false);
            this.pResponseInfo.ResumeLayout(false);
            this.tpResponseText.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbImageUploader;
        private System.Windows.Forms.Button btnTextUploaderTest;
        private System.Windows.Forms.Label lblURLShortener;
        private System.Windows.Forms.ComboBox cbTextUploader;
        private System.Windows.Forms.Button btnURLShortenerTest;
        private System.Windows.Forms.Label lblTextUploader;
        private System.Windows.Forms.GroupBox gbCustomUploaders;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnDuplicate;
        private System.Windows.Forms.Button btnClearUploaders;
        private HelpersLib.ExportImportControl eiCustomUploaders;
        private System.Windows.Forms.ListBox lbCustomUploaderList;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.ComboBox cbURLShortener;
        private System.Windows.Forms.ComboBox cbFileUploader;
        private System.Windows.Forms.Button btnImageUploaderTest;
        private System.Windows.Forms.Label lblFileUploader;
        private System.Windows.Forms.Button btnFileUploaderTest;
        private System.Windows.Forms.Label lblImageUploader;
        private System.Windows.Forms.TabControl tcCustomUploader;
        private System.Windows.Forms.TabPage tpRequest;
        private System.Windows.Forms.Label lblHeaders;
        private System.Windows.Forms.Label lblParameters;
        private System.Windows.Forms.Panel pRequestURL;
        private System.Windows.Forms.RichTextBox rtbRequestURL;
        private System.Windows.Forms.ComboBox cbRequestMethod;
        private System.Windows.Forms.Label lblRequestURL;
        private System.Windows.Forms.ComboBox cbBody;
        private System.Windows.Forms.Label lblRequestMethod;
        private System.Windows.Forms.Label lblBody;
        private System.Windows.Forms.Panel pBodyArguments;
        private System.Windows.Forms.Label lblFileFormName;
        private System.Windows.Forms.TextBox txtFileFormName;
        private System.Windows.Forms.Panel pBodyData;
        private System.Windows.Forms.Button btnDataBeautify;
        private System.Windows.Forms.Button btnDataMinify;
        private System.Windows.Forms.Panel pData;
        private System.Windows.Forms.RichTextBox rtbData;
        private System.Windows.Forms.TabPage tpResponse;
        private System.Windows.Forms.Label lblParseResponse;
        private System.Windows.Forms.Panel pResultDeletionURL;
        private System.Windows.Forms.RichTextBox rtbResultDeletionURL;
        private System.Windows.Forms.Label lblResultDeletionURL;
        private System.Windows.Forms.Panel pResultThumbnailURL;
        private System.Windows.Forms.RichTextBox rtbResultThumbnailURL;
        private System.Windows.Forms.Panel pResultURL;
        private System.Windows.Forms.RichTextBox rtbResultURL;
        private System.Windows.Forms.Label lblResultThumbnailURL;
        private System.Windows.Forms.Label lblResultURL;
        private System.Windows.Forms.TabControl tcResponseParse;
        private System.Windows.Forms.TabPage tpJsonParse;
        private System.Windows.Forms.Button btnJsonAddSyntax;
        private System.Windows.Forms.Button btnJsonPathHelp;
        private System.Windows.Forms.Label lblJsonPathExample;
        private System.Windows.Forms.Label lblJsonPath;
        private System.Windows.Forms.TextBox txtJsonPath;
        private System.Windows.Forms.TabPage tpXmlParse;
        private System.Windows.Forms.Button btnXmlAddSyntax;
        private System.Windows.Forms.Button btnXPathHelp;
        private System.Windows.Forms.Label lblXPathExample;
        private System.Windows.Forms.Label lblXPath;
        private System.Windows.Forms.TextBox txtXPath;
        private System.Windows.Forms.TabPage tpRegexParse;
        private System.Windows.Forms.Button btnRegexAddSyntax;
        private System.Windows.Forms.TabPage tpTest;
        private System.Windows.Forms.TabControl tcTest;
        private System.Windows.Forms.TabPage tpResult;
        private System.Windows.Forms.Panel pResult;
        private System.Windows.Forms.RichTextBox rtbResult;
        private System.Windows.Forms.TabPage tpResponseText;
        private System.Windows.Forms.ToolStripContainer tscResponseText;
        private System.Windows.Forms.ToolStrip tsResponseText;
        private System.Windows.Forms.ToolStripButton tsbResponseTextJSONFormat;
        private System.Windows.Forms.ToolStripButton tsbResponseTextXMLFormat;
        private System.Windows.Forms.ToolStripButton tsbResponseTextCopy;
        private System.Windows.Forms.Label lblDestinationType;
        private System.Windows.Forms.Label lblName;
        private HelpersLib.MenuButton mbDestinationType;
        private System.Windows.Forms.ContextMenuStrip cmsDestinationType;
        private System.Windows.Forms.ToolTip ttHelpTip;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblURLSharingService;
        private System.Windows.Forms.ComboBox cbURLSharingService;
        private System.Windows.Forms.Button btnURLSharingServiceTest;
        private System.Windows.Forms.ContextMenuStrip cmsHelp;
        private System.Windows.Forms.ToolStripMenuItem tsmiCustomUploaderGuide;
        private System.Windows.Forms.ToolStripMenuItem tsmiCustomUploaders;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportAll;
        private HelpersLib.MenuButton mbHelp;
        private System.Windows.Forms.DataGridView dgvParameters;
        private System.Windows.Forms.DataGridView dgvHeaders;
        private System.Windows.Forms.DataGridView dgvArguments;
        private System.Windows.Forms.DataGridViewTextBoxColumn cHeadersName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cHeadersValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn cParametersName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cParametersValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn cArgumentsName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cArgumentsValue;
        private System.Windows.Forms.DataGridView dgvRegex;
        private System.Windows.Forms.DataGridViewTextBoxColumn cRegex;
        private System.Windows.Forms.Label lblRegex;
        private System.Windows.Forms.Panel pResponseText;
        private System.Windows.Forms.RichTextBox rtbResponseText;
        private System.Windows.Forms.TabPage tpResponseInfo;
        private System.Windows.Forms.Panel pResponseInfo;
        private System.Windows.Forms.RichTextBox rtbResponseInfo;
        private System.Windows.Forms.ToolStripMenuItem tsmiUpdateFolder;
        private System.Windows.Forms.Panel pResultErrorMessage;
        private System.Windows.Forms.RichTextBox rtbResultErrorMessage;
        private System.Windows.Forms.Label lblResultErrorMessage;
    }
}