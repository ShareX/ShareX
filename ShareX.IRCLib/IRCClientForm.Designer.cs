namespace ShareX.IRCLib
{
    partial class IRCClientForm
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
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.cmsMessage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiMessageBold = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMessageItalic = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMessageUnderline = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMessageNormal = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColors = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorWhite = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorBlack = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorBlue = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorGreen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorLightRed = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorBrown = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorPurple = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorOrange = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorYellow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorLightGreen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorCyan = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorLightCyan = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorLightBlue = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorPink = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorGrey = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorLightGrey = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMessageSend = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpMain = new System.Windows.Forms.TabPage();
            this.btnConnect = new System.Windows.Forms.Button();
            this.pgSettings = new System.Windows.Forms.PropertyGrid();
            this.tpOutput = new System.Windows.Forms.TabPage();
            this.txtCommand = new System.Windows.Forms.TextBox();
            this.lblCommand = new System.Windows.Forms.Label();
            this.btnCommandSend = new System.Windows.Forms.Button();
            this.tpMessages = new System.Windows.Forms.TabPage();
            this.btnMessagesMenu = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lblChannel = new System.Windows.Forms.Label();
            this.txtMessages = new System.Windows.Forms.TextBox();
            this.txtChannel = new System.Windows.Forms.TextBox();
            this.cmsMessage.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tpMain.SuspendLayout();
            this.tpOutput.SuspendLayout();
            this.tpMessages.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtMessage
            // 
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.Location = new System.Drawing.Point(248, 604);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(600, 20);
            this.txtMessage.TabIndex = 0;
            this.txtMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMessage_KeyDown);
            // 
            // cmsMessage
            // 
            this.cmsMessage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiMessageBold,
            this.tsmiMessageItalic,
            this.tsmiMessageUnderline,
            this.tsmiMessageNormal,
            this.tsmiColors});
            this.cmsMessage.Name = "cmsMessage";
            this.cmsMessage.ShowImageMargin = false;
            this.cmsMessage.Size = new System.Drawing.Size(101, 114);
            // 
            // tsmiMessageBold
            // 
            this.tsmiMessageBold.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tsmiMessageBold.Name = "tsmiMessageBold";
            this.tsmiMessageBold.Size = new System.Drawing.Size(100, 22);
            this.tsmiMessageBold.Text = "Bold";
            this.tsmiMessageBold.Click += new System.EventHandler(this.tsmiMessageBold_Click);
            // 
            // tsmiMessageItalic
            // 
            this.tsmiMessageItalic.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tsmiMessageItalic.Name = "tsmiMessageItalic";
            this.tsmiMessageItalic.Size = new System.Drawing.Size(100, 22);
            this.tsmiMessageItalic.Text = "Italic";
            this.tsmiMessageItalic.Click += new System.EventHandler(this.tsmiMessageItalic_Click);
            // 
            // tsmiMessageUnderline
            // 
            this.tsmiMessageUnderline.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tsmiMessageUnderline.Name = "tsmiMessageUnderline";
            this.tsmiMessageUnderline.Size = new System.Drawing.Size(100, 22);
            this.tsmiMessageUnderline.Text = "Underline";
            this.tsmiMessageUnderline.Click += new System.EventHandler(this.tsmiMessageUnderline_Click);
            // 
            // tsmiMessageNormal
            // 
            this.tsmiMessageNormal.Name = "tsmiMessageNormal";
            this.tsmiMessageNormal.Size = new System.Drawing.Size(100, 22);
            this.tsmiMessageNormal.Text = "Normal";
            this.tsmiMessageNormal.Click += new System.EventHandler(this.tsmiMessageNormal_Click);
            // 
            // tsmiColors
            // 
            this.tsmiColors.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiColorWhite,
            this.tsmiColorBlack,
            this.tsmiColorBlue,
            this.tsmiColorGreen,
            this.tsmiColorLightRed,
            this.tsmiColorBrown,
            this.tsmiColorPurple,
            this.tsmiColorOrange,
            this.tsmiColorYellow,
            this.tsmiColorLightGreen,
            this.tsmiColorCyan,
            this.tsmiColorLightCyan,
            this.tsmiColorLightBlue,
            this.tsmiColorPink,
            this.tsmiColorGrey,
            this.tsmiColorLightGrey});
            this.tsmiColors.Name = "tsmiColors";
            this.tsmiColors.Size = new System.Drawing.Size(100, 22);
            this.tsmiColors.Text = "Colors";
            // 
            // tsmiColorWhite
            // 
            this.tsmiColorWhite.BackColor = System.Drawing.Color.White;
            this.tsmiColorWhite.ForeColor = System.Drawing.Color.Black;
            this.tsmiColorWhite.Name = "tsmiColorWhite";
            this.tsmiColorWhite.Size = new System.Drawing.Size(135, 22);
            this.tsmiColorWhite.Text = "White";
            this.tsmiColorWhite.Click += new System.EventHandler(this.tsmiColorWhite_Click);
            // 
            // tsmiColorBlack
            // 
            this.tsmiColorBlack.BackColor = System.Drawing.Color.Black;
            this.tsmiColorBlack.ForeColor = System.Drawing.Color.White;
            this.tsmiColorBlack.Name = "tsmiColorBlack";
            this.tsmiColorBlack.Size = new System.Drawing.Size(135, 22);
            this.tsmiColorBlack.Text = "Black";
            this.tsmiColorBlack.Click += new System.EventHandler(this.tsmiColorBlack_Click);
            // 
            // tsmiColorBlue
            // 
            this.tsmiColorBlue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(127)))));
            this.tsmiColorBlue.ForeColor = System.Drawing.Color.White;
            this.tsmiColorBlue.Name = "tsmiColorBlue";
            this.tsmiColorBlue.Size = new System.Drawing.Size(135, 22);
            this.tsmiColorBlue.Text = "Blue";
            this.tsmiColorBlue.Click += new System.EventHandler(this.tsmiColorBlue_Click);
            // 
            // tsmiColorGreen
            // 
            this.tsmiColorGreen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(147)))), ((int)(((byte)(0)))));
            this.tsmiColorGreen.ForeColor = System.Drawing.Color.White;
            this.tsmiColorGreen.Name = "tsmiColorGreen";
            this.tsmiColorGreen.Size = new System.Drawing.Size(135, 22);
            this.tsmiColorGreen.Text = "Green";
            this.tsmiColorGreen.Click += new System.EventHandler(this.tsmiColorGreen_Click);
            // 
            // tsmiColorLightRed
            // 
            this.tsmiColorLightRed.BackColor = System.Drawing.Color.Red;
            this.tsmiColorLightRed.ForeColor = System.Drawing.Color.White;
            this.tsmiColorLightRed.Name = "tsmiColorLightRed";
            this.tsmiColorLightRed.Size = new System.Drawing.Size(135, 22);
            this.tsmiColorLightRed.Text = "Light Red";
            this.tsmiColorLightRed.Click += new System.EventHandler(this.tsmiColorLightRed_Click);
            // 
            // tsmiColorBrown
            // 
            this.tsmiColorBrown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.tsmiColorBrown.ForeColor = System.Drawing.Color.White;
            this.tsmiColorBrown.Name = "tsmiColorBrown";
            this.tsmiColorBrown.Size = new System.Drawing.Size(135, 22);
            this.tsmiColorBrown.Text = "Brown";
            this.tsmiColorBrown.Click += new System.EventHandler(this.tsmiColorBrown_Click);
            // 
            // tsmiColorPurple
            // 
            this.tsmiColorPurple.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(0)))), ((int)(((byte)(156)))));
            this.tsmiColorPurple.ForeColor = System.Drawing.Color.White;
            this.tsmiColorPurple.Name = "tsmiColorPurple";
            this.tsmiColorPurple.Size = new System.Drawing.Size(135, 22);
            this.tsmiColorPurple.Text = "Purple";
            this.tsmiColorPurple.Click += new System.EventHandler(this.tsmiColorPurple_Click);
            // 
            // tsmiColorOrange
            // 
            this.tsmiColorOrange.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(127)))), ((int)(((byte)(0)))));
            this.tsmiColorOrange.ForeColor = System.Drawing.Color.White;
            this.tsmiColorOrange.Name = "tsmiColorOrange";
            this.tsmiColorOrange.Size = new System.Drawing.Size(135, 22);
            this.tsmiColorOrange.Text = "Orange";
            this.tsmiColorOrange.Click += new System.EventHandler(this.tsmiColorOrange_Click);
            // 
            // tsmiColorYellow
            // 
            this.tsmiColorYellow.BackColor = System.Drawing.Color.Yellow;
            this.tsmiColorYellow.ForeColor = System.Drawing.Color.Black;
            this.tsmiColorYellow.Name = "tsmiColorYellow";
            this.tsmiColorYellow.Size = new System.Drawing.Size(135, 22);
            this.tsmiColorYellow.Text = "Yellow";
            this.tsmiColorYellow.Click += new System.EventHandler(this.tsmiColorYellow_Click);
            // 
            // tsmiColorLightGreen
            // 
            this.tsmiColorLightGreen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(252)))), ((int)(((byte)(0)))));
            this.tsmiColorLightGreen.ForeColor = System.Drawing.Color.Black;
            this.tsmiColorLightGreen.Name = "tsmiColorLightGreen";
            this.tsmiColorLightGreen.Size = new System.Drawing.Size(135, 22);
            this.tsmiColorLightGreen.Text = "Light Green";
            this.tsmiColorLightGreen.Click += new System.EventHandler(this.tsmiColorLightGreen_Click);
            // 
            // tsmiColorCyan
            // 
            this.tsmiColorCyan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(147)))), ((int)(((byte)(147)))));
            this.tsmiColorCyan.ForeColor = System.Drawing.Color.White;
            this.tsmiColorCyan.Name = "tsmiColorCyan";
            this.tsmiColorCyan.Size = new System.Drawing.Size(135, 22);
            this.tsmiColorCyan.Text = "Cyan";
            this.tsmiColorCyan.Click += new System.EventHandler(this.tsmiColorCyan_Click);
            // 
            // tsmiColorLightCyan
            // 
            this.tsmiColorLightCyan.BackColor = System.Drawing.Color.Aqua;
            this.tsmiColorLightCyan.ForeColor = System.Drawing.Color.Black;
            this.tsmiColorLightCyan.Name = "tsmiColorLightCyan";
            this.tsmiColorLightCyan.Size = new System.Drawing.Size(135, 22);
            this.tsmiColorLightCyan.Text = "Light Cyan";
            this.tsmiColorLightCyan.Click += new System.EventHandler(this.tsmiColorLightCyan_Click);
            // 
            // tsmiColorLightBlue
            // 
            this.tsmiColorLightBlue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(252)))));
            this.tsmiColorLightBlue.ForeColor = System.Drawing.Color.White;
            this.tsmiColorLightBlue.Name = "tsmiColorLightBlue";
            this.tsmiColorLightBlue.Size = new System.Drawing.Size(135, 22);
            this.tsmiColorLightBlue.Text = "Light Blue";
            this.tsmiColorLightBlue.Click += new System.EventHandler(this.tsmiColorLightBlue_Click);
            // 
            // tsmiColorPink
            // 
            this.tsmiColorPink.BackColor = System.Drawing.Color.Fuchsia;
            this.tsmiColorPink.ForeColor = System.Drawing.Color.White;
            this.tsmiColorPink.Name = "tsmiColorPink";
            this.tsmiColorPink.Size = new System.Drawing.Size(135, 22);
            this.tsmiColorPink.Text = "Pink";
            this.tsmiColorPink.Click += new System.EventHandler(this.tsmiColorPink_Click);
            // 
            // tsmiColorGrey
            // 
            this.tsmiColorGrey.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.tsmiColorGrey.ForeColor = System.Drawing.Color.White;
            this.tsmiColorGrey.Name = "tsmiColorGrey";
            this.tsmiColorGrey.Size = new System.Drawing.Size(135, 22);
            this.tsmiColorGrey.Text = "Grey";
            this.tsmiColorGrey.Click += new System.EventHandler(this.tsmiColorGrey_Click);
            // 
            // tsmiColorLightGrey
            // 
            this.tsmiColorLightGrey.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.tsmiColorLightGrey.ForeColor = System.Drawing.Color.Black;
            this.tsmiColorLightGrey.Name = "tsmiColorLightGrey";
            this.tsmiColorLightGrey.Size = new System.Drawing.Size(135, 22);
            this.tsmiColorLightGrey.Text = "Light Grey";
            this.tsmiColorLightGrey.Click += new System.EventHandler(this.tsmiColorLightGrey_Click);
            // 
            // btnMessageSend
            // 
            this.btnMessageSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMessageSend.Enabled = false;
            this.btnMessageSend.Location = new System.Drawing.Point(888, 602);
            this.btnMessageSend.Name = "btnMessageSend";
            this.btnMessageSend.Size = new System.Drawing.Size(80, 24);
            this.btnMessageSend.TabIndex = 2;
            this.btnMessageSend.Text = "Send";
            this.btnMessageSend.UseVisualStyleBackColor = true;
            this.btnMessageSend.Click += new System.EventHandler(this.btnMessageSend_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.BackColor = System.Drawing.Color.White;
            this.txtOutput.Location = new System.Drawing.Point(8, 8);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutput.Size = new System.Drawing.Size(960, 584);
            this.txtOutput.TabIndex = 2;
            this.txtOutput.WordWrap = false;
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpMain);
            this.tcMain.Controls.Add(this.tpOutput);
            this.tcMain.Controls.Add(this.tpMessages);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 0);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(984, 661);
            this.tcMain.TabIndex = 0;
            // 
            // tpMain
            // 
            this.tpMain.Controls.Add(this.btnConnect);
            this.tpMain.Controls.Add(this.pgSettings);
            this.tpMain.Location = new System.Drawing.Point(4, 22);
            this.tpMain.Name = "tpMain";
            this.tpMain.Padding = new System.Windows.Forms.Padding(3);
            this.tpMain.Size = new System.Drawing.Size(976, 635);
            this.tpMain.TabIndex = 2;
            this.tpMain.Text = "Main";
            this.tpMain.UseVisualStyleBackColor = true;
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConnect.Location = new System.Drawing.Point(8, 600);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(112, 24);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // pgSettings
            // 
            this.pgSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgSettings.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.pgSettings.Location = new System.Drawing.Point(8, 8);
            this.pgSettings.Name = "pgSettings";
            this.pgSettings.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgSettings.Size = new System.Drawing.Size(960, 584);
            this.pgSettings.TabIndex = 1;
            this.pgSettings.ToolbarVisible = false;
            // 
            // tpOutput
            // 
            this.tpOutput.Controls.Add(this.txtCommand);
            this.tpOutput.Controls.Add(this.lblCommand);
            this.tpOutput.Controls.Add(this.btnCommandSend);
            this.tpOutput.Controls.Add(this.txtOutput);
            this.tpOutput.Location = new System.Drawing.Point(4, 22);
            this.tpOutput.Name = "tpOutput";
            this.tpOutput.Padding = new System.Windows.Forms.Padding(3);
            this.tpOutput.Size = new System.Drawing.Size(976, 635);
            this.tpOutput.TabIndex = 0;
            this.tpOutput.Text = "IRC output";
            this.tpOutput.UseVisualStyleBackColor = true;
            // 
            // txtCommand
            // 
            this.txtCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCommand.Location = new System.Drawing.Point(72, 604);
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.Size = new System.Drawing.Size(808, 20);
            this.txtCommand.TabIndex = 0;
            this.txtCommand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCommand_KeyDown);
            // 
            // lblCommand
            // 
            this.lblCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCommand.AutoSize = true;
            this.lblCommand.Location = new System.Drawing.Point(8, 608);
            this.lblCommand.Name = "lblCommand";
            this.lblCommand.Size = new System.Drawing.Size(57, 13);
            this.lblCommand.TabIndex = 2;
            this.lblCommand.Text = "Command:";
            // 
            // btnCommandSend
            // 
            this.btnCommandSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCommandSend.Enabled = false;
            this.btnCommandSend.Location = new System.Drawing.Point(888, 602);
            this.btnCommandSend.Name = "btnCommandSend";
            this.btnCommandSend.Size = new System.Drawing.Size(80, 24);
            this.btnCommandSend.TabIndex = 1;
            this.btnCommandSend.Text = "Send";
            this.btnCommandSend.UseVisualStyleBackColor = true;
            this.btnCommandSend.Click += new System.EventHandler(this.btnCommandSend_Click);
            // 
            // tpMessages
            // 
            this.tpMessages.Controls.Add(this.btnMessagesMenu);
            this.tpMessages.Controls.Add(this.lblMessage);
            this.tpMessages.Controls.Add(this.lblChannel);
            this.tpMessages.Controls.Add(this.txtMessages);
            this.tpMessages.Controls.Add(this.txtChannel);
            this.tpMessages.Controls.Add(this.txtMessage);
            this.tpMessages.Controls.Add(this.btnMessageSend);
            this.tpMessages.Location = new System.Drawing.Point(4, 22);
            this.tpMessages.Name = "tpMessages";
            this.tpMessages.Padding = new System.Windows.Forms.Padding(3);
            this.tpMessages.Size = new System.Drawing.Size(976, 635);
            this.tpMessages.TabIndex = 1;
            this.tpMessages.Text = "Messages";
            this.tpMessages.UseVisualStyleBackColor = true;
            // 
            // btnMessagesMenu
            // 
            this.btnMessagesMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMessagesMenu.Location = new System.Drawing.Point(856, 602);
            this.btnMessagesMenu.Name = "btnMessagesMenu";
            this.btnMessagesMenu.Size = new System.Drawing.Size(24, 24);
            this.btnMessagesMenu.TabIndex = 4;
            this.btnMessagesMenu.Text = "...";
            this.btnMessagesMenu.UseVisualStyleBackColor = true;
            this.btnMessagesMenu.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnMessagesMenu_MouseClick);
            // 
            // lblMessage
            // 
            this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(192, 608);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(53, 13);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Message:";
            // 
            // lblChannel
            // 
            this.lblChannel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblChannel.AutoSize = true;
            this.lblChannel.Location = new System.Drawing.Point(8, 608);
            this.lblChannel.Name = "lblChannel";
            this.lblChannel.Size = new System.Drawing.Size(76, 13);
            this.lblChannel.TabIndex = 2;
            this.lblChannel.Text = "Channel/Nick:";
            // 
            // txtMessages
            // 
            this.txtMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessages.BackColor = System.Drawing.Color.White;
            this.txtMessages.Location = new System.Drawing.Point(8, 8);
            this.txtMessages.Multiline = true;
            this.txtMessages.Name = "txtMessages";
            this.txtMessages.ReadOnly = true;
            this.txtMessages.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtMessages.Size = new System.Drawing.Size(960, 584);
            this.txtMessages.TabIndex = 3;
            this.txtMessages.WordWrap = false;
            // 
            // txtChannel
            // 
            this.txtChannel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtChannel.Location = new System.Drawing.Point(88, 604);
            this.txtChannel.Name = "txtChannel";
            this.txtChannel.Size = new System.Drawing.Size(96, 20);
            this.txtChannel.TabIndex = 1;
            // 
            // IRCClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.tcMain);
            this.Name = "IRCClientForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShareX - IRC client";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.cmsMessage.ResumeLayout(false);
            this.tcMain.ResumeLayout(false);
            this.tpMain.ResumeLayout(false);
            this.tpOutput.ResumeLayout(false);
            this.tpOutput.PerformLayout();
            this.tpMessages.ResumeLayout(false);
            this.tpMessages.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnMessageSend;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpOutput;
        private System.Windows.Forms.TabPage tpMessages;
        private System.Windows.Forms.TextBox txtMessages;
        private System.Windows.Forms.TextBox txtChannel;
        private System.Windows.Forms.Label lblChannel;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.TabPage tpMain;
        private System.Windows.Forms.PropertyGrid pgSettings;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtCommand;
        private System.Windows.Forms.Label lblCommand;
        private System.Windows.Forms.Button btnCommandSend;
        private System.Windows.Forms.ContextMenuStrip cmsMessage;
        private System.Windows.Forms.ToolStripMenuItem tsmiMessageBold;
        private System.Windows.Forms.ToolStripMenuItem tsmiMessageItalic;
        private System.Windows.Forms.ToolStripMenuItem tsmiMessageUnderline;
        private System.Windows.Forms.ToolStripMenuItem tsmiMessageNormal;
        private System.Windows.Forms.Button btnMessagesMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiColors;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorWhite;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorBlack;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorBlue;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorGreen;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorLightRed;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorBrown;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorPurple;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorOrange;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorYellow;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorLightGreen;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorCyan;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorLightCyan;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorLightBlue;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorPink;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorGrey;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorLightGrey;
    }
}

