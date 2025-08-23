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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomUploaderSettingsForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            cbImageUploader = new System.Windows.Forms.ComboBox();
            btnTextUploaderTest = new System.Windows.Forms.Button();
            lblURLShortener = new System.Windows.Forms.Label();
            cbTextUploader = new System.Windows.Forms.ComboBox();
            btnURLShortenerTest = new System.Windows.Forms.Button();
            lblTextUploader = new System.Windows.Forms.Label();
            mbHelp = new ShareX.HelpersLib.MenuButton();
            cmsHelp = new System.Windows.Forms.ContextMenuStrip(components);
            tsmiCustomUploaderGuide = new System.Windows.Forms.ToolStripMenuItem();
            tsmiClearUploaders = new System.Windows.Forms.ToolStripMenuItem();
            tsmiExportAll = new System.Windows.Forms.ToolStripMenuItem();
            tsmiUpdateFolder = new System.Windows.Forms.ToolStripMenuItem();
            btnNew = new System.Windows.Forms.Button();
            btnDuplicate = new System.Windows.Forms.Button();
            eiCustomUploaders = new ShareX.HelpersLib.ExportImportControl();
            lbCustomUploaderList = new System.Windows.Forms.ListBox();
            btnRemove = new System.Windows.Forms.Button();
            cbURLShortener = new System.Windows.Forms.ComboBox();
            cbFileUploader = new System.Windows.Forms.ComboBox();
            btnImageUploaderTest = new System.Windows.Forms.Button();
            lblFileUploader = new System.Windows.Forms.Label();
            btnFileUploaderTest = new System.Windows.Forms.Button();
            lblImageUploader = new System.Windows.Forms.Label();
            dgvHeaders = new System.Windows.Forms.DataGridView();
            cHeadersName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            cHeadersValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgvParameters = new System.Windows.Forms.DataGridView();
            cParametersName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            cParametersValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            lblHeaders = new System.Windows.Forms.Label();
            lblParameters = new System.Windows.Forms.Label();
            pRequestURL = new System.Windows.Forms.Panel();
            rtbRequestURL = new System.Windows.Forms.RichTextBox();
            cbRequestMethod = new System.Windows.Forms.ComboBox();
            lblRequestURL = new System.Windows.Forms.Label();
            cbBody = new System.Windows.Forms.ComboBox();
            lblRequestMethod = new System.Windows.Forms.Label();
            lblBody = new System.Windows.Forms.Label();
            pBodyArguments = new System.Windows.Forms.Panel();
            dgvArguments = new System.Windows.Forms.DataGridView();
            cArgumentsName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            cArgumentsValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            lblFileFormName = new System.Windows.Forms.Label();
            txtFileFormName = new System.Windows.Forms.TextBox();
            pBodyData = new System.Windows.Forms.Panel();
            btnDataBeautify = new System.Windows.Forms.Button();
            btnDataMinify = new System.Windows.Forms.Button();
            pData = new System.Windows.Forms.Panel();
            rtbData = new System.Windows.Forms.RichTextBox();
            pResultErrorMessage = new System.Windows.Forms.Panel();
            rtbResultErrorMessage = new System.Windows.Forms.RichTextBox();
            lblResultErrorMessage = new System.Windows.Forms.Label();
            pResultDeletionURL = new System.Windows.Forms.Panel();
            rtbResultDeletionURL = new System.Windows.Forms.RichTextBox();
            lblResultDeletionURL = new System.Windows.Forms.Label();
            pResultThumbnailURL = new System.Windows.Forms.Panel();
            rtbResultThumbnailURL = new System.Windows.Forms.RichTextBox();
            pResultURL = new System.Windows.Forms.Panel();
            rtbResultURL = new System.Windows.Forms.RichTextBox();
            lblResultThumbnailURL = new System.Windows.Forms.Label();
            lblResultURL = new System.Windows.Forms.Label();
            lblDestinationType = new System.Windows.Forms.Label();
            lblName = new System.Windows.Forms.Label();
            mbDestinationType = new ShareX.HelpersLib.MenuButton();
            cmsDestinationType = new System.Windows.Forms.ContextMenuStrip(components);
            txtName = new System.Windows.Forms.TextBox();
            lblURLSharingService = new System.Windows.Forms.Label();
            cbURLSharingService = new System.Windows.Forms.ComboBox();
            btnURLSharingServiceTest = new System.Windows.Forms.Button();
            ttHelpTip = new System.Windows.Forms.ToolTip(components);
            lblUploaders = new System.Windows.Forms.Label();
            pMain = new System.Windows.Forms.Panel();
            btnTestURLSyntax = new System.Windows.Forms.Button();
            cmsHelp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHeaders).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvParameters).BeginInit();
            pRequestURL.SuspendLayout();
            pBodyArguments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvArguments).BeginInit();
            pBodyData.SuspendLayout();
            pData.SuspendLayout();
            pResultErrorMessage.SuspendLayout();
            pResultDeletionURL.SuspendLayout();
            pResultThumbnailURL.SuspendLayout();
            pResultURL.SuspendLayout();
            pMain.SuspendLayout();
            SuspendLayout();
            // 
            // cbImageUploader
            // 
            cbImageUploader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbImageUploader.FormattingEnabled = true;
            resources.ApplyResources(cbImageUploader, "cbImageUploader");
            cbImageUploader.Name = "cbImageUploader";
            cbImageUploader.SelectedIndexChanged += cbCustomUploaderImageUploader_SelectedIndexChanged;
            // 
            // btnTextUploaderTest
            // 
            resources.ApplyResources(btnTextUploaderTest, "btnTextUploaderTest");
            btnTextUploaderTest.Name = "btnTextUploaderTest";
            btnTextUploaderTest.UseVisualStyleBackColor = true;
            btnTextUploaderTest.Click += btnCustomUploaderTextUploaderTest_Click;
            // 
            // lblURLShortener
            // 
            resources.ApplyResources(lblURLShortener, "lblURLShortener");
            lblURLShortener.Name = "lblURLShortener";
            // 
            // cbTextUploader
            // 
            cbTextUploader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbTextUploader.FormattingEnabled = true;
            resources.ApplyResources(cbTextUploader, "cbTextUploader");
            cbTextUploader.Name = "cbTextUploader";
            cbTextUploader.SelectedIndexChanged += cbCustomUploaderTextUploader_SelectedIndexChanged;
            // 
            // btnURLShortenerTest
            // 
            resources.ApplyResources(btnURLShortenerTest, "btnURLShortenerTest");
            btnURLShortenerTest.Name = "btnURLShortenerTest";
            btnURLShortenerTest.UseVisualStyleBackColor = true;
            btnURLShortenerTest.Click += btnCustomUploaderURLShortenerTest_Click;
            // 
            // lblTextUploader
            // 
            resources.ApplyResources(lblTextUploader, "lblTextUploader");
            lblTextUploader.Name = "lblTextUploader";
            // 
            // mbHelp
            // 
            resources.ApplyResources(mbHelp, "mbHelp");
            mbHelp.Menu = cmsHelp;
            mbHelp.Name = "mbHelp";
            mbHelp.UseVisualStyleBackColor = true;
            // 
            // cmsHelp
            // 
            cmsHelp.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tsmiCustomUploaderGuide, tsmiClearUploaders, tsmiExportAll, tsmiUpdateFolder });
            cmsHelp.Name = "cmsCustomUploaderHelp";
            cmsHelp.ShowImageMargin = false;
            resources.ApplyResources(cmsHelp, "cmsHelp");
            // 
            // tsmiCustomUploaderGuide
            // 
            tsmiCustomUploaderGuide.Name = "tsmiCustomUploaderGuide";
            resources.ApplyResources(tsmiCustomUploaderGuide, "tsmiCustomUploaderGuide");
            tsmiCustomUploaderGuide.Click += tsmiCustomUploaderGuide_Click;
            // 
            // tsmiClearUploaders
            // 
            tsmiClearUploaders.Name = "tsmiClearUploaders";
            resources.ApplyResources(tsmiClearUploaders, "tsmiClearUploaders");
            tsmiClearUploaders.Click += tsmiClearUploaders_Click;
            // 
            // tsmiExportAll
            // 
            tsmiExportAll.Name = "tsmiExportAll";
            resources.ApplyResources(tsmiExportAll, "tsmiExportAll");
            tsmiExportAll.Click += tsmiCustomUploaderExportAll_Click;
            // 
            // tsmiUpdateFolder
            // 
            tsmiUpdateFolder.Name = "tsmiUpdateFolder";
            resources.ApplyResources(tsmiUpdateFolder, "tsmiUpdateFolder");
            tsmiUpdateFolder.Click += tsmiUpdateFolder_Click;
            // 
            // btnNew
            // 
            resources.ApplyResources(btnNew, "btnNew");
            btnNew.Name = "btnNew";
            btnNew.UseVisualStyleBackColor = true;
            btnNew.Click += btnCustomUploaderNew_Click;
            // 
            // btnDuplicate
            // 
            resources.ApplyResources(btnDuplicate, "btnDuplicate");
            btnDuplicate.Name = "btnDuplicate";
            btnDuplicate.UseVisualStyleBackColor = true;
            btnDuplicate.Click += btnCustomUploaderDuplicate_Click;
            // 
            // eiCustomUploaders
            // 
            eiCustomUploaders.CustomFilter = "ShareX custom uploader (*.sxcu)|*.sxcu";
            eiCustomUploaders.DefaultFileName = null;
            eiCustomUploaders.ExportIgnoreDefaultValue = true;
            eiCustomUploaders.ExportIgnoreNull = true;
            resources.ApplyResources(eiCustomUploaders, "eiCustomUploaders");
            eiCustomUploaders.Name = "eiCustomUploaders";
            eiCustomUploaders.ObjectType = null;
            eiCustomUploaders.SerializationBinder = null;
            eiCustomUploaders.ExportRequested += eiCustomUploaders_ExportRequested;
            eiCustomUploaders.ImportRequested += eiCustomUploaders_ImportRequested;
            eiCustomUploaders.ImportCompleted += eiCustomUploaders_ImportCompleted;
            // 
            // lbCustomUploaderList
            // 
            lbCustomUploaderList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lbCustomUploaderList.FormattingEnabled = true;
            resources.ApplyResources(lbCustomUploaderList, "lbCustomUploaderList");
            lbCustomUploaderList.Name = "lbCustomUploaderList";
            lbCustomUploaderList.SelectedIndexChanged += lbCustomUploaderList_SelectedIndexChanged;
            // 
            // btnRemove
            // 
            resources.ApplyResources(btnRemove, "btnRemove");
            btnRemove.Name = "btnRemove";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += btnCustomUploaderRemove_Click;
            // 
            // cbURLShortener
            // 
            cbURLShortener.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbURLShortener.FormattingEnabled = true;
            resources.ApplyResources(cbURLShortener, "cbURLShortener");
            cbURLShortener.Name = "cbURLShortener";
            cbURLShortener.SelectedIndexChanged += cbCustomUploaderURLShortener_SelectedIndexChanged;
            // 
            // cbFileUploader
            // 
            cbFileUploader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbFileUploader.FormattingEnabled = true;
            resources.ApplyResources(cbFileUploader, "cbFileUploader");
            cbFileUploader.Name = "cbFileUploader";
            cbFileUploader.SelectedIndexChanged += cbCustomUploaderFileUploader_SelectedIndexChanged;
            // 
            // btnImageUploaderTest
            // 
            resources.ApplyResources(btnImageUploaderTest, "btnImageUploaderTest");
            btnImageUploaderTest.Name = "btnImageUploaderTest";
            btnImageUploaderTest.UseVisualStyleBackColor = true;
            btnImageUploaderTest.Click += btnCustomUploaderImageUploaderTest_Click;
            // 
            // lblFileUploader
            // 
            resources.ApplyResources(lblFileUploader, "lblFileUploader");
            lblFileUploader.Name = "lblFileUploader";
            // 
            // btnFileUploaderTest
            // 
            resources.ApplyResources(btnFileUploaderTest, "btnFileUploaderTest");
            btnFileUploaderTest.Name = "btnFileUploaderTest";
            btnFileUploaderTest.UseVisualStyleBackColor = true;
            btnFileUploaderTest.Click += btnCustomUploaderFileUploaderTest_Click;
            // 
            // lblImageUploader
            // 
            resources.ApplyResources(lblImageUploader, "lblImageUploader");
            lblImageUploader.Name = "lblImageUploader";
            // 
            // dgvHeaders
            // 
            dgvHeaders.AllowUserToResizeRows = false;
            dgvHeaders.BackgroundColor = System.Drawing.SystemColors.Window;
            dgvHeaders.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dgvHeaders.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dgvHeaders.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvHeaders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHeaders.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { cHeadersName, cHeadersValue });
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            dgvHeaders.DefaultCellStyle = dataGridViewCellStyle2;
            dgvHeaders.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            dgvHeaders.GridColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(dgvHeaders, "dgvHeaders");
            dgvHeaders.MultiSelect = false;
            dgvHeaders.Name = "dgvHeaders";
            dgvHeaders.RowHeadersVisible = false;
            dgvHeaders.CellValueChanged += dgvHeaders_CellValueChanged;
            dgvHeaders.EditingControlShowing += dgv_EditingControlShowing;
            // 
            // cHeadersName
            // 
            resources.ApplyResources(cHeadersName, "cHeadersName");
            cHeadersName.Name = "cHeadersName";
            cHeadersName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cHeadersValue
            // 
            cHeadersValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(cHeadersValue, "cHeadersValue");
            cHeadersValue.Name = "cHeadersValue";
            cHeadersValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvParameters
            // 
            dgvParameters.AllowUserToResizeRows = false;
            dgvParameters.BackgroundColor = System.Drawing.SystemColors.Window;
            dgvParameters.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dgvParameters.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dgvParameters.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dgvParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvParameters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { cParametersName, cParametersValue });
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            dgvParameters.DefaultCellStyle = dataGridViewCellStyle4;
            dgvParameters.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            dgvParameters.GridColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(dgvParameters, "dgvParameters");
            dgvParameters.MultiSelect = false;
            dgvParameters.Name = "dgvParameters";
            dgvParameters.RowHeadersVisible = false;
            dgvParameters.CellValueChanged += dgvParameters_CellValueChanged;
            dgvParameters.EditingControlShowing += dgv_EditingControlShowing;
            // 
            // cParametersName
            // 
            resources.ApplyResources(cParametersName, "cParametersName");
            cParametersName.Name = "cParametersName";
            cParametersName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cParametersValue
            // 
            cParametersValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(cParametersValue, "cParametersValue");
            cParametersValue.Name = "cParametersValue";
            cParametersValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // lblHeaders
            // 
            resources.ApplyResources(lblHeaders, "lblHeaders");
            lblHeaders.Name = "lblHeaders";
            // 
            // lblParameters
            // 
            resources.ApplyResources(lblParameters, "lblParameters");
            lblParameters.Name = "lblParameters";
            // 
            // pRequestURL
            // 
            pRequestURL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pRequestURL.Controls.Add(rtbRequestURL);
            resources.ApplyResources(pRequestURL, "pRequestURL");
            pRequestURL.Name = "pRequestURL";
            // 
            // rtbRequestURL
            // 
            rtbRequestURL.BorderStyle = System.Windows.Forms.BorderStyle.None;
            rtbRequestURL.DetectUrls = false;
            resources.ApplyResources(rtbRequestURL, "rtbRequestURL");
            rtbRequestURL.Name = "rtbRequestURL";
            rtbRequestURL.TextChanged += rtbCustomUploaderRequestURL_TextChanged;
            // 
            // cbRequestMethod
            // 
            cbRequestMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbRequestMethod.FormattingEnabled = true;
            resources.ApplyResources(cbRequestMethod, "cbRequestMethod");
            cbRequestMethod.Name = "cbRequestMethod";
            cbRequestMethod.SelectedIndexChanged += cbCustomUploaderRequestType_SelectedIndexChanged;
            // 
            // lblRequestURL
            // 
            resources.ApplyResources(lblRequestURL, "lblRequestURL");
            lblRequestURL.Name = "lblRequestURL";
            // 
            // cbBody
            // 
            cbBody.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbBody.DropDownWidth = 280;
            cbBody.FormattingEnabled = true;
            resources.ApplyResources(cbBody, "cbBody");
            cbBody.Name = "cbBody";
            cbBody.SelectedIndexChanged += cbCustomUploaderRequestFormat_SelectedIndexChanged;
            // 
            // lblRequestMethod
            // 
            resources.ApplyResources(lblRequestMethod, "lblRequestMethod");
            lblRequestMethod.Name = "lblRequestMethod";
            // 
            // lblBody
            // 
            resources.ApplyResources(lblBody, "lblBody");
            lblBody.Name = "lblBody";
            // 
            // pBodyArguments
            // 
            pBodyArguments.Controls.Add(dgvArguments);
            pBodyArguments.Controls.Add(lblFileFormName);
            pBodyArguments.Controls.Add(txtFileFormName);
            resources.ApplyResources(pBodyArguments, "pBodyArguments");
            pBodyArguments.Name = "pBodyArguments";
            // 
            // dgvArguments
            // 
            dgvArguments.AllowUserToResizeRows = false;
            dgvArguments.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dgvArguments.BackgroundColor = System.Drawing.SystemColors.Window;
            dgvArguments.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dgvArguments.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dgvArguments.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            dgvArguments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvArguments.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { cArgumentsName, cArgumentsValue });
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            dgvArguments.DefaultCellStyle = dataGridViewCellStyle7;
            dgvArguments.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            dgvArguments.GridColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(dgvArguments, "dgvArguments");
            dgvArguments.MultiSelect = false;
            dgvArguments.Name = "dgvArguments";
            dgvArguments.RowHeadersVisible = false;
            dgvArguments.CellValueChanged += dgvArguments_CellValueChanged;
            dgvArguments.EditingControlShowing += dgv_EditingControlShowing;
            // 
            // cArgumentsName
            // 
            resources.ApplyResources(cArgumentsName, "cArgumentsName");
            cArgumentsName.Name = "cArgumentsName";
            cArgumentsName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cArgumentsValue
            // 
            cArgumentsValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            cArgumentsValue.DefaultCellStyle = dataGridViewCellStyle6;
            resources.ApplyResources(cArgumentsValue, "cArgumentsValue");
            cArgumentsValue.Name = "cArgumentsValue";
            cArgumentsValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // lblFileFormName
            // 
            resources.ApplyResources(lblFileFormName, "lblFileFormName");
            lblFileFormName.Name = "lblFileFormName";
            // 
            // txtFileFormName
            // 
            resources.ApplyResources(txtFileFormName, "txtFileFormName");
            txtFileFormName.Name = "txtFileFormName";
            txtFileFormName.TextChanged += txtCustomUploaderFileForm_TextChanged;
            // 
            // pBodyData
            // 
            pBodyData.Controls.Add(btnDataBeautify);
            pBodyData.Controls.Add(btnDataMinify);
            pBodyData.Controls.Add(pData);
            resources.ApplyResources(pBodyData, "pBodyData");
            pBodyData.Name = "pBodyData";
            // 
            // btnDataBeautify
            // 
            resources.ApplyResources(btnDataBeautify, "btnDataBeautify");
            btnDataBeautify.Name = "btnDataBeautify";
            btnDataBeautify.UseVisualStyleBackColor = true;
            btnDataBeautify.Click += btnCustomUploaderDataBeautify_Click;
            // 
            // btnDataMinify
            // 
            resources.ApplyResources(btnDataMinify, "btnDataMinify");
            btnDataMinify.Name = "btnDataMinify";
            btnDataMinify.UseVisualStyleBackColor = true;
            btnDataMinify.Click += btnCustomUploaderDataMinify_Click;
            // 
            // pData
            // 
            pData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pData.Controls.Add(rtbData);
            resources.ApplyResources(pData, "pData");
            pData.Name = "pData";
            // 
            // rtbData
            // 
            rtbData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            rtbData.DetectUrls = false;
            resources.ApplyResources(rtbData, "rtbData");
            rtbData.Name = "rtbData";
            rtbData.TextChanged += rtbCustomUploaderData_TextChanged;
            // 
            // pResultErrorMessage
            // 
            pResultErrorMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pResultErrorMessage.Controls.Add(rtbResultErrorMessage);
            resources.ApplyResources(pResultErrorMessage, "pResultErrorMessage");
            pResultErrorMessage.Name = "pResultErrorMessage";
            // 
            // rtbResultErrorMessage
            // 
            rtbResultErrorMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            rtbResultErrorMessage.DetectUrls = false;
            resources.ApplyResources(rtbResultErrorMessage, "rtbResultErrorMessage");
            rtbResultErrorMessage.Name = "rtbResultErrorMessage";
            rtbResultErrorMessage.TextChanged += rtbResultErrorMessage_TextChanged;
            // 
            // lblResultErrorMessage
            // 
            resources.ApplyResources(lblResultErrorMessage, "lblResultErrorMessage");
            lblResultErrorMessage.Name = "lblResultErrorMessage";
            // 
            // pResultDeletionURL
            // 
            pResultDeletionURL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pResultDeletionURL.Controls.Add(rtbResultDeletionURL);
            resources.ApplyResources(pResultDeletionURL, "pResultDeletionURL");
            pResultDeletionURL.Name = "pResultDeletionURL";
            // 
            // rtbResultDeletionURL
            // 
            rtbResultDeletionURL.BorderStyle = System.Windows.Forms.BorderStyle.None;
            rtbResultDeletionURL.DetectUrls = false;
            resources.ApplyResources(rtbResultDeletionURL, "rtbResultDeletionURL");
            rtbResultDeletionURL.Name = "rtbResultDeletionURL";
            rtbResultDeletionURL.TextChanged += rtbCustomUploaderDeletionURL_TextChanged;
            // 
            // lblResultDeletionURL
            // 
            resources.ApplyResources(lblResultDeletionURL, "lblResultDeletionURL");
            lblResultDeletionURL.Name = "lblResultDeletionURL";
            // 
            // pResultThumbnailURL
            // 
            pResultThumbnailURL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pResultThumbnailURL.Controls.Add(rtbResultThumbnailURL);
            resources.ApplyResources(pResultThumbnailURL, "pResultThumbnailURL");
            pResultThumbnailURL.Name = "pResultThumbnailURL";
            // 
            // rtbResultThumbnailURL
            // 
            rtbResultThumbnailURL.BorderStyle = System.Windows.Forms.BorderStyle.None;
            rtbResultThumbnailURL.DetectUrls = false;
            resources.ApplyResources(rtbResultThumbnailURL, "rtbResultThumbnailURL");
            rtbResultThumbnailURL.Name = "rtbResultThumbnailURL";
            rtbResultThumbnailURL.TextChanged += rtbCustomUploaderThumbnailURL_TextChanged;
            // 
            // pResultURL
            // 
            pResultURL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pResultURL.Controls.Add(rtbResultURL);
            resources.ApplyResources(pResultURL, "pResultURL");
            pResultURL.Name = "pResultURL";
            // 
            // rtbResultURL
            // 
            rtbResultURL.BorderStyle = System.Windows.Forms.BorderStyle.None;
            rtbResultURL.DetectUrls = false;
            resources.ApplyResources(rtbResultURL, "rtbResultURL");
            rtbResultURL.Name = "rtbResultURL";
            rtbResultURL.TextChanged += rtbCustomUploaderURL_TextChanged;
            // 
            // lblResultThumbnailURL
            // 
            resources.ApplyResources(lblResultThumbnailURL, "lblResultThumbnailURL");
            lblResultThumbnailURL.Name = "lblResultThumbnailURL";
            // 
            // lblResultURL
            // 
            resources.ApplyResources(lblResultURL, "lblResultURL");
            lblResultURL.Name = "lblResultURL";
            // 
            // lblDestinationType
            // 
            resources.ApplyResources(lblDestinationType, "lblDestinationType");
            lblDestinationType.Name = "lblDestinationType";
            // 
            // lblName
            // 
            resources.ApplyResources(lblName, "lblName");
            lblName.Name = "lblName";
            // 
            // mbDestinationType
            // 
            resources.ApplyResources(mbDestinationType, "mbDestinationType");
            mbDestinationType.Menu = cmsDestinationType;
            mbDestinationType.Name = "mbDestinationType";
            ttHelpTip.SetToolTip(mbDestinationType, resources.GetString("mbDestinationType.ToolTip"));
            mbDestinationType.UseVisualStyleBackColor = true;
            // 
            // cmsDestinationType
            // 
            cmsDestinationType.ImageScalingSize = new System.Drawing.Size(24, 24);
            cmsDestinationType.Name = "cmsCustomUploaderDestinationType";
            cmsDestinationType.ShowCheckMargin = true;
            cmsDestinationType.ShowImageMargin = false;
            resources.ApplyResources(cmsDestinationType, "cmsDestinationType");
            // 
            // txtName
            // 
            resources.ApplyResources(txtName, "txtName");
            txtName.Name = "txtName";
            txtName.TextChanged += txtCustomUploaderName_TextChanged;
            // 
            // lblURLSharingService
            // 
            resources.ApplyResources(lblURLSharingService, "lblURLSharingService");
            lblURLSharingService.Name = "lblURLSharingService";
            // 
            // cbURLSharingService
            // 
            cbURLSharingService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbURLSharingService.FormattingEnabled = true;
            resources.ApplyResources(cbURLSharingService, "cbURLSharingService");
            cbURLSharingService.Name = "cbURLSharingService";
            cbURLSharingService.SelectedIndexChanged += cbCustomUploaderURLSharingService_SelectedIndexChanged;
            // 
            // btnURLSharingServiceTest
            // 
            resources.ApplyResources(btnURLSharingServiceTest, "btnURLSharingServiceTest");
            btnURLSharingServiceTest.Name = "btnURLSharingServiceTest";
            btnURLSharingServiceTest.UseVisualStyleBackColor = true;
            btnURLSharingServiceTest.Click += btnCustomUploaderURLSharingServiceTest_Click;
            // 
            // ttHelpTip
            // 
            ttHelpTip.AutomaticDelay = 0;
            ttHelpTip.AutoPopDelay = 30000;
            ttHelpTip.BackColor = System.Drawing.SystemColors.Window;
            ttHelpTip.InitialDelay = 500;
            ttHelpTip.ReshowDelay = 100;
            ttHelpTip.UseAnimation = false;
            ttHelpTip.UseFading = false;
            // 
            // lblUploaders
            // 
            resources.ApplyResources(lblUploaders, "lblUploaders");
            lblUploaders.Name = "lblUploaders";
            // 
            // pMain
            // 
            pMain.Controls.Add(btnTestURLSyntax);
            pMain.Controls.Add(pResultErrorMessage);
            pMain.Controls.Add(dgvHeaders);
            pMain.Controls.Add(lblResultErrorMessage);
            pMain.Controls.Add(txtName);
            pMain.Controls.Add(dgvParameters);
            pMain.Controls.Add(pResultDeletionURL);
            pMain.Controls.Add(lblName);
            pMain.Controls.Add(lblResultDeletionURL);
            pMain.Controls.Add(lblHeaders);
            pMain.Controls.Add(pResultThumbnailURL);
            pMain.Controls.Add(mbDestinationType);
            pMain.Controls.Add(pResultURL);
            pMain.Controls.Add(lblParameters);
            pMain.Controls.Add(lblResultThumbnailURL);
            pMain.Controls.Add(lblDestinationType);
            pMain.Controls.Add(lblResultURL);
            pMain.Controls.Add(pRequestURL);
            pMain.Controls.Add(cbRequestMethod);
            pMain.Controls.Add(lblRequestURL);
            pMain.Controls.Add(cbBody);
            pMain.Controls.Add(lblBody);
            pMain.Controls.Add(lblRequestMethod);
            pMain.Controls.Add(pBodyArguments);
            pMain.Controls.Add(pBodyData);
            resources.ApplyResources(pMain, "pMain");
            pMain.Name = "pMain";
            // 
            // btnTestURLSyntax
            // 
            resources.ApplyResources(btnTestURLSyntax, "btnTestURLSyntax");
            btnTestURLSyntax.Name = "btnTestURLSyntax";
            btnTestURLSyntax.UseVisualStyleBackColor = true;
            btnTestURLSyntax.Click += btnTestURLSyntax_Click;
            // 
            // CustomUploaderSettingsForm
            // 
            AllowDrop = true;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(pMain);
            Controls.Add(lblUploaders);
            Controls.Add(mbHelp);
            Controls.Add(btnNew);
            Controls.Add(btnDuplicate);
            Controls.Add(eiCustomUploaders);
            Controls.Add(lbCustomUploaderList);
            Controls.Add(btnRemove);
            Controls.Add(btnURLSharingServiceTest);
            Controls.Add(cbURLSharingService);
            Controls.Add(lblURLSharingService);
            Controls.Add(cbImageUploader);
            Controls.Add(btnTextUploaderTest);
            Controls.Add(lblImageUploader);
            Controls.Add(lblURLShortener);
            Controls.Add(btnFileUploaderTest);
            Controls.Add(cbTextUploader);
            Controls.Add(lblFileUploader);
            Controls.Add(btnURLShortenerTest);
            Controls.Add(btnImageUploaderTest);
            Controls.Add(lblTextUploader);
            Controls.Add(cbFileUploader);
            Controls.Add(cbURLShortener);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "CustomUploaderSettingsForm";
            DragDrop += CustomUploaderSettingsForm_DragDrop;
            DragEnter += CustomUploaderSettingsForm_DragEnter;
            cmsHelp.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvHeaders).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvParameters).EndInit();
            pRequestURL.ResumeLayout(false);
            pBodyArguments.ResumeLayout(false);
            pBodyArguments.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvArguments).EndInit();
            pBodyData.ResumeLayout(false);
            pData.ResumeLayout(false);
            pResultErrorMessage.ResumeLayout(false);
            pResultDeletionURL.ResumeLayout(false);
            pResultThumbnailURL.ResumeLayout(false);
            pResultURL.ResumeLayout(false);
            pMain.ResumeLayout(false);
            pMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbImageUploader;
        private System.Windows.Forms.Button btnTextUploaderTest;
        private System.Windows.Forms.Label lblURLShortener;
        private System.Windows.Forms.ComboBox cbTextUploader;
        private System.Windows.Forms.Button btnURLShortenerTest;
        private System.Windows.Forms.Label lblTextUploader;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnDuplicate;
        private HelpersLib.ExportImportControl eiCustomUploaders;
        private System.Windows.Forms.ListBox lbCustomUploaderList;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.ComboBox cbURLShortener;
        private System.Windows.Forms.ComboBox cbFileUploader;
        private System.Windows.Forms.Button btnImageUploaderTest;
        private System.Windows.Forms.Label lblFileUploader;
        private System.Windows.Forms.Button btnFileUploaderTest;
        private System.Windows.Forms.Label lblImageUploader;
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
        private System.Windows.Forms.Panel pResultDeletionURL;
        private System.Windows.Forms.RichTextBox rtbResultDeletionURL;
        private System.Windows.Forms.Label lblResultDeletionURL;
        private System.Windows.Forms.Panel pResultThumbnailURL;
        private System.Windows.Forms.RichTextBox rtbResultThumbnailURL;
        private System.Windows.Forms.Panel pResultURL;
        private System.Windows.Forms.RichTextBox rtbResultURL;
        private System.Windows.Forms.Label lblResultThumbnailURL;
        private System.Windows.Forms.Label lblResultURL;
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
        private System.Windows.Forms.ToolStripMenuItem tsmiExportAll;
        private HelpersLib.MenuButton mbHelp;
        private System.Windows.Forms.DataGridView dgvParameters;
        private System.Windows.Forms.DataGridView dgvHeaders;
        private System.Windows.Forms.DataGridView dgvArguments;
        private System.Windows.Forms.ToolStripMenuItem tsmiUpdateFolder;
        private System.Windows.Forms.Panel pResultErrorMessage;
        private System.Windows.Forms.RichTextBox rtbResultErrorMessage;
        private System.Windows.Forms.Label lblResultErrorMessage;
        private System.Windows.Forms.Label lblUploaders;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearUploaders;
        private System.Windows.Forms.Panel pMain;
        private System.Windows.Forms.Button btnTestURLSyntax;
        private System.Windows.Forms.DataGridViewTextBoxColumn cHeadersName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cHeadersValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn cParametersName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cParametersValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn cArgumentsName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cArgumentsValue;
    }
}